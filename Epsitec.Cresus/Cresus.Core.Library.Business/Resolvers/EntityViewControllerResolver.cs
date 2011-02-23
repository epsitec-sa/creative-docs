//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Orchestrators.Navigation;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Resolvers
{
	/// <summary>
	/// The <c>EntityViewControllerResolver</c> is used to instantiate a controller for a given
	/// entity type, mode and sub type ID.
	/// </summary>
	public static class EntityViewControllerResolver
	{
		public static EntityViewController Resolve(string name, AbstractEntity entity, ViewControllerMode mode, int controllerSubTypeId, ResolutionMode resolutionMode)
		{
			var entityType = entity.GetType ();
			var type = EntityViewControllerResolver.ResolveEntityViewController(entityType, mode, controllerSubTypeId);

			if (type == null)
			{
				if ((mode == ViewControllerMode.Creation) ||
					(resolutionMode == ResolutionMode.NullOnError))
				{
					return null;
				}

				System.Diagnostics.Debug.Assert (resolutionMode == ResolutionMode.ThrowOnError);

				throw new System.InvalidOperationException (string.Format ("Cannot create controller {0} for entity of type {1} using ViewControllerMode.{2}", name, entity.GetType (), mode));
			}

			object[] constructorArguments = new object[] { name, entity };
			object controllerInstance = System.Activator.CreateInstance (type, constructorArguments);
			
			return controllerInstance as EntityViewController;
		}

		
		private static string GetViewControllerPrefix(ViewControllerMode mode)
		{
			switch (mode)
			{
				case ViewControllerMode.Summary:
					return "Summary";
				case ViewControllerMode.Edition:
					return "Edition";
				case ViewControllerMode.Creation:
					return "Creation";

				case ViewControllerMode.None:
					throw new System.NotSupportedException (string.Format ("ViewControllerMode.{0} cannot be specified here", mode));

				default:
					throw new System.NotSupportedException (string.Format ("ViewControllerMode.{0} not supported", mode));
			}
		}

		private static System.Type FindViewControllerType(System.Type entityType, ViewControllerMode mode, int controllerSubTypeId)
		{
			System.Type match;

			var baseTypePrefix = EntityViewControllerResolver.GetViewControllerPrefix (mode);
			var baseTypeName   = string.Concat (baseTypePrefix, "ViewController`1");

			//	Find all concrete classes which use either the generic SummaryViewController or the
			//	generic EditionViewController base classes, which match the entity type (usually,
			//	there should be exactly one such type).

			var controllerTypes = from type in typeof (EntityViewController).Assembly.GetTypes ()
								  where type.IsClass && !type.IsAbstract
								  let baseType = type.BaseType
								  where baseType.IsGenericType && baseType.Name.StartsWith (baseTypeName)
								  let baseEntityType = baseType.GetGenericArguments ()[0]
								  select new { Type = type, BaseEntityType = baseEntityType };

			var types = from type in controllerTypes
						where type.BaseEntityType == entityType
						select type.Type;

			types = EntityViewControllerResolver.FilterTypes (controllerSubTypeId, types);
			match = types.FirstOrDefault ();

			if (match != null)
			{
				return match;
			}
			
			//	No specific controller was found for the entity type; now search for a controller
			//	which supports a base type of the given entity (e.g. AbstractContactEntity for a
			//	MailContactEntity) :

			types = from type in controllerTypes
					where type.BaseEntityType.IsAssignableFrom (entityType)
					select type.Type;

			types = EntityViewControllerResolver.FilterTypes (controllerSubTypeId, types);
			match = types.FirstOrDefault ();

			return match;
		}

		private static IEnumerable<System.Type> FilterTypes(int controllerSubTypeId, IEnumerable<System.Type> types)
		{
			if (controllerSubTypeId < 0)
			{
				return types.Where (type => type.GetCustomAttributes (typeof (ControllerSubTypeAttribute), false).Length == 0);
			}
			else
			{
				return types.Where (type => type.GetCustomAttributes (typeof (ControllerSubTypeAttribute), false).Cast<ControllerSubTypeAttribute> ().Any (attribute => attribute.Id == controllerSubTypeId));
			}
		}

		private static System.Type ResolveEntityViewController(System.Type entityType, ViewControllerMode mode, int controllerSubTypeId)
		{
			if (mode == ViewControllerMode.None)
			{
				return null;
			}
			else
			{
				return EntityViewControllerResolver.FindViewControllerType (entityType, mode, controllerSubTypeId);
			}
		}
	}
}
