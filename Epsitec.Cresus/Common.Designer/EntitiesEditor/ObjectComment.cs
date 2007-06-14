using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.EntitiesEditor
{
	/// <summary>
	/// Boîte pour représenter une entité.
	/// </summary>
	public class ObjectComment : AbstractObject
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


		public ObjectComment(Editor editor) : base(editor)
		{
			this.isVisible = true;
			this.boxColor = MainColor.Yellow;

			this.textLayoutTitle = new TextLayout();
			this.textLayoutTitle.DefaultFontSize = 14;
			this.textLayoutTitle.BreakMode = TextBreakMode.SingleLine | TextBreakMode.Ellipsis;
			this.textLayoutTitle.Alignment = ContentAlignment.MiddleCenter;
			this.textLayoutTitle.Text = "<b>Commentaire</b>";

			this.textLayoutComment = new TextLayout();
			this.textLayoutComment.DefaultFontSize = 10;
			this.textLayoutComment.BreakMode = TextBreakMode.Hyphenate | TextBreakMode.Split;
			this.textLayoutComment.Text = "Commentaire libre, que vous pouvez modifier à volonté.";
		}


		public string Text
		{
			//	Texte du commentaire.
			get
			{
				return this.textLayoutComment.Text;
			}
			set
			{
				this.textLayoutComment.Text = value;
			}
		}

		public AbstractObject AttachObject
		{
			//	Object liée (boîte ou connection).
			get
			{
				return this.attachObject;
			}
			set
			{
				this.attachObject = value;
			}
		}

		public override Rectangle Bounds
		{
			//	Retourne la boîte de l'objet.
			//	Attention: le dessin peut déborder, par exemple pour l'ombre.
			get
			{
				return this.isVisible ? this.bounds : Rectangle.Empty;
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

		public void SetBounds(Rectangle bounds)
		{
			//	Modifie la boîte de l'objet.
			this.bounds = bounds;
		}

		public bool IsVisible
		{
			//	Est-ce que le commentaire est visible.
			get
			{
				return this.isVisible;
			}
			set
			{
				if (this.isVisible != value)
				{
					this.isVisible = value;

					this.editor.UpdateAfterCommentChanged();
					this.editor.DirtySerialization = true;
				}
			}
		}


		protected override string GetToolTipText(ActiveElement element)
		{
			//	Retourne le texte pour le tooltip.
			if (this.isDraggingMove || this.isDraggingWidth || this.isDraggingAttach)
			{
				return null;  // pas de tooltip
			}

			return base.GetToolTipText(element);
		}

		public override bool MouseMove(Point pos)
		{
			//	Met en évidence la boîte selon la position de la souris.
			//	Si la souris est dans cette boîte, retourne true.
			if (this.isDraggingMove)
			{
				Rectangle bounds = this.bounds;

				bounds.Offset(pos-this.draggingPos);
				this.draggingPos = pos;

				this.SetBounds(bounds);
				this.editor.Invalidate();
				return true;
			}
			else if (this.isDraggingWidth)
			{
				Rectangle bounds = this.bounds;

				bounds.Right = pos.X;
				bounds.Width = System.Math.Max(bounds.Width, AbstractObject.commentMinWidth);

				this.SetBounds(bounds);
				this.UpdateHeight();
				this.editor.Invalidate();
				return true;
			}
			else if (this.isDraggingAttach)
			{
				ObjectConnection connection = this.attachObject as ObjectConnection;

				double attach = connection.PointToAttach(pos);
				if (attach != 0)
				{
					Point oldPos = connection.PositionConnectionComment;
					connection.Field.CommentAttach = attach;
					Point newPos = connection.PositionConnectionComment;

					Rectangle bounds = this.bounds;
					bounds.Offset(newPos-oldPos);
					this.SetBounds(bounds);  // déplace le commentaire

					this.editor.Invalidate();
				}
				return true;
			}
			else
			{
				return base.MouseMove(pos);
			}
		}

		public override void MouseDown(Point pos)
		{
			//	Le bouton de la souris est pressé.
			if (this.hilitedElement == ActiveElement.CommentMove)
			{
				this.isDraggingMove = true;
				this.draggingPos = pos;
				this.editor.LockObject(this);
			}

			if (this.hilitedElement == ActiveElement.CommentWidth)
			{
				this.isDraggingWidth = true;
				this.editor.LockObject(this);
			}

			if (this.hilitedElement == ActiveElement.CommentAttachToConnection)
			{
				this.isDraggingAttach = true;
				this.editor.LockObject(this);
			}
		}

		public override void MouseUp(Point pos)
		{
			//	Le bouton de la souris est relâché.
			if (this.isDraggingMove)
			{
				this.isDraggingMove = false;
				this.editor.LockObject(null);
				this.editor.UpdateAfterCommentChanged();
				this.editor.DirtySerialization = true;
			}
			else if (this.isDraggingWidth)
			{
				this.isDraggingWidth = false;
				this.editor.LockObject(null);
				this.editor.UpdateAfterCommentChanged();
				this.editor.DirtySerialization = true;
			}
			else if (this.isDraggingAttach)
			{
				this.isDraggingAttach = false;
				this.editor.LockObject(null);
				this.editor.UpdateAfterCommentChanged();
				this.editor.DirtySerialization = true;
			}
			else
			{
				if (this.hilitedElement == ActiveElement.CommentClose)
				{
					this.IsVisible = false;
				}

				if (this.hilitedElement == ActiveElement.CommentEdit)
				{
					this.EditComment();
				}

				if (this.hilitedElement == ActiveElement.CommentColorButton1)
				{
					this.BackgroundMainColor = MainColor.Yellow;
					this.UpdateFieldColor();
				}

				if (this.hilitedElement == ActiveElement.CommentColorButton2)
				{
					this.BackgroundMainColor = MainColor.Orange;
					this.UpdateFieldColor();
				}

				if (this.hilitedElement == ActiveElement.CommentColorButton3)
				{
					this.BackgroundMainColor = MainColor.Red;
					this.UpdateFieldColor();
				}

				if (this.hilitedElement == ActiveElement.CommentColorButton4)
				{
					this.BackgroundMainColor = MainColor.Lilac;
					this.UpdateFieldColor();
				}

				if (this.hilitedElement == ActiveElement.CommentColorButton5)
				{
					this.BackgroundMainColor = MainColor.Purple;
					this.UpdateFieldColor();
				}

				if (this.hilitedElement == ActiveElement.CommentColorButton6)
				{
					this.BackgroundMainColor = MainColor.Blue;
					this.UpdateFieldColor();
				}

				if (this.hilitedElement == ActiveElement.CommentColorButton7)
				{
					this.BackgroundMainColor = MainColor.Green;
					this.UpdateFieldColor();
				}

				if (this.hilitedElement == ActiveElement.CommentColorButton8)
				{
					this.BackgroundMainColor = MainColor.DarkGrey;
					this.UpdateFieldColor();
				}
			}
		}

		protected override bool MouseDetect(Point pos, out ActiveElement element, out int fieldRank)
		{
			//	Détecte l'élément actif visé par la souris.
			element = ActiveElement.None;
			fieldRank = -1;

			if (pos.IsZero || !this.isVisible)
			{
				return false;
			}

			//	Souris dans le bouton pour modifier la largeur ?
			if (this.DetectRoundButton(this.PositionWidthButton, pos))
			{
				element = ActiveElement.CommentWidth;
				return true;
			}

			//	Souris dans le bouton de fermeture ?
			if (this.DetectRoundButton(this.PositionCloseButton, pos))
			{
				element = ActiveElement.CommentClose;
				return true;
			}

			//	Souris dans le bouton de déplacer l'attache ?
			if (this.DetectRoundButton(this.PositionAttachToConnectionButton, pos))
			{
				element = ActiveElement.CommentAttachToConnection;
				return true;
			}

			//	Souris dans l'en-tête ?
			if (this.HeaderRectangle.Contains(pos))
			{
				element = ActiveElement.CommentMove;
				return true;
			}

			//	Souris dans le bouton des couleurs ?
			if (this.DetectSquareButton(this.PositionColorButton(0), pos))
			{
				element = ActiveElement.CommentColorButton1;
				return true;
			}

			if (this.DetectSquareButton(this.PositionColorButton(1), pos))
			{
				element = ActiveElement.CommentColorButton2;
				return true;
			}

			if (this.DetectSquareButton(this.PositionColorButton(2), pos))
			{
				element = ActiveElement.CommentColorButton3;
				return true;
			}

			if (this.DetectSquareButton(this.PositionColorButton(3), pos))
			{
				element = ActiveElement.CommentColorButton4;
				return true;
			}

			if (this.DetectSquareButton(this.PositionColorButton(4), pos))
			{
				element = ActiveElement.CommentColorButton5;
				return true;
			}

			if (this.DetectSquareButton(this.PositionColorButton(5), pos))
			{
				element = ActiveElement.CommentColorButton6;
				return true;
			}

			if (this.DetectSquareButton(this.PositionColorButton(6), pos))
			{
				element = ActiveElement.CommentColorButton7;
				return true;
			}

			if (this.DetectSquareButton(this.PositionColorButton(7), pos))
			{
				element = ActiveElement.CommentColorButton8;
				return true;
			}

			//	Souris dans la boîte ?
			if (this.bounds.Contains(pos))
			{
				element = ActiveElement.CommentEdit;
				return true;
			}

			return false;
		}


		public void EditComment()
		{
			//	Modifie le texte du commentaire.
			Module module = this.editor.Module;
			string text = this.textLayoutComment.Text;
			text = module.MainWindow.DlgEntityComment(text);
			if (text != null)
			{
				this.textLayoutComment.Text = text;
				this.UpdateHeight();
				this.editor.UpdateAfterCommentChanged();
				this.editor.DirtySerialization = true;
			}
		}

		public void UpdateHeight()
		{
			//	Adapte la hauteur du commentaire en fonction de sa largeur et du contenu.
			Rectangle rect = this.bounds;
			rect.Deflate(ObjectComment.textMargin);
			this.textLayoutComment.LayoutSize = rect.Size;

			double h = System.Math.Floor(this.textLayoutComment.FindTextHeight()+1);
			h += ObjectComment.textMargin*2;

			if (this.GetAttachMode() == AttachMode.Bottom)
			{
				this.bounds.Height = h;
			}
			else
			{
				this.bounds.Bottom = this.bounds.Top-h;
			}
		}


		public override void DrawBackground(Graphics graphics)
		{
			//	Dessine le fond de l'objet.
		}

		public override void DrawForeground(Graphics graphics)
		{
			//	Dessine le dessus de l'objet.
			if (!this.isVisible)
			{
				return;
			}
			
			Rectangle rect;
			Rectangle rh = Rectangle.Empty;
			if (this.hilitedElement != ActiveElement.None)
			{
				rh = this.HeaderRectangle;
			}

			//	Dessine l'ombre.
			rect = this.bounds;
			if (!this.isDraggingMove && !this.isDraggingWidth && !this.isDraggingAttach)
			{
				rect = Rectangle.Union(rect, rh);
			}
			rect.Inflate(2);
			rect.Offset(8, -8);
			this.DrawShadow(graphics, rect, 8, 8, 0.2);

			//	Dessine l'en-tête.
			if (!rh.IsEmpty && !this.isDraggingMove && !this.isDraggingWidth && !this.isDraggingAttach)
			{
				rect = rh;
				rect.Inflate(0.5);
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.ColorCommentHeader(this.hilitedElement == ActiveElement.CommentMove, this.isDraggingMove || this.isDraggingWidth));
				graphics.AddRectangle(rect);
				graphics.RenderSolid(Color.FromBrightness(0));

				rect.Width -= rect.Height;
				rect.Offset(0, 1);
				this.textLayoutTitle.LayoutSize = rect.Size;
				this.textLayoutTitle.Paint(rect.BottomLeft, graphics, Rectangle.MaxValue, Color.FromBrightness(1), GlyphPaintStyle.Normal);
			}

			//	Dessine la boîte vide avec la queue (bulle de bd).
			Path path = this.GetFramePath();

			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(this.ColorComment(this.hilitedElement != ActiveElement.None));

			graphics.Rasterizer.AddOutline(path);
			graphics.RenderSolid(Color.FromBrightness(0));

			//	Dessine le texte.
			rect = this.bounds;
			rect.Deflate(ObjectComment.textMargin);
			this.textLayoutComment.LayoutSize = rect.Size;
			this.textLayoutComment.Paint(rect.BottomLeft, graphics, Rectangle.MaxValue, Color.FromBrightness(this.IsDarkColorMain ? 1:0), GlyphPaintStyle.Normal);

			//	Dessine le bouton de fermeture.
			if (!rh.IsEmpty)
			{
				if (this.hilitedElement == ActiveElement.CommentClose)
				{
					this.DrawRoundButton(graphics, this.PositionCloseButton, AbstractObject.buttonRadius, GlyphShape.Close, true, false);
				}
				else if (this.IsHeaderHilite && !this.isDraggingMove && !this.isDraggingWidth && !this.isDraggingAttach)
				{
					this.DrawRoundButton(graphics, this.PositionCloseButton, AbstractObject.buttonRadius, GlyphShape.Close, false, false);
				}
			}

			//	Dessine les boutons des couleurs.
			this.DrawColorButton(graphics, ActiveElement.CommentColorButton1, 0, MainColor.Yellow);
			this.DrawColorButton(graphics, ActiveElement.CommentColorButton2, 1, MainColor.Orange);
			this.DrawColorButton(graphics, ActiveElement.CommentColorButton3, 2, MainColor.Red);
			this.DrawColorButton(graphics, ActiveElement.CommentColorButton4, 3, MainColor.Lilac);
			this.DrawColorButton(graphics, ActiveElement.CommentColorButton5, 4, MainColor.Purple);
			this.DrawColorButton(graphics, ActiveElement.CommentColorButton6, 5, MainColor.Blue);
			this.DrawColorButton(graphics, ActiveElement.CommentColorButton7, 6, MainColor.Green);
			this.DrawColorButton(graphics, ActiveElement.CommentColorButton8, 7, MainColor.DarkGrey);

			//	Dessine le bouton pour modifier la largeur.
			if (this.hilitedElement == ActiveElement.CommentWidth)
			{
				this.DrawRoundButton(graphics, this.PositionWidthButton, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, true, false);
			}
			else if (this.IsHeaderHilite && !this.isDraggingMove && !this.isDraggingWidth && !this.isDraggingAttach)
			{
				this.DrawRoundButton(graphics, this.PositionWidthButton, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, false, false);
			}

			//	Dessine le bouton pour déplacer l'attache.
			Point p = this.PositionAttachToConnectionButton;
			if (!p.IsZero)
			{
				if (this.hilitedElement == ActiveElement.CommentAttachToConnection)
				{
					this.DrawRoundButton(graphics, p, AbstractObject.buttonRadius, "C", true, false);
				}
				else if (this.IsHeaderHilite && !this.isDraggingMove && !this.isDraggingWidth && !this.isDraggingAttach)
				{
					this.DrawRoundButton(graphics, p, AbstractObject.buttonRadius, "C", false, false);
				}
			}
		}

		protected void DrawColorButton(Graphics graphics, ActiveElement activeElement, int rank, MainColor color)
		{
			//	Dessine un bouton pour choisir une couleur.
			if (!this.isDraggingMove && !this.isDraggingWidth && !this.isDraggingAttach)
			{
				if (this.hilitedElement == activeElement)
				{
					this.DrawSquareButton(graphics, this.PositionColorButton(rank), color, this.boxColor == color, true);
				}
				else if (this.IsHeaderHilite)
				{
					this.DrawSquareButton(graphics, this.PositionColorButton(rank), color, this.boxColor == color, false);
				}
			}
		}

		protected bool IsHeaderHilite
		{
			//	Indique si la souris est dans l'en-tête.
			get
			{
				return (this.hilitedElement == ActiveElement.CommentEdit ||
						this.hilitedElement == ActiveElement.CommentMove ||
						this.hilitedElement == ActiveElement.CommentClose ||
						this.hilitedElement == ActiveElement.CommentColorButton1 ||
						this.hilitedElement == ActiveElement.CommentColorButton2 ||
						this.hilitedElement == ActiveElement.CommentColorButton3 ||
						this.hilitedElement == ActiveElement.CommentColorButton4 ||
						this.hilitedElement == ActiveElement.CommentColorButton5 ||
						this.hilitedElement == ActiveElement.CommentColorButton6 ||
						this.hilitedElement == ActiveElement.CommentColorButton7 ||
						this.hilitedElement == ActiveElement.CommentColorButton8 ||
						this.hilitedElement == ActiveElement.CommentAttachToConnection);
			}
		}


		protected Path GetFramePath()
		{
			//	Retourne le chemin du cadre du commentaire (rectangle avec éventuellement une queue,
			//	comme une bulle de bd).
			Path path = new Path();

			AttachMode mode = this.GetAttachMode();
			Point himself = this.GetAttachHimself(mode);
			Point other = this.GetAttachOther(mode);
			
			Rectangle bounds = this.bounds;
			bounds.Inflate(0.5);

			if (mode == AttachMode.None || himself.IsZero || other.IsZero)
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

		protected Point GetAttachHimself(AttachMode mode)
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

				if (pos.IsZero && this.attachObject is ObjectBox)
				{
					ObjectBox box = this.attachObject as ObjectBox;

					if (mode == AttachMode.Left || mode == AttachMode.Right)
					{
						pos.X = (mode == AttachMode.Left) ? bounds.Left : bounds.Right;

						double miny = System.Math.Max(box.Bounds.Bottom, bounds.Bottom);
						double maxy = System.Math.Min(box.Bounds.Top, bounds.Top);

						if (miny <= maxy)
						{
							pos.Y = (miny+maxy)/2;
						}
						else
						{
							pos.Y = (bounds.Top < box.Bounds.Top) ? bounds.Top : bounds.Bottom;
						}
					}

					if (mode == AttachMode.Bottom || mode == AttachMode.Top)
					{
						pos.Y = (mode == AttachMode.Bottom) ? bounds.Bottom : bounds.Top;

						double minx = System.Math.Max(box.Bounds.Left, bounds.Left);
						double maxx = System.Math.Min(box.Bounds.Right, bounds.Right);

						if (minx <= maxx)
						{
							pos.X = (minx+maxx)/2;
						}
						else
						{
							pos.X = (bounds.Right < box.Bounds.Right) ? bounds.Right : bounds.Left;
						}
					}
				}

				if (pos.IsZero && this.attachObject is ObjectConnection)
				{
					ObjectConnection connection = this.attachObject as ObjectConnection;
					Point attach = connection.PositionConnectionComment;

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

		protected Point GetAttachOther(AttachMode mode)
		{
			//	Retourne le point d'attache sur l'objet lié (boîte ou commentaire).
			Point pos = Point.Zero;

			if (mode != AttachMode.None)
			{
				Rectangle bounds = this.bounds;
				bounds.Inflate(0.5);

				if (this.attachObject is ObjectBox)
				{
					ObjectBox box = this.attachObject as ObjectBox;

					if (mode == AttachMode.BottomLeft)
					{
						return box.Bounds.TopRight;
					}

					if (mode == AttachMode.BottomRight)
					{
						return box.Bounds.TopLeft;
					}

					if (mode == AttachMode.TopLeft)
					{
						return box.Bounds.BottomRight;
					}

					if (mode == AttachMode.TopRight)
					{
						return box.Bounds.BottomLeft;
					}
					
					Point himself = this.GetAttachHimself(mode);

					if (mode == AttachMode.Left || mode == AttachMode.Right)
					{
						pos.X = (mode == AttachMode.Left) ? box.Bounds.Right : box.Bounds.Left;

						if (himself.Y < box.Bounds.Bottom)
						{
							pos.Y = box.Bounds.Bottom;
						}
						else if (himself.Y > box.Bounds.Top)
						{
							pos.Y = box.Bounds.Top;
						}
						else
						{
							pos.Y = himself.Y;
						}
					}

					if (mode == AttachMode.Bottom || mode == AttachMode.Top)
					{
						pos.Y = (mode == AttachMode.Bottom) ? box.Bounds.Top : box.Bounds.Bottom;

						if (himself.X < box.Bounds.Left)
						{
							pos.X = box.Bounds.Left;
						}
						else if (himself.X > box.Bounds.Right)
						{
							pos.X = box.Bounds.Right;
						}
						else
						{
							pos.X = himself.X;
						}
					}
				}

				if (this.attachObject is ObjectConnection)
				{
					ObjectConnection connection = this.attachObject as ObjectConnection;
					pos = connection.PositionConnectionComment;

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
			if (this.attachObject is ObjectBox)
			{
				ObjectBox box = this.attachObject as ObjectBox;

				if (!this.bounds.IntersectsWith(box.Bounds))
				{
					if (this.bounds.Bottom >= box.Bounds.Top && this.bounds.Right <= box.Bounds.Left)
					{
						return AttachMode.BottomRight;
					}
					
					if (this.bounds.Top <= box.Bounds.Bottom && this.bounds.Right <= box.Bounds.Left)
					{
						return AttachMode.TopRight;
					}
					
					if (this.bounds.Bottom >= box.Bounds.Top && this.bounds.Left >= box.Bounds.Right)
					{
						return AttachMode.BottomLeft;
					}
					
					if (this.bounds.Top <= box.Bounds.Bottom && this.bounds.Left >= box.Bounds.Right)
					{
						return AttachMode.TopLeft;
					}
					
					if (this.bounds.Bottom >= box.Bounds.Top)  // commentaire en dessus ?
					{
						return AttachMode.Bottom;
					}
					
					if (this.bounds.Top <= box.Bounds.Bottom)  // commentaire en dessous ?
					{
						return AttachMode.Top;
					}

					if (this.bounds.Left >= box.Bounds.Right)  // commentaire à droite ?
					{
						return AttachMode.Left;
					}

					if (this.bounds.Right <= box.Bounds.Left)  // commentaire à gauche ?
					{
						return AttachMode.Right;
					}
				}
			}

			if (this.attachObject is ObjectConnection)
			{
				ObjectConnection connection = this.attachObject as ObjectConnection;
				Point attach = connection.PositionConnectionComment;
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

		protected Point PositionAttachToConnectionButton
		{
			//	Retourne la position du bouton pour modifier l'attache à la connection.
			get
			{
				if (this.attachObject != null && this.attachObject is ObjectConnection)
				{
					ObjectConnection connection = this.attachObject as ObjectConnection;
					return connection.PositionConnectionComment;
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
			return new Point(this.bounds.Left+AbstractObject.buttonSquare+(AbstractObject.buttonSquare+0.5)*rank*2, this.bounds.Bottom-1-AbstractObject.buttonSquare);
		}

		protected Color ColorComment(bool hilited)
		{
			if (hilited)
			{
				return this.GetColorLighter(this.GetColorMain(), 0.3);
			}
			else
			{
				return this.GetColorLighter(this.GetColorMain(), 0.2);
			}
		}

		protected Color ColorCommentHeader(bool hilited, bool dragging)
		{
			if (dragging)
			{
				return this.GetColorMain(0.9);
			}
			else if (hilited)
			{
				return this.GetColorLighter(this.GetColorMain(), 0.9);
			}
			else
			{
				return this.GetColorLighter(this.GetColorMain(), 0.7);
			}
		}


		protected void UpdateFieldColor()
		{
			//	Met à jour l'information de couleur dans le champ associé.
			if (this.attachObject is ObjectConnection)
			{
				ObjectConnection connection = this.attachObject as ObjectConnection;
				connection.Field.CommentMainColor = this.BackgroundMainColor;
			}
		}


		#region Serialization
		public void WriteXml(XmlWriter writer)
		{
			//	Sérialise toutes les informations du commentaire.
			//	Utilisé seulement pour les commentaires associés à des boîtes.
			//	Les commentaires associés à des connections sont sérialisés par Field.
			writer.WriteStartElement("Comment");
			
			writer.WriteElementString("Bounds", this.bounds.ToString());
			writer.WriteElementString("Text", this.textLayoutComment.Text);
			writer.WriteElementString("Color", this.boxColor.ToString());

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

					if (name == "Bounds")
					{
						this.bounds = Rectangle.Parse(element);
					}
					else if (name == "Text")
					{
						this.textLayoutComment.Text = element;
					}
					else if (name == "Color")
					{
						this.boxColor = (MainColor) System.Enum.Parse(typeof(MainColor), element);
					}
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
			}
		}
		#endregion


		protected static readonly double commentHeaderHeight = 24;
		protected static readonly double textMargin = 5;
		protected static readonly double queueThickness = 5;

		protected Rectangle bounds;
		protected AbstractObject attachObject;
		protected bool isVisible;
		protected TextLayout textLayoutTitle;
		protected TextLayout textLayoutComment;

		protected bool isDraggingMove;
		protected bool isDraggingWidth;
		protected bool isDraggingAttach;
		protected Point draggingPos;
	}
}
