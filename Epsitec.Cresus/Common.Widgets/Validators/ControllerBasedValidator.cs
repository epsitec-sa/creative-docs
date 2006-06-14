//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Validators
{
	public class ControllerBasedValidator : AbstractValidator
	{
		public ControllerBasedValidator() : base (null)
		{
		}

		public ControllerBasedValidator(Widget widget, Controllers.AbstractController controller) : base (widget)
		{
			this.controller = controller;
		}
		
		
		
		public override void Validate()
		{
			if ((this.controller != null) &&
				(this.controller.IsValidValue (this.controller.GetActualValue ())))
			{
				this.SetState (ValidationState.Ok);
			}
			else
			{
				this.SetState (ValidationState.Error);
			}
		}
		
		
		protected override void AttachWidget(Widget widget)
		{
			base.AttachWidget (widget);

			if (this.controller != null)
			{
				this.controller.ActualValueChanged += this.HandleActualValueChanged;
			}
		}
		
		protected override void DetachWidget(Widget widget)
		{
			if (this.controller != null)
			{
				this.controller.ActualValueChanged -= this.HandleActualValueChanged;
			}
			
			base.DetachWidget (widget);
		}
		
		
		private void HandleActualValueChanged(object sender)
		{
			this.MakeDirty (false);
		}

		private Controllers.AbstractController controller;
	}
}
