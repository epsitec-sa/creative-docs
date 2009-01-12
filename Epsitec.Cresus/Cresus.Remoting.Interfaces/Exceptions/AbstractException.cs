//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting.Exceptions
{
	/// <summary>
	/// The <c>GenericException</c> exception is the base class for the various
	/// remoting exceptions.
	/// </summary>
	
	[System.Serializable]
	
	public abstract class AbstractException : System.Exception
	{
		public AbstractException()
		{
		}
		
		
		#region ISerializable Members
		protected AbstractException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
