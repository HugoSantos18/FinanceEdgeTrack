using System.Text.Json;

namespace FinanceEdgeTrackAPI.Error;

public class ErrorDetails
{
    public string? MessageError { get; set; }
    public int? StatusCode { get; set; }
    public string? Trace { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
