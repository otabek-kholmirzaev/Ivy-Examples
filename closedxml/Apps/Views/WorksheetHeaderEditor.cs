using System.Data;
using ClosedXML.Excel;

public class WorksheetHeaderEditor : ViewBase
{
    public WorksheetHeaderEditor(DataTable table, IXLWorksheet worksheet)
    {
        Table = table;
        Worksheet = worksheet;
    }

    private DataTable Table { get; }
    private IXLWorksheet Worksheet { get; }

    public override object? Build()
    {
        var client = UseService<IClientProvider>();
        var workbookConnection = UseService<WorkbookConnection>();
        var workbookRepository = workbookConnection.GetWorkbookRepository();

        var columnName = UseState((string?)null);

        var columnTypes = new string[] { "int", "double", "decimal", "long", "string" };
        var selectedType = UseState("int");
        var refreshToken = this.UseRefreshToken();

        var addColumnButton = new Button("Add column", _ =>
        {
            Table.Columns.Add(columnName.Value, GetColumnTypeFromString(selectedType.Value));
            Console.WriteLine("ABB");
            refreshToken.Refresh();
        });

        var saveTable = new Button("Save table", _ =>
        {
            Worksheet.Clear();
            Worksheet.Cell(1, 1).InsertTable(Table);
            workbookRepository.Save();
            client.Toast("Changes saved!");
        });

        var viewToReturn = Layout.Vertical()
                | Text.Label("Select column type")
                | selectedType.ToSelectInput(columnTypes.ToOptions()).Variant(SelectInputs.Select)
                | new TextInput(columnName, placeholder: "New column name")
                | Layout.Horizontal(
                    [
                        addColumnButton,
                        saveTable
                    ]);


        Console.WriteLine($"Redraw: {Table.GetHashCode()}");

        return new HeaderLayout(
             header: new Card(viewToReturn)
                 .Title("Worksheet editor"),
             content: Layout.Vertical()
             | new RowEditorView(Table,refreshToken)
             | new Card(new WorksheetFileView(Table)));
    }

    private static Type GetColumnTypeFromString(string selectedType)
    {
        switch (selectedType)
        {
            case "string":
                return typeof(string);
            case "int":
                return typeof(int);
            case "double":
                return typeof(double);
            case "decimal":
                return typeof(decimal);
            case "long":
                return typeof(long);
            default:
                throw new ArgumentOutOfRangeException(selectedType, "Column type not supported!");
        }
    }
}
