

using Microsoft.SemanticKernel;
using System.Net;

namespace MicrosoftSemanticKernelDemo.Apps;

[App(icon: Icons.PartyPopper, title: "MicrosoftSemanticKernel Demo")]
public class MicrosoftSemanticKernelApp : ViewBase
{
    public record NotesModel(
        string NotesFromMeeting
    );

    public override object? Build()
    {
        //initialize Open Ai
        //***  REPLACE API_KEY WITH YOUR OPENAI APIKEY  ***
        var kernel = Kernel.CreateBuilder()
            .AddOpenAIChatCompletion("gpt-4o-mini", "API_KEY")
            .Build();

        // Create the function for extracting an action list from meeting notes
        var extractTasks = kernel.CreateFunctionFromPrompt("Extract action items as a list without title \n{{$input}}");

        // Use a model for the meetings notes to be able to use .ToForm() method
        var notesModel = UseState(() => new NotesModel("Today we discussed project deadlines. John will send the report tomorrow. Maria will set up the next meeting."));

        // Execute the extraction function with the notes that come from the text area.
        var tasksString = kernel.InvokeAsync(extractTasks, new() { ["input"] = notesModel.Value.NotesFromMeeting }).Result.GetValue<string>().Replace("\n", "<br/>");
        
        return Layout.Horizontal()
            | new Card(
                Layout.Vertical()
                    | notesModel.ToForm()
                        .Builder(m => m.NotesFromMeeting, s => s.ToTextAreaInput())  
            ).Title("Input").Width(1 / 2f)
            | new Card(
                Layout.Vertical()
                    | Text.H3("Tasks")
                    | new Html (tasksString)
            ).Title("Result").Width(1 / 2f);
    }
}