//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Platform
{
	public enum WindowManagerOperation
	{
		None,
		
		MoveWindow,

		ResizeTop,
		ResizeBottom,
		ResizeLeft,
		ResizeRight,
		
		ResizeTopLeft,
		ResizeTopRight,
		ResizeBottomLeft,
		ResizeBottomRight,
		
		PressMinimizeButton,
		PressMaximizeButton,
	}
}
