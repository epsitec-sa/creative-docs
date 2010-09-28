//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
				this.CreateUIWorkflows (data);
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
					TextAccessor		= this.CreateAccessor (x => TextFormatter.FormatText (x.IdA)),
					CompactTextAccessor = this.CreateAccessor (x => TextFormatter.FormatText (x.IdA)),
					EntityMarshaler		= this.CreateEntityMarshaler (),
				});
		}

		private void CreateUIWorkflows(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = false,
					Name		 = "Workflow",
					IconUri		 = "Data.Workflow",
					Title		 = TextFormatter.FormatText ("Workflow"),
					CompactTitle = TextFormatter.FormatText ("Workflows"),
					Text		 = CollectionTemplate.DefaultEmptyText
				});

			var template = new CollectionTemplate<WorkflowEntity> ("Workflow", this.BusinessContext);

			template.DefineText (x => TextFormatter.FormatText ("Variante", x.Id+1));
			template.DefineCompactText (x => TextFormatter.FormatText ("Variante", x.Id+1));

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
					Title		 = TextFormatter.FormatText ("Evénement"),
					CompactTitle = TextFormatter.FormatText ("Evénements"),
					Text		 = CollectionTemplate.DefaultEmptyText
				});

			var template = new CollectionTemplate<WorkflowEventEntity> ("WorkflowEvent", this.BusinessContext);

			template.DefineText (x => x.GetSummary ());
			template.DefineCompactText (x => x.GetSummary ());

			data.Add (this.CreateCollectionAccessor (template, x => x.Workflows.SelectMany (w => w.Events).ToList ()));
		}
	}
}
