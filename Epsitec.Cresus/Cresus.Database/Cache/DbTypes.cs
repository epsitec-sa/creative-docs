//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.Database.Cache
{
	/// <summary>
	/// La classe Cache.DbTypes implémente un cache pour les types (DbType).
	/// </summary>
	public class DbTypes
	{
		public DbTypes()
		{
			this.cache = new System.Collections.Hashtable ();
		}
		
		
		public DbType		this[DbKey key]
		{
			get
			{
				return this.cache[key] as DbType;
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
		
		
		protected System.Collections.Hashtable	cache;
	}
}
