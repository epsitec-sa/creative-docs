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


namespace Epsitec.Common.Text.Properties
{
    /// <summary>
    /// L'énumération SizeUnits détermine les unités à utiliser pour décrire
    /// la taille d'une fonte, etc.
    /// </summary>
    public enum SizeUnits : byte
    {
        None,

        Points, //	n [pt]
        Millimeters, //	n [mm]
        Inches, //	n [in]

        DeltaPoints, //	N +/- n [pt]
        DeltaMillimeters, //	N +/- n [mm]
        DeltaInches, //	N +/- n [in]

        Percent, //	N * n%
        PercentNotCombining //	N * n% (ne se combine pas avec d'autres valeurs)
    }
}
