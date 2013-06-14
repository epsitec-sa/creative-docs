//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Data.Platform
{
	/// <summary>
	/// The <c>SwissPostFullZip</c> structure contains the zip code and its add-on (complement).
	/// The Swiss Post defines for instance that St-Cierges has a zip code of 1410 and an add-on
	/// of 5.
	/// </summary>
	public struct SwissPostFullZip : System.IEquatable<SwissPostFullZip>, System.IComparable<SwissPostFullZip>
	{
		public SwissPostFullZip(int zipCode, int zipComplement)
		{
			System.Diagnostics.Debug.Assert (zipComplement >= 0);
			System.Diagnostics.Debug.Assert (zipComplement < 100);

			this.code = zipCode * 100 + zipComplement;
		}

		
		public int								ZipCode
		{
			get
			{
				return this.code / 100;
			}
		}

		public int								ZipCodeAddOn
		{
			get
			{
				return this.code % 100;
			}
		}


		#region IComparable<SwissPostFullZip> Members

		public int CompareTo(SwissPostFullZip other)
		{
			if (this.code < other.code)
			{
				return -1;
			}
			if (this.code > other.code)
			{
				return 1;
			}

			return 0;
		}

		#endregion

		#region IEquatable<SwissPostFullZip> Members

		public bool Equals(SwissPostFullZip other)
		{
			return this.code == other.code;
		}

		#endregion

		
		public override int GetHashCode()
		{
			return this.code;
		}

		public override bool Equals(object obj)
		{
			return this.code == ((SwissPostFullZip) obj).code;
		}

		public override string ToString()
		{
			return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0:0000}:{1:00}", this.ZipCode, this.ZipCodeAddOn);
		}

		
		public static bool operator ==(SwissPostFullZip a, SwissPostFullZip b)
		{
			return a.code == b.code;
		}

		public static bool operator !=(SwissPostFullZip a, SwissPostFullZip b)
		{
			return a.code != b.code;
		}

		
		public static string GetZipCode(int? zipCode, int? zipCodeAddOn = null)
		{
			if (zipCode == null)
			{
				return null;
			}
			else if (zipCodeAddOn == null)
			{
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0:0000}", zipCode.Value);
			}
			else
			{
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0:0000}{1:00}", zipCode.Value, zipCodeAddOn.Value);
			}
		}
		
		public static string GetZipCodeAddOn(int? value)
		{
			if (value == null)
			{
				return null;
			}
			else
			{
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0:00}", value.Value);
			}
		}

		
		private readonly int					code;
	}
}

