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


using Epsitec.Common.Text;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Text
{
    /// <summary>
    /// Vérifie le bon fonctionnement de la classe TextConverter.
    /// </summary>
    [TestFixture]
    public sealed class CheckTextConverter
    {
        [Test]
        public static void ConverterWorksOnValidData()
        {
            string str1 = "Abc\u1234\ud840\udc01.";
            string str2;

            uint[] text;

            TextConverter.ConvertFromString(str1, out text);

            Assert.IsTrue(text.Length == 6);
            Assert.IsTrue(text[3] == 0x1234);
            Assert.IsTrue(text[4] == 0x20001);
            Assert.IsTrue(text[5] == '.');

            TextConverter.ConvertToString(text, out str2);

            Assert.IsTrue(str1 == str2);
        }

        [Test]
        public static void Ex1()
        {
            Assert.Throws<Unicode.IllegalCodeException>(() =>
            {
                uint[] text = { 0xD800, 0xDC00, 0x1234 };
                string str;

                TextConverter.ConvertToString(text, out str);
            });
        }

        [Test]
        public static void Ex2()
        {
            Assert.Throws<Unicode.IllegalCodeException>(() =>
            {
                uint[] text = { 0x10FFFF, 0x110000 };
                string str;

                TextConverter.ConvertToString(text, out str);
            });
        }
    }
}
