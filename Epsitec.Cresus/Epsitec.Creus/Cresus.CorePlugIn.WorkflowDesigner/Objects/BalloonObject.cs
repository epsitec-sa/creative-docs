//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.CorePlugIn.WorkflowDesigner.Objects
{
	public abstract class BalloonObject : AbstractObject
	{
		protected enum AttachMode
		{
			None,
			Left,
			Right,
			Bottom,
			Top,
			BottomLeft,
			BottomRight,
			TopLeft,
			TopRight,
		}


		public BalloonObject(Editor editor, AbstractEntity entity)
			: base (editor, entity)
		{
			this.textLayoutTitle = new TextLayout();
			this.textLayoutTitle.DefaultFontSize = 14;
			this.textLayoutTitle.BreakMode = TextBreakMode.SingleLine | TextBreakMode.Ellipsis;
			this.textLayoutTitle.Alignment = ContentAlignment.MiddleCenter;
			this.textLayoutTitle.Text = "Commentaires";
		}


		public AbstractObject AttachObject
		{
			//	Object liée (boîte ou connexion).
			get
			{
				return this.attachObject;
			}
			set
			{
				this.attachObject = value;
			}
		}

		public Rectangle InternalBounds
		{
			//	Retourne la boîte de l'objet.
			get
			{
				return this.bounds;
			}
		}

		public override void Move(double dx, double dy)
		{
			//	Déplace l'objet.
			this.bounds.Offset (dx, dy);
			this.UpdateGeometry ();
		}


		public override List<AbstractObject> FriendObjects
		{
			//	Les objets amis sont toutes les connexions qui partent ou arrivent de cet objet.
			get
			{
				var list = new List<AbstractObject> ();

				if (this.attachObject != null)
				{
					list.Add (this.attachObject);
				}

				return list;
			}
		}


		public override void DrawBackground(Graphics graphics)
		{
			//	Dessine le fond de l'objet.
			base.DrawBackground (graphics);

			Rectangle rect;
			Rectangle rh = Rectangle.Empty;
			if (this.hilitedElement != ActiveElement.None)
			{
				rh = this.HeaderRectangle;
			}

			//	Dessine l'ombre.
			rect = this.bounds;
			if (this.draggingMode == DraggingMode.None)
			{
				rect = Rectangle.Union (rect, rh);
			}
			rect.Inflate (2);
			rect.Offset (AbstractObject.shadowOffset, -AbstractObject.shadowOffset);
			this.DrawRoundShadow (graphics, rect, 4, (int) AbstractObject.shadowOffset, 0.2);

			//	Dessine l'en-tête.
			if (!rh.IsEmpty)
			{
				rect = rh;
				rect.Inflate (0.5);
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (this.ColorCommentHeader (this.hilitedElement == ActiveElement.CommentMove, this.draggingMode != DraggingMode.None));
				graphics.AddRectangle (rect);
				graphics.RenderSolid (this.colorFactory.GetColor (0, (this.draggingMode == DraggingMode.None) ? 1 : 0.2));

				rect.Width -= rect.Height;  // place pur le bouton 'x' à droite
				rect.Offset (0, 1);
				this.textLayoutTitle.LayoutSize = rect.Size;
				this.textLayoutTitle.Paint (rect.BottomLeft, graphics, Rectangle.MaxValue, this.colorFactory.GetColor (1), GlyphPaintStyle.Normal);
			}

			//	Dessine la boîte vide avec la queue (bulle de bd).
			Path path = this.GetFramePath ();

			graphics.Rasterizer.AddSurface (path);
			graphics.RenderSolid (this.ColorComment (this.hilitedElement != ActiveElement.None));

			graphics.Rasterizer.AddOutline (path);
			graphics.RenderSolid (this.colorFactory.GetColor (0));
		}

		public override void DrawForeground(Graphics graphics)
		{
			//	Dessine le dessus de l'objet.
			base.DrawForeground (graphics);
		}

		private Path GetFramePath()
		{
			//	Retourne le chemin du cadre du commentaire (rectangle avec éventuellement une queue,
			//	comme une bulle de bd).
			Path path = new Path();

			AttachMode mode = this.GetAttachMode();
			Point himself = this.GetAttachHimself(mode);
			Point other = this.GetAttachOther(mode);

			double d = Point.Distance (himself, other) - this.AttachRadius;

			if (this.attachObject is LinkableObject)
			{
				other = Point.Move (other, himself, this.AttachRadius);
			}
			
			Rectangle bounds = this.bounds;
			bounds.Inflate(0.5);

			if (mode == AttachMode.None || himself.IsZero || other.IsZero || d <= 0)
			{
				path.AppendRectangle(bounds);
			}
			else if (mode == AttachMode.Left)
			{
				Point h1 = himself;
				Point h2 = himself;

				h1.Y -= ObjectComment.queueThickness;
				h2.Y += ObjectComment.queueThickness;
				
				if (h1.Y < bounds.Bottom)
				{
					h2.Y += bounds.Bottom-h1.Y;
					h1.Y = bounds.Bottom;
				}
				
				if (h2.Y > bounds.Top)
				{
					h1.Y -= h2.Y-bounds.Top;
					h2.Y = bounds.Top;
				}

				path.MoveTo(other);
				path.LineTo(h1);
				path.LineTo(bounds.BottomLeft);
				path.LineTo(bounds.BottomRight);
				path.LineTo(bounds.TopRight);
				path.LineTo(bounds.TopLeft);
				path.LineTo(h2);
				path.Close();
			}
			else if (mode == AttachMode.Right)
			{
				Point h1 = himself;
				Point h2 = himself;

				h1.Y -= ObjectComment.queueThickness;
				h2.Y += ObjectComment.queueThickness;
				
				if (h1.Y < bounds.Bottom)
				{
					h2.Y += bounds.Bottom-h1.Y;
					h1.Y = bounds.Bottom;
				}
				
				if (h2.Y > bounds.Top)
				{
					h1.Y -= h2.Y-bounds.Top;
					h2.Y = bounds.Top;
				}

				path.MoveTo(other);
				path.LineTo(h1);
				path.LineTo(bounds.BottomRight);
				path.LineTo(bounds.BottomLeft);
				path.LineTo(bounds.TopLeft);
				path.LineTo(bounds.TopRight);
				path.LineTo(h2);
				path.Close();
			}
			else if (mode == AttachMode.Bottom)
			{
				Point h1 = himself;
				Point h2 = himself;

				h1.X -= ObjectComment.queueThickness;
				h2.X += ObjectComment.queueThickness;
				
				if (h1.X < bounds.Left)
				{
					h2.X += bounds.Left-h1.X;
					h1.X = bounds.Left;
				}
				
				if (h2.X > bounds.Right)
				{
					h1.X -= h2.X-bounds.Right;
					h2.X = bounds.Right;
				}

				path.MoveTo(other);
				path.LineTo(h1);
				path.LineTo(bounds.BottomLeft);
				path.LineTo(bounds.TopLeft);
				path.LineTo(bounds.TopRight);
				path.LineTo(bounds.BottomRight);
				path.LineTo(h2);
				path.Close();
			}
			else if (mode == AttachMode.Top)
			{
				Point h1 = himself;
				Point h2 = himself;

				h1.X -= ObjectComment.queueThickness;
				h2.X += ObjectComment.queueThickness;
				
				if (h1.X < bounds.Left)
				{
					h2.X += bounds.Left-h1.X;
					h1.X = bounds.Left;
				}
				
				if (h2.X > bounds.Right)
				{
					h1.X -= h2.X-bounds.Right;
					h2.X = bounds.Right;
				}

				path.MoveTo(other);
				path.LineTo(h1);
				path.LineTo(bounds.TopLeft);
				path.LineTo(bounds.BottomLeft);
				path.LineTo(bounds.BottomRight);
				path.LineTo(bounds.TopRight);
				path.LineTo(h2);
				path.Close();
			}
			else if (mode == AttachMode.BottomLeft)
			{
				Point h1 = himself;
				Point h2 = himself;

				h1.Y += ObjectComment.queueThickness*System.Math.Sqrt(2);
				h2.X += ObjectComment.queueThickness*System.Math.Sqrt(2);
				
				path.MoveTo(other);
				path.LineTo(h1);
				path.LineTo(bounds.TopLeft);
				path.LineTo(bounds.TopRight);
				path.LineTo(bounds.BottomRight);
				path.LineTo(h2);
				path.Close();
			}
			else if (mode == AttachMode.BottomRight)
			{
				Point h1 = himself;
				Point h2 = himself;

				h1.Y += ObjectComment.queueThickness*System.Math.Sqrt(2);
				h2.X -= ObjectComment.queueThickness*System.Math.Sqrt(2);
				
				path.MoveTo(other);
				path.LineTo(h1);
				path.LineTo(bounds.TopRight);
				path.LineTo(bounds.TopLeft);
				path.LineTo(bounds.BottomLeft);
				path.LineTo(h2);
				path.Close();
			}
			else if (mode == AttachMode.TopLeft)
			{
				Point h1 = himself;
				Point h2 = himself;

				h1.Y -= ObjectComment.queueThickness*System.Math.Sqrt(2);
				h2.X += ObjectComment.queueThickness*System.Math.Sqrt(2);
				
				path.MoveTo(other);
				path.LineTo(h1);
				path.LineTo(bounds.BottomLeft);
				path.LineTo(bounds.BottomRight);
				path.LineTo(bounds.TopRight);
				path.LineTo(h2);
				path.Close();
			}
			else if (mode == AttachMode.TopRight)
			{
				Point h1 = himself;
				Point h2 = himself;

				h1.Y -= ObjectComment.queueThickness*System.Math.Sqrt(2);
				h2.X -= ObjectComment.queueThickness*System.Math.Sqrt(2);
				
				path.MoveTo(other);
				path.LineTo(h1);
				path.LineTo(bounds.BottomRight);
				path.LineTo(bounds.BottomLeft);
				path.LineTo(bounds.TopLeft);
				path.LineTo(h2);
				path.Close();
			}

			return path;
		}

		private Point GetAttachHimself(AttachMode mode)
		{
			//	Retourne le point d'attache sur le commentaire.
			Point pos = Point.Zero;

			if (mode != AttachMode.None)
			{
				Rectangle bounds = this.bounds;
				bounds.Inflate(0.5);

				if (mode == AttachMode.BottomLeft)
				{
					pos = bounds.BottomLeft;
				}

				if (mode == AttachMode.BottomRight)
				{
					pos = bounds.BottomRight;
				}

				if (mode == AttachMode.TopLeft)
				{
					pos = bounds.TopLeft;
				}

				if (mode == AttachMode.TopRight)
				{
					pos = bounds.TopRight;
				}

				if (pos.IsZero && this.attachObject is LinkableObject)
				{
					var box = this.attachObject as LinkableObject;
					Rectangle boxBounds = box.Bounds;
					boxBounds.Deflate (this.AttachRadius);

					if (mode == AttachMode.Left || mode == AttachMode.Right)
					{
						pos.X = (mode == AttachMode.Left) ? bounds.Left : bounds.Right;

						double miny = System.Math.Max(boxBounds.Bottom, bounds.Bottom);
						double maxy = System.Math.Min(boxBounds.Top, bounds.Top);

						if (miny <= maxy)
						{
							pos.Y = (miny+maxy)/2;
						}
						else
						{
							pos.Y = (bounds.Top < boxBounds.Top) ? bounds.Top : bounds.Bottom;
						}
					}

					if (mode == AttachMode.Bottom || mode == AttachMode.Top)
					{
						pos.Y = (mode == AttachMode.Bottom) ? bounds.Bottom : bounds.Top;

						double minx = System.Math.Max(boxBounds.Left, bounds.Left);
						double maxx = System.Math.Min(boxBounds.Right, bounds.Right);

						if (minx <= maxx)
						{
							pos.X = (minx+maxx)/2;
						}
						else
						{
							pos.X = (bounds.Right < boxBounds.Right) ? bounds.Right : bounds.Left;
						}
					}
				}

				if (pos.IsZero && this.attachObject is ObjectLink)
				{
					ObjectLink link = this.attachObject as ObjectLink;
					Point attach = link.PositionLinkComment;

					if (mode == AttachMode.Left || mode == AttachMode.Right)
					{
						pos.X = (mode == AttachMode.Left) ? bounds.Left : bounds.Right;

						if (attach.Y < bounds.Bottom)
						{
							pos.Y = bounds.Bottom;
						}
						else if (attach.Y > bounds.Top)
						{
							pos.Y = bounds.Top;
						}
						else
						{
							pos.Y = attach.Y;
						}
					}

					if (mode == AttachMode.Bottom || mode == AttachMode.Top)
					{
						pos.Y = (mode == AttachMode.Bottom) ? bounds.Bottom : bounds.Top;

						if (attach.X < bounds.Left)
						{
							pos.X = bounds.Left;
						}
						else if (attach.X > bounds.Right)
						{
							pos.X = bounds.Right;
						}
						else
						{
							pos.X = attach.X;
						}
					}
				}
			}

			return pos;
		}

		private Point GetAttachOther(AttachMode mode)
		{
			//	Retourne le point d'attache sur l'objet lié (boîte ou commentaire).
			Point pos = Point.Zero;

			if (mode != AttachMode.None)
			{
				Rectangle bounds = this.bounds;
				bounds.Inflate(0.5);

				if (this.attachObject is LinkableObject)
				{
					var box = this.attachObject as LinkableObject;
					Rectangle boxBounds = box.Bounds;
					boxBounds.Deflate (this.AttachRadius);

					if (mode == AttachMode.BottomLeft)
					{
						return boxBounds.TopRight;
					}

					if (mode == AttachMode.BottomRight)
					{
						return boxBounds.TopLeft;
					}

					if (mode == AttachMode.TopLeft)
					{
						return boxBounds.BottomRight;
					}

					if (mode == AttachMode.TopRight)
					{
						return boxBounds.BottomLeft;
					}
					
					Point himself = this.GetAttachHimself(mode);

					if (mode == AttachMode.Left || mode == AttachMode.Right)
					{
						pos.X = (mode == AttachMode.Left) ? boxBounds.Right : boxBounds.Left;

						if (himself.Y < boxBounds.Bottom)
						{
							pos.Y = boxBounds.Bottom;
						}
						else if (himself.Y > boxBounds.Top)
						{
							pos.Y = boxBounds.Top;
						}
						else
						{
							pos.Y = himself.Y;
						}
					}

					if (mode == AttachMode.Bottom || mode == AttachMode.Top)
					{
						pos.Y = (mode == AttachMode.Bottom) ? boxBounds.Top : boxBounds.Bottom;

						if (himself.X < boxBounds.Left)
						{
							pos.X = boxBounds.Left;
						}
						else if (himself.X > boxBounds.Right)
						{
							pos.X = boxBounds.Right;
						}
						else
						{
							pos.X = himself.X;
						}
					}
				}

				if (this.attachObject is ObjectLink)
				{
					ObjectLink link = this.attachObject as ObjectLink;
					pos = link.PositionLinkComment;

					if (mode == AttachMode.Bottom || mode == AttachMode.BottomLeft || mode == AttachMode.BottomRight)
					{
						pos.Y += 2;
					}

					if (mode == AttachMode.Top || mode == AttachMode.TopLeft || mode == AttachMode.TopRight)
					{
						pos.Y -= 2;
					}

					if (mode == AttachMode.Left || mode == AttachMode.BottomLeft || mode == AttachMode.TopLeft)
					{
						pos.X += 2;
					}

					if (mode == AttachMode.Right || mode == AttachMode.BottomRight || mode == AttachMode.TopRight)
					{
						pos.X -= 2;
					}
				}
			}

			return pos;
		}

		protected AttachMode GetAttachMode()
		{
			//	Cherche d'où doit partir la queue du commentaire (de quel côté).
			if (this.attachObject is LinkableObject)
			{
				var box = this.attachObject as LinkableObject;
				Rectangle boxBounds = box.Bounds;
				boxBounds.Deflate (this.AttachRadius);

				if (!this.bounds.IntersectsWith(boxBounds))
				{
					if (this.bounds.Bottom >= boxBounds.Top && this.bounds.Right <= boxBounds.Left)
					{
						return AttachMode.BottomRight;
					}
					
					if (this.bounds.Top <= boxBounds.Bottom && this.bounds.Right <= boxBounds.Left)
					{
						return AttachMode.TopRight;
					}
					
					if (this.bounds.Bottom >= boxBounds.Top && this.bounds.Left >= boxBounds.Right)
					{
						return AttachMode.BottomLeft;
					}
					
					if (this.bounds.Top <= boxBounds.Bottom && this.bounds.Left >= boxBounds.Right)
					{
						return AttachMode.TopLeft;
					}
					
					if (this.bounds.Bottom >= boxBounds.Top)  // commentaire en dessus ?
					{
						return AttachMode.Bottom;
					}
					
					if (this.bounds.Top <= boxBounds.Bottom)  // commentaire en dessous ?
					{
						return AttachMode.Top;
					}

					if (this.bounds.Left >= boxBounds.Right)  // commentaire à droite ?
					{
						return AttachMode.Left;
					}

					if (this.bounds.Right <= boxBounds.Left)  // commentaire à gauche ?
					{
						return AttachMode.Right;
					}
				}
			}

			if (this.attachObject is ObjectLink)
			{
				ObjectLink link = this.attachObject as ObjectLink;
				Point attach = link.PositionLinkComment;
				if (!attach.IsZero && !this.bounds.Contains(attach))
				{
					if (this.bounds.Top <= attach.Y && this.bounds.Right <= attach.X)
					{
						return AttachMode.TopRight;
					}

					if (this.bounds.Bottom >= attach.Y && this.bounds.Right <= attach.X)
					{
						return AttachMode.BottomRight;
					}

					if (this.bounds.Top <= attach.Y && this.bounds.Left >= attach.X)
					{
						return AttachMode.TopLeft;
					}

					if (this.bounds.Bottom >= attach.Y && this.bounds.Left >= attach.X)
					{
						return AttachMode.BottomLeft;
					}

					if (this.bounds.Bottom >= attach.Y)  // commentaire en dessus ?
					{
						return AttachMode.Bottom;
					}
					
					if (this.bounds.Top <= attach.Y)  // commentaire en dessous ?
					{
						return AttachMode.Top;
					}

					if (this.bounds.Left >= attach.X)  // commentaire à droite ?
					{
						return AttachMode.Left;
					}

					if (this.bounds.Right <= attach.X)  // commentaire à gauche ?
					{
						return AttachMode.Right;
					}
				}
			}

			return AttachMode.None;
		}


		private double AttachRadius
		{
			get
			{
				if (this.attachObject is ObjectEdge)
				{
					return this.attachObject.Bounds.Height/2;
				}
				else
				{
					return ObjectEdge.frameSize.Height/2;
				}
			}
		}


		protected Rectangle HeaderRectangle
		{
			get
			{
				Rectangle rect = this.bounds;
				rect.Bottom = rect.Top;
				rect.Height = ObjectComment.commentHeaderHeight;
				return rect;
			}
		}

		protected Point PositionCloseButton
		{
			//	Retourne la position du bouton de fermeture.
			get
			{
				Rectangle rect = this.HeaderRectangle;
				return new Point(rect.Right-rect.Height/2, rect.Center.Y);
			}
		}

		protected Point PositionWidthButton
		{
			//	Retourne la position du bouton pour modifier la largeur.
			get
			{
				return new Point(this.bounds.Right, this.bounds.Center.Y);
			}
		}

		protected Point PositionAttachToLinkButton
		{
			//	Retourne la position du bouton pour modifier l'attache à la connexion.
			get
			{
				if (this.attachObject != null && this.attachObject is ObjectLink)
				{
					ObjectLink link = this.attachObject as ObjectLink;
					return link.PositionLinkComment;
				}
				else
				{
					return Point.Zero;
				}
			}
		}

		protected Point PositionColorButton(int rank)
		{
			//	Retourne la position du bouton pour choisir la couleur.
			return new Point (this.bounds.Left+ActiveButton.buttonSquare+(ActiveButton.buttonSquare+0.5)*rank*2, this.bounds.Bottom-1-ActiveButton.buttonSquare);
		}

		protected Color ColorComment(bool hilited)
		{
			if (hilited)
			{
				return this.colorFactory.GetColorAdjusted (this.colorFactory.GetColorMain (), 0.3);
			}
			else
			{
				return this.colorFactory.GetColorAdjusted (this.colorFactory.GetColorMain (), 0.2);
			}
		}

		protected Color ColorCommentHeader(bool hilited, bool dragging)
		{
			if (dragging)
			{
				return this.colorFactory.GetColorMain (0.1);
			}
			else if (hilited)
			{
				return this.colorFactory.GetColorAdjusted (this.colorFactory.GetColorMain (), 0.9);
			}
			else
			{
				return this.colorFactory.GetColorAdjusted (this.colorFactory.GetColorMain (), 0.7);
			}
		}


		#region Serialize
		public override void Serialize(XElement xml)
		{
			base.Serialize (xml);

			xml.Add (new XAttribute ("obj", (this.attachObject == null) ? 0 : this.attachObject.UniqueId));
		}

		public override void Deserialize(XElement xml)
		{
			base.Deserialize (xml);

			this.attachObject = this.editor.Search ((int) xml.Attribute ("obj"));
		}
		#endregion


		protected static readonly double		commentHeaderHeight = 24;
		protected static readonly double		textMargin = 5;
		protected static readonly double		queueThickness = 5;

		protected AbstractObject				attachObject;
		protected TextLayout					textLayoutTitle;

		protected Point							draggingPos;
	}
}
