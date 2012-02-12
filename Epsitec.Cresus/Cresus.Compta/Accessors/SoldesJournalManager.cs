//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Cette classe gère les soldes des comptes pour un journal et une période donnée.
	/// </summary>
	public class SoldesJournalManager
	{
		public SoldesJournalManager(ComptaEntity compta)
		{
			this.compta = compta;

			this.soldesDébit  = new Dictionary<ComptaCompteEntity, decimal> ();
			this.soldesCrédit = new Dictionary<ComptaCompteEntity, decimal> ();
		}


		public void Initialize(IEnumerable<ComptaEcritureEntity> journal, Date? dateDébut = null, Date? dateFin = null)
		{
			this.journal   = journal;
			this.dateDébut = dateDébut;
			this.dateFin   = dateFin;

			this.UpdateSoldes ();
		}


#if false
		public void AddEcriture(ComptaEcritureEntity écriture)
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

		public void SubEcriture(ComptaEcritureEntity écriture)
		{
			if (écriture.Débit != null && !écriture.Débit.Numéro.IsNullOrEmpty)
			{
				this.AddSoldeCompteDébit (écriture.Débit, -écriture.Montant);
			}

			if (écriture.Crédit != null && !écriture.Crédit.Numéro.IsNullOrEmpty)
			{
				this.AddSoldeCompteCrédit (écriture.Crédit, -écriture.Montant);
			}
		}
#endif


		public decimal? GetSolde(ComptaCompteEntity compte)
		{
			if (this.journal == null)
			{
				return null;
			}

			if (compte.Type != TypeDeCompte.Normal &&
				compte.Type != TypeDeCompte.Groupe)
			{
				return null;
			}

			if (compte.Catégorie == CatégorieDeCompte.Passif ||
				compte.Catégorie == CatégorieDeCompte.Produit)
			{
				return this.GetSoldeCrédit (compte).GetValueOrDefault () - this.GetSoldeDébit (compte).GetValueOrDefault ();
			}
			else
			{
				return this.GetSoldeDébit (compte).GetValueOrDefault () - this.GetSoldeCrédit (compte).GetValueOrDefault ();
			}
		}

		public decimal? GetSoldeDébit(ComptaCompteEntity compte)
		{
			if (this.journal == null)
			{
				return null;
			}

			decimal solde;
			if (this.soldesDébit.TryGetValue (compte, out solde))
			{
				return solde;
			}

			return null;
		}

		public decimal? GetSoldeCrédit(ComptaCompteEntity compte)
		{
			if (this.journal == null)
			{
				return null;
			}

			decimal solde;
			if (this.soldesCrédit.TryGetValue (compte, out solde))
			{
				return solde;
			}

			return null;
		}


		private void UpdateSoldes()
		{
			//	Met à jour tous les totaux, pour le journal et la période choisie.
			this.soldesDébit.Clear ();
			this.soldesCrédit.Clear ();

			if (this.journal == null)
			{
				return;
			}

			//	Génère une fois pour toutes le journal à prendre en compte.
			IEnumerable<ComptaEcritureEntity> journal;

			if (this.dateDébut.HasValue || this.dateFin.HasValue)
			{
				journal = this.journal.Where (x => this.DatesMatch (x.Date));
			}
			else
			{
				journal = this.journal;
			}

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
			foreach (var compte in this.compta.PlanComptable.Where (x => x.Type == TypeDeCompte.Normal))
			{
				var soldeDébit  = this.GetSoldeDébit  (compte).GetValueOrDefault ();
				var soldeCrédit = this.GetSoldeCrédit (compte).GetValueOrDefault ();

				var c = compte;
				while (c != null && c.Groupe != null && !c.Groupe.Numéro.IsNullOrEmpty)
				{
					c = c.Groupe;

					this.AddSoldeCompteDébit  (c, soldeDébit);
					this.AddSoldeCompteCrédit (c, soldeCrédit);
				}
			}
		}

		private bool DatesMatch(Date date)
		{
			if (this.dateDébut.HasValue && date < this.dateDébut.Value)
			{
				return false;
			}

			if (this.dateFin.HasValue && date > this.dateFin.Value)
			{
				return false;
			}

			return true;
		}


		private void AddSoldeCompteDébit(ComptaCompteEntity compte, decimal montant)
		{
			decimal solde;
			if (!this.soldesDébit.TryGetValue (compte, out solde))
			{
				solde = 0;
			}

			this.soldesDébit[compte] = solde + montant;
		}

		private void AddSoldeCompteCrédit(ComptaCompteEntity compte, decimal montant)
		{
			decimal solde;
			if (!this.soldesCrédit.TryGetValue (compte, out solde))
			{
				solde = 0;
			}

			this.soldesCrédit[compte] = solde + montant;
		}


		private readonly ComptaEntity								compta;
		private readonly Dictionary<ComptaCompteEntity, decimal>	soldesDébit;
		private readonly Dictionary<ComptaCompteEntity, decimal>	soldesCrédit;

		private IEnumerable<ComptaEcritureEntity>					journal;
		private Date?												dateDébut;
		private Date?												dateFin;
	}
}
