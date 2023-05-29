using System;
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

Response<Completions> response = await client.GetCompletionsAsync(
    OPENAI_MODEL_NAME, 
    "Hello, world!");

foreach (Choice choice in response.Value.Choices)
{
    Console.WriteLine(choice.Text);
}
