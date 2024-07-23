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


namespace Epsitec.Common.Debug
{
    public static class Sos
    {
        public static string GetAddress(object o)
        {
            if (o == null)
            {
                return "00000000";
            }
            else
            {
                unsafe
                {
                    System.TypedReference tr = __makeref(o);
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
                    System.IntPtr ptr = **(System.IntPtr**)(&tr);
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
                    return ptr.ToString("X");
                }
            }
        }
    }
}
