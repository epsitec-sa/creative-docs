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
using Epsitec.Cresus.Compta.ViewSettings.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.ViewSettings.Controllers
{
	/// <summary>
	/// Ce contrôleur gère la barre d'outil supérieure des réglages de présentation pour la comptabilité.
	/// </summary>
	public class ViewSettingsController
	{
		public ViewSettingsController(AbstractController controller)
		{
			this.controller = controller;

			this.compta           = this.controller.ComptaEntity;
			this.dataAccessor     = this.controller.DataAccessor;
			this.businessContext  = this.controller.BusinessContext;
			this.viewSettingsList = this.controller.ViewSettingsList;

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
						if (this.topPanelLeftController.Specialist)
						{
							this.extendedListViewSettings.Focus ();
						}
						else
						{
							this.compactComboViewSettings.Focus ();
						}
					}
				}
			}
		}


		public void CreateUI(FrameBox parent, System.Action viewSettingsChangedAction)
		{
			this.viewSettingsChangedAction = viewSettingsChangedAction;

			this.toolbar = new FrameBox
			{
				Parent              = parent,
				DrawFullFrame       = true,
				BackColor           = UIBuilder.ViewSettingsBackColor,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock                = DockStyle.Top,
				Margins             = new Margins (0, 0, 0, -1),
				Visibility          = false,
			};

			var topPanelLeftFrame = new FrameBox
			{
				Parent         = this.toolbar,
				DrawFullFrame  = true,
				PreferredWidth = 20,
				Dock           = DockStyle.Left,
				Padding        = new Margins (5),
			};

			//	Crée les frames gauche, centrale et droite.
			this.mainFrame = new FrameBox
			{
				Parent         = this.toolbar,
				Dock           = DockStyle.Fill,
				Padding        = new Margins (5),
			};

			var topPanelRightFrame = new FrameBox
			{
				Parent         = this.toolbar,
				DrawFullFrame  = true,
				PreferredWidth = 20,
				Dock           = DockStyle.Right,
				Padding        = new Margins (5),
			};

			this.CreateComptactViewSettingsUI (this.mainFrame);
			this.CreateExtendedViewSettingsUI (this.mainFrame);

			//	Remplissage de la frame gauche.
			this.topPanelLeftController = new TopPanelLeftController (this.controller);
			this.topPanelLeftController.CreateUI (topPanelLeftFrame, true, "Panel.ViewSettings", this.LevelChangedAction);

			//	Remplissage de la frame droite.
			this.topPanelRightController = new TopPanelRightController (this.controller);
			this.topPanelRightController.CreateUI (topPanelRightFrame, "Utilise le premier réglage de présentation", this.ClearAction, this.controller.MainWindowController.ClosePanelViewSettings, this.LevelChangedAction);

			this.UpdateCombo ();
			this.UpdateList ();
			this.UpdateWidgets ();
		}

		private void ClearAction()
		{
			this.viewSettingsList.SelectedIndex = 0;
			this.UpdateAfterSelectionChanged ();
			this.ViewSettingsChanged ();
		}

		private void LevelChangedAction()
		{
			this.UpdateLevel ();
		}

		private void ViewSettingsChanged()
		{
			var viewSettings = this.viewSettingsList.Selected;
			if (viewSettings != null)
			{
				this.CopyViewSettingsToData (viewSettings);
				this.viewSettingsChangedAction ();
			}

			this.UpdateWidgets ();
		}

		private void UpdateLevel()
		{
			this.comptactFrame.Visibility = !this.topPanelLeftController.Specialist;
			this.extendedFrame.Visibility =  this.topPanelLeftController.Specialist;

			this.topPanelLeftController.Specialist = this.topPanelLeftController.Specialist;
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
			this.UpdateSummary ();
			this.UpdateComboSelection ();
			this.UpdateListSelection ();
		}


		private void CreateComptactViewSettingsUI(FrameBox parent)
		{
			this.comptactFrame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
			};

			new StaticText
			{
				Parent           = this.comptactFrame,
				Text             = "Réglage",
				ContentAlignment = ContentAlignment.MiddleRight,
				PreferredWidth   = UIBuilder.LeftLabelWidth-10,
				PreferredHeight  = 20,
				Dock             = DockStyle.Left,
				Margins          = new Margins (0, 10, 0, 0),
			};

			this.compactComboViewSettings = new TextFieldCombo
			{
				Parent          = this.comptactFrame,
				PreferredWidth  = ViewSettingsController.fieldWidth,
				PreferredHeight = 20,
				MenuButtonWidth = UIBuilder.ComboButtonWidth,
				IsReadOnly      = true,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 10, 0, 0),
			};

			this.compactUpdateButton = new IconButton
			{
				Parent            = this.comptactFrame,
				IconUri           = UIBuilder.GetResourceIconUri ("ViewSettings.Update"),
				PreferredIconSize = new Size (20, 20),
				PreferredSize     = new Size (20, 20),
				Dock              = DockStyle.Left,
			};

			this.compactUseButton = new IconButton
			{
				Parent            = this.comptactFrame,
				IconUri           = UIBuilder.GetResourceIconUri ("ViewSettings.Use"),
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

			this.compactComboViewSettings.SelectedItemChanged += delegate
			{
				if (this.ignoreChanges.IsZero && this.compactComboViewSettings.SelectedItemIndex != -1)
				{
					this.viewSettingsList.SelectedIndex = this.compactComboViewSettings.SelectedItemIndex;
					this.UpdateAfterSelectionChanged ();
					this.ViewSettingsChanged ();
				}
			};

			this.compactUseButton.Clicked += delegate
			{
				this.ViewSettingsChanged ();
			};

			this.compactUpdateButton.Clicked += delegate
			{
				string message = string.Format ("Voulez-vous vraiment mettre à jour le réglage de présentation \"{0}\"<br/>d'après la recherche, le filtre et les options en cours ?", this.viewSettingsList.Selected.Name);
				var result = this.controller.MainWindowController.QuestionDialog (message);

				if (result == DialogResult.Yes)
				{
					this.UpdateViewSettingsAction ();
				}
			};
		}

		private void CreateExtendedViewSettingsUI(FrameBox parent)
		{
			this.extendedFrame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 15*7+29,  // 7 lignes dans la liste
				Dock            = DockStyle.Top,
			};

			var leftFrame = new FrameBox
			{
				Parent = this.extendedFrame,
				Dock   = DockStyle.Left,
			};

			var centerFrame = new FrameBox
			{
				Parent          = this.extendedFrame,
				PreferredWidth  = ViewSettingsController.fieldWidth,
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
				Parent           = leftFrame,
				Text             = "Réglages",
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				ContentAlignment = ContentAlignment.MiddleRight,
				PreferredWidth   = UIBuilder.LeftLabelWidth-10,
				PreferredHeight  = 20,
				Dock             = DockStyle.Top,
				Margins          = new Margins (0, 10, 0, 0),
			};

			//	Panneau du milieu.
			this.extendedListViewSettings = new ScrollList
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

			this.CreateExtendedViewSettingsToolbarUI (rightFrame);
			this.CreateExtendedViewSettingsSummaryUI (rightFrame);

			this.extendedListViewSettings.SelectedItemChanged += delegate
			{
				if (this.ignoreChanges.IsZero && this.extendedListViewSettings.SelectedItemIndex != -1)
				{
					this.viewSettingsList.SelectedIndex = this.extendedListViewSettings.SelectedItemIndex;
					this.UpdateAfterSelectionChanged ();
					this.ViewSettingsChanged ();
				}
			};

			this.extendedFieldName.EditionAccepted += delegate
			{
				if (this.ignoreChanges.IsZero && this.viewSettingsList.Selected != null)
				{
					this.viewSettingsList.Selected.Name = this.extendedFieldName.FormattedText;
					this.UpdateCombo ();
					this.UpdateList ();
				}
			};
		}

		private void CreateExtendedViewSettingsToolbarUI(FrameBox parent)
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
				IconUri         = UIBuilder.GetResourceIconUri ("ViewSettings.Add"),
				PreferredSize   = new Size (w, w),
				Dock            = DockStyle.Left,
			};

			this.extendedUpdateButton = new IconButton
			{
				Parent          = toolbar,
				IconUri         = UIBuilder.GetResourceIconUri ("ViewSettings.Update"),
				PreferredSize   = new Size (w, w),
				Dock            = DockStyle.Left,
				Margins         = new Margins (2, 0, 0, 0),
			};

			this.extendedUseButton = new IconButton
			{
				Parent          = toolbar,
				IconUri         = UIBuilder.GetResourceIconUri ("ViewSettings.Use"),
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
					IconUri         = UIBuilder.GetResourceIconUri ("ViewSettings.Up"),
					PreferredSize   = new Size (w/2, w/2),
					Dock            = DockStyle.Top,
				};

				this.extendedDownButton = new IconButton
				{
					Parent          = upDown,
					IconUri         = UIBuilder.GetResourceIconUri ("ViewSettings.Down"),
					PreferredSize   = new Size (w/2, w/2),
					Dock            = DockStyle.Bottom,
				};
			}

			this.extendedRemoveButton = new IconButton
			{
				Parent          = toolbar,
				IconUri         = UIBuilder.GetResourceIconUri ("ViewSettings.Delete"),
				PreferredSize   = new Size (w, w),
				Dock            = DockStyle.Left,
			};

			this.CreateExtendedViewSettingsAttributeUI (toolbar);

			ToolTip.Default.SetToolTip (this.compactUseButton,    "Utilise la recherche, le filtre et les options définis dans le réglage de présentation");
			ToolTip.Default.SetToolTip (this.compactUpdateButton, "Met à jour le réglage de présentation d'après la recherche, le filtre et les options en cours");

			ToolTip.Default.SetToolTip (this.extendedUseButton,    "Utilise la recherche, le filtre et les options définis dans le réglage de présentation");
			ToolTip.Default.SetToolTip (this.extendedAddButton,    "Conserve la recherche, le filtre et les options dans un nouveau réglage de présentation");
			ToolTip.Default.SetToolTip (this.extendedUpdateButton, "Met à jour le réglage de présentation d'après la recherche, le filtre et les options en cours");
			ToolTip.Default.SetToolTip (this.extendedUpButton,     "Monte le réglage de présentation d'une ligne dnas la liste");
			ToolTip.Default.SetToolTip (this.extendedDownButton,   "Descend le réglage de présentation d'une ligne dnas la liste");
			ToolTip.Default.SetToolTip (this.extendedRemoveButton, "Supprime le réglage de présentation");

			this.extendedUseButton.Clicked += delegate
			{
				this.ViewSettingsChanged ();
			};

			this.extendedAddButton.Clicked += delegate
			{
				var viewSettings = new ViewSettingsData ();
				viewSettings.Name = this.NewViewSettingsName;
				this.CopyDataToViewSettings (viewSettings);

				this.viewSettingsList.List.Add (viewSettings);
				this.viewSettingsList.Selected = viewSettings;

				this.UpdateCombo ();
				this.UpdateList ();
				this.UpdateButtons ();
				this.UpdateSummary ();

				this.extendedFieldName.SelectAll ();
				this.extendedFieldName.Focus ();
			};

			this.extendedUpdateButton.Clicked += delegate
			{
				this.UpdateViewSettingsAction ();

				this.extendedFieldName.SelectAll ();
				this.extendedFieldName.Focus ();
			};

			this.extendedUpButton.Clicked += delegate
			{
				int sel = this.extendedListViewSettings.SelectedItemIndex;

				var m1 = this.viewSettingsList.List[sel-1];
				var m2 = this.viewSettingsList.List[sel];

				this.viewSettingsList.List[sel-1] = m2;
				this.viewSettingsList.List[sel]   = m1;

				this.UpdateCombo ();
				this.UpdateList ();
				this.UpdateButtons ();
			};

			this.extendedDownButton.Clicked += delegate
			{
				int sel = this.extendedListViewSettings.SelectedItemIndex;

				var m1 = this.viewSettingsList.List[sel+1];
				var m2 = this.viewSettingsList.List[sel];

				this.viewSettingsList.List[sel+1] = m2;
				this.viewSettingsList.List[sel]   = m1;

				this.UpdateCombo ();
				this.UpdateList ();
				this.UpdateButtons ();
			};

			this.extendedRemoveButton.Clicked += delegate
			{
				int sel = this.extendedListViewSettings.SelectedItemIndex;
				this.viewSettingsList.List.RemoveAt (sel);

				this.UpdateCombo ();
				this.UpdateList ();

				sel = System.Math.Min (sel, this.viewSettingsList.List.Count-1);
				var viewSettings = (sel == -1) ? null : this.viewSettingsList.List[sel];

				this.viewSettingsList.Selected = viewSettings;

				this.UpdateAfterSelectionChanged ();
				this.ViewSettingsChanged ();
			};
		}

		private void CreateExtendedViewSettingsAttributeUI(FrameBox parent)
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

			ToolTip.Default.SetToolTip (this.extendedAttributeReadonly,  "Une coche indique que ce réglage de présentation peut être utilisé en l'état, mais plus modifié");
			ToolTip.Default.SetToolTip (this.extendedAttributePermanent, "Une coche indique que ce réglage de présentation est indestructible");

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

		private void CreateExtendedViewSettingsSummaryUI(FrameBox parent)
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
			};

			var line5 = new FrameBox
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

			this.extendedTempoIconSummary = new StaticText
			{
				Parent         = line3,
				FormattedText  = UIBuilder.GetTextIconUri ("Panel.Tempo"),
				PreferredWidth = 30,
				Dock           = DockStyle.Left,
			};

			this.extendedOptionsIconSummary = new StaticText
			{
				Parent         = line4,
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

			this.extendedTempoSummary = new CheckButton
			{
				Parent          = line3,
				TextBreakMode   = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				PreferredHeight = 20,
				Dock            = DockStyle.Fill,
			};

			this.extendedOptionsSummary = new CheckButton
			{
				Parent          = line4,
				TextBreakMode   = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				PreferredHeight = 20,
				Dock            = DockStyle.Fill,
			};

			this.extendedShowPanelModeLabel = new StaticText
			{
				Parent         = line5,
				FormattedText  = "Action spéciale",
				PreferredWidth = 85,
				Dock           = DockStyle.Left,
			};

			GlyphButton showPanelModeButton;
			this.extendedShowPanelModeFrame = UIBuilder.CreatePseudoCombo (line5, out this.extendedShowPanelMode, out showPanelModeButton);
			this.extendedShowPanelModeFrame.PreferredWidth = 300;

			//	Connexions des événements.
			this.extendedSearchSummary.ActiveStateChanged += delegate
			{
				var viewSettings = this.viewSettingsList.Selected;
				if (viewSettings != null && this.ignoreChanges.IsZero)
				{
					viewSettings.EnableSearch = this.extendedSearchSummary.ActiveState == ActiveState.Yes;
				}
			};

			this.extendedFilterSummary.ActiveStateChanged += delegate
			{
				var viewSettings = this.viewSettingsList.Selected;
				if (viewSettings != null && this.ignoreChanges.IsZero)
				{
					viewSettings.EnableFilter = this.extendedFilterSummary.ActiveState == ActiveState.Yes;
				}
			};

			this.extendedTempoSummary.ActiveStateChanged += delegate
			{
				var viewSettings = this.viewSettingsList.Selected;
				if (viewSettings != null && this.ignoreChanges.IsZero)
				{
					viewSettings.EnableTemporal = this.extendedTempoSummary.ActiveState == ActiveState.Yes;
				}
			};

			this.extendedOptionsSummary.ActiveStateChanged += delegate
			{
				var viewSettings = this.viewSettingsList.Selected;
				if (viewSettings != null && this.ignoreChanges.IsZero)
				{
					viewSettings.EnableOptions = this.extendedOptionsSummary.ActiveState == ActiveState.Yes;
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

			ToolTip.Default.SetToolTip (frame, "Résumé du réglage de présentation");
		}

		private void UpdateViewSettingsAction()
		{
			var viewSettings = this.viewSettingsList.Selected;

			if (viewSettings != null && !viewSettings.Readonly)  // garde-fou
			{
				this.CopyDataToViewSettings (viewSettings);

				this.UpdateButtons ();
				this.UpdateSummary ();
			}
		}


		private void UpdateCombo()
		{
			using (this.ignoreChanges.Enter ())
			{
				this.compactComboViewSettings.Items.Clear ();

				foreach (var viewSettings in this.viewSettingsList.List)
				{
					this.compactComboViewSettings.Items.Add (viewSettings.Name);
				}

				this.compactComboViewSettings.Enable = this.viewSettingsList.List.Any ();

			}

			this.UpdateComboSelection ();
		}

		private void UpdateComboSelection()
		{
			using (this.ignoreChanges.Enter ())
			{
				if (this.viewSettingsList.Selected == null)
				{
					this.compactComboViewSettings.FormattedText = FormattedText.Empty;
				}
				else
				{
					this.compactComboViewSettings.FormattedText = this.viewSettingsList.Selected.Name;
				}
			}
		}

		private void UpdateList()
		{
			using (this.ignoreChanges.Enter ())
			{
				this.extendedListViewSettings.Items.Clear ();

				foreach (var viewSettings in this.viewSettingsList.List)
				{
					this.extendedListViewSettings.Items.Add (viewSettings.Name);
				}

			}

			this.UpdateListSelection ();
		}

		private void UpdateListSelection()
		{
			using (this.ignoreChanges.Enter ())
			{
				if (this.viewSettingsList.Selected == null)
				{
					this.extendedListViewSettings.SelectedItemIndex = -1;
					this.extendedFieldName.FormattedText = FormattedText.Empty;
				}
				else
				{
					this.extendedListViewSettings.SelectedItemIndex = this.viewSettingsList.SelectedIndex;
					this.extendedListViewSettings.ShowSelected (ScrollShowMode.Extremity);

					this.extendedFieldName.FormattedText = this.viewSettingsList.Selected.Name;
				}

				this.compactComboViewSettings.SelectedItemIndex = this.extendedListViewSettings.SelectedItemIndex;
			}
		}

		private void UpdateAfterSelectionChanged()
		{
			using (this.ignoreChanges.Enter ())
			{
				this.compactComboViewSettings.SelectedItemIndex = this.viewSettingsList.SelectedIndex;
				this.extendedListViewSettings.SelectedItemIndex = this.viewSettingsList.SelectedIndex;
			}
		}

		private void UpdateButtons()
		{
			int sel = this.extendedListViewSettings.SelectedItemIndex;
			int count = this.viewSettingsList.List.Count;
			bool eq = this.CompareTo (this.viewSettingsList.Selected);

			this.topPanelRightController.ClearEnable = sel != 0 || !eq;

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
				var viewSettings = this.viewSettingsList.Selected;

				if (viewSettings == null)
				{
					var compactSummary = FormattedText.Empty;

					this.extendedSearchSummary .FormattedText = FormattedText.Empty;
					this.extendedFilterSummary .FormattedText = FormattedText.Empty;
					this.extendedTempoSummary  .FormattedText = FormattedText.Empty;
					this.extendedOptionsSummary.FormattedText = FormattedText.Empty;
					this.extendedShowPanelMode .FormattedText = FormattedText.Empty;

					this.extendedSearchSummary .ActiveState = ActiveState.No;
					this.extendedFilterSummary .ActiveState = ActiveState.No;
					this.extendedTempoSummary  .ActiveState = ActiveState.No;
					this.extendedOptionsSummary.ActiveState = ActiveState.No;
				}
				else
				{
					this.compactSummary.FormattedText = viewSettings.GetSummary (this.controller.ColumnMappers);

					this.extendedSearchSummary .FormattedText = viewSettings.GetSearchSummary  (this.controller.ColumnMappers);
					this.extendedFilterSummary .FormattedText = viewSettings.GetFilterSummary  (this.controller.ColumnMappers);
					this.extendedTempoSummary  .FormattedText = viewSettings.GetTemporalSummary   (this.controller.ColumnMappers);
					this.extendedOptionsSummary.FormattedText = viewSettings.GetOptionsSummary (this.controller.ColumnMappers);

					this.extendedShowPanelMode.FormattedText = viewSettings.ShowPanelModeSummary;
					if (this.extendedShowPanelMode.FormattedText.IsNullOrEmpty)
					{
						this.extendedShowPanelMode.FormattedText = "Aucune";
					}

					if (this.extendedSearchSummary.FormattedText.IsNullOrEmpty)
					{
						this.extendedSearchSummary.FormattedText = "Vide";
					}

					if (this.extendedFilterSummary.FormattedText.IsNullOrEmpty)
					{
						this.extendedFilterSummary.FormattedText = "Vide";
					}

					if (this.extendedTempoSummary.FormattedText.IsNullOrEmpty)
					{
						this.extendedTempoSummary.FormattedText = "Vide";
					}

					if (this.extendedOptionsSummary.FormattedText.IsNullOrEmpty)
					{
						this.extendedOptionsSummary.FormattedText = "Vide";
					}

					this.extendedSearchSummary.ActiveState  = viewSettings.EnableSearch  ? ActiveState.Yes : ActiveState.No;
					this.extendedFilterSummary.ActiveState  = viewSettings.EnableFilter  ? ActiveState.Yes : ActiveState.No;
					this.extendedTempoSummary.ActiveState   = viewSettings.EnableTemporal   ? ActiveState.Yes : ActiveState.No;
					this.extendedOptionsSummary.ActiveState = viewSettings.EnableOptions ? ActiveState.Yes : ActiveState.No;

					this.extendedAttributePermanent.ActiveState = viewSettings.Permanent ? ActiveState.Yes : ActiveState.No;
					this.extendedAttributeReadonly .ActiveState = viewSettings.Readonly  ? ActiveState.Yes : ActiveState.No;

					var tooltipSummary = this.TooltipSummary;
					ToolTip.Default.SetToolTip (this.comptactFrame, tooltipSummary);
				}

				this.extendedSearchSummary .Enable = (viewSettings != null && !this.IsReadonly);
				this.extendedFilterSummary .Enable = (viewSettings != null && !this.IsReadonly);
				this.extendedTempoSummary  .Enable = (viewSettings != null && !this.IsReadonly);
				this.extendedOptionsSummary.Enable = (viewSettings != null && !this.IsReadonly);

				this.extendedShowPanelModeFrame.Enable = (viewSettings != null && !this.IsReadonly);

				this.extendedSearchIconSummary .Visibility = this.controller.HasShowSearchPanel;
				this.extendedFilterIconSummary .Visibility = this.controller.HasShowFilterPanel;
				this.extendedTempoIconSummary  .Visibility = this.controller.HasShowTemporalPanel;
				this.extendedOptionsIconSummary.Visibility = this.controller.HasShowOptionsPanel;

				this.extendedSearchSummary .Visibility = this.controller.HasShowSearchPanel;
				this.extendedFilterSummary .Visibility = this.controller.HasShowFilterPanel;
				this.extendedTempoSummary  .Visibility = this.controller.HasShowTemporalPanel;
				this.extendedOptionsSummary.Visibility = this.controller.HasShowOptionsPanel;

				this.extendedShowPanelModeLabel.Visibility = (viewSettings != null);
				this.extendedShowPanelModeFrame.Visibility = (viewSettings != null);

				this.extendedAttributePermanent.Visibility = (viewSettings != null);
				this.extendedAttributeReadonly .Visibility = (viewSettings != null);
			}
		}

		private FormattedText TooltipSummary
		{
			get
			{
				var builder = new System.Text.StringBuilder ();

				var viewSettings = this.viewSettingsList.Selected;

				if (viewSettings == null)
				{
					builder.Append ("Aucun");
				}
				else
				{
					FormattedText summary;

					summary = viewSettings.GetSearchSummary (this.controller.ColumnMappers);
					if (!summary.IsNullOrEmpty)
					{
						builder.Append (UIBuilder.GetTextIconUri ("Panel.Search"));
						builder.Append ("  ");
						builder.Append (summary);
						builder.Append ("<br/>");
					}

					summary = viewSettings.GetFilterSummary (this.controller.ColumnMappers);
					if (!summary.IsNullOrEmpty)
					{
						builder.Append (UIBuilder.GetTextIconUri ("Panel.Filter"));
						builder.Append ("  ");
						builder.Append (summary);
						builder.Append ("<br/>");
					}

					summary = viewSettings.GetTemporalSummary (this.controller.ColumnMappers);
					if (!summary.IsNullOrEmpty)
					{
						builder.Append (UIBuilder.GetTextIconUri ("Panel.Tempo"));
						builder.Append ("  ");
						builder.Append (summary);
						builder.Append ("<br/>");
					}

					summary = viewSettings.GetOptionsSummary (this.controller.ColumnMappers);
					if (!summary.IsNullOrEmpty)
					{
						builder.Append (UIBuilder.GetTextIconUri ("Panel.Options"));
						builder.Append ("  ");
						builder.Append (summary);
						builder.Append ("<br/>");
					}

					summary = viewSettings.ShowPanelModeSummary;
					if (!summary.IsNullOrEmpty)
					{
						builder.Append (UIBuilder.GetTextIconUri ("Panel.Info"));
						builder.Append ("  ");
						builder.Append (summary);
						builder.Append ("<br/>");
					}
				}

				var text = builder.ToString ();

				if (text.EndsWith ("<br/>"))
				{
					text = text.Substring (0, text.Length-5);  // enlève le <br/> à la fin
				}

				return text;
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
			var viewSettings = this.viewSettingsList.Selected;

			this.AddMenuPanelMode (menu, "Aucune", () => this.IsNop, x => this.SetNop (), true);

			if (viewSettings.ShowSearch != ShowPanelMode.DoesNotExist)
			{
				menu.Items.Add (new MenuSeparator ());

				this.AddMenuPanelMode (menu, "Laisser le panneau des recherches dans son état",  () => viewSettings.ShowSearch == ShowPanelMode.Nop,            x => viewSettings.ShowSearch = ShowPanelMode.Nop);
				this.AddMenuPanelMode (menu, "Cacher le panneau des recherches",                 () => viewSettings.ShowSearch == ShowPanelMode.Hide,           x => viewSettings.ShowSearch = ShowPanelMode.Hide);
				this.AddMenuPanelMode (menu, "Montrer le panneau des recherches en mode simple", () => viewSettings.ShowSearch == ShowPanelMode.ShowBeginner,   x => viewSettings.ShowSearch = ShowPanelMode.ShowBeginner);
				this.AddMenuPanelMode (menu, "Montrer le panneau des recherches en mode avancé", () => viewSettings.ShowSearch == ShowPanelMode.ShowSpecialist, x => viewSettings.ShowSearch = ShowPanelMode.ShowSpecialist);
			}

			if (viewSettings.ShowFilter != ShowPanelMode.DoesNotExist)
			{
				menu.Items.Add (new MenuSeparator ());

				this.AddMenuPanelMode (menu, "Laisser le panneau du filtre dans son état",  () => viewSettings.ShowFilter == ShowPanelMode.Nop,            x => viewSettings.ShowFilter = ShowPanelMode.Nop);
				this.AddMenuPanelMode (menu, "Cacher le panneau du filtre",                 () => viewSettings.ShowFilter == ShowPanelMode.Hide,           x => viewSettings.ShowFilter = ShowPanelMode.Hide);
				this.AddMenuPanelMode (menu, "Montrer le panneau du filtre en mode simple", () => viewSettings.ShowFilter == ShowPanelMode.ShowBeginner,   x => viewSettings.ShowFilter = ShowPanelMode.ShowBeginner);
				this.AddMenuPanelMode (menu, "Montrer le panneau du filtre en mode avancé", () => viewSettings.ShowFilter == ShowPanelMode.ShowSpecialist, x => viewSettings.ShowFilter = ShowPanelMode.ShowSpecialist);
			}

			if (viewSettings.ShowTemporal != ShowPanelMode.DoesNotExist)
			{
				menu.Items.Add (new MenuSeparator ());

				this.AddMenuPanelMode (menu, "Laisser le panneau du filtre temporel dans son état",  () => viewSettings.ShowTemporal == ShowPanelMode.Nop,            x => viewSettings.ShowTemporal = ShowPanelMode.Nop);
				this.AddMenuPanelMode (menu, "Cacher le panneau du filtre temporel",                 () => viewSettings.ShowTemporal == ShowPanelMode.Hide,           x => viewSettings.ShowTemporal = ShowPanelMode.Hide);
				this.AddMenuPanelMode (menu, "Montrer le panneau du filtre temporel en mode simple", () => viewSettings.ShowTemporal == ShowPanelMode.ShowBeginner,   x => viewSettings.ShowTemporal = ShowPanelMode.ShowBeginner);
				this.AddMenuPanelMode (menu, "Montrer le panneau du filtre temporel en mode avancé", () => viewSettings.ShowTemporal == ShowPanelMode.ShowSpecialist, x => viewSettings.ShowTemporal = ShowPanelMode.ShowSpecialist);
			}

			if (viewSettings.ShowOptions != ShowPanelMode.DoesNotExist)
			{
				menu.Items.Add (new MenuSeparator ());

				this.AddMenuPanelMode (menu, "Laisser le panneau des options dans son état",  () => viewSettings.ShowOptions == ShowPanelMode.Nop,            x => viewSettings.ShowOptions = ShowPanelMode.Nop);
				this.AddMenuPanelMode (menu, "Cacher le panneau des options",                 () => viewSettings.ShowOptions == ShowPanelMode.Hide,           x => viewSettings.ShowOptions = ShowPanelMode.Hide);
				this.AddMenuPanelMode (menu, "Montrer le panneau des options en mode simple", () => viewSettings.ShowOptions == ShowPanelMode.ShowBeginner,   x => viewSettings.ShowOptions = ShowPanelMode.ShowBeginner);
				this.AddMenuPanelMode (menu, "Montrer le panneau des options en mode avancé", () => viewSettings.ShowOptions == ShowPanelMode.ShowSpecialist, x => viewSettings.ShowOptions = ShowPanelMode.ShowSpecialist);
			}

			TextFieldCombo.AdjustComboSize (parentButton, menu, false);

			menu.Host = parentButton.Window;
			menu.ShowAsComboList (parentButton, Point.Zero, parentButton);
		}

		private bool IsNop
		{
			get
			{
				var viewSettings = this.viewSettingsList.Selected;

				return viewSettings.ShowSearch  == ShowPanelMode.Nop && 
					   viewSettings.ShowFilter  == ShowPanelMode.Nop && 
					   viewSettings.ShowTemporal   == ShowPanelMode.Nop && 
					   viewSettings.ShowOptions == ShowPanelMode.Nop;
			}
		}

		private void SetNop()
		{
			var viewSettings = this.viewSettingsList.Selected;

			viewSettings.ShowSearch  = ShowPanelMode.Nop;
			viewSettings.ShowFilter  = ShowPanelMode.Nop;
			viewSettings.ShowTemporal   = ShowPanelMode.Nop;
			viewSettings.ShowOptions = ShowPanelMode.Nop;
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
			foreach (var viewSettings in this.viewSettingsList.List)
			{
				if (this.CompareTo (viewSettings))
				{
					return true;
				}
			}

			return false;
		}

		private bool CompareTo(ViewSettingsData viewSettings)
		{
			if (viewSettings != null)
			{
				//	Compare avec la visibilité des panneaux, si cela a un sens.
				if (viewSettings.ShowSearch != ShowPanelMode.Nop && viewSettings.ShowSearch != ShowPanelMode.DoesNotExist)
				{
					if (this.controller.MainWindowController.ShowSearchPanel != (viewSettings.ShowSearch != ShowPanelMode.Hide))
					{
						return false;
					}
				}

				if (viewSettings.ShowFilter != ShowPanelMode.Nop && viewSettings.ShowFilter != ShowPanelMode.DoesNotExist)
				{
					if (this.controller.MainWindowController.ShowFilterPanel != (viewSettings.ShowFilter != ShowPanelMode.Hide))
					{
						return false;
					}
				}

				if (viewSettings.ShowTemporal != ShowPanelMode.Nop && viewSettings.ShowTemporal != ShowPanelMode.DoesNotExist)
				{
					if (this.controller.MainWindowController.ShowTemporalPanel != (viewSettings.ShowTemporal != ShowPanelMode.Hide))
					{
						return false;
					}
				}

				if (viewSettings.ShowOptions != ShowPanelMode.Nop && viewSettings.ShowOptions != ShowPanelMode.DoesNotExist)
				{
					if (this.controller.MainWindowController.ShowOptionsPanel != (viewSettings.ShowOptions != ShowPanelMode.Hide))
					{
						return false;
					}
				}

				//	Compare avec les données.
				if (this.dataAccessor != null && this.dataAccessor.SearchData != null && viewSettings.Search != null && viewSettings.EnableSearch)
				{
					if (!this.dataAccessor.SearchData.CompareTo (viewSettings.Search))
					{
						return false;
					}
				}

				if (this.dataAccessor != null && this.dataAccessor.FilterData != null && viewSettings.Filter != null && viewSettings.EnableFilter)
				{
					if (!this.dataAccessor.FilterData.CompareTo (viewSettings.Filter))
					{
						return false;
					}
				}

				if (this.dataAccessor != null && this.dataAccessor.TemporalData != null && viewSettings.Temporal != null && viewSettings.EnableTemporal)
				{
					if (!this.dataAccessor.TemporalData.CompareTo (viewSettings.Temporal))
					{
						return false;
					}
				}

				if (this.dataAccessor != null && this.dataAccessor.Options != null && viewSettings.Options != null && viewSettings.EnableOptions)
				{
					if (!this.dataAccessor.Options.CompareTo (viewSettings.Options))
					{
						return false;
					}
				}
			}

			return true;
		}

		private void CopyDataToViewSettings(ViewSettingsData viewSettings)
		{
			//	Met les paramètres des panneaux dans un réglage de présentation (panneaux -> viewSettings).
			if (this.dataAccessor != null && this.dataAccessor.SearchData != null)
			{
				viewSettings.Search = this.dataAccessor.SearchData.CopyFrom ();
			}

			if (this.dataAccessor != null && this.dataAccessor.FilterData != null)
			{
				viewSettings.Filter = this.dataAccessor.FilterData.CopyFrom ();
			}

			if (this.dataAccessor != null && this.dataAccessor.TemporalData != null)
			{
				viewSettings.Temporal = this.dataAccessor.TemporalData.CopyFrom ();
			}

			if (this.dataAccessor != null && this.dataAccessor.Options != null)
			{
				viewSettings.Options = this.dataAccessor.Options.CopyFrom ();
			}

			//	Met un mode spécial si le panneau n'existe pas.
			if (!this.controller.HasShowSearchPanel)
			{
				viewSettings.ShowSearch = ShowPanelMode.DoesNotExist;
			}

			if (!this.controller.HasShowFilterPanel)
			{
				viewSettings.ShowFilter = ShowPanelMode.DoesNotExist;
			}

			if (!this.controller.HasShowTemporalPanel)
			{
				viewSettings.ShowTemporal = ShowPanelMode.DoesNotExist;
			}

			if (!this.controller.HasShowOptionsPanel)
			{
				viewSettings.ShowOptions = ShowPanelMode.DoesNotExist;
			}
		}

		private void CopyViewSettingsToData(ViewSettingsData viewSettings)
		{
			//	Utilise un réglage de présentation (viewSettings -> panneaux).
			if (this.dataAccessor != null && this.dataAccessor.SearchData != null && viewSettings.Search != null && viewSettings.EnableSearch)
			{
				viewSettings.Search.CopyTo (this.dataAccessor.SearchData);
			}

			if (this.dataAccessor != null && this.dataAccessor.FilterData != null && viewSettings.Filter != null && viewSettings.EnableFilter)
			{
				viewSettings.Filter.CopyTo (this.dataAccessor.FilterData);
			}

			if (this.dataAccessor != null && this.dataAccessor.TemporalData != null && viewSettings.Temporal != null && viewSettings.EnableTemporal)
			{
				viewSettings.Temporal.CopyTo (this.dataAccessor.TemporalData);
			}

			if (this.dataAccessor != null && this.dataAccessor.Options != null && viewSettings.Options != null && viewSettings.EnableOptions)
			{
				viewSettings.Options.CopyTo (this.dataAccessor.Options);
			}

			//	Effectue éventuellement l'action spéciale, qui consiste à montrer ou cacher des panneaux.
			if (viewSettings.ShowSearch != ShowPanelMode.Nop && viewSettings.ShowSearch != ShowPanelMode.DoesNotExist)
			{
				this.controller.MainWindowController.ShowSearchPanel = (viewSettings.ShowSearch != ShowPanelMode.Hide);
				this.controller.SearchSpecialist = (viewSettings.ShowSearch == ShowPanelMode.ShowSpecialist);
			}

			if (viewSettings.ShowFilter != ShowPanelMode.Nop && viewSettings.ShowFilter != ShowPanelMode.DoesNotExist)
			{
				this.controller.MainWindowController.ShowFilterPanel = (viewSettings.ShowFilter != ShowPanelMode.Hide);
				this.controller.FilterSpecialist = (viewSettings.ShowFilter == ShowPanelMode.ShowSpecialist);
			}

			if (viewSettings.ShowTemporal != ShowPanelMode.Nop && viewSettings.ShowTemporal != ShowPanelMode.DoesNotExist)
			{
				this.controller.MainWindowController.ShowTemporalPanel = (viewSettings.ShowTemporal != ShowPanelMode.Hide);
				this.controller.TempoSpecialist = (viewSettings.ShowTemporal == ShowPanelMode.ShowSpecialist);
			}

			if (viewSettings.ShowOptions != ShowPanelMode.Nop && viewSettings.ShowOptions != ShowPanelMode.DoesNotExist)
			{
				this.controller.MainWindowController.ShowOptionsPanel = (viewSettings.ShowOptions != ShowPanelMode.Hide);
				this.controller.OptionsSpecialist = (viewSettings.ShowOptions == ShowPanelMode.ShowSpecialist);
			}
		}


		private bool IsReadonly
		{
			get
			{
				var viewSettings = this.viewSettingsList.Selected;
				if (viewSettings == null)
				{
					return false;
				}
				else
				{
					return viewSettings.Readonly;
				}
			}
			set
			{
				var viewSettings = this.viewSettingsList.Selected;
				if (viewSettings != null)
				{
					viewSettings.Readonly = value;
				}
			}
		}

		private bool IsPermanent
		{
			get
			{
				var viewSettings = this.viewSettingsList.Selected;
				if (viewSettings == null)
				{
					return false;
				}
				else
				{
					return viewSettings.Permanent;
				}
			}
			set
			{
				var viewSettings = this.viewSettingsList.Selected;
				if (viewSettings != null)
				{
					viewSettings.Permanent = value;
				}
			}
		}

		private string NewViewSettingsName
		{
			get
			{
				int i = 1;

				while (true)
				{
					string name = "Réglages" + " " + i.ToString ();

					if (this.viewSettingsList.List.Where (x => x.Name == name).Any ())
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
		private readonly ComptaEntity					compta;
		private readonly BusinessContext				businessContext;
		private readonly AbstractDataAccessor			dataAccessor;
		private readonly ViewSettingsList				viewSettingsList;
		private readonly SafeCounter					ignoreChanges;

		private System.Action							viewSettingsChangedAction;

		private FrameBox								mainFrame;
		private FrameBox								toolbar;
		private TopPanelLeftController					topPanelLeftController;
		private TopPanelRightController					topPanelRightController;
		private bool									showPanel;

		private FrameBox								comptactFrame;
		private TextFieldCombo							compactComboViewSettings;
		private Button									compactUseButton;
		private Button									compactUpdateButton;
		private StaticText								compactSummary;

		private FrameBox								extendedFrame;
		private ScrollList								extendedListViewSettings;
		private TextFieldEx								extendedFieldName;
		private Button									extendedUseButton;
		private Button									extendedAddButton;
		private Button									extendedUpdateButton;
		private Button									extendedUpButton;
		private Button									extendedDownButton;
		private Button									extendedRemoveButton;
		private StaticText								extendedSearchIconSummary;
		private StaticText								extendedFilterIconSummary;
		private StaticText								extendedTempoIconSummary;
		private StaticText								extendedOptionsIconSummary;
		private CheckButton								extendedSearchSummary;
		private CheckButton								extendedFilterSummary;
		private CheckButton								extendedTempoSummary;
		private CheckButton								extendedOptionsSummary;
		private FrameBox								extendedShowPanelModeFrame;
		private StaticText								extendedShowPanelModeLabel;
		private StaticText								extendedShowPanelMode;

		private CheckButton								extendedAttributeReadonly;
		private CheckButton								extendedAttributePermanent;
	}
}
