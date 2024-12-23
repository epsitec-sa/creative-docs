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


namespace Epsitec.Common.Support.Data
{
    /// <summary>
    /// La classe ComponentCollection gère le stockage des références sur les
    /// instances implémentant IComponent.
    /// </summary>
    public class ComponentCollection : System.Collections.ICollection, System.IDisposable
    {
        public ComponentCollection(IContainer container)
        {
            this.list = new System.Collections.ArrayList();
            this.container = container;
        }

        public IComponent this[int index]
        {
            get { return this.list[index] as IComponent; }
        }

        public IContainer Container
        {
            get { return this.container; }
        }

        public void Add(IComponent component)
        {
            component.Disposed += this.HandleComponentDisposed;

            this.list.Add(component);
            this.container.NotifyComponentInsertion(this, component);
        }

        public void Remove(IComponent component)
        {
            int index = this.list.IndexOf(component);

            if (index < 0)
            {
                throw new System.ArgumentException("Component not found in collection");
            }

            component.Disposed -= this.HandleComponentDisposed;

            this.list.RemoveAt(index);
            this.container.NotifyComponentRemoval(this, component);
        }

        #region ICollection Members
        public bool IsSynchronized
        {
            get { return this.list.IsSynchronized; }
        }

        public int Count
        {
            get { return this.list.Count; }
        }

        public void CopyTo(System.Array array, int index)
        {
            this.list.CopyTo(array, index);
        }

        public object SyncRoot
        {
            get { return this.list.SyncRoot; }
        }
        #endregion

        #region IEnumerable Members
        public System.Collections.IEnumerator GetEnumerator()
        {
            return this.list.GetEnumerator();
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }
        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.list.Count > 0)
                {
                    IComponent[] components = new IComponent[this.list.Count];
                    this.list.CopyTo(components, 0);

                    for (int i = 0; i < components.Length; i++)
                    {
                        this.Remove(components[i]);
                    }
                }

                System.Diagnostics.Debug.Assert(this.list.Count == 0);

                this.list = null;
                this.container = null;
            }
        }

        protected System.Collections.ArrayList list;
        protected IContainer container;

        private void HandleComponentDisposed(object sender)
        {
            IComponent component = sender as IComponent;

            this.Remove(component);
        }
    }
}
