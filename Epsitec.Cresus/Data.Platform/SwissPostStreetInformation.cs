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

		/// <summary>
		/// Check if the root name matches this instance.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns><c>true</c> if the root name matches this instance; otherwise, <c>false</c>.</returns>
		public bool MatchRootName(string name)
		{
			if (this.StreetNameShort == name)
			{
				return true;
			}

			var rootName = TextConverter.ConvertToUpperAndStripAccents (name);

			if (this.StreetNameRoot == rootName)
			{
				return true;
			}

			int pos = rootName.LastIndexOfAny (SwissPostStreet.nameSeparators);

			if (pos < 0)
			{
				return false;
			}
			else
			{
				return rootName.Substring (pos+1) == this.StreetNameRoot;
			}
		}


		public override string ToString()
		{
			return string.Concat (this.ZipCode, " ", this.StreetName, " ", this.HouseNumberFrom, "-", this.HouseNumberTo);
		}
	}
}
