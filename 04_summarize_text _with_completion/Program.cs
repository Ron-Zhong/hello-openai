using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string azureOpenAiEndpoint = configuration["AZURE_OPENAI_ENDPOINT"] ?? string.Empty;
string azureOpenAiKey = configuration["AZURE_OPENAI_API_KEY"] ?? string.Empty;
string deploymentName = configuration["AZURE_OPENAI_DEPLOYMENT_ID"] ?? string.Empty;
var client = new OpenAIClient(new Uri(azureOpenAiEndpoint), new AzureKeyCredential(azureOpenAiKey));

// Sample Code: Summarize Text with Completion
string textToSummarize = @"
About this course
COVID-19 is now treated as an endemic disease in Singapore; however, vulnerable groups remain at high risk for severe outcomes and require protection against the virus. This CME module explores the burden and impact of COVID-19 across different populations, particularly adults, children, and pregnant women. In addition, it also reviews the efficacy and safety data of COVID-19 vaccines and outlines the latest vaccination recommendations for these populations.

Learning outcomes
Upon completion of this module, you should be able to:

Gain comprehensive knowledge of the latest mRNA COVID-19 vaccination data in adults, children and pregnant women
Effectively address individual, parental and caregiver concerns regarding COVID-19 vaccination
Confidently advise individuals, parents, and caregivers on the benefits, risks, and recommended schedule of COVID-19 vaccination, enabling them to make informed vaccination decisions
Topics covered
The COVID-19 landscape in Singapore
The burden of disease of COVID-19, including its impact on maternal and perinatal outcomes
The efficacy and safety of COVID-19 vaccines in adults, children and pregnant women
The latest recommendations on COVID-19 vaccination, including scheduling for adults, paediatric populations and pregnant women
Barriers to vaccination and strategies to overcome them
:";

string summarizationPrompt = @$"
    Summarize the following text in 30~50 words.

    Text:
    """"""
    {textToSummarize}
    """"""

    Summary:
";

//Console.Write($"Input: {summarizationPrompt}");
var completionsOptions = new CompletionsOptions()
{
    DeploymentName = deploymentName, 
    Prompts = { summarizationPrompt },
    MaxTokens = 1500
};

Response<Completions> completionsResponse = await client.GetCompletionsAsync(completionsOptions);
string completion = completionsResponse.Value.Choices[0].Text;
Console.WriteLine($"Summarization: {completion}");