//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Exceptions
{
	/// <summary>
	/// Exception signalant que la base de données existe déjà (ou toute
	/// autre structure).
	/// </summary>
	
	[System.Serializable]
	
	public class ExistsException : GenericException
	{
		public ExistsException() : base (DbAccess.Empty)
		{
		}
		
		public ExistsException(string message) : base (DbAccess.Empty, message)
		{
		}
		
		public ExistsException(DbAccess db_access) : base (db_access)
		{
		}
		
		public ExistsException(DbAccess db_access, string message) : base (db_access, message)
		{
		}
		
		public ExistsException(DbAccess db_access, string message, System.Exception inner_exception) : base (db_access, message, inner_exception)
		{
		}
		
		
		#region ISerializable Members
		protected ExistsException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
