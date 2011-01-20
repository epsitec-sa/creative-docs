//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Entities;

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
				this.CreateUIToto              (data);
			}
		}

		private void CreateUIAffair(SummaryDataItems data)
		{
			data.Add (
				new SummaryDataItem
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

		private void CreateUIDocuments(SummaryDataItems data)
		{
			var summaryData = new SummaryDataItem
			{
				AutoGroup    = false,
				Name		 = "DocMetadata",
				IconUri		 = "Data.Document",
				Title		 = TextFormatter.FormatText ("Document lié"),
				CompactTitle = TextFormatter.FormatText ("Documents liés"),
				Text		 = CollectionTemplate.DefaultEmptyText
			};

			data.Add (summaryData);

			var template = new CollectionTemplate<DocumentMetadataEntity> ("DocMetadata", this.BusinessContext);

			template.DefineTitle       (x => x.GetCompactSummary ());
			template.DefineText        (x => x.GetSummary ());
			template.DefineCompactText (x => x.GetCompactSummary ());

			data.Add (this.CreateCollectionAccessor (template, x => x.Documents));
		}

		private void CreateUIDocumentWorkflows(SummaryDataItems data)
		{
#if false
			var summaryData =
				new SummaryData
				{
					AutoGroup    = false,
					Name		 = "DocWorkflow",
					IconUri		 = "Data.Document",
					Title		 = TextFormatter.FormatText ("Document"),
					CompactTitle = TextFormatter.FormatText ("Documents"),
					Text		 = CollectionTemplate.DefaultEmptyText
				};

			summaryData.SetEntityConverter<WorkflowEntity> (x => x.ActiveDocument);

			data.Add (summaryData);

			var template = new CollectionTemplate<WorkflowEntity> ("DocWorkflow", this.BusinessContext);

			template.DefineTitle       (x => x.GetCompactSummary ());
			template.DefineText        (x => x.GetSummary ());
			template.DefineCompactText (x => x.GetCompactSummary ());

			data.Add (this.CreateCollectionAccessor (template, x => x.Workflows));
#endif
		}

		private void CreateUIEvents(SummaryDataItems data)
		{
#if false
			data.Add (
				new SummaryData
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

		private void CreateUIComments(SummaryDataItems data)
		{
			Common.CreateUIComments (this.BusinessContext, data, this.EntityGetter, x => x.Comments);
		}


		private void CreateUIToto(SummaryDataItems data)
		{
			var summaryData = new SummaryDataItem
			{
				Name         = "Toto",
				IconUri		 = "Data.Toto",
				Title		 = TextFormatter.FormatText ("Toto"),
				CompactTitle = TextFormatter.FormatText ("Toto"),
				CreateUI     = this.CreateUIToto2,
			};

			data.Add (summaryData);
		}

		private void CreateUIToto2(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin (tile, horizontalSeparator: true);
			builder.CreateStaticText (tile, 100, "Toto !");
			builder.CreateMargin (tile, horizontalSeparator: true);
		}
	}
}
