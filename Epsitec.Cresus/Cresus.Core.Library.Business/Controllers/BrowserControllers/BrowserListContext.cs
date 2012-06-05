//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.BigList;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Data;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	public class BrowserListContext : System.IDisposable
	{
		public BrowserListContext()
		{
			this.itemProvider = new BrowserListItemProvider (this);
			this.itemMapper   = new BrowserListItemMapper ();
			this.itemRenderer = new BrowserListItemRenderer (this);
		}


		public DataSetAccessor					Accessor
		{
			get
			{
				return this.accessor;
			}
		}

		public DataContext						IsolatedDataContext
		{
			get
			{
				return this.accessor.IsolatedDataContext;
			}
		}

		public BrowserListItemProvider			ItemProvider
		{
			get
			{
				return this.itemProvider;
			}
		}

		public BrowserListItemMapper			ItemMapper
		{
			get
			{
				return this.itemMapper;
			}
		}

		public BrowserListItemRenderer			ItemRenderer
		{
			get
			{
				return this.itemRenderer;
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
			
			this.itemProvider.Reset ();
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
			return this.IsolatedDataContext.ResolveEntity (entityKey);
		}



		internal EntityKey GetEntityKey(AbstractEntity entity)
		{
			if (entity == null)
			{
				throw new System.ArgumentNullException ("entity");
			}

			var key = this.IsolatedDataContext.GetNormalizedEntityKey (entity);

			if (key == null)
			{
				throw new System.ArgumentException ("Cannot resolve entity");
			}

			return key.Value;
		}



		private DataSetAccessor					accessor;
		
		private BrowserListItemProvider			itemProvider;
		private BrowserListItemMapper			itemMapper;
		private BrowserListItemRenderer			itemRenderer;
	}
}
