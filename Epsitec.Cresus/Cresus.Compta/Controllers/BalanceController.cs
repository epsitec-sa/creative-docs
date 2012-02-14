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
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Options.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère la balance de vérification de la comptabilité.
	/// </summary>
	public class BalanceController : AbstractController
	{
		public BalanceController(Application app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.dataAccessor = new BalanceDataAccessor (this);

			this.memoryList = this.mainWindowController.GetMemoryList ("Présentation.Balance.Memory");
		}


		protected override void CreateOptions(FrameBox parent)
		{
			this.optionsController = new BalanceOptionsController (this);
			this.optionsController.CreateUI (parent, this.OptionsChanged);
			this.optionsController.ShowPanel = this.mainWindowController.ShowOptionsPanel;

			this.UpdateColumnMappers ();
		}

		protected override void UpdateTitle()
		{
			this.SetTitle ("Balance de vérification");
			this.SetSubtitle (this.périodeEntity.ShortTitle);
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


		protected override FormattedText GetArrayText(int row, ColumnType columnType)
		{
			//	Retourne le texte contenu dans une cellule.
			var text = this.dataAccessor.GetText (row, columnType);
			var data = this.dataAccessor.GetReadOnlyData (row) as BalanceData;

			var options = this.dataAccessor.Options as BalanceOptions;

			if (columnType == ColumnType.Titre)
			{
				for (int i = 0; i < data.Niveau; i++)
				{
					text = FormattedText.Concat (UIBuilder.leftIndentText, text);
				}
			}
			else if (columnType == ColumnType.Débit       ||
					 columnType == ColumnType.Crédit      ||
					 columnType == ColumnType.SoldeDébit  ||
					 columnType == ColumnType.SoldeCrédit ||
					 columnType == ColumnType.Budget      )
			{
				if (!data.NeverFiltered && options.HideZero && text == Converters.MontantToString (0))
				{
					text = FormattedText.Empty;
				}
				
				if (!text.IsNullOrEmpty)
				{
					for (int i = 0; i < data.Niveau; i++)
					{
						text = FormattedText.Concat (text, UIBuilder.rightIndentText);
					}
				}
			}

			return data.Typo (text);
		}


		protected override IEnumerable<ColumnMapper> InitialColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.Numéro,      0.15, ContentAlignment.MiddleLeft,  "Numéro");
				yield return new ColumnMapper (ColumnType.Titre,       0.60, ContentAlignment.MiddleLeft,  "Titre du compte");
				yield return new ColumnMapper (ColumnType.Catégorie,   0.20, ContentAlignment.MiddleLeft,  "Catégorie", show: false);
				yield return new ColumnMapper (ColumnType.Type,        0.20, ContentAlignment.MiddleLeft,  "Type",      show: false);
				yield return new ColumnMapper (ColumnType.Débit,       0.20, ContentAlignment.MiddleRight, "Débit");
				yield return new ColumnMapper (ColumnType.Crédit,      0.20, ContentAlignment.MiddleRight, "Crédit");
				yield return new ColumnMapper (ColumnType.SoldeDébit,  0.20, ContentAlignment.MiddleRight, "Solde D");
				yield return new ColumnMapper (ColumnType.SoldeCrédit, 0.20, ContentAlignment.MiddleRight, "Solde C");
				yield return new ColumnMapper (ColumnType.Budget,      0.20, ContentAlignment.MiddleRight, "Budget");

				yield return new ColumnMapper (ColumnType.Date,        0.20, ContentAlignment.MiddleLeft,  "Date",       show: false);
				yield return new ColumnMapper (ColumnType.Solde,       0.20, ContentAlignment.MiddleLeft,  "Solde",      show: false);
				yield return new ColumnMapper (ColumnType.Profondeur,  0.20, ContentAlignment.MiddleLeft,  "Profondeur", show: false);
			}
		}
	}
}
