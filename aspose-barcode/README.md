# Aspose.BarCode Demo

An interactive web application showcasing the integration of [Aspose.BarCode](https://products.aspose.com/barcode/net) with the [Ivy Framework](https://github.com/Ivy-Interactive/Ivy).

## Features

This demo application demonstrates the core capabilities of Aspose.BarCode:

- **Barcode Generation**: Create barcodes in multiple symbologies including:
  - QR Code
  - Code 128
  - Data Matrix
  - PDF417
  - EAN13
  - Code 39
  - Aztec
  - Codabar

- **Customization Options**:
  - Configurable barcode properties (size, text display)
  - Different sample data for each barcode type

- **Sample Gallery**: Display examples of all supported barcode types with sample data

- **Export Functionality**: Download generated barcodes as PNG images

## Technical Implementation

The application showcases how to integrate Aspose.BarCode with Ivy's reactive UI framework:

- **State Management**: Uses Ivy's `UseState` for reactive UI updates
- **File Handling**: Demonstrates file upload and download capabilities
- **Image Processing**: Real-time barcode generation and recognition
- **Clean Architecture**: Follows Ivy conventions with proper separation of concerns

## Code Highlights

```csharp
// Barcode generation with custom properties
using var generator = new BarcodeGenerator(barcodeType, barcodeText.Value);
generator.Parameters.Barcode.XDimension.Pixels = 4;
generator.Parameters.Barcode.BarHeight.Pixels = barcodeSize.Value;
generator.Parameters.Barcode.CodeTextParameters.Location = showText.Value 
    ? CodeLocation.Below 
    : CodeLocation.None;

// Barcode recognition from uploaded image
using var reader = new BarCodeReader(stream, DecodeType.AllSupportedTypes);
foreach (var result in reader.ReadBarCodes())
{
    results.Add($"Type: {result.CodeTypeName}, Text: {result.CodeText}");
}
```

## Run

```bash
dotnet watch
```

The application will start on `https://localhost:5001` with hot reload enabled for development.

## Usage

1. **Generate Barcodes**:
   - The application shows current settings for barcode generation
   - Click "Download Current Barcode" to generate and save a barcode
   - Customize the barcode by modifying the code and rebuilding

2. **Explore Sample Gallery**:
   - View examples of all supported barcode types
   - See how different data formats work with each symbology
   - Download sample barcodes for testing

## Deploy

```bash
ivy deploy
```

## Dependencies

- **Ivy Framework**: Web framework for building interactive applications
- **Aspose.BarCode**: Comprehensive barcode generation and recognition library

## Resources

- [Ivy Framework Documentation](https://docs.ivy.app)
- [Aspose.BarCode Documentation](https://docs.aspose.com/barcode/net/)
- [Ivy Examples](https://samples.ivy.app)
- [Ivy Discord Community](https://discord.gg/sSwGzZAYb6)