//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Resolvers
{
	public static class EntitySpecialControllerResolver
	{
		public static IEntitySpecialController Create(TileContainer container, AbstractEntity entity, int mode)
		{
			var factory = EntitySpecialControllerResolver.Resolve ().FirstOrDefault (x => x.CanRepresent (entity, mode));

			if (factory == null)
			{
				return null;
			}
			else
			{
				return factory.Create (container, entity, mode);
			}
		}
		
		private static IEnumerable<IEntitySpecialControllerFactory> Resolve()
		{
			if (EntitySpecialControllerResolver.factories == null)
			{
				EntitySpecialControllerResolver.factories = InterfaceImplementationResolver<IEntitySpecialControllerFactory>.CreateInstances ().ToList ();
			}

			return EntitySpecialControllerResolver.factories;
		}

		[System.ThreadStatic]
		private static List<IEntitySpecialControllerFactory> factories;
	}
}
