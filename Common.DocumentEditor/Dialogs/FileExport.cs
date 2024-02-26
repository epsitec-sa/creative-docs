namespace Epsitec.Common.DocumentEditor.Dialogs
{
    /// <summary>
    /// Dialogue pour exporter un fichier.
    /// </summary>
    public class FileExport : AbstractFile
    {
        public FileExport(DocumentEditor editor)
            : base(editor)
        {
            this.owner = this.editor.Window;
            this.enableNavigation = true;
            this.enableMultipleSelection = false;
            this.hasOptions = false;
            this.fileDialogType = Epsitec.Common.Dialogs.FileDialogType.Save;
        }

        protected override string ActionButtonName
        {
            get { return Res.Strings.Dialog.Export.Button.OK; }
        }
    }
}
