//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support.Extensions;

namespace Epsitec.Cresus.Graph.Dialogs
{
	public class QuestionDialog : ConfirmationDialog
	{
		public QuestionDialog(Caption header, IEnumerable<Caption> captions)
		{
			this.DefineTitle (FormattedText.ParseSimpleText (GraphProgram.Application.ShortWindowTitle));
			this.DefineHeader (QuestionDialog.CreateCaptionText (header));
			
			captions.ForEach (question => this.AddQuestion (QuestionDialog.CreateCaptionText (question)));

			this.OwnerWindow = GraphProgram.Application.Window;
		}

		public QuestionDialog(Caption header, params Caption[] captions)
			: this (header, (IEnumerable<Caption>) captions)
		{
		}

		
		protected override Widget CreateUI()
		{
			var widget = base.CreateUI ();

			var frame  = new FrameBox ()
			{
				PreferredSize = widget.PreferredSize,
			};

			var band = new FrameBox ()
			{
				Parent = frame,
				Dock = DockStyle.Top,
				PreferredHeight = 100,
				BackColor = Color.FromBrightness (1),
			};

			new Separator ()
			{
				Parent = frame,
				Dock = DockStyle.Top,
				PreferredHeight = 1,
				IsHorizontalLine = true,
			};

			widget.Dock = DockStyle.Fill;
			widget.Parent = frame;
			widget.Padding = new Margins (12, 12, 8, 8);

			return frame;
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
	}
}
