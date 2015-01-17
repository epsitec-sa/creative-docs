using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Labels;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Resolvers
{
	public static class LabelTextFactoryResolver
	{
		public static LabelTextFactory Resolve(Type entityType, int id)
		{
			var factoryType = LabelTextFactoryResolver.FindFactoryType (entityType, id);

			if (factoryType == null)
			{
				throw new ArgumentException ("LabelTextFactory not found.");
			}

			var factoryInstance = Activator.CreateInstance (factoryType);

			return (LabelTextFactory) factoryInstance;
		}

		private static Type FindFactoryType(Type entityType, int id)
		{
			var labelTextFactoryType = typeof (LabelTextFactory);

			// Find a class that :
			// - is concrete
			// - is a subtype of LabelTextFactory
			// - is a subtype of a generic type, and the first generic type parameter of this super
			//   type is entityType
			// - has an LabelTextFactoryId attribute whose value matches id, or no such attribute
			//   if id is 0

			var factoryTypes =
				from type in TypeEnumerator.Instance.GetAllClassTypes ()
				where !type.IsAbstract
				where labelTextFactoryType.IsAssignableFrom (type)
				let baseType = type.GetBaseTypes ().FirstOrDefault (bt => bt.IsGenericType)
				where baseType != null
				where baseType.GetGenericArguments ()[0] == entityType
				let attribute = type.GetCustomAttributes<LabelTextFactoryIdAttribute> ().FirstOrDefault ()
				where (attribute == null && id == 0) || (attribute != null && attribute.Id == id)
				select type;

			var factoryType = factoryTypes.FirstOrDefault ();

			// We found a type that matches the search criteria, so we return it.
			if (factoryType != null)
			{
				return factoryType;
			}

			var baseEntityType = entityType.BaseType;

			// If the entity is derived from another, we try to find a factory for that base type.
			if (baseEntityType != typeof (AbstractEntity))
			{
				return LabelTextFactoryResolver.FindFactoryType (baseEntityType, id);
			}

			// No factory exists for the given arguments.
			return null;
		}
	}
}
