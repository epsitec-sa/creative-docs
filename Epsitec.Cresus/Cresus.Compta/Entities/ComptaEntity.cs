//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Entities
{
	public partial class ComptaEntity
	{
		public override IEnumerable<FormattedText> GetFormattedEntityKeywords()
		{
			yield return TextFormatter.FormatText (this.BeginDate);
			yield return TextFormatter.FormatText (this.Name);
		}
		
		public override FormattedText GetCompactSummary()
		{
			return this.GetSummary ();
		}
		
		public override FormattedText GetSummary()
		{
			if (this.BeginDate.HasValue && this.EndDate.HasValue)
			{
				var b = this.BeginDate.Value;
				var e = this.EndDate.Value;

				if (b.Year == e.Year &&
					b.Day ==  1 && b.Month ==  1 &&
					e.Day == 31 && e.Month == 12)  // du 1.1 au 31.12 ?
				{
					return TextFormatter.FormatText (b.Year.ToString (), this.Name);
				}
			}

			return TextFormatter.FormatText (this.BeginDate, "—", this.EndDate, this.Name);
		}


		public int ProchainMultiId
		{
			get
			{
				if (this.Journal.Count == 0)
				{
					return 1;
				}
				else
				{
					return this.Journal.Max (x => x.MultiId) + 1;
				}
			}
		}

		public FormattedText ProchainePièce
		{
			//	A partir de "AB102", retourne "AB103" (par exemple).
			get
			{
				var pièce = this.DernièrePièce;

				if (!pièce.IsNullOrEmpty)
				{
					string p = pièce.ToSimpleText ();
					int i = pièce.Length-1;
					while (i >= 0)
					{
						if (p[i] >= '0' && p[i] <= '9')
						{
							i--;
						}
						else
						{
							break;
						}
					}

					if (i < pièce.Length-1)
					{
						int n;
						if (int.TryParse (p.Substring (i+1), out n))
						{
							return p.Substring (0, i+1) + (n+1).ToString ();
						}
					}
				}

				return pièce;
			}
		}

		public Date ProchaineDate
		{
			//	Retourne la date par défaut pour la prochaine écriture.
			get
			{
				if (this.DernièreDate.HasValue)
				{
					return this.DernièreDate.Value;
				}

				if (this.Journal.Count == 0)
				{
					if (this.BeginDate.HasValue)
					{
						return this.BeginDate.Value;
					}
					else
					{
						return new Date (System.DateTime.Now);
					}
				}
				else
				{
					return this.Journal.Last ().Date;
				}
			}
		}

		public bool ParseDate(FormattedText text, out Date? date)
		{
			//	Transforme un texte en une date valide pour la comptabilité.
			if (text.IsNullOrEmpty)
			{
				date = null;
				return true;
			}

			var brut = text.ToSimpleText ();
			brut = brut.Replace (".", " ");
			brut = brut.Replace (",", " ");
			brut = brut.Replace ("/", " ");
			brut = brut.Replace ("-", " ");
			brut = brut.Replace (":", " ");
			brut = brut.Replace (";", " ");
			brut = brut.Replace ("  ", " ");
			brut = brut.Replace ("  ", " ");

			var words = brut.Split (' ');
			var defaultDate = this.ProchaineDate;
			int day, month, year;

			int.TryParse (words[0], out day);

			if (words.Length <= 1 || !int.TryParse (words[1], out month))
			{
				month = defaultDate.Month;
			}

			if (words.Length <= 2 || !int.TryParse (words[2], out year))
			{
				year = defaultDate.Year;
			}

			try
			{
				date = new Date (year, month, day);
			}
			catch
			{
				date = defaultDate;
				return false;
			}

			if (this.BeginDate.HasValue && date < this.BeginDate.Value)
			{
				date = this.BeginDate.Value;
				return false;
			}

			if (this.EndDate.HasValue && date > this.EndDate.Value)
			{
				date = this.EndDate.Value;
				return false;
			}

			return true;
		}


		#region Niveaux d'imbrications
		public void UpdateNiveauCompte()
		{
			//	Met à jour le niveau d'imbrication (0..n) de tous les comptes.
			foreach (var compte in this.PlanComptable)
			{
				this.UpdateNiveauCompte (compte);
			}
		}

		private void UpdateNiveauCompte(ComptaCompteEntity compte)
		{
			//	Met à jour le niveau d'imbrication (0..n) d'un compte.
			var c = compte;
			int niveau = 0;

			while (c != null && c.Groupe != null && !c.Groupe.Numéro.IsNullOrEmpty)
			{
				c = this.PlanComptable.Where (x => x.Numéro == c.Groupe.Numéro).FirstOrDefault ();

				if (c == null)
				{
					break;
				}

				niveau++;
			}

			if (compte.Niveau != niveau)
			{
				compte.Niveau = niveau;
			}
		}
		#endregion


		#region Soldes des comptes
		public void PlanComptableUpdate(Date? dateDébut = null, Date? dateFin = null)
		{
			//	Met à jour tous les soldes des comptes, pour une période donnée.
			if (this.soldesCrédit == null)
			{
				this.soldesDébit  = new Dictionary<FormattedText, decimal?> ();
				this.soldesCrédit = new Dictionary<FormattedText, decimal?> ();
			}
			else
			{
				this.soldesDébit.Clear ();
				this.soldesCrédit.Clear ();
			}

			//	Génère une fois pour toutes le journal à prendre en compte.
			var journal = this.Journal.Where (x => ComptaEntity.Match (dateDébut, dateFin, x.Date));

			//	Cummule les totaux de tous les comptes finaux.
			foreach (var écriture in journal)
			{
				if (écriture.Débit != null && !écriture.Débit.Numéro.IsNullOrEmpty)
				{
					this.AddSoldeCompteDébit (écriture.Débit, écriture.Montant);
				}

				if (écriture.Crédit != null && !écriture.Crédit.Numéro.IsNullOrEmpty)
				{
					this.AddSoldeCompteCrédit (écriture.Crédit, écriture.Montant);
				}
			}

			//	Calcule les totaux des comptes de regroupement.
			foreach (var compte in this.PlanComptable.Where (x => x.Type == TypeDeCompte.Normal))
			{
				var soldeDébit  = this.GetSoldeCompteDébit (compte).GetValueOrDefault ();
				var soldeCrédit = this.GetSoldeCompteCrédit (compte).GetValueOrDefault ();

				var c = compte;
				while (c != null && c.Groupe != null && !c.Groupe.Numéro.IsNullOrEmpty)
				{
					c = c.Groupe;

					this.AddSoldeCompteDébit (c, soldeDébit);
					this.AddSoldeCompteCrédit (c, soldeCrédit);
				}
			}
		}

		private static bool Match(Date? dateDébut, Date? dateFin, Date? date)
		{
			if (date.HasValue)
			{
				if (dateDébut.HasValue && date.Value < dateDébut.Value)
				{
					return false;
				}

				if (dateFin.HasValue && date.Value > dateFin.Value)
				{
					return false;
				}
			}

			return true;
		}

		public decimal? GetSoldeCompte(ComptaCompteEntity compte)
		{
			//	Retourne le solde d'un compte (selon PlanComptableUpdate).
			if (compte.Type != TypeDeCompte.Normal &&
				compte.Type != TypeDeCompte.Groupe)
			{
				return null;
			}

			if (compte.Catégorie == CatégorieDeCompte.Passif ||
				compte.Catégorie == CatégorieDeCompte.Produit)
			{
				return this.GetSoldeCompteCrédit (compte).GetValueOrDefault () - this.GetSoldeCompteDébit (compte).GetValueOrDefault ();
			}
			else
			{
				return this.GetSoldeCompteDébit (compte).GetValueOrDefault () - this.GetSoldeCompteCrédit (compte).GetValueOrDefault ();
			}
		}

		public decimal? GetSoldeCompteDébit(ComptaCompteEntity compte)
		{
			//	Retourne le solde d'un compte au débit (selon PlanComptableUpdate).
			decimal? solde;
			if (this.soldesDébit != null && this.soldesDébit.TryGetValue (compte.Numéro, out solde))
			{
				return solde;
			}
			else
			{
				return null;
			}
		}

		public decimal? GetSoldeCompteCrédit(ComptaCompteEntity compte)
		{
			//	Retourne le solde d'un compte au crédit (selon PlanComptableUpdate).
			decimal? solde;
			if (this.soldesCrédit != null && this.soldesCrédit.TryGetValue (compte.Numéro, out solde))
			{
				return solde;
			}
			else
			{
				return null;
			}
		}

		private void AddSoldeCompteDébit(ComptaCompteEntity compte, decimal montant)
		{
			decimal? solde;
			if (!this.soldesDébit.TryGetValue (compte.Numéro, out solde))
			{
				solde = 0;
			}

			solde += montant;

			this.soldesDébit[compte.Numéro] = solde;
		}

		private void AddSoldeCompteCrédit(ComptaCompteEntity compte, decimal montant)
		{
			decimal? solde;
			if (!this.soldesCrédit.TryGetValue (compte.Numéro, out solde))
			{
				solde = 0;
			}

			solde += montant;

			this.soldesCrédit[compte.Numéro] = solde;
		}
		#endregion


		public int GetJournalCount(ComptaJournalEntity journal)
		{
			//	Retourne le nombre d'écritures d'un journal.
			if (journal == null)  // tous les journaux ?
			{
				return this.Journal.Count ();
			}
			else
			{
				return this.Journal.Where (x => x.Journal == journal).Count ();
			}
		}

		public string GetJournalSummary(ComptaJournalEntity journal)
		{
			//	Retourne le résumé d'un journal d'écritures.
			IEnumerable<ComptaEcritureEntity> écritures;

			if (journal == null)  // tous les journaux ?
			{
				écritures = this.Journal;
			}
			else
			{
				écritures = this.Journal.Where (x => x.Journal == journal);
			}

			int count = écritures.Count();

			if (count == 0)
			{
				return "Aucune écriture";
			}
			else if (count == 1)
			{
				var date = écritures.First ().Date.ToString ();
				return string.Format ("1 écriture, le {0}", date);
			}
			else
			{
				var beginDate = écritures.First ().Date;
				var endDate   = écritures.Last  ().Date;

				if (beginDate == endDate)
				{
					return string.Format ("{0} écritures, le {1}", count.ToString (), beginDate.ToString ());
				}
				else
				{
					return string.Format ("{0} écritures, du {1} au {2}", count.ToString (), beginDate.ToString (), endDate.ToString ());
				}
			}
		}


		private Dictionary<FormattedText, decimal?> soldesDébit;
		private Dictionary<FormattedText, decimal?> soldesCrédit;
	}
}
