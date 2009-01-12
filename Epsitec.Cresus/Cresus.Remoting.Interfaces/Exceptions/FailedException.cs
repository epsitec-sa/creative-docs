//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting.Exceptions
{
	/// <summary>
	/// La classe FailedException permet de signaler qu'une opération a échoué.
	/// </summary>
	
	[System.Serializable]
	
	public class FailedException : AbstractException
	{
		public FailedException()
		{
		}
		
		
		#region ISerializable Members
		protected FailedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
