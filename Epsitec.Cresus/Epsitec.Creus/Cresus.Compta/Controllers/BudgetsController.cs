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
using Epsitec.Cresus.Compta.Permanents;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les budgets du plan comptable de la comptabilité.
	/// </summary>
	public class BudgetsController : AbstractController
	{
		public BudgetsController(ComptaApplication app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.dataAccessor = new BudgetsDataAccessor (this);

			this.UpdateColumnMappers ();
		}


		public override bool HasSearchPanel
		{
			get
			{
				return true;
			}
		}

		public override bool HasFilterPanel
		{
			get
			{
				return true;
			}
		}

		public override bool HasOptionsPanel
		{
			get
			{
				return false;
			}
		}

		public override bool HasInfoPanel
		{
			get
			{
				return true;
			}
		}


		protected override void UpdateTitle()
		{
			this.SetGroupTitle ("Budget");
			this.SetTitle ("Budget");
			this.SetSubtitle (this.période.ShortTitle);
		}


		protected override FormattedText GetArrayText(int row, ColumnType columnType)
		{
			//	Retourne le texte contenu dans une cellule.
			var text = this.dataAccessor.GetText (row, columnType);

			if (columnType == ColumnType.Titre)
			{
				var compte = this.dataAccessor.GetEditionEntity (row) as ComptaCompteEntity;

				for (int i = 0; i < compte.Niveau; i++)
				{
					text = FormattedText.Concat (UIBuilder.leftIndentText, text);
				}
			}

			return text;
		}


		protected override void CreateEditor(FrameBox parent)
		{
			this.editorController = new BudgetsEditorController (this);
			this.editorController.CreateUI (parent, this.UpdateArrayContent);
		}


		#region Context menu
		protected override VMenu ContextMenu
		{
			//	Retourne le menu contextuel à utiliser.
			get
			{
				var menu = new VMenu ();

				this.PutContextMenuExtrait (menu);
				this.PutContextMenuPlanComptable (menu);

				return menu;
			}
		}

		private void PutContextMenuExtrait(VMenu menu)
		{
			var compte = this.dataAccessor.GetEditionEntity (this.arrayController.SelectedRow) as ComptaCompteEntity;

			var item = this.PutContextMenuItem (menu, "Présentation.Extrait", string.Format ("Extrait du compte {0}", compte.Numéro));

			item.Clicked += delegate
			{
				var présentation = this.mainWindowController.ShowPrésentation (ControllerType.Extrait);

				var permanent = présentation.DataAccessor.Permanents as ExtraitDeComptePermanents;
				permanent.NuméroCompte = compte.Numéro;

				présentation.UpdateAfterChanged ();
			};
		}

		private void PutContextMenuPlanComptable(VMenu menu)
		{
			var compte = this.dataAccessor.GetEditionEntity (this.arrayController.SelectedRow) as ComptaCompteEntity;

			var item = this.PutContextMenuItem (menu, "Présentation.PlanComptable", string.Format ("Définition du compte {0}", compte.Numéro));

			item.Clicked += delegate
			{
				var présentation = this.mainWindowController.ShowPrésentation (ControllerType.PlanComptable);

				int row = (présentation.DataAccessor as PlanComptableDataAccessor).GetIndexOf (compte);
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
				yield return new ColumnMapper (ColumnType.Numéro,          0.20, ContentAlignment.MiddleLeft,  "Numéro",          "Numéro du compte", enable: false);
				yield return new ColumnMapper (ColumnType.Titre,           0.80, ContentAlignment.MiddleLeft,  "Titre du compte", "Titre du compte",  enable: false);
				yield return new ColumnMapper (ColumnType.Solde,           0.20, ContentAlignment.MiddleRight, "Solde",           "Solde du compte",  enable: false);
				yield return new ColumnMapper (ColumnType.BudgetPrécédent, 0.20, ContentAlignment.MiddleRight, "",                "Budget de la période précédente");
				yield return new ColumnMapper (ColumnType.Budget,          0.20, ContentAlignment.MiddleRight, "",                "Budget de la période actuelle");
				yield return new ColumnMapper (ColumnType.BudgetFutur,     0.20, ContentAlignment.MiddleRight, "",                "Budget de la période future");
			}
		}

		protected override void UpdateColumnMappers()
		{
			//	Défini les titres des 3 colonnes "budget".
			this.SetColumnBudget (ColumnType.BudgetPrécédent, -1, false);
			this.SetColumnBudget (ColumnType.Budget,           0, true);
			this.SetColumnBudget (ColumnType.BudgetFutur,      1, false);
		}

		private void SetColumnBudget(ColumnType columnType, int offset, bool isBold)
		{
			//	Défini le titre d'une colonne "budget".
			var other = this.compta.GetPériode (this.période, offset);

			this.ShowHideColumn (columnType, other != null);

			if (other != null)
			{
				var title = other.ShortTitle;

				if (title.Length <= 4)  // simplement l'année ?
				{
					title = FormattedText.Concat ("Budget ", title);
				}

				if (isBold)
				{
					title = title.ApplyBold ();
				}

				this.SetColumnDescription (columnType, title);
			}
		}
	}
}
