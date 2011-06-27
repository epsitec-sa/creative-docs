//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Orchestrators.Navigation;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Factories;

namespace Epsitec.Cresus.Core.Resolvers
{
	/// <summary>
	/// The <c>EntityViewControllerResolver</c> is used to instantiate a controller for a given
	/// entity type, mode and sub type ID.
	/// </summary>
	internal static class EntityViewControllerResolver
	{
		public static EntityViewController Resolve(ViewControllerMode mode, int controllerSubTypeId, ResolutionMode resolutionMode)
		{
			var name   = EntityViewControllerFactory.Default.ControllerName;
			var entity = EntityViewControllerFactory.Default.Entity;
			
			var entityType     = entity.GetType ();
			var controllerType = EntityViewControllerResolver.ResolveEntityViewController (entityType, mode, controllerSubTypeId);

			if (controllerType == null)
			{
				if (mode == ViewControllerMode.Creation)
				{
					controllerType = EntityViewControllerResolver.ResolveEntityViewController (entityType, ViewControllerMode.Summary, controllerSubTypeId)
						?? EntityViewControllerResolver.ResolveEntityViewController (entityType, ViewControllerMode.Edition, controllerSubTypeId);
				}
			}

			if (controllerType == null)
			{
				if (resolutionMode == ResolutionMode.NullOnError)
				{
					return null;
				}
				else
				{
					System.Diagnostics.Debug.Assert (resolutionMode == ResolutionMode.ThrowOnError);

					throw new System.InvalidOperationException (string.Format ("Cannot create controller {0} for entity of type {1} using ViewControllerMode.{2}", name, entity.GetType (), mode));
				}
			}

			object controllerInstance = System.Activator.CreateInstance (controllerType, true);
			
			return controllerInstance as EntityViewController;
		}

		
		private static string GetViewControllerPrefix(ViewControllerMode mode)
		{
			switch (mode)
			{
				case ViewControllerMode.Summary:
				case ViewControllerMode.Edition:
				case ViewControllerMode.Creation:
					return mode.ToString ();

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

			var controllerTypes = from type in Epsitec.Common.Types.TypeEnumerator.Instance.GetAllTypes ()
								  where type.IsClass && !type.IsAbstract && type.BaseType != null
								     && type.BaseType.IsGenericType && type.BaseType.Name.StartsWith (baseTypeName)
								  select new
								  {
									  Type = type,
									  BaseEntityType = type.BaseType.GetGenericArguments ()[0]
								  };

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

			if (match != null)
			{
				return match;
			}

			if (entityType.IsGenericType)
			{
				//	The caller is asking for a controller operating on Entity<T>. Try to find
				//	a generic controller Controller<T> operating on Entity<T>.

				var entityTypeName  = entityType.GetGenericTypeDefinition ().ToString ();
				var genericTypeArgs = entityType.GetGenericArguments ();

				//	We cannot use type equality, as the base entity type's FullName is null,
				//	while the entity type's FullName is not. Fortunately, comparing the type
				//	ToString() results is OK:
				
				types = from type in controllerTypes
						where type.BaseEntityType.ToString () == entityTypeName
						select type.Type;

				types = EntityViewControllerResolver.FilterTypes (controllerSubTypeId, types);
				match = types.FirstOrDefault ();

				if (match != null)
				{
					//	We cannot return the generic Controller<> type as it could not be
					//	instantiated -- inject the type arguments of Entity<T> to build a
					//	fully defined generic type:

					match = match.MakeGenericType (genericTypeArgs);
				}
			}

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
