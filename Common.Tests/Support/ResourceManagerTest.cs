/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Serialization;
using Epsitec.Common.Types.Serialization.IO;
using Epsitec.Common.Widgets;
using NUnit.Framework;
using System.Collections.Generic;

namespace Epsitec.Common.Tests.Support
{
    [TestFixture]
    public class ResourceManagerTest
    {
        [SetUp]
        public void SetUp()
        {
            this.manager = new ResourceManager(@"S:\Epsitec.Cresus\Common.Tests");
            this.manager.DefineDefaultModuleName("Test");
            this.manager.ActivePrefix = "file";
            this.manager.ActiveCulture = Epsitec.Common.Support.Resources.FindCultureInfo("en");
            this.manager.Pool.SetupDefaultRootPaths();
        }

        [Test]
        public void CheckProviderCount()
        {
            Assert.Greater(this.manager.ProviderCount, 0);
        }

        [Test]
        public void CheckNormalization()
        {
            Assert.AreEqual("file/4:strings", this.manager.NormalizeFullId("file/4:strings"));
            Assert.AreEqual("file/4:strings", this.manager.NormalizeFullId("file/:strings"));
            Assert.AreEqual("file/4:strings", this.manager.NormalizeFullId("file/Test:strings"));
            Assert.AreEqual("file/4:strings", this.manager.NormalizeFullId("file:strings"));
            Assert.AreEqual("file/4:strings", this.manager.NormalizeFullId(":strings"));
            Assert.AreEqual("file/4:strings", this.manager.NormalizeFullId("/:strings"));
            Assert.AreEqual("file/4:strings", this.manager.NormalizeFullId("/4:strings"));
            Assert.AreEqual("file/4:strings", this.manager.NormalizeFullId("/Test:strings"));
            Assert.AreEqual("file/4:strings", this.manager.NormalizeFullId("strings"));
        }

        [Test]
        [Ignore("The ressource file or the culture is probably wrong.")]
        public void CheckGetBundle()
        {
            string t1 = "Hello, world";
            string t2 = "Druid - Hello, world";
            string t3 = "Druid - Good bye...";

            Assert.AreEqual(t1, this.manager.GetText("file/Test:strings#Text1"));
            Assert.AreEqual(t1, this.manager.GetText("file/4:strings#Text1"));
            Assert.AreEqual(t1, this.manager.GetText("file:strings#Text1"));
            Assert.AreEqual(t1, this.manager.GetText("/Test:strings#Text1"));
            Assert.AreEqual(t1, this.manager.GetText("/:strings#Text1"));
            Assert.AreEqual(t1, this.manager.GetText(":strings#Text1"));
            Assert.AreEqual(t1, this.manager.GetText("strings#Text1"));

            Assert.AreEqual(t2, this.manager.GetText("file/Test:strings#$0"));
            Assert.AreEqual(t2, this.manager.GetText("file/4:strings#$0"));
            Assert.AreEqual(t2, this.manager.GetText("file:strings#$0"));
            Assert.AreEqual(t2, this.manager.GetText("/Test:strings#$0"));
            Assert.AreEqual(t2, this.manager.GetText("/:strings#$0"));
            Assert.AreEqual(t2, this.manager.GetText(":strings#$0"));
            Assert.AreEqual(t2, this.manager.GetText("strings#$0"));

            Assert.AreEqual(t3, this.manager.GetText("file/4:strings#$01"));

            Assert.AreEqual(t2, this.manager.GetText("[4]"));
            Assert.AreEqual(t3, this.manager.GetText("[4001]"));
            Assert.AreEqual(t3, this.manager.GetText(new Druid(4, 0, 1)));

            Assert.AreEqual(
                ResourceBundle.Field.Null,
                this.manager.GetBundle("file/4:Strings")[Druid.Parse("[4008]")].AsString
            );
            Assert.IsNull(this.manager.GetText(Druid.Parse("[4008]")));
            Assert.IsNull(this.manager.GetText(Druid.Parse("[7002]")));

            //	Author is only defined in '00' language; 'de' defines the author as <null/>
            //	whereas 'en' does not define anything about the author; check fallbacks :

            Assert.AreEqual(
                "Pierre Arnaud",
                this.manager.GetText(Druid.Parse("[7001]"), ResourceLevel.Default)
            );
            Assert.AreEqual(
                "Pierre Arnaud",
                this.manager.GetText(
                    Druid.Parse("[7001]"),
                    ResourceLevel.Merged,
                    Epsitec.Common.Support.Resources.FindCultureInfo("de")
                )
            );
            Assert.AreEqual(
                "Pierre Arnaud",
                this.manager.GetText(
                    Druid.Parse("[7001]"),
                    ResourceLevel.Merged,
                    Epsitec.Common.Support.Resources.FindCultureInfo("en")
                )
            );
            Assert.AreEqual(
                ResourceBundle.Field.Null,
                this.manager.GetBundle(
                    "file/7:Strings",
                    ResourceLevel.Localized,
                    Epsitec.Common.Support.Resources.FindCultureInfo("de")
                )[Druid.Parse("[7001]")].AsString
            );
            Assert.AreEqual(
                null,
                this.manager.GetBundle(
                    "file/7:Strings",
                    ResourceLevel.Localized,
                    Epsitec.Common.Support.Resources.FindCultureInfo("en")
                )[Druid.Parse("[7001]")].AsString
            );
            Assert.AreEqual(
                null,
                this.manager.GetText(
                    Druid.Parse("[7001]"),
                    ResourceLevel.Localized,
                    Epsitec.Common.Support.Resources.FindCultureInfo("de")
                )
            );
            Assert.AreEqual(
                null,
                this.manager.GetText(
                    Druid.Parse("[7001]"),
                    ResourceLevel.Localized,
                    Epsitec.Common.Support.Resources.FindCultureInfo("en")
                )
            );

            Assert.AreEqual(
                "Cf. Common.Tests",
                this.manager.GetBundle("file/7:Strings", ResourceLevel.Default)[
                    Druid.Parse("[7001]")
                ].About
            );
            Assert.AreEqual(
                "Author muss nicht übersetzt werden",
                this.manager.GetBundle(
                    "file/7:Strings",
                    ResourceLevel.Merged,
                    Epsitec.Common.Support.Resources.FindCultureInfo("de")
                )[Druid.Parse("[7001]")].About,
                "Incorrect override by 'de' over '00' of Field.About"
            );
            Assert.AreEqual(
                "Cf. Common.Tests",
                this.manager.GetBundle(
                    "file/7:Strings",
                    ResourceLevel.Merged,
                    Epsitec.Common.Support.Resources.FindCultureInfo("en")
                )[Druid.Parse("[7001]")].About,
                "Incorrect fallback from 'en' to '00' of Field.About"
            );

            ResourceBundle bundle = this.manager.GetBundle(new Druid(4, 0, 0));

            Assert.IsNotNull(bundle);
            Assert.AreEqual("DruidExperimentalBundle", bundle.Caption);
            Assert.AreEqual(1, bundle.FieldCount);
            Assert.AreEqual(new Druid(4, 0, 0), bundle.Id);

            Assert.IsTrue(this.manager.GetBundle("Strings").Id.IsEmpty);
            Assert.AreEqual("Strings", this.manager.GetBundle("file:Strings").Name);
            Assert.AreEqual("file/4:Strings", this.manager.GetBundle("file:Strings").PrefixedName);
        }

        [Test]
        public void CheckGetBundleEx1()
        {
            Assert.Throws<ResourceException>(() => this.manager.GetBundle("file:Strings#Text1"));
        }

        [Test]
        public void CheckGetBundleEx2()
        {
            Assert.Throws<ResourceException>(() => this.manager.GetBundle("[4]"));
        }

        [Test]
        public void CheckGetBundleField()
        {
            string t2 = "Druid - Hello, world";
            string t3 = "Druid - Good bye...";

            Assert.AreEqual(
                t2,
                this.manager.GetStringsBundleField(
                    Druid.Parse("[4]"),
                    ResourceLevel.Default
                ).AsString
            );
            Assert.AreEqual(
                t3,
                this.manager.GetStringsBundleField(
                    Druid.Parse("[4001]"),
                    ResourceLevel.Default
                ).AsString
            );
        }

        [Test]
        public void CheckGetCaption()
        {
            Druid idA = Druid.Parse("[4002]");
            Druid idQ = Druid.Parse("[4003]");

            for ( int i = 0; i < 4; i++ )
            {
                manager.ActiveCulture = Epsitec.Common.Support.Resources.FindSpecificCultureInfo("en");

                Caption captionA = this.manager.GetCaption(idA, ResourceLevel.Merged);
                Caption captionQ = this.manager.GetCaption(idQ, ResourceLevel.Merged);

                int captionsBefore = this.manager.DebugCountLiveCaptions();

                Assert.AreEqual("Pattern angle expressed in degrees.", captionA.Description);
                Assert.AreEqual("Quality coefficient.", captionQ.Description);
                Assert.AreEqual("A", Collection.Extract(captionA.SortedLabels, 0));
                Assert.AreEqual("Pattern angle", Collection.Extract(captionA.SortedLabels, 2));
                Assert.AreEqual("Q", Collection.Extract(captionQ.SortedLabels, 0));

                manager.ActiveCulture = Epsitec.Common.Support.Resources.FindSpecificCultureInfo("fr");

                captionA = this.manager.GetCaption(idA, ResourceLevel.Merged);
                captionQ = this.manager.GetCaption(idQ, ResourceLevel.Merged);

                Assert.AreEqual(
                    "Angle de rotation de la trame, exprimé en degrés.",
                    captionA.Description
                );
                Assert.AreEqual("Coefficient de Qualité.", captionQ.Description);
                Assert.AreEqual("A", Collection.Extract(captionA.SortedLabels, 0));
                Assert.AreEqual("Angle de la trame", Collection.Extract(captionA.SortedLabels, 2));
                Assert.AreEqual("Q", Collection.Extract(captionQ.SortedLabels, 0));

                int captionsCount = this.manager.DebugCountLiveCaptions();

                Assert.AreEqual(captionsCount, 4);
            }
        }

        [Test]
        public void CheckGetCaptionUsingDruids()
        {
            manager.ActiveCulture = Epsitec.Common.Support.Resources.FindSpecificCultureInfo("fr");

            Druid id = Druid.Parse("[4001]");

            Caption caption;

            caption = this.manager.GetCaption(id, ResourceLevel.Merged);

            Assert.AreEqual("[Test]", caption.Description);
            Assert.AreEqual("Text B", Collection.Extract(caption.Labels, 0));
            Assert.AreEqual("Texte C en français", Collection.Extract(caption.Labels, 1));

            //	Switching to English changes the caption's contents :

            manager.ActiveCulture = Epsitec.Common.Support.Resources.FindSpecificCultureInfo("en");

            Assert.AreEqual("Text A", caption.Description);
            Assert.AreEqual("Text B", Collection.Extract(caption.Labels, 0));
            Assert.AreEqual("Text C", Collection.Extract(caption.Labels, 1));
        }

        [Test]
        public void CheckGetForeignModuleBundle()
        {
            string t1 = "Druid - From other module";
            string t2 = "Druid - Hello, world";

            Assert.AreEqual(t2, this.manager.GetText("file:strings#$0"));

            Assert.AreEqual(t1, this.manager.GetText("file/OtherModule:strings#$0"));
            Assert.AreEqual(t1, this.manager.GetText("file/31:strings#$0"));

            Assert.AreEqual(t2, this.manager.GetText("file:strings#$0"));
            Assert.AreEqual(t2, this.manager.GetText("file/Test:strings#$0"));
            Assert.AreEqual(t2, this.manager.GetText("file/4:strings#$0"));

            Assert.AreEqual(t1, this.manager.GetText("[V]"));
            Assert.AreEqual(t2, this.manager.GetText("[4]"));
        }

        [Test]
        public void CheckBinding()
        {
            ResourceManager manager = this.manager;
            Visual visual = new Visual();
            System.Globalization.CultureInfo culture = manager.ActiveCulture;

            manager.ActiveCulture = Epsitec.Common.Support.Resources.FindCultureInfo("fr");

            Assert.AreEqual(
                "Druid - Bonjour",
                manager.GetData("file/Test:strings#$0", ResourceLevel.Localized, null)
            );
            Assert.AreEqual(
                "Druid - Bonjour",
                manager.GetData("[4]", ResourceLevel.Localized, null)
            );

            manager.Bind(visual, Visual.NameProperty, "[4]");

            Assert.IsTrue(visual.IsBound(Visual.NameProperty));
            Assert.AreEqual(
                DataSourceType.Resource,
                visual.GetBindingExpression(Visual.NameProperty).DataSourceType
            );

            Assert.AreEqual("Druid - Bonjour", visual.Name);

            manager.ActiveCulture = Epsitec.Common.Support.Resources.FindCultureInfo("en");

            Assert.AreEqual("Druid - Hello, world", visual.Name);

            manager.ActiveCulture = culture;
        }

        [Test]
        public void CheckBindingPerformance()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            ResourceManager manager = new ResourceManager(this.GetType());
            Visual visual = new Visual();

            manager.DefineDefaultModuleName("LowLevelTest");
            manager.ActiveCulture = Epsitec.Common.Support.Resources.FindCultureInfo("en");
            manager.GetData("file:strings#title.SettingsWindow", ResourceLevel.Localized, null);
            manager.ActiveCulture = Epsitec.Common.Support.Resources.FindCultureInfo("fr");
            manager.GetData("file:strings#title.SettingsWindow", ResourceLevel.Localized, null);

            System.GC.Collect();

            long memory1 = System.GC.GetTotalMemory(true);

            stopwatch.Start();
            stopwatch.Stop();

            int max = 100 * 1000;

            List<Visual> list = new List<Visual>();

            stopwatch.Reset();
            stopwatch.Start();

            for (int i = 0; i < max; i++)
            {
                visual = new Visual();
                list.Add(visual);
            }

            stopwatch.Stop();
            long memory2 = System.GC.GetTotalMemory(true);
            stopwatch.Start();

            for (int i = 0; i < max; i++)
            {
                visual = list[i];
                manager.Bind(visual, Visual.NameProperty, "file:strings#title.SettingsWindow");
            }

            stopwatch.Stop();

            long memory3 = System.GC.GetTotalMemory(true);

            System.Console.Out.WriteLine(
                "Created {0} bindings in {1} ms",
                max,
                stopwatch.ElapsedMilliseconds
            );
            System.Console.Out.WriteLine("Visual:  {0} bytes/instance", (memory2 - memory1) / max);
            System.Console.Out.WriteLine("Binding: {0} bytes/instance", (memory3 - memory2) / max);
            System.Console.Out.Flush();

            stopwatch.Reset();
            stopwatch.Start();

            manager.ActiveCulture = Epsitec.Common.Support.Resources.FindCultureInfo("en");

            stopwatch.Stop();

            System.Console.Out.WriteLine(
                "Switch to culture '{0}' bindings in {1} ms",
                manager.ActiveCulture.EnglishName,
                stopwatch.ElapsedMilliseconds
            );

            stopwatch.Reset();
            stopwatch.Start();

            manager.ActiveCulture = Epsitec.Common.Support.Resources.FindCultureInfo("fr");

            stopwatch.Stop();

            System.Console.Out.WriteLine(
                "Switch to culture '{0}' bindings in {1} ms",
                manager.ActiveCulture.EnglishName,
                stopwatch.ElapsedMilliseconds
            );

            stopwatch.Reset();
            stopwatch.Start();

            manager.ActiveCulture = Epsitec.Common.Support.Resources.FindCultureInfo("en");

            stopwatch.Stop();

            System.Console.Out.WriteLine(
                "Switch to culture '{0}' bindings in {1} ms",
                manager.ActiveCulture.EnglishName,
                stopwatch.ElapsedMilliseconds
            );

            stopwatch.Reset();
            stopwatch.Start();

            manager.ActiveCulture = Epsitec.Common.Support.Resources.FindCultureInfo("fr");

            stopwatch.Stop();

            System.Console.Out.WriteLine(
                "Switch to culture '{0}' bindings in {1} ms",
                manager.ActiveCulture.EnglishName,
                stopwatch.ElapsedMilliseconds
            );

            long memory4 = System.GC.GetTotalMemory(true);

            System.Console.Out.WriteLine("Memory delta after switches: {0}", memory4 - memory3);
            System.Console.Out.Flush();

            for (int i = 0; i < max; i++)
            {
                visual = list[i];
                visual.ClearAllBindings();
            }

            long memory5 = System.GC.GetTotalMemory(true);

            System.Console.Out.WriteLine(
                "Memory delta after ClearAllBindings: {0} bytes/instance",
                (memory5 - memory4) / max
            );
            System.Console.Out.Flush();

            System.GC.Collect();
            manager.TrimCache();
            System.GC.Collect();

            long memory6 = System.GC.GetTotalMemory(true);

            System.Console.Out.WriteLine(
                "Total memory delta after TrimCache & GC: {0} bytes/instance",
                (memory6 - memory4) / max
            );
            System.Console.Out.Flush();

            long memFreedAfterClearing = memory4 - memory6;
            long memTakenForBindings = memory3 - memory2;
            Assert.IsTrue(memFreedAfterClearing > memTakenForBindings);
            Assert.IsTrue(memTakenForBindings / max < 340); // 314 before r5433 ... then 326, now 329 !
        }

        [Test]
        public void CheckBinding1Serialization()
        {
            string result = this.SerializeToXml();

            System.Console.Out.WriteLine(result);
        }

        [Test]
        public void CheckBinding2Deserialization()
        {
            string result = this.SerializeToXml();

            System.IO.StringReader stringReader = new System.IO.StringReader(result);
            System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader(stringReader);

            while (xmlReader.Read())
            {
                if (
                    (xmlReader.NodeType == System.Xml.XmlNodeType.Element)
                    && (xmlReader.LocalName == "root")
                )
                {
                    break;
                }
            }

            Context context = new DeserializerContext(new XmlReader(xmlReader));

            context.ExternalMap.Record(Context.WellKnownTagResourceManager, this.manager);

            Widget root = Storage.Deserialize(context) as Widget;

            Assert.IsNotNull(root);
            Assert.AreEqual("RootWidget", root.Name);
            Assert.AreEqual("Druid - Hello, world", root.Text);
            Assert.AreEqual(1, root.TabIndex);
            Assert.AreEqual(2, root.Children.Count);
            Assert.AreEqual(typeof(Button), root.Children[0].GetType());
            Assert.AreEqual(root, root.Children[0].Parent);
            Assert.AreEqual("Druid - Good bye...", (root.Children[0] as Button).Text);
            Assert.AreEqual(typeof(VScroller), root.Children[1].GetType());
            Assert.AreEqual(root, root.Children[1].Parent);
        }

        private string SerializeToXml()
        {
            string result;
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();
            System.IO.StringWriter stringWriter = new System.IO.StringWriter(buffer);
            System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter(stringWriter);

            xmlWriter.Indentation = 2;
            xmlWriter.IndentChar = ' ';
            xmlWriter.Formatting = System.Xml.Formatting.Indented;
            xmlWriter.WriteStartDocument(true);
            xmlWriter.WriteStartElement("root");

            Context context = new SerializerContext(new XmlWriter(xmlWriter));

            context.ExternalMap.Record(Context.WellKnownTagResourceManager, this.manager);

            Widget root = new Widget();
            Button button = new Button();
            VScroller scroller = new VScroller();

            this.manager.Bind(root, Widget.TextProperty, "[4]");
            this.manager.Bind(button, Widget.TextProperty, "[4001]");

            root.Name = "RootWidget";
            root.TabIndex = 1;
            root.Children.Add(button);
            root.Children.Add(scroller);

            Storage.Serialize(root, context);

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
            xmlWriter.Close();

            result = buffer.ToString();
            return result;
        }

        private ResourceManager manager;
    }
}
