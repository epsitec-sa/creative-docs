//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'exception DbFactoryException est utilisée par les gestionnaires de données
	/// universels pour les erreurs internes qui leur sont propres.
	/// </summary>
	
	[System.Serializable]
	
	public class DbFactoryException : DbException, System.Runtime.Serialization.ISerializable
	{
		public DbFactoryException() : base (DbAccess.Empty)
		{
		}
		
		public DbFactoryException(string message) : base (DbAccess.Empty, message)
		{
		}
		
		public DbFactoryException(string message, System.Exception inner_exception) : base (DbAccess.Empty, message, inner_exception)
		{
		}
		
		
		#region ISerializable Members
		protected DbFactoryException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
		
		void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
