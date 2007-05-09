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
			
			Assert.IsFalse (accessor.ContainsChanges);

			accessor.Load (this.manager);

			Assert.AreEqual (8, accessor.Collection.Count);
			
			Assert.AreEqual (Druid.Parse ("[4002]"), accessor.Collection[Druid.Parse ("[4002]")].Id);
			Assert.AreEqual ("Text A", accessor.Collection[Druid.Parse ("[4002]")].GetCultureData ("00").GetValue (Res.Fields.ResourceString.Text));

			Assert.AreEqual (Druid.Parse ("[4006]"), accessor.Collection[Druid.Parse ("[4006]")].Id);
			Assert.AreEqual ("Text1", accessor.Collection[Druid.Parse ("[4006]")].Name);
			Assert.AreEqual ("Hello, world", accessor.Collection["Text1"].GetCultureData ("00").GetValue (Res.Fields.ResourceString.Text));

			Types.StructuredData data1 = accessor.Collection["Text1"].GetCultureData ("fr");
			Types.StructuredData data2 = accessor.Collection["Text1"].GetCultureData ("fr");

			Assert.AreSame (data1, data2);
			Assert.AreEqual ("Bonjour", data1.GetValue (Res.Fields.ResourceString.Text));
			Assert.IsFalse (accessor.ContainsChanges);

			data1 = accessor.Collection["Text1"].GetCultureData ("de");
			data2 = accessor.Collection["Text1"].GetCultureData ("de");

			Assert.IsNotNull (data1);
			Assert.AreSame (data1, data2);
			Assert.AreEqual (Types.UndefinedValue.Instance, data1.GetValue (Res.Fields.ResourceString.Text));
			Assert.IsFalse (accessor.ContainsChanges);
		}


		ResourceManager manager;
	}
}
