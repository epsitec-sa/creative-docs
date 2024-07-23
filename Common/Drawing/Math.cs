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


namespace Epsitec.Common
{
    public static class Math
    {
        public static float Clip(float value)
        {
            if (value < 0)
                return 0;
            if (value > 1)
                return 1;
            return value;
        }

        public static double Clip(double value)
        {
            if (value < 0.0)
                return 0.0;
            if (value > 1.0)
                return 1.0;
            return value;
        }

        public static double ClipAngleRad(double angle)
        {
            //	Retourne un angle normalisé, c'est-à-dire compris entre 0 et 2*PI.

            angle = angle % (System.Math.PI * 2.0);
            return (angle < 0.0) ? System.Math.PI * 2.0 + angle : angle;
        }

        public static double ClipAngleDeg(double angle)
        {
            //	Retourne un angle normalisé, c'est-à-dire compris entre 0 et 360°.

            angle = angle % 360.0;
            return (angle < 0.0) ? 360.0 + angle : angle;
        }

        public static double DegToRad(double angle)
        {
            return angle * System.Math.PI / 180.0;
        }

        public static double RadToDeg(double angle)
        {
            return angle * 180.0 / System.Math.PI;
        }

        public static bool IsSafeNaN(this double value)
        {
            try
            {
                return double.IsNaN(value);
            }
            catch (System.ArithmeticException)
            {
                return true;
            }
        }

        public static bool Equal(double a, double b, double δ)
        {
            //	Compare deux nombres avec une certaine marge d'erreur.

            if (a == b)
            {
                return true;
            }

            if (a.IsSafeNaN() && b.IsSafeNaN())
            {
                return true;
            }

            if (double.IsNegativeInfinity(a) && double.IsNegativeInfinity(b))
            {
                return true;
            }

            if (double.IsPositiveInfinity(a) && double.IsPositiveInfinity(b))
            {
                return true;
            }

            double diff = a - b;

            if (diff < 0)
            {
                return -diff < δ;
            }
            else
            {
                return diff < δ;
            }
        }
    }
}
