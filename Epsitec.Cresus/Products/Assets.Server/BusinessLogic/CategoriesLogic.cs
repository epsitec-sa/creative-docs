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

			return AssetCalculator.GetObjectPropertyString (obj, null, ObjectField.Name);
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

			var name   = AssetCalculator.GetObjectPropertyString  (obj, null, ObjectField.Name);
			var taux   = AssetCalculator.GetObjectPropertyDecimal (obj, null, ObjectField.AmortizationRate);
			var type   = AssetCalculator.GetObjectPropertyInt     (obj, null, ObjectField.AmortizationType);
			var period = AssetCalculator.GetObjectPropertyInt     (obj, null, ObjectField.Periodicity);

			var stringTaux   = TypeConverters.RateToString (taux);
			var stringType   = EnumDictionaries.GetAmortizationTypeName (type);
			var stringPeriod = EnumDictionaries.GetPeriodicityName (period);

			return string.Format ("{0} {1} {2} {3}", name, stringTaux, stringType, stringPeriod);
		}
	}
}
