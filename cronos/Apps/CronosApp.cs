namespace CronosExample.Apps;

[App(icon:Icons.TimerReset, title:"Cronos")]
public class CronosApp : ViewBase
{
    public override object? Build()
    {
        return "HelloWorld";
    }
}