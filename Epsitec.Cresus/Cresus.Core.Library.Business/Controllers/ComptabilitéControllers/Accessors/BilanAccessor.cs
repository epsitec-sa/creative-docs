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
	/// Gère l'accès aux données du bilan de la comptabilité.
	/// </summary>
	public class BilanAccessor : AbstractDataAccessor<BilanColumn, BilanData>
	{
		public BilanAccessor(ComptabilitéEntity comptabilitéEntity)
			: base (comptabilitéEntity)
		{
			this.options = new BilanOptions (this.comptabilitéEntity);

			this.UpdateSortedList ();
		}


		public override void UpdateSortedList()
		{
			this.sortedEntities = new List<BilanData> ();

			decimal totalGauche = 0;
			decimal totalDroite = 0;

			foreach (var compte in this.comptabilitéEntity.PlanComptable.Where (x => x.Catégorie == CatégorieDeCompte.Actif).OrderBy (x => x.Numéro))
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

				var data = new BilanData ();
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
			foreach (var compte in this.comptabilitéEntity.PlanComptable.Where (x => x.Catégorie == CatégorieDeCompte.Passif).OrderBy (x => x.Numéro))
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

				BilanData data;

				if (rank >= this.SortedList.Count)
				{
					data = new BilanData ();
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
				var data = new BilanData ();

				if (totalGauche < totalDroite)
				{
					data.TitreGauche = "Différence (découvert)";
					data.SoldeGauche = totalDroite - totalGauche;
					data.IsBold      = true;

					totalGauche = totalDroite;
				}

				if (totalGauche > totalDroite)
				{
					data.TitreDroite = "Différence (capital)";
					data.SoldeDroite = totalGauche - totalDroite;
					data.IsBold      = true;

					totalDroite = totalGauche;
				}

				this.SortedList.Add (data);
			}

			//	Dernière ligne
			{
				var data = new BilanData ();

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


		public override FormattedText GetText(int row, BilanColumn column)
		{
			if (row < 0 || row >= this.Count)
			{
				return FormattedText.Null;
			}

			var data = this.sortedEntities[row];

			switch (column)
			{
				case BilanColumn.NuméroGauche:
					return data.NuméroGauche;

				case BilanColumn.TitreGauche:
					return data.TitreGauche;

				case BilanColumn.SoldeGauche:
					if (data.SoldeGauche.HasValue)
					{
						return data.SoldeGauche.Value.ToString ("0.00");
					}
					else
					{
						return FormattedText.Empty;
					}

				case BilanColumn.NuméroDroite:
					return data.NuméroDroite;

				case BilanColumn.TitreDroite:
					return data.TitreDroite;

				case BilanColumn.SoldeDroite:
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

		private BilanOptions Options
		{
			get
			{
				return this.options as BilanOptions;
			}
		}
	}
}