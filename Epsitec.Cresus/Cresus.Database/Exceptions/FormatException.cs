//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Exceptions
{
	/// <summary>
	/// L'exception FormatException représente une erreur de format (type invalide,
	/// etc.) qui n'est pas encore forcément lié à une base.
	/// </summary>
	
	[System.Serializable]
	
	public class FormatException : GenericException
	{
		public FormatException() : base (DbAccess.Empty)
		{
		}
		
		public FormatException(string message) : base (DbAccess.Empty, message)
		{
		}
		
		public FormatException(string message, System.Exception inner_exception) : base (DbAccess.Empty, message, inner_exception)
		{
		}
		
		public FormatException(DbAccess db_access, string message) : base (db_access, message)
		{
		}
		
		public FormatException(DbAccess db_access, string message, System.Exception inner_exception) : base (db_access, message, inner_exception)
		{
		}
		
		
		#region ISerializable Members
		protected FormatException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
