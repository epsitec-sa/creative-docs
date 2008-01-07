//	Copyright � 2005-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Debug
{
	/// <summary>
	/// La classe AssertFailedException permet de signaler l'�chec d'une
	/// assertion.
	/// </summary>
	
	[System.Serializable]
	
	public class AssertFailedException : FailureException
	{
		public AssertFailedException()
		{
		}
		
		public AssertFailedException(string message) : base (string.Format ("Assert failed: {0}", message))
		{
		}
		
		
		#region Support for ISerializable
		public AssertFailedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
