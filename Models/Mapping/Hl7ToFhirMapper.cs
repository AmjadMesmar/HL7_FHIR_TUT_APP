using System.Globalization;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7FhirWorkbench.Models.Hl7V2;

namespace Hl7FhirWorkbench.Models.Mapping;

public static class Hl7ToFhirMapper
{
    private static readonly FhirJsonSerializer Serializer = new();

    public static TransformationResult MapMessage(Hl7Message message)
    {
        var result = new TransformationResult();
        var links = result.Mappings;
        var notes = result.ArchitecturalNotes;

        var pid = message.GetSegment("PID");
        var pv1 = message.GetSegment("PV1");
        var obr = message.GetSegment("OBR");
        var obxSegments = message.GetSegments("OBX").ToList();

        Patient? fhirPatient = null;
        if (pid != null)
        {
            fhirPatient = MapPatient(pid, links);
        }

        Encounter? fhirEncounter = null;
        if (pv1 != null)
        {
            fhirEncounter = MapEncounter(pv1, links, fhirPatient);
        }

        DiagnosticReport? fhirReport = null;
        List<Observation> observations = new();
        if (obr != null)
        {
            fhirReport = MapDiagnosticReport(obr, links, fhirPatient);
            int obxIdx = 1;
            foreach (var obx in obxSegments)
            {
                var obs = MapObservation(obx, links, fhirPatient, obxIdx++);
                observations.Add(obs);
                fhirReport.Result.Add(new ResourceReference($"urn:uuid:{obs.Id}"));
            }
        }

        // Determine output: Bundle or Single Resource
        if (fhirReport != null && observations.Count > 0)
        {
            var bundle = new Bundle
            {
                Id = Guid.NewGuid().ToString(),
                Type = Bundle.BundleType.Collection,
                Timestamp = DateTimeOffset.UtcNow
            };

            if (fhirPatient != null)
            {
                bundle.Entry.Add(new Bundle.EntryComponent { FullUrl = $"urn:uuid:{fhirPatient.Id}", Resource = fhirPatient });
            }

            bundle.Entry.Add(new Bundle.EntryComponent { FullUrl = $"urn:uuid:{fhirReport.Id}", Resource = fhirReport });
            foreach (var obs in observations)
            {
                bundle.Entry.Add(new Bundle.EntryComponent { FullUrl = $"urn:uuid:{obs.Id}", Resource = obs });
            }

            result.FhirResource = bundle;
            notes.Add("A FHIR Bundle (Collection) was generated to package the Patient, DiagnosticReport, and its constituent Observations together.");
            notes.Add("In HL7 v2, OBR and OBX are implicitly grouped by position in the ER7 stream. In FHIR, grouping is explicit via 'DiagnosticReport.result' ResourceReferences.");
        }
        else if (fhirPatient != null && fhirEncounter != null)
        {
            var bundle = new Bundle
            {
                Id = Guid.NewGuid().ToString(),
                Type = Bundle.BundleType.Collection,
                Timestamp = DateTimeOffset.UtcNow
            };
            bundle.Entry.Add(new Bundle.EntryComponent { FullUrl = $"urn:uuid:{fhirPatient.Id}", Resource = fhirPatient });
            bundle.Entry.Add(new Bundle.EntryComponent { FullUrl = $"urn:uuid:{fhirEncounter.Id}", Resource = fhirEncounter });

            result.FhirResource = bundle;
            notes.Add("Admission/Encounter messages combine demographic identity (PID) and clinical context (PV1). In FHIR, these are distinct resources linked via 'Encounter.subject'.");
        }
        else if (fhirPatient != null)
        {
            result.FhirResource = fhirPatient;
            notes.Add("A standalone FHIR 'Patient' resource was generated from the PID demographic segment.");
        }
        else
        {
            // Fallback empty bundle
            result.FhirResource = new Bundle
            {
                Type = Bundle.BundleType.Collection
            };
            notes.Add("No recognized clinical segments (PID, PV1, OBR/OBX) were found in the provided HL7 v2 message.");
        }

        var rawJson = Serializer.SerializeToString(result.FhirResource);
        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(rawJson);
            result.FhirJson = System.Text.Json.JsonSerializer.Serialize(doc.RootElement, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        }
        catch
        {
            result.FhirJson = rawJson;
        }
        return result;
    }

    private static Patient MapPatient(Hl7Segment pid, List<FieldMappingLink> links)
    {
        var patient = new Patient
        {
            Id = Guid.NewGuid().ToString(),
            Active = true
        };

        // PID-3 Patient Identifier List
        var mrnField = pid.GetField(3);
        if (mrnField != null && !string.IsNullOrWhiteSpace(mrnField.Value))
        {
            string mrnVal = pid.GetValue(3, 1);
            string authority = pid.GetValue(3, 4);
            var identifier = new Identifier
            {
                System = string.IsNullOrWhiteSpace(authority) ? "http://hospital.example.org/mrn" : $"http://{authority.ToLowerInvariant()}.org/mrn",
                Value = mrnVal,
                Type = new CodeableConcept("http://terminology.hl7.org/CodeSystem/v2-0203", "MR", "Medical Record Number")
            };
            patient.Identifier.Add(identifier);

            links.Add(new FieldMappingLink(
                SourceField: "PID-3.1",
                SourceLabel: "Patient Identifier (MRN)",
                SourceValue: mrnVal,
                TargetFhirPath: "Patient.identifier[0].value",
                TargetLabel: "Identifier.value",
                TargetValue: mrnVal,
                ConceptExplanation: "In HL7 v2, PID-3 is a composite field (CX). Component 1 is ID number, Component 4 is Assigning Authority.",
                InteropGotcha: "HL7 v2 uses local strings (e.g. 'HOSPITAL_A'); FHIR requires a globally unique URI for 'system' to prevent cross-institution ID collisions.",
                ConceptExplanationAr: "في معيار HL7 v2، الحقل PID-3 مركب (CX)؛ المكون الأول يمثل رقم الملف والمكون الرابع جهة الإصدار.",
                InteropGotchaAr: "معيار HL7 v2 يكتفي بنصوص محلية، بينما يشترط FHIR معرف URI عالمي فريد لمنع تضارب أرقام الملفات بين المنشآت.",
                SourceLabelAr: "معرف المريض (رقم الملف MRN)",
                TargetLabelAr: "قيمة المعرف (Identifier.value)"
            ));
        }

        // PID-5 Patient Name
        var nameField = pid.GetField(5);
        if (nameField != null && !string.IsNullOrWhiteSpace(nameField.Value))
        {
            string family = pid.GetValue(5, 1);
            string given = pid.GetValue(5, 2);
            string middle = pid.GetValue(5, 3);

            var humanName = new HumanName
            {
                Use = HumanName.NameUse.Official,
                Family = family
            };
            if (!string.IsNullOrWhiteSpace(given)) humanName.GivenElement.Add(new FhirString(given));
            if (!string.IsNullOrWhiteSpace(middle)) humanName.GivenElement.Add(new FhirString(middle));
            patient.Name.Add(humanName);

            links.Add(new FieldMappingLink(
                SourceField: "PID-5 (5.1 & 5.2)",
                SourceLabel: "Patient Name (Family^Given)",
                SourceValue: $"{family}^{given}",
                TargetFhirPath: "Patient.name[0]",
                TargetLabel: "HumanName",
                TargetValue: $"{given} {family}",
                ConceptExplanation: "HL7 v2 XPN datatype splits names with hats (^). FHIR uses a structured HumanName object with 'family' and a list of 'given' strings.",
                InteropGotcha: "In FHIR, middle names belong in the 'given' array as second elements, not in a separate 'middle' property.",
                ConceptExplanationAr: "يقسم نوع البيانات XPN في HL7 v2 الاسم برموز القبعة (^)، بينما يبنيه FHIR ككائن HumanName يحتوي العائلة وقائمة الأسماء الشخصية.",
                InteropGotchaAr: "في FHIR، توضع الأسماء الوسطى والجد داخل مصفوفة given كعناصر تالية، وليس في خاصية منفصلة.",
                SourceLabelAr: "اسم المريض (العائلة^الشخصي)",
                TargetLabelAr: "كائن الاسم البشري (HumanName)"
            ));
        }

        // PID-7 Date of Birth
        var dobVal = pid.GetValue(7, 1);
        if (!string.IsNullOrWhiteSpace(dobVal))
        {
            string fhirDate = FormatHl7Date(dobVal);
            patient.BirthDate = fhirDate;

            links.Add(new FieldMappingLink(
                SourceField: "PID-7",
                SourceLabel: "Date/Time of Birth",
                SourceValue: dobVal,
                TargetFhirPath: "Patient.birthDate",
                TargetLabel: "Date (YYYY-MM-DD)",
                TargetValue: fhirDate,
                ConceptExplanation: "HL7 v2 stores timestamps in compact YYYYMMDD[HHMMSS] format. FHIR enforces ISO-8601 (YYYY-MM-DD).",
                InteropGotcha: "If v2 sends time of birth (e.g., in neonatology), FHIR requires an extension 'patient-birthTime' since birthDate only holds the date.",
                ConceptExplanationAr: "يخزن HL7 v2 التاريخ بصيغة YYYYMMDD مدمجة، بينما يفرض FHIR معيار ISO-8601 الدولي (YYYY-MM-DD).",
                InteropGotchaAr: "إذا أرسل v2 وقت الولادة (كما في أقسام حديثي الولادة)، يتطلب FHIR إضافة ملحق extension مخصص لأن birthDate يحمل التاريخ فقط.",
                SourceLabelAr: "تاريخ وساعة الولادة",
                TargetLabelAr: "التاريخ بصيغة ISO"
            ));
        }

        // PID-8 Administrative Sex
        var sexVal = pid.GetValue(8, 1);
        if (!string.IsNullOrWhiteSpace(sexVal))
        {
            var gender = sexVal.ToUpperInvariant() switch
            {
                "M" => AdministrativeGender.Male,
                "F" => AdministrativeGender.Female,
                "O" => AdministrativeGender.Other,
                _ => AdministrativeGender.Unknown
            };
            patient.Gender = gender;

            links.Add(new FieldMappingLink(
                SourceField: "PID-8",
                SourceLabel: "Administrative Sex",
                SourceValue: sexVal,
                TargetFhirPath: "Patient.gender",
                TargetLabel: "AdministrativeGender code",
                TargetValue: gender.ToString()?.ToLowerInvariant() ?? "unknown",
                ConceptExplanation: "HL7 v2 uses User-defined Table 0001 (M, F, O, U, A, N). FHIR binds strictly to a 4-value required ValueSet (male, female, other, unknown).",
                ConceptExplanationAr: "يستخدم HL7 v2 جدول 0001 المفتوح، بينما يلتزم FHIR بنطاق إلزامي صارم من 4 قيم (male, female, other, unknown).",
                SourceLabelAr: "الجنس الإداري",
                TargetLabelAr: "رمز الجنس الإداري"
            ));
        }

        // PID-11 Address
        var addrField = pid.GetField(11);
        if (addrField != null && !string.IsNullOrWhiteSpace(addrField.Value))
        {
            string street = pid.GetValue(11, 1);
            string city = pid.GetValue(11, 3);
            string state = pid.GetValue(11, 4);
            string zip = pid.GetValue(11, 5);
            string country = pid.GetValue(11, 6);

            var address = new Address
            {
                Use = Address.AddressUse.Home,
                City = city,
                State = state,
                PostalCode = zip,
                Country = country
            };
            if (!string.IsNullOrWhiteSpace(street)) address.Line = new[] { street };
            patient.Address.Add(address);

            links.Add(new FieldMappingLink(
                SourceField: "PID-11",
                SourceLabel: "Patient Address (XAD)",
                SourceValue: addrField.Value,
                TargetFhirPath: "Patient.address[0]",
                TargetLabel: "Address",
                TargetValue: $"{street}, {city}, {state} {zip}",
                ConceptExplanation: "HL7 v2 XAD composite datatype maps cleanly to FHIR's Address datatype.",
                ConceptExplanationAr: "يقابل نوع البيانات المركب XAD في HL7 v2 كائن العنوان المنظم Address في FHIR بكل دقة.",
                SourceLabelAr: "عنوان المريض (XAD)",
                TargetLabelAr: "كائن العنوان (Address)"
            ));
        }

        // PID-13 Phone
        var phoneVal = pid.GetValue(13, 1);
        if (!string.IsNullOrWhiteSpace(phoneVal))
        {
            patient.Telecom.Add(new ContactPoint
            {
                System = ContactPoint.ContactPointSystem.Phone,
                Value = phoneVal,
                Use = ContactPoint.ContactPointUse.Home
            });

            links.Add(new FieldMappingLink(
                SourceField: "PID-13",
                SourceLabel: "Phone Number - Home",
                SourceValue: phoneVal,
                TargetFhirPath: "Patient.telecom[0].value",
                TargetLabel: "ContactPoint.value",
                TargetValue: phoneVal,
                ConceptExplanation: "In FHIR, phones, emails, and faxes are unified under the 'ContactPoint' datatype with a 'system' discriminator.",
                ConceptExplanationAr: "يوحد FHIR أرقام الهواتف والبريد الإلكتروني تحت كائن ContactPoint مع محدد system للتمييز.",
                SourceLabelAr: "رقم هاتف المنزل",
                TargetLabelAr: "قيمة نقطة الاتصال"
            ));
        }

        return patient;
    }

    private static Encounter MapEncounter(Hl7Segment pv1, List<FieldMappingLink> links, Patient? patient)
    {
        var encounter = new Encounter
        {
            Id = Guid.NewGuid().ToString()
        };

        if (patient != null)
        {
            encounter.Subject = new ResourceReference($"urn:uuid:{patient.Id}")
            {
                Display = patient.Name.FirstOrDefault()?.Family
            };
        }

        // PV1-2 Patient Class
        var pClass = pv1.GetValue(2, 1).ToUpperInvariant();
        Coding classCoding = pClass switch
        {
            "I" => new Coding("http://terminology.hl7.org/CodeSystem/v3-ActCode", "IMP", "inpatient encounter"),
            "O" => new Coding("http://terminology.hl7.org/CodeSystem/v3-ActCode", "AMB", "ambulatory"),
            "E" => new Coding("http://terminology.hl7.org/CodeSystem/v3-ActCode", "EMER", "emergency"),
            _ => new Coding("http://terminology.hl7.org/CodeSystem/v3-ActCode", "AMB", "ambulatory")
        };
        encounter.Class = classCoding;

        links.Add(new FieldMappingLink(
            SourceField: "PV1-2",
            SourceLabel: "Patient Class",
            SourceValue: pClass,
            TargetFhirPath: "Encounter.class",
            TargetLabel: "Coding (ActCode)",
            TargetValue: classCoding.Code ?? "AMB",
            ConceptExplanation: "HL7 v2 uses Table 0004 (I, O, E, P, R, B). FHIR binds 'class' to the HL7 v3 ActCode vocabulary ('IMP', 'AMB', 'EMER').",
            ConceptExplanationAr: "يستخدم HL7 v2 الجدول 0004، بينما يربط FHIR تصنيف التنويم بمصطلحات HL7 v3 ActCode القياسية (IMP, AMB, EMER).",
            SourceLabelAr: "تصنيف المريض (منوم/عيادات/طوارئ)",
            TargetLabelAr: "ترميز صنف الزيارة (ActCode)"
        ));

        // PV1-19 Visit Number
        var visitNum = pv1.GetValue(19, 1);
        if (!string.IsNullOrWhiteSpace(visitNum))
        {
            encounter.Identifier.Add(new Identifier
            {
                System = "http://hospital.example.org/visit-number",
                Value = visitNum,
                Type = new CodeableConcept("http://terminology.hl7.org/CodeSystem/v2-0203", "VN", "Visit Number")
            });

            links.Add(new FieldMappingLink(
                SourceField: "PV1-19",
                SourceLabel: "Visit Number / Account",
                SourceValue: visitNum,
                TargetFhirPath: "Encounter.identifier[0].value",
                TargetLabel: "Identifier",
                TargetValue: visitNum,
                ConceptExplanation: "The unique visit or encounter identifier generated by the Patient Administration System (PAS).",
                ConceptExplanationAr: "رقم الزيارة أو الحساب المالي الفريد المولد من نظام إدارة المرضى والقبول (PAS).",
                SourceLabelAr: "رقم الزيارة / الحساب المالي",
                TargetLabelAr: "معرف الزيارة الفريد"
            ));
        }

        // PV1-3 Location (POC^Room^Bed)
        var poc = pv1.GetValue(3, 1);
        var room = pv1.GetValue(3, 2);
        var bed = pv1.GetValue(3, 3);
        if (!string.IsNullOrWhiteSpace(poc) || !string.IsNullOrWhiteSpace(room) || !string.IsNullOrWhiteSpace(bed))
        {
            var locDesc = string.Join(" / ", new[] { poc, room, bed }.Where(s => !string.IsNullOrWhiteSpace(s)));
            encounter.Location.Add(new Encounter.LocationComponent
            {
                Location = new ResourceReference { Display = locDesc },
                Status = Encounter.EncounterLocationStatus.Active
            });

            links.Add(new FieldMappingLink(
                SourceField: "PV1-3",
                SourceLabel: "Assigned Patient Location",
                SourceValue: $"{poc}^{room}^{bed}",
                TargetFhirPath: "Encounter.location[0].location.display",
                TargetLabel: "Location Reference",
                TargetValue: locDesc,
                ConceptExplanation: "HL7 v2 sends hierarchical bed locations in a single composite field (PL). In FHIR, Location is its own first-class Resource.",
                ConceptExplanationAr: "يرسل HL7 v2 موقع السرير في حقل مركب هرمي (PL)، بينما يعتبر FHIR الموقع مورداً مستقلاً بذاته (Location).",
                SourceLabelAr: "موقع المريض وسرير التنويم",
                TargetLabelAr: "مرجع موقع الإقامة"
            ));
        }

        // PV1-44 Admit Date & PV1-45 Discharge Date
        var admitDate = pv1.GetValue(44, 1);
        var dischargeDate = pv1.GetValue(45, 1);

        encounter.Period = new Period();
        if (!string.IsNullOrWhiteSpace(admitDate))
        {
            encounter.Period.Start = FormatHl7DateTime(admitDate);
        }
        if (!string.IsNullOrWhiteSpace(dischargeDate))
        {
            encounter.Period.End = FormatHl7DateTime(dischargeDate);
            encounter.Status = Encounter.EncounterStatus.Finished;
        }
        else
        {
            encounter.Status = Encounter.EncounterStatus.InProgress;
        }

        links.Add(new FieldMappingLink(
            SourceField: "PV1-44 / PV1-45",
            SourceLabel: "Admit & Discharge Date/Time",
            SourceValue: $"{admitDate} -> {dischargeDate}",
            TargetFhirPath: "Encounter.period & Encounter.status",
            TargetLabel: "Period & Status",
            TargetValue: $"{encounter.Status} ({encounter.Period.Start} to {encounter.Period.End ?? "now"})",
            ConceptExplanation: "HL7 v2 uses separate timestamps (PV1-44 admit, PV1-45 discharge). In FHIR, status is an explicit state machine ('in-progress', 'finished') combined with a start/end Period.",
            ConceptExplanationAr: "يستخدم HL7 v2 أوقاتاً منفصلة، بينما يدمج FHIR الحالة كآلة حالات واضحة (in-progress/finished) مع فترة زمنية Period.",
            SourceLabelAr: "تاريخ ووقت الدخول والخروج",
            TargetLabelAr: "فترة الزيارة وحالة التنويم"
        ));

        return encounter;
    }

    private static DiagnosticReport MapDiagnosticReport(Hl7Segment obr, List<FieldMappingLink> links, Patient? patient)
    {
        var report = new DiagnosticReport
        {
            Id = Guid.NewGuid().ToString(),
            Status = DiagnosticReport.DiagnosticReportStatus.Final,
            Category = new List<CodeableConcept>
            {
                new("http://terminology.hl7.org/CodeSystem/v2-0074", "LAB", "Laboratory")
            }
        };

        if (patient != null)
        {
            report.Subject = new ResourceReference($"urn:uuid:{patient.Id}")
            {
                Display = patient.Name.FirstOrDefault()?.Family
            };
        }

        // OBR-4 Universal Service Identifier (Panel Code)
        var code = obr.GetValue(4, 1);
        var name = obr.GetValue(4, 2);
        var system = obr.GetValue(4, 3);
        var codeSystemUri = system.Equals("LN", StringComparison.OrdinalIgnoreCase) ? "http://loinc.org" : "http://hospital.example.org/tests";

        report.Code = new CodeableConcept(codeSystemUri, code, name);

        links.Add(new FieldMappingLink(
            SourceField: "OBR-4",
            SourceLabel: "Universal Service Identifier (Panel)",
            SourceValue: $"{code}^{name}^{system}",
            TargetFhirPath: "DiagnosticReport.code",
            TargetLabel: "CodeableConcept",
            TargetValue: $"{code} ({name})",
            ConceptExplanation: "OBR-4 represents the ordered battery or panel. In FHIR, this is the DiagnosticReport.code.",
            InteropGotcha: "HL7 v2 standardizes '^LN' as LOINC, but local legacy systems often use '^99ROC' or '^LOCAL' codes requiring terminology mapping tables.",
            ConceptExplanationAr: "يمثل حقل OBR-4 الفحص أو الحزمة المخبرية المطلوبة، ويتحول إلى DiagnosticReport.code في FHIR.",
            InteropGotchaAr: "يوصي HL7 v2 بترميز LOINC ولكن الأنظمة القديمة ترسل أحياناً رموزاً محلية تتطلب جداول ربط ومطابقة.",
            SourceLabelAr: "معرف الخدمة الشامل (حزمة التحاليل)",
            TargetLabelAr: "المفهوم المرمز (CodeableConcept)"
        ));

        return report;
    }

    private static Observation MapObservation(Hl7Segment obx, List<FieldMappingLink> links, Patient? patient, int sequence)
    {
        var obs = new Observation
        {
            Id = Guid.NewGuid().ToString(),
            Status = ObservationStatus.Final,
            Category = new List<CodeableConcept>
            {
                new("http://terminology.hl7.org/CodeSystem/observation-category", "laboratory", "Laboratory")
            }
        };

        if (patient != null)
        {
            obs.Subject = new ResourceReference($"urn:uuid:{patient.Id}");
        }

        // OBX-3 Test Identifier
        var testCode = obx.GetValue(3, 1);
        var testName = obx.GetValue(3, 2);
        var codingSys = obx.GetValue(3, 3);
        var sysUri = codingSys.Equals("LN", StringComparison.OrdinalIgnoreCase) ? "http://loinc.org" : "http://hospital.example.org/tests";
        obs.Code = new CodeableConcept(sysUri, testCode, testName);

        // OBX-5 Value & OBX-6 Units
        var valType = obx.GetValue(2, 1);
        var rawVal = obx.GetValue(5, 1);
        var units = obx.GetValue(6, 1);

        if (valType.Equals("NM", StringComparison.OrdinalIgnoreCase) && decimal.TryParse(rawVal, NumberStyles.Any, CultureInfo.InvariantCulture, out var numVal))
        {
            obs.Value = new Quantity(numVal, units)
            {
                System = "http://unitsofmeasure.org",
                Code = units
            };
        }
        else
        {
            obs.Value = new FhirString(rawVal);
        }

        // OBX-7 Reference Range
        var refRange = obx.GetValue(7, 1);
        if (!string.IsNullOrWhiteSpace(refRange))
        {
            obs.ReferenceRange.Add(new Observation.ReferenceRangeComponent
            {
                Text = refRange
            });
        }

        // OBX-8 Abnormal Flag
        var flag = obx.GetValue(8, 1);
        if (!string.IsNullOrWhiteSpace(flag))
        {
            string flagDisplay = flag switch
            {
                "H" => "High",
                "L" => "Low",
                "A" => "Abnormal",
                _ => "Normal"
            };
            obs.Interpretation.Add(new CodeableConcept("http://terminology.hl7.org/CodeSystem/v3-ObservationInterpretation", flag, flagDisplay));
        }

        links.Add(new FieldMappingLink(
            SourceField: $"OBX #{sequence} (OBX-3 & OBX-5)",
            SourceLabel: $"Lab Result #{sequence} ({testName})",
            SourceValue: $"{testCode} = {rawVal} {units} (Flag: {flag})",
            TargetFhirPath: $"Observation.code & Observation.valueQuantity",
            TargetLabel: "Observation Resource",
            TargetValue: $"{testName}: {rawVal} {units}",
            ConceptExplanation: "Each OBX segment produces a distinct FHIR Observation resource containing discrete Quantity and Interpretation (abnormal flag) elements.",
            InteropGotcha: "OBX-2 defines the data type ('NM', 'ST', 'CE'). In FHIR, Observation uses polymorphic value elements: valueQuantity, valueCodeableConcept, valueString, etc.",
            ConceptExplanationAr: "يتحول كل مقطع OBX إلى مورد Observation مستقل يحتوي على كمية رقمية وتفسير سريري واضح.",
            InteropGotchaAr: "يحدد OBX-2 نوع البيانات، بينما يستخدم FHIR عناصر قيمة متعددة الأشكال مثل valueQuantity و valueString.",
            SourceLabelAr: $"نتيجة الفحص المخبري #{sequence} ({testName})",
            TargetLabelAr: "مورد الملاحظة السريرية (Observation)"
        ));

        return obs;
    }

    private static string FormatHl7Date(string raw)
    {
        if (raw.Length >= 8 && DateTime.TryParseExact(raw.Substring(0, 8), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
        {
            return dt.ToString("yyyy-MM-dd");
        }
        return raw;
    }

    private static string FormatHl7DateTime(string raw)
    {
        if (raw.Length >= 14 && DateTime.TryParseExact(raw.Substring(0, 14), "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
        {
            return dt.ToString("yyyy-MM-dd'T'HH:mm:sszzz");
        }
        if (raw.Length >= 8 && DateTime.TryParseExact(raw.Substring(0, 8), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
        {
            return d.ToString("yyyy-MM-dd");
        }
        return raw;
    }
}
