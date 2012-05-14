//	Copyright � 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.Core.Data;

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	public class BrowserListContext : System.IDisposable
	{
		public BrowserListContext(DataContext dataContext)
		{
			this.dataContext = dataContext;
		}


		public DataSetAccessor Accessor
		{
			get
			{
				return this.accessor;
			}
		}

		public void SetAccessor(DataSetAccessor collectionAccessor)
		{
			if (this.accessor != null)
			{
				this.accessor.Dispose ();
				this.accessor = null;
			}

			this.accessor = collectionAccessor;
		}

		public FormattedText GetDisplayText(AbstractEntity entity)
		{
			if (entity == null)
			{
				return CollectionTemplate.DefaultEmptyText;
			}
			else
			{
				var text = entity.GetCompactSummary ();

				if (text.IsNullOrEmpty)
				{
					return CollectionTemplate.DefaultEmptyText;
				}
				else
				{
					return text;
				}
			}
		}

		
		#region IDisposable Members

		public void Dispose()
		{
			this.SetAccessor (null);
		}

		#endregion

		
		internal AbstractEntity ResolveEntity(EntityKey? entityKey)
		{
			return this.dataContext.ResolveEntity (entityKey);
		}



		internal EntityKey GetEntityKey(AbstractEntity entity)
		{
			if (entity == null)
			{
				throw new System.ArgumentNullException ("entity");
			}

			var key = this.dataContext.GetNormalizedEntityKey (entity);

			if (key == null)
			{
				throw new System.ArgumentException ("Cannot resolve entity");
			}

			return key.Value;
		}



		private readonly DataContext			dataContext;
		private DataSetAccessor					accessor;
	}
}
