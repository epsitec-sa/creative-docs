//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	public struct ComparableData : System.IEquatable<ComparableData>, System.IComparable<ComparableData>
	{
		public ComparableData(object value)
		{
			this.Value = value;
		}

		public readonly object Value;

		public bool IsEmpty
		{
			get
			{
				return this.Value == null;
			}
		}

		public static readonly ComparableData Empty = new ComparableData ();

		#region IComparable<ComparableData> Members
		public int CompareTo(ComparableData other)
		{
			if (other.Value is int)
			{
				return ((int) this.Value).CompareTo ((int) other.Value);
			}
			else if (other.Value is decimal)
			{
				return ((decimal) this.Value).CompareTo ((decimal) other.Value);
			}
			else if (other.Value is System.DateTime)
			{
				return ((System.DateTime) this.Value).CompareTo ((System.DateTime) other.Value);
			}
			else if (other.Value is string)
			{
				return ((string) this.Value).CompareTo ((string) other.Value);
			}
			else
			{
				return 0;
			}
		}
		#endregion

		#region IEquatable<ComparableData> Members
		public bool Equals(ComparableData other)
		{
			if (other.Value is int)
			{
				return (int) this.Value == (int) other.Value;
			}
			else if (other.Value is decimal)
			{
				return (decimal) this.Value == (decimal) other.Value;
			}
			else if (other.Value is System.DateTime)
			{
				return (System.DateTime) this.Value == (System.DateTime) other.Value;
			}
			else if (other.Value is string)
			{
				return (string) this.Value == (string) other.Value;
			}
			else
			{
				return false;
			}
		}
		#endregion
	}
}
