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
				EntityPrinter = this.entityPrinter,
				Entity = this.entities.FirstOrDefault (),
			};

			this.preview.Invalidate ();  // pour forcer le dessin

#if true
			var debugPrevButton1 = new GlyphButton
			{
				Parent = window.Root,
				GlyphShape = Common.Widgets.GlyphShape.Minus,
				PreferredWidth = 20,
				PreferredHeight = 20,
				Anchor = AnchorStyles.BottomLeft,
				Margins = new Margins (10, 0, 0, 10),
			};

			this.debugParam1 = new StaticText
			{
				Parent = window.Root,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
				PreferredWidth = 30,
				PreferredHeight = 20,
				Anchor = AnchorStyles.BottomLeft,
				Margins = new Margins (10+20, 0, 0, 10),
			};

			var debugNextButton1 = new GlyphButton
			{
				Parent = window.Root,
				GlyphShape = Common.Widgets.GlyphShape.Plus,
				PreferredWidth = 20,
				PreferredHeight = 20,
				Anchor = AnchorStyles.BottomLeft,
				Margins = new Margins (10+20+30, 0, 0, 10),
			};

			var debugPrevButton2 = new GlyphButton
			{
				Parent = window.Root,
				GlyphShape = Common.Widgets.GlyphShape.Minus,
				PreferredWidth = 20,
				PreferredHeight = 20,
				Anchor = AnchorStyles.BottomLeft,
				Margins = new Margins (10+20+30+50, 0, 0, 10),
			};

			this.debugParam2 = new StaticText
			{
				Parent = window.Root,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
				PreferredWidth = 30,
				PreferredHeight = 20,
				Anchor = AnchorStyles.BottomLeft,
				Margins = new Margins (10+20+30+50+20, 0, 0, 10),
			};

			var debugNextButton2 = new GlyphButton
			{
				Parent = window.Root,
				GlyphShape = Common.Widgets.GlyphShape.Plus,
				PreferredWidth = 20,
				PreferredHeight = 20,
				Anchor = AnchorStyles.BottomLeft,
				Margins = new Margins (10+20+30+50+20+30, 0, 0, 10),
			};

			debugPrevButton1.Clicked += new EventHandler<MessageEventArgs> (debugPrevButton1_Clicked);
			debugNextButton1.Clicked += new EventHandler<MessageEventArgs> (debugNextButton1_Clicked);
			debugPrevButton2.Clicked += new EventHandler<MessageEventArgs> (debugPrevButton2_Clicked);
			debugNextButton2.Clicked += new EventHandler<MessageEventArgs> (debugNextButton2_Clicked);

			this.UpdateDebug ();
#endif
			
			this.closeButton = new Button ()
			{
				Parent = window.Root,
				Text = "Fermer",
				Anchor = AnchorStyles.BottomRight,
				Margins = new Margins (0, 10, 0, 10),
				TabIndex = 1,
			};
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

		private void UpdateDebug()
		{
			this.debugParam1.Text = this.entityPrinter.DebugParam1.ToString ();
			this.debugParam2.Text = this.entityPrinter.DebugParam2.ToString ();

			this.preview.Invalidate ();
		}



		protected void SetupEvents(Window window)
		{
			this.closeButton.Clicked += (sender, e) => this.CloseDialog ();
		}


		private readonly Application application;
		private readonly IEnumerable<AbstractEntity> entities;
		private readonly Printers.AbstractEntityPrinter entityPrinter;

		private Widgets.PreviewEntity preview;
		private Button closeButton;
		private StaticText debugParam1;
		private StaticText debugParam2;
	}
}
