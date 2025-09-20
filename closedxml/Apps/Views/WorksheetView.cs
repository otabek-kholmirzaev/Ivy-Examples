using System.Data;

public class WorksheetFileView : ViewBase
{
    public WorksheetFileView(DataTable table)
    {
        Table = table;
    }

    public DataTable Table { get; }

    public override object? Build()
    {
        //var client = UseService<IClientProvider>();
        var rows = Table.AsEnumerable().ToList();
        // client.Toast($"Rows saved: {rows.Count}");
        // client.Toast($"Columns saved: {Table.Columns.Count}");
        var tableColumns = Table.Columns.Cast<DataColumn>().ToList();
        var listOfTableRows = new List<TableRow>();
        var cols = new List<TableCell>();

        foreach (var col in tableColumns)
            cols.Add(new TableCell(col.ColumnName).IsHeader());

        listOfTableRows.Add(new TableRow([.. cols]));

        foreach (var row in rows)
        {
            var listOfTableCells = new List<TableCell>();
            foreach (var rowValue in row.ItemArray)
            {
                listOfTableCells.Add(new TableCell(rowValue.ToString()));
            }
                
            listOfTableRows.Add(new TableRow([.. listOfTableCells]));
        }
        Console.WriteLine($"Redraw in WorksheetFileView: Table hashcode: {Table.GetHashCode()}");
        return new Table([.. listOfTableRows]);
    }
}