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

			while (reader.ReadToFollowing(Xml.FieldDescription))
			{
				FieldDescription field = new FieldDescription(FieldDescription.FieldType.Field);
				field.ReadXml(reader);
				this.fields.Add(field);
			}

			// TODO: désérialiser this.entityId !
		}
		#endregion


		protected Druid entityId;
		protected List<FieldDescription> fields;
	}
}
