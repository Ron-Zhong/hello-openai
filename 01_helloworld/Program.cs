using System;
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

Response<Completions> response = await client.GetCompletionsAsync(
    "text-davinci-003", 
    "Hello, world!");

foreach (Choice choice in response.Value.Choices)
{
    Console.WriteLine(choice.Text);
}