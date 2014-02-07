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
				return ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Nom);
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
				var t1 = ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Prénom);
				var t2 = ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Nom);
				var t3 = ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Entreprise);

				return string.Join (" ", t1, t2, t3).Trim ();
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

				var titre = ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Titre);
				PersonsLogic.PutLine (lines, titre);

				var prénom = ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Prénom);
				var nom    = ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Nom);
				PersonsLogic.PutLine (lines, prénom, nom);

				var entreprise = ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Entreprise);
				PersonsLogic.PutLine (lines, entreprise);

				var adresse = ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Adresse);
				PersonsLogic.PutLine (lines, adresse);

				var npa   = ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Npa);
				var ville = ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Ville);
				PersonsLogic.PutLine (lines, npa, ville);

				var pays = ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Pays);
				PersonsLogic.PutLine (lines, pays);

				var tel1 = ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Téléphone1);
				PersonsLogic.PutLine (lines, tel1);

				var tel2 = ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Téléphone2);
				PersonsLogic.PutLine (lines, tel2);

				var tel3 = ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Téléphone3);
				PersonsLogic.PutLine (lines, tel3);

				var mail = ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Mail);
				PersonsLogic.PutLine (lines, mail);

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
