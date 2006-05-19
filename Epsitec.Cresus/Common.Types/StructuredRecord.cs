//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public class StructuredRecord : IStructuredTypeProvider
	{
		public StructuredRecord(StructuredType type)
		{
			this.type = type;
		}

		public StructuredType StructuredType
		{
			get
			{
				if (this.type == null)
				{
					//	TODO: implement
					throw new System.NotImplementedException ();
					//return new DynamicStructuredType (this);
				}
				else
				{
					return this.type;
				}
			}
		}

		#region IStructuredTypeProvider Members

		IStructuredType IStructuredTypeProvider.GetStructuredType()
		{
			return this.StructuredType;
		}

		#endregion
		
		
		private StructuredType type;
	}
}
