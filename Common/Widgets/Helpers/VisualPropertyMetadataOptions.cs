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


namespace Epsitec.Common.Widgets.Helpers
{
    /// <summary>
    /// VisualPropertyMetadataOptions.
    /// </summary>

    [System.Flags]
    public enum VisualPropertyMetadataOptions
    {
        None = 0,

        AffectsMeasure = 0x0001,
        AffectsArrange = 0x0002,
        AffectsDisplay = 0x0004,
        AffectsChildrenLayout = 0x0008,
        AffectsTextLayout = 0x0010,

        InheritsValue = 0x1000, //	la valeur de la propriété peut être héritée par des enfants
        ChangesSilently = 0x2000, //	les changements de la propriété ne génèrent pas d'événement
    }
}
