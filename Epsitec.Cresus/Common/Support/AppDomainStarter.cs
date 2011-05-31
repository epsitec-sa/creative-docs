//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System;

namespace Epsitec.Common.Support
{
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
		/// <param name="info">The application domain setup information (defaults to shadow copying files).</param>
		public AppDomainStarter(string name = null, AppDomainSetup info = null)
		{
			this.name = name ?? System.Guid.NewGuid ().ToString ("D");

			if (info == null)
			{
				info = new System.AppDomainSetup
				{
					ShadowCopyFiles = "true"
				};
			}
			
			this.domain = System.AppDomain.CreateDomain (this.name, null, info);
		}

		public void Start<T>(T args, System.Action<T> action)
		{
			Activator inner = this.CreateActivator ();
			inner.Activate (args, action);
		}

		public void Start(System.Action action)
		{
			Activator inner = this.CreateActivator ();
			inner.Activate (action);
		}

		/// <summary>
		/// Starts the action in an isolated application domain.
		/// </summary>
		/// <typeparam name="T">The type of the argument</typeparam>
		/// <param name="domainName">The name of the domain.</param>
		/// <param name="args">The arguments for the action.</param>
		/// <param name="action">The action.</param>
		/// <returns>The <see cref="AppDomainStarter"></see> instance.</returns>
		public static AppDomainStarter StartInIsolatedAppDomain<T>(string domainName, T args, System.Action<T> action)
		{
			var starter = new AppDomainStarter (domainName);
			starter.Start (args, action);
			return starter;
		}
		
		/// <summary>
		/// Starts the action in an isolated application domain.
		/// </summary>
		/// <param name="domainName">The name of the domain.</param>
		/// <param name="action">The action.</param>
		/// <returns>The <see cref="AppDomainStarter"></see> instance.</returns>
		public static AppDomainStarter StartInIsolatedAppDomain(string domainName, System.Action action)
		{
			var starter = new AppDomainStarter (domainName);
			starter.Start (action);
			return starter;
		}

		private Activator CreateActivator()
		{
			string typename     = typeof (Activator).FullName;
			string assemblyName = typeof (Activator).Assembly.FullName;
			
			return (Activator) this.domain.CreateInstanceAndUnwrap (assemblyName, typename);
		}

		#region Activator Class

		private class Activator : System.MarshalByRefObject
		{
			public Activator()
			{
			}
			
			public void Activate(System.Action action)
			{
				action ();
			}

			public void Activate<T>(T args, System.Action<T> action)
			{
				action (args);
			}
		}

		#endregion

		private readonly string name;
		private readonly System.AppDomain domain;
	}
}
