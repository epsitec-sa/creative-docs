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
	/// D�crit un noeud, un champ ou un s�parateur dans un masque de saisie.
	/// </summary>
	public class FieldDescription : System.IEquatable<FieldDescription>
	{
		public enum FieldType
		{
			Field			= 1,	// champ
			Node			= 2,	// noeud
			Line			= 10,	// s�parateur trait horizontal
			Title			= 11,	// s�parateur titre automatique
			BoxBegin		= 20,	// d�but d'une bo�te
			BoxEnd			= 21,	// fin d'une bo�te
			InsertionPoint	= 30,	// sp�cifie le point d'insertion, dans un module de patch
			Hide			= 31,	// champ du module de r�f�rence � cacher, dans un module de patch
		}

		public enum SeparatorType
		{
			Normal			= 0,	// les champs sont proches
			Compact			= 1,	// les champs se touchent (chevauchement d'un pixel)
			Extend			= 2,	// les champs sont tr�s espac�s
			Append			= 3,	// le champ suivant sera sur la m�me ligne
		}


		protected FieldDescription()
		{
			//	Constructeur prot�g�, commun � tous les autres.
			this.backColor = Color.Empty;
			this.separator = SeparatorType.Normal;
			this.columnsRequired = FormEngine.MaxColumnsRequired;
			this.rowsRequired = 1;
			this.containerLayoutMode = ContainerLayoutMode.None;
			this.containerMargins = Margins.Zero;
			this.containerPadding = Margins.Zero;
			this.containerBackColor = Color.Empty;
			this.containerFrameState = FrameState.None;
			this.containerFrameWidth = 1;
		}

		public FieldDescription(FieldType type) : this()
		{
			//	Constructeur.
			//	Le type est toujours d�termin� � la cr�ation. Il ne pourra plus changer par la suite.
			this.guid = System.Guid.NewGuid();
			this.type = type;
		}

		public FieldDescription(FieldDescription model) : this()
		{
			//	Constructeur copiant une instance existante.
			this.guid = model.guid;
			this.type = model.type;
			this.backColor = model.backColor;
			this.separator = model.separator;
			this.columnsRequired = model.columnsRequired;
			this.rowsRequired = model.rowsRequired;
			this.nodeDescription = model.nodeDescription;
			this.fieldIds = model.fieldIds;
			this.containerLayoutMode = model.containerLayoutMode;
			this.containerMargins = model.containerMargins;
			this.containerPadding = model.containerPadding;
			this.containerBackColor = model.containerBackColor;
			this.containerFrameState = model.containerFrameState;
			this.containerFrameWidth = model.containerFrameWidth;
		}

		public FieldDescription(XmlReader reader) : this()
		{
			//	Constructeur qui d�s�rialise.
			this.ReadXml(reader);
		}


		public System.Guid Guid
		{
			//	Retourne l'identificateur unique.
			get
			{
				return this.guid;
			}
		}

		public FieldType Type
		{
			//	Retourne le type immuable de cet �l�ment.
			get
			{
				return this.type;
			}
		}


		public void SetNode(List<FieldDescription> descriptions)
		{
			//	Donne la liste des descriptions du noeud.
			System.Diagnostics.Debug.Assert(this.type == FieldType.Node);

			this.nodeDescription = new List<FieldDescription>();
			foreach (FieldDescription description in descriptions)
			{
				this.nodeDescription.Add(description);
			}
		}

		public List<FieldDescription> NodeDescription
		{
			//	Retourne la liste des descriptions du noeud.
			get
			{
				return this.nodeDescription;
			}
		}


		public void SetFields(string listDruids)
		{
			//	Donne d'une liste de Druids s�par�s par des points.
			//	Par exemple: listDruids = "[630B2].[630S2]"
			if (string.IsNullOrEmpty(listDruids))
			{
				this.fieldIds = null;
			}
			else
			{
				System.Diagnostics.Debug.Assert(this.type == FieldType.Field);

				this.fieldIds = new List<Druid>();
				string[] druids = listDruids.Split('.');
				foreach (string druid in druids)
				{
					Druid id = Druid.Parse(druid);
					this.fieldIds.Add(id);
				}
			}
		}

		public List<Druid> FieldIds
		{
			//	Liste des Druids qui repr�sentent le champ.
			get
			{
				return this.fieldIds;
			}
		}

		public string GetPath(string prefix)
		{
			//	Retourne le chemin permettant d'acc�der au champ.
			//	Par exemple, si prefix = "Data": retourne "Data.[630B2].[630S2]"
			if (this.type == FieldType.Field)
			{
				System.Text.StringBuilder builder = new System.Text.StringBuilder();

				if (prefix != null)
				{
					builder.Append(prefix);
				}

				foreach (Druid druid in this.fieldIds)
				{
					if (builder.Length > 0)
					{
						builder.Append(".");
					}

					builder.Append(druid);
				}

				return builder.ToString();
			}
			else
			{
				return null;
			}
		}


		public SeparatorType Separator
		{
			//	Type de s�paration apr�s le champ suivant.
			get
			{
				return this.separator;
			}
			set
			{
				this.separator = value;
			}
		}

		public Color BackColor
		{
			//	Couleur de fond pour le champ.
			get
			{
				return this.backColor;
			}
			set
			{
				this.backColor = value;
			}
		}

		public int ColumnsRequired
		{
			//	Nombre de colonnes requises [1..10].
			get
			{
				return this.columnsRequired;
			}
			set
			{
				value = System.Math.Max(value, 1);
				value = System.Math.Min(value, FormEngine.MaxColumnsRequired);
				this.columnsRequired = value;
			}
		}

		public int RowsRequired
		{
			//	Nombre de lignes requises [1..20].
			get
			{
				return this.rowsRequired;
			}
			set
			{
				value = System.Math.Max(value, 1);
				value = System.Math.Min(value, FormEngine.MaxRowsRequired);
				this.rowsRequired = value;
			}
		}

		public ContainerLayoutMode ContainerLayoutMode
		{
			//	Mode de layout pour les bo�tes contenues dans un BoxBegin.
			get
			{
				return this.containerLayoutMode;
			}
			set
			{
				System.Diagnostics.Debug.Assert(this.type == FieldType.BoxBegin);
				this.containerLayoutMode = value;
			}
		}

		public Margins ContainerMargins
		{
			//	Marges d'une bo�te.
			get
			{
				return this.containerMargins;
			}
			set
			{
				System.Diagnostics.Debug.Assert(this.type == FieldType.BoxBegin);
				this.containerMargins = value;
			}
		}

		public Margins ContainerPadding
		{
			//	Padding d'une bo�te.
			get
			{
				return this.containerPadding;
			}
			set
			{
				System.Diagnostics.Debug.Assert(this.type == FieldType.BoxBegin);
				this.containerPadding = value;
			}
		}

		public Color ContainerBackColor
		{
			//	Couleur de fond d'une bo�te.
			get
			{
				return this.containerBackColor;
			}
			set
			{
				System.Diagnostics.Debug.Assert(this.type == FieldType.BoxBegin);
				this.containerBackColor = value;
			}
		}

		public FrameState ContainerFrameState
		{
			//	Bordures d'une bo�te.
			get
			{
				return this.containerFrameState;
			}
			set
			{
				System.Diagnostics.Debug.Assert(this.type == FieldType.BoxBegin);
				this.containerFrameState = value;
			}
		}

		public double ContainerFrameWidth
		{
			//	Epaisseur des bordures d'une bo�te.
			get
			{
				return this.containerFrameWidth;
			}
			set
			{
				System.Diagnostics.Debug.Assert(this.type == FieldType.BoxBegin);
				this.containerFrameWidth = value;
			}
		}


		#region IEquatable<FieldDescription> Members

		public bool Equals(FieldDescription other)
		{
			return FieldDescription.Equals(this, other);
		}

		#endregion

		public override bool Equals(object obj)
		{
			return (obj is FieldDescription) && (FieldDescription.Equals(this, (FieldDescription) obj));
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public static bool Equals(FieldDescription a, FieldDescription b)
		{
			//	Retourne true si les deux objets sont �gaux.
			if ((a == null) != (b == null))
			{
				return false;
			}

			if (a == null && b == null)
			{
				return true;
			}

			if (!a.guid.Equals(b.guid) ||
				a.type != b.type ||
				!a.backColor.Equals(b.backColor) ||
				a.separator != b.separator ||
				a.columnsRequired != b.columnsRequired ||
				a.rowsRequired != b.rowsRequired ||
				a.containerLayoutMode != b.containerLayoutMode ||
				!a.containerMargins.Equals(b.containerMargins) ||
				!a.containerPadding.Equals(b.containerPadding) ||
				!a.containerBackColor.Equals(b.containerBackColor) ||
				a.containerFrameState != b.containerFrameState ||
				a.containerFrameWidth != b.containerFrameWidth)
			{
				return false;
			}

			if ((a.fieldIds == null) != (b.fieldIds == null))
			{
				return false;
			}

			if (a.fieldIds != null)
			{
				if (a.fieldIds.Count != b.fieldIds.Count)
				{
					return false;
				}

				for (int i=0; i<a.fieldIds.Count; i++)
				{
					if (a.fieldIds[i] != b.fieldIds[i])
					{
						return false;
					}
				}
			}

			return true;
		}


		#region Serialisation
		public void WriteXml(XmlWriter writer)
		{
			//	S�rialise toute la description.
			//	NodeDescription n'est pas s�rialis�, car on ne peut s�rialiser que des listes (pas d'arbres).
			if (this.type == FieldType.Node)
			{
				throw new System.InvalidOperationException("WriteXml: le type Node ne peut pas �tre s�rialis�.");
			}

			writer.WriteStartElement(Xml.FieldDescription);
			
			writer.WriteElementString(Xml.Guid, this.guid.ToString());
			writer.WriteElementString(Xml.Type, this.type.ToString());
			writer.WriteElementString(Xml.FieldIds, this.GetPath(null));

			if (!this.backColor.IsEmpty)
			{
				writer.WriteElementString(Xml.BackColor, Color.ToString(this.backColor));
			}

			writer.WriteElementString(Xml.Separator, this.separator.ToString());
			writer.WriteElementString(Xml.ColumnsRequired, this.columnsRequired.ToString(System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteElementString(Xml.RowsRequired, this.rowsRequired.ToString(System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteElementString(Xml.ContainerLayoutMode, this.containerLayoutMode.ToString());

			if (this.containerMargins != Margins.Zero)
			{
				writer.WriteElementString(Xml.ContainerMargins, this.containerMargins.ToString());
			}

			if (this.containerPadding != Margins.Zero)
			{
				writer.WriteElementString(Xml.ContainerPadding, this.containerPadding.ToString());
			}

			if (!this.containerBackColor.IsEmpty)
			{
				writer.WriteElementString(Xml.ContainerBackColor, Color.ToString(this.containerBackColor));
			}

			writer.WriteElementString(Xml.ContainerFrameState, this.containerFrameState.ToString());
			writer.WriteElementString(Xml.ContainerFrameWidth, this.containerFrameWidth.ToString(System.Globalization.CultureInfo.InvariantCulture));

			writer.WriteEndElement();
		}

		protected void ReadXml(XmlReader reader)
		{
			//	D�s�rialise toute la description.
			//	NodeDescription n'est pas d�s�rialis�, car on ne peut s�rialiser que des listes (pas d'arbres).
			reader.Read();

			while (true)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					string name = reader.LocalName;

					if (name == "??")
					{
					}
					else
					{
						string element = reader.ReadElementString();

						if (name == Xml.Guid)
						{
							this.guid = new System.Guid(element);
						}
						else if (name == Xml.Type)
						{
							this.type = (FieldType) System.Enum.Parse(typeof(FieldType), element);
						}
						else if (name == Xml.FieldIds)
						{
							this.SetFields(element);
						}
						else if (name == Xml.BackColor)
						{
							this.backColor = Color.Parse(element);
						}
						else if (name == Xml.Separator)
						{
							this.separator = (SeparatorType) System.Enum.Parse(typeof(SeparatorType), element);
						}
						else if (name == Xml.ColumnsRequired)
						{
							this.columnsRequired = int.Parse(element);
						}
						else if (name == Xml.RowsRequired)
						{
							this.rowsRequired = int.Parse(element);
						}
						else if (name == Xml.ContainerLayoutMode)
						{
							this.containerLayoutMode = (ContainerLayoutMode) System.Enum.Parse(typeof(ContainerLayoutMode), element);
						}
						else if (name == Xml.ContainerMargins)
						{
							this.containerMargins = Margins.Parse(element);
						}
						else if (name == Xml.ContainerPadding)
						{
							this.containerPadding = Margins.Parse(element);
						}
						else if (name == Xml.ContainerBackColor)
						{
							this.containerBackColor = Color.Parse(element);
						}
						else if (name == Xml.ContainerFrameState)
						{
							this.containerFrameState = (FrameState) System.Enum.Parse(typeof(FrameState), element);
						}
						else if (name == Xml.ContainerFrameWidth)
						{
							this.containerFrameWidth = double.Parse(element);
						}
						else
						{
							throw new System.NotSupportedException(string.Format("Unexpected XML node {0} found in FieldDescription", name));
						}
					}
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					System.Diagnostics.Debug.Assert(reader.Name == Xml.FieldDescription);
					break;
				}
				else
				{
					reader.Read();
				}
			}
		}
		#endregion


		protected System.Guid guid;
		protected FieldType type;
		protected List<FieldDescription> nodeDescription;
		protected List<Druid> fieldIds;
		protected Color backColor;
		protected SeparatorType separator;
		protected int columnsRequired;
		protected int rowsRequired;
		protected ContainerLayoutMode containerLayoutMode;
		protected Margins containerMargins;
		protected Margins containerPadding;
		protected Color containerBackColor;
		protected FrameState containerFrameState;
		protected double containerFrameWidth;
	}
}
