//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Dialogs;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Data;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SpecialControllers
{
	/// <summary>
	/// Ce contrôleur affiche les boutons d'action pour une image bitmap.
	/// </summary>
	public class SpecialImageActionsController : IEntitySpecialController
	{
		public SpecialImageActionsController(TileContainer tileContainer, ImageEntity imageEntity)
		{
			this.tileContainer = tileContainer;
			this.imageEntity = imageEntity;
		}


		public void CreateUI(Widget parent, UIBuilder builder, bool isReadOnly)
		{
			this.isReadOnly = isReadOnly;

			var controller = this.tileContainer.EntityViewController;
			this.businessContext = controller.BusinessContext;
			this.dataContext = controller.DataContext;
			this.coreData = controller.Data;

			var frameBox = parent as FrameBox;
			System.Diagnostics.Debug.Assert (frameBox != null);

			var importButton = builder.CreateButton (frameBox, 0, null, "Importer une image...");
			ToolTip.Default.SetToolTip (importButton, "Importe l'image à partir d'un fichier BMP, TIF, PNG, JPG ou GIF");

			importButton.Clicked += delegate
			{
				this.ImportImage ();
			};

			if (this.HasUpdateImageButton)
			{
				var updateButton = builder.CreateButton (frameBox, 0, null, "Mettre à jour l'image");
				ToolTip.Default.SetToolTip (updateButton, "Met à jour l'image si le fichier a changé");

				updateButton.Clicked += delegate
				{
					this.UpdateImage ();
				};
			}
		}


		private bool HasUpdateImageButton
		{
			get
			{
				if (this.imageEntity.ImageBlob.IsNull ())
				{
					return false;
				}

				var builder = new Epsitec.Common.IO.UriBuilder (this.imageEntity.ImageBlob.FileUri);

				string userName = System.Environment.UserName.ToLowerInvariant ();
				string host     = System.Environment.MachineName.ToLowerInvariant ();

				return (builder.UserName == userName &&
						builder.Host     == host &&
						System.IO.File.Exists (builder.Path));
			}
		}

		private void UpdateImage()
		{
			var builder = new Epsitec.Common.IO.UriBuilder (this.imageEntity.ImageBlob.FileUri);

			this.ImportImage (builder.Path);
		}

		private void ImportImage()
		{
			var filename = this.OpenFileDialog (this.imageEntity.ImageBlob.FileUri);

			if (!string.IsNullOrWhiteSpace (filename))
			{
				this.ImportImage (filename);
			}
		}

		private string OpenFileDialog(string uri)
		{
			//	Exemple de contenu pour uri:
			//	"file://daniel@daniel-pc/C:/Users/Daniel/Documents/t.jpg"
			var dialog = new FileOpenDialog ();

			var builder = new Epsitec.Common.IO.UriBuilder (uri);
			// builder.Scheme     == "file"
			// builder.UserName   == "daniel"
			// builder.Host       == "daniel-pc"
			// builder.Path       == "C:/Users/Daniel/Documents/t.jpg"
			// builder.Fragment   == null
			// builder.Password   == null
			// builder.Query      == null
			// builder.PortNumber == 0

			if (builder.Scheme == "file")
			{
				string directory = System.IO.Path.GetDirectoryName (builder.Path);
				string filename  = System.IO.Path.GetFileName (builder.Path);

				string userName = System.Environment.UserName.ToLowerInvariant ();
				string host     = System.Environment.MachineName.ToLowerInvariant ();

				if (builder.UserName == userName &&
					builder.Host     == host &&
					System.IO.Directory.Exists (directory))
				{
					dialog.InitialDirectory = directory;
				}

				dialog.FileName = filename;
			}

			dialog.Title = "Importation d'une image bitmap";

			dialog.Filters.Add ("image", "Image", "*.bmp;*.tif;*.png;*.jpg;*.gif");
			dialog.Filters.Add ("any", "Tous les fichiers", "*.*");

			dialog.AcceptMultipleSelection = false;
			dialog.OwnerWindow = this.tileContainer.Window;
			dialog.OpenDialog ();

			return (dialog.Result == DialogResult.Accept) ? dialog.FileName : null;
		}

		private void ImportImage(string filename)
		{
			var file = new System.IO.FileInfo (filename);
			var store = this.coreData.GetComponent<ImageDataStore> ();

			store.UpdateImage (this.dataContext, this.imageEntity, file);

			if (this.imageEntity.Name.IsNullOrEmpty)
			{
				this.imageEntity.Name = System.IO.Path.GetFileNameWithoutExtension (this.imageEntity.ImageBlob.FileName);
			}
		}


		private class Factory : DefaultEntitySpecialControllerFactory<ImageEntity>
		{
			protected override IEntitySpecialController Create(TileContainer container, ImageEntity entity, int mode)
			{
				return new SpecialImageActionsController (container, entity);
			}
		}

	
		private readonly TileContainer tileContainer;
		private readonly ImageEntity imageEntity;

		private bool isReadOnly;
		private BusinessContext businessContext;
		private DataContext dataContext;
		private CoreData coreData;
	}
}
