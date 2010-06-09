//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.DataLayer
{
	public class DataContextPool
	{
		public DataContextPool()
		{
			this.dataContexts = new HashSet<DataContext> ();
		}

		public DataContext FindDataContext(AbstractEntity entity)
		{
			return this.dataContexts.FirstOrDefault (context => context.Contains (entity));
		}

		private readonly HashSet<DataContext> dataContexts;
	}
}
