using System.Linq;
using System.Text;
using Constructs;
using Ivy.Examples.Constructs.Domain;

namespace Ivy.Examples.Constructs.Apps;

[App(icon: Icons.PartyPopper, title: "Constructs (AWS)")]
public class ConstructsApp : ViewBase
{
    // Lazy init to avoid crashing the app host on env/jsii errors
    private RootConstruct? _root;

    public override object? Build()
    {
        return _root ??= new RootConstruct();
    }
}
