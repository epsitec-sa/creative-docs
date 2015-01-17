//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Assets.Core.Collections
{
	public struct CollectionItem<T>
	{
		public CollectionItem(int index, T value)
		{
			this.index = index;
			this.value = value;
		}

		
		public int								Index
		{
			get
			{
				return this.index;
			}
		}
		
		public T								Value
		{
			get
			{
				return this.value;
			}
		}


		private readonly int					index;
		private readonly T						value;
	}
}
