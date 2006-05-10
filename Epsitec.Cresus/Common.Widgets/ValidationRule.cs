//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe ValidationRule représente des règles de validation pour des
	/// commandes.
	/// </summary>
	public class ValidationRule : IValidator
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
			this.command_states.Add (CommandState.Get (name));
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
			this.NotifyBecameDirty ();
			
			if (deep)
			{
				foreach (IValidator validator in this.validators)
				{
					validator.MakeDirty (deep);
				}
			}
		}
		#endregion
		
		#region ValidatorList Class
		public class ValidatorList : Types.HostedList<IValidator>
		{
			public ValidatorList(ValidationRule host) : base (host.NotifyValidatorsChanged, host.NotifyValidatorsChanged)
			{
				this.host = host;
			}

			protected override void NotifyBeforeInsertion(IValidator item)
			{
				base.NotifyBeforeInsertion (item);

				if (this.Contains (item))
				{
					throw new System.ArgumentException ("Duplicate insertion");
				}
				if (item == null)
				{
					throw new System.ArgumentNullException ("item", "Expected an IValidator");
				}
			}

			protected override void NotifyInsertion(IValidator item)
			{
				item.BecameDirty += this.HandleBecameDirty;
				
				base.NotifyInsertion (item);
			}

			protected override void NotifyRemoval(IValidator item)
			{
				base.NotifyRemoval (item);

				item.BecameDirty -= this.HandleBecameDirty;
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
		public class CommandStateList : Types.HostedList<CommandState>
		{
			public CommandStateList(ValidationRule host) : base (host.NotifyCommandStatesChanged, host.NotifyCommandStatesChanged)
			{
			}
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
		
		
		protected virtual void NotifyBecameDirty()
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
		
		protected virtual void NotifyValidatorsChanged(IValidator item)
		{
			this.MakeDirty (false);
		}
		
		protected virtual void NotifyCommandStatesChanged(CommandState item)
		{
			this.MakeDirty (false);
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
