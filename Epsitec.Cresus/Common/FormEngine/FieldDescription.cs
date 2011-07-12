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
	public sealed class FieldDescription : System.IEquatable<FieldDescription>
	{
		public enum FieldType
		{
			None			= 0,	// aucun
			Field			= 1,	// champ
			SubForm			= 2,	// sous-masque
			Node			= 3,	// noeud
			Glue			= 10,	// colle deux éléments sur la même ligne
			Line			= 20,	// séparateur trait horizontal
			Title			= 21,	// séparateur titre automatique
			BoxBegin		= 30,	// début d'une boîte
			BoxEnd			= 31,	// fin d'une boîte
			Command			= 40,	// commande
		}

		[DesignerVisible]
		public enum SeparatorType
		{
			Normal			= 0,	// les champs sont proches
			Compact			= 1,	// les champs se touchent (chevauchement d'un pixel)
			Extend			= 2,	// les champs sont très espacés
		}

		[DesignerVisible]
		public enum BoxLayoutType
		{
			Grid			= 0,	// grille
			HorizontalLeft	= 1,	// empilement horizontal calé à gauche
			HorizontalCenter= 2,	// empilement horizontal centré
			HorizontalRight	= 3,	// empilement horizontal calé à droite
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

		[DesignerVisible]
		public enum FontColorType
		{
			Default			= 0,	// transparent
			Red				= 1,	// rouge
			Green			= 2,	// vert
			Blue			= 3,	// bleu
		}

		[DesignerVisible]
		public enum FontFaceType
		{
			Default			= 0,	// standard
			Courier			= 1,	// Courier New
			Times			= 2,	// Times New Roman
			Black			= 3,	// Arial Black
		}

		[DesignerVisible]
		public enum FontStyleType
		{
			Normal			= 0,	// roman
			Bold			= 1,	// gras
			Italic			= 2,	// italique
			BoldItalic		= 3,	// gras-italique
		}

		[DesignerVisible]
		public enum FontSizeType
		{
			Normal			= 0,	// normal
			Small			= 1,	// petit
			Large			= 2,	// grand
			VeryLarge		= 3,	// très grand
		}

		[DesignerVisible]
		public enum CommandButtonClass
		{
			Default			= 0,	// utilise les réglages par défaut définis au niveau de la commande
			DialogButton	= 1,	// bouton pour les dialogues
			RichDialogButton= 2,	// comme ci-dessus, avec icône
			FlatButton		= 3,	// bouton plat pour barre d'outils
		}


		private FieldDescription()
		{
			//	Constructeur protégé, commun à tous les autres.
			this.Reset();
			this.subFormId = Druid.Empty;
			this.deltaHidden = false;
			this.deltaShowed = false;
			this.deltaMoved = false;
			this.deltaInserted = false;
			this.deltaModified = false;
			this.deltaForwardTab = false;
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
			this.source = model.source;
			this.nodeDescription = model.nodeDescription;
			this.fieldIds = model.fieldIds;
			this.subFormId = model.subFormId;
			this.backColor = model.backColor;
			this.labelFontColor = model.labelFontColor;
			this.fieldFontColor = model.fieldFontColor;
			this.labelFontFace = model.labelFontFace;
			this.fieldFontFace = model.fieldFontFace;
			this.labelFontStyle = model.labelFontStyle;
			this.fieldFontStyle = model.fieldFontStyle;
			this.labelFontSize = model.labelFontSize;
			this.fieldFontSize = model.fieldFontSize;
			this.commandButtonClass = model.commandButtonClass;
			this.separatorBottom = model.separatorBottom;
			this.columnsRequired = model.columnsRequired;
			this.rowsRequired = model.rowsRequired;
			this.boxLayoutType = model.boxLayoutType;
			this.boxPaddingType = model.boxPaddingType;
			this.boxFrameEdges = model.boxFrameEdges;
			this.boxFrameWidth = model.boxFrameWidth;
			this.lineWidth = model.lineWidth;
			this.preferredWidth = model.preferredWidth;
			this.labelReplacement = model.labelReplacement;
			this.verbosity = model.verbosity;
			this.deltaHidden = model.deltaHidden;
			this.deltaShowed = model.deltaShowed;
			this.deltaMoved = model.deltaMoved;
			this.deltaInserted = model.deltaInserted;
			this.deltaModified = model.deltaModified;
			this.deltaForwardTab = model.deltaForwardTab;
			this.deltaAttachGuid = model.deltaAttachGuid;
			this.deltaBrokenAttach = model.deltaBrokenAttach;
			this.forwardTabGuid = model.forwardTabGuid;
		}

		public FieldDescription(XmlReader reader) : this()
		{
			//	Constructeur qui désérialise.
			this.ReadXml(reader);
		}

		public void Reset()
		{
			//	Remet à zéro les propriétés liées à l'aspect.
			this.backColor = BackColorType.None;
			this.labelFontColor = FontColorType.Default;
			this.fieldFontColor = FontColorType.Default;
			this.labelFontFace = FontFaceType.Default;
			this.fieldFontFace = FontFaceType.Default;
			this.labelFontStyle = FontStyleType.Normal;
			this.fieldFontStyle = FontStyleType.Normal;
			this.labelFontSize = FontSizeType.Normal;
			this.fieldFontSize = FontSizeType.Normal;
			this.commandButtonClass = CommandButtonClass.Default;
			this.separatorBottom = SeparatorType.Normal;
			this.columnsRequired = Engine.MaxColumnsRequired;
			this.rowsRequired = 1;
			this.boxLayoutType = BoxLayoutType.Grid;
			this.boxPaddingType = BoxPaddingType.Normal;
			this.boxFrameEdges = FrameEdges.None;
			this.boxFrameWidth = 1;
			this.lineWidth = 1;
			this.preferredWidth = 100;
			this.labelReplacement = Druid.Empty;
			this.verbosity = UI.Verbosity.Default;
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
			//	Retourne le type immuable de cet élément (déterminé lors de la création de l'objet).
			get
			{
				return this.type;
			}
		}

		public bool IsForwardTab
		{
			//	Retourne true si cet élément gère la touche Tab.
			get
			{
				return this.type == FieldType.Field || this.type == FieldType.Command;
			}
		}

		public FieldDescription Source
		{
			//	Descripteur source, non sérialisé. Est utilisé pour les champs en provenance d'un sous-masque.
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

					case FieldType.Command:
						return "Commande";

					case FieldType.SubForm:
						return "Masque de saisie";

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

		public void SetField(Druid druid)
		{
			//	Donne un seul Druid.
			this.fieldIds = new List<Druid>();
			this.fieldIds.Add(druid);
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
				System.Diagnostics.Debug.Assert(this.type == FieldType.Field || this.type == FieldType.SubForm || this.type == FieldType.Command);

				this.fieldIds = new List<Druid>();
				string[] druids = druidsPath.Split('.');
				foreach (string druid in druids)
				{
					Druid id = Druid.Parse(druid);
					this.fieldIds.Add(id);
				}
			}
		}

		public Support.EntityEngine.EntityFieldPath GetFieldPath()
		{
			//	Retourne le chemin encodé pour le champ correspondant.
			return Support.EntityEngine.EntityFieldPath.CreateRelativePath(this.GetPath(null));
		}

		public string GetPath(string prefix)
		{
			//	Retourne le chemin permettant d'accéder au champ.
			//	Par exemple, si prefix = "Data": retourne "Data.[630B2].[630S2]"
			//	Par exemple, si prefix = null:   retourne "[630B2].[630S2]"
			if (this.type == FieldType.Field || this.type == FieldType.SubForm || this.type == FieldType.Command)
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

		public bool HasTextStyle
		{
			//	Indique si au moins un style de texte est défini.
			get
			{
				return this.HasLabelTextStyle || this.HasFieldTextStyle;
			}
		}

		public bool HasLabelTextStyle
		{
			//	Indique si au moins un style de texte est défini pour l'étiquette.
			get
			{
				return this.labelFontColor != FontColorType.Default ||
					   this.labelFontFace  != FontFaceType.Default  ||
					   this.labelFontStyle != FontStyleType.Normal  ||
					   this.labelFontSize  != FontSizeType.Normal;
			}
		}

		public bool HasFieldTextStyle
		{
			//	Indique si au moins un style de texte est défini pour le champ.
			get
			{
				return this.fieldFontColor != FontColorType.Default ||
					   this.fieldFontFace  != FontFaceType.Default  ||
					   this.fieldFontStyle != FontStyleType.Normal  ||
					   this.fieldFontSize  != FontSizeType.Normal;
			}
		}

		public FontColorType LabelFontColor
		{
			//	Couleur de la police pour l'étiquette.
			get
			{
				return this.labelFontColor;
			}
			set
			{
				this.labelFontColor = value;
			}
		}

		public FontColorType FieldFontColor
		{
			//	Couleur de la police pour le champ.
			get
			{
				return this.fieldFontColor;
			}
			set
			{
				this.fieldFontColor = value;
			}
		}

		public FontFaceType LabelFontFace
		{
			//	Nom de la police pour l'étiquette.
			get
			{
				return this.labelFontFace;
			}
			set
			{
				this.labelFontFace = value;
			}
		}

		public FontFaceType FieldFontFace
		{
			//	Nom de la police pour le champ.
			get
			{
				return this.fieldFontFace;
			}
			set
			{
				this.fieldFontFace = value;
			}
		}

		public FontStyleType LabelFontStyle
		{
			//	Style de la police pour l'étiquette.
			get
			{
				return this.labelFontStyle;
			}
			set
			{
				this.labelFontStyle = value;
			}
		}

		public FontStyleType FieldFontStyle
		{
			//	Style de la police pour le champ.
			get
			{
				return this.fieldFontStyle;
			}
			set
			{
				this.fieldFontStyle = value;
			}
		}

		public FontSizeType LabelFontSize
		{
			//	Taille de la police pour l'étiquette.
			get
			{
				return this.labelFontSize;
			}
			set
			{
				this.labelFontSize = value;
			}
		}

		public FontSizeType FieldFontSize
		{
			//	Taille de la police pour le champ.
			get
			{
				return this.fieldFontSize;
			}
			set
			{
				this.fieldFontSize = value;
			}
		}

		public CommandButtonClass CommandButtonClassValue
		{
			//	Type d'une commande.
			get
			{
				return this.commandButtonClass;
			}
			set
			{
				this.commandButtonClass = value;
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

		public BoxLayoutType BoxLayout
		{
			//	Type du contenu pour les boîtes.
			get
			{
				return this.boxLayoutType;
			}
			set
			{
				System.Diagnostics.Debug.Assert(this.type == FieldType.BoxBegin || this.type == FieldType.SubForm);
				this.boxLayoutType = value;
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
				System.Diagnostics.Debug.Assert(this.type == FieldType.BoxBegin || this.type == FieldType.SubForm);
				this.boxPaddingType = value;
			}
		}

		public FrameEdges BoxFrameEdges
		{
			//	Bordures d'une boîte.
			get
			{
				return this.boxFrameEdges;
			}
			set
			{
				System.Diagnostics.Debug.Assert(this.type == FieldType.BoxBegin || this.type == FieldType.SubForm);
				this.boxFrameEdges = value;
			}
		}

		public double BoxFrameWidth
		{
			//	Epaisseur des bordures d'une boîte.
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

		public double LineWidth
		{
			//	Epaisseur des traits de séparation.
			get
			{
				return this.lineWidth;
			}
			set
			{
				System.Diagnostics.Debug.Assert(this.type == FieldType.Line || this.type == FieldType.Title);
				this.lineWidth = value;
			}
		}

		public double PreferredWidth
		{
			//	Largeur préférentielle, si le parent est en mode BoxLayoutType.Horizontal*.
			get
			{
				return this.preferredWidth;
			}
			set
			{
				this.preferredWidth = value;
			}
		}

		public Druid LabelReplacement
		{
			//	Druid optionnel du caption qui remplace le texte par défaut.
			get
			{
				return this.labelReplacement;
			}
			set
			{
				this.labelReplacement = value;
			}
		}

		public UI.Verbosity Verbosity
		{
			//	Mode pour le label d'un placeholder.
			get
			{
				return this.verbosity;
			}
			set
			{
				this.verbosity = value;
			}
		}

		public bool Delta
		{
			//	Retourne true si une information delta quelconque existe.
			get
			{
				return this.DeltaHidden ||
					   this.DeltaShowed ||
					   this.DeltaMoved ||
					   this.DeltaInserted ||
					   this.DeltaModified ||
					   this.DeltaForwardTab ||
					   this.DeltaBrokenAttach;
			}
		}

		public bool DeltaHidden
		{
			//	Dans un Form delta, indique un champ qu'il faut cacher dans la liste finale.
			//	Dans un Form final, indique un champ caché. Il est important que le champ soit là, même si
			//	FormEngine ne générera pas les widgets correspondant.
			//	Guid = champ à cacher
			get
			{
				return this.deltaHidden;
			}
			set
			{
				this.deltaHidden = value;
			}
		}

		public bool DeltaShowed
		{
			//	Dans un Form delta, indique un champ qu'il faut montrer (décacher) dans la liste finale.
			//	Dans un Form final, indique un champ visible.
			//	En fait, cette propriété annule l'effet DeltaHidden du parent.
			//	Guid = champ à montrer
			get
			{
				return this.deltaShowed;
			}
			set
			{
				this.deltaShowed = value;
			}
		}

		public bool DeltaMoved
		{
			//	Dans un Form delta, indique un champ qu'il faut déplacer dans la liste finale.
			//	Guid = champ à déplacer
			//	DeltaAttachGuid = champ *après* lequel on s'attache
			//	... = paramètres du champ
			get
			{
				return this.deltaMoved;
			}
			set
			{
				this.deltaMoved = value;
			}
		}

		public bool DeltaInserted
		{
			//	Dans un Form delta, indique un champ supplémentaire qu'il faut insérer dans la liste finale.
			//	Guid = champ inséré dans le masque final
			//	DeltaAttachGuid = champ *après* lequel on s'attache
			//	... = paramètres du champ
			get
			{
				return this.deltaInserted;
			}
			set
			{
				this.deltaInserted = value;
			}
		}

		public bool DeltaModified
		{
			//	Dans un Form delta, indique un champ qu'il faut modifier dans la liste finale.
			//	Guid = champ modifié
			//	... = paramètres du champ
			get
			{
				return this.deltaModified;
			}
			set
			{
				this.deltaModified = value;
			}
		}

		public bool DeltaForwardTab
		{
			//	Dans un Form delta, indique une destination pour Tab dans la liste finale.
			//	Guid = champ modifié
			//	... = paramètres du champ
			get
			{
				return this.deltaForwardTab;
			}
			set
			{
				this.deltaForwardTab = value;
			}
		}

		public System.Guid DeltaAttachGuid
		{
			//	Retourne l'identificateur unique *après* lequel est attaché un champ déplacé par un delta.
			//	Utilisé uniquement si this.type == FieldType.DeltaAttach ou DeltaInsert !
			get
			{
				return this.deltaAttachGuid;
			}
			set
			{
				this.deltaAttachGuid = value;
			}
		}

		public bool DeltaBrokenAttach
		{
			//	Indique si un lien DeltaAttachGuid est cassé, c'est-à-dire si le Guid n'existe plus.
			//	Utilisé uniquement si this.type == FieldType.DeltaAttach ou DeltaInsert !
			get
			{
				return this.deltaBrokenAttach;
			}
			set
			{
				this.deltaBrokenAttach = value;
			}
		}

		public System.Guid ForwardTabGuid
		{
			//	Retourne l'identificateur unique où doit amener la touche Tab (exception).
			get
			{
				return this.forwardTabGuid;
			}
			set
			{
				this.forwardTabGuid = value;
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
				a.source != b.source ||
				a.subFormId != b.subFormId ||
				a.backColor != b.backColor ||
				a.labelFontColor != b.labelFontColor ||
				a.fieldFontColor != b.fieldFontColor ||
				a.labelFontFace != b.labelFontFace ||
				a.fieldFontFace != b.fieldFontFace ||
				a.labelFontStyle != b.labelFontStyle ||
				a.fieldFontStyle != b.fieldFontStyle ||
				a.labelFontSize != b.labelFontSize ||
				a.fieldFontSize != b.fieldFontSize ||
				a.commandButtonClass != b.commandButtonClass ||
				a.separatorBottom != b.separatorBottom ||
				a.columnsRequired != b.columnsRequired ||
				a.rowsRequired != b.rowsRequired ||
				a.boxLayoutType != b.boxLayoutType ||
				a.boxPaddingType != b.boxPaddingType ||
				a.boxFrameEdges != b.boxFrameEdges ||
				a.boxFrameWidth != b.boxFrameWidth ||
				a.lineWidth != b.lineWidth ||
				a.preferredWidth != b.preferredWidth ||
				a.labelReplacement != b.labelReplacement ||
				a.verbosity != b.verbosity ||
				a.deltaHidden != b.deltaHidden ||
				a.deltaShowed != b.deltaShowed ||
				a.deltaMoved != b.deltaMoved ||
				a.deltaInserted != b.deltaInserted ||
				a.deltaModified != b.deltaModified ||
				a.deltaForwardTab != b.deltaForwardTab ||
				!a.deltaAttachGuid.Equals(b.deltaAttachGuid) ||
				!a.forwardTabGuid.Equals(b.forwardTabGuid))
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

			if (!this.subFormId.IsEmpty)
			{
				writer.WriteElementString(Xml.SubFormId, this.subFormId.ToString());
			}

			if (this.backColor != BackColorType.None)
			{
				writer.WriteElementString(Xml.BackColor, this.backColor.ToString());
			}

			if (this.labelFontColor != FontColorType.Default)
			{
				writer.WriteElementString(Xml.LabelFontColor, this.labelFontColor.ToString());
			}

			if (this.fieldFontColor != FontColorType.Default)
			{
				writer.WriteElementString(Xml.FieldFontColor, this.fieldFontColor.ToString());
			}

			if (this.labelFontFace != FontFaceType.Default)
			{
				writer.WriteElementString(Xml.LabelFontFace, this.labelFontFace.ToString());
			}

			if (this.fieldFontFace != FontFaceType.Default)
			{
				writer.WriteElementString(Xml.FieldFontFace, this.fieldFontFace.ToString());
			}

			if (this.labelFontStyle != FontStyleType.Normal)
			{
				writer.WriteElementString(Xml.LabelFontStyle, this.labelFontStyle.ToString());
			}

			if (this.fieldFontStyle != FontStyleType.Normal)
			{
				writer.WriteElementString(Xml.FieldFontStyle, this.fieldFontStyle.ToString());
			}

			if (this.labelFontSize != FontSizeType.Normal)
			{
				writer.WriteElementString(Xml.LabelFontSize, this.labelFontSize.ToString());
			}

			if (this.fieldFontSize != FontSizeType.Normal)
			{
				writer.WriteElementString(Xml.FieldFontSize, this.fieldFontSize.ToString());
			}

			if (this.commandButtonClass != CommandButtonClass.Default)
			{
				writer.WriteElementString(Xml.CommandButtonClass, this.commandButtonClass.ToString());
			}

			writer.WriteElementString(Xml.SeparatorBottom, this.separatorBottom.ToString());
			writer.WriteElementString(Xml.ColumnsRequired, this.columnsRequired.ToString(System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteElementString(Xml.RowsRequired, this.rowsRequired.ToString(System.Globalization.CultureInfo.InvariantCulture));

			writer.WriteElementString(Xml.BoxLayoutType, this.boxLayoutType.ToString());
			writer.WriteElementString(Xml.BoxPaddingType, this.boxPaddingType.ToString());
			writer.WriteElementString(Xml.BoxFrameEdges, this.boxFrameEdges.ToString());
			writer.WriteElementString(Xml.BoxFrameWidth, this.boxFrameWidth.ToString(System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteElementString(Xml.LineWidth, this.lineWidth.ToString(System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteElementString(Xml.PreferredWidth, this.preferredWidth.ToString(System.Globalization.CultureInfo.InvariantCulture));

			if (this.labelReplacement.IsValid)
			{
				writer.WriteElementString(Xml.LabelReplacement, this.labelReplacement.ToString());
			}

			if (this.verbosity != UI.Verbosity.Default)
			{
				writer.WriteElementString(Xml.Verbosity, this.verbosity.ToString());
			}

			if (this.deltaHidden)
			{
				writer.WriteElementString(Xml.DeltaHidden, this.deltaHidden.ToString());
			}

			if (this.deltaShowed)
			{
				writer.WriteElementString(Xml.DeltaShowed, this.deltaShowed.ToString());
			}

			if (this.deltaMoved)
			{
				writer.WriteElementString(Xml.DeltaMoved, this.deltaMoved.ToString());
			}

			if (this.deltaInserted)
			{
				writer.WriteElementString(Xml.DeltaInserted, this.deltaInserted.ToString());
			}

			if (this.deltaModified)
			{
				writer.WriteElementString(Xml.DeltaModified, this.deltaModified.ToString());
			}

			if (this.deltaForwardTab)
			{
				writer.WriteElementString(Xml.DeltaForwardTab, this.deltaForwardTab.ToString());
			}

			if (this.deltaAttachGuid != System.Guid.Empty)
			{
				writer.WriteElementString(Xml.DeltaAttachGuid, this.deltaAttachGuid.ToString());
			}

			if (this.deltaBrokenAttach)
			{
				writer.WriteElementString(Xml.DeltaBrokenAttach, this.deltaBrokenAttach.ToString());
			}

			if (this.forwardTabGuid != System.Guid.Empty)
			{
				writer.WriteElementString(Xml.ForwardTabGuid, this.forwardTabGuid.ToString());
			}

			writer.WriteEndElement();
		}

		private void ReadXml(XmlReader reader)
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
						else if (name == Xml.SubFormId)
						{
							this.subFormId = Druid.Parse(element);
						}
						else if (name == Xml.BackColor)
						{
							this.backColor = (BackColorType) System.Enum.Parse(typeof(BackColorType), element);
						}
						else if (name == Xml.LabelFontColor)
						{
							this.labelFontColor = (FontColorType) System.Enum.Parse(typeof(FontColorType), element);
						}
						else if (name == Xml.FieldFontColor)
						{
							this.fieldFontColor = (FontColorType) System.Enum.Parse(typeof(FontColorType), element);
						}
						else if (name == Xml.LabelFontFace)
						{
							this.labelFontFace = (FontFaceType) System.Enum.Parse(typeof(FontFaceType), element);
						}
						else if (name == Xml.FieldFontFace)
						{
							this.fieldFontFace = (FontFaceType) System.Enum.Parse(typeof(FontFaceType), element);
						}
						else if (name == Xml.LabelFontStyle)
						{
							this.labelFontStyle = (FontStyleType) System.Enum.Parse(typeof(FontStyleType), element);
						}
						else if (name == Xml.FieldFontStyle)
						{
							this.fieldFontStyle = (FontStyleType) System.Enum.Parse(typeof(FontStyleType), element);
						}
						else if (name == Xml.LabelFontSize)
						{
							this.labelFontSize = (FontSizeType) System.Enum.Parse(typeof(FontSizeType), element);
						}
						else if (name == Xml.FieldFontSize)
						{
							this.fieldFontSize = (FontSizeType) System.Enum.Parse(typeof(FontSizeType), element);
						}
						else if (name == Xml.CommandButtonClass)
						{
							this.commandButtonClass = (CommandButtonClass) System.Enum.Parse(typeof(CommandButtonClass), element);
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
						else if (name == Xml.BoxLayoutType)
						{
							this.boxLayoutType = (BoxLayoutType) System.Enum.Parse(typeof(BoxLayoutType), element);
						}
						else if (name == Xml.BoxPaddingType)
						{
							this.boxPaddingType = (BoxPaddingType) System.Enum.Parse(typeof(BoxPaddingType), element);
						}
						else if (name == Xml.BoxFrameEdges)
						{
							this.boxFrameEdges = (FrameEdges) System.Enum.Parse(typeof(FrameEdges), element);
						}
						else if (name == Xml.BoxFrameWidth)
						{
							this.boxFrameWidth = double.Parse(element);
						}
						else if (name == Xml.LineWidth)
						{
							this.lineWidth = double.Parse(element);
						}
						else if (name == Xml.PreferredWidth)
						{
							this.preferredWidth = double.Parse(element);
						}
						else if (name == Xml.LabelReplacement)
						{
							this.labelReplacement = Druid.Parse(element);
						}
						else if (name == Xml.Verbosity)
						{
							this.verbosity = (UI.Verbosity) System.Enum.Parse(typeof(UI.Verbosity), element);
						}
						else if (name == Xml.DeltaHidden)
						{
							this.deltaHidden = bool.Parse(element);
						}
						else if (name == Xml.DeltaShowed)
						{
							this.deltaShowed = bool.Parse(element);
						}
						else if (name == Xml.DeltaMoved)
						{
							this.deltaMoved = bool.Parse(element);
						}
						else if (name == Xml.DeltaInserted)
						{
							this.deltaInserted = bool.Parse(element);
						}
						else if (name == Xml.DeltaModified)
						{
							this.deltaModified = bool.Parse(element);
						}
						else if (name == Xml.DeltaForwardTab)
						{
							this.deltaForwardTab = bool.Parse(element);
						}
						else if (name == Xml.DeltaAttachGuid)
						{
							this.deltaAttachGuid = new System.Guid(element);
						}
						else if (name == Xml.DeltaBrokenAttach)
						{
							this.deltaBrokenAttach = bool.Parse(element);
						}
						else if (name == Xml.ForwardTabGuid)
						{
							this.forwardTabGuid = new System.Guid(element);
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


		public static double GetRealSeparator(SeparatorType type, bool isLastOfBox)
		{
			//	Retourne la valeur réelle d'une marge d'après son type.
			switch (type)
			{
				case SeparatorType.Compact:
					//	Lorsqu'il s'agit du dernier élément d'une boîte, il ne faut pas
					//	utiliser une marge négative !
					return isLastOfBox ? 0 : -1;

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

		public static Color GetRealBackColor(BackColorType type)
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

		public static Color GetRealFontColor(FontColorType type)
		{
			//	Retourne la couleur réelle d'après son type.
			switch (type)
			{
				case FontColorType.Red:
					return Color.FromRgb(1.0, 0.0, 0.0);

				case FontColorType.Green:
					return Color.FromRgb(0.0, 0.8, 0.0);

				case FontColorType.Blue:
					return Color.FromRgb(0.0, 0.4, 1.0);

				default:
					return Color.Empty;
			}
		}

		public static void GetRealFontStrings(FontFaceType faceType, FontStyleType styleType, out string faceName, out string styleName)
		{
			//	Retourne la police réelle d'après son type.
			faceName = Font.DefaultFontFamily;
			styleName = "Regular";

			switch (faceType)
			{
				case FontFaceType.Courier:
					faceName = "Courier New";
					break;

				case FontFaceType.Times:
					faceName = "Times New Roman";
					break;

				case FontFaceType.Black:
					faceName = "Arial";
					break;
			}

			switch (styleType)
			{
				case FontStyleType.Bold:
					styleName = "Bold";
					break;

				case FontStyleType.Italic:
					styleName = "Italic";
					break;

				case FontStyleType.BoldItalic:
					styleName = "Bold Italic";
					break;
			}
		}

		public static double GetRealFontSize(FontSizeType type)
		{
			//	Retourne la taille réelle d'après son type.
			//	Rappel: la taille standard vaut 10.8 !
			switch (type)
			{
				case FontSizeType.Small:
					return 8.0;

				case FontSizeType.Large:
					return 13.0;

				case FontSizeType.VeryLarge:
					return 16.0;

				default:
					return double.NaN;
			}
		}


		private System.Guid					guid;
		private FieldType					type;
		private FieldDescription			source;
		private List<FieldDescription>		nodeDescription;
		private List<Druid>					fieldIds;
		private Druid						subFormId;
		private BackColorType				backColor;
		private FontColorType				labelFontColor;
		private FontColorType				fieldFontColor;
		private FontFaceType				labelFontFace;
		private FontFaceType				fieldFontFace;
		private FontStyleType				labelFontStyle;
		private FontStyleType				fieldFontStyle;
		private FontSizeType				labelFontSize;
		private FontSizeType				fieldFontSize;
		private CommandButtonClass			commandButtonClass;
		private SeparatorType				separatorBottom;
		private int							columnsRequired;
		private int							rowsRequired;
		private BoxLayoutType				boxLayoutType;
		private BoxPaddingType				boxPaddingType;
		private FrameEdges					boxFrameEdges;
		private double						boxFrameWidth;
		private double						lineWidth;
		private double						preferredWidth;
		private Druid						labelReplacement;
		private UI.Verbosity				verbosity;
		private bool						deltaHidden;
		private bool						deltaShowed;
		private bool						deltaMoved;
		private bool						deltaInserted;
		private bool						deltaModified;
		private bool						deltaForwardTab;
		private System.Guid					deltaAttachGuid;
		private bool						deltaBrokenAttach;
		private System.Guid					forwardTabGuid;
	}
}
