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

		
		public long								LocalId
		{
			get
			{
				return this.value % DbId.LocalRange;
			}
		}

		public int								ClientId
		{
			get
			{
				return (int) ((this.value / DbId.LocalRange) % DbId.ClientRange);
			}
		}
		
		public long								Value
		{
			get
			{
				return this;
			}
		}

		
		public bool								IsServer
		{
			get
			{
				return (this.ClientId == 1);
			}
		}
		
		public bool								IsValid
		{
			get
			{
				if ((this.value < DbId.MinimumValid) ||
					(this.value > DbId.MaximumValid))
				{
					return false;
				}
				
				return true;
			}
		}
		
		
		public static DbIdClass AnalyzeClass(DbId id)
		{
			long local_id  = id.LocalId;
			int  client_id = id.ClientId;
			
			if ((id.value < DbId.MinimumValid) ||
				(id.value > DbId.MaximumValid))
			{
				return DbIdClass.Invalid;
			}
			
			if (client_id == DbId.TempClientId)
			{
				return DbIdClass.Temporary;
			}
			
			return DbIdClass.Standard;
		}

		
		public static DbId CreateId(long local_id, int client_id)
		{
			System.Diagnostics.Debug.Assert (local_id >= 0);
			System.Diagnostics.Debug.Assert (local_id < DbId.LocalRange);
			System.Diagnostics.Debug.Assert (client_id >= 0);
			System.Diagnostics.Debug.Assert (client_id < DbId.ClientRange);
			
			return new DbId (local_id + DbId.LocalRange * client_id);
		}

		public static DbId CreateTempId(long local_id)
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
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType () != typeof (DbId))
			{
				return false;
			}
			
			DbId that = (DbId) obj;
			
			//	On s'assure que deux IDs invalides sont considérés comme égaux.
			
			if ((that.value < DbId.MinimumValid) ||
				(that.Value > DbId.MaximumValid))
			{
				if ((this.value < DbId.MinimumValid) ||
					(this.Value > DbId.MaximumValid))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			
			return this.value == that.value;
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
		
		public static readonly DbId				Invalid = new DbId (-1);
		public static readonly DbId				Zero    = new DbId (0);
		
		public const long						LocalRange		= 1000000000000;	//	10^12
		public const int						ClientRange		= 1000000;			//	10^6
		
		public const int						TempClientId	= DbId.ClientRange - 1;
		
		public const long						MinimumValid	= 0;
		public const long						MaximumValid	= DbId.LocalRange * DbId.ClientRange - 1;
		public const long						MinimumTemp		= DbId.LocalRange * DbId.TempClientId;
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
