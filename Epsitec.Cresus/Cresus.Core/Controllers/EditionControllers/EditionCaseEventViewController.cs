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
		}

		protected override void CreateUI(TileContainer container)
		{
			using (var builder = new UIBuilder (container, this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.CaseEvent", "Evénement");

				this.CreateUIMain (builder);

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

//?			builder.CreateTextField      (tile,  90, "Date",        Marshaler.Create (() => this.Entity.Date,        x => this.Entity.Date = x));
			builder.CreateTextFieldMulti (tile, 150, "Description", Marshaler.Create (() => this.Entity.Description, x => this.Entity.Description = x));
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

			var template = new CollectionTemplate<DocumentEntity> ("Document", data.Controller)
				.DefineText        (x => UIBuilder.FormatText (x.Description))
				.DefineCompactText (x => UIBuilder.FormatText (x.Description));

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.Documents, template));
		}


		protected override EditionStatus GetEditionStatus()
		{
			if (string.IsNullOrEmpty (this.Entity.Description))
			{
				return EditionStatus.Empty;
			}

			return EditionStatus.Valid;
		}
	}
}
