//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les options d'affichage du journal des écritures de la comptabilité.
	/// </summary>
	public class JournalOptionsController : AbstractOptionsController
	{
		public JournalOptionsController(ComptaEntity comptaEntity, JournalOptions options)
			: base (comptaEntity, options)
		{
		}


		public override void CreateUI(FrameBox parent, System.Action optionsChanged)
		{
			this.toolbar = new FrameBox
			{
				Parent              = parent,
				DrawFullFrame       = true,
				BackColor           = Color.FromBrightness (0.96),  // gris très clair
				ContainerLayoutMode = Common.Widgets.ContainerLayoutMode.VerticalFlow,
				Dock                = DockStyle.Top,
				Margins             = new Margins (0, 0, 0, 6),
				Padding             = new Margins (5),
			};

			this.CreateComptactJournalUI (this.toolbar, optionsChanged);
			this.CreateExtendedJournalUI (this.toolbar, optionsChanged);
			this.CreateModeButtonUI (this.toolbar);
			this.UpdateMode ();
		}

		private void CreateComptactJournalUI(FrameBox parent, System.Action optionsChanged)
		{
			this.comptactFrame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
			};

			new StaticText
			{
				Parent          = this.comptactFrame,
				Text            = "Choix du journal à utiliser",
				PreferredWidth  = 140,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
			};

			this.comboJournaux = new TextFieldCombo
			{
				Parent          = this.comptactFrame,
				PreferredWidth  = JournalOptionsController.JournauxWidth,
				PreferredHeight = 20,
				IsReadOnly      = true,
				Dock            = DockStyle.Left,
			};

			this.summary = new StaticText
			{
				Parent  = this.comptactFrame,
				Dock    = DockStyle.Fill,
				Margins = new Margins (20, 0, 0, 0),
			};

			this.UpdateCombo ();
			this.UpdateSummary ();

			this.comboJournaux.TextChanged += delegate
			{
				if (!this.ignoreChange)
				{
					var journal = this.comptaEntity.Journaux.Where (x => x.Name == this.comboJournaux.FormattedText).FirstOrDefault ();
					if (journal != null)
					{
						this.Options.Journal = journal;
						this.UpdateSummary ();
						this.UpdateList ();
						optionsChanged ();
					}
				}
			};
		}

		private void CreateExtendedJournalUI(FrameBox parent, System.Action optionsChanged)
		{
			this.extendedFrame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 147,
				Dock            = DockStyle.Top,
			};

			var leftFrame = new FrameBox
			{
				Parent          = this.extendedFrame,
				PreferredWidth  = 100,
				Dock            = DockStyle.Left,
			};

			var centerFrame = new FrameBox
			{
				Parent          = this.extendedFrame,
				PreferredWidth  = JournalOptionsController.JournauxWidth,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 20, 0, 0),
			};

			var rightFrame = new FrameBox
			{
				Parent          = this.extendedFrame,
				PreferredWidth  = 80,
				Dock            = DockStyle.Left,
			};

			//	Panneau de gauche.
			new StaticText
			{
				Parent          = leftFrame,
				Text            = "Liste des journaux",
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
			};

			//	Panneau du milieu.
			this.listJournaux = new ScrollList
			{
				Parent  = centerFrame,
				Dock    = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 2),
			};

			this.fieldName = new TextFieldEx
			{
				Parent                       = centerFrame,
				PreferredHeight              = 20,
				Dock                         = DockStyle.Bottom,
				DefocusAction                = DefocusAction.AutoAcceptOrRejectEdition,
				SwallowEscapeOnRejectEdition = true,
				SwallowReturnOnAcceptEdition = true,
			};

			//	Panneau de droite.
			this.addButton = new Button
			{
				Parent          = rightFrame,
				Text            = "Nouveau",
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 0, 10),
			};

			this.upButton = new Button
			{
				Parent          = rightFrame,
				Text            = "Monter",
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 0, 1),
			};

			this.downButton = new Button
			{
				Parent          = rightFrame,
				Text            = "Descendre",
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 0, 10),
			};

			this.removeButton = new Button
			{
				Parent          = rightFrame,
				Text            = "Supprimer",
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
			};

			//	Connexions.
			this.UpdateList ();
			this.UpdateButtons ();

			this.listJournaux.SelectedItemChanged += delegate
			{
				if (!this.ignoreChange && this.listJournaux.SelectedItemIndex != -1)
				{
					this.Options.Journal = this.comptaEntity.Journaux[this.listJournaux.SelectedItemIndex];
					this.UpdateCombo ();
					this.UpdateList ();
					this.UpdateSummary ();
					optionsChanged ();
				}
			};

			this.fieldName.EditionAccepted += delegate
			{
				if (!this.ignoreChange)
				{
					this.Options.Journal.Name = this.fieldName.FormattedText;
					this.UpdateCombo ();
					this.UpdateList ();
				}
			};

			this.addButton.Clicked += delegate
			{
				var nouveauJournal = new ComptaJournalEntity ();
				nouveauJournal.Name = "Nouveau";
				this.comptaEntity.Journaux.Add (nouveauJournal);

				this.Options.Journal = nouveauJournal;
				this.UpdateCombo ();
				this.UpdateList ();
				this.UpdateButtons ();
				this.UpdateSummary ();
				optionsChanged ();

				this.fieldName.SelectAll ();
				this.fieldName.Focus ();
			};

			this.upButton.Clicked += delegate
			{
				int sel = this.listJournaux.SelectedItemIndex;

				var j1 = this.comptaEntity.Journaux[sel-1];
				var j2 = this.comptaEntity.Journaux[sel];

				this.comptaEntity.Journaux[sel-1] = j2;
				this.comptaEntity.Journaux[sel]   = j1;

				this.UpdateCombo ();
				this.UpdateList ();
				this.UpdateButtons ();
			};

			this.downButton.Clicked += delegate
			{
				int sel = this.listJournaux.SelectedItemIndex;

				var j1 = this.comptaEntity.Journaux[sel+1];
				var j2 = this.comptaEntity.Journaux[sel];

				this.comptaEntity.Journaux[sel+1] = j2;
				this.comptaEntity.Journaux[sel]   = j1;

				this.UpdateCombo ();
				this.UpdateList ();
				this.UpdateButtons ();
			};

			this.removeButton.Clicked += delegate
			{
				int sel = this.listJournaux.SelectedItemIndex;
				this.comptaEntity.Journaux.RemoveAt (sel);

				sel = System.Math.Min (sel, this.comptaEntity.Journaux.Count-1);
				var journal = this.comptaEntity.Journaux[sel];

				this.Options.Journal = journal;
				this.UpdateCombo ();
				this.UpdateList ();
				this.UpdateButtons ();
				this.UpdateSummary ();
				optionsChanged ();
			};
		}


		private void CreateModeButtonUI(FrameBox parent)
		{
			this.modeButton = new GlyphButton
			{
				Parent        = parent,
				ButtonStyle   = ButtonStyle.ToolItem,
				PreferredSize = new Size (14, 14),
				Anchor        = AnchorStyles.BottomRight,
			};

			this.modeButton.Clicked += delegate
			{
				this.isExtended = !this.isExtended;
				this.UpdateMode ();
			};
		}

		private void UpdateMode()
		{
			this.modeButton.GlyphShape = this.isExtended ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;
			this.modeButton.Margins = new Margins (0, 0, 0, this.isExtended ? 0 : 3);
			ToolTip.Default.SetToolTip (this.modeButton, this.isExtended ? "Réduit le panneau" : "Etend le panneau pour permettre de modifier la liste des journaux");

			this.comptactFrame.Visibility = !this.isExtended;
			this.extendedFrame.Visibility =  this.isExtended;
		}


		private void UpdateCombo()
		{
			this.comboJournaux.Items.Clear ();

			foreach (var journal in this.comptaEntity.Journaux)
			{
				this.comboJournaux.Items.Add (journal.Name);
			}

			this.ignoreChange = true;
			this.comboJournaux.FormattedText = this.Options.Journal.Name;
			this.ignoreChange = false;
		}

		private void UpdateList()
		{
			this.listJournaux.Items.Clear ();

			foreach (var journal in this.comptaEntity.Journaux)
			{
				this.listJournaux.Items.Add (journal.Name);
			}

			this.ignoreChange = true;
			this.listJournaux.SelectedItemIndex = this.comptaEntity.Journaux.IndexOf (this.Options.Journal);
			this.fieldName.FormattedText = this.Options.Journal.Name;
			this.ignoreChange = false;
		}

		private void UpdateButtons()
		{
			int sel = this.listJournaux.SelectedItemIndex;
			int count = this.listJournaux.Items.Count;

			this.upButton.Enable     = (sel != -1 && sel > 0);
			this.downButton.Enable   = (sel != -1 && sel < count-1);
			this.removeButton.Enable = (sel != -1 && count > 1);
		}

		private void UpdateSummary()
		{
			this.summary.Text = this.comptaEntity.GetJournalSummary (this.Options.Journal);
		}



		private new JournalOptions Options
		{
			get
			{
				return this.options as JournalOptions;
			}
		}


		private static readonly double JournauxWidth = 250;

		private bool					isExtended;
		private GlyphButton				modeButton;

		private FrameBox				comptactFrame;
		private TextFieldCombo			comboJournaux;
		private StaticText				summary;

		private FrameBox				extendedFrame;
		private ScrollList				listJournaux;
		private TextFieldEx				fieldName;
		private Button					addButton;
		private Button					upButton;
		private Button					downButton;
		private Button					removeButton;

		private bool					ignoreChange;
	}
}
