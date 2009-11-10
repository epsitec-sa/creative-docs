﻿//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.Extensions
{
	public static class HashSetExtensions
	{
		public static void AddRange<T>(this HashSet<T> set, IEnumerable<T> collection)
		{
			foreach (T item in collection)
			{
				set.Add (item);
			}
		}
	}
}
