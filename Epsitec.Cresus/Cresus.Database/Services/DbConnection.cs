namespace Epsitec.Cresus.Database.Services
{


	/// <summary>
	/// The <c>DbConnection</c> class is an immutable object that represents a row of data in the
	/// CR_CONNECTION table in the database.
	/// </summary>
	public sealed class DbConnection
	{


		/// <summary>
		/// Creates a new instance of <see cref="DbConnection"/>.
		/// </summary>
		/// <param name="id">The <see cref="DbId"/> of the connection.</param>
		/// <param name="identity">The identity that describes who established the connection.</param>
		/// <param name="status">The status of the connection.</param>
		/// <param name="establishmentTime">The instant when the connection was established.</param>
		/// <param name="RefreshTime">The instant when the connection was refreshed.</param>
		internal DbConnection(DbId id, string identity, DbConnectionStatus status, System.DateTime establishmentTime, System.DateTime RefreshTime)
		{
			this.Id = id;
			this.Identity = identity;
			this.Status = status;
			this.EstablishmentTime = establishmentTime;
			this.RefreshTime = RefreshTime;
		}


		/// <summary>
		/// Gets the <see cref="DbId"/> of the connection represented by this instance.
		/// </summary>
		public DbId Id
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the identity of the connection represented by this instance.
		/// </summary>
		public string Identity
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the <see cref="DbConnectionStatus"/> of the connection represented by this
		/// instance.
		/// </summary>
		public DbConnectionStatus Status
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the time at which the connection represented by this instance has been established.
		/// </summary>
		public System.DateTime EstablishmentTime
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the last time at which the connection represented by this instance has been refreshed.
		/// </summary>
		public System.DateTime RefreshTime
		{
			get;
			private set;
		}


	}


}
