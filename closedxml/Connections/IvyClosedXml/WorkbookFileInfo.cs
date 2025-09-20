using ClosedXML.Excel;

public class WorkbookFileInfo : IDisposable
{

    public WorkbookFileInfo(string fileName)
    {
        FileName = fileName;
        Workbook = new XLWorkbook();
        Workbook.AddWorksheet("Sheet1");
    }

    public string FileName { get; }
    public XLWorkbook Workbook { get; }

    public void Dispose()
    {
        Workbook?.Dispose();
        GC.SuppressFinalize(this);
    }
}