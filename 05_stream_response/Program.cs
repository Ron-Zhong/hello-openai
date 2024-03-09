using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

const string model = "gpt-35-turbo";
var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = configuration["AZURE_OPENAI_ENDPOINT"] ?? string.Empty;
var key = configuration["AZURE_OPENAI_API_KEY"] ?? string.Empty;
var client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));


// Sample Code: Stream Chat Messages with non-Azure OpenAI
var options = new ChatCompletionsOptions()
{
    Messages =
    {
        new ChatMessage(ChatRole.System, "You are a helpful assistant. You will talk like a pirate."),
        new ChatMessage(ChatRole.User, "Can you help me?"),
        new ChatMessage(ChatRole.Assistant, "Arrrr! Of course, me hearty! What can I do for ye?"),
        new ChatMessage(ChatRole.User, "What's the best way to train a parrot?"),
    }
};

Response<StreamingChatCompletions> response = await client.GetChatCompletionsStreamingAsync(model, options);
using StreamingChatCompletions streamingChatCompletions = response.Value;

await foreach (StreamingChatChoice choice in streamingChatCompletions.GetChoicesStreaming())
{
    await foreach (ChatMessage message in choice.GetMessageStreaming())
    {
        Console.Write(message.Content);
    }
    Console.WriteLine();
}