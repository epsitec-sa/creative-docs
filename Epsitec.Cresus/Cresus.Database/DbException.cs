//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// Classe de base pour les exceptions de l'interface avec la base de données.
	/// </summary>
	
	[System.Serializable]
	
	public class DbException : System.ApplicationException, System.Runtime.Serialization.ISerializable
	{
		public DbException(DbAccess db_access)
		{
			this.db_access = db_access;
		}
		
		public DbException(DbAccess db_access, string message) : base (message)
		{
			this.db_access = db_access;
		}
		
		public DbException(DbAccess db_access, string message, System.Exception inner_exception) : base (message, inner_exception)
		{
			this.db_access = db_access;
		}
		
		
		public DbAccess							DbAccess
		{
			get { return this.db_access; }
		}
		
		
		#region ISerializable Members
		protected DbException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
			this.db_access = (DbAccess) info.GetValue ("db_access", typeof (DbAccess));
		}
		
		void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue ("db_access", this.db_access);
			base.GetObjectData (info, context);
		}
		#endregion
		
		protected DbAccess						db_access;
	}
}
