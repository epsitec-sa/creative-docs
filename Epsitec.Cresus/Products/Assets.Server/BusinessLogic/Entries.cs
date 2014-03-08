//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public class Entries : System.IDisposable
	{
		public Entries(DataAccessor accessor)
		{
			this.accessor = accessor;
		}

		public void Dispose()
		{
		}


		public void CreateEntry(AmortizedAmount amount)
		{
			//	Crée l'écriture liée à un amortissement ordinaire.
			this.RemoveEntry (amount);

			if (amount.EntryScenario != EntryScenario.None)
			{
				var debit  = this.GetDebit  (amount);
				var credit = this.GetCredit (amount);
				var title  = this.GetTitle  (amount);
				var value  = this.GetValue  (amount);

				amount.EntryGuid = this.AddEntry (this.RootEntry, amount.Date, debit, credit, null, title, value);
			}
		}

		public void RemoveEntry(AmortizedAmount amount)
		{
			//	Supprime l'écriture liée à un AmortizedAmount.
			if (amount.EntryGuid.IsEmpty)
			{
				return;
			}

			var entry = this.accessor.GetObject (BaseType.Entries, amount.EntryGuid);
			if (entry != null)
			{
				this.accessor.RemoveObject (BaseType.Entries, entry);
			}

			amount.EntryGuid = Guid.Empty;
		}


		private Guid GetDebit(AmortizedAmount amount)
		{
			//	Retourne le compte à utiliser au débit.
			switch (amount.EntryScenario)
			{
				case EntryScenario.Purchase:
					return amount.Account1;  // compte contrepartie d'achat

				case EntryScenario.Sale:
					return amount.Account2;  // compte contrepartie de vente

				case EntryScenario.Amortization:
					return amount.Account5;  // compte de charge d'amortissement

				case EntryScenario.Revaluation:
					return amount.Account6;  // compte de réévaluation

				default:
					return Guid.Empty;
			}
		}

		private Guid GetCredit(AmortizedAmount amount)
		{
			//	Retourne le compte à utiliser au crédit.
			switch (amount.EntryScenario)
			{
				case EntryScenario.Purchase:
					return amount.Account3;  // compte d'immobilisation

				case EntryScenario.Sale:
					return amount.Account3;  // compte d'immobilisation 

				case EntryScenario.Amortization:
					return amount.Account4;  // compte d'amortissement

				case EntryScenario.Revaluation:
					return amount.Account4;  // compte d'amortissement

				default:
					return Guid.Empty;
			}
		}

		private string GetTitle(AmortizedAmount amount)
		{
			//	Retourne le libellé de l'écriture.
			var guid = amount.AssetGuid;
			var timestamp = new Timestamp (amount.Date, 0);

			var name = AssetsLogic.GetSummary (this.accessor, guid, timestamp);

			switch (amount.EntryScenario)
			{
				case EntryScenario.Purchase:
					return "Achat " + name;

				case EntryScenario.Sale:
					return "Vente " + name;

				case EntryScenario.Amortization:
					return "Amortissement " + name;

				case EntryScenario.Revaluation:
					return "Réévaluation " + name;

				default:
					return name;
			}
		}

		private decimal GetValue(AmortizedAmount amount)
		{
			//	Retourne le montant de l'écriture.
			switch (amount.EntryScenario)
			{
				case EntryScenario.Purchase:
					return amount.FinalAmortizedAmount.GetValueOrDefault ();

				case EntryScenario.Sale:
					return amount.FinalAmortizedAmount.GetValueOrDefault ();

				case EntryScenario.Amortization:
					return amount.FinalAmortization;

				case EntryScenario.Revaluation:
					return amount.FinalAmortizedAmount.GetValueOrDefault ();

				default:
					return 0.0m;
			}
		}


		private Guid AddEntry(Guid guidParent, System.DateTime date, Guid debit, Guid credit, string stamp, string title, decimal value)
		{
			var entry = new DataObject ();

			var entries = this.accessor.Mandat.GetData (BaseType.Entries);
			entries.Add (entry);

			var start  = new Timestamp (new System.DateTime (2000, 1, 1), 0);
			var e = new DataEvent (start, EventType.Input);
			entry.AddEvent (e);

			if (!guidParent.IsEmpty)
			{
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, guidParent));
			}

			e.AddProperty (new DataDateProperty (ObjectField.EntryDate, date));

			if (!debit.IsEmpty)
			{
				e.AddProperty (new DataGuidProperty (ObjectField.EntryDebitAccount, debit));
			}

			if (!credit.IsEmpty)
			{
				e.AddProperty (new DataGuidProperty (ObjectField.EntryCreditAccount, credit));
			}

			e.AddProperty (new DataStringProperty (ObjectField.EntryStamp, stamp));
			e.AddProperty (new DataStringProperty (ObjectField.EntryTitle, title));
			e.AddProperty (new DataDecimalProperty (ObjectField.EntryAmount, value));

			return entry.Guid;
		}

		private Guid RootEntry
		{
			//	Retourne l'"écriture" racine, de laquelle toutes les écritures doivent
			//	être parentes.
			get
			{
				var entries = this.accessor.Mandat.GetData (BaseType.Entries);
				System.Diagnostics.Debug.Assert (entries.Any ());
				return entries[0].Guid;
			}
		}
	

		private readonly DataAccessor			accessor;
	}
}
