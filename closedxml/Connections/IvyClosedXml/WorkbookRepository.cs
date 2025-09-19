using System.Collections.ObjectModel;
using ClosedXML.Excel;

/// <summary>
/// Repository class for managing Excel Workbooks 
/// by creating, removing and storing current workbook on which operations are made
/// </summary>
public class WorkbookRepository
{
    private List<WorkbookFileInfo> excelFiles { get; set; }
    private WorkbookFileInfo currentFile { get; set; }
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

    public ReadOnlyCollection<WorkbookFileInfo> GetFiles()
    {
        return new ReadOnlyCollection<WorkbookFileInfo>(excelFiles);
    }

    internal void AddNewFile(string fileName)
    {
        ValidateFileName(fileName);

        if (ContainsFile(fileName))
            throw new ArgumentException("File name already exists in the collection.");

        excelFiles.Add(new WorkbookFileInfo(fileName, new XLWorkbook()));
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
}