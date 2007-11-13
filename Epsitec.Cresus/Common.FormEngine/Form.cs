using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

namespace Epsitec.Common.FormEngine
{
	/// <summary>
	/// Description d'un formulaire.
	/// </summary>
	public class Form
	{
		public Form()
		{
			this.entityId = Druid.Empty;
			this.fields = new List<FieldDescription>();
		}

		public Druid EntityId
		{
			get
			{
				return this.entityId;
			}
			set
			{
				this.entityId = value;
			}
		}

		public List<FieldDescription> Fields
		{
			get
			{
				return this.fields;
			}
		}

		public bool Compare(Form form)
		{
			if (this.entityId != form.entityId)
			{
				return false;
			}

			if (this.fields.Count != form.fields.Count)
			{
				return false;
			}

			for (int i=0; i<this.fields.Count; i++)
			{
				FieldDescription f1 = this.fields[i];
				FieldDescription f2 = form.fields[i];
				if (!f1.Compare(f2))
				{
					return false;
				}
			}

			return true;
		}


		#region Serialization
		public string Serialize()
		{
			//	Sérialise le masque et retourne le résultat dans un string.
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();
			System.IO.StringWriter stringWriter = new System.IO.StringWriter(buffer);
			XmlTextWriter writer = new XmlTextWriter(stringWriter);
			writer.Formatting = Formatting.None;

			this.WriteXml(writer);

			writer.Flush();
			writer.Close();
			return buffer.ToString();
		}

		public void Deserialize(string data)
		{
			//	Désérialise le masque à partir d'un string de données.
			System.IO.StringReader stringReader = new System.IO.StringReader(data);
			XmlTextReader reader = new XmlTextReader(stringReader);
			
			this.ReadXml(reader);

			reader.Close();
		}

		protected void WriteXml(XmlWriter writer)
		{
			//	Sérialise tout le masque.
			writer.WriteStartDocument();

			writer.WriteStartElement(Xml.Form);
			writer.WriteElementString(Xml.EntityId, this.entityId.ToString());
			foreach (FieldDescription field in this.fields)
			{
				field.WriteXml(writer);
			}
			writer.WriteEndElement();
			
			writer.WriteEndDocument();
		}

		protected void ReadXml(XmlReader reader)
		{
			//	Désérialise tout le masque.
			this.fields.Clear();

			reader.Read();

			//	TODO: attention, la logique de désérialisation est certainement fausse !
			while (true)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					string name = reader.LocalName;

					if (name == Xml.Form)
					{
						reader.Read();
					}
					else if (name == Xml.FieldDescription)
					{
						FieldDescription field = new FieldDescription(reader);
						this.fields.Add(field);
					}
					else
					{
						string element = reader.ReadElementString();

						if (name == Xml.EntityId)
						{
							this.entityId = Druid.Parse(element);
						}
						else
						{
							throw new System.NotSupportedException(string.Format("Unexpected XML node {0} found in FieldDescription", name));
						}
					}
				}
				else if (reader.NodeType == XmlNodeType.None)
				{
					break;
				}
				else
				{
					reader.Read();
				}
			}
			
#if false
			while (reader.ReadToFollowing(Xml.FieldDescription))
			{
				FieldDescription field = new FieldDescription(FieldDescription.FieldType.Field);
				field.ReadXml(reader);
				this.fields.Add(field);
			}

			// TODO: désérialiser this.entityId !
#endif
		}
		#endregion


		protected Druid entityId;
		protected List<FieldDescription> fields;
	}
}
