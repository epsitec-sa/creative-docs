//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	public sealed class CultureMapList : Types.Collections.ObservableList<CultureMap>
	{
		public CultureMap this[string name]
		{
			get
			{
				foreach (CultureMap item in this)
				{
					if (item.Name == name)
					{
						return item;
					}
				}

				return null;
			}
		}

		public CultureMap this[Druid id]
		{
			get
			{
				foreach (CultureMap item in this)
				{
					if (item.Id == id)
					{
						return item;
					}
				}

				return null;
			}
		}

		protected override void NotifyBeforeSet(int index, CultureMap oldValue, CultureMap newValue)
		{
			throw new System.InvalidOperationException (string.Format ("Class {0} Item operator is read-only", this.GetType ().Name));
		}
	}
}
