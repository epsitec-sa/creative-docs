//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting.Exceptions
{
	/// <summary>
	/// La classe InvalidOperationException permet de signaler qu'une opération
	/// spécifiée n'est pas valide.
	/// </summary>
	
	[System.Serializable]
	
	public class InvalidOperationException : GenericException
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
