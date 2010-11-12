//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	/// <summary>
	/// The <c>VatCodeRate</c> structure defines both a VAT code and an
	/// associated VAT rate, since there might be more than one rate for
	/// any given code (as was the case between 2010 and 2011 when the
	/// VAT transitioned from 7.6% to 8.0%).
	/// </summary>
	public struct VatCodeRate : System.IEquatable<VatCodeRate>, System.IComparable<VatCodeRate>
	{
		public VatCodeRate(VatCode code, decimal rate)
		{
			this.code = code;
			this.rate = rate;
		}


		public VatCode Code
		{
			get
			{
				return this.code;
			}
		}

		public decimal Rate
		{
			get
			{
				return this.rate;
			}
		}

		
		#region IEquatable<VatCodeRate> Members

		public bool Equals(VatCodeRate other)
		{
			return this.code == other.code
						&& this.rate == other.rate;
		}

		#endregion

		#region IComparable<VatCodeRate> Members

		public int CompareTo(VatCodeRate other)
		{
			if (this.code < other.code)
			{
				return -1;
			}
			if (this.code > other.code)
			{
				return 1;
			}
			
			if (this.rate < other.rate)
			{
				return -1;
			}
			if (this.rate > other.rate)
            {
				return 1;
            }

			return 0;
		}

		#endregion
		
		
		public override bool Equals(object obj)
		{
			if (obj is VatCodeRate)
			{
				return this.Equals ((VatCodeRate) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return this.code.GetHashCode () ^ this.rate.GetHashCode ();
		}

		public override string ToString()
		{
			return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}@{1:0.0##}%", this.code, this.rate * 100);
		}


		private readonly VatCode	code;
		private readonly decimal	rate;
	}
}
