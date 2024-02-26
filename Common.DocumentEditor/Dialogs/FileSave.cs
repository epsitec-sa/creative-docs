using Epsitec.Common.Dialogs;
using Epsitec.Common.Document;

namespace Epsitec.Common.DocumentEditor.Dialogs
{
    /// <summary>
    /// Dialogue pour ouvrir un document existant.
    /// </summary>
    public class FileSave : AbstractFile
    {
        public FileSave(DocumentEditor editor)
            : base(editor)
        {
            this.owner = this.editor.Window;
            this.title = Res.Strings.Dialog.Save.TitleDoc;
            this.enableNavigation = true;
            this.enableMultipleSelection = false;
            this.hasOptions = false;
            this.fileDialogType = FileDialogType.Save;

            if (editor.DocumentType == DocumentType.Pictogram)
            {
                this.Filters.Add(new FilterItem("x", Res.Strings.Dialog.File.Icon, ".icon"));
            }
            else
            {
                this.Filters.Add(new FilterItem("x", Res.Strings.Dialog.File.Document, ".crdoc"));
            }
        }
    }
}
