//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting.Exceptions
{
	/// <summary>
	/// La classe GenericException sert de base à toutes les exeptions du
	/// namespace Cresus.Remoting.Exceptions.
	/// </summary>
	
	[System.Serializable]
	
	public class GenericException : System.Exception
	{
		public GenericException()
		{
		}
		
		
		#region ISerializable Members
		protected GenericException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
