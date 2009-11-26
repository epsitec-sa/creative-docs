//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Graph.Data
{
	/// <summary>
	/// The <c>ChartValue</c> structure represents a key/value pair, which in this
	/// context is represented by a label (<c>string</c>) and a value (<c>double</c>).
	/// </summary>
	public struct ChartValue : System.IEquatable<ChartValue>, System.IComparable<ChartValue>
	{
		public ChartValue(string label, double value)
		{
			this.label = label;
			this.value = value;
		}

		public string Label
		{
			get
			{
				return this.label ?? "";
			}
		}

		public double Value
		{
			get
			{
				return this.value;
			}
		}


		public static readonly ChartValue Empty = new ChartValue ();


		public override bool Equals(object obj)
		{
			if (obj is ChartValue)
			{
				return this.Equals ((ChartValue) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return this.Label.GetHashCode () ^ this.value.GetHashCode ();
		}

		public override string ToString()
		{
			return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}={1}", this.Label, this.value);
		}


		#region IEquatable<ChartValue> Members

		public bool Equals(ChartValue other)
		{
			return this.Label == other.Label
				&& this.value == other.value;
		}

		#endregion

		#region IComparable<ChartValue> Members

		public int CompareTo(ChartValue other)
		{
			if (this.value < other.value)
			{
				return -1;
			}
			else if (this.value > other.value)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		#endregion

		private readonly string label;
		private readonly double value;
	}
}
