//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Types;

using Epsitec.Common.Support.Extensions;

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
		public UidGenerator CreateUidGenerator(string name, IEnumerable<UidSlot> slots)
		{
			UidGenerator.CreateUidGenerator (this.dbInfrastructure, name, slots);
			
			return UidGenerator.GetUidGenerator (this.dbInfrastructure, name);
		}

		/// <summary>
		/// Deletes a generator for unique ids from the database.
		/// </summary>
		/// <param name="name">The name of the generator.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		public void DeleteUidGenerator(string name)
		{
			UidGenerator.DeleteUidGenerator (this.dbInfrastructure, name);
		}

		/// <summary>
		/// Tells whether a generator for unique ids exists in the database.
		/// </summary>
		/// <param name="name">The name of the generator.</param>
		/// <returns><c>true</c> if a generator with <paramref name="name"/> exists in the database, <c>false</c> if there aren't.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		public bool UidGeneratorExists(string name)
		{
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
		public UidGenerator GetUidGenerator(string name)
		{
			return UidGenerator.GetUidGenerator (this.dbInfrastructure, name);
		}
		
		/// <summary>
		/// Opens the high-level connection with the database: this will create a
		/// new <see cref="ConnectionInformation"/>.
		/// </summary>
		/// <param name="identity">The user/machine identity.</param>
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

		public void CloseConnection()
		{
			if (this.connectionInformation == null)
			{
				throw new System.InvalidOperationException ("This instance is not connected.");
			}

			this.connectionInformation.Close ();
		}
		
		/// <summary>
		/// This method is called periodically in order to notify the database that this
		/// instance of the application is still up and running.
		/// </summary>
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
				
		public void RefreshConnectionInformation()
		{
			if (this.connectionInformation == null)
			{
				throw new System.InvalidOperationException ("This instance has never been connected.");
			}

			this.connectionInformation.RefreshStatus ();
		}

		public bool AreAllLocksAvailable(IEnumerable<string> lockNames)
		{
			this.AssertIsConnected ();

			return LockTransaction.AreAllLocksAvailable (this.dbInfrastructure, this.connectionInformation.ConnectionId, lockNames);
		}
				
		public LockTransaction CreateLockTransaction(IEnumerable<string> lockNames)
		{
			this.AssertIsConnected ();

			return new LockTransaction (this.dbInfrastructure, this.connectionInformation.ConnectionId, lockNames);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.dbInfrastructure.ClearValue (DataInfrastructure.DbInfrastructureProperty);
			}

			base.Dispose (disposing);
		}


		private void AssertIsConnected()
		{
			if (this.connectionInformation == null || this.connectionInformation.Status != ConnectionStatus.Open)
			{
				throw new System.InvalidOperationException ("This instance is not connected.");
			}
		}

		private string GetConnectionName()
		{
			System.Diagnostics.Debug.Assert (this.connectionInformation != null);
			System.Diagnostics.Debug.Assert (this.connectionInformation.Status == ConnectionStatus.Open);

			return this.connectionInformation.ConnectionId.ToString (System.Globalization.CultureInfo.InvariantCulture);
		}
				
		private static DependencyProperty DbInfrastructureProperty = DependencyProperty<DataInfrastructure>.RegisterAttached ("DataInfrastructure", typeof (DataInfrastructure));

		private readonly DbInfrastructure dbInfrastructure;
		private ConnectionInformation connectionInformation;
	}
}
