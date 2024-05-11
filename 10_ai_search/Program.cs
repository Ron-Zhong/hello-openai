using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string azureOpenAiEndpoint = configuration["AZURE_OPENAI_ENDPOINT"] ?? string.Empty;
string azureOpenAiKey = configuration["AZURE_OPENAI_API_KEY"] ?? string.Empty;
string deploymentName = configuration["AZURE_OPENAI_DEPLOYMENT_ID"] ?? string.Empty;
string searchEndpoint = configuration["AZURE_AI_SEARCH_ENDPOINT"] ?? string.Empty;
string searchKey = configuration["AZURE_AI_SEARCH_API_KEY"] ?? string.Empty;
string searchIndex = configuration["AZURE_AI_SEARCH_INDEX"] ?? string.Empty;


var client = new OpenAIClient(new Uri(azureOpenAiEndpoint), new AzureKeyCredential(azureOpenAiKey));

while (true)
{
    Console.Write("\n\n Input: ");
    string input = Console.ReadLine()!;
    await ChatAsync(input);
}
async Task ChatAsync(string input)
{
    input += " Would you respond with a list of answers?";
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
            Console.Write($"{chatUpdate.Role.Value.ToString().ToUpperInvariant()}: ");
        }
        if (!string.IsNullOrEmpty(chatUpdate.ContentUpdate))
        {
            Console.Write(chatUpdate.ContentUpdate);
        }
    }
}