//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Exceptions
{
	/// <summary>
	/// Exception signalant un conflit pendant une mise à jour.
	/// </summary>
	
	[System.Serializable]
	
	public class ConflictingException : GenericException
	{
		public ConflictingException() : base (DbAccess.Empty)
		{
		}
		
		public ConflictingException(string message) : base (DbAccess.Empty, message)
		{
		}
		
		public ConflictingException(DbAccess db_access) : base (db_access)
		{
		}
		
		public ConflictingException(DbAccess db_access, string message) : base (db_access, message)
		{
		}
		
		public ConflictingException(DbAccess db_access, string message, System.Exception inner_exception) : base (db_access, message, inner_exception)
		{
		}
		
		
		#region ISerializable Members
		protected ConflictingException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
