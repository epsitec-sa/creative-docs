//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	[System.Flags]
	public enum ItemStateDetails
	{
		None		= 0x00,

		Flags		= 0x01,
		Full		= 0x02,

		All			= Flags | Full
	}
}
