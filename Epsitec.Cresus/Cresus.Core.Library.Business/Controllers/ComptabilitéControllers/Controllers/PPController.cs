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
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Business.Finance.Comptabilité;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ComptabilitéControllers
{
	/// <summary>
	/// Ce contrôleur gère les pertes et profits de la comptabilité.
	/// </summary>
	public class PPController : AbstractController<PPColumn, PPData>
	{
		public PPController(TileContainer tileContainer, ComptabilitéEntity comptabilitéEntity)
			: base (tileContainer, comptabilitéEntity)
		{
			this.dataAccessor = new PPAccessor (this.comptabilitéEntity);

			this.columnMappers = new List<AbstractColumnMapper<PPColumn>> ();
			foreach (var mapper in this.ColumnMappers)
			{
				this.columnMappers.Add (mapper);
			}
		}


		protected override void CreateOptions(FrameBox parent)
		{
			this.optionsController = new PPOptionsController (this.tileContainer, this.comptabilitéEntity, this.dataAccessor.AccessorOptions as PPOptions);
			this.optionsController.CreateUI (parent, this.OptinsChanged);
		}

		protected override void FinalizeOptions(FrameBox parent)
		{
			this.optionsController.FinalizeUI (parent);
		}

		protected override void FinalUpdate()
		{
			base.FinalUpdate ();
		}


		protected override FormattedText GetArrayText(int row, int column)
		{
			//	Retourne le texte contenu dans une cellule.
			var mapper = this.columnMappers[column];
			var text = this.dataAccessor.GetText (row, mapper.Column);
			var data = this.dataAccessor.SortedList[row];

			if (mapper.Column == PPColumn.TitreGauche)
			{
				for (int i = 0; i < data.NiveauGauche; i++)
				{
					text = FormattedText.Concat (UIBuilder.leftIndentText, text);
				}
			}
			else if (mapper.Column == PPColumn.TitreDroite)
			{
				for (int i = 0; i < data.NiveauDroite; i++)
				{
					text = FormattedText.Concat (UIBuilder.leftIndentText, text);
				}
			}

			return data.Typo (text);
		}


		#region Column Mappers
		private IEnumerable<ColumnMapper> ColumnMappers
		{
			get
			{
				yield return new ColumnMapper (PPColumn.NuméroGauche, 0.20, ContentAlignment.MiddleLeft,  "");
				yield return new ColumnMapper (PPColumn.TitreGauche,  0.60, ContentAlignment.MiddleLeft,  "Charges");
				yield return new ColumnMapper (PPColumn.SoldeGauche,  0.20, ContentAlignment.MiddleRight, "");

				yield return new ColumnMapper (PPColumn.Espace,       0.01, ContentAlignment.MiddleLeft,  "");

				yield return new ColumnMapper (PPColumn.NuméroDroite, 0.20, ContentAlignment.MiddleLeft,  "");
				yield return new ColumnMapper (PPColumn.TitreDroite,  0.60, ContentAlignment.MiddleLeft,  "Produits");
				yield return new ColumnMapper (PPColumn.SoldeDroite,  0.20, ContentAlignment.MiddleRight, "");
			}
		}

		private class ColumnMapper : AbstractColumnMapper<PPColumn>
		{
			public ColumnMapper(PPColumn column, double relativeWidth, ContentAlignment alignment, FormattedText description)
				: base (column, null, relativeWidth, alignment, description, null)
			{
			}
		}
		#endregion


	}
}
