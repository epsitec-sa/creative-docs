//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Resolvers
{
	public static class CoreDataComponentFactoryResolver
	{
		public static IEnumerable<ICoreDataComponentFactory> Resolve()
		{
			return from type in CoreDataComponentFactoryResolver.FindFactorySystemTypes ()
				   select System.Activator.CreateInstance (type) as ICoreDataComponentFactory;
		}

		private static IEnumerable<System.Type> FindFactorySystemTypes()
		{
			string interfaceTypeName = typeof (ICoreDataComponentFactory).FullName;

			var types = from assembly in System.AppDomain.CurrentDomain.GetAssemblies ()
						from type in assembly.GetTypes ()
						where type.IsClass && !type.IsAbstract
						where type.GetInterface (interfaceTypeName) != null
						select type;

			return types;
		}
	}
}
