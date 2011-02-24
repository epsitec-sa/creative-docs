//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
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
		public static System.Type Resolve(string actionClass)
		{
			System.Type action;
			
			if (BusinessActionResolver.actionCache == null)
			{
				BusinessActionResolver.actionCache = new Dictionary<string, System.Type> ();
			}
			else
			{
				if (BusinessActionResolver.actionCache.TryGetValue (actionClass, out action))
				{
					return action;
				}
			}

			action = BusinessActionResolver.FindBusinessActionSystemType (actionClass);

			BusinessActionResolver.actionCache[actionClass] = action;
			
			return action;
		}

		/// <summary>
		/// Gets the action classes currently available.
		/// </summary>
		/// <returns>
		/// The collection of action classes specified by their names.
		/// </returns>
		public static IEnumerable<string> GetActionClasses()
		{
			int suffixLen = BusinessActionResolver.ClassSuffixActions.Length;

			return from type in BusinessActionResolver.FindBusinessActionSystemTypes ()
				   let name = type.Name
				   orderby name ascending
				   select name.Substring (0, name.Length - suffixLen);
		}

		/// <summary>
		/// Gets the action verbs for a specified action class.
		/// </summary>
		/// <param name="actionClass">The action class.</param>
		/// <returns>
		/// The collection of action verbs for the specified action class.
		/// </returns>
		public static IEnumerable<ActionVerb> GetActionVerbs(string actionClass)
		{
			return BusinessActionResolver.GetActionVerbs (BusinessActionResolver.Resolve (actionClass));
		}

		/// <summary>
		/// Gets the action verbs for a specified action class.
		/// </summary>
		/// <param name="type">The action class type.</param>
		/// <returns>
		/// The collection of action verbs for the specified action class.
		/// </returns>
		public static IEnumerable<ActionVerb> GetActionVerbs(System.Type type)
		{
			if (type != null)
			{
				var methods = type.GetMethods (System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

				foreach (var method in methods.Where (x => (x.GetParameters ().Length == 0) && (x.ReturnType == typeof (void))))
				{
					var attributes = method.GetCustomAttributes (typeof (ActionAttribute), inherit: false);

					if (attributes.Length == 0)
					{
						yield return new ActionVerb (method);
					}
					else
					{
						yield return new ActionVerb (((ActionAttribute)(attributes[0])).PublishedName, method);
					}
				}
			}
		}


		private static IEnumerable<System.Type> FindBusinessActionSystemTypes()
		{
			var types = from type in TypeEnumerator.Instance.GetAllTypes ()
						let name = type.Name
						where name.EndsWith (BusinessActionResolver.ClassSuffixActions)
						where type.IsClass && type.IsAbstract && type.IsSealed
						select type;

			return types;
		}

		private static System.Type FindBusinessActionSystemType(string actionClass)
		{
			actionClass = actionClass + BusinessActionResolver.ClassSuffixActions;

			var types = from type in TypeEnumerator.Instance.GetAllTypes ()
						let name = type.Name
						where name == actionClass
						where type.IsClass && type.IsAbstract && type.IsSealed
						select type;

			return types.FirstOrDefault ();
		}

		private const string ClassSuffixActions = "Actions";
		
		[System.ThreadStatic]
		private static Dictionary<string, System.Type>	actionCache;
	}
}