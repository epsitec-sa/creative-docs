//	Copyright © 2015-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Epsitec.Data.Platform.MatchSort.Mapping;
using Epsitec.Data.Platform.MatchSort.Rows;

namespace Epsitec.Data.Platform.MatchSort
{
	public sealed class MatchSortExtractor
	{
		public static void WriteRecordsToFile<T>(string csvFilePath, string recordType, string outputFile)
		{
			var outputDir = System.IO.Path.GetDirectoryName (outputFile);
			System.IO.Directory.CreateDirectory (outputDir);

			using (var stream = File.OpenText (csvFilePath))
			using (var outputStream = File.CreateText (outputFile))
			{
				var csvReader   = new CsvReader (stream, MatchSortLoader.ConfigureBaseReader ());
				var csvWriter   = new CsvWriter (outputStream, MatchSortExtractor.ConfigureWriter<T> ());
				while (csvReader.Read ())
				{
					if (recordType == csvReader.GetField (0))
					{
						csvWriter.WriteRecord<T> (csvReader.GetRecord<T> ());
						csvWriter.NextRecord ();
					}
				}
			}
		}

		private static CsvConfiguration ConfigureWriter<T>()
		{
			var config = new CsvConfiguration (CultureInfo.CurrentCulture);
			config.Encoding = System.Text.Encoding.UTF8;
			config.Delimiter = ";";
			config.HasHeaderRecord = false;
			config.IgnoreQuotes = true;
            config.IncludePrivateMembers = true;
            config.AutoMap<T> ();
			return config;
		}
	}
}
