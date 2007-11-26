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
	/// Décrit un noeud, un champ ou un séparateur dans un masque de saisie.
	/// </summary>
	public class FieldDescription : System.IEquatable<FieldDescription>
	{
		public enum FieldType
		{
			Field			= 1,	// champ
			Node			= 2,	// noeud
			Glue			= 10,	// colle deux éléments sur la même ligne
			Line			= 20,	// séparateur trait horizontal
			Title			= 21,	// séparateur titre automatique
			BoxBegin		= 30,	// début d'une boîte
			BoxEnd			= 31,	// fin d'une boîte
			InsertionPoint	= 40,	// spécifie le point d'insertion, dans un module de patch
			Hide			= 41,	// champ du module de référence à cacher, dans un module de patch
		}

		[DesignerVisible]
		public enum SeparatorType
		{
			Normal			= 0,	// les champs sont proches
			Compact			= 1,	// les champs se touchent (chevauchement d'un pixel)
			Extend			= 2,	// les champs sont très espacés
		}

		[DesignerVisible]
		public enum BoxPaddingType
		{
			Normal			= 0,	// marge standard
			Compact			= 1,	// marge nulle
			Extend			= 2,	// marge étendue
		}

		[DesignerVisible]
		public enum BackColorType
		{
			None			= 0,	// transparent
			Gray			= 1,	// gris clair
			Yellow			= 2,	// jaune pâle
			Red				= 3,	// rouge pâle
		}


		protected FieldDescription()
		{
			//	Constructeur protégé, commun à tous les autres.
			this.backColor = BackColorType.None;
			this.separatorBottom = SeparatorType.Normal;
			this.boxPaddingType = BoxPaddingType.Normal;
			this.columnsRequired = Engine.MaxColumnsRequired;
			this.rowsRequired = 1;
			this.containerFrameState = FrameState.None;
			this.containerFrameWidth = 1;
		}

		public FieldDescription(FieldType type) : this()
		{
			//	Constructeur.
			//	Le type est toujours déterminé à la création. Il ne pourra plus changer par la suite.
			this.guid = System.Guid.NewGuid();
			this.type = type;
		}

		public FieldDescription(FieldDescription model) : this()
		{
			//	Constructeur copiant une instance existante.
			this.guid = model.guid;
			this.type = model.type;
			this.backColor = model.backColor;
			this.separatorBottom = model.separatorBottom;
			this.boxPaddingType = model.boxPaddingType;
			this.columnsRequired = model.columnsRequired;
			this.rowsRequired = model.rowsRequired;
			this.nodeDescription = model.nodeDescription;
			this.fieldIds = model.fieldIds;
			this.containerFrameState = model.containerFrameState;
			this.containerFrameWidth = model.containerFrameWidth;
		}

		public FieldDescription(XmlReader reader) : this()
		{
			//	Constructeur qui désérialise.
			this.ReadXml(reader);
		}


		public System.Guid Guid
		{
			//	Retourne l'identificateur unique.
			get
			{
				return this.guid;
			}
			set
			{
				this.guid = value;
			}
		}

		public int UniqueId
		{
			//	Identificateur unique, non sérialisé. Permet le lien avec les widgets (propriété Index) créés
			//	par le moteur FormEgine.CreateForm().
			get
			{
				return this.uniqueId;
			}
			set
			{
				this.uniqueId = value;
			}
		}

		public FieldType Type
		{
			//	Retourne le type immuable de cet élément (déterminé lors de la création de l'objet).
			get
			{
				return this.type;
			}
		}

		public string Description
		{
			//	Retourne un texte de description.
			get
			{
				switch (this.type)
				{
					case FieldType.Field:
						return "Champ";

					case FieldType.Glue:
						return "Colle";

					case FieldType.Line:
						return "Séparateur";

					case FieldType.Title:
						return "Titre";

					case FieldType.BoxBegin:
						return "Boîte";

					case FieldType.BoxEnd:
						return "Fin de boîte";

					default:
						return null;
				}
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


		public List<Druid> FieldIds
		{
			//	Liste des Druids qui représentent le champ.
			get
			{
				return this.fieldIds;
			}
		}

		public void SetFields(string druidsPath)
		{
			//	Donne d'une liste de Druids séparés par des points.
			//	Par exemple: druidsPath = "[630B2].[630S2]"
			if (string.IsNullOrEmpty(druidsPath))
			{
				this.fieldIds = null;
			}
			else
			{
				System.Diagnostics.Debug.Assert(this.type == FieldType.Field);

				this.fieldIds = new List<Druid>();
				string[] druids = druidsPath.Split('.');
				foreach (string druid in druids)
				{
					Druid id = Druid.Parse(druid);
					this.fieldIds.Add(id);
				}
			}
		}

		public string GetPath(string prefix)
		{
			//	Retourne le chemin permettant d'accéder au champ.
			//	Par exemple, si prefix = "Data": retourne "Data.[630B2].[630S2]"
			//	Par exemple, si prefix = null:   retourne "[630B2].[630S2]"
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


		public SeparatorType SeparatorBottom
		{
			//	Type de séparation après le champ suivant.
			get
			{
				return this.separatorBottom;
			}
			set
			{
				this.separatorBottom = value;
			}
		}

		public BoxPaddingType BoxPadding
		{
			//	Type de marge intérieure pour les boîtes.
			get
			{
				return this.boxPaddingType;
			}
			set
			{
				this.boxPaddingType = value;
			}
		}

		public BackColorType BackColor
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
				value = System.Math.Max(value, 0);
				value = System.Math.Min(value, Engine.MaxColumnsRequired);
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
				value = System.Math.Min(value, Engine.MaxRowsRequired);
				this.rowsRequired = value;
			}
		}

		public FrameState ContainerFrameState
		{
			//	Bordures d'une boîte.
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
			//	Epaisseur des bordures d'une boîte.
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
			//	Retourne true si les deux objets sont égaux.
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
				a.backColor != b.backColor ||
				a.separatorBottom != b.separatorBottom ||
				a.boxPaddingType != b.boxPaddingType ||
				a.columnsRequired != b.columnsRequired ||
				a.rowsRequired != b.rowsRequired ||
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
			//	Sérialise toute la description.
			//	NodeDescription n'est pas sérialisé, car on ne peut sérialiser que des listes (pas d'arbres).
			if (this.type == FieldType.Node)
			{
				throw new System.InvalidOperationException("WriteXml: le type Node ne peut pas être sérialisé.");
			}

			writer.WriteStartElement(Xml.FieldDescription);
			
			writer.WriteElementString(Xml.Guid, this.guid.ToString());
			writer.WriteElementString(Xml.Type, this.type.ToString());
			writer.WriteElementString(Xml.FieldIds, this.GetPath(null));

			if (this.backColor != BackColorType.None)
			{
				writer.WriteElementString(Xml.BackColor, this.backColor.ToString());
			}

			writer.WriteElementString(Xml.SeparatorBottom, this.separatorBottom.ToString());
			writer.WriteElementString(Xml.BoxPaddingType, this.boxPaddingType.ToString());
			writer.WriteElementString(Xml.ColumnsRequired, this.columnsRequired.ToString(System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteElementString(Xml.RowsRequired, this.rowsRequired.ToString(System.Globalization.CultureInfo.InvariantCulture));

			writer.WriteElementString(Xml.ContainerFrameState, this.containerFrameState.ToString());
			writer.WriteElementString(Xml.ContainerFrameWidth, this.containerFrameWidth.ToString(System.Globalization.CultureInfo.InvariantCulture));

			writer.WriteEndElement();
		}

		protected void ReadXml(XmlReader reader)
		{
			//	Désérialise toute la description.
			//	NodeDescription n'est pas désérialisé, car on ne peut sérialiser que des listes (pas d'arbres).
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
							this.backColor = (BackColorType) System.Enum.Parse(typeof(BackColorType), element);
						}
						else if (name == Xml.SeparatorBottom)
						{
							this.separatorBottom = (SeparatorType) System.Enum.Parse(typeof(SeparatorType), element);
						}
						else if (name == Xml.BoxPaddingType)
						{
							this.boxPaddingType = (BoxPaddingType) System.Enum.Parse(typeof(BoxPaddingType), element);
						}
						else if (name == Xml.ColumnsRequired)
						{
							this.columnsRequired = int.Parse(element);
						}
						else if (name == Xml.RowsRequired)
						{
							this.rowsRequired = int.Parse(element);
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


		public static double GetRealSeparator(SeparatorType type)
		{
			//	Retourne la valeur réelle d'une marge d'après son type.
			switch (type)
			{
				case SeparatorType.Compact:
					return -1;

				case SeparatorType.Extend:
					return 10;

				default:
					return 2;
			}
		}

		public static Margins GetRealBoxPadding(BoxPaddingType type)
		{
			//	Retourne la valeur réelle d'une marge d'après son type.
			switch (type)
			{
				case BoxPaddingType.Compact:
					return new Margins(0, 0, 0, 0);

				case BoxPaddingType.Extend:
					return new Margins(10, 10, 10, 10);

				default:
					return new Margins(5, 5, 5, 5);
			}
		}

		public static Color GetRealColor(BackColorType type)
		{
			//	Retourne la couleur réelle d'après son type.
			switch (type)
			{
				case BackColorType.Gray:
					return Color.FromBrightness(0.8);

				case BackColorType.Red:
					return Color.FromRgb(1.0, 0.5, 0.5);

				case BackColorType.Yellow:
					return Color.FromRgb(1.0, 0.9, 0.5);

				default:
					return Color.Empty;
			}
		}


		protected System.Guid guid;
		protected int uniqueId;
		protected FieldType type;
		protected List<FieldDescription> nodeDescription;
		protected List<Druid> fieldIds;
		protected BackColorType backColor;
		protected SeparatorType separatorBottom;
		protected BoxPaddingType boxPaddingType;
		protected int columnsRequired;
		protected int rowsRequired;
		protected FrameState containerFrameState;
		protected double containerFrameWidth;
	}
}
