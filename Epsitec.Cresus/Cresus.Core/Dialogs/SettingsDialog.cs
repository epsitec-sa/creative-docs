//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Dialogs.SettingsTabPages;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs
{
	/// <summary>
	/// Dialogue pour l'ensemble des réglages globaux.
	/// </summary>
	public class SettingsDialog : CoreDialog, ISettingsDialog
	{
		public SettingsDialog(CoreApp application)
			: base (application)
		{
			this.settingsTabPages = new List<SettingsTabPages.AbstractSettingsTabPage> ();
		}



		#region ISettingsTabBook Members

		CoreData ISettingsDialog.Data
		{
			get
			{
				return this.application.FindComponent<CoreData> ();
			}
		}

		#endregion
		
		protected override void SetupWindow(Window window)
		{
			window.Text = "Réglages globaux";
			window.MakeFixedSizeWindow ();
			window.ClientSize = new Size (940, 600);
		}

		protected override void SetupWidgets(Window window)
		{
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
				Name = "TabBook",
				Parent = frame,
				Dock = DockStyle.Fill,
			};

			//	Crée l'onglet 'printer'.
			var printerUnitsPage = new TabPage
			{
				TabTitle = "Unités d'impression",
				Name = "printerUnits",
			};

			this.tabBook.Items.Add (printerUnitsPage);

			this.ActiveLastPage ();

			//	Crée le pied de page.
			this.errorInfo = new StaticText
			{
				Parent = footer,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 10, 0, 0),
			};

			this.cancelButton = new Button ()
			{
				CommandObject = Epsitec.Common.Dialogs.Res.Commands.Dialog.Generic.Cancel,
				Parent = footer,
				ButtonStyle = Common.Widgets.ButtonStyle.DefaultCancel,
				Dock = DockStyle.Right,
				Margins = new Margins (10, 0, 0, 0),
				TabIndex = 101,
			};

			this.acceptButton = new Button ()
			{
				CommandObject = Epsitec.Common.Dialogs.Res.Commands.Dialog.Generic.Ok,
				Parent = footer,
				ButtonStyle = Common.Widgets.ButtonStyle.DefaultAccept,
				Dock = DockStyle.Right,
				TabIndex = 100,
			};

			//	Rempli les onglets.
			var printerUnits = new SettingsTabPages.PrintingUnitsTabPage (this);
			printerUnits.CreateUI (printerUnitsPage);
			this.settingsTabPages.Add (printerUnits);

			foreach (var tab in this.settingsTabPages)
			{
				tab.AcceptStateChanging += new EventHandler (this.HandlerSettingsAcceptStateChanging);
			}

			this.RegisterWithPersistenceManager (this.tabBook);
		}

		protected override void UpdateWidgets()
		{
		}

		[Command (Epsitec.Common.Dialogs.Res.CommandIds.Dialog.Generic.Cancel)]
		private void ExecuteCancelCommand()
		{
			if (this.cancelButton.Enable)
			{
				this.CloseAndRejectChanges ();
			}
		}

		[Command (Epsitec.Common.Dialogs.Res.CommandIds.Dialog.Generic.Ok)]
		private void ExecuteOkCommand()
		{
			if (this.acceptButton.Enable)
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

				this.acceptButton.Enable = true;
			}
			else  // erreur ?
			{
				this.errorInfo.Text = errorMessage;
				this.errorInfo.BackColor = Color.FromName ("Gold");

				this.acceptButton.Enable = false;
			}
		}


		private void ActiveLastPage()
		{
			string name = SettingsDialog.lastActivedPageName;

			if (string.IsNullOrEmpty (name))
			{
				name = "printerUnits";  // page par défaut
			}

			var page = this.tabBook.Items.Where (x => x.Name == name).FirstOrDefault ();
			this.tabBook.ActivePage = page;
		}

		private void UpdateLastActivedPageName()
		{
			SettingsDialog.lastActivedPageName = this.tabBook.ActivePage.Name;
		}


		private static string									lastActivedPageName;

		private readonly List<AbstractSettingsTabPage>			settingsTabPages;

		private TabBook											tabBook;
		private StaticText										errorInfo;
		private Button											acceptButton;
		private Button											cancelButton;
	}
}
