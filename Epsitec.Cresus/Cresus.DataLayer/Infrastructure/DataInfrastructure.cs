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
		/// This method is called periodically in order to notify the database that this
		/// instance of the application is still up and running.
		/// </summary>
		public void KeepAlive()
		{
			System.Diagnostics.Debug.Assert (this.connectionInformation != null);
			System.Diagnostics.Debug.WriteLine ("KeepAlive pulsed");

			//	TODO: ...
		}

		/// <summary>
		/// Opens the high-level connection with the database: this will create a
		/// new <see cref="ConnectionInformation"/>.
		/// </summary>
		/// <param name="identity">The user/machine identity.</param>
		public void OpenConnection(string identity)
		{
			System.Diagnostics.Debug.Assert (this.connectionInformation == null);
			
			//	TODO: do real work here...
			
			this.connectionInformation = new ConnectionInformation ();
		}

		public void CloseConnection()
		{
			System.Diagnostics.Debug.Assert (this.connectionInformation != null);

			//	TODO: do real work here...

			this.connectionInformation = null;
		}

		public void RefreshConnectionInformation()
		{
			if (this.connectionInformation == null)
			{
				return;
			}

			//	TODO: do real work here...
		}

		
		
		public bool TryCreateLockTransaction(IEnumerable<string> lockNames, out LockTransaction lockTransaction)
		{
			// TODO Get the real user name.
			// Marc

			string userName = this.GetConnectionName ();

			return LockTransaction.TryCreateLockTransaction (this.dbInfrastructure, lockNames, userName, out lockTransaction);
		}


		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.dbInfrastructure.ClearValue (DataInfrastructure.DbInfrastructureProperty);
			}

			base.Dispose (disposing);
		}

		private string GetConnectionName()
		{
			System.Diagnostics.Debug.Assert (this.connectionInformation != null);
			System.Diagnostics.Debug.Assert (this.connectionInformation.Status == ConnectionStatus.Active);

			return this.connectionInformation.ConnectionId.ToString (System.Globalization.CultureInfo.InvariantCulture);
		}
		
		
		private static DependencyProperty DbInfrastructureProperty = DependencyProperty<DataInfrastructure>.RegisterAttached ("DataInfrastructure", typeof (DataInfrastructure));

		private readonly DbInfrastructure dbInfrastructure;
		private ConnectionInformation connectionInformation;
	}
}
