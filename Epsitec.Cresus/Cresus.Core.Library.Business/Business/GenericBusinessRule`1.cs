//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business
{
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
	}
}
