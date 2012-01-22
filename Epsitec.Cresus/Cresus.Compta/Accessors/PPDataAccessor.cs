//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Gère l'accès aux données des pertes et profits de la comptabilité.
	/// </summary>
	public class PPDataAccessor : AbstractDataAccessor
	{
		public PPDataAccessor(BusinessContext businessContext, ComptaEntity comptaEntity, MainWindowController mainWindowController)
			: base (businessContext, comptaEntity, mainWindowController)
		{
			this.options = this.mainWindowController.GetSettingsOptions<PPOptions> ("Présentation.PPOptions", this.comptaEntity);

			this.UpdateAfterOptionsChanged ();
		}


		public override void UpdateAfterOptionsChanged()
		{
			this.readonlyData.Clear ();
			this.MinMaxClear ();

			decimal totalGauche = 0;
			decimal totalDroite = 0;

			this.comptaEntity.PlanComptableUpdate (this.options.DateDébut, this.options.DateFin);

			foreach (var compte in this.comptaEntity.PlanComptable.Where (x => x.Catégorie == CatégorieDeCompte.Charge))
			{
				if (this.Options.Profondeur.HasValue && compte.Niveau >= this.Options.Profondeur.Value)
				{
					continue;
				}

				var solde = this.comptaEntity.GetSoldeCompte (compte);

				if (!this.Options.ComptesNuls && solde.GetValueOrDefault () == 0)
				{
					continue;
				}

				var data = new PPData ();
				this.readonlyData.Add (data);

				data.NuméroGauche = compte.Numéro;
				data.TitreGauche  = compte.Titre;
				data.NiveauGauche = compte.Niveau;

				if (this.HasSolde (compte))
				{
					data.SoldeGauche = solde;
					totalGauche += solde.GetValueOrDefault ();
					this.SetMinMaxValue (solde);
				}

				data.BudgetGauche = this.GetBudget (compte);
			}

			int rank = 0;
			foreach (var compte in this.comptaEntity.PlanComptable.Where (x => x.Catégorie == CatégorieDeCompte.Produit))
			{
				if (this.Options.Profondeur.HasValue && compte.Niveau >= this.Options.Profondeur.Value)
				{
					continue;
				}

				var solde = this.comptaEntity.GetSoldeCompte (compte);

				if (!this.Options.ComptesNuls && solde.GetValueOrDefault () == 0)
				{
					continue;
				}

				PPData data;

				if (rank >= this.readonlyData.Count)
				{
					data = new PPData ();
					this.readonlyData.Add (data);
				}
				else
				{
					data = this.readonlyData[rank] as PPData;
				}

				data.NuméroDroite = compte.Numéro;
				data.TitreDroite  = compte.Titre;
				data.NiveauDroite = compte.Niveau;

				if (this.HasSolde (compte))
				{
					data.SoldeDroite = solde;
					totalDroite += solde.GetValueOrDefault ();
					this.SetMinMaxValue (solde);
				}

				data.BudgetDroite = this.GetBudget (compte);

				rank++;
			}

			this.SetBottomSeparatorToPreviousLine ();

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

					this.SetMinMaxValue (data.SoldeGauche);
				}

				if (totalGauche > totalDroite)
				{
					data.TitreDroite = "Différence (perte)";
					data.SoldeDroite = totalGauche - totalDroite;
					data.IsBold      = true;

					totalDroite = totalGauche;

					this.SetMinMaxValue (data.SoldeDroite);
				}

				this.readonlyData.Add (data);
			}

			//	Dernière ligne
			{
				var data = new PPData ();

				data.SoldeGauche = totalGauche;
				data.SoldeDroite = totalDroite;
				data.IsBold      = true;

				this.readonlyData.Add (data);

				this.SetMinMaxValue (data.SoldeGauche);
				this.SetMinMaxValue (data.SoldeDroite);
			}
		}

		private bool HasSolde(ComptaCompteEntity compte)
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


		public override FormattedText GetText(int row, ColumnType column)
		{
			if (row < 0 || row >= this.Count)
			{
				return FormattedText.Null;
			}

			var data = this.readonlyData[row] as PPData;

			switch (column)
			{
				case ColumnType.NuméroGauche:
					return data.NuméroGauche;

				case ColumnType.TitreGauche:
					return data.TitreGauche;

				case ColumnType.SoldeGauche:
					return AbstractDataAccessor.GetMontant (data.SoldeGauche);

				case ColumnType.SoldeGraphiqueGauche:
					return this.GetMinMaxText (data.SoldeGauche);

				case ColumnType.BudgetGauche:
					return this.GetBudgetText (data.SoldeGauche, data.BudgetGauche);

				case ColumnType.NuméroDroite:
					return data.NuméroDroite;

				case ColumnType.TitreDroite:
					return data.TitreDroite;

				case ColumnType.SoldeDroite:
					return AbstractDataAccessor.GetMontant (data.SoldeDroite);

				case ColumnType.SoldeGraphiqueDroite:
					return this.GetMinMaxText (data.SoldeDroite);

				case ColumnType.BudgetDroite:
					return this.GetBudgetText (data.SoldeDroite, data.BudgetDroite);

				default:
					return FormattedText.Null;
			}
		}


		private static FormattedText GetNuméro(ComptaCompteEntity compte)
		{
			if (compte == null)
			{
				return JournalDataAccessor.multi;
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