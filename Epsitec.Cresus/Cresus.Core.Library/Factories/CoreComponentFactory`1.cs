//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Debug;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Resolvers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Factories
{
	/// <summary>
	/// The <c>CoreComponentFactory</c> class provides methods to register and setup
	/// components.
	/// </summary>
	/// <typeparam name="THost">The type of the host.</typeparam>
	/// <typeparam name="TFactory">The type of the factory interface (used by the resolver).</typeparam>
	/// <typeparam name="TComponent">The type of the component.</typeparam>
	public abstract class CoreComponentFactory<THost, TFactory, TComponent> : CoreComponentFactory
		where THost : class, ICoreComponentHost<TComponent>
		where TFactory : class, ICoreComponentFactory<THost, TComponent>
		where TComponent : class, ICoreComponent<THost, TComponent>
	{
		static CoreComponentFactory()
		{
			CoreComponentFactory.Setup ();
		}

		/// <summary>
		/// Registers the components with their host. This will find all components which
		/// have a factory based on <see cref="TFactory"/>. It will then instantiate the
		/// components in the appropriate order.
		/// </summary>
		/// <param name="host">The host.</param>
		public static void RegisterComponents(THost host)
		{
			if (CoreComponentFactory.registerRecursionCount == null)
			{
				CoreComponentFactory.registerRecursionCount = new SafeCounter ();
			}

			var factories = CoreComponentFactoryResolver<TFactory>.Resolve ();

			using (CoreComponentFactory.registerRecursionCount.Enter ())
			{
				bool again = true;

				while (again)
				{
					again = false;

					foreach (var factory in factories)
					{
						var type = factory.GetComponentType ();

						if (host.ContainsComponent (type))
						{
							continue;
						}

						if (factory.CanCreate (host))
						{
							long ms = Profiler.ElapsedMilliseconds (() => host.RegisterComponent (type, factory.Create (host)));

							if (ms > 10)
							{
								int recursion = CoreComponentFactory.registerRecursionCount.Value;
								System.Diagnostics.Debug.WriteLine (string.Format ("WARNING {2} slow component; {0} took {1}ms to register", type.FullName, ms, new string ('>', recursion)));
							}

							again = true;
						}
					}
				}
			}
		}

		/// <summary>
		/// Initializes the components. The components must have been registered previously
		/// with method <see cref="RegisterComponents"/>. The order in which the components
		/// get set up is determined by the components themselves.
		/// </summary>
		/// <param name="componentCollection">The component collection.</param>
		public static void SetupComponents(IEnumerable<TComponent> componentCollection)
		{
			var components = componentCollection.ToList ();

			bool again = true;

			while (again)
			{
				again = false;

				foreach (var component in components)
				{
					if (component.IsSetupPending)
					{
						if (component.CanExecuteSetupPhase ())
						{
							component.ExecuteSetupPhase ();

							CoreComponentFactory<THost, TFactory, TComponent>.RegisterDisposableComponent (component);
							again = true;
						}
					}
				}
			}

			System.Diagnostics.Debug.Assert (components.All (x => x.IsSetupPending == false));
		}

		private static void RegisterDisposableComponent(TComponent component)
		{
			var data = component.Host;
			var disposable = component.GetDisposable ();

			if (disposable != null)
			{
				data.RegisterComponentAsDisposable (disposable);
			}
		}
	}
}
