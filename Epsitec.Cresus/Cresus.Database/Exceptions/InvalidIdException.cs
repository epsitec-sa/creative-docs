//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Exceptions
{
	/// <summary>
	/// Exception signalant que l'ID de la clef n'est pas valide dans
	/// le contexte courant (soit parce que la clef n'a pas été initialisée,
	/// soit parce que c'est une clef temporaire).
	/// </summary>
	
	[System.Serializable]
	
	public class InvalidIdException : GenericException
	{
		public InvalidIdException() : base (DbAccess.Empty)
		{
		}
		
		public InvalidIdException(string message) : base (DbAccess.Empty, message)
		{
		}
		
		public InvalidIdException(string message, System.Exception inner_exception) : base (DbAccess.Empty, message, inner_exception)
		{
		}
		
		public InvalidIdException(DbAccess db_access) : base (db_access)
		{
		}
		
		public InvalidIdException(DbAccess db_access, string message) : base (db_access, message)
		{
		}
		
		public InvalidIdException(DbAccess db_access, string message, System.Exception inner_exception) : base (db_access, message, inner_exception)
		{
		}
		
		
		#region ISerializable Members
		protected InvalidIdException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
			
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
