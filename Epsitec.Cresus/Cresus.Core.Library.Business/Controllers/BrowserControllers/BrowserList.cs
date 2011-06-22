//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	public class BrowserList : IEnumerable<BrowserListItem>, System.IDisposable
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

		
		public void DefineEntities(IEnumerable<AbstractEntity> entities)
		{
			this.list.Clear ();
			this.list.AddRange (entities.Select (x => new BrowserListItem (x)));
		}

		public void Insert(AbstractEntity entity)
		{
			this.list.Add (new BrowserListItem (entity));
		}

		public void Invalidate()
		{
			this.list.ForEach (x => x.ClearCachedDisplayText ());
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

		public int GetIndex(EntityKey? key)
		{
			if (key == null)
			{
				return -1;
			}

			var rowKey = key.Value.RowKey;

			return this.list.FindIndex (x => x.GetRowKey (this) == rowKey);
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

		internal string ValueConverterFunction(object value)
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