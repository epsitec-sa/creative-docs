//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Exceptions
{
	/// <summary>
	/// Classe de base pour les exceptions de l'interface avec la base de données.
	/// </summary>
	
	[System.Serializable]
	
	public class GenericException : System.ApplicationException, System.Runtime.Serialization.ISerializable
	{
		public GenericException(DbAccess db_access)
		{
			this.db_access = db_access;
		}
		
		public GenericException(DbAccess db_access, string message) : base (message)
		{
			this.db_access = db_access;
		}
		
		public GenericException(DbAccess db_access, string message, System.Exception inner_exception) : base (message, inner_exception)
		{
			this.db_access = db_access;
		}
		
		
		public DbAccess							DbAccess
		{
			get { return this.db_access; }
		}
		
		
		#region ISerializable Members
		protected GenericException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
			this.db_access = (DbAccess) info.GetValue ("db_access", typeof (DbAccess));
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue ("db_access", this.db_access);
			base.GetObjectData (info, context);
		}
		#endregion
		
		protected DbAccess						db_access;
	}
}
