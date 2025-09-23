## Aspose.BarCode QR Demo

This example shows a minimal, focused integration of **Aspose.BarCode for .NET** inside an **Ivy** application. 
It implements a simple QR code generator with adjustable module (X-dimension) size and a validated border color.

> Goal: keep code tiny, idiomatic, and easy to extend for additional symbologies later.

### Features

- Generate QR codes from arbitrary user text.
- Configure X-dimension (module size) in pixels.
- Enter a HEX color (`#RRGGBB` or `#AARRGGBB`) for the border – input is validated with graceful fallback.
- One-click download of the generated PNG.
- Example of safe color parsing logic (no unhandled exceptions on bad input).

### UI Preview (Conceptual)

```
🤖 QR Code Generator
Text: [ 1234567               ]
X-dimension: [15]
Border color: [#000000]
[Generate]
```

### Core Code (Excerpt)

```csharp
using var gen = new BarcodeGenerator(EncodeTypes.QR, userText.Value);
gen.Parameters.Barcode.XDimension.Pixels = selectedXDimension.Value;
gen.Parameters.Border.Color = color;          // parsed + validated
gen.Parameters.Border.Width.Pixels = 20;

using var ms = new MemoryStream();
gen.Save(ms, BarCodeImageFormat.Png);
return ms.ToArray();
```

Color parsing is defensive:

```csharp
var raw = userSelectedColor.Value?.Trim();
if (string.IsNullOrEmpty(raw)) color = Color.Black; else {
    if (raw.StartsWith("#")) raw = raw[1..];
    if (raw.Length is not (6 or 8) || !raw.All(Uri.IsHexDigit)) color = Color.Black; else { /* parse */ }
}
```

### Run

```powershell
dotnet watch
```

### Deploy

```powershell
ivy deploy
```

### Dependencies

- [Aspose.BarCode](https://products.aspose.com/barcode/net)
- [Ivy Framework](https://github.com/Ivy-Interactive/Ivy)

### Resources

- Ivy Docs: https://docs.ivy.app
- Aspose.BarCode Docs: https://docs.aspose.com/barcode/net/
- Samples: https://samples.ivy.app
- Community: https://discord.gg/sSwGzZAYb6