using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

string azureOpenAiEndpoint = configuration["AZURE_OPENAI_ENDPOINT"] ?? string.Empty;
string azureOpenAiKey = configuration["AZURE_OPENAI_API_KEY"] ?? string.Empty;
string deploymentName = configuration["AZURE_OPENAI_DEPLOYMENT_ID"] ?? string.Empty;
string prompt = "A beautiful sunset over the ocean.";

OpenAIClient client = new (new Uri(azureOpenAiEndpoint), new AzureKeyCredential(azureOpenAiKey));


var imageGenerationOptions = new ImageGenerationOptions
{
    Prompt = prompt,
    DeploymentName = deploymentName,
    Size = new ImageSize("1792x1024"),
    ImageCount = 1
};

var response = await client.GetImageGenerationsAsync(imageGenerationOptions);
Console.WriteLine(response.Value.Data[0].Url.AbsoluteUri);
