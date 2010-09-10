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

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs
{
	/// <summary>
	/// Dialogue pour monter un aperçu d'une page avant l'impression. On peut naviguer dans les différentes
	/// pages du document.
	/// </summary>
	class PreviewDialog : AbstractDialog, IAttachedDialog
	{
		public PreviewDialog(CoreApplication application, Printers.AbstractEntityPrinter entityPrinter, IEnumerable<AbstractEntity> entities)
		{
			this.IsApplicationWindow = true;  // pour avoir les boutons Minimize/Maximize/Close !

			this.application   = application;
			this.entityPrinter = entityPrinter;
			this.entities      = entities;

			this.application.AttachDialog (this);

			this.previewerController = new PreviewerController (this.entityPrinter, this.entities);

			this.entityPrinter.IsPreview = true;
			this.entityPrinter.BuildSections ();
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

			var pageSize = this.entityPrinter.PageSize;
			string path = System.IO.Path.Combine (Globals.Directories.ExecutableRoot, "app.ico");

			window.Icon = this.application.Window.Icon;
			window.Text = "Aperçu avant impression";
			window.ClientSize = new Size (pageSize.Width*3+10+10, pageSize.Height*3+10+40);
			window.Root.WindowStyles = WindowStyles.DefaultDocumentWindow;  // pour avoir les boutons Minimize/Maximize/Close !

			window.WindowCloseClicked += delegate
			{
				this.OnDialogClosed ();
				this.CloseDialog ();
			};
		}

		protected void SetupWidgets(Window window)
		{
			var previewBox = new FrameBox
			{
				Parent = window.Root,
				Anchor = AnchorStyles.All,
				Margins = new Margins (10, 10, 10, 40),
			};

			this.footer = new FrameBox
			{
				Parent = window.Root,
				PreferredHeight = 20,
				Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
				Margins = new Margins (10, 10, 0, 10),
			};

			this.updateButton = new Button
			{
				Parent = this.footer,
				Text = "Mettre à jour",
				Dock = DockStyle.Left,
				Margins = new Margins (0, 0, 0, 0),
			};

			var toolbarBox = new FrameBox
			{
				Parent = this.footer,
				Dock = DockStyle.Fill,
				Margins = new Margins (20, 20, 0, 0),
			};

			this.closeButton = new Button ()
			{
				Parent = this.footer,
				Text = "Fermer",
				Dock = DockStyle.Right,
				TabIndex = 1,
			};

			this.printButton = new Button ()
			{
				Parent = this.footer,
				Text = "Tâches d'impression",
				PreferredWidth = 120,
				Dock = DockStyle.Right,
				Margins = new Margins (0, 10, 0, 0),
				TabIndex = 1,
			};

			this.previewerController.CreateUI (previewBox, toolbarBox);
		}

		protected void SetupEvents(Window window)
		{
			this.updateButton.Clicked += delegate
			{
				this.Update ();
			};

			this.printButton.Clicked += delegate
			{
				this.CloseDialog ();

				PrintEngine.Print (this.entities, this.entityPrinter);
			};

			this.closeButton.Clicked += delegate
			{
				this.CloseDialog ();
			};

			this.closeButton.Clicked += (sender, e) => this.CloseDialog ();
		}


		protected override void OnDialogClosed()
		{
			base.OnDialogClosed ();

			this.application.DetachDialog (this);
		}



		#region IAttachedDialog Members

		public IEnumerable<AbstractEntity> Entities
		{
			get
			{
				return this.entities;
			}
		}

		public void Update()
		{
			//?this.entityPrinter.Clear ();
			//?this.entityPrinter.BuildSections ();

			//?this.preview.CurrentPage = System.Math.Min (this.preview.CurrentPage, this.entityPrinter.PageCount ()-1);
			//?this.UpdatePage ();
		}

		#endregion


		private readonly CoreApplication				application;
		private readonly IEnumerable<AbstractEntity>	entities;
		private readonly Printers.AbstractEntityPrinter	entityPrinter;
		private readonly Printers.PreviewerController	previewerController;

		private FrameBox								footer;

		private Button									updateButton;
		private Button									printButton;
		private Button									closeButton;
	}
}
