//	Copyright � 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		public static string Import(Comptabilit�Entity comptabilit�, string filename)
		{
			try
			{
				var lines = System.IO.File.ReadAllLines (filename, System.Text.Encoding.Default);
				return ImportExport.Import (comptabilit�, lines);
			}
			catch (System.Exception ex)
			{
				return ex.Message;
			}
		}

		private static string Import(Comptabilit�Entity comptabilit�, string[] lines)
		{
			comptabilit�.Journal.Clear ();
			comptabilit�.PlanComptable.Clear ();

			{
				int i = ImportExport.IndexOfLine (lines, "TITLE=");
				if (i != -1)
				{
					comptabilit�.Name = lines[i].Substring (6);
				}
			}

			{
				int i = ImportExport.IndexOfLine (lines, "DATEBEG=");
				if (i != -1)
				{
					comptabilit�.BeginDate = ImportExport.GetDate (lines[i].Substring (8));
				}
			}

			{
				int i = ImportExport.IndexOfLine (lines, "DATEEND=");
				if (i != -1)
				{
					comptabilit�.EndDate = ImportExport.GetDate (lines[i].Substring (8));
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
					var num�ro = ImportExport.GetEntryContentText (lines, indexCompte, "NUM");
					var titre  = ImportExport.GetEntryContentText (lines, indexCompte, "NAME");

					if (string.IsNullOrEmpty (num�ro) || num�ro.Contains ("/") || string.IsNullOrEmpty (titre))
					{
						continue;
					}

					var compte = new Comptabilit�CompteEntity ();

					compte.Num�ro    = num�ro;
					compte.Titre     = titre;
					compte.Cat�gorie = ImportExport.GetEntryContentCat�gorie (lines, indexCompte, "CAT");
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
						groups.Add (num�ro, group);
					}

					var boucle = ImportExport.GetEntryContentText (lines, indexCompte, "BOUCLE");
					if (!string.IsNullOrEmpty (boucle))
					{
						boucles.Add (num�ro, boucle);
					}

					comptabilit�.PlanComptable.Add (compte);
				}
			}

			//	Met apr�s-coup les champs de type pointeur.
			foreach (var item in groups)
			{
				var c1 = comptabilit�.PlanComptable.Where (x => x.Num�ro == item.Key).FirstOrDefault ();
				var c2 = comptabilit�.PlanComptable.Where (x => x.Num�ro == item.Value).FirstOrDefault ();

				if (c1 != null && c2 != null)
				{
					c1.Groupe = c2;
				}
			}

			foreach (var item in boucles)
			{
				var c1 = comptabilit�.PlanComptable.Where (x => x.Num�ro == item.Key).FirstOrDefault ();
				var c2 = comptabilit�.PlanComptable.Where (x => x.Num�ro == item.Value).FirstOrDefault ();

				if (c1 != null && c2 != null)
				{
					c1.CompteOuvBoucl = c2;
				}
			}

			comptabilit�.UpdateNiveauCompte ();

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

		private static Cat�gorieDeCompte GetEntryContentCat�gorie(string[] lines, int index, string key)
		{
			var value = ImportExport.GetEntryContentInt (lines, index, key);

			if (value.HasValue)
			{
				switch (value.Value)
				{
					case 0x02:
						return Cat�gorieDeCompte.Actif;
					case 0x04:
						return Cat�gorieDeCompte.Passif;
					case 0x08:
						return Cat�gorieDeCompte.Charge;
					case 0x10:
						return Cat�gorieDeCompte.Produit;
					case 0x20:
						return Cat�gorieDeCompte.Exploitation;
				}
			}

			return Cat�gorieDeCompte.Inconnu;
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

				if (lines[index].StartsWith ("ENTRY"))  // est-on sur l'entr�e suivante ?
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
