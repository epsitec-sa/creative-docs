//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{
	public enum PageType
	{
		Unknown,

		Single,			// page unique
		First,			// première page (0)
		Following,		// page suivante (1..n)

		ESR,			// page avec BV
		Label,			// page avec étiquette
	}
}
