using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;
using Epsitec.Common.IO;
using System.IO;
using Epsitec.Common.Dialogs;

namespace Epsitec.Common.DocumentEditor.Dialogs
{
	/// <summary>
	/// Dialogue pour ouvrir un document existant.
	/// </summary>
	public class FileSaveModel : AbstractFile
	{
		public FileSaveModel(DocumentEditor editor) : base(editor)
		{
			this.owner                   = this.editor.Window;
			this.title                   = Res.Strings.Dialog.Save.TitleMod;
			this.enableNavigation        = true;
			this.enableMultipleSelection = false;
			this.hasOptions              = false;
			this.fileDialogType          = FileDialogType.Save;

			if (editor.DocumentType == DocumentType.Pictogram)
			{
				this.Filters.Add (new FilterItem ("x", Res.Strings.Dialog.File.Model, ".iconmod"));
			}
			else
			{
				this.Filters.Add (new FilterItem ("x", Res.Strings.Dialog.File.Model, ".crmod"));
			}
		}
	}
}
