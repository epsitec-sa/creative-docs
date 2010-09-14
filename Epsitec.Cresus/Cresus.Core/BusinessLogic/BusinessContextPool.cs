//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.BusinessLogic
{
	public sealed class BusinessContextPool
	{
		public BusinessContextPool(CoreData data)
		{
			this.data = data;
			this.businessContexts = new List<BusinessContext> ();
		}

		public bool IsEmpty
		{
			get
			{
				return this.businessContexts.Count == 0;
			}
		}

		public BusinessContext CreateBusinessContext()
		{
			return new BusinessContext (this);
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
			this.businessContexts.Add (context);
		}

		internal void Remove(BusinessContext context)
		{
			this.businessContexts.Remove (context);
		}

		private readonly List<BusinessContext> businessContexts;
		private readonly CoreData data;
	}
}
