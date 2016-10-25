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
		public static string GetExplanation(DataAccessor accessor, System.DateTime date, string number, out bool hasError)
		{
			//	Retourne le texte explicatif d'un centre de charge. Exemples:
			//	"P1000 — Projet 1000"
			//	"XX — Inconnu dans le plan comptable"
			string explanationsValue;

			if (string.IsNullOrEmpty (number))  // aucun number ?
			{
				explanationsValue = null;
				hasError = false;
			}
			else  // number présent ?
			{
				//	Cherche le plan comptable correspondant à la date.
				var baseType = accessor.Mandat.GetCentersBase (date);

				if (baseType.AccountsDateRange.IsEmpty)  // pas de plan comptable ?
				{
					explanationsValue = CentersLogic.AddError (number, Res.Strings.CentersLogic.InvalidDate.ToString ());
					hasError = true;
				}
				else  // plan comptable trouvé ?
				{
					//	Cherche le résumé du centre de charge.
					var summary = CentersLogic.GetSummary (accessor, baseType, number);

					if (string.IsNullOrEmpty (summary))  // number inexistant ?
					{
						explanationsValue = CentersLogic.AddError (number, Res.Strings.CentersLogic.CodeDoesNotExist.ToString ());
						hasError = true;
					}
					else
					{
						explanationsValue = summary;  // par exemple "P1000 — Projet 1000"
						hasError = false;
					}
				}
			}

			return explanationsValue;
		}

		private static string AddError(string text, string error)
		{
			//	Retourne un texte explicatif composé du numéro du number et de l'erreur.
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


		private static string GetSummary(DataAccessor accessor, BaseType baseType, string number)
		{
			//	Retourne le résumé (par exemple "P1000 — Projet 1000") d'après le seul number.
			if (!string.IsNullOrEmpty (number))
			{
				var obj = CentersLogic.GetCenter (accessor, baseType, number);

				if (obj != null)
				{
					var name = ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Name);

					if (!string.IsNullOrEmpty (name))
					{
						return string.Join (number, " — ", name);
					}
				}
			}

			return number;
		}

		private static DataObject GetCenter(DataAccessor accessor, BaseType baseType, string number)
		{
			if (baseType != BaseType.Unknown)
			{
				var data = accessor.Mandat.GetData (baseType);

				foreach (var obj in data)
				{
					var n = ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Number);
					if (n == number)
					{
						return obj;
					}
				}
			}

			return null;
		}

		public static string GetCenter(DataAccessor accessor, BaseType baseType, Guid guid)
		{
			//	Retourne le number d'un centre de charge.
			var obj = accessor.GetObject (baseType, guid);
			if (obj == null)
			{
				return null;
			}
			else
			{
				return ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Number);
			}
		}
	}
}
