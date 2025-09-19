using ClosedXML.Excel;

public class WorkbookFileInfo(string fileName, XLWorkbook xLWorkbook) : IDisposable
{
    public string FileName { get; } = fileName;
    public XLWorkbook Workbook { get; } = xLWorkbook;

    public void Dispose()
    {
        Workbook?.Dispose();
        GC.SuppressFinalize(this);
    }
}