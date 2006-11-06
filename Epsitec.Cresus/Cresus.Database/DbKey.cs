//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbKey</c> class represents a key which can be used by the database
	/// engine to index or look up its data. The key has at least one identifier.
	/// </summary>
	[System.Serializable]
	public struct DbKey : System.IComparable<DbKey>, System.IEquatable<DbKey>
	{
		public DbKey(DbId id) : this (id, DbRowStatus.Live)
		{
		}
		
		public DbKey(DbId id, DbRowStatus status)
		{
			this.id     = id;
			this.status = DbKey.ConvertToIntStatus (status);
		}
		
		public DbKey(System.Data.DataRow dataRow)
		{
			object valueId     = dataRow[Tags.ColumnId];
			object valueStatus = dataRow[Tags.ColumnStatus];
			
			long id;
			
			if ((InvariantConverter.Convert (valueId, out id)) &&
				(id >= 0))
			{
				short status;
				
				InvariantConverter.Convert (valueStatus, out status);
				
				this.id     = id;
				this.status = status;
			}
			else
			{
				throw new System.ArgumentException ("Row does not contain valid key", "dataRow");
			}
		}
		
		public DbKey(object[] dataRow)
		{
			object valueId     = dataRow[0];
			object valueStatus = dataRow[1];
			
			long id;
			
			if ((InvariantConverter.Convert (valueId, out id)) &&
				(id >= 0))
			{
				short status;
				
				InvariantConverter.Convert (valueStatus, out status);
				
				this.id     = id;
				this.status = status;
			}
			else
			{
				throw new System.ArgumentException ("Row does not contain valid key.", "dataRow");
			}
		}


		public DbId								Id
		{
			get
			{
				return this.id;
			}
		}
		
		public DbRowStatus						Status
		{
			get
			{
				return DbKey.ConvertFromIntStatus (this.status);
			}
		}
		
		public short							IntStatus
		{
			get
			{
				return this.status;
			}
		}
		
		public bool								IsTemporary
		{
			get
			{
				return DbKey.CheckTemporaryId (this.id);
			}
		}

		public bool								IsEmpty
		{
			get
			{
				return (this.id == 0) && (this.status == 0);
			}
		}
		
		public static readonly DbKey			Empty = new DbKey ();
		
		public void SerializeAttributes(System.Xml.XmlTextWriter xmlWriter)
		{
			this.SerializeAttributes (xmlWriter, "key.");
		}
		
		public void SerializeAttributes(System.Xml.XmlTextWriter xmlWriter, string prefix)
		{
			DbTools.WriteAttribute (xmlWriter, prefix+"id", InvariantConverter.ToString (this.id));
			DbTools.WriteAttribute (xmlWriter, prefix+"stat", this.status == 0 ? null : InvariantConverter.ToString (this.status));
		}

		public static DbKey DeserializeAttributes(System.Xml.XmlTextReader xmlReader)
		{
			return DbKey.DeserializeAttributes (xmlReader, "");
		}

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

		public static DbId CreateTemporaryId()
		{
			return DbId.CreateTempId (System.Threading.Interlocked.Increment (ref DbKey.tempId));
		}
		
		public static bool CheckTemporaryId(DbId id)
		{
			if ((id >= DbId.MinimumTemp) &&
				(id <= DbId.MaximumTemp))
			{
				return true;
			}
			
			return false;
		}
		
		public static short ConvertToIntStatus(DbRowStatus status)
		{
			return (short) status;
		}
		
		public static DbRowStatus ConvertFromIntStatus(int status)
		{
			return (DbRowStatus) status;
		}
		
		
		public const DbRawType					RawTypeForId		= DbRawType.Int64;
		public const DbRawType					RawTypeForStatus	= DbRawType.Int16;
		
		private static long						tempId;

		private DbId id;
		private short status;
	}
}
