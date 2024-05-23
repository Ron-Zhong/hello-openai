### Azure OpenAI C# Samples

#### How to generate images with Azure OpenAI DALL-E 3 model?
```cs
OpenAIClient client = new (new Uri(azureOpenAiEndpoint), new AzureKeyCredential(azureOpenAiKey));
var imageGenerationOptions = new ImageGenerationOptions
{
    Prompt = prompt,
    DeploymentName = deploymentName,
    Size = new ImageSize("1792x1024"),
    ImageCount = 1
};
var response = await client.GetImageGenerationsAsync(imageGenerationOptions);
Console.WriteLine(response.Value.Data[0].Url.AbsoluteUri);
```
##### Sample:
![chrome-capture-2024-4-23 (1)](https://github.com/ron-zhong/azure-openai/assets/43414651/15f468e5-cff8-4cd6-8b76-2dff3d0e490e)



#### How to chat with your data Azure OpenAI's gpt-3.5-turbo model?
```cs
await foreach (StreamingChatCompletionsUpdate chatUpdate in client.GetChatCompletionsStreaming(chatCompletionsOptions))
{
    if (chatUpdate.Role.HasValue)
    {
        await Console.Out.WriteAsync(($"{chatUpdate.Role.Value.ToString().ToUpperInvariant()}: ");
    }
    if (!string.IsNullOrEmpty(chatUpdate.ContentUpdate))
    {
        await Console.Out.WriteAsync(chatUpdate.ContentUpdate);
    }
}
```
##### Sample:
![chat completion](https://github.com/ron-zhong/azure-openai/assets/43414651/1de71dd4-4e77-4c0a-b78e-f53c6d4aa9f4)


#### How to do semantic search with Azure AI Search?
```cs
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
```
##### Sample:
![Semantic AI Search](https://github.com/ron-zhong/azure-openai/assets/43414651/a65558e5-8955-4718-ba68-3ee1a3506226)


With Azure SDK and Azure.AI.OpenAI Nuget package, it makes .NET developers easy to work on GenAI software development.

Hope you will enjoy my code samples. Cheers!! 


#### Prerequisite: To setup required credentials with below commands or with secret manager in Visual Studio 2022

```pwsh
# For Azure OpenAI related sample code
setx AZURE_OPENAI_ENDPOINT REPLACE_WITH_YOUR_AOAI_ENDPOINT_VALUE_HERE
setx AZURE_OPENAI_API_KEY REPLACE_WITH_YOUR_AOAI_KEY_VALUE_HERE
setx AZURE_OPENAI_DEPLOYMENT_ID REPLACE_WITH_YOUR_AOAI_DEPLOYMENT_VALUE_HERE

# For Azure AI Search related sample code
setx AZURE_AI_SEARCH_ENDPOINT REPLACE_WITH_YOUR_AZURE_SEARCH_RESOURCE_VALUE_HERE
setx AZURE_AI_SEARCH_API_KEY REPLACE_WITH_YOUR_AZURE_SEARCH_RESOURCE_KEY_VALUE_HERE
setx AZURE_AI_SEARCH_INDEX REPLACE_WITH_YOUR_INDEX_NAME_HERE
```


##### Reference:
- [Quickstart: Generate images with Azure OpenAI Service](https://learn.microsoft.com/en-us/azure/ai-services/openai/dall-e-quickstart?pivots=programming-language-csharp&tabs=dalle3%2Ccommand-line)
- [Quickstart: Chat with Azure OpenAI models using your own data](https://learn.microsoft.com/en-us/azure/ai-services/openai/use-your-data-quickstart?context=%2Fazure%2Fsearch%2Fcontext%2Fcontext&tabs=command-line%2Cpython-new&pivots=programming-language-csharp#async-with-streaming)
- [Quickstart: Semantic ranking with .NET or Python](https://learn.microsoft.com/en-us/azure/search/search-get-started-semantic?tabs=dotnet)
