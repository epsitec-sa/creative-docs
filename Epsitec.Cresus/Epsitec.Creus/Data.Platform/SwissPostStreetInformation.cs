//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Data.Platform
{
	/// <summary>
	/// The <c>SwissPostStreetInformation</c> class describes a MAT[CH] street Switzerland
	/// light entry.
	/// </summary>
	public sealed class SwissPostStreetInformation
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SwissPostStreetInformation"/> class
		/// based on a source line taken from MAT[CH]street Switzerland light.
		/// http://www.post.ch/en/post-startseite/post-adress-services-match/post-direct-marketing-datengrundlage/post-direct-marketing-match-street.htm
		/// http://www.post.ch/en/post-startseite/post-adress-services-match/post-direct-marketing-datengrundlage/post-direct-marketing-match-street/post-match-street-schweiz-light-factsheet.pdf
		/// </summary>
		/// <param name="line">The line.</param>
		public SwissPostStreetInformation(string line)
		{
			this.StreetCode            = InvariantConverter.ParseInt (line.Substring (0, 6));
			this.BasicPostCode         = InvariantConverter.ParseInt (line.Substring (6, 4));
			this.LanguageCode          = InvariantConverter.ParseInt<SwissPostLanguageCode> (line.Substring (10, 1));
			this.ZipCode               = InvariantConverter.ParseInt (line.Substring (36, 4));
			this.ZipComplement         = InvariantConverter.ParseInt (line.Substring (40, 2));
			this.DividerCode           = InvariantConverter.ParseInt<SwissPostDividerCode> (line.Substring (42, 1));
			this.HouseNumberFrom       = InvariantConverter.ParseInt (line.Substring (43, 4));
			this.HouseNumberFromAlpha  = line.Substring (47, 2).TrimEnd ();
			this.HouseNumberTo         = InvariantConverter.ParseInt (line.Substring (49, 4));
			this.HouseNumberToAlpha    = line.Substring (53, 2).TrimEnd ();
			this.StreetName            = line.Substring (55, 25).TrimEnd ();
			this.StreetNameRoot        = line.Substring (80, 10).TrimEnd ();
			this.StreetNameType        = InvariantConverter.ParseInt (line.Substring (90, 2));
			this.StreetNamePreposition = InvariantConverter.ParseInt (line.Substring (92, 2));
			this.StreetNameShort       = this.StreetName.Split (',').First ();
			this.NormalizedStreetName  = SwissPostStreet.NormalizeStreetName (this.StreetName);
		}

		public readonly int						StreetCode;
		public readonly int						BasicPostCode;
		public readonly SwissPostLanguageCode	LanguageCode;
		public readonly int						ZipCode;
		public readonly int						ZipComplement;
		public readonly SwissPostDividerCode	DividerCode;
		public readonly int						HouseNumberFrom;
		public readonly string					HouseNumberFromAlpha;
		public readonly int						HouseNumberTo;
		public readonly string					HouseNumberToAlpha;
		public readonly string					StreetName;
		public readonly string					StreetNameShort;
		public readonly string					StreetNameRoot;
		public readonly int						StreetNameType;
		public readonly int						StreetNamePreposition;
		public readonly string					NormalizedStreetName;


		/// <summary>
		/// Check if the name matches this instance.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns><c>true</c> if the name matches this instance; otherwise, <c>false</c>.</returns>
		public bool MatchName(string name)
		{
			return this.NormalizedStreetName == SwissPostStreet.NormalizeStreetName (name);
		}

		/// <summary>
		/// Matches the name with heuristics, trying to correct common misspellings, such as
		/// "-aux" which should be written "-eaux", or "-is" which should be written "-it",
		/// etc.
		/// </summary>
		/// <param name="tokens">The array of street name tokens.</param>
		/// <returns><c>true</c> if the name could be matched; otherwise, <c>false</c>.</returns>
		public bool MatchNameWithHeuristics(string[] tokens)
		{
			var name1 = string.Join (" ", tokens);

			if ((this.NormalizedStreetName == name1) ||
				(this.MatchRootName (name1)))
			{
				return true;
			}

			var name2 = string.Join (" ", tokens.Where (x => !char.IsDigit (x[0]) && !SwissPostStreet.HeuristicTokens.Contains (x)));

			if (name1 != name2)
			{
				if ((this.NormalizedStreetName == name2) ||
					(this.MatchRootName (name2)))
				{
					return true;
				}
			}
			
			return this.MatchNameWithMisspellingHeuristics (name2);
		}

		private bool MatchNameWithMisspellingHeuristics(string name)
		{
			int len = name.Length;

			if (len > 3)
			{
				if (name.EndsWith ("AUX") && name[len-4] != 'E')
				{
					var probe = name.Substring (0, len-3) + "EAUX";
					
					if ((this.NormalizedStreetName == probe) ||
						(this.MatchRootName (probe)))
					{
						return true;
					}
				}
				if (name[len-1] != 'S')
				{
					var probe = name + "S";

					if ((this.NormalizedStreetName == probe) ||
						(this.MatchRootName (probe)))
					{
						return true;
					}
				}
				if (name.EndsWith ("IS"))
				{
					var probe = name.Substring (0, len-2) + "IT";

					if ((this.NormalizedStreetName == probe) ||
						(this.MatchRootName (probe)))
					{
						return true;
					}
				}
				if (name.EndsWith ("IT"))
				{
					var probe = name.Substring (0, len-2) + "IS";

					if ((this.NormalizedStreetName == probe) ||
						(this.MatchRootName (probe)))
					{
						return true;
					}
				}
				if (name.StartsWith ("ARTISANALE "))
				{
					var probe = name.Substring (11);

					if ((this.NormalizedStreetName == probe) ||
						(this.MatchRootName (probe)))
					{
						return true;
					}
				}
				if (name.StartsWith ("INDUSTRIELLE "))
				{
					var probe = name.Substring (13);

					if ((this.NormalizedStreetName == probe) ||
						(this.MatchRootName (probe)))
					{
						return true;
					}
				}
			}
			
			return false;
		}

		/// <summary>
		/// Matches the house number if it is in the range covered by this street information
		/// record.
		/// </summary>
		/// <param name="houseNumber">The house number.</param>
		/// <returns><c>true</c> if the house number matches; otherwise, <c>false</c>.</returns>
		public bool MatchHouseNumber(int houseNumber)
		{
			if (houseNumber == 0)
			{
				return true;
			}

			switch (this.DividerCode)
			{
				case SwissPostDividerCode.All:
				case SwissPostDividerCode.None:
					break;
				
				case SwissPostDividerCode.Even:
					if ((houseNumber & 0x01) == 1)
					{
						return false;
					}
					break;
				case SwissPostDividerCode.Odd:
					if ((houseNumber & 0x01) == 0)
					{
						return false;
					}
					break;
			}

			if ((this.HouseNumberFrom == 0) ||
				(this.HouseNumberTo == 0))
			{
				return true;
			}

			if ((houseNumber >= this.HouseNumberFrom) &&
				(houseNumber <= this.HouseNumberTo))
			{
				return true;
			}

			return false;
		}
		
		/// <summary>
		/// Check if the name matches the root name or the short name of the street.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns><c>true</c> if the name matches this instance; otherwise, <c>false</c>.</returns>
		public bool MatchShortNameOrRootName(string name)
		{
			if (this.StreetNameShort == name)
			{
				return true;
			}

			return this.MatchRootName (TextConverter.ConvertToUpperAndStripAccents (name));
		}
		
		/// <summary>
		/// Check if the root name matches this instance.
		/// </summary>
		/// <param name="name">The root name (uppercase and without accents).</param>
		/// <returns><c>true</c> if the root name matches this instance; otherwise, <c>false</c>.</returns>
		private bool MatchRootName(string rootName)
		{
			int len = rootName.Length;
			int pos = 0;

			if (rootName.Length > 10)
			{
				if (string.CompareOrdinal (rootName, 0, this.StreetNameRoot, 0, 10) == 0)
				{
					return true;
				}
			}
			else if (this.StreetNameRoot == rootName)
			{
				return true;
			}

			while (true)
			{
				pos = rootName.IndexOfAny (SwissPostStreet.NameSeparators, pos)+1;

				if (pos == 0)
				{
					return false;
				}
				if (string.CompareOrdinal (rootName, pos, this.StreetNameRoot, 0, System.Math.Min (10, len-pos)) == 0)
				{
					return true;
				}
				if (pos < 12)
				{
					if (string.CompareOrdinal (rootName, 0, this.StreetNameRoot, 0, pos-1) == 0)
					{
						return true;
					}
				}
			}
		}


		public override string ToString()
		{
			return string.Concat (this.ZipCode, " ", this.StreetName, " ", this.HouseNumberFrom, "-", this.HouseNumberTo);
		}
	}
}
