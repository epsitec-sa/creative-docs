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
		public EditionImageViewController(string name, Entities.ImageEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.Image", "Image");

				this.CreateUIImport   (builder);
				this.CreateUIMain     (builder);
				//?this.CreateUIBlob     (builder);
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

			var button = builder.CreateButton (tile, 0, null, "Importer une image...");

			button.Clicked += delegate
			{
				this.Import ();
			};
		}

		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField      (tile,   0, "Nom",         Marshaler.Create (() => this.Entity.Name,        x => this.Entity.Name = x));
			builder.CreateTextFieldMulti (tile, 100, "Description", Marshaler.Create (() => this.Entity.Description, x => this.Entity.Description = x));
		}

		private void CreateUIBlob(UIBuilder builder)
		{
			var controller = new SelectionController<ImageBlobEntity> (this.BusinessContext)
			{
				ValueGetter         = () => this.Entity.ImageBlob,
				ValueSetter         = x => this.Entity.ImageBlob = x,
				ReferenceController = new ReferenceController (() => this.Entity.ImageBlob, creator: this.CreateNewBlob),
			};

			builder.CreateAutoCompleteTextField ("Image bitmap", controller);
		}

		private void CreateUIGroup(UIBuilder builder)
		{
			var controller = new SelectionController<ImageGroupEntity> (this.BusinessContext)
			{
				CollectionValueGetter    = () => this.Entity.ImageGroups,
				ToFormattedTextConverter = x => TextFormatter.FormatText (x.Name).IfNullOrEmptyReplaceWith (CollectionTemplate.DefaultEmptyText),
			};

			builder.CreateEditionDetailedItemPicker ("ImageGroups", this.Entity, "Groupes auxquels l'image appartient", controller, Business.EnumValueCardinality.Any, ViewControllerMode.Summary, 2);
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


		private void Import()
		{
			var filename = this.OpenFileDialog (CoreProgram.Application, this.Entity.ImageBlob.FileUri);

			if (!string.IsNullOrWhiteSpace (filename))
			{
				this.Import (filename);
			}
		}

		private string OpenFileDialog(CoreApplication application, string uri)
		{
			//	Exemple de contenu pour uri:
			//	"file://daniel@daniel-pc/C:/Users/Daniel/Documents/t.jpg"

			var dialog = new FileOpenDialog ();

			//?if (!string.IsNullOrEmpty (uri) && uri.StartsWith ("file://"))
			if (false)  // TODO: ne marche pas !
			{
				uri = uri.Substring (7);

				dialog.InitialDirectory = System.IO.Path.GetDirectoryName (uri);
				dialog.FileName = System.IO.Path.GetFileName (uri);
			}

			dialog.Title = "Importation d'une image bitmap";

			dialog.Filters.Add ("image", "Image", "*.bmp;*.tif;*.png;*.jpg");
			dialog.Filters.Add ("any", "Tous les fichiers", "*.*");

			dialog.AcceptMultipleSelection = false;
			dialog.Owner = application.Window;
			dialog.OpenDialog ();
			if (dialog.Result != DialogResult.Accept)
			{
				return null;
			}

			return dialog.FileName;
		}

		private void Import(string filename)
		{
			var file = new System.IO.FileInfo (filename);
			var store = this.Data.ImageDataStore;

			store.UpdateImage (this.DataContext, this.Entity, file);

			if (this.Entity.Name.IsNullOrEmpty)
			{
				this.Entity.Name = System.IO.Path.GetFileNameWithoutExtension (this.Entity.ImageBlob.FileName);
			}
		}
	}
}
