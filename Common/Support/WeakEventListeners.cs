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

namespace Epsitec.Common.Support
{
    /// <summary>
    /// The <c>WeakEventListeners</c> structure stores a list of weak references
    /// to event handlers (aka listeners) which can be invoked.
    /// </summary>
    public struct WeakEventListeners
    {
        /// <summary>
        /// Adds the specified listener delegate.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public void Add(System.Delegate listener)
        {
            System.Delegate[] invocations = listener.GetInvocationList();

            if (invocations.Length > 1)
            {
                foreach (System.Delegate item in invocations)
                {
                    this.Add(item);
                }
            }
            else if (invocations.Length == 1)
            {
                if (this.listeners == null)
                {
                    this.listeners = new List<WeakEventListener>();
                }

                this.listeners.Add(new WeakEventListener(listener));
            }
        }

        /// <summary>
        /// Removes the specified listener delegate.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public void Remove(System.Delegate listener)
        {
            System.Delegate[] invocations = listener.GetInvocationList();

            if (invocations.Length > 1)
            {
                foreach (System.Delegate item in invocations)
                {
                    this.Remove(item);
                }
            }
            else if (invocations.Length == 1)
            {
                if (this.listeners != null)
                {
                    bool removed = false;
                    this.listeners.RemoveAll(e =>
                        e.IsDead || (removed == false && (true == (removed = e.Equals(listener))))
                    );
                }
            }
        }

        /// <summary>
        /// Invokes the listeners.
        /// </summary>
        public void Invoke()
        {
            this.Invoke(new object[0]);
        }

        /// <summary>
        /// Invokes the listeners with the specified sender as parameter.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void Invoke(object sender)
        {
            this.Invoke(new object[] { sender });
        }

        /// <summary>
        /// Invokes the listerners with the specified sender and event as
        /// parameters.
        /// </summary>
        /// <typeparam name="T">The event argument type.</typeparam>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event argument.</param>
        public void Invoke<T>(object sender, T e)
            where T : EventArgs
        {
            this.Invoke(new object[] { sender, e });
        }

        /// <summary>
        /// Gets the number of items in the listener list.
        /// </summary>
        /// <returns>The number of items.</returns>
        public int DebugGetListenerCount()
        {
            return this.listeners == null ? 0 : this.listeners.Count;
        }

        private void Invoke(object[] parameters)
        {
            if ((this.listeners != null) && (this.listeners.Count > 0))
            {
                WeakEventListener[] temp = this.listeners.ToArray();

                foreach (WeakEventListener listener in temp)
                {
                    if (listener.Invoke(parameters) == false)
                    {
                        this.listeners.Remove(listener);
                    }
                }
            }
        }

        private List<WeakEventListener> listeners;
    }
}
