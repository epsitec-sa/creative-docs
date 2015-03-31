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
		public static void Initialize()
		{
			var streetRepo = SwissPostStreetRepository.Current;
			var zipRepo    = SwissPostZipRepository.Current;
			var countries  = Iso3166.GetCountries ("FR").ToArray ();
		}

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
				var zips  = csv.GetRecords<SwissPostZipInformation> ().ToList ();
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
