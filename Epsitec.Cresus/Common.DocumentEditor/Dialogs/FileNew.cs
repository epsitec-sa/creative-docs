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
	/// Dialogue pour créer un nouveau document.
	/// </summary>
	public class FileNew : AbstractFile
	{
		public FileNew(DocumentEditor editor) : base(editor)
		{
			this.title                   = Res.Strings.Dialog.New.Title;
			this.owner                   = this.editor.Window;
			this.FileExtension           = (editor.DocumentType == DocumentType.Pictogram) ? ".iconmod" : ".crmod";
			this.enableNavigation        = true;
			this.enableMultipleSelection = false;
			this.fileDialogType          = Epsitec.Common.Dialogs.FileDialogType.New;
		}
	}
}
