namespace Hl7FhirWorkbench.Models.Glossary;

public record GlossaryItem(
    string Term, 
    string Category, 
    string ShortDefinition, 
    string LongExplanation, 
    string Standard,
    string? ArabicTerm = null,
    string? ArabicDefinition = null,
    string? ArabicExplanation = null
);

public static class GlossaryData
{
    public static readonly List<GlossaryItem> Items = new()
    {
        new("MRN", "Identity", "Medical Record Number", 
            "A unique identifier assigned by a specific hospital to identify a patient across visits. Maps to PID-3 in HL7 v2 and Patient.identifier in FHIR.", 
            "Both",
            "رقم السجل الطبي (MRN)",
            "معرّف فريد للمريض داخل المستشفى",
            "معرّف دائم يخصصه المستشفى لتتبع المريض عبر الزيارات المختلفة. يقابله في HL7 v2 الحقل PID-3 وفي FHIR الحقل Patient.identifier."),

        new("FIN / Visit #", "Encounter", "Financial Account / Encounter Number", 
            "Identifies a specific clinical visit or hospital admission. Maps to PV1-19 in v2 and Encounter.identifier in FHIR.", 
            "Both",
            "رقم الزيارة / الحساب المالي (FIN)",
            "معرّف الزيارة أو التنويم السريري المحدد",
            "معرّف مالي وإداري فريد لكل دخول للمستشفى أو زيارة عيادة. يقابله في HL7 v2 الحقل PV1-19 وفي FHIR الحقل Encounter.identifier."),

        new("Placer Order Number", "Orders", "Order ID assigned by the requesting physician/EHR", 
            "The tracking ID assigned by the EHR or doctor when ordering a lab or radiology test. Sent in ORC-2 / OBR-2 in v2; ServiceRequest.identifier in FHIR.", 
            "HL7 v2",
            "رقم طلب الجهة الطالبة (Placer)",
            "معرّف الطلب الصادر من الطبيب أو نظام السجلات (EHR)",
            "رقم تتبع ينشئه نظام السجلات عند طلب فحص مخبري أو شعاعي. يرسل في حقول ORC-2 / OBR-2 في v2، وفي ServiceRequest.identifier في FHIR."),

        new("Filler Order Number", "Orders", "Order ID assigned by the performing laboratory/system", 
            "The tracking ID assigned by the lab instrument or LIS performing the test. Sent in ORC-3 / OBR-3 in v2; DiagnosticReport.identifier in FHIR.", 
            "HL7 v2",
            "رقم طلب الجهة المنفذة (Filler)",
            "معرّف الطلب لدى المختبر أو جهاز الفحص المنفّذ",
            "رقم تتبع يخصصه نظام المختبر (LIS) أو قسم الأشعة (RIS) لمتابعة تنفيذ الفحص. يرسل في ORC-3 / OBR-3 في v2، وفي DiagnosticReport.identifier في FHIR."),

        new("Accession Number", "Orders", "Unique specimen identifier in laboratory/imaging", 
            "A unique specimen or scan identifier generated when blood is drawn or an imaging study is registered. Sent in OBR-20 in v2.", 
            "Both",
            "رقم الاستلام المخبري (Accession)",
            "معرّف عينة الدم أو الفحص الشعاعي",
            "رمز باركود أو معرّف فريد يوضع على أنبوب العينة أو دراسة الأشعة عند السحب لربط النتائج بالمريض بدقة."),

        new("MLLP", "Networking", "Minimal Lower Layer Protocol", 
            "A framing protocol used over raw TCP sockets on port 2575. Wraps HL7 v2 messages with start byte 0x0B and end bytes 0x1C 0x0D.", 
            "HL7 v2",
            "بروتوكول الطبقة الدنيا الأدنى (MLLP)",
            "بروتوكول تأطير الحزم عبر مقابس TCP للشبكات السريرية",
            "بروتوكول تغليف رسائل HL7 v2 عبر مقابس شبكة TCP خام (المنفذ الافتراضي 2575)، يبدأ الرسالة بالبايت 0x0B وينهيها بالبايتين 0x1C 0x0D."),

        new("ACK / NACK", "Networking", "Acknowledgment / Negative Acknowledgment", 
            "Immediate reply packet sent by the receiver over MLLP. AA=Application Accept, AE=Application Error, AR=Application Reject.", 
            "HL7 v2",
            "إشعار الاستلام (ACK / NACK)",
            "رسالة التأكيد الفورية عبر MLLP",
            "حزمة إشعار استلام يرسلها النظام المستقبل فوراً عبر اتصال TCP: حيث AA تعني قبول ناجح، AE خطأ معالجة، و AR رفض الرسالة."),

        new("MSA Segment", "Networking", "Message Acknowledgment Segment", 
            "Contains the ACK code (AA/AE/AR) and references the original message's MSH-10 control ID in MSA-2.", 
            "HL7 v2",
            "قطاع إشعار الرسالة (MSA)",
            "قطاع توثيق حالة استلام الرسالة الأصلية",
            "يحتوي قطاع MSA على كود القبول أو الرفض في MSA-1 ويربط الإشعار بمعرّف الرسالة الأصلية MSH-10 في الحقل MSA-2."),

        new("ER7", "Syntax", "Pipes-and-Hats Syntax", 
            "The default delimited text encoding used by HL7 v2. Uses | (fields), ^ (components), ~ (repetitions), and & (subcomponents).", 
            "HL7 v2",
            "صيغة الأنابيب والقبعات (ER7)",
            "الصيغة النصية المحددة بفواصل لرسائل HL7 v2",
            "ترميز نصي خفيف يعتمد الفواصل: الأنبوب | للحقول، والقبعة ^ للمكونات، والمدّة ~ للتكرار، وعلامة & للمكونات الفرعية."),

        new("LOINC", "Terminology", "Logical Observation Identifiers Names and Codes", 
            "Universal standard for laboratory and clinical test identifiers (e.g. 2339-0 for Glucose, 718-7 for Hemoglobin).", 
            "Both",
            "معيار لوينك (LOINC)",
            "المعيار العالمي لترميز الفحوصات المخبرية والسريرية",
            "قاموس قياسي عالمي موحد لترميز الفحوصات المخبرية والقياسات الحيوية (مثلاً 2339-0 لتحليل الجلوكوز، و 718-7 للهيموغلوبين)."),

        new("SNOMED CT", "Terminology", "Systematized Nomenclature of Medicine -- Clinical Terms", 
            "Comprehensive clinical terminology used for diagnoses, clinical findings, body structures, and procedures.", 
            "FHIR",
            "سنوميد سي تي (SNOMED CT)",
            "المصطلحات السريرية الطبية الممنهجة الشاملة",
            "أشمل معجم للمصطلحات والمفاهيم الطبية عالمياً، يغطي التشخيصات، والأعراض، والإجراءات الجراحية والتشريحية بدقة متناهية."),

        new("ICD-10", "Terminology", "International Classification of Diseases, 10th Revision", 
            "Diagnostic coding standard maintained by the WHO used for billing and mortality tracking (e.g. I21.0 for STEMI).", 
            "Both",
            "التصنيف الدولي للأمراض (ICD-10)",
            "معيار منظمة الصحة العالمية لتصنيف الأمراض والفوترة",
            "ترميز تشخيصي معتمد عالمياً ومن منظمة الصحة العالمية، يُعتمد بشكل رئيسي للفوترة الطبية ومطالبات التأمين وإحصاءات الأوبئة."),

        new("UCUM", "Terminology", "Unified Code for Units of Measure", 
            "Standardized scientific unit syntax (e.g. mg/dL, mmol/L, mm[Hg]) required by FHIR for quantitative observations.", 
            "FHIR",
            "المعيار الموحد لوحدات القياس (UCUM)",
            "الصيغة القياسية للوحدات العلمية في الملاحظات الطبية",
            "صيغة موحدة إلزامية في موارد FHIR لكتابة وحدات القياس العلمية الدقيقة مثل (mg/dL أو mmol/L أو mm[Hg])."),

        new("Bundle", "FHIR Core", "A container collection of multiple FHIR resources", 
            "A first-class FHIR resource used for document packages, search query responses, batches, or atomic database transactions.", 
            "FHIR",
            "حزمة الموارد (Bundle)",
            "حاوية برمجية تجمع موارد FHIR متعددة",
            "مورد قياسي أساسي في FHIR يُستخدم لتجميع عدة موارد سوياً في وثيقة واحدة، أو كنتيجة لبحث، أو كمعاملة قاعدة بيانات ذرية واحدة."),

        new("ResourceReference", "FHIR Core", "A pointer connecting one FHIR resource to another", 
            "References use urn:uuid:... inside bundles or relative URLs like Patient/123 to create a rich clinical knowledge graph.", 
            "FHIR",
            "المرجع المتبادل (ResourceReference)",
            "رابط ذكي يربط مورداً بمورد سريري آخر",
            "روابط تشعبية مثل Patient/123 أو urn:uuid:... تنشئ شبكة مترابطة من البيانات السريرية كالمريض والزيارة والتقرير."),

        new("SMART on FHIR", "Architecture", "OAuth2 & OpenID Connect profile for healthcare apps", 
            "Security and authorization framework that allows third-party web and mobile apps to securely connect to EHR FHIR APIs.", 
            "FHIR",
            "سمارت أون فاير (SMART on FHIR)",
            "بروتوكول الأمان والتفويض لتطبيقات الرعاية الصحية",
            "إطار أمان مبني على OAuth2 و OpenID Connect يسمح للتطبيقات المحمولة بالاتصال الموثوق والآمن بأنظمة السجلات الطبية."),

        new("Interface Engine", "Architecture", "Middleware router and message transformer", 
            "Hospital enterprise service bus (e.g. Mirth Connect, Rhapsody, Cloverleaf) that converts ER7 feeds, routes traffic, and calls FHIR APIs.", 
            "Both",
            "محرك التكامل (Interface Engine)",
            "الوسيط البرمجي لتحويل وتوجيه الرسائل السريرية",
            "خادم وسيط في المستشفيات (مثل Mirth Connect أو Rhapsody) يستقبل رسائل HL7 v2 ويحوّلها ويوجهها ويدمجها مع خدمات FHIR APIs."),

        new("ADT Message", "HL7 v2", "Admission, Discharge, and Transfer", 
            "The core backbone message family in HL7 v2 notifying hospital departments about patient movement (A01=Admit, A03=Discharge, A08=Update).", 
            "HL7 v2",
            "رسائل حركة المرضى (ADT)",
            "رسائل الدخول والخروج والتحويل وتحديث البيانات",
            "عصب التواصل الإداري في المستشفيات لإخطار الأقسام بحركة المريض (A01 للدخول، A03 للخروج، A08 لتحديث البيانات الديموغرافية)."),

        new("ORM Message", "HL7 v2", "Order Message", 
            "Transmits diagnostic or medication orders from EHR to department systems (e.g. ORM^O01).", 
            "HL7 v2",
            "رسائل الطلبات الطبية (ORM)",
            "إرسال طلبات الفحوصات والأشعة والأدوية للأقسام",
            "رسالة إلكترونية تُرسل من شاشة الطبيب إلى المختبر أو قسم الأشعة أو الصيدلية لطلب فحص أو علاج محدد (مثل ORM^O01)."),

        new("ORU Message", "HL7 v2", "Observation Result Unsolicited", 
            "Transmits finalized laboratory or diagnostic results with OBX result segments from LIS to EHR (e.g. ORU^R01).", 
            "HL7 v2",
            "رسائل النتائج السريرية والمخبرية (ORU)",
            "إرسال النتائج المخبرية والشعاعية المعتمدة إلى ملف المريض",
            "رسالة تنقل نتائج التحاليل المخبرية والتقارير الطبية متضمنة قطاعات OBX بالقيم الطبيعية والحالات الحرجة إلى شاشة الطبيب."),

        new("Cardinality", "FHIR Core", "Resource element count rules (0..1, 1..1, 0..*, 1..*)", 
            "Defines whether a field is optional, mandatory, or an array in FHIR specifications and profiles.", 
            "FHIR",
            "قواعد التكرار والإلزام (Cardinality)",
            "تحديد إلزامية وتكرار الحقول في موارد FHIR",
            "توضح ما إذا كان الحقل اختيارياً (0..1) أو إجبارياً (1..1) أو مصفوفة قابلة للتكرار (0..* أو 1..*) في المعايير ومواصفات التخصيص."),

        new("Polymorphic Element", "FHIR Core", "Elements ending in [x] supporting multiple datatypes", 
            "E.g. Observation.value[x] which can be valueQuantity, valueCodeableConcept, valueString, etc.", 
            "FHIR",
            "العنصر متعدد الأشكال ([x])",
            "عناصر في FHIR تدعم أكثر من نوع بيانات",
            "عنصر ينتهي بـ [x] في مواصفة FHIR مثل Observation.value[x] بحيث يمكن أن يأتي كقيمة رقمية valueQuantity أو نصية أو كودية.")
    };
}
