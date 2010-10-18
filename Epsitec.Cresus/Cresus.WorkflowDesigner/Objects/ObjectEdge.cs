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

			this.SetBounds (new Rectangle (Point.Zero, ObjectEdge.frameSize));

			//	Crée la liaison unique.
			this.CreateInitialLinks ();
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

		public bool IsExtended
		{
			//	Etat de la boîte (compact ou étendu).
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
			this.UpdateButtonsGeometry ();
		}


		public override void CreateInitialLinks()
		{
			this.objectLinks.Clear ();
	
			var link = new ObjectLink (this.editor, this.Entity);
			link.SrcObject = this;
			link.DstObject = this.editor.SearchInitialObject (this.Entity.NextNode);  // null si n'existe pas (et donc moignon o--->)

			if (link.DstObject == null)
			{
				link.SetStumpAngle (0);
			}

			this.objectLinks.Add (link);
		}


		public override void SetBoundsAtEnd(Point start, Point end)
		{
			double d = (ObjectEdge.frameSize.Width+ObjectEdge.frameSize.Height)/4;  // approximation

			Point center = Point.Move (end, start, -d);
			Rectangle rect = new Rectangle (center.X-ObjectEdge.frameSize.Width/2, center.Y-ObjectEdge.frameSize.Height/2, ObjectEdge.frameSize.Width, ObjectEdge.frameSize.Height);

			this.SetBounds (rect);
		}


		public override void RemoveEntityLink(LinkableObject dst)
		{
			System.Diagnostics.Debug.Assert (dst.AbstractEntity is WorkflowNodeEntity);
			System.Diagnostics.Debug.Assert (this.Entity.NextNode == dst.AbstractEntity as WorkflowNodeEntity);

			this.Entity.NextNode = null;
		}

		public override void AddEntityLink(LinkableObject dst)
		{
			System.Diagnostics.Debug.Assert (dst.AbstractEntity is WorkflowNodeEntity);

			this.Entity.NextNode = dst.AbstractEntity as WorkflowNodeEntity;
		}


		public override Vector GetLinkVector(double angle, bool isDst)
		{
			double margin = ObjectEdge.roundFrameRadius * (isDst ? 2.5 : 1.5);

			Point c = this.bounds.Center;
			Point p = Transform.RotatePointDeg (c, angle, new Point (c.X+this.bounds.Width+this.bounds.Height, c.Y));

			Segment s = this.GetIntersect (c, p);
			if (s != null)
			{
				if (s.anchor == LinkAnchor.Left || s.anchor == LinkAnchor.Right)
				{
					s.intersection.Y = System.Math.Max (s.intersection.Y, s.p1.Y+margin);
					s.intersection.Y = System.Math.Min (s.intersection.Y, s.p2.Y-margin);
				}

				if (s.anchor == LinkAnchor.Bottom || s.anchor == LinkAnchor.Top)
				{
					s.intersection.X = System.Math.Max (s.intersection.X, s.p1.X+margin);
					s.intersection.X = System.Math.Min (s.intersection.X, s.p2.X-margin);
				}

				Point i = new Point (System.Math.Floor (s.intersection.X), System.Math.Floor (s.intersection.Y));

				switch (s.anchor)
				{
					case LinkAnchor.Left:
						return new Vector (i, new Size (-1, 0));

					case LinkAnchor.Right:
						return new Vector (i, new Size (1, 0));

					case LinkAnchor.Bottom:
						return new Vector (i, new Size (0, -1));

					case LinkAnchor.Top:
						return new Vector (i, new Size (0, 1));
				}
			}

			return Vector.Zero;
		}

		public override Point GetLinkStumpPos(double angle)
		{
			Point c = this.bounds.Center;
			Point p = Transform.RotatePointDeg (c, angle, new Point (c.X+this.bounds.Width+this.bounds.Height, c.Y));

			Segment s = this.GetIntersect (c, p);
			if (s != null)
			{
				return Point.Move (s.intersection, c, -AbstractObject.lengthStumpLink);
			}

			return this.bounds.Center;
		}

		private Segment GetIntersect(Point c, Point p)
		{
			Point i;

			i = Geometry.IsIntersect (c, p, this.bounds.BottomRight, this.bounds.TopRight);
			if (!i.IsZero)
			{
				return new Segment (i, this.bounds.BottomRight, this.bounds.TopRight, LinkAnchor.Right);
			}

			i = Geometry.IsIntersect (c, p, this.bounds.BottomLeft, this.bounds.BottomRight);
			if (!i.IsZero)
			{
				return new Segment (i, this.bounds.BottomLeft, this.bounds.BottomRight, LinkAnchor.Bottom);
			}

			i = Geometry.IsIntersect (c, p, this.bounds.BottomLeft,  this.bounds.TopLeft);
			if (!i.IsZero)
			{
				return new Segment (i, this.bounds.BottomLeft, this.bounds.TopLeft, LinkAnchor.Left);
			}

			i = Geometry.IsIntersect (c, p, this.bounds.TopLeft, this.bounds.TopRight);
			if (!i.IsZero)
			{
				return new Segment (i, this.bounds.TopLeft, this.bounds.TopRight, LinkAnchor.Top);
			}

			return null;
		}

		private class Segment
		{
			public Segment(Point intersection, Point p1, Point p2, LinkAnchor anchor)
			{
				this.intersection = intersection;
				this.p1           = p1;
				this.p2           = p2;
				this.anchor       = anchor;
			}

			public Point		intersection;
			public Point		p1;
			public Point		p2;
			public LinkAnchor	anchor;
		}


		public override void AcceptEdition()
		{
			if (this.editingElement == ActiveElement.EdgeHeader)
			{
				this.Entity.Name = this.editingTextField.Text;
				this.UpdateTitle ();
			}

			if (this.editingElement == ActiveElement.EdgeEditDescription)
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

			if (element == ActiveElement.EdgeEditDescription)
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

			this.editingTextField.Parent = this.editor;
			this.editingTextField.SetManualBounds (rect);
			this.editingTextField.Text = text;
			this.editingTextField.TabIndex = 1;
			this.editingTextField.SelectAll ();
			this.editingTextField.Focus ();

			this.editor.EditingObject = this;
			this.hilitedElement = ActiveElement.None;
			this.UpdateButtonsState ();
		}

		private void StopEdition()
		{
			this.editor.Children.Remove (this.editingTextField);
			this.editingTextField = null;

			this.editor.EditingObject = null;
			this.UpdateButtonsState ();
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
			//	Met en évidence la boîte selon la position de la souris.
			//	Si la souris est dans cette boîte, retourne true.
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
			//	Le bouton de la souris est pressé.
			base.MouseDown (message, pos);

			this.initialPos = pos;

			if (this.hilitedElement == ActiveElement.EdgeHeader && this.editor.LinkableObjectsCount > 1)
			{
				this.isDragging = true;
				this.UpdateButtonsState ();
				this.draggingOffset = pos-this.bounds.BottomLeft;
				this.editor.Invalidate();
				this.editor.LockObject(this);
			}

			if (this.hilitedElement == ActiveElement.EdgeChangeWidth)
			{
				this.isChangeWidth = true;
				this.UpdateButtonsState ();
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
				if (this.hilitedElement == ActiveElement.EdgeHeader ||
					this.hilitedElement == ActiveElement.EdgeEditDescription)
				{
					if (this.isDragging)
					{
						this.editor.UpdateAfterGeometryChanged (this);
						this.isDragging = false;
						this.UpdateButtonsState ();
						this.editor.LockObject (null);
						this.editor.SetLocalDirty ();
					}

					this.StartEdition (this.hilitedElement);
					return;
				}
			}

			if (this.isDragging)
			{
				this.editor.UpdateAfterGeometryChanged (this);
				this.isDragging = false;
				this.UpdateButtonsState ();
				this.editor.LockObject (null);
				this.editor.SetLocalDirty ();
				return;
			}

			if (this.isChangeWidth)
			{
				this.editor.UpdateAfterGeometryChanged (this);
				this.isChangeWidth = false;
				this.UpdateButtonsState ();
				this.editor.LockObject (null);
				this.editor.SetLocalDirty ();
				return;
			}

			if (this.hilitedElement == ActiveElement.EdgeExtend)
			{
				this.IsExtended = !this.IsExtended;
			}

			if (this.hilitedElement == ActiveElement.EdgeClose)
			{
				this.editor.CloseObject (this);
				this.editor.UpdateAfterGeometryChanged (null);
			}

			if (this.hilitedElement == ActiveElement.EdgeComment)
			{
				this.AddComment();
			}

			if (this.hilitedElement == ActiveElement.EdgeColor1)
			{
				this.BackgroundColorItem = ColorItem.Yellow;
			}

			if (this.hilitedElement == ActiveElement.EdgeColor2)
			{
				this.BackgroundColorItem = ColorItem.Orange;
			}

			if (this.hilitedElement == ActiveElement.EdgeColor3)
			{
				this.BackgroundColorItem = ColorItem.Red;
			}

			if (this.hilitedElement == ActiveElement.EdgeColor4)
			{
				this.BackgroundColorItem = ColorItem.Lilac;
			}

			if (this.hilitedElement == ActiveElement.EdgeColor5)
			{
				this.BackgroundColorItem = ColorItem.Purple;
			}

			if (this.hilitedElement == ActiveElement.EdgeColor6)
			{
				this.BackgroundColorItem = ColorItem.Blue;
			}

			if (this.hilitedElement == ActiveElement.EdgeColor7)
			{
				this.BackgroundColorItem = ColorItem.Green;
			}

			if (this.hilitedElement == ActiveElement.EdgeColor8)
			{
				this.BackgroundColorItem = ColorItem.Grey;
			}
		}

		public override ActiveElement MouseDetectBackground(Point pos)
		{
			//	Détecte l'élément actif visé par la souris.
			if (this.RectangleTitle.Contains (pos))
			{
				return ActiveElement.EdgeHeader;
			}

			if (this.RectangleSubtitle.Contains (pos))
			{
				return ActiveElement.EdgeEditDescription;
			}

			if (this.bounds.Contains (pos))
			{
				return ActiveElement.EdgeInside;
			}

			return ActiveElement.None;
		}

		public override ActiveElement MouseDetectForeground(Point pos)
		{
			//	Détecte l'élément actif visé par la souris.
			if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked)
			{
				return this.DetectButtons (pos);
			}

			return ActiveElement.None;
		}

		public override bool IsMousePossible(ActiveElement element)
		{
			//	Indique si l'opération est possible.
			return true;
		}


		public override string DebugInformations
		{
			get
			{
				return string.Format ("Edge: {0} {1}", this.Entity.Name.ToString (), this.DebugInformationsObjectLinks);
			}
		}

		public override string DebugInformationsBase
		{
			get
			{
				return string.Format ("{0}", this.Entity.Name.ToString ());
			}
		}

	
		private void AddComment()
		{
			//	Ajoute un commentaire à la boîte.
			if (this.comment == null)
			{
				this.comment = new ObjectComment (this.editor, this.Entity);
				this.comment.AttachObject = this;

				Rectangle rect = this.bounds;
				rect.Top = rect.Top+50;
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

			bool dragging = (this.hilitedElement == ActiveElement.EdgeHeader || this.hilitedElement == ActiveElement.EdgeEditDescription || this.isHilitedForLinkChanging);

			//	Dessine l'ombre.
			rect = this.bounds;
			rect.Offset (ObjectEdge.shadowOffset, -(ObjectEdge.shadowOffset));
			this.DrawRoundShadow (graphics, rect, ObjectEdge.roundFrameRadius, (int) ObjectEdge.shadowOffset, 0.2);

			//	Construit le chemin du cadre.
			rect = this.bounds;
			rect.Deflate(1);
			Path path = this.PathRoundRectangle (rect, ObjectEdge.roundFrameRadius);

			//	Dessine l'intérieur en blanc.
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid (this.colorFactory.GetColor (1));

			//	Dessine l'intérieur en dégradé.
			graphics.Rasterizer.AddSurface(path);
			Color c1 = this.colorFactory.GetColorMain (dragging ? 0.8 : 0.4);
			Color c2 = this.colorFactory.GetColorMain (dragging ? 0.4 : 0.1);
			this.RenderHorizontalGradient(graphics, this.bounds, c1, c2);

			Color colorLine = this.colorFactory.GetColor (0.9);
			if (dragging)
			{
				colorLine = this.colorFactory.GetColorMain (0.3);
			}

			Color colorFrame = dragging ? this.colorFactory.GetColorMain () : this.colorFactory.GetColor (0);

			//	Dessine en blanc la zone pour les champs.
			if (this.isExtended)
			{
				Rectangle inside = new Rectangle (this.bounds.Left+1, this.bounds.Bottom+AbstractObject.footerHeight, this.bounds.Width-2, this.bounds.Height-AbstractObject.footerHeight-AbstractObject.headerHeight);
				graphics.AddFilledRectangle (inside);
				graphics.RenderSolid (this.colorFactory.GetColor (1));
				graphics.AddFilledRectangle (inside);
				Color ci1 = this.colorFactory.GetColorMain (dragging ? 0.2 : 0.1);
				Color ci2 = this.colorFactory.GetColorMain (0.0);
				this.RenderHorizontalGradient (graphics, inside, ci1, ci2);

				//	Ombre supérieure.
				Rectangle shadow = new Rectangle (this.bounds.Left+1, this.bounds.Top-AbstractObject.headerHeight-8, this.bounds.Width-2, 8);
				graphics.AddFilledRectangle (shadow);
				this.RenderVerticalGradient (graphics, shadow, Color.FromAlphaRgb (0.0, 0, 0, 0), Color.FromAlphaRgb (0.3, 0, 0, 0));

				graphics.AddLine (this.bounds.Left+2, this.bounds.Top-AbstractObject.headerHeight-0.5, this.bounds.Right-2, this.bounds.Top-AbstractObject.headerHeight-0.5);
				graphics.AddLine (this.bounds.Left+2, this.bounds.Bottom+AbstractObject.footerHeight+0.5, this.bounds.Right-2, this.bounds.Bottom+AbstractObject.footerHeight+0.5);
				graphics.RenderSolid (colorFrame);
			}

			//	Dessine le titre.
			Color titleColor = dragging ? this.colorFactory.GetColor (1) : this.colorFactory.GetColor (0);

			rect = this.RectangleTitle;
			rect.Deflate (2, 2);
			this.title.LayoutSize = rect.Size;
			this.title.Paint (rect.BottomLeft, graphics, Rectangle.MaxValue, titleColor, GlyphPaintStyle.Normal);

			//	Dessine le sous-titre.
			if (this.isExtended)
			{
				Color subtitleColor = this.colorFactory.GetColor (0);

				rect = this.RectangleSubtitle;
				this.subtitle.LayoutSize = rect.Size;
				this.subtitle.Paint (rect.BottomLeft, graphics, Rectangle.MaxValue, subtitleColor, GlyphPaintStyle.Normal);
			}

			//	Dessine le cadre en noir.
			graphics.Rasterizer.AddOutline (path, 2);
			graphics.RenderSolid (colorFrame);
		}

		public override void DrawForeground(Graphics graphics)
		{
			//	Dessine le bouton compact/étendu.
			this.DrawButtons (graphics);
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

				return (this.hilitedElement == ActiveElement.EdgeHeader ||
						this.hilitedElement == ActiveElement.EdgeEditDescription ||
						this.hilitedElement == ActiveElement.EdgeInside ||
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
						this.hilitedElement == ActiveElement.EdgeClose ||
						this.hilitedElement == ActiveElement.EdgeChangeWidth);
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
				return new Point (this.bounds.Left+ActiveButton.buttonRadius+6, this.bounds.Top-AbstractObject.headerHeight/2);
			}
		}

		private Point PositionExtendButton
		{
			//	Retourne la position du bouton pour étendre.
			get
			{
				return new Point (this.bounds.Right-ActiveButton.buttonRadius*3-8, this.bounds.Top-AbstractObject.headerHeight/2);
			}
		}

		private Point PositionCloseButton
		{
			//	Retourne la position du bouton pour fermer.
			get
			{
				return new Point (this.bounds.Right-ActiveButton.buttonRadius-6, this.bounds.Top-AbstractObject.headerHeight/2);
			}
		}

		private Point PositionColorButton(int rank)
		{
			//	Retourne la position du bouton pour choisir la couleur.
			if (this.isExtended)
			{
				return new Point (this.bounds.Left-2+(ActiveButton.buttonSquare+0.5)*(rank+1)*2, this.bounds.Bottom+4+ActiveButton.buttonSquare);
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


		protected override void CreateButtons()
		{
			this.buttons.Add (new ActiveButton (ActiveElement.EdgeClose,       this.colorFactory, GlyphShape.Close,          this.UpdateButtonGeometryClose,       this.UpdateButtonStateClose));
			this.buttons.Add (new ActiveButton (ActiveElement.EdgeComment,     this.colorFactory, "C",                       this.UpdateButtonGeometryComment,     this.UpdateButtonStateComment));
			this.buttons.Add (new ActiveButton (ActiveElement.EdgeExtend,      this.colorFactory, GlyphShape.ArrowUp,        this.UpdateButtonGeometryExtend,      this.UpdateButtonStateExtend));
			this.buttons.Add (new ActiveButton (ActiveElement.EdgeChangeWidth, this.colorFactory, GlyphShape.HorizontalMove, this.UpdateButtonGeometryChangeWidth, this.UpdateButtonStateChangeWidth));

			this.buttons.Add (new ActiveButton (ActiveElement.EdgeColor1, this.colorFactory, ColorItem.Yellow, this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.EdgeColor2, this.colorFactory, ColorItem.Orange, this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.EdgeColor3, this.colorFactory, ColorItem.Red,    this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.EdgeColor4, this.colorFactory, ColorItem.Lilac,  this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.EdgeColor5, this.colorFactory, ColorItem.Purple, this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.EdgeColor6, this.colorFactory, ColorItem.Blue,   this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.EdgeColor7, this.colorFactory, ColorItem.Green,  this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.EdgeColor8, this.colorFactory, ColorItem.Grey,   this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
		}

		private void UpdateButtonGeometryClose(ActiveButton button)
		{
			button.Center = this.PositionCloseButton;
		}

		private void UpdateButtonGeometryComment(ActiveButton button)
		{
			button.Center = this.PositionCommentButton;
		}

		private void UpdateButtonGeometryExtend(ActiveButton button)
		{
			button.Center = this.PositionExtendButton;
		}

		private void UpdateButtonGeometryChangeWidth(ActiveButton button)
		{
			button.Center = this.PositionChangeWidthButton;
		}

		private void UpdateButtonGeometryColor(ActiveButton button)
		{
			int rank = button.Element - ActiveElement.EdgeColor1;

			button.Center = this.PositionColorButton (rank);
		}

		private void UpdateButtonStateClose(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Visible = this.IsHeaderHilite && !this.IsDragging;
		}

		private void UpdateButtonStateComment(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Visible = this.IsHeaderHilite && !this.IsDragging;
		}

		private void UpdateButtonStateExtend(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.Glyph = this.isExtended ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;
			button.State.Visible = this.IsHeaderHilite && !this.IsDragging;
		}

		private void UpdateButtonStateChangeWidth(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Visible = button.State.Hilited || (this.IsHeaderHilite && !this.IsDragging && this.isExtended);
		}

		private void UpdateButtonStateColor(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Selected = this.colorFactory.ColorItem == button.ColorItem;
			button.State.Visible = this.IsHeaderHilite && !this.IsDragging && this.isExtended;
		}

		private bool IsDragging
		{
			get
			{
				return this.isDragging || this.isChangeWidth || this.editor.IsEditing;
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
							this.boxColor = (ColorItem) System.Enum.Parse(typeof(ColorItem), element);
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


		private static readonly Size			frameSize = new Size (200, 100);
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

		private ActiveElement					editingElement;
		private AbstractTextField				editingTextField;
	}
}
