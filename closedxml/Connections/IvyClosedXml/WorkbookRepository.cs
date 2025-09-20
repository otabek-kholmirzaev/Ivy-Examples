using System.Collections.ObjectModel;
using System.Data;
using ClosedXML.Excel;

/// <summary>
/// Repository class for managing Excel Workbooks 
/// by creating, removing and storing current workbook on which operations are made
/// </summary>
public class WorkbookRepository
{
    private List<WorkbookFileInfo> excelFiles { get; set; }
    private WorkbookFileInfo? currentFile { get; set; }
    public WorkbookRepository()
    {
        excelFiles = new List<WorkbookFileInfo>();
    }

    /// <summary>
    /// Set file on which operations are made.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public void SetCurrentFile(string fileName)
    {
        currentFile = GetFileByName(fileName);
    }

    public WorkbookFileInfo GetCurrentFile()
    {
        return currentFile;
    }

    public DataTable GetCurrentTable()
    {
        var table = Worksheet.Tables.FirstOrDefault();

        Console.WriteLine($"GetCurrentTable is {table}");
        if (table == null)
            return new DataTable() { TableName = "FirstTable" };

        return table.AsNativeDataTable();
    }

    public ReadOnlyCollection<WorkbookFileInfo> GetFiles()
    {
        return new ReadOnlyCollection<WorkbookFileInfo>(excelFiles);
    }

    internal void AddNewFile(string fileName)
    {
        ValidateFileName(fileName);

        if (ContainsFile(fileName))
            throw new ArgumentException("File name already exists in the collection.");

        excelFiles.Add(new WorkbookFileInfo(fileName));
    }

    private void ValidateFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentOutOfRangeException("File name cannot be empty!");

        char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
        if (fileName.Any(c => invalidChars.Contains(c)))
            throw new ArgumentOutOfRangeException("Invalid characters in file name!");

        // Check for trailing periods or spaces
        if (fileName.EndsWith(".") || fileName.EndsWith(" "))
            throw new ArgumentOutOfRangeException("File ends with invalid character!");
    }

    internal void RemoveFile(string fileName)
    {
        if (ContainsFile(fileName))
        {
            var fileToRemove = GetFileByName(fileName);
            fileToRemove.Workbook.Dispose();
            excelFiles.Remove(fileToRemove);

            if (excelFiles.Count == 0)
            {
                currentFile = null;
            }
        }
    }

    private bool ContainsFile(string fileName)
    {
        return excelFiles.Any(f => f.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
    }

    private WorkbookFileInfo GetFileByName(string fileName)
    {
        return excelFiles.Single(f => f.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
    }

    internal void Save(DataTable table)
    {
        TryRemoveExistingTable();

        Worksheet
        .Cell(1, 1)
        .InsertTable(table, "FirstTable", true);

        GetCurrentFile().Workbook.SaveAs(currentFile.FileName + ".xlsx");
    }


    private IXLWorksheet Worksheet => currentFile.Workbook.Worksheets.FirstOrDefault();
    
    private void TryRemoveExistingTable()
    {
        try
        {
            var table = Worksheet.Tables.FirstOrDefault(f => f.Name == "FirstTable");

            Console.WriteLine($"Table is {table}");

            if (table != null)
            {
                Worksheet.Tables.Remove(0);
                table.Clear(XLClearOptions.All);
                Worksheet.Clear(XLClearOptions.All);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }
    }
}