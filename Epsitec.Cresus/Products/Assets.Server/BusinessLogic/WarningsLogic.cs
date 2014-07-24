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
				return accessor.Mandat.GlobalSettings.GetUserFields (baseType)
					.Where (x => x.Field == field && x.Required)
					.Any ();
			}
			else if (baseType == BaseType.Categories)
			{
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
				return field == ObjectField.CategoryName
					|| field == ObjectField.AmortizationRate
					|| field == ObjectField.AmortizationType
					|| field == ObjectField.Periodicity
					|| field == ObjectField.Prorata
					|| field == ObjectField.Round
					|| field == ObjectField.ResidualValue;
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

			//	On cherche les champs indéfinis dans les catégories.
			WarningsLogic.CheckEmpty (warnings, accessor, BaseType.Categories,
				ObjectField.Name,
				ObjectField.AmortizationRate,
				ObjectField.AmortizationType,
				ObjectField.Periodicity,
				ObjectField.Prorata,
				ObjectField.Round,
				ObjectField.ResidualValue);

			//	On cherche les champs indéfinis dans les groupes.
			WarningsLogic.CheckEmpty (warnings, accessor, BaseType.Groups,
				ObjectField.Name,
				ObjectField.Number);

			//	On s'occupe des objets d'immobilisation.
			foreach (var asset in accessor.Mandat.GetData (BaseType.Assets))
			{
				foreach (var e in asset.Events)
				{
					//	On cherche les champs définis par l'utilisateur indéfinis.
					var requiredFields = accessor.Mandat.GlobalSettings.GetUserFields (BaseType.Assets)
						.Where (x => x.Required)
						.Select (x => x.Field)
						.ToArray ();
					WarningsLogic.CheckEmpty (warnings, BaseType.Assets, asset, e, requiredFields);

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
					WarningsLogic.CheckAccount (warnings, accessor, asset, e, ObjectField.Account1);
					WarningsLogic.CheckAccount (warnings, accessor, asset, e, ObjectField.Account2);
					WarningsLogic.CheckAccount (warnings, accessor, asset, e, ObjectField.Account3);
					WarningsLogic.CheckAccount (warnings, accessor, asset, e, ObjectField.Account4);
					WarningsLogic.CheckAccount (warnings, accessor, asset, e, ObjectField.Account5);
					WarningsLogic.CheckAccount (warnings, accessor, asset, e, ObjectField.Account6);
				}
			}

			WarningsLogic.CheckRequired (warnings, accessor, BaseType.Persons);
		}


		private static void CheckAccount(List<Warning> warnings, DataAccessor accessor, DataObject asset, DataEvent e, ObjectField accountField)
		{
			var p = ObjectProperties.GetObjectProperty (asset, e.Timestamp, accountField, synthetic: true) as DataStringProperty;

			if (p == null || string.IsNullOrEmpty (p.Value))
			{
				//-const string desc = "Le compte n'est pas défini";
				//-var warning = new Warning (BaseType.Assets, asset.Guid, e.Guid, accountField, desc);
				//-warnings.Add (warning);
			}
			else
			{
				var number = p.Value;  // numéro du compte (ex. "1000")

				var baseType = accessor.Mandat.GetAccountsBase (e.Timestamp.Date);
				var account = AccountsLogic.GetAccount (accessor, baseType, number);

				if (account == null)
				{
					var desc = string.Format ("Le compte {0} n'est pas défini à cette date", number);
					var warning = new Warning (BaseType.Assets, asset.Guid, e.Guid, accountField, desc);
					warnings.Add (warning);
				}
			}
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
			}

			if (hasWarning)
			{
				const string desc = "Le champ n'est pas défini";
				var warning = new Warning (baseType, obj.Guid, eventGuid, field, desc);
				warnings.Add (warning);
			}
		}
	}
}
