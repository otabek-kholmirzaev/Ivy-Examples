public class ExcelFileSheets() : ViewBase
{
    public override object? Build()
    {
        var workbookConnection = UseService<WorkbookConnection>();
        var workbookRepository = workbookConnection.GetWorkbookRepository();

        var currentFile = workbookRepository.GetCurrentFile();
        Console.WriteLine($"Current File {currentFile.FileName}");

        if (currentFile == null)
            return null;

        var tabs = new List<Tab>();

        foreach (var worksheet in currentFile.Workbook.Worksheets)
        {
            Console.WriteLine($"Worksheet: {worksheet.Worksheet.Name}");
            var tab = new Tab(worksheet.Name, new WorksheetHeaderEditor(workbookRepository.GetCurrentTable()));
            tabs.Add(tab);
        }

        return
        Layout.Tabs([.. tabs])
        .Variant(TabsVariant.Tabs);
    }
}
