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


		protected EntityType CreateExample<EntityType>() where EntityType : AbstractEntity, new()
		{
			return new EntityType ();
		}


		protected IEnumerable<EntityType> GetEntitiesByExample<EntityType>(EntityType example) where EntityType : AbstractEntity
		{
			return this.dataBrowser.GetByExample<EntityType> (example, true);
		}


		protected IEnumerable<EntityType> GetEntitiesByExample<EntityType>(EntityType example, int index, int count) where EntityType : AbstractEntity
		{
			return this.dataBrowser.GetByExample<EntityType> (example, true).Skip (index + 1).Take (count);
		}


		protected IEnumerable<AbstractEntity> GetReferencers(AbstractEntity target)
		{
			return this.dataBrowser.GetReferencers (target, true).Select (result => result.Item1);
		}


		private readonly DataContext dataContext;
		private readonly DataBrowser dataBrowser;
	}
}
