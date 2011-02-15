//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business.UserManagement;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs
{
	/// <summary>
	/// Dialogue pour l'ensemble du debug.
	/// </summary>
	public class DebugDialog : AbstractDialog
	{
		public DebugDialog(CoreApplication application)
		{
			this.application = application;

			this.IsModal = false;

			this.settingsTabPages = new List<SettingsTabPages.AbstractSettingsTabPage> ();
		}


		protected override Window CreateWindow()
		{
			this.window = new Window ();

			this.SetupWindow ();
			this.SetupWidgets ();
			this.UpdateWidgets ();

			this.window.AdjustWindowSize ();

			return this.window;
		}

		private void SetupWindow()
		{
			this.OwnerWindow = this.application.Window;
			this.window.Icon = this.application.Window.Icon;
			this.window.Text = "Dépannage";
			this.window.ClientSize = new Size (850, 600);

			this.window.WindowCloseClicked += delegate
			{
				this.RejectChangingsAndClose ();
			};
		}

		private void SetupWidgets()
		{
			bool devel = CoreProgram.Application.UserManager.IsAuthenticatedUserAtPowerLevel (UserPowerLevel.Developer);

			var frame = new FrameBox
			{
				Parent = this.window.Root,
				Dock = DockStyle.Fill,
				Margins = new Margins (10, 10, 10, 0),
			};

			var footer = new FrameBox
			{
				Parent = this.window.Root,
				PreferredHeight = 20,
				Dock = DockStyle.Bottom,
				Margins = new Margins (10, 10, 10, 10),
			};

			//	Crée les onglets.
			this.tabBook = new TabBook
			{
				Parent = frame,
				Dock = DockStyle.Fill,
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

			this.ActiveLastPage ();

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
				Parent = footer,
				Text = "Fermer",
				ButtonStyle = Common.Widgets.ButtonStyle.DefaultAccept,
				Dock = DockStyle.Right,
				TabIndex = 100,
			};

			//	Rempli les onglets.
			if (devel)
			{
				var loggingSettings = new SettingsTabPages.LoggingTabPage (this.application);
				loggingSettings.CreateUI (loggingPage);
				this.settingsTabPages.Add (loggingSettings);
			}

			if (devel)
			{
				var maintenanceSettings = new SettingsTabPages.MaintenanceTabPage (this.application);
				maintenanceSettings.CreateUI (maintenancePage);
				this.settingsTabPages.Add (maintenanceSettings);
			}

			foreach (var tab in this.settingsTabPages)
			{
				tab.AcceptStateChanging += new EventHandler (this.HandlerSettingsAcceptStateChanging);
			}

			//	Connection des événements.
			this.closeButton.Clicked += delegate
			{
				this.AcceptChangingsAndClose ();
			};
		}

		private void AcceptChangingsAndClose()
		{
			this.UpdateLastActivedPageName ();

			foreach (var tab in this.settingsTabPages)
			{
				tab.AcceptChangings ();
			}

			this.Result = DialogResult.Accept;
			this.OnDialogClosed ();
			this.CloseDialog ();
		}

		private void RejectChangingsAndClose()
		{
			this.UpdateLastActivedPageName ();

			foreach (var tab in this.settingsTabPages)
			{
				tab.RejectChangings ();
			}

			this.Result = DialogResult.Cancel;
			this.OnDialogClosed ();
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

		private void UpdateWidgets()
		{
		}


		private void ActiveLastPage()
		{
			string name = DebugDialog.lastActivedPageName;

			if (string.IsNullOrEmpty (name))
			{
				name = "logging";  // page par défaut
			}

			var page = this.tabBook.Items.Where (x => x.Name == name).FirstOrDefault ();
			this.tabBook.ActivePage = page;
		}

		private void UpdateLastActivedPageName()
		{
			DebugDialog.lastActivedPageName = this.tabBook.ActivePage.Name;
		}


		private static string									lastActivedPageName;

		private readonly CoreApplication						application;
		private List<SettingsTabPages.AbstractSettingsTabPage>	settingsTabPages;

		private Window											window;
		private TabBook											tabBook;
		private StaticText										errorInfo;
		private Button											closeButton;
	}
}
