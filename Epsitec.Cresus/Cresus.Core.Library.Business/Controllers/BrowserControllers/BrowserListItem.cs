//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers.DataAccessors;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	/// <summary>
	/// The <c>BrowserListItem</c> class represents one item in the browser list.
	/// </summary>
	public sealed class BrowserListItem
	{
		public BrowserListItem(AbstractEntity entity, EntityKey entityKey)
		{
			this.entity = entity;
			this.entityKey = entityKey;
		}


		public AbstractEntity					Entity
		{
			get
			{
				return this.entity;
			}
		}

		public EntityKey						EntityKey
		{
			get
			{
				return this.entityKey;
			}
		}

		
		internal EntityKey GetEntityKey(BrowserList list)
		{
			return list.GetEntityKey (this.entity);
		}

		internal Epsitec.Cresus.Database.DbKey GetRowKey(BrowserList list)
		{
			return this.GetEntityKey (list).RowKey;
		}

		internal FormattedText GetDisplayText(BrowserList list)
		{
			if (this.cachedText == null)
			{
				this.GenerateDisplayText (list);
			}

			return this.cachedText.Value;
		}

		internal void ClearCachedDisplayText()
		{
			this.cachedText = null;
		}


		private void GenerateDisplayText(BrowserList list)
		{
			this.cachedText = list.GenerateEntityDisplayText (this.entity);

			if (this.cachedText.Value.IsNullOrEmpty)
			{
				this.cachedText = CollectionTemplate.DefaultEmptyText;
			}
		}

		
		private readonly AbstractEntity			entity;
		private readonly EntityKey				entityKey;
		private FormattedText?					cachedText;
	}
}
