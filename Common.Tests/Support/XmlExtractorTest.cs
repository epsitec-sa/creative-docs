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
    public class XmlExtractorTest
    {
        [Test]
        public void Check()
        {
            var extractor = new XmlExtractor();
            var sourceXml =
                @"<?xml version=""1.0"" encoding=""UTF-8""?>"
                + "\n"
                /**/+ @"<!-- edited with ... -->"
                + "\n"
                /**/+ @"<xs:schema xmlns:xs=""http://www.w3.org/2001/XMLSchema"" elementFormDefault=""qualified"" attributeFormDefault=""unqualified"" version=""2.0"">"
                + "\n"
                /**/+ @"<xs:annotation>bla</xs:annotation>"
                + "\n"
                /**/+ @"</xs:schema>"
                + "\n";

            var afterXml =
                "\n"
                /**/+ "bla\n";

            var lines = (sourceXml + afterXml).Split('\n');

            foreach (var line in lines)
            {
                extractor.AppendLine(line);

                if (extractor.Finished)
                {
                    break;
                }
            }

            Assert.AreEqual("", extractor.ExcessText);
            Assert.AreEqual(sourceXml, extractor.ToString());
        }

        [Test]
        public void CheckEmpty()
        {
            var extractor = new XmlExtractor();

            Assert.IsFalse(extractor.Finished);
            Assert.IsFalse(extractor.Started);

            extractor.Append("");

            Assert.IsFalse(extractor.Finished);
            Assert.IsFalse(extractor.Started);

            extractor.Append(" \t\r\n  ");

            Assert.IsFalse(extractor.Finished);
            Assert.IsFalse(extractor.Started);

            Assert.AreEqual(" \t\r\n  ", extractor.ToString());
        }

        [Test]
        public void CheckDeclaration1()
        {
            var extractor = new XmlExtractor();

            extractor.Append("<??>");

            Assert.IsFalse(extractor.Finished);
            Assert.IsFalse(extractor.Started);
        }

        [Test]
        public void CheckDeclaration2()
        {
            var extractor = new XmlExtractor();

            extractor.Append("<?xml");
            extractor.Append("foo ?>");

            Assert.IsFalse(extractor.Finished);
            Assert.IsFalse(extractor.Started);
        }

        [Test]
        public void CheckDeclarationEx1()
        {
            var extractor = new XmlExtractor();

            Assert.Throws<System.FormatException>(() => extractor.Append("<?>"));
        }

        [Test]
        public void CheckDeclarationEx2()
        {
            var extractor = new XmlExtractor();

            Assert.Throws<System.FormatException>(() => extractor.Append("<? -->"));
        }

        [Test]
        public void CheckDeclarationEx3()
        {
            var extractor = new XmlExtractor();

            Assert.Throws<System.FormatException>(() => extractor.Append("<? >"));
        }

        [Test]
        public void CheckDeclarationEx4()
        {
            var extractor = new XmlExtractor();

            Assert.Throws<System.FormatException>(() => extractor.Append("<? < ?>"));
        }

        [Test]
        public void CheckElement1()
        {
            var extractor = new XmlExtractor();

            extractor.Append("<foo>");

            Assert.IsTrue(extractor.Started);

            extractor.Append("</foo>");

            Assert.IsTrue(extractor.Started);
            Assert.IsTrue(extractor.Finished);
        }

        [Test]
        public void CheckElement2()
        {
            var extractor = new XmlExtractor();

            extractor.Append("<foo/>");

            Assert.IsTrue(extractor.Started);
            Assert.IsTrue(extractor.Finished);
        }

        [Test]
        public void CheckElementEx1()
        {
            var extractor = new XmlExtractor();

            Assert.Throws<System.FormatException>(() => extractor.Append("< < > >"));
        }

        [Test]
        public void CheckElementEx2()
        {
            var extractor = new XmlExtractor();

            Assert.Throws<System.FormatException>(() => extractor.Append(">"));
        }

        [Test]
        public void CheckElementEx3()
        {
            var extractor = new XmlExtractor();

            extractor.Append("<foo></foo>");
            Assert.Throws<System.InvalidOperationException>(() => extractor.Append("<bar></bar>"));
        }

        [Test]
        public void CheckComment1()
        {
            var extractor = new XmlExtractor();

            extractor.Append("<foo><!-- </foo> < < < -->");
            extractor.Append("<bar></bar>");
            extractor.Append("</foo>");
        }

        [Test]
        public void CheckComment2()
        {
            var extractor = new XmlExtractor();

            extractor.Append("<!-- 1 -->");
            Assert.IsFalse(extractor.Finished);
            extractor.Append("<!-- 2 -->");
            Assert.IsFalse(extractor.Finished);
            extractor.Append("<bar></bar>");
            Assert.IsTrue(extractor.Finished);
        }

        [Test]
        public void CheckComment3()
        {
            var extractor = new XmlExtractor();

            extractor.Append("<!-- 1 -->");

            Assert.IsTrue(extractor.Started);
            Assert.IsFalse(extractor.Finished);
        }

        [Test]
        public void CheckExcessEx1()
        {
            var extractor = new XmlExtractor();

            Assert.Throws<System.FormatException>(() => extractor.Append("<!-- 1 --> abc"));
        }

        [Test]
        public void CheckExcessEx2()
        {
            var extractor = new XmlExtractor();

            Assert.Throws<System.FormatException>(() => extractor.Append("x"));
        }

        [Test]
        public void CheckExcessEx3()
        {
            var extractor = new XmlExtractor();

            Assert.Throws<System.FormatException>(() => extractor.Append("<? xml ?> abc"));
        }

        [Test]
        public void CheckExcess1()
        {
            var extractor = new XmlExtractor();

            extractor.Append("<foo>x</foo>  y");

            Assert.IsTrue(extractor.Finished);
            Assert.AreEqual("y", extractor.ExcessText);
        }
    }
}
