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

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les budgets du plan comptable de la comptabilité.
	/// </summary>
	public class BudgetsController : AbstractController
	{
		public BudgetsController(Application app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.dataAccessor = new BudgetsDataAccessor (this);

			this.viewSettingsList = this.mainWindowController.GetViewSettingsList ("Présentation.Budgets.ViewSettings");

			this.UpdateColumnMappers ();
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
				return false;
			}
		}

		public override bool HasShowInfoPanel
		{
			get
			{
				return true;
			}
		}


		protected override void UpdateTitle()
		{
			this.SetTitle ("Budget");
			this.SetSubtitle (this.périodeEntity.ShortTitle);
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


		protected override void CreateFooter(FrameBox parent)
		{
			this.footerController = new BudgetsFooterController (this);
			this.footerController.CreateUI (parent, this.UpdateArrayContent);
		}


		protected override IEnumerable<ColumnMapper> InitialColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.Numéro,          0.20, ContentAlignment.MiddleLeft,  "Numéro",          "Numéro du compte");
				yield return new ColumnMapper (ColumnType.Titre,           0.80, ContentAlignment.MiddleLeft,  "Titre du compte", "Titre du compte");
				yield return new ColumnMapper (ColumnType.Solde,           0.20, ContentAlignment.MiddleRight, "Solde",           "Solde du compte");
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
			var other = this.comptaEntity.GetPériode (this.périodeEntity, offset);

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
