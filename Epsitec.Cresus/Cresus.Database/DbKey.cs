//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbKey</c> class represents a key which can be used by the database
	/// engine to index or look up its data. The key has at least one identifier.
	/// </summary>
	[System.Serializable]
	public struct DbKey : System.IComparable<DbKey>, System.IEquatable<DbKey>, IXmlSerializable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DbKey"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		public DbKey(DbId id)
		{
			this.id = id;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbKey"/> class.
		/// </summary>
		/// <param name="dataRow">The data row.</param>
		public DbKey(System.Data.DataRow dataRow)
		{
			this.id = 0;
			
			this.DefineId (dataRow[Tags.ColumnId]);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbKey"/> class.
		/// </summary>
		/// <param name="dataRow">The data row.</param>
		public DbKey(object[] dataRow)
		{
			this.id = 0;

			this.DefineId (dataRow[0]);
		}


		/// <summary>
		/// Gets the key id.
		/// </summary>
		/// <value>The id.</value>
		public DbId								Id
		{
			get
			{
				return this.id;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance represents an empty key.
		/// </summary>
		/// <value><c>true</c> if this instance represents an empty key; otherwise, <c>false</c>.</value>
		public bool								IsEmpty
		{
			get
			{
				return this.id.IsEmpty;
			}
		}

		/// <summary>
		/// Sets the row key id and status.
		/// </summary>
		/// <param name="row">The data row.</param>
		public void SetRowKey(System.Data.DataRow row)
		{
			DbKey.SetRowId (row, this.Id);
		}

		#region IXmlSerializable Members

		/// <summary>
		/// Serializes the instance using the specified XML writer.
		/// </summary>
		/// <param name="xmlWriter">The XML writer.</param>
		public void Serialize(System.Xml.XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteStartElement ("key");
			this.SerializeAttributes (xmlWriter);
			xmlWriter.WriteEndElement ();
		}

		#endregion

		/// <summary>
		/// Serializes the key as XML attributes.
		/// </summary>
		/// <param name="xmlWriter">The XML writer.</param>
		public void SerializeAttributes(System.Xml.XmlTextWriter xmlWriter)
		{
			this.SerializeAttributes (xmlWriter, "key.");
		}

		/// <summary>
		/// Serializes the key as XML attributes, using the specified prefix for
		/// the attribute names.
		/// </summary>
		/// <param name="xmlWriter">The XML writer.</param>
		/// <param name="prefix">The prefix.</param>
		public void SerializeAttributes(System.Xml.XmlTextWriter xmlWriter, string prefix)
		{
			DbTools.WriteAttribute (xmlWriter, prefix+"id", InvariantConverter.ToString (this.id));
		}

		public static DbKey Deserialize(System.Xml.XmlTextReader xmlReader)
		{
			if ((xmlReader.NodeType == System.Xml.XmlNodeType.Element) &&
				(xmlReader.Name == "key"))
			{
				bool isEmptyElement = xmlReader.IsEmptyElement;

				DbKey key = DbKey.DeserializeAttributes (xmlReader);
				
				if (!isEmptyElement)
				{
					xmlReader.ReadEndElement ();
				}
				
				return key;
			}
			else
			{
				throw new System.Xml.XmlException (string.Format ("Unexpected element {0}", xmlReader.LocalName), null, xmlReader.LineNumber, xmlReader.LinePosition);
			}
		}

		/// <summary>
		/// Deserializes the key from XML attributes.
		/// </summary>
		/// <param name="xmlReader">The XML reader.</param>
		/// <returns></returns>
		public static DbKey DeserializeAttributes(System.Xml.XmlTextReader xmlReader)
		{
			return DbKey.DeserializeAttributes (xmlReader, "");
		}

		/// <summary>
		/// Deserializes the key from XML attributes, using the specified prefix
		/// for the attribute names.
		/// </summary>
		/// <param name="xmlReader">The XML reader.</param>
		/// <param name="prefix">The prefix.</param>
		/// <returns></returns>
		public static DbKey DeserializeAttributes(System.Xml.XmlTextReader xmlReader, string prefix)
		{
			string argId   = xmlReader.GetAttribute (prefix+"id");

			if (string.IsNullOrEmpty (argId))
			{
				return DbKey.Empty;
			}

			DbId id = InvariantConverter.ParseLong (argId);

			return new DbKey (id);
		}
		
		#region IComparable Members

		public int CompareTo(DbKey other)
		{
			return this.id.CompareTo (other.id);
		}

		#endregion

		#region IEquatable<DbKey> Members

		public bool Equals(DbKey other)
		{
			return this.id == other.id;
		}

		#endregion
		
		#region Equals, ==, !=, GetHashCode and ToString support
		
		public override bool Equals(object obj)
		{
			if (obj is DbKey)
			{
				return this.Equals ((DbKey) obj);
			}
			else
			{
				return false;
				
			}
		}
		
		public override int GetHashCode()
		{
			return this.id.GetHashCode ();
		}
		
		public override string ToString()
		{
			return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}", this.id);
		}

		public static bool operator==(DbKey a, DbKey b)
		{
			return a.Equals (b);
		}

		public static bool operator!=(DbKey a, DbKey b)
		{
			return !a.Equals (b);
		}
		
		#endregion

		/// <summary>
		/// Gets the row id.
		/// </summary>
		/// <param name="row">The row.</param>
		/// <returns>The row id.</returns>
		public static DbId GetRowId(System.Data.DataRow row)
		{
			return InvariantConverter.ToLong (row[Tags.ColumnId]);
		}

		/// <summary>
		/// Sets the row id.
		/// </summary>
		/// <param name="row">The row.</param>
		/// <param name="id">The row id.</param>
		public static void SetRowId(System.Data.DataRow row, DbId id)
		{
			row[Tags.ColumnId] = id.Value;
		}

		/// <summary>
		/// Defines the id of the key based on object values.
		/// </summary>
		/// <param name="valueId">The id value (<c>long</c>).</param>
		private void DefineId(object valueId)
		{
			long id;

			if ((InvariantConverter.Convert (valueId, out id)) &&
				(id >= 0))
			{
				this.id     = id;
			}
			else
			{
				throw new System.ArgumentException ("Invalid key specification");
			}
		}
		
		public const DbRawType					RawTypeForId		= DbRawType.Int64;

		private DbId							id;

		public static DbKey Empty
		{
			get
			{
				return DbKey.empty;
			}
		}

		private static readonly DbKey empty = new DbKey (0);

	}
}
