//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Exceptions
{
	/// <summary>
	/// Exception signalant que la base de données a détecté une erreur de syntaxe
	/// (au niveau des arguments de connexion, dans une requête SQL, dans un nom
	/// de table, etc.)
	/// </summary>
	
	[System.Serializable]
	
	public class MissingTransactionException : GenericException
	{
		public MissingTransactionException(DbAccess db_access) : base (db_access)
		{
		}
		
		public MissingTransactionException(DbAccess db_access, string message) : base (db_access, message)
		{
		}
		
		public MissingTransactionException(DbAccess db_access, string message, System.Exception inner_exception) : base (db_access, message, inner_exception)
		{
		}
		
		
		#region ISerializable Members
		protected MissingTransactionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
			
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
