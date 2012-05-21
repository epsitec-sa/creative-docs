//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Graph.Data
{
	public struct Dimension : System.IEquatable<Dimension>, System.IComparable<Dimension>
	{
		public Dimension(string key, string value)
		{
			if (string.IsNullOrEmpty (key))
			{
				throw new System.ArgumentNullException ("key");
			}
			
			if (string.IsNullOrEmpty (value))
			{
				throw new System.ArgumentNullException ("value");
			}

			if (key.Contains (DimensionVector.KeyValueSeparator) ||
				key.Contains (DimensionVector.DimensionSeparator))
			{
				throw new System.ArgumentException ("Dimension key contains illegal character");
			}

			if (value.Contains (DimensionVector.KeyValueSeparator) ||
				value.Contains (DimensionVector.DimensionSeparator))
			{
				throw new System.ArgumentException ("Dimension value contains illegal character");
			}

			this.key   = key;
			this.value = value;
		}

		
		public string Key
		{
			get
			{
				return this.key;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}


		public override bool Equals(object obj)
		{
			if (obj is Dimension)
			{
				return this.Equals ((Dimension) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return this.key.GetHashCode () ^ this.value.GetHashCode ();
		}

		public override string ToString()
		{
			return string.Concat (this.key, "=", this.value);
		}


		#region IEquatable<Dimension> Members

		public bool Equals(Dimension other)
		{
			return this.key == other.key
				&& this.value == other.value;
		}

		#endregion

		#region IComparable<Dimension> Members

		public int CompareTo(Dimension other)
		{
			int comparison = string.CompareOrdinal (this.key, other.key);

			if (comparison == 0)
			{
				comparison = string.CompareOrdinal (this.value, other.value);
			}

			return comparison;
		}

		#endregion

		
		private readonly string key;
		private readonly string value;
	}
}
