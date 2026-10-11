namespace Hl7FhirWorkbench.Models.Hl7V2;

public static class Hl7V2Parser
{
    private static readonly Dictionary<string, string> SegmentDescriptions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["MSH"] = "Message Header (Delimiters, Routing, Timestamp, Trigger Event)",
        ["EVN"] = "Event Type (Event Trigger Date/Time, Operator)",
        ["PID"] = "Patient Identification (Demographics, MRN, Name, DOB, Gender, Address)",
        ["PV1"] = "Patient Visit (Visit Class, Location, Assigned Room/Bed, Attending Doctor)",
        ["PV2"] = "Patient Visit - Additional Info (Admit Reason, Expected Length of Stay)",
        ["OBR"] = "Observation Request (Diagnostic Order / Lab Panel Request)",
        ["OBX"] = "Observation / Result (Single test result, quantitative value, units, flags)",
        ["DG1"] = "Diagnosis (ICD Code, Diagnosis Type, Attending Diagnosis)",
        ["AL1"] = "Patient Allergy Information (Allergen, Severity, Reaction)",
        ["IN1"] = "Insurance Information (Payer, Policy Number, Group Number)"
    };

    private static readonly Dictionary<string, Dictionary<int, string>> FieldDescriptions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["MSH"] = new()
        {
            [1] = "Field Delimiter",
            [2] = "Encoding Characters",
            [3] = "Sending Application",
            [4] = "Sending Facility",
            [5] = "Receiving Application",
            [6] = "Receiving Facility",
            [7] = "Date/Time of Message",
            [8] = "Security",
            [9] = "Message Type (MsgType^TriggerEvent)",
            [10] = "Message Control ID",
            [11] = "Processing ID (P=Prod, D=Debug, T=Test)",
            [12] = "Version ID (e.g. 2.3, 2.5.1)"
        },
        ["PID"] = new()
        {
            [1] = "Set ID - PID",
            [2] = "Patient ID (External)",
            [3] = "Patient Identifier List (MRN, Authority, Type)",
            [4] = "Alternate Patient ID",
            [5] = "Patient Name (Family^Given^Middle^Suffix^Prefix)",
            [6] = "Mother's Maiden Name",
            [7] = "Date/Time of Birth (YYYYMMDD)",
            [8] = "Administrative Sex (M, F, O, U)",
            [9] = "Patient Alias",
            [10] = "Race",
            [11] = "Patient Address (Street^Other^City^State^Zip)",
            [12] = "County Code",
            [13] = "Phone Number - Home",
            [14] = "Phone Number - Business",
            [15] = "Primary Language",
            [16] = "Marital Status",
            [18] = "Patient Account Number",
            [19] = "SSN Number"
        },
        ["PV1"] = new()
        {
            [1] = "Set ID - PV1",
            [2] = "Patient Class (I=Inpatient, O=Outpatient, E=Emergency)",
            [3] = "Assigned Patient Location (POC^Room^Bed^Facility)",
            [4] = "Admission Type",
            [7] = "Attending Doctor (ID^Family^Given)",
            [8] = "Referring Doctor",
            [10] = "Hospital Service",
            [14] = "Admit Source",
            [17] = "Admitting Doctor",
            [18] = "Patient Type",
            [19] = "Visit Number",
            [44] = "Admit Date/Time",
            [45] = "Discharge Date/Time"
        },
        ["OBR"] = new()
        {
            [1] = "Set ID - OBR",
            [2] = "Placer Order Number",
            [3] = "Filler Order Number",
            [4] = "Universal Service Identifier (Code^Name^CodingSystem)",
            [7] = "Observation Date/Time",
            [14] = "Specimen Received Date/Time",
            [16] = "Ordering Provider",
            [22] = "Results Rpt/Status Chng - Date/Time",
            [25] = "Result Status (F=Final, P=Preliminary, C=Corrected)"
        },
        ["OBX"] = new()
        {
            [1] = "Set ID - OBX",
            [2] = "Value Type (NM=Numeric, ST=String, CE=Coded, TX=Text)",
            [3] = "Observation Identifier (Code^Text^System)",
            [4] = "Observation Sub-ID",
            [5] = "Observation Value",
            [6] = "Units (Identifier^Text^System)",
            [7] = "References Range (e.g. 70-99)",
            [8] = "Abnormal Flags (L=Low, H=High, A=Abnormal, N=Normal)",
            [11] = "Observation Result Status (F=Final, C=Corrected)",
            [14] = "Date/Time of the Observation"
        }
    };

    public static Hl7Message Parse(string raw)
    {
        var message = new Hl7Message { RawMessage = raw };
        if (string.IsNullOrWhiteSpace(raw)) return message;

        // Normalize line endings
        var lines = raw.Replace("\r\n", "\n")
                       .Replace("\r", "\n")
                       .Split('\n', StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length == 0) return message;

        // Parse MSH to discover delimiters
        var mshLine = lines.FirstOrDefault(l => l.StartsWith("MSH"));
        if (mshLine != null && mshLine.Length >= 8)
        {
            message.FieldDelimiter = mshLine[3];
            var encChars = mshLine.Substring(4, Math.Min(4, mshLine.Length - 4));
            if (encChars.Length >= 1) message.ComponentDelimiter = encChars[0];
            if (encChars.Length >= 2) message.RepetitionDelimiter = encChars[1];
            if (encChars.Length >= 3) message.EscapeCharacter = encChars[2];
            if (encChars.Length >= 4) message.SubcomponentDelimiter = encChars[3];
        }

        int lineNumber = 1;
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmed)) continue;

            var segment = ParseSegment(trimmed, lineNumber++, message);
            message.Segments.Add(segment);

            if (segment.SegmentId.Equals("MSH", StringComparison.OrdinalIgnoreCase))
            {
                // MSH-9 Message Type
                var msgTypeField = segment.GetField(9);
                if (msgTypeField != null)
                {
                    message.MessageType = msgTypeField.Components.Count > 0 ? msgTypeField.Components[0].Value : msgTypeField.Value;
                    if (msgTypeField.Components.Count > 1)
                    {
                        message.TriggerEvent = msgTypeField.Components[1].Value;
                    }
                }
                // MSH-10 Message Control ID
                var ctrlIdField = segment.GetField(10);
                if (ctrlIdField != null)
                {
                    message.MessageControlId = ctrlIdField.Value;
                }
            }
        }

        return message;
    }

    private static Hl7Segment ParseSegment(string line, int lineNumber, Hl7Message msg)
    {
        var seg = new Hl7Segment
        {
            LineNumber = lineNumber,
            RawContent = line
        };

        if (line.StartsWith("MSH", StringComparison.OrdinalIgnoreCase))
        {
            seg.SegmentId = "MSH";
            seg.Description = SegmentDescriptions.GetValueOrDefault("MSH", "Message Header");
            
            // MSH-1 is the delimiter itself
            seg.Fields.Add(new Hl7Field
            {
                Index = 1,
                Name = "MSH-1",
                Value = msg.FieldDelimiter.ToString(),
                Description = "Field Delimiter"
            });

            // Split remainder
            var parts = line.Substring(4).Split(msg.FieldDelimiter);
            for (int i = 0; i < parts.Length; i++)
            {
                int fieldIndex = i + 2; // MSH-2 is encoding characters
                var fieldVal = parts[i];
                var field = ParseField("MSH", fieldIndex, fieldVal, msg);
                seg.Fields.Add(field);
            }
            return seg;
        }

        var tokens = line.Split(msg.FieldDelimiter);
        seg.SegmentId = tokens[0].Trim();
        seg.Description = SegmentDescriptions.GetValueOrDefault(seg.SegmentId, $"HL7 {seg.SegmentId} Segment");

        for (int i = 1; i < tokens.Length; i++)
        {
            int fieldIndex = i;
            var fieldVal = tokens[i];
            var field = ParseField(seg.SegmentId, fieldIndex, fieldVal, msg);
            seg.Fields.Add(field);
        }

        return seg;
    }

    private static Hl7Field ParseField(string segmentId, int fieldIndex, string rawValue, Hl7Message msg)
    {
        var field = new Hl7Field
        {
            Index = fieldIndex,
            Name = $"{segmentId}-{fieldIndex}",
            Value = rawValue,
            Description = GetFieldDescription(segmentId, fieldIndex)
        };

        if (rawValue.Contains(msg.RepetitionDelimiter))
        {
            field.Repetitions = rawValue.Split(msg.RepetitionDelimiter).ToList();
        }

        if (rawValue.Contains(msg.ComponentDelimiter))
        {
            var comps = rawValue.Split(msg.ComponentDelimiter);
            for (int c = 0; c < comps.Length; c++)
            {
                field.Components.Add(new Hl7Component
                {
                    Index = c + 1,
                    Name = $"{segmentId}-{fieldIndex}.{c + 1}",
                    Value = comps[c]
                });
            }
        }

        return field;
    }

    private static string GetFieldDescription(string segmentId, int fieldIndex)
    {
        if (FieldDescriptions.TryGetValue(segmentId, out var dict) && dict.TryGetValue(fieldIndex, out var desc))
        {
            return desc;
        }
        return $"{segmentId} Field #{fieldIndex}";
    }
}
