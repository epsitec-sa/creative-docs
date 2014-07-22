//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.Views.EditorPages;

namespace Epsitec.Cresus.Assets.App.Views
{
	public struct SummaryControllerTile
	{
		public SummaryControllerTile(string text, string tooltip = null, ContentAlignment alignment = ContentAlignment.MiddleLeft, bool hilited = false, bool readOnly = false, bool hatch = false, bool label = false)
		{
			this.Text      = text;
			this.Tootip    = tooltip;
			this.Alignment = alignment;
			this.Hilited   = hilited;
			this.ReadOnly  = readOnly;
			this.Hatch     = hatch;
			this.Label     = label;
		}

		public FieldColorType GetFieldColorType(bool isLocked)
		{
			if (this.Hilited)
			{
				return this.ReadOnly || isLocked ? FieldColorType.Result : FieldColorType.Defined;
			}
			else
			{
				return this.ReadOnly || isLocked ? FieldColorType.Readonly : FieldColorType.Editable;
			}
		}

		public readonly string				Text;
		public readonly string				Tootip;
		public readonly ContentAlignment	Alignment;
		public readonly bool				Hilited;
		public readonly bool				ReadOnly;
		public readonly bool				Hatch;
		public readonly bool				Label;
	}
}
