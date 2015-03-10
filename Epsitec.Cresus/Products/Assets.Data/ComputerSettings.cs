//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Data.Helpers;
using Epsitec.Cresus.Assets.Data.Serialization;

namespace Epsitec.Cresus.Assets.Data
{
	/// <summary>
	/// C'est ici que sont concentrés tous les réglages globaux de l'application,
	/// non liés au mandat mais liés à l'utilisateur.
	/// </summary>
	public class ComputerSettings
	{
		public ComputerSettings()
		{
			this.SoftwareLanguage = "fr";

			FolderItem item = FileManager.GetFolderItem (FolderId.VirtualMyDocuments, FolderQueryMode.NoIcons);
			this.MandatDirectory = item.FullPath;

			this.Deserialize ();
		}


		public string							SoftwareLanguage;
		public string							MandatDirectory;
		public string							MandatFilename;
		public WindowPlacement					WindowPlacement;


		#region Serialize
		public void Serialize()
		{
			var settings = new System.Xml.XmlWriterSettings
			{
				Indent = true,
			};

			try
			{
				var writer = System.Xml.XmlWriter.Create (ComputerSettings.SettingsFilename, settings);
				this.Serialize (writer);
			}
			catch
			{
			}
		}

		private void Serialize(System.Xml.XmlWriter writer)
		{
			writer.WriteStartDocument ();
			writer.WriteStartElement (X.ComputerSettings);

			writer.WriteElementString (X.DocumentVersion,  DataMandat.SerializationVersion);
			writer.WriteElementString (X.SoftwareLanguage, this.SoftwareLanguage);
			writer.WriteElementString (X.MandatDirectory,  this.MandatDirectory);
			writer.WriteElementString (X.MandatFilename,   this.MandatFilename);
			this.SerializeWindowPlacement (writer, X.WindowPlacement, this.WindowPlacement);

			writer.WriteEndElement ();
			writer.WriteEndDocument ();

			writer.Flush ();
			writer.Close ();
		}

		private void SerializeWindowPlacement(System.Xml.XmlWriter writer, string name, WindowPlacement windowPlacement)
		{
			writer.WriteStartElement (name);

			writer.WriteDecimalAttribute (X.Attr.Left,   (decimal) windowPlacement.Bounds.Left);
			writer.WriteDecimalAttribute (X.Attr.Bottom, (decimal) windowPlacement.Bounds.Bottom);
			writer.WriteDecimalAttribute (X.Attr.Width,  (decimal) windowPlacement.Bounds.Width);
			writer.WriteDecimalAttribute (X.Attr.Height, (decimal) windowPlacement.Bounds.Height);

			writer.WriteBoolAttribute (X.Attr.IsFullScreen, windowPlacement.IsFullScreen);
			writer.WriteBoolAttribute (X.Attr.IsMinimized,  windowPlacement.IsMinimized);
			writer.WriteBoolAttribute (X.Attr.IsHidden,     windowPlacement.IsHidden);

			writer.WriteEndElement ();
		}
		#endregion


		#region Deserialize
		private void Deserialize()
		{
			try
			{
				var reader = System.Xml.XmlReader.Create (ComputerSettings.SettingsFilename);
				this.Deserialize (reader);
			}
			catch
			{
			}
		}

		private void Deserialize(System.Xml.XmlReader reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					if (reader.Name == X.ComputerSettings)
					{
						this.DeserializeComputerSettings (reader);
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}

		private void DeserializeComputerSettings(System.Xml.XmlReader reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case X.DocumentVersion:
							var version = reader.ReadElementContentAsString ();
							break;

						case X.SoftwareLanguage:
							this.SoftwareLanguage = reader.ReadElementContentAsString ();
							break;

						case X.MandatDirectory:
							this.MandatDirectory = reader.ReadElementContentAsString ();
							break;

						case X.MandatFilename:
							this.MandatFilename = reader.ReadElementContentAsString ();
							break;

						case X.WindowPlacement:
							this.WindowPlacement = this.DeserializeWindowPlacement (reader);
							break;
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}

		private WindowPlacement DeserializeWindowPlacement(System.Xml.XmlReader reader)
		{
			var bounds = new Rectangle (
				(double) reader[X.Attr.Left  ].ParseDecimal (),
				(double) reader[X.Attr.Bottom].ParseDecimal (),
				(double) reader[X.Attr.Width ].ParseDecimal (),
				(double) reader[X.Attr.Height].ParseDecimal ());

			var windowPlacement = new WindowPlacement (bounds,
				isFullScreen: reader[X.Attr.IsFullScreen].ParseBool (),
				isMinimized:  reader[X.Attr.IsMinimized ].ParseBool (),
				isHidden:     reader[X.Attr.IsHidden    ].ParseBool ());

			reader.Read ();  // on avance

			return windowPlacement;
		}
		#endregion


		private static string					SettingsFilename
		{
			//	Retourne le chemin du fichier des ComputerSettings, du genre:
			//	"C:\Users\Daniel\AppData\Roaming\Epsitec\Crésus Assets\Crésus.Assets.ApplicationSettings.settings"
			get
			{
				var dir = Epsitec.Common.Support.Globals.Directories.UserAppData;
				return System.IO.Path.Combine (dir, "Crésus.Assets.ApplicationSettings.settings");
			}
		}
	}
}
