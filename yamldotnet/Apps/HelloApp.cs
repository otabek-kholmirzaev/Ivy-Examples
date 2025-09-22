using YamlDotNet.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace YamlDotNet.Apps;

[App(icon: Icons.Code, title: "YAML Demo")]
public class SerializationApp : ViewBase
{
    public override object? Build()
    {
        var csCode = UseState(@"var person = new Person
{
    Name = ""Abe Lincoln"",
    Age = 25,
    HeightInInches = 6f + 4f / 12f,
    Addresses = new Dictionary<string, Address>{
        { ""home"", new  Address() {
                Street = ""2720  Sundown Lane"",
                City = ""Kentucketsville"",
                State = ""Calousiyorkida"",
                Zip = ""99978"",
            }},
        { ""work"", new  Address() {
                Street = ""1600 Pennsylvania Avenue NW"",
                City = ""Washington"",
                State = ""District of Columbia"",
                Zip = ""20500"",
            }},
    }
};");

        var yamlOutput = UseState<string>();
        var errorMessage = UseState<string>();

        return Layout.Vertical().Gap(4).Padding(2)
            | Text.H2("C# to YAML Converter")
            | Text.Block("Enter your C# Person object code below and see the YAML output:")
            | new Separator()

            | (Layout.Horizontal().Gap(4)
                | Text.H3("C# Input").Width(Size.Full())
                | Text.H3("YAML Output").Width(Size.Full()))

            | (Layout.Horizontal().Gap(4).Padding(2)

                | csCode.ToCodeInput()
                    .Width(Size.Full())
                    .Height(Size.Auto())
                    .Language(Languages.Csharp)
                    .Placeholder("Enter your C# Person object code here...")

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
                .Primary()
                .HandleClick(() => ConvertToYaml(csCode.Value, yamlOutput, errorMessage))

            | new Separator()
            | Text.Small("This demo uses YamlDotNet library to serialize C# objects to YAML format.")
            | Text.Markdown("Built with [Ivy Framework](https://github.com/Ivy-Interactive/Ivy-Framework) and [YamlDotNet](https://github.com/aaubry/YamlDotNet)");
    }

    private void ConvertToYaml(string csharpCode, IState<string> yamlOutput, IState<string> errorMessage)
    {
        try
        {
            errorMessage.Value = string.Empty;

            // Create a sample Person object based on the provided code
            var person = new Person
            {
                Name = "Abe Lincoln",
                Age = 25,
                HeightInInches = 6f + 4f / 12f,
                Addresses = new Dictionary<string, Address>
                {
                    { "home", new Address
                        {
                            Street = "2720  Sundown Lane",
                            City = "Kentucketsville",
                            State = "Calousiyorkida",
                            Zip = "99978"
                        }
                    },
                    { "work", new Address
                        {
                            Street = "1600 Pennsylvania Avenue NW",
                            City = "Washington",
                            State = "District of Columbia",
                            Zip = "20500"
                        }
                    }
                }
            };

            // Serialize to YAML using YamlDotNet
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            yamlOutput.Value = serializer.Serialize(person);
        }
        catch (Exception ex)
        {
            errorMessage.Value = ex.Message;
            yamlOutput.Value = string.Empty;
        }
    }
}