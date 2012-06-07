//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Drawing
{
	[System.Flags]
	public enum GridSnapping
	{
		None = 0x00,
		X = 0x01,
		Y = 0x02,
		Both = 0x03,
	}
}
