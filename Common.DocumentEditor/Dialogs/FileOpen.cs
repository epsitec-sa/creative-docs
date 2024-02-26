using Epsitec.Common.Dialogs;
using Epsitec.Common.Document;

namespace Epsitec.Common.DocumentEditor.Dialogs
{
    /// <summary>
    /// Dialogue pour ouvrir un document existant.
    /// </summary>
    public class FileOpen : AbstractFile
    {
        public FileOpen(DocumentEditor editor)
            : base(editor)
        {
            this.title = Res.Strings.Dialog.Open.TitleDoc;
            this.owner = this.editor.Window;
            this.enableNavigation = true;
            this.enableMultipleSelection = true;
            this.fileDialogType = FileDialogType.Open;

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
