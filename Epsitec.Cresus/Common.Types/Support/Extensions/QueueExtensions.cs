//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.Extensions
{
	public static class QueueExtensions
	{
		public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> collection)
		{
			foreach (T item in collection)
			{
				queue.Enqueue (item);
			}
		}

		/// <summary>
		/// Requeues the specified item (i.e. removes it from the queue and enqueues it
		/// at the end of the queue).
		/// </summary>
		/// <typeparam name="T">Specifies the type of elements in the queue.</typeparam>
		/// <param name="queue">The queue.</param>
		/// <param name="item">The item to requeue (or enqueue).</param>
		public static void Requeue<T>(this Queue<T> queue, T item)
			where T : class
		{
			var items = queue.Where (x => !x.Equals (item)).ToList ();

			queue.Clear ();
			items.ForEach (x => queue.Enqueue (x));
			queue.Enqueue (item);
		}
	}
}
