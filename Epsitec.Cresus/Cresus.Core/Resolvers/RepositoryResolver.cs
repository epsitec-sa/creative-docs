//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.BusinessLogic;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Repositories;

namespace Epsitec.Cresus.Core.Resolvers
{
	public static class RepositoryResolver
	{
		public static Repository Resolve(System.Type entityType, CoreData data)
		{
			var type = RepositoryResolver.FindRepositorySystemTypes (entityType).FirstOrDefault ();

			if (type != null)
			{
				return System.Activator.CreateInstance (type, new object[] { data }) as Repository;
			}
			else
			{
				return null;
			}
		}
		
		private static IEnumerable<System.Type> FindRepositorySystemTypes(System.Type entityType)
		{
			var baseTypeName = "Repository`1";

			var types = from assembly in System.AppDomain.CurrentDomain.GetAssemblies ()
						from type in assembly.GetTypes ()
						where type.IsClass && !type.IsAbstract
						let baseType = type.BaseType
						where baseType.IsGenericType && baseType.Name.StartsWith (baseTypeName) && baseType.GetGenericArguments ()[0] == entityType
						select type;

			return types;
		}
	}
}
