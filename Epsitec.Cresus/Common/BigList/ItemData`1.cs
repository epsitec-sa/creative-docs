//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public class ItemData<T> : ItemData
	{
		public ItemData(T data)
		{
			this.data = data;
		}


		public T Data
		{
			get
			{
				return this.data;
			}
		}


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
