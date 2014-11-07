//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;
using Epsitec.Cresus.Assets.Data.Helpers;

namespace Epsitec.Cresus.Assets.Data
{
	/// <summary>
	/// C'est ici que sont concentrés tous les réglages globaux de l'application liés
	/// au mandat, mais non liés à l'utilisateur ni à la UI.
	/// </summary>
	public class GlobalSettings
	{
		public GlobalSettings(UndoManager undoManager)
		{
			this.undoManager = undoManager;

			this.MandatLanguage = "fr";

			this.SaveMandatMode = SaveMandatMode.SaveUI;
			this.CopyNameStrategy = CopyNameStrategy.NameBracketCopy;
		}


		#region Serialize
		public void Serialize(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement ("GlobalSettings");

			writer.WriteElementString ("DocumentVersion",  DataMandat.DocumentVersion);
			writer.WriteElementString ("MandatLanguage",   this.MandatLanguage);
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
						case "DocumentVersion":
							var version = reader.ReadElementContentAsString ();
							break;

						case "MandatLanguage":
							this.MandatLanguage = reader.ReadElementContentAsString ();
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


		public string							MandatLanguage;
		public SaveMandatMode					SaveMandatMode;
		public CopyNameStrategy					CopyNameStrategy;


		private readonly UndoManager			undoManager;
	}
}
