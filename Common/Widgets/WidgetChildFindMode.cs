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
    [System.Flags]
    public enum WidgetChildFindMode
    {
        All = 0,
        SkipHidden = 0x00000001,
        SkipDisabled = 0x00000002,
        SkipTransparent = 0x00000004,
        SkipEmbedded = 0x00000008,
        SkipNonContainer = 0x00000010,
        SkipMask = 0x000000ff,

        Deep = 0x00010000
    }
}
