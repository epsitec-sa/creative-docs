//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting.Exceptions
{
	/// <summary>
	/// The <c>PendingException</c> exception is raised when an operation is
	/// still executing.
	/// </summary>
	
	[System.Serializable]
	
	public class PendingException : AbstractException
	{
		public PendingException()
		{
		}
		
		
		#region ISerializable Members
		protected PendingException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
