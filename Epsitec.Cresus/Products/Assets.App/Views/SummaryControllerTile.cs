//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Views
{
	public struct SummaryControllerTile
	{
		public SummaryControllerTile(string text, string tooltip = null, ContentAlignment alignment = ContentAlignment.MiddleLeft, bool defined = false, bool readOnly = false, bool label = false)
		{
			this.Text      = text;
			this.Tootip    = tooltip;
			this.Alignment = alignment;
			this.Defined   = defined;
			this.ReadOnly  = readOnly;
			this.Label     = label;
		}

		public FieldColorType FieldColorType
		{
			get
			{
				if (this.Defined)
				{
					return FieldColorType.Defined;
				}
				else
				{
					return this.ReadOnly ? FieldColorType.Readonly : FieldColorType.Editable;
				}
			}
		}

		public readonly string				Text;
		public readonly string				Tootip;
		public readonly ContentAlignment	Alignment;
		public readonly bool				Defined;
		public readonly bool				ReadOnly;
		public readonly bool				Label;
	}
}
