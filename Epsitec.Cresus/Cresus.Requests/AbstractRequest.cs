//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Runtime.Serialization.Formatters.Binary;

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// Classe de base pour les requêtes.
	/// </summary>
	
	[System.Serializable]
	
	public abstract class AbstractRequest : System.Runtime.Serialization.ISerializable
	{
		protected AbstractRequest(RequestType type)
		{
			this.SetupRequestType (type);
		}
		
		
		public RequestType						RequestType
		{
			get
			{
				return this.type;
			}
		}
		
		
		public abstract void Execute(ExecutionEngine engine);
		
		#region ISerializable Members
		protected AbstractRequest(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
//-			this.db_access = (DbAccess) info.GetValue ("db_access", typeof (DbAccess));
		}
		
		public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
//-			info.AddValue ("db_access", this.db_access);
		}
		#endregion
		
		public static byte[] SerializeToMemory(AbstractRequest request)
		{
			BinaryFormatter        formatter = new BinaryFormatter ();
			System.IO.MemoryStream stream    = new System.IO.MemoryStream ();
			
			formatter.Serialize (stream, request);
			stream.Close ();
			
			return stream.ToArray ();
		}
		
		public static AbstractRequest DeserializeFromMemory(byte[] buffer)
		{
			BinaryFormatter        formatter = new BinaryFormatter ();
			System.IO.MemoryStream stream    = new System.IO.MemoryStream(buffer, 0, buffer.Length, false, false);
			
			AbstractRequest request = formatter.Deserialize (stream) as AbstractRequest;
			
			stream.Close ();
			
			return request;
		}
		
		
		protected void SetupRequestType(RequestType type)
		{
			this.type = type;
		}
		
		
		private RequestType						type = RequestType.Unknown;
	}
}
