//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Resolvers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Factories
{
	public static class CoreDataComponentFactory
	{
		public static void RegisterComponents(CoreData data)
		{
			var factories = CoreDataComponentFactoryResolver.Resolve ();

			bool again = true;

			while (again)
			{
				again = false;

				foreach (var factory in factories)
				{
					var type = factory.GetComponentType ();
					var name = type.FullName;

					if (data.ContainsComponent (name))
					{
						continue;
					}

					if (factory.CanCreate (data))
					{
						data.RegisterComponent (name, factory.Create (data));
						again = true;
					}
				}
			}
		}

		public static void SetupComponents(IEnumerable<CoreDataComponent> componentCollection)
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
							again = true;
						}
					}
				}
			}

			System.Diagnostics.Debug.Assert (components.All (x => x.IsSetupPending == false));
		}
	}
}
