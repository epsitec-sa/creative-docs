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
using Epsitec.Cresus.Compta.Widgets;

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


		public PanelsToolbarController PanelsToolbarController
		{
			get
			{
				return this.panelsToolbarController;
			}
		}

		public void SetTitle(FormattedText title)
		{
			if (this.titleLabel.Visibility)
			{
				this.titleLabel.FormattedText = title;
				this.titleFrame.PreferredWidth = this.titleLabel.GetBestFitSize ().Width;
			}
		}

		public FrameBox GetTitleFrame()
		{
			this.titleLabel.Visibility = false;
			return this.titleFrame;
		}


		public void CreateUI(FrameBox parent, System.Action viewSettingsChangedAction)
		{
			this.viewSettingsChangedAction = viewSettingsChangedAction;

			this.toolbar = new FrameBox
			{
				Parent              = parent,
				DrawFullFrame       = true,
				BackColor           = RibbonController.GetBackgroundColor1 (),
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock                = DockStyle.Top,
				Margins             = new Margins (0, 0, 0, -1),
			};

			this.titleFrame = new FrameBox
			{
				Parent         = this.toolbar,
				Dock           = DockStyle.Left,
				Margins        = new Margins (10, 2, 5, 5),
			};

			this.titleLabel = new StaticText
			{
				Parent         = this.titleFrame,
				Dock           = DockStyle.Fill,
			};

			this.titleLabel.HypertextClicked += delegate
			{
				if (this.dataAccessor != null && this.dataAccessor.FilterData != null && !this.dataAccessor.FilterData.IsEmpty)
				{
					this.controller.MainWindowController.OpenPanelFilter ();
				}

				if (!this.controller.MainWindowController.TemporalData.IsEmpty)
				{
					this.controller.MainWindowController.OpenPanelTemporal ();
				}
			};

			this.mainFrame = new FrameBox
			{
				Parent         = this.toolbar,
				Dock           = DockStyle.Fill,
			};

			var specialistFrame = new FrameBox
			{
				Parent         = this.toolbar,
				PreferredWidth = 20,
				Dock           = DockStyle.Right,
				Padding        = new Margins (0, 5, 5, 5),
			};

			var panelsToolbarFrame = new FrameBox
			{
				Parent         = this.toolbar,
				DrawFullFrame  = true,
				PreferredWidth = 20,
				Dock           = DockStyle.Right,
			};

			this.CreateComptactViewSettingsUI (this.mainFrame);
			this.CreateExtendedViewSettingsUI (this.mainFrame);
			this.CreateSpecialistUI (specialistFrame);

			this.panelsToolbarController = new PanelsToolbarController (this.controller);
			this.panelsToolbarController.CreateUI (panelsToolbarFrame);

			this.UpdateTabs ();
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
#if false
				this.CopyViewSettingsToData (viewSettings);
				this.viewSettingsChangedAction ();
#else
				this.controller.MainWindowController.ShowPrésentation (viewSettings);
#endif
			}

			this.UpdateWidgets ();
		}

		private void UpdateLevel()
		{
			this.comptactFrame.Visibility = !this.specialist;
			this.extendedFrame.Visibility =  this.specialist;

			this.UpdateSpecialist ();
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
			this.UpdateTabSelected ();
			this.UpdateListSelection ();
		}


		private void CreateSpecialistUI(FrameBox parent)
		{
			this.specialistButton = new GlyphButton
			{
				Parent          = parent,
				ButtonStyle     = ButtonStyle.ToolItem,
				PreferredWidth  = 20,
				PreferredHeight = 24,
				Dock            = DockStyle.Top,
			};

			this.specialistButton.Clicked += delegate
			{
				this.specialist = !this.specialist;
				this.UpdateLevel ();
			};
		}

		private void UpdateSpecialist()
		{
			this.specialistButton.GlyphShape = this.specialist ? GlyphShape.TriangleUp : GlyphShape.TriangleDown;
		}


		private void CreateComptactViewSettingsUI(FrameBox parent)
		{
			this.comptactFrame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 34,
				Dock            = DockStyle.Top,
				Padding         = new Margins (5, 0, 5, 0),
			};

			this.compactTabsPane = new TabsPane
			{
				Parent          = this.comptactFrame,
				PreferredHeight = 34-5,
				Dock            = DockStyle.Fill,
			};

			if (this.controller.HasOptionsPanel || this.controller.HasFilterPanel)
			{
				this.compactUpdateButton = new IconButton
				{
					IconUri           = UIBuilder.GetResourceIconUri ("ViewSettings.Update"),
					PreferredIconSize = new Size (20, 20),
					PreferredSize     = new Size (24, 24),
				};

				this.compactUseButton = new IconButton
				{
					IconUri           = UIBuilder.GetResourceIconUri ("ViewSettings.Use"),
					PreferredIconSize = new Size (20, 20),
					PreferredSize     = new Size (24, 24),
				};

				ToolTip.Default.SetToolTip (this.compactUseButton,    "Utilise le filtre et les options définis dans le réglage de présentation");
				ToolTip.Default.SetToolTip (this.compactUpdateButton, "Met à jour le réglage de présentation d'après le filtre et les options en cours");

				this.compactTabsPane.AddAdditionnalWidget (this.compactUpdateButton);
				this.compactTabsPane.AddAdditionnalWidget (this.compactUseButton);

				this.compactUseButton.Clicked += delegate
				{
					this.CopyViewSettingsToData (this.viewSettingsList.Selected);
					this.ViewSettingsChanged ();
				};

				this.compactUpdateButton.Clicked += delegate
				{
					string message = string.Format ("Voulez-vous vraiment mettre à jour le réglage de présentation \"{0}\"<br/>d'après le filtre et les options en cours ?", this.viewSettingsList.Selected.Name);
					var result = this.controller.MainWindowController.QuestionDialog (message);

					if (result == DialogResult.Yes)
					{
						this.UpdateViewSettingsAction ();
					}
				};
			}
		}

		private void CreateExtendedViewSettingsUI(FrameBox parent)
		{
			this.extendedFrame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 15*7+29,  // 7 lignes dans la liste
				Dock            = DockStyle.Top,
				Padding         = new Margins (5),
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
					this.UpdateTabs ();
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

			ToolTip.Default.SetToolTip (this.extendedUseButton,    "Utilise le filtre et les options définis dans le réglage de présentation");
			ToolTip.Default.SetToolTip (this.extendedAddButton,    "Conserve le filtre et les options dans un nouveau réglage de présentation");
			ToolTip.Default.SetToolTip (this.extendedUpdateButton, "Met à jour le réglage de présentation d'après le filtre et les options en cours");
			ToolTip.Default.SetToolTip (this.extendedUpButton,     "Monte le réglage de présentation d'une ligne dnas la liste");
			ToolTip.Default.SetToolTip (this.extendedDownButton,   "Descend le réglage de présentation d'une ligne dnas la liste");
			ToolTip.Default.SetToolTip (this.extendedRemoveButton, "Supprime le réglage de présentation");

			this.extendedUseButton.Clicked += delegate
			{
				this.CopyViewSettingsToData (this.viewSettingsList.Selected);
				this.ViewSettingsChanged ();
			};

			this.extendedAddButton.Clicked += delegate
			{
				var viewSettings = new ViewSettingsData
				{
					Name           = this.NewViewSettingsName,
					ControllerType = this.controller.MainWindowController.SelectedDocument,
				};

				this.CopyDataToViewSettings (viewSettings);

				this.viewSettingsList.List.Add (viewSettings);
				this.viewSettingsList.Selected = viewSettings;

				this.UpdateTabs ();
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

				this.UpdateTabs ();
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

				this.UpdateTabs ();
				this.UpdateList ();
				this.UpdateButtons ();
			};

			this.extendedRemoveButton.Clicked += delegate
			{
				int sel = this.extendedListViewSettings.SelectedItemIndex;
				this.viewSettingsList.List.RemoveAt (sel);

				this.UpdateTabs ();
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


		private void UpdateTabs()
		{
			if (this.viewSettingsList != null)
			{
#if false
				this.comptactTabFrame.Children.Clear ();

				for (int i = 0; i < this.viewSettingsList.List.Count; i++)
				{
					this.CreateTab (this.comptactTabFrame, i);
				}

				if (this.controller.HasOptionsPanel || this.controller.HasFilterPanel)
				{
					var addButton = new TabButton
					{
						Parent            = this.comptactTabFrame,
						FormattedText     = "+",
						TextBreakMode     = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
						PreferredWidth    = 26,
						PreferredHeight   = 26,
						Dock              = DockStyle.Left,
						TabIndex          = 999,
						AutoFocus         = false,
					};

					addButton.Clicked += delegate
					{
						this.AddViewSettings ();
					};
				}
#endif

				this.compactTabsPane.Clear ();

				for (int i = 0; i < this.viewSettingsList.List.Count; i++)
				{
					var item = new TabItem
					{
						Description      = this.viewSettingsList.List[i].Name,
						RenameEnable     = !this.viewSettingsList.List[i].Readonly,
						DeleteEnable     = !this.viewSettingsList.List[i].Permanent,
						RenameVisibility = true,
						DeleteVisibility = true,
					};

					this.compactTabsPane.Add (item);
				}

				if (this.controller.HasOptionsPanel || this.controller.HasFilterPanel)
				{
					var item = new TabItem
					{
						Description = " + ",
					};

					this.compactTabsPane.Add (item);
				}

				this.compactTabsPane.SelectedIndexChanged += delegate
				{
					int sel = this.compactTabsPane.SelectedIndex;

					if ((this.controller.HasOptionsPanel || this.controller.HasFilterPanel) && sel == this.viewSettingsList.List.Count)
					{
						this.AddViewSettings ();
					}
					else
					{
						this.viewSettingsList.SelectedIndex = sel;
						this.UpdateAfterSelectionChanged ();
						this.ViewSettingsChanged ();
					}
				};

				this.compactTabsPane.RenameDoing += new TabsPane.RenameEventHandler (this.HandlerTabsPaneRenameDoing);
				this.compactTabsPane.DeleteDoing += new TabsPane.DeleteEventHandler (this.HandlerTabsPaneDeleteDoing);
			}
		}

		private void HandlerTabsPaneRenameDoing(object sender, int index, FormattedText text)
		{
			this.viewSettingsList.List[index].Name = text;

			this.UpdateAfterSelectionChanged ();
			this.ViewSettingsChanged ();
		}

		private void HandlerTabsPaneDeleteDoing(object sender, int index)
		{
			this.viewSettingsList.List.RemoveAt (index);

			if (this.viewSettingsList.SelectedIndex >= this.viewSettingsList.List.Count)
			{
				this.viewSettingsList.SelectedIndex = this.viewSettingsList.List.Count-1;
			}

			this.UpdateAfterSelectionChanged ();
			this.ViewSettingsChanged ();
		}

		private void CreateTab(Widget parent, int index)
		{
			bool select = index == this.viewSettingsList.SelectedIndex;

			var button = new TabButton
			{
				Parent            = parent,
				FormattedText     = this.viewSettingsList.List[index].Name,
				TextBreakMode     = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				ActiveState       = select ? ActiveState.Yes : ActiveState.No,
				PreferredHeight   = 26,
				Dock              = DockStyle.Left,
				TabIndex          = index,
				AutoFocus         = false,
			};

			button.PreferredWidth = button.GetBestFitSize ().Width + 10;

			button.Clicked += delegate
			{
				this.viewSettingsList.SelectedIndex = button.TabIndex;
				this.UpdateAfterSelectionChanged ();
				this.ViewSettingsChanged ();
			};
		}

		private void UpdateTabSelected()
		{
			if (this.viewSettingsList != null)
			{
#if false
				foreach (var widget in this.comptactTabFrame.Children)
				{
					var button = widget as TabButton;

					if (button != null)
					{
						bool select = button.TabIndex == this.viewSettingsList.SelectedIndex;
						button.ActiveState = select ? ActiveState.Yes : ActiveState.No;
					}
				}
#endif

				this.compactTabsPane.SelectedIndex = this.viewSettingsList.SelectedIndex;
			}
		}

		private void AddViewSettings()
		{
			var viewSettings = new ViewSettingsData
			{
				Name           = this.NewViewSettingsName,
				ControllerType = this.controller.MainWindowController.SelectedDocument,
			};

			this.CopyDataToViewSettings (viewSettings);

			this.viewSettingsList.List.Add (viewSettings);
			this.viewSettingsList.Selected = viewSettings;

			this.UpdateTabs ();
			this.UpdateList ();
			this.UpdateButtons ();
			this.UpdateSummary ();

			this.ViewSettingsChanged ();
		}


		private void UpdateList()
		{
			if (this.viewSettingsList != null)
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
		}

		private void UpdateListSelection()
		{
			if (this.viewSettingsList != null)
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

					//?this.compactComboViewSettings.SelectedItemIndex = this.extendedListViewSettings.SelectedItemIndex;
				}
			}
		}

		private void UpdateAfterSelectionChanged()
		{
			if (this.viewSettingsList != null)
			{
				using (this.ignoreChanges.Enter ())
				{
					//?this.compactComboViewSettings.SelectedItemIndex = this.viewSettingsList.SelectedIndex;
					this.UpdateTabSelected ();
					this.extendedListViewSettings.SelectedItemIndex = this.viewSettingsList.SelectedIndex;
				}
			}
		}

		private void UpdateButtons()
		{
			if (this.viewSettingsList != null)
			{
				int sel = this.extendedListViewSettings.SelectedItemIndex;
				int count = this.viewSettingsList.List.Count;
				bool eq = this.CompareTo (this.viewSettingsList.Selected);

				if (this.compactUseButton != null)
				{
					this.compactUseButton.Enable    = (sel != -1 && !eq);
					this.compactUpdateButton.Enable = (sel != -1 && !eq && !this.IsReadonly);
				}

				this.extendedAddButton.Enable    = true;
				this.extendedUseButton.Enable    = (sel != -1 && !eq);
				this.extendedUpButton.Enable     = (sel != -1 && sel > 0);
				this.extendedUpdateButton.Enable = (sel != -1 && !eq && !this.IsReadonly);
				this.extendedDownButton.Enable   = (sel != -1 && sel < count-1);
				this.extendedRemoveButton.Enable = (sel != -1 && !this.IsPermanent);

				this.extendedFieldName.IsReadOnly = this.IsReadonly;
				this.extendedFieldName.Invalidate ();  // pour contourner un bug !
			}
		}

		private void UpdateSummary()
		{
			if (this.viewSettingsList != null)
			{
				using (this.ignoreChanges.Enter ())
				{
					var viewSettings = this.viewSettingsList.Selected;

					if (viewSettings == null)
					{
						var compactSummary = FormattedText.Empty;
					}
					else
					{
						//?this.compactSummary.FormattedText = viewSettings.GetSummary (this.controller.ColumnMappers);

						this.extendedAttributePermanent.ActiveState = viewSettings.Permanent ? ActiveState.Yes : ActiveState.No;
						this.extendedAttributeReadonly .ActiveState = viewSettings.Readonly  ? ActiveState.Yes : ActiveState.No;

						//?ToolTip.Default.SetToolTip (this.comptactFrame, this.TooltipSummary);
					}

					this.extendedAttributePermanent.Visibility = (viewSettings != null);
					this.extendedAttributeReadonly .Visibility = (viewSettings != null);
				}
			}
		}

		private FormattedText TooltipSummary
		{
			get
			{
				var builder = new System.Text.StringBuilder ();

				var viewSettings = (this.viewSettingsList == null) ? null : this.viewSettingsList.Selected;

				if (viewSettings == null)
				{
					builder.Append ("Aucun");
				}
				else
				{
					FormattedText summary;

					summary = viewSettings.GetFilterSummary (this.controller.ColumnMappers);
					if (!summary.IsNullOrEmpty)
					{
						builder.Append (UIBuilder.GetTextIconUri ("Panel.Filter"));
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
				}

				var text = builder.ToString ();

				if (text.EndsWith ("<br/>"))
				{
					text = text.Substring (0, text.Length-5);  // enlève le <br/> à la fin
				}

				return text;
			}
		}


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
				if (this.dataAccessor != null && this.dataAccessor.FilterData != null && viewSettings.BaseFilter != null)
				{
					if (!this.dataAccessor.FilterData.CompareTo (viewSettings.BaseFilter))
					{
						return false;
					}
				}

				if (this.dataAccessor != null && this.dataAccessor.Options != null && viewSettings.BaseOptions != null)
				{
					if (!this.dataAccessor.Options.CompareTo (viewSettings.BaseOptions))
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
			if (this.dataAccessor != null && this.dataAccessor.FilterData != null)
			{
				viewSettings.BaseFilter = this.dataAccessor.FilterData.CopyFrom ();
			}

			if (this.dataAccessor != null && this.dataAccessor.Options != null)
			{
				viewSettings.BaseOptions = this.dataAccessor.Options.CopyFrom ();
			}
		}

		private void CopyViewSettingsToData(ViewSettingsData viewSettings)
		{
			//	Utilise un réglage de présentation (viewSettings -> panneaux).
			if (this.dataAccessor != null && this.dataAccessor.FilterData != null && viewSettings.BaseFilter != null)
			{
				viewSettings.BaseFilter.CopyTo (this.dataAccessor.FilterData);
			}

			if (this.dataAccessor != null && this.dataAccessor.Options != null && viewSettings.BaseOptions != null)
			{
				viewSettings.BaseOptions.CopyTo (this.dataAccessor.Options);
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

		private FrameBox								titleFrame;
		private StaticText								titleLabel;
		private FrameBox								mainFrame;
		private FrameBox								toolbar;
		private bool									showPanel;
		private bool									specialist;

		private GlyphButton								specialistButton;

		private FrameBox								comptactFrame;
		private Button									compactUseButton;
		private Button									compactUpdateButton;
		private StaticText								compactSummary;
		private TabsPane								compactTabsPane;

		private FrameBox								extendedFrame;
		private ScrollList								extendedListViewSettings;
		private TextFieldEx								extendedFieldName;
		private Button									extendedUseButton;
		private Button									extendedAddButton;
		private Button									extendedUpdateButton;
		private Button									extendedUpButton;
		private Button									extendedDownButton;
		private Button									extendedRemoveButton;

		private CheckButton								extendedAttributeReadonly;
		private CheckButton								extendedAttributePermanent;

		private PanelsToolbarController					panelsToolbarController;
	}
}
