//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les budgets du plan comptable de la comptabilité.
	/// </summary>
	public class BudgetsController : AbstractController
	{
		public BudgetsController(Application app, BusinessContext businessContext, ComptaEntity comptaEntity, MainWindowController mainWindowController)
			: base (app, businessContext, comptaEntity, mainWindowController)
		{
			this.dataAccessor = new BudgetsDataAccessor (this.businessContext, this.comptaEntity, this.mainWindowController);
			this.InitializeColumnMapper ();
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
		}


		protected override FormattedText GetArrayText(int row, int column)
		{
			//	Retourne le texte contenu dans une cellule.
			var mapper = this.columnMappers[column];
			var text = this.dataAccessor.GetText (row, mapper.Column);

			if (mapper.Column == ColumnType.Titre)
			{
				var compte = this.dataAccessor.GetEditionData (row) as ComptaCompteEntity;

				for (int i = 0; i < compte.Niveau; i++)
				{
					text = FormattedText.Concat (UIBuilder.leftIndentText, text);
				}
			}

			return text;
		}


		protected override void CreateFooter(FrameBox parent)
		{
			this.footerController = new BudgetsFooterController (this.app, this.businessContext, this.comptaEntity, this.dataAccessor, this.columnMappers, this, this.arrayController);
			this.footerController.CreateUI (parent, this.UpdateArrayContent);
		}


		protected override IEnumerable<ColumnMapper> ColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.Numéro,          0.20, ContentAlignment.MiddleLeft,  "Numéro",          "Numéro du compte");
				yield return new ColumnMapper (ColumnType.Titre,           0.80, ContentAlignment.MiddleLeft,  "Titre du compte", "Titre du compte");
				yield return new ColumnMapper (ColumnType.Solde,           0.20, ContentAlignment.MiddleRight, "Solde",           "Solde du compte");
				yield return new ColumnMapper (ColumnType.BudgetPrécédent, 0.20, ContentAlignment.MiddleRight, "Année préc.",     "Budget de l'année précédente");
				yield return new ColumnMapper (ColumnType.Budget,          0.20, ContentAlignment.MiddleRight, "Budget",          "Budget actuel");
				yield return new ColumnMapper (ColumnType.BudgetFutur,     0.20, ContentAlignment.MiddleRight, "Budget futur",    "Budget futur");
			}
		}
	}
}
