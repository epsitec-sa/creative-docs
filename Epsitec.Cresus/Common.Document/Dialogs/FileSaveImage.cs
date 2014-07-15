using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using System.IO;

namespace Epsitec.Common.Document.Dialogs
{
	/// <summary>
	/// Dialogue pour exporter une image bitmap.
	/// </summary>
	public class FileSaveImage : AbstractFile
	{
		public FileSaveImage(Document document, Window ownerWindow) : base(document, ownerWindow)
		{
			this.title                   = Res.Strings.Dialog.SaveImage.Title;
			this.owner                   = this.ownerWindow;
			this.FileFilterPattern       = "*.bmp|*.tif|*.tiff|*.jpg|*.jpeg|*.gif|*.png|*.wmf|*.emf";
			this.enableNavigation        = true;
			this.enableMultipleSelection = false;
			this.fileDialogType          = Epsitec.Common.Dialogs.FileDialogType.Save;
		}


		protected override string ActionButtonName
		{
			get
			{
				return Res.Strings.Dialog.SaveImage.ActionButtonName;
			}
		}

		protected override void FavoritesAddApplicationFolders()
		{
			this.AddFavorite(FolderId.MyPictures);
		}
	}
}
