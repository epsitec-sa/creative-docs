//	Copyright © 2004-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe MulticastValidator représente le "et" logique d'un ensemble de
	/// règles règles de validation.
	/// </summary>
	public sealed class MulticastValidator : IValidator
	{
		public MulticastValidator()
		{
			this.validators = new IValidator[0];
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
		
		public FormattedText						ErrorMessage
		{
			get
			{
				//	TODO: détermine le message en fonction des dépendances...
				
				return FormattedText.Empty;
			}
		}
		
		
		public event Support.EventHandler			BecameDirty;
		
		public void Validate()
		{
			this.state = ValidationState.Unknown;
			
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
				this.OnUpdateState (ValidationState.Unknown);
			}
			else
			{
				this.OnUpdateState (valid ? ValidationState.Ok : ValidationState.Error);
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
		
		public void Add(IValidator validator)
		{
			if (validator == this)
			{
				throw new System.ArgumentException ("Circular reference");
			}
			
			MulticastValidator mv = validator as MulticastValidator;
			
			if (mv == null)
			{
				this.AddValidator (validator);
			}
			else
			{
				foreach (IValidator item in mv.validators)
				{
					this.AddValidator (item);
				}
			}
		}
		
		public void Remove(IValidator validator)
		{
			if (validator == this)
			{
				throw new System.ArgumentException ("Circular reference");
			}

			MulticastValidator mv = validator as MulticastValidator;
			
			if (mv == null)
			{
				this.RemoveValidator (validator);
			}
			else
			{
				foreach (IValidator item in mv.validators)
				{
					this.RemoveValidator (item);
				}
			}
		}

		
		public static IValidator Combine(IValidator validatorA, IValidator validatorB)
		{
			if (validatorA == null)
			{
				return validatorB;
			}
			
			if (validatorB == null)
			{
				return validatorA;
			}
			
			MulticastValidator mv = new MulticastValidator ();
			
			mv.Add (validatorA);
			mv.Add (validatorB);
			
			return mv;
		}

		public static IValidator Remove(IValidator validatorA, IValidator validatorB)
		{
			if (validatorA == null)
			{
				return null;
			}
			if (validatorB == null)
			{
				return validatorA;
			}

			MulticastValidator mv = validatorA as MulticastValidator;

			if (mv == null)
			{
				foreach (IValidator item in MulticastValidator.GetValidators (validatorB))
				{
					if (item == validatorA)
					{
						return null;
					}
				}
				
				return validatorA;
			}

			mv.Remove (validatorB);

			return MulticastValidator.Simplify (mv);
		}
		
		public static IValidator Simplify(IValidator validator)
		{
			MulticastValidator mv = validator as MulticastValidator;
			
			if (mv != null)
			{
				if (mv.validators.Length == 1)
				{
					return mv.validators[0];
				}
				if (mv.validators.Length == 0)
				{
					return null;
				}
			}
			
			return validator;
		}
		
		public static IEnumerable<IValidator> GetValidators(IValidator validator)
		{
			if (validator != null)
			{
				MulticastValidator mv = validator as MulticastValidator;

				if (mv == null)
				{
					yield return validator;
				}
				else
				{
					for (int i = 0; i < mv.validators.Length; i++)
					{
						yield return mv.validators[i];
					}
				}
			}
		}


		#region Private Management Methods

		private void AddValidator(IValidator validator)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			list.AddRange (this.validators);

			if (list.Contains (validator))
			{
				throw new System.InvalidOperationException ("Cannot insert twice same IValidator.");
			}
			if (validator == null)
			{
				throw new System.ArgumentNullException ("validator", "Expected an IValidator.");
			}

			list.Add (validator);

			IValidator[] validators = new IValidator[list.Count];
			list.CopyTo (validators);

			validator.BecameDirty += this.HandleBecameDirty;
			this.validators = validators;

			this.OnValidatorsChanged ();
		}

		private void RemoveValidator(IValidator validator)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			list.AddRange (this.validators);

			if (!list.Contains (validator))
			{
				throw new System.InvalidOperationException ("Cannot remove IValidator, not found.");
			}
			if (validator == null)
			{
				throw new System.ArgumentNullException ("validator", "Expected an IValidator.");
			}

			list.Remove (validator);
			IValidator[] validators = new IValidator[list.Count];
			list.CopyTo (validators);

			validator.BecameDirty -= this.HandleBecameDirty;
			this.validators = validators;

			this.OnValidatorsChanged ();
		}


		private void HandleBecameDirty(object sender)
		{
			this.MakeDirty (false);
		}


		private void OnUpdateState(ValidationState newState)
		{
			System.Diagnostics.Debug.Assert (newState != ValidationState.Dirty);

			this.state = newState;
		}

		private void OnBecameDirty()
		{
			if (this.BecameDirty != null)
			{
				this.BecameDirty (this);
			}
		}

		private void OnValidatorsChanged()
		{
			this.MakeDirty (false);
		}

		#endregion

		private ValidationState state = ValidationState.Dirty;
		private IValidator[] validators;
	}
}
