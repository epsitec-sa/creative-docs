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
using Epsitec.Cresus.Core.Print;
using Epsitec.Cresus.Core.Print.Serialization;
using Epsitec.Cresus.Core.Print.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Business.UserManagement;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs
{
	/// <summary>
	/// Dialogue pour monter un aperçu d'une page avant l'impression. On peut naviguer dans les différentes
	/// pages du document.
	/// </summary>
	class PrintPreviewDialog : AbstractDialog
	{
		public PrintPreviewDialog(CoreApplication application, CoreData coreData, List<DeserializedJob> jobs)
		{
			this.IsApplicationWindow = true;  // pour avoir les boutons Minimize/Maximize/Close !

			this.application = application;
			this.coreData = coreData;
			this.jobs = jobs;
			this.pages = Print.Common.GetDeserializedPages (this.jobs).ToList ();
			this.previewerController = new XmlPreviewerController (this.coreData, this.jobs);
		}


		protected override Window CreateWindow()
		{
			Window window = new Window ();

			this.SetupWindow  (window);
			this.SetupWidgets (window);
			this.SetupEvents  (window);

			window.AdjustWindowSize ();

			return window;
		}

		protected void SetupWindow(Window window)
		{
			this.OwnerWindow = this.application.Window;
			window.Icon = this.application.Window.Icon;
			window.Text = "Aperçu de l'impression";
			window.ClientSize = new Size (800, 600);
			window.Root.WindowStyles = WindowStyles.DefaultDocumentWindow;  // pour avoir les boutons Minimize/Maximize/Close !

			window.WindowCloseClicked += delegate
			{
				this.OnDialogClosed ();
				this.CloseDialog ();
			};
		}

		protected void SetupWidgets(Window window)
		{
			int tabIndex = 1;

			var previewBox = new FrameBox
			{
				Parent = window.Root,
				Dock = DockStyle.Fill,
				Margins = new Margins (10, 10, 10, 10),
				TabIndex = tabIndex++,
			};

			var footer = new FrameBox
			{
				Parent = window.Root,
				PreferredHeight = 20,
				Dock = DockStyle.Bottom,
				Margins = new Margins (10, 10, 0, 10),
				TabIndex = tabIndex++,
			};

			//	Crée le pied de page.
			this.closeButton = new Button
			{
				Parent = footer,
				Text = "Fermer",
				ButtonStyle = Common.Widgets.ButtonStyle.DefaultCancel,
				Dock = DockStyle.Right,
				Margins = new Margins (10, 0, 0, 0),
				TabIndex = tabIndex++,
			};

			this.printButton = new Button
			{
				Parent = footer,
				Text = "Imprimer",
				ButtonStyle = Common.Widgets.ButtonStyle.DefaultAccept,
				Dock = DockStyle.Right,
				Margins = new Margins (20, 0, 0, 0),
				TabIndex = tabIndex++,
			};

			var pagesToolbarBox = new FrameBox
			{
				Parent = footer,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 0),
				TabIndex = tabIndex++,
			};

			this.previewerController.CreateUI (previewBox, pagesToolbarBox);
			this.UpdateWidgets ();
		}

		protected void SetupEvents(Window window)
		{
			this.printButton.Clicked += delegate
			{
				this.CloseAction (cancel: false);
			};

			this.closeButton.Clicked += delegate
			{
				this.CloseAction (cancel: true);
			};
		}

		private void CloseAction(bool cancel)
		{
			if (cancel)
			{
				this.Result = DialogResult.Cancel;
			}
			else
			{
				this.Result = DialogResult.Accept;
			}

			this.CloseDialog ();
		}

		private void UpdateWidgets()
		{
		}


		protected override void OnDialogClosed()
		{
			base.OnDialogClosed ();
		}


		private readonly CoreApplication						application;
		private readonly CoreData								coreData;
		private readonly List<DeserializedJob>					jobs;
		private readonly List<DeserializedPage>					pages;
		private readonly XmlPreviewerController					previewerController;

		private Button											printButton;
		private Button											closeButton;
	}
}
