using Azure.AI.OpenAI;
using Azure;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

// Azure OpenAI setup
var apiBase = "https://mims-openai-jp.openai.azure.com/"; // Add your endpoint here
var apiKey = configuration["AZURE_OPENAI_API_KEY"]; // Add your OpenAI API key here
var deploymentId = "gpt-35-turbo"; // Add your deployment ID here

// Azure AI Search setup
var searchEndpoint = "https://mims-ai-search-jp.search.windows.net"; // Add your Azure AI Search endpoint here
var searchKey = configuration["AZURE_AI_SEARCH_KEY"]; // Add your Azure AI Search admin key here
var searchIndexName = "index-sit-msp-articles"; // Add your Azure AI Search index name here
var client = new OpenAIClient(new Uri(apiBase), new AzureKeyCredential(apiKey!));

var chatCompletionsOptions = new ChatCompletionsOptions()
{
    Messages =
    {
        new Chat(ChatRole.System, "What are the differences between Azure Machine Learning and Azure AI services?")
    },
    // The addition of AzureChatExtensionsOptions enables the use of Azure OpenAI capabilities that add to
    // the behavior of Chat Completions, here the "using your own data" feature to supplement the context
    // with information from an Azure AI Search resource with documents that have been indexed.
    AzureExtensionsOptions = new AzureChatExtensionsOptions()
    {
        Extensions =
        {
            new AzureCognitiveSearchChatExtensionConfiguration()
            {
                SearchEndpoint = new Uri(searchEndpoint),
                IndexName = searchIndexName,
                SearchKey = new AzureKeyCredential(searchKey!),
                QueryType = new AzureChatExtensionType(azure_search),
                Parameters = FromString([object Object]),
            },
        },
    },
    DeploymentName = gpt - 35 - turbo,
    MaxTokens = 800,
    StopSequences = null,
    Temperature = 0,
};

var response = await client.GetChatCompletionsAsync(
    //deploymentId,
    chatCompletionsOptions);

var message = response.Value.Choices[0].Message;
// The final, data-informed response still appears in the ChatMessages as usual
Console.WriteLine($"{message.Role}: {message.Content}");
// Responses that used extensions will also have Context information
// to explain extension activity and provide supplemental information like citations.
//Console.WriteLine($"Citations and other information:");
//foreach (var contextMessage in message.AzureExtensionsContext.Messages)
//{
//    // Note: citations and other extension payloads from the "tool" role are often encoded JSON documents
//    // and need to be parsed as such; that step is omitted here for brevity.
//    Console.WriteLine($"{contextMessage.Role}: {contextMessage.Content}");
//}

