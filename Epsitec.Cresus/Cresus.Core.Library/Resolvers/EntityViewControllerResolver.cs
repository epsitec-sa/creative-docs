//	Copyright © 2010-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Factories;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Resolvers
{
	/// <summary>
	/// The <c>EntityViewControllerResolver</c> is used to instantiate a controller for a given
	/// entity type, mode and sub type ID.
	/// </summary>
	internal static class EntityViewControllerResolver
	{
		public static EntityViewController Resolve(System.Type entityType, ViewControllerMode mode, int? controllerSubTypeId, ResolutionMode resolutionMode)
		{
			mode = EntityViewControllerResolver.FilterMode (mode);

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

					var name = EntityViewControllerFactory.Default.ControllerName;
					throw new System.InvalidOperationException (string.Format ("Cannot create controller {0} for entity of type {1} using ViewControllerMode.{2}", name, entityType, mode));
				}
			}

			object controllerInstance = System.Activator.CreateInstance (controllerType, true);
			
			return controllerInstance as EntityViewController;
		}


		private static ViewControllerMode FilterMode(ViewControllerMode mode)
		{
			switch (mode)
			{
				case ViewControllerMode.CreationOrSummary:
					return ViewControllerMode.Summary;

				case ViewControllerMode.CreationOrEdition:
					return ViewControllerMode.Edition;

				default:
					return mode;
			}
		}
		
		private static string GetViewControllerPrefix(ViewControllerMode mode)
		{
			switch (mode)
			{
				case ViewControllerMode.Summary:
				case ViewControllerMode.Edition:
				case ViewControllerMode.Creation:
				case ViewControllerMode.Action:
				case ViewControllerMode.Set:
				case ViewControllerMode.BrickCreation:
					return mode.ToString ();

				case ViewControllerMode.None:
					throw new System.NotSupportedException (string.Format ("ViewControllerMode.{0} cannot be specified here", mode));

				default:
					throw new System.NotSupportedException (string.Format ("ViewControllerMode.{0} not supported", mode));
			}
		}

		private static System.Type FindViewControllerType(System.Type entityType, ViewControllerMode mode, int? controllerSubTypeId)
		{
			System.Type match;

			var baseTypePrefix = EntityViewControllerResolver.GetViewControllerPrefix (mode);
			var baseTypeName   = string.Concat (baseTypePrefix, "ViewController");
			var entityViewControllerType = typeof (EntityViewController);

			// Find all classes that :
			// - are concrete
			// - are a subtype of EntityViewController
			// - have a super type with a name like "SummaryViewController,", "SetViewController",
			// - "EditionViewController", ... and is a generic type
			// - where the first generic type parameter of the direct super class is entityType

			var controllerTypes =
			(
				from type in TypeEnumerator.Instance.GetAllTypes ()
				where entityViewControllerType.IsAssignableFrom (type)
					&& type.IsClass
					&& !type.IsAbstract
				let baseType = type
					.GetBaseTypes ()
					.FirstOrDefault (bt => bt.IsGenericType && bt.Name.StartsWith (baseTypeName))
				where baseType != null
				select new
				{
					Type = type,
					BaseEntityType = baseType.GetGenericArguments ()[0]
				}
			).ToList ();

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

		private static IEnumerable<System.Type> FilterTypes(int? controllerSubTypeId, IEnumerable<System.Type> types)
		{
			int id = controllerSubTypeId.GetValueOrDefault (-1);
			
			if (id >= 0)
			{
				return types.Where (type => type.GetCustomAttributes<ControllerSubTypeAttribute> ().Any (attribute => attribute.Id == id));
			}
			else
			{
				return types.Where (type => type.GetCustomAttributes<ControllerSubTypeAttribute> ().IsEmpty ());
			}
		}

		private static System.Type ResolveEntityViewController(System.Type entityType, ViewControllerMode mode, int? controllerSubTypeId)
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
