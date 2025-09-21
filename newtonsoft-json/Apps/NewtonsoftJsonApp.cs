using Newtonsoft.Json;
using NewtonsoftJsonApp.Models;

namespace NewtonsoftJsonApp.Apps
{

    [App(title: "Newtonsoft Demo", icon: Icons.File)]
    public class NewtonsoftJsonApp : ViewBase
    {
        public override object? Build()
        {
            //var nameState = this.UseState<string>();
            var client = UseService<IClientProvider>();
            var user = UseState<UserData>(() => new UserData());
            var isSerialized = UseState<bool>(true);

            string json = @"{
              'FullName': 'John Doe',
              'Email': 'johndoe@example.com',
              'IsActive': true,
              'DateCreated': '2013-01-20T00:00:00Z',
              'Roles': [
                'User',
                'Admin'
              ]
            }";

            var rawData = UseState<string>(json);

            var formBuilder = user.ToForm();
            var (onSubmit, formView, validationView, loading) = formBuilder.UseForm(this.Context);

            async ValueTask HandleSubmit()
            {
                if (await onSubmit())
                {
                    SerializeData();

                    // Form data is automatically copied to contact.Value
                    // You can access the client service here if needed
                }
            }

            void HandleButtonClick()
            {
                if (isSerialized.Value)
                {
                    DeserializeData();
                }
                else
                {
                    SerializeData();
                }
            }

            void SerializeData()
            {
                try
                {
                    var rawInfo = JsonConvert.SerializeObject(user.Value, Formatting.Indented);
                    rawData.Set(rawInfo);                   
                    isSerialized.Set(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    client.Toast(ex.Message);
                }
            }

            void DeserializeData()
            {
                try
                {
                    var userData = JsonConvert.DeserializeObject<UserData>(rawData.Value);
                    user.Set(userData);
                    
                    isSerialized.Set(false);
                }
                catch (Exception ex)
                {
                    client.Toast(ex.Message);
                }
            }

            //UseEffect(async () =>
            //{
            //    formBuilder = user.ToForm();
            //}, user);

            return
                Layout.Horizontal().Center()
                    | new Card(
                        Layout.Vertical()
                        | Text.H4("Simple Json Data")
                        | rawData.ToCodeInput(variant: CodeInputs.Default, language: Languages.Json).Height(80)
                            .Disabled(!isSerialized.Value)
                        )
                        .Width(Size.Half()).Height(150)
                    | new Button(isSerialized.Value ? "Deserialize" : "Serialize",
                        _ => HandleButtonClick())
                        .Icon(isSerialized.Value ? Icons.ArrowRight : Icons.ArrowLeft,
                            isSerialized.Value ? Align.Right : Align.Left)

                    | new Card(
                        Layout.Vertical()
                        | Text.H4(user.Value != null ? user.Value?.FullName : "No name")
                        | formBuilder
                    ).Width(Size.Half()).Height(150);
        }

    }
}
