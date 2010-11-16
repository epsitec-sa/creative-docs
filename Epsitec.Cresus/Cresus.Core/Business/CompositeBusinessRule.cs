//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business
{
	/// <summary>
	/// A <c>CompositeBusinessRule</c> implements zero, one or several rules for a specified
	/// entity type. It is instantiated by the <see cref="BusinessRuleResolver"/>.
	/// </summary>
	public class CompositeBusinessRule : GenericBusinessRule
	{
		internal CompositeBusinessRule(System.Type entityType, IEnumerable<GenericBusinessRule> rules)
		{
			this.rules = new List<GenericBusinessRule> (rules);
			this.entityType = entityType;
		}

		
		public override System.Type EntityType
		{
			get
			{
				return this.entityType;
			}
		}

		
		public override void Apply(RuleType ruleType, AbstractEntity entity)
		{
			this.rules.ForEach (rule => rule.Apply (ruleType, entity));
		}

		
		private readonly List<GenericBusinessRule> rules;
		private readonly System.Type entityType;
	}
}
