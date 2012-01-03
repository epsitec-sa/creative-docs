﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Business.Finance.Comptabilité;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ComptabilitéControllers
{
	/// <summary>
	/// Gère l'accès aux données des pertes et profits de la comptabilité.
	/// </summary>
	public class PPAccessor : AbstractDataAccessor<PPColumn, PPData>
	{
		public PPAccessor(ComptabilitéEntity comptabilitéEntity)
			: base (comptabilitéEntity)
		{
			this.options = new PPOptions (this.comptabilitéEntity);

			this.UpdateSortedList ();
		}


		public override void UpdateSortedList()
		{
			this.sortedEntities = new List<PPData> ();

			decimal totalGauche = 0;
			decimal totalDroite = 0;

			foreach (var compte in this.comptabilitéEntity.PlanComptable.Where (x => x.Catégorie == CatégorieDeCompte.Charge).OrderBy (x => x.Numéro))
			{
				if (this.Options.Profondeur.HasValue && compte.Niveau >= this.Options.Profondeur.Value)
				{
					continue;
				}

				var solde = this.comptabilitéEntity.GetSoldeCompte (compte, this.options.DateDébut, this.options.DateFin);

				if (!this.Options.ComptesNuls && solde.GetValueOrDefault () == 0)
				{
					continue;
				}

				var data = new PPData ();
				this.SortedList.Add (data);

				data.NuméroGauche = compte.Numéro;
				data.TitreGauche  = compte.Titre;
				data.NiveauGauche = compte.Niveau;

				if (this.HasSolde (compte))
				{
					data.SoldeGauche = solde;
					totalGauche += solde.GetValueOrDefault ();
				}
			}

			int rank = 0;
			foreach (var compte in this.comptabilitéEntity.PlanComptable.Where (x => x.Catégorie == CatégorieDeCompte.Produit).OrderBy (x => x.Numéro))
			{
				if (this.Options.Profondeur.HasValue && compte.Niveau >= this.Options.Profondeur.Value)
				{
					continue;
				}

				var solde = this.comptabilitéEntity.GetSoldeCompte (compte, this.options.DateDébut, this.options.DateFin);

				if (!this.Options.ComptesNuls && solde.GetValueOrDefault () == 0)
				{
					continue;
				}

				PPData data;

				if (rank >= this.SortedList.Count)
				{
					data = new PPData ();
					this.SortedList.Add (data);
				}
				else
				{
					data = this.SortedList[rank];
				}

				data.NuméroDroite = compte.Numéro;
				data.TitreDroite  = compte.Titre;
				data.NiveauDroite = compte.Niveau;

				if (this.HasSolde (compte))
				{
					data.SoldeDroite = solde;
					totalDroite += solde.GetValueOrDefault ();
				}

				rank++;
			}

			//	Avant-dernière ligne.
			if (totalGauche != totalDroite)
			{
				var data = new PPData ();

				if (totalGauche < totalDroite)
				{
					data.TitreGauche = "Différence (bénéfice)";
					data.SoldeGauche = totalDroite - totalGauche;
					data.IsBold      = true;

					totalGauche = totalDroite;
				}

				if (totalGauche > totalDroite)
				{
					data.TitreDroite = "Différence (perte)";
					data.SoldeDroite = totalGauche - totalDroite;
					data.IsBold      = true;

					totalDroite = totalGauche;
				}

				this.SortedList.Add (data);
			}

			//	Dernière ligne
			{
				var data = new PPData ();

				data.SoldeGauche = totalGauche;
				data.SoldeDroite = totalDroite;
				data.IsBold      = true;

				this.SortedList.Add (data);
			}
		}

		private bool HasSolde(ComptabilitéCompteEntity compte)
		{
			//	Indique si le solde du compte doit figurer dans le tableau.
			//	Si la profondeur n'est pas spécifiée, on accepte tous les comptes normaux.
			//	Si la profondeur est spécifiée, on accepte les comptes qui ont exactement cette profondeur.
			if (compte.Type == TypeDeCompte.Normal)
			{
				return true;
			}

			if (this.Options.Profondeur.HasValue && compte.Niveau+1 == this.Options.Profondeur.Value)
			{
				return true;
			}

			return false;
		}


		public override FormattedText GetText(int row, PPColumn column)
		{
			if (row < 0 || row >= this.Count)
			{
				return FormattedText.Null;
			}

			var data = this.sortedEntities[row];

			switch (column)
			{
				case PPColumn.NuméroGauche:
					return data.NuméroGauche;

				case PPColumn.TitreGauche:
					return data.TitreGauche;

				case PPColumn.SoldeGauche:
					if (data.SoldeGauche.HasValue)
					{
						return data.SoldeGauche.Value.ToString ("0.00");
					}
					else
					{
						return FormattedText.Empty;
					}

				case PPColumn.NuméroDroite:
					return data.NuméroDroite;

				case PPColumn.TitreDroite:
					return data.TitreDroite;

				case PPColumn.SoldeDroite:
					if (data.SoldeDroite.HasValue)
					{
						return data.SoldeDroite.Value.ToString ("0.00");
					}
					else
					{
						return FormattedText.Empty;
					}

				default:
					return FormattedText.Null;
			}
		}


		private static FormattedText GetNuméro(ComptabilitéCompteEntity compte)
		{
			if (compte == null)
			{
				return JournalAccessor.multi;
			}
			else
			{
				return compte.Numéro;
			}
		}

		private PPOptions Options
		{
			get
			{
				return this.options as PPOptions;
			}
		}
	}
}