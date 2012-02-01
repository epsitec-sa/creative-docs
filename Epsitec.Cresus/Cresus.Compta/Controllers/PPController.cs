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
	/// Ce contrôleur gère les pertes et profits de la comptabilité.
	/// </summary>
	public class PPController : AbstractController
	{
		public PPController(Application app, BusinessContext businessContext, ComptaEntity comptaEntity, MainWindowController mainWindowController)
			: base (app, businessContext, comptaEntity, mainWindowController)
		{
			this.dataAccessor = new PPDataAccessor (this.businessContext, this.comptaEntity, this.columnMappers, this.mainWindowController);
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


		protected override void CreateOptions(FrameBox parent)
		{
			this.optionsController = new PPOptionsController (this);
			this.optionsController.CreateUI (parent, this.OptionsChanged);
			this.optionsController.ShowPanel = this.ShowOptionsPanel;

			this.UpdateColumnMappers ();
		}

		protected override void OptionsChanged()
		{
			this.UpdateColumnMappers ();
			this.UpdateArray ();

			base.OptionsChanged ();
		}

		protected override void UpdateTitle()
		{
			this.SetTitle ("Pertes et Profits");
		}


		protected override FormattedText GetArrayText(int row, ColumnType columnType)
		{
			//	Retourne le texte contenu dans une cellule.
			var text = this.dataAccessor.GetText (row, columnType);
			var data = this.dataAccessor.GetReadOnlyData (row) as PPData;

			if (columnType == ColumnType.TitreGauche)
			{
				for (int i = 0; i < data.NiveauGauche; i++)
				{
					text = FormattedText.Concat (UIBuilder.leftIndentText, text);
				}
			}
			else if (columnType == ColumnType.TitreDroite)
			{
				for (int i = 0; i < data.NiveauDroite; i++)
				{
					text = FormattedText.Concat (UIBuilder.leftIndentText, text);
				}
			}

			return data.Typo (text);
		}


		protected override IEnumerable<ColumnMapper> InitialColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.NuméroGauche,         0.20, ContentAlignment.MiddleLeft,  "");
				yield return new ColumnMapper (ColumnType.TitreGauche,          0.60, ContentAlignment.MiddleLeft,  "Charges");
				yield return new ColumnMapper (ColumnType.SoldeGauche,          0.20, ContentAlignment.MiddleRight, "");
				yield return new ColumnMapper (ColumnType.SoldeGraphiqueGauche, 0.20, ContentAlignment.MiddleRight, "", hideForSearch: true);
				yield return new ColumnMapper (ColumnType.BudgetGauche,         0.20, ContentAlignment.MiddleRight, "");

				yield return new ColumnMapper (ColumnType.Espace, 0.01, ContentAlignment.MiddleLeft, "", true);

				yield return new ColumnMapper (ColumnType.NuméroDroite,         0.20, ContentAlignment.MiddleLeft,  "");
				yield return new ColumnMapper (ColumnType.TitreDroite,          0.60, ContentAlignment.MiddleLeft,  "Produits");
				yield return new ColumnMapper (ColumnType.SoldeDroite,          0.20, ContentAlignment.MiddleRight, "");
				yield return new ColumnMapper (ColumnType.SoldeGraphiqueDroite, 0.20, ContentAlignment.MiddleRight, "", hideForSearch: true);
				yield return new ColumnMapper (ColumnType.BudgetDroite,         0.20, ContentAlignment.MiddleRight, "");

				yield return new ColumnMapper (ColumnType.Date, 0.20, ContentAlignment.MiddleRight, "Date", show: false);
			}
		}

		protected override void UpdateColumnMappers()
		{
			var options = this.dataAccessor.AccessorOptions as PPOptions;

			this.ShowHideColumn (ColumnType.SoldeGraphiqueGauche, options.HasGraphics);
			this.ShowHideColumn (ColumnType.BudgetGauche,         options.BudgetEnable && this.optionsController != null);
			this.ShowHideColumn (ColumnType.SoldeGraphiqueDroite, options.HasGraphics);
			this.ShowHideColumn (ColumnType.BudgetDroite,         options.BudgetEnable && this.optionsController != null);

			this.SetColumnDescription (ColumnType.BudgetGauche, options.BudgetColumnDescription);
			this.SetColumnDescription (ColumnType.BudgetDroite, options.BudgetColumnDescription);
		}
	}
}
