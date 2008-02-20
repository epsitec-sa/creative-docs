//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>ValueEventArgs</c> class provides a value for events which are used
	/// to notify insertions and removals, for instance.
	/// </summary>
	public class ValueEventArgs : EventArgs
	{
		public ValueEventArgs(object value)
		{
			this.value = value;
		}
		
		public object							Value
		{
			get
			{
				return this.value;
			}
		}
		
		private object							value;
	}
}
