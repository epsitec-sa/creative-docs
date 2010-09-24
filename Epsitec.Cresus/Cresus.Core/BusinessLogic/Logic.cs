//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

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

		public CoreData Data
		{
			get
			{
				return this.BusinessContext.Data;
			}
		}

		public CoreApplication Application
		{
			get
			{
				return CoreProgram.Application;
			}
		}

		public BusinessSettingsEntity BusinessSettings
		{
			get
			{
				return this.Application.BusinessSettings;
			}
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


		public IEnumerable<T> GetAllEntities<T>(Extraction extraction = Extraction.Default)
			where T : AbstractEntity, new ()
		{
			return this.Data.GetAllEntities<T> (extraction);
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
