namespace QuantumTestSuite.API.Models;

public class GitHubUser
{
    public string Login { get; set; } = string.Empty;
    public string HtmlUrl { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
}

public class GitHubSearchResponse
{
    public int TotalCount { get; set; }
    public bool IncompleteResults { get; set; }
    public List<GitHubUser> Items { get; set; } = new();
}
