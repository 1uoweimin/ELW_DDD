namespace SearchService.Infrastructure.Options;

public class ElasticSearchOptions
{
    public string Url { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string C_Fingerprint { get; set; } = null!;
}
