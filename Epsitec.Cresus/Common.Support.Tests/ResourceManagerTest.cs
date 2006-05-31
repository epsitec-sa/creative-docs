//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture]
	public class ResourceManagerTest
	{
		[SetUp]
		public void SetUp()
		{
			this.manager = new ResourceManager (@"S:\Epsitec.Cresus\Common.Support.Tests");
			this.manager.DefineDefaultModuleName ("Test");
			this.manager.ActivePrefix = "file";
			this.manager.ActiveCulture = Resources.FindCultureInfo ("en");
		}

		[Test]
		public void CheckProviderCount()
		{
			Assert.Greater (this.manager.ProviderCount, 0);
		}

		[Test]
		public void CheckGetModuleInfos()
		{
			ResourceModuleInfo[] modules = Types.Collection.ToArray (this.manager.GetModuleInfos ("file"));
			
			Assert.AreEqual (3, modules.Length);
			Assert.AreEqual ("LowLevelTest", modules[0].Name);
			Assert.AreEqual ("OtherModule", modules[1].Name);
			Assert.AreEqual ("Test", modules[2].Name);
			Assert.AreEqual (5, modules[0].Id);
			Assert.AreEqual (31, modules[1].Id);
			Assert.AreEqual (4, modules[2].Id);
		}

		[Test]
		public void CheckGetBundle()
		{
			string t1 = "Hello, world";
			string t2 = "Druid - Hello, world";
			string t3 = "Druid - Good bye...";

			Assert.AreEqual (t1, this.manager.GetText ("file/Test:strings#Text1"));
			Assert.AreEqual (t1, this.manager.GetText ("file/4:strings#Text1"));
			Assert.AreEqual (t1, this.manager.GetText ("file:strings#Text1"));
			Assert.AreEqual (t1, this.manager.GetText ("/Test:strings#Text1"));
			Assert.AreEqual (t1, this.manager.GetText ("/:strings#Text1"));
			Assert.AreEqual (t1, this.manager.GetText (":strings#Text1"));
			Assert.AreEqual (t1, this.manager.GetText ("strings#Text1"));

			Assert.AreEqual (t2, this.manager.GetText ("file/Test:DruidData#$0"));
			Assert.AreEqual (t2, this.manager.GetText ("file/4:DruidData#$0"));
			Assert.AreEqual (t2, this.manager.GetText ("file:DruidData#$0"));
			Assert.AreEqual (t2, this.manager.GetText ("/Test:DruidData#$0"));
			Assert.AreEqual (t2, this.manager.GetText ("/:DruidData#$0"));
			Assert.AreEqual (t2, this.manager.GetText (":DruidData#$0"));
			Assert.AreEqual (t2, this.manager.GetText ("DruidData#$0"));

			Assert.AreEqual (t3, this.manager.GetText ("file/4:DruidData#$01"));

			Assert.AreEqual (t2, this.manager.GetText ("[4]"));
			Assert.AreEqual (t3, this.manager.GetText ("[4001]"));
		}

		[Test]
		public void CheckGetForeignModuleBundle()
		{
			string t1 = "Druid - From other module";
			string t2 = "Druid - Hello, world";

			Assert.AreEqual (t2, this.manager.GetText ("file:DruidData#$0"));

			Assert.AreEqual (t1, this.manager.GetText ("file/OtherModule:DruidData#$0"));
			Assert.AreEqual (t1, this.manager.GetText ("file/31:DruidData#$0"));

			Assert.AreEqual (t2, this.manager.GetText ("file:DruidData#$0"));
			Assert.AreEqual (t2, this.manager.GetText ("file/Test:DruidData#$0"));
			Assert.AreEqual (t2, this.manager.GetText ("file/4:DruidData#$0"));

			Assert.AreEqual (t1, this.manager.GetText ("[V]"));
			Assert.AreEqual (t2, this.manager.GetText ("[4]"));
		}

		private ResourceManager manager;
	}
}
