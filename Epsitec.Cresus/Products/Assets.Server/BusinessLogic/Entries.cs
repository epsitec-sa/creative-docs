﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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


		private bool HasEntry(AmortizedAmount amount)
		{
			var editedProperties = this.GetEntryProperties (amount, GetEntryPropertiesType.EditedOrBase);
			return editedProperties.IsValid;
		}

		private AmortizedAmount CreateEntry(AmortizedAmount amount)
		{
			//	Crée l'écriture liée à un amortissement ordinaire.
			System.Diagnostics.Debug.Assert (amount.Date.Year != 1);

			var editedProperties = this.GetEntryProperties (amount, GetEntryPropertiesType.EditedOrBase);

			if (amount.EntryScenario == EntryScenario.None || !editedProperties.IsValid)
			{
				return this.RemoveEntry (amount);
			}
			else
			{
				var entryGuid = amount.EntryGuid;
				var entrySeed = amount.EntrySeed;

				var dataEntry = this.GetEntry (amount.AssetGuid, amount.EventGuid);

				if (dataEntry == null)
				{
					dataEntry = this.CreateDataEntry (amount, ref entryGuid, ref entrySeed);
				}

				var entryProperties = this.GetEntryProperties (amount, GetEntryPropertiesType.Current);
				this.UpdateEntry (dataEntry, entryProperties);

				return AmortizedAmount.SetEntry (amount, entryGuid, entrySeed+1);
			}
		}

		private AmortizedAmount RemoveEntry(AmortizedAmount amount)
		{
			//	Supprime l'écriture liée à un AmortizedAmount.
			var entry = this.GetEntry (amount.AssetGuid, amount.EventGuid);

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
			var aa = new AmortizedAmount (AmortizationType.Unknown,
				null, null, null, null, null, null, null, null,
				scenario, System.DateTime.Now, Guid.Empty, Guid.Empty, Guid.Empty, 0);

			var a01 = this.accessor.EditionAccessor.GetFieldString (ObjectField.AccountPurchaseDebit);
			var a02 = this.accessor.EditionAccessor.GetFieldString (ObjectField.AccountPurchaseCredit);
			var a03 = this.accessor.EditionAccessor.GetFieldString (ObjectField.AccountSaleDebit);
			var a04 = this.accessor.EditionAccessor.GetFieldString (ObjectField.AccountSaleCredit);
			var a05 = this.accessor.EditionAccessor.GetFieldString (ObjectField.AccountAmortizationAutoDebit);
			var a06 = this.accessor.EditionAccessor.GetFieldString (ObjectField.AccountAmortizationAutoCredit);
			var a07 = this.accessor.EditionAccessor.GetFieldString (ObjectField.AccountAmortizationExtraDebit);
			var a08 = this.accessor.EditionAccessor.GetFieldString (ObjectField.AccountAmortizationExtraCredit);
			var a09 = this.accessor.EditionAccessor.GetFieldString (ObjectField.AccountIncreaseDebit);
			var a10 = this.accessor.EditionAccessor.GetFieldString (ObjectField.AccountIncreaseCredit);
			var a11 = this.accessor.EditionAccessor.GetFieldString (ObjectField.AccountDecreaseDebit);
			var a12 = this.accessor.EditionAccessor.GetFieldString (ObjectField.AccountDecreaseCredit);

			var ea = new EntryAccounts (a01, a02, a03, a04, a05, a06, a07, a08, a09, a10, a11, a12);

			text    = null;
			tooltip = null;
			
			switch (column)
			{
				case 0:
					text = this.GetDebit (aa, null, ea, GetEntryPropertiesType.Sample, out tooltip);
					break;
			
				case 1:
					text = this.GetCredit (aa, null, ea, GetEntryPropertiesType.Sample, out tooltip);
					break;
			
				case 2:
					text = this.GetTitle (aa, null, GetEntryPropertiesType.Sample);
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

		public EntryProperties GetEntryProperties(AmortizedAmount amount, GetEntryPropertiesType type)
		{
			var asset = this.accessor.GetObject (BaseType.Assets, amount.AssetGuid);
			System.Diagnostics.Debug.Assert (asset != null);

			var assetEvent = asset.GetEvent (amount.EventGuid);
			System.Diagnostics.Debug.Assert (assetEvent != null);

			var entryAccounts = this.GetEntryAccounts (amount);

			string tooltip;

			return new EntryProperties
			{
				Date   = this.GetDate   (amount, assetEvent,                type),
				Debit  = this.GetDebit  (amount, assetEvent, entryAccounts, type, out tooltip),
				Credit = this.GetCredit (amount, assetEvent, entryAccounts, type, out tooltip),
				Stamp  = this.GetStamp  (amount, assetEvent,                type),
				Title  = this.GetTitle  (amount, assetEvent,                type),
				Amount = this.GetValue  (amount, assetEvent,                type),
			};
		}


		private EntryAccounts GetEntryAccounts(AmortizedAmount amount)
		{
			//	Retourne la liste des comptes à utiliser pour passer une écriture liée
			//	à l'événement contenant ce montant.
			if (this.accessor != null)
			{
				var obj = this.accessor.GetObject (BaseType.Assets, amount.AssetGuid);
				if (obj != null)
				{
					var timestamp = new Timestamp (amount.Date, 0);

					return new EntryAccounts
					(
						ObjectProperties.GetObjectPropertyString (obj, timestamp, ObjectField.AccountPurchaseDebit),
						ObjectProperties.GetObjectPropertyString (obj, timestamp, ObjectField.AccountPurchaseCredit),
						ObjectProperties.GetObjectPropertyString (obj, timestamp, ObjectField.AccountSaleDebit),
						ObjectProperties.GetObjectPropertyString (obj, timestamp, ObjectField.AccountSaleCredit),
						ObjectProperties.GetObjectPropertyString (obj, timestamp, ObjectField.AccountAmortizationAutoDebit),
						ObjectProperties.GetObjectPropertyString (obj, timestamp, ObjectField.AccountAmortizationAutoCredit),
						ObjectProperties.GetObjectPropertyString (obj, timestamp, ObjectField.AccountAmortizationExtraDebit),
						ObjectProperties.GetObjectPropertyString (obj, timestamp, ObjectField.AccountAmortizationExtraCredit),
						ObjectProperties.GetObjectPropertyString (obj, timestamp, ObjectField.AccountIncreaseDebit),
						ObjectProperties.GetObjectPropertyString (obj, timestamp, ObjectField.AccountIncreaseCredit),
						ObjectProperties.GetObjectPropertyString (obj, timestamp, ObjectField.AccountDecreaseDebit),
						ObjectProperties.GetObjectPropertyString (obj, timestamp, ObjectField.AccountDecreaseCredit)
					);
				}
			}

			return EntryAccounts.Empty;
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


		private System.DateTime GetDate(AmortizedAmount amount, DataEvent assetEvent, GetEntryPropertiesType type)
		{
			//	Retourne la date à utiliser pour l'écriture.
			if (type == GetEntryPropertiesType.Current)
			{
				var p = assetEvent.GetProperty (ObjectField.AssetEntryForcedDate) as DataDateProperty;
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

			return amount.Date;
		}

		private string GetDebit(AmortizedAmount amount, DataEvent assetEvent, EntryAccounts entryAccouts, GetEntryPropertiesType type, out string tooltip)
		{
			//	Retourne le compte à utiliser au débit de l'écriture.
			tooltip = null;

			if (type == GetEntryPropertiesType.Current)
			{
				var p = assetEvent.GetProperty (ObjectField.AssetEntryForcedDebit) as DataStringProperty;
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
					return entryAccouts.AccountPurchaseDebit;

				case EntryScenario.Sale:
					tooltip = DataDescriptions.GetObjectFieldDescription (ObjectField.AccountSaleDebit);
					return entryAccouts.AccountSaleDebit;

				case EntryScenario.AmortizationAuto:
					tooltip = DataDescriptions.GetObjectFieldDescription (ObjectField.AccountAmortizationAutoDebit);
					return entryAccouts.AccountAmortizationAutoDebit;

				case EntryScenario.AmortizationExtra:
					tooltip = DataDescriptions.GetObjectFieldDescription (ObjectField.AccountAmortizationExtraDebit);
					return entryAccouts.AccountAmortizationExtraDebit;

				case EntryScenario.Increase:
					tooltip = DataDescriptions.GetObjectFieldDescription (ObjectField.AccountIncreaseDebit);
					return entryAccouts.AccountIncreaseDebit;

				case EntryScenario.Decrease:
					tooltip = DataDescriptions.GetObjectFieldDescription (ObjectField.AccountDecreaseDebit);
					return entryAccouts.AccountDecreaseDebit;

				default:
					return null;
			}
		}

		private string GetCredit(AmortizedAmount amount, DataEvent assetEvent, EntryAccounts entryAccouts, GetEntryPropertiesType type, out string tooltip)
		{
			//	Retourne le compte à utiliser au crédit de l'écriture.
			tooltip = null;

			if (type == GetEntryPropertiesType.Current)
			{
				var p = assetEvent.GetProperty (ObjectField.AssetEntryForcedCredit) as DataStringProperty;
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
					return entryAccouts.AccountPurchaseCredit;

				case EntryScenario.Sale:
					tooltip = DataDescriptions.GetObjectFieldDescription (ObjectField.AccountSaleCredit);
					return entryAccouts.AccountSaleCredit;

				case EntryScenario.AmortizationAuto:
					tooltip = DataDescriptions.GetObjectFieldDescription (ObjectField.AccountAmortizationAutoCredit);
					return entryAccouts.AccountAmortizationAutoCredit;

				case EntryScenario.AmortizationExtra:
					tooltip = DataDescriptions.GetObjectFieldDescription (ObjectField.AccountAmortizationExtraCredit);
					return entryAccouts.AccountAmortizationExtraCredit;

				case EntryScenario.Increase:
					tooltip = DataDescriptions.GetObjectFieldDescription (ObjectField.AccountIncreaseCredit);
					return entryAccouts.AccountIncreaseCredit;

				case EntryScenario.Decrease:
					tooltip = DataDescriptions.GetObjectFieldDescription (ObjectField.AccountDecreaseCredit);
					return entryAccouts.AccountDecreaseCredit;

				default:
					return null;
			}
		}

		private string GetStamp(AmortizedAmount amount, DataEvent assetEvent, GetEntryPropertiesType type)
		{
			//	Retourne la pièce de l'écriture.
			if (type == GetEntryPropertiesType.Current)
			{
				var p = assetEvent.GetProperty (ObjectField.AssetEntryForcedStamp) as DataStringProperty;
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

		private string GetTitle(AmortizedAmount amount, DataEvent assetEvent, GetEntryPropertiesType type)
		{
			//	Retourne le libellé de l'écriture.
			if (type == GetEntryPropertiesType.Current)
			{
				var p = assetEvent.GetProperty (ObjectField.AssetEntryForcedTitle) as DataStringProperty;
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
				var guid = amount.AssetGuid;
				var timestamp = new Timestamp (amount.Date, 0);

				name = AssetsLogic.GetSummary (this.accessor, guid, timestamp);
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

		private decimal GetValue(AmortizedAmount amount, DataEvent assetEvent, GetEntryPropertiesType type)
		{
			//	Retourne le montant de l'écriture.
			if (type == GetEntryPropertiesType.Current)
			{
				var p = assetEvent.GetProperty (ObjectField.AssetEntryForcedAmount) as DataDecimalProperty;
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
					return amount.FinalAmortizedAmount.GetValueOrDefault ();

				case EntryScenario.Sale:
					//	Lors d'une vente, la nouvelle valeur de l'objet est de zéro.
					//	Mais il faut passer une écriture avec le montant de la vente,
					//	c'est-à-dire la dernière valeur moins la valeur actuelle (en
					//	principe nulle).
					return amount.PreviousAmount.GetValueOrDefault ()
						 - amount.FinalAmortizedAmount.GetValueOrDefault ();

				case EntryScenario.AmortizationAuto:
				case EntryScenario.AmortizationExtra:
					return amount.FinalAmortization;

				case EntryScenario.Increase:
				case EntryScenario.Decrease:
					return amount.FinalAmortizedAmount.GetValueOrDefault ();

				default:
					return 0.0m;
			}
		}


		private DataObject CreateDataEntry(AmortizedAmount amount, ref Guid entryGuid, ref int entrySeed)
		{
			var entry = new DataObject (this.accessor.UndoManager);

			entryGuid = entry.Guid;
			entrySeed = 0;

			var entries = this.accessor.Mandat.GetData (BaseType.Entries);
			entries.Add (entry);

			var start  = new Timestamp (this.accessor.Mandat.StartDate, 0);
			var e = new DataEvent (this.accessor.UndoManager, start, EventType.Input);
			entry.AddEvent (e);

			e.AddProperty (new DataGuidProperty (ObjectField.EntryAssetGuid, amount.AssetGuid));
			e.AddProperty (new DataGuidProperty (ObjectField.EntryEventGuid, amount.EventGuid));

			return entry;
		}

		private Guid UpdateEntry(DataObject entry, EntryProperties entryProperties)
		{
			var e = entry.GetEvent (0);

			e.AddProperty (new DataDateProperty    (ObjectField.EntryDate,          entryProperties.Date));
			e.AddProperty (new DataStringProperty  (ObjectField.EntryDebitAccount,  entryProperties.Debit));
			e.AddProperty (new DataStringProperty  (ObjectField.EntryCreditAccount, entryProperties.Credit));
			e.AddProperty (new DataStringProperty  (ObjectField.EntryStamp,         entryProperties.Stamp));
			e.AddProperty (new DataStringProperty  (ObjectField.EntryTitle,         entryProperties.Title));
			e.AddProperty (new DataDecimalProperty (ObjectField.EntryAmount,        entryProperties.Amount));

			return entry.Guid;
		}


		#region Static helpers
		public static EntryProperties GetEntryProperties(DataAccessor accessor, AmortizedAmount amount, GetEntryPropertiesType type)
		{
			if (accessor == null)
			{
				return null;
			}

			using (var entries = new Entries (accessor))
			{
				return entries.GetEntryProperties (amount, type);
			}
		}

		public static bool HasEntry(DataAccessor accessor, AmortizedAmount aa)
		{
			if (accessor == null)
			{
				return false;
			}

			using (var entries = new Entries (accessor))
			{
				return entries.HasEntry (aa);
			}
		}

		public static AmortizedAmount CreateEntry(DataAccessor accessor, AmortizedAmount aa)
		{
			if (accessor == null)
			{
				return aa;
			}

			using (var entries = new Entries (accessor))
			{
				return entries.CreateEntry (aa);
			}
		}

		public static AmortizedAmount RemoveEntry(DataAccessor accessor, AmortizedAmount aa)
		{
			if (accessor == null)
			{
				return aa;
			}

			using (var entries = new Entries (accessor))
			{
				return entries.RemoveEntry (aa);
			}
		}
		#endregion


		private readonly DataAccessor			accessor;
	}
}
