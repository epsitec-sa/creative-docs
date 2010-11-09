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

		public static IEnumerable<string> GetActionClasses()
		{
			const string suffix = "Actions";
			int suffixLen = suffix.Length;

			return from type in BusinessActionResolver.FindBusinessActionSystemTypes ()
				   let name = type.Name
				   orderby name ascending
				   select name.Substring (0, name.Length - suffixLen);
		}

		public static IEnumerable<ActionVerb> GetActionVerbs(string actionClass)
		{
			System.Type type = BusinessActionResolver.Resolve (actionClass);

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
			var types = from assembly in System.AppDomain.CurrentDomain.GetAssemblies ()
						from type in assembly.GetTypes ()
						let name = type.Name
						where name.EndsWith ("Actions")
						where type.IsClass && type.IsAbstract && type.IsSealed
						select type;

			return types;
		}

		private static System.Type FindBusinessActionSystemType(string actionClass)
		{
			actionClass = actionClass + "Actions";

			var types = from assembly in System.AppDomain.CurrentDomain.GetAssemblies ()
						from type in assembly.GetTypes ()
						let name = type.Name
						where name == actionClass
						where type.IsClass && type.IsAbstract && type.IsSealed
						select type;

			return types.FirstOrDefault ();
		}

		[System.ThreadStatic]
		private static Dictionary<string, System.Type> actionCache;
		private static readonly object[] noArguments = new object[] { };
	}
}