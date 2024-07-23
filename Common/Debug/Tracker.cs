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

namespace Epsitec.Common.Debug
{
    /// <summary>
    /// The <c>>Tracker</c> static class provides methods which can track and
    /// label objects. This is useful when an object has to be identified by
    /// its reference rather than just by its content.
    /// </summary>
    public static class Tracker
    {
        /// <summary>
        /// Registers the specified object. This associates a unique tag with
        /// the specified object. If the object was already known by the tracker,
        /// then the same tag will be issued.
        /// </summary>
        /// <param name="obj">The object to register.</param>
        /// <returns>A unique tag associated with the object.</returns>
        public static string Register(object obj)
        {
            string old = Tracker.Identify(obj);
            if (old != null)
                return old;
            Item item = new Item(string.Format("{0}", Tracker.id++), obj);
            Tracker.items.Add(item);
            return item.Name;
        }

        /// <summary>
        /// Registers the specified object. This associates a unique tag with
        /// the specified object. If the object was already known by the tracker,
        /// then the same tag will be issued. A user provided name will be appended
        /// to the tag.
        /// </summary>
        /// <param name="name">The user provided name.</param>
        /// <param name="obj">The object to register.</param>
        /// <returns>A unique tag associated with the object.</returns>
        public static string Register(string name, object obj)
        {
            string old = Tracker.Identify(obj);
            if (old != null)
                return old;
            Item item = new Item(string.Format("{0}:{1}", Tracker.id++, name), obj);
            Tracker.items.Add(item);
            return item.Name;
        }

        /// <summary>
        /// Identifies the specified object.
        /// </summary>
        /// <param name="obj">The object to identify.</param>
        /// <returns>The tag of the object if it is known, <c>null</c> otherwise.</returns>
        public static string Identify(object obj)
        {
            if (obj == null)
            {
                return "<null>";
            }

            for (int i = 0; i < Tracker.items.Count; i++)
            {
                Item item = Tracker.items[i];

                if (item.Object == null)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("{0} recycled", item.Name));
                    Tracker.items.RemoveAt(i);
                    i--;
                }
                else if (System.Object.ReferenceEquals(item.Object, obj))
                {
                    return item.Name;
                }
            }

            return null;
        }

        /// <summary>
        /// Forgets about the specified object.
        /// </summary>
        /// <param name="obj">The object to forget.</param>
        /// <returns>The tag of the object if it is known, <c>null</c> otherwise.</returns>
        public static string Forget(object obj)
        {
            if (obj == null)
            {
                return "<null>";
            }

            for (int i = 0; i < Tracker.items.Count; i++)
            {
                Item item = Tracker.items[i];

                if (item.Object == null)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("{0} recycled", item.Name));
                    Tracker.items.RemoveAt(i);
                    i--;
                }
                else if (System.Object.ReferenceEquals(item.Object, obj))
                {
                    Tracker.items.RemoveAt(i);
                    return item.Name;
                }
            }

            return null;
        }

        #region Item Structure

        private struct Item
        {
            public Item(string name, object obj)
            {
                this.name = name;
                this.weak = new System.WeakReference(obj);
            }

            public string Name
            {
                get { return this.name; }
            }

            public object Object
            {
                get { return this.weak.Target; }
            }

            private readonly string name;
            private readonly System.WeakReference weak;
        }

        #endregion

        static private readonly List<Item> items = new List<Item>();
        private static int id;
    }
}
