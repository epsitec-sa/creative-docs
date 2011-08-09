﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
using Epsitec.Cresus.Bricks;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionDocumentCategoryViewController : EditionViewController<Entities.DocumentCategoryEntity>
	{
#if true
		protected override void CreateBricks(BrickWall<DocumentCategoryEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.Name)
				  .Field (x => x.Description)
				.End ()
#if false
				.Separator ()
				.Input ()
				  .Field (x => x.DocumentType)
				  .Field (x => x.DocumentSource)
				  .Field (x => x.DocumentFlowDirection)
				  .Field (x => x.DocumentOptions)
				  .Field (x => x.DocumentPrintingUnits)
				.End ()
#endif
				.Separator ()
				.Input ()
				  .Field (x => x).WithSpecialController ()
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.DocumentPrintingUnits)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.DocumentSource)
				  .Field (x => x.DocumentFlowDirection)
				.End ()
				;
		}
#else
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

			builder.CreateAutoCompleteTextField (tile, 0, "Type du document",  Marshaler.Create (() => this.Entity.DocumentType,          x => this.Entity.DocumentType = x),          EnumKeyValues.FromEnum<DocumentType> ());
			builder.CreateAutoCompleteTextField (tile, 0, "Source",            Marshaler.Create (() => this.Entity.DocumentSource,        x => this.Entity.DocumentSource = x),        EnumKeyValues.FromEnum<DocumentSource> ());
			builder.CreateAutoCompleteTextField (tile, 0, "Direction du flux", Marshaler.Create (() => this.Entity.DocumentFlowDirection, x => this.Entity.DocumentFlowDirection = x), EnumKeyValues.FromEnum<DocumentFlowDirection> ());
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
#endif
	}
}
