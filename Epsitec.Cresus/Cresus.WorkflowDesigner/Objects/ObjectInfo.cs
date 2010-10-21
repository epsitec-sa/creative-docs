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
	public class ObjectInfo : AbstractObject
	{
		private enum AttachMode
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


		public ObjectInfo(Editor editor, AbstractEntity entity)
			: base (editor, entity)
		{
			this.isVisible = true;
			this.colorFactory.ColorItem = ColorItem.Blue;

			this.textLayouts = new List<TextLayout> ();
		}


		public ObjectNode AttachObject
		{
			get
			{
				return this.attachObject;
			}
			set
			{
				this.attachObject = value;
			}
		}

		public void UpdateAfterAttachChanged()
		{
			this.UpdateTextLayouts ();
			this.UpdateHeight ();
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
			this.UpdateButtonsGeometry ();
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
			this.UpdateButtonsGeometry ();
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


		protected override string GetToolTipText(ActiveElement element)
		{
			//	Retourne le texte pour le tooltip.
			if (this.isDraggingMove || this.isDraggingWidth)
			{
				return null;  // pas de tooltip
			}

			switch (element)
			{
				case ActiveElement.InfoClose:
					return "Ferme les informations";

				case ActiveElement.InfoWidth:
					return "Change la largeur des informations";
			}

			return base.GetToolTipText (element);
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
			else
			{
				return base.MouseMove(message, pos);
			}
		}

		public override void MouseDown(Message message, Point pos)
		{
			//	Le bouton de la souris est pressé.
			this.initialPos = pos;

			if (this.hilitedElement == ActiveElement.InfoMove)
			{
				this.isDraggingMove = true;
				this.UpdateButtonsState ();
				this.draggingPos = pos;
				this.editor.LockObject(this);
			}

			if (this.hilitedElement == ActiveElement.InfoWidth)
			{
				this.isDraggingWidth = true;
				this.UpdateButtonsState ();
				this.editor.LockObject (this);
			}
		}

		public override void MouseUp(Message message, Point pos)
		{
			//	Le bouton de la souris est relâché.
			if (this.isDraggingMove)
			{
				this.isDraggingMove = false;
				this.UpdateButtonsState ();
				this.editor.LockObject (null);
				this.editor.UpdateAfterCommentChanged();
				this.editor.SetLocalDirty ();
			}
			else if (this.isDraggingWidth)
			{
				this.isDraggingWidth = false;
				this.UpdateButtonsState ();
				this.editor.LockObject (null);
				this.editor.UpdateAfterCommentChanged();
				this.editor.SetLocalDirty ();
			}
			else
			{
				if (this.hilitedElement == ActiveElement.InfoClose)
				{
					this.IsVisible = false;

					if (this.attachObject != null)
					{
						this.attachObject.Info = null;
						this.editor.RemoveInfo (this);
					}
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

			//	Souris dans la boîte ?
			if (this.bounds.Contains(pos))
			{
				if (pos.Y <= this.bounds.Bottom+ObjectInfo.footerHeight)
				{
					return ActiveElement.InfoMove;
				}

				if (pos.Y >= this.bounds.Top-ObjectInfo.footerHeight)
				{
					return ActiveElement.InfoMove;
				}

				return ActiveElement.InfoInside;
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

			if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked)
			{
				return this.DetectButtons (pos);
			}

			return ActiveElement.None;
		}


		public void UpdateHeight()
		{
			//	Adapte la hauteur de l'information en fonction de sa largeur et du contenu.
			Rectangle rect = this.bounds;

			double h = ObjectInfo.headerHeight + ObjectInfo.lineMargin*2 + ObjectInfo.footerHeight;

			int lineCount = 0;
			if (this.attachObject != null)
			{
				lineCount = this.attachObject.Entity.Edges.Count;
			}
			h += System.Math.Max (lineCount, 1) * ObjectInfo.lineHeight;

			var a = this.GetAttachMode();
			if (a == AttachMode.Bottom || a == AttachMode.None)
			{
				this.bounds.Height = h;
			}
			else
			{
				this.bounds.Bottom = this.bounds.Top-h;
			}

			this.UpdateButtonsGeometry ();
		}


		public override void DrawBackground(Graphics graphics)
		{
			//	Dessine le fond de l'objet.
			if (!this.isVisible)
			{
				return;
			}

			Rectangle rect;
			Color c1, c2;

			this.colorFactory.ColorItem = this.ParentColorFartory.ColorItem;

			bool dragging = (this.hilitedElement == ActiveElement.InfoInside);

			Color colorFrame = dragging ? this.colorFactory.GetColorMain () : this.colorFactory.GetColor (0);

			Color colorLine = this.colorFactory.GetColor (0.9);
			if (dragging)
			{
				colorLine = this.colorFactory.GetColorMain (0.3);
			}

			//	Dessine la liaison.
			AttachMode mode = this.GetAttachMode ();
			Point himself = this.GetAttachHimself (mode);
			Point other   = this.GetAttachOther (mode);

			if (this.attachObject is LinkableObject)
			{
				other = Point.Move (other, himself, ObjectEdge.frameSize.Height/2);
			}

			himself = Point.Move (himself, other, -ObjectInfo.headerHeight);

			himself = Point.GridAlign (himself, 0.5, 1);
			other   = Point.GridAlign (other,   0.5, 1);

			graphics.AddLine (himself, other);
			graphics.RenderSolid (colorFrame);

			//	Dessine l'ombre.
			rect = this.bounds;
			rect.Offset (ObjectInfo.shadowOffset, -(ObjectInfo.shadowOffset));
			this.DrawRoundShadow (graphics, rect, ObjectInfo.headerHeight, (int) ObjectInfo.shadowOffset, 0.2);

			//	Construit le chemin du cadre.
			rect = this.bounds;
			rect.Deflate (1);
			Path path = this.PathRoundRectangle (rect, ObjectInfo.headerHeight);

			//	Dessine l'intérieur en blanc.
			graphics.Rasterizer.AddSurface (path);
			graphics.RenderSolid (this.colorFactory.GetColor (1));

			//	Dessine l'intérieur en dégradé.
			graphics.Rasterizer.AddSurface (path);
			c1 = this.colorFactory.GetColorMain (dragging ? 0.8 : 0.4);
			c2 = this.colorFactory.GetColorMain (dragging ? 0.4 : 0.1);
			this.RenderHorizontalGradient (graphics, this.bounds, c1, c2);

			//	Dessine la zone pour les lignes.
			Rectangle inside = new Rectangle (this.bounds.Left+1, this.bounds.Bottom+ObjectInfo.footerHeight, this.bounds.Width-2, this.bounds.Height-ObjectInfo.footerHeight-ObjectInfo.headerHeight);
			graphics.AddFilledRectangle (inside);
			graphics.RenderSolid (this.colorFactory.GetColor (1));
			graphics.AddFilledRectangle (inside);
			Color ci1 = this.colorFactory.GetColorMain (dragging ? 0.2 : 0.1);
			Color ci2 = this.colorFactory.GetColorMain (0.0);
			this.RenderHorizontalGradient (graphics, inside, ci1, ci2);

			//	Ombre supérieure.
			Rectangle shadow = new Rectangle (this.bounds.Left+1, this.bounds.Top-ObjectInfo.headerHeight-8, this.bounds.Width-2, 8);
			graphics.AddFilledRectangle (shadow);
			this.RenderVerticalGradient (graphics, shadow, Color.FromAlphaRgb (0.0, 0, 0, 0), Color.FromAlphaRgb (0.3, 0, 0, 0));

			graphics.AddLine (this.bounds.Left+2, this.bounds.Top-ObjectInfo.headerHeight-0.5, this.bounds.Right-2, this.bounds.Top-ObjectInfo.headerHeight-0.5);
			graphics.AddLine (this.bounds.Left+2, this.bounds.Bottom+ObjectInfo.footerHeight+0.5, this.bounds.Right-2, this.bounds.Bottom+ObjectInfo.footerHeight+0.5);
			graphics.RenderSolid (colorFrame);

			//	Dessine les lignes.
			if (this.attachObject != null)
			{
				for (int i = 0; i < this.attachObject.Entity.Edges.Count; i++)
				{
					rect = this.RectangleLine (i);

					graphics.AddLine (rect.Left+1, rect.Bottom-0.5, rect.Right-1, rect.Bottom-0.5);
					graphics.RenderSolid (colorLine);

					rect.Deflate (10, 0);
					this.textLayouts[i].LayoutSize = rect.Size;
					this.textLayouts[i].Paint (rect.BottomLeft, graphics, Rectangle.MaxValue, this.colorFactory.GetColor (0), GlyphPaintStyle.Normal);
				}
			}

			//	Dessine le cadre en noir.
			graphics.Rasterizer.AddOutline (path, 2);
			graphics.RenderSolid (colorFrame);
		}

		public override void DrawForeground(Graphics graphics)
		{
			//	Dessine le dessus de l'objet.
			if (this.isVisible)
			{
				this.DrawButtons (graphics);
			}
		}

		private bool IsHeaderHilite
		{
			//	Indique si la souris est dans l'en-tête.
			get
			{
				return (this.hilitedElement == ActiveElement.InfoInside ||
						this.hilitedElement == ActiveElement.InfoMove ||
						this.hilitedElement == ActiveElement.InfoClose ||
						this.hilitedElement == ActiveElement.InfoWidth);
			}
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
					boxBounds.Deflate (ObjectEdge.frameSize.Height/2);

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
					boxBounds.Deflate (ObjectEdge.frameSize.Height/2);

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
			}

			return pos;
		}

		private AttachMode GetAttachMode()
		{
			//	Cherche d'où doit partir la queue du commentaire (de quel côté).
			if (this.attachObject is LinkableObject)
			{
				var box = this.attachObject;
				Rectangle boxBounds = box.Bounds;
				boxBounds.Deflate (ObjectEdge.frameSize.Height/2);

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

			return AttachMode.None;
		}


		private void UpdateTextLayouts()
		{
			if (this.attachObject == null)
			{
				return;
			}

			int lineCount = this.attachObject.Entity.Edges.Count;

			//	Supprime les TextLayout en excès.
			while (this.textLayouts.Count > lineCount)
			{
				this.textLayouts.RemoveAt (0);
			}

			//	Crée les TextLayout manquants.
			while (this.textLayouts.Count < lineCount)
			{
				var textLayout = new TextLayout ();
				textLayout.DefaultFontSize = 10;
				textLayout.BreakMode = TextBreakMode.SingleLine | TextBreakMode.Ellipsis;
				textLayout.Alignment = ContentAlignment.MiddleLeft;

				this.textLayouts.Add (textLayout);
			}

			for (int i = 0; i < lineCount; i++)
			{
				this.textLayouts[i].Text = this.attachObject.Entity.Edges[i].Name.ToString ();
			}
		}


		private ColorFactory ParentColorFartory
		{
			get
			{
				if (this.attachObject == null)
				{
					return this.colorFactory;
				}
				else
				{
					return this.attachObject.ColorFartory;
				}
			}
		}


		private Point PositionCloseButton
		{
			//	Retourne la position du bouton de fermeture.
			get
			{
				return new Point (this.bounds.Right-ActiveButton.buttonRadius-4, this.bounds.Top-ActiveButton.buttonRadius-4);
			}
		}

		private Point PositionWidthButton
		{
			//	Retourne la position du bouton pour modifier la largeur.
			get
			{
				return new Point(this.bounds.Right, this.bounds.Center.Y);
			}
		}

		private Rectangle RectangleLine(int rank)
		{
			return new Rectangle (this.bounds.Left, this.bounds.Top-ObjectInfo.headerHeight-ObjectInfo.lineMargin-ObjectInfo.lineHeight*(rank+1), this.bounds.Width, ObjectInfo.lineHeight);
		}


		protected override void CreateButtons()
		{
			this.buttons.Add (new ActiveButton (ActiveElement.InfoClose, this.colorFactory, GlyphShape.Close,          this.UpdateButtonGeometryClose,    this.UpdateButtonStateClose));
			this.buttons.Add (new ActiveButton (ActiveElement.InfoWidth, this.colorFactory, GlyphShape.HorizontalMove, this.UpdateButtonGeometryWidth,    this.UpdateButtonStateWidth));
		}

		private void UpdateButtonGeometryClose(ActiveButton button)
		{
			button.Center = this.PositionCloseButton;
		}

		private void UpdateButtonGeometryWidth(ActiveButton button)
		{
			button.Center = this.PositionWidthButton;
		}

		private void UpdateButtonStateClose(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Visible = this.IsHeaderHilite && !this.IsDragging;
		}

		private void UpdateButtonStateWidth(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Visible = button.State.Hilited || (this.IsHeaderHilite && !this.IsDragging);
		}

		private bool IsDragging
		{
			get
			{
				return this.isDraggingMove || this.isDraggingWidth || this.editor.IsEditing;
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
						this.boxColor = (ColorItem) System.Enum.Parse (typeof (ColorItem), element);
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


		private static readonly double			headerHeight = 15;
		private static readonly double			footerHeight = 15;
		private static readonly double			lineHeight = 20;
		private static readonly double			lineMargin = 10;
		private static readonly double			shadowOffset = 6;

		private Rectangle						bounds;
		private ObjectNode						attachObject;
		private bool							isVisible;
		private List<TextLayout>				textLayouts;

		private Point							initialPos;
		private bool							isDraggingMove;
		private bool							isDraggingWidth;
		private Point							draggingPos;
	}
}
