//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// L'exception ResourceException est levée lorsqu'un problème survient avec
	/// l'accès aux ressources.
	/// </summary>
	
	[System.Serializable]
	
	public class ResourceException : System.ApplicationException, System.Runtime.Serialization.ISerializable
	{
		public ResourceException()
		{
		}
		
		public ResourceException(string message) : base (message)
		{
		}
		
		public ResourceException(string message, System.Exception innerException) : base (message, innerException)
		{
		}
		
		
		#region ISerializable Members
		protected ResourceException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
