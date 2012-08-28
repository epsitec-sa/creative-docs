//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business
{
	/// <summary>
	/// The <c>GenericBusinessRule</c> class is the base class for the real implementations
	/// of the various business rules.
	/// </summary>
	/// <typeparam name="T">The entity type on which these rules apply.</typeparam>
	public abstract class GenericBusinessRule<T> : GenericBusinessRule
		where T : class
	{
		protected GenericBusinessRule()
		{
		}

		public sealed override System.Type EntityType
		{
			get
			{
				return typeof (T);
			}
		}

		public sealed override void Apply(RuleType ruleType, AbstractEntity entity)
		{
			System.Diagnostics.Debug.Assert (entity is T);

			this.Apply (ruleType, entity as T);
		}

		/// <summary>
		/// Applies the specified business rule.
		/// </summary>
		/// <param name="ruleType">The business rule which must be applied on the entity.</param>
		/// <param name="entity">The entity.</param>
		protected void Apply(RuleType ruleType, T entity)
		{
			switch (ruleType)
			{
				case Business.RuleType.Bind:
					this.ApplyBindRule (entity);
					break;

				case Business.RuleType.Setup:
					this.ApplySetupRule (entity);
					break;

				case Business.RuleType.Update:
					this.ApplyUpdateRule (entity);
					break;

				case Business.RuleType.Validate:
					this.ApplyValidateRule (entity);
					break;

				case Business.RuleType.Unbind:
					this.ApplyUnbindRule (entity);
					break;

				default:
					throw new System.NotSupportedException (string.Format ("RuleType.{0} not supported", ruleType));
			}
		}
		
		public virtual void ApplySetupRule(T entity)
		{
			//	The name of this method is also referenced by BusinessRuleResolver.GetMethodName
		}

		public virtual void ApplyBindRule(T entity)
		{
			//	The name of this method is also referenced by BusinessRuleResolver.GetMethodName
		}

		public virtual void ApplyUpdateRule(T entity)
		{
			//	The name of this method is also referenced by BusinessRuleResolver.GetMethodName
		}

		public virtual void ApplyValidateRule(T entity)
		{
			//	The name of this method is also referenced by BusinessRuleResolver.GetMethodName
		}

		public virtual void ApplyUnbindRule(T entity)
		{
			//	The name of this method is also referenced by BusinessRuleResolver.GetMethodName
		}
	}
}
