//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	/// <summary>
	/// Structure contenant une valeur quelconque pouvant être comparée en vue du tri.
	/// </summary>
	public struct ComparableData : System.IComparable<ComparableData>
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
		public int CompareTo(ComparableData that)
		{
			if (this.Value is int || that.Value is int)
			{
				var v1 = (this.Value is int) ? (int) this.Value : int.MinValue;
				var v2 = (that.Value is int) ? (int) that.Value : int.MinValue;

				return v1.CompareTo (v2);
			}
			else if (this.Value is decimal || that.Value is decimal)
			{
				var v1 = (this.Value is decimal) ? (decimal) this.Value : decimal.MinValue;
				var v2 = (that.Value is decimal) ? (decimal) that.Value : decimal.MinValue;

				return v1.CompareTo (v2);
			}
			else if (this.Value is System.DateTime || that.Value is System.DateTime)
			{
				var v1 = (this.Value is System.DateTime) ? (System.DateTime) this.Value : System.DateTime.MinValue;
				var v2 = (that.Value is System.DateTime) ? (System.DateTime) that.Value : System.DateTime.MinValue;

				return v1.CompareTo (v2);
			}
			else if (this.Value is string || that.Value is string)
			{
				var v1 = (this.Value is string) ? (string) this.Value : "";
				var v2 = (that.Value is string) ? (string) that.Value : "";

				return v1.CompareTo (v2);
			}
			else
			{
				return 0;
			}
		}
		#endregion
	}
}
