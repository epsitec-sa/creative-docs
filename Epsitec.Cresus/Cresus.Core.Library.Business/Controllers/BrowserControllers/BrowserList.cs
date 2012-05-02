//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.BigList;
using Epsitec.Common.BigList.Widgets;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	/// <summary>
	/// The <c>BrowserList</c> class stores the collection of items, which are then
	/// presented to the user, usually in a <see cref="ItemScrollList"/>, as managed by
	/// <see cref="BrowserScrollListController"/>.
	/// </summary>
	public sealed class BrowserList : IEnumerable<BrowserListItem>, System.IDisposable
	{
		public BrowserList(DataContext context)
		{
			this.list    = new List<BrowserListItem> ();
			this.context = context;
		}

		
		public int								Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public BrowserListItem					this[int index]
		{
			get
			{
				return this.list[index];
			}
		}

		
		public void ClearAndAddRange(IEnumerable<AbstractEntity> entities)
		{
			this.list.Clear ();
			this.list.AddRange (entities.Select (x => new BrowserListItem (x)));
		}

		public void Add(AbstractEntity entity)
		{
			this.list.Add (new BrowserListItem (entity));
		}

		public AbstractEntity RemoveAt(int index)
		{
			if ((index < 0) ||
				(index >= this.list.Count))
			{
				return null;
			}
			else
			{
				var entity = this.list[index].Entity;
				this.list.RemoveAt (index);
				return entity;
			}
		}

		public void Invalidate()
		{
			this.list.ForEach (x => x.ClearCachedDisplayText ());
		}

		public AbstractEntity GetEntity(int index)
		{
			if ((index >= 0) &&
				(index < this.list.Count))
			{
				var item = this.list[index];
				return item.Entity;
			}
			else
			{
				return null;
			}
		}
		
		public EntityKey? GetEntityKey(int index)
		{
			if ((index >= 0) &&
				(index < this.list.Count))
			{
				var item = this.list[index];
				return item.GetEntityKey (this);
			}
			else
			{
				return null;
			}
		}

		public int IndexOf(EntityKey? key)
		{
			if (key == null)
			{
				return -1;
			}

			var rowKey = key.Value.RowKey;

			return this.list.FindIndex (x => x.GetRowKey (this) == rowKey);
		}

		public int IndexOf(AbstractEntity entity)
		{
			if (entity.IsNull ())
			{
				return -1;
			}

			return this.list.FindIndex (x => x.Entity == entity);
		}

		
		internal FormattedText GenerateEntityDisplayText(AbstractEntity entity)
		{
			if (entity == null)
			{
				return FormattedText.Empty;
			}
			else
			{
				return entity.GetCompactSummary ();
			}
		}

		internal EntityKey GetEntityKey(AbstractEntity entity)
		{
			if (entity == null)
			{
				throw new System.ArgumentNullException ("entity");
			}

			var key = this.context.GetNormalizedEntityKey (entity);

			if (key == null)
			{
				throw new System.ArgumentException ("Cannot resolve entity");
			}

			return key.Value;
		}

		internal string ConvertBrowserListItemToString(object value)
		{
			BrowserListItem item = value as BrowserListItem;

			if (item == null)
			{
				return "";
			}
			else
			{
				return item.GetDisplayText (this).ToString ();
			}
		}


		#region IDisposable Members

		public void Dispose()
		{
			this.list.Clear ();
		}

		#endregion

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
		
		
		private readonly List<BrowserListItem>	list;
		private readonly DataContext			context;
	}
}