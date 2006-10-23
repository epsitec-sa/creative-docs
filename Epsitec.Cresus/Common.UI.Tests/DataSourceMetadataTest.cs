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
			
			DataSourceMetadata copy = DataSourceMetadataTest.DeserializeFromString (xml);

			Assert.AreEqual (metadata.Fields.Count, copy.Fields.Count);
			
			Assert.AreEqual (metadata.Fields[0].Id, copy.Fields[0].Id);
			Assert.AreEqual (metadata.Fields[1].Id, copy.Fields[1].Id);
			
			Assert.AreEqual (metadata.Fields[0].Type, copy.Fields[0].Type);
			Assert.AreEqual (metadata.Fields[1].Type, copy.Fields[1].Type);
		}

		#region Support Methods

		private static string SerializeToString(DataSourceMetadata metadata)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			System.IO.StringWriter stringWriter = new System.IO.StringWriter (buffer);
			System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter (stringWriter);

			using (Types.Serialization.Context context = new Types.Serialization.SerializerContext (new Types.Serialization.IO.XmlWriter (xmlWriter)))
			{
				xmlWriter.Formatting = System.Xml.Formatting.Indented;
				xmlWriter.WriteStartElement ("metadata");

				context.ActiveWriter.WriteAttributeStrings ();

				Types.Storage.Serialize (metadata, context);

				xmlWriter.WriteEndElement ();
				xmlWriter.Flush ();
				xmlWriter.Close ();

				return buffer.ToString ();
			}
		}
		
		private static DataSourceMetadata DeserializeFromString(string xml)
		{
			System.IO.StringReader stringReader = new System.IO.StringReader (xml);
			System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader (stringReader);

			xmlReader.Read ();

			System.Diagnostics.Debug.Assert (xmlReader.NodeType == System.Xml.XmlNodeType.Element);
			System.Diagnostics.Debug.Assert (xmlReader.LocalName == "metadata");

			Types.Serialization.Context context = new Types.Serialization.DeserializerContext (new Types.Serialization.IO.XmlReader (xmlReader));

			return Types.Storage.Deserialize (context) as DataSourceMetadata;
		}

		#endregion
	}
}
