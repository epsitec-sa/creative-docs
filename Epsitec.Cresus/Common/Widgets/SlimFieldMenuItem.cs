//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	public class SlimFieldMenuItem
	{
		public SlimFieldMenuItem(StringArray texts,
			/**/				 ActiveState active = ActiveState.No,
			/**/				 EnableState enable = EnableState.Enabled,
			/**/				 SlimFieldMenuItemStyle style = SlimFieldMenuItemStyle.Value,
			/**/				 SlimFieldMenuItemHilite hilite = SlimFieldMenuItemHilite.None)
		{
			this.Texts  = texts;
			this.Active = active;
			this.Enable = enable;
			this.Style  = style;
			this.Hilite = hilite;
		}

		
		public StringArray						Texts
		{
			get;
			set;
		}

		public ActiveState						Active
		{
			get;
			set;
		}

		public EnableState						Enable
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
