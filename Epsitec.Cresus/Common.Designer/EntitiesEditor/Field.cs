using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.EntitiesEditor
{
	/// <summary>
	/// Cette classe contient toutes les informations relatives � une ligne, c'est-�-dire � un champ.
	/// </summary>
	public class Field
	{
		public enum RouteType
		{
			// Cas A:
			// o--------->
			//
			// Cas A personnalis�:
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
			// Cas Bt personnalis�:
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
			// Cas Bb personnalis�:
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
			// Les cas A et B ont un routage automatique ou personnalis�.
			// 'x' = poign�e pour personnaliser le routage.

			Close,		// connection ferm�e
			Himself,	// connection sur soi-m�me
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
			this.commentText = "Commentaire libre, que vous pouvez modifier � volont�.";
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
			//	Retourne le TextLayout utilis� pour le nom du champ.
			get
			{
				return this.textLayoutField;
			}
		}

		public TextLayout TextLayoutType
		{
			//	Retourne le TextLayout utilis� pour le type du champ.
			get
			{
				return this.textLayoutType;
			}
		}

		public FieldRelation Relation
		{
			//	Type de la relation �ventuelle du champ.
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
			//	Destination de la relation �ventuelle du champ.
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
			//	Rang du champ dans le tableau (num�ro de la ligne, 0..n)
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
			//	Objet destination de la connection (si la relation est explor�e).
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
			//	Indique si une relation est explor�e, c'est-�-dire si l'entit� destination est visible.
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
			//	Indique si la bo�te source d'une relation est �tendue.
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
			//	Indique si la bo�te source est attach�e � droite ou � gauche.
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
			//	Est-ce qu'un commentaire est attach� ?
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
			//	Bo�te du commentaire.
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
			//	Une distance positive commence depuis le d�but de la connection.
			//	Une distance n�gative commence depuis la fin de la connection.
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
			//	Position interm�diaire de la connection (cas A).
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
			//	Position interm�diaire de la connection (cas A).
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
			//	Position interm�diaire de la connection (cas A).
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
			//	Position interm�diaire de la connection (cas B).
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
			//	Position interm�diaire de la connection (cas B).
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
			//	Position interm�diaire de la connection (cas C).
			//	0.5 correspond au milieu (valeur par d�faut).
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
			//	Position interm�diaire de la connection (cas D).
			//	0.0 correspond � la boucle la plus serr�e (valeur par d�faut).
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

		
		protected Editor editor;
		protected TextLayout textLayoutField;
		protected TextLayout textLayoutType;
		protected FieldRelation relation;
		protected Druid destination;
		protected int rank;
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
		protected bool asComment;
		protected Point commentPosition;
		protected Rectangle commentBounds;
		protected string commentText;
		protected double commentAttach;
		protected AbstractObject.MainColor commentMainColor;
	}
}
