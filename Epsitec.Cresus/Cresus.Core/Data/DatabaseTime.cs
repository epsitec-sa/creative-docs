//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	/// <summary>
	/// The <c>DatabaseDateTime</c> structure wraps a <see cref="System.DateTime"/> value
	/// representing a date and time based on the database server clock.
	/// </summary>
	public struct DatabaseTime : System.IEquatable<DatabaseTime>, System.IComparable<DatabaseTime>
	{
		public DatabaseTime(System.DateTime value)
		{
			this.value = value;
		}


		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <value>The value.</value>
		public System.DateTime					Value
		{
			get
			{
				return this.value;
			}
		}


		/// <summary>
		/// Get the database time as a local application date and time.
		/// </summary>
		/// <param name="data">The <see cref="CoreData"/> instance.</param>
		/// <returns>The local application date and time.</returns>
		public System.DateTime ToLocalTime(CoreData data)
		{
			return this.value + data.ConnectionManager.TimeOffset;
		}


		#region IEquatable<DatabaseDateTime> Members

		public bool Equals(DatabaseTime other)
		{
			return other.value == this.value;
		}

		#endregion

		#region IComparable<DatabaseDateTime> Members

		public int CompareTo(DatabaseTime other)
		{
			return this.value.CompareTo (other.Value);
		}

		#endregion

		private readonly System.DateTime		value;
	}
}
