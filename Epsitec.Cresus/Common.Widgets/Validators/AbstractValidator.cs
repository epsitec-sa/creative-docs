namespace Epsitec.Common.Widgets.Validators
{
	/// <summary>
	/// La classe AbstractValidator permet de simplifier l'implémentation de
	/// l'interface IValidator en relation avec des widgets.
	/// </summary>
	public abstract class AbstractValidator : Support.IValidator, System.IDisposable
	{
		public AbstractValidator(Widget widget)
		{
			this.widget = widget;
			this.AttachWidget (this.widget);
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
				this.DetachWidget (this.widget);
				this.widget = null;
			}
		}
		
		#region IValidator Members
		public Support.ValidationState			State
		{
			get
			{
				return this.state;
			}
		}
		
		public abstract void Validate();
		
		public void MakeDirty(bool deep)
		{
			this.state = Support.ValidationState.Dirty;
			this.OnBecameDirty ();
		}
		
		public bool								IsValid
		{
			get
			{
				if (this.state == Support.ValidationState.Dirty)
				{
					this.Validate ();
				}
				
				return (this.state == Support.ValidationState.Ok);
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
			widget.RemoveValidator (null);
		}
		
		
		protected Support.ValidationState		state;
		protected Widget						widget;
	}
}
