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
	/// Ce contrôleur gère le résumé TVA de la comptabilité.
	/// </summary>
	public class RésuméTVAController : AbstractController
	{
		public RésuméTVAController(ComptaApplication app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.dataAccessor = new RésuméTVADataAccessor (this);

			this.viewSettingsList = this.mainWindowController.GetViewSettingsList ("Présentation.RésuméTVA.ViewSettings");
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
			this.optionsController = new RésuméTVAOptionsController (this);
			this.optionsController.CreateUI (parent, this.OptionsChanged);
			this.optionsController.ShowPanel = this.mainWindowController.ShowOptionsPanel;

			this.UpdateColumnMappers ();
		}

		protected override void OptionsChanged()
		{
			this.dataAccessor.UpdateAfterOptionsChanged ();
			this.ClearHilite ();
			this.UpdateColumnMappers ();
			this.UpdateArray ();

			this.UpdateArrayContent ();
			this.UpdateTitle ();
			this.FilterUpdateTopToolbar ();
			this.UpdateViewSettings ();
		}

		protected override void UpdateTitle()
		{
			this.SetTitle ("Résumé TVA");
			this.SetSubtitle (this.période.ShortTitle);
		}


		protected override FormattedText GetArrayText(int row, ColumnType columnType)
		{
			//	Retourne le texte contenu dans une cellule.
			var text = this.dataAccessor.GetText (row, columnType);
			var data = this.dataAccessor.GetReadOnlyData (row) as RésuméTVAData;

			if (columnType == ColumnType.Solde)
			{
				if (!data.NeverFiltered)
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
				yield return new ColumnMapper (ColumnType.Compte,     0.20, ContentAlignment.MiddleLeft,  "Compte");
				yield return new ColumnMapper (ColumnType.CodeTVA,    0.20, ContentAlignment.MiddleLeft,  "Code TVA");
				yield return new ColumnMapper (ColumnType.TauxTVA,    0.20, ContentAlignment.MiddleLeft,  "Taux");
				yield return new ColumnMapper (ColumnType.Date,       0.20, ContentAlignment.MiddleLeft,  "Date");
				yield return new ColumnMapper (ColumnType.Pièce,      0.20, ContentAlignment.MiddleLeft,  "Pièce");
				yield return new ColumnMapper (ColumnType.Titre,      1.00, ContentAlignment.MiddleLeft,  "Code TVA / Titre du compte");
				yield return new ColumnMapper (ColumnType.Montant,    0.20, ContentAlignment.MiddleRight, "Montant HT");
				yield return new ColumnMapper (ColumnType.MontantTVA, 0.20, ContentAlignment.MiddleRight, "TVA");
			}
		}

		protected override void UpdateColumnMappers()
		{
			var options = this.dataAccessor.Options as RésuméTVAOptions;

			this.ShowHideColumn (ColumnType.CodeTVA, options.MontreEcritures);
			this.ShowHideColumn (ColumnType.TauxTVA, options.MontreEcritures);
			this.ShowHideColumn (ColumnType.Date,    options.MontreEcritures);
			this.ShowHideColumn (ColumnType.Pièce,   options.MontreEcritures);

			this.SetColumnDescription (ColumnType.Titre,   options.MontreEcritures ? "Libellé" : "Code TVA / Titre du compte");
			this.SetColumnDescription (ColumnType.Montant, options.MontantTTC      ? "Montant TTC" : "Montant HT");
		}
	}
}
