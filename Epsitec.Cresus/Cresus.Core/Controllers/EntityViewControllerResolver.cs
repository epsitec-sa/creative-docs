//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Orchestrators.Navigation;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// The <c>EntityViewControllerResolver</c> is used to instantiate a controller for a given
	/// entity type, mode and sub type ID.
	/// </summary>
	public static class EntityViewControllerResolver
	{
		public static EntityViewController Resolve(DataViewOrchestrator orchestrator, string name, AbstractEntity entity, ViewControllerMode mode, int controllerSubTypeId, NavigationPathElement navigationPathElement)
		{
			EntityViewControllerResolver.defaults = new DefaultSettings
			{
				Orchestrator = orchestrator,
				Mode = mode,
				NavigationPathElement = navigationPathElement,
			};
			
			try
			{
				return EntityViewControllerResolver.ResolveEntityViewController (name, entity, mode, controllerSubTypeId);
			}
			finally
			{
				EntityViewControllerResolver.defaults = null;
			}
		}


		public static DefaultSettings Default
		{
			get
			{
				return EntityViewControllerResolver.defaults;
			}
		}

		public sealed class DefaultSettings
		{
			public DataViewOrchestrator Orchestrator
			{
				get;
				set;
			}

			public ViewControllerMode Mode
			{
				get;
				set;
			}

			public NavigationPathElement NavigationPathElement
			{
				get;
				set;
			}
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

				default:
					throw new System.NotSupportedException (string.Format ("ViewControllerMode.{0} not supported", mode));
			}
		}

		private static System.Type FindViewControllerType(System.Type entityType, ViewControllerMode mode, int controllerSubTypeId)
		{
			var baseTypePrefix = EntityViewControllerResolver.GetViewControllerPrefix (mode);
			var baseTypeName   = string.Concat (baseTypePrefix, "ViewController`1");

			//	Find all concrete classes which use either the generic SummaryViewController or the
			//	generic EditionViewController base classes, which match the entity type (usually,
			//	there should be exactly one such type).

			var types = from type in typeof (EntityViewController).Assembly.GetTypes ()
						where type.IsClass && !type.IsAbstract
						let baseType = type.BaseType
						where baseType.IsGenericType && baseType.Name.StartsWith (baseTypeName) && baseType.GetGenericArguments ()[0] == entityType
						select type;

			if (controllerSubTypeId < 0)
			{
				types = types.Where (type => type.GetCustomAttributes (typeof (ControllerSubTypeAttribute), false).Length == 0);
			}
			else
			{
				types = types.Where (type => type.GetCustomAttributes (typeof (ControllerSubTypeAttribute), false).Cast<ControllerSubTypeAttribute> ().Any (attribute => attribute.Id == controllerSubTypeId));
			}

			//	Si on n'a rien trouvé et qu'on cherche un controllerSubTypeId précis, on effectue une nouvelle
			//	recherche moins restrictive. Ceci est nécessaire pour trouver SummaryContactRoleListViewController !
			//	TODO: C'est une verrue qu'il faudra probablement améliorer.

			if (types.Count () == 0 && controllerSubTypeId >= 0)
			{
				types = from type in typeof (EntityViewController).Assembly.GetTypes ()
						where type.IsClass && !type.IsAbstract
						let baseType = type.BaseType
						where baseType.IsGenericType && baseType.Name.StartsWith (baseTypeName)
						select type;

				types = types.Where (type => type.GetCustomAttributes (typeof (ControllerSubTypeAttribute), false).Cast<ControllerSubTypeAttribute> ().Any (attribute => attribute.Id == controllerSubTypeId));
			}

			return types.FirstOrDefault ();
		}

		private static EntityViewController ResolveEntityViewController(string name, AbstractEntity entity, ViewControllerMode mode, int controllerSubTypeId)
		{
			switch (mode)
			{
				case ViewControllerMode.None:
					return null;

				case ViewControllerMode.Summary:
				case ViewControllerMode.Edition:
				case ViewControllerMode.Creation:
					break;

				default:
					throw new System.NotSupportedException (string.Format ("ViewControllerMode.{0} not supported", mode));
			}

			var type = EntityViewControllerResolver.FindViewControllerType (entity.GetType (), mode, controllerSubTypeId);

			if (type == null)
			{
				if (mode == ViewControllerMode.Creation)
				{
					return null;
				}

				throw new System.InvalidOperationException (string.Format ("Cannot create controller {0} for entity of type {1} using ViewControllerMode.{2}", name, entity.GetType (), mode));
			}

			return System.Activator.CreateInstance (type, new object[] { name, entity }) as EntityViewController;
		}

		[System.ThreadStatic]
		private static DefaultSettings defaults;
	}
}
