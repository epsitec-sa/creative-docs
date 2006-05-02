//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public class StructuredDataField
	{
		public StructuredDataField(string name, INamedType type)
		{
			this.name = name;
			this.type = type;
		}

		public string							Name
		{
			get
			{
				return this.name;
			}
		}
		public INamedType						Type
		{
			get
			{
				return this.type;
			}
		}

		
		private string name;
		private INamedType type;
	}
}
