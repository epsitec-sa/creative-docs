//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbTypeDef</c> class represents a type definition, as stored in
	/// the database.
	/// </summary>
	public sealed class DbTypeDef : IName, System.IEquatable<DbTypeDef>, IXmlSerializable
	{
		public DbTypeDef()
		{
		}

		public DbTypeDef(INamedType namedType)
			: this (namedType, DbKey.Empty)
		{
		}

		public DbTypeDef(INamedType namedType, DbKey key)
		{
			this.key        = key;
			this.name       = namedType.Name;
			this.typeId     = namedType.CaptionId;
			this.rawType    = TypeConverter.GetRawType (namedType);
			this.simpleType = TypeConverter.GetSimpleType (namedType, out this.numDef);

			IStringType stringType = namedType as IStringType;

			if (stringType != null)
			{
				this.length         = stringType.MaximumLength;
				this.isFixedLength  = stringType.UseFixedLengthStorage;
				this.isMultilingual = stringType.UseMultilingualStorage;
			}
		}
		
		
		public DbSimpleType SimpleType
		{
			get
			{
				return this.simpleType;
			}
		}

		public DbRawType RawType
		{
			get
			{
				return this.rawType;
			}
		}

		public DbNumDef NumDef
		{
			get
			{
				return this.numDef;
			}
		}

		public int Length
		{
			get
			{
				return this.length;
			}
		}

		public bool IsFixedLength
		{
			get
			{
				return this.isFixedLength;
			}
		}

		public bool IsMultilingual
		{
			get
			{
				return this.isMultilingual;
			}
		}

		public Druid TypeId
		{
			get
			{
				return this.typeId;
			}
		}

		public DbKey Key
		{
			get
			{
				return this.key;
			}
		}

		#region IName Members

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		#endregion

		#region IEquatable<DbTypeDef> Members

		public bool Equals(DbTypeDef other)
		{
			return this == other;
		}

		#endregion

		public override bool Equals(object obj)
		{
			return this.Equals (obj as DbTypeDef);
		}

		public override int GetHashCode()
		{
			return this.name.GetHashCode () ^ this.typeId.GetHashCode ();
		}

		public override string ToString()
		{
			return string.Format ("{0}:{1}", this.name, this.typeId);
		}

		public static bool operator==(DbTypeDef a, DbTypeDef b)
		{
			if (object.ReferenceEquals (a, b))
			{
				return true;
			}
			if (object.ReferenceEquals (a, null))
			{
				return false;
			}
			if (object.ReferenceEquals (b, null))
			{
				return false;
			}

			return (a.name == b.name)
				&& (a.typeId == b.typeId)
				&& (a.rawType == b.rawType)
				&& (a.simpleType == b.simpleType)
				&& (a.numDef == b.numDef)
				&& (a.length == b.length)
				&& (a.isFixedLength == b.isFixedLength)
				&& (a.isMultilingual == b.isMultilingual)
				&& (a.key == b.key);
			
		}

		public static bool operator!=(DbTypeDef a, DbTypeDef b)
		{
			return !(a == b);
		}

		public void Serialize(System.Xml.XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteStartElement ("type");

			DbTools.WriteAttribute (xmlWriter, "name", DbTools.StringToString (this.Name));
			DbTools.WriteAttribute (xmlWriter, "type", DbTools.DruidToString (this.typeId));
			DbTools.WriteAttribute (xmlWriter, "raw", DbTools.RawTypeToString (this.rawType));
			DbTools.WriteAttribute (xmlWriter, "simple", DbTools.SimpleTypeToString (this.simpleType));
			DbTools.WriteAttribute (xmlWriter, "length", DbTools.IntToString (this.length));
			DbTools.WriteAttribute (xmlWriter, "fixed", DbTools.BoolToString (this.isFixedLength));
			DbTools.WriteAttribute (xmlWriter, "multi", DbTools.BoolToString (this.isMultilingual));

			if (this.numDef != null)
			{
				this.numDef.SerializeAttributes (xmlWriter, "num.");
			}

			if (!this.key.IsEmpty)
			{
				this.key.SerializeAttributes (xmlWriter, "key.");
			}
			
			xmlWriter.WriteEndElement ();
		}

		public static DbTypeDef Deserialize(System.Xml.XmlTextReader xmlReader)
		{
			if ((xmlReader.NodeType == System.Xml.XmlNodeType.Element) &&
				(xmlReader.Name == "type"))
			{
				DbTypeDef type = new DbTypeDef ();

				type.name           = DbTools.ParseString (xmlReader.GetAttribute ("name"));
				type.typeId         = DbTools.ParseDruid (xmlReader.GetAttribute ("type"));
				type.rawType        = DbTools.ParseRawType (xmlReader.GetAttribute ("raw"));
				type.simpleType     = DbTools.ParseSimpleType (xmlReader.GetAttribute ("simple"));
				type.length         = DbTools.ParseInt (xmlReader.GetAttribute ("length"));
				type.isFixedLength  = DbTools.ParseBool (xmlReader.GetAttribute ("fixed"));
				type.isMultilingual = DbTools.ParseBool (xmlReader.GetAttribute ("multi"));

				type.numDef = DbNumDef.DeserializeAttributes (xmlReader, "num.");
				type.key    = DbKey.DeserializeAttributes (xmlReader, "key.");

				xmlReader.ReadEndElement ();

				return type;
			}
			else
			{
				throw new System.Xml.XmlException (string.Format ("Unexpected element {0}", xmlReader.LocalName), null, xmlReader.LineNumber, xmlReader.LinePosition);
			}
		}


		private string name;
		private Druid typeId;
		private DbRawType rawType;
		private DbSimpleType simpleType;
		private DbNumDef numDef;
		private int length;
		private bool isFixedLength;
		private bool isMultilingual;
		private DbKey key;

		internal void DefineInternalKey(DbKey key)
		{
			this.key = key;
		}
	}
}
