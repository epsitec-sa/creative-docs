//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.IO;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.Helpers;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	/// <summary>
	/// Cette classe centralise la gestion de la persistance des données.
	/// </summary>
	public static class DataIO
	{
		public static MandatInfo OpenInfo(string filename)
		{
			//	Lit le petit fichier d'informations, soit à partir du fichier xx.description.xml
			//	s'il existe, sinon à partir du fichier compressé.
			if (DataIO.ExistingInfo (filename))
			{
				var reader = System.Xml.XmlReader.Create (DataIO.GetInfoFilename (filename));
				return DataIO.OpenInfo (reader);
			}
			else
			{
				var zip = new ZipFile ();
				zip.LoadFile (filename);

				var data = zip["description.xml"].Data;
				var stream = new System.IO.MemoryStream (data);

				var reader = System.Xml.XmlReader.Create (stream);
				return DataIO.OpenInfo (reader);
			}
		}

		public static void OpenMandat(DataAccessor accessor, string filename,
			System.Action<System.Xml.XmlReader> localSettingsOpenAction)
		{
			//	Lit le mandat, soit à partir des fichiers xx.data.xml et xx.accounts.xml
			//	s'ils existent, sinon à partir du fichier compressé.

			//	On s'occupe de la partie data.
			if (DataIO.ExistingData (filename))
			{
				DataIO.OpenData (filename, accessor);  // lit directement le fichier xml
			}
			else
			{
				var zip = new ZipFile ();
				zip.LoadFile (filename);

				var data = zip["data.xml"].Data;
				var stream = new System.IO.MemoryStream (data);

				DataIO.OpenData (stream, accessor);
			}

			//	On s'occupe de la partie global settings.
			if (DataIO.ExistingGlobalSettings (filename))
			{
				DataIO.OpenGlobalSettings (filename, accessor);  // lit directement le fichier xml
			}
			else
			{
				var zip = new ZipFile ();
				zip.LoadFile (filename);

				var data = zip["globalsettings.xml"].Data;
				var stream = new System.IO.MemoryStream (data);

				DataIO.OpenGlobalSettings (stream, accessor);
			}

			//	On s'occupe de la partie accounts (plans comptables).
			if (DataIO.ExistingAccounts (filename))
			{
				DataIO.OpenAccounts (filename, accessor);  // lit directement le fichier xml
			}
			else
			{
				var zip = new ZipFile ();
				zip.LoadFile (filename);

				var data = zip["accounts.xml"].Data;
				var stream = new System.IO.MemoryStream (data);

				DataIO.OpenAccounts (stream, accessor);
			}

			//	On s'occupe de la partie local settings.
			if (DataIO.ExistingLocalSettings (filename))
			{
				DataIO.OpenLocalSettings (filename, localSettingsOpenAction);  // lit directement le fichier xml
			}
			else
			{
				var zip = new ZipFile ();
				zip.LoadFile (filename);

				var data = zip[DataIO.LocalSettingsZipPath].Data;  // "localsettings.user.xml"
				var stream = new System.IO.MemoryStream (data);

				DataIO.OpenLocalSettings (stream, localSettingsOpenAction);
			}
		}

		public static void SaveMandat(DataAccessor accessor, string filename, SaveMandatMode mode,
			System.Action<System.Xml.XmlWriter> localSettingsSaveAction)
		{
			//	Enregistre le mandat dans un fichier compressé. Selon le mode, on crée
			//	en plus les fichiers xx.description.xml, xx.data.xml et xx.accounts.xml.
			var zip = new ZipFile ();

			//	On s'occupe de la partie description.
			{
				var stream = new System.IO.MemoryStream ();
				DataIO.SaveInfo (stream, accessor.Mandat.MandatInfo);

				var data = new byte[stream.Length];
				stream.Position = 0;
				stream.Read (data, 0, (int) stream.Length);

				zip.AddEntry ("description.xml", data);
			}

			//	On s'occupe de la partie data.
			{
				var stream = new System.IO.MemoryStream ();
				DataIO.SaveData (stream, accessor);

				var data = new byte[stream.Length];
				stream.Position = 0;
				stream.Read (data, 0, (int) stream.Length);

				zip.AddEntry ("data.xml", data);
			}

			//	On s'occupe de la partie global settings.
			{
				var stream = new System.IO.MemoryStream ();
				DataIO.SaveGlobalSettings (stream, accessor);

				var data = new byte[stream.Length];
				stream.Position = 0;
				stream.Read (data, 0, (int) stream.Length);

				zip.AddEntry ("globalsettings.xml", data);
			}

			//	On s'occupe de la partie accounts (plans comptables).
			{
				var stream = new System.IO.MemoryStream ();
				DataIO.SaveAccounts (stream, accessor);

				var data = new byte[stream.Length];
				stream.Position = 0;
				stream.Read (data, 0, (int) stream.Length);

				zip.AddEntry ("accounts.xml", data);
			}

			//	On s'occupe de la partie local settings.
			if ((mode & SaveMandatMode.SaveUI) != 0)
			{
				var stream = new System.IO.MemoryStream ();
				DataIO.SaveLocalSettings (stream, localSettingsSaveAction);

				var data = new byte[stream.Length];
				stream.Position = 0;
				stream.Read (data, 0, (int) stream.Length);

				zip.AddEntry (DataIO.LocalSettingsZipPath, data);  // "localsettings.user.xml"
			}

			System.IO.File.Delete (filename);
			zip.SaveFile (filename);

			if ((mode & SaveMandatMode.KeepUnzip) != 0)  // pour le debug ?
			{
				//	On enregistre en plus tous les fichiers séparément.
				DataIO.SaveInfo           (filename, accessor.Mandat.MandatInfo);
				DataIO.SaveData           (filename, accessor);
				DataIO.SaveGlobalSettings (filename, accessor);
				DataIO.SaveAccounts       (filename, accessor);

				if ((mode & SaveMandatMode.SaveUI) != 0)
				{
					DataIO.SaveLocalSettings (filename, localSettingsSaveAction);
				}
			}
			else
			{
				//	Supprime les fichiers de debug, s'ils existent. C'est plus prudent,
				//	car ce sont eux qui sont lus en priorité.
				System.IO.File.Delete (DataIO.GetInfoFilename           (filename));
				System.IO.File.Delete (DataIO.GetDataFilename           (filename));
				System.IO.File.Delete (DataIO.GetGlobalSettingsFilename (filename));
				System.IO.File.Delete (DataIO.GetAccountsFilename       (filename));
				System.IO.File.Delete (DataIO.GetLocalSettingsFilename  (filename));
			}
		}


		#region Open informations
		private static MandatInfo OpenInfo(System.Xml.XmlReader reader)
		{
			//	Lit le petit fichier xml d'informations.
			var info = MandatInfo.Empty;

			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case "FileDescription":
							info = DataIO.OpenInfoDescription (reader);
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

		private static MandatInfo OpenInfoDescription(System.Xml.XmlReader reader)
		{
			string version          = null;
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
						case "DocumentVersion":
							version = reader.ReadElementContentAsString ();
							break;

						case "Software":
							softwareId       = reader["id"];
							softwareVersion  = reader["ver"];
							softwareLanguage = reader["lang"];
							break;

						case "File":
							fileName    = reader["name"];
							fileGuid    = reader["id"].ParseGuid ();
							fileVersion = reader["ver"];
							break;

						case "Templates":
							break;

						case "FileSources":
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
					if (reader.Name == "Summary")
					{
						string summary = reader["text"];
					}
					else if (reader.Name == "Data")
					{
						int i = IOHelpers.ParseInt (reader["value"]);

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
		#endregion


		#region Save informations
		private static void SaveInfo(string filename, MandatInfo info)
		{
			//	Ecrit le petit fichier xml d'informations.
			var settings = new System.Xml.XmlWriterSettings
			{
				Indent = true,
			};

			var writer = System.Xml.XmlWriter.Create (DataIO.GetInfoFilename (filename), settings);
			DataIO.SaveInfo (writer, info);
		}

		private static void SaveInfo(System.IO.MemoryStream stream, MandatInfo info)
		{
			//	Ecrit le petit fichier xml d'informations.
			var settings = new System.Xml.XmlWriterSettings
			{
				Indent = true,
			};

			var writer = System.Xml.XmlWriter.Create (stream, settings);
			DataIO.SaveInfo (writer, info);
		}

		private static void SaveInfo(System.Xml.XmlWriter writer, MandatInfo info)
		{
			writer.WriteStartDocument ();
			writer.WriteStartElement ("FileDescription");

			writer.WriteElementString ("DocumentVersion", DataMandat.DocumentVersion);

			writer.WriteStartElement    ("Software");
			writer.WriteAttributeString ("id",   info.SoftwareId);
			writer.WriteAttributeString ("ver",  info.SoftwareVersion);
			writer.WriteAttributeString ("lang", info.SoftwareLanguage);
			writer.WriteEndElement      ();

			writer.WriteStartElement    ("File");
			writer.WriteAttributeString ("name", info.FileName);
			writer.WriteAttributeString ("id",   info.FileGuid.ToStringIO ());
			writer.WriteAttributeString ("ver",  info.FileVersion);
			writer.WriteEndElement      ();

			writer.WriteStartElement    ("Templates");
			writer.WriteEndElement      ();

			writer.WriteStartElement    ("FileSources");
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

			writer.WriteStartElement ("Summary");
			writer.WriteAttributeString ("text", statistics.Summary);
			writer.WriteEndElement ();

			DataIO.SaveStatistics (writer, "AssetCount",    statistics.AssetCount);
			DataIO.SaveStatistics (writer, "EventCount",    statistics.EventCount);
			DataIO.SaveStatistics (writer, "CategoryCount", statistics.CategoryCount);
			DataIO.SaveStatistics (writer, "GroupCount",    statistics.GroupCount);
			DataIO.SaveStatistics (writer, "PersonCount",   statistics.PersonCount);
			DataIO.SaveStatistics (writer, "ReportCount",   statistics.ReportCount);
			DataIO.SaveStatistics (writer, "AccountCount",  statistics.AccountCount);

			writer.WriteEndElement ();
		}

		private static void SaveStatistics(System.Xml.XmlWriter writer, string name, int count)
		{
			writer.WriteStartElement ("Data");
			writer.WriteAttributeString ("name", name);
			writer.WriteAttributeString ("value", count.ToStringIO ());
			writer.WriteEndElement ();
		}
		#endregion


		#region Open data
		private static void OpenData(string filename, DataAccessor accessor)
		{
			var reader = System.Xml.XmlReader.Create (DataIO.GetDataFilename (filename));
			accessor.Mandat = new DataMandat (reader);
			reader.Close ();
		}

		private static void OpenData(System.IO.MemoryStream stream, DataAccessor accessor)
		{
			var reader = System.Xml.XmlReader.Create (stream);
			accessor.Mandat = new DataMandat (reader);
			reader.Close ();
		}
		#endregion


		#region Save data
		private static void SaveData(System.IO.MemoryStream stream, DataAccessor accessor)
		{
			var settings = new System.Xml.XmlWriterSettings
			{
				Indent = true,
			};

			var writer = System.Xml.XmlWriter.Create (stream, settings);
			accessor.Mandat.Serialize (writer);

			writer.Flush ();
			writer.Close ();
		}

		private static void SaveData(string filename, DataAccessor accessor)
		{
			var settings = new System.Xml.XmlWriterSettings
			{
				Indent = true,
			};

			var writer = System.Xml.XmlWriter.Create (DataIO.GetDataFilename (filename), settings);
			accessor.Mandat.Serialize (writer);

			writer.Flush ();
			writer.Close ();
		}
		#endregion


		#region Open global settings
		private static void OpenGlobalSettings(string filename, DataAccessor accessor)
		{
			var reader = System.Xml.XmlReader.Create (DataIO.GetGlobalSettingsFilename (filename));
			accessor.GlobalSettings.Deserialize (reader);
			reader.Close ();
		}

		private static void OpenGlobalSettings(System.IO.MemoryStream stream, DataAccessor accessor)
		{
			var reader = System.Xml.XmlReader.Create (stream);
			accessor.GlobalSettings.Deserialize (reader);
			reader.Close ();
		}
		#endregion


		#region Save global settings
		private static void SaveGlobalSettings(System.IO.MemoryStream stream, DataAccessor accessor)
		{
			var settings = new System.Xml.XmlWriterSettings
			{
				Indent = true,
			};

			var writer = System.Xml.XmlWriter.Create (stream, settings);
			accessor.GlobalSettings.Serialize (writer);

			writer.Flush ();
			writer.Close ();
		}

		private static void SaveGlobalSettings(string filename, DataAccessor accessor)
		{
			var settings = new System.Xml.XmlWriterSettings
			{
				Indent = true,
			};

			var writer = System.Xml.XmlWriter.Create (DataIO.GetGlobalSettingsFilename (filename), settings);
			accessor.GlobalSettings.Serialize (writer);

			writer.Flush ();
			writer.Close ();
		}
		#endregion


		#region Open local settings
		private static void OpenLocalSettings(string filename, System.Action<System.Xml.XmlReader> localSettingsOpenAction)
		{
			var reader = System.Xml.XmlReader.Create (DataIO.GetLocalSettingsFilename (filename));

			if (localSettingsOpenAction != null)
			{
				localSettingsOpenAction (reader);
			}

			reader.Close ();
		}

		private static void OpenLocalSettings(System.IO.MemoryStream stream, System.Action<System.Xml.XmlReader> localSettingsOpenAction)
		{
			var reader = System.Xml.XmlReader.Create (stream);

			if (localSettingsOpenAction != null)
			{
				localSettingsOpenAction (reader);
			}

			reader.Close ();
		}
		#endregion


		#region Save local settings
		private static void SaveLocalSettings(System.IO.MemoryStream stream, System.Action<System.Xml.XmlWriter> localSettingsSaveAction)
		{
			var settings = new System.Xml.XmlWriterSettings
			{
				Indent = true,
			};

			var writer = System.Xml.XmlWriter.Create (stream, settings);

			if (localSettingsSaveAction != null)
			{
				localSettingsSaveAction (writer);
			}

			writer.Flush ();
			writer.Close ();
		}

		private static void SaveLocalSettings(string filename, System.Action<System.Xml.XmlWriter> localSettingsSaveAction)
		{
			var settings = new System.Xml.XmlWriterSettings
			{
				Indent = true,
			};

			var writer = System.Xml.XmlWriter.Create (DataIO.GetLocalSettingsFilename (filename), settings);

			if (localSettingsSaveAction != null)
			{
				localSettingsSaveAction (writer);
			}

			writer.Flush ();
			writer.Close ();
		}
		#endregion


		#region Open accounts
		private static void OpenAccounts(string filename, DataAccessor accessor)
		{
			var reader = System.Xml.XmlReader.Create (DataIO.GetAccountsFilename (filename));
			accessor.Mandat.DeserializeAccounts (reader);
			reader.Close ();
		}

		private static void OpenAccounts(System.IO.MemoryStream stream, DataAccessor accessor)
		{
			var reader = System.Xml.XmlReader.Create (stream);
			accessor.Mandat.DeserializeAccounts (reader);
			reader.Close ();
		}
		#endregion


		#region Save accounts
		private static void SaveAccounts(System.IO.MemoryStream stream, DataAccessor accessor)
		{
			var settings = new System.Xml.XmlWriterSettings
			{
				Indent = true,
			};

			var writer = System.Xml.XmlWriter.Create (stream, settings);
			accessor.Mandat.SerializeAccounts (writer);

			writer.Flush ();
			writer.Close ();
		}

		private static void SaveAccounts(string filename, DataAccessor accessor)
		{
			var settings = new System.Xml.XmlWriterSettings
			{
				Indent = true,
			};

			var writer = System.Xml.XmlWriter.Create (DataIO.GetAccountsFilename (filename), settings);
			accessor.Mandat.SerializeAccounts (writer);

			writer.Flush ();
			writer.Close ();
		}
		#endregion


		private static string LocalSettingsZipPath
		{
			//	Retourne le nom du fichier à utiliser à l'intérieur du fichier compressé,
			//	pour les LocalSettings.
			//	Par exemple "localsettings.MacPro-2014-DR_Daniel.xml".
			get
			{
				var user = DataIO.MapToValidFilename (DataIO.CurrentUser);
				return string.Format ("localsettings.{0}.xml", user);
			}
		}

		private static string MapToValidFilename(string filename)
		{
			//	Remplace les caractères invalides d'un nom de fichier par des soulignés.
			//	Retourne un nom toujours valide.
			foreach (char c in System.IO.Path.GetInvalidFileNameChars ())
			{
				filename = filename.Replace (c, '_');
			}

			return filename;
		}

		private static string CurrentUser
		{
			//	Retourne le nom de l'utilisateur courant.
			//	"MacPro-2014-DR\Daniel" sur ma machine.
			get
			{
				try
				{
					return System.Security.Principal.WindowsIdentity.GetCurrent ().Name;
				}
				catch
				{
					return "anonymous";  // garde-fou
				}
			}
		}


		private static bool ExistingInfo(string filename)
		{
			return System.IO.File.Exists (DataIO.GetInfoFilename (filename));
		}

		private static bool ExistingData(string filename)
		{
			return System.IO.File.Exists (DataIO.GetDataFilename (filename));
		}

		private static bool ExistingAccounts(string filename)
		{
			return System.IO.File.Exists (DataIO.GetAccountsFilename (filename));
		}

		private static bool ExistingGlobalSettings(string filename)
		{
			return System.IO.File.Exists (DataIO.GetGlobalSettingsFilename (filename));
		}

		private static bool ExistingLocalSettings(string filename)
		{
			return System.IO.File.Exists (DataIO.GetLocalSettingsFilename (filename));
		}


		private static string GetInfoFilename(string filename)
		{
			return filename + ".description.xml";
		}

		private static string GetDataFilename(string filename)
		{
			return filename + ".data.xml";
		}

		private static string GetAccountsFilename(string filename)
		{
			return filename + ".accounts.xml";
		}

		private static string GetGlobalSettingsFilename(string filename)
		{
			return filename + ".globalsettings.xml";
		}

		private static string GetLocalSettingsFilename(string filename)
		{
			return string.Concat (filename, ".", DataIO.LocalSettingsZipPath);
		}
	}
}
