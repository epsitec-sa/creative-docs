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
	/// Ce contrôleur gère la balance de vérification de la comptabilité.
	/// </summary>
	public class BalanceController : AbstractEditorController<BalanceColumn, BalanceData, BalanceOptions>
	{
		public BalanceController(TileContainer tileContainer, ComptabilitéEntity comptabilitéEntity)
			: base (tileContainer, comptabilitéEntity)
		{
			this.dataAccessor = new BalanceAccessor (this.comptabilitéEntity);

			this.columnMappers = new List<AbstractColumnMapper<BalanceColumn>> ();
			foreach (var mapper in this.ColumnMappers)
			{
				this.columnMappers.Add (mapper);
			}
		}


		protected override void CreateOptions(FrameBox parent)
		{
			this.optionsController = new BalanceOptionsController (this.tileContainer, this.comptabilitéEntity, this.dataAccessor.AccessorOptions as BalanceOptions);
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
			var compte = this.dataAccessor.SortedList[row];

			if (mapper.Column == BalanceColumn.Titre)
			{
				for (int i = 0; i < compte.Niveau; i++)
				{
					text = FormattedText.Concat (UIBuilder.leftIndentText, text);
				}
			}
			else if (mapper.Column == BalanceColumn.Débit ||
					 mapper.Column == BalanceColumn.Crédit ||
					 mapper.Column == BalanceColumn.SoldeDébit ||
					 mapper.Column == BalanceColumn.SoldeCrédit ||
					 mapper.Column == BalanceColumn.Budget)
			{
				for (int i = 0; i < compte.Niveau; i++)
				{
					text = FormattedText.Concat (text, UIBuilder.rightIndentText);
				}
			}

			if (compte.IsHilited)
			{
				text = text.ApplyBold ();
			}

			return text;
		}


		#region Column Mappers
		private IEnumerable<ColumnMapper> ColumnMappers
		{
			get
			{
				yield return new ColumnMapper (BalanceColumn.Numéro,      0.15, ContentAlignment.MiddleLeft,  "Numéro");
				yield return new ColumnMapper (BalanceColumn.Titre,       0.60, ContentAlignment.MiddleLeft,  "Titre du compte");
				yield return new ColumnMapper (BalanceColumn.Débit,       0.20, ContentAlignment.MiddleRight, "Débit");
				yield return new ColumnMapper (BalanceColumn.Crédit,      0.20, ContentAlignment.MiddleRight, "Crédit");
				yield return new ColumnMapper (BalanceColumn.SoldeDébit,  0.20, ContentAlignment.MiddleRight, "Solde D");
				yield return new ColumnMapper (BalanceColumn.SoldeCrédit, 0.20, ContentAlignment.MiddleRight, "Solde C");
				yield return new ColumnMapper (BalanceColumn.Budget,      0.20, ContentAlignment.MiddleRight, "Budget");
			}
		}

		private class ColumnMapper : AbstractColumnMapper<BalanceColumn>
		{
			public ColumnMapper(BalanceColumn column, double relativeWidth, ContentAlignment alignment, FormattedText description)
				: base (column, null, relativeWidth, alignment, description, null)
			{
			}
		}
		#endregion


	}
}
