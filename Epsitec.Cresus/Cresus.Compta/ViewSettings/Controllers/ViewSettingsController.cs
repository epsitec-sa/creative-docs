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

			var viewsFrame = new FrameBox
			{
				Parent         = this.toolbar,
				PreferredWidth = 20,
				Dock           = DockStyle.Right,
				Padding        = new Margins (5),
			};

			var button = new IconButton
			{
				Parent        = viewsFrame,
				IconUri       = UIBuilder.GetResourceIconUri ("Views.Menu"),
				PreferredSize = new Size (24, 24),
				Dock          = DockStyle.Top,
			};

			ToolTip.Default.SetToolTip (button, "Choix des vues (pas encore disponible)");

			var panelsToolbarFrame = new FrameBox
			{
				Parent         = this.toolbar,
				DrawFullFrame  = true,
				PreferredWidth = 20,
				Dock           = DockStyle.Right,
			};

			this.CreateComptactViewSettingsUI (this.mainFrame);

			this.panelsToolbarController = new PanelsToolbarController (this.controller);
			this.panelsToolbarController.CreateUI (panelsToolbarFrame);

			this.UpdateTabs ();
			this.UpdateWidgets ();
		}

		private void ClearAction()
		{
			this.viewSettingsList.SelectedIndex = 0;
			this.UpdateAfterSelectionChanged ();
			this.ViewSettingsChanged ();
		}

		private void ViewSettingsChanged()
		{
			var viewSettings = this.viewSettingsList.Selected;
			if (viewSettings != null)
			{
				this.controller.MainWindowController.ShowPrésentation (viewSettings);
			}

			this.UpdateWidgets ();
		}

		private void UpdateWidgets()
		{
			this.UpdateButtons ();
		}

		public void Update()
		{
			this.UpdateButtons ();
			this.UpdateTabSelected ();
		}


		private void CreateComptactViewSettingsUI(FrameBox parent)
		{
			var frame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 34,
				Dock            = DockStyle.Top,
				Padding         = new Margins (5, 0, 5, 0),
			};

			this.tabsPane = new TabsPane
			{
				Parent          = frame,
				PreferredHeight = 34-5,
				Dock            = DockStyle.Fill,
			};

			if (this.controller.HasOptionsPanel || this.controller.HasFilterPanel)
			{
				this.updateButton = new IconButton
				{
					IconUri           = UIBuilder.GetResourceIconUri ("ViewSettings.Update"),
					PreferredIconSize = new Size (20, 20),
					PreferredSize     = new Size (24, 24),
				};

				this.useButton = new IconButton
				{
					IconUri           = UIBuilder.GetResourceIconUri ("ViewSettings.Use"),
					PreferredIconSize = new Size (20, 20),
					PreferredSize     = new Size (24, 24),
				};

				ToolTip.Default.SetToolTip (this.useButton,    "Utilise le filtre et les options définis dans le réglage de présentation");
				ToolTip.Default.SetToolTip (this.updateButton, "Met à jour le réglage de présentation d'après le filtre et les options en cours");

				this.tabsPane.AddAdditionnalWidget (this.updateButton);
				this.tabsPane.AddAdditionnalWidget (this.useButton);

				this.useButton.Clicked += delegate
				{
					this.CopyViewSettingsToData (this.viewSettingsList.Selected);
					this.ViewSettingsChanged ();
				};

				this.updateButton.Clicked += delegate
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

		private void UpdateViewSettingsAction()
		{
			var viewSettings = this.viewSettingsList.Selected;

			if (viewSettings != null && !viewSettings.Readonly)  // garde-fou
			{
				this.CopyDataToViewSettings (viewSettings);

				this.UpdateButtons ();
			}
		}


		private void UpdateTabs()
		{
			if (this.viewSettingsList != null)
			{
				this.tabsPane.Clear ();

				for (int i = 0; i < this.viewSettingsList.List.Count; i++)
				{
					var icon = "Edit.Tab.User";

					if (this.viewSettingsList.List[i].Readonly)
					{
						var type = this.viewSettingsList.List[i].ControllerType;

						if (type == ControllerType.Journal ||
							type == ControllerType.Extrait ||
							type == ControllerType.PlanComptable ||
							type == ControllerType.Balance ||
							type == ControllerType.Extrait ||
							type == ControllerType.Bilan ||
							type == ControllerType.PP ||
							type == ControllerType.Exploitation ||
							type == ControllerType.RésuméPériodique ||
							type == ControllerType.Soldes ||
							type == ControllerType.RésuméTVA)
						{
							icon = "Edit.Tab.System";
						}
						else
						{
							icon = "Edit.Tab.Settings";
						}
					}

					var item = new TabItem
					{
						Icon             = icon,
						FormattedText    = this.viewSettingsList.List[i].Name,
						RenameEnable     = !this.viewSettingsList.List[i].Readonly,
						DeleteEnable     = !this.viewSettingsList.List[i].Readonly,
						MoveBeginEnable  = i > 0,
						MoveEndEnable    = i < this.viewSettingsList.List.Count-1,
						RenameVisibility = true,
						DeleteVisibility = true,
						MoveVisibility   = true,
					};

					this.tabsPane.Add (item);
				}

				if (this.controller.HasOptionsPanel || this.controller.HasFilterPanel)
				{
					var item = new TabItem
					{
						FormattedText = " + ",
					};

					this.tabsPane.Add (item);
				}

				this.tabsPane.SelectedIndexChanged += new EventHandler (this.HandlerTabsPaneSelectedIndexChanged);
				this.tabsPane.RenameDoing += new TabsPane.RenameEventHandler (this.HandlerTabsPaneRenameDoing);
				this.tabsPane.DeleteDoing += new TabsPane.DeleteEventHandler (this.HandlerTabsPaneDeleteDoing);
				this.tabsPane.DraggingDoing += new TabsPane.DraggingEventHandler (this.HandlerTabsPaneDraggingDoing);
			}
		}

		private void HandlerTabsPaneSelectedIndexChanged(object sender)
		{
			int sel = this.tabsPane.SelectedIndex;

			if ((this.controller.HasOptionsPanel || this.controller.HasFilterPanel) && sel == this.viewSettingsList.List.Count)  // onglet "+" ?
			{
				this.AddViewSettings ();
			}
			else
			{
				this.viewSettingsList.SelectedIndex = sel;
				this.UpdateAfterSelectionChanged ();
				this.ViewSettingsChanged ();
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

		private void HandlerTabsPaneDraggingDoing(object sender, int srcIndex, int dstIndex)
		{
			if (srcIndex == dstIndex || srcIndex == dstIndex-1)
			{
				return;
			}

			var s = this.viewSettingsList.List[srcIndex];
			this.viewSettingsList.List.RemoveAt (srcIndex);

			if (dstIndex > srcIndex)
			{
				dstIndex--;
			}

			this.viewSettingsList.List.Insert (dstIndex, s);
			this.viewSettingsList.SelectedIndex = dstIndex;

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
				this.tabsPane.SelectedIndex = this.viewSettingsList.SelectedIndex;
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
			this.UpdateButtons ();

			this.ViewSettingsChanged ();
		}


		private void UpdateAfterSelectionChanged()
		{
			if (this.viewSettingsList != null)
			{
				this.UpdateTabSelected ();
			}
		}

		private void UpdateButtons()
		{
			if (this.viewSettingsList != null)
			{
				int sel = this.viewSettingsList.SelectedIndex;
				int count = this.viewSettingsList.List.Count;
				bool eq = this.CompareTo (this.viewSettingsList.Selected);

				if (this.useButton != null)
				{
					this.useButton.Enable    = (sel != -1 && !eq);
					this.updateButton.Enable = (sel != -1 && !eq && !this.IsReadonly);
				}
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

	
		private readonly AbstractController				controller;
		private readonly ComptaEntity					compta;
		private readonly BusinessContext				businessContext;
		private readonly AbstractDataAccessor			dataAccessor;
		private readonly ViewSettingsList				viewSettingsList;

		private System.Action							viewSettingsChangedAction;

		private FrameBox								titleFrame;
		private StaticText								titleLabel;
		private FrameBox								mainFrame;
		private FrameBox								toolbar;
		private bool									showPanel;

		private Button									useButton;
		private Button									updateButton;
		private TabsPane								tabsPane;

		private PanelsToolbarController					panelsToolbarController;
	}
}
