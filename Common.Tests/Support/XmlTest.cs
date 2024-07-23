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
using System.Xml.Linq;

namespace Epsitec.Common.Tests.Support
{
    [TestFixture]
    public class XmlTest
    {
        [Test]
        public void CheckCData()
        {
            string data1 = "";
            string data2 = "a > b";
            string data3 = "A<[[ ]]>B";

            XDocument doc1 = new XDocument(new XElement("data", Xml.ToCData(data1)));
            XDocument doc2 = new XDocument(new XElement("data", Xml.ToCData(data2)));
            XDocument doc3 = new XDocument(new XElement("data", Xml.ToCData(data3)));

            System.Console.WriteLine(doc1.ToString(SaveOptions.None));
            System.Console.WriteLine(doc2.ToString(SaveOptions.None));
            System.Console.WriteLine(doc3.ToString(SaveOptions.None));

            string result1 = Xml.FromCData(doc1.Element("data").FirstNode);
            string result2 = Xml.FromCData(doc2.Element("data").FirstNode);
            string result3 = Xml.FromCData(doc3.Element("data").FirstNode);

            System.Console.WriteLine(result1);
            System.Console.WriteLine(result2);
            System.Console.WriteLine(result3);

            Assert.AreEqual(data1, result1);
            Assert.AreEqual(data2, result2);
            Assert.AreEqual(data3, result3);
        }
    }
}
