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


using Epsitec.Common.Support.Data;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets.Validators
{
    /// <summary>
    /// The <c>SelectionValidator</c> class considers a value valid only if it
    /// belongs to an item returned through the attached widget's
    /// <see cref="IKeyedStringSelection"/> interface.
    /// </summary>
    public class SelectionValidator : AbstractValidator
    {
        public SelectionValidator()
            : base(null) { }

        public SelectionValidator(Widget widget)
            : base(widget)
        {
            if (widget is IKeyedStringSelection)
            {
                //	OK.
            }
            else
            {
                throw new System.ArgumentException(
                    string.Format(
                        "Widget {0} does not support interface IKeyedStringSelection",
                        widget
                    ),
                    "widget"
                );
            }
        }

        public override void Validate()
        {
            var sel = this.Widget as IKeyedStringSelection;

            if ((sel.SelectedItemIndex >= 0) && (this.IsSelectionValid(sel)))
            {
                this.SetState(ValidationState.Ok);
            }
            else
            {
                this.SetState(ValidationState.Error);
            }
        }

        protected virtual bool IsSelectionValid(IKeyedStringSelection selection)
        {
            return true;
        }

        protected override void AttachWidget(Widget widget)
        {
            base.AttachWidget(widget);

            var sel = this.Widget as IKeyedStringSelection;

            sel.SelectedItemChanged += this.HandleSelectionSelectedIndexChanged;
        }

        protected override void DetachWidget(Widget widget)
        {
            base.DetachWidget(widget);

            var sel = this.Widget as IKeyedStringSelection;

            sel.SelectedItemChanged -= this.HandleSelectionSelectedIndexChanged;
        }

        private void HandleSelectionSelectedIndexChanged(object sender)
        {
            this.MakeDirty(false);
        }
    }
}
