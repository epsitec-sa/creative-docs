//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	[System.Flags]
	public enum SearchMode
	{
		Unknown            = 0x0000,

		IgnoreCase         = 0x0001,  // a = A
		IgnoreDiacritic    = 0x0002,  // e = é
		Phonetic           = 0x0004,  // ph = f
		Regex              = 0x0008,

		Fragment           = 0x0100,  // trouve "a" dans "blabla"
		WholeWords         = 0x0200,  // trouve "les" dans "salut les copains"
		FullText           = 0x0400,  // trouve "toto" dans "toto"
	}
}
