//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.IO
{
	/// <summary>
	/// Cette classe s'occupe des import/export avec l'ancien logiciel Crésus Comptabilité (DR/MW).
	/// </summary>
	public class CrésusCompta
	{
		public string ImportFile(ComptaEntity compta, ref ComptaPériodeEntity période, string filename)
		{
			this.compta = compta;

			string ext = System.IO.Path.GetExtension (filename).ToLower ();

			if (ext == ".crp")
			{
				return this.ImportPlanComptable(filename, ref période);
			}

			if (ext == ".txt")
			{
				return this.ImportEcritures (filename, ref période);
			}

			return "Le fichier ne contient pas des données connues.";
		}


		#region Plan comptable
		private string ImportPlanComptable(string filename, ref ComptaPériodeEntity période)
		{
			//	Importe un plan comptable "crp".
			try
			{
				this.lines = System.IO.File.ReadAllLines (filename, System.Text.Encoding.Default);

				try
				{
					string err = this.ImportPlanComptable (ref période);

					if (string.IsNullOrEmpty (err) && this.compta.Name.IsNullOrEmpty)
					{
						this.compta.Name = System.IO.Path.GetFileNameWithoutExtension (filename);
					}

					return err;
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

		private string ImportPlanComptable(ref ComptaPériodeEntity période)
		{
			var nc = new NewCompta ();
			nc.NewNull (this.compta);
			nc.CreatePériodes (this.compta);

			var now = Date.Today;
			période = this.compta.Périodes.Where (x => x.DateDébut.Year == now.Year).FirstOrDefault ();

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
					var date = this.GetDate (this.lines[i].Substring (8));
					période = this.compta.Périodes.Where (x => x.DateDébut.Year == date.Year).FirstOrDefault ();
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

		private Date GetDate(string text)
		{
			System.DateTime d;
			if (System.DateTime.TryParse (text, out d))
			{
				return new Date (d);
			}

			return Date.Today;
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
		#endregion


		#region Ecritures tabulées
		private string ImportEcritures(string filename, ref ComptaPériodeEntity période)
		{
			//	Importe un texte tabulé "txt".
			try
			{
				this.lines = System.IO.File.ReadAllLines (filename, System.Text.Encoding.Default);

				try
				{
					var journal = new List<ComptaEcritureEntity> ();
					var err = this.ImportEcritures (journal);

					if (!string.IsNullOrEmpty (err))
					{
						return err;
					}

					période = this.CreatePériode (journal);

					//	Met tous les libellés des écritures dans les libellés usuels.
					foreach (var écriture in journal)
					{
						this.compta.AddLibellé (période, écriture.Libellé);
					}

					return null;  // ok
				}
				catch (System.Exception ex)
				{
					return string.Concat ("Le fichier ne contient pas un texte tabulé conforme.<br/>", ex.Message);
				}
			}
			catch (System.Exception ex)
			{
				return ex.Message;
			}
		}

		private string ImportEcritures(List<ComptaEcritureEntity> journal)
		{
			int count = 0;
			ComptaEcritureEntity lastEcriture = null;

			foreach (var line in lines)
			{
				if (string.IsNullOrEmpty (line))
				{
					continue;
				}

				var words = line.Split ('\t');

				if (words.Length < 9)
				{
					continue;
				}

				var date    = Converters.ParseDate (words[0]);
				var débit   = this.GetCompte (words[1]);
				var crédit  = this.GetCompte (words[2]);
				var pièce   = words[3];
				var libellé = words[4];
				var montant = this.GetMontant (words[5]);
				var multi   = this.GetInt (words[8]);
				var jp      = this.compta.Journaux[0];

				if (!date.HasValue)
				{
					continue;
				}

				if (débit == null && crédit == null)
				{
					continue;
				}

				var écriture = new ComptaEcritureEntity
				{
					Date    = date.Value,
					Débit   = débit,
					Crédit  = crédit,
					Pièce   = pièce,
					Libellé = libellé,
					Montant = montant,
					MultiId = multi,
					Journal = jp,
				};

				journal.Add (écriture);

				if (lastEcriture != null && lastEcriture.MultiId != 0 && lastEcriture.MultiId != écriture.MultiId)
				{
					lastEcriture.TotalAutomatique = true;
				}

				lastEcriture = écriture;
				count++;
			}

			if (lastEcriture != null && lastEcriture.MultiId != 0)
			{
				lastEcriture.TotalAutomatique = true;
			}

			if (count == 0)
			{
				return "Le fichier ne contient aucune écriture.";
			}

			return null;  // ok
		}

		private ComptaPériodeEntity CreatePériode(List<ComptaEcritureEntity> journal)
		{
			Date beginDate, endDate;
			this.GetYear (journal,  out beginDate, out endDate);

			//	Cherche si les écritures lues sont compatibles avec une période existante.
			foreach (var p in this.compta.Périodes)
			{
				if (beginDate >= p.DateDébut && endDate <= p.DateFin)
				{
					p.Journal.Clear ();
					journal.ForEach (x => p.Journal.Add (x));
					return p;
				}
			}

			//	Crée une nouvelle période.
			var np = new ComptaPériodeEntity ();

			beginDate = new Date (beginDate.Year,  1,  1);
			endDate   = new Date (  endDate.Year, 12, 31);

			np.DateDébut    = beginDate;
			np.DateFin      =   endDate;
			np.DernièreDate = beginDate;

			this.compta.Périodes.Add (np);
			return np;
		}

		private void GetYear(List<ComptaEcritureEntity> journal, out Date beginDate, out Date endDate)
		{
			beginDate = journal.First ().Date;
			endDate   = journal.Last  ().Date;
		}

		private ComptaCompteEntity GetCompte(string text)
		{
			if (!string.IsNullOrEmpty (text) && text != "...")
			{
				return this.compta.PlanComptable.Where (x => x.Numéro == text).FirstOrDefault ();
			}

			return null;
		}

		private decimal GetMontant(string text)
		{
			return Converters.ParseMontant (text).GetValueOrDefault ();
		}

		private int GetInt(string text)
		{
			int n;
			if (int.TryParse (text, out n))
			{
				return n;
			}

			return 0;
		}
		#endregion


		private ComptaEntity			compta;
		private string[]				lines;
	}
}
