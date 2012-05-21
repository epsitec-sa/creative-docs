//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Exceptions
{
	/// <summary>
	/// The <c>MissingTransactionException</c> exception is raised when a command is
	/// executed outside of a properly initialized transaction.
	/// </summary>

	[System.Serializable]

	public sealed class MissingTransactionException : GenericException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MissingTransactionException"/> class.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		public MissingTransactionException(DbAccess databaseAccess)
			: base (databaseAccess)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MissingTransactionException"/> class.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		/// <param name="message">The message.</param>
		public MissingTransactionException(DbAccess databaseAccess, string message)
			: base (databaseAccess, message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MissingTransactionException"/> class.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public MissingTransactionException(DbAccess databaseAccess, string message, System.Exception innerException)
			: base (databaseAccess, message, innerException)
		{
		}


		#region ISerializable Members

		private MissingTransactionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
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
