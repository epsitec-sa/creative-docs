//	Copyright � 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Exceptions
{
	/// <summary>
	/// The <c>InvalidIdException</c> exception is thrown when the key ID is not
	/// valid in a given context, either because the key is not properly initialized
	/// or because a temporary key is specified where a real key is expected.
	/// </summary>

	[System.Serializable]

	public sealed class InvalidIdException : GenericException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidIdException"/> class.
		/// </summary>
		public InvalidIdException()
			: base (DbAccess.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidIdException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public InvalidIdException(string message)
			: base (DbAccess.Empty, message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidIdException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public InvalidIdException(string message, System.Exception innerException)
			: base (DbAccess.Empty, message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidIdException"/> class.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		public InvalidIdException(DbAccess databaseAccess)
			: base (databaseAccess)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidIdException"/> class.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		/// <param name="message">The message.</param>
		public InvalidIdException(DbAccess databaseAccess, string message)
			: base (databaseAccess, message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidIdException"/> class.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public InvalidIdException(DbAccess databaseAccess, string message, System.Exception innerException)
			: base (databaseAccess, message, innerException)
		{
		}


		#region ISerializable Members
		
		protected InvalidIdException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base (info, context)
		{
		}

		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}

		#endregion
	}
}
