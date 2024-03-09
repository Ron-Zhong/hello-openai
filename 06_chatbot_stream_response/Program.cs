using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

const string model = "gpt-35-turbo";
var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = configuration["AZURE_OPENAI_ENDPOINT"] ?? string.Empty;
var key = configuration["AZURE_OPENAI_API_KEY"] ?? string.Empty;
var client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));


// Sample Code: Stream Chat Messages with non-Azure OpenAI
while (true)
{
    Console.WriteLine("You:");
    var input = Console.ReadLine();

    var options = new ChatCompletionsOptions()
    {
        Messages =
        {
            new ChatMessage(ChatRole.User, input),
        }
    };

    Console.WriteLine();
    Console.WriteLine("OpenAI:");

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

    Console.WriteLine();
}
