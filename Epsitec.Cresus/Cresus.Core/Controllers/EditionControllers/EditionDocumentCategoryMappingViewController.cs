//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionDocumentCategoryMappingViewController : EditionViewController<Entities.DocumentCategoryMappingEntity>
	{
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.DocumentCategoryMapping", "Assignation pour l'impression");

				this.CreateUIMain (builder);
				this.CreateUICategories (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			var types  = Epsitec.Cresus.Core.Print.EntityPrinters.AbstractPrinter.GetPrintableEntityTypes ();
			var values = EnumKeyValues.FromEntityIds (types.Select (x => EntityInfo.GetTypeId (x)));

			builder.CreateAutoCompleteTextField (tile, 0, "Type des données imprimables", Marshaler.Create (() => this.Entity.PrintableEntity, x => this.Entity.PrintableEntity = x), values, x => TextFormatter.FormatText (x));
		}

		private void CreateUICategories(UIBuilder builder)
		{
			var controller = new SelectionController<DocumentCategoryEntity> (this.BusinessContext)
			{
				CollectionValueGetter    = () => this.Entity.DocumentCategories,
				ToFormattedTextConverter = x => TextFormatter.FormatText (x.Name).IfNullOrEmptyReplaceWith (CollectionTemplate.DefaultEmptyText),
			};

			builder.CreateEditionDetailedItemPicker ("DocumentCategory", this.Entity, "Catégories", controller, EnumValueCardinality.Any, ViewControllerMode.Summary);
		}
	}
}
