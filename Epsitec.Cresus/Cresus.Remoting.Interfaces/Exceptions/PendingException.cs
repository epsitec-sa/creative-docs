//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting.Exceptions
{
	/// <summary>
	/// La classe PendingException permet de signaler qu'une opération est
	/// toujours en cours d'exécution.
	/// </summary>
	
	[System.Serializable]
	
	public class PendingException : GenericException
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
