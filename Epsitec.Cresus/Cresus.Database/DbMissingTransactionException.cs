//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// Exception signalant que la base de données a détecté une erreur de syntaxe
	/// (au niveau des arguments de connexion, dans une requête SQL, dans un nom
	/// de table, etc.)
	/// </summary>
	
	[System.Serializable]
	
	public class DbMissingTransactionException : DbException, System.Runtime.Serialization.ISerializable
	{
		public DbMissingTransactionException(DbAccess db_access) : base (db_access)
		{
		}
		
		public DbMissingTransactionException(DbAccess db_access, string message) : base (db_access, message)
		{
		}
		
		public DbMissingTransactionException(DbAccess db_access, string message, System.Exception inner_exception) : base (db_access, message, inner_exception)
		{
		}
		
		
		#region ISerializable Members
		protected DbMissingTransactionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
			
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
