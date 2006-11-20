//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		public DbKey(DbId id) : this (id, DbRowStatus.Live)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbKey"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="status">The status.</param>
		public DbKey(DbId id, DbRowStatus status)
		{
			this.id     = id;
			this.status = DbKey.ConvertToIntStatus (status);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbKey"/> class.
		/// </summary>
		/// <param name="dataRow">The data row.</param>
		public DbKey(System.Data.DataRow dataRow)
		{
			this.id = 0;
			this.status = 0;
			
			this.DefineIdAndStatus (dataRow[Tags.ColumnId], dataRow[Tags.ColumnStatus]);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbKey"/> class.
		/// </summary>
		/// <param name="dataRow">The data row.</param>
		public DbKey(object[] dataRow)
		{
			this.id = 0;
			this.status = 0;

			this.DefineIdAndStatus (dataRow[0], dataRow[1]);
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
		/// Gets the row status.
		/// </summary>
		/// <value>The status.</value>
		public DbRowStatus						Status
		{
			get
			{
				return DbKey.ConvertFromIntStatus (this.status);
			}
		}

		/// <summary>
		/// Gets the row status, represented as a number.
		/// </summary>
		/// <value>The int status.</value>
		internal short							IntStatus
		{
			get
			{
				return this.status;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance represents a temporary key.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance represents a temporary key; otherwise, <c>false</c>.
		/// </value>
		public bool								IsTemporary
		{
			get
			{
				return DbKey.CheckTemporaryId (this.id);
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
				return (this.id == 0) && (this.status == 0);
			}
		}
		
		public static readonly DbKey			Empty = new DbKey ();

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
			DbTools.WriteAttribute (xmlWriter, prefix+"stat", this.status == 0 ? null : InvariantConverter.ToString (this.status));
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
			string argStat = xmlReader.GetAttribute (prefix+"stat");

			if ((string.IsNullOrEmpty (argId)) &&
				(string.IsNullOrEmpty (argStat)))
			{
				return DbKey.Empty;
			}

			DbId id     = InvariantConverter.ParseLong (argId);
			int  status = InvariantConverter.ParseInt (argStat);

			return new DbKey (id, DbKey.ConvertFromIntStatus (status));
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
		/// Creates a temporary id.
		/// </summary>
		/// <returns></returns>
		public static DbId CreateTemporaryId()
		{
			return DbId.CreateTempId (System.Threading.Interlocked.Increment (ref DbKey.tempId));
		}

		/// <summary>
		/// Checks whether the id is a temporary id.
		/// </summary>
		/// <param name="id">The id to check.</param>
		/// <returns><c>true</c> if the id is a temporary id; otherwise, <c>false</c>.</returns>
		public static bool CheckTemporaryId(DbId id)
		{
			if ((id >= DbId.MinimumTemp) &&
				(id <= DbId.MaximumTemp))
			{
				return true;
			}
			
			return false;
		}

		/// <summary>
		/// Converts the status to an integer representation.
		/// </summary>
		/// <param name="status">The status.</param>
		/// <returns>The status represented as an integer.</returns>
		internal static short ConvertToIntStatus(DbRowStatus status)
		{
			return (short) status;
		}

		/// <summary>
		/// Converts an integer representation of the status back to a status.
		/// </summary>
		/// <param name="status">The value.</param>
		/// <returns>The status.</returns>
		internal static DbRowStatus ConvertFromIntStatus(int status)
		{
			return (DbRowStatus) status;
		}

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
		/// Gets the row status.
		/// </summary>
		/// <param name="row">The row.</param>
		/// <returns>The row status.</returns>
		public static DbRowStatus GetRowStatus(System.Data.DataRow row)
		{
			return DbKey.ConvertFromIntStatus (InvariantConverter.ToShort (row[Tags.ColumnStatus]));
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
		/// Sets the row status.
		/// </summary>
		/// <param name="row">The row.</param>
		/// <param name="status">The row status.</param>
		public static void SetRowStatus(System.Data.DataRow row, DbRowStatus status)
		{
			row[Tags.ColumnStatus] = (short) status;
		}

		/// <summary>
		/// Defines the id and status of the key based on object values.
		/// </summary>
		/// <param name="valueId">The id value (<c>long</c>).</param>
		/// <param name="valueStatus">The status value (<c>short</c>).</param>
		private void DefineIdAndStatus(object valueId, object valueStatus)
		{
			long id;

			if ((InvariantConverter.Convert (valueId, out id)) &&
				(id >= 0))
			{
				this.id     = id;
				this.status = InvariantConverter.ToShort (valueStatus);
			}
			else
			{
				throw new System.ArgumentException ("Invalid key specification");
			}
		}
		
		public const DbRawType					RawTypeForId		= DbRawType.Int64;
		public const DbRawType					RawTypeForStatus	= DbRawType.Int16;
		
		private static long						tempId;

		private DbId							id;
		private short							status;
	}
}
