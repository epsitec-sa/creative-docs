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
    /// The <c>FieldOptions</c> enumeration defines a set of options which can
    /// be attached to a field.
    /// See <see cref="StructuredTypeField"/>.
    /// </summary>
    [DesignerVisible]
    [System.Flags]
    public enum FieldOptions : ushort
    {
        // NOTE This enumeration is used within StructuredTypeField where it can be serialized. The
        // serialization process combines this value with other in a single integer in order to gain
        // space. Currently, only the least significant 12 bits of this value will be persisted because
        // of shifting and masking. If you ever increase the number of values of this enumeration in
        // such a way that requires it to be a short or a bigger type, you must adapt the existing
        // code in StructuredTypeField in order to ensure that all values can be persisted there.

        /// <summary>
        /// There is no option defined for this field.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// The field is nullable, which basically means that it can be empty,
        /// i.e. have no value at all.
        /// </summary>
        Nullable = 0x01,

        /// <summary>
        /// The field relation points to a private (i.e. not shared) target.
        /// </summary>
        PrivateRelation = 0x02,

        /// <summary>
        /// The field should be used to generate an ascending index (0..n).
        /// </summary>
        IndexAscending = 0x04,

        /// <summary>
        /// The field should be used to generate a descending index (n..0).
        /// </summary>
        IndexDescending = 0x08,

        /// <summary>
        /// The field is virtual: it has no direct database backing and will
        /// not be persisted as is.
        /// </summary>
        Virtual = 0x10,

        /// <summary>
        /// If the field requires sorting (it is a string), then apply a case
        /// insensitive collation (i.e. "a" is equal to "A").
        /// </summary>
        CollationCaseInsensitive = 0x20,

        /// <summary>
        /// If the field requires sorting (it is a string), then apply an accent
        /// insensitive collation (i.e. "a" is equal to "à"/"ä"/...).
        /// </summary>
        CollationAccentInsensitive = 0x40,

        /// <summary>
        /// The field might contain large amounts of data and should not be prefetched
        /// when a query does not explicitely require its value. This is useful for BLOBs.
        /// </summary>
        DisablePrefetch = 0x80,
    }
}
