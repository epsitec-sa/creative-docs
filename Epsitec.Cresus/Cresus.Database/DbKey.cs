//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		public virtual object Clone()
		{
			DbKey key = System.Activator.CreateInstance (this.GetType ()) as DbKey;
			
			key.id = this.id;
			key.revision = this.revision;
			key.raw_status = this.raw_status;
			
			return key;
		}
		#endregion
		
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
