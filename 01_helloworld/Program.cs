using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

const string model = "gpt-35-turbo";
var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = configuration["AZURE_OPENAI_ENDPOINT"] ?? string.Empty;
var key = configuration["AZURE_OPENAI_API_KEY"] ?? string.Empty;
var client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));


Response<Completions> response = await client.GetCompletionsAsync(
    model, 
    "Hello, world!");

foreach (Choice choice in response.Value.Choices)
{
    Console.WriteLine(choice.Text);
}
