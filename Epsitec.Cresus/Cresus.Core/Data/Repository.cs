//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.Core.Data
{
	public abstract class Repository
	{
		protected Repository(DataContext dataContext)
		{
			this.dataContext = dataContext;
			this.dataBrowser = new DataBrowser (this.dataContext);
		}


		public DataContext DataContext
		{
			get
			{
				return this.dataContext;
			}
		}


		protected T CreateExample<T>() where T : AbstractEntity, new()
		{
			return new T ();
		}


		protected IEnumerable<T> GetEntitiesByExample<T>(T example)
			where T : AbstractEntity
		{
			return this.dataBrowser.GetByExample<T> (example, true);
		}


		protected IEnumerable<T> GetEntitiesByExample<T>(T example, EntityConstrainer constrainer)
			where T : AbstractEntity
		{
			return this.dataBrowser.GetByExample<T> (example, constrainer, true);
		}


		protected IEnumerable<T> GetEntitiesByExample<T>(T example, int index, int count)
			where T : AbstractEntity
		{
			return this.dataBrowser.GetByExample<T> (example, true).Skip (index + 1).Take (count);
		}


		protected IEnumerable<T> GetEntitiesByExample<T>(T example, EntityConstrainer constrainer, int index, int count)
			where T : AbstractEntity
		{
			return this.dataBrowser.GetByExample<T> (example, constrainer, true).Skip (index + 1).Take (count);
		}


		protected IEnumerable<AbstractEntity> GetReferencers(AbstractEntity target)
		{
			return this.dataBrowser.GetReferencers (target, true).Select (result => result.Item1);
		}


		private readonly DataContext dataContext;
		private readonly DataBrowser dataBrowser;
	}
}
