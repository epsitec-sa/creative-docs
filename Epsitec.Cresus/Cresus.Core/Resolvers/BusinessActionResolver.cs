//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Resolvers
{
	/// <summary>
	/// The <c>BusinessActionResolver</c> returns a business action implementation.
	/// </summary>
	public static class BusinessActionResolver
	{
		/// <summary>
		/// Resolves a business rule for the specified entity and rule types. This returns a
		/// <see cref="CompositeBusinessAction"/> with zero, one or more simple rules.
		/// </summary>
		public static GenericBusinessAction Resolve(string actionClass)
		{
			GenericBusinessAction action;
			
			if (BusinessActionResolver.actionCache == null)
			{
				BusinessActionResolver.actionCache = new Dictionary<string, GenericBusinessAction> ();
			}
			else
			{
				if (BusinessActionResolver.actionCache.TryGetValue (actionClass, out action))
				{
					return action;
				}
			}

			action = BusinessActionResolver.CreateBusinessActions (actionClass).FirstOrDefault ();

			BusinessActionResolver.actionCache[actionClass] = action;
			
			return action;
		}

		public static IEnumerable<string> GetActionClasses()
		{
			return from type in BusinessActionResolver.FindBusinessActionSystemTypes ()
				   orderby type.Name ascending
				   select type.Name;
		}

		public static IEnumerable<string> GetVerbs(string actionClass)
		{
			GenericBusinessAction action = BusinessActionResolver.Resolve (actionClass);

			if (action != null)
			{
				var type = action.GetType ();
				var methods = type.GetMethods (System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

				foreach (var method in methods)
				{
					yield return method.Name;
				}
			}
		}


		private static IEnumerable<System.Type> FindBusinessActionSystemTypes()
		{
			var types = from assembly in System.AppDomain.CurrentDomain.GetAssemblies ()
						from type in assembly.GetTypes ()
						where type.IsClass && !type.IsAbstract
						where type.BaseType == typeof (GenericBusinessAction)
						select type;

			return types;
		}

		private static IEnumerable<GenericBusinessAction> CreateBusinessActions(string actionClass)
		{
			var types   = BusinessActionResolver.FindBusinessActionSystemTypes ();
			var actions = from type in types
						  where type.Name == actionClass
						  select System.Activator.CreateInstance (type, BusinessActionResolver.noArguments) as GenericBusinessAction;

			return actions;
		}

		[System.ThreadStatic]
		private static Dictionary<string, GenericBusinessAction> actionCache;
		private static readonly object[] noArguments = new object[] { };
	}
}