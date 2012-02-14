//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Options.Controllers;
using Epsitec.Cresus.Compta.Fields.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère la ExtraitDeCompte de vérification de la comptabilité.
	/// </summary>
	public class ExtraitDeCompteController : AbstractController
	{
		public ExtraitDeCompteController(Application app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.dataAccessor = new ExtraitDeCompteDataAccessor (this);

			this.memoryList = this.mainWindowController.GetMemoryList ("Présentation.ExtraitDeCompte.Memory");
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
			this.optionsController = new ExtraitDeCompteOptionsController (this);
			this.optionsController.CreateUI (parent, this.OptionsChanged);
			this.optionsController.ShowPanel = this.ShowOptionsPanel;

			this.UpdateColumnMappers ();
		}

		protected override void OptionsChanged()
		{
			this.UpdateColumnMappers ();
			this.UpdateArray ();
			this.UpdateWindowTitle ();

			base.OptionsChanged ();
		}

		protected override void UpdateTitle()
		{
			var numéro = (this.dataAccessor.AccessorOptions as ExtraitDeCompteOptions).NuméroCompte;
			var compte = this.comptaEntity.PlanComptable.Where (x => x.Numéro == numéro).FirstOrDefault ();

			if (compte == null)
			{
				this.SetTitle (null);
			}
			else
			{
				this.SetTitle (Core.TextFormatter.FormatText ("Compte", compte.Numéro, compte.Titre));
			}

			this.SetSubtitle (this.périodeEntity.ShortTitle);
		}

		private void UpdateWindowTitle()
		{
			var numéro = (this.dataAccessor.AccessorOptions as ExtraitDeCompteOptions).NuméroCompte;
			var compte = this.comptaEntity.PlanComptable.Where (x => x.Numéro == numéro).FirstOrDefault ();

			if (compte == null)
			{
				this.mainWindowController.SetTitleComplement (null);
			}
			else
			{
				this.mainWindowController.SetTitleComplement (string.Concat (compte.Numéro, " ", compte.Titre));
			}
		}


		protected override FormattedText GetArrayText(int row, ColumnType columnType)
		{
			//	Retourne le texte contenu dans une cellule.
			var text = this.dataAccessor.GetText (row, columnType);
			var data = this.dataAccessor.GetReadOnlyData (row) as ExtraitDeCompteData;

			if (columnType == ColumnType.Solde &&
				row == this.dataAccessor.Count-2)  // total sur l'avant-dernière ligne ?
			{
				text = text.ApplyBold ();
			}

			return data.Typo (text);
		}


		protected override IEnumerable<ColumnMapper> InitialColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.Date,           0.20, ContentAlignment.MiddleLeft, "Date");
				yield return new ColumnMapper (ColumnType.CP,             0.20, ContentAlignment.MiddleLeft,  "C/P");
				yield return new ColumnMapper (ColumnType.Pièce,          0.20, ContentAlignment.MiddleLeft,  "Pièce");
				yield return new ColumnMapper (ColumnType.Libellé,        0.60, ContentAlignment.MiddleLeft,  "Libellé");
				yield return new ColumnMapper (ColumnType.Débit,          0.20, ContentAlignment.MiddleRight, "Débit");
				yield return new ColumnMapper (ColumnType.Crédit,         0.20, ContentAlignment.MiddleRight, "Crédit");
				yield return new ColumnMapper (ColumnType.Solde,          0.20, ContentAlignment.MiddleRight, "Solde");
				yield return new ColumnMapper (ColumnType.SoldeGraphique, 0.20, ContentAlignment.MiddleRight, "", hideForSearch: true);
				yield return new ColumnMapper (ColumnType.Journal,        0.20, ContentAlignment.MiddleLeft, "Journal");
			}
		}

		protected override void UpdateColumnMappers()
		{
			var options = this.dataAccessor.AccessorOptions as ExtraitDeCompteOptions;

			this.ShowHideColumn (ColumnType.SoldeGraphique, options.HasGraphics);
			this.ShowHideColumn (ColumnType.Journal,        this.comptaEntity.Journaux.Count > 1);
		}
	}
}
