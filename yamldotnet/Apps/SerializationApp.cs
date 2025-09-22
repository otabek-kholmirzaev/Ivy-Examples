using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Services;

namespace YamlDotNet.Apps;

[App(icon: Icons.Code, title: "YAML Demo")]
public class SerializationApp : ViewBase
{
    private readonly CSharpParser _parser = new();

    public override object? Build()
    {
        var csCode = UseState(@"Name = ""John Doe"",
Age = 30,
IsActive = true,
Tags = new[] { ""developer"", ""csharp"", ""yaml"" },
Scores = new Dictionary<string, int> { { ""math"", 95 }, { ""science"", 88 }, { ""english"", 92 }}");

        var yamlOutput = UseState<string>();
        var errorMessage = UseState<string>();

        return Layout.Vertical().Gap(4).Padding(2)
            | Text.H2("Universal C# to YAML Converter")
            | Text.Block("Enter any C# object code below and see the YAML output:")
            | new Separator()

            | (Layout.Horizontal().Gap(4)
                | Text.H3("C# Input").Width(Size.Full())
                | Text.H3("YAML Output").Width(Size.Full()))

            | (Layout.Horizontal().Gap(4).Padding(2)
                | csCode.ToCodeInput()
                    .Width(Size.Full())
                    .Height(Size.Auto())
                    .Language(Languages.Csharp)
                    .Placeholder("Enter your C# object code here...")

                | new Separator()

                | (string.IsNullOrEmpty(errorMessage.Value)
                    ? yamlOutput.ToCodeInput()
                        .Width(Size.Full())
                        .Height(Size.Auto())
                        .ShowCopyButton(true)
                    : Text.Block($"Error: {errorMessage.Value}")
                        .Color(Colors.Red)))

            // Convert Button
            | new Button("Convert to YAML")
                .HandleClick(() => ConvertToYaml(csCode.Value, yamlOutput, errorMessage))

            | new Separator()
            | Text.Small("This demo uses YamlDotNet library to serialize any C# object to YAML format.")
            | Text.Markdown("Built with [Ivy Framework](https://github.com/Ivy-Interactive/Ivy-Framework) and [YamlDotNet](https://github.com/aaubry/YamlDotNet)");
    }

    private void ConvertToYaml(string csharpCode, IState<string> yamlOutput, IState<string> errorMessage)
    {
        try
        {
            errorMessage.Value = string.Empty;

            // Parse the user input to create dynamic object
            var data = _parser.ParseUserInput(csharpCode);

            // Serialize to YAML using YamlDotNet
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            yamlOutput.Value = serializer.Serialize(data);
        }
        catch (Exception ex)
        {
            errorMessage.Value = $"Error: {ex.Message}";
            yamlOutput.Value = string.Empty;
        }
    }

}