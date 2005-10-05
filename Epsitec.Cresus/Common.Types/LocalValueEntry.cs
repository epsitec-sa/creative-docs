//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// LocalValueEntry.
	/// </summary>
	public class LocalValueEntry
	{
		public LocalValueEntry(Property property, object value)
		{
			this.property = property;
			this.value    = value;
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
