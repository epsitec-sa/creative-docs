//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Requests
{
	/// <summary>
	/// Classe de base pour les requêtes.
	/// </summary>
	
	[System.Serializable]
	
	public abstract class Base : System.Runtime.Serialization.ISerializable
	{
		protected Base()
		{
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
	}
}
