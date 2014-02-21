//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class PersonsLogic
	{
		public static string GetShortName(DataAccessor accessor, Guid guid)
		{
			//	Retourne le nom court d'une personne, du genre:
			//	"Dupond"
			var obj = accessor.GetObject (BaseType.Persons, guid);
			if (obj == null)
			{
				return null;
			}
			else
			{
				return AssetCalculator.GetObjectPropertyString (obj, null, ObjectField.Name);
			}
		}


		public static string GetFullName(DataAccessor accessor, Guid guid)
		{
			//	Retourne le nom complet d'une personne, du genre:
			//	"Jean Dupond Epsitec SA"
			var obj = accessor.GetObject (BaseType.Persons, guid);
			if (obj == null)
			{
				return null;
			}
			else
			{
#if false
				var t1 = AssetCalculator.GetObjectPropertyString (obj, null, ObjectField.FirstName);
				var t2 = AssetCalculator.GetObjectPropertyString (obj, null, ObjectField.Name);
				var t3 = AssetCalculator.GetObjectPropertyString (obj, null, ObjectField.Company);

				return string.Join (" ", t1, t2, t3).Trim ();
#else
				//	On prend les 4 premiers champs de type texte, habituellement Nom,
				//	Prénom, Titre et Entreprise.
				var list = new List<string> ();
				int count = 0;

				foreach (var userField in accessor.Settings.GetUserFields (BaseType.Persons)
					.Where (x => x.Type == FieldType.String))
				{
					var text = AssetCalculator.GetObjectPropertyString (obj, null, userField.Field);

					if (!string.IsNullOrEmpty (text))
					{
						list.Add (text);
					}

					count++;

					if (count >= 4)
					{
						break;
					}
				}

				return string.Join (" ", list).Trim ();
#endif
			}
		}


		public static string GetSummary(DataAccessor accessor, Guid guid)
		{
			//	Retourne le résumé multi-lignes d'une personne, du genre:
			//	Monsieur
			//	Jean Dupond
			//	Epsitec SA
			//	Rue de Neuchâtel 32
			//	1400 Yverdon-les-Bains
			//	Suisse
			var obj = accessor.GetObject (BaseType.Persons, guid);
			if (obj == null)
			{
				return null;
			}
			else
			{
				var lines = new List<string> ();

#if false
				var titre = AssetCalculator.GetObjectPropertyString (obj, null, ObjectField.Title);
				PersonsLogic.PutLine (lines, titre);

				var prénom = AssetCalculator.GetObjectPropertyString (obj, null, ObjectField.FirstName);
				var nom    = AssetCalculator.GetObjectPropertyString (obj, null, ObjectField.Name);
				PersonsLogic.PutLine (lines, prénom, nom);

				var entreprise = AssetCalculator.GetObjectPropertyString (obj, null, ObjectField.Company);
				PersonsLogic.PutLine (lines, entreprise);

				var adresse = AssetCalculator.GetObjectPropertyString (obj, null, ObjectField.Address);
				PersonsLogic.PutLine (lines, adresse);

				var npa   = AssetCalculator.GetObjectPropertyString (obj, null, ObjectField.Zip);
				var ville = AssetCalculator.GetObjectPropertyString (obj, null, ObjectField.City);
				PersonsLogic.PutLine (lines, npa, ville);

				var pays = AssetCalculator.GetObjectPropertyString (obj, null, ObjectField.Country);
				PersonsLogic.PutLine (lines, pays);

				var tel1 = AssetCalculator.GetObjectPropertyString (obj, null, ObjectField.Phone1);
				PersonsLogic.PutLine (lines, tel1);

				var tel2 = AssetCalculator.GetObjectPropertyString (obj, null, ObjectField.Phone2);
				PersonsLogic.PutLine (lines, tel2);

				var tel3 = AssetCalculator.GetObjectPropertyString (obj, null, ObjectField.Phone3);
				PersonsLogic.PutLine (lines, tel3);

				var mail = AssetCalculator.GetObjectPropertyString (obj, null, ObjectField.Mail);
				PersonsLogic.PutLine (lines, mail);
#else
				foreach (var userField in accessor.Settings.GetUserFields (BaseType.Persons))
				{
					var text = AssetCalculator.GetObjectPropertyString (obj, null, userField.Field);
					PersonsLogic.PutLine (lines, text);
				}
#endif

				return string.Join ("<br/>", lines);
			}
		}

		private static void PutLine(List<string> lines, params string[] words)
		{
			var line = string.Join (" ", words
				.Where (x => !string.IsNullOrEmpty (x))
				.Select (x => x.Replace ("<br/>", ", "))).Trim ();

			if (!string.IsNullOrEmpty (line))
			{
				lines.Add (line);
			}
		}
	}
}
