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
	public struct DatabaseDateTime : System.IEquatable<DatabaseDateTime>, System.IComparable<DatabaseDateTime>
	{
		public DatabaseDateTime(System.DateTime value)
		{
			this.value = value;
		}


		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <value>The value.</value>
		public System.DateTime Value
		{
			get
			{
				return this.value;
			}
		}


		#region IEquatable<DatabaseDateTime> Members

		public bool Equals(DatabaseDateTime other)
		{
			return other.value == this.value;
		}

		#endregion

		#region IComparable<DatabaseDateTime> Members

		public int CompareTo(DatabaseDateTime other)
		{
			return this.value.CompareTo (other.Value);
		}

		#endregion

		private readonly System.DateTime		value;
	}
}
