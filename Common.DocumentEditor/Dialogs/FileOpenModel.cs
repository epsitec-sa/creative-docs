using Epsitec.Common.Dialogs;
using Epsitec.Common.Document;

namespace Epsitec.Common.DocumentEditor.Dialogs
{
    /// <summary>
    /// Dialogue pour ouvrir un document existant.
    /// </summary>
    public class FileOpenModel : AbstractFile
    {
        public FileOpenModel(DocumentEditor editor)
            : base(editor)
        {
            this.title = Res.Strings.Dialog.Open.TitleMod;
            this.owner = this.editor.Window;
            this.enableNavigation = true;
            this.enableMultipleSelection = true;
            this.fileDialogType = FileDialogType.Open;

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
