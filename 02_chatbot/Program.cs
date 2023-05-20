using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
                .AddUserSecrets<Program>();
IConfiguration Configuration = builder.Build();

var AZURE_OPENAI_ENABLED = true;
var AZURE_OPENAI_ENDPOINT = "https://poc-openai-mims.openai.azure.com/";
var AZURE_OPENAI_API_KEY = Configuration["azure-openai-api-key"] ?? string.Empty;
var OPENAI_API_KEY = Configuration["openai-api-key"] ?? string.Empty;

OpenAIClient client = AZURE_OPENAI_ENABLED
    ? new OpenAIClient(
        new Uri(AZURE_OPENAI_ENDPOINT),
        new AzureKeyCredential(AZURE_OPENAI_API_KEY))
    : new OpenAIClient(OPENAI_API_KEY);

string deploymentName = "text-davinci-003";
string prompt = "What is Azure OpenAI?";
Console.Write($"Input: {prompt}");

Response<Completions> completionsResponse = client.GetCompletions(deploymentName, prompt);
string completion = completionsResponse.Value.Choices[0].Text;
Console.WriteLine($"Chatbot: {completion}");