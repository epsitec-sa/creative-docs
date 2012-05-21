//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Exceptions
{
	/// <summary>
	/// The <c>ExistsException</c> exception is raised when a database or an object already
	/// exists.
	/// </summary>
	
	[System.Serializable]
	
	public sealed class ExistsException : GenericException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ExistsException"/> class.
		/// </summary>
		public ExistsException() : base (DbAccess.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ExistsException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public ExistsException(string message) : base (DbAccess.Empty, message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ExistsException"/> class.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		public ExistsException(DbAccess databaseAccess)
			: base (databaseAccess)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ExistsException"/> class.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		/// <param name="message">The message.</param>
		public ExistsException(DbAccess databaseAccess, string message)
			: base (databaseAccess, message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ExistsException"/> class.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public ExistsException(DbAccess databaseAccess, string message, System.Exception innerException)
			: base (databaseAccess, message, innerException)
		{
		}
		
		
		#region ISerializable Members

		private ExistsException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
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
