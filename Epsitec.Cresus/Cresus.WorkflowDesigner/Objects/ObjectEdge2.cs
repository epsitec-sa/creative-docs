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
	public class ObjectEdge2 : LinkableObject
	{
		public ObjectEdge2(Editor editor, AbstractEntity entity)
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

			this.UpdateTitle ();
			this.UpdateSubtitle ();
		}


		public string Title
		{
			//	Titre au sommet de la boîte (nom du noeud).
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
			//	Titre au sommet de la boîte (nom du noeud).
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

		public ObjectComment Comment
		{
			//	Commentaire lié.
			get
			{
				return this.comment;
			}
			set
			{
				this.comment = value;
			}
		}

		public override Rectangle Bounds
		{
			//	Retourne la boîte de l'objet.
			//	Attention: le dessin peut déborder, par exemple pour l'ombre.
			get
			{
				return this.bounds;
			}
		}

		public override void Move(double dx, double dy)
		{
			//	Déplace l'objet.
			this.bounds.Offset(dx, dy);
		}

		public void SetBounds(Rectangle bounds)
		{
			//	Modifie la boîte de l'objet.
			Point p1 = this.bounds.TopLeft;
			this.bounds = bounds;
			Point p2 = this.bounds.TopLeft;

			//	S'il existe un commentaire associé, il doit aussi être déplacé.
			if (this.comment != null)
			{
				Rectangle rect = this.comment.InternalBounds;
				rect.Offset (p2-p1);
				this.comment.SetBounds (rect);
			}
		}


		public override void CreateLinks()
		{
			this.links.Clear ();
	
			var link = new Link (this.editor, this);
			link.DstNode = this.editor.SearchObject (this.Entity.NextNode);

			//?var objectLink = new ObjectLink (this.editor, this.Entity);
			//?objectLink.Link = link;

			//?link.ObjectLink = objectLink;

			this.links.Add (link);
		}

		public override Point GetLinkSrcVerticalPosition(Point dstPos)
		{
			//	Retourne la position verticale pour un trait de liaison.
			if (dstPos.IsZero)
			{
				return this.bounds.Center;
			}

			double r = ObjectEdge2.roundFrameRadius;

			//?if (dstPos.X >= this.bounds.Left+r && dstPos.X <= this.bounds.Right-r)
			if (false)
			{
				if (dstPos.Y < this.bounds.Bottom+r)
				{
					return new Point (dstPos.X, this.bounds.Bottom);
				}
				else
				{
					return new Point (dstPos.X, this.bounds.Top);
				}
			}
			else
			{
				double x = (dstPos.X < this.bounds.Left) ? this.bounds.Left : this.bounds.Right;
				double y = dstPos.Y;

				if (dstPos.Y < this.bounds.Bottom+r)
				{
					y = this.bounds.Bottom+r;
				}

				if (dstPos.Y > this.bounds.Top-r)
				{
					y = this.bounds.Top-r;
				}

				return new Point (x, y);
			}
		}

		public override Point GetLinkDstPosition(double posv, ObjectNode.EdgeAnchor anchor)
		{
			//	Retourne la position où accrocher la destination.
			double r = ObjectEdge2.roundFrameRadius;

			if (anchor == ObjectNode.EdgeAnchor.Left || anchor == ObjectNode.EdgeAnchor.Right)
			{
				double x = (anchor == ObjectNode.EdgeAnchor.Left) ? this.bounds.Left : this.bounds.Right;
				double y;

				if (posv < this.bounds.Bottom+r)
				{
					y = this.bounds.Bottom+r;
				}
				else if (posv > this.bounds.Top-r)
				{
					y = this.bounds.Top-r;
				}
				else
				{
					y = posv;
				}

				return new Point (x, y);
			}

			if (anchor == ObjectNode.EdgeAnchor.Top)
			{
				return new Point (this.bounds.Center.X, this.bounds.Top);
			}

			if (anchor == ObjectNode.EdgeAnchor.Bottom)
			{
				return new Point (this.bounds.Center.X, this.bounds.Bottom);
			}

			return Point.Zero;
		}


		public bool IsHilitedForEdgeChanging
		{
			//	Indique si cet objet est mis en évidence pendant un changement de noeud destination (EdgeChangeDst).
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
			if (this.editingElement == ActiveElement.NodeHeader)
			{
				this.Entity.Name = this.editingTextField.Text;
				this.UpdateTitle ();
			}

			if (this.editingElement == ActiveElement.NodeEdgeName)
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

			if (element == ActiveElement.NodeHeader)
			{
				rect = this.RectangleTitle;
				rect.Deflate (4, 6);

				text = this.Entity.Name.ToString ();

				this.editingTextField = new TextField ();
			}

			if (element == ActiveElement.NodeEdgeName)
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


		protected override string GetToolTipText(ActiveElement element, int edgeRank)
		{
			//	Retourne le texte pour le tooltip.
			if (this.isDragging || this.isChangeWidth)
			{
				return null;  // pas de tooltip
			}

#if false
			switch (element)
			{
				case ActiveElement.BoxHeader:
					if (this.editor.BoxCount == 1)
					{
						return null;
					}
					else
					{
						if (this.isRoot)
						{
							return Res.Strings.Entities.Action.BoxHeader;
						}
						else if (this.editor.CurrentModifyMode == Editor.ModifyMode.Locked)
						{
							return Res.Strings.Entities.Action.BoxHeader1;
						}
						else
						{
							return Res.Strings.Entities.Action.BoxHeader2;
						}
					}

				case ActiveElement.BoxSources:
					if (this.sourcesList.Count == 0)
					{
						return null;
					}
					else
					{
						return Res.Strings.Entities.Action.BoxSources1;
					}

				case ActiveElement.BoxExtend:
					if (this.isExtended)
					{
						return Res.Strings.Entities.Action.BoxExtend1;
					}
					else
					{
						return Res.Strings.Entities.Action.BoxExtend2;
					}

				case ActiveElement.BoxComment:
					if (this.comment == null)
					{
						return Res.Strings.Entities.Action.BoxComment1;
					}
					else if (!this.comment.IsVisible)
					{
						return string.Format(Res.Strings.Entities.Action.BoxComment2, this.comment.Text);
					}
					else
					{
						return Res.Strings.Entities.Action.BoxComment3;
					}

				case ActiveElement.BoxInfo:
					if (this.info == null || !this.info.IsVisible)
					{
						return string.Format(Res.Strings.Entities.Action.BoxInfo1, this.GetInformations(true));
					}
					else
					{
						return Res.Strings.Entities.Action.BoxInfo2;
					}

				case ActiveElement.BoxClose:
					if (this.isRoot)
					{
						return null;
					}
					else
					{
						return Res.Strings.Entities.Action.BoxClose;
					}

				case ActiveElement.BoxFieldGroup:
					return this.GetGroupTooltip(fieldRank);

				case ActiveElement.BoxFieldExpression:
					string expression = this.fields[fieldRank].LocalExpression;
					string deepExpression = this.fields[fieldRank].InheritedExpression;
					if (!string.IsNullOrEmpty(expression))
					{
						return string.Format(Res.Strings.Entities.Action.BoxFieldExpression1, expression);
					}
					else if (expression != "" && !string.IsNullOrEmpty(deepExpression))
					{
						return string.Format(Res.Strings.Entities.Action.BoxFieldExpression1, deepExpression);
					}
					else
					{
						return Res.Strings.Entities.Action.BoxFieldExpression;
					}
			}
#endif

			return base.GetToolTipText(element, edgeRank);
		}

		public override bool MouseMove(Message message, Point pos)
		{
			//	Met en évidence la boîte selon la position de la souris.
			//	Si la souris est dans cette boîte, retourne true.
			if (this.isDragging)
			{
				Rectangle bounds = this.editor.NodeGridAlign (new Rectangle (pos-this.draggingOffset, this.Bounds.Size));
				this.SetBounds(bounds);
				this.editor.UpdateLinks();
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
			//	Le bouton de la souris est pressé.
			base.MouseDown (message, pos);

			this.initialPos = pos;

			if (this.hilitedElement == ActiveElement.NodeHeader && this.editor.NodeCount2 > 1)
			{
				this.isDragging = true;
				this.draggingOffset = pos-this.bounds.BottomLeft;
				this.editor.Invalidate();
				this.editor.LockObject(this);
			}

			if (this.hilitedElement == ActiveElement.NodeChangeWidth)
			{
				this.isChangeWidth = true;
				this.changeWidthPos = pos.X;
				this.changeWidthInitial = this.bounds.Width;
				this.editor.LockObject (this);
			}
		}

		public override void MouseUp(Message message, Point pos)
		{
			//	Le bouton de la souris est relâché.
			base.MouseUp (message, pos);

			if (pos == this.initialPos)
			{
				if (this.hilitedElement == ActiveElement.NodeHeader ||
					this.hilitedElement == ActiveElement.NodeEdgeName)
				{
					if (this.isDragging)
					{
						//?this.editor.UpdateAfterMoving (this);
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
				//?this.editor.UpdateAfterMoving (this);
				this.isDragging = false;
				this.editor.LockObject (null);
				this.editor.SetLocalDirty ();
				return;
			}

			if (this.isChangeWidth)
			{
				//?this.editor.UpdateAfterMoving (this);
				this.isChangeWidth = false;
				this.editor.LockObject (null);
				this.editor.SetLocalDirty ();
				return;
			}

			if (this.hilitedElement == ActiveElement.NodeOpenLeft)
			{
			}

			if (this.hilitedElement == ActiveElement.NodeOpenRight)
			{
			}

			if (this.hilitedElement == ActiveElement.NodeClose)
			{
				//?this.editor.CloseEdge2(this);
				this.editor.UpdateAfterAddOrRemoveEdge2(null);
			}

			if (this.hilitedElement == ActiveElement.NodeEdgeTitle)
			{
			}

			if (this.hilitedElement == ActiveElement.NodeComment)
			{
				this.AddComment();
			}

			if (this.hilitedElement == ActiveElement.NodeColor5)
			{
				this.BackgroundMainColor = MainColor.Yellow;
			}

			if (this.hilitedElement == ActiveElement.NodeColor6)
			{
				this.BackgroundMainColor = MainColor.Orange;
			}

			if (this.hilitedElement == ActiveElement.NodeColor3)
			{
				this.BackgroundMainColor = MainColor.Red;
			}

			if (this.hilitedElement == ActiveElement.NodeColor7)
			{
				this.BackgroundMainColor = MainColor.Lilac;
			}

			if (this.hilitedElement == ActiveElement.NodeColor8)
			{
				this.BackgroundMainColor = MainColor.Purple;
			}

			if (this.hilitedElement == ActiveElement.NodeColor1)
			{
				this.BackgroundMainColor = MainColor.Blue;
			}

			if (this.hilitedElement == ActiveElement.NodeColor2)
			{
				this.BackgroundMainColor = MainColor.Green;
			}

			if (this.hilitedElement == ActiveElement.NodeColor4)
			{
				this.BackgroundMainColor = MainColor.Grey;
			}
		}

		public override bool MouseDetect(Point pos, out ActiveElement element, out int edgeRank)
		{
			//	Détecte l'élément actif visé par la souris.
			if (base.MouseDetect (pos, out element, out edgeRank))
			{
				return true;
			}

			element = ActiveElement.None;
			edgeRank = -1;
			this.SetEdgesHilited(false);

			if (pos.IsZero)
			{
				//	Si l'une des connexion est dans l'état EdgeOpen*, il faut afficher
				//	aussi les petits cercles de gauche.
				if (this.IsEdgeReadyForOpen())
				{
					this.SetEdgesHilited(true);
				}
				return false;
			}

			if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectRoundButton (this.PositionChangeWidthButton, pos))
			{
				element = ActiveElement.NodeChangeWidth;
				return true;
			}

			//	Souris dans le bouton d'ouverture ?
			if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectRoundButton (this.PositionOpenLeftButton, pos))
			{
				element = ActiveElement.NodeOpenLeft;
				return true;
			}

			if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectRoundButton (this.PositionOpenRightButton, pos))
			{
				element = ActiveElement.NodeOpenRight;
				return true;
			}

			//	Souris dans le bouton de fermeture ?
			if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectRoundButton (this.PositionCloseButton, pos))
			{
				element = ActiveElement.NodeClose;
				return true;
			}

			//	Souris dans le bouton des commentaires ?
			if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectRoundButton(this.PositionCommentButton, pos))
			{
				element = ActiveElement.NodeComment;
				return true;
			}

			//	Souris dans le bouton des couleurs ?
			if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectSquareButton(this.PositionColorButton(0), pos))
			{
				element = ActiveElement.NodeColor5;
				return true;
			}

			if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectSquareButton(this.PositionColorButton(1), pos))
			{
				element = ActiveElement.NodeColor6;
				return true;
			}

			if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectSquareButton(this.PositionColorButton(2), pos))
			{
				element = ActiveElement.NodeColor3;
				return true;
			}

			if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectSquareButton(this.PositionColorButton(3), pos))
			{
				element = ActiveElement.NodeColor7;
				return true;
			}

			if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectSquareButton (this.PositionColorButton (4), pos))
			{
				element = ActiveElement.NodeColor8;
				return true;
			}

			if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectSquareButton (this.PositionColorButton (5), pos))
			{
				element = ActiveElement.NodeColor1;
				return true;
			}

			if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectSquareButton (this.PositionColorButton (6), pos))
			{
				element = ActiveElement.NodeColor2;
				return true;
			}

			if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectSquareButton (this.PositionColorButton (7), pos))
			{
				element = ActiveElement.NodeColor4;
				return true;
			}

			if (this.RectangleTitle.Contains (pos))
			{
				element = ActiveElement.NodeHeader;
				return true;
			}

			if (this.RectangleSubtitle.Contains (pos))
			{
				element = ActiveElement.NodeEdgeName;
				return true;
			}

			if (!this.bounds.Contains (pos))
			{
				return false;
			}

			element = ActiveElement.NodeInside;
			this.SetEdgesHilited(true);
			return true;
		}

		public override bool IsMousePossible(ActiveElement element, int edgeRank)
		{
			//	Indique si l'opération est possible.
			return true;
		}


		private void AddComment()
		{
			//	Ajoute un commentaire à la boîte.
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

				this.comment.EditComment ();  // édite tout de suite le texte du commentaire
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
			//	Héritage	->	Traitillé
			//	Interface	->	Trait plein avec o---
			Rectangle rect;

			bool dragging = (this.hilitedElement == ActiveElement.NodeHeader || this.isHilitedForEdgeChanging);

			//	Dessine l'ombre.
			rect = this.bounds;
			rect.Offset (ObjectEdge2.shadowOffset, -(ObjectEdge2.shadowOffset));
			this.DrawRoundShadow (graphics, rect, ObjectEdge2.roundFrameRadius, (int) ObjectEdge2.shadowOffset, 0.2);

			//	Construit le chemin du cadre.
			rect = this.bounds;
			rect.Deflate(1);
			Path path = this.PathRoundRectangle (rect, ObjectEdge2.roundFrameRadius);

			//	Dessine l'intérieur en blanc.
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(this.GetColor(1));

			//	Dessine l'intérieur en dégradé.
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
			Rectangle inside = new Rectangle (this.bounds.Left+1, this.bounds.Bottom+AbstractObject.footerHeight, this.bounds.Width-2, this.bounds.Height-AbstractObject.footerHeight-AbstractObject.headerHeight);
			graphics.AddFilledRectangle (inside);
			graphics.RenderSolid (this.GetColor (1));
			graphics.AddFilledRectangle (inside);
			Color ci1 = this.GetColorMain (dragging ? 0.2 : 0.1);
			Color ci2 = this.GetColorMain (0.0);
			this.RenderHorizontalGradient (graphics, inside, ci1, ci2);

			//	Ombre supérieure.
			Rectangle shadow = new Rectangle (this.bounds.Left+1, this.bounds.Top-AbstractObject.headerHeight-8, this.bounds.Width-2, 8);
			graphics.AddFilledRectangle (shadow);
			this.RenderVerticalGradient (graphics, shadow, Color.FromAlphaRgb (0.0, 0, 0, 0), Color.FromAlphaRgb (0.3, 0, 0, 0));

			graphics.AddLine (this.bounds.Left+2, this.bounds.Top-AbstractObject.headerHeight-0.5, this.bounds.Right-2, this.bounds.Top-AbstractObject.headerHeight-0.5);
			graphics.AddLine (this.bounds.Left+2, this.bounds.Bottom+AbstractObject.footerHeight+0.5, this.bounds.Right-2, this.bounds.Bottom+AbstractObject.footerHeight+0.5);
			graphics.RenderSolid (colorFrame);

			//	Dessine le titre.
			Color titleColor = dragging ? this.GetColor (1) : this.GetColor (0);

			rect = this.RectangleTitle;
			rect.Deflate (2, 2);
			this.title.LayoutSize = rect.Size;
			this.title.Paint (rect.BottomLeft, graphics, Rectangle.MaxValue, titleColor, GlyphPaintStyle.Normal);

			//	Dessine le sous-titre.
			Color subtitleColor = this.GetColor (0);

			rect = this.RectangleSubtitle;
			this.subtitle.LayoutSize = rect.Size;
			this.subtitle.Paint (rect.BottomLeft, graphics, Rectangle.MaxValue, subtitleColor, GlyphPaintStyle.Normal);

			//	Dessine le cadre en noir.
			graphics.Rasterizer.AddOutline (path, 2);
			graphics.RenderSolid (colorFrame);

			//	Dessine le bouton d'ouverture.
			if (this.hilitedElement == ActiveElement.NodeOpenLeft)
			{
				this.DrawRoundButton (graphics, this.PositionOpenLeftButton, AbstractObject.buttonRadius, GlyphShape.ArrowLeft, true, false, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawRoundButton (graphics, this.PositionOpenLeftButton, AbstractObject.buttonRadius, GlyphShape.ArrowLeft, false, false, true);
			}

			if (this.hilitedElement == ActiveElement.NodeOpenRight)
			{
				this.DrawRoundButton (graphics, this.PositionOpenRightButton, AbstractObject.buttonRadius, GlyphShape.ArrowRight, true, false, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawRoundButton (graphics, this.PositionOpenRightButton, AbstractObject.buttonRadius, GlyphShape.ArrowRight, false, false, true);
			}

			//	Dessine le bouton de fermeture.
			if (this.hilitedElement == ActiveElement.NodeClose)
			{
				this.DrawRoundButton (graphics, this.PositionCloseButton, AbstractObject.buttonRadius, GlyphShape.Close, true, false, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawRoundButton (graphics, this.PositionCloseButton, AbstractObject.buttonRadius, GlyphShape.Close, false, false, true);
			}

			//	Dessine le bouton des commentaires.
			if (this.hilitedElement == ActiveElement.NodeComment)
			{
				this.DrawRoundButton (graphics, this.PositionCommentButton, AbstractObject.buttonRadius, "C", true, false);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawRoundButton (graphics, this.PositionCommentButton, AbstractObject.buttonRadius, "C", false, false);
			}

			//	Dessine le bouton des couleurs.
			if (this.hilitedElement == ActiveElement.NodeColor5)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (0), MainColor.Yellow, this.boxColor == MainColor.Yellow, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (0), MainColor.Yellow, this.boxColor == MainColor.Yellow, false);
			}

			if (this.hilitedElement == ActiveElement.NodeColor6)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (1), MainColor.Orange, this.boxColor == MainColor.Orange, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (1), MainColor.Orange, this.boxColor == MainColor.Orange, false);
			}

			if (this.hilitedElement == ActiveElement.NodeColor3)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (2), MainColor.Red, this.boxColor == MainColor.Red, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (2), MainColor.Red, this.boxColor == MainColor.Red, false);
			}

			if (this.hilitedElement == ActiveElement.NodeColor7)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (3), MainColor.Lilac, this.boxColor == MainColor.Lilac, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (3), MainColor.Lilac, this.boxColor == MainColor.Lilac, false);
			}

			if (this.hilitedElement == ActiveElement.NodeColor8)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (4), MainColor.Purple, this.boxColor == MainColor.Purple, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (4), MainColor.Purple, this.boxColor == MainColor.Purple, false);
			}
			
			if (this.hilitedElement == ActiveElement.NodeColor1)
			{
				this.DrawSquareButton(graphics, this.PositionColorButton(5), MainColor.Blue, this.boxColor == MainColor.Blue, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton(graphics, this.PositionColorButton(5), MainColor.Blue, this.boxColor == MainColor.Blue, false);
			}

			if (this.hilitedElement == ActiveElement.NodeColor2)
			{
				this.DrawSquareButton(graphics, this.PositionColorButton(6), MainColor.Green, this.boxColor == MainColor.Green, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton(graphics, this.PositionColorButton(6), MainColor.Green, this.boxColor == MainColor.Green, false);
			}

			if (this.hilitedElement == ActiveElement.NodeColor4)
			{
				this.DrawSquareButton(graphics, this.PositionColorButton(7), MainColor.Grey, this.boxColor == MainColor.Grey, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton(graphics, this.PositionColorButton(7), MainColor.Grey, this.boxColor == MainColor.Grey, false);
			}

			//	Dessine le bouton pour changer la largeur.
			if (this.hilitedElement == ActiveElement.NodeChangeWidth)
			{
				this.DrawRoundButton (graphics, this.PositionChangeWidthButton, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, true, false);
			}
			if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.hilitedElement == ActiveElement.NodeHeader && !this.isDragging)
			{
				this.DrawRoundButton (graphics, this.PositionChangeWidthButton, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, false, false);
			}

			//	Dessine les connexions.
			this.DrawLinks (graphics);
		}

		private bool IsHeaderHilite
		{
			//	Indique si la souris est dans l'en-tête.
			get
			{
				if (this.editor.CurrentModifyMode == Editor.ModifyMode.Locked)
				{
					return false;
				}

				return (this.hilitedElement == ActiveElement.NodeHeader ||
						this.hilitedElement == ActiveElement.NodeComment ||
						this.hilitedElement == ActiveElement.NodeInfo ||
						this.hilitedElement == ActiveElement.NodeColor1 ||
						this.hilitedElement == ActiveElement.NodeColor2 ||
						this.hilitedElement == ActiveElement.NodeColor3 ||
						this.hilitedElement == ActiveElement.NodeColor4 ||
						this.hilitedElement == ActiveElement.NodeColor5 ||
						this.hilitedElement == ActiveElement.NodeColor6 ||
						this.hilitedElement == ActiveElement.NodeColor7 ||
						this.hilitedElement == ActiveElement.NodeColor8 ||
						this.hilitedElement == ActiveElement.NodeExtend ||
						this.hilitedElement == ActiveElement.NodeOpenLeft ||
						this.hilitedElement == ActiveElement.NodeOpenRight ||
						this.hilitedElement == ActiveElement.NodeClose);
			}
		}

		public override void DrawForeground(Graphics graphics)
		{
			//	Dessine le dessus de l'objet.
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

		private Point PositionCloseButton
		{
			//	Retourne la position du bouton pour fermer.
			get
			{
				return new Point (this.bounds.Right-AbstractObject.buttonRadius-6, this.bounds.Top-AbstractObject.headerHeight/2);
			}
		}

		private Point PositionOpenLeftButton
		{
			//	Retourne la position du bouton pour ouvrir.
			get
			{
				return new Point (this.bounds.Left, this.bounds.Center.Y);
			}
		}

		private Point PositionOpenRightButton
		{
			//	Retourne la position du bouton pour ouvrir.
			get
			{
				return new Point (this.bounds.Right, this.bounds.Center.Y);
			}
		}

		private Point PositionColorButton(int rank)
		{
			//	Retourne la position du bouton pour choisir la couleur.
			return new Point (this.bounds.Left-2+(AbstractObject.buttonSquare+0.5)*(rank+1)*2, this.bounds.Bottom+4+AbstractObject.buttonSquare);
		}

		private Point PositionChangeWidthButton
		{
			//	Retourne la position du bouton pour changer la largeur.
			get
			{
				return new Point (this.bounds.Right-1, this.bounds.Bottom+AbstractObject.footerHeight/2+1);
			}
		}

		private string GetGroupTooltip(int rank)
		{
			//	Retourne le tooltip à afficher pour un groupe.
			return null;  // pas de tooltip
		}


		private void UpdateTitle()
		{
			//	Met à jour le titre du noeud.
			this.Title = this.Entity.Name.ToString ();
		}

		private void UpdateSubtitle()
		{
			//	Met à jour le sous-titre du noeud.
			this.Subtitle = this.Entity.Description.ToString ();
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
			//	Sérialise toutes les informations de la boîte et de ses champs.
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

			if (this.comment != null && this.comment.IsVisible)  // commentaire associé ?
			{
				this.comment.WriteXml(writer);
			}
			
			if (this.info != null && this.info.IsVisible)  // informations associées ?
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
			//	Ajuste le contenu de la boîte après sa désérialisation.
		}
		#endregion


		public static readonly Size				frameSize = new Size (200, 100);
		public static readonly double			roundFrameRadius = 12;
		private static readonly double			shadowOffset = 6;

		private ObjectComment					comment;
		private Rectangle						bounds;
		private string							titleString;
		private TextLayout						title;
		private string							subtitleString;
		private TextLayout						subtitle;

		private Point							initialPos;

		private bool							isDragging;
		private Point							draggingOffset;

		private bool							isChangeWidth;
		private double							changeWidthPos;
		private double							changeWidthInitial;

		private bool							isHilitedForEdgeChanging;

		private ActiveElement					editingElement;
		private AbstractTextField				editingTextField;
	}
}
