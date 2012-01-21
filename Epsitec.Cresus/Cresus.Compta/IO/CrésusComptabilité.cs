//	Copyright � 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.IO
{
	/// <summary>
	/// Cette classe s'occupe des import/export avec l'ancien logiciel Cr�sus Comptabilit�.
	/// </summary>
	public static class Cr�susComptabilit�
	{
		public static string ImportPlanComptable(Comptabilit�Entity comptabilit�, string filename)
		{
			//	Importe un plan comptable "crp".
			string ext = System.IO.Path.GetExtension (filename).ToLower ();
			if (ext != ".crp")
			{
				return "Le fichier ne contient pas un plan comptable.";
			}

			try
			{
				var lines = System.IO.File.ReadAllLines (filename, System.Text.Encoding.Default);

				try
				{
					return Cr�susComptabilit�.ImportPlanComptable (comptabilit�, lines);
				}
				catch (System.Exception ex)
				{
					return string.Concat ("Le fichier ne contient pas un plan comptable.<br/>", ex.Message);
				}
			}
			catch (System.Exception ex)
			{
				return ex.Message;
			}
		}

		private static string ImportPlanComptable(Comptabilit�Entity comptabilit�, string[] lines)
		{
			comptabilit�.Journal.Clear ();
			comptabilit�.PlanComptable.Clear ();

			//	Importe les donn�es globales.
			{
				int i = Cr�susComptabilit�.IndexOfLine (lines, "TITLE=");
				if (i != -1)
				{
					comptabilit�.Name = lines[i].Substring (6);
				}
			}

			{
				int i = Cr�susComptabilit�.IndexOfLine (lines, "DATEBEG=");
				if (i != -1)
				{
					comptabilit�.BeginDate = Cr�susComptabilit�.GetDate (lines[i].Substring (8));
				}
			}

			{
				int i = Cr�susComptabilit�.IndexOfLine (lines, "DATEEND=");
				if (i != -1)
				{
					comptabilit�.EndDate = Cr�susComptabilit�.GetDate (lines[i].Substring (8));
				}
			}

			//	Importe tous les comptes.
			int indexCompte = Cr�susComptabilit�.IndexOfLine (lines, "BEGIN=COMPTES");

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
					var num�ro = Cr�susComptabilit�.GetEntryContentText (lines, indexCompte, "NUM");
					var titre  = Cr�susComptabilit�.GetEntryContentText (lines, indexCompte, "NAME");

					if (string.IsNullOrEmpty (num�ro) || num�ro.Contains ("/") || string.IsNullOrEmpty (titre))
					{
						continue;
					}

					var compte = new Comptabilit�CompteEntity ();

					compte.Num�ro    = num�ro;
					compte.Titre     = titre;
					compte.Cat�gorie = Cr�susComptabilit�.GetEntryContentCat�gorie (lines, indexCompte, "CAT");
					compte.Type      = Cr�susComptabilit�.GetEntryContentType      (lines, indexCompte, "STATUS");
					//compte.Monnaie   = Cr�susComptabilit�.GetEntryContentText      (lines, indexCompte, "CURRENCY");

					var niveau = Cr�susComptabilit�.GetEntryContentInt (lines, indexCompte, "LEVEL");
					if (niveau.HasValue)
					{
						compte.Niveau = niveau.Value;
					}

					var ordre = Cr�susComptabilit�.GetEntryContentInt (lines, indexCompte, "ORDER");
					if (ordre.HasValue)
					{
						compte.IndexOuvBoucl = ordre.Value;
					}

					var group = Cr�susComptabilit�.GetEntryContentText (lines, indexCompte, "GROUP");
					if (!string.IsNullOrEmpty (group))
					{
						groups.Add (num�ro, group);
					}

					var boucle = Cr�susComptabilit�.GetEntryContentText (lines, indexCompte, "BOUCLE");
					if (!string.IsNullOrEmpty (boucle))
					{
						boucles.Add (num�ro, boucle);
					}

					comptabilit�.PlanComptable.Add (compte);
				}
			}

			//	Met apr�s-coup les champs qui pointent sur des comptes.
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
			var value = Cr�susComptabilit�.GetEntryContentInt (lines, index, key);

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
			var value = Cr�susComptabilit�.GetEntryContentInt (lines, index, key);

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
			var text = Cr�susComptabilit�.GetEntryContentText (lines, index, key);

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

				if (lines[index].StartsWith ("ENTRY") ||  // est-on sur l'entr�e suivante ?
					lines[index].StartsWith ("END="))     // fin du bloc ?
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
