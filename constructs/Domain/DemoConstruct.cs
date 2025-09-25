using Constructs;

namespace Ivy.Examples.Constructs.Domain;

public class DemoConstruct : Construct
{
    public Construct ChildA { get; }
    public Construct ChildB { get; }
    public Construct Nested { get; }
    public Construct Leaf1 { get; }
    public Construct Leaf2 { get; }

    public DemoConstruct(Construct scope, string id) : base(scope, id)
    {
        ChildA = new Construct(this, "ChildA");
        ChildB = new Construct(this, "ChildB");

        Nested = new Construct(this, "Nested");
        Leaf1 = new Construct(Nested, "Leaf1");
        Leaf2 = new Construct(Nested, "Leaf2");
    }

    public static RootConstruct BuildRoot()
    {
        var root = new RootConstruct("Root");
        _ = new DemoConstruct(root, "Demo");
        return root;
    }
}
