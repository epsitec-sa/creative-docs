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
    public class CultureMapListTest
    {
        [SetUp]
        public void Initialize()
        {
            this.list = new CultureMapList(null);

            this.list.Add(
                Epsitec.Common.Support.Internal.Test.CreateCultureMap(
                    null,
                    Druid.Parse("[0001]"),
                    CultureMapSource.ReferenceModule
                )
            );
            this.list.Add(
                Epsitec.Common.Support.Internal.Test.CreateCultureMap(
                    null,
                    Druid.Parse("[0002]"),
                    CultureMapSource.ReferenceModule
                )
            );
            this.list.Add(
                Epsitec.Common.Support.Internal.Test.CreateCultureMap(
                    null,
                    Druid.Parse("[0003]"),
                    CultureMapSource.ReferenceModule
                )
            );
            this.list.Add(
                Epsitec.Common.Support.Internal.Test.CreateCultureMap(
                    null,
                    Druid.Parse("[0004]"),
                    CultureMapSource.ReferenceModule
                )
            );

            this.list[0].Name = "A";
            this.list[1].Name = "B";
            this.list[2].Name = "C";
            this.list[3].Name = "D";
        }

        [Test]
        public void CheckItemOperatorByName()
        {
            Assert.AreEqual(this.list["A"], this.list[0]);
            Assert.AreEqual(this.list["B"], this.list[1]);
            Assert.AreEqual(this.list["C"], this.list[2]);
            Assert.AreEqual(this.list["D"], this.list[3]);
        }

        [Test]
        public void CheckItemOperatorByDruid()
        {
            Assert.AreEqual(this.list[Druid.Parse("[0001]")], this.list[0]);
            Assert.AreEqual(this.list[Druid.Parse("[0002]")], this.list[1]);
            Assert.AreEqual(this.list[Druid.Parse("[0003]")], this.list[2]);
            Assert.AreEqual(this.list[Druid.Parse("[0004]")], this.list[3]);
        }

        [Test]
        public void CheckItemOperatorEx1()
        {
            System.Collections.Generic.IList<CultureMap> list = this.list;
            Assert.Throws<System.InvalidOperationException>(() => list[0] = null);
        }

        [Test]
        public void CheckItemOperatorEx2()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() =>
            {
                CultureMap map = this.list[4];
            });
        }

        private CultureMapList list;
    }
}
