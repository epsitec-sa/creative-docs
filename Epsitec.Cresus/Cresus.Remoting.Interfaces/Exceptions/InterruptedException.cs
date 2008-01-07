//	Copyright � 2004-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting.Exceptions
{
	/// <summary>
	/// La classe InterruptedException permet d'interrompre une op�ration.
	/// </summary>
	
	[System.Serializable]
	
	public class InterruptedException : GenericException
	{
		public InterruptedException()
		{
		}
		
		
		#region ISerializable Members
		protected InterruptedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
