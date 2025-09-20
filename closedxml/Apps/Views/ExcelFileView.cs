
using System.Data;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components.Forms;

public class ExcelFileView() : ViewBase
{
    public override object? Build()
    {
        var workbookConnection = UseService<WorkbookConnection>();
        var workbookRepository = workbookConnection.GetWorkbookRepository();

        var currentFile = workbookRepository.GetCurrentFile();
        if (currentFile == null)
            return null;
        var tabs = new List<Tab>();

        foreach (var w in currentFile.Workbook.Worksheets)
        {
            var t = new Tab(w.Name, new WorksheetEditor(w.Worksheet));
            tabs.Add(t);
        }

        return Layout.Tabs(tabs.ToArray())
        .Variant(TabsVariant.Tabs);
    }
}

public class WorksheetEditor : ViewBase
{
    public WorksheetEditor(IXLWorksheet worksheet)
    {
        Worksheet = worksheet;
    }

    private IXLWorksheet Worksheet { get; }

    public override object? Build()
    {
        var button = new Button("Add column");
        var inputText = new InputText();
        var client = UseService<IClientProvider>();
        var withoutValue = UseState((string?)null);
        
        var dataTable = new DataTable();
        var columnTypes = new string[]{"int","double","decimal","long","string"};

        var selectedType = UseState("int");

        var t = Layout.Vertical()
                | Ivy.Views.Text.Label("Select column type")
                | selectedType.ToSelectInput(columnTypes.ToOptions()).Variant(SelectInputs.Select)
                | new TextInput(withoutValue, placeholder: "New column name")
                | new Button("Add column", _ => dataTable.Columns.Add(withoutValue.Value, typeof(string)));
   

        return new HeaderLayout(
         header: new Card(t)
             .Title("Worksheet editor"),
         content: Layout.Vertical().Gap(1)
             | "Test 233");
    }
}