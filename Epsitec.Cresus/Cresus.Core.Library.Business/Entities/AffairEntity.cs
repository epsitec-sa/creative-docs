//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class AffairEntity
	{
		public IList<BusinessDocumentEntity> WorkflowDocuments
		{
			get
			{
				if (this.documents == null)
                {
					throw new System.NotImplementedException ();
#if false
					this.documents = new ObservableList<BusinessDocumentEntity> ();
					this.documents.AddRange (this.Workflow.Threads.SelectMany (thread => thread.ActiveDocuments).Select (x => x.BusinessDocument).Where (x => x.IsNotNull ()).Distinct ());
#endif
                }

				//	TODO : refresh this list when changes happen

				return this.documents.AsReadOnly ();
			}
		}


		public override FormattedText GetSummary()
		{
			if (this.Workflow.IsNull ())
			{
				return TextFormatter.FormatText (this.IdA);
			}
			else
			{
				var thread = this.Workflow.Threads.FirstOrDefault ();

				if (thread.IsNull () || thread.History.Count == 0)
				{
					return TextFormatter.FormatText (this.IdA);
				}

				var date = Misc.GetDateTimeShortDescription (thread.History[0].Date);
				return TextFormatter.FormatText (this.IdA, " - ", date, "(", this.Documents.Count, "doc.)");
			}
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.IdA);
		}


		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.IdA.GetEntityStatus ());
				a.Accumulate (this.IdB.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.IdC.GetEntityStatus ().TreatAsOptional ());

				a.Accumulate (this.Description.GetEntityStatus ());
//HACK:			a.Accumulate (this.Relation.GetEntityStatus ());
				a.Accumulate (this.DefaultDebtorBookAccount.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.ActiveSalesRepresentative.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.ActiveAffairOwner.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.SubAffairs.Select (x => x.GetEntityStatus ()));
				a.Accumulate (this.Comments.Select (x => x.GetEntityStatus ()));

				return a.EntityStatus;
			}
		}


		private ObservableList<BusinessDocumentEntity> documents;
	}
}