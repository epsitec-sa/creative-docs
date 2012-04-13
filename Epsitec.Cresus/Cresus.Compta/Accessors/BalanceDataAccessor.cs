//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
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

				var data = new BalanceData ();

				data.Entity    = compte;
				data.Numéro    = compte.Numéro;
				data.Titre     = compte.Titre;
				data.Catégorie = compte.Catégorie;
				data.Type      = compte.Type;
				data.Niveau    = compte.Niveau;
				data.Débit     = soldeDébit;
				data.Crédit    = soldeCrédit;

				data.PériodePrécédente  = this.GetBudget (compte, ComparisonShowed.PériodePrécédente);
				data.PériodePénultième  = this.GetBudget (compte, ComparisonShowed.PériodePénultième);
				data.Budget             = this.GetBudget (compte, ComparisonShowed.Budget);
				data.BudgetProrata      = this.GetBudget (compte, ComparisonShowed.BudgetProrata);
				data.BudgetFutur        = this.GetBudget (compte, ComparisonShowed.BudgetFutur);
				data.BudgetFuturProrata = this.GetBudget (compte, ComparisonShowed.BudgetFuturProrata);

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
					return this.GetBudgetText (data.SoldeDébit, data.SoldeCrédit, data.Budget, monnaie);

				case ColumnType.BudgetProrata:
					return this.GetBudgetText (data.SoldeDébit, data.SoldeCrédit, data.BudgetProrata, monnaie);

				case ColumnType.BudgetFutur:
					return this.GetBudgetText (data.SoldeDébit, data.SoldeCrédit, data.BudgetFutur, monnaie);

				case ColumnType.BudgetFuturProrata:
					return this.GetBudgetText (data.SoldeDébit, data.SoldeCrédit, data.BudgetFuturProrata, monnaie);

				case ColumnType.PériodePrécédente:
					return this.GetBudgetText (data.SoldeDébit, data.SoldeCrédit, data.PériodePrécédente, monnaie);

				case ColumnType.PériodePénultième:
					return this.GetBudgetText (data.SoldeDébit, data.SoldeCrédit, data.PériodePénultième, monnaie);

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
			var budget = this.budgetsManager.GetBudget (compte, type);
			this.SetMinMaxValue (budget);
			return budget;
		}

		private FormattedText GetBudgetText(decimal? soldeDébit, decimal? soldeCrédit, decimal? budget, ComptaMonnaieEntity monnaie)
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

			return this.budgetsManager.GetBudgetText (solde, budget, this.minValue, this.maxValue, monnaie);
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