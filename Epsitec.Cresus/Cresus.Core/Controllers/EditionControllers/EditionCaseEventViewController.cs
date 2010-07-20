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
	public class EditionCaseEventViewController : EditionViewController<Entities.CaseEventEntity>
	{
		public EditionCaseEventViewController(string name, Entities.CaseEventEntity entity)
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
			if (string.IsNullOrEmpty (this.Entity.Description))
			{
				return EditionStatus.Empty;
			}

			return EditionStatus.Valid;
		}
		protected override void UpdateEmptyEntityStatus(DataLayer.Context.DataContext context, bool isEmpty)
		{
			var entity = this.Entity;
			context.UpdateEmptyEntityStatus (entity, isEmpty);
		}

	
		protected override void CreateUI(TileContainer container)
		{
			using (var builder = new UIBuilder (container, this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.CaseEvent", "Evénement");

				this.CreateUICaseEventTypes (builder);
				this.CreateUIMain           (builder);

				builder.CreateFooterEditorTile ();
			}

			//	Summary:
			var containerController = new TileContainerController (this, container);
			var data = containerController.DataItems;

			this.CreateUIDocuments (data);

			containerController.GenerateTiles ();
		}


		private void CreateUIMain(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField      (tile, 150, "Date et heure", Marshaler.Create (() => this.Entity.Date,        x => this.Entity.Date = x));
			builder.CreateTextFieldMulti (tile, 150, "Description",   Marshaler.Create (() => this.Entity.Description, x => this.Entity.Description = x));
		}

		private void CreateUICaseEventTypes(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var controller = new SelectionController<Entities.CaseEventTypeEntity>
			{
				ValueGetter              = () => this.Entity.EventType,
				ValueSetter              = x => this.Entity.EventType = x,
				PossibleItemsGetter      = () => CoreProgram.Application.Data.GetCaseEventTypes (),
				ToFormattedTextConverter = x => UIBuilder.FormatText (x.Code)
			};

			builder.CreateEditionDetailedRadio (0, "Type de l'événement", controller);
		}


		private void CreateUIDocuments(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "Document",
					IconUri		 = "Data.Document",
					Title		 = UIBuilder.FormatText ("Documents"),
					CompactTitle = UIBuilder.FormatText ("Documents"),
					Text		 = CollectionTemplate.DefaultEmptyText
				});

			var template = new CollectionTemplate<DocumentEntity> ("Document", data.Controller, this.DataContext)
				.DefineText        (x => UIBuilder.FormatText (x.Description))
				.DefineCompactText (x => UIBuilder.FormatText (x.Description));

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.Documents, template));
		}
	}
}
