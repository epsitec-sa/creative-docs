//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
		/// <summary>
		/// Initializes a new instance of the <see cref="DbTypeDef"/> class.
		/// </summary>
		public DbTypeDef()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbTypeDef"/> class.
		/// </summary>
		/// <param name="namedType">The named type.</param>
		public DbTypeDef(INamedType namedType)
			: this (namedType, DbKey.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbTypeDef"/> class.
		/// </summary>
		/// <param name="namedType">The named type.</param>
		/// <param name="key">The key.</param>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="DbTypeDef"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="simpleType">The simple type.</param>
		/// <param name="numDef">The numeric defition.</param>
		/// <param name="length">The length (if this is a string).</param>
		/// <param name="isFixedLength">If set to <c>true</c>, denotes a fixed length string.</param>
		/// <param name="nullability">The nullability mode.</param>
		public DbTypeDef(string name, DbSimpleType simpleType, DbNumDef numDef, int length, bool isFixedLength, DbNullability nullability)
			: this (name, simpleType, numDef, length, isFixedLength, nullability, false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbTypeDef"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="simpleType">The simple type.</param>
		/// <param name="numDef">The numeric defition.</param>
		/// <param name="length">The length (if this is a string).</param>
		/// <param name="isFixedLength">If set to <c>true</c>, denotes a fixed length string.</param>
		/// <param name="nullability">The nullability mode.</param>
		/// <param name="isMultilingual">If set to <c>true</c>, denotes a multilingual type.</param>
		public DbTypeDef(string name, DbSimpleType simpleType, DbNumDef numDef, int length, bool isFixedLength, DbNullability nullability, bool isMultilingual)
		{
			this.name = name;
			this.simpleType = simpleType;
			this.numDef = numDef;
			this.rawType = TypeConverter.GetRawType (this.simpleType, this.numDef);
			this.length = length;
			this.isFixedLength = isFixedLength;
			this.isNullable = nullability == DbNullability.Yes;
			this.isMultilingual = isMultilingual;
		}


		/// <summary>
		/// Gets the simple type.
		/// </summary>
		/// <value>The simple type.</value>
		public DbSimpleType						SimpleType
		{
			get
			{
				return this.simpleType;
			}
		}

		/// <summary>
		/// Gets the raw type.
		/// </summary>
		/// <value>The raw type.</value>
		public DbRawType						RawType
		{
			get
			{
				return this.rawType;
			}
		}

		/// <summary>
		/// Gets the numeric definition.
		/// </summary>
		/// <value>The numeric definition or <c>null</c> if there is none.</value>
		public DbNumDef							NumDef
		{
			get
			{
				return this.numDef;
			}
		}

		/// <summary>
		/// Gets the length.
		/// </summary>
		/// <value>The length or <c>1</c> if the length does not make sense.</value>
		public int								Length
		{
			get
			{
				return System.Math.Max (1, this.length);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this type represents fixed length strings.
		/// </summary>
		/// <value>
		/// 	<c>true</c> for fixed length strings; otherwise, <c>false</c>.
		/// </value>
		public bool								IsFixedLength
		{
			get
			{
				return this.length == 0 ? true : this.isFixedLength;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this type is multilingual.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this type is multilingual; otherwise, <c>false</c>.
		/// </value>
		public bool								IsMultilingual
		{
			get
			{
				return this.isMultilingual;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this type is nullable.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this type is nullable; otherwise, <c>false</c>.
		/// </value>
		public bool								IsNullable
		{
			get
			{
				return this.isNullable;
			}
		}

		/// <summary>
		/// Gets the type id.
		/// </summary>
		/// <value>The type id.</value>
		public Druid							TypeId
		{
			get
			{
				return this.typeId;
			}
		}

		/// <summary>
		/// Gets the key for the type metadata.
		/// </summary>
		/// <value>The key for the type metadata.</value>
		public DbKey							Key
		{
			get
			{
				return this.key;
			}
		}

		#region IName Members

		/// <summary>
		/// Gets the name of the type.
		/// </summary>
		/// <value>The name of the type.</value>
		public string							Name
		{
			get
			{
				return this.name;
			}
		}

		#endregion

		internal void DefineKey(DbKey key)
		{
			this.key = key;
		}

		internal void DefineName(string name)
		{
			this.name = name;
		}
		
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

		#region IXmlSerializable Members

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
				(this.numDef.InternalRawType == DbRawType.Unknown))
			{
				this.numDef.SerializeAttributes (xmlWriter, "num.");
			}

			xmlWriter.WriteEndElement ();
		}

		#endregion

		/// <summary>
		/// Deserializes a <c>DbTypeDef</c> from the specified XML reader.
		/// </summary>
		/// <param name="xmlReader">The XML reader.</param>
		/// <returns>The deserialized <c>DbTypeDef</c>.</returns>
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

		private string							name;
		private Druid							typeId;
		private DbRawType						rawType;
		private DbSimpleType					simpleType;
		private DbNumDef						numDef;
		private int								length;
		private bool							isFixedLength;
		private bool							isMultilingual;
		private bool							isNullable;
		private DbKey							key;
	}
}
