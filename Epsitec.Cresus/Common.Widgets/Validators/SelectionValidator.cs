namespace Epsitec.Common.Widgets.Validators
{
	/// <summary>
	/// Summary description for NamedStringSelectionValidator.
	/// </summary>
	public class SelectionValidator : AbstractValidator
	{
		public SelectionValidator() : base (null)
		{
		}
		
		
		public SelectionValidator(Widget widget) : base (widget)
		{
			if (widget is Support.Data.INamedStringSelection)
			{
				//	OK.
			}
			else
			{
				throw new System.ArgumentException (string.Format ("Widget {0} does not support interface INamedStringSelection.", widget), "widget");
			}
		}
		
		
		public override void Validate()
		{
			Support.Data.INamedStringSelection sel = this.widget as Support.Data.INamedStringSelection;
			
			if ((sel.SelectedIndex >= 0) &&
				(this.IsSelectionValid (sel)))
			{
				this.state = Support.ValidationState.Ok;
			}
			else
			{
				this.state = Support.ValidationState.Error;
			}
		}
		
		
		protected virtual bool IsSelectionValid(Support.Data.INamedStringSelection selection)
		{
			return true;
		}
		
		
		protected override void AttachWidget(Widget widget)
		{
			base.AttachWidget (widget);
			
			Support.Data.INamedStringSelection sel = this.widget as Support.Data.INamedStringSelection;
			
			sel.SelectedIndexChanged += new Epsitec.Common.Support.EventHandler (this.HandleSelectionSelectedIndexChanged);
		}
		
		protected override void DetachWidget(Widget widget)
		{
			base.DetachWidget (widget);
			
			Support.Data.INamedStringSelection sel = this.widget as Support.Data.INamedStringSelection;
			
			sel.SelectedIndexChanged -= new Epsitec.Common.Support.EventHandler (this.HandleSelectionSelectedIndexChanged);
		}
		
		
		private void HandleSelectionSelectedIndexChanged(object sender)
		{
			this.MakeDirty (false);
		}
	}
}
