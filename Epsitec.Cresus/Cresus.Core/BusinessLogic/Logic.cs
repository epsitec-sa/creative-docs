//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.BusinessLogic
{
	public sealed class Logic
	{
		public Logic(System.Type entityType)
		{
			this.entityType = entityType;
			this.rules = new Dictionary<RuleType, GenericBusinessRule> ();
		}


		public void ApplyRules(RuleType ruleType, AbstractEntity entity)
		{
			var rule = this.ResolveRule (ruleType);
			var link = Logic.current;

			Logic.current = this;

			try
			{
				rule.Apply (entity);
			}
			finally
			{
				Logic.current = link;
			}
		}




		private GenericBusinessRule ResolveRule(RuleType ruleType)
		{
			GenericBusinessRule rule;

			if (this.rules.TryGetValue (ruleType, out rule))
			{
				return rule;
			}

			rule = BusinessRuleResolver.Resolve (this.entityType, ruleType);

			this.rules[ruleType] = rule;

			return rule;
		}


		public static Logic Current
		{
			get
			{
				return Logic.current;
			}
		}

		[System.ThreadStatic]
		private static Logic current;
		
		private readonly System.Type entityType;
		private readonly Dictionary<RuleType, GenericBusinessRule> rules;
	}
}
