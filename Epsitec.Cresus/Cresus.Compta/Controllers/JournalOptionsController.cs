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
#if false
			//	S'il n'y a qu'un journal, on ouvre le panneau en mode étendu, car la seule chose
			//	à faire est éventuellement d'en créer un.
			this.isExtended = (this.comptaEntity.Journaux.Count == 1);
#endif
		}


		public override void CreateUI(FrameBox parent, System.Action optionsChanged)
		{
			this.toolbar = new FrameBox
			{
				Parent              = parent,
				DrawFullFrame       = true,
				BackColor           = AbstractOptionsController.backColor,
				ContainerLayoutMode = Common.Widgets.ContainerLayoutMode.VerticalFlow,
				Dock                = DockStyle.Top,
				Margins             = new Margins (0, 0, 0, 6),
				Padding             = new Margins (5),
			};

			this.CreateComptactJournalUI (this.toolbar, optionsChanged);
			this.CreateExtendedJournalUI (this.toolbar, optionsChanged);
			this.CreateModeButtonUI (this.toolbar);
			this.UpdateMode ();
			this.UpdateSummary ();
		}

		public override void UpdateContent()
		{
			if (this.showPanel)
			{
				this.UpdateSummary ();
			}
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

			this.compactComboJournaux = new TextFieldCombo
			{
				Parent          = this.comptactFrame,
				PreferredWidth  = JournalOptionsController.JournauxWidth,
				PreferredHeight = 20,
				IsReadOnly      = true,
				Dock            = DockStyle.Left,
			};

			this.compactSummary = new StaticText
			{
				Parent  = this.comptactFrame,
				Dock    = DockStyle.Fill,
				Margins = new Margins (20, 0, 0, 0),
			};

			this.UpdateCombo ();

			this.compactComboJournaux.TextChanged += delegate
			{
				if (!this.ignoreChange)
				{
					var journal = this.comptaEntity.Journaux.Where (x => x.Name == this.compactComboJournaux.FormattedText).FirstOrDefault ();
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
				PreferredHeight = 15*5+29,
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
				Dock            = DockStyle.Fill,
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
			this.extendedListJournaux = new ScrollList
			{
				Parent  = centerFrame,
				Dock    = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 2),
			};

			this.extendedFieldName = new TextFieldEx
			{
				Parent                       = centerFrame,
				PreferredHeight              = 20,
				Dock                         = DockStyle.Bottom,
				DefocusAction                = DefocusAction.AutoAcceptOrRejectEdition,
				SwallowEscapeOnRejectEdition = true,
				SwallowReturnOnAcceptEdition = true,
			};

			//	Panneau de droite.
			{
				var frame = new FrameBox
				{
					Parent          = rightFrame,
					PreferredHeight = 20,
					Dock            = DockStyle.Top,
					Margins         = new Margins (0, 0, 0, 10),
				};

				this.extendedAddButton = new Button
				{
					Parent          = frame,
					Text            = "Nouveau",
					PreferredHeight = 20,
					PreferredWidth  = 80,
					Dock            = DockStyle.Left,
				};
			}

			{
				var frame = new FrameBox
				{
					Parent          = rightFrame,
					PreferredHeight = 20,
					Dock            = DockStyle.Top,
					Margins         = new Margins (0, 0, 0, 1),
				};

				this.extendedUpButton = new Button
				{
					Parent          = frame,
					Text            = "Monter",
					PreferredHeight = 20,
					PreferredWidth  = 80,
					Dock            = DockStyle.Left,
				};
			}

			{
				var frame = new FrameBox
				{
					Parent          = rightFrame,
					PreferredHeight = 20,
					Dock            = DockStyle.Top,
				};

				this.extendedDownButton = new Button
				{
					Parent          = frame,
					Text            = "Descendre",
					PreferredHeight = 20,
					PreferredWidth  = 80,
					Dock            = DockStyle.Left,
				};
			}

			{
				var frame = new FrameBox
				{
					Parent          = rightFrame,
					PreferredHeight = 20,
					Dock            = DockStyle.Bottom,
				};

				this.extendedRemoveButton = new Button
				{
					Parent          = frame,
					Text            = "Supprimer",
					PreferredHeight = 20,
					PreferredWidth  = 80,
					Dock            = DockStyle.Left,
				};

				this.extendedSummary = new StaticText
				{
					Parent  = frame,
					Dock    = DockStyle.Fill,
					Margins = new Margins (20, 0, 0, 0),
				};
			}

			ToolTip.Default.SetToolTip (this.extendedAddButton,    "Crée un nouveau journal pouvant contenir des écritures");
			ToolTip.Default.SetToolTip (this.extendedUpButton,     "Monte le journal d'une ligne dnas la liste");
			ToolTip.Default.SetToolTip (this.extendedDownButton,   "Descend le journal d'une ligne dnas la liste");
			ToolTip.Default.SetToolTip (this.extendedRemoveButton, "Supprime le journal (il ne contient aucune écriture)");

			//	Connexions.
			this.UpdateList ();
			this.UpdateButtons ();

			this.extendedListJournaux.SelectedItemChanged += delegate
			{
				if (!this.ignoreChange && this.extendedListJournaux.SelectedItemIndex != -1)
				{
					this.Options.Journal = this.comptaEntity.Journaux[this.extendedListJournaux.SelectedItemIndex];
					this.UpdateCombo ();
					this.UpdateList ();
					this.UpdateButtons ();
					this.UpdateSummary ();
					optionsChanged ();
				}
			};

			this.extendedFieldName.EditionAccepted += delegate
			{
				if (!this.ignoreChange)
				{
					this.Options.Journal.Name = this.extendedFieldName.FormattedText;
					this.UpdateCombo ();
					this.UpdateList ();
					optionsChanged ();
				}
			};

			this.extendedAddButton.Clicked += delegate
			{
				var nouveauJournal = new ComptaJournalEntity ();
				nouveauJournal.Name = this.NewJournalName;
				this.comptaEntity.Journaux.Add (nouveauJournal);

				this.Options.Journal = nouveauJournal;
				this.UpdateCombo ();
				this.UpdateList ();
				this.UpdateButtons ();
				this.UpdateSummary ();
				optionsChanged ();

				this.extendedFieldName.SelectAll ();
				this.extendedFieldName.Focus ();
			};

			this.extendedUpButton.Clicked += delegate
			{
				int sel = this.extendedListJournaux.SelectedItemIndex;

				var j1 = this.comptaEntity.Journaux[sel-1];
				var j2 = this.comptaEntity.Journaux[sel];

				this.comptaEntity.Journaux[sel-1] = j2;
				this.comptaEntity.Journaux[sel]   = j1;

				this.UpdateCombo ();
				this.UpdateList ();
				this.UpdateButtons ();
			};

			this.extendedDownButton.Clicked += delegate
			{
				int sel = this.extendedListJournaux.SelectedItemIndex;

				var j1 = this.comptaEntity.Journaux[sel+1];
				var j2 = this.comptaEntity.Journaux[sel];

				this.comptaEntity.Journaux[sel+1] = j2;
				this.comptaEntity.Journaux[sel]   = j1;

				this.UpdateCombo ();
				this.UpdateList ();
				this.UpdateButtons ();
			};

			this.extendedRemoveButton.Clicked += delegate
			{
				int sel = this.extendedListJournaux.SelectedItemIndex;
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
				this.Options.IsExtended = !this.Options.IsExtended;
				this.UpdateMode ();
			};
		}

		private void UpdateMode()
		{
			this.modeButton.GlyphShape = this.Options.IsExtended ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;
			this.modeButton.Margins = new Margins (0, 0, 0, this.Options.IsExtended ? 0 : 3);
			ToolTip.Default.SetToolTip (this.modeButton, this.Options.IsExtended ? "Réduit le panneau" : "Etend le panneau pour permettre de modifier la liste des journaux");

			this.comptactFrame.Visibility = !this.Options.IsExtended;
			this.extendedFrame.Visibility =  this.Options.IsExtended;
		}


		private void UpdateCombo()
		{
			this.compactComboJournaux.Items.Clear ();

			foreach (var journal in this.comptaEntity.Journaux)
			{
				this.compactComboJournaux.Items.Add (journal.Name);
			}

			this.ignoreChange = true;
			this.compactComboJournaux.FormattedText = this.Options.Journal.Name;
			this.ignoreChange = false;
		}

		private void UpdateList()
		{
			this.extendedListJournaux.Items.Clear ();

			foreach (var journal in this.comptaEntity.Journaux)
			{
				this.extendedListJournaux.Items.Add (journal.Name);
			}

			this.ignoreChange = true;
			this.extendedListJournaux.SelectedItemIndex = this.comptaEntity.Journaux.IndexOf (this.Options.Journal);
			this.extendedFieldName.FormattedText = this.Options.Journal.Name;
			this.ignoreChange = false;
		}

		private void UpdateButtons()
		{
			int sel = this.extendedListJournaux.SelectedItemIndex;
			int count = this.extendedListJournaux.Items.Count;
			int n = this.comptaEntity.GetJournalCount (this.Options.Journal);

			this.extendedUpButton.Enable     = (sel != -1 && sel > 0);
			this.extendedDownButton.Enable   = (sel != -1 && sel < count-1);
			this.extendedRemoveButton.Enable = (sel != -1 && count > 1 && n == 0);
		}

		private void UpdateSummary()
		{
			var summary = this.comptaEntity.GetJournalSummary (this.Options.Journal);

			this.compactSummary.Text = summary;
			this.extendedSummary.Text = summary;
		}


		private string NewJournalName
		{
			get
			{
				int i = 1;

				while (true)
				{
					string name = "Nouveau" + ((i == 1) ? "" : " " + i.ToString ());

					if (this.comptaEntity.Journaux.Where (x => x.Name == name).Any ())
					{
						i++;
					}
					else
					{
						return name;
					}
				}
			}
		}



		private new JournalOptions Options
		{
			get
			{
				return this.options as JournalOptions;
			}
		}


		private static readonly double JournauxWidth = 200;

		private GlyphButton				modeButton;

		private FrameBox				comptactFrame;
		private TextFieldCombo			compactComboJournaux;
		private StaticText				compactSummary;

		private FrameBox				extendedFrame;
		private ScrollList				extendedListJournaux;
		private TextFieldEx				extendedFieldName;
		private Button					extendedAddButton;
		private Button					extendedUpButton;
		private Button					extendedDownButton;
		private Button					extendedRemoveButton;
		private StaticText				extendedSummary;

		private bool					ignoreChange;
	}
}
