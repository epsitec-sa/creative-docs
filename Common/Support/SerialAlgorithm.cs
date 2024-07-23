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


using Epsitec.Serial;

namespace Epsitec.Common.Support
{
    /// <summary>
    /// SerialAlgorithm définit les fonctions de base nécessaires pour valider
    /// une clef de logiciel.
    /// </summary>
    public static class SerialAlgorithm
    {
        public static int ProductGeneration { get; private set; }

        public static int ProductGracePeriod { get; private set; }

        public static System.DateTime BuildDate { get; private set; }

        public static string ReadCrDocSerial()
        {
            string key = null;
            return (string)
                Microsoft.Win32.Registry.GetValue(SerialAlgorithm.RegistrySerialPath, "ID", key);
        }

        public static bool CheckSerial(string key, int productFamily)
        {
            return CresusSerialAlgorithm.CheckSerial(key, productFamily);
        }

        public static bool CheckSerial(string key, int productFamily, out bool updatesAllowed)
        {
            return CresusSerialAlgorithm.CheckSerial(
                key,
                productFamily,
                SerialAlgorithm.BuildDate,
                out updatesAllowed
            );
        }

        public static System.DateTime GetExpirationDate(string key)
        {
            return CresusSerialAlgorithm.GetSerialLimit(key);
        }

        public static int GetProduct(string key)
        {
            return CresusSerialAlgorithm.GetProduct(key);
        }

        public static void SetProductBuildDate(System.DateTime date)
        {
            SerialAlgorithm.BuildDate = date;
        }

        public static void SetProductGenerationNumber(int generation, int grace)
        {
            SerialAlgorithm.ProductGeneration = generation;
            SerialAlgorithm.ProductGracePeriod = grace;
        }

        private const string RegistrySerialPath =
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Epsitec\Cresus Documents\Setup";
    }
}
