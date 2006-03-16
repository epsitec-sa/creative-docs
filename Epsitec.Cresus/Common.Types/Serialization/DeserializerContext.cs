//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types.Serialization.Generic;

namespace Epsitec.Common.Types.Serialization
{
	public class DeserializerContext : Context
	{
		public DeserializerContext(IO.AbstractReader reader)
		{
			this.reader = reader;
		}
	}
}
