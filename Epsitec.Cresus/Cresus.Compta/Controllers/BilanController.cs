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
		public BilanController(Application app, BusinessContext businessContext, ComptaEntity comptaEntity, MainWindowController mainWindowController)
			: base (app, businessContext, comptaEntity, mainWindowController)
		{
			this.dataAccessor = new BilanDataAccessor (this);
		}


		protected override void CreateOptions(FrameBox parent)
		{
			this.optionsController = new BilanOptionsController (this);
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
			this.SetTitle ("Bilan intermédiaire");
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
			var data = this.dataAccessor.GetReadOnlyData (row) as DoubleData;

			var options = this.dataAccessor.AccessorOptions as DoubleOptions;

			if (columnType == ColumnType.Titre)
			{
				for (int i = 0; i < data.Niveau; i++)
				{
					text = FormattedText.Concat ("    ", text);
				}
			}
			else if (columnType == ColumnType.Solde)
			{
				if (!data.NeverFiltered && options.HideZero && text == "0.00")
				{
					text = FormattedText.Empty;
				}
			}

			return data.Typo (text);
		}


		protected override IEnumerable<ColumnMapper> InitialColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.Numéro,         0.20, ContentAlignment.MiddleLeft,  "Numéro");
				yield return new ColumnMapper (ColumnType.Titre,          1.00, ContentAlignment.MiddleLeft,  "Titre du compte");
				yield return new ColumnMapper (ColumnType.Solde,          0.20, ContentAlignment.MiddleRight, "Montant");
				yield return new ColumnMapper (ColumnType.SoldeGraphique, 0.20, ContentAlignment.MiddleRight, "", hideForSearch: true);
				yield return new ColumnMapper (ColumnType.Budget,         0.20, ContentAlignment.MiddleRight, "");

				yield return new ColumnMapper (ColumnType.Date,       0.20, ContentAlignment.MiddleLeft, "Date",       show: false);
				yield return new ColumnMapper (ColumnType.Profondeur, 0.20, ContentAlignment.MiddleLeft, "Profondeur", show: false);
			}
		}

		protected override void UpdateColumnMappers()
		{
			var options = this.dataAccessor.AccessorOptions as DoubleOptions;

			this.ShowHideColumn (ColumnType.SoldeGraphique, options.HasGraphics);
			this.ShowHideColumn (ColumnType.Budget,         options.BudgetEnable && this.optionsController != null);

			this.SetColumnDescription (ColumnType.Budget, options.BudgetColumnDescription);
		}
	}
}
