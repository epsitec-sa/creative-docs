//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;

using System.Collections.Generic;
using Epsitec.Common.Types;

namespace Epsitec.Common.UI
{
	[TestFixture]
	public class DataSourceMetadataTest
	{
		[SetUp]
		public void Initialize()
		{
			Epsitec.Common.Widgets.Widget.Initialize ();
		}

		[Test]
		public void CheckSerialization()
		{
			string xml;
			DataSourceMetadata metadata = new DataSourceMetadata ();

			metadata.Fields.Add (new StructuredTypeField ("Name", StringType.Default));
			metadata.Fields.Add (new StructuredTypeField ("Price", DecimalType.Default));

			xml = DataSourceMetadataTest.SerializeToString (metadata);

			DataSourceMetadata copy = DataSourceMetadataTest.DeserializeMetadataFromString (xml);

			Assert.AreEqual (metadata.Fields.Count, copy.Fields.Count);

			Assert.AreEqual (metadata.Fields[0].Id, copy.Fields[0].Id);
			Assert.AreEqual (metadata.Fields[1].Id, copy.Fields[1].Id);

			Assert.AreEqual (metadata.Fields[0].Type, copy.Fields[0].Type);
			Assert.AreEqual (metadata.Fields[1].Type, copy.Fields[1].Type);
		}
		
		[Test]
		public void CheckSerializationWithPanel()
		{
			string xml;
			Panel panel = new Panel ();
			DataSourceMetadata metadata = panel.DataSourceMetadata;

			metadata.Fields.Add (new StructuredTypeField ("Name", StringType.Default));
			metadata.Fields.Add (new StructuredTypeField ("Price", DecimalType.Default));

			xml = DataSourceMetadataTest.SerializeToString (panel);

			System.Console.Out.WriteLine (xml);

			Panel panelCopy = DataSourceMetadataTest.DeserializePanelFromString (xml);
			DataSourceMetadata copy = panelCopy.DataSourceMetadata;

			Assert.AreEqual (metadata.Fields.Count, copy.Fields.Count);

			Assert.AreEqual (metadata.Fields[0].Id, copy.Fields[0].Id);
			Assert.AreEqual (metadata.Fields[1].Id, copy.Fields[1].Id);

			Assert.AreEqual (metadata.Fields[0].Type, copy.Fields[0].Type);
			Assert.AreEqual (metadata.Fields[1].Type, copy.Fields[1].Type);
		}

		#region Support Methods

		private static string SerializeToString(DataSourceMetadata metadata)
		{
			return Types.Serialization.SimpleSerialization.SerializeToString (metadata, "metadata", System.Xml.Formatting.Indented);
		}

		private static DataSourceMetadata DeserializeMetadataFromString(string xml)
		{
			return Types.Serialization.SimpleSerialization.DeserializeFromString (xml, "metadata") as DataSourceMetadata;
		}

		private static string SerializeToString(Panel panel)
		{
			return Types.Serialization.SimpleSerialization.SerializeToString (panel, "panel", System.Xml.Formatting.Indented);
		}

		private static Panel DeserializePanelFromString(string xml)
		{
			return Types.Serialization.SimpleSerialization.DeserializeFromString (xml, "panel") as Panel;
		}

		#endregion
	}
}
