//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
	[System.Flags]
	public enum DataExtractionMode
	{
		Default			= 0,

		Sorted			= 0x00000001,

		IncludeArchives = 0x00010000,
	}
}
