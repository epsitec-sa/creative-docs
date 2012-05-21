//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Exceptions
{
	/// <summary>
	/// The <c>ConflictingException</c> exception is raised when a conflict is detected
	/// during an update.
	/// </summary>

	[System.Serializable]

	public sealed class ConflictingException : GenericException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConflictingException"/> class.
		/// </summary>
		public ConflictingException()
			: base (DbAccess.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConflictingException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public ConflictingException(string message)
			: base (DbAccess.Empty, message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConflictingException"/> class.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		public ConflictingException(DbAccess databaseAccess)
			: base (databaseAccess)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConflictingException"/> class.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		/// <param name="message">The message.</param>
		public ConflictingException(DbAccess databaseAccess, string message)
			: base (databaseAccess, message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConflictingException"/> class.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public ConflictingException(DbAccess databaseAccess, string message, System.Exception innerException)
			: base (databaseAccess, message, innerException)
		{
		}


		#region ISerializable Members

		private ConflictingException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
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
