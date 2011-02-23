//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business
{
	public sealed class BusinessContextPool : CoreDataComponent
	{
		private BusinessContextPool(CoreData data)
			: base (data)
		{
			this.pool = new List<BusinessContext> ();
		}

		public bool IsEmpty
		{
			get
			{
				return this.pool.Count == 0;
			}
		}

		public BusinessContext CreateBusinessContext()
		{
			return new BusinessContext (this);
		}

		public void DisposeBusinessContext(BusinessContext context)
		{
			if (this.pool.Contains (context))
			{
				context.Dispose ();

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
			return this.Data.CreateDataContext (name);
		}

		internal void DisposeDataContext(BusinessContext context, DataContext dataContext)
		{
			if (context.ContainsChanges ())
			{
				context.SaveChanges ();
			}

			this.Data.DisposeDataContext (dataContext);
		}

		internal void Add(BusinessContext context)
		{
			this.pool.Add (context);
		}

		internal void Remove(BusinessContext context)
		{
			this.pool.Remove (context);
		}

		#region BusinessContextPoolFactory Class

		public sealed class BusinessContextPoolFactory : ICoreDataComponentFactory
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
