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
using Epsitec.Cresus.Compta.Permanents.Data;
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

			this.viewSettingsList = this.mainWindowController.GetViewSettingsList ("Présentation.Journal.ViewSettings");
		}


		protected override void CreateOptions(FrameBox parent)
		{
			this.optionsController = new JournalOptionsController (this);
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
			this.editorController.UpdateEditorContent ();
			this.UpdateArrayContent ();
			this.UpdateTitle ();
			this.FilterUpdateTopToolbar ();
			this.UpdateViewSettings ();
		}

		protected override void UpdateTitle()
		{
			int id = (this.dataAccessor.Options as JournalOptions).JournalId;
			var journal = this.ComptaEntity.Journaux.Where (x => x.Id == id).FirstOrDefault ();

			if (journal == null)  // tous les journaux ?
			{
				var name = Core.TextFormatter.FormatText (JournalOptionsController.AllJournaux).ApplyFontColor (Color.FromName ("Red"));
				this.SetTitle (name);
			}
			else
			{
				this.SetTitle (Core.TextFormatter.FormatText ("Journal", journal.Nom));
			}

			this.SetSubtitle (this.période.ShortTitle);
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
				return true;
			}
		}


		protected override FormattedText GetArrayText(int row, ColumnType columnType)
		{
			//	Retourne le texte contenu dans une cellule.
			return this.dataAccessor.GetText (row, columnType);
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
				var item = this.PutContextMenuItem (menu, "Présentation.Extrait", string.Format ("Extrait du compte {0}", compte.Numéro));

				item.Clicked += delegate
				{
					var présentation = this.mainWindowController.ShowPrésentation (Res.Commands.Présentation.Extrait);

					var permanent = présentation.DataAccessor.Permanents as ExtraitDeComptePermanents;
					permanent.NuméroCompte = compte.Numéro;

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
				yield return new ColumnMapper (ColumnType.Journal,    0.25, ContentAlignment.MiddleLeft,  "Journal", "Journal auquel appartient l'écriture");
				yield return new ColumnMapper (ColumnType.OrigineTVA, 0.05, ContentAlignment.MiddleCenter, "", "Origine TVA", enable: false);
				yield return new ColumnMapper (ColumnType.Type,       0.05, ContentAlignment.MiddleCenter, "", "Type de la ligne", enable: false);
			}
		}

		protected override void UpdateColumnMappers()
		{
			var options = this.dataAccessor.Options as JournalOptions;

			this.ShowHideColumn (ColumnType.Journal, options.JournalId == 0);  // tous les journaux ?
			this.ShowHideColumn (ColumnType.Type, this.settingsList.GetBool (SettingsType.EcritureMontreType));
			this.ShowHideColumn (ColumnType.OrigineTVA, this.settingsList.GetBool (SettingsType.EcritureMontreOrigineTVA));
		}
	}
}
