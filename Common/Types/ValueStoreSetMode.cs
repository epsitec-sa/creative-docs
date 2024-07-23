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
    /// The <c>ValueStoreSetMode</c> enumeration defines the supported modes
    /// for the <see cref="IValueStore.SetValue"/> method.
    /// </summary>
    public enum ValueStoreSetMode
    {
        /// <summary>
        /// Default set mode.
        /// </summary>
        Default,

        /// <summary>
        /// Short-circuit the set logic and simply set the value without further
        /// handling.
        /// </summary>
        ShortCircuit,

        /// <summary>
        /// Special set mode used to define the initial collection (when the first
        /// read access is made on a collection field).
        /// </summary>
        InitialCollection,
    }
}
