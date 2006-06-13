//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbKey stocke une clef de la base de données. Cette
	/// clef comporte en tout cas un identificateur (ID).
	/// </summary>
	public class DbKey : System.ICloneable, System.IComparable
	{
		public DbKey()
		{
		}
		
		public DbKey(DbId id) : this (id, DbRowStatus.Live)
		{
		}
		
		public DbKey(DbId id, DbRowStatus status)
		{
			this.id         = id;
			this.int_status = DbKey.ConvertToIntStatus (status);
		}
		
		public DbKey(System.Data.DataRow data_row)
		{
			object value_id       = data_row[Tags.ColumnId];
			object value_status   = data_row[Tags.ColumnStatus];
			
			long id;
			
			if ((Common.Types.InvariantConverter.Convert (value_id, out id)) &&
				(id >= 0))
			{
				short status;
				
				Common.Types.InvariantConverter.Convert (value_status, out status);
				
				this.id         = id;
				this.int_status = status;
			}
			else
			{
				throw new System.ArgumentException ("Row does not contain valid key.", "data_row");
			}
		}
		
		public DbKey(object[] data_row)
		{
			object value_id       = data_row[0];
			object value_status   = data_row[1];
			
			long id;
			
			if ((Common.Types.InvariantConverter.Convert (value_id, out id)) &&
				(id >= 0))
			{
				short status;
				
				Common.Types.InvariantConverter.Convert (value_status, out status);
				
				this.id         = id;
				this.int_status = status;
			}
			else
			{
				throw new System.ArgumentException ("Row does not contain valid key.", "data_row");
			}
		}
		
		
		public DbId								Id
		{
			get { return this.id; }
		}
		
		public DbRowStatus						Status
		{
			get
			{
				return DbKey.ConvertFromIntStatus (this.int_status);
			}
		}
		
		public short							IntStatus
		{
			get
			{
				return this.int_status;
			}
		}
		
		public bool								IsTemporary
		{
			get
			{
				return DbKey.CheckTemporaryId (this.id);
			}
		}
		
		
		public static DbId CreateTemporaryId()
		{
			lock (DbKey.temp_lock)
			{
				return DbId.CreateTempId (DbKey.temp_id++);
			}
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
		
		public static void SerializeToXmlAttributes(System.Text.StringBuilder buffer, DbKey key)
		{
			if (key != null)
			{
				key.SerializeXmlAttributes (buffer);
			}
		}
		
		public static DbKey DeserializeFromXmlAttributes(System.Xml.XmlElement xml)
		{
			//	Utilise les attributs de l'élément passé en entrée pour reconstruire
			//	une instance de DbKey. Retourne null si aucun attribut ne correspond.
			
			string arg_id   = xml.GetAttribute ("key.id");
			string arg_stat = xml.GetAttribute ("key.stat");
			
			if ((arg_id == "") &&
				(arg_stat == ""))
			{
				return null;
			}
			
			DbId id         = 0;
			int  int_status = 0;
			
			if (arg_id.Length > 0)
			{
				id = System.Int64.Parse (arg_id, System.Globalization.CultureInfo.InvariantCulture);
			}
			
			if (arg_stat.Length > 0)
			{
				int_status = System.Int32.Parse (arg_stat, System.Globalization.CultureInfo.InvariantCulture);
			}
			
			return new DbKey (id, DbKey.ConvertFromIntStatus (int_status));
		}
		
		
		#region ICloneable Members
		public object Clone()
		{
			return this.CloneCopyToNewObject (this.CloneNewObject ());
		}
		#endregion
		
		protected virtual object CloneNewObject()
		{
			return new DbKey ();
		}
		
		protected virtual object CloneCopyToNewObject(object o)
		{
			DbKey that = o as DbKey;
			
			that.id         = this.id;
			that.int_status = this.int_status;
			
			return that;
		}
		
		
		#region IComparable Members
		public int CompareTo(object obj)
		{
			DbKey key = obj as DbKey;
			
			if (key == null)
			{
				return 1;
			}
			
			return this.id.CompareTo (key.id);
		}
		#endregion
		
		#region Equals, GetHashCode and ToString support
		public override bool Equals(object obj)
		{
			//	Ne considère que Id et Revision pour la comparaison (et pour le
			//	calcul d'une valeur de hachage).
			
			DbKey key = obj as DbKey;
			
			if (key == null)
			{
				return false;
			}
			
			return (key.id == this.id);
		}
		
		public override int GetHashCode()
		{
			return this.id.GetHashCode ();
		}
		
		public override string ToString()
		{
			return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}", this.id);
		}
		#endregion
		
		protected void SerializeXmlAttributes(System.Text.StringBuilder buffer)
		{
			buffer.Append (@" key.id=""");
			buffer.Append (this.id.ToString (System.Globalization.CultureInfo.InvariantCulture));
			buffer.Append (@"""");
			
			if (this.int_status != 0)
			{
				buffer.Append (@" key.stat=""");
				buffer.Append (this.int_status.ToString (System.Globalization.CultureInfo.InvariantCulture));
				buffer.Append (@"""");
			}
		}
		
		
		public const DbRawType					RawTypeForId		= DbRawType.Int64;
		public const DbRawType					RawTypeForStatus	= DbRawType.Int16;
		
		private static object					temp_lock	= new object ();
		private static long						temp_id		= 0;
		
		protected DbId							id;
		protected short							int_status;
	}
}
