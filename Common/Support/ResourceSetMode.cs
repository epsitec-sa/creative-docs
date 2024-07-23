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
    /// The <c>ResourceSetMode</c> is used to specify the action of a call to the
    /// <c>SetBundle</c> method of the resource manager.
    /// </summary>
    public enum ResourceSetMode
    {
        /// <summary>
        /// No action. This does nothing.
        /// </summary>
        None,

        /// <summary>
        /// Create the bundle if it does not existe; otherwise, generates an error.
        /// </summary>
        CreateOnly,

        /// <summary>
        /// Update the bundle if it exists; otherwise, generates an error.
        /// </summary>
        UpdateOnly,

        /// <summary>
        /// Create or update the bundle.
        /// </summary>
        Write,

        /// <summary>
        /// Create or update the bundle in memory only. This will not write the
        /// bundle to the resource provider, but just update the pool cache.
        /// </summary>
        InMemory,

        /// <summary>
        /// Remove the bundle. This will delete the associated file or entry in
        /// the database.
        /// </summary>
        Remove
    }
}
