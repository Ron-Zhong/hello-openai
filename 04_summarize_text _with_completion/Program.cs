using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

// Initialize configuration
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


// Sample Code: Summarize Text with Completion
string textToSummarize = @"
    Two independent experiments reported their results this morning at CERN, Europe's high-energy physics laboratory near Geneva in Switzerland. Both show convincing evidence of a new boson particle weighing around 125 gigaelectronvolts, which so far fits predictions of the Higgs previously made by theoretical physicists.

    ""As a layman I would say: 'I think we have it'. Would you agree?"" Rolf-Dieter Heuer, CERN's director-general, asked the packed auditorium. The physicists assembled there burst into applause.
:";

string summarizationPrompt = @$"
    Summarize the following text.

    Text:
    """"""
    {textToSummarize}
    """"""

    Summary:
";

Console.Write($"Input: {summarizationPrompt}");
var completionsOptions = new CompletionsOptions()
{
    Prompts = { summarizationPrompt },
};

Response<Completions> completionsResponse = client.GetCompletions(OPENAI_MODEL_NAME, completionsOptions);
string completion = completionsResponse.Value.Choices[0].Text;
Console.WriteLine($"Summarization: {completion}");