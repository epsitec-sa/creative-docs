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
	/// Ce contrôleur gère le bilan de la comptabilité.
	/// </summary>
	public class BilanController : AbstractController
	{
		public BilanController(Application app, BusinessContext businessContext, ComptabilitéEntity comptabilitéEntity, MainWindowController mainWindowController)
			: base (app, businessContext, comptabilitéEntity, mainWindowController)
		{
			this.dataAccessor = new BilanDataAccessor (this.businessContext, this.comptabilitéEntity, this.mainWindowController);
			this.InitializeColumnMapper ();
		}


		protected override void CreateOptions(FrameBox parent)
		{
			this.optionsController = new BilanOptionsController (this.comptabilitéEntity, this.dataAccessor.AccessorOptions as BilanOptions);
			this.optionsController.CreateUI (parent, this.OptinsChanged);
			this.optionsController.ShowPanel = this.ShowOptionsPanel;
		}

		protected override void OptinsChanged()
		{
			this.InitializeColumnMapper ();
			this.UpdateArray ();

			base.OptinsChanged ();
		}

		protected override void FinalUpdate()
		{
			base.FinalUpdate ();
		}


		public override bool HasShowSearchPanel
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


		protected override FormattedText GetArrayText(int row, int column)
		{
			//	Retourne le texte contenu dans une cellule.
			var mapper = this.columnMappers[column];
			var text = this.dataAccessor.GetText (row, mapper.Column);
			var data = this.dataAccessor.GetReadOnlyData (row) as BilanData;

			if (mapper.Column == ColumnType.TitreGauche)
			{
				for (int i = 0; i < data.NiveauGauche; i++)
				{
					text = FormattedText.Concat (UIBuilder.leftIndentText, text);
				}
			}
			else if (mapper.Column == ColumnType.TitreDroite)
			{
				for (int i = 0; i < data.NiveauDroite; i++)
				{
					text = FormattedText.Concat (UIBuilder.leftIndentText, text);
				}
			}

			return data.Typo (text);
		}


		protected override IEnumerable<ColumnMapper> ColumnMappers
		{
			get
			{
				var options = this.dataAccessor.AccessorOptions as BilanOptions;

				yield return new ColumnMapper (ColumnType.NuméroGauche, 0.20, ContentAlignment.MiddleLeft,  "");
				yield return new ColumnMapper (ColumnType.TitreGauche,  0.60, ContentAlignment.MiddleLeft,  "Actif");
				yield return new ColumnMapper (ColumnType.SoldeGauche,  0.20, ContentAlignment.MiddleRight, "");

				if (options.HasGraphics)
				{
					yield return new ColumnMapper (ColumnType.SoldeGraphiqueGauche, 0.20, ContentAlignment.MiddleRight, "");
				}

				if (options.BudgetEnable)
				{
					yield return new ColumnMapper (ColumnType.BudgetGauche, 0.20, ContentAlignment.MiddleRight, this.optionsController.Options.BudgetColumnDescription);
				}

				yield return new ColumnMapper (ColumnType.Espace, 0.01, ContentAlignment.MiddleLeft, "");

				yield return new ColumnMapper (ColumnType.NuméroDroite, 0.20, ContentAlignment.MiddleLeft,  "");
				yield return new ColumnMapper (ColumnType.TitreDroite,  0.60, ContentAlignment.MiddleLeft,  "Passif");
				yield return new ColumnMapper (ColumnType.SoldeDroite,  0.20, ContentAlignment.MiddleRight, "");

				if (options.HasGraphics)
				{
					yield return new ColumnMapper (ColumnType.SoldeGraphiqueDroite, 0.20, ContentAlignment.MiddleRight, "");
				}

				if (options.BudgetEnable)
				{
					yield return new ColumnMapper (ColumnType.BudgetDroite, 0.20, ContentAlignment.MiddleRight, this.optionsController.Options.BudgetColumnDescription);
				}
			}
		}
	}
}
