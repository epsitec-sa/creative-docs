//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
			this.StreetCode            = line.Substring (0, 6);
			this.BasicPostCode         = line.Substring (6, 4);
			this.LanguageCode          = line.Substring (10, 1);
			//this.StreetNameUppercase   = line.Substring (11, 25).TrimEnd ();
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
			this.NormalizedStreetName  = SwissPostStreet.NormalizeStreetName (this.StreetName);
		}

		public readonly string					StreetCode;
		public readonly string					BasicPostCode;
		public readonly string					LanguageCode;
		//public readonly string					StreetNameUppercase;
		public readonly string					ZipCode;
		public readonly string					ZipComplement;
		public readonly string					DividerCode;
		public readonly string					HouseNumberFrom;
		public readonly string					HouseNumberFromAlpha;
		public readonly string					HouseNumberTo;
		public readonly string					HouseNumberToAlpha;
		public readonly string					StreetName;
		public readonly string					StreetNameShort;
		public readonly string					StreetNameRoot;
		public readonly string					StreetNameType;
		public readonly string					StreetNamePreposition;
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

		public bool MatchNameWithHeuristics(string[] tokens)
		{
			//var tokens = SwissPostStreet.TokenizeStreetName (name);

			var name = string.Join (" ", tokens);

			if ((this.NormalizedStreetName == name) ||
				(this.MatchRootName (name)))
			{
				return true;
			}

			name = string.Join (" ", tokens.Where (x => SwissPostStreet.heuristicTokens.Contains (x) == false).Where (x => !char.IsDigit (x[0])));

			if ((this.NormalizedStreetName == name) ||
				(this.MatchRootName (name)))
			{
				return true;
			}

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
				pos = rootName.IndexOfAny (SwissPostStreet.nameSeparators, pos)+1;

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
