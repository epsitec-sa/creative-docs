using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// Cette classe enregistre les réglages globaux de Designer dans un fichier.
	/// </summary>
	public class Settings
	{
		public Settings()
		{
			this.modules = new List<ResourceModuleId>();
		}


		public List<ResourceModuleId> Modules
		{
			//	Liste des modules ouverts.
			get
			{
				return this.modules;
			}
		}


		public bool Write()
		{
			//	Ecrit le fichier des réglages globaux.
			try
			{
				File.WriteAllBytes(this.GlobalSettingsFilename, this.Serialize());
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool Read()
		{
			//	Lit le fichier des réglages globaux.
			try
			{
				this.Deserialize(File.ReadAllBytes(this.GlobalSettingsFilename));
				return true;
			}
			catch
			{
				return false;
			}
		}

		protected byte[] Serialize()
		{
			//	Retourne les données à sérialiser (texte xml).
			MemoryStream buffer = new MemoryStream();
			XmlTextWriter writer = new XmlTextWriter(buffer, System.Text.Encoding.UTF8);
			writer.Formatting = Formatting.Indented;

			this.WriteXml(writer);

			writer.Flush();
			writer.Close();
			return buffer.ToArray ();
		}

		protected void Deserialize(byte[] data)
		{
			//	Désérialise le texte xml.
			MemoryStream buffer = new MemoryStream(data);
			XmlTextReader reader = new XmlTextReader(buffer);
			
			this.ReadXml(reader);

			reader.Close();
		}


		protected void WriteXml(XmlWriter writer)
		{
			//	Génère les données xml.
			writer.WriteStartDocument();

			writer.WriteStartElement("Modules");
			foreach (ResourceModuleId module in this.modules)
			{
				writer.WriteStartElement("Module");
				writer.WriteElementString("ResourceModuleId", Types.InvariantConverter.ConvertToString(module));
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			
			writer.WriteEndDocument();
		}

		protected void ReadXml(XmlReader reader)
		{
			//	Analyse les données xml.
			this.modules.Clear();

			while (reader.ReadToFollowing("Module"))
			{
				reader.Read();

				while (true)
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						string name = reader.LocalName;
						string element = reader.ReadElementString();

						if (name == "ResourceModuleId")
						{
							ResourceModuleId module = Types.InvariantConverter.ConvertFromString<ResourceModuleId>(element);
							this.modules.Add(module);
						}
					}
					else if (reader.NodeType == XmlNodeType.EndElement)
					{
						System.Diagnostics.Debug.Assert(reader.Name == "Module");
						break;
					}
					else
					{
						reader.Read();
					}
				}
			}
		}


		protected string GlobalSettingsFilename
		{
			//	Retourne le nom du fichier des réglages de l'application.
			//	Le dossier est qq chose du genre:
			//	C:\Documents and Settings\Daniel Roux\Application Data\Epsitec\Crésus Designer
			//	C:\Users\Daniel Roux\AppData\Roaming\Epsitec\Crésus Designer (sous Vista)
			get
			{
				return string.Concat(Common.Support.Globals.Directories.UserAppData, "\\Designer.settings");
			}
		}


		protected List<ResourceModuleId> modules;
	}
}
