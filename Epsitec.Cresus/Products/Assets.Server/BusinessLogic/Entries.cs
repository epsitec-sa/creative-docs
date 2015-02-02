//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
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


		private bool HasEntry(DataObject asset, DataEvent e, AmortizedAmount amount)
		{
			var editedProperties = this.GetEntryProperties (asset, e, amount, GetEntryPropertiesType.EditedOrBase);
			return editedProperties.IsValid;
		}

		private AmortizedAmount CreateEntry(DataObject asset, DataEvent e, AmortizedAmount amount)
		{
			//	Crée l'écriture liée à un amortissement ordinaire.
			System.Diagnostics.Debug.Assert (e.Timestamp.Date.Year != 1);

			var editedProperties = this.GetEntryProperties (asset, e, amount, GetEntryPropertiesType.EditedOrBase);

			if (amount.EntryScenario == EntryScenario.None || !editedProperties.IsValid)
			{
				return this.RemoveEntry (asset, e, amount);
			}
			else
			{
				var entryGuid = amount.EntryGuid;
				var entrySeed = amount.EntrySeed;

				var dataEntry = this.GetEntry (asset, e);

				if (dataEntry == null)
				{
					dataEntry = this.CreateDataEntry (asset, e, amount, ref entryGuid, ref entrySeed);
				}

				var entryProperties = this.GetEntryProperties (asset, e, amount, GetEntryPropertiesType.Current);
				this.UpdateEntry (dataEntry, entryProperties);

				return AmortizedAmount.SetEntry (amount, entryGuid, entrySeed+1);
			}
		}

		private AmortizedAmount RemoveEntry(DataObject asset, DataEvent e, AmortizedAmount amount)
		{
			//	Supprime l'écriture liée à un AmortizedAmount.
			var entry = this.GetEntry (asset, e);

			if (entry == null)
			{
				return amount;
			}
			else
			{
				this.accessor.RemoveObject (BaseType.Entries, entry);

				return AmortizedAmount.SetEntry (amount, Guid.Empty, amount.EntrySeed+1);
			}
		}


		public void GetSample(EntryScenario scenario, int column, out string text, out string tooltip)
		{
			//	Retourne le texte permettant de peupler le contrôleur qui montre les
			//	exemples d'écritures pour les différents scénarios (EntrySamples).
			var aa = new AmortizedAmount (scenario);
			var ea = new EntryAccounts ();

			foreach (var field in DataAccessor.AccountFields)
			{
				ea[field] = this.accessor.EditionAccessor.GetFieldString (field);
			}

			text    = null;
			tooltip = null;
			
			switch (column)
			{
				case 0:
					text = this.GetDebit (null, null, aa, ea, GetEntryPropertiesType.Sample, out tooltip);
					break;
			
				case 1:
					text = this.GetCredit (null, null, aa, ea, GetEntryPropertiesType.Sample, out tooltip);
					break;
			
				case 2:
					text = this.GetTitle (null, null, aa, GetEntryPropertiesType.Sample);
					break;
			}
		}


		public enum GetEntryPropertiesType
		{
			Base,
			Current,
			EditedOrBase,
			Sample,
		}

		public EntryProperties GetEntryProperties(DataObject asset, DataEvent e, AmortizedAmount amount, GetEntryPropertiesType type)
		{
			var entryAccounts = this.GetEntryAccounts (asset, e, amount);

			string tooltip;

			return new EntryProperties
			{
				Date    = this.GetDate    (asset, e, amount,                type),
				Debit   = this.GetDebit   (asset, e, amount, entryAccounts, type, out tooltip),
				Credit  = this.GetCredit  (asset, e, amount, entryAccounts, type, out tooltip),
				Stamp   = this.GetStamp   (asset, e, amount,                type),
				Title   = this.GetTitle   (asset, e, amount,                type),
				Amount  = this.GetValue   (asset, e, amount,                type),
				VatCode = this.GetVatCode (asset, e, amount,                type),
			};
		}


		private EntryAccounts GetEntryAccounts(DataObject asset, DataEvent e, AmortizedAmount amount)
		{
			//	Retourne la liste des comptes à utiliser pour passer une écriture liée
			//	à l'événement contenant ce montant.
			var ea = new EntryAccounts ();

			foreach (var field in DataAccessor.AccountFields)
			{
				ea[field] = ObjectProperties.GetObjectPropertyString (asset, e.Timestamp, field);
			}

			return ea;
		}


		private DataObject GetEntry(DataObject asset, DataEvent e)
		{
			var entries = this.accessor.Mandat.GetData (BaseType.Entries);

			foreach (var entry in entries)
			{
				if (asset.Guid == ObjectProperties.GetObjectPropertyGuid (entry, null, ObjectField.EntryAssetGuid) &&
					    e.Guid == ObjectProperties.GetObjectPropertyGuid (entry, null, ObjectField.EntryEventGuid))
				{
					return entry;
				}
			}

			return null;
		}


		private System.DateTime GetDate(DataObject asset, DataEvent e, AmortizedAmount amount, GetEntryPropertiesType type)
		{
			//	Retourne la date à utiliser pour l'écriture.
			if (type == GetEntryPropertiesType.Current)
			{
				var p = e.GetProperty (ObjectField.AssetEntryForcedDate) as DataDateProperty;
				if (p != null)
				{
					return p.Value;
				}
			}

			if (type == GetEntryPropertiesType.EditedOrBase)
			{
				var date = this.accessor.EditionAccessor.GetFieldDate (ObjectField.AssetEntryForcedDate, synthetic: false);
				if (date.HasValue)
				{
					return date.Value;
				}
			}

			return e.Timestamp.Date;
		}

		private string GetDebit(DataObject asset, DataEvent e, AmortizedAmount amount, EntryAccounts entryAccouts, GetEntryPropertiesType type, out string tooltip)
		{
			//	Retourne le compte à utiliser au débit de l'écriture.
			tooltip = null;

			if (type == GetEntryPropertiesType.Current)
			{
				var p = e.GetProperty (ObjectField.AssetEntryForcedDebit) as DataStringProperty;
				if (p != null && !string.IsNullOrEmpty (p.Value))
				{
					return p.Value;
				}
			}

			if (type == GetEntryPropertiesType.EditedOrBase)
			{
				var field = this.accessor.EditionAccessor.GetFieldString (ObjectField.AssetEntryForcedDebit, synthetic: false);
				if (!string.IsNullOrEmpty (field))
				{
					return field;
				}
			}

			switch (amount.EntryScenario)
			{
				case EntryScenario.Purchase:
					tooltip = DataDescriptions.GetObjectFieldDescription (ObjectField.AccountPurchaseDebit);
					return entryAccouts[ObjectField.AccountPurchaseDebit];

				case EntryScenario.Sale:
					tooltip = DataDescriptions.GetObjectFieldDescription (ObjectField.AccountSaleDebit);
					return entryAccouts[ObjectField.AccountSaleDebit];

				case EntryScenario.AmortizationAuto:
					tooltip = DataDescriptions.GetObjectFieldDescription (ObjectField.AccountAmortizationAutoDebit);
					return entryAccouts[ObjectField.AccountAmortizationAutoDebit];

				case EntryScenario.AmortizationExtra:
					tooltip = DataDescriptions.GetObjectFieldDescription (ObjectField.AccountAmortizationExtraDebit);
					return entryAccouts[ObjectField.AccountAmortizationExtraDebit];

				case EntryScenario.Increase:
					tooltip = DataDescriptions.GetObjectFieldDescription (ObjectField.AccountIncreaseDebit);
					return entryAccouts[ObjectField.AccountIncreaseDebit];

				case EntryScenario.Decrease:
					tooltip = DataDescriptions.GetObjectFieldDescription (ObjectField.AccountDecreaseDebit);
					return entryAccouts[ObjectField.AccountDecreaseDebit];

				case EntryScenario.Adjust:
					tooltip = DataDescriptions.GetObjectFieldDescription (ObjectField.AccountAdjustDebit);
					return entryAccouts[ObjectField.AccountAdjustDebit];

				default:
					return null;
			}
		}

		private string GetCredit(DataObject asset, DataEvent e, AmortizedAmount amount, EntryAccounts entryAccouts, GetEntryPropertiesType type, out string tooltip)
		{
			//	Retourne le compte à utiliser au crédit de l'écriture.
			tooltip = null;

			if (type == GetEntryPropertiesType.Current)
			{
				var p = e.GetProperty (ObjectField.AssetEntryForcedCredit) as DataStringProperty;
				if (p != null && !string.IsNullOrEmpty (p.Value))
				{
					return p.Value;
				}
			}

			if (type == GetEntryPropertiesType.EditedOrBase)
			{
				var field = this.accessor.EditionAccessor.GetFieldString (ObjectField.AssetEntryForcedCredit, synthetic: false);
				if (!string.IsNullOrEmpty (field))
				{
					return field;
				}
			}

			switch (amount.EntryScenario)
			{
				case EntryScenario.Purchase:
					tooltip = DataDescriptions.GetObjectFieldDescription (ObjectField.AccountPurchaseCredit);
					return entryAccouts[ObjectField.AccountPurchaseCredit];

				case EntryScenario.Sale:
					tooltip = DataDescriptions.GetObjectFieldDescription (ObjectField.AccountSaleCredit);
					return entryAccouts[ObjectField.AccountSaleCredit];

				case EntryScenario.AmortizationAuto:
					tooltip = DataDescriptions.GetObjectFieldDescription (ObjectField.AccountAmortizationAutoCredit);
					return entryAccouts[ObjectField.AccountAmortizationAutoCredit];

				case EntryScenario.AmortizationExtra:
					tooltip = DataDescriptions.GetObjectFieldDescription (ObjectField.AccountAmortizationExtraCredit);
					return entryAccouts[ObjectField.AccountAmortizationExtraCredit];

				case EntryScenario.Increase:
					tooltip = DataDescriptions.GetObjectFieldDescription (ObjectField.AccountIncreaseCredit);
					return entryAccouts[ObjectField.AccountIncreaseCredit];

				case EntryScenario.Decrease:
					tooltip = DataDescriptions.GetObjectFieldDescription (ObjectField.AccountDecreaseCredit);
					return entryAccouts[ObjectField.AccountDecreaseCredit];

				case EntryScenario.Adjust:
					tooltip = DataDescriptions.GetObjectFieldDescription (ObjectField.AccountAdjustCredit);
					return entryAccouts[ObjectField.AccountAdjustCredit];

				default:
					return null;
			}
		}

		private string GetStamp(DataObject asset, DataEvent e, AmortizedAmount amount, GetEntryPropertiesType type)
		{
			//	Retourne la pièce de l'écriture.
			if (type == GetEntryPropertiesType.Current)
			{
				var p = e.GetProperty (ObjectField.AssetEntryForcedStamp) as DataStringProperty;
				if (p != null)
				{
					return p.Value;
				}
			}

			if (type == GetEntryPropertiesType.EditedOrBase)
			{
				var s = this.accessor.EditionAccessor.GetFieldString (ObjectField.AssetEntryForcedStamp, synthetic: false);
				if (!string.IsNullOrEmpty (s))
				{
					return s;
				}
			}

			return null;
		}

		private string GetTitle(DataObject asset, DataEvent e, AmortizedAmount amount, GetEntryPropertiesType type)
		{
			//	Retourne le libellé de l'écriture.
			if (type == GetEntryPropertiesType.Current)
			{
				var p = e.GetProperty (ObjectField.AssetEntryForcedTitle) as DataStringProperty;
				if (p != null)
				{
					return p.Value;
				}
			}

			if (type == GetEntryPropertiesType.EditedOrBase)
			{
				var s = this.accessor.EditionAccessor.GetFieldString (ObjectField.AssetEntryForcedTitle, synthetic: false);
				if (!string.IsNullOrEmpty (s))
				{
					return s;
				}
			}

			string name;

			if (type == GetEntryPropertiesType.Sample)
			{
				name = Res.Strings.Entries.Sample.DummyAsset.ToString ();
			}
			else
			{
				name = AssetsLogic.GetSummary (this.accessor, asset, e.Timestamp);
			}

			var title = EnumDictionaries.GetEntryScenarioTitle (amount.EntryScenario);

			if (string.IsNullOrEmpty (title))
			{
				return name;
			}
			else
			{
				return string.Format (title, name);
			}
		}

		private decimal GetValue(DataObject asset, DataEvent e, AmortizedAmount amount, GetEntryPropertiesType type)
		{
			//	Retourne le montant de l'écriture.
			if (type == GetEntryPropertiesType.Current)
			{
				var p = e.GetProperty (ObjectField.AssetEntryForcedAmount) as DataDecimalProperty;
				if (p != null)
				{
					return p.Value;
				}
			}

			if (type == GetEntryPropertiesType.EditedOrBase)
			{
				var d = this.accessor.EditionAccessor.GetFieldDecimal (ObjectField.AssetEntryForcedAmount, synthetic: false);
				if (d.HasValue)
				{
					return d.Value;
				}
			}

			switch (amount.EntryScenario)
			{
				case EntryScenario.Purchase:
					return amount.FinalAmount.GetValueOrDefault ();

				case EntryScenario.Sale:
					//	Lors d'une vente, la nouvelle valeur de l'objet est de zéro.
					//	Mais il faut passer une écriture avec le montant de la vente,
					//	c'est-à-dire la dernière valeur moins la valeur actuelle (en
					//	principe nulle).
					return amount.InitialAmount.GetValueOrDefault ()
						 - amount.FinalAmount.GetValueOrDefault ();

				case EntryScenario.AmortizationAuto:
				case EntryScenario.AmortizationExtra:
					return amount.InitialAmount.GetValueOrDefault ()
						 - amount.FinalAmount.GetValueOrDefault ();

				case EntryScenario.Increase:
				case EntryScenario.Decrease:
				case EntryScenario.Adjust:
					return amount.FinalAmount.GetValueOrDefault ();

				default:
					return 0.0m;
			}
		}

		private string GetVatCode(DataObject asset, DataEvent e, AmortizedAmount amount, GetEntryPropertiesType type)
		{
			//	Retourne le code TVA de l'écriture.
			if (type == GetEntryPropertiesType.Current)
			{
				var p = e.GetProperty (ObjectField.AssetEntryForcedVatCode) as DataStringProperty;
				if (p != null)
				{
					return p.Value;
				}
			}

			if (type == GetEntryPropertiesType.EditedOrBase)
			{
				var s = this.accessor.EditionAccessor.GetFieldString (ObjectField.AssetEntryForcedVatCode, synthetic: false);
				if (!string.IsNullOrEmpty (s))
				{
					return s;
				}
			}

			return null;
		}


		private DataObject CreateDataEntry(DataObject asset, DataEvent e, AmortizedAmount amount, ref Guid entryGuid, ref int entrySeed)
		{
			var entry = new DataObject (this.accessor.UndoManager);

			entryGuid = entry.Guid;
			entrySeed = 0;

			var entries = this.accessor.Mandat.GetData (BaseType.Entries);
			entries.Add (entry);

			var start  = new Timestamp (this.accessor.Mandat.StartDate, 0);
			var ee = new DataEvent (this.accessor.UndoManager, start, EventType.Input);
			entry.AddEvent (ee);

			ee.AddProperty (new DataGuidProperty (ObjectField.EntryAssetGuid, asset.Guid));
			ee.AddProperty (new DataGuidProperty (ObjectField.EntryEventGuid, e.Guid));

			return entry;
		}

		private Guid UpdateEntry(DataObject entry, EntryProperties entryProperties)
		{
			var e = entry.GetInputEvent ();

			e.AddProperty (new DataDateProperty    (ObjectField.EntryDate,          entryProperties.Date));
			e.AddProperty (new DataStringProperty  (ObjectField.EntryDebitAccount,  entryProperties.Debit));
			e.AddProperty (new DataStringProperty  (ObjectField.EntryCreditAccount, entryProperties.Credit));
			e.AddProperty (new DataStringProperty  (ObjectField.EntryStamp,         entryProperties.Stamp));
			e.AddProperty (new DataStringProperty  (ObjectField.EntryTitle,         entryProperties.Title));
			e.AddProperty (new DataDecimalProperty (ObjectField.EntryAmount,        entryProperties.Amount));
			e.AddProperty (new DataStringProperty  (ObjectField.EntryVatCode,       entryProperties.VatCode));

			return entry.Guid;
		}


		#region Static helpers
		public static EntryProperties GetEntryProperties(DataAccessor accessor, DataObject asset, DataEvent e, AmortizedAmount amount, GetEntryPropertiesType type)
		{
			if (accessor == null)
			{
				return null;
			}

			using (var entries = new Entries (accessor))
			{
				return entries.GetEntryProperties (asset, e, amount, type);
			}
		}

		public static bool HasEntry(DataAccessor accessor, DataObject asset, DataEvent e, AmortizedAmount aa)
		{
			if (accessor == null)
			{
				return false;
			}

			using (var entries = new Entries (accessor))
			{
				return entries.HasEntry (asset, e, aa);
			}
		}

		public static AmortizedAmount CreateEntry(DataAccessor accessor, DataObject asset, DataEvent e, AmortizedAmount aa)
		{
			if (accessor == null)
			{
				return aa;
			}

			using (var entries = new Entries (accessor))
			{
				return entries.CreateEntry (asset, e, aa);
			}
		}

		public static AmortizedAmount RemoveEntry(DataAccessor accessor, DataObject asset, DataEvent e, AmortizedAmount aa)
		{
			if (accessor == null)
			{
				return aa;
			}

			using (var entries = new Entries (accessor))
			{
				return entries.RemoveEntry (asset, e, aa);
			}
		}

		public static void ChangeEntryScenario(DataEvent e, EntryScenario scenario)
		{
			var p = e.GetProperty (ObjectField.MainValue) as DataAmortizedAmountProperty;
			var aa = AmortizedAmount.SetEntryScenario (p.Value, scenario);
			Amortizations.SetAmortizedAmount (e, aa);
		}
		#endregion


		private readonly DataAccessor			accessor;
	}
}
