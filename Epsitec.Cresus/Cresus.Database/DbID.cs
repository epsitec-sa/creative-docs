//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbID encapsule un identificateur utilisé par la base de données
	/// (ID = clef d'une fiche).
	/// </summary>
	
	[System.Serializable]
	public struct DbID : System.IComparable
	{
		public DbID(long id)
		{
			this.value = id;
		}
		
		
		public static implicit operator long(DbID id)
		{
			return id.value;
		}
		
		public static implicit operator DbID(long id)
		{
			return new DbID (id);
		}
		
		
		public override bool Equals(object obj)
		{
			return this.value.Equals (obj);
		}
		
		public override int GetHashCode()
		{
			return this.value.GetHashCode ();
		}
		
		public override string ToString()
		{
			return this.value.ToString ();
		}
		
		
		public string ToString(System.IFormatProvider provider)
		{
			return this.value.ToString (provider);
		}
		
		public string ToString(string format, System.IFormatProvider provider)
		{
			return this.value.ToString (format, provider);
		}
		
		public string ToString(string format)
		{
			return this.value.ToString (format);
		}
		
		
		#region IComparable Members
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			
			if (obj is DbID)
			{
				DbID value = (DbID) obj;
				return this.value.CompareTo (value.value);
			}
			
			if (obj is long)
			{
				long value = (long) obj;
				return this.value.CompareTo (value);
			}
			
			throw new System.ArgumentException ("Comparison with type {0} not possible.", obj.GetType ().Name);
		}
		#endregion
		
		public const long						MinimumValid	=                   0;
		public const long						MaximumValid	=  999999999999999999;
		public const long						MinimumTemp		= 1000000000000000000;
		public const long						MaximumTemp		= 1000000999999999999;
		
		private long							value;
	}
}
