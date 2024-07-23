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


namespace Epsitec.Common.Types
{
    /// <summary>
    /// The <c>StructuredTypeClass</c> enumeration is used to describe how a
    /// structured type should be interpreted.
    /// See <see cref="StructuredType"/>.
    /// </summary>
    [DesignerVisible]
    public enum StructuredTypeClass
    {
        /// <summary>
        /// The structured type should be considered as such: a simple structured
        /// type without any special semantic.
        /// </summary>
        None,

        /// <summary>
        /// The structured type defines an entity.
        /// </summary>
        Entity,

        /// <summary>
        /// The structured type defines an interface.
        /// </summary>
        Interface,
    }
}
