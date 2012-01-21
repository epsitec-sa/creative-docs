﻿//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	public class WindowController
	{
		public WindowController(Application app)
		{
			this.app = app;

			this.controllers = new List<AbstractController> ();

			this.comptabilité = new ComptabilitéEntity ();  // crée une compta vide !!!

			this.app.CommandDispatcher.RegisterController (this);
		}


		public void CreateUI(Window window)
		{
			this.window = window;

			var button = new Button (Epsitec.Common.Dialogs.Res.Commands.Dialog.Generic.Close)
			{
				Parent     = window.Root,
				Anchor     = AnchorStyles.TopLeft,
				Visibility = false,
			};

			//	Crée les frames principales.
			this.ribbonController = new RibbonController (this.app);
			this.ribbonController.CreateUI (window.Root);

			//	Crée la zone éditable.
			this.mainFrame = new FrameBox
			{
				Parent  = window.Root,
				Dock    = DockStyle.Fill,
				Padding = new Margins (3),
			};

			this.SelectDefaultPrésentation ();
			this.CreateController ();
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


		private void CreateController()
		{
			this.DisposeController ();

			switch (this.selectedCommandDocument.Name)
			{
				case "Présentation.Journal":
					this.controller = new JournalController (this.app, this.businessContext, this.comptabilité, this.controllers);
					break;

				case "Présentation.PlanComptable":
					this.controller = new PlanComptableController (this.app, this.businessContext, this.comptabilité, this.controllers);
					break;

				case "Présentation.Balance":
					this.controller = new BalanceController (this.app, this.businessContext, this.comptabilité, this.controllers);
					break;

				case "Présentation.Extrait":
					this.controller = new ExtraitDeCompteController (this.app, this.businessContext, this.comptabilité, this.controllers);
					break;

				case "Présentation.Bilan":
					this.controller = new BilanController (this.app, this.businessContext, this.comptabilité, this.controllers);
					break;

				case "Présentation.PP":
					this.controller = new PPController (this.app, this.businessContext, this.comptabilité, this.controllers);
					break;

				case "Présentation.Exploitation":
					this.controller = new ExploitationController (this.app, this.businessContext, this.comptabilité, this.controllers);
					break;

				case "Présentation.Budgets":
					this.controller = new BudgetsController (this.app, this.businessContext, this.comptabilité, this.controllers);
					break;
			}

			if (this.controller != null)
			{
				this.controller.CreateUI (this.mainFrame);
				this.controllers.Add (this.controller);
			}

			this.UpdateTitle ();
		}

		private void DisposeController()
		{
			if (this.controller != null)
			{
				if (this.controllers.Contains (this.controller))
				{
					this.controllers.Remove (this.controller);
				}

				this.controller.Dispose ();
				this.controller = null;
			}

			this.mainFrame.Children.Clear ();
		}

		private void UpdateTitle()
		{
			string title = string.Concat ("Crésus MCH-2 / ", this.comptabilité.GetCompactSummary (), " / ", this.selectedCommandDocument.Description);
			this.window.Text = title;
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
			//?this.OpenNewWindow ();
		}

		[Command (Res.CommandIds.Edit.Accept)]
		private void CommandEditAccept()
		{
			if (this.controller.FooterController != null)
			{
				this.controller.FooterController.AcceptAction ();
			}
		}

		[Command (Res.CommandIds.Edit.Cancel)]
		private void CommandEditCancel()
		{
			if (this.controller.FooterController != null)
			{
				this.controller.FooterController.CancelAction ();
			}
		}

		[Command (Res.CommandIds.Edit.Duplicate)]
		private void CommandEditDuplicate()
		{
			if (this.controller.FooterController != null)
			{
				this.controller.FooterController.DuplicateAction ();
			}
		}

		[Command (Res.CommandIds.Edit.Delete)]
		private void CommandEditDelete()
		{
			if (this.controller.FooterController != null)
			{
				this.controller.FooterController.DeleteAction ();
			}
		}

		[Command (Res.CommandIds.Multi.Insert)]
		private void CommandMultiInsert()
		{
			if (this.controller.FooterController != null)
			{
				this.controller.FooterController.InsertLineAction ();
			}
		}

		[Command (Res.CommandIds.Multi.Delete)]
		private void CommandMultiDelete()
		{
			if (this.controller.FooterController != null)
			{
				this.controller.FooterController.DeleteLineAction ();
			}
		}

		[Command (Res.CommandIds.Multi.Up)]
		private void CommandMultiUp()
		{
			if (this.controller.FooterController != null)
			{
				this.controller.FooterController.LineUpAction ();
			}
		}

		[Command (Res.CommandIds.Multi.Down)]
		private void CommandMultiDown()
		{
			if (this.controller.FooterController != null)
			{
				this.controller.FooterController.LineDownAction ();
			}
		}

		[Command (Res.CommandIds.Multi.Swap)]
		private void CommandMultiSwap()
		{
			if (this.controller.FooterController != null)
			{
				this.controller.FooterController.LineSwapAction ();
			}
		}

		[Command (Res.CommandIds.Multi.Auto)]
		private void CommandMultiAuto()
		{
			if (this.controller.FooterController != null)
			{
				this.controller.FooterController.LineAutoAction ();
			}
		}

		[Command (Epsitec.Common.Dialogs.Res.CommandIds.Dialog.Generic.Close)]
		private void ProcessClose()
		{
			this.DisposeController ();
		}


	
		private readonly Application					app;
		private readonly List<AbstractController>		controllers;

		private Window									window;
		private BusinessContext							businessContext;
		private ComptabilitéEntity						comptabilité;
		private Command									selectedCommandDocument;
		private AbstractController						controller;
		private RibbonController						ribbonController;
		private FrameBox								mainFrame;
	}
}
