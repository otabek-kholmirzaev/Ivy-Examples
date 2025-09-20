public class FileCreateDialog : ViewBase
{
    public RefreshToken RefreshToken { get; }

    public FileCreateDialog(RefreshToken refreshToken)
    {
        RefreshToken = refreshToken;
    }

    public override object? Build()
    {
        var workbookConnection = UseService<WorkbookConnection>();
        var workbookRepository = workbookConnection.GetWorkbookRepository();

        var fileName = UseState((string?)null);
        var selectedCountry = this.UseState<string?>(default(string));
        var client = UseService<IClientProvider>();
        var existingFiles = UseState(GetFilesFromRepo(workbookRepository));
        var selectedFile = UseState("");

        UseEffect(() =>
        {
            try
            {
                workbookRepository.SetCurrentFile(selectedFile.Value);
                //call Refresh so that ExcelFileView will redraw with the newly selected file
                RefreshToken.Refresh();
            }
            catch (Exception e)
            {
                client.Toast(e);
            }
        },[selectedFile]);

        var toolbar =
            Layout.Horizontal().Gap(4)
                | fileName.ToTextInput(placeholder: "File name")
                | new Button("Add file")
                    .Icon(Icons.Plus)
                    .Variant(ButtonVariant.Primary)
                    .HandleClick(() => HandleAddFile(workbookRepository, fileName, existingFiles, selectedFile))
                | new Button("Export")
                    .Icon(Icons.Download)
                    .Variant(ButtonVariant.Ghost)
                    .HandleClick(_ => client.Toast("Export clicked"))
                | selectedFile
                .ToSelectInput(existingFiles.Value.ToOptions(), placeholder: "Selected file")
                .Variant(SelectInputs.Select)
                | new Button("Delete file",
                () => HandleDeleteFile(workbookRepository, existingFiles, selectedFile))
                .Destructive();

        return toolbar;
    }

    private void HandleAddFile(WorkbookRepository workbookRepository, IState<string> fileName, IState<List<string>> existingFiles, IState<string> selectedFile)
    {
        workbookRepository.AddNewFile(fileName.Value);
        existingFiles.Set(GetFilesFromRepo(workbookRepository));
        if (existingFiles.Value.Count == 1)
        {
            selectedFile.Set(existingFiles.Value[0]);
            workbookRepository.SetCurrentFile(existingFiles.Value[0]);
        }
        
        RefreshToken.Refresh();
    }

    private void HandleDeleteFile(WorkbookRepository workbookRepository, IState<List<string>> existingFiles, IState<string> selectedFile)
    {
        if (existingFiles.Value.Contains(selectedFile.Value))
        {
            workbookRepository.RemoveFile(selectedFile.Value);
            existingFiles.Set(GetFilesFromRepo(workbookRepository));
        }

        if (existingFiles.Value.Count >= 1)
        {
            selectedFile.Set(existingFiles.Value[0]);
            workbookRepository.SetCurrentFile(existingFiles.Value[0]);
        }
        RefreshToken.Refresh();
    }

    public List<string> GetFilesFromRepo(WorkbookRepository workbookRepository)
    {
        return workbookRepository.GetFiles().Select(f => f.FileName).ToList();
    }
}