//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		public void SetSwissPostZipInformations()
		{
			this.Zip = SwissPostZipRepository.Current.FindByOnrpCode (this.OnrpCode);
		}

		public void SetSwissPostHouseInformations()
		{
			var currentStreetHouses = SwissPostHouseRepository.Current.FindByStreetCode (this.StreetCode);
			this.HouseNumberFrom = currentStreetHouses.Min (h => h.HouseNumber);
			this.HouseNumberTo = currentStreetHouses.Max (h => h.HouseNumber);
			this.HouseNumberFromAlpha = this.HouseNumberFrom + currentStreetHouses.Where (h => h.HouseNumber == this.HouseNumberFrom).Min (h => h.HouseLetter);
			this.HouseNumberToAlpha = this.HouseNumberTo + currentStreetHouses.Where (h => h.HouseNumber == this.HouseNumberTo).Max (h => h.HouseLetter);
		}

		/// <summary>
		/// Old check&fix for this class
		/// </summary>
		public void BuildAndCheckRootName()
		{
			this.StreetNameRoot = TextConverter.ConvertToUpperAndStripAccents (this.StreetNameShort.Split (',')[0]);
			if (this.StreetNameRoot.Length < 2)
			{
				//	Very short root names are often an indication that something is incorrect in the source data
				//	file. "Marjovet B" has a root name "B" whereas it should be "MARJOVET", for instance...

				var names = TextConverter.ConvertToUpperAndStripAccents (this.StreetNameShort).Split (' ', '-', '\'').Where (x => x.Length > 1).ToArray ();

				if (names.Length > 0)
				{
					if ((names.Length > 1) &&
						(names[0] == "CHALET"))
					{
						names = names.Skip (1).ToArray ();
					}

					var fix = names[0];

					this.StreetNameRoot = fix;
					System.Console.WriteLine ("Very short root name detected, fix applied", this.ToString ());
				}
			}

			if (SwissPostStreet.HeuristicTokens.Contains (this.StreetNameRoot))
			{
				var names = TextConverter.ConvertToUpperAndStripAccents (this.StreetNameShort).Split (' ', '-', '\'').Where (x => x.Length > 1).ToArray ();

				if (!names.Any (x => x.StartsWith (this.StreetNameRoot) || x.EndsWith (this.StreetNameRoot)))
				{
					this.StreetNameRoot = names.Last ();
					System.Console.WriteLine ("Fix applied for {0}", this.ToString ());
				}
			}
		}

		public void BuildNormalizedName()
		{
			this.NormalizedStreetName = SwissPostStreet.NormalizeStreetName (this.StreetName);
		}

		/// <summary>
		/// Return corresponding Mat[CH]Sort datatable id (REC_ART)
		/// </summary>
		/// <returns></returns>
		public static string GetMatchRecordId()
		{
			return "04";
		}

		public SwissPostFullZip			ZipCodeAndAddOn
		{
			get
			{
				return new SwissPostFullZip (this.Zip.ZipCode, this.Zip.ZipCodeAddOn);
			}
		}

		public int						StreetCode
		{
			get;
			set;
		}
		public int						OnrpCode
		{
			get;
			set;
		}
		public SwissPostLanguageCode	LanguageCode
		{
			get;
			set;
		}
		public SwissPostZipInformation  Zip
		{
			get;
			internal set;
		}
		public SwissPostDividerCode	    DividerCode
		{
			get;
			set;
		}
		public int						HouseNumberFrom
		{
			get;
			set;
		}
		public string					HouseNumberFromAlpha
		{
			get;
			set;
		}
		public int						HouseNumberTo
		{
			get;
			set;
		}
		public string					HouseNumberToAlpha
		{
			get;
			set;
		}
		public string					StreetName
		{
			get;
			set;
		}
		public string					StreetNameShort
		{
			get;
			set;
		}
		public string					StreetNameRoot
		{
			get;
			set;
		}
		public int						StreetNameType
		{
			get;
			set;
		}
		public int						StreetNamePreposition
		{
			get;
			set;
		}
		public string					NormalizedStreetName
		{
			get;
			set;
		}


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
					var name1 = rootName.Substring (pos, System.Math.Min (10, len-pos));
					var name2 = this.StreetNameRoot.Substring (0, System.Math.Min (10, this.StreetNameRoot.Length));

					if (name1 == name2)
					{
						return true;
					}
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
			return string.Concat (this.Zip.ZipCode, " ", this.StreetName, " ", this.HouseNumberFrom, "-", this.HouseNumberTo);
		}
	}
}
