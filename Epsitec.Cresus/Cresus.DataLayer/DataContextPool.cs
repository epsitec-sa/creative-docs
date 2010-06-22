//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.DataLayer
{
	public sealed class DataContextPool : IEnumerable<DataContext>
	{
		private DataContextPool()
		{
			this.dataContexts = new HashSet<DataContext> ();
		}

		public bool Add(DataContext context)
		{
			System.Diagnostics.Debug.WriteLine ("Added context #" + context.UniqueId);
			return this.dataContexts.Add (context);
		}

		public bool Remove(DataContext context)
		{
			System.Diagnostics.Debug.WriteLine ("Removed context #" + context.UniqueId);
			return this.dataContexts.Remove (context);
		}

		public DataContext FindDataContext(AbstractEntity entity)
		{
			return this.dataContexts.FirstOrDefault (context => context.Contains (entity));
		}

		public EntityKey FindEntityKey(AbstractEntity entity)
		{
			if (entity == null)
            {
				return EntityKey.Empty;
            }

			var context = this.FindDataContext (entity);

			if (context == null)
			{
				return EntityKey.Empty;
			}
			else
			{
				return context.GetEntityKey (entity);
			}
		}

		#region IEnumerable<DataContext> Members

		public IEnumerator<DataContext> GetEnumerator()
		{
			return this.dataContexts.GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator ();
		}

		#endregion

		public static DataContextPool Instance
		{
			get
			{
				return DataContextPool.instance;
			}
		}

		private static readonly DataContextPool instance = new DataContextPool ();
		private readonly HashSet<DataContext> dataContexts;
	}
}
