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

		public Epsitec.Cresus.Database.DbKey	RowKey
		{
			get
			{
				return this.entityKey.RowKey;
			}
		}
		
		internal FormattedText GetDisplayText(BrowserListContext context)
		{
			if (this.cachedText == null)
			{
				this.cachedText = context.GetDisplayText (this.Entity);
			}

			return this.cachedText.Value;
		}

		internal void ClearCachedDisplayText()
		{
			this.cachedText = null;
		}

		
		private readonly AbstractEntity			entity;
		private readonly EntityKey				entityKey;
		private FormattedText?					cachedText;
	}
}
