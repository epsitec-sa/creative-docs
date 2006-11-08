//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbLogEntry</c> class represents a log entry (see <see cref="DbLogger"/>).
	/// </summary>
	[System.Serializable]
	public struct DbLogEntry : System.IEquatable<DbLogEntry>, System.IComparable<DbLogEntry>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DbLogEntry"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		public DbLogEntry(DbId id)
		{
			this.id        = id;
			this.dateTime = System.DateTime.UtcNow;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbLogEntry"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="dateTime">The date and time.</param>
		public DbLogEntry(DbId id, System.DateTime dateTime)
		{
			this.id        = id;
			this.dateTime = dateTime;
		}

		/// <summary>
		/// Gets the log entry id.
		/// </summary>
		/// <value>The id.</value>
		public DbId								Id
		{
			get
			{
				return this.id;
			}
		}

		/// <summary>
		/// Gets the log entry date time.
		/// </summary>
		/// <value>The date time.</value>
		public System.DateTime					DateTime
		{
			get
			{
				return this.dateTime;
			}
		}

		#region IEquatable<DbLogEntry> Members

		public bool Equals(DbLogEntry other)
		{
			return (this.Id == other.Id)
				&& (this.DateTime == other.DateTime);
		}

		#endregion

		#region IComparable<DbLogEntry> Members

		public int CompareTo(DbLogEntry other)
		{
			int result = this.DateTime.CompareTo (other.DateTime);

			if (result == 0)
			{
				return this.Id.CompareTo (other.Id);
			}
			else
			{
				return result;
			}
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (obj is DbLogEntry)
			{
				return this.Equals ((DbLogEntry) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return this.Id.GetHashCode () ^ this.DateTime.GetHashCode ();
		}

		private DbId							id;
		private System.DateTime					dateTime;
	}
}