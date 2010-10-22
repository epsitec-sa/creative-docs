//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business
{
	[System.Flags]
	public enum EntitySaveMode
	{
		None			= 0,

		IncludeEmpty	= 0x0001,
	}
}
