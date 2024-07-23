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
    /// The <c>DataLifetimeExpectancy</c> enumeration defines how long data
    /// is expected to live.
    /// </summary>
    [DesignerVisible]
    public enum DataLifetimeExpectancy
    {
        /// <summary>
        /// The lifetime expectancy is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// The data is volatile and it might change frequently. Avoid storing it
        /// over a long time.
        /// </summary>
        Volatile,

        /// <summary>
        /// The data is stable and it won't change frequently. It might be meaningful
        /// to cache it over a longer period of time.
        /// </summary>
        Stable,

        /// <summary>
        /// The data is immutable. It will not change, or only after a restart (e.g.
        /// after the application resumed from maintenance mode).
        /// </summary>
        Immutable,
    }
}
