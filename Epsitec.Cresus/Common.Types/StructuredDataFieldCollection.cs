//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public class StructuredDataFieldCollection : List<StructuredDataField>
	{
		public StructuredDataFieldCollection()
		{
		}

		
		
		private string name;
		private INamedType type;
	}
}
