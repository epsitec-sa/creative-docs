using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture]
	public class ResourceAccessorTest
	{
		[SetUp]
		public void Initialize()
		{
			this.manager = new ResourceManager (typeof (ResourceAccessorTest));
			this.manager.DefineDefaultModuleName ("Test");
		}

		[Test]
		public void CheckStringAccessor()
		{
			ResourceAccessors.StringResourceAccessor accessor = new ResourceAccessors.StringResourceAccessor ();

			accessor.Load (this.manager);

			Assert.AreEqual (8, accessor.Collection.Count);
			
			Assert.AreEqual (Druid.Parse ("[4002]"), accessor.Collection[Druid.Parse ("[4002]")].Id);
			Assert.AreEqual ("Text A", accessor.Collection[Druid.Parse ("[4002]")].GetCultureData ("00").GetValue (Res.Fields.ResourceString.Text));

			Assert.AreEqual (Druid.Parse ("[4006]"), accessor.Collection[Druid.Parse ("[4006]")].Id);
			Assert.AreEqual ("Text1", accessor.Collection[Druid.Parse ("[4006]")].Name);
			Assert.AreEqual ("Hello, world", accessor.Collection["Text1"].GetCultureData ("00").GetValue (Res.Fields.ResourceString.Text));

			Assert.AreEqual ("Bonjour", accessor.Collection["Text1"].GetCultureData ("fr").GetValue (Res.Fields.ResourceString.Text));
		}


		ResourceManager manager;
	}
}
