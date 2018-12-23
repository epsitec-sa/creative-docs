//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Epsitec.Data.Platform.MatchSort;

namespace Epsitec.Data.Platform
{
	public static class SwissPost
	{
        static SwissPost()
        {
            SwissPost.Streets   = new SwissPostStreetRepository ();
            SwissPost.Zips      = new SwissPostZipRepository ();
            SwissPost.Countries = Iso3166.GetCountries ("FR").ToArray ();
        }

        public static void Initialize()
		{
		}


        public static SwissPostStreetRepository Streets { get; }
        public static SwissPostZipRepository Zips { get; }
        public static IList<GeoNamesCountryInformation> Countries { get; }


        /// <summary>
        /// Generate a Cresus nupost.txt retro-comaptible format
        /// </summary>
        /// <param name="outputFile">full path with file name and ext.</param>
        public static void GenerateCresusNupoFile(string outputFile)
		{
			var encoding     = System.Text.Encoding.GetEncoding ("Windows-1252");
			var cresusConfig = new CsvConfiguration ();
			cresusConfig.Encoding = encoding;
			cresusConfig.Delimiter = "  ";
			cresusConfig.HasHeaderRecord = false;
			cresusConfig.IgnoreQuotes = true;

			using (var stream = System.IO.File.OpenText (SwissPostZip.GetSwissPostZipCsv ()))
			using (var outputStream = new System.IO.StreamWriter (System.IO.File.CreateText (outputFile).BaseStream, encoding))
			{
				var csv   = new CsvReader (stream, MatchSortLoader.ConfigureSwissPostReader<SwissPostZipInformation> ());
				var zips  = csv.GetRecords<SwissPostZipInformation> ().OrderBy (z => z.ZipCode).ToList ();
				var csvWriter  = new CsvWriter (outputStream, cresusConfig);
				foreach (var zip in zips)
				{
					if (zip.ZipType != SwissPostZipType.Internal)
					{
						csvWriter.WriteField (zip.ZipCode);
						csvWriter.WriteField (zip.Canton);
						csvWriter.WriteField (zip.LongName);
						csvWriter.NextRecord ();
					}					
				}
			}
		}

		public static MatchWebClient MatchWebClient = new MatchWebClient ();
	}
}
