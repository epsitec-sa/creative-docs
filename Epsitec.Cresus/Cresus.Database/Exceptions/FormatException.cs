//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Exceptions
{
	/// <summary>
	/// The <c>FormatException</c> represents a format error related to the
	/// database layer. It is similar to <see cref="System.FormatException"/>.
	/// </summary>
	
	[System.Serializable]
	
	public sealed class FormatException : GenericException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FormatException"/> class.
		/// </summary>
		public FormatException() : base (DbAccess.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FormatException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public FormatException(string message) : base (DbAccess.Empty, message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FormatException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public FormatException(string message, System.Exception innerException) : base (DbAccess.Empty, message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FormatException"/> class.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		/// <param name="message">The message.</param>
		public FormatException(DbAccess databaseAccess, string message)
			: base (databaseAccess, message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FormatException"/> class.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public FormatException(DbAccess databaseAccess, string message, System.Exception innerException)
			: base (databaseAccess, message, innerException)
		{
		}
		
		
		#region ISerializable Members

		private FormatException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
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
