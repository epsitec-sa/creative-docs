using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture]
	public class CultureMapListTest
	{
		[SetUp]
		public void Initialize()
		{
			this.list = new CultureMapList (null);

			this.list.Add (Internal.Test.CreateCultureMap (null, Druid.Parse ("[0001]"), CultureMapSource.ReferenceModule));
			this.list.Add (Internal.Test.CreateCultureMap (null, Druid.Parse ("[0002]"), CultureMapSource.ReferenceModule));
			this.list.Add (Internal.Test.CreateCultureMap (null, Druid.Parse ("[0003]"), CultureMapSource.ReferenceModule));
			this.list.Add (Internal.Test.CreateCultureMap (null, Druid.Parse ("[0004]"), CultureMapSource.ReferenceModule));

			this.list[0].Name = "A";
			this.list[1].Name = "B";
			this.list[2].Name = "C";
			this.list[3].Name = "D";
		}

		[Test]
		public void CheckItemOperatorByName()
		{
			Assert.AreEqual (this.list["A"], this.list[0]);
			Assert.AreEqual (this.list["B"], this.list[1]);
			Assert.AreEqual (this.list["C"], this.list[2]);
			Assert.AreEqual (this.list["D"], this.list[3]);
		}

		[Test]
		public void CheckItemOperatorByDruid()
		{
			Assert.AreEqual (this.list[Druid.Parse ("[0001]")], this.list[0]);
			Assert.AreEqual (this.list[Druid.Parse ("[0002]")], this.list[1]);
			Assert.AreEqual (this.list[Druid.Parse ("[0003]")], this.list[2]);
			Assert.AreEqual (this.list[Druid.Parse ("[0004]")], this.list[3]);
		}

		[Test]
		[ExpectedException (typeof (System.InvalidOperationException))]
		public void CheckItemOperatorEx1()
		{
			System.Collections.Generic.IList<CultureMap> list = this.list;
			list[0] = null;
		}

		[Test]
		[ExpectedException (typeof (System.ArgumentOutOfRangeException))]
		public void CheckItemOperatorEx2()
		{
			CultureMap map = this.list[4];
		}


		private CultureMapList list;
	}
}
