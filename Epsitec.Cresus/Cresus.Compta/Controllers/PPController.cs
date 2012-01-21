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
		public PPController(Application app, BusinessContext businessContext, ComptabilitéEntity comptabilitéEntity)
			: base (app, businessContext, comptabilitéEntity)
		{
			this.dataAccessor = new PPDataAccessor (this.businessContext, this.comptabilitéEntity);
			this.InitializeColumnMapper ();
		}


		protected override void CreateOptions(FrameBox parent)
		{
			this.optionsController = new PPOptionsController (this.comptabilitéEntity, this.dataAccessor.AccessorOptions as PPOptions);
			this.optionsController.CreateUI (parent, this.OptinsChanged);
		}

		protected override void FinalizeOptions(FrameBox parent)
		{
			this.optionsController.FinalizeUI (parent);
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

		public override void UpdateData()
		{
		}


		protected override FormattedText GetArrayText(int row, int column)
		{
			//	Retourne le texte contenu dans une cellule.
			var mapper = this.columnMappers[column];
			var text = this.dataAccessor.GetText (row, mapper.Column);
			var data = this.dataAccessor.GetReadOnlyData (row) as PPData;

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
				var options = this.dataAccessor.AccessorOptions as PPOptions;

				yield return new ColumnMapper (ColumnType.NuméroGauche, 0.20, ContentAlignment.MiddleLeft,  "");
				yield return new ColumnMapper (ColumnType.TitreGauche,  0.60, ContentAlignment.MiddleLeft,  "Charges");
				yield return new ColumnMapper (ColumnType.SoldeGauche,  0.20, ContentAlignment.MiddleRight, "");

				if (options.HasGraphics)
				{
					yield return new ColumnMapper (ColumnType.SoldeGraphiqueGauche, 0.20, ContentAlignment.MiddleRight, "");
				}

				if (options.BudgetEnable)
				{
					yield return new ColumnMapper (ColumnType.BudgetGauche, 0.20, ContentAlignment.MiddleRight, this.optionsController.Options.BudgetColumnDescription);
				}

				yield return new ColumnMapper (ColumnType.Espace,       0.01, ContentAlignment.MiddleLeft,  "");

				yield return new ColumnMapper (ColumnType.NuméroDroite, 0.20, ContentAlignment.MiddleLeft,  "");
				yield return new ColumnMapper (ColumnType.TitreDroite,  0.60, ContentAlignment.MiddleLeft,  "Produits");
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
