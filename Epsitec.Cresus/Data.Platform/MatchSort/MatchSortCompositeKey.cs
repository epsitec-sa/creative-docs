using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Epsitec.Data.Platform
{
	public struct MatchSortCompositeKey<T1, T2>
	{
		public T1 PK;
		public T2 FK;
	}
}
