﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

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

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField      (tile,   0, "Nom",         Marshaler.Create (() => this.Entity.Name,        x => this.Entity.Name = x));
			builder.CreateTextFieldMulti (tile, 100, "Description", Marshaler.Create (() => this.Entity.Description, x => this.Entity.Description = x));

			builder.CreateMargin (tile, horizontalSeparator: true);

			builder.CreateAutoCompleteTextField (tile, 0, "Type du document",  Marshaler.Create (() => this.Entity.DocumentType,          x => this.Entity.DocumentType = x),          Business.Enumerations.GetAllPossibleDocumentType (),          x => TextFormatter.FormatText (x.Values[0]));
			builder.CreateAutoCompleteTextField (tile, 0, "Source",            Marshaler.Create (() => this.Entity.DocumentSource,        x => this.Entity.DocumentSource = x),        Business.Enumerations.GetAllPossibleDocumentSource (),        x => TextFormatter.FormatText (x.Values[0]));
			builder.CreateAutoCompleteTextField (tile, 0, "Direction du flux", Marshaler.Create (() => this.Entity.DocumentFlowDirection, x => this.Entity.DocumentFlowDirection = x), Business.Enumerations.GetAllPossibleDocumentFlowDirection (), x => TextFormatter.FormatText (x.Values[0]));
		}

		private void CreateUIOptions(UIBuilder builder)
		{
			var controller = new SelectionController<DocumentOptionsEntity> (this.BusinessContext)
			{
				CollectionValueGetter    = () => this.Entity.DocumentOptions,
				ToFormattedTextConverter = x => TextFormatter.FormatText (x.Name).IfNullOrEmptyReplaceWith (CollectionTemplate.DefaultEmptyText),
			};

			builder.CreateEditionDetailedItemPicker ("DocumentOptions", this.Entity, "Options du document", controller, Business.EnumValueCardinality.Any, ViewControllerMode.Summary);
		}


	}
}
