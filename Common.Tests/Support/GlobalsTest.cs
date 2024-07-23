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

using Epsitec.Common.Support;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Support
{
    [TestFixture]
    public class GlobalsTest
    {
        [Test]
        public void CheckDirectories()
        {
            System.Console.Out.WriteLine(
                "Common App Data: {0}",
                Globals.Directories.CommonAppDataRevision
            );
            System.Console.Out.WriteLine(
                "User App Data:   {0}",
                Globals.Directories.UserAppDataRevision
            );
            System.Console.Out.WriteLine("Executable:      {0}", Globals.Directories.Executable);
            System.Console.Out.WriteLine("Executable Name: {0}", Globals.ExecutableName);
        }
    }
}
