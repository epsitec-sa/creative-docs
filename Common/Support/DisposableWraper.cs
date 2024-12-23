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
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support
{
    /// <summary>
    /// The <see cref="DisposableWrapper"/> class provides an easy way to wrap a call to an
    /// <see cref="Action"/> into a <see cref="IDisposable"/> object that will ensure that it will
    /// be called exactly once if the object is disposed and that an
    /// <see cref="System.InvalidOperationException"/> will be thrown if the object has not been disposed.
    /// </summary>
    public class DisposableWrapper : System.IDisposable
    {
        protected DisposableWrapper(System.Action action)
        {
            this.action = action;

            this.actionDone = false;
        }

        ~DisposableWrapper()
        {
            if (!this.actionDone)
            {
                throw new System.InvalidOperationException("Caller forgot to call Dispose");
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (!this.actionDone)
            {
                this.action();

                this.actionDone = true;

                if (this.actionFinally != null)
                {
                    while (this.actionFinally.Count > 0)
                    {
                        var actionFinally = this.actionFinally.Dequeue();
                        actionFinally();
                    }
                }
            }

            System.GC.SuppressFinalize(this);
        }

        #endregion


        /// <summary>
        /// Gets an instance of <see cref="IDisposable"/> that will call <paramref name="action"/>
        /// when disposed for the first time and that will throw an <see cref="System.InvalidOperationException"/>
        /// if not disposed.
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to execute when disposed.</param>
        /// <returns>The <see cref="IDisposable"/> object</returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="action"/> is <c>null</c>.</exception>
        public static System.IDisposable CreateDisposable(System.Action action)
        {
            action.ThrowIfNull("action");

            return new DisposableWrapper(action);
        }

        /// <summary>
        /// Gets an instance of <see cref="IDisposable"/> that will call <paramref name="action"/>
        /// when disposed for the first time and that will throw an <see cref="System.InvalidOperationException"/>
        /// if not disposed.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="action">The <see cref="Action"/> to execute when disposed.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The <see cref="IDisposable"/> object
        /// </returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="action"/> is <c>null</c>.</exception>
        public static DisposableWrapper<T> CreateDisposable<T>(System.Action action, T value)
        {
            return DisposableWrapper<T>.CreateDisposable(action, value);
        }

        /// <summary>
        /// Gets an instance of <see cref="IDisposable"/> that will dispose all objects in
        /// <paramref name="disposables"/> when disposed and that will throw an
        /// <see cref="System.InvalidOperationException"/> if not disposed.
        /// </summary>
        /// <remarks>
        /// All the given objects will be disposed, even if one of them throws an exception when its
        /// dispose method is called. If a single object throws an exception, it is catch and
        /// thrown after all other objects have been disposed. If more than one object throws an
        /// exception when disposed, they are catch and throw after all other objects have been
        /// disposed, grouped in an instance of GroupedException.
        /// </remarks>
        /// <param name="disposables">The objects to dispose.</param>
        /// <returns>The <see cref="IDisposable"/> object</returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="action"/> is <c>null</c>.</exception>
        public static System.IDisposable CombineDisposables(params System.IDisposable[] disposables)
        {
            disposables.ThrowIfNull("disposables");
            disposables.ThrowIf(
                ds => ds.Any(d => d == null),
                "disposables cannot contain null items"
            );

            System.Action finalizer = () =>
            {
                var exceptions = new List<System.Exception>();

                foreach (var disposable in disposables)
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    catch (System.Exception e)
                    {
                        exceptions.Add(e);
                    }
                }

                if (exceptions.Count == 1)
                {
                    throw exceptions.Single();
                }
                else if (exceptions.Count > 1)
                {
                    throw new GroupedException(exceptions);
                }
            };

            return DisposableWrapper.CreateDisposable(finalizer);
        }

        protected void EnqueueFinallyAction(System.Action action)
        {
            if (this.actionFinally == null)
            {
                this.actionFinally = new Queue<System.Action>();
            }

            this.actionFinally.Enqueue(action);
        }

        private readonly System.Action action;
        private Queue<System.Action> actionFinally;
        private bool actionDone;
    }
}
