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
	class MultilingualEditionDialog : AbstractDialog
	{
		public MultilingualEditionDialog(AbstractTextField textField)
		{
			this.IsApplicationWindow = true;  // pour avoir les boutons Minimize/Maximize/Close !

			this.textField = textField;
			this.multilingualText = new MultilingualText (this.textField.FormattedText);

			this.textFields = new List<AbstractTextField> ();
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
			this.OwnerWindow = this.textField.Window;

			window.Icon = this.textField.Window.Icon;
			window.Text = "Edition multilingue";
			window.ClientSize = new Size (600, 400);
			window.Root.WindowStyles = WindowStyles.DefaultDocumentWindow;  // pour avoir les boutons Minimize/Maximize/Close !

			window.WindowCloseClicked += delegate
			{
				this.OnDialogClosed ();
				this.CloseDialog ();
			};
		}

		protected void SetupWidgets(Window window)
		{
			var frame = new FrameBox
			{
				Parent = window.Root,
				Anchor = AnchorStyles.All,
				Margins = new Margins (10, 10, 10, 40),
			};

			foreach (var id in this.multilingualText.GetContainedLanguageIds ())
			{
				var label = new StaticText
				{
					Parent = frame,
					Text = string.Format ("Id langue = {0} :", id.ToString ()),
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
				};

				var textField = new TextField
				{
					Parent = frame,
					FormattedText = this.multilingualText.GetTextOrDefault (id),
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderTextField),
				};

				this.textFields.Add (textField);
			}

			this.footer = new FrameBox
			{
				Parent = window.Root,
				PreferredHeight = 20,
				Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
				Margins = new Margins (10, 10, 0, 10),
			};

			this.closeButton = new Button ()
			{
				Parent = this.footer,
				Text = "Fermer",
				ButtonStyle = Common.Widgets.ButtonStyle.DefaultAcceptAndCancel,
				Dock = DockStyle.Right,
				TabIndex = 1,
			};
		}

		protected void SetupEvents(Window window)
		{
			this.closeButton.Clicked += delegate
			{
				this.CloseDialog ();
			};
		}


		protected override void OnDialogClosed()
		{
			base.OnDialogClosed ();
		}



		private readonly AbstractTextField				textField;
		private readonly MultilingualText				multilingualText;
		private readonly List<AbstractTextField>		textFields;

		private FrameBox								footer;
		private Button									closeButton;
	}
}
