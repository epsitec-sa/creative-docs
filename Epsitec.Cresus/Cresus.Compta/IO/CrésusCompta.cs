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
			NewCompta.NewNull (this.compta);
			NewCompta.CreateP�riodes (this.compta);

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
			var monnaies = new Dictionary<ComptaCompteEntity, int> ();

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

					var monnaie = this.GetEntryContentInt (indexCompte, "CURRENCY").GetValueOrDefault (0);
					if (monnaie != 0)
					{
						monnaies.Add (compte, monnaie);
					}

					//	Il ne samble pas y avoir d'autre moyen pour savoir s'il s'agit d'un compte de TVA !
					if (compte.Type == TypeDeCompte.Normal && titre.Contains ("TVA"))
					{
						compte.Type = TypeDeCompte.TVA;
					}

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
					if (!string.IsNullOrEmpty (codeTVA))
					{
						codesTVA.Add (compte, codeTVA);
					}

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

#if true
			//	Plut�t que d'essayer d'importer difficilement les donn�es de Cr�sus Comptabilit�, je pr�f�re les
			//	recr�er de toutes pi�ces. A priori, il n'y a pas de raison qu'elles soient diff�rentes, non !?
			NewCompta.CreateTVA (this.compta);
#else
			//	Importe les taux de TVA.
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
					var codeTVA = new ComptaCodeTVAEntity ()
					{
						Code        = this.GetEntryContentText (indexTVA, "NAME"),
						Description = this.GetEntryContentText (indexTVA, "COMMENT"),
						Compte      = this.compta.PlanComptable.Where (x => x.Num�ro == this.GetEntryContentText (indexTVA, "COMPTE")).FirstOrDefault (),
						D�duction   = this.GetMontant (this.GetEntryContentText (indexTVA, "PCTDEDUCT")),
						ListeTaux   = this.compta.GetListeTVA (this.GetMontant (this.GetEntryContentText (indexTVA, "TAUX")) / 100),
					};

					if (codeTVA.ListeTaux != null && !codeTVA.Description.ToString ().ToLower ().Contains ("obsol�te"))
					{
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

				compte.CodeTVAParD�faut = this.compta.CodesTVA.Where (x => x.Code == codeTVA).FirstOrDefault ();
				this.SetCodeTVA (compte);
			}

			//	Importe les monnaies.
			int indexCurrencies = this.IndexOfLine ("BEGIN=CURRENCIES");
			this.compta.Monnaies.Clear ();

			{
				//	Cr�e la monnaie CHF de base.
				var monnaie = new ComptaMonnaieEntity ()
				{
					CodeISO     = "CHF",
					Description = Currencies.GetCurrencySpecies ("CHF"),
					D�cimales   = 2,
					Arrondi     = 0.01m,
					Cours       = 1.0m,
					Unit�       = 1,
				};

				this.compta.Monnaies.Add (monnaie);
			}

			while (++indexCurrencies < this.lines.Length)
			{
				var line = this.lines[indexCurrencies];

				if (string.IsNullOrEmpty (line))
				{
					continue;
				}

				if (line.StartsWith ("END=CURRENCIES"))
				{
					break;
				}

				if (line.StartsWith ("ENTRY"))
				{
					var iso = this.GetEntryContentText (indexCurrencies, "NAME");

					var monnaie = new ComptaMonnaieEntity ()
					{
						CodeISO     = iso,
						Description = Currencies.GetCurrencySpecies (iso),
						D�cimales   = 2,
						Arrondi     = 0.01m,
						Cours       = Converters.ParseDecimal (this.GetEntryContentText (indexCurrencies, "COURS")).GetValueOrDefault (1),
						Unit�       = Converters.ParseInt (this.GetEntryContentText (indexCurrencies, "UNITE")).GetValueOrDefault (1),
						CompteGain  = this.compta.PlanComptable.Where (x => x.Num�ro == this.GetEntryContentText (indexCurrencies, "CGAIN")).FirstOrDefault (),
						ComptePerte = this.compta.PlanComptable.Where (x => x.Num�ro == this.GetEntryContentText (indexCurrencies, "CPERTE")).FirstOrDefault (),
					};

					this.compta.Monnaies.Add (monnaie);
				}
			}

			if (this.compta.Monnaies.Count >= 2)
			{
				this.compta.Monnaies[0].CompteGain  = this.compta.Monnaies[1].CompteGain;
				this.compta.Monnaies[0].ComptePerte = this.compta.Monnaies[1].ComptePerte;
			}

			//	Met les monnaies dans le plan compatable.
			foreach (var compte in this.compta.PlanComptable)
			{
				if (monnaies.ContainsKey (compte))
				{
					int index = monnaies[compte];
					if (index < this.compta.Monnaies.Count)
					{
						compte.Monnaie = this.compta.Monnaies[index];
					}
				}
				else
				{
					compte.Monnaie = this.compta.Monnaies[0];
				}
			}

			return null;  // ok
		}

		private void SetCodeTVA(ComptaCompteEntity compte)
		{
			//	Essaie de "deviner" les codes TVA possibles � partir d'un code d�fini.
			if (compte.CodeTVAParD�faut != null)
			{
				var zero = this.compta.CodesTVA.Where (x => x.Code == "EXPORT").FirstOrDefault ();
				if (zero != null)
				{
					compte.CodesTVAPossibles.Add (zero);
				}

				if (compte.CodeTVAParD�faut.Code.Length >= 3)
				{
					var prefix = compte.CodeTVAParD�faut.Code.ToString ().Substring (0, 3);

					foreach (var codeTVA in this.compta.CodesTVA)
					{
						if (codeTVA.Code.ToString ().StartsWith (prefix))
						{
							compte.CodesTVAPossibles.Add (codeTVA);
						}
					}
				}
				else
				{
					compte.CodesTVAPossibles.Add (compte.CodeTVAParD�faut);
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
				var codeTVA = words[12];

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
					Date    = date.Value,
					D�bit   = d�bit,
					Cr�dit  = cr�dit,
					Pi�ce   = pi�ce,
					Libell� = libell�,
					Montant = montant,
					MultiId = multi,
					Journal = jp,
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
				var encore   = (i+2 < journal.Count) ? journal[i+2] : null;

				if (this.MergeEcritures1 (�criture, suivante, encore))
				{
					i += 2;
				}
				else
				{
					i++;
				}
			}
		}

		private bool MergeEcritures1(ComptaEcritureEntity �criture, ComptaEcritureEntity suivante, ComptaEcritureEntity encore)
		{
			if (�criture.MultiId == 0 || �criture.MultiId != suivante.MultiId)
			{
				return false;
			}

			if ((�criture.D�bit  != null || suivante.D�bit  != null) &&
				(�criture.Cr�dit != null || suivante.Cr�dit != null))
			{
				return false;
			}

			var compteBase = (�criture.D�bit == null) ? �criture.Cr�dit : �criture.D�bit;
			var compteTVA  = (suivante.D�bit == null) ? suivante.Cr�dit : suivante.D�bit;

			if (compteTVA.Type != TypeDeCompte.TVA)
			{
				return false;
			}

			var lib1 = �criture.Libell�.ToString ();
			var lib2 = suivante.Libell�.ToString ();

			//	Exemples d'�critures possibles:

			//	4200 ...  Achats Roger, (IPM) net
			//	1170 ...  Achats Roger, 7.6% de TVA (IPM)

			//	5283 ...  Invitation client, (IPFREP) net
			//	1171 ...  Invitation client, 7.6% de TVA d�duit � 50.00%

			//	...  3900 Escompte net
			//	...  2200 Part TVA escompte (TVA)

			//	Cherche le code TVA, dans le 2�me libell� puis dans le 1er.
			string code = Cr�susCompta.ExtractCodeTVA (lib2);
			if (string.IsNullOrEmpty (code))
			{
				code = Cr�susCompta.ExtractCodeTVA (lib1);
				if (string.IsNullOrEmpty (code))
				{
					return false;
				}
			}

			var codeTVA = this.compta.CodesTVA.Where (x => x.Code == code).FirstOrDefault ();
			if (codeTVA == null)
			{
				return false;
			}

			//	Cherche le taux, dans le 2�me libell� puis en le calculant.
			decimal? taux = Cr�susCompta.GetTaux (lib2);
			if (!taux.HasValue)
			{
				taux = Cr�susCompta.GetTaux (�criture.Montant, suivante.Montant);
			}
			if (!taux.HasValue)
			{
				return false;
			}

			�criture.Type              = (int) TypeEcriture.BaseTVA;
			�criture.OrigineTVA        = (compteBase == �criture.D�bit) ? "D" : "C";
			�criture.Libell�           = Cr�susCompta.SimplifyLibell� (lib1);
			�criture.MontantCompl�ment = suivante.Montant;
			�criture.CodeTVA           = codeTVA;
			�criture.TauxTVA           = taux;

			suivante.Type              = (int) TypeEcriture.CodeTVA;
			suivante.OrigineTVA        = (compteBase == �criture.D�bit) ? "D" : "C";
			suivante.Libell�           = �criture.Libell�;
			suivante.MontantCompl�ment = �criture.Montant;
			suivante.CodeTVA           = codeTVA;
			suivante.TauxTVA           = taux;

			if (encore != null)
			{
				string ending = string.Concat (" Total, (", codeTVA.Code, ")");
				var lib = encore.Libell�.ToString ();

				if (lib.EndsWith (ending))
				{
					encore.Libell� = lib.Substring (0, lib.Length-ending.Length);
				}
			}

			return true;
		}

		private static string ExtractCodeTVA(string libell�)
		{
			//	Extrait le code TVA d'un libell�.
			//	"Eau avril, 2.4% de TVA (IPIRED)"	-> IPIRED
			//	"Elec avril, (IPI) net"				-> IPI
			//	"Part TVA escompte (TVA)"			-> TVA
			if (string.IsNullOrEmpty (libell�))
			{
				return null;
			}

			int i1 = libell�.LastIndexOf ('(');
			if (i1 == -1)
			{
				return null;
			}

			int i2 = libell�.LastIndexOf (')');
			if (i2 == -1)
			{
				return null;
			}

			return libell�.Substring (i1+1, i2-i1-1);
		}

		private static decimal? GetTaux(decimal montantHT, decimal montantTVA)
		{
			//	Retourne le taux de TVA arrondi � une d�cimale.
			//	HT = 255.81, TVA = 19.44 -> 0.07599 -> 0.076
			if (montantHT == 0)
			{
				return null;
			}
			else
			{
				var taux = montantTVA / montantHT;
				return System.Math.Floor ((taux*1000) + 0.5m) / 1000;
			}
		}

		private static decimal? GetTaux(string libell�)
		{
			//	Retourne le taux de TVA contenu dans un libell�.
			//	"Eau avril, 2.4% de TVA (IPIRED)"	-> 0.024
			if (string.IsNullOrEmpty (libell�))
			{
				return null;
			}

			var words = libell�.Replace (",", " ").Split (' ');

			foreach (var word in words)
			{
				if (!string.IsNullOrEmpty (word) && word.Last () == '%')
				{
					var n = Converters.ParsePercent (word);

					if (n.HasValue && n >= 0 && n < 0.2m)  // valeur comprise entre 0% et 20% ?
					{
						return n;
					}
				}
			}

			return null;
		}

		private static string SimplifyLibell�(string libell�)
		{
			//	Simplifie le libell� de base.
			//	"Elec mars, (IPI) net" -> "Elec mars"
			if (string.IsNullOrEmpty (libell�))
			{
				return null;
			}

			int i = libell�.LastIndexOf (',');
			if (i == -1)
			{
				return libell�;
			}

			return libell�.Substring (0, i);
		}
		#endregion


		private ComptaEntity			compta;
		private string[]				lines;
	}
}
