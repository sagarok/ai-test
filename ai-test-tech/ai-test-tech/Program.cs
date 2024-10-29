using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Ai.Test.Tech;

internal class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
        string endpoint = config["AZURE_OPENAI_ENDPOINT"];
        string deployment = config["AZURE_OPENAI_GPT_NAME"];
        string key = config["AZURE_OPENAI_KEY"];

        var aiPromptSettings = new OpenAIPromptExecutionSettings() { MaxTokens = 400 };
        AzureOpenAIChatCompletionService service = new(deployment, endpoint, key);

        ChatHistory chatHistory = new($"""
    You are an upbeat and friendly car recommender assistant for a car retailing company.
    You should engage with users, asking relevant questions to comprehend their car preferences 
    such as budget, car type, fuel type, brand, or specific features. 
    Upon capturing these responses, you should suggest the most suitable cars with their specifications.
    The focus should be on user-friendliness to ensure a smooth experience.
    In your responses use only letters, digits and punctuation characters. No characters, unprintable in a console.
    You introduce yourself when first saying hello.
    """);

        var userNameDisp = Environment.UserName.PadRight(10, ' ');
        var assistantNameDisp = "Assistant ";

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("-------------------------");
        Console.WriteLine("Welcome to Best cars inc.");
        Console.WriteLine("-------------------------");
        Console.WriteLine();

        string? userInput = "Hi!";
        chatHistory.AddUserMessage(userInput);
        await PrintAssistantResponseAsync();

        do
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"{userNameDisp} >>> ");
            userInput = Console.ReadLine() ?? "";
            chatHistory.AddUserMessage(userInput);

            await PrintAssistantResponseAsync();
        }
        while (!string.IsNullOrWhiteSpace(userInput)
            && string.Compare(userInput.Trim(), "bye", StringComparison.OrdinalIgnoreCase) != 0);

        async Task PrintAssistantResponseAsync()
        {
            var response = await service.GetChatMessageContentAsync(chatHistory, aiPromptSettings);
            chatHistory.Add(response);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{assistantNameDisp} >>> {response}");
        }
    }
}
