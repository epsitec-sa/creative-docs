//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

namespace Epsitec.Cresus.Database.Exceptions
{
	/// <summary>
	/// L'exception FactoryException est utilisée par les gestionnaires de données
	/// universels pour les erreurs internes qui leur sont propres.
	/// </summary>
	
	[System.Serializable]
	
	public class FactoryException : GenericException, System.Runtime.Serialization.ISerializable
	{
		public FactoryException() : base (DbAccess.Empty)
		{
		}
		
		public FactoryException(string message) : base (DbAccess.Empty, message)
		{
		}
		
		public FactoryException(string message, System.Exception inner_exception) : base (DbAccess.Empty, message, inner_exception)
		{
		}
		
		
		#region ISerializable Members
		protected FactoryException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
