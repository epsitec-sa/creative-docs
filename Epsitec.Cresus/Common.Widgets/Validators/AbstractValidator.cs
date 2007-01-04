//	Copyright � 2004-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Validators
{
	/// <summary>
	/// La classe AbstractValidator permet de simplifier l'impl�mentation de
	/// l'interface IValidator en relation avec des widgets.
	/// </summary>
	public abstract class AbstractValidator : IValidator, System.IDisposable
	{
		public AbstractValidator(Widget widget)
		{
			this.widget = widget;
			
			if (this.widget != null)
			{
				this.AttachWidget (this.widget);
			}
		}
		
		
		internal void InternalAttach(Widget widget)
		{
			System.Diagnostics.Debug.Assert (this.widget == null);
			System.Diagnostics.Debug.Assert (widget != null);
			
			this.widget = widget;
			this.AttachWidget (this.widget);
		}

		public Widget Widget
		{
			get
			{
				return this.widget;
			}
		}
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.widget != null)
				{
					this.DetachWidget (this.widget);
					
					this.widget = null;
				}
			}
		}

		protected virtual void SetState(ValidationState state)
		{
			this.state = state;
		}
		
		#region IValidator Members
		public ValidationState					State
		{
			get
			{
				return this.state;
			}
		}
		
		public abstract void Validate();
		
		public void MakeDirty(bool deep)
		{
			this.state = ValidationState.Dirty;
			this.OnBecameDirty ();
		}
		
		public bool								IsValid
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

		public string							ErrorMessage
		{
			get
			{
				return null;
			}
		}
		
		public event Support.EventHandler		BecameDirty;
		#endregion
		
		protected virtual void OnBecameDirty()
		{
			if (this.widget != null)
			{
				this.widget.AsyncValidation ();
			}
			
			if (this.BecameDirty != null)
			{
				this.BecameDirty (this);
			}
		}
		
		protected virtual void AttachWidget(Widget widget)
		{
			widget.AddValidator (this);
		}
		
		protected virtual void DetachWidget(Widget widget)
		{
			widget.RemoveValidator (this);
		}
		
		
		private ValidationState					state;
		private Widget							widget;
	}
}
