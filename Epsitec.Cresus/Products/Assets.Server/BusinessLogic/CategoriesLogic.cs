//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class CategoriesLogic
	{
		public static bool HasAccounts(DataAccessor accessor)
		{
			//	Indique s'il existe une catégorie d'immobilisation qui fait référence à un compte.
			foreach (var cat in accessor.Mandat.GetData (BaseType.Categories))
			{
				if (CategoriesLogic.HasAccounts (cat))
				{
					return true;
				}
			}

			return false;
		}

		private static bool HasAccounts(DataObject cat)
		{
			return CategoriesLogic.HasAccounts (cat, ObjectField.Account1)
				|| CategoriesLogic.HasAccounts (cat, ObjectField.Account2)
				|| CategoriesLogic.HasAccounts (cat, ObjectField.Account3)
				|| CategoriesLogic.HasAccounts (cat, ObjectField.Account4)
				|| CategoriesLogic.HasAccounts (cat, ObjectField.Account5)
				|| CategoriesLogic.HasAccounts (cat, ObjectField.Account6)
				|| CategoriesLogic.HasAccounts (cat, ObjectField.Account7)
				|| CategoriesLogic.HasAccounts (cat, ObjectField.Account8);
		}

		private static bool HasAccounts(DataObject cat, ObjectField field)
		{
			var account = ObjectProperties.GetObjectPropertyString (cat, null, field);
			return !string.IsNullOrEmpty (account);
		}


		public static void ClearAccounts(DataAccessor accessor)
		{
			//	Efface toutes les références aux comptes dans toutes les catégories d'immobilisation.
			foreach (var cat in accessor.Mandat.GetData (BaseType.Categories))
			{
				CategoriesLogic.ClearAccounts (cat);
			}
		}

		private static void ClearAccounts(DataObject cat)
		{
			var e = cat.GetEvent (0);

			e.RemoveProperty (ObjectField.Account1);
			e.RemoveProperty (ObjectField.Account2);
			e.RemoveProperty (ObjectField.Account3);
			e.RemoveProperty (ObjectField.Account4);
			e.RemoveProperty (ObjectField.Account5);
			e.RemoveProperty (ObjectField.Account6);
			e.RemoveProperty (ObjectField.Account7);
			e.RemoveProperty (ObjectField.Account8);
		}


		public static string GetSummary(DataAccessor accessor, Guid guid)
		{
			//	Retourne le nom court d'une catégorie, du genre:
			//	"Véhicules"
			var obj = accessor.GetObject (BaseType.Categories, guid);
			if (obj == null)
			{
				return null;
			}

			return ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Name);
		}


		public static string GetFullName(DataAccessor accessor, Guid guid)
		{
			//	Retourne le nom complet d'une catégorie, du genre:
			//	"Véhicules 12.5% Linéaire Annuel"
			var obj = accessor.GetObject (BaseType.Categories, guid);
			if (obj == null)
			{
				return null;
			}

			var name   = ObjectProperties.GetObjectPropertyString  (obj, null, ObjectField.Name);
			var taux   = ObjectProperties.GetObjectPropertyDecimal (obj, null, ObjectField.AmortizationRate);
			var type   = ObjectProperties.GetObjectPropertyInt     (obj, null, ObjectField.AmortizationType);
			var period = ObjectProperties.GetObjectPropertyInt     (obj, null, ObjectField.Periodicity);

			var stringTaux   = TypeConverters.RateToString (taux);
			var stringType   = EnumDictionaries.GetAmortizationTypeName (type);
			var stringPeriod = EnumDictionaries.GetPeriodicityName (period);

			return string.Format ("{0} {1} {2} {3}", name, stringTaux, stringType, stringPeriod);
		}
	}
}
