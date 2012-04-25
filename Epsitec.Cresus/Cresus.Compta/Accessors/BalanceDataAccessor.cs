//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Graph;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Options.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Gère l'accès aux données de la balance de vérification de la comptabilité.
	/// </summary>
	public class BalanceDataAccessor : AbstractDataAccessor
	{
		public BalanceDataAccessor(AbstractController controller)
			: base (controller)
		{
			this.options    = this.mainWindowController.GetSettingsOptions<BalanceOptions> ("Présentation.Balance.Options", this.compta);
			this.searchData = this.mainWindowController.GetSettingsSearchData ("Présentation.Balance.Search");
			this.filterData = this.mainWindowController.GetSettingsSearchData ("Présentation.Balance.Filter");
			//?this.filterData = this.mainWindowController.GetSettingsSearchData<SearchData> ("Présentation.Balance.Filter", this.FilterInitialize);

			this.UpdateAfterOptionsChanged ();
		}

		private void FilterInitialize(SearchData data)
		{
			data.FirstTabData.Column              = ColumnType.Solde;
			data.FirstTabData.SearchText.Mode     = SearchMode.WholeContent;
			data.FirstTabData.SearchText.Invert   = true;
			data.FirstTabData.SearchText.FromText = Converters.MontantToString (0, this.compta.Monnaies[0]);
		}


		public override void UpdateFilter()
		{
			Date? beginDate, endDate;
			this.filterData.GetBeginnerDates (out beginDate, out endDate);

			if (this.lastBeginDate != beginDate || this.lastEndDate != endDate)
			{
				this.UpdateAfterOptionsChanged ();
			}

			base.UpdateFilter ();
			this.UpdateTypo ();
		}

		public override void UpdateAfterOptionsChanged()
		{
			this.readonlyAllData.Clear ();

			decimal totalDébit  = 0;
			decimal totalCrédit = 0;
			decimal totalSoldeD = 0;
			decimal totalSoldeC = 0;

			this.filterData.GetBeginnerDates (out this.lastBeginDate, out this.lastEndDate);
			this.soldesJournalManager.Initialize (this.période.Journal, this.lastBeginDate, this.lastEndDate);

			this.budgetsManager = new BudgetsManager (this.compta, this.période, this.options, this.lastBeginDate, this.lastEndDate);

			foreach (var compte in this.compta.PlanComptable)
			{
				if (compte.Catégorie == CatégorieDeCompte.Inconnu)
				{
					continue;
				}

				var soldeDébit  = this.soldesJournalManager.GetSoldeDébit  (compte).GetValueOrDefault ();
				var soldeCrédit = this.soldesJournalManager.GetSoldeCrédit (compte).GetValueOrDefault ();
				var différence = soldeCrédit - soldeDébit;

				var data = new BalanceData
				{
					Entity             = compte,
					Numéro             = compte.Numéro,
					Titre              = compte.Titre,
					Catégorie          = compte.Catégorie,
					Type               = compte.Type,
					Niveau             = compte.Niveau,
					Débit              = soldeDébit,
					Crédit             = soldeCrédit,

					PériodePrécédente  = this.GetBudget (compte, ComparisonShowed.PériodePrécédente),
					PériodePénultième  = this.GetBudget (compte, ComparisonShowed.PériodePénultième),
					Budget             = this.GetBudget (compte, ComparisonShowed.Budget),
					BudgetProrata      = this.GetBudget (compte, ComparisonShowed.BudgetProrata),
					BudgetFutur        = this.GetBudget (compte, ComparisonShowed.BudgetFutur),
					BudgetFuturProrata = this.GetBudget (compte, ComparisonShowed.BudgetFuturProrata),
				};

				if (différence < 0)
				{
					data.SoldeDébit = -différence;
				}
				if (différence > 0)
				{
					data.SoldeCrédit = différence;
				}

				if (compte.Type == TypeDeCompte.Normal ||
					compte.Type == TypeDeCompte.TVA    )
				{
					totalDébit  += soldeDébit;
					totalCrédit += soldeCrédit;

					if (différence < 0)
					{
						totalSoldeD -= différence;
					}
					if (différence > 0)
					{
						totalSoldeC += différence;
					}
				}

				this.readonlyAllData.Add (data);
			}

			this.SetBottomSeparatorToPreviousLine ();

			// Génère la dernière ligne.
			{
				var data = new BalanceData ();

				data.Titre         = "Mouvement";
				data.Débit         = totalDébit;
				data.Crédit        = totalCrédit;
				data.SoldeDébit    = totalSoldeD;
				data.SoldeCrédit   = totalSoldeC;
				data.IsItalic      = true;
				data.NeverFiltered = true;

				this.readonlyAllData.Add (data);
			}

			this.UpdateFilter ();
		}

		private void UpdateTypo()
		{
			BalanceData lastData = null;

			foreach (var d in this.readonlyData)
			{
				var data = d as BalanceData;

				if (lastData != null)
				{
					lastData.HasBottomSeparator = (lastData.Catégorie != data.Catégorie);
				}

				data.IsBold = !data.NeverFiltered && (lastData == null || lastData.Catégorie != data.Catégorie);

				lastData = data;
			}

			if (this.readonlyData.Any ())
			{
				this.readonlyData.Last ().HasBottomSeparator = true;
			}
		}


		protected override void UpdateAfterFilterUpdated()
		{
			//	Appelé après la mise à jour du filtre, pour mettre à jour les données graphiques.
			if (!this.Options.HasGraphics)
			{
				return;
			}

			this.cube.Dimensions = 2;
			this.cube.Clear ();
			this.graphOptions.Mode = GraphMode.SideBySide;

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
				var data = d as BalanceData;

				if (data.NeverFiltered)
				{
					continue;
				}

				//	Spécifie la légende de l'axe Y.
				this.cube.SetShortTitle (1, y, data.Numéro);
				this.cube.SetFullTitle (1, y, FormattedText.Concat(data.Numéro, " ", data.Titre));

				x = 0;
				if (data.SoldeDébit.HasValue)
				{
					this.cube.SetValue (x++, y, data.SoldeDébit);
				}
				else
				{
					this.cube.SetValue (x++, y, data.SoldeCrédit);
				}

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
		}


		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var data = this.GetReadOnlyData (row, all) as BalanceData;

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

				case ColumnType.Catégorie:
					return Converters.CatégorieToString (data.Catégorie);

				case ColumnType.Type:
					return Converters.TypeToString (data.Type);

				case ColumnType.Débit:
					return Converters.MontantToString (data.Débit, monnaie);

				case ColumnType.Crédit:
					return Converters.MontantToString (data.Crédit, monnaie);

				case ColumnType.SoldeDébit:
					return Converters.MontantToString (data.SoldeDébit, monnaie);

				case ColumnType.SoldeCrédit:
					return Converters.MontantToString (data.SoldeCrédit, monnaie);

				case ColumnType.Budget:
					return this.GetBudgetText (data.Numéro, data.SoldeDébit, data.SoldeCrédit, data.Budget, monnaie);

				case ColumnType.BudgetProrata:
					return this.GetBudgetText (data.Numéro, data.SoldeDébit, data.SoldeCrédit, data.BudgetProrata, monnaie);

				case ColumnType.BudgetFutur:
					return this.GetBudgetText (data.Numéro, data.SoldeDébit, data.SoldeCrédit, data.BudgetFutur, monnaie);

				case ColumnType.BudgetFuturProrata:
					return this.GetBudgetText (data.Numéro, data.SoldeDébit, data.SoldeCrédit, data.BudgetFuturProrata, monnaie);

				case ColumnType.PériodePrécédente:
					return this.GetBudgetText (data.Numéro, data.SoldeDébit, data.SoldeCrédit, data.PériodePrécédente, monnaie);

				case ColumnType.PériodePénultième:
					return this.GetBudgetText (data.Numéro, data.SoldeDébit, data.SoldeCrédit, data.PériodePénultième, monnaie);

				case ColumnType.SoldeGraphique:
					return AbstractDataAccessor.GetGraphicText (row);

				case ColumnType.Solde:
					if (data.Catégorie == CatégorieDeCompte.Passif ||
						data.Catégorie == CatégorieDeCompte.Produit)
					{
						return Converters.MontantToString (data.Crédit.GetValueOrDefault () - data.Débit.GetValueOrDefault (), monnaie);
					}
					else
					{
						return Converters.MontantToString (data.Débit.GetValueOrDefault () - data.Crédit.GetValueOrDefault (), monnaie);
					}

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

		private FormattedText GetBudgetText(FormattedText name, decimal? soldeDébit, decimal? soldeCrédit, decimal? budget, ComptaMonnaieEntity monnaie)
		{
			decimal? solde = null;

			if (soldeDébit.HasValue)
			{
				solde = soldeDébit.Value;
			}

			if (soldeCrédit.HasValue)
			{
				solde = soldeCrédit.Value;
			}

			return this.budgetsManager.GetBudgetText (name, solde, budget, this.minValue, this.maxValue, monnaie);
		}


		private new BalanceOptions Options
		{
			get
			{
				return this.options as BalanceOptions;
			}
		}


		private BudgetsManager					budgetsManager;
	}
}