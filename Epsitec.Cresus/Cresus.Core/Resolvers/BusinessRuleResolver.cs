//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Resolvers
{
	/// <summary>
	/// The <c>BusinessRuleResolver</c> returns a business rule implementation for the specified
	/// entity type and rule type.
	/// </summary>
	public static class BusinessRuleResolver
	{
		/// <summary>
		/// Resolves a business rule for the specified entity and rule types. This returns a
		/// <see cref="CompositeBusinessRule"/> with zero, one or more simple rules.
		/// </summary>
		/// <param name="entityType">Type of the entity.</param>
		/// <param name="ruleType">Type of the rule.</param>
		/// <returns>The business rule.</returns>
		public static GenericBusinessRule Resolve(System.Type entityType, RuleType ruleType)
		{
			var businessRules = BusinessRuleResolver.CreateBusinessRules (entityType, ruleType);

			return new CompositeBusinessRule (entityType, businessRules);
		}


		private static IEnumerable<System.Type> FindBusinessRuleSystemTypes(System.Type entityType, RuleType ruleType)
		{
			string baseTypeName = "GenericBusinessRule`1";
			string methodName   = BusinessRuleResolver.GetMethodName (ruleType);

			var candidates = new HashSet<System.Type> ();

			candidates.Add (entityType);
			candidates.AddRange (entityType.GetInterfaces ());

			var types = from assembly in System.AppDomain.CurrentDomain.GetAssemblies ()
						from type in assembly.GetTypes ()
						where type.IsClass && !type.IsAbstract && type.GetCustomAttributes (typeof (BusinessRuleAttribute), false).Length > 0
						let baseType = type.BaseType
						where baseType.IsGenericType && baseType.Name.StartsWith (baseTypeName)
						let genericType = baseType.GetGenericArguments ()[0]
						where candidates.Contains (genericType) && BusinessRuleResolver.ImplementsRuleMethod (type, methodName)
						orderby (genericType.IsInterface ? 1 : 0) ascending
						orderby (genericType.Name)
						select type;

			return types;
		}

		private static bool ImplementsRuleMethod(System.Type type, string methodName)
		{
			var method = type.GetMethod (methodName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

			if ((method != null) &&
				(method.DeclaringType == type) &&
				(method.Attributes.HasFlag (System.Reflection.MethodAttributes.Virtual)) &&
				(method.Attributes.HasFlag (System.Reflection.MethodAttributes.VtableLayoutMask) == false) &&
				(method.ReturnType == typeof (void)))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Gets the name of the method as it is implemented in <see cref="GenericBusinessRule{T}"/>.
		/// </summary>
		/// <param name="ruleType">Business rule type.</param>
		/// <returns>The name of the method which implements the specified business rule.</returns>
		private static string GetMethodName(RuleType ruleType)
		{
			switch (ruleType)
			{
				case RuleType.Bind:
					return "ApplyBindRule";

				case RuleType.Setup:
					return "ApplySetupRule";

				case RuleType.Update:
					return "ApplyUpdateRule";

				case RuleType.Validate:
					return "ApplyValidateRule";

				default:
					throw new System.NotImplementedException ();
			}
		}

		private static IEnumerable<GenericBusinessRule> CreateBusinessRules(System.Type entityType, RuleType ruleType)
		{
			var types = BusinessRuleResolver.FindBusinessRuleSystemTypes (entityType, ruleType);

			var rules = from type in types
						select System.Activator.CreateInstance (type, BusinessRuleResolver.noArguments) as GenericBusinessRule;

			foreach (var rule in rules)
			{
				rule.RuleType = ruleType;
				yield return rule;
			}
		}


		private static readonly object[] noArguments = new object[] { };
	}
}