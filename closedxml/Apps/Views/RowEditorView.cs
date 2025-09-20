using System.Data;

public class RowEditorView : ViewBase
{
    public RowEditorView(DataTable table, RefreshToken refreshToken)
    {
        Table = table;
        RefreshToken = refreshToken;
        inputsForRowData = new List<TextInput>();
    }
    private DataTable Table { get; }
    private RefreshToken RefreshToken { get; }
    private List<TextInput> inputsForRowData;

    public override object? Build()
    {
        inputsForRowData = new List<TextInput>();
        var dataColumns = Table.Columns.Cast<DataColumn>().ToList();

        foreach (var col in dataColumns)
        {
            var inputState = UseState((string)null);
            var txtInput = new TextInput(inputState, placeholder: col.ColumnName);
            inputsForRowData.Add(txtInput);
        }

        return
            new StackLayout(inputsForRowData.ToArray(), Orientation.Horizontal)
        | new Button("Add row", _ =>
        {
            var newRow = inputsForRowData.Select(input => input.Value).ToArray();
            Console.WriteLine("In button");
            Table.Rows.Add(newRow);
            RefreshToken.Refresh();
        });
    }
}