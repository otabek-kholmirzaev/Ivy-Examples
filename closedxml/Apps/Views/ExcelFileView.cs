public class ExcelFileView() : ViewBase
{
    public override object? Build()
    {
        var workbookConnection = UseService<WorkbookConnection>();
        var workbookRepository = workbookConnection.GetWorkbookRepository();
        var client = UseService<IClientProvider>();
        var currentFile = workbookRepository.GetCurrentFile();

        

        return currentFile?.FileName;
    }
}