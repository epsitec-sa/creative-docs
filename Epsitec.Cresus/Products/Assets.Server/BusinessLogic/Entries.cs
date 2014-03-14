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
				var dataEntry = this.GetEntry (amount.AssetGuid, amount.EventGuid);

				if (dataEntry == null)
				{
					dataEntry = this.CreateDataEntry (amount);
				}

				var entryProperties = this.GetEntryProperties (amount, false);

				this.UpdateEntry (dataEntry, entryProperties);
				amount.EntrySeed++;
			}
		}

		public void RemoveEntry(AmortizedAmount amount)
		{
			//	Supprime l'écriture liée à un AmortizedAmount.
			var entry = this.GetEntry (amount.AssetGuid, amount.EventGuid);
			if (entry != null)
			{
				this.accessor.RemoveObject (BaseType.Entries, entry);

				amount.EntryGuid = Guid.Empty;
				amount.EntrySeed++;
			}
		}


		public EntryProperties GetEntryProperties(AmortizedAmount amount, bool baseProperties)
		{
			var asset = this.accessor.GetObject (BaseType.Assets, amount.AssetGuid);
			var assetEvent = asset.GetEvent (amount.EventGuid);
			var entryAccouts = amount.EntryAccounts;

			return new EntryProperties
			{
				Date   = this.GetDate   (amount, assetEvent,               baseProperties),
				Debit  = this.GetDebit  (amount, assetEvent, entryAccouts, baseProperties),
				Credit = this.GetCredit (amount, assetEvent, entryAccouts, baseProperties),
				Stamp  = this.GetStamp  (amount, assetEvent,               baseProperties),
				Title  = this.GetTitle  (amount, assetEvent,               baseProperties),
				Amount = this.GetValue  (amount, assetEvent,               baseProperties),
			};
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


		private System.DateTime GetDate(AmortizedAmount amount, DataEvent assetEvent, bool baseProperties)
		{
			//	Retourne la date à utiliser pour l'écriture.
			if (!baseProperties)
			{
				var p = assetEvent.GetProperty (ObjectField.AssetEntryForcedStamp) as DataDateProperty;
				if (p != null)
				{
					return p.Value;
				}
			}

			return amount.Date;
		}

		private Guid GetDebit(AmortizedAmount amount, DataEvent assetEvent, EntryAccounts entryAccouts, bool baseProperties)
		{
			//	Retourne le compte à utiliser au débit de l'écriture.
			if (!baseProperties)
			{
				var p = assetEvent.GetProperty (ObjectField.AssetEntryForcedStamp) as DataGuidProperty;
				if (p != null)
				{
					return p.Value;
				}
			}

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

		private Guid GetCredit(AmortizedAmount amount, DataEvent assetEvent, EntryAccounts entryAccouts, bool baseProperties)
		{
			//	Retourne le compte à utiliser au crédit de l'écriture.
			if (!baseProperties)
			{
				var p = assetEvent.GetProperty (ObjectField.AssetEntryForcedStamp) as DataGuidProperty;
				if (p != null)
				{
					return p.Value;
				}
			}

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

		private string GetStamp(AmortizedAmount amount, DataEvent assetEvent, bool baseProperties)
		{
			//	Retourne la pièce de l'écriture.
			if (!baseProperties)
			{
				var p = assetEvent.GetProperty (ObjectField.AssetEntryForcedStamp) as DataStringProperty;
				if (p != null)
				{
					return p.Value;
				}
			}

			return null;
		}

		private string GetTitle(AmortizedAmount amount, DataEvent assetEvent, bool baseProperties)
		{
			//	Retourne le libellé de l'écriture.
			if (!baseProperties)
			{
				var p = assetEvent.GetProperty (ObjectField.AssetEntryForcedTitle) as DataStringProperty;
				if (p != null)
				{
					return p.Value;
				}
			}

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

		private decimal GetValue(AmortizedAmount amount, DataEvent assetEvent, bool baseProperties)
		{
			//	Retourne le montant de l'écriture.
			if (!baseProperties)
			{
				var p = assetEvent.GetProperty (ObjectField.AssetEntryForcedStamp) as DataDecimalProperty;
				if (p != null)
				{
					return p.Value;
				}
			}

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


		private DataObject CreateDataEntry(AmortizedAmount amount)
		{
			var entry = new DataObject ();

			amount.EntryGuid = entry.Guid;
			amount.EntrySeed = 0;

			var entries = this.accessor.Mandat.GetData (BaseType.Entries);
			entries.Add (entry);

			var start  = new Timestamp (new System.DateTime (2000, 1, 1), 0);
			var e = new DataEvent (start, EventType.Input);
			entry.AddEvent (e);

			e.AddProperty (new DataGuidProperty (ObjectField.EntryAssetGuid, amount.AssetGuid));
			e.AddProperty (new DataGuidProperty (ObjectField.EntryEventGuid, amount.EventGuid));

			return entry;
		}

		private Guid UpdateEntry(DataObject entry, EntryProperties entryProperties)
		{
			var e = entry.GetEvent (0);

			e.AddProperty (new DataDateProperty (ObjectField.EntryDate, entryProperties.Date));

			if (!entryProperties.Debit.IsEmpty)
			{
				e.AddProperty (new DataGuidProperty (ObjectField.EntryDebitAccount, entryProperties.Debit));
			}

			if (!entryProperties.Credit.IsEmpty)
			{
				e.AddProperty (new DataGuidProperty (ObjectField.EntryCreditAccount, entryProperties.Credit));
			}

			e.AddProperty (new DataStringProperty  (ObjectField.EntryStamp,  entryProperties.Stamp));
			e.AddProperty (new DataStringProperty  (ObjectField.EntryTitle,  entryProperties.Title));
			e.AddProperty (new DataDecimalProperty (ObjectField.EntryAmount, entryProperties.Amount));

			return entry.Guid;
		}


		private readonly DataAccessor			accessor;
	}
}
