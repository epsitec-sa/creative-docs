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
		public BrowserListItem(AbstractEntity entity)
		{
			this.entity = entity;
		}


		public AbstractEntity					Entity
		{
			get
			{
				return this.entity;
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
			if (this.text == null)
			{
				this.GenerateDisplayText (list);
			}

			return this.text.Value;
		}

		internal void ClearCachedDisplayText()
		{
			this.text = null;
		}


		private void GenerateDisplayText(BrowserList list)
		{
			this.text = list.GenerateEntityDisplayText (this.entity);

			if (this.text.Value.IsNullOrEmpty)
			{
				this.text = CollectionTemplate.DefaultEmptyText;
			}
		}

		
		private readonly AbstractEntity			entity;
		private FormattedText?					text;
	}
}
