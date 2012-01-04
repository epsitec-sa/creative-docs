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
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Business.Finance.Comptabilité;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ComptabilitéControllers
{
	/// <summary>
	/// Ce contrôleur gère la ExtraitDeCompte de vérification de la comptabilité.
	/// </summary>
	public class ExtraitDeCompteController : AbstractController<ExtraitDeCompteColumn, ExtraitDeCompteData>
	{
		public ExtraitDeCompteController(BusinessContext businessContext, ComptabilitéEntity comptabilitéEntity)
			: base (businessContext, comptabilitéEntity)
		{
			this.dataAccessor = new ExtraitDeCompteAccessor (this.comptabilitéEntity);

			this.columnMappers = new List<AbstractColumnMapper<ExtraitDeCompteColumn>> ();
			foreach (var mapper in this.ColumnMappers)
			{
				this.columnMappers.Add (mapper);
			}
		}


		protected override void CreateOptions(FrameBox parent)
		{
			this.optionsController = new ExtraitDeCompteOptionsController (this.comptabilitéEntity, this.dataAccessor.AccessorOptions as ExtraitDeCompteOptions);
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

			if (mapper.Column == ExtraitDeCompteColumn.Solde &&
				row == this.dataAccessor.Count-2)  // total sur l'avant-dernière ligne ?
			{
				text = text.ApplyBold ();
			}

			return data.Typo (text);
		}


		#region Column Mappers
		private IEnumerable<ColumnMapper> ColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ExtraitDeCompteColumn.Date,    0.20, ContentAlignment.MiddleLeft,  "Date");
				yield return new ColumnMapper (ExtraitDeCompteColumn.CP,      0.20, ContentAlignment.MiddleLeft,  "C/P");
				yield return new ColumnMapper (ExtraitDeCompteColumn.Pièce,   0.20, ContentAlignment.MiddleLeft,  "Pièce");
				yield return new ColumnMapper (ExtraitDeCompteColumn.Libellé, 0.60, ContentAlignment.MiddleLeft,  "Libellé");
				yield return new ColumnMapper (ExtraitDeCompteColumn.Débit,   0.20, ContentAlignment.MiddleRight, "Débit");
				yield return new ColumnMapper (ExtraitDeCompteColumn.Crédit,  0.20, ContentAlignment.MiddleRight, "Crédit");
				yield return new ColumnMapper (ExtraitDeCompteColumn.Solde,   0.20, ContentAlignment.MiddleRight, "Solde");
			}
		}

		private class ColumnMapper : AbstractColumnMapper<ExtraitDeCompteColumn>
		{
			public ColumnMapper(ExtraitDeCompteColumn column, double relativeWidth, ContentAlignment alignment, FormattedText description)
				: base (column, null, relativeWidth, alignment, description, null)
			{
			}
		}
		#endregion


	}
}
