//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;
using Epsitec.Cresus.Assets.Data.Helpers;

namespace Epsitec.Cresus.Assets.Data
{
	public class GlobalSettings
	{
		public GlobalSettings(UndoManager undoManager)
		{
			this.undoManager = undoManager;

			this.Language = "fr";

			FolderItem item = FileManager.GetFolderItem (FolderId.VirtualMyDocuments, FolderQueryMode.NoIcons);
			var filename = System.IO.Path.Combine (item.FullPath, "default" + IOHelpers.Extension);
			this.MandatFilename = filename;

			this.SaveMandatMode = SaveMandatMode.SaveUI | SaveMandatMode.KeepUnzip;
			this.CopyNameStrategy = CopyNameStrategy.NameBracketCopy;
		}


		#region Serialize
		public void Serialize(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement ("GlobalSettings");

			writer.WriteElementString ("Language",         this.Language);
			writer.WriteElementString ("MandatFilename",   this.MandatFilename);
			writer.WriteElementString ("SaveMandatMode",   this.SaveMandatMode.ToStringIO ());
			writer.WriteElementString ("CopyNameStrategy", this.CopyNameStrategy.ToStringIO ());

			writer.WriteEndElement ();
		}

		public void Deserialize(System.Xml.XmlReader reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					if (reader.Name == "GlobalSettings")
					{
						this.DeserializeGlobalSettings (reader);
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}

		private void DeserializeGlobalSettings(System.Xml.XmlReader reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case "Language":
							this.Language = reader.ReadElementContentAsString ();
							break;

						case "MandatFilename":
							this.MandatFilename = reader.ReadElementContentAsString ();
							break;

						case "SaveMandatMode":
							this.SaveMandatMode = (SaveMandatMode) reader.ReadElementContentAsString ().ParseType (typeof (SaveMandatMode));
							break;

						case "CopyNameStrategy":
							this.CopyNameStrategy = (CopyNameStrategy) reader.ReadElementContentAsString ().ParseType (typeof (CopyNameStrategy));
							break;
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}
		#endregion


		public string							Language;
		public string							MandatFilename;
		public SaveMandatMode					SaveMandatMode;
		public CopyNameStrategy					CopyNameStrategy;


		private readonly UndoManager			undoManager;
	}
}
