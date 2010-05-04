//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	[System.Flags]
	public enum ClippingMode
	{
		AutoSize = 0,
		ClipWidth = 0x1,
		ClipHeight = 0x2,
		ClipAll = ClipWidth | ClipHeight
	}
}
