using Constructs;
using Ivy.Examples.Constructs.Domain;
using Ivy.Examples.Constructs.Extensions;

namespace Ivy.Examples.Constructs.Apps;

[App(icon: Icons.PartyPopper, title: "Constructs (AWS)")]
public class ConstructsApp : ViewBase
{
    // Lazy init to avoid crashing the app host on env/jsii errors
    private RootConstruct? _root;

    public override object? Build()
    {
        var status = UseState(string.Empty);
        var parent = UseState(string.Empty);  // parent path to add under
        var childId = UseState(string.Empty); // new child id
        var maxLines = UseState(40);          // visible lines for tree

        if (_root is null)
        {
            try
            {
                _root = DemoConstruct.BuildRoot();
                parent.Set(_root.Node.Path);  // "/Root"
            }
            catch (Exception ex)
            {
                return Layout.Center()
                     | new Card(
                         Layout.Vertical().Gap(6).Padding(3)
                         | Text.H2("Constructs initialization failed")
                         | Text.Block("Make sure Node.js 20/22 LTS is installed (jsii runtime).")
                         | new Separator()
                         | Text.Markdown("```text\n" + ex + "\n```")
                       )
                       .Width(Size.Units(140).Max(800));
            }
        }

        // Filter to subtree so the view fits on screen
        var basePath = string.IsNullOrWhiteSpace(parent.Value) ? _root!.Node.Path : parent.Value.Trim();
        var asciiLines = _root.RenderAsciiLines(basePath);
        var visible = asciiLines.Take(maxLines.Value).ToList();
        var hasMore = asciiLines.Count > visible.Count;

        return Layout.Center()
             | new Card(
                 Layout.Vertical().Gap(8).Padding(3)
                 | Text.H2("AWS Constructs — interactive demo")
                 | Text.Block("Select a parent path, add a child construct, or reset to the canonical tree.")
                 | new Separator()

                 | Text.Block("Parent path")
                 | parent.ToInput(placeholder: "/Root/Demo")

                 | Text.Block("New child id")
                 | childId.ToInput(placeholder: "ChildX")

                 | Layout.Horizontal().Gap(4)
                   | new Button("Add child", onClick: () =>
                   {
                       var p = string.IsNullOrWhiteSpace(parent.Value) ? _root!.Node.Path : parent.Value.Trim();
                       var id = childId.Value?.Trim();

                       if (string.IsNullOrWhiteSpace(id))
                       {
                           status.Set("Enter a non-empty child id.");
                           return;
                       }

                       var parentNode = _root.FindByPath(p) ?? _root!;
                       _ = new Construct(parentNode, id);

                       childId.Set(string.Empty);
                       status.Set($"Added: {parentNode.Node.Path}/{id}");
                   })
                   | new Button("Reset", onClick: () =>
                   {
                       _root = DemoConstruct.BuildRoot();
                       parent.Set(_root.Node.Path);
                       status.Set("Tree reset.");
                       maxLines.Set(40);
                   })

                 | new Separator()
                 | Text.Block("Current tree (subtree of the selected parent)")
                 | Text.Markdown("```text\n" + string.Join('\n', visible) + "\n```")
                 | (hasMore
                     ? new Button("Show more", onClick: () => maxLines.Set(maxLines.Value + 50))
                     : (visible.Count > 40
                         ? new Button("Collapse", onClick: () => maxLines.Set(40))
                         : Text.Block(string.Empty)))
                 | (string.IsNullOrWhiteSpace(status.Value) ? Text.Block("") : Text.Block("Status: " + status.Value))
               )
               .Width(Size.Units(120).Max(720));
    }
}
