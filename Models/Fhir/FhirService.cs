using System.Net.Http.Headers;
using System.Text.Json;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace Hl7FhirWorkbench.Models.Fhir;

public class FhirService
{
    private readonly HttpClient _httpClient;
    private readonly FhirJsonSerializer _serializer;
    private readonly FhirJsonDeserializer _deserializer;

    public FhirService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _serializer = new FhirJsonSerializer();
        _deserializer = new FhirJsonDeserializer();
    }

    public string SerializeToJson(Resource resource)
    {
        try
        {
            var rawJson = _serializer.SerializeToString(resource);
            using var doc = JsonDocument.Parse(rawJson);
            return JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"{{\"error\": \"Serialization failed: {ex.Message}\"}}";
        }
    }

    public Resource? ParseFromJson(string json)
    {
        try
        {
            return _deserializer.Deserialize<Resource>(json);
        }
        catch
        {
            return null;
        }
    }

    public async Task<(bool Success, string ResultJson, string? ErrorMessage)> QueryPublicFhirServerAsync(
        string resourceType = "Patient",
        string searchParam = "name=Smith",
        string baseServerUrl = "https://hapi.fhir.org/baseR4")
    {
        try
        {
            var url = $"{baseServerUrl.TrimEnd('/')}/{resourceType}?{searchParam}&_count=5";
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/fhir+json"));

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(8));
            var response = await _httpClient.SendAsync(request, cts.Token);

            if (!response.IsSuccessStatusCode)
            {
                return (false, string.Empty, $"Server returned HTTP {response.StatusCode}: {response.ReasonPhrase}");
            }

            var content = await response.Content.ReadAsStringAsync(cts.Token);
            
            // Format JSON cleanly
            try
            {
                using var doc = JsonDocument.Parse(content);
                var prettyJson = JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions { WriteIndented = true });
                return (true, prettyJson, null);
            }
            catch
            {
                return (true, content, null);
            }
        }
        catch (OperationCanceledException)
        {
            return (false, string.Empty, "Request timed out after 8 seconds. The public sandbox server may be under heavy load or offline.");
        }
        catch (Exception ex)
        {
            return (false, string.Empty, $"Network/Connection Error: {ex.Message}");
        }
    }
}
