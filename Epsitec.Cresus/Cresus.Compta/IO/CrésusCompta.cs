//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.IO
{
	/// <summary>
	/// Cette classe s'occupe des import/export avec l'ancien logiciel Crésus Comptabilité (DR/MW).
	/// </summary>
	public class CrésusCompta
	{
		public string ImportPlanComptable(ComptaEntity compta, string filename)
		{
			//	Importe un plan comptable "crp".
			this.compta = compta;

			string ext = System.IO.Path.GetExtension (filename).ToLower ();
			if (ext != ".crp")
			{
				return "Le fichier ne contient pas un plan comptable.";
			}

			try
			{
				this.lines = System.IO.File.ReadAllLines (filename, System.Text.Encoding.Default);

				try
				{
					return this.ImportPlanComptable ();
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

		private string ImportPlanComptable()
		{
			new NewCompta ().NewNull (this.compta);

			//	Importe les données globales.
			{
				int i = this.IndexOfLine ("TITLE=");
				if (i != -1)
				{
					this.compta.Name = this.lines[i].Substring (6);
				}
			}

			{
				int i = this.IndexOfLine ("DATEBEG=");
				if (i != -1)
				{
					this.compta.BeginDate = this.GetDate (this.lines[i].Substring (8));
				}
			}

			{
				int i = this.IndexOfLine ("DATEEND=");
				if (i != -1)
				{
					this.compta.EndDate = this.GetDate (this.lines[i].Substring (8));
				}
			}

			//	Importe tous les journaux.
			int indexJournal = this.IndexOfLine ("BEGIN=JOURNAUX");

			var journaux = new Dictionary<int?, string> ();

			while (++indexJournal < this.lines.Length)
			{
				var line = this.lines[indexJournal];

				if (string.IsNullOrEmpty (line))
				{
					continue;
				}

				if (line.StartsWith ("END=JOURNAUX"))
				{
					break;
				}

				if (line.StartsWith ("ENTRY"))
				{
					var rank = this.GetEntryContentInt  (indexJournal, "NUM");
					var name = this.GetEntryContentText (indexJournal, "NAME");
					journaux.Add (rank, name);
				}
			}

			var journauxTriés = journaux.OrderBy (x => x.Key);

			foreach (var j in journauxTriés)
			{
				var journal = new ComptaJournalEntity ();
				journal.Name = j.Value;
				this.compta.Journaux.Add (journal);
			}

			//	Importe tous les comptes.
			int indexCompte = this.IndexOfLine ("BEGIN=COMPTES");

			var groups  = new Dictionary<string, string> ();
			var boucles = new Dictionary<string, string> ();

			while (++indexCompte < this.lines.Length)
			{
				var line = this.lines[indexCompte];

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
					var numéro = this.GetEntryContentText (indexCompte, "NUM");
					var titre  = this.GetEntryContentText (indexCompte, "NAME");

					if (string.IsNullOrEmpty (numéro) || numéro.Contains ("/") || string.IsNullOrEmpty (titre))
					{
						continue;
					}

					var compte = new ComptaCompteEntity ();

					compte.Numéro    = numéro;
					compte.Titre     = titre;
					compte.Catégorie = this.GetEntryContentCatégorie (indexCompte, "CAT");
					compte.Type      = this.GetEntryContentType      (indexCompte, "STATUS");
					//compte.Monnaie   = this.GetEntryContentText      (indexCompte, "CURRENCY");

					var niveau = this.GetEntryContentInt (indexCompte, "LEVEL");
					if (niveau.HasValue)
					{
						compte.Niveau = niveau.Value;
					}

					var ordre = this.GetEntryContentInt (indexCompte, "ORDER");
					if (ordre.HasValue)
					{
						compte.IndexOuvBoucl = ordre.Value;
					}

					var group = this.GetEntryContentText (indexCompte, "GROUP");
					if (!string.IsNullOrEmpty (group))
					{
						groups.Add (numéro, group);
					}

					var boucle = this.GetEntryContentText (indexCompte, "BOUCLE");
					if (!string.IsNullOrEmpty (boucle))
					{
						boucles.Add (numéro, boucle);
					}

					this.compta.PlanComptable.Add (compte);
				}
			}

			//	Met après-coup les champs qui pointent sur des comptes.
			foreach (var item in groups)
			{
				var c1 = this.compta.PlanComptable.Where (x => x.Numéro == item.Key).FirstOrDefault ();
				var c2 = this.compta.PlanComptable.Where (x => x.Numéro == item.Value).FirstOrDefault ();

				if (c1 != null && c2 != null)
				{
					c1.Groupe = c2;
				}
			}

			foreach (var item in boucles)
			{
				var c1 = this.compta.PlanComptable.Where (x => x.Numéro == item.Key).FirstOrDefault ();
				var c2 = this.compta.PlanComptable.Where (x => x.Numéro == item.Value).FirstOrDefault ();

				if (c1 != null && c2 != null)
				{
					c1.CompteOuvBoucl = c2;
				}
			}

			this.compta.UpdateNiveauCompte ();

			return null;  // ok
		}

		private Date? GetDate(string text)
		{
			System.DateTime d;

			if (System.DateTime.TryParse (text, out d))
			{
				return new Date (d);
			}

			return null;
		}

		private CatégorieDeCompte GetEntryContentCatégorie(int index, string key)
		{
			var value = this.GetEntryContentInt (index, key);

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

		private TypeDeCompte GetEntryContentType(int index, string key)
		{
			var value = this.GetEntryContentInt (index, key);

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

		private int? GetEntryContentInt(int index, string key)
		{
			var text = this.GetEntryContentText (index, key);

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

		private string GetEntryContentText(int index, string key)
		{
			key = key+"=";

			while (++index < this.lines.Length)
			{
				if (this.lines[index].StartsWith (key))
				{
					return this.lines[index].Substring (key.Length).Trim ();
				}

				if (this.lines[index].StartsWith ("ENTRY") ||  // est-on sur l'entrée suivante ?
					this.lines[index].StartsWith ("END="))     // fin du bloc ?
				{
					break;
				}
			}

			return null;
		}

		private int IndexOfLine(string key)
		{
			for (int i = 0; i < this.lines.Length; i++)
			{
				if (this.lines[i].StartsWith (key))
				{
					return i;
				}
			}

			return -1;
		}


		private ComptaEntity		compta;
		private string[]			lines;
	}
}
