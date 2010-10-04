using Epsitec.Common.Support;
using Epsitec.Common.Types;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.EntityEngine
{
	[System.Flags]
	public enum EntityStatus
	{
		None = 0,

		Empty = 0x01,
		Valid = 0x02,
	}
}
