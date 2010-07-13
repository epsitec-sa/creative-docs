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

	class PreviewDialog : AbstractDialog
	{
		public PreviewDialog(Application application, Printers.AbstractEntityPrinter entityPrinter, IEnumerable<AbstractEntity> entities)
		{
			this.application   = application;
			this.entityPrinter = entityPrinter;
			this.entities      = entities;
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
			window.Text = "Aperçu avant impression";
			window.WindowSize = new Size (210*3, 297*3+40);
			window.MakeFloatingWindow ();
		}

		protected void SetupWidgets(Window window)
		{
			this.preview = new Widgets.PreviewEntity
			{
				Parent = window.Root,
				Anchor = AnchorStyles.All,
				Margins = new Margins (10, 10, 10, 40),
			};

			this.preview.Build (this.entityPrinter);

			this.footer = new FrameBox
			{
				Parent = window.Root,
				PreferredHeight = 20,
				Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
				Margins = new Margins (10, 10, 0, 10),
			};

			this.preview.Invalidate ();  // pour forcer le dessin

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

			this.UpdatePage ();

#if true
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
#endif
			
			this.closeButton = new Button ()
			{
				Parent = this.footer,
				Text = "Fermer",
				Dock = DockStyle.Right,
				TabIndex = 1,
			};
		}

		protected void SetupEvents(Window window)
		{
			this.pagePrevButton.Clicked += new EventHandler<MessageEventArgs> (pagePrevButton_Clicked);
			this.pageNextButton.Clicked += new EventHandler<MessageEventArgs> (pageNextButton_Clicked);

			this.debugPrevButton1.Clicked += new EventHandler<MessageEventArgs> (debugPrevButton1_Clicked);
			this.debugNextButton1.Clicked += new EventHandler<MessageEventArgs> (debugNextButton1_Clicked);
			this.debugPrevButton2.Clicked += new EventHandler<MessageEventArgs> (debugPrevButton2_Clicked);
			this.debugNextButton2.Clicked += new EventHandler<MessageEventArgs> (debugNextButton2_Clicked);

			this.closeButton.Clicked += (sender, e) => this.CloseDialog ();
		}

		private void pagePrevButton_Clicked(object sender, MessageEventArgs e)
		{
			this.entityPrinter.ShowedPage -= GetStep (e);
			this.UpdatePage ();
		}

		private void pageNextButton_Clicked(object sender, MessageEventArgs e)
		{
			this.entityPrinter.ShowedPage += GetStep (e);
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
			this.pageRank.Text = (this.entityPrinter.ShowedPage+1).ToString ();

			this.preview.Invalidate ();
		}

		private void UpdateDebug()
		{
			this.debugParam1.Text = this.entityPrinter.DebugParam1.ToString ();
			this.debugParam2.Text = this.entityPrinter.DebugParam2.ToString ();

			this.preview.Invalidate ();
		}




		private readonly Application application;
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
		
		private Button closeButton;
	}
}
