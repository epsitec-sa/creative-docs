//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbId encapsule un identificateur utilisé par la base de données
	/// (ID = clef d'une fiche).
	/// </summary>
	
	[System.Serializable]
	public struct DbId : System.IComparable
	{
		public DbId(long id)
		{
			this.value = id;
		}

		
		public long								LocalID
		{
			get
			{
				return this.value % DbId.LocalRange;
			}
		}

		public int								ClientID
		{
			get
			{
				return (int) ((this.value / DbId.LocalRange) % DbId.ClientRange);
			}
		}

		
		public static DbIdClass AnalyzeClass(DbId id)
		{
			long local_id  = id.LocalID;
			int  client_id = id.ClientID;
			
			if ((id.value < DbId.MinimumValid) ||
				(id.value > DbId.MaximumValid))
			{
				return DbIdClass.Invalid;
			}
			
			if (client_id == DbId.TempClientID)
			{
				return DbIdClass.Temporary;
			}
			
			return DbIdClass.Standard;
		}

		
		public static DbId CreateID(long local_id, int client_id)
		{
			System.Diagnostics.Debug.Assert (local_id >= 0);
			System.Diagnostics.Debug.Assert (local_id < DbId.LocalRange);
			System.Diagnostics.Debug.Assert (client_id >= 0);
			System.Diagnostics.Debug.Assert (client_id < DbId.ClientRange);
			
			return new DbId (local_id + DbId.LocalRange * client_id);
		}

		public static DbId CreateTempID(long local_id)
		{
			System.Diagnostics.Debug.Assert (local_id >= 0);
			System.Diagnostics.Debug.Assert (local_id < DbId.LocalRange);

			return new DbId (local_id + DbId.MinimumTemp);
		}


		public static implicit operator long(DbId id)
		{
			return id.value;
		}
		
		public static implicit operator DbId(long id)
		{
			return new DbId (id);
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
			
			if (obj is DbId)
			{
				DbId value = (DbId) obj;
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
		
		private const long						LocalRange		= 1000000000000;	//	10^12
		private const long						ClientRange		= 1000000;			//	10^6
		
		private const long						TempClientID	= DbId.ClientRange - 1;
		
		public const long						MinimumValid	= 0;
		public const long						MaximumValid	= DbId.LocalRange * DbId.ClientRange - 1;
		public const long						MinimumTemp		= DbId.LocalRange * DbId.TempClientID;
		public const long						MaximumTemp		= DbId.MinimumTemp + DbId.LocalRange - 1;
		
		private long							value;
	}
	
	/// <summary>
	/// L'énumération DbIdClass identifie les diverses classes d'identificateurs
	/// connues.
	/// </summary>
	public enum DbIdClass
	{
		Standard,								//	ID standard
		Temporary,								//	ID temporaire
		Invalid									//	ID invalide
	}
}
