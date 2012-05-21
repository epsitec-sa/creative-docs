//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Graph.Widgets;

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Linq;
using System.Collections.Generic;

namespace Epsitec.Cresus.Graph.Dialogs
{
	public class QuestionDialog : ConfirmationDialog
	{
		public QuestionDialog(Caption header, IEnumerable<Caption> captions)
		{
			this.DefineTitle (GraphProgram.Application.ShortWindowTitle);
			this.DefineHeader (QuestionDialog.CreateCaptionText (header));

			this.headerTitle = header.SortedLabels.First ();
			
			captions.ForEach (question => this.AddQuestion (QuestionDialog.CreateCaptionText (question)));

			this.OwnerWindow = GraphProgram.Application.Window;
		}

		public QuestionDialog(Caption header, params Caption[] captions)
			: this (header, (IEnumerable<Caption>) captions)
		{
		}


		public void DefineArguments(params string[] args)
		{
			this.UpdateQuestions (
				x =>
				{
					for (int i = 0; i < args.Length; i++)
					{
						x = x.Replace (string.Format (System.Globalization.CultureInfo.InvariantCulture, "{{{0}}}", i), args[i]);
					}

					return x;
				});
		}
		
		protected override Widget CreateUI()
		{
			var widget = base.CreateUI ();

			var frame  = new FrameBox ()
			{
				PreferredSize = widget.PreferredSize + new Size (2, 2),
				DrawFullFrame = true,
				Padding = new Margins (3, 3, 3, 3),
				BackColor = Color.Mix (Epsitec.Common.Widgets.Adorners.Factory.Active.ColorBorder, Color.FromBrightness (1), 0.25),
			};

			var inside = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = frame,
				DrawFullFrame = true,
				Padding = new Margins (1, 1, 1, 1),
			};

			var band = new DialogHeader ()
			{
				Parent = inside,
				Text = this.headerTitle,
			};

			widget.Dock = DockStyle.Fill;
			widget.Parent = inside;
			widget.Padding = new Margins (12, 12, 8, 8);

			return frame;
		}

		protected override void SetupWindow(Window dialogWindow)
		{
			base.SetupWindow (dialogWindow);

			dialogWindow.MakeFramelessWindow ();
		}
		
		private static FormattedText CreateCaptionText(Caption caption)
		{
			var text = new System.Text.StringBuilder ();

			text.Append ("<font size=\"120%\">");
			text.Append (caption.DefaultLabel);
			text.Append ("</font>");

			if (!string.IsNullOrEmpty (caption.Description))
			{
				text.Append ("<br/>");
				text.Append (caption.Description);
			}

			return new FormattedText (text.ToString ());
		}

		private string headerTitle;
	}
}
