//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Text.Internal;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Text
{
    /// <summary>
    /// Summary description for CheckInternalCursorIdArray.
    /// </summary>
    [TestFixture]
    public sealed class CheckInternalCursorIdArray
    {
        [Test]
        public static void RunTests()
        {
            CursorIdArray array = new CursorIdArray();

            CursorId id1 = 1;
            CursorId id2 = 2;
            CursorId id3 = 3;

            array.Add(id1, 0);
            array.Add(id2, 4);
            array.Add(id3, 1);

            Assert.IsTrue(array.GetCursorPosition(id1) == 0);
            Assert.IsTrue(array.GetCursorPosition(id2) == 4);
            Assert.IsTrue(array.GetCursorPosition(id3) == 1);

            array.Move(id3, 2);

            Assert.IsTrue(array.GetCursorPosition(id1) == 0);
            Assert.IsTrue(array.GetCursorPosition(id2) == 4);
            Assert.IsTrue(array.GetCursorPosition(id3) == 2);

            array.Move(id3, 1);

            Assert.IsTrue(array.GetCursorPosition(id1) == 0);
            Assert.IsTrue(array.GetCursorPosition(id2) == 4);
            Assert.IsTrue(array.GetCursorPosition(id3) == 1);

            array.Move(id3, 4);

            Assert.IsTrue(array.GetCursorPosition(id1) == 0);
            Assert.IsTrue(array.GetCursorPosition(id2) == 4);
            Assert.IsTrue(array.GetCursorPosition(id3) == 4);

            array.Move(id3, 0);

            Assert.IsTrue(array.GetCursorPosition(id1) == 0);
            Assert.IsTrue(array.GetCursorPosition(id2) == 4);
            Assert.IsTrue(array.GetCursorPosition(id3) == 0);

            array.Move(id3, 10);

            Assert.IsTrue(array.GetCursorPosition(id1) == 0);
            Assert.IsTrue(array.GetCursorPosition(id2) == 4);
            Assert.IsTrue(array.GetCursorPosition(id3) == 10);

            array.Move(id1, 1);
            array.Move(id2, 0);

            Assert.IsTrue(array.GetCursorPosition(id1) == 1);
            Assert.IsTrue(array.GetCursorPosition(id2) == 0);
            Assert.IsTrue(array.GetCursorPosition(id3) == 10);

            array.Move(id1, 1);
            array.Move(id2, 2);
            array.Move(id3, 3);

            Assert.IsTrue(array.GetCursorPosition(id1) == 1);
            Assert.IsTrue(array.GetCursorPosition(id2) == 2);
            Assert.IsTrue(array.GetCursorPosition(id3) == 3);

            array.Remove(id1);

            Assert.IsTrue(array.GetCursorPosition(id2) == 2);
            Assert.IsTrue(array.GetCursorPosition(id3) == 3);
            Assert.IsTrue(array.ElementCount == 2);

            array.Add(id1, 1);

            Assert.IsTrue(array.GetCursorPosition(id1) == 1);
            Assert.IsTrue(array.GetCursorPosition(id2) == 2);
            Assert.IsTrue(array.GetCursorPosition(id3) == 3);

            array.Remove(id2);

            Assert.IsTrue(array.GetCursorPosition(id1) == 1);
            Assert.IsTrue(array.GetCursorPosition(id3) == 3);
            Assert.IsTrue(array.ElementCount == 2);

            array.Add(id2, 2);

            Assert.IsTrue(array.GetCursorPosition(id1) == 1);
            Assert.IsTrue(array.GetCursorPosition(id2) == 2);
            Assert.IsTrue(array.GetCursorPosition(id3) == 3);

            array.Remove(id3);

            Assert.IsTrue(array.GetCursorPosition(id1) == 1);
            Assert.IsTrue(array.GetCursorPosition(id2) == 2);
            Assert.IsTrue(array.ElementCount == 2);

            array.Add(id3, 3);

            Assert.IsTrue(array.GetCursorPosition(id1) == 1);
            Assert.IsTrue(array.GetCursorPosition(id2) == 2);
            Assert.IsTrue(array.GetCursorPosition(id3) == 3);

            array.Remove(id3);
            array.Remove(id1);

            Assert.IsTrue(array.GetCursorPosition(id2) == 2);
            Assert.IsTrue(array.ElementCount == 1);

            array.Remove(id2);

            Assert.IsTrue(array.ElementCount == 0);

            CursorIdArray a = new CursorIdArray();
            CursorIdArray b = new CursorIdArray();

            a.Add(id1, 10);
            a.Add(id2, 20);
            a.Add(id3, 30);

            a.ProcessMigration(25, ref b);

            Assert.IsTrue(a.ElementCount == 2);
            Assert.IsTrue(b.ElementCount == 1);

            Assert.IsTrue(a.GetCursorPosition(id1) == 10);
            Assert.IsTrue(a.GetCursorPosition(id2) == 20);
            Assert.IsTrue(b.GetCursorPosition(id3) == 5);

            a.ProcessMigration(20, ref b);

            Assert.IsTrue(a.ElementCount == 2);
            Assert.IsTrue(b.ElementCount == 1);

            Assert.IsTrue(a.GetCursorPosition(id1) == 10);
            Assert.IsTrue(a.GetCursorPosition(id2) == 20);
            Assert.IsTrue(b.GetCursorPosition(id3) == 5);

            a.ProcessMigration(18, ref b);

            Assert.IsTrue(a.ElementCount == 1);
            Assert.IsTrue(b.ElementCount == 2);

            Assert.IsTrue(a.GetCursorPosition(id1) == 10);
            Assert.IsTrue(b.GetCursorPosition(id2) == 2);
            Assert.IsTrue(b.GetCursorPosition(id3) == 5);

            a.Add(id2, 20);
            a.Add(id3, 30);

            b.Remove(id2);
            b.Remove(id3);

            a.ProcessMigration(5, ref b);

            Assert.IsTrue(a.ElementCount == 0);
            Assert.IsTrue(b.ElementCount == 3);

            Assert.IsTrue(b.GetCursorPosition(id1) == 5);
            Assert.IsTrue(b.GetCursorPosition(id2) == 15);
            Assert.IsTrue(b.GetCursorPosition(id3) == 25);
        }
    }
}
