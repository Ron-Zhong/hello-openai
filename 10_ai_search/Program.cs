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

OpenAIClient client = new (new Uri(azureOpenAiEndpoint), new AzureKeyCredential(azureOpenAiKey));

while (true)
{
    await Console.Out.WriteAsync("\n\nMIMS AI: Ask me any healthcare questions.\nUser: ");
    string input = Console.ReadLine()!;
    await ChatWithYourDataAsync(input);
}
async Task ChatWithYourDataAsync(string input)
{
    //input += " Please give a summary in 3 lines and list down the answer in bulletins and each line shouldn't be more than 30 words.";
    var chatCompletionsOptions = new ChatCompletionsOptions()
    {
        MaxTokens = 150,
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
            await Console.Out.WriteAsync("MIMS AI: \n\t");
        }
        if (!string.IsNullOrEmpty(chatUpdate.ContentUpdate))
        {
            await Console.Out.WriteAsync(chatUpdate.ContentUpdate);
        }
    }
}