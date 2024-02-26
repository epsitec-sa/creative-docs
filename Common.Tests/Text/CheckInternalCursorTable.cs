//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Text.Internal;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Text
{
    /// <summary>
    /// Summary description for CheckInternalCursorTable.
    /// </summary>
    [TestFixture]
    public sealed class CheckInternalCursorTable
    {
        [Test]
        public static void RunTests()
        {
            CursorTable table = new CursorTable();

            CursorId id1 = table.NewCursor();

            Assert.IsTrue(id1 == 1);
            Assert.IsTrue(table.ReadCursor(id1) == Cursor.Empty);
            Assert.IsTrue(table.ReadCursor(id1).CursorState == CursorState.Copied);

            /*
            Debug.Expect.Exception(new Debug.Method(Ex1), typeof(AssertFailedException));
            Debug.Expect.Exception(new Debug.Method(Ex2), typeof(AssertFailedException));
            Debug.Expect.Exception(new Debug.Method(Ex3), typeof(AssertFailedException));
            */

            table.RecycleCursor(id1);

            CursorId id2 = table.NewCursor();

            Assert.IsTrue(id1 == 1);
            Assert.IsTrue(id2 == 1);

            /*
            Debug.Expect.Exception(new Debug.Method(Ex4), typeof(AssertFailedException));
            Debug.Expect.Exception(new Debug.Method(Ex5), typeof(AssertFailedException));
            */

            CursorId id3 = table.NewCursor();
            CursorId id4 = table.NewCursor();
            CursorId id5 = table.NewCursor();

            table.RecycleCursor(id3);

            System.Collections.ArrayList list = new System.Collections.ArrayList();

            foreach (CursorId id in table)
            {
                list.Add(id);
            }

            Assert.IsTrue(list.Count == 3);
            Assert.IsTrue((CursorId)list[0] == id2);
            Assert.IsTrue((CursorId)list[1] == id4);
            Assert.IsTrue((CursorId)list[2] == id5);
        }

        private static void Ex1()
        {
            CursorTable table = new CursorTable();
            table.ReadCursor(0);
        }

        private static void Ex2()
        {
            CursorTable table = new CursorTable();
            table.ReadCursor(1);
        }

        private static void Ex3()
        {
            CursorTable table = new CursorTable();
            table.ReadCursor(2);
        }

        private static void Ex4()
        {
            CursorTable table = new CursorTable();

            CursorId id = table.NewCursor();

            table.RecycleCursor(id);
            table.RecycleCursor(id);
        }

        private static void Ex5()
        {
            CursorTable table = new CursorTable();

            CursorId id = table.NewCursor();

            table.RecycleCursor(id);
            table.ReadCursor(id);
        }
    }
}
