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
			window.Text = "Aperçu";
			window.WindowSize = new Size (800, 1000);
			window.MakeFloatingWindow ();
		}

		protected void SetupWidgets(Window window)
		{
			var preview = new Widgets.PreviewEntity
			{
				Parent = window.Root,
				Anchor = AnchorStyles.All,
				Margins = new Margins (10, 10, 10, 40),
				EntityPrinter = this.entityPrinter,
				Entity = this.entities.FirstOrDefault (),
			};

			preview.Invalidate ();  // pour forcer le dessin
			
			this.closeButton = new Button ()
			{
				Parent = window.Root,
				Anchor = AnchorStyles.BottomRight,
				Margins = new Margins (0, 10, 0, 10),
				Text = "Fermer",
				TabIndex = 1,
			};
		}

		protected void SetupEvents(Window window)
		{
			this.closeButton.Clicked += (sender, e) => this.CloseDialog ();
		}


		private readonly Application application;
		private readonly IEnumerable<AbstractEntity> entities;
		private readonly Printers.AbstractEntityPrinter entityPrinter;

		private Button closeButton;
	}
}
