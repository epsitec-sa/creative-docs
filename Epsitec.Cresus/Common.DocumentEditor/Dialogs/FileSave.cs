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
	public class FileSave : AbstractFile
	{
		public FileSave(DocumentEditor editor) : base(editor)
		{
			this.owner                   = this.editor.Window;
			this.title                   = Res.Strings.Dialog.Save.TitleDoc;
			this.FileExtension           = (editor.DocumentType == DocumentType.Pictogram) ? ".icon" : ".crdoc";
			this.enableNavigation        = true;
			this.enableMultipleSelection = false;
			this.hasOptions              = false;
			this.fileDialogType          = Epsitec.Common.Dialogs.FileDialogType.Save;
		}
	}
}
