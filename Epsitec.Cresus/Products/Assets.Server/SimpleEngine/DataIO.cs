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
		public static MandatInfo OpenInfo(string filename)
		{
			//	Lit le petit fichier xml d'informations.
			var info = MandatInfo.Empty;

			var infoFilename = DataIO.GetInfoFilename (filename);
			var reader = System.Xml.XmlReader.Create (infoFilename);

			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case "FileDescription":
							info = DataIO.OpenInfo (reader);
							break;
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}

			reader.Close ();

			return info;
		}

		private static MandatInfo OpenInfo(System.Xml.XmlReader reader)
		{
			string softwareId       = null;
			string softwareVersion  = null;
			string softwareLanguage = null;
			string fileName         = null;
			var    fileGuid         = Guid.Empty;
			string fileVersion      = null;
			var    statistics       = MandatStatistics.Empty;

			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case "Software":
							softwareId       = reader["id"];
							softwareVersion  = reader["ver"];
							softwareLanguage = reader["lang"];
							break;

						case "File":
							fileName    = reader["name"];
							fileGuid    = Guid.Parse (reader["id"]);
							fileVersion = reader["ver"];
							break;

						case "About":
							statistics = DataIO.OpenStatistics (reader);
							break;
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}

			return new MandatInfo (softwareId, softwareVersion, softwareLanguage, fileName, fileGuid, fileVersion, statistics);
		}

		private static MandatStatistics OpenStatistics(System.Xml.XmlReader reader)
		{
			int assetCount    = 0;
			int eventCount    = 0;
			int categoryCount = 0;
			int groupCount    = 0;
			int personCount   = 0;
			int reportCount   = 0;
			int accountCount  = 0;

			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					if (reader.Name == "Data")
					{
						int i = int.Parse (reader["value"], System.Globalization.CultureInfo.InvariantCulture);

						switch (reader["name"])
						{
							case "AssetCount":
								assetCount = i;
								break;

							case "EventCount":
								eventCount = i;
								break;

							case "CategoryCount":
								categoryCount = i;
								break;

							case "GroupCount":
								groupCount = i;
								break;

							case "PersonCount":
								personCount = i;
								break;

							case "ReportCount":
								reportCount = i;
								break;

							case "AccountCount":
								accountCount = i;
								break;
						}
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}

			return new MandatStatistics (assetCount, eventCount, categoryCount, groupCount, personCount, reportCount, accountCount);
		}


		public static void SaveInfo(string filename, MandatInfo info)
		{
			//	Ecrit le petit fichier xml d'informations.
			var settings = new System.Xml.XmlWriterSettings
			{
				Indent = true,
			};

			var infoFilename = DataIO.GetInfoFilename (filename);
			var writer = System.Xml.XmlWriter.Create (infoFilename, settings);

			writer.WriteStartDocument ();
			writer.WriteStartElement ("FileDescription");

			writer.WriteStartElement    ("Software");
			writer.WriteAttributeString ("id",   info.SoftwareId);
			writer.WriteAttributeString ("ver",  info.SoftwareVersion);
			writer.WriteAttributeString ("lang", info.SoftwareLanguage);
			writer.WriteEndElement      ();

			writer.WriteStartElement    ("File");
			writer.WriteAttributeString ("name", info.FileName);
			writer.WriteAttributeString ("id",   info.FileGuid.ToString ());
			writer.WriteAttributeString ("ver",  info.FileVersion);
			writer.WriteEndElement      ();

			DataIO.SaveStatistics (writer, info.Statistics);

			writer.WriteEndElement ();
			writer.WriteEndDocument ();

			writer.Flush ();
			writer.Close ();
		}

		private static void SaveStatistics(System.Xml.XmlWriter writer, MandatStatistics statistics)
		{
			writer.WriteStartElement ("About");

			DataIO.SaveStatistics (writer, "AssetCount",    statistics.AssetCount   );
			DataIO.SaveStatistics (writer, "EventCount",    statistics.EventCount   );
			DataIO.SaveStatistics (writer, "CategoryCount", statistics.CategoryCount);
			DataIO.SaveStatistics (writer, "GroupCount",    statistics.GroupCount   );
			DataIO.SaveStatistics (writer, "PersonCount",   statistics.PersonCount  );
			DataIO.SaveStatistics (writer, "ReportCount",   statistics.ReportCount  );
			DataIO.SaveStatistics (writer, "AccountCount",  statistics.AccountCount );

			writer.WriteEndElement ();
		}

		private static void SaveStatistics(System.Xml.XmlWriter writer, string name, int count)
		{
			writer.WriteStartElement    ("Data");
			writer.WriteAttributeString ("name",  name);
			writer.WriteAttributeString ("value", count.ToString (System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteEndElement      ();
		}


		public static void OpenMandat(DataAccessor accessor, string filename)
		{
			//	Lit le mandat, soit à partir du long fichier xml s'il existe, sinon à
			//	partir du fichier compressé.
			if (DataIO.ExistingXml (filename))
			{
				DataIO.OpenXml (accessor, filename);  // lit directement le fichier xml
			}
			else
			{
				DataIO.Decompress (filename);         // décompresse le fichier .gz
				DataIO.OpenXml (accessor, filename);  // puis lit le fichier xml
				DataIO.DeleteXml (filename);          // pour finalement le supprimer
			}
		}

		private static void OpenXml(DataAccessor accessor, string filename)
		{
			var xmlFilename = DataIO.GetXmlFilename (filename);
			var reader = System.Xml.XmlReader.Create (xmlFilename);
			accessor.Mandat = new DataMandat (reader);
			reader.Close ();
		}


		public static void SaveMandat(DataAccessor accessor, string filename, SaveMandatMode mode)
		{
			//	Enregistre le mandat dans un fichier compressé. Selon le mode, on conserve
			//	ou non le long fichier xml.
			DataIO.SaveInfo (filename, accessor.Mandat.MandatInfo);
			DataIO.SaveXml (accessor, filename);
			DataIO.Compress (filename);
			DataIO.CacheZip (filename);

			if ((mode & SaveMandatMode.KeepXml) == 0)
			{
				DataIO.DeleteXml (filename);
			}
		}

		private static void SaveXml(DataAccessor accessor, string filename)
		{
			var settings = new System.Xml.XmlWriterSettings
			{
				Indent = true,
			};

			var xmlFilename = DataIO.GetXmlFilename (filename);
			var writer = System.Xml.XmlWriter.Create (xmlFilename, settings);
			accessor.Mandat.Serialize (writer);

			writer.Flush ();
			writer.Close ();
		}


		private static void Compress(string filename)
		{
			//	Compresse le fichier xml -> gz.
			var xmlFilename = DataIO.GetXmlFilename (filename);
			var zipFilename = DataIO.GetZipFilename (filename);

			System.IO.File.Delete (zipFilename);
			Compression.GZipCompressFile (xmlFilename, zipFilename);
		}

		private static void Decompress(string filename)
		{
			//	Décompresse le fichier gz -> xml.
			var xmlFilename = DataIO.GetXmlFilename (filename);
			var zipFilename = DataIO.GetZipFilename (filename);

			System.IO.File.Delete (xmlFilename);
			Compression.GZipDecompressFile (zipFilename, xmlFilename);
		}


		private static bool ExistingXml(string filename)
		{
			//	Retourne true si le fichier xml existe.
			var xmlFilename = DataIO.GetXmlFilename (filename);
			return System.IO.File.Exists (xmlFilename);
		}

		private static void DeleteXml(string filename)
		{
			//	Supprime le fichier xml.
			var xmlFilename = DataIO.GetXmlFilename (filename);
			System.IO.File.Delete (xmlFilename);
		}


		private static void CacheZip(string filename)
		{
			//	Cache le fichier comprimé.
			var zipFilename = DataIO.GetZipFilename (filename);

			var attributes = System.IO.File.GetAttributes (zipFilename);
			attributes |= System.IO.FileAttributes.Hidden;
			System.IO.File.SetAttributes (zipFilename, attributes);
		}


		private static string GetInfoFilename(string filename)
		{
			//	Retourne le nom du fichier court, le seul visible.
			//	C'est en fait le même que celui qui a été choisi par l'utilisateur.
			return filename;
		}

		private static string GetZipFilename(string filename)
		{
			//	Retourne le nom du fichier comprimé caché, contenant les données.
			return filename + ".gz";
		}

		private static string GetXmlFilename(string filename)
		{
			//	Retourne le nom du fichier xml temporaire, habituellement détruit après-coup.
			return filename + ".xml";
		}
	}
}
