//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Core.Helpers;
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

				return accessor.UserFieldsAccessor.GetUserFields (baseType)
					.Where (x => x.Field == field && x.Required)
					.Any ();
			}
			else if (baseType == BaseType.Categories)
			{
				//	Liste des champs obligatoires d'une catégorie d'immobilisation.
				return field == ObjectField.Name
					|| field == ObjectField.MethodGuid
					|| field == ObjectField.Periodicity;
			}
			else if (baseType == BaseType.Assets)
			{
				//	Liste des champs obligatoires d'un objet d'immobilisation.
				return field == ObjectField.MainValue
					|| field == ObjectField.CategoryName
					|| field == ObjectField.MethodGuid
					|| field == ObjectField.Periodicity;
			}
			else if (baseType == BaseType.Groups)
			{
				//	Liste des champs obligatoires d'un groupe.
				return field == ObjectField.Name
					|| field == ObjectField.Number;
			}
			else if (baseType == BaseType.Methods)
			{
				//	Liste des champs obligatoires d'une méthode.
				return field == ObjectField.Name;
			}
			else if (baseType == BaseType.Arguments)
			{
				//	Liste des champs obligatoires d'un argument.
				return field == ObjectField.Name
					|| field == ObjectField.ArgumentType
					|| field == ObjectField.ArgumentVariable;
			}
			else
			{
				return false;
			}
		}


		public static string GetError(DataAccessor accessor, DataObject asset, DataEvent e, ObjectField field, bool showAccounts)
		{
			//	Retourne l'éventuelle erreur liée à un champ d'un événement d'un objet d'immobilisation.
			var warnings = new List<Warning> ();

			//	Cherche l'ensemble des erreurs pour l'événement de l'objet.
			WarningsLogic.CheckAsset (warnings, accessor, asset, e, showAccounts);

			//	On s'intéresse uniquement à la première erreur qui concerne le champ.
			var warning = warnings.Where (x => x.Field == field).FirstOrDefault ();
			if (!warning.IsEmpty)
			{
				return warning.Description;
			}

			return null;  // ok
		}


		public static List<Warning> GetWarnings(DataAccessor accessor, bool showAccounts)
		{
			//	Retourne la liste de tous les warnings actuels.
			var warnings = new List<Warning> ();

			//	Il doit y avoir au moins un champ obligatoire.
			WarningsLogic.CheckRequiredField (warnings, accessor, BaseType.AssetsUserFields,  Res.Strings.WarningsLogic.RequiredUserFields.Missing.ToString ());
			WarningsLogic.CheckRequiredField (warnings, accessor, BaseType.PersonsUserFields, Res.Strings.WarningsLogic.RequiredUserFields.Missing.ToString ());

			//	On cherche les champs indéfinis dans les catégories.
			WarningsLogic.CheckEmpty (warnings, accessor, BaseType.Categories,
				ObjectField.Name,
				ObjectField.MethodGuid,
				ObjectField.Periodicity);

			//	On cherche les comptes indéfinis dans les catégories.
			foreach (var cat in accessor.Mandat.GetData (BaseType.Categories))
			{
				//	On cherche les comptes indéfinis.
				bool skip = false;
				foreach (var field in DataAccessor.AccountFields)
				{
					WarningsLogic.CheckCategoryAccount (warnings, accessor, cat, field, ref skip);
				}
			}

			//	On cherche les méthodes avec des erreurs de compilation.
			foreach (var method in accessor.Mandat.GetData (BaseType.Methods))
			{
				var arguments = ArgumentsLogic.GetArgumentsDotNetCode (accessor, method);
				var expression = ObjectProperties.GetObjectPropertyString (method, null, ObjectField.Expression);
				if (!accessor.AmortizationExpressions.Check (arguments, expression))
				{
					var warning = new Warning (BaseType.Methods, method.Guid, Guid.Empty, ObjectField.Expression, Res.Strings.WarningsLogic.Expression.CompileError.ToString ());
					warnings.Add (warning);
				}
			}

			//	On cherche les champs indéfinis dans les groupes.
			WarningsLogic.CheckEmpty (warnings, accessor, BaseType.Groups,
				ObjectField.Name,
				ObjectField.Number);

			//	On cherche les champs indéfinis dans les méthodes.
			WarningsLogic.CheckEmpty (warnings, accessor, BaseType.Methods,
				ObjectField.Name);

			//	On cherche les champs indéfinis dans les arguments.
			WarningsLogic.CheckEmpty (warnings, accessor, BaseType.Arguments,
				ObjectField.Name,
				ObjectField.ArgumentType,
				ObjectField.ArgumentVariable);

			//	On s'occupe des objets d'immobilisation.
			foreach (var asset in accessor.Mandat.GetData (BaseType.Assets))
			{
				foreach (var e in asset.Events)
				{
					WarningsLogic.CheckAsset (warnings, accessor, asset, e, showAccounts);
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
				//	Si les catégories et les groupes sont définis, on vérifie si les objets
				//	d'immobilisation sont définis.
				var warning = new Warning (BaseType.Assets, Guid.Empty, Guid.Empty, ObjectField.Unknown, Res.Strings.WarningsLogic.Undefined.Assets.ToString ());
				warnings.Add (warning);
			}

			if (!accessor.Mandat.AccountsDateRanges.Any ())
			{
				var warning = new Warning (BaseType.Accounts, Guid.Empty, Guid.Empty, ObjectField.Unknown, Res.Strings.WarningsLogic.Undefined.Accounts.ToString ());
				warnings.Add (warning);
			}

			//	Vérifie les amortissements.
			WarningsLogic.CheckAmortizations (warnings, accessor);

			return warnings;
		}

		private static void CheckAsset(List<Warning> warnings, DataAccessor accessor, DataObject asset, DataEvent e, bool showAccounts)
		{
			//	Cherche toutes les erreurs liées à un événement d'un objet d'immobilisation.
			if (e.Type == EventType.PreInput ||
				e.Type == EventType.Input)
			{
				//	On cherche si la valeur comptable est indéfinie à l'entrée.
				WarningsLogic.CheckEmpty (warnings, accessor, asset, e, ObjectField.MainValue);

				//	On cherche les champs définis par l'utilisateur restés indéfinis.
				var requiredFields = accessor.UserFieldsAccessor.GetUserFields (BaseType.AssetsUserFields)
					.Where (x => x.Required)
					.Select (x => x.Field)
					.ToArray ();
				WarningsLogic.CheckEmpty (warnings, accessor, asset, e, requiredFields);
			}

			if (WarningsLogic.IsDefinableAccount (e.Type))
			{
				//	On cherche les champs pour l'amortissement indéfinis.
				WarningsLogic.CheckEmpty (warnings, accessor, asset, e,
				ObjectField.CategoryName,
				ObjectField.MethodGuid,
				ObjectField.Periodicity);

				if (showAccounts)
				{
					//	On cherche les comptes indéfinis.
					bool skip = false;
					foreach (var field in DataAccessor.AccountFields)
					{
						WarningsLogic.CheckAssetAccount (warnings, accessor, asset, e, field, ref skip);
					}
				}
			}

			if (showAccounts)
			{
				//	On cherche les comptes incorrects dans les écritures.
				WarningsLogic.CheckAssetEntry (warnings, accessor, asset, e);
			}

			//	On cherche les groupes avec des ratios dont la somme n'est pas 100%.
			var guid = GroupsGuidRatioLogic.GetPercentErrorGroupGuid (accessor, e);
			if (!guid.IsEmpty)
			{
				var groupName = GroupsLogic.GetShortName (accessor, guid);
				var message = string.Format (Res.Strings.WarningsLogic.Ratio.Percent.ToString (), groupName);
				var warning = new Warning (BaseType.Assets, asset.Guid, e.Guid, ObjectField.GroupGuidRatioFirst, message);
				warnings.Add (warning);
			}
		}


		private static void CheckRequiredField(List<Warning> warnings, DataAccessor accessor, BaseType baseType, string description)
		{
			//	Vérifie s'il existe au moins un champ obligatoire.
			if (!accessor.UserFieldsAccessor.GetUserFields (baseType).Where (x => x.Required).Any ())
			{
				var first = accessor.UserFieldsAccessor.GetUserFields (baseType).FirstOrDefault ();
				Guid guid;

				if (first == null)
				{
					guid = Guid.Empty;
				}
				else
				{
					guid = first.Guid;
				}

				var warning = new Warning (baseType, guid, Guid.Empty, ObjectField.Unknown, description);
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

			var e = cat.GetInputEvent ();
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

			if (p != null && !p.Value.EntryGuid.IsEmpty)
			{
				var aa = p.Value;
				var ep = Entries.GetEntryProperties (accessor, asset, e, aa, Entries.GetEntryPropertiesType.Current);

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
			return type == EventType.PreInput
				|| type == EventType.Input
				|| type == EventType.Modification
				|| type == EventType.Output;
		}


		private static void CheckRequired(List<Warning> warnings, DataAccessor accessor, BaseType baseType)
		{
			var requiredFields = accessor.UserFieldsAccessor.GetUserFields (baseType)
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
						WarningsLogic.CheckEmpty (warnings, accessor, baseType, obj, field);
					}
				}

				first = false;
			}
		}

		private static void CheckEmpty(List<Warning> warnings, DataAccessor accessor, BaseType baseType, DataObject obj, ObjectField field)
		{
			var e = obj.GetInputEvent ();
			var p = e.GetProperty (field);
			WarningsLogic.CheckEmpty (warnings, accessor, baseType, obj, Guid.Empty, field, p);
		}

		private static void CheckEmpty(List<Warning> warnings, DataAccessor accessor, DataObject asset, DataEvent e, params ObjectField[] fields)
		{
			//	Vérifie les champs de l'événement d'un objet d'immobilisation.
			foreach (var field in fields)
			{
				var p = ObjectProperties.GetObjectProperty (asset, e.Timestamp, field, synthetic: true);
				WarningsLogic.CheckEmpty (warnings, accessor, BaseType.Assets, asset, e.Guid, field, p);
			}
		}

		private static void CheckEmpty(List<Warning> warnings, DataAccessor accessor, BaseType baseType, DataObject obj, Guid eventGuid, ObjectField field, AbstractDataProperty p)
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
					var a = pp.Value.FinalAmount;
					hasWarning = !a.HasValue;
				}
			}

			if (hasWarning)
			{
				string desc = Res.Strings.WarningsLogic.UndefinedField.ToString ();
				var warning = new Warning (baseType, obj.Guid, eventGuid, field, desc);
				warnings.Add (warning);
			}
		}


		#region Amortizations logic
		private static void CheckAmortizations(List<Warning> warnings, DataAccessor accessor)
		{
			//	Vérifie si tous les objets sont amortis. On cherche d'abord la date de
			//	l'amortissement le plus récent, dans l'ensemble des objets. Ensuite, tous
			//	les objets qui ne sont pas amortis jusqu'à cette date ont un avertissement,
			//	sauf s'ils sont sortis ou s'il ne doivent pas être amorti (taux nul).
			//	Suggestion de Stéphane Schmelzer
			var last = WarningsLogic.GetLastAmortization (accessor);

			if (last.HasValue)
			{
				var message = string.Format (Res.Strings.WarningsLogic.Amortization.Missing.ToString (), TypeConverters.DateToString (last.Value.Date));

				foreach (var asset in accessor.Mandat.GetData (BaseType.Assets))
				{
					if (!AssetCalculator.IsOutOfBoundsEvent (asset, last.Value))
					{
						var exp = ObjectProperties.GetObjectPropertyString (asset, last.Value, ObjectField.Expression);
						
						if (!string.IsNullOrEmpty (exp))
						{
							var t = WarningsLogic.GetLastAmortization (asset);

							if (!t.HasValue || t < last)
							{
								var warning = new Warning (BaseType.Assets, asset.Guid, Guid.Empty, ObjectField.Unknown, message);
								warnings.Add (warning);
							}
						}
					}
				}
			}
		}

		private static Timestamp? GetLastAmortization(DataAccessor accessor)
		{
			//	Retourne la date de l'armortissement le plus récent, parmi tous les objets.
			Timestamp? last = null;

			foreach (var asset in accessor.Mandat.GetData (BaseType.Assets))
			{
				var t = WarningsLogic.GetLastAmortization (asset);

				if (t.HasValue)
				{
					if (last.HasValue)
					{
						if (last.Value < t.Value)
						{
							last = t;
						}
					}
					else
					{
						last = t;
					}
				}
			}

			return last;
		}

		private static Timestamp? GetLastAmortization(DataObject asset)
		{
			//	Retourne la date de l'armortissement le plus récent d'un objet.
			for (var i=asset.Events.Count-1; i>=0; i--)
			{
				var e = asset.Events[i];

				if (e.Type == EventType.AmortizationAuto  ||
					e.Type == EventType.AmortizationExtra ||
					e.Type == EventType.AmortizationSuppl )
				{
					return e.Timestamp;
				}
			}

			return null;
		}
		#endregion
	}
}
