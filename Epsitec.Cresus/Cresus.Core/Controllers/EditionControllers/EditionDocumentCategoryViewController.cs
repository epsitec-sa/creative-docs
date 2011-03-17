//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

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
	public class EditionDocumentCategoryViewController : EditionViewController<Entities.DocumentCategoryEntity>
	{
		public EditionDocumentCategoryViewController(string name, Entities.DocumentCategoryEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.DocumentCategory", "Catégorie de document");

				this.CreateUIMain (builder);
				this.CreateUIOptions (builder);
				this.CreateUIUnits (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField      (tile,   0, "Nom",         Marshaler.Create (() => this.Entity.Name,        x => this.Entity.Name = x));
			builder.CreateTextFieldMulti (tile, 100, "Description", Marshaler.Create (() => this.Entity.Description, x => this.Entity.Description = x));

			builder.CreateMargin (tile, horizontalSeparator: true);

			builder.CreateAutoCompleteTextField (tile, 0, "Type du document",  Marshaler.Create (() => this.Entity.DocumentType,          x => this.Entity.DocumentType = x),          EnumKeyValues.FromEnum<DocumentType> (),          x => TextFormatter.FormatText (x));
			builder.CreateAutoCompleteTextField (tile, 0, "Source",            Marshaler.Create (() => this.Entity.DocumentSource,        x => this.Entity.DocumentSource = x),        EnumKeyValues.FromEnum<DocumentSource> (),        x => TextFormatter.FormatText (x));
			builder.CreateAutoCompleteTextField (tile, 0, "Direction du flux", Marshaler.Create (() => this.Entity.DocumentFlowDirection, x => this.Entity.DocumentFlowDirection = x), EnumKeyValues.FromEnum<DocumentFlowDirection> (), x => TextFormatter.FormatText (x));
		}

		private void CreateUIOptions(UIBuilder builder)
		{
			var controller = new SelectionController<DocumentOptionsEntity> (this.BusinessContext)
			{
				CollectionValueGetter    = () => this.Entity.DocumentOptions,
				ToFormattedTextConverter = x => TextFormatter.FormatText (x.Name).IfNullOrEmptyReplaceWith (CollectionTemplate.DefaultEmptyText),
			};

			builder.CreateEditionDetailedItemPicker ("DocumentOptions", this.Entity, "Options du document", controller, EnumValueCardinality.Any, ViewControllerMode.Summary);
		}

		private void CreateUIUnits(UIBuilder builder)
		{
			var controller = new SelectionController<DocumentPrintingUnitsEntity> (this.BusinessContext)
			{
				CollectionValueGetter    = () => this.Entity.DocumentPrintingUnits,
				ToFormattedTextConverter = x => TextFormatter.FormatText (x.Name).IfNullOrEmptyReplaceWith (CollectionTemplate.DefaultEmptyText),
			};

			builder.CreateEditionDetailedItemPicker ("DocumentPrintingUnits", this.Entity, "Unités d'impression du document", controller, EnumValueCardinality.Any, ViewControllerMode.Summary);
		}
	}
}
