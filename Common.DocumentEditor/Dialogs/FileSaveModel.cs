using Epsitec.Common.Dialogs;
using Epsitec.Common.Document;

namespace Epsitec.Common.DocumentEditor.Dialogs
{
    /// <summary>
    /// Dialogue pour ouvrir un document existant.
    /// </summary>
    public class FileSaveModel : AbstractFile
    {
        public FileSaveModel(DocumentEditor editor)
            : base(editor)
        {
            this.owner = this.editor.Window;
            this.title = Res.Strings.Dialog.Save.TitleMod;
            this.enableNavigation = true;
            this.enableMultipleSelection = false;
            this.hasOptions = false;
            this.fileDialogType = FileDialogType.Save;

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
