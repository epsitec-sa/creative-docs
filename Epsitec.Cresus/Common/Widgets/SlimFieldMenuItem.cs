//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	public class SlimFieldMenuItem
	{
		public SlimFieldMenuItem(string text, ActiveState active = ActiveState.No, SlimFieldMenuItemStyle style = SlimFieldMenuItemStyle.Value, SlimFieldMenuItemHilite hilite = SlimFieldMenuItemHilite.None)
		{
			this.Text   = text;
			this.Active = active;
			this.Style  = style;
			this.Hilite = hilite;
		}

		
		public string							Text
		{
			get;
			set;
		}

		public ActiveState						Active
		{
			get;
			set;
		}

		public SlimFieldMenuItemStyle			Style
		{
			get;
			set;
		}

		public SlimFieldMenuItemHilite			Hilite
		{
			get;
			set;
		}


		public virtual bool ExecuteCommand(Behaviors.SlimFieldMenuBehavior source)
		{
			return false;
		}
	}
}
