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


namespace Epsitec.Common.Widgets.Validators
{
    /// <summary>
    /// La classe AbstractTextValidator permet de simplifier l'implémentation de
    /// l'interface IValidator dans les cas où le texte sert de source pour la
    /// validation.
    /// </summary>
    public abstract class AbstractTextValidator : AbstractValidator
    {
        public AbstractTextValidator(Widget widget)
            : base(widget) { }

        public override void Validate()
        {
            string text = this.Widget.Text;

            this.ValidateText(text);
        }

        protected abstract void ValidateText(string text);

        protected override void AttachWidget(Widget widget)
        {
            base.AttachWidget(widget);

            widget.TextChanged += this.HandleWidgetTextChanged;
        }

        protected override void DetachWidget(Widget widget)
        {
            base.DetachWidget(widget);

            widget.TextChanged -= this.HandleWidgetTextChanged;
        }

        private void HandleWidgetTextChanged(object sender)
        {
            this.MakeDirty(false);
        }
    }
}
