//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.Rules;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class AffairEntity
	{
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

		public override FormattedText GetTitle()
		{
			return this.GetCompactSummary ();
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
				a.Accumulate (this.DebtorBookAccount.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.ActiveSalesRepresentative.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.ActiveAffairOwner.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.SubAffairs.Select (x => x.GetEntityStatus ()));
				a.Accumulate (this.Comments.Select (x => x.GetEntityStatus ()));

				return a.EntityStatus;
			}
		}

		partial void OnCustomerChanged(CustomerEntity oldEntity, CustomerEntity newEntity)
		{
			if (Logic.IsAvailable)
			{
				AffairBusinessRules.InitializeDefaults (Logic.Current.GetComponent<BusinessContext> (), this);
			}
		}

		protected override void OnCollectionChanged(EntityCollection collection, string id, CollectionChangedEventArgs e)
		{
			if (Logic.IsNotAvailable)
			{
				return;
			}

			var action = e.Action;

			switch (id)
			{
				case "[GVA9]":	//	this.SubAffairs
					this.OnSubAffairsChanged ();
					break;

				case "[GVAA]":	//	this.Documents
					if (action == CollectionChangedAction.Add)
					{
						foreach (DocumentMetadataEntity document in e.NewItems)
						{
							this.HandleDocumentAdded (document);
						}
					}
					this.OnDocumentsChanged ();
					break;

				case "[GVAJ7]":	//	this.UnassignedPaymentTransactions
					this.OnUnassignedPaymentTransactionsChanged ();
					break;
			}
		}

		private void HandleDocumentAdded(DocumentMetadataEntity document)
		{
			if (string.IsNullOrEmpty (document.IdA))
			{
				var logic = Logic.Current;

				var businessContext = logic.GetComponent<BusinessContext> ();
				var generatorPool   = logic.GetComponent<RefIdGeneratorPool> ();
				var settings		= businessContext.GetCached<BusinessSettingsEntity> ();

				businessContext.AssignIds (document, generatorPool, settings.Generators);
			}
		}

		private void OnSubAffairsChanged()
		{
		}

		private void OnDocumentsChanged()
		{
		}

		private void OnUnassignedPaymentTransactionsChanged()
		{
		}
	}
}