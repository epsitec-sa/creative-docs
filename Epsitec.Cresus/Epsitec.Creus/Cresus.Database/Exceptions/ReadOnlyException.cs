//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Exceptions
{
	/// <summary>
	/// The <c>ReadOnlyException</c> exception is raised when an operation attempts
	/// to modify read-only data.
	///	</summary>

	[System.Serializable]

	public sealed class ReadOnlyException : GenericException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReadOnlyException"/> class.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		public ReadOnlyException(DbAccess databaseAccess)
			: base (databaseAccess)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReadOnlyException"/> class.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		/// <param name="message">The message.</param>
		public ReadOnlyException(DbAccess databaseAccess, string message)
			: base (databaseAccess, message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReadOnlyException"/> class.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public ReadOnlyException(DbAccess databaseAccess, string message, System.Exception innerException)
			: base (databaseAccess, message, innerException)
		{
		}


		#region ISerializable Members

		private ReadOnlyException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
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
