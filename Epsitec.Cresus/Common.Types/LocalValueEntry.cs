//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// LocalValueEntry.
	/// </summary>
	public struct LocalValueEntry
	{
		public LocalValueEntry(Property property, object value)
		{
			this.property = property;
			this.value    = value;
		}
		public LocalValueEntry(KeyValuePair<Property, object> pair)
		{
			this.property = pair.Key;
			this.value = pair.Value;
		}
		
		public Property							Property
		{
			get
			{
				return this.property;
			}
		}
		public object							Value
		{
			get
			{
				return this.value;
			}
		}
		
		private Property						property;
		private object							value;
	}
}
