//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

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
				foreach (CultureMap item in this)
				{
					if (item.Name == name)
					{
						this.RefreshItemIfNeeded (item);
						return item;
					}
				}

				return null;
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
				foreach (CultureMap item in this)
				{
					if (item.Id == id)
					{
						this.RefreshItemIfNeeded (item);
						return item;
					}
				}

				return null;
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
				CultureMap item = base[index];

				this.RefreshItemIfNeeded (item);

				return item;
			}
		}

		protected override void NotifyBeforeSet(int index, CultureMap oldValue, CultureMap newValue)
		{
			//	The this[int index] accessor should be considered to be read-only.
			//	We cannot enforce this at compile time; but we make sure nobody tries
			//	to use it through the base class implementation :

			throw new System.InvalidOperationException (string.Format ("Class {0} Item operator is read-only", this.GetType ().Name));
		}

		protected void RefreshItemIfNeeded(CultureMap item)
		{
			if ((item.IsRefreshNeeded) &&
				(this.accessor != null))
			{
				this.accessor.InternalRefreshItem (item);
			}
		}

		ResourceAccessors.AbstractResourceAccessor accessor;
	}
}
