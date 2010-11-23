//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;

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

				this.CreateUIMain     (builder);
				this.CreateUICategory (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField      (tile,   0, "Nom",         Marshaler.Create (() => this.Entity.Name,        x => this.Entity.Name = x));
			builder.CreateTextFieldMulti (tile, 100, "Description", Marshaler.Create (() => this.Entity.Description, x => this.Entity.Description = x));
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


		private NewEntityReference CreateNewCategory(DataContext context)
		{
			return context.CreateEntityAndRegisterAsEmpty<ImageCategoryEntity> ();
		}
	}
}
