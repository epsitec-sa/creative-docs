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
using Epsitec.Common.Types;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Support
{
    [TestFixture]
    public class ObjectDictMapperTest
    {
        [Test]
        public void CheckCopyFromDict()
        {
            StringDict dict = new StringDict();

            dict.Add("NumValue", "15");
            dict.Add("StringValue", "Hello");
            dict.Add("DecimalValue", "123.456");
            dict.Add("DateTimeValue", "2004-11-03T10:30:05.1230000");

            MyObject data = new MyObject();

            ObjectDictMapper.CopyFromDict(data, dict);

            Assert.AreEqual(15, data.NumValue);
            Assert.AreEqual("Hello", data.StringValue);
            Assert.AreEqual(123.456M, data.DecimalValue);
            Assert.AreEqual(new System.DateTime(2004, 11, 3, 10, 30, 5, 123), data.DateTimeValue);
        }

        [Test]
        public void CheckCopyToDict()
        {
            StringDict dict = new StringDict();
            MyObject data = new MyObject();

            data.NumValue = 15;
            data.StringValue = "Hello";
            data.DecimalValue = 123.456M;
            data.DateTimeValue = System.DateTime.UtcNow;

            ObjectDictMapper.CopyToDict(data, dict);

            Assert.AreEqual(4, dict.Count);

            Assert.AreEqual("15", dict["NumValue"]);
            Assert.AreEqual("Hello", dict["StringValue"]);
            Assert.AreEqual("123.456", dict["DecimalValue"]);

            System.DateTime date;
            InvariantConverter.Convert(dict["DateTimeValue"], out date);

            Assert.AreEqual(data.DateTimeValue, date);
        }

        [Test]
        public void CheckUpdateToDict()
        {
            StringDict dict = new StringDict();

            dict.Add("XYZ", "123");
            dict.Add("StringValue", "Abc");

            MyObject data = new MyObject();

            data.NumValue = 15;
            data.StringValue = "Hello";
            data.DecimalValue = 123.456M;
            data.DateTimeValue = System.DateTime.Now;

            ObjectDictMapper.UpdateToDict(data, dict);

            Assert.AreEqual(2, dict.Count);

            Assert.AreEqual("123", dict["XYZ"]);
            Assert.AreEqual("Hello", dict["StringValue"]);
        }

        public class MyObject
        {
            public int NumValue
            {
                get { return this.num_value; }
                set { this.num_value = value; }
            }
            public string StringValue
            {
                get { return this.string_value; }
                set { this.string_value = value; }
            }
            public decimal DecimalValue
            {
                get { return this.decimal_value; }
                set { this.decimal_value = value; }
            }

            public System.DateTime DateTimeValue
            {
                get { return this.date_time_value; }
                set { this.date_time_value = value; }
            }

            private int num_value;
            private string string_value;
            private decimal decimal_value;
            private System.DateTime date_time_value;
        }
    }
}
