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


namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// The <c>CommandDefaultEnableMode</c> enumeration defines how commands should be
    /// enabled by default (when nothing is specified in any <see cref="CommandContext"/>
    /// in the active command context chain.
    /// </summary>
    public enum CommandDefaultEnableMode
    {
        /// <summary>
        /// Undefined mode (will default to enabled by default).
        /// </summary>
        None,

        /// <summary>
        /// The command will be disabled by default.
        /// </summary>
        Disabled,

        /// <summary>
        /// The command will be enabled by default.
        /// </summary>
        Enabled,
    }
}
