//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	/// <summary>
	/// The <c>ItemData&lt;T&gt;</c> generic class stores a data item of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The native data type.</typeparam>
	public class ItemData<T> : ItemData
	{
		public ItemData(T data, ItemState state)
			: base (state)
		{
			this.data = data;
		}


		public T								Data
		{
			get
			{
				return this.data;
			}
		}


		/// <summary>
		/// Gets the data stored in this instance.
		/// </summary>
		/// <typeparam name="TData">The type of the data.</typeparam>
		/// <returns>
		/// The native data.
		/// </returns>
		/// <exception cref="System.ArgumentException">When the <typeparamref name="TData"/> does
		/// not match the real data type.</exception>
		public override TData GetData<TData>()
		{
			if (typeof (TData) == typeof (T))
			{
				return (TData) (object) this.data;
			}

			throw new System.ArgumentException (string.Format ("Cannot cast type from {0} to {1}", typeof (T).Name, typeof (TData).Name));
		}


		private readonly T						data;
	}
}
