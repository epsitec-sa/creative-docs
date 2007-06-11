using System.Collections.Generic;
using System.Runtime.Serialization;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.EntitiesEditor
{
	/// <summary>
	/// Cette classe contient toutes les informations relatives à une ligne, c'est-à-dire à un champ.
	/// </summary>
	[System.Serializable()]
	public class Field : ISerializable
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
			this.destination = Druid.Empty;
			this.rank = -1;
			this.isExplored = false;
			this.isSourceExpanded = false;
			
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
			this.commentText = "Commentaire libre, que vous pouvez modifier à volonté.";
		}

		public string FieldName
		{
			//	Nom du champ.
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
				return this.textLayoutType.Text;
			}
			set
			{
				this.textLayoutType.Text = value;
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

		public Druid Destination
		{
			//	Destination de la relation éventuelle du champ.
			get
			{
				return this.destination;
			}
			set
			{
				this.destination = value;
			}
		}

		public int Rank
		{
			//	Rang du champ dans le tableau (numéro de la ligne, 0..n)
			get
			{
				return this.rank;
			}
			set
			{
				this.rank = value;
			}
		}

		public ObjectBox SrcBox
		{
			//	Objet source de la connection
			get
			{
				return this.srcBox;
			}
			set
			{
				this.srcBox = value;
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
			get
			{
				return this.isAttachToRight;
			}
			set
			{
				this.isAttachToRight = value;
			}
		}


		public bool AsComment
		{
			//	Est-ce qu'un commentaire est attaché ?
			get
			{
				return this.asComment;
			}
			set
			{
				this.asComment = value;
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

		
		#region Serialization
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise l'objet.
			info.AddValue("DestinationDruid", this.destination.ToString());

			info.AddValue("SrcBoxDruid", this.GetDruid(this.srcBox).ToString());
			info.AddValue("DstBoxDruid", this.GetDruid(this.dstBox).ToString());

			info.AddValue("IsExplored", this.isExplored);
			info.AddValue("IsSourceExpanded", this.isSourceExpanded);
			info.AddValue("IsAttachToRight", this.isAttachToRight);

			info.AddValue("RouteType", this.routeType);
			info.AddValue("RouteRelativeAX1", this.routeRelativeAX1);
			info.AddValue("RouteRelativeAX2", this.routeRelativeAX2);
			info.AddValue("RouteAbsoluteAY", this.routeAbsoluteAY);
			info.AddValue("RouteRelativeBX", this.routeRelativeBX);
			info.AddValue("RouteRelativeCX", this.routeRelativeCX);
			info.AddValue("RouteAbsoluteDX", this.routeAbsoluteDX);

			info.AddValue("AsComment", this.asComment);
			info.AddValue("CommentPosition", this.commentPosition);
			info.AddValue("CommentBounds", this.commentBounds);
			info.AddValue("CommentText", this.commentText);
			info.AddValue("CommentAttach", this.commentAttach);
			info.AddValue("CommentMainColor", this.commentMainColor);
		}

		protected Druid GetDruid(ObjectBox box)
		{
			if (box == null)
			{
				return Druid.Empty;
			}
			else
			{
				return box.CultureMap.Id;
			}
		}

		protected Field(SerializationInfo info, StreamingContext context)
		{
			//	Constructeur qui désérialise l'objet.
			this.destination = Druid.Parse(info.GetString("DestinationDruid"));
			this.srcBoxDruid = Druid.Parse(info.GetString("SrcBoxDruid"));
			this.dstBoxDruid = Druid.Parse(info.GetString("DstBoxDruid"));

			this.isExplored = info.GetBoolean("IsExplored");
			this.isSourceExpanded = info.GetBoolean("IsSourceExpanded");
			this.isAttachToRight = info.GetBoolean("IsAttachToRight");

			this.routeType = (RouteType) info.GetValue("RouteType", typeof(RouteType));
			this.routeRelativeAX1 = info.GetDouble("RouteRelativeAX1");
			this.routeRelativeAX2 = info.GetDouble("RouteRelativeAX2");
			this.routeAbsoluteAY = info.GetDouble("RouteAbsoluteAY");
			this.routeRelativeBX = info.GetDouble("RouteRelativeBX");
			this.routeRelativeCX = info.GetDouble("RouteRelativeCX");
			this.routeAbsoluteDX = info.GetDouble("RouteAbsoluteDX");

			this.asComment = info.GetBoolean("AsComment");
			this.commentPosition = (Point) info.GetValue("CommentPosition", typeof(Point));
			this.commentBounds = (Rectangle) info.GetValue("CommentBounds", typeof(Rectangle));
			this.commentText = info.GetString("CommentText");
			this.commentAttach = info.GetDouble("CommentAttach");
			this.commentMainColor = (AbstractObject.MainColor) info.GetValue("CommentMainColor", typeof(AbstractObject.MainColor));
		}

		public void Restore(Field rField)
		{
			//	Restore un objet d'après un objet désérialisé (rField).
			this.srcBox = this.RestoreSearchBox(this.srcBoxDruid);
			this.dstBox = this.RestoreSearchBox(this.dstBoxDruid);

			this.isExplored = rField.isExplored;
			this.isSourceExpanded = rField.isSourceExpanded;
			this.isAttachToRight = rField.isAttachToRight;

			this.routeType = rField.routeType;
			this.routeRelativeAX1 = rField.routeRelativeAX1;
			this.routeRelativeAX2 = rField.routeRelativeAX2;
			this.routeAbsoluteAY = rField.routeAbsoluteAY;
			this.routeRelativeBX = rField.routeRelativeBX;
			this.routeRelativeCX = rField.routeRelativeCX;
			this.routeAbsoluteDX = rField.routeAbsoluteDX;

			this.asComment = rField.asComment;
			this.commentPosition = rField.commentPosition;
			this.commentBounds = rField.commentBounds;
			this.commentText = rField.commentText;
			this.commentAttach = rField.commentAttach;
			this.commentMainColor = rField.commentMainColor;
		}

		protected ObjectBox RestoreSearchBox(Druid druid)
		{
			foreach (ObjectBox box in this.editor.Boxes)
			{
				if (box.CultureMap.Id == druid)
				{
					return box;
				}
			}

			return null;
		}
		#endregion


		protected Editor editor;
		protected TextLayout textLayoutField;
		protected TextLayout textLayoutType;
		protected FieldRelation relation;
		protected Druid destination;
		protected int rank;
		protected ObjectBox srcBox;
		protected ObjectBox dstBox;
		protected Druid srcBoxDruid;
		protected Druid dstBoxDruid;
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
		protected bool asComment;
		protected Point commentPosition;
		protected Rectangle commentBounds;
		protected string commentText;
		protected double commentAttach;
		protected AbstractObject.MainColor commentMainColor;
	}
}
