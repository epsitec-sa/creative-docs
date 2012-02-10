//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Factories;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business
{
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

		public static BusinessContext GetCurrentContext(AbstractEntity entity)
		{
			var dataContext = DataContextPool.GetDataContext (entity);

			System.Diagnostics.Debug.Assert (dataContext != null, "Entity does not belong to any data context");
			
			var data = Epsitec.Cresus.Core.Library.CoreApp.FindCurrentAppSessionComponent<CoreData> ();
			var pool = data.GetComponent<BusinessContextPool> ();

			return pool.pool.Where (x => x.DataContext == dataContext).FirstOrDefault ();
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
				context.SaveChanges ();
			}

			this.Host.DisposeDataContext (dataContext);
		}

		internal void Add(BusinessContext context)
		{
			this.pool.Add (context);
		}

		internal void Remove(BusinessContext context)
		{
			this.pool.Remove (context);
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

		private readonly List<BusinessContext> pool;
	}
}
