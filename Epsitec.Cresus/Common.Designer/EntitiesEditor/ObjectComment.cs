using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.EntitiesEditor
{
	/// <summary>
	/// Bo�te pour repr�senter une entit�.
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
		}


		public ObjectComment(Editor editor) : base(editor)
		{
			this.isVisible = true;
			this.textLayout = new TextLayout();
			this.textLayout.DefaultFontSize = 10;
			this.textLayout.BreakMode = TextBreakMode.Hyphenate;
			this.textLayout.Text = "Commentaire libre, que vous pouvez modifier � volont�.";
		}


		public string Text
		{
			//	Texte du commentaire.
			get
			{
				return this.textLayout.Text;
			}
			set
			{
				this.textLayout.Text = value;
			}
		}

		public AbstractObject AttachObject
		{
			//	Object li�e (bo�te ou connection).
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
			//	Retourne la bo�te de l'objet.
			//	Attention: le dessin peut d�border, par exemple pour l'ombre.
			get
			{
				return this.isVisible ? this.bounds : Rectangle.Empty;
			}
		}

		public Rectangle InternalBounds
		{
			//	Retourne la bo�te de l'objet.
			get
			{
				return this.bounds;
			}
		}

		public void SetBounds(Rectangle bounds)
		{
			//	Modifie la bo�te de l'objet.
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
				}
			}
		}


		public override bool MouseMove(Point pos)
		{
			//	Met en �vidence la bo�te selon la position de la souris.
			//	Si la souris est dans cette bo�te, retourne true.
			if (this.isDraggingMove)
			{
				Rectangle bounds = this.bounds;

				bounds.Offset(pos-this.draggingPos);
				this.draggingPos = pos;

				this.SetBounds(bounds);
				this.editor.Invalidate();
				return true;
			}
			else if (this.isDraggingSize)
			{
				Rectangle bounds = this.bounds;

				bounds.Right = pos.X;
				bounds.Width = System.Math.Max(bounds.Width, AbstractObject.commentMinWidth);

				this.SetBounds(bounds);
				this.UpdateHeight();
				this.editor.Invalidate();
				return true;
			}
			else
			{
				return base.MouseMove(pos);
			}
		}

		public override void MouseDown(Point pos)
		{
			//	Le bouton de la souris est press�.
			if (this.hilitedElement == ActiveElement.CommentMove)
			{
				this.isDraggingMove = true;
				this.draggingPos = pos;
				this.editor.LockObject(this);
			}

			if (this.hilitedElement == ActiveElement.CommentWidth)
			{
				this.isDraggingSize = true;
				this.editor.LockObject(this);
			}
		}

		public override void MouseUp(Point pos)
		{
			//	Le bouton de la souris est rel�ch�.
			if (this.isDraggingMove)
			{
				this.isDraggingMove = false;
				this.editor.LockObject(null);
				this.editor.UpdateAfterCommentChanged();
			}
			else if (this.isDraggingSize)
			{
				this.isDraggingSize = false;
				this.editor.LockObject(null);
				this.editor.UpdateAfterCommentChanged();
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
			}
		}

		protected override bool MouseDetect(Point pos, out ActiveElement element, out int fieldRank)
		{
			//	D�tecte l'�l�ment actif vis� par la souris.
			element = ActiveElement.None;
			fieldRank = -1;

			if (pos.IsZero || !this.isVisible)
			{
				return false;
			}

			//	Souris dans le bouton pour modifier la taille ?
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

			//	Souris dans l'en-t�te ?
			if (this.HeaderRectangle.Contains(pos))
			{
				element = ActiveElement.CommentMove;
				return true;
			}

			//	Souris dans la bo�te ?
			if (this.bounds.Contains(pos))
			{
				element = ActiveElement.CommentEdit;
				return true;
			}

			return false;
		}


		protected void EditComment()
		{
			//	Modifie le texte du commentaire.
			Module module = this.editor.Module;
			string text = this.textLayout.Text;
			text = module.MainWindow.DlgEntityComment(text);
			if (text != null)
			{
				this.textLayout.Text = text;
				this.UpdateHeight();
				this.editor.UpdateAfterCommentChanged();
			}
		}

		public void UpdateHeight()
		{
			//	Adapte la hauteur du commentaire en fonction de sa largeur et du contenu.
			Rectangle rect = this.bounds;
			rect.Deflate(ObjectComment.textMargin);
			this.textLayout.LayoutSize = rect.Size;

			double h = System.Math.Floor(this.textLayout.FindTextHeight()+1);
			h += ObjectComment.textMargin*2;

			Point p1, p2;
			AttachMode mode;
			this.GetBoxAttach(out p1, out p2, out mode);

			if (mode == AttachMode.Bottom)
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
			if (!this.isDraggingMove && !this.isDraggingSize)
			{
				rect = Rectangle.Union(rect, rh);
			}
			rect.Inflate(2);
			rect.Offset(8, -8);
			this.DrawShadow(graphics, rect, 8, 8, 0.2);

			//	Dessine l'en-t�te.
			if (!rh.IsEmpty)
			{
				rect = rh;
				rect.Inflate(0.5);
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.ColorCommentHeader(this.hilitedElement == ActiveElement.CommentMove, this.isDraggingMove || this.isDraggingSize));
				graphics.AddRectangle(rect);
				graphics.RenderSolid(Color.FromBrightness(0));

				graphics.AddText(rect.Left, rect.Bottom+1, rect.Width-rect.Height, rect.Height, "Commentaire", Font.GetFont(Font.DefaultFontFamily, "Bold"), 14, ContentAlignment.MiddleCenter);
				graphics.RenderSolid(Color.FromBrightness(1));
			}

			//	Dessine la bo�te vide.
			rect = this.bounds;
			rect.Inflate(0.5);
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.ColorComment(this.hilitedElement != ActiveElement.None));
			graphics.AddRectangle(rect);
			graphics.RenderSolid(Color.FromBrightness(0));

			//	Dessine le texte.
			rect = this.bounds;
			rect.Deflate(ObjectComment.textMargin);
			this.textLayout.LayoutSize = rect.Size;
			this.textLayout.Paint(rect.BottomLeft, graphics, Rectangle.MaxValue, Color.FromBrightness(0), GlyphPaintStyle.Normal);

			//	Dessine la liaison.
			Point p1, p2;
			AttachMode mode;
			this.GetBoxAttach(out p1, out p2, out mode);
			if (!p1.IsZero)
			{
				Path path = new Path();
				path.MoveTo(p1);
				path.LineTo(p2);
				Misc.DrawPathDash(graphics, path, 1, 4, 4, Color.FromBrightness(0));

				graphics.AddFilledCircle(p1, 3);
				graphics.AddFilledCircle(p2, 3);
				graphics.RenderSolid(Color.FromBrightness(0));
			}

			//	Dessine le bouton de fermeture.
			if (!rh.IsEmpty)
			{
				if (this.hilitedElement == ActiveElement.CommentClose)
				{
					this.DrawRoundButton(graphics, this.PositionCloseButton, AbstractObject.buttonRadius, GlyphShape.Close, true, false);
				}
				else if (this.IsHeaderHilite && !this.isDraggingMove && !this.isDraggingSize)
				{
					this.DrawRoundButton(graphics, this.PositionCloseButton, AbstractObject.buttonRadius, GlyphShape.Close, false, false);
				}
			}

			//	Dessine le bouton pour modifier la taille.
			if (this.hilitedElement == ActiveElement.CommentWidth)
			{
				this.DrawRoundButton(graphics, this.PositionWidthButton, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, true, false);
			}
			else if (this.IsHeaderHilite && !this.isDraggingMove && !this.isDraggingSize)
			{
				this.DrawRoundButton(graphics, this.PositionWidthButton, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, false, false);
			}
		}

		protected bool IsHeaderHilite
		{
			//	Indique si la souris est dans l'en-t�te.
			get
			{
				return (this.hilitedElement == ActiveElement.CommentEdit ||
						this.hilitedElement == ActiveElement.CommentMove ||
						this.hilitedElement == ActiveElement.CommentClose);
			}
		}

		protected void GetBoxAttach(out Point p1, out Point p2, out AttachMode mode)
		{
			//	Retourne les points pour attacher le commentaire � sa bo�te.
			p1 = Point.Zero;
			p2 = Point.Zero;
			mode = AttachMode.None;

			if (this.attachObject is ObjectBox && this.attachObject.Bounds.IntersectsWith(this.bounds))
			{
				return;
			}

			Point cx = Point.Move(this.bounds.TopLeft, this.bounds.TopRight, AbstractObject.headerHeight);
			Point cy = Point.Move(this.bounds.TopLeft, this.bounds.BottomLeft, ObjectComment.textMargin+8);
			Point ct = new Point(cx.X, this.bounds.Top);
			Point cb = new Point(cx.X, this.bounds.Bottom);
			Point cl = new Point(this.bounds.Left, cy.Y);
			Point cr = new Point(this.bounds.Right, cy.Y);

			Point bt, bb, bl, br;
			if (this.attachObject is ObjectBox)
			{
				ObjectBox box = this.attachObject as ObjectBox;
				Rectangle bounds = box.Bounds;
				Point bx = Point.Move(bounds.TopLeft, bounds.TopRight, AbstractObject.headerHeight);
				Point by = Point.Move(bounds.TopLeft, bounds.BottomLeft, AbstractObject.headerHeight/2+8);
				bounds.Deflate(box.IsRoot ? -1 : 1);
				bt = new Point(bx.X, bounds.Top);
				bb = new Point(bx.X, bounds.Bottom);
				bl = new Point(bounds.Left, by.Y);
				br = new Point(bounds.Right, by.Y);
			}
			else
			{
				ObjectConnection connection = this.attachObject as ObjectConnection;
				bt = bb = bl = br = connection.PositionConnectionComment;
				if (bt.IsZero)
				{
					return;
				}
			}

			double dt = Point.Distance(ct, bb);
			double db = Point.Distance(cb, bt);
			double dl = Point.Distance(cl, br);
			double dr = Point.Distance(cr, bl);

			double min = System.Math.Min(System.Math.Min(dt, db), System.Math.Min(dl, dr));

			if (min == dt)
			{
				p1 = ct;
				p2 = bb;
				mode = AttachMode.Top;
			}
			else if (min == db)
			{
				p1 = cb;
				p2 = bt;
				mode = AttachMode.Bottom;
			}
			else if (min == dl)
			{
				p1 = cl;
				p2 = br;
				mode = AttachMode.Left;
			}
			else
			{
				p1 = cr;
				p2 = bl;
				mode = AttachMode.Right;
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
			//	Retourne la position du bouton de largeur.
			get
			{
				return new Point(this.bounds.Right, this.bounds.Center.Y);
			}
		}

		protected Color ColorComment(bool hilited)
		{
			if (hilited)
			{
				return Color.FromRgb(255.0/255.0, 240.0/255.0, 150.0/255.0);
			}
			else
			{
				return Color.FromRgb(255.0/255.0, 248.0/255.0, 200.0/255.0);
			}
		}

		protected Color ColorCommentHeader(bool hilited, bool dragging)
		{
			if (dragging)
			{
				return Color.FromAlphaRgb(0.4, 124.0/255.0, 105.0/255.0, 0.0/255.0);
			}
			else if (hilited)
			{
				return Color.FromRgb(124.0/255.0, 105.0/255.0, 0.0/255.0);
			}
			else
			{
				return Color.FromRgb(172.0/255.0, 146.0/255.0, 0.0/255.0);
			}
		}


		protected static readonly double commentHeaderHeight = 24;
		protected static readonly double textMargin = 5;

		protected Rectangle bounds;
		protected AbstractObject attachObject;
		protected bool isVisible;
		protected TextLayout textLayout;

		protected bool isDraggingMove;
		protected bool isDraggingSize;
		protected Point draggingPos;
	}
}
