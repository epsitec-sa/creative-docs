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

		public long								LocalID
		{
			get
			{
				return this.value % DbID.LocalRange;
			}
		}

		public int								ClientID
		{
			get
			{
				return (int) ((this.value / DbID.LocalRange) % DbID.ClientRange);
			}
		}


		public static DbID CreateID(long local_id, int client_id)
		{
			System.Diagnostics.Debug.Assert (local_id >= 0);
			System.Diagnostics.Debug.Assert (local_id < DbID.LocalRange);
			System.Diagnostics.Debug.Assert (client_id >= 0);
			System.Diagnostics.Debug.Assert (client_id < DbID.ClientRange);
			
			return new DbID (local_id + DbID.LocalRange * client_id);
		}

		public static DbID CreateTempID(long local_id)
		{
			System.Diagnostics.Debug.Assert (local_id >= 0);
			System.Diagnostics.Debug.Assert (local_id < DbID.LocalRange);

			return new DbID (local_id + DbID.MinimumTemp);
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
		
		private const long						LocalRange		= 1000000000000;
		private const long						ClientRange		= 1000000;
		
		public const long						MinimumValid	= 0;
		public const long						MaximumValid	= DbID.LocalRange * DbID.ClientRange - 1;
		public const long						MinimumTemp		= DbID.LocalRange * DbID.ClientRange;
		public const long						MaximumTemp		= DbID.MinimumTemp + DbID.LocalRange - 1;
		
		private long							value;
	}
}
