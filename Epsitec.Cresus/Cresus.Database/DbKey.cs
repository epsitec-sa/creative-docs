//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 22/10/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbKey stocke une clef de la base de données. Cette
	/// clef comporte en tout cas un identificateur (ID).
	/// </summary>
	public class DbKey
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
		

		protected long					id;
		protected int					revision;
		protected int					raw_status;
	}
}
