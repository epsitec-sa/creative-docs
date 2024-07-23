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

using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Feel;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Widgets
{
    [TestFixture]
    public class FeelTest
    {
        [Test]
        public void CheckFeelNames()
        {
            string[] names = Epsitec.Common.Widgets.Feel.Factory.FeelNames;

            Assert.IsNotNull(names);
            Assert.IsTrue(names.Length > 0);

            foreach (string name in names)
            {
                System.Console.Out.WriteLine("Class '" + name + "' implements IFeel.");
            }
        }

        [Test]
        public void CheckFeelActivation()
        {
            string[] names = Epsitec.Common.Widgets.Feel.Factory.FeelNames;

            Assert.IsNotNull(names);
            Assert.IsTrue(names.Length > 0);

            foreach (string name in names)
            {
                Assert.IsTrue(Epsitec.Common.Widgets.Feel.Factory.SetActive(name));
                Assert.AreEqual(name, Epsitec.Common.Widgets.Feel.Factory.ActiveName);
                Assert.IsNotNull(Epsitec.Common.Widgets.Feel.Factory.Active);
            }
        }

        [Test]
        public void CheckFeelShortcuts()
        {
            Factory.SetActive("Default");

            IFeel feel = Factory.Active;

            Assert.IsNotNull(feel);

            Shortcut s1 = feel.AcceptShortcut;
            Shortcut s2 = feel.CancelShortcut;

            Assert.AreEqual(s1, new Shortcut(KeyCode.Return));
            Assert.AreEqual(s2, new Shortcut(KeyCode.Escape));
        }
    }
}
