//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe ValidationRule représente des règles de validation
	/// pour des commandes.
	/// </summary>
	public class ValidationRule : IValidator, ICommandDispatcherHost
	{
		internal ValidationRule(CommandDispatcher dispatcher) : this ("*", true)
		{
			this.dispatcher = dispatcher;
		}
		
		internal ValidationRule(string name, bool top_level)
		{
			this.name           = name;
			this.top_level      = top_level;
			this.command_states = new CommandStateList (this);
			this.validators     = new ValidatorList (this);
		}
		
		
		public ValidationRule() : this (null, false)
		{
		}
		
		public ValidationRule(string name) : this (name, false)
		{
		}
		
		public ValidationRule(IValidator validator, string command_states) : this (null, false)
		{
			this.AddValidator (validator);
			
			string[] names = command_states.Split (';', ',');
			
			foreach (string name in names)
			{
				this.AddCommandState (name.Trim ());
			}
		}
		
		
		
		public string								Name
		{
			get
			{
				return this.name == null ? "" : this.name;
			}
		}
		
		public ValidationRule.ValidatorList			Validators
		{
			get
			{
				return this.validators;
			}
		}
		
		public ValidationRule.CommandStateList		CommandStates
		{
			get
			{
				return this.command_states;
			}
		}
		
		
		public void AddValidator(IValidator validator)
		{
			this.validators.Add (validator);
		}
		
		public void AddCommandState(string name)
		{
			this.command_states.Add (name);
		}
		
		public void AddCommandState(CommandState command_state)
		{
			this.command_states.Add (command_state);
		}
		
		public void DefineDispatcher(CommandDispatcher value)
		{
			if (this.dispatcher != value)
			{
				if (this.dispatcher == null)
				{
					this.dispatcher = value;
					this.OnDispatcherDefined ();
				}
				else
				{
					throw new System.InvalidOperationException ("CommandDispatcher cannot be changed.");
				}
			}
		}
		
		#region IValidator Members
		public ValidationState						State
		{
			get
			{
				return this.state;
			}
		}
		
		public bool									IsValid
		{
			get
			{
				if (this.state == ValidationState.Dirty)
				{
					this.Validate ();
				}
				
				return (this.state == ValidationState.Ok);
			}
		}
		
		public string								ErrorMessage
		{
			get
			{
				//	TODO: détermine le message en fonction des dépendances...
				
				return null;
			}
		}
		
		
		public event Support.EventHandler			BecameDirty;
		
		public void Validate()
		{
			this.old_state = this.state;
			this.state     = ValidationState.Unknown;
			
			bool valid   = true;
			bool unknown = false;
			
			//	Recalcule l'état... Pendant toute la phase de recalcul, on passe dans un
			//	état spécial (Unknown) pour éviter que des boucles dans les dépendances ne
			//	puissent causer des récursions.
			
			foreach (IValidator validator in this.validators)
			{
				if (! validator.IsValid)
				{
					//	Oups... L'un des éléments dont nous dépendons n'est pas valide; c'est
					//	soit qu'il est dans un état inconnu, soit qu'il n'est réellement pas
					//	valide.
					
					if (validator.State == ValidationState.Unknown)
					{
						unknown = true;
					}
					
					valid = false;
				}
			}
			
			if (unknown)
			{
				this.UpdateState (ValidationState.Unknown);
			}
			else
			{
				this.UpdateState (valid ? ValidationState.Ok : ValidationState.Error);
			}
		}
		
		public void MakeDirty(bool deep)
		{
			this.state = ValidationState.Dirty;
			this.OnBecameDirty ();
			
			if (deep)
			{
				foreach (IValidator validator in this.validators)
				{
					validator.MakeDirty (deep);
				}
			}
		}
		#endregion
		
		#region ICommandDispatcherHost Members
		public IEnumerable<CommandDispatcher> GetCommandDispatchers()
		{
			if (this.dispatcher != null)
			{
				yield return this.dispatcher;
			}
		}
		#endregion
		
		#region ValidatorList Class
		public class ValidatorList : System.Collections.IList
		{
			public ValidatorList(ValidationRule host)
			{
				this.host = host;
			}
			
			
			public IValidator						this[int index]
			{
				get
				{
					return this.list[index] as IValidator;
				}
			}
			
			public ValidationRule					ValidationRule
			{
				get
				{
					return this.host;
				}
			}
			
			
			public int Add(IValidator value)
			{
				this.Attach (value as IValidator);
				int index = this.list.Add (value);
				this.OnListChanged ();
				return index;
			}
			
			
			#region IList Members
			public bool								IsReadOnly
			{
				get
				{
					return this.list.IsReadOnly;
				}
			}
			
			public bool								IsFixedSize
			{
				get
				{
					return this.list.IsFixedSize;
				}
			}
			
			object									System.Collections.IList.this[int index]
			{
				get
				{
					return this.list[index];
				}
				set
				{
					throw new System.InvalidOperationException ("Cannot replace element in ValidationRuleList.");
				}
			}
			
			
			int System.Collections.IList.Add(object value)
			{
				this.Attach (value as IValidator);
				int index = this.list.Add (value);
				this.OnListChanged ();
				return index;
			}
			
			public void Insert(int index, object value)
			{
				this.Attach (value as IValidator);
				this.list.Insert (index, value);
				this.OnListChanged ();
			}
			
			public void Remove(object value)
			{
				this.Detach (value as IValidator);
				this.list.Remove (value);
				this.OnListChanged ();
			}
			
			public void RemoveAt(int index)
			{
				this.Remove (this[index]);
			}
			
			public void Clear()
			{
				IValidator[] validators = new IValidator[this.list.Count];
				this.list.CopyTo (validators);
				
				for (int i = 0; i < validators.Length; i++)
				{
					this.Remove (validators[i]);
				}
			}
			
			public bool Contains(object value)
			{
				return this.list.Contains (value);
			}
			
			public int IndexOf(object value)
			{
				return this.list.IndexOf (value);
			}
			#endregion
			
			#region ICollection Members
			public bool								IsSynchronized
			{
				get
				{
					return this.list.IsSynchronized;
				}
			}
			
			public int								Count
			{
				get
				{
					return this.list.Count;
				}
			}
			
			public object							SyncRoot
			{
				get
				{
					return this.list.SyncRoot;
				}
			}
			
			public void CopyTo(System.Array array, int index)
			{
				this.list.CopyTo (array, index);
			}
			#endregion
			
			#region IEnumerable Members
			public System.Collections.IEnumerator GetEnumerator()
			{
				return this.list.GetEnumerator ();
			}
			#endregion
			
			protected virtual void Attach(IValidator value)
			{
				if (this.list.Contains (value))
				{
					throw new System.InvalidOperationException ("Cannot insert twice same IValidator.");
				}
				if (value == null)
				{
					throw new System.ArgumentNullException ("value", "Expected an IValidator.");
				}
				
				value.BecameDirty += new Support.EventHandler (this.HandleBecameDirty);

#if false //#fix
				if (this.host.CommandDispatchers != null)
				{
					//	Le validateur a été ajouté à une règle qui est liée à un CommandDispatcher
					//	particulier; si le validateur implémente le support pour le CommandDispatcher,
					//	alors on le met à jour.
					
					ICommandDispatcherHost cdh = value as ICommandDispatcherHost;
					
					if (cdh != null)
					{
						if (cdh.CommandDispatcher == null)
						{
							cdh.CommandDispatcher = this.host.CommandDispatcher;
						}
					}
				}
#endif
			}
			
			protected virtual void Detach(IValidator value)
			{
				if (! this.list.Contains (value))
				{
					throw new System.InvalidOperationException ("Cannot remove IValidator, not found.");
				}
				if (value == null)
				{
					throw new System.ArgumentNullException ("value", "Expected an IValidator.");
				}
				
				value.BecameDirty -= new Support.EventHandler (this.HandleBecameDirty);
			}
			
			protected virtual void OnListChanged()
			{
				this.host.OnValidatorsChanged ();
			}
			
			
			private void HandleBecameDirty(object sender)
			{
				this.host.MakeDirty (false);
			}
			
			
			protected ValidationRule				host;
			protected System.Collections.ArrayList	list = new System.Collections.ArrayList ();
		}
		#endregion
		
		#region CommandStateList Class
		public class CommandStateList : System.Collections.IList
		{
			public CommandStateList(ValidationRule host)
			{
				this.host = host;
			}
			
			
			public CommandState					this[int index]
			{
				get
				{
					return this.list[index] as CommandState;
				}
			}
			
			public ValidationRule				ValidationRule
			{
				get
				{
					return this.host;
				}
			}
			
			
			public int Add(CommandState value)
			{
				int index = this.list.Add (value);
				this.OnListChanged ();
				return index;
			}
			
			public void Add(string value)
			{
#if false //#fix
				if (dispatcher != null)
				{
					this.Add (dispatcher.GetCommandState (value));
				}
				else
				{
					if (this.names == null)
					{
						this.names = new System.Collections.ArrayList ();
					}
					
					this.names.Add (value);
				}
#endif
			}
			
			
			#region IList Members
			public bool							IsReadOnly
			{
				get
				{
					return this.list.IsReadOnly;
				}
			}
			
			public bool							IsFixedSize
			{
				get
				{
					return this.list.IsFixedSize;
				}
			}
			
			object								System.Collections.IList.this[int index]
			{
				get
				{
					return this.list[index];
				}
				set
				{
					throw new System.InvalidOperationException ("Cannot replace element in ValidationRuleList.");
				}
			}
			
			
			int System.Collections.IList.Add(object value)
			{
				int index = this.list.Add (value);
				this.OnListChanged ();
				return index;
			}
			
			public void Insert(int index, object value)
			{
				this.list.Insert (index, value);
				this.OnListChanged ();
			}
			
			public void Remove(object value)
			{
				this.list.Remove (value);
				this.OnListChanged ();
			}
			
			public void RemoveAt(int index)
			{
				this.list.RemoveAt (index);
				this.OnListChanged ();
			}
			
			public void Clear()
			{
				this.list.Clear ();
				this.OnListChanged ();
			}
			
			public bool Contains(object value)
			{
				return this.list.Contains (value);
			}
			
			public int IndexOf(object value)
			{
				return this.list.IndexOf (value);
			}
			#endregion
			
			#region ICollection Members
			public bool							IsSynchronized
			{
				get
				{
					return this.list.IsSynchronized;
				}
			}
			
			public int							Count
			{
				get
				{
					return this.list.Count;
				}
			}
			
			public object						SyncRoot
			{
				get
				{
					return this.list.SyncRoot;
				}
			}
			
			public void CopyTo(System.Array array, int index)
			{
				this.list.CopyTo (array, index);
			}
			#endregion
			
			#region IEnumerable Members
			public System.Collections.IEnumerator GetEnumerator()
			{
				return this.list.GetEnumerator ();
			}
			#endregion
			
			protected virtual void OnListChanged()
			{
				this.host.OnCommandStatesChanged ();
			}
			
			
			internal void Compile(CommandDispatcher dispatcher)
			{
				if (this.names != null)
				{
					foreach (string name in this.names)
					{
						this.Add (dispatcher.GetCommandState (name));
					}
					
					this.names.Clear ();
					this.names = null;
				}
			}
			
			
			protected ValidationRule				host;
			protected System.Collections.ArrayList	list = new System.Collections.ArrayList ();
			protected System.Collections.ArrayList	names;
		}
		#endregion
		
		protected virtual void UpdateState(ValidationState new_state)
		{
			System.Diagnostics.Debug.Assert (new_state != ValidationState.Dirty);
			
			this.state = new_state;
			
			if (this.old_state != this.state)
			{
				this.UpdateCommandStates ();
			}
		}
		
		protected virtual void UpdateCommandStates()
		{
			//	L'état final a changé; il faut donc mettre à jour les diverses commandes
			//	qui dépendent de cette règle de validation :
			
			bool is_valid = this.IsValid;
			
			foreach (CommandState command_state in this.command_states)
			{
				command_state.Enable = is_valid;
			}
		}
		
		
		protected virtual void OnBecameDirty()
		{
			if (this.BecameDirty != null)
			{
				this.BecameDirty (this);
			}
			if (this.top_level)
			{
				if (this.dispatcher != null)
				{
					this.dispatcher.NotifyValidationRuleBecameDirty ();
				}
			}
		}
		
		protected virtual void OnValidatorsChanged()
		{
			this.MakeDirty (false);
		}
		
		protected virtual void OnCommandStatesChanged()
		{
			this.MakeDirty (false);
		}
		
		protected virtual void OnDispatcherDefined()
		{
			foreach (object o in this.validators)
			{
				ICommandDispatcherHost cdh = o as ICommandDispatcherHost;
				
				if (cdh != null)
				{
#if false //#fix
					if (cdh.CommandDispatcher == null)
					{
						cdh.CommandDispatcher = this.dispatcher;
					}
#endif
				}
			}
			
			this.command_states.Compile (this.dispatcher);
		}
		
		
		protected string							name;
		protected bool								top_level;
		protected ValidationState					state = ValidationState.Dirty;
		protected ValidationState					old_state = ValidationState.Unknown;
		protected ValidationRule.CommandStateList	command_states;
		protected ValidationRule.ValidatorList		validators;
		protected CommandDispatcher					dispatcher;
	}
}
