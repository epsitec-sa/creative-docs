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
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère le journal des écritures de la comptabilité.
	/// </summary>
	public class JournalController : AbstractController
	{
		public JournalController(ComptaApplication app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.dataAccessor = new JournalDataAccessor (this);
		}


		protected override void CreateTopOptions(FrameBox parent)
		{
			this.optionsController = new JournalOptionsController (this);
			this.optionsController.CreateUI (parent, this.OptionsChanged);
			this.optionsController.ShowPanel = this.ShowOptionsPanel;

			this.UpdateColumnMappers ();
		}

		protected override void OptionsChanged()
		{
			this.dataAccessor.UpdateAfterOptionsChanged ();
			this.ClearHilite ();
			this.UpdateColumnMappers ();
			this.UpdateArray ();
			this.editorController.UpdateEditorContent ();
			this.UpdateArrayContent ();
			this.FilterUpdateTopToolbar ();
			this.UpdateViewSettings ();
		}

#if false
		protected override void CreateTitleFrame()
		{
			this.titleLabel.Visibility = false;

			var label = new StaticText
			{
				Parent          = this.titleFrame,
				FormattedText   = FormattedText.Concat ("Journal").ApplyBold ().ApplyFontSize (13.0),
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 10, 0, 0),
			};
			label.PreferredWidth = label.GetBestFitSize ().Width;

			this.comboJournaux = new TextFieldCombo
			{
				Parent          = this.titleFrame,
				PreferredWidth  = JournalController.JournauxWidth,
				MenuButtonWidth = UIBuilder.ComboButtonWidth,
				IsReadOnly      = true,
				Dock            = DockStyle.Left,
				//?Margins         = new Margins (0, 0, 3, 3),
			};

			this.comboJournaux.TextLayout.DefaultFont = Font.GetFont (Font.DefaultFontFamily, "Bold");
			this.comboJournaux.TextLayout.DefaultFontSize = 13.0;
			this.comboJournaux.Furtive = true;
			this.comboJournaux.Button.Furtive = true;

			this.summaryLabel = new StaticText
			{
				Parent          = this.titleFrame,
				TextBreakMode   = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				Dock            = DockStyle.Fill,
				Margins         = new Margins (10, 0, 0, 0),
			};

			this.UpdateCombo ();
			this.UpdateSummary ();

			this.comboJournaux.SelectedItemChanged += delegate
			{
				if (this.ignoreChanges.IsZero && this.comboJournaux.SelectedItemIndex != -1)
				{
					if (this.comboJournaux.SelectedItemIndex == this.compta.Journaux.Count)  // tous les journaux ?
					{
						this.Options.JournalId = 0;
					}
					else
					{
						this.Options.JournalId = this.compta.Journaux[this.comboJournaux.SelectedItemIndex].Id;
					}

					this.OptionsChanged ();
					this.UpdateSummary ();
				}
			};
		}
#endif

		private void UpdateCombo()
		{
			this.comboJournaux.Items.Clear ();
			FormattedText sel = JournalController.AllJournaux;

			foreach (var journal in this.compta.Journaux)
			{
				this.comboJournaux.Items.Add (journal.Nom);

				if (journal.Id ==  this.Options.JournalId)
				{
					sel = journal.Nom;
				}
			}

			this.comboJournaux.Items.Add (JournalController.AllJournaux);
			this.comboJournaux.FormattedText = sel;
		}

		private void UpdateSummary()
		{
			var journal = this.compta.Journaux.Where (x => x.Id == this.Options.JournalId).FirstOrDefault ();
			this.summaryLabel.Text = this.période.GetJournalSummary (journal);
		}

		public override ControllerType ControllerType
		{
			get
			{
				return Controllers.ControllerType.Journal;
			}
		}


		protected override FormattedText GetArrayText(int row, ColumnType columnType)
		{
			//	Retourne le texte contenu dans une cellule.
			var text = this.dataAccessor.GetText (row, columnType);

			if (columnType == ColumnType.Date)
			{
				var accessor = this.dataAccessor as JournalDataAccessor;
				if (accessor.HasEmptyDate (row))
				{
					text = FormattedText.Empty;
				}
			}

			return text;
		}


		protected override void CreateEditor(FrameBox parent)
		{
			this.editorController = new JournalEditorController (this);
			this.editorController.CreateUI (parent, this.UpdateArrayContent);
			this.editorController.ShowInfoPanel = this.mainWindowController.ShowInfoPanel;
		}


		#region Context menu
		protected override VMenu ContextMenu
		{
			//	Retourne le menu contextuel à utiliser.
			get
			{
				var menu = new VMenu ();

				//?this.PutContextMenuCommand (menu, Res.Commands.Edit.Duplicate);
				//?this.PutContextMenuCommand (menu, Res.Commands.Edit.Delete);
				//?this.PutContextMenuCommand (menu, Res.Commands.Multi.Swap);
				//?this.PutContextMenuSeparator (menu);
				this.PutContextMenuExtrait (menu);

				return menu;
			}
		}

		private void PutContextMenuExtrait(VMenu menu)
		{
			var écriture = this.dataAccessor.GetEditionEntity (this.arrayController.SelectedRow) as ComptaEcritureEntity;

			if (écriture != null)
			{
				this.PutContextMenuExtrait (menu, écriture, écriture.Débit);
				this.PutContextMenuExtrait (menu, écriture, écriture.Crédit);
			}
		}

		private void PutContextMenuExtrait(VMenu menu, ComptaEcritureEntity écriture, ComptaCompteEntity compte)
		{
			if (compte != null)
			{
				var item = this.PutContextMenuItem (menu, Présentations.GetIcon (ControllerType.ExtraitDeCompte), string.Format ("Extrait du compte {0}", compte.Numéro));

				item.Clicked += delegate
				{
					var présentation = this.mainWindowController.ShowPrésentation (ControllerType.ExtraitDeCompte);

					var options = présentation.DataAccessor.Options as ExtraitDeCompteOptions;
					options.NuméroCompte = compte.Numéro;

					présentation.UpdateAfterChanged ();

					int row = (présentation.DataAccessor as ExtraitDeCompteDataAccessor).GetIndexOf (écriture);
					if (row != -1)
					{
						présentation.SelectedArrayLine = row;
					}
				};
			}
		}
		#endregion


		protected override IEnumerable<ColumnMapper> InitialColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.Date,   0.20, ContentAlignment.MiddleLeft, "Date",   "Date de l'écriture");
				yield return new ColumnMapper (ColumnType.Débit,  0.25, ContentAlignment.MiddleLeft, "Débit",  "Numéro ou nom du compte à débiter");
				yield return new ColumnMapper (ColumnType.Crédit, 0.25, ContentAlignment.MiddleLeft, "Crédit", "Numéro ou nom du compte à créditer");

				if (this.settingsList.GetBool (SettingsType.EcriturePièces))
				{
					bool enable = !this.settingsList.GetBool (SettingsType.EcritureForcePièces);
					yield return new ColumnMapper (ColumnType.Pièce, 0.20, ContentAlignment.MiddleLeft, "Pièce", "Numéro de la pièce comptable correspondant à l'écriture", enable: enable);
				}

				yield return new ColumnMapper (ColumnType.LibelléTVA, 0.00, ContentAlignment.MiddleRight, "Code TVA et taux");
				yield return new ColumnMapper (ColumnType.CodeTVA,    0.00, ContentAlignment.MiddleLeft,  "Code TVA", "Code TVA");
				yield return new ColumnMapper (ColumnType.TauxTVA,    0.00, ContentAlignment.MiddleLeft,  "Taux TVA", "Taux de la TVA");
																	  
				yield return new ColumnMapper (ColumnType.Libellé,    0.80, ContentAlignment.MiddleLeft,  "Libellé", "Libellé de l'écriture");
				yield return new ColumnMapper (ColumnType.MontantTTC, 0.25, ContentAlignment.MiddleRight, "Montant TTC", "Montant TTC");
				yield return new ColumnMapper (ColumnType.Montant,    0.25, ContentAlignment.MiddleRight, "Montant", "Montant de l'écriture");
//?				yield return new ColumnMapper (ColumnType.Monnaie,    0.15, ContentAlignment.MiddleLeft,  "Monnaie", "Monnaie de l'écriture");
				yield return new ColumnMapper (ColumnType.Journal,    0.25, ContentAlignment.MiddleLeft,  "Journal", "Journal auquel appartient l'écriture");
				yield return new ColumnMapper (ColumnType.OrigineTVA, 0.05, ContentAlignment.MiddleCenter, "", "Origine TVA", enable: false);
				yield return new ColumnMapper (ColumnType.Type,       0.05, ContentAlignment.MiddleCenter, "", "Type de la ligne", enable: false);
			}
		}

		protected override void UpdateColumnMappers()
		{
			this.ShowHideColumn (ColumnType.Journal,    this.Options.JournalId == 0);  // tous les journaux ?
			this.ShowHideColumn (ColumnType.Type,       this.settingsList.GetBool (SettingsType.EcritureMontreType));
			this.ShowHideColumn (ColumnType.OrigineTVA, this.settingsList.GetBool (SettingsType.EcritureMontreOrigineTVA));
			this.ShowHideColumn (ColumnType.MontantTTC, this.settingsList.GetBool (SettingsType.EcritureTVA));
		}

		private JournalOptions Options
		{
			get
			{
				return this.dataAccessor.Options as JournalOptions;
			}
		}


		private static readonly double JournauxWidth = 200;
		public static readonly string AllJournaux = "Tous les journaux";

		private TextFieldCombo			comboJournaux;
		private StaticText				summaryLabel; 
	}
}
