using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;


namespace Epsitec.Cresus.DataLayer.Infrastructure
{


	/// <summary>
	/// The <c>Connection</c> class is an immutable object that represents a connection of an application
	/// user to the database.
	/// </summary>
	public sealed class Connection
	{


		/// <summary>
		/// Creates a new instance of <see cref="Connection"/>.
		/// </summary>
		/// <param name="id">The <see cref="DbId"/> of the connection.</param>
		/// <param name="identity">The identity that describes the user who established the connection.</param>
		/// <param name="status">The status of the connection.</param>
		/// <param name="establishmentTime">The instant when the connection was established.</param>
		/// <param name="RefreshTime">The instant when the connection was refreshed.</param>
		internal Connection(DbId id, string identity, ConnectionStatus status, System.DateTime establishmentTime, System.DateTime RefreshTime)
		{
			id.ThrowIf (e => e.IsEmpty, "id cannot be empty");
			identity.ThrowIfNullOrEmpty ("identity");

			this.id = id;
			this.identity = identity;
			this.status = status;
			this.establishmentTime = establishmentTime;
			this.refreshTime = RefreshTime;
		}


		/// <summary>
		/// Gets the <see cref="DbId"/> of the connection represented by this instance.
		/// </summary>
		internal DbId Id
		{
			get
			{
				return this.id;
			}
		}


		/// <summary>
		/// Gets the identity of the connection represented by this instance.
		/// </summary>
		public string Identity
		{
			get
			{
				return this.identity;
			}
		}


		/// <summary>
		/// Gets the <see cref="ConnectionStatus"/> of the connection represented by this
		/// instance.
		/// </summary>
		public ConnectionStatus Status
		{
			get
			{
				return this.status;
			}
		}


		/// <summary>
		/// Gets the time at which the connection represented by this instance has been established.
		/// </summary>
		public System.DateTime EstablishmentTime
		{
			get
			{
				return this.establishmentTime;
			}
		}


		/// <summary>
		/// Gets the last time at which the connection represented by this instance has been refreshed.
		/// </summary>
		public System.DateTime RefreshTime
		{
			get
			{
				return this.refreshTime;
			}
		}


		private readonly DbId id;


		private readonly string identity;


		private readonly ConnectionStatus status;


		private readonly System.DateTime establishmentTime;


		private readonly System.DateTime refreshTime;


	}


}
