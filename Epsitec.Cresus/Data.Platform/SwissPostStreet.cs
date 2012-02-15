//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Collections;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Data.Platform
{
	public static class SwissPostStreet
	{
		public static SwissPostStreetRepository CreateRepository()
		{
			return new SwissPostStreetRepository (SwissPostStreet.GetStreets ());
		}

		private static IEnumerable<SwissPostStreetInformation> GetStreets()
		{
			foreach (var line in SwissPostStreet.GetStreetFile ())
			{
				yield return new SwissPostStreetInformation (line);
			}
		}

		private static IEnumerable<string> GetStreetFile()
		{
			string file = SwissPostStreet.ReadZippedTextFile ("Epsitec.Data.Platform.DataFiles.MatchStreetLight.zip");
			return Epsitec.Common.IO.StringLineExtractor.GetLines (file);
		}

		private static string ReadZippedTextFile(string resourcePath)
		{
			var assembly = System.Reflection.Assembly.GetExecutingAssembly ();

			using (System.IO.Stream stream = assembly.GetManifestResourceStream (resourcePath))
			{
				var zipFile = new Epsitec.Common.IO.ZipFile ();
				zipFile.LoadFile (stream);
				var zipEntry = zipFile.Entries.First ();
				return System.Text.Encoding.Default.GetString (zipEntry.Data);
			}
		}
	}
}
