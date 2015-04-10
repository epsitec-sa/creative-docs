//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.IO;
using Epsitec.Common.Support;
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
			filename = DataIO.PreprocessFilename (filename);

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
			filename = DataIO.PreprocessFilename (filename);

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

				var data = zip[DataIO.LocalSettingsZipPath].Data;  // "localsettings.Daniel.xml"

				//	Si on lit un fichier créé par un autre utilisateur, il ne contient pas les LocalSettings.
				//	Par exemple, si le créateur du fichier est Michael et que j'essaie (Daniel) d'ouvrir son
				//	fichier, le logiciel essaiera d'ouvrir "localsettings.daniel.xml", alors que le fichier
				//	ne contient que "localsettings.michael.xml". Dans ce cas, il suffit de ne rien faire, pour
				//	conserver les réglages par défaut. Si le fichierest ensuite enregistrer, il contiendra les
				//	réglages de Michael et Daniel. Chaque utilisateur trouvera ses propres réglages à
				//	l'ouverture.
				if (data != null)
				{
					var stream = new System.IO.MemoryStream (data);

					DataIO.OpenLocalSettings (stream, localSettingsOpenAction);
				}
			}
		}

		public static void SaveMandat(DataAccessor accessor, string filename, SaveMandatMode mode,
			System.Action<System.Xml.XmlWriter> localSettingsSaveAction)
		{
			//	Enregistre le mandat dans un fichier compressé. Selon le mode, on crée
			//	en plus les fichiers xx.description.xml, xx.data.xml et xx.accounts.xml.
			filename = DataIO.PreprocessFilename (filename);

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

				zip.AddEntry (DataIO.LocalSettingsZipPath, data);  // "localsettings.Daniel.xml"
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
						case X.FileDescription:
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
			string fileLanguage     = null;
			var    statistics       = MandatStatistics.Empty;

			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case X.DocumentVersion:
							version = reader.ReadElementContentAsString ();
							break;

						case X.Software:
							softwareId       = reader[X.Attr.Id];
							softwareVersion  = reader[X.Attr.Ver];
							softwareLanguage = reader[X.Attr.Lang];
							break;

						case X.File:
							fileName     = reader[X.Attr.Name];
							fileGuid     = reader[X.Attr.Id].ParseGuid ();
							fileVersion  = reader[X.Attr.Ver];
							fileLanguage = reader[X.Attr.Lang];
							break;

						case X.Templates:
							break;

						case X.FileSources:
							break;

						case X.About:
							statistics = DataIO.OpenStatistics (reader);
							break;
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}

			return new MandatInfo (softwareId, softwareVersion, softwareLanguage, fileName, fileGuid, fileVersion, fileLanguage, statistics);
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
					if (reader.Name == X.Summary)
					{
						string summary = reader[X.Attr.Text];
					}
					else if (reader.Name == X.Data)
					{
						int i = IOHelpers.ParseInt (reader[X.Attr.Value]);

						switch (reader[X.Attr.Name])
						{
							case X.AssetCount:
								assetCount = i;
								break;

							case X.EventCount:
								eventCount = i;
								break;

							case X.CategoryCount:
								categoryCount = i;
								break;

							case X.GroupCount:
								groupCount = i;
								break;

							case X.PersonCount:
								personCount = i;
								break;

							case X.ReportCount:
								reportCount = i;
								break;

							case X.AccountCount:
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
			writer.WriteStartElement (X.FileDescription);

			writer.WriteElementString (X.DocumentVersion, DataMandat.SerializationVersion);

			writer.WriteStartElement    (X.Software);
			writer.WriteAttributeString (X.Attr.Id,   info.SoftwareId);
			writer.WriteAttributeString (X.Attr.Ver,  info.SoftwareVersion);
			writer.WriteAttributeString (X.Attr.Lang, info.SoftwareLanguage);
			writer.WriteEndElement      ();

			writer.WriteStartElement    (X.File);
			writer.WriteAttributeString (X.Attr.Name, info.FileName);
			writer.WriteAttributeString (X.Attr.Id,   info.FileGuid.ToStringIO ());
			writer.WriteAttributeString (X.Attr.Ver,  info.FileVersion);
			writer.WriteAttributeString (X.Attr.Lang, info.FileLanguage);
			writer.WriteEndElement ();

			writer.WriteStartElement    (X.Templates);
			writer.WriteEndElement      ();

			writer.WriteStartElement    (X.FileSources);
			writer.WriteEndElement      ();

			DataIO.SaveStatistics (writer, info.Statistics);

			writer.WriteEndElement ();
			writer.WriteEndDocument ();

			writer.Flush ();
			writer.Close ();
		}

		private static void SaveStatistics(System.Xml.XmlWriter writer, MandatStatistics statistics)
		{
			writer.WriteStartElement (X.About);

			writer.WriteStartElement (X.Summary);
			writer.WriteAttributeString (X.Attr.Text, statistics.Summary);
			writer.WriteEndElement ();

			DataIO.SaveStatistics (writer, X.AssetCount,    statistics.AssetCount);
			DataIO.SaveStatistics (writer, X.EventCount,    statistics.EventCount);
			DataIO.SaveStatistics (writer, X.CategoryCount, statistics.CategoryCount);
			DataIO.SaveStatistics (writer, X.GroupCount,    statistics.GroupCount);
			DataIO.SaveStatistics (writer, X.PersonCount,   statistics.PersonCount);
			DataIO.SaveStatistics (writer, X.ReportCount,   statistics.ReportCount);
			DataIO.SaveStatistics (writer, X.AccountCount,  statistics.AccountCount);

			writer.WriteEndElement ();
		}

		private static void SaveStatistics(System.Xml.XmlWriter writer, string name, int count)
		{
			writer.WriteStartElement (X.Data);
			writer.WriteAttributeString (X.Attr.Name, name);
			writer.WriteAttributeString (X.Attr.Value, count.ToStringIO ());
			writer.WriteEndElement ();
		}
		#endregion


		#region Open data
		private static void OpenData(string filename, DataAccessor accessor)
		{
			var reader = System.Xml.XmlReader.Create (DataIO.GetDataFilename (filename));
			accessor.Mandat = new DataMandat (accessor.ComputerSettings, reader);
			reader.Close ();
		}

		private static void OpenData(System.IO.MemoryStream stream, DataAccessor accessor)
		{
			var reader = System.Xml.XmlReader.Create (stream);
			accessor.Mandat = new DataMandat (accessor.ComputerSettings, reader);
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
			accessor.Mandat.DeserializeAccountsAndCo (reader);
			reader.Close ();
		}

		private static void OpenAccounts(System.IO.MemoryStream stream, DataAccessor accessor)
		{
			var reader = System.Xml.XmlReader.Create (stream);
			accessor.Mandat.DeserializeAccountsAndCo (reader);
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
			accessor.Mandat.SerializeAccountsAndCo (writer);

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
			accessor.Mandat.SerializeAccountsAndCo (writer);

			writer.Flush ();
			writer.Close ();
		}
		#endregion


		private static string LocalSettingsZipPath
		{
			//	Retourne le nom du fichier à utiliser à l'intérieur du fichier compressé,
			//	pour les LocalSettings.
			//	Par exemple "localsettings.Daniel.xml".
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

		public static string CurrentUser
		{
			//	Retourne le nom de l'utilisateur courant (par exemple "Daniel").
			get
			{
				return System.Environment.UserName ?? "anonymous";
			}
		}


		private static string PreprocessFilename(string filename)
		{
			//	Complète un nom de fichier tout nu par le chemin "Mes documents" et l'extension ".crassets".
			if (!string.IsNullOrEmpty (filename))
			{
				//	On s'occupe du chemin d'accès.
				var dir = System.IO.Path.GetDirectoryName (filename);

				if (string.IsNullOrEmpty (dir))
				{
					FolderItem item = FileManager.GetFolderItem (FolderId.VirtualMyDocuments, FolderQueryMode.NoIcons);
					filename = System.IO.Path.Combine (item.FullPath, filename);
				}

				//	On s'occupe de l'extension.
				var ext = System.IO.Path.GetExtension (filename);

				if (string.IsNullOrEmpty (ext))
				{
					filename = filename + IOHelpers.Extension;
				}
			}

			return filename;
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
