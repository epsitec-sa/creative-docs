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
using Epsitec.Common.Types;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Text
{
    /// <summary>
    /// Summary description for CheckSerializerSupport.
    /// </summary>
    [TestFixture]
    public sealed class CheckSerializerSupport
    {
        [Test]
        public static void RunTests()
        {
            Assert.IsTrue(@"[null]" == SerializerSupport.SerializeString(null));
            Assert.IsTrue(@"\1\2\5x\0y" == SerializerSupport.SerializeString(@"[]/x\y"));
            Assert.IsTrue(
                @"a\5b\05c" == SerializerSupport.SerializeStringArray(new string[] { "a", "b/c" })
            );
            Assert.IsTrue(@"[null]" == SerializerSupport.SerializeStringArray(null));
            Assert.IsTrue(
                @"[empty]" == SerializerSupport.SerializeStringArray(new string[] { })
            );

            Assert.IsTrue(@"[NaN]" == SerializerSupport.SerializeDouble(System.Double.NaN));
            Assert.IsTrue(@"10" == SerializerSupport.SerializeDouble(10));

            Assert.IsTrue(@"[true]" == SerializerSupport.SerializeBoolean(true));
            Assert.IsTrue(@"[false]" == SerializerSupport.SerializeBoolean(false));

            Assert.IsTrue(@"a/b/c" == SerializerSupport.Join("a", "b", "c"));

            string[] abc = SerializerSupport.Split(@"a/b/c");

            Assert.IsTrue(abc.Length == 3);
            Assert.IsTrue(abc[0] == "a");
            Assert.IsTrue(abc[1] == "b");
            Assert.IsTrue(abc[2] == "c");

            Assert.IsTrue(double.IsNaN(SerializerSupport.DeserializeDouble("[NaN]")));
            Assert.IsTrue(10 == SerializerSupport.DeserializeDouble("10"));

            Assert.IsTrue(SerializerSupport.DeserializeBoolean("[true]"));
            Assert.IsFalse(SerializerSupport.DeserializeBoolean("[false]"));

            string[] array1 = SerializerSupport.DeserializeStringArray(@"a\5b\05c");
            string[] array2 = new string[] { "a", "b/c" };

            Assert.IsTrue(Comparer.Equal(array1, array2));
            Assert.IsNull(SerializerSupport.DeserializeStringArray("[null]"));
            Assert.IsTrue(SerializerSupport.DeserializeStringArray("[empty]").Length == 0);
        }
    }
}
