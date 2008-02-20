//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Exceptions
{
	/// <summary>
	/// The <c>GenericException</c> exception is raised when a database layer
	/// related exception occurs. This is the base class of other more specific
	/// exceptions.
	/// </summary>

	[System.Serializable]

	public class GenericException : System.Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GenericException"/> class.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		public GenericException(DbAccess databaseAccess)
		{
			this.dbAccess = databaseAccess;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericException"/> class.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		/// <param name="message">The message.</param>
		public GenericException(DbAccess databaseAccess, string message)
			: base (message)
		{
			this.dbAccess = databaseAccess;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericException"/> class.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public GenericException(DbAccess databaseAccess, string message, System.Exception innerException)
			: base (message, innerException)
		{
			this.dbAccess = databaseAccess;
		}


		/// <summary>
		/// Gets the database access related to the exception.
		/// </summary>
		/// <value>The database access.</value>
		public DbAccess DbAccess
		{
			get
			{
				return this.dbAccess;
			}
		}


		#region ISerializable Members

		protected GenericException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base (info, context)
		{
			this.dbAccess = (DbAccess) info.GetValue ("dbAccess", typeof (DbAccess));
		}

		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue ("dbAccess", this.dbAccess);
			base.GetObjectData (info, context);
		}

		#endregion

		protected DbAccess dbAccess;
	}
}
