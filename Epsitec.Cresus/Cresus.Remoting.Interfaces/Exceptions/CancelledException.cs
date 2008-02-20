//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting.Exceptions
{
	/// <summary>
	/// La classe CancelledException permet de signaler qu'une opération a été
	/// annulée.
	/// </summary>
	
	[System.Serializable]
	
	public class CancelledException : GenericException
	{
		public CancelledException()
		{
		}
		
		
		#region ISerializable Members
		protected CancelledException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
