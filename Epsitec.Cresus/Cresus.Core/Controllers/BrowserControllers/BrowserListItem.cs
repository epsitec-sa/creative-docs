//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers.DataAccessors;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	public class BrowserListItem
	{
		public BrowserListItem(BrowserList list, AbstractEntity entity)
		{
			this.list = list;
			this.entity = entity;
		}


		public EntityKey						EntityKey
		{
			get
			{
				return this.list.GetEntityKey (this.entity);
			}
		}

		public Epsitec.Cresus.Database.DbKey	RowKey
		{
			get
			{
				return this.EntityKey.RowKey;
			}
		}

		public AbstractEntity					Entity
		{
			get
			{
				return this.entity;
			}
		}

		public FormattedText					DisplayText
		{
			get
			{
				if (this.text == null)
				{
					this.GenerateDisplayText ();
				}

				return this.text.Value;
			}
		}

		public void ClearCachedDisplayText()
		{
			this.text = null;
		}

		private void GenerateDisplayText()
		{
			this.text = this.list.GenerateEntityDisplayText (this.entity);

			if ((this.text == null) ||
				(this.text.Value.IsNullOrEmpty))
			{
				this.text = CollectionTemplate.DefaultEmptyText;
			}
		}

		private readonly BrowserList			list;
		private readonly AbstractEntity			entity;
		private FormattedText?					text;
	}
}
