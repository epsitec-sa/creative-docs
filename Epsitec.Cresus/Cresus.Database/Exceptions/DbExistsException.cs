//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

namespace Epsitec.Cresus.Database.Exceptions
{
	/// <summary>
	/// Exception signalant que la base de donn�es existe d�j�.
	/// </summary>
	
	[System.Serializable]
	
	public class ExistsException : GenericException, System.Runtime.Serialization.ISerializable
	{
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
