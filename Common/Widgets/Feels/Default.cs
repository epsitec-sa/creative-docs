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

namespace Epsitec.Common.Widgets.Feel
{
    /// <summary>
    /// Implémentation du "feel" par défaut.
    /// </summary>
    public class Default : IFeel
    {
        public Default() { }

        #region IFeel Members
        public bool TestAcceptKey(Epsitec.Common.Widgets.Message message)
        {
            if (
                (
                    (message.MessageType == MessageType.KeyPress)
                    || (message.MessageType == MessageType.KeyDown)
                )
                && (message.IsShiftPressed == false)
                && (message.IsControlPressed == false)
                && (message.IsAltPressed == false)
            )
            {
                switch (message.KeyCode)
                {
                    case KeyCode.Return:
                    case KeyCode.NumericEnter:
                        return true;
                }
            }

            return false;
        }

        public bool TestSelectItemKey(Message message)
        {
            if (
                (
                    (message.MessageType == MessageType.KeyPress)
                    || (message.MessageType == MessageType.KeyDown)
                )
                && (message.IsShiftPressed == false)
                && (message.IsControlPressed == false)
                && (message.IsAltPressed == false)
            )
            {
                switch (message.KeyCode)
                {
                    case KeyCode.Space:
                    case KeyCode.Return:
                    case KeyCode.NumericEnter:
                        return true;
                }
            }

            return false;
        }

        public bool TestPressButtonKey(Message message)
        {
            if (
                (
                    (message.MessageType == MessageType.KeyPress)
                    || (message.MessageType == MessageType.KeyDown)
                )
                && (message.IsShiftPressed == false)
                && (message.IsControlPressed == false)
                && (message.IsAltPressed == false)
            )
            {
                return (message.KeyCode == KeyCode.Space);
            }

            return false;
        }

        public bool TestCancelKey(Epsitec.Common.Widgets.Message message)
        {
            if (
                (
                    (message.MessageType == MessageType.KeyPress)
                    || (message.MessageType == MessageType.KeyDown)
                )
                && (message.IsShiftPressed == false)
                && (message.IsControlPressed == false)
                && (message.IsAltPressed == false)
            )
            {
                return (message.KeyCode == KeyCode.Escape);
            }

            return false;
        }

        public bool TestNavigationKey(Epsitec.Common.Widgets.Message message)
        {
            if (
                (
                    (message.MessageType == MessageType.KeyPress)
                    || (message.MessageType == MessageType.KeyDown)
                )
                && (message.IsControlPressed == false)
                && (message.IsAltPressed == false)
            )
            {
                return (message.KeyCode == KeyCode.Tab);
            }

            return false;
        }

        public bool TestComboOpenKey(Message message)
        {
            if (
                (
                    (message.MessageType == MessageType.KeyPress)
                    || (message.MessageType == MessageType.KeyDown)
                )
                && (message.IsShiftPressed == false)
                && (message.IsControlPressed == false)
                && (message.IsAltPressed == false)
            )
            {
                switch (message.KeyCode)
                {
                    case KeyCode.ArrowUp:
                    case KeyCode.ArrowDown:
                        return true;

                    default:
                        return false;
                }
            }

            return false;
        }

        public Shortcut AcceptShortcut
        {
            get { return new Shortcut(KeyCode.Return); }
        }

        public Shortcut CancelShortcut
        {
            get { return new Shortcut(KeyCode.Escape); }
        }
        #endregion
    }
}
