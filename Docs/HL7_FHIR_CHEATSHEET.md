# HL7 v2 & FHIR Developer Cheat Sheet

A rapid-reference guide for healthcare software engineers working with HL7 v2 ER7 messages and FHIR R4 resources.

---

## 1. HL7 v2 Segment Quick Reference

| Segment | Full Name | Key Fields to Inspect |
| :--- | :--- | :--- |
| **MSH** | Message Header | `MSH-1` (Delim: `\|`), `MSH-2` (`^~\&`), `MSH-9` (Type^Event e.g. `ADT^A01`), `MSH-10` (Control ID) |
| **EVN** | Event Type | `EVN-2` (Recorded Event Timestamp) |
| **PID** | Patient Identification | `PID-3` (MRN Identifier list), `PID-5` (Patient Name), `PID-7` (DOB), `PID-8` (Sex), `PID-11` (Address) |
| **PV1** | Patient Visit | `PV1-2` (Class: `I`/`O`/`E`), `PV1-3` (Assigned Bed/Room), `PV1-7` (Attending Doc), `PV1-44`/`45` (Admit/Discharge) |
| **ORC** | Common Order | `ORC-1` (Control Code: `NW`=New, `CA`=Cancel), `ORC-2` (Placer Order #), `ORC-7` (Priority: `STAT`) |
| **OBR** | Observation Request | `OBR-4` (Test Panel LOINC Code), `OBR-25` (Status: `F`=Final, `P`=Prelim) |
| **OBX** | Observation Result | `OBX-2` (Type: `NM`/`ST`), `OBX-3` (LOINC Code), `OBX-5` (Value), `OBX-6` (Units), `OBX-8` (Flag: `H`/`L`) |
| **DG1** | Diagnosis | `DG1-3` (Diagnosis Code e.g. ICD-10 `I21.0`), `DG1-6` (Diagnosis Type: `A`=Admitting, `F`=Final) |
| **IN1** | Insurance Policy | `IN1-2` (Plan ID), `IN1-4` (Payer Name), `IN1-36` (Policy Number) |

---

## 2. HL7 v2 Delimiters & Escape Sequences

* **Delimiters:**
  * `|` &rarr; Field Separator
  * `^` &rarr; Component Separator
  * `~` &rarr; Repetition Separator
  * `\` &rarr; Escape Character
  * `&` &rarr; Subcomponent Separator
* **Escapes inside text:**
  * `\F\` &rarr; Literal `|`
  * `\S\` &rarr; Literal `^`
  * `\R\` &rarr; Literal `~`
  * `\E\` &rarr; Literal `\`
  * `\T\` &rarr; Literal `&`
  * `\.br\` &rarr; Hard line break in clinical notes

---

## 3. Core FHIR R4 Resources

| FHIR Resource | Purpose | Equivalent v2 Segment |
| :--- | :--- | :--- |
| **Patient** | Demographics, identity, contacts | `PID` |
| **Encounter** | Inpatient stay, outpatient visit | `PV1` |
| **Observation** | Single measurement, lab result, vital | `OBX` |
| **DiagnosticReport** | Multi-test panel, radiology report | `OBR` |
| **Condition** | Diagnosis, health problem, allergy | `DG1` |
| **ServiceRequest** | Lab / Radiology order from doctor | `ORC` + `OBR` |
| **Bundle** | Collection or Transaction envelope | Complete message feed |

---

## 4. FHIR REST Search Quick Reference

* **Read:** `GET /Patient/123`
* **Search by Name:** `GET /Patient?name=Smith`
* **Search by DOB:** `GET /Patient?birthdate=ge1980-01-01`
* **Compound Join (`_include`):** `GET /DiagnosticReport?_include=DiagnosticReport:result`
* **Chained Search:** `GET /Observation?subject:Patient.name=Smith`
* **Pagination limit:** `GET /Patient?_count=10`
