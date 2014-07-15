using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using System.IO;

namespace Epsitec.Common.Document.Dialogs
{
	/// <summary>
	/// Dialogue pour importer une image bitmap.
	/// </summary>
	public class FileOpenImage : AbstractFile
	{
		public FileOpenImage(Document document, Window ownerWindow) : base(document, ownerWindow)
		{
			//	Il faut mettre en premier les extensions qu'on souhaite voir.
			this.title                   = Res.Strings.Dialog.OpenImage.Title;
			this.owner                   = this.ownerWindow;
			this.FileFilterPattern       = "*.tif|*.jpg|*.gif|*.png|*.bmp|*.wmf|*.emf|*.tiff|*.jpeg";
			this.enableNavigation        = true;
			this.enableMultipleSelection = false;
			this.fileDialogType          = Epsitec.Common.Dialogs.FileDialogType.Open;
		}


		protected override string ActionButtonName
		{
			get
			{
				return Res.Strings.Dialog.OpenImage.ActionButtonName;
			}
		}

		protected override void FavoritesAddApplicationFolders()
		{
			this.AddFavorite(FolderId.MyPictures);
		}
	}
}
