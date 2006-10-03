using NUnit.Framework;

namespace Epsitec.Common.Types
{
	[TestFixture]
	public class CollectionTest
	{
		[Test]
		public void CheckSortDescription()
		{
			SortDescription sort1 = new SortDescription (ListSortDirection.Ascending, "x");
			SortDescription sort2 = new SortDescription (ListSortDirection.Descending, "y");

			Assert.AreEqual (ListSortDirection.Ascending, sort1.Direction);
			Assert.AreEqual (ListSortDirection.Descending, sort2.Direction);
			Assert.AreEqual ("x", sort1.PropertyName);
			Assert.AreEqual ("y", sort2.PropertyName);

			ISerializationConverter conv = InvariantConverter.GetSerializationConverter (typeof (SortDescription));

			Assert.AreEqual ("A;x", conv.ConvertToString (sort1, null));
			Assert.AreEqual ("D;y", conv.ConvertToString (sort2, null));

			Assert.AreEqual (sort1, conv.ConvertFromString ("A;x", null));
			Assert.AreEqual (sort2, conv.ConvertFromString ("D;y", null));
		}
	}
}
