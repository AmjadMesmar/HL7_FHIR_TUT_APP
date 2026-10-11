namespace Hl7FhirWorkbench.Models.Hl7V2;

public class Hl7Message
{
    public string RawMessage { get; set; } = string.Empty;
    public char FieldDelimiter { get; set; } = '|';
    public char ComponentDelimiter { get; set; } = '^';
    public char RepetitionDelimiter { get; set; } = '~';
    public char EscapeCharacter { get; set; } = '\\';
    public char SubcomponentDelimiter { get; set; } = '&';
    public string MessageType { get; set; } = string.Empty;
    public string TriggerEvent { get; set; } = string.Empty;
    public string MessageControlId { get; set; } = string.Empty;
    public List<Hl7Segment> Segments { get; set; } = new();

    public Hl7Segment? GetSegment(string segmentId) =>
        Segments.FirstOrDefault(s => s.SegmentId.Equals(segmentId, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<Hl7Segment> GetSegments(string segmentId) =>
        Segments.Where(s => s.SegmentId.Equals(segmentId, StringComparison.OrdinalIgnoreCase));
}

public class Hl7Segment
{
    public int LineNumber { get; set; }
    public string SegmentId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string RawContent { get; set; } = string.Empty;
    public List<Hl7Field> Fields { get; set; } = new();

    public Hl7Field? GetField(int index) =>
        Fields.FirstOrDefault(f => f.Index == index);

    public string GetValue(int fieldIndex, int componentIndex = 1)
    {
        var field = GetField(fieldIndex);
        if (field == null) return string.Empty;
        if (componentIndex <= 1 && field.Components.Count == 0) return field.Value;
        var comp = field.Components.FirstOrDefault(c => c.Index == componentIndex);
        return comp?.Value ?? (componentIndex == 1 ? field.Value : string.Empty);
    }
}

public class Hl7Field
{
    public int Index { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Hl7Component> Components { get; set; } = new();
    public List<string> Repetitions { get; set; } = new();
}

public class Hl7Component
{
    public int Index { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
