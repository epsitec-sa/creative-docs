using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using System.Xml;


namespace Epsitec.Cresus.DataLayer.ImportExport
{
	

	// TODO Comment this class.
	// Marc


	internal sealed class ColumnDefinition
	{


		public ColumnDefinition(string name, DbRawType dbRawType, System.Type adoType, bool isIdColumn)
		{
			this.Name = name;
			this.DbRawType = dbRawType;
			this.AdoType = adoType;
			this.IsIdColumn = isIdColumn;
		}


		public string Name
		{
			get;
			private set;
		}


		public DbRawType DbRawType
		{
			get;
			private set;
		}


		public System.Type AdoType
		{
			get;
			private set;
		}


		public bool IsIdColumn
		{
			get;
			private set;
		}


		public void WriteXmlDefinition(XmlWriter xmlWriter, int index)
		{
			this.WriteXmlStart (xmlWriter, index);
			this.WriteXmlName (xmlWriter);
			this.WriteXmlDbRawType (xmlWriter);
			this.WriteXmlAdoType (xmlWriter);
			this.WriteXmlIsIdColumn (xmlWriter);
			this.WriteXmlEnd (xmlWriter);
		}


		private void WriteXmlStart(XmlWriter xmlWriter, int index)
		{
			xmlWriter.WriteStartElement ("column");
			xmlWriter.WriteAttributeString ("id", InvariantConverter.ConvertToString (index));
		}


		private void WriteXmlName(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement ("name");
			xmlWriter.WriteValue (this.Name);
			xmlWriter.WriteEndElement ();
		}


		private void WriteXmlDbRawType(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement ("dbRawType");
			xmlWriter.WriteValue (System.Enum.GetName (typeof (DbRawType), this.DbRawType));
			xmlWriter.WriteEndElement ();
		}


		private void WriteXmlAdoType(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement ("adoType");
			xmlWriter.WriteValue (this.AdoType.AssemblyQualifiedName);
			xmlWriter.WriteEndElement ();
		}


		private void WriteXmlIsIdColumn(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement ("isIdColumn");
			xmlWriter.WriteValue (this.IsIdColumn);
			xmlWriter.WriteEndElement ();
		}


		private void WriteXmlEnd(XmlWriter xmlWriter)
		{
			xmlWriter.WriteEndElement ();
		}


		public static ColumnDefinition ReadXmlDefinition(XmlReader xmlReader, int index)
		{
			ColumnDefinition.ReadXmlStart (xmlReader, index);

			string name = ColumnDefinition.ReadXmlName (xmlReader);
			DbRawType dbRawType = ColumnDefinition.ReadXmlDbRawType (xmlReader);
			System.Type adoType = ColumnDefinition.ReadXmlAdoType(xmlReader);
			bool isIdColumn = ColumnDefinition.ReadXmlIsIdColumn (xmlReader);

			ColumnDefinition.ReadXmlEnd (xmlReader);

			return new ColumnDefinition (name, dbRawType, adoType, isIdColumn);
		}


		private static void ReadXmlStart(XmlReader xmlReader, int index)
		{
			if (!string.Equals ("column", xmlReader.Name))
			{
				throw new System.FormatException ("Unexpected tag: " + xmlReader.Name + " found but column expected.");
			}
			
			string idAsString = xmlReader.GetAttribute ("id");
			int idAsInt = InvariantConverter.ConvertFromString<int> (idAsString);

			xmlReader.ReadStartElement ("column");

			if (index != idAsInt)
			{
				throw new System.FormatException ("Unexpected index for column: " + idAsInt + " found but " + index + " expected.");
			}
		}


		private static string ReadXmlName(XmlReader xmlReader)
		{
			xmlReader.ReadStartElement ("name");

			string name = xmlReader.ReadContentAsString ();

			xmlReader.ReadEndElement ();

			return name;
		}


		private static DbRawType ReadXmlDbRawType(XmlReader xmlReader)
		{
			xmlReader.ReadStartElement ("dbRawType");

			string dbRawTypeAsString = xmlReader.ReadContentAsString ();

			xmlReader.ReadEndElement ();

			return (DbRawType) System.Enum.Parse (typeof (DbRawType), dbRawTypeAsString);
		}


		private static System.Type ReadXmlAdoType(XmlReader xmlReader)
		{
			xmlReader.ReadStartElement ("adoType");

			string adoTypeAsString = xmlReader.ReadContentAsString ();

			xmlReader.ReadEndElement ();

			return TypeRosetta.GetSystemType (adoTypeAsString);
		}


		private static bool ReadXmlIsIdColumn(XmlReader xmlReader)
		{
			xmlReader.ReadStartElement ("isIdColumn");

			bool isIdColumn = xmlReader.ReadContentAsBoolean ();

			xmlReader.ReadEndElement ();

			return isIdColumn;
		}


		private static void ReadXmlEnd(XmlReader xmlReader)
		{
			xmlReader.ReadEndElement ();
		}


	}


}
