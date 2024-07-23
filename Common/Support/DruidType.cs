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


namespace Epsitec.Common.Support
{
    /// <summary>
    /// The <c>DruidType</c> enumeration defines the type of a DRUID.
    /// </summary>
    public enum DruidType
    {
        /// <summary>
        /// The DRUID is invalid; it does not reference any resource.
        /// </summary>
        Invalid,

        /// <summary>
        /// The DRUID provides a module relative resource reference. It only
        /// encodes the developer id and local id (44-bit version of the DRUID
        /// value).
        /// </summary>
        ModuleRelative,

        /// <summary>
        /// The DRUID provided a full resource reference. It encodes the module
        /// id, the developer id and the local id (64-bit version of the DRUID
        /// value).
        /// </summary>
        Full
    }
}
