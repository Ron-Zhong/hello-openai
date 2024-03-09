using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

const string model = "gpt-35-turbo";
var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = configuration["AZURE_OPENAI_ENDPOINT"] ?? string.Empty;
var key = configuration["AZURE_OPENAI_API_KEY"] ?? string.Empty;
var client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));

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

Response<Completions> completionsResponse = client.GetCompletions(model, completionsOptions);
string completion = completionsResponse.Value.Choices[0].Text;
Console.WriteLine($"Summarization: {completion}");