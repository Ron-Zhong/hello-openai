using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

const string model = "gpt-35-turbo";
var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = configuration["AZURE_OPENAI_ENDPOINT"] ?? string.Empty;
var key = configuration["AZURE_OPENAI_API_KEY"] ?? string.Empty;
var client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));


string prompt = "What is Azure OpenAI?";
Console.Write($"Input: {prompt}");

Response<Completions> completionsResponse = client.GetCompletions(model, prompt);
string completion = completionsResponse.Value.Choices[0].Text;
Console.WriteLine($"Chatbot: {completion}");