//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Exceptions
{
	/// <summary>
	/// Exception signalant que l'accès en lecture seule ne permet pas
	/// de réaliser l'opération.
	/// </summary>
	
	[System.Serializable]
	
	public class ReadOnlyException : GenericException
	{
		public ReadOnlyException(DbAccess db_access) : base (db_access)
		{
		}
		
		public ReadOnlyException(DbAccess db_access, string message) : base (db_access, message)
		{
		}
		
		public ReadOnlyException(DbAccess db_access, string message, System.Exception inner_exception) : base (db_access, message, inner_exception)
		{
		}
		
		
		#region ISerializable Members
		protected ReadOnlyException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
			
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
