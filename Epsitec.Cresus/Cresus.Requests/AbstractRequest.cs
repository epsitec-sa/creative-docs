//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		
		protected void SetupRequestType(RequestType type)
		{
			this.type = type;
		}
		
		
		private RequestType						type = RequestType.Unknown;
	}
}
