//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business
{
	public sealed class BusinessContextPool
	{
		public BusinessContextPool(CoreData data)
		{
			this.data = data;
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
			return this.data.CreateDataContext (name);
		}

		internal void DisposeDataContext(BusinessContext context, DataContext dataContext)
		{
			if (context.ContainsChanges ())
			{
				context.SaveChanges ();
			}

			this.data.DisposeDataContext (dataContext);
		}

		internal void Add(BusinessContext context)
		{
			this.pool.Add (context);
		}

		internal void Remove(BusinessContext context)
		{
			this.pool.Remove (context);
		}

		private readonly List<BusinessContext> pool;
		private readonly CoreData data;
	}
}
