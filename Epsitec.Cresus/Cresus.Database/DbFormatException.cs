//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'exception DbFormatException représente une erreur de format (type invalide,
	/// etc.) qui n'est pas encore forcément lié à une base.
	/// </summary>
	
	[System.Serializable]
	
	public class DbFormatException : DbException, System.Runtime.Serialization.ISerializable
	{
		public DbFormatException() : base (DbAccess.Empty)
		{
		}
		
		public DbFormatException(string message) : base (DbAccess.Empty, message)
		{
		}
		
		public DbFormatException(string message, System.Exception inner_exception) : base (DbAccess.Empty, message, inner_exception)
		{
		}
		
		public DbFormatException(DbAccess db_access, string message) : base (db_access, message)
		{
		}
		
		public DbFormatException(DbAccess db_access, string message, System.Exception inner_exception) : base (db_access, message, inner_exception)
		{
		}
		
		
		#region ISerializable Members
		protected DbFormatException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
		
		void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
