//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Workflows;

namespace Epsitec.Cresus.Core.Resolvers
{
	/// <summary>
	/// The <c>BusinessActionResolver</c> returns a business action implementation.
	/// </summary>
	public static class WorkflowActionResolver
	{
		/// <summary>
		/// Resolves an action class and returns its type, if it can be found.
		/// </summary>
		/// <param name="actionClass">The action class.</param>
		/// <returns>The type of the action class, or <c>null</c>.</returns>
		public static System.Type ResolveActionClass(string actionClass)
		{
			System.Type action;
			
			if (WorkflowActionResolver.actionCache == null)
			{
				WorkflowActionResolver.actionCache = new Dictionary<string, System.Type> ();
			}
			else
			{
				if (WorkflowActionResolver.actionCache.TryGetValue (actionClass, out action))
				{
					return action;
				}
			}

			action = WorkflowActionResolver.FindBusinessActionSystemType (actionClass);

			WorkflowActionResolver.actionCache[actionClass] = action;
			
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
			int suffixLen = WorkflowActionResolver.ClassSuffixActions.Length;

			return from type in WorkflowActionResolver.FindBusinessActionSystemTypes ()
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
			return WorkflowActionResolver.GetActionVerbs (WorkflowActionResolver.ResolveActionClass (actionClass));
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
					var attributes = method.GetCustomAttributes<WorkflowActionAttribute> ();

					if (attributes.IsEmpty ())
					{
						yield return new ActionVerb (method);
					}
					else
					{
						foreach (var attribute in attributes)
						{
							yield return new ActionVerb (method, attribute);
						}
					}
				}
			}
		}


		private static IEnumerable<System.Type> FindBusinessActionSystemTypes()
		{
			var types = from type in TypeEnumerator.Instance.GetAllTypes ()
						where type.IsStaticClass () && type.Name.EndsWith (WorkflowActionResolver.ClassSuffixActions)
						select type;

			return types;
		}

		private static System.Type FindBusinessActionSystemType(string actionClass)
		{
			actionClass = actionClass + WorkflowActionResolver.ClassSuffixActions;

			var types = from type in TypeEnumerator.Instance.GetAllTypes ()
						where type.IsStaticClass () && type.Name == actionClass
						select type;

			return types.FirstOrDefault ();
		}

		private const string ClassSuffixActions = "Actions";
		
		[System.ThreadStatic]
		private static Dictionary<string, System.Type>	actionCache;
	}
}