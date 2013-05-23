//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Data.Platform
{
	public struct SwissPostZipCodeFolding : System.IEquatable<SwissPostZipCodeFolding>
	{
		public SwissPostZipCodeFolding(string zipCode, string baseZipCode, string zipCodeType)
		{
			this.zipCode     = InvariantConverter.ParseInt (zipCode);
			this.baseZipCode = InvariantConverter.ParseInt (baseZipCode);
			this.zipCodeType = InvariantConverter.ParseInt<SwissPostZipType> (zipCodeType);
		}

		public SwissPostZipCodeFolding(int zipCode, int baseZipCode, SwissPostZipType zipCodeType)
		{
			this.zipCode     = zipCode;
			this.baseZipCode = baseZipCode;
			this.zipCodeType = zipCodeType;
		}

		public bool IsValid
		{
			get
			{
				return this.zipCode >= 1000 && this.zipCode <= 9999;
			}
		}

		public int ZipCode
		{
			get
			{
				return this.zipCode;
			}
		}

		public int BaseZipCode
		{
			get
			{
				return this.baseZipCode;
			}
		}

		public SwissPostZipType ZipCodeType
		{
			get
			{
				return this.zipCodeType;
			}
		}

		#region IEquatable<ZipCodeFolding> Members

		public bool Equals(SwissPostZipCodeFolding other)
		{
			return this.zipCode == other.zipCode
					&& this.baseZipCode == other.baseZipCode
					&& this.zipCodeType == other.zipCodeType;
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (obj is SwissPostZipCodeFolding)
			{
				return this.Equals ((SwissPostZipCodeFolding) obj);
			}

			return false;
		}

		public override int GetHashCode()
		{
			return this.zipCode;
		}

		public override string ToString()
		{
			return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0:0000};{1:0000};{2:00}", this.zipCode, this.baseZipCode, (int) this.zipCodeType);
		}

		public static SwissPostZipCodeFolding Parse(string value)
		{
			var args = value.Split (';');

			return new SwissPostZipCodeFolding (args[0], args[1], args[2]);
		}

		private readonly int					zipCode;
		private readonly int					baseZipCode;
		private readonly SwissPostZipType		zipCodeType;
	}
}

