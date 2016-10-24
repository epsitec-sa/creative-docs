//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class CentersLogic
	{
		public static string GetExplanation(DataAccessor accessor, System.DateTime date, string code, out bool hasError)
		{
			//	Retourne le texte explicatif d'un centre de charge. Exemples:
			//	"TVARED 3.6%"
			//	"XX — Inconnu dans le plan comptable"
			string explanationsValue;

			if (string.IsNullOrEmpty (code))  // aucun code ?
			{
				explanationsValue = null;
				hasError = false;
			}
			else  // code présent ?
			{
				//	Cherche le plan comptable correspondant à la date.
				var baseType = accessor.Mandat.GetCentersBase (date);

				if (baseType.AccountsDateRange.IsEmpty)  // pas de plan comptable ?
				{
					explanationsValue = CentersLogic.AddError (code, Res.Strings.CentersLogic.InvalidDate.ToString ());
					hasError = true;
				}
				else  // plan comptable trouvé ?
				{
					//	Cherche le résumé du code.
					var summary = CentersLogic.GetSummary (accessor, baseType, code);

					if (string.IsNullOrEmpty (summary))  // code inexistant ?
					{
						explanationsValue = CentersLogic.AddError (code, Res.Strings.CentersLogic.CodeDoesNotExist.ToString ());
						hasError = true;
					}
					else
					{
						explanationsValue = summary;  // par exemple "1000 Caisse"
						hasError = false;
					}
				}
			}

			return explanationsValue;
		}

		private static string AddError(string text, string error)
		{
			//	Retourne un texte explicatif composé du numéro du code et de l'erreur.
			if (string.IsNullOrEmpty (text))
			{
				return null;
			}
			else
			{
				if (string.IsNullOrEmpty (error))
				{
					return text;
				}
				else
				{
					return UniversalLogic.NiceJoin (text, error);
				}
			}
		}


		public static string GetSummary(DataAccessor accessor, BaseType baseType, string code)
		{
			//	Retourne le résumé (par exemple "TVARED 3.6%") d'après le seul code.
			if (!string.IsNullOrEmpty (code))
			{
				var obj = CentersLogic.GetCenter (accessor, baseType, code);

				if (obj != null)
				{
					var rate = ObjectProperties.GetObjectPropertyDecimal (obj, null, ObjectField.VatRate);

					if (rate.HasValue)
					{
						var r = TypeConverters.RateToString (rate);
						return string.Join (" ", code, r);
					}
				}
			}

			return code;
		}

		public static DataObject GetCenter(DataAccessor accessor, BaseType baseType, string code)
		{
			if (baseType != BaseType.Unknown)
			{
				var data = accessor.Mandat.GetData (baseType);

				foreach (var obj in data)
				{
					var n = ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Name);
					if (n == code)
					{
						return obj;
					}
				}
			}

			return null;
		}

		public static string GetCenter(DataAccessor accessor, BaseType baseType, Guid guid)
		{
			//	Retourne le nom d'un centre de charge.
			var obj = accessor.GetObject (baseType, guid);
			if (obj == null)
			{
				return null;
			}
			else
			{
				return ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Name);
			}
		}
	}
}
