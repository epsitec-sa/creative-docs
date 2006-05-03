//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public class StructuredRecord
	{
		public StructuredRecord(StructuredRecordType type)
		{
			this.type = type;
		}

		private StructuredRecordType type;
	}
}
