//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Epsitec.Cresus.Compta.Helpers
{
	/// <summary>
	/// Classe d'extension pour Epsitec.Common.Widgets.StaticText.
	/// </summary>
	public static class StaticTextExtensions
	{
		public static void SetColumnTitle(this StaticText staticText, string title)
		{
			staticText.FormattedText = TextFormatter.FormatText (title).ApplyFontSize (13);
			staticText.ContentAlignment = ContentAlignment.TopLeft;
			staticText.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
			staticText.PreferredHeight = 20;
			staticText.Dock = DockStyle.Top;
		}

		public static void SetColumnTitle(this StaticText staticText, FormattedText title)
		{
			staticText.FormattedText = TextFormatter.FormatText (title).ApplyFontSize (13);
			staticText.ContentAlignment = ContentAlignment.TopLeft;
			staticText.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
			staticText.PreferredHeight = 20;
			staticText.Dock = DockStyle.Top;
		}
	}
}
