//	Copyright � 2005-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Debug
{
	/// <summary>
	/// La classe FailureException permet de signaler un �chec g�n�ral.
	/// </summary>
	
	[System.Serializable]
	
	public class FailureException : System.ApplicationException
	{
		public FailureException()
		{
		}
		
		public FailureException(string message) : base (message)
		{
		}
		
		
		#region Support for ISerializable
		public FailureException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
