//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionBusinessEventViewController : EditionViewController<Entities.BusinessEventEntity>
	{
		public EditionBusinessEventViewController(string name, Entities.BusinessEventEntity entity)
			: base (name, entity)
		{
			this.InitializeDefaultValues ();
		}


		private void InitializeDefaultValues()
		{
			if (this.Entity.Date.Ticks == 0)
			{
				this.Entity.Date = System.DateTime.Now;
			}
		}


		protected override EditionStatus GetEditionStatus()
		{
			if (this.Entity.Description.IsNullOrEmpty)
			{
				return EditionStatus.Empty;
			}

			return EditionStatus.Valid;
		}
	
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.BusinessEvent", "Evénement");

				this.CreateUICaseEventTypes (builder);
				this.CreateUIMain           (builder);

				builder.CreateFooterEditorTile ();
			}

			//	Summary:
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIDocuments (data);
			}
		}


		private void CreateUIMain(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField      (tile, 150, "Date et heure", Marshaler.Create (() => this.Entity.Date,        x => this.Entity.Date = x));
			builder.CreateTextFieldMulti (tile, 150, "Description",   Marshaler.Create (() => this.Entity.Description, x => this.Entity.Description = x));
		}

		private void CreateUICaseEventTypes(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var controller = new SelectionController<Entities.CaseEventTypeEntity> (this.BusinessContext)
			{
				ValueGetter              = () => this.Entity.EventType,
				ValueSetter              = x => this.Entity.EventType = x,
				ToFormattedTextConverter = x => TextFormatter.FormatText (x.Code)
			};

			builder.CreateEditionDetailedItemPicker ("Type de l'événement", controller, Business.EnumValueCardinality.ExactlyOne);
		}


		private void CreateUIDocuments(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "Document",
					IconUri		 = "Data.Document",
					Title		 = TextFormatter.FormatText ("Documents"),
					CompactTitle = TextFormatter.FormatText ("Documents"),
					Text		 = CollectionTemplate.DefaultEmptyText
				});

			var template = new CollectionTemplate<DocumentEntity> ("Document", data.Controller, this.DataContext);

			template.DefineText (x => TextFormatter.FormatText (x.Description));
			template.DefineCompactText (x => TextFormatter.FormatText (x.Description));

			data.Add (this.CreateCollectionAccessor (template, x => x.Documents));
		}
	}
}
