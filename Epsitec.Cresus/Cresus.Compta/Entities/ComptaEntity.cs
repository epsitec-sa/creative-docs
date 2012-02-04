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
		public FormattedText JournalRésumé(ComptaJournalEntity journal)
		{
			//	Retourne le résumé d'un journal.
			int totalEcritures = 0;
			int totalPériodes = 0;

			foreach (var période in this.Périodes)
			{
				int total = période.Journal.Where (x => x.Journal == journal).Count ();

				if (total != 0)
				{
					totalEcritures += total;
					totalPériodes++;
				}
			}

			string écrituresRésumé, périodesRésumé;

			if (totalEcritures == 0)
			{
				return "Vide";
			}
			else if (totalEcritures == 1)
			{
				écrituresRésumé = "1 écriture";
			}
			else
			{
				écrituresRésumé = string.Format ("{0} écritures", totalEcritures.ToString ());
			}

			if (totalPériodes == 0)
			{
				périodesRésumé = "aucune période";
			}
			else if (totalPériodes == 1)
			{
				périodesRésumé = "1 période";
			}
			else
			{
				périodesRésumé = string.Format ("{0} périodes", totalPériodes.ToString ());
			}

			return écrituresRésumé + " dans " + périodesRésumé;
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
		public void PlanComptableUpdate(ComptaPériodeEntity période, Date? dateDébut = null, Date? dateFin = null)
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
			var journal = période.Journal.Where (x => ComptaEntity.Match (dateDébut, dateFin, x.Date));

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


		private Dictionary<FormattedText, decimal?> soldesDébit;
		private Dictionary<FormattedText, decimal?> soldesCrédit;
	}
}
