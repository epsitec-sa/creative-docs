//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Graph;
using Epsitec.Cresus.Compta.Options.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Gère l'accès aux données doubles (bilan avec actif/passif ou PP avec charge/produit) de la comptabilité.
	/// </summary>
	public abstract class DoubleDataAccessor : AbstractDataAccessor
	{
		public DoubleDataAccessor(AbstractController controller)
			: base (controller)
		{
		}


		public override void UpdateFilter()
		{
			this.UpdateReadonlyAllData ();
		}

		public override void UpdateAfterOptionsChanged()
		{
			this.UpdateReadonlyAllData ();

			base.UpdateAfterOptionsChanged ();
		}

		private void UpdateReadonlyAllData()
		{
			this.readonlyAllData.Clear ();

			this.filterData.GetBeginnerDates (out this.lastBeginDate, out this.lastEndDate);
			this.soldesJournalManager.Initialize (this.période.Journal, this.lastBeginDate, this.lastEndDate);

			this.budgetsManager = new BudgetsManager (this.compta, this.période, this.options, this.lastBeginDate, this.lastEndDate);

			//	Partie "gauche" (actif ou charge).
			int lignesGauches = this.readonlyAllData.Count ();
			decimal totalGauche = this.UpdateReadonlyAllData (this.CatégorieGauche, true);
			lignesGauches = this.readonlyAllData.Count () - lignesGauches;

			//	Partie "droite" (passif ou produit).
			int lignesDroites = this.readonlyAllData.Count ();
			decimal totalDroite = this.UpdateReadonlyAllData (this.CatégorieDroite, false);
			lignesDroites = this.readonlyAllData.Count () - lignesDroites;

			//	Ligne de résumé.
			if (totalGauche != totalDroite)
			{
				var data = new DoubleData ();
				data.NeverFiltered = true;
				data.IsItalic      = true;

				if (totalGauche < totalDroite)
				{
					data.Gauche = true;
					data.Titre  = this.DifferenceGaucheDescription;
					data.Solde  = totalDroite - totalGauche;

					this.readonlyAllData.Insert (lignesGauches, data);
					lignesGauches++;
				}

				if (totalGauche > totalDroite)
				{
					data.Gauche = false;
					data.Titre  = this.DifferenceDroiteDescription;
					data.Solde  = totalGauche - totalDroite;

					this.readonlyAllData.Add (data);
					lignesDroites++;
				}
			}

			base.UpdateFilter ();
			this.UpdateTypo ();
		}

		private decimal UpdateReadonlyAllData(CatégorieDeCompte catégorie, bool gauche)
		{
			//	Génère les lignes de la partie "gauche" ou "droite".
			//	Retourne le montant total.
			int fromProfondeur, toProfondeur;
			this.filterData.GetBeginnerProfondeurs (out fromProfondeur, out toProfondeur);

			decimal total = 0;

			foreach (var compte in this.compta.PlanComptable.Where (x => x.Catégorie == catégorie))
			{
				var solde = this.soldesJournalManager.GetSolde (compte).GetValueOrDefault ();

				var data = new DoubleData ();
				this.readonlyAllData.Add (data);

				data.Entity = compte;
				data.Gauche = gauche;
				data.Numéro = compte.Numéro;
				data.Titre  = compte.Titre;
				data.Niveau = compte.Niveau;
				data.Solde  = solde;

				data.PériodePrécédente  = this.GetBudget (compte, ComparisonShowed.PériodePrécédente);
				data.PériodePénultième  = this.GetBudget (compte, ComparisonShowed.PériodePénultième);
				data.Budget             = this.GetBudget (compte, ComparisonShowed.Budget);
				data.BudgetProrata      = this.GetBudget (compte, ComparisonShowed.BudgetProrata);
				data.BudgetFutur        = this.GetBudget (compte, ComparisonShowed.BudgetFutur);
				data.BudgetFuturProrata = this.GetBudget (compte, ComparisonShowed.BudgetFuturProrata);

				total += solde;
			}

			return total;
		}

		private void UpdateTypo()
		{
			DoubleData lastData = null;

			foreach (var d in this.readonlyData)
			{
				var data = d as DoubleData;

				if (lastData != null)
				{
					lastData.HasBottomSeparator = (lastData.Gauche != data.Gauche);
				}

				data.IsBold = !data.NeverFiltered && (lastData == null || lastData.Gauche != data.Gauche);

				lastData = data;
			}

			if (this.readonlyData.Any ())
			{
				this.readonlyData.Last ().HasBottomSeparator = true;
			}
		}


		public override void UpdateGraphData(bool force)
		{
			//	Appelé après la mise à jour du filtre, pour mettre à jour les données graphiques.
			if (!force && !this.Options.HasGraphics && !this.options.ViewGraph)
			{
				return;
			}

			this.cube.Dimensions = 2;
			this.cube.SetDimensionTitle (0, "Types de montants");
			this.cube.SetDimensionTitle (1, "Comptes");
			this.cube.Clear ();

			//	Spécifie les légendes de l'axe X.
			int x = 0;
			this.cube.SetShortTitle (0, x++, "Solde");

			if (this.Options.ComparisonEnable)
			{
				if ((this.Options.ComparisonShowed & ComparisonShowed.PériodePénultième) != 0)
				{
					this.cube.SetShortTitle (0, x++, "Période pénultième");
				}

				if ((this.Options.ComparisonShowed & ComparisonShowed.PériodePrécédente) != 0)
				{
					this.cube.SetShortTitle (0, x++, "Période précédente");
				}

				if ((this.Options.ComparisonShowed & ComparisonShowed.Budget) != 0)
				{
					this.cube.SetShortTitle (0, x++, "Budget");
				}

				if ((this.Options.ComparisonShowed & ComparisonShowed.BudgetProrata) != 0)
				{
					this.cube.SetShortTitle (0, x++, "Budget prorata");
				}

				if ((this.Options.ComparisonShowed & ComparisonShowed.BudgetFutur) != 0)
				{
					this.cube.SetShortTitle (0, x++, "Budget futur");
				}

				if ((this.Options.ComparisonShowed & ComparisonShowed.BudgetFuturProrata) != 0)
				{
					this.cube.SetShortTitle (0, x++, "Budget futur prorata");
				}
			}

			int y = 0;
			foreach (var d in this.readonlyData)
			{
				var data = d as DoubleData;

				//	Spécifie la légende de l'axe Y.
				this.cube.SetShortTitle (1, y, data.Numéro);
				this.cube.SetFullTitle (1, y, FormattedText.Concat (data.Numéro, " ", data.Titre));

				x = 0;
				this.cube.SetValue (x++, y, data.Solde);

				if (this.Options.ComparisonEnable)
				{
					if ((this.Options.ComparisonShowed & ComparisonShowed.PériodePénultième) != 0)
					{
						this.cube.SetValue (x++, y, data.PériodePénultième);
					}

					if ((this.Options.ComparisonShowed & ComparisonShowed.PériodePrécédente) != 0)
					{
						this.cube.SetValue (x++, y, data.PériodePrécédente);
					}

					if ((this.Options.ComparisonShowed & ComparisonShowed.Budget) != 0)
					{
						this.cube.SetValue (x++, y, data.Budget);
					}

					if ((this.Options.ComparisonShowed & ComparisonShowed.BudgetProrata) != 0)
					{
						this.cube.SetValue (x++, y, data.BudgetProrata);
					}

					if ((this.Options.ComparisonShowed & ComparisonShowed.BudgetFutur) != 0)
					{
						this.cube.SetValue (x++, y, data.BudgetFutur);
					}

					if ((this.Options.ComparisonShowed & ComparisonShowed.BudgetFuturProrata) != 0)
					{
						this.cube.SetValue (x++, y, data.BudgetFuturProrata);
					}
				}

				y++;
			}

			this.UpdateGraphDataToDraw ();
		}


		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var data = this.GetReadOnlyData (row, all) as DoubleData;

			if (data == null)
			{
				return FormattedText.Null;
			}

			var compte = data.Entity as ComptaCompteEntity;

			ComptaMonnaieEntity monnaie = null;

			if (compte == null)
			{
				monnaie = this.compta.Monnaies[0];
			}
			else
			{
				monnaie = compte.Monnaie;
			}

			switch (column)
			{
				case ColumnType.Numéro:
					return data.Numéro;

				case ColumnType.Titre:
					return data.Titre;

				case ColumnType.Solde:
					return Converters.MontantToString (data.Solde, monnaie);

				case ColumnType.SoldeGraphique:
					return AbstractDataAccessor.GetGraphicText (row);

				case ColumnType.Budget:
					return this.GetBudgetText (data.Numéro, data.Solde, data.Budget, monnaie);

				case ColumnType.BudgetProrata:
					return this.GetBudgetText (data.Numéro, data.Solde, data.BudgetProrata, monnaie);

				case ColumnType.BudgetFutur:
					return this.GetBudgetText (data.Numéro, data.Solde, data.BudgetFutur, monnaie);

				case ColumnType.BudgetFuturProrata:
					return this.GetBudgetText (data.Numéro, data.Solde, data.BudgetFuturProrata, monnaie);

				case ColumnType.PériodePrécédente:
					return this.GetBudgetText (data.Numéro, data.Solde, data.PériodePrécédente, monnaie);

				case ColumnType.PériodePénultième:
					return this.GetBudgetText (data.Numéro, data.Solde, data.PériodePénultième, monnaie);

				case ColumnType.Profondeur:
					return (data.Niveau+1).ToString ();

				default:
					return FormattedText.Null;
			}
		}


		private decimal? GetBudget(ComptaCompteEntity compte, ComparisonShowed type)
		{
			//	Retourne le montant d'un compte à considérer pour la colonne "budget".
			return this.budgetsManager.GetBudget (compte, type);
		}

		private FormattedText GetBudgetText(FormattedText name, decimal? solde, decimal? budget, ComptaMonnaieEntity monnaie)
		{
			return this.budgetsManager.GetBudgetText (name, solde, budget, this.minValue, this.maxValue, monnaie);
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

		private new DoubleOptions Options
		{
			get
			{
				return this.options as DoubleOptions;
			}
		}


		#region Virtual properties
		protected virtual CatégorieDeCompte CatégorieGauche
		{
			get
			{
				return CatégorieDeCompte.Inconnu;
			}
		}

		protected virtual CatégorieDeCompte CatégorieDroite
		{
			get
			{
				return CatégorieDeCompte.Inconnu;
			}
		}


		protected virtual FormattedText DifferenceGaucheDescription
		{
			get
			{
				return FormattedText.Empty;
			}
		}

		protected virtual FormattedText DifferenceDroiteDescription
		{
			get
			{
				return FormattedText.Empty;
			}
		}
		#endregion


		private BudgetsManager					budgetsManager;
	}
}