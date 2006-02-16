//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The ValueEventArgs class provides a value to the ValueEventHandler.
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
