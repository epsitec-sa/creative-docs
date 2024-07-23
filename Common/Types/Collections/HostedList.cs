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


namespace Epsitec.Common.Types.Collections
{
    /// <summary>
    /// The HostedList class implements a list of T with notifications to the
    /// host when the contents changes (insertion and removal of items).
    /// </summary>
    /// <typeparam name="T">Type of items stored in list</typeparam>
    public class HostedList<T> : ObservableList<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HostedList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="host">The host which must be notified.</param>
        public HostedList(IListHost<T> host)
        {
            this.host = host;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HostedList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="insertionCallback">The insertion callback.</param>
        /// <param name="removalCallback">The removal callback.</param>
        public HostedList(Callback insertionCallback, Callback removalCallback)
        {
            this.host = new CallbackRelay(insertionCallback, removalCallback);
        }

        /// <summary>
        /// Gets the host for this list; this is the object which is notified
        /// when insertions and removals happen.
        /// </summary>
        /// <value>The host.</value>
        public IListHost<T> Host
        {
            get { return this.host; }
        }

        public delegate void Callback(T item);

        #region CallbackRelay Class

        private class CallbackRelay : IListHost<T>
        {
            public CallbackRelay(Callback insertionCallback, Callback removalCallback)
            {
                this.insertionCallback = insertionCallback;
                this.removalCallback = removalCallback;
            }

            #region IListHost<T> Members

            public HostedList<T> Items
            {
                get { throw new System.Exception("The method or operation is not implemented."); }
            }

            public void NotifyListInsertion(T item)
            {
                this.insertionCallback(item);
            }

            public void NotifyListRemoval(T item)
            {
                this.removalCallback(item);
            }

            #endregion

            private Callback insertionCallback;
            private Callback removalCallback;
        }

        #endregion

        protected override void OnCollectionChanged(CollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            switch (e.Action)
            {
                case CollectionChangedAction.Add:
                    foreach (T item in e.NewItems)
                    {
                        this.NotifyInsertion(item);
                    }
                    break;

                case CollectionChangedAction.Move:
                    break;

                case CollectionChangedAction.Remove:
                    foreach (T item in e.OldItems)
                    {
                        this.NotifyRemoval(item);
                    }
                    break;

                case CollectionChangedAction.Replace:
                    foreach (T item in e.OldItems)
                    {
                        this.NotifyRemoval(item);
                    }
                    foreach (T item in e.NewItems)
                    {
                        this.NotifyInsertion(item);
                    }
                    break;

                default:
                    throw new System.NotImplementedException(
                        string.Format("Action.{0} not implemented", e.Action)
                    );
            }
        }

        protected virtual void NotifyInsertion(T item)
        {
            if (this.host != null)
            {
                this.host.NotifyListInsertion(item);
            }
        }

        protected virtual void NotifyRemoval(T item)
        {
            if (this.host != null)
            {
                this.host.NotifyListRemoval(item);
            }
        }

        private IListHost<T> host;
    }
}
