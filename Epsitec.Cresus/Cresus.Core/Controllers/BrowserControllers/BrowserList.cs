//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	public class BrowserList : IEnumerable<BrowserListItem>
	{
		public BrowserList(DataContext context)
		{
			this.list = new List<BrowserListItem> ();
			this.context = context;
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public void DefineEntities(IEnumerable<AbstractEntity> entities)
		{
			this.list.Clear ();
			this.list.AddRange (entities.Select (x => new BrowserListItem (this, x)));
		}

		public EntityKey? GetEntityKey(int index)
		{
			if ((index >= 0) &&
				(index < this.list.Count))
			{
				var item = this.list[index];
				return item.EntityKey;
			}
			else
			{
				return null;
			}
		}

		internal FormattedText GenerateEntityDisplayText(AbstractEntity entity)
		{
			if (entity == null)
            {
				throw new System.ArgumentNullException ("entity");
            }

			return BrowserViewController.GetEntityDisplayText (entity);
		}

        internal EntityKey GetEntityKey(AbstractEntity entity)
		{
			if (entity == null)
			{
				throw new System.ArgumentNullException ("entity");
			}

			var key = this.context.GetEntityKey (entity);

			if (key == null)
			{
				throw new System.ArgumentException ("Cannot resolve entity");
			}

			return key.Value;
		}

		internal static string ValueConverterFunction(object value)
		{
			BrowserListItem item = value as BrowserListItem;

			if (item == null)
			{
				return "";
			}
			else
			{
				return item.DisplayText.ToString ();
			}
		}
		
		private readonly List<BrowserListItem> list;
		private readonly DataContext context;

		#region IEnumerable<BrowserListItem> Members

		public IEnumerator<BrowserListItem> GetEnumerator()
		{
			return this.list.GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator ();
		}

		#endregion
	}
}