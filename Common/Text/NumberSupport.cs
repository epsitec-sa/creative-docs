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


namespace Epsitec.Common.Text
{
    /// <summary>
    /// La classe NumberSupport implémente des méthodes utiles pour la manipulation
    /// de nombres.
    /// </summary>
    public sealed class NumberSupport
    {
        private NumberSupport() { }

        public static double Combine(double a, double b)
        {
            //	Combine deux valeurs. Si b est NaN, alors retourne a. Sinon
            //	retourne b.

            if (b.IsSafeNaN())
            {
                return a;
            }
            else
            {
                return b;
            }
        }

        public static bool Equal(double a, double b)
        {
            if (a == b)
            {
                return true;
            }

            if (a.IsSafeNaN() && b.IsSafeNaN())
            {
                return true;
            }

            return false;
        }

        public static bool Different(double a, double b)
        {
            if (a == b)
            {
                return false;
            }

            if (a.IsSafeNaN() && b.IsSafeNaN())
            {
                return false;
            }

            return true;
        }

        public static int Compare(double a, double b)
        {
            if (a == b)
            {
                return 0;
            }

            if (a.IsSafeNaN() && b.IsSafeNaN())
            {
                return 0;
            }

            if (a.IsSafeNaN())
            {
                return -1;
            }
            if (b.IsSafeNaN())
            {
                return 1;
            }

            return (a < b) ? -1 : 1;
        }
    }
}
