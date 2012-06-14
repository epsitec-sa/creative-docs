//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	public class SlimFieldMenuItem<T> : SlimFieldMenuItem
	{
		public SlimFieldMenuItem(T value,
			/**/				 StringArray texts,
			/**/				 ActiveState active = ActiveState.No,
			/**/				 EnableState enable = EnableState.Enabled,
			/**/				 SlimFieldMenuItemStyle style = SlimFieldMenuItemStyle.Value,
			/**/				 SlimFieldMenuItemHilite hilite = SlimFieldMenuItemHilite.None)
			: base (texts, active, enable, style, hilite)
		{
			this.Value = value;
		}


		public T Value
		{
			get;
			set;
		}
	}
}
