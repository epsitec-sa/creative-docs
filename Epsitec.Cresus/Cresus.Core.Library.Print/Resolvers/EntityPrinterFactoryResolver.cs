//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Print.EntityPrinters;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Resolvers
{
	/// <summary>
	/// The <c>EntityPrinterFactoryResolver</c> class is used to find implementations of the
	/// <see cref="IEntityPrinterFactory"/> interface.
	/// </summary>
	public sealed class EntityPrinterFactoryResolver
	{
		public static IEnumerable<IEntityPrinterFactory> Resolve()
		{
			if (EntityPrinterFactoryResolver.factories == null)
			{
				EntityPrinterFactoryResolver.factories = InterfaceImplementationResolver<IEntityPrinterFactory>.CreateInstances ().ToList ();
			}

			return EntityPrinterFactoryResolver.factories;
		}

		[System.ThreadStatic]
		private static List<IEntityPrinterFactory> factories;
	}
}
