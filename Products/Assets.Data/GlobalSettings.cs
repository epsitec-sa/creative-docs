//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Helpers;
using Epsitec.Cresus.Assets.Data.Serialization;

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
			writer.WriteStartElement (X.GlobalSettings);

			writer.WriteElementString (X.DocumentVersion,  DataMandat.SerializationVersion);
			writer.WriteElementString (X.MandatLanguage,   this.MandatLanguage);
			writer.WriteElementString (X.SaveMandatMode,   this.SaveMandatMode.ToStringIO ());
			writer.WriteElementString (X.CopyNameStrategy, this.CopyNameStrategy.ToStringIO ());

			writer.WriteEndElement ();
		}

		public void Deserialize(System.Xml.XmlReader reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					if (reader.Name == X.GlobalSettings)
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
						case X.DocumentVersion:
							var version = reader.ReadElementContentAsString ();
							break;

						case X.MandatLanguage:
							this.MandatLanguage = reader.ReadElementContentAsString ();
							break;

						case X.SaveMandatMode:
							this.SaveMandatMode = reader.ReadElementContentAsString ().ParseType<SaveMandatMode> ();
							break;

						case X.CopyNameStrategy:
							this.CopyNameStrategy = reader.ReadElementContentAsString ().ParseType<CopyNameStrategy> ();
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
