//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// L'exception ResourceException est lev�e lorsqu'un probl�me survient avec
	/// l'acc�s aux ressources.
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
		
		public ResourceException(string message, System.Exception inner_exception) : base (message, inner_exception)
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
