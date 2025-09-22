using Aspose.BarCode.Generation;
using Aspose.BarCode.BarCodeRecognition;

namespace AsposeBarcodeDemo.Apps;

/// <summary>
/// Simple QR Code generator application
/// </summary>
[App(icon: Icons.QrCode, title: "QR Code Generator")]
public class AsposeBarcodeDemoApp : ViewBase
{
    public override object? Build()
    {
        // State management for the demo
        var userText = this.UseState("1234567");
        var selectedXDimension = this.UseState(15);
        var userSelectedColor = this.UseState("#000000");

        // Generate QR code download URL
        var downloadUrl = this.UseDownload(
            () =>
            {
                if (string.IsNullOrEmpty(userText.Value))
                    return Array.Empty<byte>();

                // Initialize BarcodeGenerator
                var gen = new BarcodeGenerator(EncodeTypes.QR, userText.Value);
                
                // Set generation parameters
                gen.Parameters.Barcode.XDimension.Pixels = selectedXDimension.Value;
                
                // Convert hex color to RGB components
                var colorHex = userSelectedColor.Value.Replace("#", "");
                var color = Aspose.Drawing.Color.FromArgb(
                    Convert.ToInt32(colorHex.Substring(0, 2), 16),
                    Convert.ToInt32(colorHex.Substring(2, 2), 16),
                    Convert.ToInt32(colorHex.Substring(4, 2), 16)
                );
                gen.Parameters.Border.Color = color;
                gen.Parameters.Border.Width.Pixels = 20;
                
                // Generate and save Barcode image
                using var ms = new MemoryStream();
                gen.Save(ms, BarCodeImageFormat.Png);
                return ms.ToArray();
            },
            "image/png",
            "image.png"
        );

        // Build the simple UI
        return new Card(
                Layout.Vertical().Gap(2).Padding(2)
                | Layout.Vertical().Gap(2)
                    | Text.H2("🤖 QR Code Generator")
                | Layout.Vertical().Gap(2)
                    | Text.H4("📝 Text:")
                    | userText.ToInput(placeholder: "Enter text for QR code")
                | Layout.Vertical().Gap(2)
                    | Text.H4("🖼️ X-dimension (1px—100px):")
                    | Layout.Horizontal().Gap(2)
                    | selectedXDimension.ToInput(placeholder: "10-100")
                | Layout.Vertical().Gap(2)
                    | Text.H4("🎨 Border color:")
                    | userSelectedColor.ToInput(placeholder: "#000000")
                | Layout.Vertical().Gap(2)
                    | Text.H4("✅ Image Format: Png")
                | new Button("Generate")
                    .Primary()
                    .Url(downloadUrl.Value)
              )
              .Width(Ivy.Shared.Size.Units(300));
    }
}