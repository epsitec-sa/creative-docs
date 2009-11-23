//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Graph.Widgets;

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
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
			this.DefineTitle (FormattedText.ParseSimpleText (GraphProgram.Application.ShortWindowTitle));
			this.DefineHeader (QuestionDialog.CreateCaptionText (header));

			this.headerTitle = header.SortedLabels.First ();
			
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

			var band = new DialogHeader ()
			{
				Parent = frame,
				Text = this.headerTitle,
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

		private string headerTitle;
	}
}
