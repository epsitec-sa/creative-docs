//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Runtime.Serialization.Formatters.Binary;

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// Classe de base pour les requêtes.
	/// </summary>
	
	[System.Serializable]
	
	public abstract class Base : System.Runtime.Serialization.ISerializable
	{
		protected Base(Type type)
		{
			this.SetupType (type);
		}
		
		
		public Type								Type
		{
			get
			{
				return this.type;
			}
		}
		
		
		#region ISerializable Members
		protected Base(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
//-			this.db_access = (DbAccess) info.GetValue ("db_access", typeof (DbAccess));
		}
		
		public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
//-			info.AddValue ("db_access", this.db_access);
		}
		#endregion
		
		public static byte[] SerializeToMemory(Base request)
		{
			BinaryFormatter        formatter = new BinaryFormatter ();
			System.IO.MemoryStream stream    = new System.IO.MemoryStream ();
			
			formatter.Serialize (stream, request);
			stream.Close ();
			
			return stream.ToArray ();
		}
		
		public static Base DeserializeFromMemory(byte[] buffer)
		{
			BinaryFormatter        formatter = new BinaryFormatter ();
			System.IO.MemoryStream stream    = new System.IO.MemoryStream(buffer, 0, buffer.Length, false, false);
			
			Base request = formatter.Deserialize (stream) as Base;
			
			stream.Close ();
			
			return request;
		}
		
		protected void SetupType(Type type)
		{
			this.type = type;
		}
		
		
		private Type							type = Type.Unknown;
	}
}
