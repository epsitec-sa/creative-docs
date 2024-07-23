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


namespace Epsitec.Common.UI.Controllers
{
    /// <summary>
    /// The <c>WidgetType</c> enumeration is used to categorize the widgets used
    /// in a <see cref="Placeholder"/>.
    /// </summary>
    public enum WidgetType
    {
        /// <summary>
        /// This is a simple label.
        /// </summary>
        Label,

        /// <summary>
        /// The is an input element; the user can interact with it.
        /// </summary>
        Input,

        /// <summary>
        /// This is a read only content element. It provides the user with
        /// information, but cannot it does not offer any interaction.
        /// </summary>
        Content,

        /// <summary>
        /// This is a command element, usually a button.
        /// </summary>
        Command
    }
}
