//	Copyright © 2006-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.UI.Validators
{
	public class ControllerBasedValidator : Widgets.Validators.AbstractValidator
	{
		public ControllerBasedValidator() : base (null)
		{
		}

		public ControllerBasedValidator(Widgets.Widget widget, UI.Controllers.AbstractController controller) : base (widget)
		{
			this.controller = controller;
		}
		
		
		
		public override void Validate()
		{
			if ((this.controller != null) &&
				(this.controller.IsValidUserInterfaceValue (this.controller.GetUserInterfaceValue ())))
			{
				this.SetState (ValidationState.Ok);
			}
			else
			{
				this.SetState (ValidationState.Error);
			}
		}
		
		
		protected override void AttachWidget(Widgets.Widget widget)
		{
			base.AttachWidget (widget);

			if (this.controller != null)
			{
				this.controller.ActualValueChanged += this.HandleActualValueChanged;
			}
		}
		
		protected override void DetachWidget(Widgets.Widget widget)
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
