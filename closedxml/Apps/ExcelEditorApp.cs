namespace IvyClosedXml.Apps;

[App(icon: Icons.ChartBar, path: ["Apps"])]
public class ExcelEditorApp : ViewBase
{
    public override object? Build()
    {
        var refreshToken = this.UseRefreshToken();
        return new HeaderLayout(
         header: new Card(new FileCreateDialog(refreshToken))
             .Title("Excel file editor"),
         content: Layout.Vertical().Gap(1)
             | new ExcelFileSheets()
      );
    }
}
