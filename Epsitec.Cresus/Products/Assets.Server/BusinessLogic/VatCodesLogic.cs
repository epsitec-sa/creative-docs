//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class VatCodesLogic
	{
		public static string GetExplanation(DataAccessor accessor, System.DateTime date, string code,
			out bool hasError, out bool gotoVisible)
		{
			//	Retourne le texte explicatif d'un code TVA. Exemples:
			//	"TVARED 3.6% Prestations du secteur d'hébergement"
			//	"XX — Inconnu dans le plan comptable"
			string explanationsValue;

			if (string.IsNullOrEmpty (code))  // aucun code ?
			{
				explanationsValue = null;
				hasError = false;
				gotoVisible = false;
			}
			else  // code présent ?
			{
				//	Cherche le plan comptable correspondant à la date.
				var baseType = accessor.Mandat.GetVatCodesBase (date);

				if (baseType.AccountsDateRange.IsEmpty)  // pas de plan comptable ?
				{
					explanationsValue = VatCodesLogic.AddError (code, Res.Strings.VatCodesLogic.InvalidDate.ToString ());
					hasError = true;
					gotoVisible = false;
				}
				else  // plan comptable trouvé ?
				{
					//	Cherche le résumé du code.
					var summary = VatCodesLogic.GetSummary (accessor, baseType, code);

					if (string.IsNullOrEmpty (summary))  // code inexistant ?
					{
						explanationsValue = VatCodesLogic.AddError (code, Res.Strings.VatCodesLogic.CodeDoesNotExist.ToString ());
						hasError = true;
						gotoVisible = false;
					}
					else
					{
						explanationsValue = summary;  // par exemple "1000 Caisse"
						hasError = false;
						gotoVisible = true;
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
			//	Retourne le résumé (par exemple "TVARED 3.6% Prestations du secteur d'hébergement") d'après le seul code.
			if (!string.IsNullOrEmpty (code))
			{
				var obj = VatCodesLogic.GetVatCode (accessor, baseType, code);

				if (obj != null)
				{
					var rate = ObjectProperties.GetObjectPropertyDecimal (obj, null, ObjectField.VatRate);
					var desc = ObjectProperties.GetObjectPropertyString  (obj, null, ObjectField.Description);

					if (rate.HasValue && !string.IsNullOrEmpty (desc))
					{
						var r = TypeConverters.RateToString (rate);
						return string.Join (" ", code, r, desc);
					}
				}
			}

			return code;
		}

		public static DataObject GetVatCode(DataAccessor accessor, BaseType baseType, string code)
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
	}
}
