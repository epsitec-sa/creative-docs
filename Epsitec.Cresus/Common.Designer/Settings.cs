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
					Globals.Properties.SetProperty (AbstractResourceAccessor.DeveloperIdPropertyName, devId);
				}
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

			writer.WriteStartElement("Settings");
			
			writer.WriteStartElement("Modules");
			foreach (ResourceModuleId module in this.modules)
			{
				writer.WriteStartElement("Module");
				writer.WriteElementString("ResourceModuleId", Types.InvariantConverter.ConvertToString(module));
				writer.WriteEndElement();
			}
			writer.WriteEndElement();

			writer.WriteStartElement("Identity");
			writer.WriteElementString("UserName", this.identityCard == null ? "" : this.identityCard.UserName);
			writer.WriteEndElement();

			writer.WriteEndElement();
			
			writer.WriteEndDocument();
		}

		protected void ReadXml(XmlReader reader)
		{
			//	Analyse les données xml.
			this.modules.Clear();

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
								this.ReadXmlModules(reader);
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

		protected void ReadXmlModules(XmlReader reader)
		{
			reader.Read();

			while (true)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					string name = reader.LocalName;

					if (name == "Module")
					{
						this.ReadXmlModule(reader);
					}
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					System.Diagnostics.Debug.Assert(reader.Name == "Modules");
					reader.Read();
					break;
				}
				else
				{
					reader.Read();
				}
			}
		}

		protected void ReadXmlIdentity(XmlReader reader)
		{
			reader.Read ();

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


		protected void ReadXmlModule(XmlReader reader)
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
					reader.Read();
					break;
				}
				else
				{
					reader.Read();
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
		protected IdentityCard identityCard;
	}
}
