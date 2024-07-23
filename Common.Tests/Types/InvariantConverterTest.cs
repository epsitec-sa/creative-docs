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

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using NUnit.Framework;
using System;
using System.Globalization;

namespace Epsitec.Common.Tests.Types
{
    [TestFixture]
    public class InvariantConverterTest
    {
        [Test]
        public void CheckIsNull()
        {
            object a = null;
            object b = System.DBNull.Value;

            Assert.IsTrue(InvariantConverter.IsNull(a));
            Assert.IsTrue(InvariantConverter.IsNull(b));
            Assert.IsTrue(!InvariantConverter.IsNotNull(a));
            Assert.IsTrue(!InvariantConverter.IsNotNull(b));
        }

        [Test]
        public void CheckToDateTime()
        {
            object a = null;
            object b = System.DBNull.Value;
            object c = "";
            object d = 632350746051230000;
            object e = new System.DateTime(2004, 11, 3, 10, 30, 5, 123);
            object f = "2004-11-03T10:30:05.1230000";
            object g = "2004-11-03 10:30:05Z";
            object h = "2004-11-03 10:30:05";

            System.DateTime result;

            Assert.IsTrue(!InvariantConverter.Convert(a, out result));
            Assert.IsTrue(!InvariantConverter.Convert(b, out result));
            Assert.IsTrue(!InvariantConverter.Convert(c, out result));

            Assert.IsTrue(InvariantConverter.Convert(d, out result));
            Assert.AreEqual(new System.DateTime(2004, 11, 3, 10, 30, 5, 123), result);
            Assert.IsTrue(InvariantConverter.Convert(e, out result));
            Assert.AreEqual(new System.DateTime(2004, 11, 3, 10, 30, 5, 123), result);
            Assert.IsTrue(InvariantConverter.Convert(f, out result));
            Assert.AreEqual(new System.DateTime(2004, 11, 3, 10, 30, 5, 123), result);
            Assert.IsTrue(InvariantConverter.Convert(g, out result));
            Assert.AreEqual(new System.DateTime(2004, 11, 3, 10, 30, 5, 0), result);
            Assert.IsTrue(InvariantConverter.Convert(h, out result));
            Assert.AreEqual(new System.DateTime(2004, 11, 3, 10, 30, 5, 0), result);

            System.DateTime tloc = System.DateTime.Now;
            System.DateTime tutc = System.DateTime.UtcNow;

            System.Console.WriteLine(
                "Now [u] : {0}, UTC {1}",
                tloc.ToString("u"),
                tutc.ToString("u")
            );
            System.Console.WriteLine(
                "Now [U] : {0}, UTC {1}",
                tloc.ToString("U"),
                tutc.ToString("U")
            );
            System.Console.WriteLine(
                "Now [r] : {0}, UTC {1}",
                tloc.ToString("r"),
                tutc.ToString("r")
            );
            System.Console.WriteLine(
                "Now [s] : {0}, UTC {1}",
                tloc.ToString("s"),
                tutc.ToString("s")
            );
        }

        [Test]
        public void CheckToDecimal()
        {
            object a = null;
            object b = System.DBNull.Value;
            object c = "";
            object d = 10;
            object e = 10.0;
            object f = 10.00M;
            object g = true;
            object h = "10";
            object i = "10.00";

            decimal result;

            Assert.IsTrue(!InvariantConverter.Convert(a, out result));
            Assert.IsTrue(!InvariantConverter.Convert(b, out result));
            Assert.IsTrue(!InvariantConverter.Convert(c, out result));

            Assert.IsTrue(InvariantConverter.Convert(d, out result));
            Assert.IsTrue(result == 10);
            Assert.IsTrue(InvariantConverter.Convert(e, out result));
            Assert.IsTrue(result == 10);
            Assert.IsTrue(InvariantConverter.Convert(f, out result));
            Assert.IsTrue(result == 10);
            Assert.IsTrue(InvariantConverter.Convert(g, out result));
            Assert.IsTrue(result == 1);
            Assert.IsTrue(InvariantConverter.Convert(h, out result));
            Assert.IsTrue(result == 10);
            Assert.IsTrue(InvariantConverter.Convert(i, out result));
            Assert.IsTrue(result == 10);
        }

        [Test]
        public void CheckToDecimalEx1()
        {
            decimal result;
            Assert.Throws<System.FormatException>(
                () => InvariantConverter.Convert("X", out result)
            );
        }

        [Test]
        public void CheckToDecimalEx2()
        {
            decimal result;
            Assert.Throws<System.NotSupportedException>(
                () => InvariantConverter.Convert(new Rectangle(10, 20, 30, 40), out result)
            );
        }

        [Test]
        public void CheckToEnum1()
        {
            System.Enum v1,
                v2,
                v3,
                v;

            Assert.IsTrue(InvariantConverter.Convert(MyEnum.First, typeof(MyEnum), out v1));
            Assert.IsTrue(InvariantConverter.Convert("Second", typeof(MyEnum), out v2));
            Assert.IsTrue(InvariantConverter.Convert(99, typeof(MyEnum), out v3));

            Assert.AreEqual(MyEnum.First, v1);
            Assert.AreEqual(MyEnum.Second, v2);
            Assert.AreEqual(MyEnum.Extra, v3);

            Assert.IsFalse(InvariantConverter.Convert(0, typeof(MyEnum), out v));
            Assert.IsFalse(InvariantConverter.Convert("", typeof(MyEnum), out v));
            Assert.IsFalse(InvariantConverter.Convert("X", typeof(MyEnum), out v));
            Assert.IsFalse(InvariantConverter.Convert("0", typeof(MyEnum), out v));
        }

        [Test]
        public void CheckToEnum2()
        {
            System.Enum v1,
                v2,
                v3,
                v4,
                v;

            Assert.IsTrue(InvariantConverter.Convert(MyFlags.Flag1, typeof(MyFlags), out v1));
            Assert.IsTrue(InvariantConverter.Convert("Flag2, Flag4", typeof(MyFlags), out v2));
            Assert.IsTrue(InvariantConverter.Convert(9, typeof(MyFlags), out v3));
            Assert.IsTrue(InvariantConverter.Convert(0x0f, typeof(MyFlags), out v4));

            Assert.AreEqual(MyFlags.Flag1, v1);
            Assert.AreEqual(MyFlags.Flag2 | MyFlags.Flag4, v2);
            Assert.AreEqual(MyFlags.Flag1 | MyFlags.Flag4, v3);
            Assert.AreEqual(MyFlags.Flag1 | MyFlags.Flag2 | MyFlags.Flag3 | MyFlags.Flag4, v4);

            // Not supported for now: I could not find a simple way to check that a given numeric flag value
            // is valid when converting from numeric to flag enum type
            //Assert.IsFalse(InvariantConverter.Convert(-1, typeof(MyFlags), out v));
            //Assert.IsFalse(InvariantConverter.Convert("-1", typeof(MyFlags), out v));

            Assert.IsFalse(InvariantConverter.Convert("", typeof(MyFlags), out v));
            Assert.IsFalse(InvariantConverter.Convert("X", typeof(MyFlags), out v));
        }

        [Test]
        public void CheckToString()
        {
            object a = null;
            object b = System.DBNull.Value;
            object c = "test";
            object d = 10;
            object e = 10.0;
            object f = 10.00M;
            object g = true;
            object h = new Color(0.1, 0.2, 0.3, 0.4);
            object i = new Rectangle(10, 20, 30, 40);
            object j = new System.DateTime(2004, 11, 3, 10, 30, 5, 123, DateTimeKind.Utc);

            string result;

            Assert.IsTrue(!InvariantConverter.Convert(a, out result));
            Assert.IsTrue(!InvariantConverter.Convert(b, out result));

            Assert.IsTrue(InvariantConverter.Convert(c, out result));
            Assert.AreEqual("test", result);
            Assert.IsTrue(InvariantConverter.Convert(d, out result));
            Assert.AreEqual("10", result);
            Assert.IsTrue(InvariantConverter.Convert(e, out result));
            Assert.AreEqual("10", result);
            Assert.IsTrue(InvariantConverter.Convert(f, out result));
            Assert.AreEqual("10.00", result);
            Assert.IsTrue(InvariantConverter.Convert(g, out result));
            Assert.AreEqual("True", result);
            Assert.IsTrue(InvariantConverter.Convert(h, out result));
            Assert.AreEqual("#199A;#3;#4CCD;#6", result);
            Assert.IsTrue(InvariantConverter.Convert(i, out result));
            Assert.AreEqual("10;20;30;40", result);
            Assert.IsTrue(InvariantConverter.Convert(i, out result));
            Assert.AreEqual("10;20;30;40", result);
            Assert.IsTrue(InvariantConverter.Convert(j, out result));
            Assert.AreEqual("2004-11-03T10:30:05.1230000Z", result);
        }

        [Test]
        public void CheckTypeConverter()
        {
            ISerializationConverter convC = InvariantConverter.GetSerializationConverter(
                typeof(string)
            );
            ISerializationConverter convD = InvariantConverter.GetSerializationConverter(
                typeof(int)
            );
            ISerializationConverter convE = InvariantConverter.GetSerializationConverter(
                typeof(double)
            );
            ISerializationConverter convF = InvariantConverter.GetSerializationConverter(
                typeof(decimal)
            );
            ISerializationConverter convG = InvariantConverter.GetSerializationConverter(
                typeof(bool)
            );
            ISerializationConverter convH = InvariantConverter.GetSerializationConverter(
                typeof(Color)
            );
            ISerializationConverter convI = InvariantConverter.GetSerializationConverter(
                typeof(Rectangle)
            );
            ISerializationConverter convJ = InvariantConverter.GetSerializationConverter(
                typeof(System.DateTime)
            );
            ISerializationConverter convK = InvariantConverter.GetSerializationConverter(
                typeof(DecimalRange)
            );

            object c = "test";
            object d = 10;
            object e = 10.0;
            object f = 10.00M;
            object g = true;
            object h = new Color(0.1, 0.2, 0.3, 0.4);
            object i = new Rectangle(10, 20, 30, 40);
            object j = new System.DateTime(2004, 11, 3, 10, 30, 5, 123);
            object k = new DecimalRange(1, 100, 0.10M);

            Assert.AreEqual("test", convC.ConvertToString(c, null));
            Assert.AreEqual("10", convD.ConvertToString(d, null));
            Assert.AreEqual("10", convE.ConvertToString(e, null));
            Assert.AreEqual("10.00", convF.ConvertToString(f, null));
            Assert.AreEqual("True", convG.ConvertToString(g, null));

            Assert.AreEqual("#199A;#3;#4CCD;#6", convH.ConvertToString(h, null));
            Assert.AreEqual("10;20;30;40", convI.ConvertToString(i, null));
            Assert.AreEqual("11/03/2004 10:30:05", convJ.ConvertToString(j, null));
            Assert.AreEqual("1 100 0.10", convK.ConvertToString(k, null));

            Assert.AreEqual(k, convK.ConvertFromString("1 100 0.10", null));
        }

        [Test]
        public void CheckAutomaticValueConverter()
        {
            System.Globalization.CultureInfo culture = System
                .Globalization
                .CultureInfo
                .InvariantCulture;

            Assert.AreEqual(
                "Abc",
                AutomaticValueConverter.Instance.Convert(
                    new FormattedText("Abc"),
                    typeof(string),
                    null,
                    culture
                )
            );
            Assert.AreEqual(
                new FormattedText("B&amp;B"),
                AutomaticValueConverter.Instance.Convert(
                    "B&amp;B",
                    typeof(FormattedText),
                    null,
                    culture
                )
            );

            Assert.AreEqual(
                InvalidValue.Value,
                AutomaticValueConverter.Instance.Convert("abc", typeof(int), null, culture)
            );
            Assert.AreEqual(
                10,
                AutomaticValueConverter.Instance.Convert("10", typeof(int), null, culture)
            );
            Assert.AreEqual(
                "10",
                AutomaticValueConverter.Instance.ConvertBack(10, typeof(string), null, culture)
            );

            Assert.AreEqual(
                "First",
                AutomaticValueConverter.Instance.Convert(
                    MyEnum.First,
                    typeof(string),
                    null,
                    culture
                )
            );
            Assert.AreEqual(
                MyEnum.First,
                AutomaticValueConverter.Instance.Convert(
                    MyEnum.First,
                    typeof(MyEnum),
                    null,
                    culture
                )
            );
            Assert.AreEqual(
                MyEnum.First,
                AutomaticValueConverter.Instance.Convert("First", typeof(MyEnum), null, culture)
            );
            Assert.AreEqual(
                MyEnum.First,
                AutomaticValueConverter.Instance.Convert(1, typeof(MyEnum), null, culture)
            );
            Assert.AreEqual(
                InvalidValue.Value,
                AutomaticValueConverter.Instance.Convert("XXX", typeof(MyEnum), null, culture)
            );
            Assert.AreEqual(
                InvalidValue.Value,
                AutomaticValueConverter.Instance.Convert(
                    MyFlags.Flag1,
                    typeof(MyEnum),
                    null,
                    culture
                )
            );
            Assert.AreEqual(
                1.0,
                AutomaticValueConverter.Instance.Convert(
                    MyEnum.First,
                    typeof(double),
                    null,
                    culture
                )
            );
            Assert.AreEqual(
                3,
                AutomaticValueConverter.Instance.Convert(
                    MyFlags.Flag1 | MyFlags.Flag2,
                    typeof(int),
                    null,
                    culture
                )
            );
            Assert.AreEqual(
                MyFlags.Flag1 | MyFlags.Flag2,
                AutomaticValueConverter.Instance.Convert(3, typeof(MyFlags), null, culture)
            );
            Assert.AreEqual(
                "Flag1, Flag2",
                AutomaticValueConverter.Instance.Convert(
                    MyFlags.Flag1 | MyFlags.Flag2,
                    typeof(string),
                    null,
                    culture
                )
            );
            Assert.AreEqual(
                MyFlags.Flag1 | MyFlags.Flag2,
                AutomaticValueConverter.Instance.Convert(
                    "Flag2, Flag1",
                    typeof(MyFlags),
                    null,
                    culture
                )
            );
            // Not supported for now: I could not find a simple way to check that a given numeric flag value
            // is valid when converting from numeric to flag enum type
            /*
            Assert.AreEqual(
                InvalidValue.Value,
                AutomaticValueConverter.Instance.Convert(-1025, typeof(MyFlags), null, culture)
            );
            */
            Assert.AreEqual(
                "",
                AutomaticValueConverter.Instance.Convert("", typeof(string), null, culture)
            );
            Assert.AreEqual(
                InvalidValue.Value,
                AutomaticValueConverter.Instance.Convert("", typeof(int), null, culture)
            );
        }

        [Test]
        public void CheckTemplatizedStringConverter()
        {
            Assert.AreEqual("test", InvariantConverter.ConvertToString("test"));
            Assert.AreEqual("10", InvariantConverter.ConvertToString(10));
            Assert.AreEqual("10", InvariantConverter.ConvertToString(10.0));
            Assert.AreEqual("10.00", InvariantConverter.ConvertToString(10.00M));
            Assert.AreEqual("True", InvariantConverter.ConvertToString(true));
            Assert.AreEqual(
                "#199A;#3;#4CCD;#6",
                InvariantConverter.ConvertToString(Color.FromAlphaRgb(0.1, 0.2, 0.3, 0.4))
            );
            Assert.AreEqual(
                "10;20;30;40",
                InvariantConverter.ConvertToString(new Rectangle(10, 20, 30, 40))
            );
            Assert.AreEqual(
                "2004-11-03T10:30:05.1230000Z",
                InvariantConverter.ConvertToString(
                    new System.DateTime(2004, 11, 3, 10, 30, 5, 123, DateTimeKind.Utc)
                )
            );
            Assert.AreEqual(
                @"Test;123;C:\foo\bar\Test;A",
                InvariantConverter.ConvertToString(
                    new ResourceModuleId(
                        "Test",
                        @"C:\foo\bar\Test",
                        123,
                        Epsitec.Common.Support.ResourceModuleLayer.Application
                    )
                )
            );

            Assert.AreEqual("test", InvariantConverter.ConvertFromString<string>("test"));
            Assert.AreEqual(10, InvariantConverter.ConvertFromString<int>("10"));
            Assert.AreEqual(10, InvariantConverter.ConvertFromString<double>("10"));
            Assert.AreEqual(10.00M, InvariantConverter.ConvertFromString<decimal>("10.00"));
            Assert.AreEqual(true, InvariantConverter.ConvertFromString<bool>("True"));
            Assert.AreEqual(
                Color.FromAlphaRgb(0.1, 0.2, 0.3, 0.4),
                InvariantConverter.ConvertFromString<Color>("#199A;#3;#4CCD;#6")
            );
            Assert.AreEqual(
                new Rectangle(10, 20, 30, 40),
                InvariantConverter.ConvertFromString<Rectangle>("10;20;30;40")
            );
            Assert.AreEqual(
                new System.DateTime(2004, 11, 3, 10, 30, 5, 123, DateTimeKind.Utc),
                InvariantConverter.ConvertFromString<System.DateTime>(
                    "2004-11-03T10:30:05.1230000Z"
                )
            );
            Assert.AreEqual(
                new ResourceModuleId(
                    "Test",
                    @"C:\foo\bar\Test",
                    123,
                    Epsitec.Common.Support.ResourceModuleLayer.Application
                ),
                InvariantConverter.ConvertFromString<ResourceModuleId>(
                    @"Test;123;C:\foo\bar\Test;A"
                )
            );
        }

        [Test]
        public void MicrosoftBug()
        {
            //	Only 'r' and 'u' do not provide the proper output, when compared to
            //	the documentation...

            DateTime dt = DateTime.Now;
            String[] format =
            {
                "d",
                "D",
                "f",
                "F",
                "g",
                "G",
                "m",
                "r",
                "s",
                "t",
                "T",
                "u",
                "U",
                "y",
                "dddd, MMMM dd yyyy",
                "ddd, MMM d \"'\"yy",
                "dddd, MMMM dd",
                "M/yy",
                "dd-MM-yy",
            };
            String date;
            for (int i = 0; i < format.Length; i++)
            {
                date = dt.ToString(format[i], DateTimeFormatInfo.InvariantInfo);
                Console.WriteLine(String.Concat(format[i], " :", date));
            }
        }

        private enum MyEnum
        {
            None = -1,
            First = 1,
            Second = 2,
            Third = 3,
            Extra = 99
        }

        [System.Flags]
        private enum MyFlags
        {
            None = 0,
            Flag1 = 1,
            Flag2 = 2,
            Flag3 = 4,
            Flag4 = 8
        }
    }
}
