//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Exceptions
{
	/// <summary>
	/// Exception signalant un problème de dead-lock.
	/// </summary>
	
	[System.Serializable]
	
	public class DeadLockException : GenericException
	{
		public DeadLockException() : base (DbAccess.Empty)
		{
		}
		
		public DeadLockException(string message) : base (DbAccess.Empty, message)
		{
		}
		
		public DeadLockException(DbAccess db_access) : base (db_access)
		{
		}
		
		public DeadLockException(DbAccess db_access, string message) : base (db_access, message)
		{
		}
		
		public DeadLockException(DbAccess db_access, string message, System.Exception inner_exception) : base (db_access, message, inner_exception)
		{
		}
		
		
		#region ISerializable Members
		protected DeadLockException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
