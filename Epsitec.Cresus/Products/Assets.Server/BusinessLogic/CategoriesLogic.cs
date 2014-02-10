//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.Helpers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class CategoriesLogic
	{
		public static string GetShortName(DataAccessor accessor, Guid guid)
		{
			//	Retourne le nom court d'une catégorie, du genre:
			//	"Véhicules"
			var obj = accessor.GetObject (BaseType.Categories, guid);
			if (obj == null)
			{
				return null;
			}

			return ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Name);
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

			var name   = ObjectCalculator.GetObjectPropertyString  (obj, null, ObjectField.Name);
			var taux   = ObjectCalculator.GetObjectPropertyDecimal (obj, null, ObjectField.AmortizationRate);
			var type   = ObjectCalculator.GetObjectPropertyInt     (obj, null, ObjectField.AmortizationType);
			var period = ObjectCalculator.GetObjectPropertyInt     (obj, null, ObjectField.Periodicity);

			var stringTaux   = TypeConverters.RateToString (taux);
			var stringType   = CategoriesLogic.GetTypeAmortissement (type);
			var stringPeriod = CategoriesLogic.GetPériodicité (period);

			return string.Format ("{0} {1} {2} {3}", name, stringTaux, stringType, stringPeriod);
		}

		private static string GetTypeAmortissement(int? type)
		{
			if (type.HasValue)
			{
				return EnumDictionaries.GetTypeAmortissementName ((AmortizationType) type.Value);
			}

			return null;
		}

		private static string GetPériodicité(int? type)
		{
			if (type.HasValue)
			{
				return EnumDictionaries.GetPériodicitéName ((Periodicity) type.Value);
			}

			return null;
		}
	}
}
