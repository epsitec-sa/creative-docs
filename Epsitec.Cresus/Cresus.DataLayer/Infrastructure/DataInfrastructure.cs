﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Services;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.ImportExport;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Schema;

using System.Collections;
using System.Collections.Generic;

using System.IO;

using System.Linq;

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
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");

			if (dbInfrastructure.ContainsValue (DataInfrastructure.DbInfrastructureProperty))
			{
				throw new System.ArgumentException ("DbInfrastructure already attached to another DataInfrastructure object", "dbInfrastructure");
			}

			this.dbInfrastructure = dbInfrastructure;
			this.dbInfrastructure.SetValue (DataInfrastructure.DbInfrastructureProperty, this);
			this.SchemaEngine = new SchemaEngine (this.dbInfrastructure);
			this.DataContextPool = new DataContextPool ();
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
		/// The <see cref="DataContextPool"/> associated with this instance.
		/// </summary>
		public DataContextPool DataContextPool
		{
			get;
			private set;
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
		/// Gets the <see cref="SchemaEngine"/> associated with this instance.
		/// </summary>
		internal SchemaEngine SchemaEngine
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets an information stored in the database.
		/// </summary>
		/// <param name="key">The key defining the information whose value to get.</param>
		/// <returns>The value of the information corresponding to the given key.</returns>
		public string GetDatabaseInfo(string key)
		{
			return this.dbInfrastructure.InfoManager.GetInfo (key);
		}

		/// <summary>
		/// Sets the information corresponding to the given key to the given value in the database.
		/// </summary>
		/// <param name="key">The key defining the information whose value to set.</param>
		/// <param name="value">The new value of the information.</param>
		public void SetDatabaseInfo(string key, string value)
		{
			this.dbInfrastructure.InfoManager.SetInfo (key, value);
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

		public DataContext CreateDataContext(bool enableNullVirtualization = false)
		{
			DataContext dataContext = new DataContext (this, enableNullVirtualization);

			this.DataContextPool.Add (dataContext);

			return dataContext;
		}

		public bool DeleteDataContext(DataContext dataContext)
		{
			dataContext.Dispose ();

			return this.DataContextPool.Remove (dataContext);
		}

		public bool ContainsDataContext(DataContext dataContext)
		{
			return this.DataContextPool.Contains (dataContext);
		}

		/// <summary>
		/// Does the real job of disposing this instance.
		/// </summary>
		/// <param name="disposing">Tells whether this method is called by the Dispose() method or by the destructor.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.dbInfrastructure.ClearValue (DataInfrastructure.DbInfrastructureProperty);

				foreach (DataContext dataContext in this.DataContextPool.ToList ())
				{
					this.DeleteDataContext (dataContext);
				}
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
		/// Checks that the given <see cref="AbstractEntity"/> given by <typeparamref name="TEntity"/>
		/// is properly defined in the database.
		/// </summary>
		/// <typeparam name="TEntity">The type of the <see cref="AbstractEntity"/> to check.</typeparam>
		/// <returns><c>true</c> if the type of the <see cref="AbstractEntity"/> is properly defined in the database, <c>false</c> if it is not.</returns>		
		/// <exception cref="System.InvalidOperationException">If this instance is not connected.</exception>
		public bool CheckSchema<TEntity>() where TEntity : AbstractEntity, new ()
		{
			this.AssertIsConnected ();

			return this.SchemaEngine.SchemaBuilder.CheckSchema (EntityInfo<TEntity>.GetTypeId ());
		}

		public bool CheckSchema(Druid entityId)
		{
			this.AssertIsConnected ();

			return this.SchemaEngine.SchemaBuilder.CheckSchema (entityId);
		}

		/// <summary>
		/// Creates and persists the schema describing an <see cref="AbstractEntity"/> to the
		/// database. The schema for the <see cref="AbstractEntity"/> is created with all the
		/// dependencies.
		/// </summary>
		/// <typeparam name="TEntity">The type of the <see cref="AbstractEntity"/> whose schema to create.</typeparam>
		/// <exception cref="System.InvalidOperationException">If this instance is not connected.</exception>
		public void CreateSchema<TEntity>()
			where TEntity : AbstractEntity, new ()
		{
			this.AssertIsConnected ();

			this.SchemaEngine.CreateSchema<TEntity> ();
		}

		public void CreateSchema(Druid entityId)
		{
			this.AssertIsConnected ();

			this.SchemaEngine.CreateSchema (entityId);
		}

		/// <summary>
		/// Exports a set of <see cref="AbstractEntity"/> to an xml file. The set of exported
		/// <see cref="AbstractEntity"/> is defined by the given <see cref="AbstractEntity"/> and
		/// the given predicate. The algorithm will explore the graph of the <see cref="AbstractEntity"/>
		/// by recursively following all the relations that target an <see cref="AbstractEntity"/>
		/// which satisfies the given predicate, starting with the given <see cref="AbstractEntity"/>.
		/// The resulting subset of the graph will be exported.
		/// </summary>
		/// <param name="file">The file that will contain the exported data.</param>
		/// <param name="dataContext">The <see cref="DataContext"/> that owns the given <see cref="AbstractEntity"/>.</param>
		/// <param name="entities">The collection of entities which will be exported.</param>
		/// <param name="predicate">The predicate used to determine whether to export an <see cref="AbstractEntity"/> or not.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="file"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dataContext"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="dataContext"/> has not been created by this instance.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entities"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="entities"/> contains any entity foreign to <paramref name="dataContext"/>.</exception>
		public void Export(FileInfo file, DataContext dataContext, IEnumerable<AbstractEntity> entities, System.Func<AbstractEntity, bool> predicate = null)
		{
			file.ThrowIfNull ("file");
			dataContext.ThrowIfNull ("dataContext");
			dataContext.ThrowIf (d => d.DataInfrastructure != this, "dataContext has not been created by this instance.");
			entities.ThrowIfNull ("entity");
			entities.ThrowIf (e => e.Any (x => dataContext.IsForeignEntity (x)), "entity is not owned by dataContext.");

			if (predicate == null)
			{
				predicate = x => true;
			}

			ImportExportManager.Export (file, dataContext, entities, predicate);
		}

		/// <summary>
		/// Imports a set of <see cref="AbstractEntity"/> from an xml file that has been written by
		/// the <see cref="DataInfrastructure.Export"/> method.
		/// </summary>
		/// <param name="file">The file containing the data to import.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="file"/> is <c>null</c>.</exception>
		public void Import(FileInfo file)
		{
			file.ThrowIfNull ("file");

			ImportExportManager.Import (file, this);
		}


		/// <summary>
		/// Exports the Epsitec data stored in the database.
		/// </summary>
		/// <param name="file">The file in which to write the data.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="file"/> is <c>null</c>.</exception>
		public void ExportEpsitecData(FileInfo file)
		{
			file.ThrowIfNull ("file");

			EpsitecEntitySerializer.Export (file, this.dbInfrastructure);
		}


		/// <summary>
		/// Replaces the Epsitec data stored whitin the database by the one stored in the given file.
		/// </summary>
		/// <param name="file">The file from which to read the data.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="file"/> is <c>null</c>.</exception>
		public void ImportEpsitecData(FileInfo file)
		{
			file.ThrowIfNull ("file");

			DbId connectionId = new DbId (this.ConnectionInformation.ConnectionId);
			DbLogEntry dbLogEntry = this.DbInfrastructure.Logger.CreateLogEntry (connectionId);

			EpsitecEntitySerializer.CleanDatabase (file, this.dbInfrastructure);
			EpsitecEntitySerializer.Import (file, this.dbInfrastructure, dbLogEntry);
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
