//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Data.Helpers;

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
			writer.WriteStartElement ("ComputerSettings");

			writer.WriteElementString ("DocumentVersion",  DataMandat.SerializationVersion);
			writer.WriteElementString ("SoftwareLanguage", this.SoftwareLanguage);
			writer.WriteElementString ("MandatDirectory",  this.MandatDirectory);
			writer.WriteElementString ("MandatFilename",   this.MandatFilename);
			this.SerializeWindowPlacement (writer, "WindowPlacement", this.WindowPlacement);

			writer.WriteEndElement ();
			writer.WriteEndDocument ();

			writer.Flush ();
			writer.Close ();
		}

		private void SerializeWindowPlacement(System.Xml.XmlWriter writer, string name, WindowPlacement windowPlacement)
		{
			writer.WriteStartElement (name);

			IOHelpers.WriteDecimalAttribute (writer, "Left",   (decimal) windowPlacement.Bounds.Left);
			IOHelpers.WriteDecimalAttribute (writer, "Bottom", (decimal) windowPlacement.Bounds.Bottom);
			IOHelpers.WriteDecimalAttribute (writer, "Width",  (decimal) windowPlacement.Bounds.Width);
			IOHelpers.WriteDecimalAttribute (writer, "Height", (decimal) windowPlacement.Bounds.Height);

			IOHelpers.WriteBoolAttribute (writer, "IsFullScreen", windowPlacement.IsFullScreen);
			IOHelpers.WriteBoolAttribute (writer, "IsMinimized",  windowPlacement.IsMinimized);
			IOHelpers.WriteBoolAttribute (writer, "IsHidden",     windowPlacement.IsHidden);

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
					if (reader.Name == "ComputerSettings")
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
						case "DocumentVersion":
							var version = reader.ReadElementContentAsString ();
							break;

						case "SoftwareLanguage":
							this.SoftwareLanguage = reader.ReadElementContentAsString ();
							break;

						case "MandatDirectory":
							this.MandatDirectory = reader.ReadElementContentAsString ();
							break;

						case "MandatFilename":
							this.MandatFilename = reader.ReadElementContentAsString ();
							break;

						case "WindowPlacement":
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
				(double) reader["Left"  ].ParseDecimal (),
				(double) reader["Bottom"].ParseDecimal (),
				(double) reader["Width" ].ParseDecimal (),
				(double) reader["Height"].ParseDecimal ());

			var windowPlacement = new WindowPlacement (bounds,
				isFullScreen: reader["IsFullScreen"].ParseBool (),
				isMinimized:  reader["IsMinimized" ].ParseBool (),
				isHidden:     reader["IsHidden"    ].ParseBool ());

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
