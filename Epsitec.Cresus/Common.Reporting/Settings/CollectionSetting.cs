//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting.Settings
{
	public class CollectionSetting
	{
		public CollectionSetting()
		{
		}



		public System.Comparison<AbstractEntity> SortComparison
		{
			get;
			set;
		}

		public System.Predicate<AbstractEntity> FilterComparison
		{
			get;
			set;
		}
		
		
		public int GroupDepth
		{
			get
			{
				return 0;
			}
		}

		public List<AbstractEntity> CreateList(IEnumerable<AbstractEntity> collection)
		{
			List<AbstractEntity> list = new List<AbstractEntity> ();

			if (this.FilterComparison != null)
			{
				foreach (var item in collection)
				{
					if (this.FilterComparison (item))
					{
						list.Add (item);
					}
				}
			}
			else
			{
				list.AddRange (collection);
			}

			if (this.SortComparison != null)
			{
				list.Sort (this.SortComparison);
			}

			return list;
		}
	}
}
