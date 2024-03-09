using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

const string model = "gpt-35-turbo";
var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = configuration["AZURE_OPENAI_ENDPOINT"] ?? string.Empty;
var key = configuration["AZURE_OPENAI_API_KEY"] ?? string.Empty;
var client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));


// Sample Code: Generate Multiple Chatbot Responses With Subscription Key
List<string> examplePrompts = new(){
    "How are you today?",
    "What is Azure OpenAI?",
    "Why do children love dinosaurs?",
    "Generate a proof of Euler's identity",
    "Describe in single words only the good things that come into your mind about your mother.",
};

foreach (string prompt in examplePrompts)
{
    Console.WriteLine($"Input: {prompt}");
    CompletionsOptions completionsOptions = new CompletionsOptions();
    completionsOptions.Prompts.Add(prompt);

    Response<Completions> completionsResponse = client.GetCompletions(model, completionsOptions);
    string completion = completionsResponse.Value.Choices[0].Text;
    Console.WriteLine($"Chatbot: {completion}");
}