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
using Epsitec.Cresus.Compta.Permanents;
using Epsitec.Cresus.Compta.Fields.Controllers;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère la ExtraitDeCompte de vérification de la comptabilité.
	/// </summary>
	public class ExtraitDeCompteController : AbstractController
	{
		public ExtraitDeCompteController(ComptaApplication app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.dataAccessor = new ExtraitDeCompteDataAccessor (this);

			this.UpdateColumnMappers ();
		}


		public override bool HasGraph
		{
			get
			{
				return true;
			}
		}

		public override bool HasSearchPanel
		{
			get
			{
				return true;
			}
		}

		public override bool HasFilterPanel
		{
			get
			{
				return true;
			}
		}

		public override bool HasOptionsPanel
		{
			get
			{
				return true;
			}
		}

		public override bool HasInfoPanel
		{
			get
			{
				return true;
			}
		}


		protected override void CreateOptions(FrameBox parent)
		{
			this.optionsController = new ExtraitDeCompteOptionsController (this);
			this.optionsController.CreateUI (parent, this.OptionsChanged);
			this.optionsController.ShowPanel = this.mainWindowController.ShowOptionsPanel;

			this.UpdateColumnMappers ();
		}

		protected override void OptionsChanged()
		{
			this.UpdateColumnMappers ();
			this.UpdateArray ();
			this.UpdateWindowTitle ();

			base.OptionsChanged ();
		}

		protected override void CreateTitleFrame()
		{
#if false
			this.titleLabel.Visibility = false;

			var label = new StaticText
			{
				Parent          = this.titleFrame,
				FormattedText   = FormattedText.Concat ("Compte").ApplyBold ().ApplyFontSize (13.0),
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 10, 0, 0),
			};
			label.PreferredWidth = label.GetBestFitSize ().Width;

			this.compteController = UIBuilder.CreateAutoCompleteField (this, this.titleFrame, this.NuméroCompte, "Compte", this.ValidateCompteAction, this.CompteChangedAction);
			this.compteController.Furtive = true;
			this.compteController.Box.Dock = DockStyle.Left;
			//?this.compteController.Box.Margins = new Margins (0, 0, 3, 3);
			this.compteController.EditWidget.TextLayout.DefaultFont = Font.GetFont (Font.DefaultFontFamily, "Bold");
			this.compteController.EditWidget.TextLayout.DefaultFontSize = 13.0;

			this.summaryLabel = new StaticText
			{
				Parent          = this.titleFrame,
				TextBreakMode   = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				Dock            = DockStyle.Fill,
				Margins         = new Margins (10, 0, 0, 0),
			};

			this.UpdateComptes ();
			this.UpdateSummary ();
#else
			if (this.viewSettingsController == null)
			{
				return;
			}

			var frame = this.viewSettingsController.GetTitleFrame ();

			var label = new StaticText
			{
				Parent          = frame,
				FormattedText   = FormattedText.Concat ("Compte").ApplyBold ().ApplyFontSize (13.0),
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 10, 0, 0),
			};
			label.PreferredWidth = label.GetBestFitSize ().Width;

			this.compteController = UIBuilder.CreateAutoCompleteField (this, frame, this.NuméroCompte, "Compte", this.ValidateCompteAction, this.CompteChangedAction);
			this.compteController.Furtive = true;
			this.compteController.Box.Dock = DockStyle.Left;
			//?this.compteController.Box.Margins = new Margins (0, 0, 3, 3);
			this.compteController.EditWidget.TextLayout.DefaultFont = Font.GetFont (Font.DefaultFontFamily, "Bold");
			this.compteController.EditWidget.TextLayout.DefaultFontSize = 13.0;

			this.summaryLabel = new StaticText
			{
				Parent          = frame,
				TextBreakMode   = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				Dock            = DockStyle.Left,
				Margins         = new Margins (10, 0, 0, 0),
			};

			this.UpdateComptes ();
			this.UpdateSummary ();
#endif
		}

		private void UpdateComptes()
		{
			UIBuilder.UpdateAutoCompleteTextField (this.compteController.EditWidget as AutoCompleteTextField, this.compta.PlanComptable);

			using (this.ignoreChanges.Enter ())
			{
				this.compteController.EditionData.Text = this.NuméroCompte;
			}
		}

		private void UpdateSummary()
		{
			var compte = this.compta.PlanComptable.Where (x => x.Numéro == this.NuméroCompte).FirstOrDefault ();

			if (compte == null)
			{
				this.summaryLabel.FormattedText = FormattedText.Concat ("Inconnu").ApplyBold ().ApplyFontSize (13.0).ApplyFontColor (Color.FromName ("Red"));
			}
			else
			{
				this.summaryLabel.FormattedText = compte.Titre.ApplyBold ().ApplyFontSize (13.0);
			}

			this.summaryLabel.PreferredWidth = System.Math.Min (this.summaryLabel.GetBestFitSize ().Width, 150);
		}

		private void ValidateCompteAction(EditionData data)
		{
			data.ClearError ();
		}

		private void CompteChangedAction(int line, ColumnType columnType)
		{
			if (this.ignoreChanges.IsZero)
			{
				this.NuméroCompte = this.compteController.EditionData.Text;
				this.UpdateSummary ();
				this.OptionsChanged ();
			}
		}

		private FormattedText NuméroCompte
		{
			get
			{
				var p = this.dataAccessor.Permanents as ExtraitDeComptePermanents;
				return p.NuméroCompte.ToSimpleText ();
			}
			set
			{
				var p = this.dataAccessor.Permanents as ExtraitDeComptePermanents;
				p.NuméroCompte = value;
			}
		}

		protected override void UpdateTitle()
		{
			var numéro = (this.dataAccessor.Permanents as ExtraitDeComptePermanents).NuméroCompte;
			var compte = this.compta.PlanComptable.Where (x => x.Numéro == numéro).FirstOrDefault ();

			if (compte == null)
			{
				this.SetGroupTitle (Présentations.GetGroupName (ControllerType.Extrait));
			}
			else
			{
				this.SetGroupTitle (Core.TextFormatter.FormatText ("Compte", compte.Numéro, compte.Titre));
			}

			this.SetSubtitle (this.période.ShortTitle);
		}

		private void UpdateWindowTitle()
		{
			var numéro = (this.dataAccessor.Permanents as ExtraitDeComptePermanents).NuméroCompte;
			var compte = this.compta.PlanComptable.Where (x => x.Numéro == numéro).FirstOrDefault ();

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


		protected override void CreateEditor(FrameBox parent)
		{
			this.editorController = new ExtraitDeCompteEditorController (this);
			this.editorController.CreateUI (parent, this.UpdateArrayContent);
		}


		#region Context menu
		protected override VMenu ContextMenu
		{
			//	Retourne le menu contextuel à utiliser.
			get
			{
				var menu = new VMenu ();

				this.PutContextMenuCP (menu);
				this.PutContextMenuJournal (menu);

				return menu;
			}
		}

		private void PutContextMenuCP(VMenu menu)
		{
			var data = this.dataAccessor.GetReadOnlyData (this.arrayController.SelectedRow) as ExtraitDeCompteData;

			var enable = (data != null && data.CP != null);
			var text = enable ? string.Format ("Montre la contrepartie {0}", data.CP.Numéro) : "Montre la contrepartie";
			var item = this.PutContextMenuItem (menu, "Présentation.Extrait", text, enable);

			item.Clicked += delegate
			{
				var présentation = this.mainWindowController.ShowPrésentation (ControllerType.Extrait);

				var permanent = présentation.DataAccessor.Permanents as ExtraitDeComptePermanents;
				permanent.NuméroCompte = data.CP.Numéro;

				présentation.UpdateAfterChanged ();
			};
		}

		private void PutContextMenuJournal(VMenu menu)
		{
			var data = this.dataAccessor.GetReadOnlyData (this.arrayController.SelectedRow) as ExtraitDeCompteData;
			var écriture = data.Entity as ComptaEcritureEntity;

			var item = this.PutContextMenuItem (menu, "Présentation.Journal", "Montre dans le journal", écriture != null);

			item.Clicked += delegate
			{
				var présentation = this.mainWindowController.ShowPrésentation (ControllerType.Journal);

				int row = (présentation.DataAccessor as JournalDataAccessor).GetIndexOf (écriture);
				if (row != -1)
				{
					présentation.SelectedArrayLine = row;
				}
			};
		}
		#endregion


		protected override IEnumerable<ColumnMapper> InitialColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.Date,           0.20, ContentAlignment.MiddleLeft,  "Date",       "Date de l'écriture");
				yield return new ColumnMapper (ColumnType.CP,             0.20, ContentAlignment.MiddleLeft,  "C/P",        "Numéro ou nom du compte de la contrepartie");
				yield return new ColumnMapper (ColumnType.Pièce,          0.20, ContentAlignment.MiddleLeft,  "Pièce",      "Numéro de la pièce comptable correspondant à l'écriture");
				yield return new ColumnMapper (ColumnType.Libellé,        0.60, ContentAlignment.MiddleLeft,  "Libellé",    "Libellé de l'écriture");
				yield return new ColumnMapper (ColumnType.Débit,          0.20, ContentAlignment.MiddleRight, "Débit",      "Montant de l'écriture");
				yield return new ColumnMapper (ColumnType.Crédit,         0.20, ContentAlignment.MiddleRight, "Crédit",     "Montant de l'écriture");
				yield return new ColumnMapper (ColumnType.CodeTVA,        0.20, ContentAlignment.MiddleLeft,  "Code TVA",   edition: false);
				yield return new ColumnMapper (ColumnType.TauxTVA,        0.15, ContentAlignment.MiddleRight, "Taux",       edition: false);
				yield return new ColumnMapper (ColumnType.CompteTVA,      0.25, ContentAlignment.MiddleLeft,  "Compte TVA", edition: false);
				yield return new ColumnMapper (ColumnType.Journal,        0.20, ContentAlignment.MiddleLeft,  "Journal",    "Journal auquel appartient l'écriture");
				yield return new ColumnMapper (ColumnType.Solde,          0.20, ContentAlignment.MiddleRight, "Solde",      edition: false);
				yield return new ColumnMapper (ColumnType.SoldeGraphique, 0.20, ContentAlignment.MiddleRight, "",           edition: false, hideForSearch: true);
			}
		}

		protected override void UpdateColumnMappers()
		{
			var options = this.dataAccessor.Options as ExtraitDeCompteOptions;

			this.ShowHideColumn (ColumnType.SoldeGraphique, options.HasGraphics);
			this.ShowHideColumn (ColumnType.Journal,        this.compta.Journaux.Count > 1);

#if false
			bool hasTVA    = this.settingsList.GetBool (SettingsType.EcritureTVA);
			bool compteTVA = this.settingsList.GetBool (SettingsType.EcritureMontreCompteTVA) && hasTVA;
#else
			bool hasTVA    = false;
			bool compteTVA = false;
#endif

			this.ShowHideColumn (ColumnType.CodeTVA,   hasTVA);
			this.ShowHideColumn (ColumnType.TauxTVA,   hasTVA);
			this.ShowHideColumn (ColumnType.CompteTVA, compteTVA);
		}


		private AutoCompleteFieldController		compteController;
		private StaticText						summaryLabel;
	}
}
