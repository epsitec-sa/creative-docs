//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Memory.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Memory.Controllers
{
	/// <summary>
	/// Ce contrôleur gère la barre d'outil supérieure de mémoire pour la comptabilité.
	/// </summary>
	public class MemoryController
	{
		public MemoryController(AbstractController controller)
		{
			this.controller = controller;

			this.comptaEntity    = this.controller.ComptaEntity;
			this.dataAccessor    = this.controller.DataAccessor;
			this.businessContext = this.controller.BusinessContext;
			this.memoryList      = this.controller.MemoryList;

			this.showPanel = false;
			this.ignoreChanges = new SafeCounter ();
		}


		public bool ShowPanel
		{
			get
			{
				return this.showPanel;
			}
			set
			{
				if (this.showPanel != value)
				{
					this.showPanel = value;
					this.toolbar.Visibility = this.showPanel;

					if (this.showPanel)
					{
						//?this.searchController.SetFocus ();
					}
					else
					{
						//?this.searchController.SearchClear ();
					}
				}
			}
		}


		public void CreateUI(FrameBox parent, System.Action searchStartAction, System.Action<int> searchNextAction)
		{
			this.toolbar = new FrameBox
			{
				Parent              = parent,
				DrawFullFrame       = true,
				BackColor           = Color.FromHexa ("ffeecc"),  // orange pastel
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock                = DockStyle.Top,
				Margins             = new Margins (0, 0, 0, 5),
				Visibility          = false,
			};

			//	Crée les frames gauche, centrale et droite.
			this.mainFrame = new FrameBox
			{
				Parent         = this.toolbar,
				Dock           = DockStyle.Fill,
				Padding        = new Margins (5),
			};

			var levelFrame = new FrameBox
			{
				Parent         = this.toolbar,
				DrawFullFrame  = true,
				PreferredWidth = 20,
				Dock           = DockStyle.Right,
				Padding        = new Margins (5),
			};

			this.CreateComptactJournalUI (this.mainFrame);
			this.CreateExtendedJournalUI (this.mainFrame);

			//	Remplissage de la frame gauche.
			this.levelController = new LevelController (this.controller);
			this.levelController.CreateUI (levelFrame, "Remet ???", this.ClearAction, this.LevelChangedAction);

			this.UpdateWidgets ();
		}

		private void ClearAction()
		{
		}

		protected virtual void LevelChangedAction()
		{
			this.UpdateLevel ();
		}

		private void UpdateLevel()
		{
			this.comptactFrame.Visibility = !this.levelController.Specialist;
			this.extendedFrame.Visibility =  this.levelController.Specialist;

			this.levelController.Specialist = this.levelController.Specialist;
		}

		private void UpdateWidgets()
		{
			this.UpdateCombo ();
			this.UpdateList ();
			this.UpdateButtons ();
			this.UpdateLevel ();
			this.UpdateSummary ();
		}


		private void CreateComptactJournalUI(FrameBox parent)
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
				Text            = "Mémoire",
				PreferredWidth  = UIBuilder.LeftLabelWidth,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
			};

			this.compactComboJournaux = new TextFieldCombo
			{
				Parent          = this.comptactFrame,
				PreferredWidth  = MemoryController.JournauxWidth,
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

			this.compactComboJournaux.SelectedItemChanged += delegate
			{
				if (this.ignoreChanges.IsZero && this.compactComboJournaux.SelectedItemIndex != -1)
				{
				}
			};
		}

		private void CreateExtendedJournalUI(FrameBox parent)
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
				PreferredWidth  = UIBuilder.LeftLabelWidth,
				Dock            = DockStyle.Left,
			};

			var centerFrame = new FrameBox
			{
				Parent          = this.extendedFrame,
				PreferredWidth  = MemoryController.JournauxWidth,
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
				Text            = "Mémoires",
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

			ToolTip.Default.SetToolTip (this.extendedAddButton,    "Crée une nouvelle mémoire");
			ToolTip.Default.SetToolTip (this.extendedUpButton,     "Monte la mémoire d'une ligne dnas la liste");
			ToolTip.Default.SetToolTip (this.extendedDownButton,   "Descend la mémoire d'une ligne dnas la liste");
			ToolTip.Default.SetToolTip (this.extendedRemoveButton, "Supprime la mémoire");

			//	Connexions.
			this.UpdateList ();
			this.UpdateButtons ();

			this.extendedListJournaux.SelectedItemChanged += delegate
			{
				if (this.ignoreChanges.IsZero && this.extendedListJournaux.SelectedItemIndex != -1)
				{
				}
			};

			this.extendedFieldName.EditionAccepted += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
				}
			};

			this.extendedAddButton.Clicked += delegate
			{
				this.extendedFieldName.SelectAll ();
				this.extendedFieldName.Focus ();
			};

			this.extendedUpButton.Clicked += delegate
			{
				int sel = this.extendedListJournaux.SelectedItemIndex;

				this.UpdateCombo ();
				this.UpdateList ();
				this.UpdateButtons ();
			};

			this.extendedDownButton.Clicked += delegate
			{
				int sel = this.extendedListJournaux.SelectedItemIndex;

				this.UpdateCombo ();
				this.UpdateList ();
				this.UpdateButtons ();
			};

			this.extendedRemoveButton.Clicked += delegate
			{
				int sel = this.extendedListJournaux.SelectedItemIndex;
			};
		}


		private void UpdateCombo()
		{
			this.compactComboJournaux.Items.Clear ();

			foreach (var journal in this.comptaEntity.Journaux)
			{
				//?this.compactComboJournaux.Items.Add (journal.Name);
			}

			using (this.ignoreChanges.Enter ())
			{
				//?this.compactComboJournaux.FormattedText = (this.Options.Journal == null) ? MemoryController.AllJournaux : this.Options.Journal.Name;
			}
		}

		private void UpdateList()
		{
			this.extendedListJournaux.Items.Clear ();

			foreach (var journal in this.comptaEntity.Journaux)
			{
				//?this.extendedListJournaux.Items.Add (journal.Name);
			}

			using (this.ignoreChanges.Enter ())
			{
				//?this.extendedListJournaux.SelectedItemIndex = (this.Options.Journal == null) ? this.comptaEntity.Journaux.Count : this.comptaEntity.Journaux.IndexOf (this.Options.Journal);
				//?this.extendedFieldName.FormattedText = (this.Options.Journal == null) ? MemoryController.AllJournaux : this.Options.Journal.Name;
			}
		}

		private void UpdateButtons()
		{
			int sel = this.extendedListJournaux.SelectedItemIndex;
			int count = this.comptaEntity.Journaux.Count;
			bool allJournaux = (sel == count);

			this.extendedUpButton.Enable     = (!allJournaux && sel != -1 && sel > 0);
			this.extendedDownButton.Enable   = (!allJournaux && sel != -1 && sel < count-1);
			this.extendedRemoveButton.Enable = (!allJournaux && sel != -1 && count > 1);
		}

		private void UpdateSummary()
		{
			var summary = "Coucou";

			this.compactSummary.Text = summary;
			this.extendedSummary.Text = summary;
		}


		private static readonly double					toolbarHeight = 20;
		private static readonly double					JournauxWidth = 200;

		private readonly AbstractController				controller;
		private readonly ComptaEntity					comptaEntity;
		private readonly BusinessContext				businessContext;
		private readonly AbstractDataAccessor			dataAccessor;
		private readonly MemoryList						memoryList;
		private readonly SafeCounter					ignoreChanges;

		private FrameBox								mainFrame;
		private FrameBox								toolbar;
		private LevelController							levelController;
		private bool									showPanel;

		private FrameBox								comptactFrame;
		private TextFieldCombo							compactComboJournaux;
		private StaticText								compactSummary;

		private FrameBox								extendedFrame;
		private ScrollList								extendedListJournaux;
		private TextFieldEx								extendedFieldName;
		private Button									extendedAddButton;
		private Button									extendedUpButton;
		private Button									extendedDownButton;
		private Button									extendedRemoveButton;
		private StaticText								extendedSummary;
	}
}
