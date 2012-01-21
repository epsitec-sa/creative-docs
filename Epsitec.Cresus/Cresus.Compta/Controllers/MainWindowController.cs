//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Dialogs;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.IO;
using Epsitec.Cresus.Compta.Entities;

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

			this.comptabilité = new ComptabilitéEntity ();  // crée une compta vide !!!
			new NewComptabilité ().NewEmpty (this.comptabilité);

			this.dirty = true;  // pour forcer la màj
			this.Dirty = false;

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
		}


		public List<AbstractController> Controllers
		{
			//	Retourne la liste de contrôleurs de toutes les fenêtres ouvertes.
			get
			{
				return this.controllers;
			}
		}

		public bool Dirty
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
			if (this.comptabilité.PlanComptable.Any ())
			{
				this.selectedCommandDocument = Res.Commands.Présentation.Journal;
			}
			else  // plan comptable vide ?
			{
				this.selectedCommandDocument = Res.Commands.Présentation.PlanComptable;
			}

			this.ribbonController.PrésentationCommandsUpdate (this.selectedCommandDocument);
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
		}

		private AbstractController CreateController(Window parentWindow, Command command)
		{
			AbstractController controller = null;

			if (command.Name.EndsWith ("Présentation.Journal"))
			{
				controller = new JournalController (this.app, this.businessContext, this.comptabilité, this);
			}

			if (command.Name.EndsWith ("Présentation.PlanComptable"))
			{
				controller = new PlanComptableController (this.app, this.businessContext, this.comptabilité, this);
			}

			if (command.Name.EndsWith ("Présentation.Balance"))
			{
				controller = new BalanceController (this.app, this.businessContext, this.comptabilité, this);
			}

			if (command.Name.EndsWith ("Présentation.Extrait"))
			{
				controller = new ExtraitDeCompteController (this.app, this.businessContext, this.comptabilité, this);
			}

			if (command.Name.EndsWith ("Présentation.Bilan"))
			{
				controller = new BilanController (this.app, this.businessContext, this.comptabilité, this);
			}

			if (command.Name.EndsWith ("Présentation.PP"))
			{
				controller = new PPController (this.app, this.businessContext, this.comptabilité, this);
			}

			if (command.Name.EndsWith ("Présentation.Exploitation"))
			{
				controller = new ExploitationController (this.app, this.businessContext, this.comptabilité, this);
			}

			if (command.Name.EndsWith ("Présentation.Budgets"))
			{
				controller = new BudgetsController (this.app, this.businessContext, this.comptabilité, this);
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

		public string GetTitle(Command command)
		{
			return string.Concat ("Crésus MCH-2 / ", this.comptabilité.GetCompactSummary (), " / ", command.Description);
		}

		private void UpdateControllers()
		{
			//	Met à jour tous les contrôleurs.
			foreach (var controller in this.controllers)
			{
				controller.Update ();
			}
		}


		#region Dialogs
		private string FileOpenDialog(string filename)
		{
			var dialog = new FileOpenDialog ();

			//dialog.InitialDirectory = this.globalSettings.InitialDirectory;
			dialog.FileName = filename;
			dialog.Title = "Ouverture d'une comptabilité";
			dialog.Filters.Add ("cre", "Comptbilité", "*.cre");
			dialog.Filters.Add ("crp", "Plan comptable", "*.crp");
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
			dialog.Filters.Add ("cre", "Comptbilité", "*.cre");
			dialog.Filters.Add ("crp", "Plan comptable", "*.crp");
			dialog.PromptForOverwriting = true;
			dialog.OwnerWindow = this.mainWindow;
			dialog.OpenDialog ();
			if (dialog.Result != Common.Dialogs.DialogResult.Accept)
			{
				return null;
			}

			return dialog.FileName;
		}

		private void ErrorDialog(string message)
		{
			var dialog = MessageDialog.CreateOk ("Erreur", Common.Dialogs.DialogIcon.Warning, message);

			dialog.OwnerWindow = this.mainWindow;
			dialog.OpenDialog ();
		}
		#endregion


		#region Command handlers
		[Command (Res.CommandIds.File.New)]
		private void CommandFileNew()
		{
			new NewComptabilité ().NewEmpty (this.comptabilité);

			this.controller.ClearHilite();
			this.UpdateControllers ();
			this.Dirty = false;
		}

		[Command (Res.CommandIds.File.Open)]
		private void CommandFileOpen()
		{
			var filename = this.FileOpenDialog (null);

			if (!string.IsNullOrEmpty (filename))
			{
				string err = new CrésusComptabilité ().ImportPlanComptable (this.comptabilité, filename);

				this.controller.ClearHilite ();
				this.UpdateControllers ();

				if (!string.IsNullOrEmpty (err))
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
		private void ProcessShowPrésentation(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ribbonController.PrésentationCommandsUpdate (e.Command);

			this.selectedCommandDocument = e.Command;
			this.CreateController ();
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
				this.controller.FooterController.InsertLineAction ();
			}
		}

		[Command (Res.CommandIds.Multi.Delete)]
		private void CommandMultiDelete()
		{
			if (this.controller != null && this.controller.FooterController != null)
			{
				this.controller.FooterController.DeleteLineAction ();
			}
		}

		[Command (Res.CommandIds.Multi.Up)]
		private void CommandMultiUp()
		{
			if (this.controller != null && this.controller.FooterController != null)
			{
				this.controller.FooterController.LineUpAction ();
			}
		}

		[Command (Res.CommandIds.Multi.Down)]
		private void CommandMultiDown()
		{
			if (this.controller != null && this.controller.FooterController != null)
			{
				this.controller.FooterController.LineDownAction ();
			}
		}

		[Command (Res.CommandIds.Multi.Swap)]
		private void CommandMultiSwap()
		{
			if (this.controller != null && this.controller.FooterController != null)
			{
				this.controller.FooterController.LineSwapAction ();
			}
		}

		[Command (Res.CommandIds.Multi.Auto)]
		private void CommandMultiAuto()
		{
			if (this.controller != null && this.controller.FooterController != null)
			{
				this.controller.FooterController.LineAutoAction ();
			}
		}

		[Command (Res.CommandIds.Global.Settings)]
		private void CommandGlobalSettings()
		{
		}
		#endregion


		private readonly Application					app;
		private readonly List<AbstractController>		controllers;

		private Window									mainWindow;
		private BusinessContext							businessContext;
		private ComptabilitéEntity						comptabilité;
		private Command									selectedCommandDocument;
		private AbstractController						controller;
		private RibbonController						ribbonController;
		private FrameBox								mainFrame;
		private bool									dirty;
	}
}
