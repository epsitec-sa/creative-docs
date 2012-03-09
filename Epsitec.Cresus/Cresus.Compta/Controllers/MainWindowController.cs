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
using Epsitec.Cresus.Compta.Permanents.Data;
using Epsitec.Cresus.Compta.ViewSettings.Data;
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
			this.settingsData = new Dictionary<string, ISettingsData> ();
			this.settingsList = new SettingsList ();
			this.defaultSettingsList = new SettingsList ();
			this.navigatorEngine = new NavigatorEngine ();
			this.piècesGenerator = new PiècesGenerator (this);

			this.Dirty = false;

			this.showSearchPanel       = false;
			this.showFilterPanel       = false;
			this.showOptionsPanel      = false;
			this.showViewSettingsPanel = false;
			this.showInfoPanel         = true;

			Converters.ImportSettings (this.settingsList);
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

#if true
			//	Hack pour éviter de devoir tout refaire à chaque exécution !
			this.compta = new ComptaEntity ();  // crée une compta vide
			NewCompta.NewEmpty (this.compta);

			this.currentUser = this.compta.Utilisateurs.First ();  // login avec 'admin'
			new CrésusCompta ().ImportFile (this.compta, ref this.période, "C:\\Users\\Daniel\\Desktop\\Comptas\\pme 2011.crp");
			new CrésusCompta ().ImportFile (this.compta, ref this.période, "C:\\Users\\Daniel\\Desktop\\Comptas\\écritures.txt");

			this.InitializeAfterNewCompta ();
			this.ChangePériode (-1);  // en 2011
#endif
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
				this.AdaptSettingsData ();
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

				this.UpdatePrésentationCommands ();

				this.navigatorEngine.Clear ();
				this.UpdateNavigatorCommands ();

				if (this.controller != null)
				{
					this.controller.UpdateUser ();
				}
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
					this.UpdateFileCommands ();
				}
			}
		}


		public AbstractController ShowPrésentation(Command cmd)
		{
			//	Utilise une autre présentation.
			this.NavigatorUpdate ();

			this.SelectedCommandDocument = cmd;
			this.UpdatePrésentationCommands ();
			this.CreateController ();

			this.NavigatorPut ();

			return this.controller;
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

			int limit = 14;  // menu de 14 lignes au maximum
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

			string icon = string.Format (@"<img src=""{0}"" voff=""-10"" dx=""32"" dy=""32""/>   ", data.Command.Icon);

			var item = new MenuItem ()
			{
				IconUri       = UIBuilder.GetMarkStateIconUri (index == this.navigatorEngine.Index),
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
			this.SelectedCommandDocument = cmd;
			this.UpdatePrésentationCommands ();
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


		private void InitializeAfterNewCompta()
		{
			//	Initialisation pour pouvoir travailler dans une nouvelle comptabilité, soit vide
			//	(commande New) soit existante (commande Open).
			this.dirty = false;

			this.AutoLogin ();

			this.settingsData.Clear ();
			new DefaultViewSettings (this).CreateDefaultViewSettings ();

			this.SelectCurrentPériode ();
			this.SelectDefaultPrésentation ();
			this.controller.UpdateUser ();
			this.controller.ClearHilite ();
		}

		private void InitializeAfterCloseCompta()
		{
			//	Initialisation après avoir fermé la comptabilité.
			this.compta = null;
			this.dirty = false;
			this.currentUser = null;  // logout

			this.SelectDefaultPrésentation ();
		}

		private void AutoLogin()
		{
			//	S'il existe un utilisateur sans mot de passe, on le prend.
			var utilisateur = this.compta.Utilisateurs.Where (x => string.IsNullOrEmpty (x.MotDePasse)).FirstOrDefault ();

			if (utilisateur != null)
			{
				this.currentUser = utilisateur;
			}
		}

		private void SelectDefaultPrésentation()
		{
			if (this.compta == null)  // compta fermée ?
			{
				this.SelectedCommandDocument = Res.Commands.Présentation.Open;
			}
			else if (this.currentUser == null)  // déconnecté ?
			{
				this.SelectedCommandDocument = Res.Commands.Présentation.Login;
			}
			else if (this.compta.PlanComptable.Any ())  // plan comptable existe ?
			{
				this.SelectedCommandDocument = Res.Commands.Présentation.Journal;
			}
			else  // plan comptable vide ?
			{
				this.SelectedCommandDocument = Res.Commands.Présentation.PlanComptable;
			}

			this.ribbonController.PrésentationsLastButton.CommandObject = Res.Commands.Présentation.Réglages;

			this.CreateController ();
			this.UpdateTitle ();
			this.NavigatorFirst ();

			this.UpdatePrésentationCommands ();
			this.UpdateFileCommands ();
		}


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

			//	Les comparaisons ci-dessous doivent accepter les commandes Res.Commands.Présentation.Xyz
			//	et Res.Commands.NouvellePrésentation.Xyz !

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

			if (command.Name.EndsWith ("Présentation.CodesTVA"))
			{
				controller = new CodesTVAController (this.app, this.businessContext, this);
			}

			if (command.Name.EndsWith ("Présentation.ListeTVA"))
			{
				controller = new ListesTVAController (this.app, this.businessContext, this);
			}

			if (command.Name.EndsWith ("Présentation.TauxTVA"))
			{
				controller = new TauxTVAController (this.app, this.businessContext, this);
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
			if (this.compta  == null)
			{
				return "Crésus Comptabilité NG";
			}
			else
			{
				return string.Concat ("Crésus Comptabilité NG / ", this.compta.Nom, " / ", this.GetShortTitle (command));
			}
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


		public bool ShowViewSettingsPanel
		{
			get
			{
				return this.showViewSettingsPanel;
			}
			set
			{
				this.showViewSettingsPanel = value;
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


		private Command SelectedCommandDocument
		{
			get
			{
				return this.selectedCommandDocument;
			}
			set
			{
				this.selectedCommandDocument = value;

				//	S'il s'agit d'une présentation accessible par le menu, on met à jour la commande.
				if (Converters.MenuPrésentationCommands.Contains (this.selectedCommandDocument))
				{
					this.ribbonController.PrésentationsLastButton.CommandObject = this.selectedCommandDocument;
				}
			}
		}

		private void UpdatePrésentationCommands()
		{
			foreach (var command in Converters.PrésentationCommands)
			{
				CommandState cs = this.app.CommandContext.GetCommandState (command);
				cs.ActiveState = (command == this.selectedCommandDocument) ? ActiveState.Yes : ActiveState.No;

				if (this.compta == null)
				{
					if (command == Res.Commands.Présentation.Open)
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
					if (command == Res.Commands.Présentation.Login)
					{
						cs.Enable = true;  // cette commande doit toujours être disponible !
					}
					else
					{
						if (this.currentUser == null)  // déconnecté ?
						{
							if (command == Res.Commands.Présentation.Open)
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
							cs.Enable = this.HasPrésentationCommand (command);
						}
					}
				}
			}

			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Présentation.Menu);
				cs.Enable = (this.compta != null && this.currentUser != null);
			}

			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Présentation.New);
				cs.Enable = (this.compta != null && this.currentUser != null);
			}
		}

		private bool HasPrésentationCommand(Command cmd)
		{
			if (this.currentUser == null)  // déconnecté ?
			{
				return false;
			}
			else
			{
				if (this.currentUser.Admin)
				{
					return true;  // l'administrateur a toujours accès à tout
				}
				else
				{
					return Converters.ContainsPrésentationCommand (this.currentUser.Présentations, cmd);
				}
			}
		}


		private void UpdateFileCommands()
		{
			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.File.Import);
				cs.Enable = this.compta != null;
			}

			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.File.Save);
				cs.Enable = this.compta != null && this.dirty;
			}

			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.File.SaveAs);
				cs.Enable = this.compta != null;
			}

			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.File.Close);
				cs.Enable = this.compta != null;
			}

			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.File.Print);
				cs.Enable = this.compta != null;
			}
		}

		private void UpdatePanelCommands()
		{
			if (this.controller != null)
			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Panel.ViewSettings);
				cs.ActiveState = this.showViewSettingsPanel ? ActiveState.Yes : ActiveState.No;
				cs.Enable = (this.controller == null) ? false : this.controller.HasShowViewSettingsPanel;
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
				this.controller.UpdatePanelsShowed (this.showViewSettingsPanel, this.showSearchPanel, this.showFilterPanel, this.showOptionsPanel, this.showInfoPanel);
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
				this.AdaptSettingsData ();
				this.controller.UpdateAfterChanged ();

				this.NavigatorPut ();
			}
		}

		private void SelectCurrentPériode()
		{
			if (this.compta != null)
			{
				var now = Date.Today;
				this.période = this.compta.Périodes.Where (x => x.DateDébut.Year == now.Year).FirstOrDefault ();

				if (this.période == null)
				{
					this.période = this.compta.Périodes.First ();

					this.UpdatePériodeCommands ();
					this.AdaptSettingsData ();
				}
			}
		}

		private void AdaptSettingsData()
		{
			foreach (var data in this.settingsData.Values)
			{
				if (data is SearchData)
				{
					MainWindowController.AdaptSearchData (this.période, data as SearchData);
				}

				if (data is ViewSettingsList)
				{
					var viewSettingsList = data as ViewSettingsList;
					foreach (var viewSettingsData in viewSettingsList.List)
					{
						MainWindowController.AdaptSearchData (this.période, viewSettingsData.Search);
						MainWindowController.AdaptSearchData (this.période, viewSettingsData.Filter);
					}
				}
			}
		}

		private static void AdaptSearchData(ComptaPériodeEntity période, SearchData data)
		{
			//	Adapte une recherche pour être dans une période donnée.
			foreach (var node in data.NodesData)
			{
				foreach (var tab in node.TabsData)
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
			dialog.OwnerWindow = this.mainWindow;

			dialog.OpenDialog ();
			if (dialog.Result != DialogResult.Accept)
			{
				return null;
			}

			return dialog.FileName;
		}

		private string FileImportDialog(string filename)
		{
			var dialog = new FileOpenDialog ();

			//dialog.InitialDirectory = this.globalSettings.InitialDirectory;
			dialog.FileName = filename;
			dialog.Title = "Importation de données comptables";
			dialog.Filters.Add ("txt", "Texte tabulé", "*.txt");
			dialog.OwnerWindow = this.mainWindow;

			dialog.OpenDialog ();
			if (dialog.Result != DialogResult.Accept)
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
			if (dialog.Result != DialogResult.Accept)
			{
				return null;
			}

			return dialog.FileName;
		}

		public void ErrorDialog(FormattedText message)
		{
			var dialog = MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, message);

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
			this.compta = new ComptaEntity ();  // crée une compta vide
			NewCompta.NewEmpty (this.compta);

			this.InitializeAfterNewCompta ();
		}

		[Command (Res.CommandIds.File.Open)]
		private void CommandFileOpen()
		{
			var filename = this.FileOpenDialog (null);

			if (!string.IsNullOrEmpty (filename))
			{
				var newCompta = new ComptaEntity ();  // crée une compta vide
				string err = new CrésusCompta ().ImportFile (newCompta, ref this.période, filename);

				if (string.IsNullOrEmpty (err))
				{
					this.compta = newCompta;
					this.InitializeAfterNewCompta ();
				}
				else
				{
					this.ErrorDialog (err);
				}
			}
		}

		[Command (Res.CommandIds.File.Import)]
		private void CommandFileImport()
		{
			var filename = this.FileImportDialog (null);

			if (!string.IsNullOrEmpty (filename))
			{
				string err = new CrésusCompta ().ImportFile (this.compta, ref this.période, filename);

				if (string.IsNullOrEmpty (err))
				{
				}
				else
				{
					this.ErrorDialog (err);
				}
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

			this.UpdateFileCommands ();
		}

		[Command (Res.CommandIds.File.SaveAs)]
		private void CommandFileSaveAs()
		{
			var filename = this.FileSaveDialog (null);

			if (!string.IsNullOrEmpty (filename))
			{
			}

			this.UpdateFileCommands ();
		}

		[Command (Res.CommandIds.File.Close)]
		private void CommandFileClose()
		{
			this.InitializeAfterCloseCompta ();
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
		[Command (Cresus.Compta.Res.CommandIds.Présentation.CodesTVA)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.ListeTVA)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.TauxTVA)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.PiècesGenerator)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Utilisateurs)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Réglages)]
		private void ProcessShowPrésentation(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ShowPrésentation (e.Command);
		}

		[Command (Cresus.Compta.Res.CommandIds.Présentation.Menu)]
		private void ProcessMenuPrésentation(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ShowPrésentationsMenu ();
		}

		[Command (Cresus.Compta.Res.CommandIds.Présentation.New)]
		private void ProcessNewPrésentation(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ShowNewWindowMenu ();
		}

		[Command (Cresus.Compta.Res.CommandIds.NouvellePrésentation.Journal)]
		[Command (Cresus.Compta.Res.CommandIds.NouvellePrésentation.PlanComptable)]
		[Command (Cresus.Compta.Res.CommandIds.NouvellePrésentation.Budgets)]
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

		[Command (Res.CommandIds.Panel.ViewSettings)]
		private void CommandPanelViewSettings()
		{
			this.showViewSettingsPanel = !this.showViewSettingsPanel;
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


		[Command (Res.CommandIds.Edit.Create)]
		private void CommandEditCreate()
		{
			if (this.controller != null)
			{
				this.controller.CreateAction ();
			}
		}

		[Command (Res.CommandIds.Edit.Accept)]
		private void CommandEditAccept()
		{
			if (this.controller != null)
			{
				this.controller.AcceptAction ();
			}
		}

		[Command (Res.CommandIds.Edit.Cancel)]
		private void CommandEditCancel()
		{
			if (this.controller != null)
			{
				this.controller.CancelAction ();
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
			if (this.controller != null && this.controller.EditorController != null)
			{
				this.controller.EditorController.MoveAction (-1);
			}
		}

		[Command (Res.CommandIds.Edit.Down)]
		private void CommandEditDown()
		{
			if (this.controller != null && this.controller.EditorController != null)
			{
				this.controller.EditorController.MoveAction (1);
			}
		}

		[Command (Res.CommandIds.Edit.Duplicate)]
		private void CommandEditDuplicate()
		{
			if (this.controller != null && this.controller.EditorController != null)
			{
				this.controller.EditorController.DuplicateAction ();
			}
		}

		[Command (Res.CommandIds.Edit.Delete)]
		private void CommandEditDelete()
		{
			if (this.controller != null && this.controller.EditorController != null)
			{
				this.controller.EditorController.DeleteAction ();
			}
		}

		[Command (Res.CommandIds.Multi.Insert)]
		private void CommandMultiInsert()
		{
			if (this.controller != null && this.controller.EditorController != null)
			{
				this.controller.EditorController.MultiInsertLineAction ();
			}
		}

		[Command (Res.CommandIds.Multi.Delete)]
		private void CommandMultiDelete()
		{
			if (this.controller != null && this.controller.EditorController != null)
			{
				this.controller.EditorController.MultiDeleteLineAction ();
			}
		}

		[Command (Res.CommandIds.Multi.Up)]
		private void CommandMultiUp()
		{
			if (this.controller != null && this.controller.EditorController != null)
			{
				this.controller.EditorController.MultiMoveLineAction (-1);
			}
		}

		[Command (Res.CommandIds.Multi.Down)]
		private void CommandMultiDown()
		{
			if (this.controller != null && this.controller.EditorController != null)
			{
				this.controller.EditorController.MultiMoveLineAction (1);
			}
		}

		[Command (Res.CommandIds.Multi.Swap)]
		private void CommandMultiSwap()
		{
			if (this.controller != null && this.controller.EditorController != null)
			{
				this.controller.EditorController.MultiLineSwapAction ();
			}
		}

		[Command (Res.CommandIds.Multi.Auto)]
		private void CommandMultiAuto()
		{
			if (this.controller != null && this.controller.EditorController != null)
			{
				this.controller.EditorController.MultiLineAutoAction ();
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
			this.controller.EditorController.InsertModèle (n);
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


		#region Présentations menu
		private void ShowPrésentationsMenu()
		{
			this.ShowPrésentationsMenu (this.ribbonController.PrésentationsMenuButton);
		}

		private void ShowPrésentationsMenu(Widget parentButton)
		{
			var menu = new VMenu ();

			foreach (var command in Converters.MenuPrésentationCommands)
			{
				this.AddPrésentationToMenu (menu, command);
			}

			if (menu.Items.Count == 0)
			{
				return;
			}

			TextFieldCombo.AdjustComboSize (parentButton, menu, false);

			menu.Host = parentButton;
			menu.ShowAsComboList (parentButton, Point.Zero, parentButton);
		}

		private void AddPrésentationToMenu(VMenu menu, Command cmd)
		{
			if (this.HasPrésentationCommand (cmd))
			{
				var item = new MenuItem ()
				{
					CommandObject = cmd,
				};

				menu.Items.Add (item);
			}
		}
		#endregion


		#region New window menu
		public void ShowNewWindowMenu()
		{
			this.ShowNewWindowMenu (this.ribbonController.NewWindowMenuButton);
		}

		private void ShowNewWindowMenu(Widget parentButton)
		{
			var menu = new VMenu ();

			this.AddNewWindowToMenu (menu, Res.Commands.Présentation.Journal,          Res.CommandIds.NouvellePrésentation.Journal);
			this.AddNewWindowToMenu (menu, Res.Commands.Présentation.Extrait,          Res.CommandIds.NouvellePrésentation.Extrait);
			this.AddNewWindowToMenu (menu, Res.Commands.Présentation.Balance,          Res.CommandIds.NouvellePrésentation.Balance);
			this.AddNewWindowToMenu (menu, Res.Commands.Présentation.Bilan,            Res.CommandIds.NouvellePrésentation.Bilan);
			this.AddNewWindowToMenu (menu, Res.Commands.Présentation.PP,               Res.CommandIds.NouvellePrésentation.PP);
			this.AddNewWindowToMenu (menu, Res.Commands.Présentation.Exploitation,     Res.CommandIds.NouvellePrésentation.Exploitation);
			this.AddNewWindowToMenu (menu, Res.Commands.Présentation.Budgets,          Res.CommandIds.NouvellePrésentation.Budgets);
			this.AddNewWindowToMenu (menu, Res.Commands.Présentation.Change,           Res.CommandIds.NouvellePrésentation.Change);
			this.AddNewWindowToMenu (menu, Res.Commands.Présentation.RésuméPériodique, Res.CommandIds.NouvellePrésentation.RésuméPériodique);
			this.AddNewWindowToMenu (menu, Res.Commands.Présentation.RésuméTVA,        Res.CommandIds.NouvellePrésentation.RésuméTVA);
			this.AddNewWindowToMenu (menu, Res.Commands.Présentation.DécompteTVA,      Res.CommandIds.NouvellePrésentation.DécompteTVA);
			this.AddNewWindowToMenu (menu, Res.Commands.Présentation.PlanComptable,    Res.CommandIds.NouvellePrésentation.PlanComptable);

			if (menu.Items.Count == 0)
			{
				return;
			}

			TextFieldCombo.AdjustComboSize (parentButton, menu, false);

			menu.Host = parentButton;
			menu.ShowAsComboList (parentButton, Point.Zero, parentButton);
		}

		private void AddNewWindowToMenu(VMenu menu, Command cmd, Druid commandId)
		{
			if (this.HasPrésentationCommand (cmd))
			{
				var item = new MenuItem ()
				{
					CommandId = commandId,
				};

				menu.Items.Add (item);
			}
		}
		#endregion


		#region Settings data
		public AbstractPermanents GetSettingsPermanents<T>(string key, ComptaEntity compta)
			where T : AbstractPermanents, new ()
		{
			ISettingsData result;
			if (this.settingsData.TryGetValue (key, out result))
			{
				return result as AbstractPermanents;
			}

			AbstractPermanents data = new T ();
			data.SetComptaEntity (compta);
			data.Clear ();

			this.settingsData.Add (key, data);

			return data;
		}

		public AbstractOptions GetSettingsOptions<T>(string key, ComptaEntity compta)
			where T : AbstractOptions, new ()
		{
			ISettingsData result;
			if (this.settingsData.TryGetValue (key, out result))
			{
				return result as AbstractOptions;
			}

			AbstractOptions data = new T ();
			data.SetComptaEntity (compta);
			data.Clear ();

			this.settingsData.Add (key, data);

			return data;
		}

		public SearchData GetSettingsSearchData(string key, System.Action<SearchData> initialize = null)
		{
			ISettingsData result;
			if (this.settingsData.TryGetValue (key, out result))
			{
				return result as SearchData;
			}

			var data = new SearchData ();

			if (initialize != null)
			{
				initialize (data);
			}

			this.settingsData.Add (key, data);

			return data;
		}

		public ViewSettingsList GetViewSettingsList(string key)
		{
			ISettingsData result;
			if (this.settingsData.TryGetValue (key, out result))
			{
				return result as ViewSettingsList;
			}

			ViewSettingsList data = new ViewSettingsList ();

			this.settingsData.Add (key, data);

			return data;
		}
		#endregion


		private readonly Application						app;
		private readonly List<AbstractController>			controllers;
		private readonly Dictionary<string, ISettingsData>	settingsData;
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
		private bool										showViewSettingsPanel;
		private bool										showInfoPanel;
		private ComptaUtilisateurEntity						currentUser;
	}
}
