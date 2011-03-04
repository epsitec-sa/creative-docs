using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.ResourceAccessors;
using Epsitec.Common.Drawing;
using Epsitec.Common.Identity;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// Cette classe enregistre les r�glages globaux de Designer dans un fichier.
	/// </summary>
	public class Settings
	{
		private Settings()
		{
			this.modules = new List<ResourceModuleId>();
			this.saveAllImageParameters = new Dictionary<string, string> ();
		}


		public List<ResourceModuleId> Modules
		{
			//	Liste des modules ouverts.
			get
			{
				return this.modules;
			}
		}

		public IdentityCard IdentityCard
		{
			get
			{
				return this.identityCard;
			}
			set
			{
				if (this.identityCard != value)
				{
					this.identityCard = value;
					int devId = (value == null) ? -1 : value.DeveloperId;
					Globals.Properties.SetProperty(AbstractResourceAccessor.DeveloperIdPropertyName, devId);
				}
			}
		}

		public int DeveloperId
		{
			get
			{
				if (this.identityCard == null)
				{
					return -1;
				}
				else
				{
					return this.identityCard.DeveloperId;
				}
			}
		}

		public string GetSaveAllImagesData(string moduleName)
		{
			if (this.saveAllImageParameters.ContainsKey (moduleName))
			{
				return this.saveAllImageParameters[moduleName];
			}
			else
			{
				return null;
			}
		}

		public void SetSaveAllImagesData(string moduleName, string data)
		{
			this.saveAllImageParameters[moduleName] = data;
		}

		public static readonly Settings Default = new Settings ();


		public bool Write()
		{
			//	Ecrit le fichier des r�glages globaux.
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
			//	Lit le fichier des r�glages globaux.
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

		private byte[] Serialize()
		{
			//	Retourne les donn�es � s�rialiser (texte xml).
			MemoryStream buffer = new MemoryStream();
			XmlTextWriter writer = new XmlTextWriter(buffer, System.Text.Encoding.UTF8);
			writer.Formatting = Formatting.Indented;

			this.WriteXml(writer);

			writer.Flush();
			writer.Close();
			return buffer.ToArray();
		}

		private void Deserialize(byte[] data)
		{
			//	D�s�rialise le texte xml.
			MemoryStream buffer = new MemoryStream(data);
			XmlTextReader reader = new XmlTextReader(buffer);
			
			this.ReadXml(reader);

			reader.Close();
		}


		private void WriteXml(XmlWriter writer)
		{
			//	G�n�re les donn�es xml.
			writer.WriteStartDocument();

			writer.WriteStartElement("Settings");
			
			if (this.modules.Count > 0)
			{
				writer.WriteStartElement("Modules");
				foreach (ResourceModuleId module in this.modules)
				{
					writer.WriteStartElement("Module");
					writer.WriteElementString("ResourceModuleId", Types.InvariantConverter.ConvertToString(module));
					writer.WriteEndElement();
				}
				writer.WriteEndElement();
			}

			if (this.saveAllImageParameters.Count > 0)
			{
				writer.WriteStartElement ("SaveAllImageParameters");
				foreach (var pair in this.saveAllImageParameters)
				{
					writer.WriteStartElement ("SaveAllImageParameter");
					writer.WriteElementString ("ModuleName", pair.Key);
					writer.WriteElementString ("Parameters", pair.Value);
					writer.WriteEndElement ();
				}
				writer.WriteEndElement ();
			}

			writer.WriteStartElement("Identity");
			writer.WriteElementString("UserName", this.identityCard == null ? "" : this.identityCard.UserName);
			writer.WriteEndElement();

			writer.WriteEndElement();
			
			writer.WriteEndDocument();
		}

		private void ReadXml(XmlReader reader)
		{
			//	Analyse les donn�es xml.
			this.modules.Clear();
			this.saveAllImageParameters.Clear ();

			while (reader.ReadToFollowing("Settings"))
			{
				reader.Read();

				while (true)
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						string name = reader.LocalName;

						switch (name)
						{
							case "Modules":
								this.ReadXmlModules (reader);
								break;

							case "SaveAllImageParameters":
								this.ReadXmlSaveAllImageParameters (reader);
								break;

							case "Identity":
								this.ReadXmlIdentity(reader);
								break;

							default:
								throw new System.FormatException();
						}
					}
					else if (reader.NodeType == XmlNodeType.EndElement)
					{
						System.Diagnostics.Debug.Assert(reader.Name == "Settings");
						reader.Read();
						break;
					}
					else
					{
						reader.Read();
					}
				}
			}
		}

		private void ReadXmlModules(XmlReader reader)
		{
			reader.Read ();

			while (true)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					string name = reader.LocalName;

					if (name == "Module")
					{
						this.ReadXmlModule (reader);
					}
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					System.Diagnostics.Debug.Assert (reader.Name == "Modules");
					reader.Read ();
					break;
				}
				else
				{
					reader.Read ();
				}
			}
		}

		private void ReadXmlSaveAllImageParameters(XmlReader reader)
		{
			reader.Read ();

			while (true)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					string name = reader.LocalName;

					if (name == "SaveAllImageParameter")
					{
						this.ReadXmlSaveAllImageParameter (reader);
					}
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					System.Diagnostics.Debug.Assert (reader.Name == "SaveAllImageParameters");
					reader.Read ();
					break;
				}
				else
				{
					reader.Read ();
				}
			}
		}

		private void ReadXmlIdentity(XmlReader reader)
		{
			reader.Read();

			while (true)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					string name = reader.LocalName;
					string element = reader.ReadElementString();

					if (name == "UserName")
					{
						this.IdentityCard = IdentityRepository.Default.FindIdentityCard(element);
					}
				}
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					System.Diagnostics.Debug.Assert(reader.Name == "Identity");
					reader.Read();
					break;
				}
				else
				{
					reader.Read();
				}
			}
		}

		private void ReadXmlModule(XmlReader reader)
		{
			reader.Read ();

			while (true)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					string name = reader.LocalName;
					string element = reader.ReadElementString ();

					if (name == "ResourceModuleId")
					{
						ResourceModuleId module = Types.InvariantConverter.ConvertFromString<ResourceModuleId> (element);

						if (System.IO.Directory.Exists (module.Path))
						{
							this.modules.Add (module);
						}
					}
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					System.Diagnostics.Debug.Assert (reader.Name == "Module");
					reader.Read ();
					break;
				}
				else
				{
					reader.Read ();
				}
			}
		}

		private void ReadXmlSaveAllImageParameter(XmlReader reader)
		{
			reader.Read ();

			string moduleName = null;
			string parameters = null;

			while (true)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					string name = reader.LocalName;
					string element = reader.ReadElementString ();

					if (name == "ModuleName")
					{
						moduleName = element;
					}

					if (name == "Parameters")
					{
						parameters = element;
					}
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					System.Diagnostics.Debug.Assert (reader.Name == "SaveAllImageParameter");
					reader.Read ();
					break;
				}
				else
				{
					reader.Read ();
				}
			}

			if (moduleName != null && parameters != null)
			{
				this.saveAllImageParameters.Add (moduleName, parameters);
			}
		}

		private string GlobalSettingsFilename
		{
			//	Retourne le nom du fichier des r�glages de l'application.
			//	Le dossier est qq chose du genre:
			//	C:\Documents and Settings\Daniel Roux\Application Data\Epsitec\Cr�sus Designer
			//	C:\Users\Daniel Roux\AppData\Roaming\Epsitec\Cr�sus Designer (sous Vista)
			get
			{
				return string.Concat(Common.Support.Globals.Directories.UserAppData, "\\Designer.settings");
			}
		}


		private List<ResourceModuleId> modules;
		private IdentityCard identityCard;
		private Dictionary<string, string> saveAllImageParameters;
	}
}
