//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public static class DummyEntries
	{
		internal static void AddEntries(DataMandat mandat)
		{
			var b = DummyEntries.AddEntry (mandat, Guid.Empty, new System.DateTime (2014, 1, 1), null, null, null, "Journal principal 2014", null);

			{
				var e = DummyEntries.AddEntry (mandat, b, new System.DateTime (2014, 1, 1), null, null, null, "Soldes à nouveau", null);

				DummyEntries.AddEntry (mandat, e, new System.DateTime (2014, 1, 1), "1000", "9100", "171", "Solde à nouveau Caisse", 10051.45m);
				DummyEntries.AddEntry (mandat, e, new System.DateTime (2014, 1, 1), "9100", "1010", "172", "Solde à nouveau CCP", 420.10m);
				DummyEntries.AddEntry (mandat, e, new System.DateTime (2014, 1, 1), "9100", "1020", "173", "Solde à nouveau Banque", 3644.75m);
				DummyEntries.AddEntry (mandat, e, new System.DateTime (2014, 1, 1), "9100", "2000", "174", "Solde à nouveau Créanciers", 11300.05m);
			}

			DummyEntries.AddEntry (mandat, b, new System.DateTime (2014, 2, 10), "1010", "1000", "183", "Viré à poste", 100.0m);
			DummyEntries.AddEntry (mandat, b, new System.DateTime (2014, 3, 15), "6000", "1010", "185", "Loyer mars", 2500.0m);

			{
				var e = DummyEntries.AddEntry (mandat, b, new System.DateTime (2014, 3, 31), "4200", "2000", "191", "Paiement facture 15/358-2", 532.0m);

				DummyEntries.AddEntry (mandat, e, new System.DateTime (2014, 3, 31), "4200", null, "191", "Montant net", 492.59m);
				DummyEntries.AddEntry (mandat, e, new System.DateTime (2014, 3, 31), "1170", null, "191", "TVA 8% (IPM)", 39.41m);
				DummyEntries.AddEntry (mandat, e, new System.DateTime (2014, 3, 31), null, "2000", "191", "Total", 532.0m);
			}

			DummyEntries.AddEntry (mandat, b, new System.DateTime (2014, 4, 15), "6000", "1010", "195", "Loyer avril", 2500.0m);
			DummyEntries.AddEntry (mandat, b, new System.DateTime (2014, 5, 12), "6000", "1010", "202", "Loyer mai", 2500.0m);
			DummyEntries.AddEntry (mandat, b, new System.DateTime (2014, 5, 13), "1010", "1000", "203", "Viré à poste", 500.0m);
			DummyEntries.AddEntry (mandat, b, new System.DateTime (2014, 6, 18), "6000", "1010", "204", "Loyer juin", 2500.0m);
		}


		private static Guid AddEntry(DataMandat mandat, Guid guidParent, System.DateTime? date, string debit, string credit, string stamp, string title, decimal? amount)
		{
			var entry = new DataObject ();

			var entries = mandat.GetData (BaseType.Entries);
			entries.Add (entry);

			var start  = new Timestamp (new System.DateTime (2000, 1, 1), 0);
			var e = new DataEvent (start, EventType.Input);
			entry.AddEvent (e);

			if (!guidParent.IsEmpty)
			{
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, guidParent));
			}

			if (date.HasValue)
			{
				e.AddProperty (new DataDateProperty (ObjectField.EntryDate, date.Value));
			}

			if (!string.IsNullOrEmpty (debit))
			{
				var guid  = DummyAccounts.GetAccount (mandat, debit);
				System.Diagnostics.Debug.Assert (!guid.IsEmpty);
				e.AddProperty (new DataGuidProperty (ObjectField.EntryDebitAccount, guid));
			}

			if (!string.IsNullOrEmpty (credit))
			{
				var guid = DummyAccounts.GetAccount (mandat, credit);
				System.Diagnostics.Debug.Assert (!guid.IsEmpty);
				e.AddProperty (new DataGuidProperty (ObjectField.EntryCreditAccount, guid));
			}

			e.AddProperty (new DataStringProperty (ObjectField.EntryStamp, stamp));
			e.AddProperty (new DataStringProperty (ObjectField.EntryTitle, title));

			if (amount.HasValue)
			{
				e.AddProperty (new DataDecimalProperty (ObjectField.EntryAmount, amount.Value));
			}

			return entry.Guid;
		}
	}
}