//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryAffairViewController : SummaryViewController<AffairEntity>
	{
		public SummaryAffairViewController(string name, AffairEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIAffair            (data);
				this.CreateUIDocuments         (data);
				this.CreateUIDocumentWorkflows (data);
				this.CreateUIEvents            (data);
				this.CreateUIComments          (data);
			}
		}

		private void CreateUIAffair(TileDataItems data)
		{
			data.Add (
				new TileDataItem
				{
					Name				= "Affair",
					IconUri				= "Data.Affair",
					Title				= TextFormatter.FormatText ("Affaire"),
					CompactTitle		= TextFormatter.FormatText ("Affaire"),
					TextAccessor		= this.CreateAccessor (x => x.GetSummary ()),
					CompactTextAccessor = this.CreateAccessor (x => x.GetCompactSummary ()),
					EntityMarshaler		= this.CreateEntityMarshaler (),
				});
		}

		private void CreateUIDocuments(TileDataItems data)
		{
			var tileData = new TileDataItem
			{
				AutoGroup    = false,
				Name		 = "DocMetadata",
				IconUri		 = "Data.Document",
				Title		 = TextFormatter.FormatText ("Document lié"),
				CompactTitle = TextFormatter.FormatText ("Documents liés"),
				Text		 = CollectionTemplate.DefaultEmptyText,
				HideAddAndRemoveButtons = true,
			};

			data.Add (tileData);

			var template = new CollectionTemplate<DocumentMetadataEntity> ("DocMetadata", this.BusinessContext);

			template.DefineTitle       (x => x.GetCompactSummary ());
			template.DefineText        (x => x.GetSummary ());
			template.DefineCompactText (x => x.GetCompactSummary ());

			data.Add (this.CreateCollectionAccessor (template, x => x.Documents));
		}

		private void CreateUIDocumentWorkflows(TileDataItems data)
		{
#if false
			var tileData =
				new TileDataItem
				{
					AutoGroup    = false,
					Name		 = "DocWorkflow",
					IconUri		 = "Data.Document",
					Title		 = TextFormatter.FormatText ("Document"),
					CompactTitle = TextFormatter.FormatText ("Documents"),
					Text		 = CollectionTemplate.DefaultEmptyText
				};

			tileData.SetEntityConverter<WorkflowEntity> (x => x.ActiveDocument);

			data.Add (tileData);

			var template = new CollectionTemplate<WorkflowEntity> ("DocWorkflow", this.BusinessContext);

			template.DefineTitle       (x => x.GetCompactSummary ());
			template.DefineText        (x => x.GetSummary ());
			template.DefineCompactText (x => x.GetCompactSummary ());

			data.Add (this.CreateCollectionAccessor (template, x => x.Workflows));
#endif
		}

		private void CreateUIEvents(TileDataItems data)
		{
#if false
			data.Add (
				new TileDataItem
				{
					AutoGroup    = true,
					Name		 = "WorkflowEvent",
					IconUri		 = "Data.WorkflowEvent",
					Title		 = TextFormatter.FormatText ("Evénement"),
					CompactTitle = TextFormatter.FormatText ("Evénements"),
					Text		 = CollectionTemplate.DefaultEmptyText
				});

			var template = new CollectionTemplate<WorkflowEventEntity> ("WorkflowEvent", this.BusinessContext);

			template.DefineText (x => x.GetSummary ());
			template.DefineCompactText (x => x.GetSummary ());

			data.Add (this.CreateCollectionAccessor (template, x => x.Workflows.SelectMany (w => w.Events).ToList ()));
#endif
		}

		private void CreateUIComments(TileDataItems data)
		{
			Common.CreateUIComments (this.BusinessContext, data, this.EntityGetter, x => x.Comments);
		}
	}
}
