namespace Hl7FhirWorkbench.Models.Mapping;

public record FieldMappingLink(
    string SourceField,
    string SourceLabel,
    string SourceValue,
    string TargetFhirPath,
    string TargetLabel,
    string TargetValue,
    string ConceptExplanation,
    string? InteropGotcha = null,
    string? ConceptExplanationAr = null,
    string? InteropGotchaAr = null,
    string? SourceLabelAr = null,
    string? TargetLabelAr = null
);

public class TransformationResult
{
    public Hl7.Fhir.Model.Resource FhirResource { get; set; } = null!;
    public string FhirJson { get; set; } = string.Empty;
    public List<FieldMappingLink> Mappings { get; set; } = new();
    public List<string> ArchitecturalNotes { get; set; } = new();
}
