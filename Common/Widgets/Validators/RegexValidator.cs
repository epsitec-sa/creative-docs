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
    /// La classe RegexValidator permet de valider un widget selon une expression
    /// régulière.
    /// </summary>
    public class RegexValidator : AbstractTextValidator
    {
        public RegexValidator()
            : this(null, "", true) { }

        public RegexValidator(Widget widget, string regex)
            : this(widget, regex, true) { }

        public RegexValidator(Widget widget, string regex, bool acceptEmpty)
            : base(widget)
        {
            this.SetRegex(regex);
            this.acceptEmpty = acceptEmpty;
        }

        public RegexValidator(Widget widget, System.Text.RegularExpressions.Regex regex)
            : this(widget, regex, true) { }

        public RegexValidator(
            Widget widget,
            System.Text.RegularExpressions.Regex regex,
            bool acceptEmpty
        )
            : base(widget)
        {
            this.SetRegex(regex);
            this.acceptEmpty = acceptEmpty;
        }

        public bool AcceptEmptyText
        {
            get { return this.acceptEmpty; }
            set { this.acceptEmpty = value; }
        }

        public string Regex
        {
            get { return this.regex.ToString(); }
            set
            {
                if (this.Regex != value)
                {
                    this.SetRegex(value);
                }
            }
        }

        public void SetRegex(string regex)
        {
            System.Text.RegularExpressions.RegexOptions options = System
                .Text
                .RegularExpressions
                .RegexOptions
                .Compiled;

            this.regex = new System.Text.RegularExpressions.Regex(regex, options);
        }

        public void SetRegex(System.Text.RegularExpressions.Regex regex)
        {
            this.regex = regex;
        }

        protected override void ValidateText(string text)
        {
            if (
                (this.acceptEmpty && text.Length == 0)
                || (text.Length > 0 && this.regex != null && this.regex.IsMatch(text))
            )
            {
                this.SetState(ValidationState.Ok);
            }
            else
            {
                this.SetState(ValidationState.Error);
            }
        }

        protected System.Text.RegularExpressions.Regex regex;
        protected bool acceptEmpty;
    }
}
