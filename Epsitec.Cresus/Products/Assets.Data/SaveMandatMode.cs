//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	[System.Flags]
	public enum SaveMandatMode
	{
		None         = 0x0000,
		SaveUI       = 0x0001,
		SaveUndoRedo = 0x0002,
		KeepXml      = 0x0004,
	}
}