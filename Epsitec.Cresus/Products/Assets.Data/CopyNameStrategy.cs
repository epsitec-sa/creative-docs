//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public enum CopyNameStrategy
	{
		CopyOfName,				// Copie de Toto
		NameDashCopy,			// Toto - copie
		NameBracketCopy,		// Toto (copie)
	}
}