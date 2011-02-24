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
			if (CoreDataComponentFactoryResolver.factories == null)
			{
				CoreDataComponentFactoryResolver.factories = InterfaceImplementationResolver<ICoreDataComponentFactory>.CreateInstances ().ToList ();
			}

			return CoreDataComponentFactoryResolver.factories;
		}

		[System.ThreadStatic]
		private static List<ICoreDataComponentFactory> factories;
	}
}
