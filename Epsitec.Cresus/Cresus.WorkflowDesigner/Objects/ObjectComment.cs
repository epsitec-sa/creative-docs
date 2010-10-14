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
	/// Bulle pour représenter le commentaire associé à une entité ou une connexion.
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


		public ObjectComment(Editor editor, AbstractEntity entity)
			: base (editor, entity)
		{
			this.isVisible = true;
			this.boxColor = MainColor.Yellow;

			this.textLayoutTitle = new TextLayout();
			this.textLayoutTitle.DefaultFontSize = 14;
			this.textLayoutTitle.BreakMode = TextBreakMode.SingleLine | TextBreakMode.Ellipsis;
			this.textLayoutTitle.Alignment = ContentAlignment.MiddleCenter;
			this.textLayoutTitle.Text = "Commentaires";  //Res.Strings.Entities.Comment.Title;

			this.textLayoutComment = new TextLayout();
			this.textLayoutComment.DefaultFontSize = 10;
			this.textLayoutComment.BreakMode = TextBreakMode.Hyphenate | TextBreakMode.Split;

			if (this.entity is WorkflowNodeEntity)
			{
				var node = this.entity as WorkflowNodeEntity;

				this.textLayoutComment.Text = node.Name.ToString ();  // TODO: provisoire
			}

			if (this.entity is WorkflowEdgeEntity)
			{
				var edge = this.entity as WorkflowEdgeEntity;

				this.textLayoutComment.Text = edge.Description.ToString ();  // TODO: provisoire
			}
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

		public override Rectangle Bounds
		{
			//	Retourne la boîte de l'objet.
			//	Attention: le dessin peut déborder, par exemple pour l'ombre.
			get
			{
				return this.isVisible ? this.bounds : Rectangle.Empty;
			}
		}

		public override void Move(double dx, double dy)
		{
			//	Déplace l'objet.
			this.bounds.Offset(dx, dy);
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
					this.editor.SetLocalDirty ();
				}
			}
		}


		public override void AcceptEdition()
		{
			this.Text = this.editingTextField.Text;

			this.UpdateHeight ();
			this.StopEdition ();
		}

		public override void CancelEdition()
		{
			this.StopEdition ();
		}

		private void StartEdition(ActiveElement element)
		{
			Rectangle rect = this.bounds;
			string text = this.Text;

			Point p1 = this.editor.ConvEditorToWidget (rect.TopLeft);
			Point p2 = this.editor.ConvEditorToWidget (rect.BottomRight);
			double width  = System.Math.Max (p2.X-p1.X, 175);
			double height = System.Math.Max (p1.Y-p2.Y, 10+14*5);

			rect = new Rectangle (new Point (p1.X, p1.Y-height), new Size (width, height));

			var field = new TextFieldMulti ();
			field.Parent = this.editor;
			field.ScrollerVisibility = false;
			field.SetManualBounds (rect);
			field.Text = text;
			field.TabIndex = 1;
			field.SelectAll ();
			field.Focus ();
			this.editingTextField = field;

			this.editor.EditingObject = this;
			this.hilitedElement = ActiveElement.None;
		}

		private void StopEdition()
		{
			this.editor.Children.Remove (this.editingTextField);
			this.editingTextField = null;

			this.editor.EditingObject = null;
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

		public override bool MouseMove(Message message, Point pos)
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
				ObjectLink link = this.attachObject as ObjectLink;

				double attach = link.PointToAttach(pos);
				if (attach != 0)
				{
					Point oldPos = link.PositionLinkComment;
					link.CommentAttach = attach;
					Point newPos = link.PositionLinkComment;

					Rectangle bounds = this.bounds;
					bounds.Offset(newPos-oldPos);
					this.SetBounds(bounds);  // déplace le commentaire

					this.editor.Invalidate();
				}
				return true;
			}
			else
			{
				return base.MouseMove(message, pos);
			}
		}

		public override void MouseDown(Message message, Point pos)
		{
			//	Le bouton de la souris est pressé.
			this.initialPos = pos;

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

			if (this.hilitedElement == ActiveElement.CommentAttachTo)
			{
				this.isDraggingAttach = true;
				this.editor.LockObject(this);
			}
		}

		public override void MouseUp(Message message, Point pos)
		{
			//	Le bouton de la souris est relâché.
			if (pos == this.initialPos)
			{
				if (this.hilitedElement == ActiveElement.CommentEdit)
				{
					this.StartEdition (this.hilitedElement);
					return;
				}
			}

			if (this.isDraggingMove)
			{
				this.isDraggingMove = false;
				this.editor.LockObject(null);
				this.editor.UpdateAfterCommentChanged();
				this.editor.SetLocalDirty ();
			}
			else if (this.isDraggingWidth)
			{
				this.isDraggingWidth = false;
				this.editor.LockObject(null);
				this.editor.UpdateAfterCommentChanged();
				this.editor.SetLocalDirty ();
			}
			else if (this.isDraggingAttach)
			{
				this.isDraggingAttach = false;
				this.editor.LockObject(null);
				this.editor.UpdateAfterCommentChanged();
				this.editor.SetLocalDirty ();
			}
			else
			{
				if (this.hilitedElement == ActiveElement.CommentClose)
				{
					this.IsVisible = false;

					ObjectLink link = this.attachObject as ObjectLink;
					if (link != null)
					{
						link.Comment = null;
						this.editor.RemoveComment (this);
					}
				}

				if (this.hilitedElement == ActiveElement.CommentEdit)
				{
					this.EditComment();
				}

				if (this.hilitedElement == ActiveElement.CommentColor1)
				{
					this.BackgroundMainColor = MainColor.Yellow;
				}

				if (this.hilitedElement == ActiveElement.CommentColor2)
				{
					this.BackgroundMainColor = MainColor.Orange;
				}

				if (this.hilitedElement == ActiveElement.CommentColor3)
				{
					this.BackgroundMainColor = MainColor.Red;
				}

				if (this.hilitedElement == ActiveElement.CommentColor4)
				{
					this.BackgroundMainColor = MainColor.Lilac;
				}

				if (this.hilitedElement == ActiveElement.CommentColor5)
				{
					this.BackgroundMainColor = MainColor.Purple;
				}

				if (this.hilitedElement == ActiveElement.CommentColor6)
				{
					this.BackgroundMainColor = MainColor.Blue;
				}

				if (this.hilitedElement == ActiveElement.CommentColor7)
				{
					this.BackgroundMainColor = MainColor.Green;
				}

				if (this.hilitedElement == ActiveElement.CommentColor8)
				{
					this.BackgroundMainColor = MainColor.DarkGrey;
				}
			}
		}

		public override ActiveElement MouseDetectBackground(Point pos)
		{
			//	Détecte l'élément actif visé par la souris.
			if (pos.IsZero || !this.isVisible || this.editor.CurrentModifyMode == Editor.ModifyMode.Locked)
			{
				return ActiveElement.None;
			}

			//	Souris dans l'en-tête ?
			if (this.HeaderRectangle.Contains(pos))
			{
				return ActiveElement.CommentMove;
			}

			//	Souris dans la boîte ?
			if (this.bounds.Contains(pos))
			{
				return ActiveElement.CommentEdit;
			}

			return ActiveElement.None;
		}

		public override ActiveElement MouseDetectForeground(Point pos)
		{
			//	Détecte l'élément actif visé par la souris.
			if (pos.IsZero || !this.isVisible || this.editor.CurrentModifyMode == Editor.ModifyMode.Locked)
			{
				return ActiveElement.None;
			}

			//	Souris dans le bouton pour modifier la largeur ?
			if (this.DetectRoundButton (this.PositionWidthButton, pos))
			{
				return ActiveElement.CommentWidth;
			}

			//	Souris dans le bouton de fermeture ?
			if (this.DetectRoundButton (this.PositionCloseButton, pos))
			{
				return ActiveElement.CommentClose;
			}

			//	Souris dans le bouton de déplacer l'attache ?
			if (this.DetectRoundButton (this.PositionAttachToLinkButton, pos))
			{
				return ActiveElement.CommentAttachTo;
			}

			//	Souris dans le bouton des couleurs ?
			if (this.DetectSquareButton (this.PositionColorButton (0), pos))
			{
				return ActiveElement.CommentColor1;
			}

			if (this.DetectSquareButton (this.PositionColorButton (1), pos))
			{
				return ActiveElement.CommentColor2;
			}

			if (this.DetectSquareButton (this.PositionColorButton (2), pos))
			{
				return ActiveElement.CommentColor3;
			}

			if (this.DetectSquareButton (this.PositionColorButton (3), pos))
			{
				return ActiveElement.CommentColor4;
			}

			if (this.DetectSquareButton (this.PositionColorButton (4), pos))
			{
				return ActiveElement.CommentColor5;
			}

			if (this.DetectSquareButton (this.PositionColorButton (5), pos))
			{
				return ActiveElement.CommentColor6;
			}

			if (this.DetectSquareButton (this.PositionColorButton (6), pos))
			{
				return ActiveElement.CommentColor7;
			}

			if (this.DetectSquareButton (this.PositionColorButton (7), pos))
			{
				return ActiveElement.CommentColor8;
			}

			return ActiveElement.None;
		}


		public void EditComment()
		{
			//	Modifie le texte du commentaire.
#if false
			Module module = this.editor.Module;
			string text = this.textLayoutComment.Text;
			text = module.DesignerApplication.DlgEntityComment(text);
			if (text != null)
			{
				this.textLayoutComment.Text = text;
				this.UpdateHeight();
				this.editor.UpdateAfterCommentChanged();
				this.editor.SetLocalDirty ();
			}
#endif
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
				rect = Rectangle.Union (rect, rh);
			}
			rect.Inflate (2);
			rect.Offset (8, -8);
			this.DrawRoundShadow (graphics, rect, 8, 8, 0.2);

			//	Dessine l'en-tête.
			if (!rh.IsEmpty && !this.isDraggingMove && !this.isDraggingWidth && !this.isDraggingAttach)
			{
				rect = rh;
				rect.Inflate (0.5);
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (this.ColorCommentHeader (this.hilitedElement == ActiveElement.CommentMove, this.isDraggingMove || this.isDraggingWidth));
				graphics.AddRectangle (rect);
				graphics.RenderSolid (this.GetColor (0));

				rect.Width -= rect.Height;
				rect.Offset (0, 1);
				this.textLayoutTitle.LayoutSize = rect.Size;
				this.textLayoutTitle.Paint (rect.BottomLeft, graphics, Rectangle.MaxValue, this.GetColor (1), GlyphPaintStyle.Normal);
			}

			//	Dessine la boîte vide avec la queue (bulle de bd).
			Path path = this.GetFramePath ();

			graphics.Rasterizer.AddSurface (path);
			graphics.RenderSolid (this.ColorComment (this.hilitedElement != ActiveElement.None));

			graphics.Rasterizer.AddOutline (path);
			graphics.RenderSolid (this.GetColor (0));

			//	Dessine le texte.
			rect = this.bounds;
			rect.Deflate (ObjectComment.textMargin);
			this.textLayoutComment.LayoutSize = rect.Size;
			this.textLayoutComment.Paint (rect.BottomLeft, graphics, Rectangle.MaxValue, this.GetColor (this.IsDarkColorMain ? 1:0), GlyphPaintStyle.Normal);
		}

		public override void DrawForeground(Graphics graphics)
		{
			//	Dessine le dessus de l'objet.
			if (!this.isVisible)
			{
				return;
			}

			Rectangle rh = Rectangle.Empty;
			if (this.hilitedElement != ActiveElement.None)
			{
				rh = this.HeaderRectangle;
			}

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
			this.DrawColorButton(graphics, ActiveElement.CommentColor1, 0, MainColor.Yellow);
			this.DrawColorButton(graphics, ActiveElement.CommentColor2, 1, MainColor.Orange);
			this.DrawColorButton(graphics, ActiveElement.CommentColor3, 2, MainColor.Red);
			this.DrawColorButton(graphics, ActiveElement.CommentColor4, 3, MainColor.Lilac);
			this.DrawColorButton(graphics, ActiveElement.CommentColor5, 4, MainColor.Purple);
			this.DrawColorButton(graphics, ActiveElement.CommentColor6, 5, MainColor.Blue);
			this.DrawColorButton(graphics, ActiveElement.CommentColor7, 6, MainColor.Green);
			this.DrawColorButton(graphics, ActiveElement.CommentColor8, 7, MainColor.DarkGrey);

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
			Point p = this.PositionAttachToLinkButton;
			if (!p.IsZero)
			{
				if (this.hilitedElement == ActiveElement.CommentAttachTo)
				{
					this.DrawRoundButton (graphics, p, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, true, false);
				}
				else if (this.IsHeaderHilite && !this.isDraggingMove && !this.isDraggingWidth && !this.isDraggingAttach)
				{
					this.DrawRoundButton (graphics, p, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, false, false);
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
						this.hilitedElement == ActiveElement.CommentColor1 ||
						this.hilitedElement == ActiveElement.CommentColor2 ||
						this.hilitedElement == ActiveElement.CommentColor3 ||
						this.hilitedElement == ActiveElement.CommentColor4 ||
						this.hilitedElement == ActiveElement.CommentColor5 ||
						this.hilitedElement == ActiveElement.CommentColor6 ||
						this.hilitedElement == ActiveElement.CommentColor7 ||
						this.hilitedElement == ActiveElement.CommentColor8 ||
						this.hilitedElement == ActiveElement.CommentAttachTo);
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

			double d = Point.Distance(himself, other) - ObjectEdge.roundFrameRadius;

			if (this.attachObject is LinkableObject)
			{
				other = Point.Move(other, himself, ObjectEdge.roundFrameRadius);
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

				if (pos.IsZero && this.attachObject is LinkableObject)
				{
					var box = this.attachObject as LinkableObject;
					Rectangle boxBounds = box.Bounds;
					boxBounds.Deflate(ObjectEdge.roundFrameRadius);

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

		protected Point GetAttachOther(AttachMode mode)
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
					boxBounds.Deflate(ObjectEdge.roundFrameRadius);

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
				boxBounds.Deflate(ObjectEdge.roundFrameRadius);

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
			return new Point(this.bounds.Left+AbstractObject.buttonSquare+(AbstractObject.buttonSquare+0.5)*rank*2, this.bounds.Bottom-1-AbstractObject.buttonSquare);
		}

		protected Color ColorComment(bool hilited)
		{
			if (hilited)
			{
				return this.GetColorAdjusted(this.GetColorMain(), 0.3);
			}
			else
			{
				return this.GetColorAdjusted(this.GetColorMain(), 0.2);
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
				return this.GetColorAdjusted(this.GetColorMain(), 0.9);
			}
			else
			{
				return this.GetColorAdjusted(this.GetColorMain(), 0.7);
			}
		}


		#region Serialization
		public void WriteXml(XmlWriter writer)
		{
#if false
			//	Sérialise toutes les informations du commentaire.
			//	Utilisé seulement pour les commentaires associés à des boîtes.
			//	Les commentaires associés à des connexions sont sérialisés par Field.
			writer.WriteStartElement(Xml.Comment);
			
			writer.WriteElementString(Xml.Bounds, this.bounds.ToString());
			writer.WriteElementString(Xml.Text, this.textLayoutComment.Text);
			writer.WriteElementString(Xml.Color, this.boxColor.ToString());

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

					if (name == Xml.Bounds)
					{
						this.bounds = Rectangle.Parse(element);
					}
					else if (name == Xml.Text)
					{
						this.textLayoutComment.Text = element;
					}
					else if (name == Xml.Color)
					{
						this.boxColor = (MainColor) System.Enum.Parse (typeof (MainColor), element);
					}
					else
					{
						throw new System.NotSupportedException(string.Format("Unexpected XML node {0} found in comment", name));
					}
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					System.Diagnostics.Debug.Assert(reader.Name == Xml.Comment);
					break;
				}
				else
				{
					reader.Read();
				}
			}
#endif
		}
		#endregion


		private static readonly double			commentHeaderHeight = 24;
		private static readonly double			textMargin = 5;
		private static readonly double			queueThickness = 5;

		private Rectangle						bounds;
		private AbstractObject					attachObject;
		private bool							isVisible;
		private TextLayout						textLayoutTitle;
		private TextLayout						textLayoutComment;

		private Point							initialPos;
		private bool							isDraggingMove;
		private bool							isDraggingWidth;
		private bool							isDraggingAttach;
		private Point							draggingPos;

		private AbstractTextField				editingTextField;
	}
}
