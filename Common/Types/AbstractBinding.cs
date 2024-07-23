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

namespace Epsitec.Common.Types
{
    public abstract class AbstractBinding
    {
        protected AbstractBinding() { }

        internal bool Deferred
        {
            get { return this.deferCounter > 0; }
        }

        /// <summary>
        /// Defers the changes; this method should be used in a <c>using</c>
        /// clause when changing several properties of the binding, to
        /// avoid immediate changes.
        /// </summary>
        /// <returns>An <see cref="T:System.IDisposable"/> object which should
        /// be disposed to reactivate the normal change propagation.</returns>
        public System.IDisposable DeferChanges()
        {
            return new DeferManager(this);
        }

        internal void Add(BindingExpression expression)
        {
            //	The binding expression is referenced through a weak binding
            //	by the binding itself, which allows for the expression to be
            //	garbage collected when its property dies.

            this.AddExpression(expression);
        }

        internal void Remove(BindingExpression expression)
        {
            this.RemoveExpression(expression);
        }

        protected void NotifyBeforeChange()
        {
            this.DetachBeforeChanges();
        }

        protected void NotifyAfterChange()
        {
            if (this.deferCounter == 0)
            {
                this.AttachAfterChanges();
            }
        }

        protected abstract void DetachBeforeChanges();

        protected abstract void AttachAfterChanges();

        protected abstract void AddExpression(BindingExpression expression);

        protected abstract void RemoveExpression(BindingExpression expression);

        protected abstract IEnumerable<BindingExpression> GetExpressions();

        private void BeginDefer()
        {
            this.deferCounter++;
        }

        private void EndDefer()
        {
            this.deferCounter--;
            if (this.deferCounter == 0)
            {
                this.AttachAfterChanges();
            }
        }

        #region Private DeferManager Class

        private sealed class DeferManager : System.IDisposable
        {
            public DeferManager(AbstractBinding binding)
            {
                this.binding = binding;
                this.binding.BeginDefer();
            }

            #region IDisposable Members

            void System.IDisposable.Dispose()
            {
                if (this.binding != null)
                {
                    AbstractBinding binding = this.binding;
                    this.binding = null;
                    binding.EndDefer();
                }
            }

            #endregion

            private AbstractBinding binding;
        }

        #endregion

        private int deferCounter;
    }
}
