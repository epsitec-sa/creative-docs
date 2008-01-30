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
	public sealed class FieldDescription : System.IEquatable<FieldDescription>
	{
		public enum FieldType
		{
			None			= 0,	// aucun
			Field			= 1,	// champ
			SubForm			= 2,	// sous-masque
			Node			= 3,	// noeud
			Glue			= 10,	// colle deux �l�ments sur la m�me ligne
			Line			= 20,	// s�parateur trait horizontal
			Title			= 21,	// s�parateur titre automatique
			BoxBegin		= 30,	// d�but d'une bo�te
			BoxEnd			= 31,	// fin d'une bo�te
		}

		[DesignerVisible]
		public enum SeparatorType
		{
			Normal			= 0,	// les champs sont proches
			Compact			= 1,	// les champs se touchent (chevauchement d'un pixel)
			Extend			= 2,	// les champs sont tr�s espac�s
		}

		[DesignerVisible]
		public enum BoxPaddingType
		{
			Normal			= 0,	// marge standard
			Compact			= 1,	// marge nulle
			Extend			= 2,	// marge �tendue
		}

		[DesignerVisible]
		public enum BackColorType
		{
			None			= 0,	// transparent
			Gray			= 1,	// gris clair
			Yellow			= 2,	// jaune p�le
			Red				= 3,	// rouge p�le
		}


		private FieldDescription()
		{
			//	Constructeur prot�g�, commun � tous les autres.
			this.Reset();
			this.subFormId = Druid.Empty;
			this.patchHidden = false;
			this.patchMoved = false;
			this.patchInserted = false;
			this.patchModified = false;
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
			this.source = model.source;
			this.backColor = model.backColor;
			this.separatorBottom = model.separatorBottom;
			this.columnsRequired = model.columnsRequired;
			this.rowsRequired = model.rowsRequired;
			this.nodeDescription = model.nodeDescription;
			this.fieldIds = model.fieldIds;
			this.subFormId = model.subFormId;
			this.boxPaddingType = model.boxPaddingType;
			this.boxFrameState = model.boxFrameState;
			this.boxFrameWidth = model.boxFrameWidth;
			this.patchHidden = model.patchHidden;
			this.patchMoved = model.patchMoved;
			this.patchInserted = model.patchInserted;
			this.patchModified = model.patchModified;
		}

		public FieldDescription(XmlReader reader) : this()
		{
			//	Constructeur qui d�s�rialise.
			this.ReadXml(reader);
		}

		public void Reset()
		{
			//	Remet � z�ro les propri�t�s li�es � l'aspect.
			this.backColor = BackColorType.None;
			this.separatorBottom = SeparatorType.Normal;
			this.columnsRequired = Engine.MaxColumnsRequired;
			this.rowsRequired = 1;
			this.boxPaddingType = BoxPaddingType.Normal;
			this.boxFrameState = FrameState.None;
			this.boxFrameWidth = 1;
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

		public FieldType Type
		{
			//	Retourne le type immuable de cet �l�ment (d�termin� lors de la cr�ation de l'objet).
			get
			{
				return this.type;
			}
		}

		public FieldDescription Source
		{
			//	Descripteur source, non s�rialis�. Est utilis� pour les champs en provenance d'un sous-masque.
			get
			{
				return this.source;
			}
			set
			{
				this.source = value;
			}
		}

		public FieldType SourceType
		{
			//	Retourne le type de la source.
			get
			{
				if (this.source == null)
				{
					return FieldType.Node;
				}
				else
				{
					return this.source.type;
				}
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

					case FieldType.SubForm:
						return "Masque de saisie";

					case FieldType.Glue:
						return "Colle";

					case FieldType.Line:
						return "S�parateur";

					case FieldType.Title:
						return "Titre";

					case FieldType.BoxBegin:
						return "Bo�te";

					case FieldType.BoxEnd:
						return "Fin de bo�te";

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
			//	Liste des Druids qui repr�sentent le champ.
			get
			{
				return this.fieldIds;
			}
		}

		public void SetField(Druid druid)
		{
			//	Donne un seul Druid.
			this.fieldIds = new List<Druid>();
			this.fieldIds.Add(druid);
		}

		public void SetFields(string druidsPath)
		{
			//	Donne d'une liste de Druids s�par�s par des points.
			//	Par exemple: druidsPath = "[630B2].[630S2]"
			if (string.IsNullOrEmpty(druidsPath))
			{
				this.fieldIds = null;
			}
			else
			{
				System.Diagnostics.Debug.Assert(this.type == FieldType.Field || this.type == FieldType.SubForm);

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
			//	Retourne le chemin permettant d'acc�der au champ.
			//	Par exemple, si prefix = "Data": retourne "Data.[630B2].[630S2]"
			//	Par exemple, si prefix = null:   retourne "[630B2].[630S2]"
			if (this.type == FieldType.Field || this.type == FieldType.SubForm)
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


		public Druid SubFormId
		{
			//	Druid du formulaire du sous-masque.
			get
			{
				return this.subFormId;
			}
			set
			{
				System.Diagnostics.Debug.Assert(this.type == FieldType.SubForm);
				this.subFormId = value;
			}
		}


		public SeparatorType SeparatorBottom
		{
			//	Type de s�paration apr�s le champ suivant.
			get
			{
				return this.separatorBottom;
			}
			set
			{
				this.separatorBottom = value;
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

		public BoxPaddingType BoxPadding
		{
			//	Type de marge int�rieure pour les bo�tes.
			get
			{
				return this.boxPaddingType;
			}
			set
			{
				System.Diagnostics.Debug.Assert(this.type == FieldType.BoxBegin || this.type == FieldType.SubForm);
				this.boxPaddingType = value;
			}
		}

		public FrameState BoxFrameState
		{
			//	Bordures d'une bo�te.
			get
			{
				return this.boxFrameState;
			}
			set
			{
				System.Diagnostics.Debug.Assert(this.type == FieldType.BoxBegin || this.type == FieldType.SubForm);
				this.boxFrameState = value;
			}
		}

		public double BoxFrameWidth
		{
			//	Epaisseur des bordures d'une bo�te.
			get
			{
				return this.boxFrameWidth;
			}
			set
			{
				System.Diagnostics.Debug.Assert(this.type == FieldType.BoxBegin || this.type == FieldType.SubForm);
				this.boxFrameWidth = value;
			}
		}

		public bool PatchHidden
		{
			//	Dans un Form de patch, indique un champ qu'il faut cacher dans la liste finale.
			//	Dans un Form final, indique un champ cach�. Il est important que le champ soit l�, m�me si
			//	FormEngine ne g�n�rera pas les widgets correspondant.
			//	Guid = champ � cacher
			get
			{
				return this.patchHidden;
			}
			set
			{
				this.patchHidden = value;
			}
		}

		public bool PatchMoved
		{
			//	Dans un Form de patch, indique un champ qu'il faut d�placer dans la liste finale.
			//	Guid = champ � d�placer
			//	PatchAttachGuid = champ *apr�s* lequel on s'attache
			//	... = param�tres du champ
			get
			{
				return this.patchMoved;
			}
			set
			{
				this.patchMoved = value;
			}
		}

		public bool PatchInserted
		{
			//	Dans un Form de patch, indique un champ suppl�mentaire qu'il faut ins�rer dans la liste finale.
			//	Guid = champ ins�r� dans le masque de patch
			//	PatchAttachGuid = champ *apr�s* lequel on s'attache
			//	... = param�tres du champ
			get
			{
				return this.patchInserted;
			}
			set
			{
				this.patchInserted = value;
			}
		}

		public bool PatchModified
		{
			//	Dans un Form de patch, indique un champ qu'il faut modifier dans la liste finale.
			//	Guid = champ modifi�
			//	... = param�tres du champ
			get
			{
				return this.patchModified;
			}
			set
			{
				this.patchModified = value;
			}
		}

		public System.Guid PatchAttachGuid
		{
			//	Retourne l'identificateur unique *apr�s* lequel est attach� un champ d�plac� par un patch.
			//	Utilis� uniquement si this.type == FieldType.PatchAttach ou PatchInsert !
			get
			{
				return this.patchAttachGuid;
			}
			set
			{
				this.patchAttachGuid = value;
			}
		}

		public bool PatchBrokenAttach
		{
			//	Indique si un lien PatchAttachGuid est cass�, c'est-�-dire si le Guid n'eixste plus.
			//	Utilis� uniquement si this.type == FieldType.PatchAttach ou PatchInsert !
			get
			{
				return this.patchBrokenAttach;
			}
			set
			{
				this.patchBrokenAttach = value;
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
				a.source != b.source ||
				a.subFormId != b.subFormId ||
				a.backColor != b.backColor ||
				a.separatorBottom != b.separatorBottom ||
				a.columnsRequired != b.columnsRequired ||
				a.rowsRequired != b.rowsRequired ||
				a.boxPaddingType != b.boxPaddingType ||
				a.boxFrameState != b.boxFrameState ||
				a.boxFrameWidth != b.boxFrameWidth ||
				a.patchHidden != b.patchHidden ||
				a.patchMoved != b.patchMoved ||
				a.patchInserted != b.patchInserted ||
				a.patchModified != b.patchModified ||
				!a.patchAttachGuid.Equals(b.patchAttachGuid) )
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

			if (!this.subFormId.IsEmpty)
			{
				writer.WriteElementString(Xml.SubFormId, this.subFormId.ToString());
			}

			if (this.backColor != BackColorType.None)
			{
				writer.WriteElementString(Xml.BackColor, this.backColor.ToString());
			}

			writer.WriteElementString(Xml.SeparatorBottom, this.separatorBottom.ToString());
			writer.WriteElementString(Xml.ColumnsRequired, this.columnsRequired.ToString(System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteElementString(Xml.RowsRequired, this.rowsRequired.ToString(System.Globalization.CultureInfo.InvariantCulture));

			writer.WriteElementString(Xml.BoxPaddingType, this.boxPaddingType.ToString());
			writer.WriteElementString(Xml.BoxFrameState, this.boxFrameState.ToString());
			writer.WriteElementString(Xml.BoxFrameWidth, this.boxFrameWidth.ToString(System.Globalization.CultureInfo.InvariantCulture));

			if (this.patchHidden)
			{
				writer.WriteElementString(Xml.PatchHidden, this.patchHidden.ToString());
			}

			if (this.patchMoved)
			{
				writer.WriteElementString(Xml.PatchMoved, this.patchMoved.ToString());
			}

			if (this.patchInserted)
			{
				writer.WriteElementString(Xml.PatchInserted, this.patchInserted.ToString());
			}

			if (this.patchModified)
			{
				writer.WriteElementString(Xml.PatchModified, this.patchModified.ToString());
			}

			if (this.patchAttachGuid != System.Guid.Empty)
			{
				writer.WriteElementString(Xml.PatchAttachGuid, this.patchAttachGuid.ToString());
			}

			if (this.patchBrokenAttach)
			{
				writer.WriteElementString(Xml.PatchBrokenAttach, this.patchBrokenAttach.ToString());
			}

			writer.WriteEndElement();
		}

		private void ReadXml(XmlReader reader)
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
						else if (name == Xml.SubFormId)
						{
							this.subFormId = Druid.Parse(element);
						}
						else if (name == Xml.BackColor)
						{
							this.backColor = (BackColorType) System.Enum.Parse(typeof(BackColorType), element);
						}
						else if (name == Xml.SeparatorBottom)
						{
							this.separatorBottom = (SeparatorType) System.Enum.Parse(typeof(SeparatorType), element);
						}
						else if (name == Xml.ColumnsRequired)
						{
							this.columnsRequired = int.Parse(element);
						}
						else if (name == Xml.RowsRequired)
						{
							this.rowsRequired = int.Parse(element);
						}
						else if (name == Xml.BoxPaddingType)
						{
							this.boxPaddingType = (BoxPaddingType) System.Enum.Parse(typeof(BoxPaddingType), element);
						}
						else if (name == Xml.BoxFrameState)
						{
							this.boxFrameState = (FrameState) System.Enum.Parse(typeof(FrameState), element);
						}
						else if (name == Xml.BoxFrameWidth)
						{
							this.boxFrameWidth = double.Parse(element);
						}
						else if (name == Xml.PatchHidden)
						{
							this.patchHidden = bool.Parse(element);
						}
						else if (name == Xml.PatchMoved)
						{
							this.patchMoved = bool.Parse(element);
						}
						else if (name == Xml.PatchInserted)
						{
							this.patchInserted = bool.Parse(element);
						}
						else if (name == Xml.PatchModified)
						{
							this.patchModified = bool.Parse(element);
						}
						else if (name == Xml.PatchAttachGuid)
						{
							this.patchAttachGuid = new System.Guid(element);
						}
						else if (name == Xml.PatchBrokenAttach)
						{
							this.patchBrokenAttach = bool.Parse(element);
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
			//	Retourne la valeur r�elle d'une marge d'apr�s son type.
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
			//	Retourne la valeur r�elle d'une marge d'apr�s son type.
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
			//	Retourne la couleur r�elle d'apr�s son type.
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


		private System.Guid					guid;
		private FieldType					type;
		private FieldDescription			source;
		private List<FieldDescription>		nodeDescription;
		private List<Druid>					fieldIds;
		private Druid						subFormId;
		private BackColorType				backColor;
		private SeparatorType				separatorBottom;
		private int							columnsRequired;
		private int							rowsRequired;
		private BoxPaddingType				boxPaddingType;
		private FrameState					boxFrameState;
		private double						boxFrameWidth;
		private bool						patchHidden;
		private bool						patchMoved;
		private bool						patchInserted;
		private bool						patchModified;
		private System.Guid					patchAttachGuid;
		private bool						patchBrokenAttach;
	}
}
