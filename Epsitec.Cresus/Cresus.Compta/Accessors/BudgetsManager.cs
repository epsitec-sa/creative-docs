//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Cette classe calcule le montant d'un compte à mettre dans une colonne "budget".
	/// </summary>
	public class BudgetsManager
	{
		public BudgetsManager(ComptaEntity compta, ComptaPériodeEntity période, AbstractOptions options, Date? lastBeginDate, Date? lastEndDate)
		{
			this.compta        = compta;
			this.période       = période;
			this.options       = options;
			this.lastBeginDate = lastBeginDate;
			this.lastEndDate   = lastEndDate;

			this.soldesM1 = new SoldesJournalManager (this.compta);
			this.soldesM2 = new SoldesJournalManager (this.compta);

			//	Pour ComparisonShowed.PériodePrécédente et ComparisonShowed.PériodePénultième, il faut
			//	les vrais montants, et non les valeurs au budget.
			var périodeM1 = this.compta.GetPériode (this.période, -1);
			if (périodeM1 == null)
			{
				this.soldesM1.Initialize (null);
			}
			else
			{
				this.soldesM1.Initialize (périodeM1.Journal);
			}

			var périodeM2 = this.compta.GetPériode (this.période, -2);
			if (périodeM2 == null)
			{
				this.soldesM2.Initialize (null);
			}
			else
			{
				this.soldesM2.Initialize (périodeM2.Journal);
			}
		}


		public decimal? GetBudget(ComptaCompteEntity compte, ComparisonShowed type)
		{
			//	Retourne le montant d'un compte à mettre dans une colonne "budget".
			if (!this.options.ComparisonEnable)
			{
				return null;
			}

			decimal? budget;

			switch (type)
			{
				case ComparisonShowed.PériodePrécédente:
					budget = this.soldesM1.GetSolde (compte);
					break;

				case ComparisonShowed.PériodePénultième:
					budget = this.soldesM2.GetSolde (compte);
					break;

				case ComparisonShowed.Budget:
					budget = this.compta.GetMontantBudget (this.période, 0, compte);
					break;

				case ComparisonShowed.BudgetProrata:
					budget = this.compta.GetMontantBudget (this.période, 0, compte) * this.ProrataFactor;
					break;

				case ComparisonShowed.BudgetFutur:
					budget = this.compta.GetMontantBudget (this.période, 1, compte);
					break;

				case ComparisonShowed.BudgetFuturProrata:
					budget = this.compta.GetMontantBudget (this.période, 1, compte) * this.ProrataFactor;
					break;

				default:
					budget = null;
					break;
			}

			return budget;
		}

		private decimal ProrataFactor
		{
			get
			{
				int day = 0;

				if (this.lastEndDate.HasValue)
				{
					day = this.lastEndDate.Value.DayOfYear;

					if (this.lastBeginDate.HasValue)
					{
						day -= this.lastBeginDate.Value.DayOfYear;
					}
				}
				else
				{
					var écriture = this.période.Journal.LastOrDefault ();
					if (écriture == null)
					{
						day = Date.Today.DayOfYear;
					}
					else
					{
						day = écriture.Date.DayOfYear;
					}
				}

				return (decimal) day / 365.0M;
			}
		}


		public FormattedText GetBudgetText(decimal? solde, decimal? budget, decimal minValue, decimal maxValue)
		{
			//	Retourne le texte permettant d'afficher le budget, de différentes manières.
			if (!this.options.ComparisonEnable)
			{
				return FormattedText.Empty;
			}

			if (this.options.ComparisonDisplayMode == ComparisonDisplayMode.Montant)
			{
				return Converters.MontantToString (budget);
			}
			else
			{
				if (this.options.ComparisonDisplayMode == ComparisonDisplayMode.Pourcentage)
				{
					return BudgetsManager.GetPercent (solde, budget);
				}
				else if (this.options.ComparisonDisplayMode == ComparisonDisplayMode.PourcentageMontant)
				{
					var percent = BudgetsManager.GetPercent (solde, budget);
					var montant = Converters.MontantToString (budget);

					if (percent.IsNullOrEmpty || string.IsNullOrEmpty (montant))
					{
						return FormattedText.Empty;
					}
					else
					{
						return Core.TextFormatter.FormatText (percent, "de", montant);
					}
				}
				else if (this.options.ComparisonDisplayMode == ComparisonDisplayMode.Graphique)
				{
					return BudgetsManager.GetMinMaxText (budget, solde, minValue, maxValue);
				}
				else
				{
					return Converters.MontantToString (budget-solde);
				}
			}
		}

		private static FormattedText GetPercent(decimal? m1, decimal? m2)
		{
			if (m1.HasValue && m2.HasValue && m2.Value != 0)
			{
				int n = (int) (m1.Value/m2.Value*100);
				return n.ToString () + "%";
			}
			else
			{
				return FormattedText.Empty;
			}
		}

		private static FormattedText GetMinMaxText(decimal? value1, decimal? value2, decimal minValue, decimal maxValue)
		{
			if (minValue == decimal.MaxValue ||
				maxValue == decimal.MinValue)
			{
				return FormattedText.Empty;
			}
			else
			{
				return FormattedText.Concat
					(
						StringArray.SpecialContentGraphicValue,
						"/",
						Converters.MontantToString (minValue),
						"/",
						Converters.MontantToString (maxValue),
						"/",
						Converters.MontantToString (value1),
						"/",
						Converters.MontantToString (value2)
					);
			}
		}


		private readonly ComptaEntity								compta;
		private readonly ComptaPériodeEntity						période;
		private readonly AbstractOptions							options;
		private readonly Date?										lastBeginDate;
		private readonly Date?										lastEndDate;
		private readonly SoldesJournalManager						soldesM1;
		private readonly SoldesJournalManager						soldesM2;
	}
}
