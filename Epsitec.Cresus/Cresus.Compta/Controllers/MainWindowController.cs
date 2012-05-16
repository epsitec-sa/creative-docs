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

using Epsitec.Cresus.Compta.MetaControllers;
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
			//?this.controllers = new List<AbstractController> ();
			this.settingsData = new Dictionary<string, ISettingsData> ();
			this.settingsList = new SettingsList ();
			this.defaultSettingsList = new SettingsList ();
			this.navigatorEngine = new NavigatorEngine ();
			this.piècesGenerator = new PiècesGenerator (this);
			this.temporalData = new TemporalData ();
			//?this.metas = new Dictionary<MetaControllerType, List<ControllerType>> ();
			//?this.InitMetas ();

			this.Dirty = false;

			this.showSearchPanel   = false;
			this.showFilterPanel   = false;
			this.showTemporalPanel = false;
			this.showOptionsPanel  = false;
			this.showInfoPanel     = true;

			Converters.ImportSettings (this.settingsList);
			this.app.CommandDispatcher.RegisterController (this);
		}


#if false
		private void InitMetas()
		{
			{
				var list = new List<ControllerType> ();
				list.Add (ControllerType.Open);
				this.metas.Add (MetaControllerType.Open, list);
			}

			{
				var list = new List<ControllerType> ();
				list.Add (ControllerType.Save);
				this.metas.Add (MetaControllerType.Save, list);
			}

			{
				var list = new List<ControllerType> ();
				list.Add (ControllerType.Print);
				this.metas.Add (MetaControllerType.Print, list);
			}

			{
				var list = new List<ControllerType> ();
				list.Add (ControllerType.Login);
				this.metas.Add (MetaControllerType.Login, list);
			}

			{
				var list = new List<ControllerType> ();
				list.Add (ControllerType.Journal);
				list.Add (ControllerType.Journaux);
				this.metas.Add (MetaControllerType.Journal, list);
			}

			{
				var list = new List<ControllerType> ();
				list.Add (ControllerType.Balance);
				this.metas.Add (MetaControllerType.Balance, list);
			}

			{
				var list = new List<ControllerType> ();
				list.Add (ControllerType.Extrait);
				this.metas.Add (MetaControllerType.Extrait, list);
			}

			{
				var list = new List<ControllerType> ();
				list.Add (ControllerType.Bilan);
				this.metas.Add (MetaControllerType.Bilan, list);
			}

			{
				var list = new List<ControllerType> ();
				list.Add (ControllerType.PP);
				this.metas.Add (MetaControllerType.PP, list);
			}

			{
				var list = new List<ControllerType> ();
				list.Add (ControllerType.Exploitation);
				this.metas.Add (MetaControllerType.Exploitation, list);
			}

			{
				var list = new List<ControllerType> ();
				list.Add (ControllerType.Budgets);
				this.metas.Add (MetaControllerType.Budgets, list);
			}

			{
				var list = new List<ControllerType> ();
				list.Add (ControllerType.RésuméTVA);
				list.Add (ControllerType.DécompteTVA);
				list.Add (ControllerType.CodesTVA);
				list.Add (ControllerType.ListeTVA);
				this.metas.Add (MetaControllerType.TVA, list);
			}

			{
				var list = new List<ControllerType> ();
				list.Add (ControllerType.RésuméPériodique);
				this.metas.Add (MetaControllerType.RésuméPériodique, list);
			}

			{
				var list = new List<ControllerType> ();
				list.Add (ControllerType.Soldes);
				this.metas.Add (MetaControllerType.Soldes, list);
			}

			{
				var list = new List<ControllerType> ();
				list.Add (ControllerType.DifférencesChange);
				this.metas.Add (MetaControllerType.DifférencesChange, list);
			}

			{
				var list = new List<ControllerType> ();
				list.Add (ControllerType.PlanComptable);
				list.Add (ControllerType.Libellés);
				list.Add (ControllerType.Modèles);
				list.Add (ControllerType.Monnaies);
				list.Add (ControllerType.Périodes);
				list.Add (ControllerType.PiècesGenerator);
				list.Add (ControllerType.Utilisateurs);
				list.Add (ControllerType.Réglages);
				this.metas.Add (MetaControllerType.Réglages, list);
			}
		}

		private MetaControllerType GetMeta(ControllerType type)
		{
			foreach (var pair in this.metas)
			{
				if (pair.Value.Contains (type))
				{
					return pair.Key;
				}
			}

			return MetaControllerType.Unknown;
		}
#endif

		private ControllerType GetControllerType(Command cmd)
		{
			string key = string.Concat (cmd.Name + ".ViewSettings");
			var list = this.GetViewSettingsList (key);
			if (list != null && list.Selected != null)
			{
				return list.Selected.ControllerType;
			}

			if (cmd == Res.Commands.Présentation.Open)
			{
				return ControllerType.Open;
			}

			if (cmd == Res.Commands.Présentation.Save)
			{
				return ControllerType.Save;
			}

			if (cmd == Res.Commands.Présentation.Print)
			{
				return ControllerType.Print;
			}

			if (cmd == Res.Commands.Présentation.Login)
			{
				return ControllerType.Login;
			}

			if (cmd == Res.Commands.Présentation.Journal)
			{
				return ControllerType.Journal;
			}

			if (cmd == Res.Commands.Présentation.Balance)
			{
				return ControllerType.Balance;
			}

			if (cmd == Res.Commands.Présentation.Extrait)
			{
				return ControllerType.Extrait;
			}

			if (cmd == Res.Commands.Présentation.Bilan)
			{
				return ControllerType.Bilan;
			}

			if (cmd == Res.Commands.Présentation.PP)
			{
				return ControllerType.PP;
			}

			if (cmd == Res.Commands.Présentation.Exploitation)
			{
				return ControllerType.Exploitation;
			}

			if (cmd == Res.Commands.Présentation.Budgets)
			{
				return ControllerType.Budgets;
			}

			if (cmd == Res.Commands.Présentation.DifférencesChange)
			{
				return ControllerType.DifférencesChange;
			}

			if (cmd == Res.Commands.Présentation.RésuméPériodique)
			{
				return ControllerType.RésuméPériodique;
			}

			if (cmd == Res.Commands.Présentation.Soldes)
			{
				return ControllerType.Soldes;
			}

			if (cmd == Res.Commands.Présentation.TVA)
			{
				return ControllerType.RésuméTVA;
			}

			if (cmd == Res.Commands.Présentation.Réglages)
			{
				return ControllerType.Réglages;
			}

			return ControllerType.Unknown;
		}


#if false
		public Dictionary<MetaControllerType, List<ControllerType>> Metas
		{
			get
			{
				return this.metas;
			}
		}

		public AbstractMetaController MetaController
		{
			get
			{
				return this.metaController;
			}
		}
#endif

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
			this.ribbonController = new RibbonController (this.app);
			this.ribbonController.CreateUI (window.Root);

#if false
			//	Crée le TabController.
			this.tabFrame = new FrameBox
			{
				Parent          = window.Root,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
			};

			this.tabController = new TabController (this);
			this.tabController.CreateUI (this.tabFrame);
#endif

			//	Crée la zone éditable principale.
			this.mainFrame = new FrameBox
			{
				Parent    = window.Root,
				BackColor = RibbonController.GetBackgroundColor1 (),
				//?BackColor = UIBuilder.WindowBackColor,
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

				if (this.Controller != null)
				{
					this.Controller.UpdateUser ();
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

#if false
		public List<AbstractController> Controllers
		{
			//	Retourne la liste de contrôleurs de toutes les fenêtres ouvertes.
			get
			{
				return this.controllers;
			}
		}
#endif


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

			var controller = this.Controller;

			//	Utilise un réglage de présentation (viewSettings -> panneaux).
			if (controller.DataAccessor != null && controller.DataAccessor.SearchData != null && viewSettings.Search != null && viewSettings.EnableSearch)
			{
				viewSettings.Search.CopyTo (controller.DataAccessor.SearchData);
			}

			if (controller.DataAccessor != null && controller.DataAccessor.FilterData != null && viewSettings.Filter != null && viewSettings.EnableFilter)
			{
				viewSettings.Filter.CopyTo (controller.DataAccessor.FilterData);
			}

			if (controller.DataAccessor != null && controller.DataAccessor.Options != null && viewSettings.Options != null && viewSettings.EnableOptions)
			{
				viewSettings.Options.CopyTo (controller.DataAccessor.Options);
			}

			//	Effectue éventuellement l'action spéciale, qui consiste à montrer ou cacher des panneaux.
			if (viewSettings.ShowSearch != ShowPanelMode.Nop && viewSettings.ShowSearch != ShowPanelMode.DoesNotExist)
			{
				this.ShowSearchPanel = (viewSettings.ShowSearch != ShowPanelMode.Hide);
				controller.SearchSpecialist = (viewSettings.ShowSearch == ShowPanelMode.ShowSpecialist);
			}

			if (viewSettings.ShowFilter != ShowPanelMode.Nop && viewSettings.ShowFilter != ShowPanelMode.DoesNotExist)
			{
				this.ShowFilterPanel = (viewSettings.ShowFilter != ShowPanelMode.Hide);
				controller.FilterSpecialist = (viewSettings.ShowFilter == ShowPanelMode.ShowSpecialist);
			}

			if (viewSettings.ShowOptions != ShowPanelMode.Nop && viewSettings.ShowOptions != ShowPanelMode.DoesNotExist)
			{
				this.ShowOptionsPanel = (viewSettings.ShowOptions != ShowPanelMode.Hide);
				controller.OptionsSpecialist = (viewSettings.ShowOptions == ShowPanelMode.ShowSpecialist);
			}

			controller.UpdateAfterChanged ();
			
			return this.Controller;
		}

		public AbstractController ShowPrésentation(ControllerType type)
		{
			//	Utilise une autre présentation.
			this.NavigatorUpdate ();

			this.selectedDocument = type;
			this.UpdatePrésentationCommands ();
			this.CreateController ();

			this.NavigatorPut ();

			return this.Controller;
		}


		#region Navigator
		private void NavigatorFirst()
		{
			//	Appelé la première fois, pour mettre la première présentation dans l'historique.
			this.navigatorEngine.Clear ();
			this.navigatorEngine.Put (this.Controller, this.selectedDocument);
			this.UpdateNavigatorCommands ();
		}

		private void NavigatorUpdate()
		{
			//	Appelé avant un changement de présentation, pour mettre à jour la présentation dans l'historique.
			this.navigatorEngine.Update (this.Controller, this.selectedDocument);
		}

		private void NavigatorPut()
		{
			//	Appelé après un changement de présentation, pour ajouter la nouvelle présentation dans l'historique.
			this.navigatorEngine.Put (this.Controller, this.selectedDocument);
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

			//?string icon = string.Format (@"<img src=""{0}"" voff=""-10"" dx=""32"" dy=""32""/>   ", data.Command.Icon);

			var item = new MenuItem ()
			{
				IconUri       = UIBuilder.GetMarkStateIconUri (index == this.navigatorEngine.Index),
				//?FormattedText = icon + data.Description,
				FormattedText = data.Description,
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
			this.UpdatePrésentationCommands ();
			this.CreateController ();

			this.navigatorEngine.Restore (this.Controller);

			if (this.Controller != null)
			{
				this.Controller.UpdateAfterChanged ();
			}

			this.navigatorEngine.RestoreArrayController (this.Controller);

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
			this.Controller.UpdateUser ();
			this.Controller.ClearHilite ();
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

			//?this.ribbonController.PrésentationsLastButton.CommandObject = Res.Commands.Présentation.Réglages;

			this.CreateController ();
			this.UpdateTitle ();
			this.NavigatorFirst ();

			this.UpdatePrésentationCommands ();
			this.UpdateFileCommands ();
		}


#if false
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
#endif


		private void CreateController()
		{
			//?var type = this.GetMeta (this.selectedDocument);
			//?this.metaController = this.CreateMetaController (this.mainWindow, type);

			//?this.tabController.CreateTabs (type);

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

				case ControllerType.Extrait:
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

#if false
		private AbstractMetaController CreateMetaController(Window parentWindow, MetaControllerType type)
		{
			this.DisposeMetaController ();

			AbstractMetaController controller = null;

			switch (type)
			{
				case MetaControllerType.Open:
					controller = new OpenMetaController (this.app, this.businessContext, this);
					break;

				case MetaControllerType.Login:
					controller = new LoginMetaController (this.app, this.businessContext, this);
					break;

				case MetaControllerType.Journal:
					controller = new JournalMetaController (this.app, this.businessContext, this);
					break;

				case MetaControllerType.Réglages:
					controller = new RéglagesMetaController (this.app, this.businessContext, this);
					break;
			}

			if (controller != null)
			{
				controller.CreateController (this.mainFrame, this.selectedDocument);
			}

			return controller;
		}

		private void DisposeMetaController()
		{
			if (this.metaController != null)
			{
				this.metaController.DisposeController ();
				this.metaController = null;
			}
		}
#endif


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


#if false
		private ControllerType SelectedDocument
		{
			get
			{
				return this.selectedDocument;
			}
			set
			{
				this.selectedDocument = value;

				//	S'il s'agit d'une présentation accessible par le menu, on met à jour la commande.
				if (Converters.MenuPrésentationCommands.Contains (this.selectedDocument))
				{
					this.ribbonController.PrésentationsLastButton.CommandObject = this.selectedDocument;
				}
			}
		}
#endif

		private void UpdatePrésentationCommands()
		{
			foreach (var command in Converters.PrésentationCommands)
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

#if false
			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Présentation.Menu);
				cs.Enable = (this.compta != null && this.currentUser != null);
			}

			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Présentation.New);
				cs.Enable = (this.compta != null && this.currentUser != null);
			}
#endif
		}

		public bool HasPrésentationCommand(Command cmd)
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
			if (this.Controller != null)
			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Panel.Search);
				cs.ActiveState = this.showSearchPanel ? ActiveState.Yes : ActiveState.No;
				cs.Enable = (this.Controller == null) ? false : this.Controller.HasSearchPanel;
			}

			if (this.Controller != null)
			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Panel.Filter);
				cs.ActiveState = this.showFilterPanel ? ActiveState.Yes : ActiveState.No;
				cs.Enable = (this.Controller == null) ? false : this.Controller.HasFilterPanel;
			}

			if (this.Controller != null)
			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Panel.Temporal);
				cs.ActiveState = this.showTemporalPanel ? ActiveState.Yes : ActiveState.No;
				cs.Enable = (this.Controller == null) ? false : this.Controller.HasTemporalPanel;
			}

			if (this.Controller != null)
			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Panel.Options);
				cs.ActiveState = this.showOptionsPanel ? ActiveState.Yes : ActiveState.No;
				cs.Enable = (this.Controller == null) ? false : this.Controller.HasOptionsPanel;
			}

			if (this.Controller != null)
			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Panel.Info);
				cs.ActiveState = this.showInfoPanel ? ActiveState.Yes : ActiveState.No;
				cs.Enable = (this.Controller == null) ? false : this.Controller.HasInfoPanel;
			}

			if (this.Controller != null)
			{
				this.Controller.UpdatePanelsShowed (this.showSearchPanel, this.showFilterPanel, this.showTemporalPanel, this.showOptionsPanel, this.showInfoPanel);
			}
		}

		private void UpdatePériodeCommands()
		{
			if (this.Controller != null)
			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Compta.PériodePrécédente);
				cs.Enable = (this.Controller.AcceptPériodeChanged && this.compta.GetPériode (this.période, -1) != null);
			}

			if (this.Controller != null)
			{
				CommandState cs = this.app.CommandContext.GetCommandState (Res.Commands.Compta.PériodeSuivante);
				cs.Enable = (this.Controller.AcceptPériodeChanged && this.compta.GetPériode (this.période, 1) != null);
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
				this.Controller.UpdateAfterChanged ();

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
			dialog.OwnerWindow = this.Controller.MainWindowController.Window;
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
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Journal)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Balance)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Extrait)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Bilan)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.PP)]
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Exploitation)]
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

#if false
		[Command (Cresus.Compta.Res.CommandIds.Présentation.Menu)]
		private void ProcessMenuPrésentation(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ShowPrésentationsMenu ();
		}
#endif

#if false
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
		[Command (Cresus.Compta.Res.CommandIds.NouvellePrésentation.DifférencesChange)]
		[Command (Cresus.Compta.Res.CommandIds.NouvellePrésentation.RésuméPériodique)]
		[Command (Cresus.Compta.Res.CommandIds.NouvellePrésentation.Soldes)]
		[Command (Cresus.Compta.Res.CommandIds.NouvellePrésentation.RésuméTVA)]
		[Command (Cresus.Compta.Res.CommandIds.NouvellePrésentation.DécompteTVA)]
		private void ProcessShowNouvellePrésentation(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.OpenNewWindow (e.Command);
		}
#endif


		public void ClosePanelSearch()
		{
			this.showSearchPanel = false;
			this.UpdatePanelCommands ();
		}

		public void ClosePanelFilter()
		{
			this.showFilterPanel = false;
			this.UpdatePanelCommands ();
		}

		public void OpenPanelFilter()
		{
			this.showFilterPanel = true;
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
			this.showOptionsPanel = false;
			this.UpdatePanelCommands ();
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

		[Command (Res.CommandIds.Panel.Temporal)]
		private void CommandPanelTemporal()
		{
			this.showTemporalPanel = !this.showTemporalPanel;
			this.UpdatePanelCommands ();
		}

		[Command (Res.CommandIds.Panel.Options)]
		private void CommandPanelOptions()
		{
			this.showOptionsPanel = !this.showOptionsPanel;
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
			if (this.Controller != null && this.Controller.ArrayController != null)
			{
				this.Controller.ArrayController.MoveSelection (-1);
			}
		}

		[Command (Res.CommandIds.Select.Down)]
		private void CommandSelectDown()
		{
			if (this.Controller != null && this.Controller.ArrayController != null)
			{
				this.Controller.ArrayController.MoveSelection (1);
			}
		}

		[Command (Res.CommandIds.Select.Home)]
		private void CommandSelectHome()
		{
			if (this.Controller != null && this.Controller.ArrayController != null)
			{
				this.Controller.ArrayController.MoveSelection (0);
			}
		}


		[Command (Res.CommandIds.Edit.Create)]
		private void CommandEditCreate()
		{
			if (this.Controller != null)
			{
				this.Controller.CreateAction ();
			}
		}

		[Command (Res.CommandIds.Edit.Accept)]
		private void CommandEditAccept()
		{
			if (this.Controller != null)
			{
				this.Controller.AcceptAction ();
			}
		}

		[Command (Res.CommandIds.Edit.Cancel)]
		private void CommandEditCancel()
		{
			if (this.Controller != null)
			{
				this.Controller.CancelAction ();
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
			if (this.Controller != null && this.Controller.EditorController != null)
			{
				this.Controller.EditorController.MoveAction (-1);
			}
		}

		[Command (Res.CommandIds.Edit.Down)]
		private void CommandEditDown()
		{
			if (this.Controller != null && this.Controller.EditorController != null)
			{
				this.Controller.EditorController.MoveAction (1);
			}
		}

		[Command (Res.CommandIds.Edit.Duplicate)]
		private void CommandEditDuplicate()
		{
			if (this.Controller != null && this.Controller.EditorController != null)
			{
				this.Controller.EditorController.DuplicateAction ();
			}
		}

		[Command (Res.CommandIds.Edit.Delete)]
		private void CommandEditDelete()
		{
			if (this.Controller != null && this.Controller.EditorController != null)
			{
				this.Controller.EditorController.DeleteAction ();
			}
		}

		[Command (Res.CommandIds.Multi.LastLine)]
		private void CommandMultiLastLine()
		{
			if (this.Controller != null)
			{
				this.Controller.EditorController.MultiLastLineAction ();
			}
		}

		[Command (Res.CommandIds.Multi.InsertBefore)]
		private void CommandMultiInsertBefore()
		{
			if (this.Controller != null && this.Controller.EditorController != null)
			{
				this.Controller.EditorController.MultiInsertLineAction (true);
			}
		}

		[Command (Res.CommandIds.Multi.InsertAfter)]
		private void CommandMultiInsertAfter()
		{
			if (this.Controller != null && this.Controller.EditorController != null)
			{
				this.Controller.EditorController.MultiInsertLineAction (false);
			}
		}

		[Command (Res.CommandIds.Multi.InsertTVA)]
		private void CommandMultiInsertTVA()
		{
			if (this.Controller != null && this.Controller.EditorController != null)
			{
				this.Controller.EditorController.MultiInsertTVALineAction ();
			}
		}

		[Command (Res.CommandIds.Multi.Delete)]
		private void CommandMultiDelete()
		{
			if (this.Controller != null && this.Controller.EditorController != null)
			{
				this.Controller.EditorController.MultiDeleteLineAction ();
			}
		}

		[Command (Res.CommandIds.Multi.Up)]
		private void CommandMultiUp()
		{
			if (this.Controller != null && this.Controller.EditorController != null)
			{
				this.Controller.EditorController.MultiMoveLineAction (-1);
			}
		}

		[Command (Res.CommandIds.Multi.Down)]
		private void CommandMultiDown()
		{
			if (this.Controller != null && this.Controller.EditorController != null)
			{
				this.Controller.EditorController.MultiMoveLineAction (1);
			}
		}

		[Command (Res.CommandIds.Multi.Swap)]
		private void CommandMultiSwap()
		{
			if (this.Controller != null && this.Controller.EditorController != null)
			{
				this.Controller.EditorController.MultiLineSwapAction ();
			}
		}

		[Command (Res.CommandIds.Multi.Split)]
		private void CommandMultiSplit()
		{
			if (this.Controller != null && this.Controller.EditorController != null)
			{
				this.Controller.EditorController.MultiLineSplitAction ();
			}
		}

		[Command (Res.CommandIds.Multi.Join)]
		private void CommandMultiJoin()
		{
			if (this.Controller != null && this.Controller.EditorController != null)
			{
				this.Controller.EditorController.MultiLineJoinAction ();
			}
		}

		[Command (Res.CommandIds.Multi.Auto)]
		private void CommandMultiAuto()
		{
			if (this.Controller != null && this.Controller.EditorController != null)
			{
				this.Controller.EditorController.MultiLineAutoAction ();
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
			this.Controller.EditorController.InsertModèle (n);
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


		private AbstractController Controller
		{
			get
			{
#if false
				if (this.metaController != null)
				{
					return this.metaController.Controller;
				}

				return null;
#else
				return this.controller;
#endif
			}
		}


#if false
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
#endif


#if false
		#region New window menu
		public void ShowNewWindowMenu()
		{
			this.ShowNewWindowMenu (this.ribbonController.NewWindowMenuButton);
		}

		private void ShowNewWindowMenu(Widget parentButton)
		{
			var menu = new VMenu ();

			this.AddNewWindowToMenu (menu, Res.Commands.Présentation.Journal,           Res.CommandIds.NouvellePrésentation.Journal);
			this.AddNewWindowToMenu (menu, Res.Commands.Présentation.Extrait,           Res.CommandIds.NouvellePrésentation.Extrait);
			this.AddNewWindowToMenu (menu, Res.Commands.Présentation.Balance,           Res.CommandIds.NouvellePrésentation.Balance);
			this.AddNewWindowToMenu (menu, Res.Commands.Présentation.Bilan,             Res.CommandIds.NouvellePrésentation.Bilan);
			this.AddNewWindowToMenu (menu, Res.Commands.Présentation.PP,                Res.CommandIds.NouvellePrésentation.PP);
			this.AddNewWindowToMenu (menu, Res.Commands.Présentation.Exploitation,      Res.CommandIds.NouvellePrésentation.Exploitation);
			this.AddNewWindowToMenu (menu, Res.Commands.Présentation.Budgets,           Res.CommandIds.NouvellePrésentation.Budgets);
			this.AddNewWindowToMenu (menu, Res.Commands.Présentation.DifférencesChange, Res.CommandIds.NouvellePrésentation.DifférencesChange);
			this.AddNewWindowToMenu (menu, Res.Commands.Présentation.RésuméPériodique,  Res.CommandIds.NouvellePrésentation.RésuméPériodique);
			this.AddNewWindowToMenu (menu, Res.Commands.Présentation.Soldes,            Res.CommandIds.NouvellePrésentation.Soldes);
			this.AddNewWindowToMenu (menu, Res.Commands.Présentation.RésuméTVA,         Res.CommandIds.NouvellePrésentation.RésuméTVA);
			this.AddNewWindowToMenu (menu, Res.Commands.Présentation.DécompteTVA,       Res.CommandIds.NouvellePrésentation.DécompteTVA);
			this.AddNewWindowToMenu (menu, Res.Commands.Présentation.PlanComptable,     Res.CommandIds.NouvellePrésentation.PlanComptable);

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
#endif

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

			var data = new ViewSettingsList ();

			this.settingsData.Add (key, data);

			return data;
		}
		#endregion


		private readonly ComptaApplication					app;
		//?private readonly List<AbstractController>			controllers;
		private readonly Dictionary<string, ISettingsData>	settingsData;
		private readonly SettingsList						settingsList;
		private readonly SettingsList						defaultSettingsList;
		private readonly NavigatorEngine					navigatorEngine;
		private readonly PiècesGenerator					piècesGenerator;
		private readonly TemporalData						temporalData;
		//?private readonly Dictionary<MetaControllerType, List<ControllerType>> metas;

		private Window										mainWindow;
		private BusinessContext								businessContext;
		private ComptaEntity								compta;
		private ComptaPériodeEntity							période;
		private ControllerType								selectedDocument;
		private AbstractController							controller;
		//?private AbstractMetaController						metaController;
		private RibbonController							ribbonController;
		private FrameBox									tabFrame;
		private TabController								tabController;
		private FrameBox									mainFrame;
		private string										titleComplement;
		private bool										dirty;
		private bool										showSearchPanel;
		private bool										showFilterPanel;
		private bool										showTemporalPanel;
		private bool										showOptionsPanel;
		private bool										showInfoPanel;
		private ComptaUtilisateurEntity						currentUser;
	}
}
