using Epsitec.Common.UnitTesting;

using Epsitec.Common.Types.Collections;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Common.Types.UnitTests.Collections
{


	[TestClass]
	public sealed class UnitTestCircularBuffer
	{


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new CircularBuffer<int> (-1)
			);
		}


		[TestMethod]
		public void IndexerArgumentCheck()
		{
			CircularBuffer<int> buffer = new CircularBuffer<int> (10);

			for (int i = 0; i < 5; i++)
			{
				buffer.Add (i);
			}

			ExceptionAssert.Throw<System.ArgumentOutOfRangeException>
			(
				() => { int test = buffer[-1]; }
			);

			ExceptionAssert.Throw<System.ArgumentOutOfRangeException>
			(
				() => { int test = buffer[5]; }
			);
		}
		

		[TestMethod]
		public void AddArgumentCheck()
		{
			CircularBuffer<int> buffer = new CircularBuffer<int> (10);

			for (int i = 0; i < 10; i++)
			{
				buffer.Add (i);
			}

			ExceptionAssert.Throw<System.InvalidOperationException>
			(
				() => buffer.Add (0)
			);
		}


		[TestMethod]
		public void RemoveArgumentCheck()
		{
			CircularBuffer<int> buffer = new CircularBuffer<int> (10);

			ExceptionAssert.Throw<System.InvalidOperationException>
			(
				() => buffer.Remove ()
			);
		}


		[TestMethod]
		public void FillAndEmptyBufferTest()
		{
			int size = 10;

			List<int> list = new List<int> ();
			CircularBuffer<int> buffer = new CircularBuffer<int> (size);

			Assert.AreEqual (size, buffer.Size);

			for (int i = 0; i < 5; i++)
			{
				this.Check (list, buffer);

				for (int j = 0; j < size; j++)
				{
					buffer.Add (j);
					list.Add (j);

					this.Check (list, buffer);
				}
				
				for (int j = 0; j < size; j++)
				{
					buffer.Remove ();
					list.RemoveAt (0);

					this.Check (list, buffer);
				}
			}
		}


		[TestMethod]
		public void KeepAlmostFullBuffer()
		{
			int size = 10;

			List<int> list = new List<int> ();
			CircularBuffer<int> buffer = new CircularBuffer<int> (size);

			for (int i = 0; i < buffer.Size - 1; i++)
			{
				buffer.Add (i);
				list.Add (i);
			}

			this.Check (list, buffer);

			for (int i = 0; i < 10; i++)
			{
				buffer.Add (i);
				list.Add (i);

				this.Check (list, buffer);

				buffer.Remove ();
				list.RemoveAt (0);

				this.Check (list, buffer);
			}
		}


		[TestMethod]
		public void KeepAlmostEmptyBuffer()
		{
			int size = 10;

			List<int> list = new List<int> ();
			CircularBuffer<int> buffer = new CircularBuffer<int> (size);

			this.Check (list, buffer);

			for (int i = 0; i < 20; i++)
			{
				buffer.Add (i);
				list.Add (i);

				this.Check (list, buffer);

				buffer.Remove ();
				list.RemoveAt (0);

				this.Check (list, buffer);
			}
		}


		[TestMethod]
		public void ClearTest()
		{
			int size = 10;

			CircularBuffer<object> buffer = new CircularBuffer<object> (size);

			for (int i = 0; i < size; i++)
			{
				while (buffer.Count < i)
				{
					buffer.Add (new object ());
				}

				Assert.AreEqual (i, buffer.Count);

				buffer.Clear ();

				Assert.AreEqual (0, buffer.Count);
			}
		}


		private void Check<T>(List<T> expected, CircularBuffer<T> actual)
		{
			Assert.AreEqual (expected.Count, actual.Count);

			for (int i = 0; i < expected.Count; i++)
			{
				Assert.AreEqual (expected[i], actual[i]);
			}
		}


	}


}
