//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.Database.Cache
{
	/// <summary>
	/// The <c>Generic&lt;T&gt;</c> class is used to implement a <c>DbKey</c> indexed
	/// item of <c>T</c> cache.
	/// </summary>
	public class Generic<T> where T : class
	{
		public Generic()
		{
			this.cache = new Dictionary<DbKey, T> ();
		}
		
		public T								this[DbKey key]
		{
			get
			{
				T item;

				if (this.cache.TryGetValue (key, out item))
				{
					return item;
				}
				else
				{
					return null;
				}
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
		
		private Dictionary<DbKey, T>			cache;
	}
}
