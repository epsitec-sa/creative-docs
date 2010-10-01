//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Exceptions
{
	/// <summary>
	/// The <c>IncompatibleDatabaseException</c> exception is raised when an empty database
	/// is being opened.
	/// </summary>
	[System.Serializable]
	public sealed class IncompatibleDatabaseException : GenericException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IncompatibleDatabaseException"/> class.
		/// </summary>
		public IncompatibleDatabaseException()
			: base (DbAccess.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IncompatibleDatabaseException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public IncompatibleDatabaseException(string message)
			: base (DbAccess.Empty, message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IncompatibleDatabaseException"/> class.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		public IncompatibleDatabaseException(DbAccess databaseAccess)
			: base (databaseAccess)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IncompatibleDatabaseException"/> class.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		/// <param name="message">The message.</param>
		public IncompatibleDatabaseException(DbAccess databaseAccess, string message)
			: base (databaseAccess, message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IncompatibleDatabaseException"/> class.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public IncompatibleDatabaseException(DbAccess databaseAccess, string message, System.Exception innerException)
			: base (databaseAccess, message, innerException)
		{
		}


		#region ISerializable Members

		private IncompatibleDatabaseException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
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
