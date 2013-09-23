//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

namespace Epsitec.Cresus.Assets.App.Widgets
{
	[System.Flags]
	public enum TimelineValueDisplayMode
	{
		Dots		= 0x0001,
		Lines		= 0x0002,
		Surfaces	= 0x0004,

		All			= 0xffff,
	};
}
