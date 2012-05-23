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

			this.viewSettingsIndexes = new List<int> ();
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
				Parent          = parent,
				BackColor       = UIBuilder.WindowBackColor2,
				PreferredHeight = 5+24,
				Dock            = DockStyle.Top,
			};

			this.tabsPane = new TabsPane
			{
				Parent          = this.toolbar,
				TabLookStyle    = TabLook.OneNote,
				SelectionColor  = Color.FromName ("White"),
				IconSize        = 20,
				PreferredHeight = 5+24,
				Dock            = DockStyle.Fill,
			};

			this.CreateLeftUI ();
			this.CreateRightUI ();

			this.UpdateTabs ();
			this.UpdateWidgets ();
		}

		private void CreateLeftUI()
		{
			var leftFrame = new FrameBox
			{
				PreferredWidth  = 10,
				PreferredHeight = 24,
				Padding         = new Margins (0, 0, 0, 5),
			};

			this.titleFrame = new FrameBox
			{
				Parent          = leftFrame,
				PreferredWidth  = 10,
				PreferredHeight = 24,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 2, 0, 0),
			};

			this.titleLabel = new StaticText
			{
				Parent          = this.titleFrame,
				PreferredWidth  = 10,
				PreferredHeight = 24,
				Dock            = DockStyle.Fill,
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

			this.tabsPane.AddLeftWidget (leftFrame);
		}

		private void CreateRightUI()
		{
			var rightFrame = new FrameBox
			{
				PreferredWidth  = 10,
				PreferredHeight = 24,
				Padding         = new Margins (0, 0, 0, 5),
			};

			if (this.controller.HasOptionsPanel || this.controller.HasFilterPanel)
			{
				this.updateButton = new IconButton
				{
					Parent            = rightFrame,
					IconUri           = UIBuilder.GetResourceIconUri ("ViewSettings.Update"),
					PreferredIconSize = new Size (20, 20),
					PreferredSize     = new Size (24, 24),
					Dock              = DockStyle.Left,
				};

				this.useButton = new IconButton
				{
					Parent            = rightFrame,
					IconUri           = UIBuilder.GetResourceIconUri ("ViewSettings.Use"),
					PreferredIconSize = new Size (20, 20),
					PreferredSize     = new Size (24, 24),
					Dock              = DockStyle.Left,
					Margins           = new Margins (0, 10, 0, 0),
				};

				ToolTip.Default.SetToolTip (this.useButton, "Utilise le filtre et les options définis dans le réglage de présentation");
				ToolTip.Default.SetToolTip (this.updateButton, "Met à jour le réglage de présentation d'après le filtre et les options en cours");

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

			var panelsToolbarFrame = new FrameBox
			{
				Parent          = rightFrame,
				PreferredWidth  = 20,
				PreferredHeight = 24,
				Dock            = DockStyle.Left,
			};

			this.panelsToolbarController = new PanelsToolbarController (this.controller);
			this.panelsToolbarController.CreateUI (panelsToolbarFrame);

			var button = new IconButton
			{
				Parent        = rightFrame,
				IconUri       = UIBuilder.GetResourceIconUri ("Views.Menu"),
				PreferredSize = new Size (24, 24),
				Dock          = DockStyle.Left,
				Margins       = new Margins (10, 0, 0, 0),
			};

			ToolTip.Default.SetToolTip (button, "Choix des vues (pas encore disponible)");

			this.tabsPane.AddRightWidget (rightFrame);
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
				//	Cherche les index des ViewSettings accessibles à l'utilisateur.
				this.viewSettingsIndexes.Clear ();

				for (int i = 0; i < this.viewSettingsList.List.Count; i++)
				{
					if (this.HasPrésentation (this.viewSettingsList.List[i].ControllerType))
					{
						this.viewSettingsIndexes.Add (i);
					}
				}

				//	Génère les onglets des ViewSettings accessibles à l'utilisateur.
				this.tabsPane.Clear ();

				for (int i = 0; i < this.viewSettingsIndexes.Count; i++)
				{
					var index = this.viewSettingsIndexes[i];

					var item = new TabItem
					{
						Icon             = Présentations.GetTabIcon (this.viewSettingsList.List[index]),
						FormattedText    = this.viewSettingsList.List[index].Name,
						RenameEnable     = !this.viewSettingsList.List[index].Readonly,
						DeleteEnable     = !this.viewSettingsList.List[index].Readonly,
						MoveBeginEnable  = i > 0,
						MoveEndEnable    = i < this.viewSettingsIndexes.Count-1,
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
						Icon    = "Edit.Tab.Create",
						Tooltip = "Crée une nouvelle présentation",
					};

					this.tabsPane.Add (item);
				}

				this.tabsPane.SelectedIndexChanged += new EventHandler (this.HandlerTabsPaneSelectedIndexChanged);
				this.tabsPane.RenameDoing += new TabsPane.RenameEventHandler (this.HandlerTabsPaneRenameDoing);
				this.tabsPane.DeleteDoing += new TabsPane.DeleteEventHandler (this.HandlerTabsPaneDeleteDoing);
				this.tabsPane.DraggingDoing += new TabsPane.DraggingEventHandler (this.HandlerTabsPaneDraggingDoing);
			}
		}

		private bool HasPrésentation(ControllerType type)
		{
			if (type == ControllerType.Login)
			{
				return true;  // le login est toujours possible !
			}

			var user = this.controller.MainWindowController.CurrentUser;
			if (user == null)  // déconnecté ?
			{
				return false;
			}
			else
			{
				if (user.Admin)
				{
					return true;  // l'administrateur a toujours accès à tout
				}
				else
				{
					return Présentations.ContainsPrésentationType (user.Présentations, type);
				}
			}
		}

		private void HandlerTabsPaneSelectedIndexChanged(object sender)
		{
			int index = this.tabsPane.SelectedIndex;

			if ((this.controller.HasOptionsPanel || this.controller.HasFilterPanel) && index == this.viewSettingsIndexes.Count)  // onglet "+" ?
			{
				this.AddViewSettings ();
			}
			else
			{
				int sel = this.viewSettingsIndexes[index];
				this.viewSettingsList.SelectedIndex = sel;
				this.UpdateAfterSelectionChanged ();
				this.ViewSettingsChanged ();
			}
		}

		private void HandlerTabsPaneRenameDoing(object sender, int index, FormattedText text)
		{
			int sel = this.viewSettingsIndexes[index];
			this.viewSettingsList.List[sel].Name = text;

			this.UpdateAfterSelectionChanged ();
			this.ViewSettingsChanged ();
		}

		private void HandlerTabsPaneDeleteDoing(object sender, int index)
		{
			int sel = this.viewSettingsIndexes[index];
			this.viewSettingsList.List.RemoveAt (sel);
			this.viewSettingsIndexes.RemoveAt (index);

			if (index >= this.viewSettingsIndexes.Count)
			{
				index = this.viewSettingsIndexes.Count-1;
			}

			sel = this.viewSettingsIndexes[index];
			this.viewSettingsList.SelectedIndex = sel;

			this.UpdateAfterSelectionChanged ();
			this.ViewSettingsChanged ();
		}

		private void HandlerTabsPaneDraggingDoing(object sender, int srcIndex, int dstIndex)
		{
			if (srcIndex == dstIndex || srcIndex == dstIndex-1)
			{
				return;
			}

			if (srcIndex < this.viewSettingsIndexes.Count)
			{
				srcIndex = this.viewSettingsIndexes[srcIndex];
			}
			else
			{
				srcIndex = this.viewSettingsList.List.Count;
			}

			if (dstIndex < this.viewSettingsIndexes.Count)
			{
				dstIndex = this.viewSettingsIndexes[dstIndex];
			}
			else
			{
				dstIndex = this.viewSettingsList.List.Count;
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

		private void UpdateTabSelected()
		{
			if (this.viewSettingsList != null)
			{
				int sel = this.viewSettingsIndexes.IndexOf (this.viewSettingsList.SelectedIndex);
				this.tabsPane.SelectedIndex = sel;
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
		private readonly List<int>						viewSettingsIndexes;

		private System.Action							viewSettingsChangedAction;

		private FrameBox								toolbar;
		private TabsPane								tabsPane;
		private FrameBox								titleFrame;
		private StaticText								titleLabel;
		private Button									useButton;
		private Button									updateButton;

		private PanelsToolbarController					panelsToolbarController;
	}
}
