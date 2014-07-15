using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;
using Epsitec.Common.IO;
using System.IO;

namespace Epsitec.Common.DocumentEditor.Dialogs
{
	/// <summary>
	/// Dialogue pour ouvrir un document existant.
	/// </summary>
	public class FileOpenModel : AbstractFile
	{
		public FileOpenModel(DocumentEditor editor) : base(editor)
		{
			this.title                   = Res.Strings.Dialog.Open.TitleMod;
			this.owner                   = this.editor.Window;
			this.FileExtension           = (editor.DocumentType == DocumentType.Pictogram) ? ".iconmod" : ".crmod";
			this.enableNavigation        = true;
			this.enableMultipleSelection = true;
			this.fileDialogType          = Epsitec.Common.Dialogs.FileDialogType.Open;
		}
	}
}
