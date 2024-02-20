//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

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
