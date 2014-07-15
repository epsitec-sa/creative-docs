using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using System.IO;
using Epsitec.Common.Dialogs;

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
			this.enableNavigation        = true;
			this.enableMultipleSelection = false;
			this.fileDialogType          = FileDialogType.Open;

			this.Filters.Add (new FilterItem ("x", "Image", "*.tif|*.jpg|*.gif|*.png|*.bmp|*.wmf|*.emf|*.tiff|*.jpeg"));
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
