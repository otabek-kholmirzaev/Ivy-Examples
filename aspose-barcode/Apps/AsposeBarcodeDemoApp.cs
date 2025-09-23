using Aspose.BarCode.Generation;

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
                using var gen = new BarcodeGenerator(EncodeTypes.QR, userText.Value);
                
                // Set generation parameters
                gen.Parameters.Barcode.XDimension.Pixels = selectedXDimension.Value;
                
                // Convert hex color to RGB components
                var raw = userSelectedColor.Value?.Trim();
                Aspose.Drawing.Color color;

                if (string.IsNullOrEmpty(raw))
                {
                    color = Aspose.Drawing.Color.Black; // fallback
                }
                else
                {
                    if (raw.StartsWith("#"))
                        raw = raw[1..];

                    // Accept only 6 (RRGGBB) or 8 (AARRGGBB)
                    if (raw.Length is not (6 or 8) || !raw.All(Uri.IsHexDigit))
                    {
                        color = Aspose.Drawing.Color.Black; // fallback
                    }
                    else
                    {
                        int idx = 0;
                        int ReadComponent() => Convert.ToInt32(raw.Substring(idx, 2), 16);

                        int a, r, g, b;
                        if (raw.Length == 8)
                        {
                            a = ReadComponent(); idx += 2;
                        }
                        else
                        {
                            a = 255;
                        }
                        
                        r = ReadComponent(); idx += 2;
                        g = ReadComponent(); idx += 2;
                        b = ReadComponent();

                        color = Aspose.Drawing.Color.FromArgb(a, r, g, b);
                    }
                }

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
              .Width(Size.Units(300));
    }
}