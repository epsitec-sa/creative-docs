//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class WarningsLogic
	{
		public static bool IsRequired(DataAccessor accessor, BaseType baseType, ObjectField field)
		{
			//	Retourne true si un champ est requis.
			if (field >= ObjectField.UserFieldFirst &&
				field <= ObjectField.UserFieldLast)
			{
				//	On fouille tous les champs définis comme "obligatoire" par l'utilisateur.
				switch (baseType.Kind)
				{
					case BaseTypeKind.Assets:
						baseType = BaseType.AssetsUserFields;
						break;

					case BaseTypeKind.Persons:
						baseType = BaseType.PersonsUserFields;
						break;

					default:
						throw new System.InvalidOperationException (string.Format ("Unknown BaseType {0}", baseType.ToString ()));
				}

				return accessor.Mandat.GlobalSettings.GetUserFields (baseType)
					.Where (x => x.Field == field && x.Required)
					.Any ();
			}
			else if (baseType == BaseType.Categories)
			{
				//	Liste des champs obligatoires d'une catégorie d'immobilisation.
				return field == ObjectField.Name
					|| field == ObjectField.AmortizationRate
					|| field == ObjectField.AmortizationType
					|| field == ObjectField.Periodicity
					|| field == ObjectField.Prorata
					|| field == ObjectField.Round
					|| field == ObjectField.ResidualValue;
			}
			else if (baseType == BaseType.Assets)
			{
				//	Liste des champs obligatoires d'un objet d'immobilisation.
				return field == ObjectField.MainValue
					|| field == ObjectField.CategoryName
					|| field == ObjectField.AmortizationRate
					|| field == ObjectField.AmortizationType
					|| field == ObjectField.Periodicity
					|| field == ObjectField.Prorata
					|| field == ObjectField.Round
					|| field == ObjectField.ResidualValue;
			}
			else if (baseType == BaseType.Groups)
			{
				//	Liste des champs obligatoires d'un groupe.
				return field == ObjectField.Name
					|| field == ObjectField.Number;
			}
			else
			{
				return false;
			}
		}


		public static void GetWarnings(List<Warning> warnings, DataAccessor accessor)
		{
			//	Retourne la liste de tous les warnings actuels.
			warnings.Clear ();

			//	Il doit y avoir au moins un champ obligatoire.
			WarningsLogic.CheckRequiredField (warnings, accessor, BaseType.AssetsUserFields,  Res.Strings.WarningsLogic.RequiredUserFields.Missing.ToString ());
			WarningsLogic.CheckRequiredField (warnings, accessor, BaseType.PersonsUserFields, Res.Strings.WarningsLogic.RequiredUserFields.Missing.ToString ());

			//	On cherche les champs indéfinis dans les catégories.
			WarningsLogic.CheckEmpty (warnings, accessor, BaseType.Categories,
				ObjectField.Name,
				ObjectField.AmortizationRate,
				ObjectField.AmortizationType,
				ObjectField.Periodicity,
				ObjectField.Prorata,
				ObjectField.Round,
				ObjectField.ResidualValue);

			//	On cherche les comptes indéfinis dans les catégories.
			foreach (var cat in accessor.Mandat.GetData (BaseType.Categories))
			{
				//	On cherche les comptes indéfinis.
				bool skip = false;
				WarningsLogic.CheckCategoryAccount (warnings, accessor, cat, ObjectField.Account1, ref skip);
				WarningsLogic.CheckCategoryAccount (warnings, accessor, cat, ObjectField.Account2, ref skip);
				WarningsLogic.CheckCategoryAccount (warnings, accessor, cat, ObjectField.Account3, ref skip);
				WarningsLogic.CheckCategoryAccount (warnings, accessor, cat, ObjectField.Account4, ref skip);
				WarningsLogic.CheckCategoryAccount (warnings, accessor, cat, ObjectField.Account5, ref skip);
				WarningsLogic.CheckCategoryAccount (warnings, accessor, cat, ObjectField.Account6, ref skip);
				WarningsLogic.CheckCategoryAccount (warnings, accessor, cat, ObjectField.Account7, ref skip);
			}

			//	On cherche les champs indéfinis dans les groupes.
			WarningsLogic.CheckEmpty (warnings, accessor, BaseType.Groups,
				ObjectField.Name,
				ObjectField.Number);

			//	On s'occupe des objets d'immobilisation.
			foreach (var asset in accessor.Mandat.GetData (BaseType.Assets))
			{
				foreach (var e in asset.Events)
				{
					if (e.Type == EventType.Input)
					{
						//	On cherche si la valeur comptable est indéfinie à l'entrée.
						WarningsLogic.CheckEmpty (warnings, BaseType.Assets, asset, e, ObjectField.MainValue);

						//	On cherche les champs définis par l'utilisateur restés indéfinis.
						var requiredFields = accessor.Mandat.GlobalSettings.GetUserFields (BaseType.AssetsUserFields)
							.Where (x => x.Required)
							.Select (x => x.Field)
							.ToArray ();
						WarningsLogic.CheckEmpty (warnings, BaseType.Assets, asset, e, requiredFields);
					}

					//	On cherche les champs pour l'amortissement indéfinis.
					WarningsLogic.CheckEmpty (warnings, BaseType.Assets, asset, e,
						ObjectField.CategoryName,
						ObjectField.AmortizationRate,
						ObjectField.AmortizationType,
						ObjectField.Periodicity,
						ObjectField.Prorata,
						ObjectField.Round,
						ObjectField.ResidualValue);

					//	On cherche les comptes indéfinis.
					if (WarningsLogic.IsDefinableAccount (e.Type))
					{
						bool skip = false;
						WarningsLogic.CheckAssetAccount (warnings, accessor, asset, e, ObjectField.Account1, ref skip);
						WarningsLogic.CheckAssetAccount (warnings, accessor, asset, e, ObjectField.Account2, ref skip);
						WarningsLogic.CheckAssetAccount (warnings, accessor, asset, e, ObjectField.Account3, ref skip);
						WarningsLogic.CheckAssetAccount (warnings, accessor, asset, e, ObjectField.Account4, ref skip);
						WarningsLogic.CheckAssetAccount (warnings, accessor, asset, e, ObjectField.Account5, ref skip);
						WarningsLogic.CheckAssetAccount (warnings, accessor, asset, e, ObjectField.Account6, ref skip);
						WarningsLogic.CheckAssetAccount (warnings, accessor, asset, e, ObjectField.Account7, ref skip);
					}

					//	On cherche les comptes incorrects dans les écritures.
					WarningsLogic.CheckAssetEntry (warnings, accessor, asset, e);
				}
			}

			//	On cherche les champs indéfinis dans les personnes.
			WarningsLogic.CheckRequired (warnings, accessor, BaseType.PersonsUserFields);

			//	Vérifie si certaines bases sont vides.
			bool checkAssets = true;

			if (!accessor.Mandat.GetData (BaseType.Categories).Any ())
			{
				var warning = new Warning (BaseType.Categories, Guid.Empty, Guid.Empty, ObjectField.Unknown, Res.Strings.WarningsLogic.Undefined.Categories.ToString ());
				warnings.Add (warning);
				checkAssets = false;
			}

			if (accessor.Mandat.GetData (BaseType.Groups).Count () <= 1)
			{
				var warning = new Warning (BaseType.Groups, Guid.Empty, Guid.Empty, ObjectField.Unknown, Res.Strings.WarningsLogic.Undefined.Groups.ToString ());
				warnings.Add (warning);
				checkAssets = false;
			}

			if (checkAssets && !accessor.Mandat.GetData (BaseType.Assets).Any ())
			{
				var warning = new Warning (BaseType.Assets, Guid.Empty, Guid.Empty, ObjectField.Unknown, Res.Strings.WarningsLogic.Undefined.Assets.ToString ());
				warnings.Add (warning);
			}

			if (!accessor.Mandat.AccountsDateRanges.Any ())
			{
				var warning = new Warning (BaseType.Accounts, Guid.Empty, Guid.Empty, ObjectField.Unknown, Res.Strings.WarningsLogic.Undefined.Accounts.ToString ());
				warnings.Add (warning);
			}
		}


		private static void CheckRequiredField(List<Warning> warnings, DataAccessor accessor, BaseType baseType, string description)
		{
			//	Vérifie s'il existe au moins un champ obligatoire.
			if (!accessor.GlobalSettings.GetUserFields (baseType).Where (x => x.Required).Any ())
			{
				var first = accessor.GlobalSettings.GetUserFields (baseType).FirstOrDefault ();

				var warning = new Warning (baseType, first.Guid, Guid.Empty, ObjectField.Unknown, description);
				warnings.Add (warning);
			}
		}


		private static void CheckCategoryAccount(List<Warning> warnings, DataAccessor accessor, DataObject cat, ObjectField accountField, ref bool skip)
		{
			//	Vérification d'un compte d'une catégorie d'immobilisation.
			if (skip)
			{
				return;
			}

			var e = cat.GetEvent (0);
			var p = ObjectProperties.GetObjectProperty (cat, e.Timestamp, accountField, synthetic: true) as DataStringProperty;

			if (p == null || string.IsNullOrEmpty (p.Value))
			{
				//	Il est permis de ne pas définir un compte. Il n'y aura simplement pas d'écriture
				//	générée.
			}
			else
			{
				var number = p.Value;  // numéro du compte (ex. "1000")
				var lastDate = accessor.Mandat.AccountsDateRanges.LastOrDefault ().IncludeFrom;
				var baseType = accessor.Mandat.GetAccountsBase (lastDate);

				if (baseType.AccountsDateRange.IsEmpty)
				{
					string desc = Res.Strings.WarningsLogic.NoAccounts.ToString ();
					var warning = new Warning (BaseType.Categories, cat.Guid, Guid.Empty, accountField, desc);
					warnings.Add (warning);

					skip = true;  // il n'est pas pertinent de signaler cette erreur pour les autres comptes
				}
				else
				{
					var account = AccountsLogic.GetAccount (accessor, baseType, number);

					if (account == null)
					{
						var desc = string.Format (Res.Strings.WarningsLogic.NotInAccounts.ToString (), number);
						var warning = new Warning (BaseType.Categories, cat.Guid, Guid.Empty, accountField, desc);
						warnings.Add (warning);
					}
				}
			}
		}

		private static void CheckAssetAccount(List<Warning> warnings, DataAccessor accessor, DataObject asset, DataEvent e, ObjectField accountField, ref bool skip)
		{
			//	Vérification d'un compte d'un objet d'immobilisation.
			if (skip)
			{
				return;
			}

			var p = ObjectProperties.GetObjectProperty (asset, e.Timestamp, accountField, synthetic: true) as DataStringProperty;

			if (p == null || string.IsNullOrEmpty (p.Value))
			{
				//	Il est permis de ne pas définir un compte. Il n'y aura simplement pas d'écriture
				//	générée.
			}
			else
			{
				var number = p.Value;  // numéro du compte (ex. "1000")
				var baseType = accessor.Mandat.GetAccountsBase (e.Timestamp.Date);

				if (baseType.AccountsDateRange.IsEmpty)
				{
					string desc = Res.Strings.WarningsLogic.NoAccountsToDate.ToString ();
					var warning = new Warning (BaseType.Assets, asset.Guid, e.Guid, accountField, desc);
					warnings.Add (warning);

					skip = true;  // il n'est pas pertinent de signaler cette erreur pour les autres comptes
				}
				else
				{
					var account = AccountsLogic.GetAccount (accessor, baseType, number);

					if (account == null)
					{
						var desc = string.Format (Res.Strings.WarningsLogic.NotInAccountsToDate.ToString (), number);
						var warning = new Warning (BaseType.Assets, asset.Guid, e.Guid, accountField, desc);
						warnings.Add (warning);
					}
				}
			}
		}


		private static void CheckAssetEntry(List<Warning> warnings, DataAccessor accessor, DataObject asset, DataEvent e)
		{
			//	Vérifie les comptes d'une écriture d'un événement d'un objet d'immobilisation.
			var p = ObjectProperties.GetObjectProperty (asset, e.Timestamp, ObjectField.MainValue, synthetic: false) as DataAmortizedAmountProperty;

			if (p != null)
			{
				var aa = p.Value;
				var ep = Entries.GetEntryProperties (accessor, aa, Entries.GetEntryPropertiesType.Current);

				if (ep != null)
				{
					bool skip = false;
					WarningsLogic.CheckAssetEntry (warnings, accessor, asset, e, ep.Debit , ref skip);
					WarningsLogic.CheckAssetEntry (warnings, accessor, asset, e, ep.Credit, ref skip);
				}
			}
		}

		private static void CheckAssetEntry(List<Warning> warnings, DataAccessor accessor, DataObject asset, DataEvent e, string number, ref bool skip)
		{
			if (skip || string.IsNullOrEmpty(number))
			{
				return;
			}

			var baseType = accessor.Mandat.GetAccountsBase (e.Timestamp.Date);

			if (baseType.AccountsDateRange.IsEmpty)
			{
				if (!WarningsLogic.IsDefinableAccount (e.Type))
				{
					string desc = Res.Strings.WarningsLogic.NoAccountsToDate.ToString ();
					var warning = new Warning (BaseType.Assets, asset.Guid, e.Guid, ObjectField.MainValue, desc);
					warnings.Add (warning);

					skip = true;  // il n'est pas pertinent de signaler cette erreur pour les autres comptes
				}
			}
			else
			{
				var account = AccountsLogic.GetAccount (accessor, baseType, number);

				if (account == null)
				{
					var desc = string.Format (Res.Strings.WarningsLogic.NotInAccountsToDate.ToString (), number);
					var warning = new Warning (BaseType.Assets, asset.Guid, e.Guid, ObjectField.MainValue, desc);
					warnings.Add (warning);
				}
			}
		}


		private static bool IsDefinableAccount(EventType type)
		{
			//	Pour un type d'événement, indique si on a accès aux définitions des
			//	comptes (donc si l'onglet "Amortissement" est présent).
			//	Doit être en accord avec ObjectEditor.GetAssetAvailablePages !
			return type == EventType.Input
				|| type == EventType.Modification
				|| type == EventType.Output;
		}


		private static void CheckRequired(List<Warning> warnings, DataAccessor accessor, BaseType baseType)
		{
			var requiredFields = accessor.Mandat.GlobalSettings.GetUserFields (baseType)
				.Where (x => x.Required)
				.Select (x => x.Field)
				.ToArray ();

			WarningsLogic.CheckEmpty (warnings, accessor, BaseType.Persons, requiredFields);
		}


		private static void CheckEmpty(List<Warning> warnings, DataAccessor accessor, BaseType baseType, params ObjectField[] fields)
		{
			bool first = true;

			foreach (var obj in accessor.Mandat.GetData (baseType))
			{
				//	Dans la base des groupes, la première ligne n'a jamais de numéro
				//	(c'est l'objet "Groupes", père de tous les groupes).
				//	Il faut donc l'ignorer.
				if (baseType.Kind != BaseTypeKind.Groups || !first)
				{
					foreach (var field in fields)
					{
						WarningsLogic.CheckEmpty (warnings, baseType, obj, field);
					}
				}

				first = false;
			}
		}

		private static void CheckEmpty(List<Warning> warnings, BaseType baseType, DataObject obj, ObjectField field)
		{
			var e = obj.GetEvent (0);
			var p = e.GetProperty (field);
			WarningsLogic.CheckEmpty (warnings, baseType, obj, Guid.Empty, field, p);
		}

		private static void CheckEmpty(List<Warning> warnings, BaseType baseType, DataObject asset, DataEvent e, params ObjectField[] fields)
		{
			foreach (var field in fields)
			{
				var p = ObjectProperties.GetObjectProperty (asset, e.Timestamp, field, synthetic: true);
				WarningsLogic.CheckEmpty (warnings, baseType, asset, e.Guid, field, p);
			}
		}

		private static void CheckEmpty(List<Warning> warnings, BaseType baseType, DataObject obj, Guid eventGuid, ObjectField field, AbstractDataProperty p)
		{
			bool hasWarning = false;

			if (p == null)
			{
				hasWarning = true;
			}
			else
			{
				if (p is DataStringProperty)
				{
					var pp  = p as DataStringProperty;
					hasWarning = string.IsNullOrEmpty (pp.Value);
				}
				else if (p is DataAmortizedAmountProperty)
				{
					var pp  = p as DataAmortizedAmountProperty;
					hasWarning = !pp.Value.FinalAmortizedAmount.HasValue;
				}
			}

			if (hasWarning)
			{
				string desc = Res.Strings.WarningsLogic.UndefinedField.ToString ();
				var warning = new Warning (baseType, obj.Guid, eventGuid, field, desc);
				warnings.Add (warning);
			}
		}
	}
}
