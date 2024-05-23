using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

string azureOpenAiEndpoint = configuration["AZURE_OPENAI_ENDPOINT"] ?? string.Empty;
string azureOpenAiKey = configuration["AZURE_OPENAI_API_KEY"] ?? string.Empty;
string deploymentName = configuration["AZURE_OPENAI_DEPLOYMENT_ID"] ?? string.Empty;
string searchEndpoint = configuration["AZURE_AI_SEARCH_ENDPOINT"] ?? string.Empty;
string searchKey = configuration["AZURE_AI_SEARCH_API_KEY"] ?? string.Empty;
string searchIndex = configuration["AZURE_AI_SEARCH_INDEX"] ?? string.Empty;

OpenAIClient client = new(new Uri(azureOpenAiEndpoint), new AzureKeyCredential(azureOpenAiKey));
SearchClient searchClient = new(new Uri(searchEndpoint), searchIndex, new AzureKeyCredential(searchKey));

string PromptInput()
{
    while (true)
    {
        Console.Write("MIMS Specialty AI Bot: Ask me any healthcare questions.\nUser: ");
        var input = Console.ReadLine();

        if (!string.IsNullOrEmpty(input)) return input;

        Console.WriteLine("Please enter a valid input.");
    }
}

while (true)
{
    int size = 10;
    int skip = 0;

    var input = PromptInput();
    await ChatWithYourDataAsync(input);
    await AiSearchAsync(input, size, skip);
}


async Task ChatWithYourDataAsync(string input)
{
    //input += " Please give a summary in 3 lines and list down the answer in bulletins and each line shouldn't be more than 30 words.";
    var chatCompletionsOptions = new ChatCompletionsOptions()
    {
        DeploymentName = deploymentName,
        Messages =
        {
            new ChatRequestUserMessage(input),
        },
        AzureExtensionsOptions = new AzureChatExtensionsOptions()
        {
            Extensions =
            {
                new AzureSearchChatExtensionConfiguration()
                {
                    SearchEndpoint = new Uri(searchEndpoint),
                    Authentication = new OnYourDataApiKeyAuthenticationOptions(searchKey),
                    IndexName = searchIndex,
                },
            }
        }
    };
    await foreach (StreamingChatCompletionsUpdate chatUpdate in client.GetChatCompletionsStreaming(chatCompletionsOptions))
    {
        if (chatUpdate.Role.HasValue)
        {
            //await Console.Out.WriteAsync(($"{chatUpdate.Role.Value.ToString().ToUpperInvariant()}: ");
            await Console.Out.WriteLineAsync("\n\n Generative AI:");
        }
        if (!string.IsNullOrEmpty(chatUpdate.ContentUpdate))
        {
            await Console.Out.WriteAsync(chatUpdate.ContentUpdate);
        }
    }
}

async Task AiSearchAsync(string input, int size, int skip)
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
        Size = size,
        Skip = skip
    };
    options.Select.Add(nameof(Article.Title));
    options.Select.Add(nameof(Article.LegacyUrl));


    await Console.Out.WriteLineAsync("\n\n\n");
    await Console.Out.WriteLineAsync("Generative AI:");

    SearchResults<Article> response = await searchClient.SearchAsync<Article>(input, options);
    foreach (SearchResult<Article> result in response.GetResults())
    {
        await Console.Out.WriteLineAsync($"Title: {result.Document.Title}");
        await Console.Out.WriteLineAsync($"Website: {result.Document.LegacyUrl}");
        await Console.Out.WriteLineAsync();
    }
}

async Task GetCitations(ChatCompletionsOptions options)
{
    Response<ChatCompletions> response = client.GetChatCompletions(options);
    ChatResponseMessage responseMessage = response.Value.Choices[0].Message;
    Console.WriteLine($"Context information (e.g. citations) from chat extensions:");
    Console.WriteLine("===");
    foreach (var citation in responseMessage.AzureExtensionsContext.Citations)
    {
        await Console.Out.WriteLineAsync(citation.Content);
    }
    Console.WriteLine("===");
}
