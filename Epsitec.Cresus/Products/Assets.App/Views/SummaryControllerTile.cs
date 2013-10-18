//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Views
{
	public struct SummaryControllerTile
	{
		public SummaryControllerTile(string text, string tooltip = null, ContentAlignment alignment = ContentAlignment.MiddleLeft, bool hilited = false)
		{
			this.Text      = text;
			this.Tootip    = tooltip;
			this.Alignment = alignment;
			this.Hilited   = hilited;
		}

		public readonly string				Text;
		public readonly string				Tootip;
		public readonly ContentAlignment	Alignment;
		public readonly bool				Hilited;
	}
}
