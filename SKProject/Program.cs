using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel.Plugins.Web.Bing;


var builder = Kernel.CreateBuilder();

// Add logging  
builder.Services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Error));
 
builder.Services.AddAzureOpenAIChatCompletion(

    );
var kernel = builder.Build();

//Add Bing Search
#pragma warning disable SKEXP0050
var bingConnector = new BingConnector("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
var plugin = new WebSearchEnginePlugin(bingConnector);
kernel.ImportPluginFromObject(plugin, "BingPlugin");


//import the function 
kernel.ImportPluginFromType<Demographics>();


//tell OpenAI it is ok to import the function
var settings = new OpenAIPromptExecutionSettings(){ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions};

//Get chat completion service
var chatService = kernel.GetRequiredService<IChatCompletionService>();

//Create chat history
ChatHistory chat = new();

while (true){
   Console.Write("Q:");

   chat.AddUserMessage(Console.ReadLine());
   //add the kernel and settings to the chat service
   var r = await chatService.GetChatMessageContentAsync(chat, settings, kernel);
   Console.WriteLine("A:" + r);
   chat.Add(r); // add response to chat history

}

class Demographics {
    [KernelFunction]
    public int GetPersonAge(string name){
        return name switch{
            "Lucy" => 30,
            "Lily" => 25,
            _ => 0
        };
        }
    }
