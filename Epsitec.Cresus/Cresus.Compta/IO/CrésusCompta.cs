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
			NewCompta.NewNull (this.compta);
			NewCompta.CreatePériodes (this.compta);

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
						groups.Add (numéro, group);
					}

					var boucle = this.GetEntryContentText (indexCompte, "BOUCLE");
					if (!string.IsNullOrEmpty (boucle))
					{
						boucles.Add (numéro, boucle);
					}

					var codeTVA = this.GetEntryContentText (indexCompte, "VATCODE");
					if (!string.IsNullOrEmpty (codeTVA))
					{
						codesTVA.Add (compte, codeTVA);
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

#if true
			//	Plutôt que d'essayer d'importer difficilement les données de Crésus Comptabilité, je préfère les
			//	recréer de toutes pièces. A priori, il n'y a pas de raison qu'elles soient différentes, non !?
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
						Compte      = this.compta.PlanComptable.Where (x => x.Numéro == this.GetEntryContentText (indexTVA, "COMPTE")).FirstOrDefault (),
						Déduction   = this.GetMontant (this.GetEntryContentText (indexTVA, "PCTDEDUCT")),
						ListeTaux   = this.compta.GetListeTVA (this.GetMontant (this.GetEntryContentText (indexTVA, "TAUX")) / 100),
					};

					if (codeTVA.ListeTaux != null && !codeTVA.Description.ToString ().ToLower ().Contains ("obsolète"))
					{
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

				compte.CodeTVAParDéfaut = this.compta.CodesTVA.Where (x => x.Code == codeTVA).FirstOrDefault ();
				this.SetCodeTVA (compte);
			}

			//	Importe les monnaies.
			int indexCurrencies = this.IndexOfLine ("BEGIN=CURRENCIES");
			this.compta.Monnaies.Clear ();

			{
				//	Crée la monnaie CHF de base.
				var monnaie = new ComptaMonnaieEntity ()
				{
					CodeISO     = "CHF",
					Description = Currencies.GetCurrencySpecies ("CHF"),
					Décimales   = 2,
					Arrondi     = 0.01m,
					Cours       = 1.0m,
					Unité       = 1,
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
						Décimales   = 2,
						Arrondi     = 0.01m,
						Cours       = Converters.ParseDecimal (this.GetEntryContentText (indexCurrencies, "COURS")).GetValueOrDefault (1),
						Unité       = Converters.ParseInt (this.GetEntryContentText (indexCurrencies, "UNITE")).GetValueOrDefault (1),
						CompteGain  = this.compta.PlanComptable.Where (x => x.Numéro == this.GetEntryContentText (indexCurrencies, "CGAIN")).FirstOrDefault (),
						ComptePerte = this.compta.PlanComptable.Where (x => x.Numéro == this.GetEntryContentText (indexCurrencies, "CPERTE")).FirstOrDefault (),
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
			//	Essaie de "deviner" les codes TVA possibles à partir d'un code défini.
			if (compte.CodeTVAParDéfaut != null)
			{
				var zero = this.compta.CodesTVA.Where (x => x.Code == "EXPORT").FirstOrDefault ();
				if (zero != null)
				{
					compte.CodesTVAPossibles.Add (zero);
				}

				if (compte.CodeTVAParDéfaut.Code.Length >= 3)
				{
					var prefix = compte.CodeTVAParDéfaut.Code.ToString ().Substring (0, 3);

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
					compte.CodesTVAPossibles.Add (compte.CodeTVAParDéfaut);
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
				var codeTVA = words[12];

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

			this.MergeStep1 (journal);

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
				var encore   = (i+2 < journal.Count) ? journal[i+2] : null;

				if (this.MergeEcritures1 (écriture, suivante, encore))
				{
					i += 2;
				}
				else
				{
					i++;
				}
			}
		}

		private bool MergeEcritures1(ComptaEcritureEntity écriture, ComptaEcritureEntity suivante, ComptaEcritureEntity encore)
		{
			if (écriture.MultiId == 0 || écriture.MultiId != suivante.MultiId)
			{
				return false;
			}

			if ((écriture.Débit  != null || suivante.Débit  != null) &&
				(écriture.Crédit != null || suivante.Crédit != null))
			{
				return false;
			}

			var compteBase = (écriture.Débit == null) ? écriture.Crédit : écriture.Débit;
			var compteTVA  = (suivante.Débit == null) ? suivante.Crédit : suivante.Débit;

			if (compteTVA.Type != TypeDeCompte.TVA)
			{
				return false;
			}

			var lib1 = écriture.Libellé.ToString ();
			var lib2 = suivante.Libellé.ToString ();

			//	Exemples d'écritures possibles:

			//	4200 ...  Achats Roger, (IPM) net
			//	1170 ...  Achats Roger, 7.6% de TVA (IPM)

			//	5283 ...  Invitation client, (IPFREP) net
			//	1171 ...  Invitation client, 7.6% de TVA déduit à 50.00%

			//	...  3900 Escompte net
			//	...  2200 Part TVA escompte (TVA)

			//	Cherche le code TVA, dans le 2ème libellé puis dans le 1er.
			string code = CrésusCompta.ExtractCodeTVA (lib2);
			if (string.IsNullOrEmpty (code))
			{
				code = CrésusCompta.ExtractCodeTVA (lib1);
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

			//	Cherche le taux, dans le 2ème libellé puis en le calculant.
			decimal? taux = CrésusCompta.GetTaux (lib2);
			if (!taux.HasValue)
			{
				taux = CrésusCompta.GetTaux (écriture.Montant, suivante.Montant);
			}
			if (!taux.HasValue)
			{
				return false;
			}

			écriture.Type              = (int) TypeEcriture.BaseTVA;
			écriture.OrigineTVA        = (compteBase == écriture.Débit) ? "D" : "C";
			écriture.Libellé           = CrésusCompta.SimplifyLibellé (lib1);
			écriture.MontantComplément = suivante.Montant;
			écriture.CodeTVA           = codeTVA;
			écriture.TauxTVA           = taux;

			suivante.Type              = (int) TypeEcriture.CodeTVA;
			suivante.OrigineTVA        = (compteBase == écriture.Débit) ? "D" : "C";
			suivante.Libellé           = écriture.Libellé;
			suivante.MontantComplément = écriture.Montant;
			suivante.CodeTVA           = codeTVA;
			suivante.TauxTVA           = taux;

			if (encore != null)
			{
				string ending = string.Concat (" Total, (", codeTVA.Code, ")");
				var lib = encore.Libellé.ToString ();

				if (lib.EndsWith (ending))
				{
					encore.Libellé = lib.Substring (0, lib.Length-ending.Length);
				}
			}

			return true;
		}

		private static string ExtractCodeTVA(string libellé)
		{
			//	Extrait le code TVA d'un libellé.
			//	"Eau avril, 2.4% de TVA (IPIRED)"	-> IPIRED
			//	"Elec avril, (IPI) net"				-> IPI
			//	"Part TVA escompte (TVA)"			-> TVA
			if (string.IsNullOrEmpty (libellé))
			{
				return null;
			}

			int i1 = libellé.LastIndexOf ('(');
			if (i1 == -1)
			{
				return null;
			}

			int i2 = libellé.LastIndexOf (')');
			if (i2 == -1)
			{
				return null;
			}

			return libellé.Substring (i1+1, i2-i1-1);
		}

		private static decimal? GetTaux(decimal montantHT, decimal montantTVA)
		{
			//	Retourne le taux de TVA arrondi à une décimale.
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

		private static decimal? GetTaux(string libellé)
		{
			//	Retourne le taux de TVA contenu dans un libellé.
			//	"Eau avril, 2.4% de TVA (IPIRED)"	-> 0.024
			if (string.IsNullOrEmpty (libellé))
			{
				return null;
			}

			var words = libellé.Replace (",", " ").Split (' ');

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

		private static string SimplifyLibellé(string libellé)
		{
			//	Simplifie le libellé de base.
			//	"Elec mars, (IPI) net" -> "Elec mars"
			if (string.IsNullOrEmpty (libellé))
			{
				return null;
			}

			int i = libellé.LastIndexOf (',');
			if (i == -1)
			{
				return libellé;
			}

			return libellé.Substring (0, i);
		}
		#endregion


		private ComptaEntity			compta;
		private string[]				lines;
	}
}
