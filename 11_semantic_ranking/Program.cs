using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Configuration;


var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

string searchEndpoint = configuration["AZURE_AI_SEARCH_ENDPOINT"] ?? string.Empty;
string searchKey = configuration["AZURE_AI_SEARCH_API_KEY"] ?? string.Empty;
string searchIndex = configuration["AZURE_AI_SEARCH_INDEX"] ?? string.Empty;

Uri serviceEndpoint = new(searchEndpoint);
AzureKeyCredential credential = new(searchKey);
SearchClient searchClient = new(serviceEndpoint, searchIndex, credential);

while (true)
{
    await Console.Out.WriteAsync("\n\nMIMS AI: What news and updates are you looking for today?\nUser: ");
    string input = Console.ReadLine()!;
    await AiSearchAsync(input);
}


async Task AiSearchAsync(string input)
{
    var options = new SearchOptions()
    {
        QueryType = SearchQueryType.Semantic,
        SemanticSearch = new SemanticSearchOptions
        {
            SemanticConfigurationName = "semantic-config",
            QueryCaption = new QueryCaption(QueryCaptionType.Extractive)
        },
        IncludeTotalCount = true,
        Size = 5,
    };
    options.Select.Add(nameof(Article.Title));
    options.Select.Add(nameof(Article.LegacyUrl));

    SearchResults<Article> response = await searchClient.SearchAsync<Article>(input, options);
    foreach(SearchResult<Article> result in response.GetResults())
    {
        await Console.Out.WriteLineAsync($"Title: {result.Document.Title}");
        await Console.Out.WriteLineAsync($"LegacyUrl: {result.Document.LegacyUrl}");
        await Console.Out.WriteLineAsync();
    }
}

class Article
{
    public string Title { get; set; }
    public string Intro { get; set; }
    public string Content { get; set; }
    public string LegacyUrl { get; set; }
}