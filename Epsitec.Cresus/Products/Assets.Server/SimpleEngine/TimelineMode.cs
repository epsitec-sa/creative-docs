//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	[System.Flags]
	public enum TimelineMode
	{
		Compacted	= 0x00000001,
		Expanded	= 0x00000002,

		DaysOfWeek	= 0x00000100,
		WeeksOfYear	= 0x00000200,
		Graph		= 0x00000400,
		Labels		= 0x00000800,
	}
}
