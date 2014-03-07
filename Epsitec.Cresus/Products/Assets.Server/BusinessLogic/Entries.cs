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


		public Guid CreateEntry(DataObject obj, System.DateTime date, AmortizationDetails details)
		{
			//	Crée l'écriture liée à un amortissement ordinaire et retourne son Guid.
			//	Le montant sera calculé plus tard.
			var debit  = details.Def.Debit;
			var credit = details.Def.Credit;
			var title  = this.GetTitle (obj, date);
			return this.AddEntry (this.RootEntry, date, debit, credit, null, title, null);
		}

		public void UpdateEntry(AmortizedAmount amount)
		{
			//	Met à jour le montant d'une écriture liée à un AmortizedAmount.
			var entry = this.accessor.GetObject (BaseType.Entries, amount.EntryGuid);

			if (entry != null)
			{
				System.Diagnostics.Debug.Assert (entry.Events.Count () == 1);
				var entryEvent = entry.GetEvent (0);

				entryEvent.AddProperty (new DataDecimalProperty (ObjectField.EntryAmount, amount.FinalAmortization));
			}
		}

		public void RemoveEntry(AmortizedAmount amount)
		{
			//	Supprime l'écriture liée à un AmortizedAmount.
			var entry = this.accessor.GetObject (BaseType.Entries, amount.EntryGuid);

			if (entry != null)
			{
				this.accessor.RemoveObject (BaseType.Entries, entry);
			}
		}


		private string GetTitle(DataObject obj, System.DateTime date)
		{
			//	Retourne le libellé d'une écriture d'amortissement ordinaire.
			var name = AssetsLogic.GetSummary (this.accessor, obj.Guid, new Timestamp (date, 0));
			return "Amortissement " + name;
		}


		private Guid AddEntry(Guid guidParent, System.DateTime? date, Guid debit, Guid credit, string stamp, string title, decimal? amount)
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

			if (date.HasValue)
			{
				e.AddProperty (new DataDateProperty (ObjectField.EntryDate, date.Value));
			}

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

			if (amount.HasValue)
			{
				e.AddProperty (new DataDecimalProperty (ObjectField.EntryAmount, amount.Value));
			}

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
