//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer.Infrastructure;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (DataInfrastructure))]

namespace Epsitec.Cresus.DataLayer.Infrastructure
{
	/// <summary>
	/// The <c>DataInfrastructure</c> class provides an high level access to the data stored in the
	/// database.
	/// </summary>
	public sealed class DataInfrastructure : DependencyObject
	{
		/// <summary>
		/// Creates a new instance of <c>DataInfrastructure</c>.
		/// </summary>
		/// <param name="dbInfrastructure">The <see cref="DbInfrastructure"/> used to communicate to the Database.</param>
		public DataInfrastructure(DbInfrastructure dbInfrastructure)
		{
			if (dbInfrastructure.ContainsValue (DataInfrastructure.DbInfrastructureProperty))
			{
				throw new System.ArgumentException ("DbInfrastructure already attached to another DataInfrastructure object", "dbInfrastructure");
			}

			this.dbInfrastructure = dbInfrastructure;
			this.dbInfrastructure.SetValue (DataInfrastructure.DbInfrastructureProperty, this);
		}
		
		/// <summary>
		/// The <see cref="DbInfrastructure"/> object used to communicate with the database.
		/// </summary>
		public DbInfrastructure DbInfrastructure
		{
			get
			{
				return this.dbInfrastructure;
			}
		}

		/// <summary>
		/// The <see cref="ConnectionInformation"/> object that describes the connection of this
		/// instance with the database.
		/// </summary>
		public ConnectionInformation ConnectionInformation
		{
			get
			{
				return this.connectionInformation;
			}
		}
		
		/// <summary>
		/// Creates a new generator for unique ids in the database.
		/// </summary>
		/// <param name="name">The name of the generator.</param>
		/// <param name="slots">The definition of the slots of the generator.</param>
		/// <returns>The <see cref="UidGenerator"/>.</returns>
		/// <remarks>
		/// The slots are defined as a sequence of minimum and maximum values, which must be locally
		/// and globally consistent.
		/// </remarks>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="slots"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="slots"/> is empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="slots"/> contains negative elements.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="slots"/> contains slots with inconsistent bounds.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="slots"/> contains overlapping slots.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not connected.</exception>
		public UidGenerator CreateUidGenerator(string name, IEnumerable<UidSlot> slots)
		{
			this.AssertIsConnected ();

			UidGenerator.CreateUidGenerator (this.dbInfrastructure, name, slots);
			
			return UidGenerator.GetUidGenerator (this.dbInfrastructure, name);
		}

		/// <summary>
		/// Deletes a generator for unique ids from the database.
		/// </summary>
		/// <param name="name">The name of the generator.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not connected.</exception>
		public void DeleteUidGenerator(string name)
		{
			this.AssertIsConnected ();

			UidGenerator.DeleteUidGenerator (this.dbInfrastructure, name);
		}

		/// <summary>
		/// Tells whether a generator for unique ids exists in the database.
		/// </summary>
		/// <param name="name">The name of the generator.</param>
		/// <returns><c>true</c> if a generator with <paramref name="name"/> exists in the database, <c>false</c> if there aren't.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not connected.</exception>
		public bool UidGeneratorExists(string name)
		{
			this.AssertIsConnected ();

			return UidGenerator.UidGeneratorExists (this.dbInfrastructure, name);
		}

		/// <summary>
		/// Gets the <see cref="UidGenerator"/> object used to manipulate a generator of unique ids
		/// in the database.
		/// </summary>
		/// <param name="name">The name of the generator.</param>
		/// <returns>The <see cref="UidGenerator"/> object.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.Exception">If the requested <see cref="UidGenerator"/> does not exists.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not connected.</exception>
		public UidGenerator GetUidGenerator(string name)
		{
			this.AssertIsConnected ();

			return UidGenerator.GetUidGenerator (this.dbInfrastructure, name);
		}
		
		/// <summary>
		/// Opens the high-level connection with the database: this will create a
		/// new <see cref="ConnectionInformation"/>.
		/// </summary>
		/// <param name="identity">The user/machine identity.</param>
		/// <exception cref="System.InvalidOperationException">If the connection is already open.</exception>
		public void OpenConnection(string identity)
		{
			if (this.connectionInformation != null)
			{
				ConnectionStatus status = this.connectionInformation.Status;

				if (status == ConnectionStatus.NotYetOpen || status == ConnectionStatus.Open)
				{
					throw new System.InvalidOperationException ("This instance is already connected.");
				}
			}

			this.connectionInformation = new ConnectionInformation (this.dbInfrastructure, identity);
			this.connectionInformation.Open ();
		}

		/// <summary>
		/// Closes the high level connection with the database.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">If the connection is not open.</exception>
		public void CloseConnection()
		{
			if (this.connectionInformation == null)
			{
				throw new System.InvalidOperationException ("This instance is not connected.");
			}

			this.connectionInformation.Close ();
		}
		
		/// <summary>
		/// Notifies the database that this instance of the application is still up and running and
		/// clean dirty data related to inactive connections in the database.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">If the connection is not open.</exception>
		public void KeepConnectionAlive()
		{
			if (this.connectionInformation == null)
			{
				throw new System.InvalidOperationException ("This instance is not connected.");
			}

			System.Diagnostics.Debug.WriteLine ("KeepAlive pulsed");

			this.connectionInformation.KeepAlive ();

#if false
			// HACK: disabled dead connection recycling -- this is a real annoyance when debugging with multiple instances running

			ConnectionInformation.InterruptDeadConnections (this.dbInfrastructure, System.TimeSpan.FromSeconds (30));
			LockTransaction.RemoveInactiveLocks (this.dbInfrastructure);
#endif
		}
				
		/// <summary>
		/// Refreshes the data about the connection of this instance.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">If the connection has never been opened.</exception>
		public void RefreshConnectionInformation()
		{
			if (this.connectionInformation == null)
			{
				throw new System.InvalidOperationException ("This instance has never been connected.");
			}

			this.connectionInformation.RefreshStatus ();
		}

		/// <summary>
		/// Tells whether all the given locks are available or not.
		/// </summary>
		/// <param name="lockNames">The name of the locks.</param>
		/// <returns><c>true</c> if all locks are available, <c>false</c> if at least one is not.</returns>
		public bool AreAllLocksAvailable(IEnumerable<string> lockNames)
		{
			this.AssertIsConnected ();

			return LockTransaction.AreAllLocksAvailable (this.dbInfrastructure, this.connectionInformation.ConnectionId, lockNames);
		}
				
		/// <summary>
		/// Creates a new <see cref="LockTransaction"/> for the given locks.
		/// </summary>
		/// <param name="lockNames">The name of the locks to get.</param>
		/// <returns>The new <see cref="LockTransaction"/> object.</returns>
		public LockTransaction CreateLockTransaction(IEnumerable<string> lockNames)
		{
			this.AssertIsConnected ();

			return new LockTransaction (this.dbInfrastructure, this.connectionInformation.ConnectionId, lockNames);
		}

		/// <summary>
		/// Does the real job of disposing this instance.
		/// </summary>
		/// <param name="disposing">Tells wheteher this method is called by the Dispose() method or by the destructor.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.dbInfrastructure.ClearValue (DataInfrastructure.DbInfrastructureProperty);
			}

			base.Dispose (disposing);
		}
		
		/// <summary>
		/// Asserts that the connection of this instance is open and throws an
		/// <see cref="System.InvalidOperationException"/> otherwise.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">If this instance is not connected.</exception>
		private void AssertIsConnected()
		{
			if (this.connectionInformation == null || this.connectionInformation.Status != ConnectionStatus.Open)
			{
				throw new System.InvalidOperationException ("This instance is not connected.");
			}
		}
		
		/// <summary>
		/// Some obscure property :-P Ask Pierre if you want to know more about it...
		/// </summary>
		private static DependencyProperty DbInfrastructureProperty = DependencyProperty<DataInfrastructure>.RegisterAttached ("DataInfrastructure", typeof (DataInfrastructure));

		/// <summary>
		/// The <see cref="DbInfrastructure"/> object used by this instance to interact with the
		/// database.
		/// </summary>
		private readonly DbInfrastructure dbInfrastructure;
		
		/// <summary>
		/// The <see cref="ConnectionInformation"/> object that stores the connection data of this
		/// instance.
		/// </summary>
		private ConnectionInformation connectionInformation;
	}
}
