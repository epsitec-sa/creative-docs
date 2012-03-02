using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.EntitiesEditor
{
	/// <summary>
	/// Cette classe contient toutes les informations relatives à une ligne, c'est-à-dire à un champ.
	/// </summary>
	public class Field
	{
		public enum RouteType
		{
			// Cas A:
			// o--------->
			//
			// Cas A personnalisé:
			//    x---x
			//    |   |
			// o--|   |-->
			//
			// 
			// Cas Bt:
			//       ^
			//       |
			// o-----|
			// 
			// Cas Bt personnalisé:
			//       ^
			//       |
			//    x--|
			//    |
			// o--|
			// 
			// Cas Bb:
			// o-----|
			//       |
			//       V
			// 
			// Cas Bb personnalisé:
			// o--|
			//    |
			//    x--|
			//       |
			//       V
			// 
			// Cas C:
			// o----|
			//      x
			//      |---->
			// 
			// Cas D:
			// o----|
			//      x
			//   <--|
			// 
			// Les cas A et B ont un routage automatique ou personnalisé.
			// 'x' = poignée pour personnaliser le routage.

			Close,		// connection fermée
			Himself,	// connection sur soi-même
			A,			// connection de type A
			Bt,			// connection de type B vers le haut
			Bb,			// connection de type B vers le bas
			C,			// connection de type C
			D,			// connection de type D
		}

		
		public Field(Editor editor)
		{
			System.Diagnostics.Debug.Assert(editor != null);

			this.editor = editor;

			this.textLayoutField = new TextLayout();
			this.textLayoutField.DefaultFontSize = 10;
			this.textLayoutField.Alignment = ContentAlignment.MiddleLeft;
			this.textLayoutField.BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;

			this.textLayoutType = new TextLayout();
			this.textLayoutType.DefaultFontSize = 10;
			this.textLayoutType.Alignment = ContentAlignment.MiddleLeft;
			this.textLayoutType.BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;

			this.relation = FieldRelation.None;
			this.membership = FieldMembership.Local;
			this.captionId = Druid.Empty;
			this.definingEntityId = Druid.Empty;
			this.definingRootEntityId = Druid.Empty;
			this.destination = Druid.Empty;
			this.isNullable = false;
			this.isVirtual = false;
			this.isPrivateRelation = false;
			this.isAscending = false;
			this.isDescending = false;
			this.cultureMapSource = CultureMapSource.Invalid;
			this.rank = -1;
			this.index = -1;
			this.isExplored = false;
			this.isSourceExpanded = false;
			this.definingEntityName = null;
			this.definingEntityClass = StructuredTypeClass.None;
			this.level = 0;
			this.isGroupTop = false;
			this.isGroupBottom = false;
			
			this.routeType = RouteType.Close;
			this.routeRelativeAX1 = 0.2;
			this.routeRelativeAX2 = 0.8;
			this.routeAbsoluteAY = 0.0;
			this.routeRelativeBX = 0.0;
			this.routeRelativeBY = 0.0;
			this.routeRelativeCX = 0.5;
			this.routeAbsoluteDX = 0.0;

			this.commentAttach = AbstractObject.minAttach;
			this.commentMainColor = AbstractObject.MainColor.Yellow;
			this.commentText = Res.Strings.Entities.Comment.DefaultText;
		}

		public void Initialize(AbstractObject obj, StructuredData dataField)
		{
			//	Met à jour selon le StructuredData du champ.
			System.Diagnostics.Debug.Assert(obj is ObjectBox);
			System.Diagnostics.Debug.Assert(!dataField.IsEmpty);

			DesignerApplication app = this.editor.Module.DesignerApplication;
			Druid fieldCaptionId = (Druid) dataField.GetValue(Support.Res.Fields.Field.CaptionId);
			FieldMembership membership = (FieldMembership) dataField.GetValue(Support.Res.Fields.Field.Membership);
			FieldRelation relation = (FieldRelation) dataField.GetValue(Support.Res.Fields.Field.Relation);
			Druid typeId = (Druid) dataField.GetValue(Support.Res.Fields.Field.TypeId);
			Druid definingTypeId = (Druid) dataField.GetValue(Support.Res.Fields.Field.DefiningTypeId);
			Druid deepDefiningTypeId = (Druid) dataField.GetValue(Support.Res.Fields.Field.DeepDefiningTypeId);
			FieldOptions options = (FieldOptions) dataField.GetValue(Support.Res.Fields.Field.Options);
			CultureMapSource source = (CultureMapSource) dataField.GetValue(Support.Res.Fields.Field.CultureMapSource);
			StructuredTypeClass definingType;
			bool? isInterfaceDefinition = Support.ResourceAccessors.StructuredTypeResourceAccessor.ToBoolean(dataField.GetValue(Support.Res.Fields.Field.IsInterfaceDefinition));

			//	Cherche les expressions définies localement et par héritage, le nom
			//	de l'entité qui définit le champ, le nom du champ et le nom du parent
			//	qui définit pour la première fois le champ :
			string localExpression = Field.GetLocalExpression(dataField, this.IsPatch, source);
			string inheritedExpression = Field.GetInheritedExpression(app, dataField, fieldCaptionId, definingTypeId, this.IsPatch, source);
			string typeName = Field.GetFieldTypeName (app, typeId, relation);
			string fieldName = Field.GetFieldName(app, fieldCaptionId);
			string definingName = Field.GetDefiningEntityName (app, this.editor.Module, deepDefiningTypeId, out definingType);
			
			this.captionId = fieldCaptionId;
			this.definingEntityId = definingTypeId;
			this.definingRootEntityId = deepDefiningTypeId;
			this.definingEntityName = definingName;
			this.definingEntityClass = definingType;
			this.localExpression = localExpression;
			this.inheritedExpression = inheritedExpression;
			this.IsNullable = (options & FieldOptions.Nullable) != 0;
			this.IsVirtual = (options & FieldOptions.Virtual) != 0;
			this.IsPrivateRelation = (options & FieldOptions.PrivateRelation) != 0;
			this.IsAscending = (options & FieldOptions.IndexAscending) != 0;
			this.IsDescending = (options & FieldOptions.IndexDescending) != 0;
			this.IsUnchangedInterfaceField = isInterfaceDefinition.HasValue && isInterfaceDefinition.Value;
			this.cultureMapSource = source;
			this.FieldName = fieldName;
			this.FieldTypeName = typeName;
			this.relation = relation;
			this.membership = membership;
			this.destination = typeId;
			this.srcBox = obj as ObjectBox;
		}

		public override string ToString()
		{
			return string.Format ("Entity={0} Root={1}", this.definingEntityId.ToString(), this.definingRootEntityId.ToString ());
		}

		public bool IsReadOnly
		{
			//	Indique s'il s'agit d'un champ non modifiable, c'est-à-dire:
			//	- Un titre
			//	- Un champ hérité
			//	- Un champ d'une interface
			get
			{
				return this.IsTitle || this.isSubtitle || this.IsInherited || this.IsInterfaceOrInterfaceTitle;
			}
		}

		public bool IsStrictlyReadOnly
		{
			//	Indique s'il s'agit d'un champ non modifiable, c'est-à-dire:
			//	- Un titre
			//	- Un champ hérité
			//	- Un champ d'une interface
			//	- Un champ d'un module de patch
			get
			{
				return this.IsTitle || this.isSubtitle || this.IsInherited || this.IsInterfaceOrInterfaceTitle || (this.IsPatch && this.cultureMapSource != CultureMapSource.PatchModule);
			}
		}

		public bool IsTitle
		{
			//	Indique si le champ est un titre d'héritage ou d'interface.
			get
			{
				return this.isTitle;
			}
			set
			{
				if (this.isTitle != value)
				{
					this.isTitle = value;
					this.textLayoutField.Alignment = (this.isTitle || this.isSubtitle) ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft;
				}
			}
		}

		public bool IsSubtitle
		{
			//	Indique si le champ est un sous-titre d'héritage ou d'interface.
			get
			{
				return this.isSubtitle;
			}
			set
			{
				if (this.isSubtitle != value)
				{
					this.isSubtitle = value;
					this.textLayoutField.Alignment = (this.isTitle || this.isSubtitle) ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft;
				}
			}
		}

		public bool IsInherited
		{
			//	Indique s'il s'agit d'un champ hérité.
			get
			{
				if (this.IsTitle || this.IsSubtitle)
				{
					return this.isInherited;
				}
				else
				{
					return this.membership != FieldMembership.Local;
				}
			}
			set
			{
				this.isInherited = value;
			}
		}

		public bool IsInterfaceOrInterfaceTitle
		{
			//	Indique s'il s'agit d'un champ d'une interface ou d'un
			//	titre appartenant à une interface.
			get
			{
				if (this.IsTitle || this.IsSubtitle)
				{
					return this.isInterfaceTitle;
				}
				else
				{
					return this.definingEntityClass == StructuredTypeClass.Interface;
				}
			}
			set
			{
				//	Si c'est un titre, on doit pouvoir spécifier s'il appartient
				//	à une interface. Cf. ObjectBox.UpdateFieldsContent.
				this.isInterfaceTitle = value;
			}
		}

		public bool IsInterfaceLocal
		{
			//	Indique s'il s'agit d'un champ d'une interface importée
			//	directement dans cette entité (un champ provenant d'une
			//	interface qui serait héritée d'un parent retourne false).
			get
			{
				return this.definingEntityClass == StructuredTypeClass.Interface && this.membership == FieldMembership.Local;
			}
		}

		public bool IsUnchangedInterfaceField
		{
			//	Indique si le champ provient d'une interface et qu'il n'a
			//	pas été modifié localement.
			get
			{
				return this.isUnchangedInterfaceField;
			}
			set
			{
				this.isUnchangedInterfaceField = value;
			}
		}

		public bool IsEditExpressionEnabled
		{
			//	Retourne true s'il est possible d'éditer l'expression de ce champ.
			//	A savoir : si une expression est définie localement ou dans un des
			//	parents, si c'est un champ appartenant à une interface importée
			//	localement ou si c'est un champ local.
			get
			{
				if (!string.IsNullOrEmpty(this.localExpression))
				{
					return true;
				}
				if (!string.IsNullOrEmpty(this.inheritedExpression))
				{
					return true;
				}
				if (this.IsInterfaceLocal)
				{
					return true;
				}
				
				return this.membership == FieldMembership.Local;
			}
		}

		public bool IsPatch
		{
			get
			{
				return this.editor.Module.IsPatch;
			}
		}

		public string DefiningEntityName
		{
			get
			{
				return this.definingEntityName;
			}
		}

		public StructuredTypeClass DefiningEntityClass
		{
			get
			{
				return this.definingEntityClass;
			}
		}

		public Druid DefiningEntityId
		{
			//	Druid définissant l'interface.
			get
			{
				return this.definingEntityId;
			}
		}

		public Druid DefiningRootEntityId
		{
			//	Druid du parent définissant l'interface.
			get
			{
				return this.definingRootEntityId;
			}
		}

		public int Level
		{
			//	Niveau d'indentation (0..n).
			get
			{
				return this.level;
			}
			set
			{
				this.level = value;
			}
		}

		public bool IsGroupTop
		{
			//	Case supérieure d'un groupe ?
			get
			{
				return this.isGroupTop;
			}
			set
			{
				this.isGroupTop = value;
			}
		}

		public bool IsGroupBottom
		{
			//	Case inférieure d'un groupe ?
			get
			{
				return this.isGroupBottom;
			}
			set
			{
				this.isGroupBottom = value;
			}
		}

		public string FieldName
		{
			//	Nom du champ ou du titre.
			get
			{
				return this.textLayoutField.Text;
			}
			set
			{
				this.textLayoutField.Text = value;
			}
		}

		public string FieldTypeName
		{
			//	Nom du type du champ.
			get
			{
				return this.fieldTypeName;
			}
			set
			{
				if (this.fieldTypeName != value)
				{
					this.fieldTypeName = value;
					this.UpdateTypeName();
				}
			}
		}

		public string InheritedExpression
		{
			//	Eventuelle expression lambda calculant le champ.
			get
			{
				return this.inheritedExpression;
			}
		}

		public string LocalExpression
		{
			//	Eventuelle expression lambda calculant le champ.
			get
			{
				return this.localExpression;
			}
			set
			{
				this.localExpression = value;
			}
		}

		public TextLayout TextLayoutField
		{
			//	Retourne le TextLayout utilisé pour le nom du champ.
			get
			{
				return this.textLayoutField;
			}
		}

		public TextLayout TextLayoutType
		{
			//	Retourne le TextLayout utilisé pour le type du champ.
			get
			{
				return this.textLayoutType;
			}
		}

		public FieldRelation Relation
		{
			//	Type de la relation éventuelle du champ.
			get
			{
				return this.relation;
			}
			set
			{
				this.relation = value;
			}
		}

		public FieldMembership Membership
		{
			//	Type de l'héritage du champ.
			get
			{
				return this.membership;
			}
		}

		public Druid CaptionId
		{
			//	Druid du champ.
			get
			{
				return this.captionId;
			}
			set
			{
				this.captionId = value;
			}
		}

		public Druid Destination
		{
			//	Destination de la relation éventuelle du champ.
			get
			{
				return this.destination;
			}
		}

		public bool IsNullable
		{
			//	Indique si le champ peut prendre la valeur "null".
			get
			{
				return this.isNullable;
			}
			set
			{
				if (this.isNullable != value)
				{
					this.isNullable = value;
					this.UpdateTypeName();
				}
			}
		}

		public bool IsVirtual
		{
			//	Indique si le champ peut prendre la valeur "null".
			get
			{
				return this.isVirtual;
			}
			set
			{
				if (this.isVirtual != value)
				{
					this.isVirtual = value;
					this.UpdateTypeName ();
				}
			}
		}

		public bool IsPrivateRelation
		{
			//	Indique si la relation du champ est privée.
			get
			{
				return this.isPrivateRelation;
			}
			set
			{
				if (this.isPrivateRelation != value)
				{
					this.isPrivateRelation = value;
					this.UpdateTypeName();
				}
			}
		}

		public bool IsAscending
		{
			get
			{
				return this.isAscending;
			}
			set
			{
				if (this.isAscending != value)
				{
					this.isAscending = value;
					this.UpdateTypeName ();
				}
			}
		}

		public bool IsDescending
		{
			get
			{
				return this.isDescending;
			}
			set
			{
				if (this.isDescending != value)
				{
					this.isDescending = value;
					this.UpdateTypeName ();
				}
			}
		}

		public CultureMapSource CultureMapSource
		{
			get
			{
				return this.cultureMapSource;
			}
			set
			{
				this.cultureMapSource = value;
			}
		}

		public int Rank
		{
			//	Rang du champ dans le tableau, sans tenir compte des titres.
			//	Un titre a un rang qui vaut -1.
			get
			{
				return this.rank;
			}
			set
			{
				this.rank = value;
			}
		}

		public int Index
		{
			//	Index de la ligne dans le tableau, en tenant compte des titres.
			//	Donc, toutes les lignes sont numérotées 0..n.
			get
			{
				return this.index;
			}
			set
			{
				this.index = value;
			}
		}

		public ObjectBox SrcBox
		{
			//	Objet source de la connection
			get
			{
				return this.srcBox;
			}
		}

		public ObjectBox DstBox
		{
			//	Objet destination de la connection (si la relation est explorée).
			get
			{
				return this.dstBox;
			}
			set
			{
				this.dstBox = value;
			}
		}

		public ObjectConnection Connection
		{
			//	Objet connection.
			get
			{
				return this.connection;
			}
			set
			{
				this.connection = value;
			}
		}

		public bool IsExplored
		{
			//	Indique si une relation est explorée, c'est-à-dire si l'entité destination est visible.
			get
			{
				return this.isExplored;
			}
			set
			{
				this.isExplored = value;
			}
		}

		public bool IsSourceExpanded
		{
			//	Indique si la boîte source d'une relation est étendue.
			get
			{
				return this.isSourceExpanded;
			}
			set
			{
				this.isSourceExpanded = value;
			}
		}

		public bool IsAttachToRight
		{
			//	Indique si la boîte source est attachée à droite ou à gauche.
			//	Direction dans laquelle part la connection.
			//	En fait, il s'agit du bouton ConnectionOpenLeft/Right utilisé pour ouvrir
			//	la connection, et non de la direction effective dans laquelle part la
			//	connection !
			get
			{
				return this.isAttachToRight;
			}
			set
			{
				this.isAttachToRight = value;
			}
		}


		public bool HasComment
		{
			//	Est-ce qu'un commentaire est attaché ?
			get
			{
				return this.hasComment;
			}
			set
			{
				this.hasComment = value;
			}
		}

		public Point CommentPosition
		{
			//	Position du commentaire.
			get
			{
				return this.commentPosition;
			}
			set
			{
				this.commentPosition = value;
			}
		}

		public Rectangle CommentBounds
		{
			//	Boîte du commentaire.
			get
			{
				return this.commentBounds;
			}
			set
			{
				this.commentBounds = value;
			}
		}

		public string CommentText
		{
			//	Texte du commentaire.
			get
			{
				return this.commentText;
			}
			set
			{
				this.commentText = value;
			}
		}

		public double CommentAttach
		{
			//	Distance d'attache du commentaire.
			//	Une distance positive commence depuis le début de la connection.
			//	Une distance négative commence depuis la fin de la connection.
			get
			{
				return this.commentAttach;
			}
			set
			{
				this.commentAttach = value;
			}
		}

		public AbstractObject.MainColor CommentMainColor
		{
			//	Couleur du commentaire.
			get
			{
				return this.commentMainColor;
			}
			set
			{
				this.commentMainColor = value;
			}
		}


		public void RouteClear()
		{
			//	Force un routage standard pour la connection.
			this.Route = RouteType.Close;
		}

		public RouteType Route
		{
			//	Type de routage de la connection.
			get
			{
				return this.routeType;
			}
			set
			{
				if (this.routeType != value)
				{
					this.routeType = value;
					this.connection.UpdateRoute();
				}
			}
		}

		public double RouteRelativeAX1
		{
			//	Position intermédiaire de la connection (cas A).
			get
			{
				return this.routeRelativeAX1;
			}
			set
			{
				value = System.Math.Max(value, 0.1);
				value = System.Math.Min(value, this.routeRelativeAX2-0.1);

				if (this.routeRelativeAX1 != value)
				{
					this.routeRelativeAX1 = value;

					this.connection.UpdateRoute();
					this.editor.Invalidate();
				}
			}
		}

		public double RouteRelativeAX2
		{
			//	Position intermédiaire de la connection (cas A).
			get
			{
				return this.routeRelativeAX2;
			}
			set
			{
				value = System.Math.Max(value, this.routeRelativeAX1+0.1);
				value = System.Math.Min(value, 0.9);

				if (this.routeRelativeAX2 != value)
				{
					this.routeRelativeAX2 = value;

					this.connection.UpdateRoute();
					this.editor.Invalidate();
				}
			}
		}

		public double RouteAbsoluteAY
		{
			//	Position intermédiaire de la connection (cas A).
			get
			{
				return this.routeAbsoluteAY;
			}
			set
			{
				if (System.Math.Abs(value) <= 5)
				{
					value = 0;
				}

				if (this.routeAbsoluteAY != value)
				{
					this.routeAbsoluteAY = value;

					this.connection.UpdateRoute();
					this.editor.Invalidate();
				}
			}
		}

		public void RouteAbsoluteAYClear()
		{
			//	Réinitialise un cas A simple, sans exécuter UpdateRoute().
			this.routeAbsoluteAY = 0;
		}

		public double RouteRelativeBX
		{
			//	Position intermédiaire de la connection (cas B).
			get
			{
				return this.routeRelativeBX;
			}
			set
			{
				value = System.Math.Max(value, 0.0);
				value = System.Math.Min(value, 0.9);

				if (this.routeRelativeBX != value)
				{
					this.routeRelativeBX = value;

					this.connection.UpdateRoute();
					this.editor.Invalidate();
				}
			}
		}

		public double RouteRelativeBY
		{
			//	Position intermédiaire de la connection (cas B).
			get
			{
				return this.routeRelativeBY;
			}
			set
			{
				value = System.Math.Max(value, 0.0);
				value = System.Math.Min(value, 0.9);

				if (this.routeRelativeBY != value)
				{
					this.routeRelativeBY = value;

					this.connection.UpdateRoute();
					this.editor.Invalidate();
				}
			}
		}

		public double RouteRelativeCX
		{
			//	Position intermédiaire de la connection (cas C).
			//	0.5 correspond au milieu (valeur par défaut).
			get
			{
				return this.routeRelativeCX;
			}
			set
			{
				value = System.Math.Max(value, 0.1);
				value = System.Math.Min(value, 0.9);

				if (this.routeRelativeCX != value)
				{
					this.routeRelativeCX = value;

					this.connection.UpdateRoute();
					this.editor.Invalidate();
				}
			}
		}

		public double RouteAbsoluteDX
		{
			//	Position intermédiaire de la connection (cas D).
			//	0.0 correspond à la boucle la plus serrée (valeur par défaut).
			get
			{
				return this.routeAbsoluteDX;
			}
			set
			{
				value = System.Math.Max(value, 0.0);

				if (this.routeAbsoluteDX != value)
				{
					this.routeAbsoluteDX = value;

					this.connection.UpdateRoute();
					this.editor.Invalidate();
				}
			}
		}


		private void UpdateTypeName()
		{
			//	Met à jour le nom du type.
			string text = this.fieldTypeName;

			if (this.isNullable)
			{
				text = "(" + text + ")";
			}

			if (this.isVirtual)
			{
				text = "<i>" + text + "</i>";
			}

			if (this.isAscending && this.isDescending)
			{
				text = Misc.Image ("EntityIndexBoth") + text;
			}
			else if (this.isAscending)
			{
				text = Misc.Image ("EntityIndexAscending") + text;
			}
			else if (this.isDescending)
			{
				text = Misc.Image ("EntityIndexDescending") + text;
			}

			this.textLayoutType.Text = text;
		}

		
		#region Serialization
		public void WriteXml(XmlWriter writer)
		{
			//	Sérialise toutes les informations du champ.
			writer.WriteStartElement(Xml.Field);
			
			writer.WriteElementString(Xml.DruidCaptionId, this.captionId.ToString());
			writer.WriteElementString(Xml.DruidDestination, this.destination.ToString());

			if (this.isExplored)
			{
				writer.WriteElementString(Xml.IsAttachedToRight, this.isAttachToRight.ToString(System.Globalization.CultureInfo.InvariantCulture));

				writer.WriteElementString(Xml.RouteType, this.routeType.ToString());

				if (this.routeRelativeAX1 != 0.2)
				{
					writer.WriteElementString(Xml.RouteRelativeAX1, this.routeRelativeAX1.ToString(System.Globalization.CultureInfo.InvariantCulture));
				}

				if (this.routeRelativeAX2 != 0.8)
				{
					writer.WriteElementString(Xml.RouteRelativeAX2, this.routeRelativeAX2.ToString(System.Globalization.CultureInfo.InvariantCulture));
				}

				if (this.routeAbsoluteAY != 0.0)
				{
					writer.WriteElementString(Xml.RouteAbsoluteAY, this.routeAbsoluteAY.ToString(System.Globalization.CultureInfo.InvariantCulture));
				}

				if (this.routeRelativeBX != 0.0)
				{
					writer.WriteElementString(Xml.RouteRelativeBX, this.routeRelativeBX.ToString(System.Globalization.CultureInfo.InvariantCulture));
				}

				if (this.routeRelativeBY != 0.0)
				{
					writer.WriteElementString(Xml.RouteRelativeBY, this.routeRelativeBY.ToString(System.Globalization.CultureInfo.InvariantCulture));
				}

				if (this.routeRelativeCX != 0.5)
				{
					writer.WriteElementString(Xml.RouteRelativeCX, this.routeRelativeCX.ToString(System.Globalization.CultureInfo.InvariantCulture));
				}

				if (this.routeAbsoluteDX != 0.5)
				{
					writer.WriteElementString(Xml.RouteAbsoluteDX, this.routeAbsoluteDX.ToString(System.Globalization.CultureInfo.InvariantCulture));
				}

				if (this.hasComment)
				{
					writer.WriteElementString(Xml.CommentPosition, this.commentPosition.ToString());
					writer.WriteElementString(Xml.CommentBounds, this.commentBounds.ToString());
					writer.WriteElementString(Xml.CommentText, this.commentText);
					writer.WriteElementString(Xml.CommentAttach, this.commentAttach.ToString(System.Globalization.CultureInfo.InvariantCulture));
					writer.WriteElementString(Xml.CommentColor, this.commentMainColor.ToString());
				}
			}

			writer.WriteEndElement();
		}

		public void ReadXml(XmlReader reader)
		{
			reader.Read();
			while (true)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					string name = reader.LocalName;
					string element = reader.ReadElementString();

					if (name == Xml.DruidCaptionId)
					{
						Druid druid = Druid.Parse(element);
						if (druid.IsValid)
						{
							this.captionId = druid;
						}
					}
					else if (name == Xml.DruidDestination)
					{
						Druid druid = Druid.Parse(element);
						if (druid.IsValid)
						{
							this.destination = druid;
						}
					}
					else if (name == Xml.IsAttachedToRight)
					{
						this.isExplored = true;
						this.isAttachToRight = bool.Parse(element);
					}
					else if (name == Xml.RouteType)
					{
						this.routeType = (RouteType) System.Enum.Parse(typeof(RouteType), element);
					}
					else if (name == Xml.RouteRelativeAX1)
					{
						this.routeRelativeAX1 = double.Parse(element);
					}
					else if (name == Xml.RouteRelativeAX2)
					{
						this.routeRelativeAX2 = double.Parse(element);
					}
					else if (name == Xml.RouteAbsoluteAY)
					{
						this.routeAbsoluteAY = double.Parse(element);
					}
					else if (name == Xml.RouteRelativeBX)
					{
						this.routeRelativeBX = double.Parse(element);
					}
					else if (name == Xml.RouteRelativeBY)
					{
						this.routeRelativeBY = double.Parse(element);
					}
					else if (name == Xml.RouteRelativeCX)
					{
						this.routeRelativeCX = double.Parse(element);
					}
					else if (name == Xml.RouteAbsoluteDX)
					{
						this.routeAbsoluteDX = double.Parse(element);
					}
					else if (name == Xml.CommentPosition)
					{
						this.hasComment = true;
						this.commentPosition = Point.Parse(element);
					}
					else if (name == Xml.CommentBounds)
					{
						this.commentBounds = Rectangle.Parse(element);
					}
					else if (name == Xml.CommentText)
					{
						this.commentText = element;
					}
					else if (name == Xml.CommentAttach)
					{
						this.commentAttach = double.Parse(element);
					}
					else if (name == Xml.CommentColor)
					{
						this.commentMainColor = (AbstractObject.MainColor) System.Enum.Parse(typeof(AbstractObject.MainColor), element);
					}
					else
					{
						throw new System.NotSupportedException(string.Format("Unexpected XML node {0} found in field", name));
					}
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					System.Diagnostics.Debug.Assert(reader.Name == Xml.Field);
					break;
				}
				else
				{
					reader.Read();
				}
			}
		}

		public void DeserializeCopyTo(Field dst)
		{
			dst.isAttachToRight = this.isAttachToRight;
			dst.routeType = this.routeType;
			dst.routeRelativeAX1 = this.routeRelativeAX1;
			dst.routeRelativeAX2 = this.routeRelativeAX2;
			dst.routeAbsoluteAY = this.routeAbsoluteAY;
			dst.routeRelativeBX = this.routeRelativeBX;
			dst.routeRelativeBY = this.routeRelativeBY;
			dst.routeRelativeCX = this.routeRelativeCX;
			dst.routeAbsoluteDX = this.routeAbsoluteDX;
			dst.hasComment = this.hasComment;
			dst.commentPosition = this.commentPosition;
			dst.commentBounds = this.commentBounds;
			dst.commentText = this.commentText;
			dst.commentAttach = this.commentAttach;
			dst.commentMainColor = this.commentMainColor;
		}
		#endregion

		private static string GetDefiningEntityName(DesignerApplication app, Module objModule, Druid typeId, out StructuredTypeClass typeClass)
		{
			//	Retourne le nom de l'entité spécifiée par typeId, ainsi que typeClass qui
			//	permet de savoir s'il s'agit d'une entité ou d'une interface.
			string name = null;

			if (typeId.IsValid)
			{
				Module module = app.SearchModule (typeId);
				if (module == null)
				{
					typeClass = StructuredTypeClass.None;
					return Field.otherModule;
				}
				else
				{
					CultureMap entity = module.AccessEntities.Accessor.Collection[typeId];

					if (module != objModule)
					{
						//	Si l'entité est définie dans un autre module, on ajoute le nom
						//	du module au nom de l'entité :
						name = string.Concat (module.ModuleId.Name, ".", entity.Name);
					}
					else
					{
						name = entity.Name;
					}

					StructuredData data = entity.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
					typeClass = (StructuredTypeClass) data.GetValue (Support.Res.Fields.ResourceStructuredType.Class);
				}
			}
			else
			{
				typeClass = StructuredTypeClass.None;
			}

			return name;
		}

		private static string GetFieldName(DesignerApplication app, Druid fieldId)
		{
			//	Retourne le nom du champ spécifié par fieldId.
			Module module = app.SearchModule (fieldId);
			if (module == null)
			{
				return Field.otherModule;
			}
			else
			{
				CultureMap field = module.AccessFields.Accessor.Collection[fieldId];
				return (field == null) ? "" : field.Name;
			}
		}

		private static string GetFieldTypeName(DesignerApplication app, Druid typeId, FieldRelation rel)
		{
			//	Retourne le nom du type ou de l'entité spécifiée par typeId.
			if (!typeId.IsValid)
			{
				return "";
			}

			Module module = app.SearchModule (typeId);
			if (module == null)
			{
				return Field.otherModule;
			}
			else
			{
				CultureMap type = module.AccessTypes.Accessor.Collection[typeId];
				if (type == null)
				{
					//	Ce n'est pas un type simple, c'est donc (forcément) une
					//	entité.
					type = module.AccessEntities.Accessor.Collection[typeId];
				}
				else
				{
					//	Le type est un type simple, pas une entité. Cela ne peut donc
					//	pas être une relation !
					//	TODO: vérifier les implications; l'assertion est fausse, car dans les
					//	ressources de Common.Support, il y a effectivement des collections de
					//	types simples !
					//-				System.Diagnostics.Debug.Assert (rel == FieldRelation.None);
				}
				return (type == null) ? "" : type.Name;
			}
		}

		private static string GetLocalExpression(StructuredData dataField, bool isPatch, CultureMapSource itemSource)
		{
			//	Cherche l'expression définie localement par le champ.
			FieldMembership membership = (FieldMembership) dataField.GetValue (Support.Res.Fields.Field.Membership);
			FieldSource source = (FieldSource) dataField.GetValue (Support.Res.Fields.Field.Source);
			if (membership == FieldMembership.Inherited)
			{
				//	L'expression n'est pas définie (ou redéfinie) localement; elle provient
				//	d'un héritage direct et doit par conséquent être ignorée.
				return null;
			}
			else if (source == FieldSource.Value)
			{
				//	Il n'y a pas d'expression définie; le champ contient une valeur et il
				//	faut retourner une chaîne vide pour représenter cela.
				return "";
			}
			else
			{
				bool usesOriginalData;
				string encoded = dataField.GetValue (Support.Res.Fields.Field.Expression, out usesOriginalData) as string;

				if (isPatch)
				{
					if ((itemSource == CultureMapSource.ReferenceModule) ||
						(itemSource == CultureMapSource.DynamicMerge && usesOriginalData))
					{
						return null;
					}
				}

				Support.EntityEngine.EntityExpression expression = Support.EntityEngine.EntityExpression.FromEncodedExpression (encoded);
				return expression.SourceCode;
			}
		}

		private static string GetInheritedExpression(DesignerApplication app, StructuredData dataField, Druid fieldId, Druid definingEntityId, bool isPatch, CultureMapSource itemSource)
		{
			//	Cherche l'expression héritée par le champ depuis un parent (ou grand-parent
			//	ou une interface).
			if (isPatch && itemSource == CultureMapSource.DynamicMerge)
			{
				string encoded = dataField.GetOriginalValue (Support.Res.Fields.Field.Expression) as string;

				if (string.IsNullOrEmpty (encoded))
				{
					return null;
				}
				else
				{
					Support.EntityEngine.EntityExpression expression = Support.EntityEngine.EntityExpression.FromEncodedExpression (encoded);
					return expression.SourceCode;
				}
			}
			if (isPatch && itemSource == CultureMapSource.ReferenceModule)
			{
				string encoded = dataField.GetValue (Support.Res.Fields.Field.Expression) as string;

				if (!string.IsNullOrEmpty (encoded))
				{
					Support.EntityEngine.EntityExpression expression = Support.EntityEngine.EntityExpression.FromEncodedExpression (encoded);
					return expression.SourceCode;
				}
			}

		again:
			if (!definingEntityId.IsValid)
			{
				return null;
			}

			Module module = app.SearchModule (definingEntityId);
			if (module != null)
			{
				CultureMap entity = module.AccessEntities.Accessor.Collection[definingEntityId];
				StructuredData entityData = entity.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
				IList<StructuredData> fields = entityData.GetValue (Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

				StructuredData fieldData = Field.FindField (fields, fieldId);
				if (fieldData != null)
				{
					string encoded = fieldData.GetValue (Support.Res.Fields.Field.Expression) as string;

					if (!string.IsNullOrEmpty (encoded))
					{
						Support.EntityEngine.EntityExpression expression = Support.EntityEngine.EntityExpression.FromEncodedExpression (encoded);
						return expression.SourceCode;
					}
				}

				Druid ancestorEntityId = (Druid) fieldData.GetValue (Support.Res.Fields.Field.DefiningTypeId);

				if (definingEntityId != ancestorEntityId)
				{
					definingEntityId = ancestorEntityId;
					goto again;
				}
			}

			return null;
		}

		private static StructuredData FindField(IList<StructuredData> fields, Druid fieldId)
		{
			//	Retourne le StructuredData dont le CaptionId est égal à fieldId.
			foreach (StructuredData data in fields)
			{
				Druid fieldDataId = (Druid) data.GetValue (Support.Res.Fields.Field.CaptionId);
				if (fieldDataId == fieldId)
				{
					return data;
				}
			}

			return null;
		}


		private static readonly string otherModule = Misc.Italic ("(autre module)");

		private Editor editor;
		private bool isTitle;
		private bool isSubtitle;
		private bool isInherited;
		private bool isInterfaceTitle;
		private bool isUnchangedInterfaceField;
		private string definingEntityName;
		private StructuredTypeClass definingEntityClass;
		private int level;
		private bool isGroupTop;
		private bool isGroupBottom;
		private TextLayout textLayoutField;
		private TextLayout textLayoutType;
		private string fieldTypeName;
		private string inheritedExpression;
		private string localExpression;
		private FieldRelation relation;
		private FieldMembership membership;
		private Druid captionId;
		private Druid definingEntityId;
		private Druid definingRootEntityId;
		private Druid destination;
		private bool isNullable;
		private bool isVirtual;
		private bool isPrivateRelation;
		private bool isAscending;
		private bool isDescending;
		private CultureMapSource cultureMapSource;
		private int rank;
		private int index;
		private ObjectBox srcBox;
		private ObjectBox dstBox;
		private ObjectConnection connection;
		private bool isExplored;
		private bool isSourceExpanded;
		private bool isAttachToRight;
		private RouteType routeType;
		private double routeRelativeAX1;
		private double routeRelativeAX2;
		private double routeAbsoluteAY;
		private double routeRelativeBX;
		private double routeRelativeBY;
		private double routeRelativeCX;
		private double routeAbsoluteDX;
		private bool hasComment;
		private Point commentPosition;
		private Rectangle commentBounds;
		private string commentText;
		private double commentAttach;
		private AbstractObject.MainColor commentMainColor;
	}
}
