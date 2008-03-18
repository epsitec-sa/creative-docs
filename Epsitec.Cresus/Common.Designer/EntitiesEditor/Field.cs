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
			this.definingTypeId = Druid.Empty;
			this.deepDefiningTypeId = Druid.Empty;
			this.destination = Druid.Empty;
			this.isNullable = false;
			this.isPrivateRelation = false;
			this.cultureMapSource = CultureMapSource.Invalid;
			this.rank = -1;
			this.index = -1;
			this.isExplored = false;
			this.isSourceExpanded = false;
			this.definingName = null;
			this.definingType = StructuredTypeClass.None;
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

			Druid fieldCaptionId = (Druid) dataField.GetValue(Support.Res.Fields.Field.CaptionId);
			FieldMembership membership = (FieldMembership) dataField.GetValue(Support.Res.Fields.Field.Membership);
			FieldRelation rel = (FieldRelation) dataField.GetValue(Support.Res.Fields.Field.Relation);
			Druid typeId = (Druid) dataField.GetValue(Support.Res.Fields.Field.TypeId);
			Druid definingTypeId = (Druid) dataField.GetValue(Support.Res.Fields.Field.DefiningTypeId);
			Druid deepDefiningTypeId = (Druid) dataField.GetValue(Support.Res.Fields.Field.DeepDefiningTypeId);
			FieldOptions options = (FieldOptions) dataField.GetValue(Support.Res.Fields.Field.Options);
			CultureMapSource source = (CultureMapSource) dataField.GetValue(Support.Res.Fields.Field.CultureMapSource);

			//	Cherche les expressions définies dans le champ de l'interface.
			string expression = this.GetExpression(dataField);

			string deepExpression = null;
			if (definingTypeId.IsValid)
			{
				deepExpression = this.GetDeepExpression(obj.Editor.Module.DesignerApplication, dataField, fieldCaptionId, definingTypeId);
			}

			object interfaceDefinition = dataField.GetValue(Support.Res.Fields.Field.IsInterfaceDefinition);
			bool isInterfaceDefinition = !UndefinedValue.IsUndefinedValue(interfaceDefinition);

			Module typeModule = obj.Editor.Module.DesignerApplication.SearchModule(typeId);
			CultureMap typeCultureMap = null;
			if (typeModule != null)
			{
				typeCultureMap = typeModule.AccessTypes.Accessor.Collection[typeId];
				if (typeCultureMap == null)
				{
					typeCultureMap = typeModule.AccessEntities.Accessor.Collection[typeId];
				}
				else
				{
					//	Le type est un type simple, pas une entité. Cela ne peut donc
					//	pas être une relation !
					rel = FieldRelation.None;
				}
			}

			Module fieldModule = obj.Editor.Module.DesignerApplication.SearchModule(fieldCaptionId);
			CultureMap fieldCultureMap = fieldModule.AccessFields.Accessor.Collection[fieldCaptionId];

			string definingName = null;
			StructuredTypeClass definingType = StructuredTypeClass.None;

			if (deepDefiningTypeId.IsValid)
			{
				Module definingModule = obj.Editor.Module.DesignerApplication.SearchModule(deepDefiningTypeId);
				CultureMap definingCultureMap = definingModule.AccessEntities.Accessor.Collection[deepDefiningTypeId];

				definingName = definingCultureMap.Name;
				if (definingModule != obj.Editor.Module)  // dans un autre module ?
				{
					definingName = string.Concat(definingModule.ModuleId.Name, ".", definingName);
				}

				StructuredData definingData = definingCultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
				definingType = (StructuredTypeClass) definingData.GetValue(Support.Res.Fields.ResourceStructuredType.Class);
			}

			this.captionId = fieldCaptionId;
			this.definingTypeId = definingTypeId;
			this.deepDefiningTypeId = deepDefiningTypeId;
			this.definingName = definingName;
			this.definingType = definingType;
			this.isInterfaceDefinition = isInterfaceDefinition;
			this.expression = expression;
			this.deepExpression = deepExpression;
			this.IsNullable = (options & FieldOptions.Nullable) != 0;
			this.IsPrivateRelation = (options & FieldOptions.PrivateRelation) != 0;
			this.cultureMapSource = source;
			this.FieldName = (fieldCultureMap == null) ? "" : fieldCultureMap.Name;
			this.TypeName = (typeCultureMap == null) ? "" : typeCultureMap.Name;
			this.relation = rel;
			this.membership = membership;
			this.destination = typeId;
			this.srcBox = obj as ObjectBox;
		}

		protected string GetExpression(StructuredData dataField)
		{
			//	Cherche l'expression définie dans le champ de l'interface.
			string encoded = dataField.GetValue(Support.Res.Fields.Field.Expression) as string;
			Support.EntityEngine.EntityExpression expression = Support.EntityEngine.EntityExpression.FromEncodedExpression(encoded);
			return expression.SourceCode;
		}

		protected string GetDeepExpression(DesignerApplication designerApplication, StructuredData dataField, Druid fieldCaptionId, Druid definingTypeId)
		{
			//	Cherche l'expression profonde définie dans le champ d'un parent de l'interface.
			string deepExpression = null;

			Module module = designerApplication.SearchModule(definingTypeId);
			CultureMap item = module.AccessEntities.Accessor.Collection[definingTypeId];
			StructuredData data = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
			IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

			StructuredData fieldData = Field.FindCaptionId(dataFields, fieldCaptionId);
			if (fieldData != null)
			{
				string encoded = fieldData.GetValue(Support.Res.Fields.Field.Expression) as string;
				Support.EntityEngine.EntityExpression expression = Support.EntityEngine.EntityExpression.FromEncodedExpression(encoded);
				deepExpression = expression.SourceCode;
			}

			return deepExpression;
		}

		static protected StructuredData FindCaptionId(IList<StructuredData> dataFields, Druid id)
		{
			//	Retourne le StructuredData dont le CaptionId est égal à Id.
			foreach (StructuredData data in dataFields)
			{
				Druid fieldDataId = (Druid) data.GetValue(Support.Res.Fields.Field.CaptionId);
				if (fieldDataId == id)
				{
					return data;
				}
			}

			return null;
		}


		public bool IsReadOnly
		{
			//	Indique s'il s'agit d'un champ non modifiable, c'est-à-dire:
			//	- Un titre
			//	- Un champ hérité
			//	- Un champ d'une interface
			get
			{
				return this.IsTitle || this.isSubtitle || this.IsInherited || this.IsInterface;
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

		public bool IsInterface
		{
			//	Indique s'il s'agit d'un champ d'une interface.
			get
			{
				if (this.IsTitle || this.IsSubtitle)
				{
					return this.isInterface;
				}
				else
				{
					return (this.definingTypeId.IsValid && this.membership == FieldMembership.Local) || this.membership == FieldMembership.LocalOverride;
				}
			}
			set
			{
				this.isInterface = value;
			}
		}

		public bool IsInterfaceDefinition
		{
			//	Indique s'il s'agit d'un champ d'une interface.
			//	Reflète l'état de Support.Res.Fields.Field.IsInterfaceDefinition.
			//	Je n'ai pas compris la différence avec IsInterface juste en dessus !
			get
			{
				return this.isInterfaceDefinition;
			}
		}

		public bool IsEditExpressionEnabled
		{
			//	Retourne true s'il est possible d'éditer l'expression de ce champ.
			get
			{
				return !string.IsNullOrEmpty(this.expression) || !string.IsNullOrEmpty(this.deepExpression);
			}
		}

		public string DefiningName
		{
			get
			{
				return this.definingName;
			}
		}

		public StructuredTypeClass DefiningType
		{
			get
			{
				return this.definingType;
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

		public string TypeName
		{
			//	Nom du type.
			get
			{
				return this.typeName;
			}
			set
			{
				if (this.typeName != value)
				{
					this.typeName = value;
					this.UpdateTypeName();
				}
			}
		}

		public string DeepExpression
		{
			//	Eventuelle expression lambda calculant le champ.
			get
			{
				return this.deepExpression;
			}
		}

		public string Expression
		{
			//	Eventuelle expression lambda calculant le champ.
			get
			{
				return this.expression;
			}
			set
			{
				this.expression = value;
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

		public Druid DefiningTypeId
		{
			//	Druid définissant l'interface.
			get
			{
				return this.definingTypeId;
			}
		}

		public Druid DeepDefiningTypeId
		{
			//	Druid du parent définissant l'interface.
			get
			{
				return this.deepDefiningTypeId;
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


		protected void UpdateTypeName()
		{
			//	Met à jour le nom du type.
			if (this.isNullable)
			{
				this.textLayoutType.Text = string.Concat("(", this.typeName, ")");
			}
			else
			{
				this.textLayoutType.Text = this.typeName;
			}
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


		protected Editor editor;
		protected bool isTitle;
		protected bool isSubtitle;
		protected bool isInherited;
		protected bool isInterface;
		protected bool isInterfaceDefinition;
		protected string definingName;
		protected StructuredTypeClass definingType;
		protected int level;
		protected bool isGroupTop;
		protected bool isGroupBottom;
		protected TextLayout textLayoutField;
		protected TextLayout textLayoutType;
		protected string typeName;
		protected string deepExpression;
		protected string expression;
		protected FieldRelation relation;
		protected FieldMembership membership;
		protected Druid captionId;
		protected Druid definingTypeId;
		protected Druid deepDefiningTypeId;
		protected Druid destination;
		protected bool isNullable;
		protected bool isPrivateRelation;
		protected CultureMapSource cultureMapSource;
		protected int rank;
		protected int index;
		protected ObjectBox srcBox;
		protected ObjectBox dstBox;
		protected ObjectConnection connection;
		protected bool isExplored;
		protected bool isSourceExpanded;
		protected bool isAttachToRight;
		protected RouteType routeType;
		protected double routeRelativeAX1;
		protected double routeRelativeAX2;
		protected double routeAbsoluteAY;
		protected double routeRelativeBX;
		protected double routeRelativeBY;
		protected double routeRelativeCX;
		protected double routeAbsoluteDX;
		protected bool hasComment;
		protected Point commentPosition;
		protected Rectangle commentBounds;
		protected string commentText;
		protected double commentAttach;
		protected AbstractObject.MainColor commentMainColor;
	}
}
