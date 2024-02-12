//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Exceptions
{
	/// <summary>
	/// The <c>DeadLockException</c> exception is raised when a deadlock is detected.
	/// </summary>

	public sealed class DeadLockException : GenericException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DeadLockException"/> class.
		/// </summary>
		public DeadLockException()
			: base (DbAccess.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DeadLockException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public DeadLockException(string message)
			: base (DbAccess.Empty, message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DeadLockException"/> class.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		public DeadLockException(DbAccess databaseAccess)
			: base (databaseAccess)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DeadLockException"/> class.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		/// <param name="message">The message.</param>
		public DeadLockException(DbAccess databaseAccess, string message)
			: base (databaseAccess, message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DeadLockException"/> class.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public DeadLockException(DbAccess databaseAccess, string message, System.Exception innerException)
			: base (databaseAccess, message, innerException)
		{
		}
	}
}
