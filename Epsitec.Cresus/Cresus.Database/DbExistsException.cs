//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// Exception signalant que la base de données existe déjà.
	/// </summary>
	
	[System.Serializable]
	
	public class DbExistsException : DbException, System.Runtime.Serialization.ISerializable
	{
		public DbExistsException(DbAccess db_access) : base (db_access)
		{
		}
		
		public DbExistsException(DbAccess db_access, string message) : base (db_access, message)
		{
		}
		
		public DbExistsException(DbAccess db_access, string message, System.Exception inner_exception) : base (db_access, message, inner_exception)
		{
		}
		
		
		#region ISerializable Members
		protected DbExistsException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
		
		void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
