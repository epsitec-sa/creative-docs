//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Data.Platform
{
	public struct SwissPostZipCodeFolding : System.IEquatable<SwissPostZipCodeFolding>
	{
		public SwissPostZipCodeFolding(string zipCode, string zipComplement, string baseZipCode, string zipCodeType)
		{
			this.zip           = new SwissPostFullZip (InvariantConverter.ParseInt (zipCode), InvariantConverter.ParseInt (zipComplement));
			this.baseZipCode   = InvariantConverter.ParseInt (baseZipCode);
			this.zipCodeType   = InvariantConverter.ParseInt<SwissPostZipType> (zipCodeType);
		}

		public SwissPostZipCodeFolding(int zipCode, int zipComplement, int baseZipCode, SwissPostZipType zipCodeType)
		{
			this.zip           = new SwissPostFullZip (zipCode, zipComplement);
			this.baseZipCode   = baseZipCode;
			this.zipCodeType   = zipCodeType;
		}

		
		public bool								IsValid
		{
			get
			{
				return this.ZipCode >= 1000 && this.ZipCode <= 9999;
			}
		}

		public int								ZipCode
		{
			get
			{
				return this.zip.ZipCode;
			}
		}

		public int								ZipCodeAddOn
		{
			get
			{
				return this.zip.ZipCodeAddOn;
			}
		}

		public SwissPostFullZip					ZipCodeAndAddOn
		{
			get
			{
				return this.zip;
			}
		}

		public int								BaseZipCode
		{
			get
			{
				return this.baseZipCode;
			}
		}

		public SwissPostZipType					ZipCodeType
		{
			get
			{
				return this.zipCodeType;
			}
		}

		#region IEquatable<ZipCodeFolding> Members

		public bool Equals(SwissPostZipCodeFolding other)
		{
			return this.zip == other.zip
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
			return this.zip.GetHashCode ();
		}

		public override string ToString()
		{
			return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0:0000};{1:00};{2:0000};{3:00}", this.ZipCode, this.ZipCodeAddOn, this.baseZipCode, (int) this.zipCodeType);
		}

		public static SwissPostZipCodeFolding Parse(string value)
		{
			var args = value.Split (';');

			return new SwissPostZipCodeFolding (args[0], args[1], args[2], args[3]);
		}

		private readonly SwissPostFullZip		zip;
		private readonly int					baseZipCode;
		private readonly SwissPostZipType		zipCodeType;
	}
}

