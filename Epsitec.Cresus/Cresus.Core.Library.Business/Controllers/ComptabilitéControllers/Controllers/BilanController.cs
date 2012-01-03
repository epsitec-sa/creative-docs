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
	/// Ce contrôleur gère le bilan de la comptabilité.
	/// </summary>
	public class BilanController : AbstractEditorController<BilanColumn, BilanData, BilanOptions>
	{
		public BilanController(TileContainer tileContainer, ComptabilitéEntity comptabilitéEntity)
			: base (tileContainer, comptabilitéEntity)
		{
			this.dataAccessor = new BilanAccessor (this.comptabilitéEntity);

			this.columnMappers = new List<AbstractColumnMapper<BilanColumn>> ();
			foreach (var mapper in this.ColumnMappers)
			{
				this.columnMappers.Add (mapper);
			}
		}


		protected override void CreateOptions(FrameBox parent)
		{
			this.optionsController = new BilanOptionsController (this.tileContainer, this.comptabilitéEntity, this.dataAccessor.AccessorOptions as BilanOptions);
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

			if (mapper.Column == BilanColumn.TitreGauche)
			{
				for (int i = 0; i < data.NiveauGauche; i++)
				{
					text = FormattedText.Concat (UIBuilder.leftIndentText, text);
				}
			}
			else if (mapper.Column == BilanColumn.TitreDroite)
			{
				for (int i = 0; i < data.NiveauDroite; i++)
				{
					text = FormattedText.Concat (UIBuilder.leftIndentText, text);
				}
			}

			return text;
		}


		#region Column Mappers
		private IEnumerable<ColumnMapper> ColumnMappers
		{
			get
			{
				yield return new ColumnMapper (BilanColumn.NuméroGauche, 0.20, ContentAlignment.MiddleLeft,  "");
				yield return new ColumnMapper (BilanColumn.TitreGauche,  0.60, ContentAlignment.MiddleLeft,  "Actif");
				yield return new ColumnMapper (BilanColumn.SoldeGauche,  0.20, ContentAlignment.MiddleRight, "");

				yield return new ColumnMapper (BilanColumn.NuméroDroite, 0.20, ContentAlignment.MiddleLeft,  "");
				yield return new ColumnMapper (BilanColumn.TitreDroite,  0.60, ContentAlignment.MiddleLeft,  "Passif");
				yield return new ColumnMapper (BilanColumn.SoldeDroite,  0.20, ContentAlignment.MiddleRight, "");
			}
		}

		private class ColumnMapper : AbstractColumnMapper<BilanColumn>
		{
			public ColumnMapper(BilanColumn column, double relativeWidth, ContentAlignment alignment, FormattedText description)
				: base (column, null, relativeWidth, alignment, description, null)
			{
			}
		}
		#endregion


	}
}
