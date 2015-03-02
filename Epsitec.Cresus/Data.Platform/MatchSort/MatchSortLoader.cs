//	Copyright © 2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Epsitec.Data.Platform.MatchSort.Mapping;
using Epsitec.Data.Platform.MatchSort.Rows;

namespace Epsitec.Data.Platform.MatchSort
{
	public sealed class MatchSortLoader
	{
		public static MatchSortMetaData GetFileMetaData(string csvFilePath)
		{
			if (System.IO.File.Exists (csvFilePath) == false)
			{
				throw new System.Exception ("The MAT[CH]sort file does not exist at path " + csvFilePath);
			}

			return MatchSortLoader.LoadBaseRecords<MatchSortMetaData> (csvFilePath, "00").SingleOrDefault ();
		}

		public static IEnumerable<T> LoadBaseRecords<T>(string csvFilePath, string recordType)
		{
			var records = new List<T> ();
			using (var stream = File.OpenText (csvFilePath))
			{
				var csv   = new CsvReader (stream, MatchSortLoader.ConfigureBaseReader ());
				while (csv.Read ())
				{
					if(recordType == csv.GetField (0))
					{
						records.Add (csv.GetRecord<T> ());
					}
				}
			}

			return records;
		}


		public static CsvConfiguration ConfigureBaseReader()
		{
			var config = new CsvConfiguration ();
			config.Encoding = System.Text.Encoding.GetEncoding ("Windows-1252");
			config.Delimiter = ";";
			config.HasHeaderRecord = false;
			config.IgnoreQuotes = true;
			config.RegisterClassMap<MatchSortMetaDataMap> ();
			config.RegisterClassMap<SwissPostZipInformationMap> ();
			return config;
		}
	}
}
