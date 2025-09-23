

using Ivy;
using Ivy.Core.Hooks;
using Microsoft.SemanticKernel;
using System.Net;

namespace MicrosoftSemanticKernelDemo.Apps;

[App(icon: Icons.PartyPopper, title: "MicrosoftSemanticKernel Demo")]
public class MicrosoftSemanticKernelApp : ViewBase
{
    private Kernel kernel;
    private KernelFunction extractTasks;
    private IState<string> notesFromMeeting;
    private IState<string[]> tasks;
    private IState<int> triggerRefresh;
    public override object? Build()
    {
        //initialize Open Ai
        //***  REPLACE API_KEY WITH YOUR OPENAI APIKEY  ***
        kernel = Kernel.CreateBuilder()
            .AddOpenAIChatCompletion("gpt-4o-mini", "API_KEY")
            .Build();

        // Create the function for extracting an action list from meeting notes
        extractTasks = kernel.CreateFunctionFromPrompt("Extract action items as a list without title \n{{$input}}");

        notesFromMeeting = UseState(() => "Today we discussed project deadlines. John will send the report tomorrow. Maria will set up the next meeting.");
        
        triggerRefresh = UseState(0);
        tasks = UseState<string[]>([]);
        
        // Execute the extraction function asynchronously
        UseEffect(async () =>
        {
            var result = await ExtractTasksFromNoteMeeting();
            tasks.Set(result);

        }, triggerRefresh,EffectTrigger.AfterInit());
        


        return Layout.Horizontal()
            | new Card(
                Layout.Vertical()
                    | notesFromMeeting.ToTextAreaInput()
                    | new Button("Update tasks").HandleClick(_ => UpdateMeetingTasks())
            ).Title("Input").Width(1 / 2f)
            | new Card(
                Layout.Vertical()
                    | Text.H3("Tasks")
                    | tasks.Value
            ).Title("Result").Width(1 / 2f);
    }

    private void UpdateMeetingTasks()
    {
        triggerRefresh.Set(triggerRefresh.Value + 1);
    }

    private Task<string[]> ExtractTasksFromNoteMeeting()
    {
        return Task.Run(async () =>
        {
            var extractedTasks = await kernel.InvokeAsync(extractTasks, new() { ["input"] = notesFromMeeting });
            return extractedTasks.GetValue<string>().Split('\n');
        });
    }
}