//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta
{
	public class MainWindow : CoreDialog
	{
		public MainWindow(CoreApp app, List<AbstractController> controllers, BusinessContext businessContext, ComptabilitéEntity comptabilité, TypeDeDocumentComptable type)
			: base (app)
		{
			this.app             = app;
			this.controllers     = controllers;
			this.businessContext = businessContext;
			this.comptabilité    = comptabilité;
			this.selectedType    = type;
		}

		public FrameBox MainFrame
		{
			get
			{
				return this.mainFrame;
			}
		}

		protected override void SetupWindow(Window window)
		{
			this.window = window;

			window.Text = "Crésus Comptabilité";
			window.ClientSize = new Size (800, 500);
			window.Root.WindowStyles = WindowStyles.DefaultDocumentWindow;  // pour avoir les boutons Minimize/Maximize/Close !
		}

		protected override void SetupWidgets(Window window)
		{
			//	Ce bouton est nécessaire pour que le bouton de fermeture [x] en haut à droite
			//	de la fenêtre existe !
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
			{
				Command command = Command.Get (Cresus.Compta.Res.CommandIds.Présentation.Journal);
				this.ribbonController.PrésentationCommandsUpdate (command);
			}

			{
				Command command = Command.Get (Cresus.Compta.Res.CommandIds.Edit.Cancel);
				CommandState cs = this.app.CommandContext.GetCommandState (command);
				cs.Enable = false;
			}
		}


		private void OpenNewWindow()
		{
			var window = new MainWindow (this.app, this.controllers, this.businessContext, this.comptabilité, this.selectedType);

			window.IsModal = false;
			window.OpenDialog ();
		}

		private void UpdateTitle()
		{
			string title = string.Concat ("Crésus Comptabilité / ", this.comptabilité.GetCompactSummary (), " / ", this.GetToolbarDescription (this.selectedType));
			this.window.Text = title;
		}


		private void CreateController()
		{
			this.DisposeController ();

			switch (this.selectedType)
			{
				case TypeDeDocumentComptable.Journal:
					this.controller = new JournalController (this.app, this.businessContext, this.comptabilité, this.controllers);
					break;

				case TypeDeDocumentComptable.PlanComptable:
					this.controller = new PlanComptableController (this.app, this.businessContext, this.comptabilité, this.controllers);
					break;

				case TypeDeDocumentComptable.Balance:
					this.controller = new BalanceController (this.app, this.businessContext, this.comptabilité, this.controllers);
					break;

				case TypeDeDocumentComptable.Extrait:
					this.controller = new ExtraitDeCompteController (this.app, this.businessContext, this.comptabilité, this.controllers);
					break;

				case TypeDeDocumentComptable.Bilan:
					this.controller = new BilanController (this.app, this.businessContext, this.comptabilité, this.controllers);
					break;

				case TypeDeDocumentComptable.PP:
					this.controller = new PPController (this.app, this.businessContext, this.comptabilité, this.controllers);
					break;

				case TypeDeDocumentComptable.Exploitation:
					this.controller = new ExploitationController (this.app, this.businessContext, this.comptabilité, this.controllers);
					break;

				case TypeDeDocumentComptable.Budgets:
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


		private FormattedText GetToolbarDescription(TypeDeDocumentComptable type)
		{
			switch (type)
			{
				case TypeDeDocumentComptable.Journal:
					return "Journal des écritures";

				case TypeDeDocumentComptable.PlanComptable:
					return "Plan comptable";

				case TypeDeDocumentComptable.Balance:
					return "Balance";

				case TypeDeDocumentComptable.Extrait:
					return "Extrait de compte";

				case TypeDeDocumentComptable.Bilan:
					return "Bilan";

				case TypeDeDocumentComptable.PP:
					return "Pertes et Profits";

				case TypeDeDocumentComptable.Exploitation:
					return "Compte d'exploitation";

				case TypeDeDocumentComptable.Budgets:
					return "Budgets";

				case TypeDeDocumentComptable.Change:
					return "Différences de change";

				case TypeDeDocumentComptable.RésuméPériodique:
					return "Résumé périodique";

				case TypeDeDocumentComptable.RésuméTVA:
					return "Résumé TVA";

				case TypeDeDocumentComptable.DécompteTVA:
					return "Décompte TVA";
			}

			return FormattedText.Empty;
		}

		private TypeDeDocumentComptable GetToolbarCommand(string commandName)
		{
			switch (commandName)
			{
				case "Compta.Présentation.Journal":
					return TypeDeDocumentComptable.Journal;

				case "Compta.Présentation.PlanComptable":
					return TypeDeDocumentComptable.PlanComptable;

				case "Compta.Présentation.Balance":
					return TypeDeDocumentComptable.Balance;

				case "Compta.Présentation.Extrait":
					return TypeDeDocumentComptable.Extrait;

				case "Compta.Présentation.Bilan":
					return TypeDeDocumentComptable.Bilan;

				case "Compta.Présentation.PP":
					return TypeDeDocumentComptable.PP;

				case "Compta.Présentation.Exploitation":
					return TypeDeDocumentComptable.Exploitation;

				case "Compta.Présentation.Budgets":
					return TypeDeDocumentComptable.Budgets;

				case "Compta.Présentation.Change":
					return TypeDeDocumentComptable.Change;

				case "Compta.Présentation.RésuméPériodique":
					return TypeDeDocumentComptable.RésuméPériodique;

				case "Compta.Présentation.RésuméTVA":
					return TypeDeDocumentComptable.RésuméTVA;

				case "Compta.Présentation.DécompteTVA":
					return TypeDeDocumentComptable.DécompteTVA;
			}

			return TypeDeDocumentComptable.Journal;
		}

		private IEnumerable<TypeDeDocumentComptable> WindowTypes
		{
			get
			{
				yield return TypeDeDocumentComptable.Journal;
				yield return TypeDeDocumentComptable.PlanComptable;
				yield return TypeDeDocumentComptable.Balance;
				yield return TypeDeDocumentComptable.Extrait;
				yield return TypeDeDocumentComptable.Bilan;
				yield return TypeDeDocumentComptable.PP;
				yield return TypeDeDocumentComptable.Exploitation;
				yield return TypeDeDocumentComptable.Budgets;
				yield return TypeDeDocumentComptable.Change;
				yield return TypeDeDocumentComptable.RésuméPériodique;
				yield return TypeDeDocumentComptable.RésuméTVA;
				yield return TypeDeDocumentComptable.DécompteTVA;
			}
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

			this.selectedType = this.GetToolbarCommand (e.Command.Name);
			this.CreateController ();
		}

		[Command (Cresus.Compta.Res.CommandIds.Présentation.New)]
		private void ProcessNewPrésentation(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.OpenNewWindow ();
		}

		[Command (Epsitec.Common.Dialogs.Res.CommandIds.Dialog.Generic.Close)]
		private void ProcessClose()
		{
			this.DisposeController ();

			this.Result = DialogResult.Cancel;
			this.CloseDialog ();
		}


		private readonly static double IconSize = 40;

		private readonly CoreApp						app;
		private readonly List<AbstractController>		controllers;
		private readonly BusinessContext				businessContext;
		private readonly ComptabilitéEntity				comptabilité;

		private Window									window;
		private RibbonController						ribbonController;
		private FrameBox								mainFrame;
		private TypeDeDocumentComptable					selectedType;

		private AbstractController						controller;
	}
}
