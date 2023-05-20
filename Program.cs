using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
                .AddUserSecrets<Program>();
IConfiguration Configuration = builder.Build();

var USE_AZURE_OPENAI = false;
var OPENAI_API_KEY = Configuration["openai-api-key"];

OpenAIClient client = USE_AZURE_OPENAI
    ? new OpenAIClient(
        new Uri("https://your-azure-openai-resource.com/"),
        new AzureKeyCredential("your-azure-openai-resource-api-key"))
    : new OpenAIClient(OPENAI_API_KEY);

Response<Completions> response = await client.GetCompletionsAsync(
    "text-davinci-003", // assumes a matching model deployment or model name
    "Hello, world!");

foreach (Choice choice in response.Value.Choices)
{
    Console.WriteLine(choice.Text);
}