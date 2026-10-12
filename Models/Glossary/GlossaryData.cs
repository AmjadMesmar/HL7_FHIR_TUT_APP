namespace Hl7FhirWorkbench.Models.Glossary;

public record GlossaryItem(string Term, string Category, string ShortDefinition, string LongExplanation, string Standard);

public static class GlossaryData
{
    public static readonly List<GlossaryItem> Items = new()
    {
        new("MRN", "Identity", "Medical Record Number", "A unique identifier assigned by a specific hospital to identify a patient across visits. Maps to PID-3 in HL7 v2 and Patient.identifier in FHIR.", "Both"),
        new("FIN / Visit #", "Encounter", "Financial Account / Encounter Number", "Identifies a specific clinical visit or hospital admission. Maps to PV1-19 in v2 and Encounter.identifier in FHIR.", "Both"),
        new("Placer Order Number", "Orders", "Order ID assigned by the requesting physician/EHR", "The tracking ID assigned by the EHR or doctor when ordering a lab or radiology test. Sent in ORC-2 / OBR-2 in v2; ServiceRequest.identifier in FHIR.", "HL7 v2"),
        new("Filler Order Number", "Orders", "Order ID assigned by the performing laboratory/system", "The tracking ID assigned by the lab instrument or LIS performing the test. Sent in ORC-3 / OBR-3 in v2; DiagnosticReport.identifier in FHIR.", "HL7 v2"),
        new("Accession Number", "Orders", "Unique specimen identifier in laboratory/imaging", "A unique specimen or scan identifier generated when blood is drawn or an imaging study is registered. Sent in OBR-20 in v2.", "Both"),
        new("MLLP", "Networking", "Minimal Lower Layer Protocol", "A framing protocol used over raw TCP sockets on port 2575. Wraps HL7 v2 messages with start byte 0x0B and end bytes 0x1C 0x0D.", "HL7 v2"),
        new("ACK / NACK", "Networking", "Acknowledgment / Negative Acknowledgment", "Immediate reply packet sent by the receiver over MLLP. AA=Application Accept, AE=Application Error, AR=Application Reject.", "HL7 v2"),
        new("MSA Segment", "Networking", "Message Acknowledgment Segment", "Contains the ACK code (AA/AE/AR) and references the original message's MSH-10 control ID in MSA-2.", "HL7 v2"),
        new("ER7", "Syntax", "Pipes-and-Hats Syntax", "The default delimited text encoding used by HL7 v2. Uses | (fields), ^ (components), ~ (repetitions), and & (subcomponents).", "HL7 v2"),
        new("LOINC", "Terminology", "Logical Observation Identifiers Names and Codes", "Universal standard for laboratory and clinical test identifiers (e.g. 2339-0 for Glucose, 718-7 for Hemoglobin).", "Both"),
        new("SNOMED CT", "Terminology", "Systematized Nomenclature of Medicine -- Clinical Terms", "Comprehensive clinical terminology used for diagnoses, clinical findings, body structures, and procedures.", "FHIR"),
        new("ICD-10", "Terminology", "International Classification of Diseases, 10th Revision", "Diagnostic coding standard maintained by the WHO used for billing and mortality tracking (e.g. I21.0 for STEMI).", "Both"),
        new("UCUM", "Terminology", "Unified Code for Units of Measure", "Standardized scientific unit syntax (e.g. mg/dL, mmol/L, mm[Hg]) required by FHIR for quantitative observations.", "FHIR"),
        new("Bundle", "FHIR Core", "A container collection of multiple FHIR resources", "A first-class FHIR resource used for document packages, search query responses, batches, or atomic database transactions.", "FHIR"),
        new("ResourceReference", "FHIR Core", "A pointer connecting one FHIR resource to another", "References use urn:uuid:... inside bundles or relative URLs like Patient/123 to create a rich clinical knowledge graph.", "FHIR"),
        new("SMART on FHIR", "Architecture", "OAuth2 & OpenID Connect profile for healthcare apps", "Security and authorization framework that allows third-party web and mobile apps to securely connect to EHR FHIR APIs.", "FHIR"),
        new("Interface Engine", "Architecture", "Middleware router and message transformer", "Hospital enterprise service bus (e.g. Mirth Connect, Rhapsody, Cloverleaf) that converts ER7 feeds, routes traffic, and calls FHIR APIs.", "Both"),
        new("ADT Message", "HL7 v2", "Admission, Discharge, and Transfer", "The core backbone message family in HL7 v2 notifying hospital departments about patient movement (A01=Admit, A03=Discharge, A08=Update).", "HL7 v2"),
        new("ORM Message", "HL7 v2", "Order Message", "Transmits diagnostic or medication orders from EHR to department systems (e.g. ORM^O01).", "HL7 v2"),
        new("ORU Message", "HL7 v2", "Observation Result Unsolicited", "Transmits finalized laboratory or diagnostic results with OBX result segments from LIS to EHR (e.g. ORU^R01).", "HL7 v2"),
        new("Cardinality", "FHIR Core", "Resource element count rules (0..1, 1..1, 0..*, 1..*)", "Defines whether a field is optional, mandatory, or an array in FHIR specifications and profiles.", "FHIR"),
        new("Polymorphic Element", "FHIR Core", "Elements ending in [x] supporting multiple datatypes", "E.g. Observation.value[x] which can be valueQuantity, valueCodeableConcept, valueString, etc.", "FHIR")
    };
}
