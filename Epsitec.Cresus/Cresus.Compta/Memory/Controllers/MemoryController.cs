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


		public void CreateUI(FrameBox parent, System.Action memoryChangedAction)
		{
			this.memoryChangedAction = memoryChangedAction;

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

			this.CreateComptactMemoryUI (this.mainFrame);
			this.CreateExtendedMemoryUI (this.mainFrame);

			//	Remplissage de la frame gauche.
			this.levelController = new LevelController (this.controller);
			this.levelController.CreateUI (levelFrame, "Remet ???", this.ClearAction, this.LevelChangedAction);

			this.UpdateWidgets ();
		}

		private void ClearAction()
		{
		}

		private void LevelChangedAction()
		{
			this.UpdateLevel ();
		}

		private void MemoryChanged()
		{
			this.UpdateWidgets ();

			var memory = this.memoryList.Selected;
			if (memory != null)
			{
				this.CopyMemoryToData (memory);
				this.memoryChangedAction ();
			}
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


		private void CreateComptactMemoryUI(FrameBox parent)
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
				PreferredWidth  = MemoryController.fieldWidth,
				PreferredHeight = 20,
				IsReadOnly      = true,
				Dock            = DockStyle.Left,
			};

			this.compactSummary = new StaticText
			{
				Parent        = this.comptactFrame,
				TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				Dock          = DockStyle.Fill,
				Margins       = new Margins (20, 0, 0, 0),
			};

			this.UpdateCombo ();

			this.compactComboJournaux.SelectedItemChanged += delegate
			{
				if (this.ignoreChanges.IsZero && this.compactComboJournaux.SelectedItemIndex != -1)
				{
					this.memoryList.SelectedIndex = this.compactComboJournaux.SelectedItemIndex;
					this.MemoryChanged ();
				}
			};
		}

		private void CreateExtendedMemoryUI(FrameBox parent)
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
				PreferredWidth  = MemoryController.fieldWidth,
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
				TextBreakMode   = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				PreferredWidth  = UIBuilder.LeftLabelWidth,
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
					Text            = "Conserver",
					PreferredHeight = 20,
					PreferredWidth  = 80,
					Dock            = DockStyle.Left,
				};

				this.extendedUpdateButton = new Button
				{
					Parent          = frame,
					Text            = "Mettre à jour",
					PreferredHeight = 20,
					PreferredWidth  = 80,
					Dock            = DockStyle.Left,
					Margins         = new Margins (10, 0, 0, 0),
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
					Parent        = frame,
					TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
					Dock          = DockStyle.Fill,
					Margins       = new Margins (20, 0, 0, 0),
				};
			}

			ToolTip.Default.SetToolTip (this.extendedAddButton,    "Conserve la recherche, le filtre et les options dans une nouvelle mémoire");
			ToolTip.Default.SetToolTip (this.extendedUpdateButton, "Met à jour la mémoire d'après la recherche, le filtre et les options");
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
					this.memoryList.SelectedIndex = this.extendedListJournaux.SelectedItemIndex;
					this.MemoryChanged ();
				}
			};

			this.extendedFieldName.EditionAccepted += delegate
			{
				if (this.ignoreChanges.IsZero && this.memoryList.Selected != null)
				{
					this.memoryList.Selected.Name = this.extendedFieldName.FormattedText;
					this.UpdateCombo ();
					this.UpdateList ();
				}
			};

			this.extendedAddButton.Clicked += delegate
			{
				var memory = new MemoryData ();
				memory.Name = this.NewMemoryName;
				this.CopyDataToMemory (memory);

				this.memoryList.List.Add (memory);
				this.memoryList.Selected = memory;

				this.UpdateCombo ();
				this.UpdateList ();
				this.UpdateButtons ();
				this.UpdateSummary ();

				this.extendedFieldName.SelectAll ();
				this.extendedFieldName.Focus ();
			};

			this.extendedUpdateButton.Clicked += delegate
			{
			};

			this.extendedUpButton.Clicked += delegate
			{
				int sel = this.extendedListJournaux.SelectedItemIndex;

				var m1 = this.memoryList.List[sel-1];
				var m2 = this.memoryList.List[sel];

				this.memoryList.List[sel-1] = m2;
				this.memoryList.List[sel]   = m1;

				this.UpdateCombo ();
				this.UpdateList ();
				this.UpdateButtons ();
			};

			this.extendedDownButton.Clicked += delegate
			{
				int sel = this.extendedListJournaux.SelectedItemIndex;

				var m1 = this.memoryList.List[sel+1];
				var m2 = this.memoryList.List[sel];

				this.memoryList.List[sel+1] = m2;
				this.memoryList.List[sel]   = m1;

				this.UpdateCombo ();
				this.UpdateList ();
				this.UpdateButtons ();
			};

			this.extendedRemoveButton.Clicked += delegate
			{
				int sel = this.extendedListJournaux.SelectedItemIndex;
				this.memoryList.List.RemoveAt (sel);

				sel = System.Math.Min (sel, this.memoryList.List.Count-1);
				var memory = this.memoryList.List[sel];

				this.memoryList.Selected = memory;
				this.MemoryChanged ();
			};
		}


		private void UpdateCombo()
		{
			using (this.ignoreChanges.Enter ())
			{
				this.compactComboJournaux.Items.Clear ();

				foreach (var memory in this.memoryList.List)
				{
					this.compactComboJournaux.Items.Add (memory.Name);
				}

				this.compactComboJournaux.Enable = this.memoryList.List.Any ();

				if (this.memoryList.Selected == null)
				{
					this.compactComboJournaux.FormattedText = FormattedText.Empty;
				}
				else
				{
					this.compactComboJournaux.FormattedText = this.memoryList.Selected.Name;
				}
			}
		}

		private void UpdateList()
		{
			using (this.ignoreChanges.Enter ())
			{
				this.extendedListJournaux.Items.Clear ();

				foreach (var memory in this.memoryList.List)
				{
					this.extendedListJournaux.Items.Add (memory.Name);
				}

				if (this.memoryList.Selected == null)
				{
					this.extendedListJournaux.SelectedItemIndex = -1;
					this.extendedFieldName.FormattedText = FormattedText.Empty;
				}
				else
				{
					this.extendedListJournaux.SelectedItemIndex = this.memoryList.SelectedIndex;
					this.extendedFieldName.FormattedText = this.memoryList.Selected.Name;
				}
			}
		}

		private void UpdateButtons()
		{
			int sel = this.extendedListJournaux.SelectedItemIndex;
			int count = this.memoryList.List.Count;

			this.extendedUpButton.Enable     = (sel != -1 && sel > 0);
			this.extendedUpdateButton.Enable = (sel != -1);
			this.extendedDownButton.Enable   = (sel != -1 && sel < count-1);
			this.extendedRemoveButton.Enable = (sel != -1);
		}

		private void UpdateSummary()
		{
			var summary = FormattedText.Empty;
			var memory = this.memoryList.Selected;

			if (memory != null)
			{
				summary = memory.GetSummary (this.controller.ColumnMappers);
			}

			this.compactSummary.FormattedText  = summary;
			this.extendedSummary.FormattedText = summary;
		}


		private void CopyDataToMemory(MemoryData memory)
		{
			if (this.dataAccessor != null && this.dataAccessor.SearchData != null)
			{
				memory.Search = new SearchData ();
				this.dataAccessor.SearchData.CopyTo (memory.Search);
			}

			if (this.dataAccessor != null && this.dataAccessor.FilterData != null)
			{
				memory.Filter = new SearchData ();
				this.dataAccessor.FilterData.CopyTo (memory.Filter);
			}
		}

		private void CopyMemoryToData(MemoryData memory)
		{
			if (memory.Search != null)
			{
				memory.Search.CopyTo (this.dataAccessor.SearchData);
			}

			if (memory.Filter != null)
			{
				memory.Filter.CopyTo (this.dataAccessor.FilterData);
			}
		}


		private string NewMemoryName
		{
			get
			{
				int i = 1;

				while (true)
				{
					string name = "Nouveau" + ((i == 1) ? "" : " " + i.ToString ());

					if (this.memoryList.List.Where (x => x.Name == name).Any ())
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

	
		private static readonly double					fieldWidth = 200;

		private readonly AbstractController				controller;
		private readonly ComptaEntity					comptaEntity;
		private readonly BusinessContext				businessContext;
		private readonly AbstractDataAccessor			dataAccessor;
		private readonly MemoryList						memoryList;
		private readonly SafeCounter					ignoreChanges;

		private System.Action							memoryChangedAction;

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
		private Button									extendedUpdateButton;
		private Button									extendedUpButton;
		private Button									extendedDownButton;
		private Button									extendedRemoveButton;
		private StaticText								extendedSummary;
	}
}
