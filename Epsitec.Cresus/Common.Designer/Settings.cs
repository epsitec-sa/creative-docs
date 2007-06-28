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
			this.modules = new List<string>();
		}


		public List<string> Modules
		{
			get
			{
				return this.modules;
			}
		}


		public bool Write()
		{
			try
			{
				File.WriteAllText(this.GlobalSettingsFilename, this.Serialize());
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool Read()
		{
			try
			{
				this.Deserialize(File.ReadAllText(this.GlobalSettingsFilename));
				return true;
			}
			catch
			{
				return false;
			}
		}

		protected string Serialize()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();
			StringWriter stringWriter = new StringWriter(buffer);
			XmlTextWriter writer = new XmlTextWriter(stringWriter);
			writer.Formatting = Formatting.Indented;

			this.WriteXml(writer);

			writer.Flush();
			writer.Close();
			return buffer.ToString();
		}

		protected void Deserialize(string data)
		{
			StringReader stringReader = new StringReader(data);
			XmlTextReader reader = new XmlTextReader(stringReader);
			
			this.ReadXml(reader);

			reader.Close();
		}


		protected void WriteXml(XmlWriter writer)
		{
			writer.WriteStartDocument();

			writer.WriteStartElement("Modules");
			foreach (string module in this.modules)
			{
				writer.WriteStartElement("Module");
				writer.WriteElementString("Path", module);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			
			writer.WriteEndDocument();
		}

		protected void ReadXml(XmlReader reader)
		{
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

						if (name == "Path")
						{
							this.modules.Add(element);
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
				return string.Concat(Common.Support.Globals.Directories.UserAppData, "\\Designer.data");
			}
		}


		protected List<string> modules;
	}
}
