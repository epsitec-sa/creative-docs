//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 22/10/2003

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
		
		public DbKey(long id) : this (id, 0, 0)
		{
		}
		
		public DbKey(long id, int revision, int raw_status)
		{
			this.id         = id;
			this.revision   = revision;
			this.raw_status = raw_status;
		}
		
		
		public long						Id
		{
			get { return this.id; }
		}
		
		public int						Revision
		{
			get { return this.revision; }
		}
		
		public int						RawStatus
		{
			get { return this.raw_status; }
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
			that.revision   = this.revision;
			that.raw_status = this.raw_status;
			
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
			
			if (this.id == key.id)
			{
				return this.revision.CompareTo (key.revision);
			}
			
			return this.id.CompareTo (key.id);
		}
		#endregion
		
		public override bool Equals(object obj)
		{
			//	Ne considère que Id et Revision pour la comparaison (et pour le
			//	calcul d'une valeur de hachage).
			
			DbKey key = obj as DbKey;
			
			if (key == null)
			{
				return false;
			}
			
			return (key.id == this.id) && (key.revision == this.revision);
		}
		
		public override int GetHashCode()
		{
			return this.id.GetHashCode () ^ (this.revision);
		}
		
		public override string ToString()
		{
			return string.Format ("[{0}.{1}]", this.id, this.revision);
		}

		
		protected void SerialiseXmlAttributes(System.Text.StringBuilder buffer)
		{
			buffer.Append (@" key.id=""");
			buffer.Append (this.id.ToString (System.Globalization.CultureInfo.InvariantCulture));
			buffer.Append (@"""");
			
			if (this.revision != 0)
			{
				buffer.Append (@" key.rev=""");
				buffer.Append (this.revision.ToString (System.Globalization.CultureInfo.InvariantCulture));
				buffer.Append (@"""");
			}
			
			if (this.raw_status != 0)
			{
				buffer.Append (@" key.stat=""");
				buffer.Append (this.raw_status.ToString (System.Globalization.CultureInfo.InvariantCulture));
				buffer.Append (@"""");
			}
		}
		
		
		public static void SerialiseToXmlAttributes(System.Text.StringBuilder buffer, DbKey key)
		{
			if (key != null)
			{
				key.SerialiseXmlAttributes (buffer);
			}
		}
		
		public static DbKey DeserialiseFromXmlAttributes(System.Xml.XmlElement xml)
		{
			//	Utilise les attributs de l'élément passé en entrée pour reconstruire
			//	une instance de DbKey. Retourne null si aucun attribut ne correspond.
			
			string arg_id   = xml.GetAttribute ("key.id");
			string arg_rev  = xml.GetAttribute ("key.rev");
			string arg_stat = xml.GetAttribute ("key.stat");
			
			if ((arg_id == "") &&
				(arg_rev == "") &&
				(arg_stat == ""))
			{
				return null;
			}
			
			long id         = 0;
			int  revision   = 0;
			int  raw_status = 0;
			
			if (arg_id.Length > 0)
			{
				id = System.Int64.Parse (arg_id, System.Globalization.CultureInfo.InvariantCulture);
			}
			
			if (arg_rev.Length > 0)
			{
				revision = System.Int32.Parse (arg_rev, System.Globalization.CultureInfo.InvariantCulture);
			}
			
			if (arg_stat.Length > 0)
			{
				raw_status = System.Int32.Parse (arg_stat, System.Globalization.CultureInfo.InvariantCulture);
			}
			
			return new DbKey (id, revision, raw_status);
		}
		
		
		protected long					id;
		protected int					revision;
		protected int					raw_status;
	}
	
	public enum DbKeyMatchMode
	{
		SimpleId,						//	ne compare que l'identificateur (ID)
		LiveId,							//	compare l'identificateur, révision=0
		ExactIdRevision					//	compare l'identificateur et la révision
	}
}
