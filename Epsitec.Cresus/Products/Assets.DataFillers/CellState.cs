//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	[System.Flags]
	public enum CellState
	{
		None        = 0x0000,
		Selected    = 0x0001,
		Event       = 0x0002,
		Error       = 0x0004,
		Unavailable = 0x0008,
	}
}
