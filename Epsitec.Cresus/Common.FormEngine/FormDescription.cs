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
	/// Description complète d'un masque de saisie.
	/// </summary>
	public sealed class FormDescription : System.IEquatable<FormDescription>
	{
		public FormDescription() : this(Druid.Empty, Druid.Empty)
		{
		}

		public FormDescription(Druid entityId, Druid formIdToPatch)
		{
			this.entityId = entityId;
			this.formIdToPatch = formIdToPatch;
			this.fields = new List<FieldDescription>();
		}

		public FormDescription(FormDescription model) : this(model.EntityId, model.FormIdToPatch)
		{
			this.fields.AddRange(model.Fields);
		}

		public Druid EntityId
		{
			//	Druid de l'entité de base du masque de saisie, utilisé pour un masque normal/patch.
			get
			{
				return this.entityId;
			}
		}

		public Druid FormIdToPatch
		{
			//	Druid du masque de base à patcher.
			get
			{
				return this.formIdToPatch;
			}
		}

		public List<FieldDescription> Fields
		{
			//	Liste des champs, séparateurs, etc.
			get
			{
				return this.fields;
			}
		}

		public bool IsPatch
		{
			//	Indique s'il s'agit d'un formulaire de patch.
			get
			{
				return !this.formIdToPatch.IsEmpty;
			}
		}


		#region IEquatable<FormDescription> Members

		public bool Equals(FormDescription other)
		{
			return FormDescription.Equals(this, other);
		}

		#endregion
		
		public override bool Equals(object obj)
		{
			return (obj is FormDescription) && (FormDescription.Equals(this, (FormDescription) obj));
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public static bool Equals(FormDescription a, FormDescription b)
		{
			//	Retourne true si les deux objets sont égaux.
			if ((a == null) != (b == null))
			{
				return false;
			}

			if (a == null && b == null)
			{
				return true;
			}

			if (a.entityId != b.entityId)
			{
				return false;
			}

			if (a.formIdToPatch != b.formIdToPatch)
			{
				return false;
			}

			if (a.fields.Count != b.fields.Count)
			{
				return false;
			}

			for (int i=0; i<a.fields.Count; i++)
			{
				FieldDescription f1 = a.fields[i];
				FieldDescription f2 = b.fields[i];
				if (!f1.Equals(f2))
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
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.OmitXmlDeclaration = true;
			settings.Indent = false;
			XmlWriter writer = XmlWriter.Create(stringWriter, settings);
			
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

		private void WriteXml(XmlWriter writer)
		{
			//	Sérialise tout le masque.
			writer.WriteStartDocument();

			writer.WriteStartElement(Xml.Form);
			writer.WriteElementString(Xml.EntityId, this.entityId.ToString());
			writer.WriteElementString(Xml.FormIdToPatch, this.formIdToPatch.ToString());
			foreach (FieldDescription field in this.fields)
			{
				field.WriteXml(writer);
			}
			writer.WriteEndElement();
			
			writer.WriteEndDocument();
		}

		private void ReadXml(XmlReader reader)
		{
			//	Désérialise tout le masque.
			this.fields.Clear();

			reader.Read();

			//	TODO: attention, la logique de désérialisation est fausse, mais ça marche provisoirement !
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
						else if (name == Xml.FormIdToPatch)
						{
							this.formIdToPatch = Druid.Parse(element);
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


		private Druid							entityId;
		private Druid							formIdToPatch;
		private readonly List<FieldDescription>	fields;
	}
}
