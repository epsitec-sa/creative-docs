//	Copyright � 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	/// Cette classe s'occupe des import/export avec l'ancien logiciel Cr�sus Comptabilit� (DR/MW).
	/// </summary>
	public class Cr�susCompta
	{
		public string ImportFile(ComptaEntity compta, ref ComptaP�riodeEntity p�riode, string filename)
		{
			this.compta = compta;

			string ext = System.IO.Path.GetExtension (filename).ToLower ();

			if (ext == ".crp")
			{
				return this.ImportPlanComptable(filename, ref p�riode);
			}

			if (ext == ".txt")
			{
				return this.ImportEcritures (filename, ref p�riode);
			}

			return "Le fichier ne contient pas des donn�es connues.";
		}


		#region Plan comptable
		private string ImportPlanComptable(string filename, ref ComptaP�riodeEntity p�riode)
		{
			//	Importe un plan comptable "crp".
			try
			{
				this.lines = System.IO.File.ReadAllLines (filename, System.Text.Encoding.Default);

				try
				{
					string err = this.ImportPlanComptable (ref p�riode);

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

		private string ImportPlanComptable(ref ComptaP�riodeEntity p�riode)
		{
			var nc = new NewCompta ();
			nc.NewNull (this.compta);
			nc.CreateP�riodes (this.compta);

			var now = Date.Today;
			p�riode = this.compta.P�riodes.Where (x => x.DateD�but.Year == now.Year).FirstOrDefault ();

			//	Importe les donn�es globales.
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
					p�riode = this.compta.P�riodes.Where (x => x.DateD�but.Year == date.Year).FirstOrDefault ();
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

			var journauxTri�s = journaux.OrderBy (x => x.Key);

			foreach (var j in journauxTri�s)
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
					var num�ro = this.GetEntryContentText (indexCompte, "NUM");
					var titre  = this.GetEntryContentText (indexCompte, "NAME");

					if (string.IsNullOrEmpty (num�ro) || num�ro.Contains ("/") || string.IsNullOrEmpty (titre))
					{
						continue;
					}

					var compte = new ComptaCompteEntity ();

					compte.Num�ro    = num�ro;
					compte.Titre     = titre;
					compte.Cat�gorie = this.GetEntryContentCat�gorie (indexCompte, "CAT");
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
						groups.Add (num�ro, group);
					}

					var boucle = this.GetEntryContentText (indexCompte, "BOUCLE");
					if (!string.IsNullOrEmpty (boucle))
					{
						boucles.Add (num�ro, boucle);
					}

					var codeTVA = this.GetEntryContentText (indexCompte, "VATCODE");
					codesTVA.Add (compte, codeTVA);

					this.compta.PlanComptable.Add (compte);
				}
			}

			//	Met apr�s-coup les champs qui pointent sur des comptes.
			foreach (var item in groups)
			{
				var c1 = this.compta.PlanComptable.Where (x => x.Num�ro == item.Key).FirstOrDefault ();
				var c2 = this.compta.PlanComptable.Where (x => x.Num�ro == item.Value).FirstOrDefault ();

				if (c1 != null && c2 != null)
				{
					c1.Groupe = c2;
				}
			}

			foreach (var item in boucles)
			{
				var c1 = this.compta.PlanComptable.Where (x => x.Num�ro == item.Key).FirstOrDefault ();
				var c2 = this.compta.PlanComptable.Where (x => x.Num�ro == item.Value).FirstOrDefault ();

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
					codeTVA.Compte      = this.compta.PlanComptable.Where (x => x.Num�ro == this.GetEntryContentText (indexTVA, "COMPTE")).FirstOrDefault ();
					codeTVA.D�duction   = this.GetMontant (this.GetEntryContentText (indexTVA, "PCTDEDUCT"));

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

			//	Met � jour les codes TVA dans les comptes.
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
			//	Plut�t que d'essayer d'importer difficilement les donn�es de Cr�sus Comptabilit�, je pr�f�re les
			//	recr�er de toutes pi�ces. A priori, il n'y a pas de raison qu'elles soient diff�rentes, non !?
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
					Nom     = "R�duit 1",
					DateFin = new Date (2010, 12, 31),
					Taux    = 0.024m,
				};
				this.compta.TauxTVA.Add (taux);
			}

			{
				var taux = new ComptaTauxTVAEntity ()
				{
					Nom       = "R�duit 2",
					DateD�but = new Date (2011, 1, 1),
					Taux      = 0.025m,
				};
				this.compta.TauxTVA.Add (taux);
			}

			{
				var taux = new ComptaTauxTVAEntity ()
				{
					Nom     = "H�bergement 1",
					DateFin = new Date (2010, 12, 31),
					Taux    = 0.036m,
				};
				this.compta.TauxTVA.Add (taux);
			}

			{
				var taux = new ComptaTauxTVAEntity ()
				{
					Nom       = "H�bergement 2",
					DateD�but = new Date (2011, 1, 1),
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
					DateD�but = new Date (2011, 1, 1),
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

		private Cat�gorieDeCompte GetEntryContentCat�gorie(int index, string key)
		{
			var value = this.GetEntryContentInt (index, key);

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

				if (this.lines[index].StartsWith ("ENTRY") ||  // est-on sur l'entr�e suivante ?
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


		#region Ecritures tabul�es
		private string ImportEcritures(string filename, ref ComptaP�riodeEntity p�riode)
		{
			//	Importe un texte tabul� "txt".
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

					p�riode = this.CreateP�riode (journal);

					//	Met tous les libell�s des �critures dans les libell�s usuels.
					foreach (var �criture in journal)
					{
						this.compta.AddLibell� (p�riode, �criture.Libell�);
					}

					return null;  // ok
				}
				catch (System.Exception ex)
				{
					return string.Concat ("Le fichier ne contient pas un texte tabul� conforme.<br/>", ex.Message);
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
				var d�bit   = this.GetCompte (words[1]);
				var cr�dit  = this.GetCompte (words[2]);
				var pi�ce   = words[3];
				var libell� = words[4];
				var montant = this.GetMontant (words[5]);
				var multi   = this.GetInt (words[8]);
				var jp      = this.compta.Journaux[0];

				if (!date.HasValue)
				{
					continue;
				}

				if (d�bit == null && cr�dit == null)
				{
					continue;
				}

				var �criture = new ComptaEcritureEntity
				{
					Date       = date.Value,
					D�bit      = d�bit,
					Cr�dit     = cr�dit,
					Pi�ce      = pi�ce,
					Libell�    = libell�,
					MontantTTC = montant,
					MultiId    = multi,
					Journal    = jp,
				};

				journal.Add (�criture);

				if (lastEcriture != null && lastEcriture.MultiId != 0 && lastEcriture.MultiId != �criture.MultiId)
				{
					lastEcriture.TotalAutomatique = true;
				}

				lastEcriture = �criture;
				count++;
			}

			if (lastEcriture != null && lastEcriture.MultiId != 0)
			{
				lastEcriture.TotalAutomatique = true;
			}

			if (count == 0)
			{
				return "Le fichier ne contient aucune �criture.";
			}

			this.MergeStep1 (journal);
			this.MergeStep2 (journal);
			this.MergeStep3 (journal);

			return null;  // ok
		}

		private ComptaP�riodeEntity CreateP�riode(List<ComptaEcritureEntity> journal)
		{
			Date beginDate, endDate;
			this.GetYear (journal,  out beginDate, out endDate);

			//	Cherche si les �critures lues sont compatibles avec une p�riode existante.
			foreach (var p in this.compta.P�riodes)
			{
				if (beginDate >= p.DateD�but && endDate <= p.DateFin)
				{
					p.Journal.Clear ();
					journal.ForEach (x => p.Journal.Add (x));
					return p;
				}
			}

			//	Cr�e une nouvelle p�riode.
			var np = new ComptaP�riodeEntity ();

			beginDate = new Date (beginDate.Year,  1,  1);
			endDate   = new Date (  endDate.Year, 12, 31);

			np.DateD�but    = beginDate;
			np.DateFin      =   endDate;
			np.Derni�reDate = beginDate;

			this.compta.P�riodes.Add (np);
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
				return this.compta.PlanComptable.Where (x => x.Num�ro == text).FirstOrDefault ();
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
			//	Fusionne les 2 �critures de TVA (lignes 'brut' et 'TVA').
			int i = 0;
			while (i < journal.Count-1)
			{
				var �criture = journal[i];
				var suivante = journal[i+1];

				var merge = this.MergeEcritures1 (�criture, suivante);
				if (merge != null)
				{
					journal.RemoveAt (i);
					journal.RemoveAt (i);
					journal.Insert (i, merge);
				}

				i++;
			}
		}

		private ComptaEcritureEntity MergeEcritures1(ComptaEcritureEntity �criture, ComptaEcritureEntity suivante)
		{
			if (�criture.MultiId == 0 || �criture.MultiId != suivante.MultiId)
			{
				return null;
			}

			if ((�criture.D�bit  != null || suivante.D�bit  != null) &&
				(�criture.Cr�dit != null || suivante.Cr�dit != null))
			{
				return null;
			}

			var lib1 = �criture.Libell�.ToString ();
			var lib2 = suivante.Libell�.ToString ();

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

			//	Cr�e la nouvelle �criture qui fusionne les 2 autres.
			var merge = new ComptaEcritureEntity ()
			{
				Date        = �criture.Date,
				D�bit       = �criture.D�bit,
				Cr�dit      = �criture.Cr�dit,
				Pi�ce       = �criture.Pi�ce,
				Libell�     = lib1.Substring (0, i1),
				MontantTTC  = �criture.MontantTTC + suivante.MontantTTC,
				MontantTVA  = suivante.MontantTTC,
				MontantHT   = �criture.MontantTTC,
				CodeTVA     = codeTVA,
				TauxTVA     = taux,
				Journal     = �criture.Journal,
				MultiId     = �criture.MultiId,
			};

			return merge;
		}

		private void MergeStep2(List<ComptaEcritureEntity> journal)
		{
			//	Fusionne les �critures multiples de 2 lignes en une seule.
			int i = 0;
			while (i < journal.Count-1)
			{
				var �criture = journal[i];
				var suivante = journal[i+1];

				int count = this.MergeMultiCount (journal, i);
				if (count == 2)
				{
					var merge = this.MergeEcritures2 (�criture, suivante);

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
		
		private ComptaEcritureEntity MergeEcritures2(ComptaEcritureEntity �criture, ComptaEcritureEntity suivante)
		{
			if (�criture.MultiId == 0 || �criture.MultiId != suivante.MultiId)
			{
				return null;
			}

			//	Cr�e la nouvelle �criture qui fusionne les 2 autres.
			var merge = new ComptaEcritureEntity ()
			{
				Date       = �criture.Date,
				D�bit      = (�criture.D�bit  == null) ? suivante.D�bit  : �criture.D�bit,
				Cr�dit     = (�criture.Cr�dit == null) ? suivante.Cr�dit : �criture.Cr�dit,
				Pi�ce      = �criture.Pi�ce,
				Libell�    = �criture.Libell�,
				MontantTTC = �criture.MontantTTC,
				MontantTVA = �criture.MontantTVA,
				MontantHT  = �criture.MontantHT,
				CodeTVA    = �criture.CodeTVA,
				TauxTVA    = �criture.TauxTVA,
				Journal    = �criture.Journal,
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
			
			decimal totalHTD�bit   = 0;
			decimal totalHTCr�dit  = 0;
			decimal totalTVAD�bit  = 0;
			decimal totalTVACr�dit = 0;

			for (int index = i; index < i+count; index++)
			{
				var �criture = journal[index];

				if (�criture.TotalAutomatique)
				{
					cp = index;
				}
				else
				{
					if (�criture.D�bit == null)  // d�bit multiple ?
					{
						totalHTCr�dit  += �criture.MontantHT.GetValueOrDefault ();
						totalTVACr�dit += �criture.MontantTVA.GetValueOrDefault ();
					}

					if (�criture.Cr�dit == null)  // cr�dit multiple ?
					{
						totalHTD�bit  += �criture.MontantHT.GetValueOrDefault ();
						totalTVAD�bit += �criture.MontantTVA.GetValueOrDefault ();
					}
				}
			}

			if (cp != -1)
			{
				decimal? totalHT  = 0;
				decimal? totalTVA = 0;

				if (journal[cp].D�bit == null)  // d�bit multiple ?
				{
					totalHT  = totalHTD�bit  - totalHTCr�dit;
					totalTVA = totalTVAD�bit - totalTVACr�dit;
				}

				if (journal[cp].Cr�dit == null)  // cr�dit multiple ?
				{
					totalHT  = totalHTCr�dit  - totalHTD�bit;
					totalTVA = totalTVACr�dit - totalTVAD�bit;
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
