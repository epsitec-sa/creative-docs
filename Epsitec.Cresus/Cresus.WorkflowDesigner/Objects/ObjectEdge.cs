//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public class ObjectEdge : LinkableObject
	{
		public ObjectEdge(Editor editor, AbstractEntity entity)
			: base (editor, entity)
		{
			System.Diagnostics.Debug.Assert (this.Entity != null);

			this.title = new TextLayout ();
			this.title.DefaultFontSize = 12;
			this.title.Alignment = ContentAlignment.MiddleCenter;
			this.title.BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;

			this.subtitle = new TextLayout ();
			this.subtitle.DefaultFontSize = 9;
			this.subtitle.Alignment = ContentAlignment.MiddleLeft;
			this.subtitle.BreakMode = TextBreakMode.Hyphenate;

			this.isExtended = true;

			this.UpdateTitle ();
			this.UpdateSubtitle ();
		}


		public string Title
		{
			//	Titre au sommet de la bo�te (nom du noeud).
			get
			{
				return this.titleString;
			}
			set
			{
				if (this.titleString != value)
				{
					this.titleString = value;
					this.title.Text = Misc.Bold (this.titleString);
				}
			}
		}

		public string Subtitle
		{
			//	Titre au sommet de la bo�te (nom du noeud).
			get
			{
				return this.subtitleString;
			}
			set
			{
				if (this.subtitleString != value)
				{
					this.subtitleString = value;
					this.subtitle.Text = this.subtitleString;
				}
			}
		}

		public bool IsExtended
		{
			//	Etat de la bo�te (compact ou �tendu).
			//	En mode compact, seul le titre est visible.
			get
			{
				return this.isExtended;
			}
			set
			{
				if (this.isExtended != value)
				{
					this.isExtended = value;

					this.UpdateBounds ();
					this.editor.UpdateAfterGeometryChanged (this);
					this.editor.SetLocalDirty ();
				}
			}
		}

		public override Rectangle Bounds
		{
			//	Retourne la bo�te de l'objet.
			//	Attention: le dessin peut d�border, par exemple pour l'ombre.
			get
			{
				return this.bounds;
			}
		}

		public override void Move(double dx, double dy)
		{
			//	D�place l'objet.
			this.bounds.Offset(dx, dy);
		}


		public override void CreateLinks()
		{
			this.objectLinks.Clear ();
	
			var link = new ObjectLink (this.editor, this.Entity);
			link.SrcObject = this;
			link.DstObject = this.editor.SearchObject (this.Entity.NextNode);

			this.objectLinks.Add (link);
		}

		public override Vector GetLinkVector(LinkAnchor anchor, Point dstPos, bool isDst)
		{
			// * 1.5 pour ne pas trop s'approcher des coins arrondis
			double r = ObjectEdge.roundFrameRadius * (isDst ? 2.5 : 1.5);

			if (anchor == LinkAnchor.Left || anchor == LinkAnchor.Right)
			{
				double x, y;

				if (this.isExtended)
				{
					x = (anchor == LinkAnchor.Left) ? this.bounds.Left : this.bounds.Right;

					if (dstPos.Y < this.bounds.Bottom+r)
					{
						y = this.bounds.Bottom+r;
					}
					else if (dstPos.Y > this.bounds.Top-r)
					{
						y = this.bounds.Top-r;
					}
					else
					{
						y = dstPos.Y;
					}
				}
				else
				{
					if (isDst)
					{
						return Vector.Zero;
					}
					else
					{
						x = (anchor == LinkAnchor.Left) ? this.bounds.Left : this.bounds.Right;
						y = this.bounds.Center.Y;
					}
				}

				Point p = new Point (x, y);
				Size dir = new Size ((anchor == LinkAnchor.Left) ? -1 : 1, 0);

				return new Vector (p, dir);
			}

			if (anchor == LinkAnchor.Bottom || anchor == LinkAnchor.Top)
			{
				double x;
				double y = (anchor == LinkAnchor.Bottom) ? this.bounds.Bottom : this.bounds.Top;

				if (dstPos.X < this.bounds.Left+r)
				{
					x = this.bounds.Left+r;
				}
				else if (dstPos.X > this.bounds.Right-r)
				{
					x = this.bounds.Right-r;
				}
				else
				{
					x = dstPos.X;
				}

				Point p = new Point (x, y);
				Size dir = new Size (0, (anchor == LinkAnchor.Bottom) ? -1 : 1);

				return new Vector (p, dir);
			}

			return Vector.Zero;
		}


		public bool IsHilitedForEdgeChanging
		{
			//	Indique si cet objet est mis en �vidence pendant un changement de noeud destination (EdgeChangeDst).
			get
			{
				return this.isHilitedForEdgeChanging;
			}
			set
			{
				this.isHilitedForEdgeChanging = value;
			}
		}


		public override void AcceptEdition()
		{
			if (this.editingElement == ActiveElement.EdgeHeader)
			{
				this.Entity.Name = this.editingTextField.Text;
				this.UpdateTitle ();
			}

			if (this.editingElement == ActiveElement.EdgeDescription)
			{
				this.Entity.Description = this.editingTextField.Text;
				this.UpdateSubtitle ();
			}

			this.StopEdition ();
		}

		public override void CancelEdition()
		{
			this.StopEdition ();
		}

		private void StartEdition(ActiveElement element)
		{
			Rectangle rect = Rectangle.Empty;
			string text = null;

			if (element == ActiveElement.EdgeHeader)
			{
				rect = this.RectangleTitle;
				rect.Deflate (4, 6);

				text = this.Entity.Name.ToString ();

				this.editingTextField = new TextField ();
			}

			if (element == ActiveElement.EdgeDescription)
			{
				rect = this.RectangleSubtitle;
				rect.Inflate (6, 1);

				text = this.Entity.Description.ToString ();

				var field = new TextFieldMulti ();
				field.ScrollerVisibility = false;
				this.editingTextField = field;
			}

			if (rect.IsEmpty)
			{
				return;
			}

			this.editingElement = element;

			Point p1 = this.editor.ConvEditorToWidget (rect.TopLeft);
			Point p2 = this.editor.ConvEditorToWidget (rect.BottomRight);
			double width  = System.Math.Max (p2.X-p1.X, 175);
			double height = System.Math.Max (p1.Y-p2.Y, 20);

			rect = new Rectangle (new Point (p1.X, p1.Y-height), new Size (width, height));

			double thickness = 2;
			Rectangle frameRect = rect;
			frameRect.Inflate (thickness);

			double descriptionHeight = 10+14*5;  // hauteur pour 5 lignes
			Rectangle descriptionRect = new Rectangle (rect.Left, rect.Bottom-descriptionHeight-thickness+1, rect.Width, descriptionHeight);

			this.editingTextField.Parent = this.editor;
			this.editingTextField.SetManualBounds (rect);
			this.editingTextField.Text = text;
			this.editingTextField.TabIndex = 1;
			this.editingTextField.SelectAll ();
			this.editingTextField.Focus ();

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
			if (this.isDragging || this.isChangeWidth)
			{
				return null;  // pas de tooltip
			}

			return base.GetToolTipText(element);
		}

		public override bool MouseMove(Message message, Point pos)
		{
			//	Met en �vidence la bo�te selon la position de la souris.
			//	Si la souris est dans cette bo�te, retourne true.
			if (this.isDragging)
			{
				this.DraggingMouseMove (pos);
				return true;
			}
			
			if (this.isChangeWidth)
			{
				Rectangle bounds = this.Bounds;
				bounds.Width = this.editor.GridAlign (System.Math.Max (pos.X-this.changeWidthPos+this.changeWidthInitial, 120));
				this.SetBounds (bounds);
				this.editor.UpdateLinks ();
				return true;
			}

			return base.MouseMove (message, pos);
		}

		public override void MouseDown(Message message, Point pos)
		{
			//	Le bouton de la souris est press�.
			base.MouseDown (message, pos);

			this.initialPos = pos;

			if (this.hilitedElement == ActiveElement.EdgeHeader && this.editor.NodeCount > 1)
			{
				this.isDragging = true;
				this.draggingOffset = pos-this.bounds.BottomLeft;
				this.editor.Invalidate();
				this.editor.LockObject(this);
			}

			if (this.hilitedElement == ActiveElement.EdgeChangeWidth)
			{
				this.isChangeWidth = true;
				this.changeWidthPos = pos.X;
				this.changeWidthInitial = this.bounds.Width;
				this.editor.LockObject (this);
			}
		}

		public override void MouseUp(Message message, Point pos)
		{
			//	Le bouton de la souris est rel�ch�.
			base.MouseUp (message, pos);

			if (pos == this.initialPos)
			{
				if (this.hilitedElement == ActiveElement.EdgeHeader ||
					this.hilitedElement == ActiveElement.EdgeDescription)
				{
					if (this.isDragging)
					{
						this.editor.UpdateAfterMoving (this);
						this.isDragging = false;
						this.editor.LockObject (null);
						this.editor.SetLocalDirty ();
					}

					this.StartEdition (this.hilitedElement);
					return;
				}
			}

			if (this.isDragging)
			{
				this.editor.UpdateAfterMoving (this);
				this.isDragging = false;
				this.editor.LockObject (null);
				this.editor.SetLocalDirty ();
				return;
			}

			if (this.isChangeWidth)
			{
				this.editor.UpdateAfterMoving (this);
				this.isChangeWidth = false;
				this.editor.LockObject (null);
				this.editor.SetLocalDirty ();
				return;
			}

			if (this.hilitedElement == ActiveElement.EdgeExtend)
			{
				this.IsExtended = !this.IsExtended;
			}

			if (this.hilitedElement == ActiveElement.EdgeOpenLink)
			{
			}

			if (this.hilitedElement == ActiveElement.EdgeClose)
			{
				this.editor.CloseObject (this);
				this.editor.UpdateAfterAddOrRemoveEdge(null);
			}

			if (this.hilitedElement == ActiveElement.EdgeComment)
			{
				this.AddComment();
			}

			if (this.hilitedElement == ActiveElement.EdgeColor1)
			{
				this.BackgroundMainColor = MainColor.Yellow;
			}

			if (this.hilitedElement == ActiveElement.EdgeColor2)
			{
				this.BackgroundMainColor = MainColor.Orange;
			}

			if (this.hilitedElement == ActiveElement.EdgeColor3)
			{
				this.BackgroundMainColor = MainColor.Red;
			}

			if (this.hilitedElement == ActiveElement.EdgeColor4)
			{
				this.BackgroundMainColor = MainColor.Lilac;
			}

			if (this.hilitedElement == ActiveElement.EdgeColor5)
			{
				this.BackgroundMainColor = MainColor.Purple;
			}

			if (this.hilitedElement == ActiveElement.EdgeColor6)
			{
				this.BackgroundMainColor = MainColor.Blue;
			}

			if (this.hilitedElement == ActiveElement.EdgeColor7)
			{
				this.BackgroundMainColor = MainColor.Green;
			}

			if (this.hilitedElement == ActiveElement.EdgeColor8)
			{
				this.BackgroundMainColor = MainColor.Grey;
			}
		}

		public override ActiveElement MouseDetectBackground(Point pos)
		{
			//	D�tecte l'�l�ment actif vis� par la souris.
			ActiveElement element = base.MouseDetectBackground (pos);
			if (element != ActiveElement.None)
			{
				return element;
			}

			if (this.RectangleTitle.Contains (pos))
			{
				return ActiveElement.EdgeHeader;
			}

			if (this.RectangleSubtitle.Contains (pos))
			{
				return ActiveElement.EdgeDescription;
			}

			if (this.bounds.Contains (pos))
			{
				return ActiveElement.EdgeInside;
			}

			return ActiveElement.None;
		}

		public override ActiveElement MouseDetectForeground(Point pos)
		{
			//	D�tecte l'�l�ment actif vis� par la souris.
			ActiveElement element = base.MouseDetectForeground (pos);
			if (element != ActiveElement.None)
			{
				return element;
			}

			//	Souris dans le bouton compact/�tendu ?
			if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked)
			{
				if (this.DetectRoundButton (this.PositionExtendButton, pos))
				{
					return ActiveElement.EdgeExtend;
				}

				//	Souris dans le bouton de fermeture ?
				if (this.DetectRoundButton (this.PositionCloseButton, pos))
				{
					return ActiveElement.EdgeClose;
				}

				//	Souris dans le bouton des commentaires ?
				if (this.DetectRoundButton (this.PositionCommentButton, pos))
				{
					return ActiveElement.EdgeComment;
				}

				if (this.isExtended)
				{
					if (this.DetectRoundButton (this.PositionChangeWidthButton, pos))
					{
						return ActiveElement.EdgeChangeWidth;
					}

					//	Souris dans le bouton d'ouverture ?
					if (this.DetectRoundButton (this.PositionOpenLinkButton, pos))
					{
						return ActiveElement.EdgeOpenLink;
					}

					//	Souris dans le bouton des couleurs ?
					if (this.DetectSquareButton (this.PositionColorButton (0), pos))
					{
						return ActiveElement.EdgeColor1;
					}

					if (this.DetectSquareButton (this.PositionColorButton (1), pos))
					{
						return ActiveElement.EdgeColor2;
					}

					if (this.DetectSquareButton (this.PositionColorButton (2), pos))
					{
						return ActiveElement.EdgeColor3;
					}

					if (this.DetectSquareButton (this.PositionColorButton (3), pos))
					{
						return ActiveElement.EdgeColor4;
					}

					if (this.DetectSquareButton (this.PositionColorButton (4), pos))
					{
						return ActiveElement.EdgeColor5;
					}

					if (this.DetectSquareButton (this.PositionColorButton (5), pos))
					{
						return ActiveElement.EdgeColor6;
					}

					if (this.DetectSquareButton (this.PositionColorButton (6), pos))
					{
						return ActiveElement.EdgeColor7;
					}

					if (this.DetectSquareButton (this.PositionColorButton (7), pos))
					{
						return ActiveElement.EdgeColor8;
					}
				}
			}

			return ActiveElement.None;
		}

		public override bool IsMousePossible(ActiveElement element)
		{
			//	Indique si l'op�ration est possible.
			return true;
		}


		private void AddComment()
		{
			//	Ajoute un commentaire � la bo�te.
			if (this.comment == null)
			{
				this.comment = new ObjectComment (this.editor, this.Entity);
				this.comment.AttachObject = this;

				Rectangle rect = this.bounds;
				rect.Left = rect.Right+30;
				rect.Width = System.Math.Max (this.bounds.Width, AbstractObject.infoMinWidth);
				this.comment.SetBounds (rect);
				this.comment.UpdateHeight ();  // adapte la hauteur en fonction du contenu

				this.editor.AddComment (this.comment);
				this.editor.UpdateAfterCommentChanged ();

				this.comment.EditComment ();  // �dite tout de suite le texte du commentaire
			}
			else
			{
				this.comment.IsVisible = !this.comment.IsVisible;
			}

			this.editor.SetLocalDirty ();
		}


		public override void DrawBackground(Graphics graphics)
		{
			//	Dessine le fond de l'objet.
			//	H�ritage	->	Traitill�
			//	Interface	->	Trait plein avec o---
			Rectangle rect;

			bool dragging = (this.hilitedElement == ActiveElement.EdgeHeader || this.isHilitedForEdgeChanging);

			//	Dessine l'ombre.
			rect = this.bounds;
			rect.Offset (ObjectEdge.shadowOffset, -(ObjectEdge.shadowOffset));
			this.DrawRoundShadow (graphics, rect, ObjectEdge.roundFrameRadius, (int) ObjectEdge.shadowOffset, 0.2);

			//	Construit le chemin du cadre.
			rect = this.bounds;
			rect.Deflate(1);
			Path path = this.PathRoundRectangle (rect, ObjectEdge.roundFrameRadius);

			//	Dessine l'int�rieur en blanc.
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(this.GetColor(1));

			//	Dessine l'int�rieur en d�grad�.
			graphics.Rasterizer.AddSurface(path);
			Color c1 = this.GetColorMain(dragging ? 0.8 : 0.4);
			Color c2 = this.GetColorMain(dragging ? 0.4 : 0.1);
			this.RenderHorizontalGradient(graphics, this.bounds, c1, c2);

			Color colorLine = this.GetColor(0.9);
			if (dragging)
			{
				colorLine = this.GetColorMain(0.3);
			}

			Color colorFrame = dragging ? this.GetColorMain() : this.GetColor(0);

			//	Dessine en blanc la zone pour les champs.
			if (this.isExtended)
			{
				Rectangle inside = new Rectangle (this.bounds.Left+1, this.bounds.Bottom+AbstractObject.footerHeight, this.bounds.Width-2, this.bounds.Height-AbstractObject.footerHeight-AbstractObject.headerHeight);
				graphics.AddFilledRectangle (inside);
				graphics.RenderSolid (this.GetColor (1));
				graphics.AddFilledRectangle (inside);
				Color ci1 = this.GetColorMain (dragging ? 0.2 : 0.1);
				Color ci2 = this.GetColorMain (0.0);
				this.RenderHorizontalGradient (graphics, inside, ci1, ci2);

				//	Ombre sup�rieure.
				Rectangle shadow = new Rectangle (this.bounds.Left+1, this.bounds.Top-AbstractObject.headerHeight-8, this.bounds.Width-2, 8);
				graphics.AddFilledRectangle (shadow);
				this.RenderVerticalGradient (graphics, shadow, Color.FromAlphaRgb (0.0, 0, 0, 0), Color.FromAlphaRgb (0.3, 0, 0, 0));

				graphics.AddLine (this.bounds.Left+2, this.bounds.Top-AbstractObject.headerHeight-0.5, this.bounds.Right-2, this.bounds.Top-AbstractObject.headerHeight-0.5);
				graphics.AddLine (this.bounds.Left+2, this.bounds.Bottom+AbstractObject.footerHeight+0.5, this.bounds.Right-2, this.bounds.Bottom+AbstractObject.footerHeight+0.5);
				graphics.RenderSolid (colorFrame);
			}

			//	Dessine le titre.
			Color titleColor = dragging ? this.GetColor (1) : this.GetColor (0);

			rect = this.RectangleTitle;
			rect.Deflate (2, 2);
			this.title.LayoutSize = rect.Size;
			this.title.Paint (rect.BottomLeft, graphics, Rectangle.MaxValue, titleColor, GlyphPaintStyle.Normal);

			//	Dessine le sous-titre.
			if (this.isExtended)
			{
				Color subtitleColor = this.GetColor (0);

				rect = this.RectangleSubtitle;
				this.subtitle.LayoutSize = rect.Size;
				this.subtitle.Paint (rect.BottomLeft, graphics, Rectangle.MaxValue, subtitleColor, GlyphPaintStyle.Normal);
			}

			//	Dessine le cadre en noir.
			graphics.Rasterizer.AddOutline (path, 2);
			graphics.RenderSolid (colorFrame);

			//	Dessine les connexions.
			this.DrawLinks (graphics);
		}

		public override void DrawForeground(Graphics graphics)
		{
			//	Dessine le bouton compact/�tendu.
			GlyphShape shape = this.isExtended ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;
			if (this.hilitedElement == ActiveElement.EdgeExtend)
			{
				this.DrawRoundButton (graphics, this.PositionExtendButton, AbstractObject.buttonRadius, shape, true, false);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawRoundButton (graphics, this.PositionExtendButton, AbstractObject.buttonRadius, shape, false, false);
			}

			//	Dessine le bouton d'ouverture.
			if (this.hilitedElement == ActiveElement.EdgeOpenLink)
			{
				this.DrawRoundButton (graphics, this.PositionOpenLinkButton, AbstractObject.buttonRadius, GlyphShape.ArrowRight, true, false, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawRoundButton (graphics, this.PositionOpenLinkButton, AbstractObject.buttonRadius, GlyphShape.ArrowRight, false, false, true);
			}

			//	Dessine le bouton de fermeture.
			if (this.hilitedElement == ActiveElement.EdgeClose)
			{
				this.DrawRoundButton (graphics, this.PositionCloseButton, AbstractObject.buttonRadius, GlyphShape.Close, true, false, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawRoundButton (graphics, this.PositionCloseButton, AbstractObject.buttonRadius, GlyphShape.Close, false, false, true);
			}

			//	Dessine le bouton des commentaires.
			if (this.hilitedElement == ActiveElement.EdgeComment)
			{
				this.DrawRoundButton (graphics, this.PositionCommentButton, AbstractObject.buttonRadius, "C", true, false);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawRoundButton (graphics, this.PositionCommentButton, AbstractObject.buttonRadius, "C", false, false);
			}

			//	Dessine le bouton des couleurs.
			if (this.hilitedElement == ActiveElement.EdgeColor1)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (0), MainColor.Yellow, this.boxColor == MainColor.Yellow, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (0), MainColor.Yellow, this.boxColor == MainColor.Yellow, false);
			}

			if (this.hilitedElement == ActiveElement.EdgeColor2)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (1), MainColor.Orange, this.boxColor == MainColor.Orange, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (1), MainColor.Orange, this.boxColor == MainColor.Orange, false);
			}

			if (this.hilitedElement == ActiveElement.EdgeColor3)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (2), MainColor.Red, this.boxColor == MainColor.Red, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (2), MainColor.Red, this.boxColor == MainColor.Red, false);
			}

			if (this.hilitedElement == ActiveElement.EdgeColor4)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (3), MainColor.Lilac, this.boxColor == MainColor.Lilac, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (3), MainColor.Lilac, this.boxColor == MainColor.Lilac, false);
			}

			if (this.hilitedElement == ActiveElement.EdgeColor5)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (4), MainColor.Purple, this.boxColor == MainColor.Purple, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (4), MainColor.Purple, this.boxColor == MainColor.Purple, false);
			}

			if (this.hilitedElement == ActiveElement.EdgeColor6)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (5), MainColor.Blue, this.boxColor == MainColor.Blue, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (5), MainColor.Blue, this.boxColor == MainColor.Blue, false);
			}

			if (this.hilitedElement == ActiveElement.EdgeColor7)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (6), MainColor.Green, this.boxColor == MainColor.Green, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (6), MainColor.Green, this.boxColor == MainColor.Green, false);
			}

			if (this.hilitedElement == ActiveElement.EdgeColor8)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (7), MainColor.Grey, this.boxColor == MainColor.Grey, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (7), MainColor.Grey, this.boxColor == MainColor.Grey, false);
			}

			//	Dessine le bouton pour changer la largeur.
			if (this.hilitedElement == ActiveElement.EdgeChangeWidth)
			{
				this.DrawRoundButton (graphics, this.PositionChangeWidthButton, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, true, false);
			}
			if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.hilitedElement == ActiveElement.EdgeHeader && !this.isDragging)
			{
				this.DrawRoundButton (graphics, this.PositionChangeWidthButton, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, false, false);
			}
		}


		private bool IsHeaderHilite
		{
			//	Indique si la souris est dans l'en-t�te.
			get
			{
				if (this.editor.CurrentModifyMode == Editor.ModifyMode.Locked)
				{
					return false;
				}

				return (this.hilitedElement == ActiveElement.EdgeHeader ||
						this.hilitedElement == ActiveElement.EdgeComment ||
						this.hilitedElement == ActiveElement.EdgeColor1 ||
						this.hilitedElement == ActiveElement.EdgeColor2 ||
						this.hilitedElement == ActiveElement.EdgeColor3 ||
						this.hilitedElement == ActiveElement.EdgeColor4 ||
						this.hilitedElement == ActiveElement.EdgeColor5 ||
						this.hilitedElement == ActiveElement.EdgeColor6 ||
						this.hilitedElement == ActiveElement.EdgeColor7 ||
						this.hilitedElement == ActiveElement.EdgeColor8 ||
						this.hilitedElement == ActiveElement.EdgeExtend ||
						this.hilitedElement == ActiveElement.EdgeOpenLink ||
						this.hilitedElement == ActiveElement.EdgeClose);
			}
		}

		private Rectangle RectangleTitle
		{
			get
			{
				return new Rectangle (this.bounds.Left, this.bounds.Top-AbstractObject.headerHeight, this.bounds.Width, AbstractObject.headerHeight);
			}
		}

		private Rectangle RectangleSubtitle
		{
			get
			{
				var rect = this.bounds;
				rect.Deflate (8, 8, AbstractObject.headerHeight+2, AbstractObject.footerHeight+2);

				return rect;
			}
		}

		private Point PositionCommentButton
		{
			//	Retourne la position du bouton pour montrer le commentaire.
			get
			{
				return new Point (this.bounds.Left+AbstractObject.buttonRadius+6, this.bounds.Top-AbstractObject.headerHeight/2);
			}
		}

		private Point PositionOpenLinkButton
		{
			//	Retourne la position du bouton pour ouvrir.
			get
			{
				if (this.isExtended)
				{
					return new Point (this.bounds.Left+AbstractObject.buttonRadius*3+8, this.bounds.Top-AbstractObject.headerHeight/2);
				}
				else
				{
					return Point.Zero;
				}
			}
		}

		private Point PositionExtendButton
		{
			//	Retourne la position du bouton pour �tendre.
			get
			{
				return new Point (this.bounds.Right-AbstractObject.buttonRadius*3-8, this.bounds.Top-AbstractObject.headerHeight/2);
			}
		}

		private Point PositionCloseButton
		{
			//	Retourne la position du bouton pour fermer.
			get
			{
				return new Point (this.bounds.Right-AbstractObject.buttonRadius-6, this.bounds.Top-AbstractObject.headerHeight/2);
			}
		}

		private Point PositionColorButton(int rank)
		{
			//	Retourne la position du bouton pour choisir la couleur.
			if (this.isExtended)
			{
				return new Point (this.bounds.Left-2+(AbstractObject.buttonSquare+0.5)*(rank+1)*2, this.bounds.Bottom+4+AbstractObject.buttonSquare);
			}
			else
			{
				return Point.Zero;
			}
		}

		private Point PositionChangeWidthButton
		{
			//	Retourne la position du bouton pour changer la largeur.
			get
			{
				if (this.isExtended)
				{
					return new Point (this.bounds.Right-1, this.bounds.Bottom+AbstractObject.footerHeight/2+1);
				}
				else
				{
					return Point.Zero;
				}
			}
		}

		private string GetGroupTooltip(int rank)
		{
			//	Retourne le tooltip � afficher pour un groupe.
			return null;  // pas de tooltip
		}


		private void UpdateTitle()
		{
			//	Met � jour le titre du noeud.
			this.Title = this.Entity.Name.ToString ();
		}

		private void UpdateSubtitle()
		{
			//	Met � jour le sous-titre du noeud.
			this.Subtitle = this.Entity.Description.ToString ();
		}

		private void UpdateBounds()
		{
			Point origin = this.bounds.TopLeft;
			double width = this.bounds.Width;
			double height = this.isExtended ? ObjectEdge.frameSize.Height : AbstractObject.headerHeight;

			Rectangle bounds = new Rectangle (origin.X, origin.Y-height, width, height);
			this.SetBounds (bounds);
		}


		public WorkflowEdgeEntity Entity
		{
			get
			{
				return this.entity as WorkflowEdgeEntity;
			}
		}


		#region Serialization
		public void WriteXml(XmlWriter writer)
		{
			//	S�rialise toutes les informations de la bo�te et de ses champs.
#if false
			writer.WriteStartElement(Xml.Box);
			
			writer.WriteElementString(Xml.Druid, this.cultureMap.Id.ToString());
			writer.WriteElementString(Xml.Bounds, this.bounds.ToString());
			writer.WriteElementString(Xml.IsExtended, this.isExtended.ToString(System.Globalization.CultureInfo.InvariantCulture));

			if (this.columnsSeparatorRelative1 != 0.5)
			{
				writer.WriteElementString(Xml.ColumnsSeparatorRelative1, this.columnsSeparatorRelative1.ToString(System.Globalization.CultureInfo.InvariantCulture));
			}
			
			writer.WriteElementString(Xml.Color, this.boxColor.ToString());

			foreach (Field field in this.fields)
			{
				field.WriteXml(writer);
			}

			if (this.comment != null && this.comment.IsVisible)  // commentaire associ� ?
			{
				this.comment.WriteXml(writer);
			}
			
			if (this.info != null && this.info.IsVisible)  // informations associ�es ?
			{
				this.info.WriteXml(writer);
			}
			
			writer.WriteEndElement();
#endif
		}

		public void ReadXml(XmlReader reader)
		{
#if false
			this.fields.Clear();

			reader.Read();

			while (true)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					string name = reader.LocalName;

					if (name == Xml.Field)
					{
						Field field = new Field(this.editor);
						field.ReadXml(reader);
						reader.Read();
						this.fields.Add(field);
					}
					else if (name == Xml.Comment)
					{
						this.comment = new ObjectComment(this.editor);
						this.comment.ReadXml(reader);
						this.comment.AttachObject = this;
						this.comment.UpdateHeight();  // adapte la hauteur en fonction du contenu
						this.editor.AddComment(this.comment);
						reader.Read();
					}
					else if (name == Xml.Info)
					{
						this.info = new ObjectInfo(this.editor);
						this.info.ReadXml(reader);
						this.info.AttachObject = this;
						this.info.UpdateHeight();  // adapte la hauteur en fonction du contenu
						this.editor.AddInfo(this.info);
						reader.Read();
					}
					else
					{
						string element = reader.ReadElementString();

						if (name == Xml.Druid)
						{
							Druid druid = Druid.Parse(element);
							if (druid.IsValid)
							{
								Module module = this.SearchModule(druid);
								this.cultureMap = module.AccessEntities.Accessor.Collection[druid];
							}
						}
						else if (name == Xml.Bounds)
						{
							this.bounds = Rectangle.Parse(element);
						}
						else if (name == Xml.IsExtended)
						{
							this.isExtended = bool.Parse(element);
						}
						else if (name == Xml.ColumnsSeparatorRelative1)
						{
							this.columnsSeparatorRelative1 = double.Parse(element);
						}
						else if (name == Xml.Color)
						{
							this.boxColor = (MainColor) System.Enum.Parse(typeof(MainColor), element);
						}
						else
						{
							throw new System.NotSupportedException(string.Format("Unexpected XML node {0} found in box", name));
						}
					}
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					System.Diagnostics.Debug.Assert(reader.Name == Xml.Box);
					break;
				}
				else
				{
					reader.Read();
				}
			}
#endif
		}

		public void AdjustAfterRead()
		{
			//	Ajuste le contenu de la bo�te apr�s sa d�s�rialisation.
		}
		#endregion


		public static readonly Size				frameSize = new Size (200, 100);
		public static readonly double			roundFrameRadius = 12;
		private static readonly double			shadowOffset = 6;

		private string							titleString;
		private TextLayout						title;
		private string							subtitleString;
		private TextLayout						subtitle;

		private bool							isExtended;
		private Point							initialPos;
		private bool							isDragging;
		private bool							isChangeWidth;
		private double							changeWidthPos;
		private double							changeWidthInitial;
		private bool							isHilitedForEdgeChanging;

		private ActiveElement					editingElement;
		private AbstractTextField				editingTextField;
	}
}
