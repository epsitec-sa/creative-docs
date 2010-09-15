//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.DataLayer.Context;

namespace Epsitec.Cresus.Core.BusinessLogic
{
	public sealed class Logic
	{
		internal Logic(System.Type entityType, BusinessContext businessContext)
		{
			this.entityType = entityType;
			this.rules = new Dictionary<RuleType, GenericBusinessRule> ();
			this.businessContext = businessContext;
		}


		public BusinessContext BusinessContext
		{
			get
			{
				return this.businessContext;
			}
		}

		public DataContext DataContext
		{
			get
			{
				return this.businessContext.DataContext;
			}
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

			rule = Resolvers.BusinessRuleResolver.Resolve (this.entityType, ruleType);

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
		private readonly BusinessContext businessContext;
	}
}
