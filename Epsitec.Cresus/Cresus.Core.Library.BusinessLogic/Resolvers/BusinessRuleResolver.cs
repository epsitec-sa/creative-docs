//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

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
		/// <returns>The business rule if one exists for the entity type; otherwise, <c>null</c>.</returns>
		public static GenericBusinessRule Resolve(System.Type entityType, RuleType ruleType)
		{
			if (entityType == null)
			{
				return null;
			}

			var businessRules = BusinessRuleResolver.CreateBusinessRules (entityType, ruleType);

			return new CompositeBusinessRule (entityType, businessRules);
		}


		private static IEnumerable<System.Type> FindBusinessRuleSystemTypes(System.Type entityType, RuleType ruleType)
		{
			string baseTypeName = "GenericBusinessRule`1";
			string methodName   = BusinessRuleResolver.GetMethodName (ruleType);

			var candidates = new HashSet<TypeRank> (BusinessRuleResolver.GetBaseTypesAndInterfaces (entityType));

			var types = from type in TypeEnumerator.Instance.GetAllTypes ()
						where type.IsClass && !type.IsAbstract && type.GetCustomAttributes<BusinessRuleAttribute> ().Any ()
						let baseType = type.BaseType
						where baseType.IsGenericType && baseType.Name.StartsWith (baseTypeName)
						let genericType = baseType.GetGenericArguments ()[0]
						where candidates.Contains (new TypeRank (genericType)) && BusinessRuleResolver.ImplementsRuleMethod (type, methodName)
						orderby candidates.First (x => x.Type == genericType) descending
						select type;

			return types;
		}

		private static IEnumerable<TypeRank> GetBaseTypesAndInterfaces(System.Type type)
		{
			HashSet<System.Type> interfaces = new HashSet<System.Type> ();
			return BusinessRuleResolver.GetBaseTypesAndInterfaces (type, interfaces, 0);
		}

		private static IEnumerable<TypeRank> GetBaseTypesAndInterfaces(System.Type type, HashSet<System.Type> baseInterfaces, int depth)
		{
			if (type != typeof (AbstractEntity))
			{
				yield return new TypeRank (type, depth*2 + 0);
			}

			var baseType = type.BaseType;

			if ((baseType != null) &&
				(baseType != type) &&
				(baseType != typeof (object)))
			{
				foreach (var result in BusinessRuleResolver.GetBaseTypesAndInterfaces (baseType, baseInterfaces, depth+1))
				{
					yield return result;
				}
			}

			foreach (var typeInterface in type.GetInterfaces ().Where (x => baseInterfaces.Add (x)))
			{
				if (type != typeof (AbstractEntity))
				{
					yield return new TypeRank (typeInterface, depth*2 + 1);
				}
			}
		}

		#region TypeRank Structure

		struct TypeRank : System.IComparable<TypeRank>, System.IEquatable<TypeRank>
		{
			public TypeRank(System.Type type, int rank = -1)
			{
				this.type = type;
				this.rank = rank;
			}

			public System.Type Type
			{
				get
				{
					return this.type;
				}
			}

			public override bool Equals(object obj)
			{
				return this.Equals ((TypeRank) obj);
			}

			public override int GetHashCode()
			{
				return this.type.GetHashCode ();
			}

			public override string ToString()
			{
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}:{1}", this.rank, this.type.FullName);
			}

			#region IEquatable<TypeRank> Members

			public bool Equals(TypeRank other)
			{
				return other.type == this.type;
			}

			#endregion

			#region IComparable<TypeRank> Members

			public int CompareTo(TypeRank other)
			{
				if (this.rank < other.rank)
				{
					return -1;
				}
				else if (this.rank > other.rank)
				{
					return 1;
				}
				else
				{
					return string.CompareOrdinal (this.type.Name, other.type.Name);
				}
			}

			#endregion

			private readonly System.Type type;
			private readonly int rank;
		}

		#endregion

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
			return string.Format ("Apply{0}Rule", ruleType);
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