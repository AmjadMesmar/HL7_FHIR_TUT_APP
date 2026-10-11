# System Architecture & Technology Selection Rationale

This document provides the in-depth architectural justification for the technologies, frameworks, and packages chosen for the **HL7 & FHIR Educational Workbench**.

---

## 1. Why `Hl7.Fhir.R4` (Firely .NET SDK 6.6.0)?

### The Alternative: Custom POCO Classes
Why not simply write custom C# classes like `class Patient { public string Name; }`?
* **145+ Complex Resources:** FHIR defines over 145 distinct resources, each with deeply nested, strongly typed hierarchies.
* **Polymorphic Elements (`choice[x]`):** In FHIR, an element like `Observation.value[x]` can take 11 distinct types: `valueQuantity`, `valueCodeableConcept`, `valueString`, `valueBoolean`, `valueRange`, `valueRatio`, `valueSampledData`, `valueTime`, `valueDateTime`, or `valuePeriod`. Handling this manually in C# with `System.Text.Json` requires complex custom polymorphism converters.
* **Strict Temporal Parsing:** FHIR dates must support partial dates (`1985`, `1985-06`, `1985-06-14`) and ISO timestamps (`1985-06-14T09:30:00Z`). Standard .NET `DateTime` throws exceptions on partial dates.
* **Firely SDK Solution:** Firely is the official .NET SDK maintained in direct collaboration with HL7 International. It handles schema validation, resource cloning, JSON/XML parsing, and REST client abstractions out of the box.

---

## 2. Why `Hl7.Fhir.Base` & `Hl7.Fhir.Conformance`?

* **`Hl7.Fhir.Base`:** Powers the low-level object model, resource reflection, and streaming serializers for both JSON and XML.
* **`Hl7.Fhir.Conformance`:** In enterprise hospital systems, raw FHIR resources are rarely accepted without checking **Profiles** (e.g. US Core, International Patient Summary). `Conformance` enables runtime inspection of `StructureDefinition` metadata to verify that a resource satisfies mandatory hospital rules.

---

## 3. Why `Fhir.Metrics` (v1.4.0)?

* In clinical medicine, a bare decimal number without a verified unit is dangerous (e.g. 100 mg vs 100 mcg is a 1,000x overdose).
* `Fhir.Metrics` provides .NET with the **UCUM (Unified Code for Units of Measure)** standard. It validates that units are syntactically and dimensionally correct and allows automated metric conversions (e.g., converting `mg/dL` to `mmol/L` for international blood glucose feeds).

---

## 4. Why Blazor Server (SignalR) instead of Blazor WebAssembly?

| Consideration | Blazor WebAssembly (WASM) | Blazor Server (SignalR) - Selected |
| :--- | :--- | :--- |
| **Initial Download** | 15–30 MB (.NET runtime + assemblies) | **< 100 KB** (standard HTML/CSS payload) |
| **Browser RAM** | 300–600 MB (in-browser mono/JIT runtime) | **< 30 MB** (pure browser DOM diffs) |
| **Server/PC Footprint** | None (client-side execution) | **~140 MB total** (very low overhead) |
| **Startup Speed** | 3–8 seconds (compiling WebAssembly) | **< 50 milliseconds** (instant SignalR circuit) |
| **Hardware Fit** | Sluggish on older/2013 dual-core CPUs | **Silky smooth on low-end hardware** |

Blazor Server executes entirely in local C# memory and streams binary diffs over a lightweight WebSocket circuit. For a low-end laptop or internal hospital network, Blazor Server provides instant responsiveness with zero browser overhead.

---

## 5. Why Custom ER7 Parser instead of NHapi?

* **The Problem with NHapi:** `NHapi` is a 15-year-old C# port of the Java HAPI v2 library. It pulls in over **40 MB of DLLs** covering every single HL7 version from 2.1 through 2.8, with verbose Java-style factory classes.
* **The Custom Parser Advantage:** A clean, 150-line pure C# parser demonstrates how ER7 actually works at the byte level:
  * Delimiter discovery in `MSH-1` and `MSH-2`
  * Splitting by Field (`|`), Component (`^`), and Repetition (`~`)
  * Immediate segment indexing (`PID-3.1`, `OBX-5`)
  * Zero external NuGet dependencies, zero assembly bloat.

---

## 6. Why Zero Database (Pure In-Memory Architecture)?

* **Message-Oriented Nature:** HL7 v2 and FHIR are **wire exchange protocols**—they represent data in motion between systems.
* **Portability:** Adding a local SQL Server or SQLite database introduces migrations, file locks, disk thrashing, and connection string configurations.
* **Instant Department Onboarding:** Because the workbench runs in-memory with pre-configured clinical presets, any developer can clone the repo and immediately run `dotnet run` without needing database installation or container configuration.
