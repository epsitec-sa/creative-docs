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


namespace Epsitec.Common.Drawing
{
    /// <summary>
    /// The <c>GripIdExtensions</c> class provides utility methods related to the
    /// <see cref="GripId"/> enumeration.
    /// </summary>
    public static class GripIdExtensions
    {
        public static bool IsVertex(this GripId gripId)
        {
            switch (gripId)
            {
                case GripId.VertexBottomLeft:
                case GripId.VertexBottomRight:
                case GripId.VertexTopLeft:
                case GripId.VertexTopRight:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsLeftVertex(this GripId gripId)
        {
            return (gripId == GripId.VertexTopLeft) || (gripId == GripId.VertexBottomLeft);
        }

        public static bool IsRightVertex(this GripId gripId)
        {
            return (gripId == GripId.VertexTopRight) || (gripId == GripId.VertexBottomRight);
        }

        public static bool IsTopVertex(this GripId gripId)
        {
            return (gripId == GripId.VertexTopLeft) || (gripId == GripId.VertexTopRight);
        }

        public static bool IsBottomVertex(this GripId gripId)
        {
            return (gripId == GripId.VertexBottomLeft) || (gripId == GripId.VertexBottomRight);
        }
    }
}
