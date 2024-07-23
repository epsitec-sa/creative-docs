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

using NUnit.Framework;

namespace Epsitec.Common.Tests.Widgets
{
    [TestFixture]
    public class AdornerFactoryTest
    {
        [Test]
        public void CheckAdornerNames()
        {
            string[] names = Epsitec.Common.Widgets.Adorners.Factory.AdornerNames;

            Assert.IsNotNull(names);
            Assert.IsTrue(names.Length > 0);

            foreach (string name in names)
            {
                System.Console.Out.WriteLine("Class '" + name + "' implements IAdorner.");
            }
        }

        [Test]
        public void CheckAdornerActivation()
        {
            string[] names = Epsitec.Common.Widgets.Adorners.Factory.AdornerNames;

            Assert.IsNotNull(names);
            Assert.IsTrue(names.Length > 0);

            foreach (string name in names)
            {
                Assert.IsTrue(Epsitec.Common.Widgets.Adorners.Factory.SetActive(name));
                Assert.AreEqual(name, Epsitec.Common.Widgets.Adorners.Factory.ActiveName);
                Assert.IsNotNull(Epsitec.Common.Widgets.Adorners.Factory.Active);
            }
        }
    }
}
