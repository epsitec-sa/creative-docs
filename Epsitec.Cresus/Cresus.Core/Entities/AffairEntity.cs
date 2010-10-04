//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class AffairEntity
	{
		public WorkflowEntity RootWorkflow
		{
			get
			{
				return this.Workflows.FirstOrDefault ().WrapNullEntity ();
			}
		}

		public IList<DocumentEntity> Documents
		{
			get
			{
				if (this.documents == null)
                {
					this.documents = new ObservableList<DocumentEntity> ();
					this.documents.AddRange (this.Workflows.SelectMany (workflow => workflow.Threads).SelectMany (thread => thread.Documents).Distinct ());
                }

				//	TODO : refresh this list when changes happen

				return this.documents.AsReadOnly ();
			}
		}


		public override FormattedText GetSummary()
		{
			var root = this.RootWorkflow;

			if (root.IsNull ())
			{
				return TextFormatter.FormatText (this.IdA);
			}
			else
			{
				var date = Misc.GetDateTimeShortDescription (root.Threads[0].History[0].Date);
				return TextFormatter.FormatText (this.IdA, " - ", date, "(", this.Documents.Count, "doc.)");
			}
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.IdA);
		}


		public override EntityStatus GetEntityStatus()
		{
			var s1 = this.IdA.GetEntityStatus ();
			var s2 = this.IdB.GetEntityStatus ().TreatAsOptional ();
			var s3 = this.IdC.GetEntityStatus ().TreatAsOptional ();
			var s4 = this.Description.GetEntityStatus ();
			var s5 = this.Relation.GetEntityStatus ();
			var s6 = this.DefaultDebtorBookAccount.GetEntityStatus ().TreatAsOptional ();
			var s7 = this.ActiveSalesRepresentative.GetEntityStatus ().TreatAsOptional ();
			var s8 = this.ActiveOwner.GetEntityStatus ().TreatAsOptional ();
			var s9 = EntityStatusHelper.CombineStatus (StatusHelperCardinality.All, this.SubAffairs.Select (x => x.GetEntityStatus ()).ToArray ());
			var s10 = EntityStatusHelper.CombineStatus (StatusHelperCardinality.All, this.Comments.Select (x => x.GetEntityStatus ()).ToArray ());

			return EntityStatusHelper.CombineStatus (StatusHelperCardinality.All, s1, s2, s3, s4, s5, s6, s7, s8, s9, s10);
		}


		private ObservableList<DocumentEntity> documents;
	}
}