//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Support;
using Epsitec.Common.Dialogs;

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
							this.extendedListMemory.Focus ();
						}
						else
						{
							this.compactComboMemory.Focus ();
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
			this.levelController.CreateUI (levelFrame, "Utilise le premier style", this.ClearAction, this.LevelChangedAction);

			this.UpdateCombo ();
			this.UpdateList ();
			this.UpdateWidgets ();
		}

		private void ClearAction()
		{
			this.memoryList.SelectedIndex = 0;
			this.UpdateAfterSelectionChanged ();
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

			this.compactComboMemory = new TextFieldCombo
			{
				Parent          = this.comptactFrame,
				PreferredWidth  = MemoryController.fieldWidth,
				PreferredHeight = 20,
				IsReadOnly      = true,
				Dock            = DockStyle.Left,
				Margins           = new Margins (0, 10, 0, 0),
			};

			this.compactUpdateButton = new IconButton
			{
				Parent            = this.comptactFrame,
				IconUri           = UIBuilder.GetResourceIconUri ("Memory.Update"),
				PreferredIconSize = new Size (20, 20),
				PreferredSize     = new Size (20, 20),
				Dock              = DockStyle.Left,
			};

			this.compactUseButton = new IconButton
			{
				Parent            = this.comptactFrame,
				IconUri           = UIBuilder.GetResourceIconUri ("Memory.Use"),
				PreferredIconSize = new Size (20, 20),
				PreferredSize     = new Size (20, 20),
				Dock              = DockStyle.Left,
			};

			this.compactSummary = new StaticText
			{
				Parent        = this.comptactFrame,
				TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				Dock          = DockStyle.Fill,
				Margins       = new Margins (20, 0, 0, 0),
			};

			this.compactComboMemory.SelectedItemChanged += delegate
			{
				if (this.ignoreChanges.IsZero && this.compactComboMemory.SelectedItemIndex != -1)
				{
					this.memoryList.SelectedIndex = this.compactComboMemory.SelectedItemIndex;
					this.UpdateAfterSelectionChanged ();
					this.MemoryChanged ();
				}
			};

			this.compactUseButton.Clicked += delegate
			{
				this.MemoryChanged ();
			};

			this.compactUpdateButton.Clicked += delegate
			{
				string message = string.Format ("Voulez-vous vraiment mettre à jour le style \"{0}\"<br/>d'après la recherche, le filtre et les options en cours ?", this.memoryList.Selected.Name);
				var dialog = MessageDialog.CreateYesNo ("Crésus Comptabilité", DialogIcon.Question, message);
				dialog.OwnerWindow = this.controller.MainWindowController.Window;
				dialog.OpenDialog ();
				var result = dialog.Result;

				if (result == DialogResult.Yes)
				{
					this.UpdateMemoryAction ();
				}
			};
		}

		private void CreateExtendedMemoryUI(FrameBox parent)
		{
			this.extendedFrame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 15*7+29,  // 7 lignes dans la liste
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
			this.extendedListMemory = new ScrollList
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

			this.CreateExtendedMemoryToolbarUI (rightFrame);
			this.CreateExtendedMemorySummaryUI (rightFrame);

			this.extendedListMemory.SelectedItemChanged += delegate
			{
				if (this.ignoreChanges.IsZero && this.extendedListMemory.SelectedItemIndex != -1)
				{
					this.memoryList.SelectedIndex = this.extendedListMemory.SelectedItemIndex;
					this.UpdateAfterSelectionChanged ();
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
		}

		private void CreateExtendedMemoryToolbarUI(FrameBox parent)
		{
			//	Panneau de droite, toolbar (en haut).
			int w = 32+4;

			var toolbar = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
			};

			this.extendedAddButton = new IconButton
			{
				Parent          = toolbar,
				IconUri         = UIBuilder.GetResourceIconUri ("Memory.Add"),
				PreferredSize   = new Size (w, w),
				Dock            = DockStyle.Left,
			};

			this.extendedUpdateButton = new IconButton
			{
				Parent          = toolbar,
				IconUri         = UIBuilder.GetResourceIconUri ("Memory.Update"),
				PreferredSize   = new Size (w, w),
				Dock            = DockStyle.Left,
				Margins         = new Margins (2, 0, 0, 0),
			};

			this.extendedUseButton = new IconButton
			{
				Parent          = toolbar,
				IconUri         = UIBuilder.GetResourceIconUri ("Memory.Use"),
				PreferredSize   = new Size (w, w),
				Dock            = DockStyle.Left,
				Margins         = new Margins (2, 0, 0, 0),
			};

			{
				var upDown = new FrameBox
				{
					Parent          = toolbar,
					PreferredSize   = new Size (w/2, 2),
					Dock            = DockStyle.Left,
					Margins         = new Margins (20, 0, 0, 0),
				};

				this.extendedUpButton = new IconButton
				{
					Parent          = upDown,
					IconUri         = UIBuilder.GetResourceIconUri ("Memory.Up"),
					PreferredSize   = new Size (w/2, w/2),
					Dock            = DockStyle.Top,
				};

				this.extendedDownButton = new IconButton
				{
					Parent          = upDown,
					IconUri         = UIBuilder.GetResourceIconUri ("Memory.Down"),
					PreferredSize   = new Size (w/2, w/2),
					Dock            = DockStyle.Bottom,
				};
			}

			this.extendedRemoveButton = new IconButton
			{
				Parent          = toolbar,
				IconUri         = UIBuilder.GetResourceIconUri ("Memory.Delete"),
				PreferredSize   = new Size (w, w),
				Dock            = DockStyle.Left,
			};

			this.CreateExtendedMemoryAttributeUI (toolbar);

			ToolTip.Default.SetToolTip (this.compactUseButton,    "Utilise la recherche, le filtre et les options définis dans le style");
			ToolTip.Default.SetToolTip (this.compactUpdateButton, "Met à jour le style d'après la recherche, le filtre et les options en cours");

			ToolTip.Default.SetToolTip (this.extendedUseButton,    "Utilise la recherche, le filtre et les options définis dans le style");
			ToolTip.Default.SetToolTip (this.extendedAddButton,    "Conserve la recherche, le filtre et les options dans un nouveau style");
			ToolTip.Default.SetToolTip (this.extendedUpdateButton, "Met à jour le style d'après la recherche, le filtre et les options en cours");
			ToolTip.Default.SetToolTip (this.extendedUpButton,     "Monte le style d'une ligne dnas la liste");
			ToolTip.Default.SetToolTip (this.extendedDownButton,   "Descend le style d'une ligne dnas la liste");
			ToolTip.Default.SetToolTip (this.extendedRemoveButton, "Supprime le style");

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
				this.UpdateMemoryAction ();

				this.extendedFieldName.SelectAll ();
				this.extendedFieldName.Focus ();
			};

			this.extendedUpButton.Clicked += delegate
			{
				int sel = this.extendedListMemory.SelectedItemIndex;

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
				int sel = this.extendedListMemory.SelectedItemIndex;

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
				int sel = this.extendedListMemory.SelectedItemIndex;
				this.memoryList.List.RemoveAt (sel);

				this.UpdateCombo ();
				this.UpdateList ();

				sel = System.Math.Min (sel, this.memoryList.List.Count-1);
				var memory = (sel == -1) ? null : this.memoryList.List[sel];

				this.memoryList.Selected = memory;

				this.UpdateAfterSelectionChanged ();
				this.MemoryChanged ();
			};
		}

		private void CreateExtendedMemoryAttributeUI(FrameBox parent)
		{
			var frame = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Right,
			};

			this.extendedAttributePermanent = new CheckButton
			{
				Parent         = frame,
				Text           = "Non supprimable",
				PreferredWidth = 120,
				Dock           = DockStyle.Bottom,
			};

			this.extendedAttributeReadonly = new CheckButton
			{
				Parent         = frame,
				Text           = "Non modifiable",
				PreferredWidth = 120,
				Dock           = DockStyle.Bottom,
			};

			ToolTip.Default.SetToolTip (this.extendedAttributeReadonly,  "Une coche indique que ce style peut être utilisé en l'état, mais plus modifié");
			ToolTip.Default.SetToolTip (this.extendedAttributePermanent, "Une coche indique que ce style est indestructible");

			this.extendedAttributeReadonly.ActiveStateChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.IsReadonly = !this.IsReadonly;
					this.UpdateButtons ();
					this.UpdateSummary ();
				}
			};

			this.extendedAttributePermanent.ActiveStateChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.IsPermanent = !this.IsPermanent;
					this.UpdateButtons ();
					this.UpdateSummary ();
				}
			};
		}

		private void CreateExtendedMemorySummaryUI(FrameBox parent)
		{
			//	Panneau de droite, résumé (en bas).
			var frame = new FrameBox
			{
				Parent          = parent,
				DrawFullFrame   = true,
				BackColor       = Color.FromAlphaColor (0.7, Color.FromName ("White")),
				Dock            = DockStyle.Bottom,
				Padding         = new Margins (10, 10, 4, 4),
			};

			var line1 = new FrameBox
			{
				Parent          = frame,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
			};

			var line2 = new FrameBox
			{
				Parent          = frame,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
			};

			var line3 = new FrameBox
			{
				Parent          = frame,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
			};

			var line4 = new FrameBox
			{
				Parent          = frame,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 5, 0),
			};

			this.extendedSearchIconSummary = new StaticText
			{
				Parent         = line1,
				FormattedText  = UIBuilder.GetTextIconUri ("Panel.Search"),
				PreferredWidth = 30,
				Dock           = DockStyle.Left,
			};

			this.extendedFilterIconSummary = new StaticText
			{
				Parent         = line2,
				FormattedText  = UIBuilder.GetTextIconUri ("Panel.Filter"),
				PreferredWidth = 30,
				Dock           = DockStyle.Left,
			};

			this.extendedOptionsIconSummary = new StaticText
			{
				Parent         = line3,
				FormattedText  = UIBuilder.GetTextIconUri ("Panel.Options"),
				PreferredWidth = 30,
				Dock           = DockStyle.Left,
			};

			this.extendedSearchSummary = new CheckButton
			{
				Parent          = line1,
				TextBreakMode   = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				PreferredHeight = 20,
				Dock            = DockStyle.Fill,
			};

			this.extendedFilterSummary = new CheckButton
			{
				Parent          = line2,
				TextBreakMode   = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				PreferredHeight = 20,
				Dock            = DockStyle.Fill,
			};

			this.extendedOptionsSummary = new CheckButton
			{
				Parent          = line3,
				TextBreakMode   = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				PreferredHeight = 20,
				Dock            = DockStyle.Fill,
			};

			this.extendedShowPanelModeLabel = new StaticText
			{
				Parent         = line4,
				FormattedText  = "Action spéciale",
				PreferredWidth = 85,
				Dock           = DockStyle.Left,
			};

			GlyphButton showPanelModeButton;
			this.extendedShowPanelModeFrame = UIBuilder.CreatePseudoCombo (line4, out this.extendedShowPanelMode, out showPanelModeButton);
			this.extendedShowPanelModeFrame.PreferredWidth = 300;

			//	Connexions des événements.
			this.extendedSearchSummary.ActiveStateChanged += delegate
			{
				var memory = this.memoryList.Selected;
				if (memory != null && this.ignoreChanges.IsZero)
				{
					memory.EnableSearch = this.extendedSearchSummary.ActiveState == ActiveState.Yes;
				}
			};

			this.extendedFilterSummary.ActiveStateChanged += delegate
			{
				var memory = this.memoryList.Selected;
				if (memory != null && this.ignoreChanges.IsZero)
				{
					memory.EnableFilter = this.extendedFilterSummary.ActiveState == ActiveState.Yes;
				}
			};

			this.extendedOptionsSummary.ActiveStateChanged += delegate
			{
				var memory = this.memoryList.Selected;
				if (memory != null && this.ignoreChanges.IsZero)
				{
					memory.EnableOptions = this.extendedOptionsSummary.ActiveState == ActiveState.Yes;
				}
			};

			this.extendedShowPanelMode.Clicked += delegate
			{
				this.ShowMenuPanelMode (this.extendedShowPanelModeFrame);
			};

			showPanelModeButton.Clicked += delegate
			{
				this.ShowMenuPanelMode (this.extendedShowPanelModeFrame);
			};

			ToolTip.Default.SetToolTip (frame, "Résumé du style");
		}

		private void UpdateMemoryAction()
		{
			var memory = this.memoryList.Selected;

			if (memory != null && !memory.Permanent)  // garde-fou
			{
				this.CopyDataToMemory (memory);

				this.UpdateButtons ();
				this.UpdateSummary ();
			}
		}


		private void UpdateCombo()
		{
			using (this.ignoreChanges.Enter ())
			{
				this.compactComboMemory.Items.Clear ();

				foreach (var memory in this.memoryList.List)
				{
					this.compactComboMemory.Items.Add (memory.Name);
				}

				this.compactComboMemory.Enable = this.memoryList.List.Any ();

				if (this.memoryList.Selected == null)
				{
					this.compactComboMemory.FormattedText = FormattedText.Empty;
				}
				else
				{
					this.compactComboMemory.FormattedText = this.memoryList.Selected.Name;
				}
			}
		}

		private void UpdateList()
		{
			using (this.ignoreChanges.Enter ())
			{
				this.extendedListMemory.Items.Clear ();

				foreach (var memory in this.memoryList.List)
				{
					this.extendedListMemory.Items.Add (memory.Name);
				}

				if (this.memoryList.Selected == null)
				{
					this.extendedListMemory.SelectedItemIndex = -1;
					this.extendedFieldName.FormattedText = FormattedText.Empty;
				}
				else
				{
					this.extendedListMemory.SelectedItemIndex = this.memoryList.SelectedIndex;
					this.extendedListMemory.ShowSelected (ScrollShowMode.Extremity);

					this.extendedFieldName.FormattedText = this.memoryList.Selected.Name;
				}

				this.compactComboMemory.SelectedItemIndex = this.extendedListMemory.SelectedItemIndex;
			}
		}

		private void UpdateAfterSelectionChanged()
		{
			using (this.ignoreChanges.Enter ())
			{
				this.compactComboMemory.SelectedItemIndex = this.memoryList.SelectedIndex;
				this.extendedListMemory.SelectedItemIndex = this.memoryList.SelectedIndex;
			}
		}

		private void UpdateButtons()
		{
			int sel = this.extendedListMemory.SelectedItemIndex;
			int count = this.memoryList.List.Count;
			bool eq = this.CompareTo (this.memoryList.Selected);

			this.levelController.ClearEnable = sel != 0 || !eq;

			this.compactUseButton.Enable    = (sel != -1 && !eq);
			this.compactUpdateButton.Enable = (sel != -1 && !eq && !this.IsReadonly);

			this.extendedAddButton.Enable    = true;
			this.extendedUseButton.Enable    = (sel != -1 && !eq);
			this.extendedUpButton.Enable     = (sel != -1 && sel > 0);
			this.extendedUpdateButton.Enable = (sel != -1 && !eq && !this.IsReadonly);
			this.extendedDownButton.Enable   = (sel != -1 && sel < count-1);
			this.extendedRemoveButton.Enable = (sel != -1 && !this.IsPermanent);

			this.extendedFieldName.IsReadOnly = this.IsReadonly;
			this.extendedFieldName.Invalidate ();  // pour contourner un bug !
		}

		private void UpdateSummary()
		{
			using (this.ignoreChanges.Enter ())
			{
				var memory = this.memoryList.Selected;

				if (memory == null)
				{
					var compactSummary = FormattedText.Empty;

					this.extendedSearchSummary .FormattedText = FormattedText.Empty;
					this.extendedFilterSummary .FormattedText = FormattedText.Empty;
					this.extendedOptionsSummary.FormattedText = FormattedText.Empty;
					this.extendedShowPanelMode .FormattedText = FormattedText.Empty;

					this.extendedSearchSummary .ActiveState = ActiveState.No;
					this.extendedFilterSummary .ActiveState = ActiveState.No;
					this.extendedOptionsSummary.ActiveState = ActiveState.No;
				}
				else
				{
					this.compactSummary.FormattedText = memory.GetSummary (this.controller.ColumnMappers);

					this.extendedSearchSummary .FormattedText = memory.GetSearchSummary  (this.controller.ColumnMappers);
					this.extendedFilterSummary .FormattedText = memory.GetFilterSummary  (this.controller.ColumnMappers);
					this.extendedOptionsSummary.FormattedText = memory.GetOptionsSummary (this.controller.ColumnMappers);
					this.extendedShowPanelMode .FormattedText = memory.ShowPanelModeSummary;

					if (this.extendedSearchSummary.FormattedText.IsNullOrEmpty)
					{
						this.extendedSearchSummary.FormattedText = "Vide";
					}

					if (this.extendedFilterSummary.FormattedText.IsNullOrEmpty)
					{
						this.extendedFilterSummary.FormattedText = "Vide";
					}

					if (this.extendedOptionsSummary.FormattedText.IsNullOrEmpty)
					{
						this.extendedOptionsSummary.FormattedText = "Vide";
					}

					this.extendedSearchSummary.ActiveState  = memory.EnableSearch  ? ActiveState.Yes : ActiveState.No;
					this.extendedFilterSummary.ActiveState  = memory.EnableFilter  ? ActiveState.Yes : ActiveState.No;
					this.extendedOptionsSummary.ActiveState = memory.EnableOptions ? ActiveState.Yes : ActiveState.No;

					this.extendedAttributePermanent.ActiveState = memory.Permanent ? ActiveState.Yes : ActiveState.No;
					this.extendedAttributeReadonly .ActiveState = memory.Readonly  ? ActiveState.Yes : ActiveState.No;
				}

				this.extendedSearchSummary .Enable = (memory != null && !this.IsReadonly);
				this.extendedFilterSummary .Enable = (memory != null && !this.IsReadonly);
				this.extendedOptionsSummary.Enable = (memory != null && !this.IsReadonly);

				this.extendedShowPanelModeFrame.Enable = (memory != null && !this.IsReadonly);

				this.extendedSearchIconSummary .Visibility = this.controller.HasShowSearchPanel;
				this.extendedFilterIconSummary .Visibility = this.controller.HasShowFilterPanel;
				this.extendedOptionsIconSummary.Visibility = this.controller.HasShowOptionsPanel;

				this.extendedSearchSummary .Visibility = this.controller.HasShowSearchPanel;
				this.extendedFilterSummary .Visibility = this.controller.HasShowFilterPanel;
				this.extendedOptionsSummary.Visibility = this.controller.HasShowOptionsPanel;

				this.extendedShowPanelModeLabel.Visibility = (memory != null);
				this.extendedShowPanelModeFrame.Visibility = (memory != null);

				this.extendedAttributePermanent.Visibility = (memory != null);
				this.extendedAttributeReadonly .Visibility = (memory != null);
			}
		}


		#region Menu panel mode
		private void ShowMenuPanelMode(Widget parentButton)
		{
			//	Affiche le menu permettant de choisir l'action spéciale.
			if (this.IsReadonly)
			{
				return;
			}

			var menu = new VMenu ();
			var memory = this.memoryList.Selected;

			this.AddMenuPanelMode (menu, "Aucune", () => this.IsNop, x => this.SetNop (), true);

			if (memory.ShowSearch != ShowPanelMode.DoesNotExist)
			{
				menu.Items.Add (new MenuSeparator ());

				this.AddMenuPanelMode (menu, "Laisser le panneau des recherches dans son état",  () => memory.ShowSearch == ShowPanelMode.Nop,            x => memory.ShowSearch = ShowPanelMode.Nop);
				this.AddMenuPanelMode (menu, "Cacher le panneau des recherches",                 () => memory.ShowSearch == ShowPanelMode.Hide,           x => memory.ShowSearch = ShowPanelMode.Hide);
				this.AddMenuPanelMode (menu, "Montrer le panneau des recherches en mode simple", () => memory.ShowSearch == ShowPanelMode.ShowBeginner,   x => memory.ShowSearch = ShowPanelMode.ShowBeginner);
				this.AddMenuPanelMode (menu, "Montrer le panneau des recherches en mode avancé", () => memory.ShowSearch == ShowPanelMode.ShowSpecialist, x => memory.ShowSearch = ShowPanelMode.ShowSpecialist);
			}

			if (memory.ShowFilter != ShowPanelMode.DoesNotExist)
			{
				menu.Items.Add (new MenuSeparator ());

				this.AddMenuPanelMode (menu, "Laisser le panneau du filtre dans son état",  () => memory.ShowFilter == ShowPanelMode.Nop,            x => memory.ShowFilter = ShowPanelMode.Nop);
				this.AddMenuPanelMode (menu, "Cacher le panneau du filtre",                 () => memory.ShowFilter == ShowPanelMode.Hide,           x => memory.ShowFilter = ShowPanelMode.Hide);
				this.AddMenuPanelMode (menu, "Montrer le panneau du filtre en mode simple", () => memory.ShowFilter == ShowPanelMode.ShowBeginner,   x => memory.ShowFilter = ShowPanelMode.ShowBeginner);
				this.AddMenuPanelMode (menu, "Montrer le panneau du filtre en mode avancé", () => memory.ShowFilter == ShowPanelMode.ShowSpecialist, x => memory.ShowFilter = ShowPanelMode.ShowSpecialist);
			}

			if (memory.ShowOptions != ShowPanelMode.DoesNotExist)
			{
				menu.Items.Add (new MenuSeparator ());

				this.AddMenuPanelMode (menu, "Laisser le panneau des options dans son état",  () => memory.ShowOptions == ShowPanelMode.Nop,            x => memory.ShowOptions = ShowPanelMode.Nop);
				this.AddMenuPanelMode (menu, "Cacher le panneau des options",                 () => memory.ShowOptions == ShowPanelMode.Hide,           x => memory.ShowOptions = ShowPanelMode.Hide);
				this.AddMenuPanelMode (menu, "Montrer le panneau des options en mode simple", () => memory.ShowOptions == ShowPanelMode.ShowBeginner,   x => memory.ShowOptions = ShowPanelMode.ShowBeginner);
				this.AddMenuPanelMode (menu, "Montrer le panneau des options en mode avancé", () => memory.ShowOptions == ShowPanelMode.ShowSpecialist, x => memory.ShowOptions = ShowPanelMode.ShowSpecialist);
			}

			TextFieldCombo.AdjustComboSize (parentButton, menu, false);

			menu.Host = parentButton.Window;
			menu.ShowAsComboList (parentButton, Point.Zero, parentButton);
		}

		private bool IsNop
		{
			get
			{
				var memory = this.memoryList.Selected;

				return memory.ShowSearch  == ShowPanelMode.Nop && 
					   memory.ShowFilter  == ShowPanelMode.Nop && 
					   memory.ShowOptions == ShowPanelMode.Nop;
			}
		}

		private void SetNop()
		{
			var memory = this.memoryList.Selected;

			memory.ShowSearch  = ShowPanelMode.Nop;
			memory.ShowFilter  = ShowPanelMode.Nop;
			memory.ShowOptions = ShowPanelMode.Nop;
		}

		private void AddMenuPanelMode(VMenu menu, FormattedText text, System.Func<bool> getter, System.Action<bool> setter, bool check = false)
		{
			var item = new MenuItem ()
			{
				IconUri       = check ? UIBuilder.GetCheckStateIconUri (getter ()) : UIBuilder.GetRadioStateIconUri (getter ()),
				FormattedText = text,
			};

			item.Clicked += delegate
			{
				setter (true);
				this.UpdateSummary ();
			};

			menu.Items.Add (item);
		}
		#endregion


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
				//	Compare avec la visibilité des panneaux, si cela a un sens.
				if (memory.ShowSearch != ShowPanelMode.Nop && memory.ShowSearch != ShowPanelMode.DoesNotExist)
				{
					if (this.controller.MainWindowController.ShowSearchPanel != (memory.ShowSearch != ShowPanelMode.Hide))
					{
						return false;
					}
				}

				if (memory.ShowFilter != ShowPanelMode.Nop && memory.ShowFilter != ShowPanelMode.DoesNotExist)
				{
					if (this.controller.MainWindowController.ShowFilterPanel != (memory.ShowFilter != ShowPanelMode.Hide))
					{
						return false;
					}
				}

				if (memory.ShowOptions != ShowPanelMode.Nop && memory.ShowOptions != ShowPanelMode.DoesNotExist)
				{
					if (this.controller.MainWindowController.ShowOptionsPanel != (memory.ShowOptions != ShowPanelMode.Hide))
					{
						return false;
					}
				}

				//	Compare avec les données.
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

				if (this.dataAccessor != null && this.dataAccessor.Options != null && memory.Options != null)
				{
					if (!this.dataAccessor.Options.CompareTo (memory.Options))
					{
						return false;
					}
				}
			}

			return true;
		}

		private void CopyDataToMemory(MemoryData memory)
		{
			//	Met les paramètres des panneaux dans une mémoire (panneaux -> memory).
			if (this.dataAccessor != null && this.dataAccessor.SearchData != null)
			{
				memory.Search = this.dataAccessor.SearchData.CopyFrom ();
			}

			if (this.dataAccessor != null && this.dataAccessor.FilterData != null)
			{
				memory.Filter = this.dataAccessor.FilterData.CopyFrom ();
			}

			if (this.dataAccessor != null && this.dataAccessor.Options != null)
			{
				memory.Options = this.dataAccessor.Options.CopyFrom ();
			}

			//	Met un mode spécial si le panneau n'existe pas.
			if (!this.controller.HasShowSearchPanel)
			{
				memory.ShowSearch = ShowPanelMode.DoesNotExist;
			}

			if (!this.controller.HasShowFilterPanel)
			{
				memory.ShowFilter = ShowPanelMode.DoesNotExist;
			}
			
			if (!this.controller.HasShowOptionsPanel)
			{
				memory.ShowOptions = ShowPanelMode.DoesNotExist;
			}
		}

		private void CopyMemoryToData(MemoryData memory)
		{
			//	Utilise une mémoire (memory -> panneaux).
			if (this.dataAccessor != null && this.dataAccessor.SearchData != null && memory.Search != null && memory.EnableSearch)
			{
				memory.Search.CopyTo (this.dataAccessor.SearchData);
			}

			if (this.dataAccessor != null && this.dataAccessor.FilterData != null && memory.Filter != null && memory.EnableFilter)
			{
				memory.Filter.CopyTo (this.dataAccessor.FilterData);
			}

			if (this.dataAccessor != null && this.dataAccessor.Options != null && memory.Options != null && memory.EnableOptions)
			{
				memory.Options.CopyTo (this.dataAccessor.Options);
			}

			//	Effectue éventuellement l'action spéciale, qui consiste à montrer ou cacher des panneaux.
			if (memory.ShowSearch != ShowPanelMode.Nop && memory.ShowSearch != ShowPanelMode.DoesNotExist)
			{
				this.controller.MainWindowController.ShowSearchPanel = (memory.ShowSearch != ShowPanelMode.Hide);
				this.controller.SearchSpecialist = (memory.ShowSearch == ShowPanelMode.ShowSpecialist);
			}

			if (memory.ShowFilter != ShowPanelMode.Nop && memory.ShowFilter != ShowPanelMode.DoesNotExist)
			{
				this.controller.MainWindowController.ShowFilterPanel = (memory.ShowFilter != ShowPanelMode.Hide);
				this.controller.FilterSpecialist = (memory.ShowFilter == ShowPanelMode.ShowSpecialist);
			}

			if (memory.ShowOptions != ShowPanelMode.Nop && memory.ShowOptions != ShowPanelMode.DoesNotExist)
			{
				this.controller.MainWindowController.ShowOptionsPanel = (memory.ShowOptions != ShowPanelMode.Hide);
				this.controller.OptionsSpecialist = (memory.ShowOptions == ShowPanelMode.ShowSpecialist);
			}
		}


		private bool IsReadonly
		{
			get
			{
				var memory = this.memoryList.Selected;
				if (memory == null)
				{
					return false;
				}
				else
				{
					return memory.Readonly;
				}
			}
			set
			{
				var memory = this.memoryList.Selected;
				if (memory != null)
				{
					memory.Readonly = value;
				}
			}
		}

		private bool IsPermanent
		{
			get
			{
				var memory = this.memoryList.Selected;
				if (memory == null)
				{
					return false;
				}
				else
				{
					return memory.Permanent;
				}
			}
			set
			{
				var memory = this.memoryList.Selected;
				if (memory != null)
				{
					memory.Permanent = value;
				}
			}
		}

		private string NewMemoryName
		{
			get
			{
				int i = 1;

				while (true)
				{
					string name = "Style" + " " + i.ToString ();

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
		private TextFieldCombo							compactComboMemory;
		private Button									compactUseButton;
		private Button									compactUpdateButton;
		private StaticText								compactSummary;

		private FrameBox								extendedFrame;
		private ScrollList								extendedListMemory;
		private TextFieldEx								extendedFieldName;
		private Button									extendedUseButton;
		private Button									extendedAddButton;
		private Button									extendedUpdateButton;
		private Button									extendedUpButton;
		private Button									extendedDownButton;
		private Button									extendedRemoveButton;
		private StaticText								extendedSearchIconSummary;
		private StaticText								extendedFilterIconSummary;
		private StaticText								extendedOptionsIconSummary;
		private CheckButton								extendedSearchSummary;
		private CheckButton								extendedFilterSummary;
		private CheckButton								extendedOptionsSummary;
		private FrameBox								extendedShowPanelModeFrame;
		private StaticText								extendedShowPanelModeLabel;
		private StaticText								extendedShowPanelMode;

		private CheckButton								extendedAttributeReadonly;
		private CheckButton								extendedAttributePermanent;
	}
}
