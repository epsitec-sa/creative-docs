//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.BusinessLogic;

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
			var baseTypeName = "GenericBusinessRule`1";

			var types = from assembly in System.AppDomain.CurrentDomain.GetAssemblies ()
						from type in assembly.GetTypes ()
						where type.IsClass && !type.IsAbstract && type.GetCustomAttributes (typeof (BusinessRuleAttribute), false).Length > 0
						let baseType = type.BaseType
						where baseType.IsGenericType && baseType.Name.StartsWith (baseTypeName) && baseType.GetGenericArguments ()[0] == entityType
						select type;

			types = types.Where (type => type.GetCustomAttributes (typeof (BusinessRuleAttribute), false).Cast<BusinessRuleAttribute> ().Any (attribute => attribute.RuleType == ruleType));

			return types;
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