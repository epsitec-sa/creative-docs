//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;
using Epsitec.Common.Dialogs;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionImageViewController : EditionViewController<Entities.ImageEntity>
	{
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.Image", "Image");

				this.CreateUIImport   (builder);
				this.CreateUIMain     (builder);
				this.CreateUIGroup    (builder);
				this.CreateUICategory (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateUIImport(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin (tile, horizontalSeparator: false);
			builder.CreateMargin (tile, horizontalSeparator: false);

			var importButton = builder.CreateButton (tile, 0, null, "Importer une image...");
			ToolTip.Default.SetToolTip (importButton, "Importe l'image à partir d'un fichier BMP, TIF, PNG, JPG ou GIF");

			importButton.Clicked += delegate
			{
				this.ImportImage ();
			};

			if (this.HasUpdateImageButton)
			{
				var updateButton = builder.CreateButton (tile, 0, null, "Mettre à jour l'image");
				ToolTip.Default.SetToolTip (updateButton, "Met à jour l'image si le fichier a changé");

				updateButton.Clicked += delegate
				{
					this.UpdateImage ();
				};
			}
		}

		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField      (tile,   0, "Nom",         Marshaler.Create (() => this.Entity.Name,        x => this.Entity.Name = x));
			builder.CreateTextFieldMulti (tile, 100, "Description", Marshaler.Create (() => this.Entity.Description, x => this.Entity.Description = x));
		}

		private void CreateUIGroup(UIBuilder builder)
		{
			var controller = new SelectionController<ImageGroupEntity> (this.BusinessContext)
			{
				CollectionValueGetter    = () => this.Entity.ImageGroups,
				ToFormattedTextConverter = x => TextFormatter.FormatText (x.Name).IfNullOrEmptyReplaceWith (CollectionTemplate.DefaultEmptyText),
			};

			builder.CreateEditionDetailedItemPicker ("ImageGroups", this.Entity, "Groupes auxquels l'image appartient", controller, EnumValueCardinality.Any, ViewControllerMode.Summary, 4);
		}

		private void CreateUICategory(UIBuilder builder)
		{
			var controller = new SelectionController<ImageCategoryEntity> (this.BusinessContext)
			{
				ValueGetter         = () => this.Entity.ImageCategory,
				ValueSetter         = x => this.Entity.ImageCategory = x,
				ReferenceController = new ReferenceController (() => this.Entity.ImageCategory, creator: this.CreateNewCategory),
			};

			builder.CreateAutoCompleteTextField ("Catégorie", controller);
		}


		private NewEntityReference CreateNewBlob(DataContext context)
		{
			return context.CreateEntityAndRegisterAsEmpty<ImageBlobEntity> ();
		}

		private NewEntityReference CreateNewCategory(DataContext context)
		{
			return context.CreateEntityAndRegisterAsEmpty<ImageCategoryEntity> ();
		}


		private bool HasUpdateImageButton
		{
			get
			{
				if (this.Entity.ImageBlob.IsNull ())
				{
					return false;
				}

				var builder = new Epsitec.Common.IO.UriBuilder (this.Entity.ImageBlob.FileUri);

				string userName = System.Environment.UserName.ToLowerInvariant ();
				string host     = System.Environment.MachineName.ToLowerInvariant ();

				return (builder.UserName == userName &&
						builder.Host     == host &&
						System.IO.File.Exists (builder.Path));
			}
		}

		private void UpdateImage()
		{
			var builder = new Epsitec.Common.IO.UriBuilder (this.Entity.ImageBlob.FileUri);

			this.ImportImage (builder.Path);
		}

		private void ImportImage()
		{
			var filename = this.OpenFileDialog (CoreProgram.Application, this.Entity.ImageBlob.FileUri);

			if (!string.IsNullOrWhiteSpace (filename))
			{
				this.ImportImage (filename);
			}
		}

		private string OpenFileDialog(CoreApplication application, string uri)
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
				string filename  = System.IO.Path.GetFileName      (builder.Path);

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
			dialog.OwnerWindow = application.Window;
			dialog.OpenDialog ();
			if (dialog.Result != DialogResult.Accept)
			{
				return null;
			}

			return dialog.FileName;
		}

		private void ImportImage(string filename)
		{
			throw new System.NotImplementedException ();
#if false
			var file = new System.IO.FileInfo (filename);
			var store = this.Data.ImageDataStore;

			store.UpdateImage (this.DataContext, this.Entity, file);

			if (this.Entity.Name.IsNullOrEmpty)
			{
				this.Entity.Name = System.IO.Path.GetFileNameWithoutExtension (this.Entity.ImageBlob.FileName);
			}
#endif
		}
	}
}
