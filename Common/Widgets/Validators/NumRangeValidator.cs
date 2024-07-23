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

namespace Epsitec.Common.Widgets.Validators
{
    /// <summary>
    /// La classe NumRangeValidator permet de vérifier que la valeur est dans les
    /// bornes requises.
    /// </summary>
    public class NumRangeValidator : AbstractValidator
    {
        public NumRangeValidator()
            : base(null) { }

        public NumRangeValidator(Widget widget)
            : base(widget)
        {
            if (widget is Support.Data.INumValue)
            {
                //	OK.
            }
            else
            {
                throw new System.ArgumentException(
                    string.Format("Widget {0} does not support interface INumValue.", widget),
                    "widget"
                );
            }
        }

        public override void Validate()
        {
            Support.Data.INumValue num = this.Widget as Support.Data.INumValue;
            Types.DecimalRange range = new Types.DecimalRange(
                num.MinValue,
                num.MaxValue,
                num.Resolution
            );

            if (range.CheckInRange(num.Value))
            {
                this.SetState(ValidationState.Ok);
            }
            else
            {
                this.SetState(ValidationState.Error);
            }
        }

        protected override void AttachWidget(Widget widget)
        {
            base.AttachWidget(widget);

            Support.Data.INumValue num = this.Widget as Support.Data.INumValue;

            num.ValueChanged += this.HandleValueChanged;
        }

        protected override void DetachWidget(Widget widget)
        {
            base.DetachWidget(widget);

            Support.Data.INumValue num = this.Widget as Support.Data.INumValue;

            num.ValueChanged -= this.HandleValueChanged;
        }

        private void HandleValueChanged(object sender)
        {
            this.MakeDirty(false);
        }
    }
}
