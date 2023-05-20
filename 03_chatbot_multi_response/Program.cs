using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

// Initialize configuration
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


// Sample Code: Generate Multiple Chatbot Responses With Subscription Key
List<string> examplePrompts = new(){
    "How are you today?",
    "What is Azure OpenAI?",
    "Why do children love dinosaurs?",
    "Generate a proof of Euler's identity",
    "Describe in single words only the good things that come into your mind about your mother.",
};

string deploymentName = "text-davinci-003";

foreach (string prompt in examplePrompts)
{
    Console.WriteLine($"Input: {prompt}");
    CompletionsOptions completionsOptions = new CompletionsOptions();
    completionsOptions.Prompts.Add(prompt);

    Response<Completions> completionsResponse = client.GetCompletions(deploymentName, completionsOptions);
    string completion = completionsResponse.Value.Choices[0].Text;
    Console.WriteLine($"Chatbot: {completion}");
}