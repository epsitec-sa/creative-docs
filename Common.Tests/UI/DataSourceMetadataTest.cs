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
using Epsitec.Common.UI;
using NUnit.Framework;

namespace Epsitec.Common.Tests.UI
{
    [TestFixture]
    public class DataSourceMetadataTest
    {
        [SetUp]
        public void Initialize()
        {
            Epsitec.Common.Widgets.Widget.Initialize();
        }

        [Test]
        public void CheckDefaultDataType()
        {
            Panel panel = new Panel();
            DataSourceMetadata metadata = panel.DataSourceMetadata;

            StructuredType type = new StructuredType();

            type.Fields.Add("A", StringType.NativeDefault);
            type.Fields.Add("B", IntegerType.Default);

            metadata.DefaultDataType = type;

            Assert.AreEqual(1, metadata.Fields.Count);
            Assert.AreEqual("*", Collection.Extract<string>(metadata.GetFieldIds(), 0));
            Assert.AreEqual(type, metadata.GetField("*").Type);

            panel.SetupSampleDataSource();

            Assert.AreEqual(null, StructuredTree.GetValue(panel.DataSource, "*.A"));
            Assert.AreEqual(UnknownValue.Value, StructuredTree.GetValue(panel.DataSource, "*.X"));
        }

        [Test]
        public void CheckPanelMetadata()
        {
            ResourceManager manager = Epsitec.Common.Support.Resources.DefaultManager;
            Panel panel = new Panel();
            panel.ResourceManager = manager;
            DataSourceMetadata metadata = panel.DataSourceMetadata;
            DataSource source = new DataSource();

            panel.DataSource = source;

            Assert.AreEqual(metadata, panel.DataSource.Metadata);
            Assert.AreEqual(metadata, panel.DataSource.GetStructuredType());

            string xml = Panel.SerializePanel(panel);

            System.Console.Out.WriteLine(xml);

            panel = Panel.DeserializePanel(xml, new DataSource(), manager);
            metadata = panel.DataSourceMetadata;
            source = panel.DataSource;

            Assert.IsNotNull(metadata);
            Assert.IsNotNull(source);
            Assert.AreEqual(panel.DataSource.GetStructuredType(), panel.DataSource.Metadata);
        }

        [Test]
        public void CheckSerialization()
        {
            string xml;
            DataSourceMetadata metadata = new DataSourceMetadata();

            metadata.Fields.Add(new StructuredTypeField("Name", StringType.NativeDefault));
            metadata.Fields.Add(new StructuredTypeField("Price", DecimalType.Default));

            xml = DataSourceMetadataTest.SerializeToString(metadata);

            DataSourceMetadata copy = DataSourceMetadataTest.DeserializeMetadataFromString(xml);

            Assert.AreEqual(metadata.Fields.Count, copy.Fields.Count);

            Assert.AreEqual(metadata.Fields[0].Id, copy.Fields[0].Id);
            Assert.AreEqual(metadata.Fields[1].Id, copy.Fields[1].Id);

            Assert.AreEqual(metadata.Fields[0].Type, copy.Fields[0].Type);
            Assert.AreEqual(metadata.Fields[1].Type, copy.Fields[1].Type);
        }

        [Test]
        public void CheckSerializationWithPanel()
        {
            string xml;
            ResourceManager manager = Epsitec.Common.Support.Resources.DefaultManager;
            Panel panel = new Panel();
            panel.ResourceManager = manager;
            panel.DataSource = new DataSource();
            DataSourceMetadata metadata = panel.DataSourceMetadata;

            metadata.Fields.Add(new StructuredTypeField("Name", StringType.NativeDefault));
            metadata.Fields.Add(new StructuredTypeField("Price", DecimalType.Default));

            xml = Panel.SerializePanel(panel);

            System.Console.Out.WriteLine(xml);

            Panel panelCopy = Panel.DeserializePanel(xml, new DataSource(), manager);
            DataSourceMetadata copy = panelCopy.DataSourceMetadata;

            Assert.AreEqual(metadata.Fields.Count, copy.Fields.Count);

            Assert.AreEqual(metadata.Fields[0].Id, copy.Fields[0].Id);
            Assert.AreEqual(metadata.Fields[1].Id, copy.Fields[1].Id);

            Assert.AreEqual(metadata.Fields[0].Type, copy.Fields[0].Type);
            Assert.AreEqual(metadata.Fields[1].Type, copy.Fields[1].Type);
        }

        [Test]
        public void CheckSerializationWithPanelAndNoDataSource()
        {
            string xml;
            ResourceManager manager = Epsitec.Common.Support.Resources.DefaultManager;
            Panel panel = new Panel();
            panel.ResourceManager = manager;
            DataSourceMetadata metadata = panel.DataSourceMetadata;

            metadata.Fields.Add(new StructuredTypeField("Name", StringType.NativeDefault));
            metadata.Fields.Add(new StructuredTypeField("Price", DecimalType.Default));

            xml = Panel.SerializePanel(panel);

            System.Console.Out.WriteLine(xml);

            Panel panelCopy = Panel.DeserializePanel(xml, null, manager);
            DataSourceMetadata copy = panelCopy.DataSourceMetadata;

            Assert.AreEqual(metadata.Fields.Count, copy.Fields.Count);

            Assert.AreEqual(metadata.Fields[0].Id, copy.Fields[0].Id);
            Assert.AreEqual(metadata.Fields[1].Id, copy.Fields[1].Id);

            Assert.AreEqual(metadata.Fields[0].Type, copy.Fields[0].Type);
            Assert.AreEqual(metadata.Fields[1].Type, copy.Fields[1].Type);
        }

        #region Support Methods

        private static string SerializeToString(DataSourceMetadata metadata)
        {
            return SimpleSerialization.SerializeToString(
                metadata,
                "metadata",
                System.Xml.Formatting.Indented
            );
        }

        private static DataSourceMetadata DeserializeMetadataFromString(string xml)
        {
            return SimpleSerialization.DeserializeFromString(xml, "metadata") as DataSourceMetadata;
        }

        #endregion
    }
}
