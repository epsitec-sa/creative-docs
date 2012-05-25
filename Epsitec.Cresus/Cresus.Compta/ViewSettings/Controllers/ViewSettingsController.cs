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
				BackColor       = UIBuilder.WindowBackColor3,
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

			this.CreateTabs ();
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

			//?Cette zone à gauche des onglets n'est plus utile !!!
			//?this.tabsPane.AddLeftWidget (leftFrame);
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
				this.reloadButton = new IconButton
				{
					Parent            = rightFrame,
					IconUri           = UIBuilder.GetResourceIconUri ("ViewSettings.Reload"),
					PreferredIconSize = new Size (20, 20),
					PreferredSize     = new Size (24, 24),
					Dock              = DockStyle.Left,
				};

				this.saveButton = new IconButton
				{
					Parent            = rightFrame,
					IconUri           = UIBuilder.GetResourceIconUri ("ViewSettings.Save"),
					PreferredIconSize = new Size (20, 20),
					PreferredSize     = new Size (24, 24),
					Dock              = DockStyle.Left,
					Visibility        = false,  // à voir !
				};

				ToolTip.Default.SetToolTip (this.reloadButton, "Réutilise les réglages initiaux");
				ToolTip.Default.SetToolTip (this.saveButton,   "Enregistre les réglages actuels");

				this.reloadButton.Clicked += delegate
				{
					this.ReloadViewSettings (this.viewSettingsList.Selected);
					this.ViewSettingsChanged ();
				};

				this.saveButton.Clicked += delegate
				{
					if (this.SaveDialog (this.viewSettingsList.Selected))
					{
						this.SaveViewSettings (this.viewSettingsList.Selected);
						this.UpdateButtons ();
					}
				};
			}

			var panelsToolbarFrame = new FrameBox
			{
				Parent          = rightFrame,
				PreferredWidth  = 20,
				PreferredHeight = 24,
				Dock            = DockStyle.Left,
				Margins         = new Margins (10, 0, 0, 0),
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


		private bool SaveDialog(ViewSettingsData viewSettings)
		{
			string message = string.Format ("Voulez-vous enregistrer le filtre et les options en cours<br/>dans les réglages \"{0}\" ?", viewSettings.Name);
			var result = this.controller.MainWindowController.QuestionDialog (message);

			return result == DialogResult.Yes;
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


		private void CreateTabs()
		{
			//	Crée tous les onglets pour les présentations.
			if (this.viewSettingsList == null)
			{
				return;
			}

			//	Cherche les index des ViewSettings accessibles à l'utilisateur.
			this.viewSettingsIndexes.Clear ();

			for (int i = 0; i < this.viewSettingsList.List.Count; i++)
			{
				if (this.HasPrésentation (this.viewSettingsList.List[i].ControllerType))
				{
					this.viewSettingsIndexes.Add (i);
				}
			}

			//	Crée les onglets des ViewSettings accessibles à l'utilisateur.
			this.tabsPane.Clear ();

			for (int i = 0; i < this.viewSettingsIndexes.Count; i++)
			{
				var index = this.viewSettingsIndexes[i];
				var viewSettings = this.viewSettingsList.List[index];

				var item = new TabItem
				{
					Icon             = Présentations.GetTabIcon (viewSettings),
					RenameEnable     = !viewSettings.Readonly,
					DeleteEnable     = !viewSettings.Readonly,
					MoveBeginEnable  = i > 0,
					MoveEndEnable    = i < this.viewSettingsIndexes.Count-1,
					RenameVisibility = true,
					DeleteVisibility = true,
					ReloadVisibility = true,
					SaveVisibility   = true,
					MoveVisibility   = true,
				};

				this.tabsPane.Add (item);
			}

			//	Si nécessaire, crée l'onglet "+".
			if (this.controller.HasOptionsPanel || this.controller.HasFilterPanel)
			{
				var item = new TabItem
				{
					Icon    = "Edit.Tab.Create",
					Tooltip = "Crée une nouvelle présentation",
				};

				this.tabsPane.Add (item);
			}

			this.UpdateTabs ();

			//	Connexion des événements.
			this.tabsPane.SelectedIndexChanged += new EventHandler                  (this.HandlerTabsPaneSelectedIndexChanged);
			this.tabsPane.RenameDoing          += new TabsPane.RenameEventHandler   (this.HandlerTabsPaneRenameDoing);
			this.tabsPane.DeleteDoing          += new TabsPane.DeleteEventHandler   (this.HandlerTabsPaneDeleteDoing);
			this.tabsPane.ReloadDoing          += new TabsPane.ReloadEventHandler   (this.HandlerTabsPaneReloadDoing);
			this.tabsPane.SaveDoing            += new TabsPane.SaveEventHandler     (this.HandlerTabsPaneSaveDoing);
			this.tabsPane.DraggingDoing        += new TabsPane.DraggingEventHandler (this.HandlerTabsPaneDraggingDoing);
		}

		private void UpdateTabs()
		{
			//	Met à jour tous les onglets pour les présentations.
			if (this.viewSettingsList == null)
			{
				return;
			}

			for (int i = 0; i < this.viewSettingsIndexes.Count; i++)
			{
				var index = this.viewSettingsIndexes[i];
				var viewSettings = this.viewSettingsList.List[index];
				bool isModified = this.IsModified (viewSettings);

				FormattedText name;
				if (isModified)
				{
					name = viewSettings.Name.ApplyBold ();
				}
				else
				{
					name = viewSettings.Name;
				}

				var item = this.tabsPane.Get (index);

				item.FormattedText = name;
				item.ReloadEnable  = isModified;
				item.SaveEnable    = isModified && !viewSettings.Readonly;

				this.tabsPane.Set (index, item);
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

		private void HandlerTabsPaneReloadDoing(object sender, int index)
		{
			int sel = this.viewSettingsIndexes[index];
			var viewSettings = this.viewSettingsList.List[sel];

			this.ReloadViewSettings (viewSettings);
			this.viewSettingsList.Selected = viewSettings;

			this.UpdateAfterSelectionChanged ();
			this.ViewSettingsChanged ();
		}

		private void HandlerTabsPaneSaveDoing(object sender, int index)
		{
			int sel = this.viewSettingsIndexes[index];
			var viewSettings = this.viewSettingsList.List[sel];

			if (this.SaveDialog (viewSettings))
			{
				this.SaveViewSettings (viewSettings);
				this.viewSettingsList.Selected = viewSettings;

				this.UpdateAfterSelectionChanged ();
				this.ViewSettingsChanged ();
			}
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

			this.CopyViewSettings (this.viewSettingsList.Selected, viewSettings);

			this.viewSettingsList.List.Add (viewSettings);
			this.viewSettingsList.Selected = viewSettings;

			this.CreateTabs ();
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
			if (this.viewSettingsList != null && this.reloadButton != null)
			{
				var viewSettings = this.viewSettingsList.Selected;

				if (viewSettings == null)
				{
					this.reloadButton.Enable = false;
					this.saveButton.Enable   = false;
				}
				else
				{
					bool isModified = this.IsModified (viewSettings);

					this.reloadButton.Enable = isModified;
					this.saveButton.Enable   = isModified && !viewSettings.Readonly;
				}
			}
		}

#if false
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
#endif

		private bool IsModified(ViewSettingsData viewSettings)
		{
			if (viewSettings != null)
			{
				if (viewSettings.BaseFilter    != null &&
					viewSettings.CurrentFilter != null &&
					viewSettings.BaseFilter.CompareTo (viewSettings.CurrentFilter) == false)
				{
					return true;
				}

				if (viewSettings.BaseOptions    != null &&
					viewSettings.CurrentOptions != null &&
					viewSettings.BaseOptions.CompareTo (viewSettings.CurrentOptions) == false)
				{
					return true;
				}
			}

			return false;
		}

#if false
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
#endif

#if false
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
#endif

		private void ReloadViewSettings(ViewSettingsData viewSettings)
		{
			if (viewSettings.BaseFilter    != null &&
				viewSettings.CurrentFilter != null)
			{
				viewSettings.BaseFilter.CopyTo (viewSettings.CurrentFilter);
			}

			if (viewSettings.BaseOptions    != null &&
				viewSettings.CurrentOptions != null)
			{
				viewSettings.BaseOptions.CopyTo (viewSettings.CurrentOptions);
			}
		}

		private void SaveViewSettings(ViewSettingsData viewSettings)
		{
			if (viewSettings.BaseFilter    != null &&
				viewSettings.CurrentFilter != null)
			{
				viewSettings.CurrentFilter.CopyTo (viewSettings.BaseFilter);
			}

			if (viewSettings.BaseOptions    != null &&
				viewSettings.CurrentOptions != null)
			{
				viewSettings.CurrentOptions.CopyTo (viewSettings.BaseOptions);
			}
		}

		private void CopyViewSettings(ViewSettingsData src, ViewSettingsData dst)
		{
			if (src.BaseFilter != null)
			{
				dst.BaseFilter = src.BaseFilter.CopyFrom ();
			}

			if (src.BaseOptions != null)
			{
				dst.BaseOptions = src.BaseOptions.CopyFrom ();
			}
		}


		private string NewViewSettingsName
		{
			get
			{
				int i = 1;

				while (true)
				{
					string name = "Vue" + " " + i.ToString ();

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
		private Button									saveButton;
		private Button									reloadButton;

		private PanelsToolbarController					panelsToolbarController;
	}
}
