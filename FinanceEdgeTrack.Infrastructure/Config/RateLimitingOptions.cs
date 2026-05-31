namespace FinanceEdgeTrack.Infrastructure.Config;

public class RateLimitingOptions
{
    public const string SectionName = "RateLimiting";

    public int AuthenticatedUserLimit { get; set; }

    public int LoginLimit { get; set; }

    public int WindowSeconds { get; set; }
}
