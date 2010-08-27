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
			this.preview = new Widgets.PreviewEntity
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

			var label = new StaticText
			{
				Parent = this.footer,
				Text = "Aperçu de la page",
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleLeft,
				PreferredWidth = 100,
				PreferredHeight = 20,
				Dock = DockStyle.Left,
				Margins = new Margins (20, 0, 0, 0),
			};

			this.pagePrevButton = new GlyphButton
			{
				Parent = this.footer,
				GlyphShape = Common.Widgets.GlyphShape.ArrowLeft,
				PreferredWidth = 20,
				PreferredHeight = 20,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 0, 0, 0),
			};

			this.pageRank = new StaticText
			{
				Parent = this.footer,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
				PreferredWidth = 30,
				PreferredHeight = 20,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 0, 0, 0),
			};

			this.pageNextButton = new GlyphButton
			{
				Parent = this.footer,
				GlyphShape = Common.Widgets.GlyphShape.ArrowRight,
				PreferredWidth = 20,
				PreferredHeight = 20,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 0, 0, 0),
			};

			if (this.entityPrinter.DocumentTypeEnumSelected == DocumentTypeEnum.Debug1 ||
				this.entityPrinter.DocumentTypeEnumSelected == DocumentTypeEnum.Debug2)
			{
				this.debugPrevButton1 = new GlyphButton
				{
					Parent = this.footer,
					GlyphShape = Common.Widgets.GlyphShape.Minus,
					PreferredWidth = 20,
					PreferredHeight = 20,
					Dock = DockStyle.Left,
					Margins = new Margins (50, 0, 0, 0),
				};

				this.debugParam1 = new StaticText
				{
					Parent = this.footer,
					ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
					PreferredWidth = 30,
					PreferredHeight = 20,
					Dock = DockStyle.Left,
					Margins = new Margins (0, 0, 0, 0),
				};

				this.debugNextButton1 = new GlyphButton
				{
					Parent = this.footer,
					GlyphShape = Common.Widgets.GlyphShape.Plus,
					PreferredWidth = 20,
					PreferredHeight = 20,
					Dock = DockStyle.Left,
					Margins = new Margins (0, 0, 0, 0),
				};


				this.debugPrevButton2 = new GlyphButton
				{
					Parent = this.footer,
					GlyphShape = Common.Widgets.GlyphShape.Minus,
					PreferredWidth = 20,
					PreferredHeight = 20,
					Dock = DockStyle.Left,
					Margins = new Margins (20, 0, 0, 0),
				};

				this.debugParam2 = new StaticText
				{
					Parent = this.footer,
					ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
					PreferredWidth = 30,
					PreferredHeight = 20,
					Dock = DockStyle.Left,
					Margins = new Margins (0, 0, 0, 0),
				};

				this.debugNextButton2 = new GlyphButton
				{
					Parent = this.footer,
					GlyphShape = Common.Widgets.GlyphShape.Plus,
					PreferredWidth = 20,
					PreferredHeight = 20,
					Dock = DockStyle.Left,
					Margins = new Margins (0, 0, 0, 0),
				};

				this.UpdateDebug ();
			}

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
				Text = "Imprimer",
				Dock = DockStyle.Right,
				Margins = new Margins (0, 10, 0, 0),
				TabIndex = 1,
			};

			this.pagesInfo = new StaticText ()
			{
				Parent = this.footer,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleRight,
				PreferredWidth = 100,
				Dock = DockStyle.Right,
				Margins = new Margins (0, 20, 0, 0),
			};

			this.preview.BuildSections (this.entityPrinter);
			this.UpdatePage ();
		}

		protected void SetupEvents(Window window)
		{
			this.pagePrevButton.Clicked += new EventHandler<MessageEventArgs> (pagePrevButton_Clicked);
			this.pageNextButton.Clicked += new EventHandler<MessageEventArgs> (pageNextButton_Clicked);

			if (this.debugPrevButton1 != null)
			{
				this.debugPrevButton1.Clicked += new EventHandler<MessageEventArgs> (debugPrevButton1_Clicked);
				this.debugNextButton1.Clicked += new EventHandler<MessageEventArgs> (debugNextButton1_Clicked);
				this.debugPrevButton2.Clicked += new EventHandler<MessageEventArgs> (debugPrevButton2_Clicked);
				this.debugNextButton2.Clicked += new EventHandler<MessageEventArgs> (debugNextButton2_Clicked);
			}

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

		private void pagePrevButton_Clicked(object sender, MessageEventArgs e)
		{
			this.entityPrinter.CurrentPage -= GetStep (e);
			this.UpdatePage ();
		}

		private void pageNextButton_Clicked(object sender, MessageEventArgs e)
		{
			this.entityPrinter.CurrentPage += GetStep (e);
			this.UpdatePage ();
		}

		private void debugPrevButton1_Clicked(object sender, MessageEventArgs e)
		{
			this.entityPrinter.DebugParam1 -= GetStep(e);
			this.UpdateDebug ();
		}

		private void debugNextButton1_Clicked(object sender, MessageEventArgs e)
		{
			this.entityPrinter.DebugParam1 += GetStep (e);
			this.UpdateDebug ();
		}

		private void debugPrevButton2_Clicked(object sender, MessageEventArgs e)
		{
			this.entityPrinter.DebugParam2 -= GetStep (e);
			this.UpdateDebug ();
		}

		private void debugNextButton2_Clicked(object sender, MessageEventArgs e)
		{
			this.entityPrinter.DebugParam2 += GetStep (e);
			this.UpdateDebug ();
		}

		private static int GetStep(MessageEventArgs e)
		{
			int step = 1;

			if ((e.Message.ModifierKeys & ModifierKeys.Control) != 0)
			{
				step *= 10;
			}

			if ((e.Message.ModifierKeys & ModifierKeys.Shift) != 0)
			{
				step *= 100;
			}

			return step;
		}

		private void UpdatePage()
		{
			this.pagePrevButton.Enable = this.entityPrinter.CurrentPage > 0;
			this.pageNextButton.Enable = this.entityPrinter.CurrentPage < this.entityPrinter.PageCount-1;
			this.pageRank.Text = (this.entityPrinter.CurrentPage+1).ToString ();

			this.preview.Invalidate ();

			this.pagesInfo.Text = string.Format ("{0} page{1}", this.entityPrinter.PageCount.ToString (), (this.entityPrinter.PageCount<=1)?"":"s");
		}

		private void UpdateDebug()
		{
			this.debugParam1.Text = this.entityPrinter.DebugParam1.ToString ();
			this.debugParam2.Text = this.entityPrinter.DebugParam2.ToString ();

			this.preview.Invalidate ();
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
			this.entityPrinter.Clear ();
			this.entityPrinter.BuildSections ();

			this.entityPrinter.CurrentPage = System.Math.Min (this.entityPrinter.CurrentPage, this.entityPrinter.PageCount-1);
			this.UpdatePage ();
		}

		#endregion


		private readonly CoreApplication application;
		private readonly IEnumerable<AbstractEntity> entities;
		private readonly Printers.AbstractEntityPrinter entityPrinter;

		private Widgets.PreviewEntity preview;
		private FrameBox footer;

		private GlyphButton pagePrevButton;
		private StaticText pageRank;
		private GlyphButton pageNextButton;

		private GlyphButton debugPrevButton1;
		private StaticText debugParam1;
		private GlyphButton debugNextButton1;

		private GlyphButton debugPrevButton2;
		private StaticText debugParam2;
		private GlyphButton debugNextButton2;

		private Button updateButton;
		private StaticText pagesInfo;
		private Button printButton;
		private Button closeButton;
	}
}
