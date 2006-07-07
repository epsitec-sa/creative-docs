//	Copyright � 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;
using System.Collections.Generic;

using Epsitec.Common.Types;

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
		public void CheckNormalization()
		{
			Assert.AreEqual ("file/4:strings", this.manager.NormalizeFullId ("file/4:strings"));
			Assert.AreEqual ("file/4:strings", this.manager.NormalizeFullId ("file/:strings"));
			Assert.AreEqual ("file/4:strings", this.manager.NormalizeFullId ("file/Test:strings"));
			Assert.AreEqual ("file/4:strings", this.manager.NormalizeFullId ("file:strings"));
			Assert.AreEqual ("file/4:strings", this.manager.NormalizeFullId (":strings"));
			Assert.AreEqual ("file/4:strings", this.manager.NormalizeFullId ("/:strings"));
			Assert.AreEqual ("file/4:strings", this.manager.NormalizeFullId ("/4:strings"));
			Assert.AreEqual ("file/4:strings", this.manager.NormalizeFullId ("/Test:strings"));
			Assert.AreEqual ("file/4:strings", this.manager.NormalizeFullId ("strings"));
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

			Assert.AreEqual (t2, this.manager.GetText ("file/Test:strings#$0"));
			Assert.AreEqual (t2, this.manager.GetText ("file/4:strings#$0"));
			Assert.AreEqual (t2, this.manager.GetText ("file:strings#$0"));
			Assert.AreEqual (t2, this.manager.GetText ("/Test:strings#$0"));
			Assert.AreEqual (t2, this.manager.GetText ("/:strings#$0"));
			Assert.AreEqual (t2, this.manager.GetText (":strings#$0"));
			Assert.AreEqual (t2, this.manager.GetText ("strings#$0"));

			Assert.AreEqual (t3, this.manager.GetText ("file/4:strings#$01"));

			Assert.AreEqual (t2, this.manager.GetText ("[4]"));
			Assert.AreEqual (t3, this.manager.GetText ("[4001]"));
			Assert.AreEqual (t3, this.manager.GetText (new Druid (4, 0, 1)));

			ResourceBundle bundle = this.manager.GetBundle (new Druid (4, 0, 0));

			Assert.IsNotNull (bundle);
			Assert.AreEqual ("DruidExperimentalBundle", bundle.Caption);
			Assert.AreEqual (1, bundle.FieldCount);
			Assert.AreEqual (new Druid (4, 0, 0), bundle.Druid);

			Assert.IsTrue (this.manager.GetBundle ("Strings").Druid.IsEmpty);
			Assert.AreEqual ("Strings", this.manager.GetBundle ("file:Strings").Name);
			Assert.AreEqual ("file/4:Strings", this.manager.GetBundle ("file:Strings").PrefixedName);
		}

		[Test]
		[ExpectedException (typeof (ResourceException))]
		public void CheckGetBundleEx1()
		{
			this.manager.GetBundle ("file:Strings#Text1");
		}

		[Test]
		[ExpectedException (typeof (ResourceException))]
		public void CheckGetBundleEx2()
		{
			this.manager.GetBundle ("[4]");
		}

		[Test]
		public void CheckGetBundleField()
		{
			string t2 = "Druid - Hello, world";
			string t3 = "Druid - Good bye...";

			Assert.AreEqual (t2, this.manager.GetBundleField (Druid.Parse ("[4]"), ResourceLevel.Default).AsString);
			Assert.AreEqual (t3, this.manager.GetBundleField (Druid.Parse ("[4001]"), ResourceLevel.Default).AsString);
		}

		[Test]
		public void CheckGetCaption()
		{
			Druid idA = Druid.Parse ("[4002]");
			Druid idQ = Druid.Parse ("[4003]");

			List<Caption> scrap = new List<Caption> ();

			Caption captionA;
			Caption captionQ;

			int n = this.manager.DebugCountLiveCaptions ();

			captionA = this.manager.GetCaption (idA, ResourceLevel.Default);
			captionQ = this.manager.GetCaption (idQ, ResourceLevel.Default);
			
			Assert.AreEqual ("Pattern angle expressed in degrees.", captionA.Description);
			Assert.AreEqual ("Quality coefficient.", captionQ.Description);
			Assert.AreEqual ("A", Collection.Extract (captionA.SortedLabels, 0));
			Assert.AreEqual ("Pattern angle", Collection.Extract (captionA.SortedLabels, 2));
			Assert.AreEqual ("Q", Collection.Extract (captionQ.SortedLabels, 0));

			Assert.AreEqual (captionA, this.manager.GetCaption (idA, ResourceLevel.Default));
			Assert.AreEqual (captionQ, this.manager.GetCaption (idQ, ResourceLevel.Default));

			Assert.AreEqual (2, this.manager.DebugCountLiveCaptions () - n);

			scrap.Add (captionA);
			scrap.Add (captionQ);
			
			manager.ActiveCulture = Resources.FindSpecificCultureInfo ("en");
			
			captionA = this.manager.GetCaption (idA, ResourceLevel.Merged);
			captionQ = this.manager.GetCaption (idQ, ResourceLevel.Merged);

			Assert.AreEqual (4, this.manager.DebugCountLiveCaptions () - n);

			scrap.Add (captionA);
			scrap.Add (captionQ);
			
			Assert.AreEqual ("Pattern angle expressed in degrees.", captionA.Description);
			Assert.AreEqual ("Quality coefficient.", captionQ.Description);
			Assert.AreEqual ("A", Collection.Extract (captionA.SortedLabels, 0));
			Assert.AreEqual ("Pattern angle", Collection.Extract (captionA.SortedLabels, 2));
			Assert.AreEqual ("Q", Collection.Extract (captionQ.SortedLabels, 0));

			manager.ActiveCulture = Resources.FindSpecificCultureInfo ("fr");

			captionA = this.manager.GetCaption (idA, ResourceLevel.Merged);
			captionQ = this.manager.GetCaption (idQ, ResourceLevel.Merged);

			Assert.AreEqual (6, this.manager.DebugCountLiveCaptions () - n);

			scrap.Add (captionA);
			scrap.Add (captionQ);
			
			Assert.AreEqual ("Angle de rotation de la trame, exprim� en degr�s.", captionA.Description);
			Assert.AreEqual ("Coefficient de Qualit�.", captionQ.Description);
			Assert.AreEqual ("A", Collection.Extract (captionA.SortedLabels, 0));
			Assert.AreEqual ("Angle de la trame", Collection.Extract (captionA.SortedLabels, 2));
			Assert.AreEqual ("Q", Collection.Extract (captionQ.SortedLabels, 0));
			
			manager.ActiveCulture = Resources.FindSpecificCultureInfo ("en");

			Assert.AreEqual (6, this.manager.DebugCountLiveCaptions () - n);

			Assert.AreEqual ("Pattern angle expressed in degrees.", captionA.Description);
			Assert.AreEqual ("Quality coefficient.", captionQ.Description);
			Assert.AreEqual ("A", Collection.Extract (captionA.SortedLabels, 0));
			Assert.AreEqual ("Pattern angle", Collection.Extract (captionA.SortedLabels, 2));
			Assert.AreEqual ("Q", Collection.Extract (captionQ.SortedLabels, 0));
			
			manager.ActiveCulture = Resources.FindSpecificCultureInfo ("fr");
			
			Assert.AreEqual ("Angle de rotation de la trame, exprim� en degr�s.", captionA.Description);
			Assert.AreEqual ("Coefficient de Qualit�.", captionQ.Description);
			Assert.AreEqual ("A", Collection.Extract (captionA.SortedLabels, 0));
			Assert.AreEqual ("Angle de la trame", Collection.Extract (captionA.SortedLabels, 2));
			Assert.AreEqual ("Q", Collection.Extract (captionQ.SortedLabels, 0));
			
			Assert.AreEqual (6, this.manager.DebugCountLiveCaptions () - n);

			scrap.Clear ();
			
			System.GC.Collect ();

			Assert.AreEqual (2, this.manager.DebugCountLiveCaptions () - n);

			Assert.IsNotNull (captionA);
			Assert.IsNotNull (captionQ);

			captionA = null;
			captionQ = null;
			
			System.GC.Collect ();

			Assert.AreEqual (0, this.manager.DebugCountLiveCaptions () - n);
		}

		[Test]
		public void CheckGetCaptionUsingDruids()
		{
			manager.ActiveCulture = Resources.FindSpecificCultureInfo ("fr");

			Druid id = Druid.Parse ("[4001]");

			Caption caption;
			
			caption = this.manager.GetCaption (id, ResourceLevel.Merged);
			
			Assert.AreEqual ("[Test]", caption.Description);
			Assert.AreEqual ("Text B", Collection.Extract (caption.Labels, 0));
			Assert.AreEqual ("Texte C en fran�ais", Collection.Extract (caption.Labels, 1));

			//	Switching to English changes the caption's contents :
			
			manager.ActiveCulture = Resources.FindSpecificCultureInfo ("en");

			Assert.AreEqual ("Text A", caption.Description);
			Assert.AreEqual ("Text B", Collection.Extract (caption.Labels, 0));
			Assert.AreEqual ("Text C", Collection.Extract (caption.Labels, 1));
		}

		[Test]
		public void CheckGetForeignModuleBundle()
		{
			string t1 = "Druid - From other module";
			string t2 = "Druid - Hello, world";

			Assert.AreEqual (t2, this.manager.GetText ("file:strings#$0"));

			Assert.AreEqual (t1, this.manager.GetText ("file/OtherModule:strings#$0"));
			Assert.AreEqual (t1, this.manager.GetText ("file/31:strings#$0"));

			Assert.AreEqual (t2, this.manager.GetText ("file:strings#$0"));
			Assert.AreEqual (t2, this.manager.GetText ("file/Test:strings#$0"));
			Assert.AreEqual (t2, this.manager.GetText ("file/4:strings#$0"));

			Assert.AreEqual (t1, this.manager.GetText ("[V]"));
			Assert.AreEqual (t2, this.manager.GetText ("[4]"));
		}

		[Test]
		public void CheckBinding()
		{
			ResourceManager manager = this.manager;
			Widgets.Visual visual = new Epsitec.Common.Widgets.Visual ();
			System.Globalization.CultureInfo culture = manager.ActiveCulture;

			manager.ActiveCulture = Resources.FindCultureInfo ("fr");

			Assert.AreEqual ("Druid - Bonjour", manager.GetData ("file/Test:strings#$0", ResourceLevel.Localized, null));
			Assert.AreEqual ("Druid - Bonjour", manager.GetData ("[4]", ResourceLevel.Localized, null));

			manager.Bind (visual, Widgets.Visual.NameProperty, "[4]");

			Assert.IsTrue (visual.IsBound (Widgets.Visual.NameProperty));
			Assert.AreEqual (Types.DataSourceType.Resource, visual.GetBindingExpression (Widgets.Visual.NameProperty).DataSourceType);

			Assert.AreEqual ("Druid - Bonjour", visual.Name);

			manager.ActiveCulture = Resources.FindCultureInfo ("en");

			Assert.AreEqual ("Druid - Hello, world", visual.Name);

			manager.ActiveCulture = culture;
		}

		[Test]
		public void CheckBindingPerformance()
		{
			System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch ();
			ResourceManager manager = new ResourceManager (this.GetType ());
			Widgets.Visual visual = new Epsitec.Common.Widgets.Visual ();

			manager.DefineDefaultModuleName ("LowLevelTest");
			manager.ActiveCulture = Resources.FindCultureInfo ("en");
			manager.GetData ("file:strings#title.SettingsWindow", ResourceLevel.Localized, null);
			manager.ActiveCulture = Resources.FindCultureInfo ("fr");
			manager.GetData ("file:strings#title.SettingsWindow", ResourceLevel.Localized, null);

			System.GC.Collect ();

			long memory1 = System.GC.GetTotalMemory (true);

			stopwatch.Start ();
			stopwatch.Stop ();

			int max = 100*1000;

			List<Widgets.Visual> list = new List<Epsitec.Common.Widgets.Visual> ();

			stopwatch.Reset ();
			stopwatch.Start ();

			for (int i = 0; i < max; i++)
			{
				visual = new Epsitec.Common.Widgets.Visual ();
				list.Add (visual);
			}

			stopwatch.Stop ();
			long memory2 = System.GC.GetTotalMemory (true);
			stopwatch.Start ();

			for (int i = 0; i < max; i++)
			{
				visual = list[i];
				manager.Bind (visual, Widgets.Visual.NameProperty, "file:strings#title.SettingsWindow");
			}

			stopwatch.Stop ();

			long memory3 = System.GC.GetTotalMemory (true);

			System.Console.Out.WriteLine ("Created {0} bindings in {1} ms", max, stopwatch.ElapsedMilliseconds);
			System.Console.Out.WriteLine ("Visual:  {0} bytes/instance", (memory2-memory1) / max);
			System.Console.Out.WriteLine ("Binding: {0} bytes/instance", (memory3-memory2) / max);
			System.Console.Out.Flush ();

			stopwatch.Reset ();
			stopwatch.Start ();

			manager.ActiveCulture = Resources.FindCultureInfo ("en");

			stopwatch.Stop ();

			System.Console.Out.WriteLine ("Switch to culture '{0}' bindings in {1} ms", manager.ActiveCulture.EnglishName, stopwatch.ElapsedMilliseconds);

			stopwatch.Reset ();
			stopwatch.Start ();

			manager.ActiveCulture = Resources.FindCultureInfo ("fr");

			stopwatch.Stop ();

			System.Console.Out.WriteLine ("Switch to culture '{0}' bindings in {1} ms", manager.ActiveCulture.EnglishName, stopwatch.ElapsedMilliseconds);

			stopwatch.Reset ();
			stopwatch.Start ();

			manager.ActiveCulture = Resources.FindCultureInfo ("en");

			stopwatch.Stop ();

			System.Console.Out.WriteLine ("Switch to culture '{0}' bindings in {1} ms", manager.ActiveCulture.EnglishName, stopwatch.ElapsedMilliseconds);

			stopwatch.Reset ();
			stopwatch.Start ();

			manager.ActiveCulture = Resources.FindCultureInfo ("fr");

			stopwatch.Stop ();

			System.Console.Out.WriteLine ("Switch to culture '{0}' bindings in {1} ms", manager.ActiveCulture.EnglishName, stopwatch.ElapsedMilliseconds);

			long memory4 = System.GC.GetTotalMemory (true);

			System.Console.Out.WriteLine ("Memory delta after switches: {0}", memory4-memory3);
			System.Console.Out.Flush ();

			for (int i = 0; i < max; i++)
			{
				visual = list[i];
				visual.ClearAllBindings ();
			}

			long memory5 = System.GC.GetTotalMemory (true);

			System.Console.Out.WriteLine ("Memory delta after ClearAllBindings: {0} bytes/instance", (memory5-memory4)/max);
			System.Console.Out.Flush ();

			System.GC.Collect ();
			manager.TrimCache ();
			System.GC.Collect ();

			long memory6 = System.GC.GetTotalMemory (true);

			System.Console.Out.WriteLine ("Total memory delta after TrimCache & GC: {0} bytes/instance", (memory6-memory4)/max);
			System.Console.Out.Flush ();

			Assert.IsTrue (System.Math.Abs ((memory4-memory6)/max - (memory3-memory2)/max) < 2);
			Assert.IsTrue ((memory3-memory2)/max < 326);	// 314 before r5433
		}
		
		[Test]
		public void CheckBinding1Serialization()
		{
			string result = this.SerializeToXml ();
			
			System.Console.Out.WriteLine (result);
		}

		[Test]
		public void CheckBinding2Deserialization()
		{
			string result = this.SerializeToXml ();
			
			System.IO.StringReader stringReader = new System.IO.StringReader (result);
			System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader (stringReader);

			while (xmlReader.Read ())
			{
				if ((xmlReader.NodeType == System.Xml.XmlNodeType.Element) &&
					(xmlReader.LocalName == "root"))
				{
					break;
				}
			}

			Types.Serialization.Context context = new Types.Serialization.DeserializerContext (new Types.Serialization.IO.XmlReader (xmlReader));
			
			context.ExternalMap.Record (Types.Serialization.Context.WellKnownTagResourceManager, this.manager);

			Widgets.Widget root = Storage.Deserialize (context) as Widgets.Widget;

			Assert.IsNotNull (root);
			Assert.AreEqual ("RootWidget", root.Name);
			Assert.AreEqual ("Druid - Hello, world", root.Text);
			Assert.AreEqual (1, root.TabIndex);
			Assert.AreEqual (2, root.Children.Count);
			Assert.AreEqual (typeof (Widgets.Button), root.Children[0].GetType ());
			Assert.AreEqual (root, root.Children[0].Parent);
			Assert.AreEqual ("Druid - Good bye...", (root.Children[0] as Widgets.Button).Text);
			Assert.AreEqual (typeof (Widgets.VScroller), root.Children[1].GetType ());
			Assert.AreEqual (root, root.Children[1].Parent);
		}

		private string SerializeToXml()
		{
			string result;
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			System.IO.StringWriter stringWriter = new System.IO.StringWriter (buffer);
			System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter (stringWriter);

			xmlWriter.Indentation = 2;
			xmlWriter.IndentChar = ' ';
			xmlWriter.Formatting = System.Xml.Formatting.Indented;
			xmlWriter.WriteStartDocument (true);
			xmlWriter.WriteStartElement ("root");

			Types.Serialization.Context context = new Types.Serialization.SerializerContext (new Types.Serialization.IO.XmlWriter (xmlWriter));

			context.ExternalMap.Record (Types.Serialization.Context.WellKnownTagResourceManager, this.manager);

			Widgets.Widget root = new Widgets.Widget ();
			Widgets.Button button = new Widgets.Button ();
			Widgets.VScroller scroller = new Widgets.VScroller ();

			this.manager.Bind (root, Widgets.Widget.TextProperty, "[4]");
			this.manager.Bind (button, Widgets.Widget.TextProperty, "[4001]");

			root.Name = "RootWidget";
			root.TabIndex = 1;
			root.Children.Add (button);
			root.Children.Add (scroller);

			Storage.Serialize (root, context);

			xmlWriter.WriteEndElement ();
			xmlWriter.WriteEndDocument ();
			xmlWriter.Flush ();
			xmlWriter.Close ();

			result = buffer.ToString ();
			return result;
		}

		private ResourceManager manager;
	}
}
