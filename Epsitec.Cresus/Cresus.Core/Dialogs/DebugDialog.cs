//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Dialogs.SettingsTabPages;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs
{
	/// <summary>
	/// Dialogue pour l'ensemble du debug.
	/// </summary>
	public class DebugDialog : CoreDialog, ISettingsDialog
	{
		public DebugDialog(CoreApp application)
			: base (application)
		{
			this.IsModal = false;
			this.settingsTabPages = new List<SettingsTabPages.AbstractSettingsTabPage> ();
		}

		#region ISettingsTabBook Members

		public CoreData Data
		{
			get
			{
				return this.application.FindComponent<CoreData> ();
			}
		}

		#endregion

		protected override void SetupWindow(Window window)
		{
			window.Text = "Dépannage";
			window.ClientSize = new Size (850, 600);
		}

		protected override void SetupWidgets(Window window)
		{
			var userManager = this.Data.GetComponent<UserManager> ();
			bool devel = userManager.IsAuthenticatedUserAtPowerLevel (UserPowerLevel.Developer);

			var frame = new FrameBox
			{
				Parent = window.Root,
				Dock = DockStyle.Fill,
				Margins = new Margins (10, 10, 10, 0),
			};

			var footer = new FrameBox
			{
				Parent = window.Root,
				PreferredHeight = 20,
				Dock = DockStyle.Bottom,
				Margins = new Margins (10, 10, 10, 10),
			};

			//	Crée les onglets.
			this.tabBook = new TabBook
			{
				Parent = frame,
				Dock = DockStyle.Fill,
				Name = "DialogTabBook",
			};

			//	Crée l'onglet 'trace'.
			TabPage loggingPage = null;

			if (devel)
			{
				loggingPage = new TabPage
				{
					TabTitle = "Trace",
					Name = "logging",
				};

				this.tabBook.Items.Add (loggingPage);
			}

			//	Crée l'onglet 'maintenance'.
			TabPage maintenancePage = null;

			if (devel)
			{
				maintenancePage = new TabPage
				{
					TabTitle = "Maintenance",
					Name = "maintenance",
				};

				this.tabBook.Items.Add (maintenancePage);
			}

			this.tabBook.ActivePageIndex = 0;

			//	Crée le pied de page.
			this.errorInfo = new StaticText
			{
				Parent = footer,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 10, 0, 0),
			};

			this.closeButton = new Button ()
			{
				CommandObject = Epsitec.Common.Dialogs.Res.Commands.Dialog.Generic.Close,
				Parent = footer,
				ButtonStyle = Common.Widgets.ButtonStyle.DefaultAccept,
				Dock = DockStyle.Right,
				TabIndex = 100,
			};

			//	Remplit les onglets.
			if (devel)
			{
				var loggingSettings = new SettingsTabPages.LoggingTabPage (this);
				loggingSettings.CreateUI (loggingPage);
				this.settingsTabPages.Add (loggingSettings);
			}

			if (devel)
			{
				var maintenanceSettings = new SettingsTabPages.MaintenanceTabPage (this);
				maintenanceSettings.CreateUI (maintenancePage);
				this.settingsTabPages.Add (maintenanceSettings);
			}

			foreach (var tab in this.settingsTabPages)
			{
				tab.AcceptStateChanging += new EventHandler (this.HandlerSettingsAcceptStateChanging);
			}

			this.RegisterWithPersistenceManager (this.tabBook);
		}

		[Command (Epsitec.Common.Dialogs.Res.CommandIds.Dialog.Generic.Close)]
		private void ExecuteCloseCommand()
		{
			if (this.closeButton.Enable)
			{
				this.CloseAndAcceptChanges ();
			}
		}

		private void CloseAndAcceptChanges()
		{
			foreach (var tab in this.settingsTabPages)
			{
				tab.AcceptChanges ();
			}

			this.Result = DialogResult.Accept;
			this.CloseDialog ();
		}

		private void CloseAndRejectChanges()
		{
			foreach (var tab in this.settingsTabPages)
			{
				tab.RejectChanges ();
			}

			this.Result = DialogResult.Cancel;
			this.CloseDialog ();
		}

		private void HandlerSettingsAcceptStateChanging(object sender)
		{
			//	Si l'un des onglets contient une erreur, on l'affiche et le bouton 'accept' est grisé.
			string errorMessage = null;

			foreach (var tab in this.settingsTabPages)
			{
				if (!string.IsNullOrEmpty (tab.ErrorMessage))
				{
					errorMessage = tab.ErrorMessage;
					break;
				}
			}

			if (string.IsNullOrEmpty (errorMessage))  // ok ?
			{
				this.errorInfo.Text = null;
				this.errorInfo.BackColor = Color.Empty;

				this.closeButton.Enable = true;
			}
			else  // erreur ?
			{
				this.errorInfo.Text = errorMessage;
				this.errorInfo.BackColor = Color.FromName ("Gold");

				this.closeButton.Enable = false;
			}
		}

		protected override void UpdateWidgets()
		{
		}


		private readonly List<AbstractSettingsTabPage>			settingsTabPages;

		private TabBook											tabBook;
		private StaticText										errorInfo;
		private Button											closeButton;
	}
}
