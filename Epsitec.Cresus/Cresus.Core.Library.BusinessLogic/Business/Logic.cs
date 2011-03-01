//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business
{
	public sealed class Logic : ICoreComponentHost<ICoreComponent>
	{
		public Logic(AbstractEntity entity, params ICoreComponent[] components)
		{
			this.entity = entity;
			this.entityType = entity.GetType ();
			this.rules = new Dictionary<RuleType, GenericBusinessRule> ();
			this.components = new CoreComponentHostImplementation<ICoreComponent> ();
			this.components.RegisterComponents (components);
		}


		public void ApplyRules(RuleType ruleType, AbstractEntity entity)
		{
			var rule = this.ResolveRule (ruleType);
			this.link = Logic.current;

			Logic.current = this;

			try
			{
				rule.Apply (ruleType, entity);
			}
			finally
			{
				Logic.current = this.link;
				this.link = null;
			}
		}


		public IEnumerable<T> Find<T>()
			where T : AbstractEntity
		{
			var logic = this;

			while (logic != null)
			{
				if (logic.entity is T)
				{
					yield return logic.entity as T;
				}

				logic = logic.link;
			}
		}


		#region ICoreComponentHost<ICoreComponent> Members

		public T GetComponent<T>() where T : ICoreComponent
		{
			return this.components.GetComponent<T> ();
		}

		public IEnumerable<ICoreComponent> GetComponents()
		{
			return this.components.GetComponents ();
		}

		public bool ContainsComponent<T>() where T : ICoreComponent
		{
			return this.components.ContainsComponent<T> ();
		}

		void ICoreComponentHost<ICoreComponent>.RegisterComponent<T>(T component)
		{
			this.components.RegisterComponent<T> (component);
		}

		#endregion

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

		private readonly AbstractEntity entity;
		private readonly System.Type entityType;
		private readonly Dictionary<RuleType, GenericBusinessRule> rules;
		private readonly CoreComponentHostImplementation<ICoreComponent> components;
		private Logic link;
	}
}
