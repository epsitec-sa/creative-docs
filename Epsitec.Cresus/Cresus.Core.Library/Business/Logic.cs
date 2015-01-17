//	Copyright � 2010-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business
{
	/// <summary>
	/// The <c>Logic</c> class is used to apply business logic rules in a given
	/// <see cref="BusinessContext"/>.
	/// </summary>
	public sealed class Logic : ICoreComponentHost<ICoreManualComponent>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Logic"/> class. Don't create
		/// an instance yourself; call <see cref="IBusinessContext.CreateLogic"/> instead.
		/// </summary>
		/// <param name="entity">The root entity (if any).</param>
		/// <param name="components">The components (such as <see cref="BusinessContext"/>
		/// and <see cref="RefIdGeneratorPool"/>).</param>
		public Logic(AbstractEntity entity, params ICoreManualComponent[] components)
		{
			this.entity = entity;
			this.entityType = entity == null ? null : entity.GetType ();
			this.rules = new Dictionary<RuleType, GenericBusinessRule> ();
			this.components = new CoreComponentHostImplementation<ICoreManualComponent> ();
			this.components.RegisterComponents (components);
		}


		/// <summary>
		/// Applies the business rule to the specified entity.
		/// </summary>
		/// <param name="ruleType">The business rule.</param>
		/// <param name="entity">The entity.</param>
		public void ApplyRule(RuleType ruleType, AbstractEntity entity, bool disableBusinessRuleExceptions = false)
		{
			GenericBusinessRule rule   = this.ResolveRule (ruleType);
			System.Action       action = () => rule.Apply (ruleType, entity);

			bool savedMode = this.disableBusinessRuleExceptions;
			
			this.disableBusinessRuleExceptions = disableBusinessRuleExceptions;
			
			try
			{
				this.ApplyAction (action);
			}
			finally
			{
				this.disableBusinessRuleExceptions = savedMode;
			}
		}

		/// <summary>
		/// Applies an action in this business logic's context. This will temporarily define
		/// <see cref="Logic.Current"/> to refer to this instance.
		/// </summary>
		/// <param name="action">The action to execute.</param>
		public void ApplyAction(System.Action action)
		{
			System.Diagnostics.Debug.Assert (this.link == null);

			this.link = Logic.current;

			Logic.current = this;

			try
			{
				action ();
			}
			finally
			{
				Logic.current = this.link;
				this.link = null;
			}
		}


		/// <summary>
		/// Finds all entities of the given type.
		/// </summary>
		/// <typeparam name="T">The type of the entity.</typeparam>
		/// <returns>A collection of entities, which might be empty.</returns>
		public IEnumerable<T> Find<T>()
			where T : AbstractEntity
		{
			return this.Find ().OfType<T> ();
		}

		/// <summary>
		/// Finds all root entities directly attached to this instance, or to a linked
		/// instance of <see cref="Logic"/>.
		/// </summary>
		/// <returns>A collection of entities, which might be empty.</returns>
		public IEnumerable<AbstractEntity> Find()
		{
			var logic = this;

			while (logic != null)
			{
				if (logic.entity != null)
				{
					yield return logic.entity;
				}
				
				logic = logic.link;
			}
		}

		#region ICoreComponentHost<ICoreComponent> Members

		public bool ContainsComponent<T>()
			where T : ICoreManualComponent
		{
			return this.components.ContainsComponent<T> ();
		}

		public T GetComponent<T>()
			where T : ICoreManualComponent
		{
			return this.components.GetComponent<T> ();
		}

		ICoreManualComponent ICoreComponentHost<ICoreManualComponent>.GetComponent(System.Type type)
		{
			return this.components.GetComponent (type);
		}

		IEnumerable<ICoreManualComponent> ICoreComponentHost<ICoreManualComponent>.GetComponents()
		{
			return this.components.GetComponents ();
		}

		void ICoreComponentHost<ICoreManualComponent>.RegisterComponent<T>(T component)
		{
			this.components.RegisterComponent<T> (component);
		}

		bool ICoreComponentHost<ICoreManualComponent>.ContainsComponent(System.Type type)
		{
			return this.components.ContainsComponent (type);
		}

		void ICoreComponentHost<ICoreManualComponent>.RegisterComponent(System.Type type, ICoreManualComponent component)
		{
			this.components.RegisterComponent (type, component);
		}

		void ICoreComponentHost<ICoreManualComponent>.RegisterComponentAsDisposable(System.IDisposable disposable)
		{
			this.components.RegisterComponentAsDisposable (disposable);
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





		/// <summary>
		/// Gets a value indicating whether the business logic context is not available.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the business logic context is not available; otherwise, <c>false</c>.
		/// </value>
		public static bool						IsNotAvailable
		{
			get
			{
				return Logic.current == null;
			}
		}

		public static bool						IsAvailable
		{
			get
			{
				return Logic.current != null;
			}
		}

		/// <summary>
		/// Gets the current business logic context.
		/// </summary>
		public static Logic						Current
		{
			get
			{
				return Logic.current;
			}
		}

		public static void BusinessRuleException(string message)
		{
			Logic.BusinessRuleException<AbstractEntity> (null, message);
		}

		public static void BusinessRuleException(FormattedText message)
		{
			Logic.BusinessRuleException<AbstractEntity> (null, message);
		}

		public static void BusinessRuleException<T>(T entity, string message)
			where T : AbstractEntity
		{
			if ((Logic.current != null) &&
				(Logic.current.disableBusinessRuleExceptions))
			{
				System.Diagnostics.Trace.WriteLine (
					string.Format ("BusinessRuleException on entity #{0} / {1}.\n{2}",
					entity == null ? "<null>" : entity.GetEntitySerialId ().ToString (),
					typeof (T).FullName,
					message));
			}
			else
			{
				throw new BusinessRuleException (entity, message);
			}
		}

		public static void BusinessRuleException<T>(T entity, FormattedText message)
			where T : AbstractEntity
		{
			if ((Logic.current != null) &&
				(Logic.current.disableBusinessRuleExceptions))
			{
				System.Diagnostics.Trace.WriteLine (string.Format ("BusinessRuleException on entity #{0} / {1}.\n{2}",
					entity == null ? "<null>" : entity.GetEntitySerialId ().ToString (),
					typeof (T).FullName,
					message.ToSimpleText ()));
			}
			else
			{
				throw new BusinessRuleException (entity, message);
			}
		}

		
		
		[System.ThreadStatic]
		private static Logic					current;

		private readonly AbstractEntity			entity;
		private readonly System.Type			entityType;
		private readonly Dictionary<RuleType, GenericBusinessRule> rules;
		private readonly CoreComponentHostImplementation<ICoreManualComponent> components;
		private Logic							link;
		private bool							disableBusinessRuleExceptions;
	}
}
