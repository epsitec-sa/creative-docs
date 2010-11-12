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
using Epsitec.Cresus.Core.Printers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Business.UserManagement;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs
{
	/// <summary>
	/// Dialogue pour choisir l'utilisateur (loggin).
	/// </summary>
	class XmlDeserializerPreviewerDialog : AbstractDialog
	{
		public XmlDeserializerPreviewerDialog(CoreApplication application, string xmlSource)
		{
			this.IsApplicationWindow = true;  // pour avoir les boutons Minimize/Maximize/Close !

			this.application = application;
			this.xmlSource   = xmlSource;
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
			window.Text = "Visualisation de la désérialisation d'un document";
			window.ClientSize = new Size (1024, 768);
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

			var topPane = new FrameBox
			{
				Parent = window.Root,
				ContainerLayoutMode = Common.Widgets.ContainerLayoutMode.HorizontalFlow,
				Dock = DockStyle.Fill,
				Margins = new Margins (10, 10, 10, 0),
				TabIndex = tabIndex++,
			};

			var footer = new FrameBox
			{
				Parent = window.Root,
				PreferredHeight = 20,
				Dock = DockStyle.Bottom,
				Margins = new Margins (10, 10, 10, 10),
				TabIndex = tabIndex++,
			};

			//	Crée la partie principale.
			this.textField = new TextFieldMulti
			{
				Parent = topPane,
				MaxLength = 100000,
				Text = TextLayout.ConvertToTaggedText (this.xmlSource),
				IsReadOnly = true,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 10, 0, 0),
			};

			this.previewer = new XmlDeserializerPreviewer
			{
				Parent = topPane,
				XmlSource = this.xmlSource,
				Dock = DockStyle.Fill,
			};

			//	Crée le pied de page.
			{
				this.closeButton = new Button ()
				{
					Parent = footer,
					Text = "Fermer",
					PreferredWidth = 60,
					ButtonStyle = Common.Widgets.ButtonStyle.DefaultCancel,
					Dock = DockStyle.Right,
					TabIndex = tabIndex++,
				};
			}

			this.UpdateWidgets ();
		}

		protected void SetupEvents(Window window)
		{
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
		private readonly string									xmlSource;

		private TextFieldMulti									textField;
		private XmlDeserializerPreviewer						previewer;
		private Button											closeButton;
	}
}
