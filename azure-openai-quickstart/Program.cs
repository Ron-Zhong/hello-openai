using Azure;
using Azure.AI.OpenAI;
using System.Text.Json;
using static System.Environment;

string azureOpenAIEndpoint = GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
string azureOpenAIKey = GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
string deploymentName = GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_ID");
string searchEndpoint = GetEnvironmentVariable("AZURE_AI_SEARCH_ENDPOINT");
string searchKey = GetEnvironmentVariable("AZURE_AI_SEARCH_API_KEY");
string searchIndex = GetEnvironmentVariable("AZURE_AI_SEARCH_INDEX");


var client = new OpenAIClient(new Uri(azureOpenAIEndpoint), new AzureKeyCredential(azureOpenAIKey));

var chatCompletionsOptions = new ChatCompletionsOptions()
{
    Messages =
    {
        new ChatRequestUserMessage("What are my available health plans?"),
    },

    AzureExtensionsOptions = new AzureChatExtensionsOptions()
    {
        Extensions =
        {
            new AzureCognitiveSearchChatExtensionConfiguration()
            {
                SearchEndpoint = new Uri(searchEndpoint),
                Key = searchKey,
                IndexName = searchIndex,
            },
        }
    },
    DeploymentName = deploymentName
};

Response<ChatCompletions> response = client.GetChatCompletions(chatCompletionsOptions);

ChatResponseMessage responseMessage = response.Value.Choices[0].Message;

Console.WriteLine($"Message from {responseMessage.Role}:");
Console.WriteLine("===");
Console.WriteLine(responseMessage.Content);
Console.WriteLine("===");

Console.WriteLine($"Context information (e.g. citations) from chat extensions:");
Console.WriteLine("===");
foreach (ChatResponseMessage contextMessage in responseMessage.AzureExtensionsContext.Messages)
{
    string contextContent = contextMessage.Content;
    try
    {
        var contextMessageJson = JsonDocument.Parse(contextMessage.Content);
        contextContent = JsonSerializer.Serialize(contextMessageJson, new JsonSerializerOptions()
        {
            WriteIndented = true,
        });
    }
    catch (JsonException)
    {}
    Console.WriteLine($"{contextMessage.Role}: {contextContent}");
}
Console.WriteLine("===");