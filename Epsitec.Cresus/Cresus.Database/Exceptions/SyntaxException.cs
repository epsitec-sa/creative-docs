//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Exceptions
{
	/// <summary>
	/// Exception signalant que la base de donn�es a d�tect� une erreur de syntaxe
	/// (au niveau des arguments de connexion, dans une requ�te SQL, dans un nom
	/// de table, etc.)
	/// </summary>
	
	[System.Serializable]
	
	public class SyntaxException : GenericException
	{
		public SyntaxException(DbAccess db_access) : base (db_access)
		{
		}
		
		public SyntaxException(DbAccess db_access, string message) : base (db_access, message)
		{
		}
		
		public SyntaxException(DbAccess db_access, string message, System.Exception inner_exception) : base (db_access, message, inner_exception)
		{
		}
		
		
		#region ISerializable Members
		protected SyntaxException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
			
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
