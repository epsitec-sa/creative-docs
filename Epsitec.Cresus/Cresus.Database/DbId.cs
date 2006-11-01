//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbId</c> structure encapsulates an identifier used by the database
	/// to access its data (this is the lowest-level encoding of an access key).
	/// Compare with <see cref="DbKey"/> which includes more information.
	/// </summary>
	
	[System.Serializable]
	public struct DbId : System.IComparable, System.IComparable<DbId>, System.IEquatable<DbId>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DbId"/> class.
		/// </summary>
		/// <param name="id">The 64-bit identifier.</param>
		public DbId(long id)
		{
			this.value = id;
		}


		/// <summary>
		/// Gets the local ID (which is a number in the 0..10^12-1 range).
		/// </summary>
		/// <value>The local ID.</value>
		public long								LocalId
		{
			get
			{
				return this.value % DbId.LocalRange;
			}
		}

		/// <summary>
		/// Gets the client ID (which is a number in the 0..10^6-1 range).
		/// </summary>
		/// <value>The client ID.</value>
		public int								ClientId
		{
			get
			{
				return (int) ((this.value / DbId.LocalRange) % DbId.ClientRange);
			}
		}

		/// <summary>
		/// Gets the <c>DbId</c> as a 64-bit numeric value.
		/// </summary>
		/// <value>The numeric value.</value>
		public long								Value
		{
			get
			{
				return this;
			}
		}


		/// <summary>
		/// Gets a value indicating whether this <c>DbId</c> refers to an
		/// object created by the server.
		/// </summary>
		/// <value><c>true</c> if this <c>DbId</c> belongs to the server; otherwise, <c>false</c>.</value>
		public bool								IsServer
		{
			get
			{
				return (this.ClientId == 1);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <c>DbId</c> is valid.
		/// </summary>
		/// <value><c>true</c> if this <c>DbId</c> is valid; otherwise, <c>false</c>.</value>
		public bool								IsValid
		{
			get
			{
				if ((this.value < DbId.MinimumValid) ||
					(this.value > DbId.MaximumValid))
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}


		/// <summary>
		/// Gets the identifier class for a given identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>The identifier class.</returns>
		public static DbIdClass GetClass(DbId id)
		{
			long localId  = id.LocalId;
			int  clientId = id.ClientId;
			
			if ((id.value < DbId.MinimumValid) ||
				(id.value > DbId.MaximumValid))
			{
				return DbIdClass.Invalid;
			}
			
			if (clientId == DbId.TempClientId)
			{
				return DbIdClass.Temporary;
			}
			
			return DbIdClass.Standard;
		}


		/// <summary>
		/// Creates an identifier based on a local ID and a client ID.
		/// </summary>
		/// <param name="localId">The local ID.</param>
		/// <param name="clientId">The client ID.</param>
		/// <returns>The identifier.</returns>
		public static DbId CreateId(long localId, int clientId)
		{
			System.Diagnostics.Debug.Assert (localId >= 0);
			System.Diagnostics.Debug.Assert (localId < DbId.LocalRange);
			System.Diagnostics.Debug.Assert (clientId >= 0);
			System.Diagnostics.Debug.Assert (clientId < DbId.ClientRange);
			
			return new DbId (localId + DbId.LocalRange * clientId);
		}

		/// <summary>
		/// Creates a temporary identifier based on a local ID.
		/// </summary>
		/// <param name="localId">The local ID.</param>
		/// <returns>The temporary identifier.</returns>
		public static DbId CreateTempId(long localId)
		{
			System.Diagnostics.Debug.Assert (localId >= 0);
			System.Diagnostics.Debug.Assert (localId < DbId.LocalRange);

			return new DbId (localId + DbId.MinimumTemp);
		}


		/// <summary>
		/// Implicitly convert from <c>DbId</c> to <c>long</c>.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>The 64-bit value.</returns>
		public static implicit operator long(DbId id)
		{
			return id.value;
		}

		/// <summary>
		/// Implicitly convert from <c>long</c> to <c>DbId</c>.
		/// </summary>
		/// <param name="id">The 64-bit value.</param>
		/// <returns>The identifier.</returns>
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

			return this.Equals (that);
		}

		public override int GetHashCode()
		{
			if (this.IsValid)
			{
				return this.value.GetHashCode ();
			}
			else
			{
				return 0;
			}
		}

		public override string ToString()
		{
			return this.value.ToString (System.Globalization.CultureInfo.InvariantCulture);
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
				return this.CompareTo (value);
			}
			
			if (obj is long)
			{
				DbId value = (DbId) (long) obj;
				return this.CompareTo (value);
			}
			
			throw new System.ArgumentException ("Comparison with object of type {0} not possible", obj.GetType ().Name);
		}
		
		#endregion

		#region IComparable<DbId> Members

		public int CompareTo(DbId other)
		{
			bool thisIsValid  = this.IsValid;
			bool otherIsValid = other.IsValid;

			if (thisIsValid && otherIsValid)
			{
				return this.value.CompareTo (other.value);
			}
			else if (thisIsValid == otherIsValid)
			{
				return 0;
			}
			else if (thisIsValid)
			{
				return -1;
			}
			else
			{
				return 1;
			}
		}

		#endregion

		#region IEquatable<DbId> Members

		public bool Equals(DbId other)
		{
			//	On s'assure que deux IDs invalides sont considérés comme égaux.

			if ((other.value < DbId.MinimumValid) ||
				(other.value > DbId.MaximumValid))
			{
				if ((this.value < DbId.MinimumValid) ||
					(this.value > DbId.MaximumValid))
				{
					return true;
				}
				else
				{
					return false;
				}
			}

			return this.value == other.value;

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
}
