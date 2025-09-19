namespace IvyClosedXml.Apps;

[App(icon: Icons.ChartBar, path: ["Apps"])]
public class ExcelEditorApp : ViewBase
{
    public override object? Build()
    {
        var client = UseService<IClientProvider>();
        var refreshToken = this.UseRefreshToken();
        
        return new HeaderLayout(
         header: new Card(new FileCreateDialog(refreshToken))
             .Title("Excel file editor"),
         content: Layout.Vertical().Gap(4)
             | Text.P("The header above remains fixed while content scrolls.")
             | Text.P("Add as much content as needed.")
             | new ExcelFileView()
      );
    }
}
