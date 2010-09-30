//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
				this.CreateUIAffair (data);
				this.CreateUIDocumentWorkflows (data);
				this.CreateUIEvents (data);
			}
		}

		private void CreateUIAffair(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
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

		private void CreateUIDocumentWorkflows(SummaryDataItems data)
		{
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
		}

		private void CreateUIEvents(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "WorkflowEvent",
					IconUri		 = "Data.WorkflowEvent",
					Title		 = TextFormatter.FormatText ("Ev�nement"),
					CompactTitle = TextFormatter.FormatText ("Ev�nements"),
					Text		 = CollectionTemplate.DefaultEmptyText
				});

			var template = new CollectionTemplate<WorkflowEventEntity> ("WorkflowEvent", this.BusinessContext);

			template.DefineText (x => x.GetSummary ());
			template.DefineCompactText (x => x.GetSummary ());

			data.Add (this.CreateCollectionAccessor (template, x => x.Workflows.SelectMany (w => w.Events).ToList ()));
		}
	}
}
