/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


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
                queue.Enqueue(item);
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
            var items = queue.Where(x => !x.Equals(item)).ToList();

            queue.Clear();
            items.ForEach(x => queue.Enqueue(x));
            queue.Enqueue(item);
        }
    }
}
