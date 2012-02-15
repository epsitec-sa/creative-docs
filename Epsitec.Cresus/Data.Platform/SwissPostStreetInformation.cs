//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Data.Platform
{
	public sealed class SwissPostStreetInformation
	{
		public SwissPostStreetInformation(string line)
		{
			this.StreetCode            = line.Substring (0, 6);
			this.BasicPostCode         = line.Substring (6, 4);
			this.LanguageCode          = line.Substring (10, 1);
			this.StreetNameUppercase   = line.Substring (11, 25).TrimEnd ();
			this.ZipCode               = line.Substring (36, 4);
			this.ZipComplement         = line.Substring (40, 2);
			this.DividerCode           = line.Substring (42, 1);
			this.HouseNumberFrom       = line.Substring (43, 4);
			this.HouseNumberFromAlpha  = line.Substring (47, 2);
			this.HouseNumberTo         = line.Substring (49, 4);
			this.HouseNumberToAlpha    = line.Substring (53, 2);
			this.StreetName            = line.Substring (55, 25).TrimEnd ();
			this.StreetNameRoot        = line.Substring (80, 10).TrimEnd ();
			this.StreetNameType        = line.Substring (90, 2);
			this.StreetNamePreposition = line.Substring (92, 2);
			this.StreetNameShort       = this.StreetName.Split (',').First ();
		}

		public string StreetCode;
		public string BasicPostCode;
		public string LanguageCode;
		public string StreetNameUppercase;
		public string ZipCode;
		public string ZipComplement;
		public string DividerCode;
		public string HouseNumberFrom;
		public string HouseNumberFromAlpha;
		public string HouseNumberTo;
		public string HouseNumberToAlpha;
		public string StreetName;
		public string StreetNameShort;
		public string StreetNameRoot;
		public string StreetNameType;
		public string StreetNamePreposition;

		public bool MatchName(string name)
		{
			if (this.StreetNameShort == name)
			{
				return true;
			}

			name = name.ToUpperInvariant ();

			if (this.StreetNameRoot == name)
			{
				return true;
			}

			int pos = name.LastIndexOfAny (SwissPostStreetInformation.nameSeparators);

			if (pos < 0)
			{
				return false;
			}
			else
			{
				return name.Substring (pos+1) == this.StreetNameRoot;
			}
		}

		public override string ToString()
		{
			return string.Concat (this.ZipCode, " ", this.StreetName, " ", this.HouseNumberFrom, "-", this.HouseNumberTo);
		}
		private static readonly char[] nameSeparators = new char[] { ' ', '-', '.', '\'' };
	}
}
