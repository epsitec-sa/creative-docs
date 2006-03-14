//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public class Collection
	{
		public static int Count<T>(IEnumerable<T> collection)
		{
			int n = 0;
			
			foreach (T item in collection)
			{
				n++;
			}
			
			return n;
		}

		public static List<T> ToList<T>(IEnumerable<T> collection)
		{
			List<T> list = new List<T> ();
			list.AddRange (collection);
			return list;
		}
		public static T[] ToArray<T>(IEnumerable<T> collection)
		{
			List<T> list = new List<T> ();
			list.AddRange (collection);
			return list.ToArray ();
		}
	}
}
