//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta
{
	public static class ImportExport
	{
		public static string Import(ComptabilitéEntity comptabilité, string filename)
		{
			try
			{
				var lines = System.IO.File.ReadAllLines (filename, System.Text.Encoding.Default);
				return ImportExport.Import (comptabilité, lines);
			}
			catch (System.Exception ex)
			{
				return ex.Message;
			}
		}

		private static string Import(ComptabilitéEntity comptabilité, string[] lines)
		{
			comptabilité.Journal.Clear ();
			comptabilité.PlanComptable.Clear ();

			{
				int i = ImportExport.IndexOfLine (lines, "TITLE=");
				if (i != -1)
				{
					comptabilité.Name = lines[i].Substring (6);
				}
			}

			{
				int i = ImportExport.IndexOfLine (lines, "DATEBEG=");
				if (i != -1)
				{
					comptabilité.BeginDate = ImportExport.GetDate (lines[i].Substring (8));
				}
			}

			{
				int i = ImportExport.IndexOfLine (lines, "DATEEND=");
				if (i != -1)
				{
					comptabilité.EndDate = ImportExport.GetDate (lines[i].Substring (8));
				}
			}

			//	Importe tous les comptes
			int indexCompte = ImportExport.IndexOfLine (lines, "BEGIN=COMPTES");

			var groups  = new Dictionary<string, string> ();
			var boucles = new Dictionary<string, string> ();

			while (++indexCompte < lines.Length)
			{
				var line = lines[indexCompte];

				if (string.IsNullOrEmpty (line))
				{
					continue;
				}

				if (line.StartsWith ("END=COMPTES"))
				{
					break;
				}

				if (line.StartsWith ("ENTRY"))
				{
					var numéro = ImportExport.GetEntryContentText (lines, indexCompte, "NUM");
					var titre  = ImportExport.GetEntryContentText (lines, indexCompte, "NAME");

					if (string.IsNullOrEmpty (numéro) || numéro.Contains ("/") || string.IsNullOrEmpty (titre))
					{
						continue;
					}

					var compte = new ComptabilitéCompteEntity ();

					compte.Numéro    = numéro;
					compte.Titre     = titre;
					compte.Catégorie = ImportExport.GetEntryContentCatégorie (lines, indexCompte, "CAT");
					compte.Type      = ImportExport.GetEntryContentType      (lines, indexCompte, "STATUS");
					//compte.Monnaie   = ImportExport.GetEntryContentText      (lines, indexCompte, "CURRENCY");

					var niveau = ImportExport.GetEntryContentInt (lines, indexCompte, "LEVEL");
					if (niveau.HasValue)
					{
						compte.Niveau = niveau.Value;
					}

					var ordre = ImportExport.GetEntryContentInt (lines, indexCompte, "ORDER");
					if (ordre.HasValue)
					{
						compte.IndexOuvBoucl = ordre.Value;
					}

					var group = ImportExport.GetEntryContentText (lines, indexCompte, "GROUP");
					if (!string.IsNullOrEmpty (group))
					{
						groups.Add (numéro, group);
					}

					var boucle = ImportExport.GetEntryContentText (lines, indexCompte, "BOUCLE");
					if (!string.IsNullOrEmpty (boucle))
					{
						boucles.Add (numéro, boucle);
					}

					comptabilité.PlanComptable.Add (compte);
				}
			}

			//	Met après-coup les champs de type pointeur.
			foreach (var item in groups)
			{
				var c1 = comptabilité.PlanComptable.Where (x => x.Numéro == item.Key).FirstOrDefault ();
				var c2 = comptabilité.PlanComptable.Where (x => x.Numéro == item.Value).FirstOrDefault ();

				if (c1 != null && c2 != null)
				{
					c1.Groupe = c2;
				}
			}

			foreach (var item in boucles)
			{
				var c1 = comptabilité.PlanComptable.Where (x => x.Numéro == item.Key).FirstOrDefault ();
				var c2 = comptabilité.PlanComptable.Where (x => x.Numéro == item.Value).FirstOrDefault ();

				if (c1 != null && c2 != null)
				{
					c1.CompteOuvBoucl = c2;
				}
			}

			comptabilité.UpdateNiveauCompte ();

			return null;  // ok
		}

		private static Date? GetDate(string text)
		{
			System.DateTime d;

			if (System.DateTime.TryParse (text, out d))
			{
				return new Date (d);
			}

			return null;
		}

		private static CatégorieDeCompte GetEntryContentCatégorie(string[] lines, int index, string key)
		{
			var value = ImportExport.GetEntryContentInt (lines, index, key);

			if (value.HasValue)
			{
				switch (value.Value)
				{
					case 0x02:
						return CatégorieDeCompte.Actif;
					case 0x04:
						return CatégorieDeCompte.Passif;
					case 0x08:
						return CatégorieDeCompte.Charge;
					case 0x10:
						return CatégorieDeCompte.Produit;
					case 0x20:
						return CatégorieDeCompte.Exploitation;
				}
			}

			return CatégorieDeCompte.Inconnu;
		}

		private static TypeDeCompte GetEntryContentType(string[] lines, int index, string key)
		{
			var value = ImportExport.GetEntryContentInt (lines, index, key);

			if (value.HasValue)
			{
				int v = value.Value & 0x18;

				if (v != 0x18)
				{
					switch (v)
					{
						case 0x00:
							return TypeDeCompte.Normal;
						case 0x08:
							return TypeDeCompte.Titre;
						case 0x10:
							return TypeDeCompte.Groupe;
					}
				}
			}

			return TypeDeCompte.Normal;
		}

		private static int? GetEntryContentInt(string[] lines, int index, string key)
		{
			var text = ImportExport.GetEntryContentText (lines, index, key);

			if (!string.IsNullOrEmpty (text))
			{
				int value;
				if (int.TryParse (text, out value))
				{
					return value;
				}
			}

			return null;
		}

		private static string GetEntryContentText(string[] lines, int index, string key)
		{
			key = key+"=";

			while (++index < lines.Length)
			{
				if (lines[index].StartsWith (key))
				{
					return lines[index].Substring (key.Length).Trim ();
				}

				if (lines[index].StartsWith ("ENTRY"))  // est-on sur l'entrée suivante ?
				{
					break;
				}
			}

			return null;
		}

		private static int IndexOfLine(string[] lines, string key)
		{
			for (int i = 0; i < lines.Length; i++)
			{
				if (lines[i].StartsWith (key))
				{
					return i;
				}
			}

			return -1;
		}
	}
}
