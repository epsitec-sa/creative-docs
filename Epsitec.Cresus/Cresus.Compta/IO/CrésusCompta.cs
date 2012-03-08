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

					if (string.IsNullOrEmpty (err) && this.compta.Nom.IsNullOrEmpty)
					{
						this.compta.Nom = System.IO.Path.GetFileNameWithoutExtension (filename);
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
					this.compta.Nom = this.lines[i].Substring (6);
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
					var rank = this.GetEntryContentInt (indexJournal, "NUM");
					var name = this.GetEntryContentText (indexJournal, "NAME");
					journaux.Add (rank, name);
				}
			}

			var journauxTriés = journaux.OrderBy (x => x.Key);

			foreach (var j in journauxTriés)
			{
				var journal = new ComptaJournalEntity ();
				journal.Id = this.compta.GetJournalId ();
				journal.Nom = j.Value;
				this.compta.Journaux.Add (journal);
			}

			//	Importe tous les comptes.
			int indexCompte = this.IndexOfLine ("BEGIN=COMPTES");

			var groups  = new Dictionary<string, string> ();
			var boucles = new Dictionary<string, string> ();
			var codesTVA = new Dictionary<ComptaCompteEntity, string> ();

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
					compte.Type      = this.GetEntryContentType (indexCompte, "STATUS");
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

					var codeTVA = this.GetEntryContentText (indexCompte, "VATCODE");
					codesTVA.Add (compte, codeTVA);

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

			//	Importe les taux de TVA.
			this.CreateTaux ();

#if false
			int indexTaux = this.IndexOfLine ("BEGIN=VATRATES");

			var taux = new Dictionary<decimal, decimal> ();

			while (++indexTaux < this.lines.Length)
			{
				var line = this.lines[indexTaux];

				if (string.IsNullOrEmpty (line))
				{
					continue;
				}

				if (line.StartsWith ("END=VATRATES"))
				{
					break;
				}

				var words = line.Split ('\t');
				decimal t1 = decimal.Parse (words[0]) / 100;
				decimal t2 = decimal.Parse (words[1]) / 100;
				if (!taux.ContainsKey (t1))
				{
					taux.Add (t1, t2);
				}
			}
#endif

			//	Importe des codes TVA.
			int indexTVA = this.IndexOfLine ("BEGIN=TVACODES");
			var codesTVAList = new List<ComptaCodeTVAEntity> ();

			while (++indexTVA < this.lines.Length)
			{
				var line = this.lines[indexTVA];

				if (string.IsNullOrEmpty (line))
				{
					continue;
				}

				if (line.StartsWith ("END=TVACODES"))
				{
					break;
				}

				if (line.StartsWith ("ENTRY"))
				{
					var codeTVA = new ComptaCodeTVAEntity ();

					codeTVA.Code        = this.GetEntryContentText (indexTVA, "NAME");
					codeTVA.Description = this.GetEntryContentText (indexTVA, "COMMENT");
					codeTVA.Compte      = this.compta.PlanComptable.Where (x => x.Numéro == this.GetEntryContentText (indexTVA, "COMPTE")).FirstOrDefault ();
					codeTVA.Déduction   = this.GetMontant (this.GetEntryContentText (indexTVA, "PCTDEDUCT"));

					var t = this.GetMontant (this.GetEntryContentText (indexTVA, "TAUX")) / 100;
					var taux = this.compta.TauxTVA.Where (x => x.Taux == t).FirstOrDefault ();

					if (taux != null)
					{
						this.InsertTaux (codeTVA, taux);
						codesTVAList.Add (codeTVA);
					}
				}
			}

			foreach (var code in codesTVAList.OrderBy (x => x.Code))
			{
				this.compta.CodesTVA.Add (code);
			}

			//	Met à jour les codes TVA dans les comptes.
			foreach (var pair in codesTVA)
			{
				var compte  = pair.Key;
				var codeTVA = pair.Value;

				compte.CodeTVA = this.compta.CodesTVA.Where (x => x.Code == codeTVA).FirstOrDefault ();
			}

			return null;  // ok
		}

		private void CreateTaux()
		{
			//	Plutôt que d'essayer d'importer difficilement les données de Crésus Comptabilité, je préfère les
			//	recréer de toutes pièces. A priori, il n'y a pas de raison qu'elles soient différentes, non !?
			{
				var taux = new ComptaTauxTVAEntity ()
				{
					Nom     = "Exclu",
					Taux    = 0.0m,
				};
				this.compta.TauxTVA.Add (taux);
			}

			{
				var taux = new ComptaTauxTVAEntity ()
				{
					Nom     = "Réduit 1",
					DateFin = new Date (2010, 12, 31),
					Taux    = 0.024m,
				};
				this.compta.TauxTVA.Add (taux);
			}

			{
				var taux = new ComptaTauxTVAEntity ()
				{
					Nom       = "Réduit 2",
					DateDébut = new Date (2011, 1, 1),
					Taux      = 0.025m,
				};
				this.compta.TauxTVA.Add (taux);
			}

			{
				var taux = new ComptaTauxTVAEntity ()
				{
					Nom     = "Hébergement 1",
					DateFin = new Date (2010, 12, 31),
					Taux    = 0.036m,
				};
				this.compta.TauxTVA.Add (taux);
			}

			{
				var taux = new ComptaTauxTVAEntity ()
				{
					Nom       = "Hébergement 2",
					DateDébut = new Date (2011, 1, 1),
					Taux      = 0.038m,
				};
				this.compta.TauxTVA.Add (taux);
			}

			{
				var taux = new ComptaTauxTVAEntity ()
				{
					Nom     = "Normal 1",
					DateFin = new Date (2010, 12, 31),
					Taux    = 0.076m,
				};
				this.compta.TauxTVA.Add (taux);
			}

			{
				var taux = new ComptaTauxTVAEntity ()
				{
					Nom       = "Normal 2",
					DateDébut = new Date (2011, 1, 1),
					Taux      = 0.08m,
				};
				this.compta.TauxTVA.Add (taux);
			}
		}

		private void InsertTaux(ComptaCodeTVAEntity codeTVA, ComptaTauxTVAEntity taux)
		{
			codeTVA.Taux.Add (taux);

			var nom = taux.Nom.ToString ().Split (' ').FirstOrDefault ();

			foreach (var t in this.compta.TauxTVA)
			{
				if (t.Nom.ToString ().StartsWith (nom) && !codeTVA.Taux.Contains (t))
				{
					codeTVA.Taux.Add (t);
				}
			}
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
					Date       = date.Value,
					Débit      = débit,
					Crédit     = crédit,
					Pièce      = pièce,
					Libellé    = libellé,
					MontantTTC = montant,
					MultiId    = multi,
					Journal    = jp,
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

			this.MergeStep1 (journal);
			this.MergeStep2 (journal);
			this.MergeStep3 (journal);

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


		private void MergeStep1(List<ComptaEcritureEntity> journal)
		{
			//	Fusionne les 2 écritures de TVA (lignes 'brut' et 'TVA').
			int i = 0;
			while (i < journal.Count-1)
			{
				var écriture = journal[i];
				var suivante = journal[i+1];

				var merge = this.MergeEcritures1 (écriture, suivante);
				if (merge != null)
				{
					journal.RemoveAt (i);
					journal.RemoveAt (i);
					journal.Insert (i, merge);
				}

				i++;
			}
		}

		private ComptaEcritureEntity MergeEcritures1(ComptaEcritureEntity écriture, ComptaEcritureEntity suivante)
		{
			if (écriture.MultiId == 0 || écriture.MultiId != suivante.MultiId)
			{
				return null;
			}

			if ((écriture.Débit  != null || suivante.Débit  != null) &&
				(écriture.Crédit != null || suivante.Crédit != null))
			{
				return null;
			}

			var lib1 = écriture.Libellé.ToString ();
			var lib2 = suivante.Libellé.ToString ();

			int i1 = lib1.LastIndexOf (", ");
			int i2 = lib2.LastIndexOf (", ");

			if (i1 == -1 || i2 == -1 || i1 != i2)
			{
				return null;
			}

			if (lib1.Substring (0, i1) != lib2.Substring (0, i2))
			{
				return null;
			}

			//	Cherche le code TVA.
			if (lib1[i1+2] != '(')
			{
				return null;
			}

			int i = lib1.IndexOf (')', i1+2);
			if (i == -1)
			{
				return null;
			}

			string code = lib1.Substring (i1+3, i-i1-3);
			var codeTVA = this.compta.CodesTVA.Where (x => x.Code == code).FirstOrDefault ();
			if (codeTVA == null)
			{
				return null;
			}

			//	Cherche le taux.
			int j = lib2.IndexOf ('%', i2+2);
			if (j == -1)
			{
				return null;
			}

			decimal taux;
			if (decimal.TryParse (lib2.Substring (i2+2, j-i2-2), out taux))
			{
				taux /= 100;
			}
			else
			{
				return null;
			}

			//	Crée la nouvelle écriture qui fusionne les 2 autres.
			var merge = new ComptaEcritureEntity ()
			{
				Date        = écriture.Date,
				Débit       = écriture.Débit,
				Crédit      = écriture.Crédit,
				Pièce       = écriture.Pièce,
				Libellé     = lib1.Substring (0, i1),
				MontantTTC  = écriture.MontantTTC + suivante.MontantTTC,
				MontantTVA  = suivante.MontantTTC,
				MontantHT   = écriture.MontantTTC,
				CodeTVA     = codeTVA,
				TauxTVA     = taux,
				Journal     = écriture.Journal,
				MultiId     = écriture.MultiId,
			};

			return merge;
		}

		private void MergeStep2(List<ComptaEcritureEntity> journal)
		{
			//	Fusionne les écritures multiples de 2 lignes en une seule.
			int i = 0;
			while (i < journal.Count-1)
			{
				var écriture = journal[i];
				var suivante = journal[i+1];

				int count = this.MergeMultiCount (journal, i);
				if (count == 2)
				{
					var merge = this.MergeEcritures2 (écriture, suivante);

					if (merge != null)
					{
						journal.RemoveAt (i);
						journal.RemoveAt (i);
						journal.Insert (i, merge);
						i++;
						continue;
					}
				}

				i += count;
			}
		}
		
		private ComptaEcritureEntity MergeEcritures2(ComptaEcritureEntity écriture, ComptaEcritureEntity suivante)
		{
			if (écriture.MultiId == 0 || écriture.MultiId != suivante.MultiId)
			{
				return null;
			}

			//	Crée la nouvelle écriture qui fusionne les 2 autres.
			var merge = new ComptaEcritureEntity ()
			{
				Date       = écriture.Date,
				Débit      = (écriture.Débit  == null) ? suivante.Débit  : écriture.Débit,
				Crédit     = (écriture.Crédit == null) ? suivante.Crédit : écriture.Crédit,
				Pièce      = écriture.Pièce,
				Libellé    = écriture.Libellé,
				MontantTTC = écriture.MontantTTC,
				MontantTVA = écriture.MontantTVA,
				MontantHT  = écriture.MontantHT,
				CodeTVA    = écriture.CodeTVA,
				TauxTVA    = écriture.TauxTVA,
				Journal    = écriture.Journal,
			};

			return merge;
		}

		private void MergeStep3(List<ComptaEcritureEntity> journal)
		{
			//	Recalcule les totaux 'brut' et 'TVA'.
			int i = 0;
			while (i < journal.Count)
			{
				int count = this.MergeMultiCount (journal, i);
				if (count > 1)
				{
					this.MergeMultiTotal (journal, i, count);
				}

				i += count;
			}
		}

		private void MergeMultiTotal(List<ComptaEcritureEntity> journal, int i, int count)
		{
			int cp = -1;
			
			decimal totalHTDébit   = 0;
			decimal totalHTCrédit  = 0;
			decimal totalTVADébit  = 0;
			decimal totalTVACrédit = 0;

			for (int index = i; index < i+count; index++)
			{
				var écriture = journal[index];

				if (écriture.TotalAutomatique)
				{
					cp = index;
				}
				else
				{
					if (écriture.Débit == null)  // débit multiple ?
					{
						totalHTCrédit  += écriture.MontantHT.GetValueOrDefault ();
						totalTVACrédit += écriture.MontantTVA.GetValueOrDefault ();
					}

					if (écriture.Crédit == null)  // crédit multiple ?
					{
						totalHTDébit  += écriture.MontantHT.GetValueOrDefault ();
						totalTVADébit += écriture.MontantTVA.GetValueOrDefault ();
					}
				}
			}

			if (cp != -1)
			{
				decimal? totalHT  = 0;
				decimal? totalTVA = 0;

				if (journal[cp].Débit == null)  // débit multiple ?
				{
					totalHT  = totalHTDébit  - totalHTCrédit;
					totalTVA = totalTVADébit - totalTVACrédit;
				}

				if (journal[cp].Crédit == null)  // crédit multiple ?
				{
					totalHT  = totalHTCrédit  - totalHTDébit;
					totalTVA = totalTVACrédit - totalTVADébit;
				}

				if (totalHT == 0)
				{
					totalHT = null;
				}

				if (totalTVA == 0)
				{
					totalTVA = null;
				}

				journal[cp].MontantHT  = totalHT;
				journal[cp].MontantTVA = totalTVA;
			}
		}

		private int MergeMultiCount(List<ComptaEcritureEntity> journal, int i)
		{
			int count = 1;
			int id = journal[i].MultiId;

			if (id != 0)
			{
				while (++i < journal.Count)
				{
					if (id != journal[i].MultiId)
					{
						break;
					}

					count++;
				}
			}

			return count;
		}
		#endregion


		private ComptaEntity			compta;
		private string[]				lines;
	}
}
