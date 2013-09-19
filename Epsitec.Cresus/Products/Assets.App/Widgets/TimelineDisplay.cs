//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

namespace Epsitec.Cresus.Assets.App.Widgets
{
	[System.Flags]
	public enum TimelineDisplay
	{
		Month      = 0x0001,
		Weeks      = 0x0002,
		DaysOfWeek = 0x0004,
		Days       = 0x0008,
		Glyphs     = 0x0010,

		Default    = 0x0019,
		All        = 0xffff,
	};
}
