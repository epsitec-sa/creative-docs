//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Debug
{
	/// <summary>
	/// La classe AssertFailedException permet de signaler l'échec d'une
	/// assertion.
	/// </summary>
	
	[System.Serializable]
	
	public class AssertFailedException : System.ApplicationException
	{
		public AssertFailedException()
		{
		}
		
		public AssertFailedException(string message) : this()
		{
			this.message = message;
		}
		
		
		public override string					Message
		{
			get
			{
				if (this.message == null)
				{
					return "Assert failed.";
				}
				else
				{
					return string.Format ("Assert failed: {0}", this.message);
				}
			}
		}
		
		
		#region Support for ISerializable
		public AssertFailedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
			this.message = info.GetString ("AssertMessage");
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
			
			info.AddValue ("AssertMessage", this.message);
		}
		#endregion
		
		private string							message;
	}
}
