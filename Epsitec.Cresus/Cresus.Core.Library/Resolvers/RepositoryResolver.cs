//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Repositories;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Resolvers
{
	/// <summary>
	/// The <c>RepositoryResolver</c> class is used to find the repository for a given
	/// entity type. Basically, a resolver class must derive from <see cref="Repository&lt;T&gt;"/>.
	/// </summary>
	public static class RepositoryResolver
	{
		public static Repository Resolve(System.Type entityType, CoreData data, DataContext dataContext)
		{
			if (RepositoryResolver.resolverCache == null)
			{
				RepositoryResolver.resolverCache = new Dictionary<System.Type, System.Type> ();
			}

			System.Type type;

			if (RepositoryResolver.resolverCache.TryGetValue (entityType, out type) == false)
			{
				type = RepositoryResolver.FindRepositorySystemTypes (entityType).FirstOrDefault ();
				RepositoryResolver.resolverCache[entityType] = type;
			}

			if (type != null)
			{
				return System.Activator.CreateInstance (type, new object[] { data, dataContext }) as Repository;
			}
			else
			{
				throw new System.Exception (string.Format ("No repository found for entity of type {0}", entityType.Name));
			}
		}

		public static T Resolve<T>(CoreData data, DataContext dataContext)
			where T : Repository
		{
			return System.Activator.CreateInstance (typeof (T), new object[] { data, dataContext }) as T;
		}

		public static Repository Clone(Repository repository)
		{
			return System.Activator.CreateInstance (repository.GetType (), new object[] { repository.Data, repository.DataContext }) as Repository;
		}


		private static IEnumerable<System.Type> FindRepositorySystemTypes(System.Type entityType)
		{
			const string baseTypeName = "Repository`1";

			var types = from type in TypeEnumerator.Instance.GetAllTypes ()
						where type.IsClass && !type.IsAbstract
						let baseType = type.BaseType
						where baseType != null && baseType.IsGenericType && baseType.Name.StartsWith (baseTypeName) && baseType.GetGenericArguments ()[0] == entityType
						select type;

			return types;
		}

		[System.ThreadStatic]
		private static Dictionary<System.Type, System.Type> resolverCache;
	}
}
