//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Cache
{
	/// <summary>
	/// La classe Cache.DbTables implémente un cache pour les tables (DbTable).
	/// </summary>
	public class DbTables
	{
		public DbTables()
		{
			this.cache = new System.Collections.Hashtable ();
		}
		
		
		public DbTable		this[DbKey key]
		{
			get
			{
				return this.cache[key] as DbTable;
			}
			set
			{
				if (value == null)
				{
					this.cache.Remove (key);
				}
				else
				{
					this.cache[key] = value;
				}
			}
		}
		
		
		public void ClearCache()
		{
			this.cache.Clear ();
		}
		
		
		protected System.Collections.Hashtable	cache;
	}
}
