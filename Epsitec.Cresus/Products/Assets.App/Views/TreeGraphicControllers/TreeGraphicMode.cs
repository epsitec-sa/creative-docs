//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Views.TreeGraphicControllers
{
	[System.Flags]
	public enum TreeGraphicMode
	{
		None                = 0x0000,

		VerticalFinalNode   = 0x0001,
		CompressEmptyValues = 0x0002,

		FixedWidth          = 0x0010,
		AutoWidthFirstLine  = 0x0020,
		AutoWidthAllLines   = 0x0040,
	}
}
