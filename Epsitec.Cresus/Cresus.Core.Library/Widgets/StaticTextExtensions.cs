//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// Classe d'extension pour Epsitec.Common.Widgets.StaticText.
	/// </summary>
	public static class StaticTextExtensions
	{
		public static void SetColumnTitle(this StaticText staticText, string title)
		{
			staticText.Text = title;
			staticText.PreferredHeight = 20;
			staticText.Dock = DockStyle.Top;
			staticText.FormattedText = TextFormatter.FormatText (title).ApplyFontSize (16);
			staticText.ContentAlignment = ContentAlignment.TopLeft;
			staticText.Margins = new Margins (0, 0, 0, 10);
		}

		public static void SetColumnTitle(this StaticText staticText, FormattedText title)
		{
			staticText.FormattedText = title;
			staticText.PreferredHeight = 20;
			staticText.Dock = DockStyle.Top;
			staticText.FormattedText = TextFormatter.FormatText (title).ApplyFontSize (16);
			staticText.ContentAlignment = ContentAlignment.TopLeft;
			staticText.Margins = new Margins (0, 0, 0, 10);
		}
	}
}
