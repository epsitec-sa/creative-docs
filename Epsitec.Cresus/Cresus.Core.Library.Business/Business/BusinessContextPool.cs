//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Library;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business
{
	/// <summary>
	/// The <c>BusinessContextPool</c> class manages the collection of <see cref="BusinessContext"/>
	/// instances.
	/// </summary>
	public sealed class BusinessContextPool : CoreDataComponent
	{
		internal BusinessContextPool(CoreData data)
			: base (data)
		{
			this.pool = new List<BusinessContext> ();
		}

		public bool								IsEmpty
		{
			get
			{
				return this.pool.Count == 0;
			}
		}


		/// <summary>
		/// Gets the <see cref="BusinessContext"/> to which the entity belongs, if there is
		/// one.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>The business context or <c>null</c>.</returns>
		public static BusinessContext GetCurrentContext(AbstractEntity entity)
		{
			var data = CoreApp.FindCurrentAppSessionComponent<CoreData> ();
			var that = data.GetComponent<BusinessContextPool> ();

			return that.FindContext (entity);
		}

		public BusinessContext FindContext(AbstractEntity entity)
		{
			var dataContext = DataContextPool.GetDataContext (entity);

			System.Diagnostics.Debug.Assert (dataContext != null, "Entity does not belong to any data context");

			//	The entity belongs to a data context. Now find the business context in the
			//	current session, which uses the same data context :

			return this.pool.Where (x => x.DataContext == dataContext).FirstOrDefault ();
		}


		internal void DisposeBusinessContext(BusinessContext context)
		{
			if (this.pool.Contains (context))
			{
				context.DisposeFromPool ();

				System.Diagnostics.Debug.Assert (this.pool.Contains (context) == false);
			}
			else
			{
				throw new System.InvalidOperationException ("Context does not belong to the pool");
			}
		}

		internal DataContext CreateDataContext(BusinessContext context)
		{
			string name = string.Format ("BusinessContext #{0}", context.UniqueId);
			return this.Host.CreateDataContext (name);
		}

		internal void DisposeDataContext(BusinessContext context, DataContext dataContext)
		{
			if (context.ContainsChanges ())
			{
				context.SaveChanges (LockingPolicy.ReleaseLock);
			}

			this.Host.DisposeDataContext (dataContext);
		}

		internal void Add(BusinessContext context)
		{
			this.pool.Add (context);
			this.OnChanged (new BusinessContextEventArgs (BusinessContextOperation.Add, context));
		}

		internal void Remove(BusinessContext context)
		{
			this.pool.Remove (context);
			this.OnChanged (new BusinessContextEventArgs (BusinessContextOperation.Remove, context));
		}

		#region Factory Class

		private sealed class Factory : ICoreDataComponentFactory
		{
			#region ICoreDataComponentFactory Members

			public bool CanCreate(CoreData data)
			{
				return true;
			}

			public CoreDataComponent Create(CoreData data)
			{
				return new BusinessContextPool (data);
			}

			public System.Type GetComponentType()
			{
				return typeof (BusinessContextPool);
			}

			#endregion
		}

		#endregion

		private void OnChanged(BusinessContextEventArgs e)
		{
			this.Changed.Raise (this, e);
		}
		
		public event EventHandler<BusinessContextEventArgs> Changed;				

		private readonly List<BusinessContext> pool;
	}
}
