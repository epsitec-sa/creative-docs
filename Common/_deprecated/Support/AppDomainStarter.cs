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


using System;

namespace Epsitec.Common.Support
{
    // NOTE Disables the warning about conflicting types. Here we can do this because it is on
    // purpose that we include this file in the projet App.Designer which results in the type being
    // defined twice in this project. See commit #17610 and commit #17611 for more details.
#pragma warning disable 436

    /// <summary>
    /// The <c>AppDomainStarter</c> class creates an <see cref="AppDomain"/> and
    /// launches an action in that context. The action may not be a lambda with a
    /// captured context, because this won't cross application domains.
    /// </summary>
    public class AppDomainStarter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDomainStarter"/> class.
        /// </summary>
        /// <param name="name">The name of the application domain (defaults to a GUID).</param>
        /// <param name="shadowCopyFiles">If set to <c>true</c>, loads assemblies as shadow copy files (defaults to <c>true</c>).</param>
        public AppDomainStarter(string name = null, bool shadowCopyFiles = true)
        {
            this.name = name ?? System.Guid.NewGuid().ToString("D");

            var info = new System.AppDomainSetup
            {
                ShadowCopyFiles = shadowCopyFiles ? "true" : "false"
            };

            this.domain = System.AppDomain.CreateDomain(this.name, null, info);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDomainStarter"/> class.
        /// </summary>
        /// <param name="name">The name of the application domain.</param>
        /// <param name="info">The application domain setup information.</param>
        public AppDomainStarter(string name, AppDomainSetup info)
        {
            System.Diagnostics.Debug.Assert(name != null);
            System.Diagnostics.Debug.Assert(info != null);

            this.domain = System.AppDomain.CreateDomain(this.name, null, info);
        }

        public void Start<T>(T args, System.Action<T> action)
        {
            Activator inner = this.CreateActivator();
            inner.Activate(args, action);
        }

        public void Start(System.Action action)
        {
            Activator inner = this.CreateActivator();
            inner.Activate(action);
        }

        /// <summary>
        /// Starts the action in an isolated application domain.
        /// </summary>
        /// <typeparam name="T">The type of the argument</typeparam>
        /// <param name="domainName">The name of the domain.</param>
        /// <param name="args">The arguments for the action.</param>
        /// <param name="action">The action.</param>
        /// <returns>The <see cref="AppDomainStarter"></see> instance.</returns>
        public static AppDomainStarter StartInIsolatedAppDomain<T>(
            string domainName,
            T args,
            System.Action<T> action
        )
        {
            var starter = new AppDomainStarter(domainName);
            starter.Start(args, action);
            return starter;
        }

        /// <summary>
        /// Starts the action in an isolated application domain.
        /// </summary>
        /// <param name="domainName">The name of the domain.</param>
        /// <param name="action">The action.</param>
        /// <returns>The <see cref="AppDomainStarter"></see> instance.</returns>
        public static AppDomainStarter StartInIsolatedAppDomain(
            string domainName,
            System.Action action
        )
        {
            var starter = new AppDomainStarter(domainName);
            starter.Start(action);
            return starter;
        }

        private Activator CreateActivator()
        {
            string typename = typeof(Activator).FullName;
            string assemblyName = typeof(Activator).Assembly.FullName;

            return (Activator)this.domain.CreateInstanceAndUnwrap(assemblyName, typename);
        }

        #region Activator Class

        private class Activator : System.MarshalByRefObject
        {
            public Activator() { }

            public void Activate(System.Action action)
            {
                action();
            }

            public void Activate<T>(T args, System.Action<T> action)
            {
                action(args);
            }
        }

        #endregion

        private readonly string name;
        private readonly System.AppDomain domain;
    }
}
