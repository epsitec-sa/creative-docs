//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Dialogs;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.IO;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Settings.Data;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Memory.Data;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère l'unique fenêtre principale, c'est-à-dire celle qui contient le ruban.
	/// La fermeture de cette fenêtre ferme l'application.
	/// </summary>
	public class MainWindowController
	{
		public MainWindowController(Application app)
		{
			this.app = app;

			this.businessContext = null;
			this.controllers = new List<AbstractController> ();
			this.settingsDatas = new Dictionary<string, ISettingsData> ();
			this.settingsList = new SettingsList ();
			this.defaultSettingsList = new SettingsList ();
			this.navigatorEngine = new NavigatorEngine ();
			this.piècesGenerator = new PiècesGenerator (this);

			this.compta = new ComptaEntity ();  // crée une compta vide !!!
			new NewCompta ().NewEmpty (this.compta);

			new DefaultMemory (this).CreateDefaultMemory ();

			this.SelectCurrentPériode ();

			this.dirty = true;  // pour forcer la màj
			this.Dirty = false;

			this.showSearchPanel  = false;
			this.showFilterPanel  = false;
			this.showOptionsPanel = false;
			this.showMemoryPanel  = false;
			this.showInfoPanel    = true;

			this.app.CommandDispatcher.RegisterController (this);
		}


		public void CreateUI(Window window)
		{
			this.mainWindow = window;

			//	Crée le ruban tout en haut.
			this.ribbonController = new RibbonController (this.app);
			this.ribbonController.CreateUI (window.Root);

			//	Crée la zone éditable principale.
			this.mainFrame = new FrameBox
			{
				Parent  = window.Root,
				Dock    = DockStyle.Fill,
				Padding = new Margins (3),
			};

			this.SelectDefaultPrésentation ();
			this.CreateController ();
			this.UpdateTitle ();
			this.NavigatorFirst ();
		}


		public Window Window
		{
			get
			{
				return this.mainWindow;
			}
		}

		public ComptaEntity Compta
		{
			get
			{
				return this.compta;
			}
		}

		public ComptaPériodeEntity Période
		{
			get
			{
				return this.période;
			}
			set
			{
				this.période = value;
				this.AdaptSettingsDatas ();
			}
		}

		public ComptaUtilisateurEntity CurrentUser
		{
			get
			{
				return this.currentUser;
			}
			set
			{
				this.currentUser = value;
				this.PrésentationCommandsUpdate (this.selectedCommandDocument);
			}
		}

		public PiècesGenerator PiècesGenerator
		{
			get
			{
				return this.piècesGenerator;
			}
		}

		public SettingsList SettingsList
		{
			get
			{
				return this.settingsList;
			}
		}

		public SettingsList DefaultSettingsList
		{
			get
			{
				return this.defaultSettingsList;
			}
		}

		public List<AbstractController> Controllers
		{
			//	Retourne la liste de contrôleurs de toutes les fenêtres ouvertes.
			get
			{
				return this.controllers;
			}
		}


		public void SetDirty()
		{
			this.Dirty = true;
		}

		private bool Dirty
		{
			get
			{
				return this.dirty;
			}
			set
			{
				if (this.dirty != value)
				{
					this.dirty = value;

					CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.File.Save);
					cs.Enable = this.dirty;
				}
			}
		}


		private void SelectDefaultPrésentation()
		{
			if (this.compta.PlanComptable.Any ())
			{
				this.selectedCommandDocument = Res.Commands.Présentation.Journal;
			}
			else  // plan comptable vide ?
			{
				this.selectedCommandDocument = Res.Commands.Présentation.PlanComptable;
			}

			this.PrésentationCommandsUpdate (this.selectedCommandDocument);
		}


		#region Navigator
		private void NavigatorFirst()
		{
			//	Appelé la première fois, pour mettre la première présentation dans l'historique.
			this.navigatorEngine.Clear ();
			this.navigatorEngine.Put (this.controller, this.selectedCommandDocument);
			this.UpdateNavigatorCommands ();
		}

		private void NavigatorUpdate()
		{
			//	Appelé avant un changement de présentation, pour mettre à jour la présentation dans l'historique.
			this.navigatorEngine.Update (this.controller, this.selectedCommandDocument);
		}

		private void NavigatorPut()
		{
			//	Appelé après un changement de présentation, pour ajouter la nouvelle présentation dans l'historique.
			this.navigatorEngine.Put (this.controller, this.selectedCommandDocument);
			this.UpdateNavigatorCommands ();
		}

		private void NavigatorPrev()
		{
			//	Appelé lorsqu'on désire voir la présentation précédente.
			this.NavigatorUpdate ();
			this.NavigatorRestore (this.navigatorEngine.Back);
		}

		private void NavigatorNext()
		{
			//	Appelé lorsqu'on désire voir la présentation suivante.
			this.NavigatorUpdate ();
			this.NavigatorRestore (this.navigatorEngine.Forward);
		}

		private void ShowNavigatorMenu(Widget parentButton)
		{
			//	Affiche le menu pour choisir une présentation de l'historique.
			var menu = new VMenu ();

			int limit = 15;  // menu de 15 lignes au maximum
			for (int i = this.navigatorEngine.Count-1; i >= 0 ; i--)  // du plus récent au plus ancien
			{
				this.AddNavigatorMenu (menu, i);

				if (--limit == 0)
				{
					break;
				}
			}

			TextFieldCombo.AdjustComboSize (parentButton, menu, false);

			menu.Host = parentButton;
			menu.ShowAsComboList (parentButton, Point.Zero, parentButton);
		}

		private void AddNavigatorMenu(VMenu menu, int index)
		{
			var data = this.navigatorEngine.GetNavigatorData (index);

			string icon = string.Format (@"<img src=""{0}"" voff=""-6"" dx=""20"" dy=""20""/>  ", data.Command.Icon);

			var item = new MenuItem ()
			{
				IconUri       = UIBuilder.GetCheckStateIconUri (index == this.navigatorEngine.Index),
				FormattedText = icon + data.Description,
				Name          = index.ToString (),  // on ne peut pas utiliser simplement Index !
			};

			item.Clicked += delegate
			{
				this.NavigatorUpdate ();

				int i = int.Parse (item.Name);
				var cmd = this.navigatorEngine.Any (i);
				this.NavigatorRestore (cmd);
			};

			menu.Items.Add (item);
		}

		private void NavigatorRestore(Command cmd)
		{
			//	Restaure une présentation utilisée précédemment.
			this.PrésentationCommandsUpdate (cmd);
			this.selectedCommandDocument = cmd;
			this.CreateController ();

			this.navigatorEngine.Restore (this.controller);

			if (this.controller != null)
			{
				this.controller.UpdateAfterChanged ();
			}

			this.navigatorEngine.RestoreArrayController (this.controller);

			this.UpdateNavigatorCommands ();
		}

		private void UpdateNavigatorCommands()
		{
			//	Met à jour les 3 commandes de navigation.
			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Navigator.Prev);
				cs.Enable = this.navigatorEngine.PrevEnable;
			}

			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Navigator.Next);
				cs.Enable = this.navigatorEngine.NextEnable;
			}

			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Navigator.Menu);
				cs.Enable = this.navigatorEngine.Count > 1;
			}
		}
		#endregion


		private void OpenNewWindow(Command command)
		{
			//	Ouvre une nouvelle fenêtre contenant une présentation fixe donnée (command).
			//	Cette fenêtre n'a pas de ruban, et les présentations ne peuvent pas être éditées.
			var secondaryWindow = new Window ();

			secondaryWindow.WindowBounds = new Rectangle (this.mainWindow.WindowBounds.Left+50, this.mainWindow.WindowBounds.Top-600-80, 800, 600);
			secondaryWindow.Root.MinSize = new Size (640, 480);
			secondaryWindow.Text = this.GetTitle (command);

			var secondaryFrame = new FrameBox
			{
				Parent  = secondaryWindow.Root,
				Dock    = DockStyle.Fill,
				Padding = new Margins (3),
			};

			var controller = this.CreateController (secondaryWindow, command);
			controller.CreateUI (secondaryFrame);
			controllers.Add (controller);

			secondaryWindow.Show ();
			secondaryWindow.MakeActive ();

			secondaryWindow.WindowCloseClicked += delegate
			{
				this.DisposeController (controller);
				secondaryWindow.Close ();
			};
		}


		private void CreateController()
		{
			this.DisposeController ();

			this.controller = this.CreateController (this.mainWindow, this.selectedCommandDocument);

			if (this.controller != null)
			{
				this.controller.CreateUI (this.mainFrame);
				this.controllers.Add (this.controller);
			}

			this.UpdatePanelCommands ();
			this.UpdatePériodeCommands ();
			this.UpdateNavigatorCommands ();
		}

		private AbstractController CreateController(Window parentWindow, Command command)
		{
			AbstractController controller = null;

			if (command.Name.EndsWith ("Présentation.Open"))
			{
				controller = new OpenController (this.app, this.businessContext, this);
			}

			if (command.Name.EndsWith ("Présentation.Save"))
			{
				controller = new SaveController (this.app, this.businessContext, this);
			}

			if (command.Name.EndsWith ("Présentation.Print"))
			{
				controller = new PrintController (this.app, this.businessContext, this);
			}

			if (command.Name.EndsWith ("Présentation.Login"))
			{
				controller = new LoginController (this.app, this.businessContext, this);
			}

			if (command.Name.EndsWith ("Présentation.Périodes"))
			{
				controller = new PériodesController (this.app, this.businessContext, this);
			}

			if (command.Name.EndsWith ("Présentation.Modèles"))
			{
				controller = new ModèlesController (this.app, this.businessContext, this);
			}

			if (command.Name.EndsWith ("Présentation.Libellés"))
			{
				controller = new LibellésController (this.app, this.businessContext, this);
			}

			if (command.Name.EndsWith ("Présentation.Journaux"))
			{
				controller = new JournauxController (this.app, this.businessContext, this);
			}

			if (command.Name.EndsWith ("Présentation.Journal"))
			{
				controller = new JournalController (this.app, this.businessContext, this);
			}

			if (command.Name.EndsWith ("Présentation.PlanComptable"))
			{
				controller = new PlanComptableController (this.app, this.businessContext, this);
			}

			if (command.Name.EndsWith ("Présentation.Balance"))
			{
				controller = new BalanceController (this.app, this.businessContext, this);
			}

			if (command.Name.EndsWith ("Présentation.Extrait"))
			{
				controller = new ExtraitDeCompteController (this.app, this.businessContext, this);
			}

			if (command.Name.EndsWith ("Présentation.Bilan"))
			{
				controller = new BilanController (this.app, this.businessContext, this);
			}

			if (command.Name.EndsWith ("Présentation.PP"))
			{
				controller = new PPController (this.app, this.businessContext, this);
			}

			if (command.Name.EndsWith ("Présentation.Exploitation"))
			{
				controller = new ExploitationController (this.app, this.businessContext, this);
			}

			if (command.Name.EndsWith ("Présentation.Budgets"))
			{
				controller = new BudgetsController (this.app, this.businessContext, this);
			}

			if (command.Name.EndsWith ("Présentation.PiècesGenerator"))
			{
				controller = new PiècesGeneratorController (this.app, this.businessContext, this);
			}

			if (command.Name.EndsWith ("Présentation.Utilisateurs"))
			{
				controller = new UtilisateursController (this.app, this.businessContext, this);
			}

			if (command.Name.EndsWith ("Présentation.Réglages"))
			{
				controller = new RéglagesController (this.app, this.businessContext, this);
			}

			if (controller != null)
			{
				controller.SetVariousParameters (parentWindow, command);
			}

			return controller;
		}

		private void DisposeController()
		{
			this.DisposeController (this.controller);
			this.controller = null;

			this.mainFrame.Children.Clear ();
		}

		private void DisposeController(AbstractController controller)
		{
			if (controller != null)
			{
				if (this.controllers.Contains (controller))
				{
					this.controllers.Remove (controller);
				}

				controller.Dispose ();
			}
		}


		private void UpdateTitle()
		{
			this.mainWindow.Text = this.GetTitle (this.selectedCommandDocument);
		}

		public void SetTitleComplement(string text)
		{
			this.titleComplement = text;
			this.UpdateTitle ();
		}

		public string GetTitle(Command command)
		{
			return string.Concat ("Crésus MCH-2 / ", this.compta.Nom, " / ", this.GetShortTitle (command));
		}

		private string GetShortTitle(Command command)
		{
			string text = command.Description;

			if (!string.IsNullOrEmpty (this.titleComplement))
			{
				text += string.Concat (" / ", this.titleComplement);
			}

			return text;
		}


		public bool ShowMemoryPanel
		{
			get
			{
				return this.showMemoryPanel;
			}
			set
			{
				this.showMemoryPanel = value;
				this.UpdatePanelCommands ();
			}
		}

		public bool ShowSearchPanel
		{
			get
			{
				return this.showSearchPanel;
			}
			set
			{
				this.showSearchPanel = value;
				this.UpdatePanelCommands ();
			}
		}

		public bool ShowFilterPanel
		{
			get
			{
				return this.showFilterPanel;
			}
			set
			{
				this.showFilterPanel = value;
				this.UpdatePanelCommands ();
			}
		}

		public bool ShowOptionsPanel
		{
			get
			{
				return this.showOptionsPanel;
			}
			set
			{
				this.showOptionsPanel = value;
				this.UpdatePanelCommands ();
			}
		}

		public bool ShowInfoPanel
		{
			get
			{
				return this.showInfoPanel;
			}
			set
			{
				this.showInfoPanel = value;
				this.UpdatePanelCommands ();
			}
		}


		private IEnumerable<Command> PrésentationCommands
		{
			get
			{
				yield return Res.Commands.Présentation.Open;
				yield return Res.Commands.Présentation.Save;
				yield return Res.Commands.Présentation.Print;
				yield return Res.Commands.Présentation.Login;
				yield return Res.Commands.Présentation.Modèles;
				yield return Res.Commands.Présentation.Libellés;
				yield return Res.Commands.Présentation.Périodes;
				yield return Res.Commands.Présentation.Journaux;
				yield return Res.Commands.Présentation.Journal;
				yield return Res.Commands.Présentation.PlanComptable;
				yield return Res.Commands.Présentation.Balance;
				yield return Res.Commands.Présentation.Extrait;
				yield return Res.Commands.Présentation.Bilan;
				yield return Res.Commands.Présentation.PP;
				yield return Res.Commands.Présentation.Exploitation;
				yield return Res.Commands.Présentation.Budgets;
				yield return Res.Commands.Présentation.Change;
				yield return Res.Commands.Présentation.RésuméPériodique;
				yield return Res.Commands.Présentation.RésuméTVA;
				yield return Res.Commands.Présentation.DécompteTVA;
				yield return Res.Commands.Présentation.PiècesGenerator;
				yield return Res.Commands.Présentation.Utilisateurs;
				yield return Res.Commands.Présentation.Réglages;
			}
		}

		private void PrésentationCommandsUpdate(Command c)
		{
			foreach (var command in this.PrésentationCommands)
			{
				CommandState cs = this.app.CommandContext.GetCommandState (command);
				cs.ActiveState = (command == c) ? ActiveState.Yes : ActiveState.No;

				if (this.currentUser == null)
				{
					if (command == Res.Commands.Présentation.Open ||
						command == Res.Commands.Présentation.Login)
					{
						cs.Enable = true;
					}
					else
					{
						cs.Enable = false;
					}
				}
				else
				{
					if (command == Res.Commands.Présentation.Réglages && !this.HasUserAccess (UserAccess.Réglages))
					{
						cs.Enable = false;
					}
					else if (command == Res.Commands.Présentation.Utilisateurs && !this.HasUserAccess (UserAccess.Utilisateurs))
					{
						cs.Enable = false;
					}
					else if (command == Res.Commands.Présentation.PiècesGenerator && !this.HasUserAccess (UserAccess.PiècesGenerator))
					{
						cs.Enable = false;
					}
					else if (command == Res.Commands.Présentation.Libellés && !this.HasUserAccess (UserAccess.Libellés))
					{
						cs.Enable = false;
					}
					else if (command == Res.Commands.Présentation.Modèles && !this.HasUserAccess (UserAccess.Modèles))
					{
						cs.Enable = false;
					}
					else if (command == Res.Commands.Présentation.Journaux && !this.HasUserAccess (UserAccess.Journaux))
					{
						cs.Enable = false;
					}
					else if (command == Res.Commands.Présentation.Périodes && !this.HasUserAccess (UserAccess.Périodes))
					{
						cs.Enable = false;
					}
					else if (command == Res.Commands.Présentation.PlanComptable && !this.HasUserAccess (UserAccess.PlanComptable))
					{
						cs.Enable = false;
					}
					else
					{
						cs.Enable = true;
					}
				}
			}
		}

		private bool HasUserAccess(UserAccess mode)
		{
			if (this.currentUser != null)
			{
				var d = (UserAccess) this.currentUser.DroitsDaccès;
				if ((d & mode) != 0)
				{
					return true;
				}
			}

			return false;
		}


		private void UpdatePanelCommands()
		{
			if (this.controller != null)
			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Panel.Memory);
				cs.ActiveState = this.showMemoryPanel ? ActiveState.Yes : ActiveState.No;
				cs.Enable = (this.controller == null) ? false : this.controller.HasShowMemoryPanel;
			}

			if (this.controller != null)
			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Panel.Search);
				cs.ActiveState = this.showSearchPanel ? ActiveState.Yes : ActiveState.No;
				cs.Enable = (this.controller == null) ? false : this.controller.HasShowSearchPanel;
			}

			if (this.controller != null)
			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Panel.Filter);
				cs.ActiveState = this.showFilterPanel ? ActiveState.Yes : ActiveState.No;
				cs.Enable = (this.controller == null) ? false : this.controller.HasShowFilterPanel;
			}

			if (this.controller != null)
			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Panel.Options);
				cs.ActiveState = this.showOptionsPanel ? ActiveState.Yes : ActiveState.No;
				cs.Enable = (this.controller == null) ? false : this.controller.HasShowOptionsPanel;
			}

			if (this.controller != null)
			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Panel.Info);
				cs.ActiveState = this.showInfoPanel ? ActiveState.Yes : ActiveState.No;
				cs.Enable = (this.controller == null) ? false : this.controller.HasShowInfoPanel;
			}

			if (this.controller != null)
			{
				this.controller.UpdatePanelsShowed (this.showMemoryPanel, this.showSearchPanel, this.showFilterPanel, this.showOptionsPanel, this.showInfoPanel);
			}
		}

		private void UpdatePériodeCommands()
		{
			if (this.controller != null)
			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Compta.PériodePrécédente);
				cs.Enable = (this.controller.AcceptPériodeChanged && this.compta.GetPériode (this.période, -1) != null);
			}

			if (this.controller != null)
			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Compta.PériodeSuivante);
				cs.Enable = (this.controller.AcceptPériodeChanged && this.compta.GetPériode (this.période, 1) != null);
			}
		}


		private void ChangePériode(int offset)
		{
			var autrePériode = this.compta.GetPériode (this.période, offset);

			if (autrePériode != null)
			{
				this.NavigatorUpdate ();

				this.période = autrePériode;
				
				this.CreateController ();
				this.AdaptSettingsDatas ();
				this.controller.UpdateAfterChanged ();

				this.NavigatorPut ();
			}
		}

		private void SelectCurrentPériode()
		{
			var now = Date.Today;
			this.période = this.compta.Périodes.Where (x => x.DateDébut.Year == now.Year).FirstOrDefault ();

			if (this.période == null)
			{
				this.période = this.compta.Périodes.First ();

				this.UpdatePériodeCommands ();
				this.AdaptSettingsDatas ();
			}
		}

		private void AdaptSettingsDatas()
		{
			foreach (var data in this.settingsDatas.Values)
			{
				if (data is SearchData)
				{
					MainWindowController.AdaptSearchData (this.période, data as SearchData);
				}

				if (data is MemoryList)
				{
					var memoryList = data as MemoryList;
					foreach (var memoryData in memoryList.List)
					{
						MainWindowController.AdaptSearchData (this.période, memoryData.Search);
						MainWindowController.AdaptSearchData (this.période, memoryData.Filter);
					}
				}
			}
		}

		private static void AdaptSearchData(ComptaPériodeEntity période, SearchData data)
		{
			//	Adapte une recherche pour être dans une période donnée.
			foreach (var tab in data.TabsData)
			{
				if (tab.Column == ColumnType.Date)
				{
					var dateDébut = Converters.ParseDate (tab.SearchText.FromText);
					if (dateDébut.HasValue)
					{
						tab.SearchText.FromText = Converters.DateToString (MainWindowController.AdaptDate (période, dateDébut.Value));
					}

					var dateFin = Converters.ParseDate (tab.SearchText.ToText);
					if (dateFin.HasValue)
					{
						tab.SearchText.ToText = Converters.DateToString (MainWindowController.AdaptDate (période, dateFin.Value));
					}
				}
			}
		}

		private static Date AdaptDate(ComptaPériodeEntity période, Date date)
		{
			//	Adapte une date pour être dans une période donnée.
			if (date < période.DateDébut ||
				date > période.DateFin   )  // date hors de la période ?
			{
				if (période.DateDébut.Year  == période.DateFin.Year &&
					période.DateDébut.Day   == 1  &&
					période.DateDébut.Month == 1  &&
					période.DateFin.Day     == 31 &&
					période.DateFin.Month   == 12)  // pile une année entière ?
				{
					date = new Date (période.DateDébut.Year, date.Month, date.Day);
				}
				else
				{
					// TODO: Il faudra faire mieux, dans le cas où la période ne correspond pas pile à une année !
					date = new Date (période.DateDébut.Year, date.Month, date.Day);
				}
			}

			return date;
		}


		#region Dialogs
		private string FileOpenDialog(string filename)
		{
			var dialog = new FileOpenDialog ();

			//dialog.InitialDirectory = this.globalSettings.InitialDirectory;
			dialog.FileName = filename;
			dialog.Title = "Ouverture d'une comptabilité";
			dialog.Filters.Add ("crp", "Plan comptable", "*.crp");
			dialog.Filters.Add ("txt", "Texte tabulé", "*.txt");
			dialog.OwnerWindow = this.mainWindow;

			dialog.OpenDialog ();
			if (dialog.Result != Common.Dialogs.DialogResult.Accept)
			{
				return null;
			}

			return dialog.FileName;
		}

		private string FileSaveDialog(string filename)
		{
			var dialog = new FileSaveDialog ();

			//dialog.InitialDirectory = this.globalSettings.InitialDirectory;
			dialog.FileName = filename;
			dialog.Title = "Enregistrement de la comptabilité";
			dialog.Filters.Add ("crp", "Plan comptable", "*.crp");
			dialog.Filters.Add ("txt", "Texte tabulé", "*.txt");
			dialog.PromptForOverwriting = true;
			dialog.OwnerWindow = this.mainWindow;

			dialog.OpenDialog ();
			if (dialog.Result != Common.Dialogs.DialogResult.Accept)
			{
				return null;
			}

			return dialog.FileName;
		}

		public void ErrorDialog(FormattedText message)
		{
			var dialog = MessageDialog.CreateOk ("Erreur", Common.Dialogs.DialogIcon.Warning, message);

			dialog.OwnerWindow = this.mainWindow;
			dialog.OpenDialog ();
		}

		public DialogResult QuestionDialog(FormattedText message)
		{
			var dialog = MessageDialog.CreateYesNo ("Question", DialogIcon.Question, message);
			dialog.OwnerWindow = this.controller.MainWindowController.Window;
			dialog.OpenDialog ();
			return dialog.Result;
		}
		#endregion


		#region Command handlers
		[Command (Res.CommandIds.File.New)]
		private void CommandFileNew()
		{
			new NewCompta ().NewEmpty (this.compta);
			this.SelectCurrentPériode ();

			this.controller.ClearHilite();
			this.CreateController ();
			this.Dirty = false;
			this.NavigatorFirst ();
		}

		[Command (Res.CommandIds.File.Open)]
		private void CommandFileOpen()
		{
			var filename = this.FileOpenDialog (null);

			if (!string.IsNullOrEmpty (filename))
			{
				string err = new CrésusCompta ().ImportFile (this.compta, ref this.période, filename);

				this.controller.ClearHilite ();
				this.CreateController ();
				this.AdaptSettingsDatas ();

				if (string.IsNullOrEmpty (err))
				{
					this.NavigatorFirst ();
				}
				else
				{
					this.ErrorDialog (err);
				}

				this.Dirty = false;
			}
		}

		[Command (Res.CommandIds.File.Save)]
		private void CommandFileSave()
		{
			var filename = this.FileSaveDialog (null);

			if (!string.IsNullOrEmpty (filename))
			{
				this.Dirty = false;
			}
		}

		[Command (Res.CommandIds.File.SaveAs)]
		private void CommandFileSaveAs()
		{
			var filename = this.FileSaveDialog (null);

			if (!string.IsNullOrEmpty (filename))
			{
			}
		}

		[Command (Res.CommandIds.File.Print)]
		private void CommandFilePrint()
		{
		}

		[Command (Cresus.Compta.Res.CommandIds.Présentation.Open)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Save)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Print)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Login)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Périodes)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Modèles)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Libellés)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Journaux)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Journal)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.PlanComptable)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Balance)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Extrait)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Bilan)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.PP)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Exploitation)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Budgets)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Change)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.RésuméPériodique)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.RésuméTVA)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.DécompteTVA)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.PiècesGenerator)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Utilisateurs)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Réglages)]
		private void ProcessShowPrésentation(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.NavigatorUpdate ();

			this.PrésentationCommandsUpdate (e.Command);
			this.selectedCommandDocument = e.Command;
			this.CreateController ();

			this.NavigatorPut ();
		}

		[Command (Cresus.Compta.Res.CommandIds.Présentation.New)]
		private void ProcessNewPrésentation(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ribbonController.ShowNewWindowMenu ();
		}

		[Command (Cresus.Compta.Res.CommandIds.NouvellePrésentation.Balance)]
		[Command (Cresus.Compta.Res.CommandIds.NouvellePrésentation.Extrait)]
		[Command (Cresus.Compta.Res.CommandIds.NouvellePrésentation.Bilan)]
		[Command (Cresus.Compta.Res.CommandIds.NouvellePrésentation.PP)]
		[Command (Cresus.Compta.Res.CommandIds.NouvellePrésentation.Exploitation)]
		[Command (Cresus.Compta.Res.CommandIds.NouvellePrésentation.Change)]
		[Command (Cresus.Compta.Res.CommandIds.NouvellePrésentation.RésuméPériodique)]
		[Command (Cresus.Compta.Res.CommandIds.NouvellePrésentation.RésuméTVA)]
		[Command (Cresus.Compta.Res.CommandIds.NouvellePrésentation.DécompteTVA)]
		private void ProcessShowNouvellePrésentation(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.OpenNewWindow (e.Command);
		}


		[Command (Res.CommandIds.Panel.Search)]
		private void CommandPanelSearch()
		{
			this.showSearchPanel = !this.showSearchPanel;
			this.UpdatePanelCommands ();
		}

		[Command (Res.CommandIds.Panel.Filter)]
		private void CommandPanelFilter()
		{
			this.showFilterPanel = !this.showFilterPanel;
			this.UpdatePanelCommands ();
		}

		[Command (Res.CommandIds.Panel.Options)]
		private void CommandPanelOptions()
		{
			this.showOptionsPanel = !this.showOptionsPanel;
			this.UpdatePanelCommands ();
		}

		[Command (Res.CommandIds.Panel.Memory)]
		private void CommandPanelMemory()
		{
			this.showMemoryPanel = !this.showMemoryPanel;
			this.UpdatePanelCommands ();
		}

		[Command (Res.CommandIds.Panel.Info)]
		private void CommandPanelInfo()
		{
			this.showInfoPanel = !this.showInfoPanel;
			this.UpdatePanelCommands ();
		}


		[Command (Res.CommandIds.Select.Up)]
		private void CommandSelectUp()
		{
			if (this.controller != null && this.controller.ArrayController != null)
			{
				this.controller.ArrayController.MoveSelection (-1);
			}
		}

		[Command (Res.CommandIds.Select.Down)]
		private void CommandSelectDown()
		{
			if (this.controller != null && this.controller.ArrayController != null)
			{
				this.controller.ArrayController.MoveSelection (1);
			}
		}

		[Command (Res.CommandIds.Select.Home)]
		private void CommandSelectHome()
		{
			if (this.controller != null && this.controller.ArrayController != null)
			{
				this.controller.ArrayController.MoveSelection (0);
			}
		}


		[Command (Res.CommandIds.Edit.Accept)]
		private void CommandEditAccept()
		{
			if (this.controller != null && this.controller.FooterController != null)
			{
				this.controller.FooterController.AcceptAction ();
			}
		}

		[Command (Res.CommandIds.Edit.Cancel)]
		private void CommandEditCancel()
		{
			if (this.controller != null && this.controller.FooterController != null)
			{
				this.controller.FooterController.CancelAction ();
			}
		}

		[Command (Res.CommandIds.Edit.Undo)]
		private void CommandEditUndo()
		{
		}

		[Command (Res.CommandIds.Edit.Redo)]
		private void CommandEditRedo()
		{
		}

		[Command (Res.CommandIds.Edit.Up)]
		private void CommandEditUp()
		{
			if (this.controller != null && this.controller.FooterController != null)
			{
				this.controller.FooterController.MoveAction (-1);
			}
		}

		[Command (Res.CommandIds.Edit.Down)]
		private void CommandEditDown()
		{
			if (this.controller != null && this.controller.FooterController != null)
			{
				this.controller.FooterController.MoveAction (1);
			}
		}

		[Command (Res.CommandIds.Edit.Duplicate)]
		private void CommandEditDuplicate()
		{
			if (this.controller != null && this.controller.FooterController != null)
			{
				this.controller.FooterController.DuplicateAction ();
			}
		}

		[Command (Res.CommandIds.Edit.Delete)]
		private void CommandEditDelete()
		{
			if (this.controller != null && this.controller.FooterController != null)
			{
				this.controller.FooterController.DeleteAction ();
			}
		}

		[Command (Res.CommandIds.Multi.Insert)]
		private void CommandMultiInsert()
		{
			if (this.controller != null && this.controller.FooterController != null)
			{
				this.controller.FooterController.MultiInsertLineAction ();
			}
		}

		[Command (Res.CommandIds.Multi.Delete)]
		private void CommandMultiDelete()
		{
			if (this.controller != null && this.controller.FooterController != null)
			{
				this.controller.FooterController.MultiDeleteLineAction ();
			}
		}

		[Command (Res.CommandIds.Multi.Up)]
		private void CommandMultiUp()
		{
			if (this.controller != null && this.controller.FooterController != null)
			{
				this.controller.FooterController.MultiMoveLineAction (-1);
			}
		}

		[Command (Res.CommandIds.Multi.Down)]
		private void CommandMultiDown()
		{
			if (this.controller != null && this.controller.FooterController != null)
			{
				this.controller.FooterController.MultiMoveLineAction (1);
			}
		}

		[Command (Res.CommandIds.Multi.Swap)]
		private void CommandMultiSwap()
		{
			if (this.controller != null && this.controller.FooterController != null)
			{
				this.controller.FooterController.MultiLineSwapAction ();
			}
		}

		[Command (Res.CommandIds.Multi.Auto)]
		private void CommandMultiAuto()
		{
			if (this.controller != null && this.controller.FooterController != null)
			{
				this.controller.FooterController.MultiLineAutoAction ();
			}
		}

		[Command (Res.CommandIds.Modèle.Insert0)]
		[Command (Res.CommandIds.Modèle.Insert1)]
		[Command (Res.CommandIds.Modèle.Insert2)]
		[Command (Res.CommandIds.Modèle.Insert3)]
		[Command (Res.CommandIds.Modèle.Insert4)]
		[Command (Res.CommandIds.Modèle.Insert5)]
		[Command (Res.CommandIds.Modèle.Insert6)]
		[Command (Res.CommandIds.Modèle.Insert7)]
		[Command (Res.CommandIds.Modèle.Insert8)]
		[Command (Res.CommandIds.Modèle.Insert9)]
		private void CommandModèleInsert(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			int n = e.Command.Name.Last () - '0';  // 0..9
			this.controller.FooterController.InsertModèle (n);
		}

		[Command (Res.CommandIds.Compta.PériodePrécédente)]
		private void CommandComptaPériodePrécédente()
		{
			this.ChangePériode (-1);
		}

		[Command (Res.CommandIds.Compta.PériodeSuivante)]
		private void CommandComptaPériodeSuivante()
		{
			this.ChangePériode (1);
		}

		[Command (Res.CommandIds.Navigator.Prev)]
		private void CommandNavigatorPrev()
		{
			this.NavigatorPrev ();
		}

		[Command (Res.CommandIds.Navigator.Next)]
		private void CommandNavigatorNext()
		{
			this.NavigatorNext ();
		}

		[Command (Res.CommandIds.Navigator.Menu)]
		private void CommandNavigatorMenu()
		{
			this.ShowNavigatorMenu (this.ribbonController.NavigatorMenuButton);
		}

		[Command (Res.CommandIds.Global.Settings)]
		private void CommandGlobalSettings()
		{
		}
		#endregion


		#region Settings data
		public AbstractOptions GetSettingsOptions<T>(string key, ComptaEntity compta)
			where T : AbstractOptions, new ()
		{
			ISettingsData result;
			if (this.settingsDatas.TryGetValue (key, out result))
			{
				return result as AbstractOptions;
			}

			AbstractOptions data = new T ();
			data.SetComptaEntity (compta);
			data.Clear ();

			this.settingsDatas.Add (key, data);

			return data;
		}

		public SearchData GetSettingsSearchData(string key, System.Action<SearchData> initialize = null)
		{
			ISettingsData result;
			if (this.settingsDatas.TryGetValue (key, out result))
			{
				return result as SearchData;
			}

			var data = new SearchData ();

			if (initialize != null)
			{
				initialize (data);
			}

			this.settingsDatas.Add (key, data);

			return data;
		}

		public MemoryList GetMemoryList(string key)
		{
			ISettingsData result;
			if (this.settingsDatas.TryGetValue (key, out result))
			{
				return result as MemoryList;
			}

			MemoryList data = new MemoryList ();

			this.settingsDatas.Add (key, data);

			return data;
		}
		#endregion


		private readonly Application						app;
		private readonly List<AbstractController>			controllers;
		private readonly Dictionary<string, ISettingsData>	settingsDatas;
		private readonly SettingsList						settingsList;
		private readonly SettingsList						defaultSettingsList;
		private readonly NavigatorEngine					navigatorEngine;
		private readonly PiècesGenerator					piècesGenerator;

		private Window										mainWindow;
		private BusinessContext								businessContext;
		private ComptaEntity								compta;
		private ComptaPériodeEntity							période;
		private Command										selectedCommandDocument;
		private AbstractController							controller;
		private RibbonController							ribbonController;
		private FrameBox									mainFrame;
		private string										titleComplement;
		private bool										dirty;
		private bool										showSearchPanel;
		private bool										showFilterPanel;
		private bool										showOptionsPanel;
		private bool										showMemoryPanel;
		private bool										showInfoPanel;
		private ComptaUtilisateurEntity						currentUser;
	}
}
