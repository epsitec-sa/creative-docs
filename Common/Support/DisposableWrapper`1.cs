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


using Epsitec.Common.Support.Extensions;

namespace Epsitec.Common.Support
{
    public sealed class DisposableWrapper<T> : DisposableWrapper
    {
        private DisposableWrapper(System.Action action, T value)
            : base(action)
        {
            this.value = value;
        }

        public T Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Queues up an action to execute once this disposable has been disposed of.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The disposable instance.</returns>
        public DisposableWrapper<T> AndFinally(System.Action action)
        {
            this.EnqueueFinallyAction(action);

            return this;
        }

        public static DisposableWrapper<T> CreateDisposable(System.Action action, T value)
        {
            action.ThrowIfNull("action");

            return new DisposableWrapper<T>(action, value);
        }

        private readonly T value;
    }
}
