//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.IO;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public static class DataIO
	{
		public static void OpenMainXml(DataAccessor accessor, string filename)
		{
			var reader = System.Xml.XmlReader.Create (filename);
			accessor.Mandat = new DataMandat (reader);
			reader.Close ();
		}

		public static void SaveMainXml(DataAccessor accessor, string filename)
		{
			var settings = new System.Xml.XmlWriterSettings
			{
				Indent = true,
			};

			var writer = System.Xml.XmlWriter.Create (filename, settings);
			accessor.Mandat.Serialize (writer);

			writer.Flush ();
			writer.Close ();
		}


		public static void Compress(string xmlFilename, string zipFilename)
		{
			System.IO.File.Delete (zipFilename);
			Compression.GZipCompressFile (xmlFilename, zipFilename);
		}

		public static void Decompress(string zipFilename, string xmlFilename)
		{
			System.IO.File.Delete (xmlFilename);
			Compression.GZipDecompressFile (zipFilename, xmlFilename);
		}


		public static void Delete(string filename)
		{
			System.IO.File.Delete (filename);
		}
	}
}
