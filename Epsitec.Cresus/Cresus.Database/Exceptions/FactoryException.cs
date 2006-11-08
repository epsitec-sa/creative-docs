//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Exceptions
{
	/// <summary>
	/// The <c>FactoryException</c> exception is raised by the database factory
	/// classes when they encounter internal errors.
	/// </summary>
	
	[System.Serializable]
	
	public sealed class FactoryException : GenericException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FactoryException"/> class.
		/// </summary>
		public FactoryException() : base (DbAccess.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FactoryException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public FactoryException(string message) : base (DbAccess.Empty, message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FactoryException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public FactoryException(string message, System.Exception innerException) : base (DbAccess.Empty, message, innerException)
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
