using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.DataLayer.Infrastructure;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Infrastructure
{


	[TestClass]
	public sealed class UnitTestUidSlot
	{


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new UidSlot (-1, 0)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new UidSlot (0, -1)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new UidSlot (10, 5)
			);
		}


		[TestMethod]
		public void ConstructorAndProperties()
		{
			for (int i = 0; i < 10; i++)
			{
				for (int j = i; j < 10; j++)
				{
					UidSlot slot = new UidSlot (i, j);

					Assert.AreEqual (i, slot.MinValue);
					Assert.AreEqual (j, slot.MaxValue);
				}
			}
		}


	}


}
