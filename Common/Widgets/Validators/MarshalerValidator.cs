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
using Epsitec.Common.Types.Converters;

namespace Epsitec.Common.Widgets.Validators
{
    /// <summary>
    /// The <c>MarshalerValidator</c> uses a <see cref="Marshaler"/> to check for
    /// the validity of the input values.
    /// </summary>
    public class MarshalerValidator : PredicateValidator
    {
        public MarshalerValidator()
            : base() { }

        public MarshalerValidator(Widget widget)
            : base(widget) { }

        public MarshalerValidator(Widget widget, Marshaler marshaler)
            : base(widget)
        {
            this.Marshaler = marshaler;
        }

        /// <summary>
        /// Gets or sets the marshaler used to validate the widget.
        /// </summary>
        /// <value>The marshaler.</value>
        public Marshaler Marshaler
        {
            get { return this.marshaler; }
            set
            {
                this.marshaler = value;
                this.Predicate = this.CreateComposedPredicate();
            }
        }

        public override FormattedText ErrorMessage
        {
            get { return this.errorMessage; }
        }

        public System.Func<string, IValidationResult> Validator
        {
            get { return this.validator; }
            set
            {
                this.validator = value;
                this.Predicate = this.CreateComposedPredicate();
            }
        }

        public static MarshalerValidator CreateValidator(Widget widget, Marshaler marshaler)
        {
            return new MarshalerValidator(widget, marshaler);
        }

        private System.Func<bool> CreateComposedPredicate()
        {
            if (this.validator == null)
            {
                if (this.marshaler == null)
                {
                    return () => true;
                }
                else
                {
                    return () => this.marshaler.CanConvert(this.GetPredicateText());
                }
            }
            else
            {
                if (this.marshaler == null)
                {
                    return () => this.EvaluateValidator(this.GetPredicateText());
                }
                else
                {
                    return delegate
                    {
                        string text = this.GetPredicateText();
                        return this.marshaler.CanConvert(text) && this.EvaluateValidator(text);
                    };
                }
            }
        }

        private string GetPredicateText()
        {
            return TextConverter.ConvertToSimpleText(this.Widget.Text);
        }

        private bool EvaluateValidator(string value)
        {
            var validationResult = this.validator(value);

            this.errorMessage = validationResult.ErrorMessage;

            return validationResult.IsValid;
        }

        private Marshaler marshaler;
        private System.Func<string, IValidationResult> validator;
        private FormattedText errorMessage;
    }
}
