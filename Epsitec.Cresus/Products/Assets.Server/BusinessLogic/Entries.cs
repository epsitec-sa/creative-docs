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
			System.Diagnostics.Debug.Assert (amount.Date.Year != 1);

			if (amount.EntryScenario == EntryScenario.None)
			{
				this.RemoveEntry (amount);
			}
			else
			{
				var entry = this.GetEntry (amount.AssetGuid, amount.EventGuid);

				if (entry == null)
				{
					entry = this.CreateBaseEntry (amount);
				}

				var entryAccouts = amount.EntryAccounts;

				var debit  = this.GetDebit  (amount, entryAccouts);
				var credit = this.GetCredit (amount, entryAccouts);
				var title  = this.GetTitle  (amount);
				var value  = this.GetValue  (amount);

				this.UpdateEntry (entry, amount.Date, debit, credit, null, title, value);

				amount.EntryGuid = entry.Guid;
			}
		}

		public void RemoveEntry(AmortizedAmount amount)
		{
			//	Supprime l'écriture liée à un AmortizedAmount.
			var entry = this.GetEntry (amount.AssetGuid, amount.EventGuid);
			if (entry != null)
			{
				this.accessor.RemoveObject (BaseType.Entries, entry);
			}
		}


		private DataObject GetEntry(Guid assetGuid, Guid eventGuid)
		{
			var entries = this.accessor.Mandat.GetData (BaseType.Entries);

			foreach (var entry in entries)
			{
				if (assetGuid == ObjectProperties.GetObjectPropertyGuid (entry, null, ObjectField.EntryAssetGuid) &&
					eventGuid == ObjectProperties.GetObjectPropertyGuid (entry, null, ObjectField.EntryEventGuid))
				{
					return entry;
				}
			}

			return null;
		}


		private Guid GetDebit(AmortizedAmount amount, EntryAccounts entryAccouts)
		{
			//	Retourne le compte à utiliser au débit.
			switch (amount.EntryScenario)
			{
				case EntryScenario.Purchase:
					return entryAccouts.Account1;  // compte contrepartie d'achat

				case EntryScenario.Sale:
					return entryAccouts.Account2;  // compte contrepartie de vente

				case EntryScenario.AmortizationAuto:
				case EntryScenario.AmortizationExtra:
					return entryAccouts.Account5;  // compte de charge d'amortissement

				case EntryScenario.Revaluation:
					return entryAccouts.Account6;  // compte de réévaluation

				default:
					return Guid.Empty;
			}
		}

		private Guid GetCredit(AmortizedAmount amount, EntryAccounts entryAccouts)
		{
			//	Retourne le compte à utiliser au crédit.
			switch (amount.EntryScenario)
			{
				case EntryScenario.Purchase:
					return entryAccouts.Account3;  // compte d'immobilisation

				case EntryScenario.Sale:
					return entryAccouts.Account3;  // compte d'immobilisation 

				case EntryScenario.AmortizationAuto:
				case EntryScenario.AmortizationExtra:
					return entryAccouts.Account4;  // compte d'amortissement

				case EntryScenario.Revaluation:
					return entryAccouts.Account4;  // compte d'amortissement

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

				case EntryScenario.AmortizationAuto:
					return "Amortissement ordinaire " + name;

				case EntryScenario.AmortizationExtra:
					return "Amortissement extraordinaire " + name;

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

				case EntryScenario.AmortizationAuto:
				case EntryScenario.AmortizationExtra:
					return amount.FinalAmortization;

				case EntryScenario.Revaluation:
					return amount.FinalAmortizedAmount.GetValueOrDefault ();

				default:
					return 0.0m;
			}
		}


		private DataObject CreateBaseEntry(AmortizedAmount amount)
		{
			var entry = new DataObject ();

			var entries = this.accessor.Mandat.GetData (BaseType.Entries);
			entries.Add (entry);

			var start  = new Timestamp (new System.DateTime (2000, 1, 1), 0);
			var e = new DataEvent (start, EventType.Input);
			entry.AddEvent (e);

			e.AddProperty (new DataGuidProperty (ObjectField.EntryAssetGuid, amount.AssetGuid));
			e.AddProperty (new DataGuidProperty (ObjectField.EntryEventGuid, amount.EventGuid));

			return entry;
		}

		private Guid UpdateEntry(DataObject entry, System.DateTime date, Guid debit, Guid credit, string stamp, string title, decimal value)
		{
			var e = entry.GetEvent (0);

			e.AddProperty (new DataDateProperty (ObjectField.EntryDate, date));

			if (!debit.IsEmpty)
			{
				e.AddProperty (new DataGuidProperty (ObjectField.EntryDebitAccount, debit));
			}

			if (!credit.IsEmpty)
			{
				e.AddProperty (new DataGuidProperty (ObjectField.EntryCreditAccount, credit));
			}

			e.AddProperty (new DataStringProperty  (ObjectField.EntryStamp,  stamp));
			e.AddProperty (new DataStringProperty  (ObjectField.EntryTitle,  title));
			e.AddProperty (new DataDecimalProperty (ObjectField.EntryAmount, value));

			return entry.Guid;
		}


		private readonly DataAccessor			accessor;
	}
}
