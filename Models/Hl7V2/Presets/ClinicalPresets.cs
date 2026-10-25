namespace Hl7FhirWorkbench.Models.Hl7V2.Presets;

public record ClinicalPreset(
    string Id,
    string Title,
    string Category,
    string TriggerEvent,
    string Summary,
    string Hl7V2Message,
    string TeachingNotes,
    string? ArabicTitle = null,
    string? ArabicCategory = null,
    string? ArabicSummary = null
)
{
    public string TitleAr => ArabicTitle ?? Title;
    public string SummaryAr => ArabicSummary ?? Summary;
    public string CategoryAr => ArabicCategory ?? Category;
}

public static class ClinicalPresets
{
    public static readonly List<ClinicalPreset> All = new()
    {
        new ClinicalPreset(
            Id: "registration",
            Title: "1. Patient Registration (Outpatient)",
            Category: "Demographics & Identity",
            TriggerEvent: "ADT^A04",
            Summary: "Registers a new patient with demographics, MRN, phone, address, and next-of-kin.",
            Hl7V2Message:
@"MSH|^~\&|REG_SYS|HOSPITAL_A|EMR|CLINIC_1|20261011103000||ADT^A04|MSG20261011001|P|2.5
EVN|A04|20261011103000
PID|1||MRN982341^^^HOSPITAL_A^MR||Smith^Eleanor^Jane||19850614|F||2106-3|1244 Healthcare Blvd^^Austin^TX^78701^USA||5125550199||ENG|M||ACC-449102|999-88-7777
PV1|1|O|CLINIC_OUTPATIENT^EXAM_3^^HOSPITAL_A||||DOC442^Adams^John^M||||||||||||VISIT-90182|",
            TeachingNotes:
@"• **ADT^A04 vs ADT^A28:** ADT^A04 is historically used for Outpatient/Walk-in Registration; ADT^A28 is explicit 'Add Person/Demographics'.
• **PID-3 (MRN):** Contains the Medical Record Number, assigning authority (`HOSPITAL_A`), and type code (`MR`). In FHIR, this becomes a `Patient.identifier`.
• **PID-5 (Name):** Uses `Family^Given^Middle^Suffix^Prefix`. Note how FHIR breaks this into `HumanName.family` and `HumanName.given[]`.
• **PV1-2 (Patient Class):** Value `O` denotes Outpatient (maps to FHIR `Encounter.class` = `AMB` ambulatory).",
            ArabicTitle: "1. تسجيل مريض جديد (عيادات خارجية)",
            ArabicCategory: "الهوية والديموغرافيا",
            ArabicSummary: "تسجيل ملف مريض جديد بالرقم الطبي، الهاتف، العنوان، وبيانات الزيارة الأولية."
        ),

        new ClinicalPreset(
            Id: "admission",
            Title: "2. Inpatient Admission (Bed & Ward)",
            Category: "Encounter & Inpatient",
            TriggerEvent: "ADT^A01",
            Summary: "Admit a patient to an inpatient ward (Point of care, Room, Bed) with an attending physician.",
            Hl7V2Message:
@"MSH|^~\&|ADT_ENGINE|METRO_HEALTH|CENTRAL_EHR|METRO_HEALTH|20261011104500||ADT^A01|MSG20261011002|P|2.5
EVN|A01|20261011104500
PID|1||MRN450192^^^METRO_HEALTH^MR||Vance^Marcus^T||19720325|M|||404 River Rd^^Denver^CO^80203^USA||3035550144||ENG|S
PV1|1|I|ICU_CARDIO^ROOM_402^BED_B^METRO_HEALTH||||DOC881^Cardio^Sarah^B|||CARD||||1||||VISIT-77391|||||||||||||||||||||||||20261011104500
DG1|1|ICD10|I21.0|ST-Elevation Myocardial Infarction|20261011104500|A",
            TeachingNotes:
@"• **ADT^A01 (Admit):** Signals physical admission of an inpatient into a hospital bed.
• **PV1-2 & PV1-3:** `I` stands for Inpatient. Field 3 (`ICU_CARDIO^ROOM_402^BED_B`) provides hierarchical location: Unit $\rightarrow$ Room $\rightarrow$ Bed.
• **PV1-7 (Attending Doctor):** Contains `ID^Family^Given`.
• **DG1 (Diagnosis):** In v2, diagnoses are often sent as `DG1` segments. In FHIR, this maps to a `Condition` resource linked via `Encounter.diagnosis`.",
            ArabicTitle: "2. تنويم مريض (جناح وغرفة وسرير)",
            ArabicCategory: "الزيارات والتنويم",
            ArabicSummary: "إشعار إدخال مريض لقسم العناية المركزة وتحديد الجناح والغرفة والسرير والطبيب المعالج."
        ),

        new ClinicalPreset(
            Id: "discharge",
            Title: "3. Patient Discharge",
            Category: "Encounter Lifecycle",
            TriggerEvent: "ADT^A03",
            Summary: "Patient discharge event updating the encounter status and recording the discharge disposition.",
            Hl7V2Message:
@"MSH|^~\&|ADT_ENGINE|METRO_HEALTH|CENTRAL_EHR|METRO_HEALTH|20261011120000||ADT^A03|MSG20261011003|P|2.5
EVN|A03|20261011120000
PID|1||MRN450192^^^METRO_HEALTH^MR||Vance^Marcus^T||19720325|M|||404 River Rd^^Denver^CO^80203^USA||3035550144
PV1|1|I|ICU_CARDIO^ROOM_402^BED_B^METRO_HEALTH||||DOC881^Cardio^Sarah^B||||||||||||VISIT-77391||||||||||||||||||01||||||20261011104500|20261011120000",
            TeachingNotes:
@"• **ADT^A03 (Discharge):** Signals the patient has left the facility.
• **PV1-45 (Discharge Date/Time):** `20261011120000`. Maps to FHIR `Encounter.period.end`.
• **PV1-36 (Discharge Disposition):** Code `01` ('Discharged to home or self care'). In FHIR, maps to `Encounter.hospitalization.dischargeDisposition`.",
            ArabicTitle: "3. خروج المريض من المستشفى",
            ArabicCategory: "دورة حياة الزيارة",
            ArabicSummary: "إشعار خروج المريض وتحديث حالة الزيارة إلى منتهية وتوثيق حالة الخروج."
        ),

        new ClinicalPreset(
            Id: "lab_results",
            Title: "4. Laboratory Results (Chemistry Panel)",
            Category: "Diagnostic & Observation",
            TriggerEvent: "ORU^R01",
            Summary: "Unsolicited transmission of a Basic Metabolic Panel with Glucose, Potassium, and Sodium (LOINC codes, units, flags).",
            Hl7V2Message:
@"MSH|^~\&|LIS_LAB|REGIONAL_LAB|EMR|HOSPITAL_A|20261011111500||ORU^R01|MSG20261011004|P|2.5
PID|1||MRN982341^^^HOSPITAL_A^MR||Smith^Eleanor^Jane||19850614|F
OBR|1|ORD-2026-901|FILL-9821|24323-8^Basic Metabolic Panel^LN|||20261011103000||||||||DOC442^Adams^John^M||||||20261011111500|||F
OBX|1|NM|2345-7^Glucose [Mass/volume] in Serum or Plasma^LN||142|mg/dL|70-99|H|||F|||20261011103000
OBX|2|NM|2823-3^Potassium [Moles/volume] in Serum or Plasma^LN||4.2|mmol/L|3.5-5.1|N|||F|||20261011103000
OBX|3|NM|2951-2^Sodium [Moles/volume] in Serum or Plasma^LN||138|mmol/L|135-145|N|||F|||20261011103000",
            TeachingNotes:
@"• **ORU^R01 Hierarchy:** MSH $\rightarrow$ PID $\rightarrow$ OBR (Order/Panel header) $\rightarrow$ OBX...OBX (Discrete test results).
• **LOINC Coding:** `2345-7` is Glucose, `2823-3` is Potassium, `2951-2` is Sodium. Note the `^LN` identifier denoting the LOINC standard.
• **OBX-2 (Value Type):** `NM` signifies Numeric. Other types include `ST` (String), `CE` (Coded), `TX` (Text).
• **OBX-8 (Abnormal Flag):** `H` = High (Glucose is 142 vs 70-99 range). In FHIR, this maps to `Observation.interpretation` ('H' / 'high').
• **FHIR Equivalence:** OBR maps to `DiagnosticReport`; each OBX maps to an `Observation` resource referenced in `DiagnosticReport.result`.",
            ArabicTitle: "4. نتائج الفحوصات المخبرية (لوحة الكيمياء)",
            ArabicCategory: "التقارير والملاحظات السريرية",
            ArabicSummary: "إرسال نتائج فحوصات الجلوكوز والبوتاسيوم والصوديوم مع أكواد LOINC والوحدات والرايات الحرجة."
        )
    };

    public static ClinicalPreset GetById(string id) =>
        All.FirstOrDefault(p => p.Id.Equals(id, StringComparison.OrdinalIgnoreCase)) ?? All[0];
}
