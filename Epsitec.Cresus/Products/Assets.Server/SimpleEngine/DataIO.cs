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
			var info = MandatInfo.Empty;

			var infoFilename = DataIO.GetInfoFilename (filename);
			var reader = System.Xml.XmlReader.Create (infoFilename);

			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case "Info":
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
			int	majRev     = 0;
			int	minRev     = 0;
			int	buildRev   = 0;
			int	idProducer = 0;
			var mandatGuid = Guid.Empty;
			var statistics = MandatStatistics.Empty;

			while (reader.Read ())
			{
				string s;

				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case "MajRev":
							s = reader.ReadElementContentAsString ();
							majRev = int.Parse (s, System.Globalization.CultureInfo.InvariantCulture);
							break;

						case "MinRev":
							s = reader.ReadElementContentAsString ();
							minRev = int.Parse (s, System.Globalization.CultureInfo.InvariantCulture);
							break;

						case "BuildRev":
							s = reader.ReadElementContentAsString ();
							buildRev = int.Parse (s, System.Globalization.CultureInfo.InvariantCulture);
							break;

						case "IdProducer":
							s = reader.ReadElementContentAsString ();
							idProducer = int.Parse (s, System.Globalization.CultureInfo.InvariantCulture);
							break;

						case "MandatGuid":
							s = reader.ReadElementContentAsString ();
							mandatGuid = Guid.Parse (s);
							break;

						case "Statistics":
							statistics = DataIO.OpenStatistics (reader);
							break;
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}

			return new MandatInfo (majRev, minRev, buildRev, idProducer, mandatGuid, statistics);
		}

		private static MandatStatistics OpenStatistics(System.Xml.XmlReader reader)
		{
			int assetsCount     = 0;
			int eventsCount     = 0;
			int categoriesCount = 0;
			int groupsCount     = 0;
			int personsCount    = 0;
			int reportsCount    = 0;
			int accountsCount   = 0;

			while (reader.Read ())
			{
				string s;

				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case "AssetsCount":
							s = reader.ReadElementContentAsString ();
							assetsCount = int.Parse (s, System.Globalization.CultureInfo.InvariantCulture);
							break;

						case "EventsCount":
							s = reader.ReadElementContentAsString ();
							eventsCount = int.Parse (s, System.Globalization.CultureInfo.InvariantCulture);
							break;

						case "CategoriesCount":
							s = reader.ReadElementContentAsString ();
							categoriesCount = int.Parse (s, System.Globalization.CultureInfo.InvariantCulture);
							break;

						case "GroupsCount":
							s = reader.ReadElementContentAsString ();
							groupsCount = int.Parse (s, System.Globalization.CultureInfo.InvariantCulture);
							break;

						case "PersonsCount":
							s = reader.ReadElementContentAsString ();
							personsCount = int.Parse (s, System.Globalization.CultureInfo.InvariantCulture);
							break;

						case "ReportsCount":
							s = reader.ReadElementContentAsString ();
							reportsCount = int.Parse (s, System.Globalization.CultureInfo.InvariantCulture);
							break;

						case "AccountsCount":
							s = reader.ReadElementContentAsString ();
							accountsCount = int.Parse (s, System.Globalization.CultureInfo.InvariantCulture);
							break;
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}

			return new MandatStatistics (assetsCount, eventsCount, categoriesCount, groupsCount, personsCount, reportsCount, accountsCount);
		}


		public static void SaveInfo(string filename, MandatInfo info)
		{
			var settings = new System.Xml.XmlWriterSettings
			{
				Indent = true,
			};

			var infoFilename = DataIO.GetInfoFilename (filename);
			var writer = System.Xml.XmlWriter.Create (infoFilename, settings);

			writer.WriteStartDocument ();
			writer.WriteStartElement ("Info");

			writer.WriteElementString ("MajRev",     info.MajRev    .ToString (System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteElementString ("MinRev",     info.MinRev    .ToString (System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteElementString ("BuildRev",   info.BuildRev  .ToString (System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteElementString ("IdProducer", info.IdProducer.ToString (System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteElementString ("MandatGuid", info.MandatGuid.ToString ());

			DataIO.SaveStatistics (writer, info.Statistics);

			writer.WriteEndElement ();
			writer.WriteEndDocument ();

			writer.Flush ();
			writer.Close ();
		}

		private static void SaveStatistics(System.Xml.XmlWriter writer, MandatStatistics statistics)
		{
			writer.WriteStartElement ("Statistics");

			writer.WriteElementString ("AssetsCount",     statistics.AssetsCount    .ToString (System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteElementString ("EventsCount",     statistics.EventsCount    .ToString (System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteElementString ("CategoriesCount", statistics.CategoriesCount.ToString (System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteElementString ("GroupsCount",     statistics.GroupsCount    .ToString (System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteElementString ("PersonsCount",    statistics.PersonsCount   .ToString (System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteElementString ("ReportsCount",    statistics.ReportsCount   .ToString (System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteElementString ("AccountsCount",   statistics.AccountsCount  .ToString (System.Globalization.CultureInfo.InvariantCulture));

			writer.WriteEndElement ();
		}


		public static void OpenMandat(DataAccessor accessor, string filename)
		{
			if (DataIO.ExistingXml (filename))
			{
				DataIO.OpenXml (accessor, filename);
			}
			else
			{
				DataIO.Decompress (filename);
				DataIO.OpenXml (accessor, filename);
				DataIO.DeleteXml (filename);
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
			DataIO.SaveInfo (filename, accessor.Mandat.MandatInfo);
			DataIO.SaveXml (accessor, filename);
			DataIO.Compress (filename);

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
			var xmlFilename = DataIO.GetXmlFilename (filename);
			var zipFilename = DataIO.GetZipFilename (filename);

			System.IO.File.Delete (zipFilename);
			Compression.GZipCompressFile (xmlFilename, zipFilename);
		}

		private static void Decompress(string filename)
		{
			var xmlFilename = DataIO.GetXmlFilename (filename);
			var zipFilename = DataIO.GetZipFilename (filename);

			System.IO.File.Delete (xmlFilename);
			Compression.GZipDecompressFile (zipFilename, xmlFilename);
		}


		private static bool ExistingXml(string filename)
		{
			var xmlFilename = DataIO.GetXmlFilename (filename);
			return System.IO.File.Exists (xmlFilename);
		}

		private static void DeleteXml(string filename)
		{
			var xmlFilename = DataIO.GetXmlFilename (filename);
			System.IO.File.Delete (xmlFilename);
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
