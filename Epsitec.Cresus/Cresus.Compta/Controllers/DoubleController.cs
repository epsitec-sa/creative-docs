//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Permanents.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les données doubles (bilan avec actif/passif ou PP avec charge/produit) de la comptabilité.
	/// </summary>
	public abstract class DoubleController : AbstractController
	{
		public DoubleController(ComptaApplication app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
		}


		public override bool HasShowSearchPanel
		{
			get
			{
				return true;
			}
		}

		public override bool HasShowFilterPanel
		{
			get
			{
				return true;
			}
		}

		public override bool HasShowOptionsPanel
		{
			get
			{
				return true;
			}
		}

		public override bool HasShowInfoPanel
		{
			get
			{
				return false;
			}
		}


		protected override void OptionsChanged()
		{
			this.UpdateColumnMappers ();
			this.UpdateArray ();

			base.OptionsChanged ();
		}


		protected override FormattedText GetArrayText(int row, ColumnType columnType)
		{
			//	Retourne le texte contenu dans une cellule.
			var text = this.dataAccessor.GetText (row, columnType);
			var data = this.dataAccessor.GetReadOnlyData (row) as DoubleData;

			var options = this.dataAccessor.Options as DoubleOptions;

			if (columnType == ColumnType.Titre)
			{
				for (int i = 0; i < data.Niveau; i++)
				{
					text = FormattedText.Concat ("    ", text);
				}
			}
			else if (columnType == ColumnType.PériodePénultième ||
					 columnType == ColumnType.PériodePrécédente ||
					 columnType == ColumnType.Solde             ||
					 columnType == ColumnType.Budget            ||
					 columnType == ColumnType.BudgetProrata     ||
					 columnType == ColumnType.BudgetFutur       ||
					 columnType == ColumnType.BudgetFuturProrata)
			{
				if (!data.NeverFiltered && options.HideZero && text == Converters.MontantToString (0))
				{
					text = FormattedText.Empty;
				}
			}

			return data.Typo (text);
		}


		#region Context menu
		protected override VMenu ContextMenu
		{
			//	Retourne le menu contextuel à utiliser.
			get
			{
				var menu = new VMenu ();

				this.PutContextMenuExtrait (menu);
				this.PutContextMenuBudget (menu);

				return menu;
			}
		}

		private void PutContextMenuExtrait(VMenu menu)
		{
			var data = this.dataAccessor.GetReadOnlyData (this.arrayController.SelectedRow) as DoubleData;

			var item = this.PutContextMenuItem (menu, "Présentation.Extrait", string.Format ("Extrait du compte {0}", data.Numéro));

			item.Clicked += delegate
			{
				var présentation = this.mainWindowController.ShowPrésentation (Res.Commands.Présentation.Extrait);

				var permanent = présentation.DataAccessor.Permanents as ExtraitDeComptePermanents;
				permanent.NuméroCompte = data.Numéro;

				présentation.UpdateAfterChanged ();
			};
		}

		private void PutContextMenuBudget(VMenu menu)
		{
			var data = this.dataAccessor.GetReadOnlyData (this.arrayController.SelectedRow) as DoubleData;

			var item = this.PutContextMenuItem (menu, "Présentation.Budgets", string.Format ("Budgets du compte {0}", data.Numéro));

			item.Clicked += delegate
			{
				var présentation = this.mainWindowController.ShowPrésentation (Res.Commands.Présentation.Budgets);

				var compte = this.compta.PlanComptable.Where (x => x.Numéro == data.Numéro).FirstOrDefault ();
				int row = (présentation.DataAccessor as BudgetsDataAccessor).GetIndexOf (compte);
				if (row != -1)
				{
					présentation.SelectedArrayLine = row;
				}
			};
		}
		#endregion


		protected override IEnumerable<ColumnMapper> InitialColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.Numéro,             0.20, ContentAlignment.MiddleLeft,  "Numéro");
				yield return new ColumnMapper (ColumnType.Titre,              1.20, ContentAlignment.MiddleLeft,  "Titre du compte");
				yield return new ColumnMapper (ColumnType.PériodePénultième,  0.20, ContentAlignment.MiddleRight, "Période pénul.");
				yield return new ColumnMapper (ColumnType.PériodePrécédente,  0.20, ContentAlignment.MiddleRight, "Période préc.");
				yield return new ColumnMapper (ColumnType.Solde,              0.20, ContentAlignment.MiddleRight, "Montant");
				yield return new ColumnMapper (ColumnType.SoldeGraphique,     0.20, ContentAlignment.MiddleRight, "", hideForSearch: true);
				yield return new ColumnMapper (ColumnType.Budget,             0.20, ContentAlignment.MiddleRight, "Budget");
				yield return new ColumnMapper (ColumnType.BudgetProrata,      0.20, ContentAlignment.MiddleRight, "Budget prorata");
				yield return new ColumnMapper (ColumnType.BudgetFutur,        0.20, ContentAlignment.MiddleRight, "Budget futur");
				yield return new ColumnMapper (ColumnType.BudgetFuturProrata, 0.20, ContentAlignment.MiddleRight, "Budget fut. pro.");

				yield return new ColumnMapper (ColumnType.Date,       0.20, ContentAlignment.MiddleLeft, "Date",       show: false);
				yield return new ColumnMapper (ColumnType.Profondeur, 0.20, ContentAlignment.MiddleLeft, "Profondeur", show: false);
			}
		}

		protected override void UpdateColumnMappers()
		{
			var options = this.dataAccessor.Options as DoubleOptions;

			this.ShowHideColumn (ColumnType.SoldeGraphique, options.HasGraphics);

			if (options.ComparisonEnable && this.optionsController != null)
			{
				this.ShowHideColumn (ColumnType.PériodePrécédente,  (options.ComparisonShowed & ComparisonShowed.PériodePrécédente ) != 0);
				this.ShowHideColumn (ColumnType.PériodePénultième,  (options.ComparisonShowed & ComparisonShowed.PériodePénultième ) != 0);
				this.ShowHideColumn (ColumnType.Budget,             (options.ComparisonShowed & ComparisonShowed.Budget            ) != 0);
				this.ShowHideColumn (ColumnType.BudgetProrata,      (options.ComparisonShowed & ComparisonShowed.BudgetProrata     ) != 0);
				this.ShowHideColumn (ColumnType.BudgetFutur,        (options.ComparisonShowed & ComparisonShowed.BudgetFutur       ) != 0);
				this.ShowHideColumn (ColumnType.BudgetFuturProrata, (options.ComparisonShowed & ComparisonShowed.BudgetFuturProrata) != 0);
			}
			else
			{
				this.ShowHideColumn (ColumnType.PériodePrécédente,  false);
				this.ShowHideColumn (ColumnType.PériodePénultième,  false);
				this.ShowHideColumn (ColumnType.Budget,             false);
				this.ShowHideColumn (ColumnType.BudgetProrata,      false);
				this.ShowHideColumn (ColumnType.BudgetFutur,        false);
				this.ShowHideColumn (ColumnType.BudgetFuturProrata, false);
			}
		}
	}
}
