//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 08/03/2004

namespace Epsitec.Common.Support
{
	using CommandState = CommandDispatcher.CommandState;
	
	/// <summary>
	/// La classe ValidationRule représente des règles de validation
	/// pour des commandes.
	/// </summary>
	public class ValidationRule : IValidator
	{
		internal ValidationRule(CommandDispatcher dispatcher) : this ("*")
		{
			this.dispatcher = dispatcher;
		}
		
		public ValidationRule(string name)
		{
			this.name           = name;
			this.command_states = new CommandStateList (this);
			this.validators     = new ValidatorList (this);
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
		
		
		public event EventHandler					BecameDirty;
		
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
			if (this.state != ValidationState.Dirty)
			{
				this.state = ValidationState.Dirty;
				this.OnBecameDirty ();
			}
			
			if (deep)
			{
				foreach (IValidator validator in this.validators)
				{
					validator.MakeDirty (deep);
				}
			}
		}
		#endregion
		
		#region ValidatorList class
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
				
				value.BecameDirty += new EventHandler (this.HandleBecameDirty);
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
				
				value.BecameDirty -= new EventHandler (this.HandleBecameDirty);
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
		
		#region CommandStateList class
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
			
			
			protected ValidationRule				host;
			protected System.Collections.ArrayList	list = new System.Collections.ArrayList ();
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
			
			foreach (CommandState command_state in this.command_states)
			{
				command_state.Enabled = this.IsValid;
			}
		}
		
		
		protected virtual void OnBecameDirty()
		{
			if (this.BecameDirty != null)
			{
				this.BecameDirty (this);
			}
			if (this.dispatcher != null)
			{
				this.dispatcher.NotifyValidationRulesBecameDirty ();
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
		
		
		protected string							name;
		protected ValidationState					state = ValidationState.Dirty;
		protected ValidationState					old_state = ValidationState.Unknown;
		protected ValidationRule.CommandStateList	command_states;
		protected ValidationRule.ValidatorList		validators;
		protected CommandDispatcher					dispatcher;
	}
}
