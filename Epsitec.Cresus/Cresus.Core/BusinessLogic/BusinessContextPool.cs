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

		public DataContext DataContext
		{
			get
			{
				return this.context;
			}
		}
		public bool IsEmpty
		{
			get
			{
				return this.businessContexts.Count == 0;
			}
		}

		public void Add(BusinessContext context)
		{
			if (this.IsEmpty)
			{
				this.context = CoreProgram.Application.MainWindowOrchestrator.DefaultDataContext;
			}

			this.businessContexts.Add (context);
		}

		public void Remove(BusinessContext context)
		{
			this.businessContexts.Remove (context);

			if (this.IsEmpty)
			{
				this.context = null;
			}
		}

		private readonly List<BusinessContext> businessContexts;
		private readonly CoreData data;
		private DataContext context;
	}
}
