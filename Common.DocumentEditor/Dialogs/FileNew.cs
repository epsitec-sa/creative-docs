using Epsitec.Common.Dialogs;
using Epsitec.Common.Document;

namespace Epsitec.Common.DocumentEditor.Dialogs
{
    /// <summary>
    /// Dialogue pour créer un nouveau document.
    /// </summary>
    public class FileNew : AbstractFile
    {
        public FileNew(DocumentEditor editor)
            : base(editor)
        {
            this.title = Res.Strings.Dialog.New.Title;
            this.owner = this.editor.Window;
            this.enableNavigation = true;
            this.enableMultipleSelection = false;
            this.fileDialogType = FileDialogType.New;

            if (editor.DocumentType == DocumentType.Pictogram)
            {
                this.Filters.Add(new FilterItem("x", Res.Strings.Dialog.File.Model, ".iconmod"));
            }
            else
            {
                this.Filters.Add(new FilterItem("x", Res.Strings.Dialog.File.Model, ".crmod"));
            }
        }
    }
}
