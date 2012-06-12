//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Resolvers
{
	public sealed class CoreComponentFactoryResolver<TFactory>
			where TFactory : class
	{
		public static IEnumerable<TFactory> Resolve()
		{
			if (CoreComponentFactoryResolver<TFactory>.factories == null)
			{
				CoreComponentFactoryResolver<TFactory>.factories = InterfaceImplementationResolver<TFactory>.CreateInstances ().ToList ();
			}

			return CoreComponentFactoryResolver<TFactory>.factories;
		}

		[System.ThreadStatic]
		private static List<TFactory>			factories;
	}
}
