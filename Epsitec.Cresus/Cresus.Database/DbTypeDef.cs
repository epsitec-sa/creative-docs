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
			else
			{
				this.length         = 0;
				this.isFixedLength  = false;
				this.isMultilingual = false;
			}

			INullableType nullableType = namedType as INullableType;

			if (nullableType != null)
			{
				this.isNullable = nullableType.IsNullable;
			}
		}

		public DbTypeDef(string name, DbSimpleType simpleType, DbNumDef numDef, int length, bool isFixedLength, Nullable isNullable)
		{
			this.name = name;
			this.simpleType = simpleType;
			this.numDef = numDef;
			this.rawType = TypeConverter.GetRawType (this.simpleType, this.numDef);
			this.length = length;
			this.isFixedLength = isFixedLength;
			this.isNullable = isNullable == Nullable.Yes;
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
				return System.Math.Max (1, this.length);
			}
		}

		public bool IsFixedLength
		{
			get
			{
				return this.length == 0 ? true : this.isFixedLength;
			}
		}

		public bool IsMultilingual
		{
			get
			{
				return this.isMultilingual;
			}
		}

		public bool IsNullable
		{
			get
			{
				return this.isNullable;
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

			DbTools.WriteAttribute (xmlWriter, "type", DbTools.DruidToString (this.typeId));
			DbTools.WriteAttribute (xmlWriter, "raw", DbTools.RawTypeToString (this.rawType));
			DbTools.WriteAttribute (xmlWriter, "simple", DbTools.SimpleTypeToString (this.simpleType));
			DbTools.WriteAttribute (xmlWriter, "length", DbTools.IntToString (this.length));
			DbTools.WriteAttribute (xmlWriter, "fixed", DbTools.BoolDefaultingToTrueToString (this.isFixedLength));
			DbTools.WriteAttribute (xmlWriter, "multi", DbTools.BoolDefaultingToFalseToString (this.isMultilingual));
			DbTools.WriteAttribute (xmlWriter, "null", DbTools.BoolDefaultingToFalseToString (this.isNullable));

			if ((this.numDef != null) &&
				(this.numDef.InternalRawType == DbRawType.Unsupported))
			{
				this.numDef.SerializeAttributes (xmlWriter, "num.");
			}

			xmlWriter.WriteEndElement ();
		}

		public static DbTypeDef Deserialize(System.Xml.XmlTextReader xmlReader)
		{
			if ((xmlReader.NodeType == System.Xml.XmlNodeType.Element) &&
				(xmlReader.Name == "type"))
			{
				bool isEmptyElement = xmlReader.IsEmptyElement;

				DbTypeDef type = new DbTypeDef ();

				type.typeId         = DbTools.ParseDruid (xmlReader.GetAttribute ("type"));
				type.rawType        = DbTools.ParseRawType (xmlReader.GetAttribute ("raw"));
				type.simpleType     = DbTools.ParseSimpleType (xmlReader.GetAttribute ("simple"));
				type.length         = DbTools.ParseInt (xmlReader.GetAttribute ("length"));
				type.isFixedLength  = DbTools.ParseDefaultingToTrueBool (xmlReader.GetAttribute ("fixed"));
				type.isMultilingual = DbTools.ParseDefaultingToFalseBool (xmlReader.GetAttribute ("multi"));
				type.isNullable     = DbTools.ParseDefaultingToFalseBool (xmlReader.GetAttribute ("null"));

				type.numDef = DbNumDef.DeserializeAttributes (xmlReader, "num.");

				if (type.numDef == null)
				{
					if (type.simpleType == DbSimpleType.Decimal)
					{
						type.numDef = DbNumDef.FromRawType (type.rawType);
					}
				}
				else
				{
					System.Diagnostics.Debug.Assert (type.simpleType == DbSimpleType.Decimal);
					System.Diagnostics.Debug.Assert (type.rawType == TypeConverter.GetRawType (type.simpleType, type.numDef));
				}

				if (!isEmptyElement)
				{
					xmlReader.ReadEndElement ();
				}
				
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
		private bool isNullable;
		private DbKey key;

		internal void DefineKey(DbKey key)
		{
			this.key = key;
		}

		internal void DefineName(string name)
		{
			this.name = name;
		}
	}
}
