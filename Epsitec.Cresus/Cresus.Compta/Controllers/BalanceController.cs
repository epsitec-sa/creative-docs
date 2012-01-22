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
	/// Ce contrôleur gère la balance de vérification de la comptabilité.
	/// </summary>
	public class BalanceController : AbstractController
	{
		public BalanceController(Application app, BusinessContext businessContext, ComptaEntity comptaEntity, MainWindowController mainWindowController)
			: base (app, businessContext, comptaEntity, mainWindowController)
		{
			this.dataAccessor = new BalanceDataAccessor (this.businessContext, this.comptaEntity, this.mainWindowController);
			this.InitializeColumnMapper ();
		}


		protected override void CreateOptions(FrameBox parent)
		{
			this.optionsController = new BalanceOptionsController (this.comptaEntity, this.dataAccessor.AccessorOptions as BalanceOptions);
			this.optionsController.CreateUI (parent, this.OptionsChanged);
			this.optionsController.ShowPanel = this.ShowOptionsPanel;

			this.InitializeColumnMapper ();
		}

		protected override void UpdateTitle()
		{
			this.SetTitle ("Balance de vérification");
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
			var data = this.dataAccessor.GetReadOnlyData (row) as BalanceData;

			if (mapper.Column == ColumnType.Titre)
			{
				for (int i = 0; i < data.Niveau; i++)
				{
					text = FormattedText.Concat (UIBuilder.leftIndentText, text);
				}
			}
			else if (mapper.Column == ColumnType.Débit ||
					 mapper.Column == ColumnType.Crédit ||
					 mapper.Column == ColumnType.SoldeDébit ||
					 mapper.Column == ColumnType.SoldeCrédit ||
					 mapper.Column == ColumnType.Budget)
			{
				for (int i = 0; i < data.Niveau; i++)
				{
					text = FormattedText.Concat (text, UIBuilder.rightIndentText);
				}
			}

			return data.Typo (text);
		}


		protected override IEnumerable<ColumnMapper> ColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.Numéro,      0.15, ContentAlignment.MiddleLeft,  "Numéro");
				yield return new ColumnMapper (ColumnType.Titre,       0.60, ContentAlignment.MiddleLeft,  "Titre du compte");
				yield return new ColumnMapper (ColumnType.Débit,       0.20, ContentAlignment.MiddleRight, "Débit");
				yield return new ColumnMapper (ColumnType.Crédit,      0.20, ContentAlignment.MiddleRight, "Crédit");
				yield return new ColumnMapper (ColumnType.SoldeDébit,  0.20, ContentAlignment.MiddleRight, "Solde D");
				yield return new ColumnMapper (ColumnType.SoldeCrédit, 0.20, ContentAlignment.MiddleRight, "Solde C");
				yield return new ColumnMapper (ColumnType.Budget,      0.20, ContentAlignment.MiddleRight, "Budget");
			}
		}
	}
}
