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


namespace Epsitec.Common.Printing
{
    /// <summary>
    /// L'énumération PaperKind indique la nature d'un papier.
    /// </summary>
    public enum PaperKind
    {
        A2,
        A3,
        A3Extra,
        A4,
        A4Extra,
        A4Plus,
        A4Small,
        A5,
        A5Extra,
        A6,

        B4Envelope,
        B5Envelope,
        B6Envelope,
        C3Envelope,
        C4Envelope,
        C5Envelope,
        C65Envelope,
        C6Envelope,

        Other,

        Custom
    }
}
