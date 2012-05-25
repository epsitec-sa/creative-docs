using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Loader
{


	internal sealed class TopologicalSort
	{


		public static List<T> GetOrdering<T>(Dictionary<T, ISet<T>> dependencies)
		{
			// This algorithm used for the topological sort of a directed acyclic graph of
			// dependencies is inspired by the one found on Wikipedia on the following page :
			// http://en.wikipedia.org/wiki/Topological_sort

			var result = new List<T> ();
			var elements = new Queue<T> ();

			while (true)
			{
				var newElements = dependencies
					.Where (x => x.Value.Count == 0)
					.Select (x => x.Key)
					.ToList ();

				foreach (var newElement in newElements)
				{
					elements.Enqueue (newElement);
					dependencies.Remove (newElement);
				}

				if (elements.Count == 0)
				{
					break;
				}

				var element = elements.Dequeue ();

				result.Add (element);

				foreach (var d in dependencies.Values)
				{
					d.Remove (element);
				}
			}

			if (dependencies.Count > 0)
			{
				throw new ArgumentException ("The depency graph has at least one cycle");
			}

			return result;
		}


	}


}
