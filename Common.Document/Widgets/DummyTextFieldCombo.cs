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

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Document.Widgets
{
    /// <summary>
    /// La classe DummyTextFieldCombo est un TextFieldCombo qui ne déroule aucun menu.
    /// Il faut utiliser DummyTextFieldCombo.Button.Clicked pour dérouler qq chose à choix.
    /// </summary>
    public class DummyTextFieldCombo : TextFieldCombo
    {
        public DummyTextFieldCombo() { }

        public DummyTextFieldCombo(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        protected override void OpenCombo()
        {
            this.OnComboOpenPressed();
        }

        protected virtual void OnComboOpenPressed()
        {
            var handler = this.GetUserEventHandler("ComboOpenPressed");
            if (handler != null)
            {
                handler(this);
            }
        }

        public event EventHandler ComboOpenPressed
        {
            add { this.AddUserEventHandler("ComboOpenPressed", value); }
            remove { this.RemoveUserEventHandler("ComboOpenPressed", value); }
        }
    }
}
