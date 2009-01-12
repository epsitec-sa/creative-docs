//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting.Exceptions
{
	/// <summary>
	/// The <c>InvalidOperationException</c> exception is thrown when an invalid
	/// operation is attempted.
	/// </summary>
	
	[System.Serializable]
	
	public class InvalidOperationException : AbstractException
	{
		public InvalidOperationException()
		{
		}
		
		
		#region ISerializable Members
		protected InvalidOperationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
