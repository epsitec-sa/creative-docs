//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	public class SlimFieldMenuItem
	{
		public SlimFieldMenuItem(string text, ActiveState active = ActiveState.No, SlimFieldMenuItemType type = SlimFieldMenuItemType.Value, SlimFieldMenuItemHilite hilite = SlimFieldMenuItemHilite.None)
		{
			this.Text   = text;
			this.Active = active;
			this.Type   = type;
			this.Hilite = hilite;
		}

		
		public string Text
		{
			get;
			set;
		}

		public ActiveState Active
		{
			get;
			set;
		}

		public SlimFieldMenuItemType Type
		{
			get;
			set;
		}

		public SlimFieldMenuItemHilite Hilite
		{
			get;
			set;
		}
	}
}
