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
	/// <see cref="BrowserListController"/>.
	/// </summary>
	public sealed class BrowserList : IEnumerable<BrowserListItem>, System.IDisposable
	{
		public BrowserList()
		{
			this.list    = new List<BrowserListItem> ();
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

		
		public void ClearAndAddRange(IEnumerable<BrowserListItem> items)
		{
			this.list.Clear ();
			this.list.AddRange (items);
		}

		public int IndexOf(EntityKey? key)
		{
			if (key == null)
			{
				return -1;
			}

			var rowKey = key.Value.RowKey;

			return this.list.FindIndex (x => x.RowKey == rowKey);
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
	}
}