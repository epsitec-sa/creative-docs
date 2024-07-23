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
    /// <summary>
    /// Summary description for Expect.
    /// </summary>
    public sealed class Expect
    {
        [System.Diagnostics.Conditional("DEBUG")]
        public static void Exception(Method method, System.Type exType)
        {
            try
            {
                method();
            }
            catch (System.Exception ex)
            {
                if (ex.GetType() == exType)
                {
                    return;
                }

                throw new AssertFailedException("Exception raised; wrong type: " + ex.GetType());
            }

            throw new AssertFailedException("Expected exception not raised.");
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Exception(Method method, string exMessage)
        {
            try
            {
                method();
            }
            catch (System.Exception ex)
            {
                if (ex.Message == exMessage)
                {
                    return;
                }

                throw new AssertFailedException(
                    "Expected exception raised; wrong message received: " + ex.Message
                );
            }

            throw new AssertFailedException("Expected exception not raised.");
        }
    }

    public delegate void Method();
}
