# HL7 v2 & FHIR R4 Interactive Educational Workbench

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Blazor Server](https://img.shields.io/badge/Blazor-Server-512BD4?logo=blazor)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![Live Demo](https://img.shields.io/badge/Live_Demo-hl7--fhir--tut--app.onrender.com-00C7B7?logo=render&logoColor=white)](https://hl7-fhir-tut-app.onrender.com/)
[![HL7 v2](https://img.shields.io/badge/HL7-v2.3_|_v2.5_|_v2.7-005691)](https://www.hl7.org/)
[![FHIR R4](https://img.shields.io/badge/FHIR-Release_4.0.1-FF5A36?logo=fire)](https://hl7.org/fhir/R4/)
[![Firely SDK](https://img.shields.io/badge/Firely_SDK-6.6.0-orange)](https://fire.ly/)
[![Author](https://img.shields.io/badge/Author-Amjad_Mesmar-005691?logo=github)](https://github.com/AmjadMesmar)
[![Role](https://img.shields.io/badge/Role-Senior_Software_Engineer-512BD4)]()
[![Zero External DB](https://img.shields.io/badge/Database-Zero_Config_In--Memory-success)]()
[![Hardware Friendly](https://img.shields.io/badge/Memory_Footprint-~140_MB_RAM-brightgreen)]()

> 🌐 **Live Interactive App**: [https://hl7-fhir-tut-app.onrender.com/](https://hl7-fhir-tut-app.onrender.com/)

An interactive, zero-overhead training portal and clinical interoperability workbench designed for hospital integration engineers, software developers, and QA teams. Built with **Blazor Server** on **.NET 10**, this application runs natively in-memory without requiring heavy database installations, Docker containers, or high-end workstations.

---

## 🏥 Why This Workbench Exists

In modern healthcare IT, systems are rarely built on a single standard:
* **Legacy EHRs, LIS, and RIS** (Epic, Cerner, Meditech, custom lab instruments) communicate primarily via **HL7 v2** pipes-and-hats (`ER7`) transmitted over raw TCP sockets wrapped in **MLLP** (Minimal Lower Layer Protocol).
* **Modern mobile apps, cloud gateways, analytics, and FHIR APIs** use **HL7 FHIR R4** (RESTful JSON/XML over HTTPS).

Connecting these two worlds requires bridging two radically different philosophies: **event-driven segment trees** vs **resource-oriented graph REST models**. This workbench is an all-in-one educational platform and developer toolkit designed to demystify both standards with hands-on, live interactive tools.

---

## 🚀 Key Features & Interactive Labs

The workbench includes **14 interactive modules** categorized into three core engineering areas:

### 1. Curriculum & Docs
| Tool | Route | What It Does |
| :--- | :--- | :--- |
| **Overview & Paths** | `/` | 3-Day Mastery Pathway checklist, interactive learning journey, scenario selector, and architectural comparison table. |
| **Master Curriculum** | `/get-started` | 1989 Historical Hardware Constraints infographic, The Rosetta Stone Concept Translation Matrix (v2 vs FHIR vs Modern equivalents), Data body comparisons (JSON vs XML vs ER7 vs RDF), Delimiters & Escape characters, 5 surgery-level messages (`ADT^A01`, `A08`, `A03`, `ORM^O01`, `ORU^R01`), 24h patient journey simulator, and knowledge check quiz. |
| **Living Documentation** | `/docs` | In-app Markdown document reader powered by `Markdig`, rendering version-controlled markdown guides directly from the `Docs/` directory. |
| **Jargon Glossary** | Global Modal | Global header search modal with 22+ clinical/interop dictionary definitions across Core Protocols, v2 Standards, FHIR Terminology, and Hospital IT. |

### 2. Interactive Labs
| Tool | Route | What It Does |
| :--- | :--- | :--- |
| **Hospital Flow Simulator** | `/hospital-flow` | Interactive multi-system hospital sequence simulator (EHR, Integration Engine, LIS, FHIR Gateway) with step-by-step trace inspection and wire formats. |
| **Interop Bridge** | `/bridge` | Real-time bidirectional HL7 v2 to FHIR R4 transformation engine with interactive field-level trace inspection and Firely JSON output. |
| **HL7 Inspector & Linter** | `/hl7-inspector` | Visual ER7 syntax deconstructor + Visual Delimiter Microscope (coordinate inspector) + Form-based ER7 Message Builder with live preview + live syntax linter. |
| **FHIR Resource Explorer** | `/fhir-explorer` | Interactive Firely SDK serialization sandbox, Resource Reference Graph visualizer, US Core Conformance Validator, and datatypes guide. |
| **REST Playground** | `/rest-playground` | Visual FHIR Search Builder (`_include`, comparator prefixes, summary modes) with live Plain-English Query Explainer and HAPI FHIR query sandbox. |

### 3. Engineering & QA Tools
| Tool | Route | What It Does |
| :--- | :--- | :--- |
| **Synthetic Data Factory** | `/data-factory` | Generates realistic clinical scenarios (Diabetic Outpatient, Acute Cardiac STEMI, Trauma ICU) in matched ER7 and FHIR formats with 1-click clipboard copy and `.hl7` / `.json` downloads. |
| **MLLP & ACK Simulator** | `/mllp-simulator` | Interactive TCP 3-way handshake & MLLP framing byte stepper (`<0x0B>` ... `<0x1C><0x0D>`) plus automated `AA`, `AE`, and `AR` acknowledgment generator. |
| **Terminology Lookup** | `/terminology` | Searchable clinical dictionary covering LOINC lab codes, ICD-10 clinical diagnoses, and HL7 administrative tables with bidirectional v2-to-FHIR concept mapping. |
| **C# Code Recipes** | `/code-recipes` | Battle-tested, copy-pasteable C# .NET 10 recipes (MLLP listener, Firely serializer, HAPI REST client, custom v2 parser) with 1-click clipboard copy. |
| **xUnit Test Generator** | `/test-generator` | Generates ready-to-run C# unit test classes for ER7 delimiter parsing, PID extraction, FHIR validation, and edge cases with copy and `.cs` download. |
| **Hospital War Room** | `/war-room` | 5 real-world hospital integration crisis scenarios + Interactive Fix-the-Bug Challenge Arena with 5 puzzles, instant validation, and hint assistance. |
| **Tech Stack & Specs** | `/project-info` | Engineering specs, NuGet bill of materials, rationale behind each dependency, and architectural justifications for low-spec hardware. |

---

## 🛠️ Architecture & Tech Stack

```
HL7_FHIR_TUT_APP/
├── Hl7FhirWorkbench.csproj         # Target: net10.0, Packages: Hl7.Fhir.R4 6.6.0, Markdig 1.4.0
├── Program.cs                      # Blazor Server setup + Dependency Injection
├── Models/
│   ├── Hl7V2/
│   │   ├── Hl7V2Models.cs          # ER7 hierarchy: Message, Segment, Field, Component
│   │   ├── Hl7V2Parser.cs          # Lightweight zero-allocation ER7 parser + dictionary
│   │   └── Presets/
│   │       └── ClinicalPresets.cs  # 4 real-world clinical scenarios
│   ├── Fhir/
│   │   └── FhirService.cs          # Firely SDK serializer, deserializer, and HAPI client
│   ├── Mapping/
│   │   ├── MappingModels.cs        # FieldMappingLink & TransformationResult records
│   │   └── Hl7ToFhirMapper.cs      # Core converter: PID->Patient, PV1->Encounter, OBX->Observation
│   └── Glossary/
│       └── GlossaryData.cs         # Clinical & Interop dictionary with category classification
├── Docs/                           # Living documentation (Docs-as-Code)
│   ├── WALKTHROUGH.md              # Test plan & verification report
│   ├── ARCHITECTURE_RATIONALE.md   # Deep architectural explanations of tech stack
│   └── HL7_FHIR_CHEATSHEET.md      # Engineer rapid-reference cheatsheet
└── Components/
    ├── Layout/
    │   ├── MainLayout.razor        # Responsive layout with healthcare branding (no horizontal scroll)
    │   ├── NavMenu.razor           # Interactive navigation sidebar with 3 organized sections
    │   └── GlossaryModal.razor     # Global search modal for clinical interoperability terms
    └── Pages/                      # 14 interactive pages & developer tools
```

### Architectural Decisions
* **.NET 10 + Blazor Server:**
  Runs C# on the server with lightweight DOM updates via WebSockets (SignalR). No large WebAssembly bundle downloads (~30MB+), resulting in near-instant initial page loads and a minimal RAM footprint (~140 MB).
* **Official Firely SDK (`Hl7.Fhir.R4` 6.6.0):**
  Uses the official, standards-compliant .NET FHIR SDK for rich typing, schema validation, and serialization.
* **Zero-Allocation Custom HL7 v2 Parser:**
  A purpose-built `ReadOnlySpan<char>` / string tokenizer that deconstructs ER7 messages in microseconds without heavy external dependencies.
* **Markdig (`1.4.0`):**
  Powers the in-app "Docs-as-Code" viewer, reading version-controlled Markdown documents directly from disk without database overhead.

---

## 🚦 Quickstart & Running Locally

### Prerequisites
* [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (Version `10.0.100` or later)

### Run the Application
1. Clone the repository:
   ```powershell
   git clone https://github.com/AmjadMesmar/HL7_FHIR_TUT_APP.git
   cd HL7_FHIR_TUT_APP
   ```

2. Run the application:
   ```powershell
   dotnet run --launch-profile http
   ```

3. Open your browser and navigate to:
   ```
   http://localhost:5199
   ```

*(All features work completely offline; the REST Playground automatically falls back to realistic mock responses if external internet access is unavailable).*

---

## 📋 Included Clinical Scenarios

1. **Patient Registration (`ADT^A04` / `ADT^A28`)**
   - Source: `PID` segment with MRN, assigning authority, name, DOB, gender, address, phone.
   - Target: FHIR `Patient` resource with composite identifiers and contact points.
2. **Inpatient Admission (`ADT^A01`)**
   - Source: `PID` demographics + `PV1` visit segment with patient class `I` (Inpatient) and bed location (`ICU_CARDIO^ROOM_402^BED_B`).
   - Target: FHIR `Bundle` containing `Patient` + `Encounter`.
3. **Patient Discharge (`ADT^A03`)**
   - Source: `PID` + `PV1` with discharge timestamp and disposition.
   - Target: FHIR `Encounter` with `status = finished` and discharge disposition.
4. **Laboratory Results (`ORU^R01`)**
   - Source: `PID` + `OBR` (BMP Panel) + 3 `OBX` segments (Glucose, Potassium, Sodium) with LOINC codes, reference ranges, and abnormal flags (`H`).
   - Target: FHIR `Bundle` with `Patient`, `DiagnosticReport`, and linked `Observation` resources.

---

## 👨‍💻 Author & Engineering Lead

**Amjad Mesmar**  
*Senior Software Engineer*  
* GitHub: [@AmjadMesmar](https://github.com/AmjadMesmar)

Senior Software Engineer with extensive experience designing and deploying high-stakes systems across healthcare interoperability (HL7 v2 / FHIR R4), enterprise ERPs, commercial platforms, and governmental infrastructure. Core expertise in system architecture, security & cryptography (AES-256, ACLs, multi-tenant licensing), and full-lifecycle engineering from requirements analysis to production IIS deployment. Outside enterprise systems, actively passionate about reverse engineering, systems architecture, and video game modding (*Mount & Blade II: Bannerlord*, *Stardew Valley*, *Age of Empires II: DE*).

---

## 📄 License & Attribution

This project is open-source and created for educational, training, and department onboarding purposes.
* **Architect & Developer:** [Amjad Mesmar](https://github.com/AmjadMesmar) (Senior Software Engineer).
* HL7® and FHIR® are registered trademarks of [Health Level Seven International](https://www.hl7.org/).
