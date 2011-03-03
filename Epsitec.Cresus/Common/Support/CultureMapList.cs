//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>CultureMapList</c> class implements an observable list of
	/// <see cref="CultureMap"/> items.
	/// </summary>
	public sealed class CultureMapList : Types.Collections.ObservableList<CultureMap>
	{
		public CultureMapList(ResourceAccessors.AbstractResourceAccessor accessor)
		{
			this.accessor = accessor;
		}

		/// <summary>
		/// Gets the <see cref="CultureMap"/> with the specified name.
		/// </summary>
		/// <value>The <see cref="CultureMap"/> or <c>null</c>.</value>
		public CultureMap this[string name]
		{
			get
			{
				return this.RefreshItemIfNeeded (this.list.FirstOrDefault (x => x.Name == name));
			}
		}

		/// <summary>
		/// Gets the <see cref="CultureMap"/> with the specified id.
		/// </summary>
		/// <value>The <see cref="CultureMap"/> or <c>null</c>.</value>
		public CultureMap this[Druid id]
		{
			get
			{
				return this.RefreshItemIfNeeded (this.Peek (id));
			}
		}

		/// <summary>
		/// Gets the <see cref="CultureMap"/> at the specified index.
		/// </summary>
		/// <value>The <see cref="CultureMap"/>.</value>
		/// <exception cref="System.ArgumentOutOfRangeException">index is out of range.</exception>
		public new CultureMap this[int index]
		{
			get
			{
				return this.RefreshItemIfNeeded (base[index]);
			}
		}


		/// <summary>
		/// Refreshes all <see cref="CultureMap"/> items, if needed.
		/// </summary>
		public void RefreshItemsIfNeeded()
		{
			this.list.ToArray ().ForEach (x => this.RefreshItemIfNeeded (x));
		}

		
		public CultureMap Peek(Druid id)
		{
			//	Same as [] indexer, but without automatic refresh.

			return this.list.FirstOrDefault (x => x.Id == id);
		}

		protected override void NotifyBeforeSet(int index, CultureMap oldValue, CultureMap newValue)
		{
			//	The this[int index] accessor should be considered to be read-only.
			//	We cannot enforce this at compile time; but we make sure nobody tries
			//	to use it through the base class implementation :

			throw new System.InvalidOperationException (string.Format ("Class {0} Item operator is read-only", this.GetType ().Name));
		}

		private CultureMap RefreshItemIfNeeded(CultureMap item)
		{
			if ((item != null) &&
				(item.IsRefreshNeeded) &&
				(this.accessor != null))
			{
				this.accessor.InternalRefreshItem (item);
			}

			return item;
		}

		private ResourceAccessors.AbstractResourceAccessor accessor;
	}
}
