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
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.IO;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Settings.Data;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Permanents;
using Epsitec.Cresus.Compta.ViewSettings.Data;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Graph;

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
		public MainWindowController(ComptaApplication app)
		{
			this.app = app;

			this.businessContext = null;
			this.settingsData = new Dictionary<string, ISettingsData> ();
			this.settingsList = new SettingsList ();
			this.defaultSettingsList = new SettingsList ();
			this.navigatorEngine = new NavigatorEngine ();
			this.piècesGenerator = new PiècesGenerator (this);
			this.temporalData = new TemporalData ();

			this.Dirty = false;

			this.showTemporalPanel = false;
			this.showInfoPanel     = true;

			Converters.ImportSettings (this.settingsList);
			this.app.CommandDispatcher.RegisterController (this);
		}


		public ControllerType SelectedDocument
		{
			get
			{
				return this.selectedDocument;
			}
		}


		public void CreateUI(Window window)
		{
			this.mainWindow = window;

			//	Crée le ruban tout en haut.
#if false
			this.ribbonController = new RibbonController (this.app);
			this.ribbonController.CreateUI (window.Root);
#endif

			this.CreateToolbarUI (window.Root);
			this.CreatePrésentationUI (window.Root);

			//	Crée la zone éditable principale.
			this.mainFrame = new FrameBox
			{
				Parent    = window.Root,
				BackColor = UIBuilder.WindowBackColor3,
				Dock      = DockStyle.Fill,
				Padding   = new Margins (3),
			};

			this.SelectDefaultPrésentation ();

#if true
			//	Hack pour éviter de devoir tout refaire à chaque exécution !
			this.compta = new ComptaEntity ();  // crée une compta vide
			NewCompta.NewEmpty (this.compta);

			this.currentUser = this.compta.Utilisateurs.First ();  // login avec 'admin'
			new CrésusCompta ().ImportFile (this.compta, ref this.période, "S:\\Epsitec.Cresus\\Cresus.Compta\\External\\Data\\pme 2011.crp");
			new CrésusCompta ().ImportFile (this.compta, ref this.période, "S:\\Epsitec.Cresus\\Cresus.Compta\\External\\Data\\écritures.txt");

			this.InitializeAfterNewCompta ();
			this.ChangePériode (-1);  // en 2011
#endif
		}

		private void CreateToolbarUI(Widget parent)
		{
			this.toolbarController = new ToolbarController (this);
			this.toolbarController.CreateUI (parent);
		}

		private void CreatePrésentationUI(Widget parent)
		{
#if false
			var frame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 44+5,
				BackColor       = UIBuilder.WindowBackColor2,
				Dock            = DockStyle.Top,
				Margins         = new Margins (-1, -1, 0, 0),
				Padding         = new Margins (0, 0, 5, 0),
			};
#else
			var frame = new GradientFrameBox
			{
				Parent              = parent,
				PreferredHeight     = 44+5,
				BackColor2          = UIBuilder.WindowBackColor1,
				BackColor1          = UIBuilder.WindowBackColor2,
				IsVerticalGradient  = true,
				//?BottomPercentOffset = 1.0 - 0.25,  // ombre dans les 25% supérieurs
				Dock                = DockStyle.Top,
				Margins             = new Margins (-1, -1, 0, 0),
				Padding             = new Margins (0, 0, 5, 0),
			};
#endif

			//	Crée un petit gap.
			new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 2,
				BackColor       = UIBuilder.WindowBackColor3,
				Dock            = DockStyle.Top,
			};

			this.présentationController = new PrésentationController (this);
			this.présentationController.CreateUI (frame);
		}


		public Window Window
		{
			get
			{
				return this.mainWindow;
			}
		}

		public CommandContext CommandContext
		{
			get
			{
				return this.app.CommandContext;
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

		public TemporalData TemporalData
		{
			get
			{
				return this.temporalData;
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


		public AbstractController ShowPrésentation(ViewSettingsData viewSettings)
		{
			this.ShowPrésentation (viewSettings.ControllerType);

			var controller = this.controller;

			//	Utilise un réglage de présentation (viewSettings -> panneaux).
			if (controller.DataAccessor != null && controller.DataAccessor.FilterData != null && viewSettings.CurrentFilter != null)
			{
				viewSettings.CurrentFilter.CopyTo (controller.DataAccessor.FilterData);
			}

			if (controller.DataAccessor != null && controller.DataAccessor.Options != null && viewSettings.CurrentOptions != null)
			{
				viewSettings.CurrentOptions.CopyTo (controller.DataAccessor.Options);
			}

			controller.UpdateAfterChanged ();
			
			return this.controller;
		}

		public AbstractController ShowPrésentation(ControllerType type)
		{
			//	Utilise une autre présentation.
			this.NavigatorUpdate ();

			this.selectedDocument = type;
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
			this.navigatorEngine.Put (this.controller, this.selectedDocument);
			this.UpdateNavigatorCommands ();
		}

		private void NavigatorUpdate()
		{
			//	Appelé avant un changement de présentation, pour mettre à jour la présentation dans l'historique.
			this.navigatorEngine.Update (this.controller, this.selectedDocument);
		}

		private void NavigatorPut()
		{
			//	Appelé après un changement de présentation, pour ajouter la nouvelle présentation dans l'historique.
			this.navigatorEngine.Put (this.controller, this.selectedDocument);
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

			string icon = UIBuilder.GetTextIconUri (Présentations.GetIcon (data.ControllerType), iconSize: 32, verticalOffset: -10);

			var item = new MenuItem ()
			{
				IconUri       = UIBuilder.GetMarkStateIconUri (index == this.navigatorEngine.Index),
				FormattedText = icon + "   " + data.Description,
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

		private void NavigatorRestore(ControllerType type)
		{
			//	Restaure une présentation utilisée précédemment.
			this.selectedDocument = type;
			this.navigatorEngine.Restore (this);
			this.CreateController ();
			this.UpdatePrésentationCommands ();

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
				this.selectedDocument = ControllerType.Open;
			}
			else if (this.currentUser == null)  // déconnecté ?
			{
				this.selectedDocument = ControllerType.Login;
			}
			else if (this.compta.PlanComptable.Any ())  // plan comptable existe ?
			{
				this.selectedDocument = ControllerType.Journal;
			}
			else  // plan comptable vide ?
			{
				this.selectedDocument = ControllerType.PlanComptable;
			}

			this.CreateController ();
			this.UpdateTitle ();
			this.NavigatorFirst ();

			this.UpdatePrésentationCommands ();
			this.UpdateFileCommands ();
		}



		private void CreateController()
		{
			this.DisposeController ();
			this.CreateController (this.mainFrame, this.selectedDocument);

			this.UpdatePanelCommands ();
			this.UpdatePériodeCommands ();
			this.UpdateNavigatorCommands ();
		}

		private void CreateController(FrameBox parent, ControllerType type)
		{
			switch (type)
			{
				case ControllerType.Open:
					this.controller = new OpenController (this.app, this.businessContext, this);
					break;

				case ControllerType.Save:
					this.controller = new SaveController (this.app, this.businessContext, this);
					break;

				case ControllerType.Print:
					this.controller = new PrintController (this.app, this.businessContext, this);
					break;

				case ControllerType.Login:
					this.controller = new LoginController (this.app, this.businessContext, this);
					break;

				case ControllerType.Périodes:
					this.controller = new PériodesController (this.app, this.businessContext, this);
					break;

				case ControllerType.Modèles:
					this.controller = new ModèlesController (this.app, this.businessContext, this);
					break;

				case ControllerType.Libellés:
					this.controller = new LibellésController (this.app, this.businessContext, this);
					break;

				case ControllerType.Journaux:
					this.controller = new JournauxController (this.app, this.businessContext, this);
					break;

				case ControllerType.Journal:
					this.controller = new JournalController (this.app, this.businessContext, this);
					break;

				case ControllerType.PlanComptable:
					this.controller = new PlanComptableController (this.app, this.businessContext, this);
					break;

				case ControllerType.Balance:
					this.controller = new BalanceController (this.app, this.businessContext, this);
					break;

				case ControllerType.ExtraitDeCompte:
					this.controller = new ExtraitDeCompteController (this.app, this.businessContext, this);
					break;

				case ControllerType.Bilan:
					this.controller = new BilanController (this.app, this.businessContext, this);
					break;

				case ControllerType.PP:
					this.controller = new PPController (this.app, this.businessContext, this);
					break;

				case ControllerType.Exploitation:
					this.controller = new ExploitationController (this.app, this.businessContext, this);
					break;

				case ControllerType.Budgets:
					this.controller = new BudgetsController (this.app, this.businessContext, this);
					break;

				case ControllerType.RésuméTVA:
					this.controller = new RésuméTVAController (this.app, this.businessContext, this);
					break;

				case ControllerType.RésuméPériodique:
					this.controller = new RésuméPériodiqueController (this.app, this.businessContext, this);
					break;

				case ControllerType.Soldes:
					this.controller = new SoldesController (this.app, this.businessContext, this);
					break;

				case ControllerType.PiècesGenerator:
					this.controller = new PiècesGeneratorController (this.app, this.businessContext, this);
					break;

				case ControllerType.CodesTVA:
					this.controller = new CodesTVAController (this.app, this.businessContext, this);
					break;

				case ControllerType.ListeTVA:
					this.controller = new ListesTVAController (this.app, this.businessContext, this);
					break;

				case ControllerType.Monnaies:
					this.controller = new MonnaiesController (this.app, this.businessContext, this);
					break;

				case ControllerType.Utilisateurs:
					this.controller = new UtilisateursController (this.app, this.businessContext, this);
					break;

				case ControllerType.Réglages:
					this.controller = new RéglagesController (this.app, this.businessContext, this);
					break;
			}

			if (this.controller != null)
			{
				this.controller.CreateUI (this.mainFrame);
				//?controller.SetVariousParameters (parentWindow, command);
			}
		}

		public void DisposeController()
		{
			if (this.controller != null)
			{
				this.controller.Dispose ();
				this.controller = null;
			}

			this.mainFrame.Children.Clear ();
		}


		private void UpdateTitle()
		{
			this.mainWindow.Text = this.GetTitle (this.selectedDocument);
		}

		public void SetTitleComplement(string text)
		{
			this.titleComplement = text;
			this.UpdateTitle ();
		}

		public string GetTitle(ControllerType type)
		{
			if (this.compta  == null)
			{
				return "Crésus Comptabilité NG";
			}
			else
			{
				return string.Concat ("Crésus Comptabilité NG / ", this.compta.Nom, " / ", this.GetShortTitle (type));
			}
		}

		private string GetShortTitle(ControllerType type)
		{
			string text = type.ToString ();  // TODO: provisoire !!!

			if (!string.IsNullOrEmpty (this.titleComplement))
			{
				text += string.Concat (" / ", this.titleComplement);
			}

			return text;
		}


		public bool ShowTemporalPanel
		{
			get
			{
				return this.showTemporalPanel;
			}
			set
			{
				this.showTemporalPanel = value;
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


		private void UpdatePrésentationCommands()
		{
			foreach (var command in Présentations.PrésentationCommands)
			{
				var type = this.GetControllerType (command);

				CommandState cs = this.app.CommandContext.GetCommandState (command);
				cs.ActiveState = (type == this.selectedDocument) ? ActiveState.Yes : ActiveState.No;

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
					if (command == Res.Commands.Présentation.Open)
					{
						cs.Enable = true;  // cette commande doit toujours être disponible !
					}
					else
					{
						cs.Enable = this.HasPrésentationCommand (command);
					}
				}
			}

			this.présentationController.UpdateTabItems ();
		}

		private ControllerType GetControllerType(Command cmd)
		{
			//	Retourne la dernière présentation utilisée pour une commande donnée.
			var list = this.GetViewSettingsList (Présentations.GetViewSettingsKey (cmd));
			if (list != null && list.Selected != null)
			{
				return list.Selected.ControllerType;
			}

			return Présentations.GetControllerType (cmd);
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
					//	Si le groupe a au moins une présentation accessible pour l'utilisateur, on considère
					//	la commande comme valide.
					foreach (var type in Présentations.GetControllerTypes (cmd))
					{
						if (Présentations.ContainsPrésentationType (this.currentUser.Présentations, type))
						{
							return true;
						}
					}

					return false;
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
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Panel.Search);
				cs.ActiveState = this.ShowSearchPanel ? ActiveState.Yes : ActiveState.No;
				cs.Enable = (this.controller == null) ? false : this.controller.HasSearchPanel;
			}

			if (this.controller != null)
			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Panel.Filter);
				cs.ActiveState = this.ShowFilterPanel ? ActiveState.Yes : ActiveState.No;
				cs.Enable = (this.controller == null) ? false : this.controller.HasFilterPanel;
			}

			if (this.controller != null)
			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Panel.Temporal);
				cs.ActiveState = this.showTemporalPanel ? ActiveState.Yes : ActiveState.No;
			}

			if (this.controller != null)
			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Panel.Options);
				cs.ActiveState = this.ShowOptionsPanel ? ActiveState.Yes : ActiveState.No;
				cs.Enable = (this.controller == null) ? false : this.controller.HasOptionsPanel;
			}

			if (this.controller != null)
			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Panel.Info);
				cs.ActiveState = this.showInfoPanel ? ActiveState.Yes : ActiveState.No;
				cs.Enable = (this.controller == null) ? false : this.controller.HasInfoPanel;
			}

			if (this.controller != null)
			{
				this.controller.UpdatePanelsShowed (this.ShowSearchPanel, this.ShowFilterPanel, this.showTemporalPanel, this.ShowOptionsPanel, this.showInfoPanel);
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
						MainWindowController.AdaptSearchData (this.période, viewSettingsData.BaseFilter);
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
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Journal)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Balance)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.ExtraitDeCompte)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Bilan)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.PP)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Budgets)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.DifférencesChange)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.RésuméPériodique)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Soldes)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.TVA)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Réglages)]
		private void ProcessShowPrésentation(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ShowPrésentation (this.GetControllerType (e.Command));
		}


		public void ClosePanelSearch()
		{
			this.ShowSearchPanel = false;
			this.UpdatePanelCommands ();
		}

		public void ClosePanelFilter()
		{
			this.ShowFilterPanel = false;
			this.UpdatePanelCommands ();
		}

		public void OpenPanelFilter()
		{
			this.ShowFilterPanel = true;
			this.UpdatePanelCommands ();
		}

		public void ClosePanelTemporal()
		{
			this.showTemporalPanel = false;
			this.UpdatePanelCommands ();
		}

		public void OpenPanelTemporal()
		{
			this.showTemporalPanel = true;
			this.UpdatePanelCommands ();
		}

		public void ClosePanelOptions()
		{
			this.ShowOptionsPanel = false;
			this.UpdatePanelCommands ();
		}


		[Command (Res.CommandIds.Panel.Search)]
		private void CommandPanelSearch()
		{
			this.ShowSearchPanel = !this.ShowSearchPanel;
			this.UpdatePanelCommands ();
		}

		[Command (Res.CommandIds.Panel.Filter)]
		private void CommandPanelFilter()
		{
			this.ShowFilterPanel = !this.ShowFilterPanel;
			this.UpdatePanelCommands ();
		}

		[Command (Res.CommandIds.Panel.Temporal)]
		private void CommandPanelTemporal()
		{
			this.showTemporalPanel = !this.showTemporalPanel;
			this.UpdatePanelCommands ();
		}

		[Command (Res.CommandIds.Panel.Options)]
		private void CommandPanelOptions()
		{
			this.ShowOptionsPanel = !this.ShowOptionsPanel;
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

		[Command (Res.CommandIds.Multi.LastLine)]
		private void CommandMultiLastLine()
		{
			if (this.controller != null)
			{
				this.controller.EditorController.MultiLastLineAction ();
			}
		}

		[Command (Res.CommandIds.Multi.InsertBefore)]
		private void CommandMultiInsertBefore()
		{
			if (this.controller != null && this.controller.EditorController != null)
			{
				this.controller.EditorController.MultiInsertLineAction (true);
			}
		}

		[Command (Res.CommandIds.Multi.InsertAfter)]
		private void CommandMultiInsertAfter()
		{
			if (this.controller != null && this.controller.EditorController != null)
			{
				this.controller.EditorController.MultiInsertLineAction (false);
			}
		}

		[Command (Res.CommandIds.Multi.InsertTVA)]
		private void CommandMultiInsertTVA()
		{
			if (this.controller != null && this.controller.EditorController != null)
			{
				this.controller.EditorController.MultiInsertTVALineAction ();
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

		[Command (Res.CommandIds.Multi.Split)]
		private void CommandMultiSplit()
		{
			if (this.controller != null && this.controller.EditorController != null)
			{
				this.controller.EditorController.MultiLineSplitAction ();
			}
		}

		[Command (Res.CommandIds.Multi.Join)]
		private void CommandMultiJoin()
		{
			if (this.controller != null && this.controller.EditorController != null)
			{
				this.controller.EditorController.MultiLineJoinAction ();
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


		private bool ShowSearchPanel
		{
			get
			{
				if (this.controller!= null)
				{
					return this.controller.ShowSearchPanel;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (this.controller != null)
				{
					this.controller.ShowSearchPanel = value;
				}
			}
		}

		private bool ShowFilterPanel
		{
			get
			{
				if (this.controller!= null)
				{
					return this.controller.ShowFilterPanel;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (this.controller != null)
				{
					this.controller.ShowFilterPanel = value;
				}
			}
		}

		private bool ShowOptionsPanel
		{
			get
			{
				if (this.controller!= null)
				{
					return this.controller.ShowOptionsPanel;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (this.controller != null)
				{
					this.controller.ShowOptionsPanel = value;
				}
			}
		}


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

#if false
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
#endif

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

			var data = new ViewSettingsList ();

			this.settingsData.Add (key, data);

			return data;
		}
		#endregion


		private readonly ComptaApplication					app;
		private readonly Dictionary<string, ISettingsData>	settingsData;
		private readonly SettingsList						settingsList;
		private readonly SettingsList						defaultSettingsList;
		private readonly NavigatorEngine					navigatorEngine;
		private readonly PiècesGenerator					piècesGenerator;
		private readonly TemporalData						temporalData;

		private Window										mainWindow;
		private BusinessContext								businessContext;
		private ComptaEntity								compta;
		private ComptaPériodeEntity							période;
		private ControllerType								selectedDocument;
		private AbstractController							controller;
		private RibbonController							ribbonController;
		private ToolbarController							toolbarController;
		private PrésentationController						présentationController;
		private FrameBox									tabFrame;
		private FrameBox									mainFrame;
		private string										titleComplement;
		private bool										dirty;
		private bool										showTemporalPanel;
		private bool										showInfoPanel;
		private ComptaUtilisateurEntity						currentUser;
	}
}
