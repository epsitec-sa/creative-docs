//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

/************************* This class is thread safe *************************/

namespace Epsitec.Cresus.DataLayer.Context
{
	/// <summary>
	/// The <c>DataContextPool</c> class manages a collection of logically associated contexts.
	/// Usually, all contexts belong to a same higher level context (such as an application
	/// instance). This class is thread safe.
	/// See also <see cref="DataContext"/>.
	/// </summary>
	public sealed class DataContextPool : IEnumerable<DataContext>, System.IDisposable
	{
		/// <summary>
		/// Builds a new empty <c>DataContext</c>.
		/// </summary>
		internal DataContextPool()
		{
			this.dataContexts = new Dictionary<long, DataContext> ();

			DataContextPool.Register (this);
		}

		/// <summary>
		/// Adds a <see cref="DataContext"/> to the pool.
		/// </summary>
		/// <param name="dataContext">The <see cref="DataContext"/> to add.</param>
		/// <returns><c>true</c> if the <see cref="DataContext"/> was not present in the pool, <c>false</c> if it was.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dataContext"/> is null.</exception>
		internal void Add(DataContext dataContext)
		{
			dataContext.ThrowIfNull ("dataContext");

			string name = dataContext.Name ?? "";
			System.Diagnostics.Debug.WriteLine ("Added context #" + dataContext.UniqueId + ", " + name);

			lock (this.dataContexts)
			{
				this.dataContexts.Add (dataContext.UniqueId, dataContext);
			}
		}

		/// <summary>
		/// Removes a <see cref="DataContext"/> from the pool.
		/// </summary>
		/// <param name="dataContext">The <see cref="DataContext"/> to remove.</param>
		/// <returns><c>true</c> if the <see cref="DataContext"/> was present in the pool, <c>false</c> if it was not.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dataContext"/> is null.</exception>
		internal bool Remove(DataContext dataContext)
		{
			dataContext.ThrowIfNull ("dataContext");

			string name = dataContext.Name ?? "";
			System.Diagnostics.Debug.WriteLine ("Removed context #" + dataContext.UniqueId + ", " + name);

			lock (this.dataContexts)
			{
				return this.dataContexts.Remove (dataContext.UniqueId);
			}
		}

		/// <summary>
		/// Tells whether the pool contains a given <see cref="DataContext"/>.
		/// </summary>
		/// <param name="dataContext">The <see cref="DataContext"/> whose presence in the pool to check.</param>
		/// <returns><c>true</c> if <paramref name="dataContext"/> is in the pool, false if it is not.</returns>
		public bool Contains(DataContext dataContext)
		{
			dataContext.ThrowIfNull ("dataContext");

			lock (this.dataContexts)
			{
				return this.dataContexts.ContainsKey (dataContext.UniqueId);
			}
		}

		/// <summary>
		/// Finds the <see cref="DataContext"/> which is responsible for <paramref name="entity"/> or
		/// null if there is none.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="DataContext"/> to find.</param>
		/// <returns>The <see cref="DataContext"/> responsible for <paramref name="entity"/>.</returns>
		public DataContext FindDataContext(AbstractEntity entity)
		{
			if (entity == null)
			{
				return null;
			}

			return this.FindDataContext (DataContextPool.GetDataContextId (entity));
		}

		/// <summary>
		/// Finds the data context with the specified ID. The search is done only in the
		/// current pool. To search across all pools, use the static version, <see cref="DataContextPool.GetDataContext(long)"/>.
		/// </summary>
		/// <param name="contextId">The context id.</param>
		/// <returns></returns>
		public DataContext FindDataContext(long contextId)
		{
			if (contextId < 0)
			{
				return null;
			}

			DataContext context = null;

			lock (this.dataContexts)
			{
				this.dataContexts.TryGetValue (contextId, out context);
			}

			return context;
		}

		/// <summary>
		/// Finds the <see cref="EntityKey"/> which represents the storage location of
		/// <paramref name="entity"/> in the database, if there is any.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="EntityKey"/> to find.</param>
		/// <returns>The <see cref="EntityKey"/> of <paramref name="entity"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is null.</exception>
		public EntityKey? FindEntityKey(AbstractEntity entity)
		{
			if (entity == null)
			{
				return null;
			}

			DataContext context = this.FindDataContext (entity);

			return (context == null) ? null : context.GetNormalizedEntityKey (entity);
		}

		/// <summary>
		/// Applies a sequence of <see cref="AbstractSynchronizationJob"/> provided by a
		/// <see cref="DataContext"/> to all the <see cref="DataContext"/> managed by this instance,
		/// if <paramref name="dataContext"/> is also managed by this instance.
		/// </summary>
		/// <param name="dataContext">The <see cref="DataContext"/> whose modifications to synchronize.</param>
		/// <param name="jobs">The sequence of <see cref="AbstractSynchronizationJob"/> to synchronize.</param>
		internal void Synchronize(DataContext dataContext, IEnumerable<AbstractSynchronizationJob> jobs)
		{
			if (this.Contains (dataContext))
			{
				List<DataContext> otherDataContexts = this
					.Where (d => d != dataContext)
					.Where (d => !d.IsDisposed)
					.ToList ();

				foreach (AbstractSynchronizationJob job in jobs)
				{
					foreach (DataContext otherDataContext in otherDataContexts)
					{
						job.Synchronize (otherDataContext);
					}
				}
			}
		}

		/// <summary>
		/// Compares two entities and returns <c>true</c> if they refer to the same database key
		/// or if they are the same memory instance.
		/// </summary>
		/// <param name="a">The reference entity.</param>
		/// <param name="b">The other entity.</param>
		/// <returns><c>true</c> if both entities refer to the same database key; otherwise, <c>false</c>.</returns>
		public bool AreEqualDatabaseInstances(AbstractEntity a, AbstractEntity b)
		{
			if (a == b)
			{
				return true;
			}

			var keyA = this.FindEntityKey (a);
			var keyB = this.FindEntityKey (b);

			if (!keyA.HasValue && a != null)
			{
				return false;
			}

			if (!keyB.HasValue && b != null)
			{
				return false;
			}

			return keyA == keyB;
		}

		#region IEnumerable<DataContext> Members


		/// <summary>
		/// Iterates over all the <see cref="DataContext"/> managed by this pool. The iteration
		/// itself is thread safe, as it happens on a copy of the pool's collection.
		/// </summary>
		/// <returns>The <see cref="DataContext"/> managed by this pool.</returns>
		public IEnumerator<DataContext> GetEnumerator()
		{
			lock (this.dataContexts)
			{
				return this.dataContexts.Values.ToList ().GetEnumerator ();
			}
		}


		#endregion

		#region IEnumerable Members


		/// <summary>
		///Iterates over all the <see cref="DataContext"/> managed by this pool.
		/// </summary>
		/// <returns>The <see cref="DataContext"/> managed by this pool.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator ();
		}


		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			DataContextPool.Unregister (this);
		}

		#endregion


		public static EntityKey? GetEntityKey(AbstractEntity entity)
		{
			DataContext context = DataContextPool.GetDataContext (entity);

			return (context == null) ? null : context.GetNormalizedEntityKey (entity);
		}


		/// <summary>
		/// Gets the <see cref="DataContext"/> associated with the entity, if any.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>The <see cref="DataContext"/> if there is one; otherwise, <c>null</c>.</returns>
		public static DataContext GetDataContext(AbstractEntity entity)
		{
			lock (DataContextPool.pools)
			{
				long dataContextId = DataContextPool.GetDataContextId (entity);

				return DataContextPool.GetDataContext (dataContextId);
			}
		}

		/// <summary>
		/// Gets the data context with the specified ID. The search is done in a thread safe
		/// manner, across all known pools.
		/// </summary>
		/// <param name="contextId">The context id.</param>
		/// <returns>The data context instance, if found; otherwise, <c>null</c>.</returns>
		private static DataContext GetDataContext(long contextId)
		{
			foreach (var pool in DataContextPool.pools)
			{
				var context = pool.FindDataContext (contextId);
				
				if (context != null)
				{
					return context;
				}
			}

			return null;
		}

		/// <summary>
		/// Gets the ID of the <see cref="DataContext"/> associated with the entity, if any.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>The ID of the associated <see cref="DataContext"/> if there is one; otherwise, <c>-1</c>.</returns>
		private static long GetDataContextId(AbstractEntity entity)
		{
			if ((entity == null) ||
				(entity.DataContextId.HasValue == false))
			{
				return -1;
			}
			else
			{
				return entity.DataContextId.Value;
			}
		}

		private static void Register(DataContextPool pool)
		{
			lock (DataContextPool.pools)
			{
				DataContextPool.pools.Add (pool);
			}
		}

		private static void Unregister(DataContextPool pool)
		{
			lock (DataContextPool.pools)
			{
				DataContextPool.pools.Remove (pool);
			}
		}

		private static WeakList<DataContextPool> pools = new WeakList<DataContextPool> ();

		
		private readonly Dictionary<long, DataContext> dataContexts;		//	collection of associated DataContext instances
	}
}
