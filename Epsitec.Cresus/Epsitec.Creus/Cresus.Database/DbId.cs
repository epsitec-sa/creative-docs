//	Copyright © 2004-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
			if (id < 0)
			{
				throw new System.ArgumentException ("id is lower than zero.");
			}

			this.value = id;
		}

		/// <summary>
		/// Gets the <c>DbId</c> as a 64-bit numeric value.
		/// </summary>
		/// <value>The numeric value.</value>
		public long Value
		{
			get
			{
				return this.value;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.value == 0;
			}
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
			return this.value.GetHashCode ();
		}

		public override string ToString()
		{
			return this.value.ToString (System.Globalization.CultureInfo.InvariantCulture);
		}

		public static DbId Parse(string value)
		{
			return new DbId (long.Parse (value, System.Globalization.CultureInfo.InvariantCulture));
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
			return this.value.CompareTo (other.value);
		}

		#endregion

		#region IEquatable<DbId> Members

		public bool Equals(DbId other)
		{
			return this.value == other.value;
		}

		#endregion

		private readonly long value;

		public static DbId Empty
		{
			get
			{
				return DbId.empty;
			}
		}

		private static readonly DbId empty = new DbId (0);

	}

}
