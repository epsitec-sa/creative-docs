using System;
namespace Epsitec.Data.Platform
{
	public interface ISwissPostStreetInformation
	{
		int BasicPostCode
		{
			get;
		}
		SwissPostDividerCode DividerCode
		{
			get;
		}
		int HouseNumberFrom
		{
			get;
		}
		string HouseNumberFromAlpha
		{
			get;
		}
		int HouseNumberTo
		{
			get;
		}
		string HouseNumberToAlpha
		{
			get;
		}
		SwissPostLanguageCode LanguageCode
		{
			get;
		}
		string NormalizedStreetName
		{
			get;
		}
		int StreetCode
		{
			get;
		}
		string StreetName
		{
			get;
		}
		int StreetNamePreposition
		{
			get;
		}
		string StreetNameRoot
		{
			get;
		}
		string StreetNameShort
		{
			get;
		}
		int StreetNameType
		{
			get;
		}
		int ZipCode
		{
			get;
		}
		int ZipCodeAddOn
		{
			get;
		}
		SwissPostFullZip ZipCodeAndAddOn
		{
			get;
		}

		bool MatchHouseNumber(int houseNumber);
		bool MatchName(string name);
		bool MatchNameWithHeuristics(string[] tokens);
		bool MatchShortNameOrRootName(string name);
	}
}
