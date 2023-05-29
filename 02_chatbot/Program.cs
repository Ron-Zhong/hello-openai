using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
                .AddUserSecrets<Program>();
IConfiguration Configuration = builder.Build();

var IS_AZURE_OPENAI = false;
var AZURE_OPENAI_ENDPOINT = Configuration["azure-openai-endpoint"] ?? string.Empty;
var AZURE_OPENAI_API_KEY = Configuration["azure-openai-api-key"] ?? string.Empty;
var OPENAI_API_KEY = Configuration["openai-api-key"] ?? string.Empty;
var OPENAI_MODEL_NAME = IS_AZURE_OPENAI ? "davinci" : "text-davinci-003";

OpenAIClient client = IS_AZURE_OPENAI
    ? new OpenAIClient(
        new Uri(AZURE_OPENAI_ENDPOINT),
        new AzureKeyCredential(AZURE_OPENAI_API_KEY))
    : new OpenAIClient(OPENAI_API_KEY);

string prompt = "What is Azure OpenAI?";
Console.Write($"Input: {prompt}");

Response<Completions> completionsResponse = client.GetCompletions(OPENAI_MODEL_NAME, prompt);
string completion = completionsResponse.Value.Choices[0].Text;
Console.WriteLine($"Chatbot: {completion}");