using Constructs;
using Ivy.Examples.Constructs.Domain;
using Ivy.Examples.Constructs.Extensions;

namespace Ivy.Examples.Constructs.Apps;

[App(icon: Icons.PartyPopper, title: "Constructs (AWS)")]
public class ConstructsApp : ViewBase
{
    private RootConstruct? _root;
    const int MaxLines = 40;

    public override object? Build()
    {
        var status = UseState(string.Empty);
        var parent = UseState(string.Empty);
        var childId = UseState(string.Empty);
        var maxLines = UseState(MaxLines);

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

        string basePath = string.IsNullOrWhiteSpace(parent.Value) ? _root.Node.Path : parent.Value.Trim();
        var asciiLines = _root.RenderAsciiLines(basePath);
        var visible = asciiLines.Take(maxLines.Value).ToList();
        bool hasMore = asciiLines.Count > visible.Count;

        // Build left (controls) panel
        var left = Layout.Vertical().Gap(6)
            | Text.H2("AWS Constructs — interactive demo")
            | Text.Block("Select a parent path, add a child construct, or reset to the canonical tree.")
            | Text.Block("Parent path, ex. Root/Demo/Nested. Start typing to filter tree view.")
            | parent.ToInput(placeholder: "/Root/Demo")
            | Text.Block("New child id")
            | childId.ToInput(placeholder: "ChildX")
            | Layout.Horizontal().Gap(4)
              | new Button("Add child", onClick: () =>
                 {
                     var p = string.IsNullOrWhiteSpace(parent.Value) ? _root.Node.Path : parent.Value.Trim();
                     var id = childId.Value.Trim();

                     if (string.IsNullOrWhiteSpace(id))
                     {
                         status.Set("Enter a non-empty child id.");
                         return;
                     }

                     var parentNode = _root.FindByPath(p);
                     if (parentNode == null)
                     {
                         status.Set($"Parent path not found: {p}");
                         return;
                     }
                     _ = new Construct(parentNode, id);

                     childId.Set(string.Empty);
                     status.Set($"Added: {parentNode.Node.Path}/{id}");
                 })
                 | new Button("Reset", onClick: () =>
                 {
                     _root = DemoConstruct.BuildRoot();
                     parent.Set(_root.Node.Path);
                     status.Set("Tree reset.");
                     maxLines.Set(MaxLines);
                 })
            | (string.IsNullOrWhiteSpace(status.Value) ? Text.Block(string.Empty) : Text.Block(status.Value));

        // Build right (tree view) panel
        var right = Layout.Vertical().Gap(6)
            | Text.Block("Current tree (subtree of the selected parent)")
            | Text.Markdown("```text\n" + string.Join('\n', visible) + "\n```")
            | (hasMore
                ? new Button("Show more", onClick: () => maxLines.Set(maxLines.Value + MaxLines))
                : (visible.Count > MaxLines
                    ? new Button("Collapse", onClick: () => maxLines.Set(MaxLines))
                    : Text.Block(string.Empty)));

        return Layout.Center()
             | new Card(
                 Layout.Horizontal().Gap(12).Padding(3)
                 | left
                 | right
               )
               .Width(Size.Units(340).Max(1000));
    }
}
