using Humanizer;

[App(icon: Icons.Box)]
public class HumanizerApp : ViewBase
{
    public override object? Build()
    {
        //State for the input field where users type new text
        var newText = this.UseState("");

        //Service for showing toast notifications
        var client = this.UseService<IClientProvider>();

        //State for storing the list of humanized items
        var humanizedTexts = this.UseState(ImmutableArray.Create<string>());

        // The current text
        string currentText = "";

        return new Card().Title("Humanizer").Description("What do you want to humanize?")
          | (Layout.Vertical()
              | (Layout.Horizontal().Width(Size.Full())
                 | newText.ToTextInput(placeholder: "New text...").Width(Size.Grow())
                 | new Button("Humanize", onClick: _ =>
                     {
                         currentText = newText.Value;
                         currentText = currentText.Humanize();
                         client.Toast(currentText, "Humanized Text");
                         humanizedTexts.Set(humanizedTexts.Value.Add(currentText));
                     }
                 ).Icon(Icons.Plus).Variant(ButtonVariant.Primary)
              )
              | (Layout.Vertical()
                 | humanizedTexts.Value.Select(text => new string(text))
              ));
    }
}