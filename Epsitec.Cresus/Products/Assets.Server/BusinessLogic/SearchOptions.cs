//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	[System.Flags]
	public enum SearchOptions
	{
		Unknown            = 0x0000,

		IgnoreCase         = 0x0001,  // a = A
		IgnoreDiacritic    = 0x0002,  // e = é
		Phonetic           = 0x0004,  // ph = f
		Regex              = 0x0008,
		WholeWords         = 0x0010,  // trouve "les" dans "salut les copains"

		Prefix             = 0x0200,  // trouve "salut" dans "salut les copains"
		Sufffix            = 0x0400,  // trouve "copains" dans "salut les copains"
		FullText           = 0x0800,  // trouve "toto" dans "toto"
	}
}
