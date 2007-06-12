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
			this.captionId = Druid.Empty;
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
		public void WriteXml(XmlWriter writer)
		{
			//	Sérialise toutes les informations du champ.
			writer.WriteStartElement("Field");
			
			writer.WriteElementString("DruidCaptionId", this.captionId.ToString());
			writer.WriteElementString("DruidDestination", this.destination.ToString());

			if (this.isExplored)
			{
				writer.WriteElementString("IsAttachToRight", this.isAttachToRight.ToString(System.Globalization.CultureInfo.InvariantCulture));

				writer.WriteElementString("RouteType", this.routeType.ToString());

				if (this.routeRelativeAX1 != 0.2)
				{
					writer.WriteElementString("RouteRelativeAX1", this.routeRelativeAX1.ToString(System.Globalization.CultureInfo.InvariantCulture));
				}

				if (this.routeRelativeAX2 != 0.8)
				{
					writer.WriteElementString("RouteRelativeAX2", this.routeRelativeAX2.ToString(System.Globalization.CultureInfo.InvariantCulture));
				}

				if (this.routeAbsoluteAY != 0.0)
				{
					writer.WriteElementString("RouteAbsoluteAY", this.routeAbsoluteAY.ToString(System.Globalization.CultureInfo.InvariantCulture));
				}

				if (this.routeRelativeBX != 0.0)
				{
					writer.WriteElementString("RouteRelativeBX", this.routeRelativeBX.ToString(System.Globalization.CultureInfo.InvariantCulture));
				}

				if (this.routeRelativeBY != 0.0)
				{
					writer.WriteElementString("RouteRelativeBY", this.routeRelativeBY.ToString(System.Globalization.CultureInfo.InvariantCulture));
				}

				if (this.routeRelativeCX != 0.5)
				{
					writer.WriteElementString("RouteRelativeCX", this.routeRelativeCX.ToString(System.Globalization.CultureInfo.InvariantCulture));
				}

				if (this.routeAbsoluteDX != 0.5)
				{
					writer.WriteElementString("RouteAbsoluteDX", this.routeAbsoluteDX.ToString(System.Globalization.CultureInfo.InvariantCulture));
				}

				if (this.asComment)
				{
					writer.WriteElementString("CommentPosition", this.commentPosition.ToString());
					writer.WriteElementString("CommentBounds", this.commentBounds.ToString());
					writer.WriteElementString("CommentText", this.commentText);
					writer.WriteElementString("CommentAttach", this.commentAttach.ToString(System.Globalization.CultureInfo.InvariantCulture));
					writer.WriteElementString("CommentColor", this.commentMainColor.ToString());
				}
			}

			writer.WriteEndElement();
		}

		public void ReadXml(XmlReader reader)
		{
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					string name = reader.LocalName;
					string element = reader.ReadElementString();

					if (name == "DruidCaptionId")
					{
						Druid druid = Druid.Parse(element);
						if (druid.IsValid)
						{
							this.captionId = druid;
						}
					}
					else if (name == "DruidDestination")
					{
						Druid druid = Druid.Parse(element);
						if (druid.IsValid)
						{
							this.destination = druid;
						}
					}
					else if (name == "IsAttachToRight")
					{
						this.isExplored = true;
						this.isAttachToRight = bool.Parse(element);
					}
					else if (name == "RouteType")
					{
						this.routeType = (RouteType) System.Enum.Parse(typeof(RouteType), element);
					}
					else if (name == "RouteRelativeAX1")
					{
						this.routeRelativeAX1 = double.Parse(element);
					}
					else if (name == "RouteRelativeAX2")
					{
						this.routeRelativeAX2 = double.Parse(element);
					}
					else if (name == "RouteAbsoluteAY")
					{
						this.routeAbsoluteAY = double.Parse(element);
					}
					else if (name == "RouteRelativeBX")
					{
						this.routeRelativeBX = double.Parse(element);
					}
					else if (name == "RouteRelativeBY")
					{
						this.routeRelativeBY = double.Parse(element);
					}
					else if (name == "RouteRelativeCX")
					{
						this.routeRelativeCX = double.Parse(element);
					}
					else if (name == "RouteAbsoluteDX")
					{
						this.routeAbsoluteDX = double.Parse(element);
					}
					else if (name == "CommentPosition")
					{
						this.asComment = true;
						this.commentPosition = Point.Parse(element);
					}
					else if (name == "CommentBounds")
					{
						this.commentBounds = Rectangle.Parse(element);
					}
					else if (name == "CommentText")
					{
						this.commentText = element;
					}
					else if (name == "CommentAttach")
					{
						this.commentAttach = double.Parse(element);
					}
					else if (name == "CommentColor")
					{
						this.commentMainColor = (AbstractObject.MainColor) System.Enum.Parse(typeof(AbstractObject.MainColor), element);
					}
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					break;
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
			dst.asComment = this.asComment;
			dst.commentPosition = this.commentPosition;
			dst.commentBounds = this.commentBounds;
			dst.commentText = this.commentText;
			dst.commentAttach = this.commentAttach;
			dst.commentMainColor = this.commentMainColor;
		}
		#endregion


		protected Editor editor;
		protected TextLayout textLayoutField;
		protected TextLayout textLayoutType;
		protected FieldRelation relation;
		protected Druid captionId;
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
