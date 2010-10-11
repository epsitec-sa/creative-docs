//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.Core.Entities;

using System.Xml;
using System.Xml.Serialization;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WorkflowDesigner.Objects
{
	/// <summary>
	/// Cette classe contient toutes les informations relatives à une ligne, c'est-à-dire à un champ.
	/// </summary>
	public class Link
	{
		public Link(Editor editor, LinkableObject srcNode)
		{
			System.Diagnostics.Debug.Assert (editor != null);

			this.editor = editor;
			this.srcNode = srcNode;

			this.index = -1;
			
			this.routeType = RouteType.Close;
			this.routeRelativeAX1 = 0.2;
			this.routeRelativeAX2 = 0.8;
			this.routeAbsoluteAY  = 0.0;
			this.routeRelativeBX  = 0.0;
			this.routeRelativeBY  = 0.0;
			this.routeRelativeCX  = 0.5;
			this.routeAbsoluteDX  = 0.0;

			this.commentAttach = AbstractObject.minAttach;
			this.commentMainColor = MainColor.Yellow;
			this.commentText = "Coucou";
		}


		public int Index
		{
			get
			{
				return this.index;
			}
			set
			{
				this.index = value;
			}
		}

		public LinkableObject SrcNode
		{
			//	Objet source de la connexion
			get
			{
				return this.srcNode;
			}
		}

		public LinkableObject DstNode
		{
			//	Objet destination de la connexion (si la connexion débouche sur un noeud).
			get
			{
				return this.dstNode;
			}
			set
			{
				this.dstNode = value;
			}
		}

		public ObjectLink ObjectLink
		{
			//	Objet connexion.
			get
			{
				return this.objectLink;
			}
			set
			{
				this.objectLink = value;
			}
		}

		public bool IsAttachToRight
		{
			//	Indique si la boîte source est attachée à droite ou à gauche.
			//	Direction dans laquelle part la connexion.
			//	En fait, il s'agit du bouton ConnexionOpenLeft/Right utilisé pour ouvrir
			//	la connexion, et non de la direction effective dans laquelle part la
			//	connexion !
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
			//	Une distance positive commence depuis le début de la connexion.
			//	Une distance négative commence depuis la fin de la connexion.
			get
			{
				return this.commentAttach;
			}
			set
			{
				this.commentAttach = value;
			}
		}

		public MainColor CommentMainColor
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
			//	Force un routage standard pour la connexion.
			this.Route = RouteType.Close;
		}

		public RouteType Route
		{
			//	Type de routage de la connexion.
			get
			{
				return this.routeType;
			}
			set
			{
				if (this.routeType != value)
				{
					this.routeType = value;
					this.objectLink.UpdateRoute ();
				}
			}
		}

		public double RouteRelativeAX1
		{
			//	Position intermédiaire de la connexion (cas A).
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

					this.objectLink.UpdateRoute ();
					this.editor.Invalidate();
				}
			}
		}

		public double RouteRelativeAX2
		{
			//	Position intermédiaire de la connexion (cas A).
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

					this.objectLink.UpdateRoute ();
					this.editor.Invalidate();
				}
			}
		}

		public double RouteAbsoluteAY
		{
			//	Position intermédiaire de la connexion (cas A).
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

					this.objectLink.UpdateRoute ();
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
			//	Position intermédiaire de la connexion (cas B).
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

					this.objectLink.UpdateRoute ();
					this.editor.Invalidate();
				}
			}
		}

		public double RouteRelativeBY
		{
			//	Position intermédiaire de la connexion (cas B).
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

					this.objectLink.UpdateRoute ();
					this.editor.Invalidate();
				}
			}
		}

		public double RouteRelativeCX
		{
			//	Position intermédiaire de la connexion (cas C).
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

					this.objectLink.UpdateRoute ();
					this.editor.Invalidate();
				}
			}
		}

		public double RouteAbsoluteDX
		{
			//	Position intermédiaire de la connexion (cas D).
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

					this.objectLink.UpdateRoute ();
					this.editor.Invalidate();
				}
			}
		}


		#region Serialization
		public void WriteXml(XmlWriter writer)
		{
#if false
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
#endif
		}

		public void ReadXml(XmlReader reader)
		{
#if false
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
#endif
		}

		public void DeserializeCopyTo(Link dst)
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


		private Editor						editor;
		private int							index;
		private LinkableObject				srcNode;
		private LinkableObject				dstNode;
		private ObjectLink					objectLink;
		private bool						isAttachToRight;
		private RouteType					routeType;
		private double						routeRelativeAX1;
		private double						routeRelativeAX2;
		private double						routeAbsoluteAY;
		private double						routeRelativeBX;
		private double						routeRelativeBY;
		private double						routeRelativeCX;
		private double						routeAbsoluteDX;
		private bool						hasComment;
		private Point						commentPosition;
		private Rectangle					commentBounds;
		private string						commentText;
		private double						commentAttach;
		private MainColor					commentMainColor;
	}
}
