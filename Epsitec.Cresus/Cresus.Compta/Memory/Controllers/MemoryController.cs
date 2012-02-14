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
						if (this.levelController.Specialist)
						{
							this.extendedListJournaux.Focus ();
						}
						else
						{
							this.compactComboJournaux.Focus ();
						}
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
				BackColor           = UIBuilder.MemoryBackColor,
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
			this.levelController.CreateUI (levelFrame, "Termine la recherche, le fitre et les options", this.ClearAction, this.LevelChangedAction);

			this.UpdateWidgets ();
		}

		private void ClearAction()
		{
			this.memoryList.Selected = null;

			if (this.dataAccessor != null)
			{
				if (this.dataAccessor.SearchData != null)
				{
					this.dataAccessor.SearchData.Clear ();
				}

				if (this.dataAccessor.FilterData != null)
				{
					this.dataAccessor.FilterData.Clear ();
				}

				if (this.dataAccessor.AccessorOptions != null)
				{
					this.dataAccessor.AccessorOptions.Clear ();
				}
			}

			this.UpdateWidgets ();
			this.MemoryChanged ();
		}

		private void LevelChangedAction()
		{
			this.UpdateLevel ();
		}

		private void MemoryChanged()
		{
			var memory = this.memoryList.Selected;
			if (memory != null)
			{
				this.CopyMemoryToData (memory);
				this.memoryChangedAction ();
			}

			this.UpdateWidgets ();
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

		public void Update()
		{
			this.UpdateButtons ();
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
				Text            = "Style",
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

			this.compactUseButton = new IconButton
			{
				Parent            = this.comptactFrame,
				IconUri           = UIBuilder.GetResourceIconUri ("Memory.Use"),
				PreferredIconSize = new Size (20, 20),
				PreferredSize     = new Size (20, 20),
				Dock              = DockStyle.Left,
				Margins           = new Margins (10, 0, 0, 0),
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

			this.compactUseButton.Clicked += delegate
			{
				this.MemoryChanged ();
			};
		}

		private void CreateExtendedMemoryUI(FrameBox parent)
		{
			this.extendedFrame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 15*6+29,
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
				Text            = "Styles",
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
				var toolbar = new FrameBox
				{
					Parent          = rightFrame,
					PreferredHeight = 20,
					Dock            = DockStyle.Top,
				};

				this.extendedAddButton = new IconButton
				{
					Parent          = toolbar,
					IconUri         = UIBuilder.GetResourceIconUri ("Memory.Add"),
					PreferredSize   = new Size (40, 40),
					Dock            = DockStyle.Left,
				};

				this.extendedUpdateButton = new IconButton
				{
					Parent          = toolbar,
					IconUri         = UIBuilder.GetResourceIconUri ("Memory.Update"),
					PreferredSize   = new Size (40, 40),
					Dock            = DockStyle.Left,
					Margins         = new Margins (2, 0, 0, 0),
				};

				this.extendedUseButton = new IconButton
				{
					Parent          = toolbar,
					IconUri         = UIBuilder.GetResourceIconUri ("Memory.Use"),
					PreferredSize   = new Size (40, 40),
					Dock            = DockStyle.Left,
					Margins         = new Margins (2, 0, 0, 0),
				};

				{
					var upDown = new FrameBox
					{
						Parent          = toolbar,
						PreferredSize   = new Size (20, 40),
						Dock            = DockStyle.Left,
						Margins         = new Margins (20, 0, 0, 0),
					};

					this.extendedUpButton = new IconButton
					{
						Parent          = upDown,
						IconUri         = UIBuilder.GetResourceIconUri ("Memory.Up"),
						PreferredSize   = new Size (20, 20),
						Dock            = DockStyle.Top,
					};

					this.extendedDownButton = new IconButton
					{
						Parent          = upDown,
						IconUri         = UIBuilder.GetResourceIconUri ("Memory.Down"),
						PreferredSize   = new Size (20, 20),
						Dock            = DockStyle.Bottom,
					};
				}

				this.extendedRemoveButton = new IconButton
				{
					Parent          = toolbar,
					IconUri         = UIBuilder.GetResourceIconUri ("Memory.Delete"),
					PreferredSize   = new Size (40, 40),
					Dock            = DockStyle.Left,
				};
			}

			{
				var frame = new FrameBox
				{
					Parent          = rightFrame,
					DrawFullFrame   = true,
					Dock            = DockStyle.Fill,
					Margins         = new Margins (0, 0, 5, 0),
					Padding         = new Margins (5, 5, 0, 0),
				};

				this.extendedSummary = new StaticText
				{
					Parent           = frame,
					ContentAlignment = ContentAlignment.MiddleLeft,
					TextBreakMode    = TextBreakMode.None,
					Dock             = DockStyle.Fill,
				};
			}

			ToolTip.Default.SetToolTip (this.compactUseButton,     "Utilise la recherche, le filtre et les options définis dans le style");
			ToolTip.Default.SetToolTip (this.extendedUseButton,    "Utilise la recherche, le filtre et les options définis dans le style");
			ToolTip.Default.SetToolTip (this.extendedAddButton,    "Conserve la recherche, le filtre et les options dans un nouveau style");
			ToolTip.Default.SetToolTip (this.extendedUpdateButton, "Met à jour le style d'après la recherche, le filtre et les options en cours");
			ToolTip.Default.SetToolTip (this.extendedUpButton,     "Monte le style d'une ligne dnas la liste");
			ToolTip.Default.SetToolTip (this.extendedDownButton,   "Descend le style d'une ligne dnas la liste");
			ToolTip.Default.SetToolTip (this.extendedRemoveButton, "Supprime le style");

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

			this.extendedUseButton.Clicked += delegate
			{
				this.MemoryChanged ();
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
				var memory = this.memoryList.Selected;

				if (memory != null)
				{
					this.CopyDataToMemory (memory);

					this.UpdateCombo ();
					this.UpdateList ();
					this.UpdateButtons ();
					this.UpdateSummary ();

					this.extendedFieldName.SelectAll ();
					this.extendedFieldName.Focus ();
				}
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
			bool eq = this.CompareTo (this.memoryList.Selected);
			bool am = this.AlreadyMemorized ();

			this.compactUseButton.Enable     = (sel != -1 && !eq);
			this.extendedAddButton.Enable    = !am;
			this.extendedUseButton.Enable    = (sel != -1 && !eq);
			this.extendedUpButton.Enable     = (sel != -1 && sel > 0);
			this.extendedUpdateButton.Enable = (sel != -1 && !am);
			this.extendedDownButton.Enable   = (sel != -1 && sel < count-1);
			this.extendedRemoveButton.Enable = (sel != -1);
		}

		private void UpdateSummary()
		{
			var compactSummary  = FormattedText.Empty;
			var extendedSummary = FormattedText.Empty;

			var memory = this.memoryList.Selected;

			if (memory != null)
			{
				compactSummary  = memory.GetSummary (this.controller.ColumnMappers, "", ", ");
				extendedSummary = memory.GetSummary (this.controller.ColumnMappers, "● ", "<br/>");
			}

			this.compactSummary.FormattedText  = compactSummary;
			this.extendedSummary.FormattedText = extendedSummary;
		}


		private bool AlreadyMemorized()
		{
			foreach (var memory in this.memoryList.List)
			{
				if (this.CompareTo (memory))
				{
					return true;
				}
			}

			return false;
		}

		private bool CompareTo(MemoryData memory)
		{
			if (memory != null)
			{
				if (this.controller.MainWindowController.ShowSearchPanel  != memory.ShowSearch ||
					this.controller.MainWindowController.ShowFilterPanel  != memory.ShowFilter ||
					this.controller.MainWindowController.ShowOptionsPanel != memory.ShowOptions)
				{
					return false;
				}

				if (this.dataAccessor != null && this.dataAccessor.SearchData != null && memory.Search != null)
				{
					if (!this.dataAccessor.SearchData.CompareTo (memory.Search))
					{
						return false;
					}
				}

				if (this.dataAccessor != null && this.dataAccessor.FilterData != null && memory.Filter != null)
				{
					if (!this.dataAccessor.FilterData.CompareTo (memory.Filter))
					{
						return false;
					}
				}

				if (this.dataAccessor != null && this.dataAccessor.AccessorOptions != null && memory.Options != null)
				{
					if (!this.dataAccessor.AccessorOptions.CompareTo (memory.Options))
					{
						return false;
					}
				}
			}

			return true;
		}

		private void CopyDataToMemory(MemoryData memory)
		{
			if (this.dataAccessor != null && this.dataAccessor.SearchData != null)
			{
				memory.Search = this.dataAccessor.SearchData.CopyFrom ();
			}

			if (this.dataAccessor != null && this.dataAccessor.FilterData != null)
			{
				memory.Filter = this.dataAccessor.FilterData.CopyFrom ();
			}

			if (this.dataAccessor != null && this.dataAccessor.AccessorOptions != null)
			{
				memory.Options = this.dataAccessor.AccessorOptions.CopyFrom ();
			}

			memory.ShowSearch  = this.controller.MainWindowController.ShowSearchPanel;
			memory.ShowFilter  = this.controller.MainWindowController.ShowFilterPanel;
			memory.ShowOptions = this.controller.MainWindowController.ShowOptionsPanel;
		}

		private void CopyMemoryToData(MemoryData memory)
		{
			if (this.dataAccessor != null && this.dataAccessor.SearchData != null && memory.Search != null)
			{
				memory.Search.CopyTo (this.dataAccessor.SearchData);
			}

			if (this.dataAccessor != null && this.dataAccessor.FilterData != null && memory.Filter != null)
			{
				memory.Filter.CopyTo (this.dataAccessor.FilterData);
			}

			if (this.dataAccessor != null && this.dataAccessor.AccessorOptions != null && memory.Options != null)
			{
				memory.Options.CopyTo (this.dataAccessor.AccessorOptions);
			}

			this.controller.MainWindowController.ShowSearchPanel  = memory.ShowSearch;
			this.controller.MainWindowController.ShowFilterPanel  = memory.ShowFilter;
			this.controller.MainWindowController.ShowOptionsPanel = memory.ShowOptions;
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
		private Button									compactUseButton;
		private StaticText								compactSummary;

		private FrameBox								extendedFrame;
		private ScrollList								extendedListJournaux;
		private TextFieldEx								extendedFieldName;
		private Button									extendedUseButton;
		private Button									extendedAddButton;
		private Button									extendedUpdateButton;
		private Button									extendedUpButton;
		private Button									extendedDownButton;
		private Button									extendedRemoveButton;
		private StaticText								extendedSummary;
	}
}
