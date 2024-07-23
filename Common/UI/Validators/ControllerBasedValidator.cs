/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using Epsitec.Common.Types;

namespace Epsitec.Common.UI.Validators
{
    public class ControllerBasedValidator : Widgets.Validators.AbstractValidator
    {
        public ControllerBasedValidator()
            : base(null) { }

        public ControllerBasedValidator(
            Widgets.Widget widget,
            UI.Controllers.AbstractController controller
        )
            : base(widget)
        {
            this.controller = controller;
        }

        public override void Validate()
        {
            if (
                (this.controller != null)
                && (
                    this.controller.IsValidUserInterfaceValue(
                        this.controller.GetUserInterfaceValue()
                    )
                )
            )
            {
                this.SetState(ValidationState.Ok);
            }
            else
            {
                this.SetState(ValidationState.Error);
            }
        }

        protected override void AttachWidget(Widgets.Widget widget)
        {
            base.AttachWidget(widget);

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

            base.DetachWidget(widget);
        }

        private void HandleActualValueChanged(object sender)
        {
            this.MakeDirty(false);
        }

        private Controllers.AbstractController controller;
    }
}
