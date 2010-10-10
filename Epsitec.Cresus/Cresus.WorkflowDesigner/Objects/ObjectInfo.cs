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
	/// <summary>
	/// Bulle pour repr�senter les informations associ�es � une entit�.
	/// </summary>
	public class ObjectInfo : AbstractObject
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


		public ObjectInfo(Editor editor, AbstractEntity entity)
			: base (editor, entity)
		{
			System.Diagnostics.Debug.Assert (this.Entity != null);

			this.isVisible = true;
			this.boxColor = MainColor.Yellow;

			this.textLayoutTitle = new TextLayout();
			this.textLayoutTitle.DefaultFontSize = 14;
			this.textLayoutTitle.BreakMode = TextBreakMode.SingleLine | TextBreakMode.Ellipsis;
			this.textLayoutTitle.Alignment = ContentAlignment.MiddleCenter;
			this.textLayoutTitle.Text = "Informations";  //Res.Strings.Entities.Info.Title;

			this.textLayoutInfo = new TextLayout();
			this.textLayoutInfo.DefaultFontSize = 10;
			this.textLayoutInfo.BreakMode = TextBreakMode.Hyphenate | TextBreakMode.Split;
		}


		public string Text
		{
			//	Texte du commentaire.
			get
			{
				return this.textLayoutInfo.Text;
			}
			set
			{
				this.textLayoutInfo.Text = value;
				this.UpdateHeight();
			}
		}

		public AbstractObject AttachObject
		{
			//	Object li�e (bo�te ou connexion).
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

		public override void Move(double dx, double dy)
		{
			//	D�place l'objet.
			this.bounds.Offset(dx, dy);
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
					this.editor.SetLocalDirty ();
				}
			}
		}


		protected override string GetToolTipText(ActiveElement element, int fieldRank)
		{
			//	Retourne le texte pour le tooltip.
			if (this.isDraggingMove || this.isDraggingWidth || this.isDraggingAttach)
			{
				return null;  // pas de tooltip
			}

			return base.GetToolTipText(element, fieldRank);
		}

		public override bool MouseMove(Message message, Point pos)
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
				ObjectEdge connexion = this.attachObject as ObjectEdge;

				double attach = connexion.PointToAttach(pos);
				if (attach != 0)
				{
					Point oldPos = connexion.PositionEdgeComment;
					connexion.Edge.CommentAttach = attach;
					Point newPos = connexion.PositionEdgeComment;

					Rectangle bounds = this.bounds;
					bounds.Offset(newPos-oldPos);
					this.SetBounds(bounds);  // d�place le commentaire

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
			//	Le bouton de la souris est press�.
			if (this.hilitedElement == ActiveElement.InfoMove)
			{
				this.isDraggingMove = true;
				this.draggingPos = pos;
				this.editor.LockObject(this);
			}

			if (this.hilitedElement == ActiveElement.InfoWidth)
			{
				this.isDraggingWidth = true;
				this.editor.LockObject(this);
			}
		}

		public override void MouseUp(Message message, Point pos)
		{
			//	Le bouton de la souris est rel�ch�.
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
				if (this.hilitedElement == ActiveElement.InfoClose)
				{
					this.IsVisible = false;

					ObjectEdge connexion = this.attachObject as ObjectEdge;
					if (connexion != null)
					{
						connexion.Edge.HasComment = false;
					}
				}
			}
		}

		protected override bool MouseDetect(Point pos, out ActiveElement element, out int fieldRank)
		{
			//	D�tecte l'�l�ment actif vis� par la souris.
			element = ActiveElement.None;
			fieldRank = -1;

			if (pos.IsZero || !this.isVisible || this.editor.CurrentModifyMode == Editor.ModifyMode.Locked)
			{
				return false;
			}

			//	Souris dans le bouton pour modifier la largeur ?
			if (this.DetectRoundButton(this.PositionWidthButton, pos))
			{
				element = ActiveElement.InfoWidth;
				return true;
			}

			//	Souris dans le bouton de fermeture ?
			if (this.DetectRoundButton(this.PositionCloseButton, pos))
			{
				element = ActiveElement.InfoClose;
				return true;
			}

			//	Souris dans l'en-t�te ?
			if (this.HeaderRectangle.Contains(pos))
			{
				element = ActiveElement.InfoMove;
				return true;
			}

			//	Souris dans la bo�te ?
			if (this.bounds.Contains(pos))
			{
				element = ActiveElement.InfoEdit;
				return true;
			}

			return false;
		}


		public void EditComment()
		{
			//	Modifie le texte du commentaire.
#if false
			Module module = this.editor.Module;
			string text = this.textLayoutInfo.Text;
			text = module.DesignerApplication.DlgEntityComment(text);
			if (text != null)
			{
				this.textLayoutInfo.Text = text;
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
			rect.Deflate(ObjectInfo.textMargin);
			this.textLayoutInfo.LayoutSize = rect.Size;

			double h = System.Math.Floor(this.textLayoutInfo.FindTextHeight()+1);
			h += ObjectInfo.textMargin*2;

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
			rect.Offset(ObjectInfo.shadowOffset, -(ObjectInfo.shadowOffset));
			this.DrawRoundShadow(graphics, rect, ObjectInfo.roundFrameRadius+ObjectInfo.shadowOffset, (int)ObjectInfo.shadowOffset, 0.2);

			//	Dessine l'en-t�te.
			if (!rh.IsEmpty && !this.isDraggingMove && !this.isDraggingWidth && !this.isDraggingAttach)
			{
				rect = rh;
				rect.Inflate(0.5);
				rect.Bottom -= ObjectInfo.roundFrameRadius;
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.ColorCommentHeader(this.hilitedElement == ActiveElement.InfoMove, this.isDraggingMove || this.isDraggingWidth));
				graphics.AddRectangle(rect);
				graphics.RenderSolid(this.GetColor(0));

				rect.Bottom += ObjectInfo.roundFrameRadius;
				rect.Width -= rect.Height;
				rect.Offset(0, 1);
				this.textLayoutTitle.LayoutSize = rect.Size;
				this.textLayoutTitle.Paint(rect.BottomLeft, graphics, Rectangle.MaxValue, this.GetColor(1), GlyphPaintStyle.Normal);
			}

			//	Dessine la bo�te vide avec la queue (bulle de bd).
			Path path = this.GetFramePath();

			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(this.ColorComment(this.hilitedElement != ActiveElement.None));

			graphics.Rasterizer.AddOutline(path);
			graphics.RenderSolid(this.GetColor(0));

			//	Dessine le texte.
			rect = this.bounds;
			rect.Deflate(ObjectInfo.textMargin);
			this.textLayoutInfo.LayoutSize = rect.Size;
			this.textLayoutInfo.Paint(rect.BottomLeft, graphics, Rectangle.MaxValue, this.GetColor(this.IsDarkColorMain ? 1:0), GlyphPaintStyle.Normal);

			//	Dessine le bouton de fermeture.
			if (!rh.IsEmpty)
			{
				if (this.hilitedElement == ActiveElement.InfoClose)
				{
					this.DrawRoundButton(graphics, this.PositionCloseButton, AbstractObject.buttonRadius, GlyphShape.Close, true, false);
				}
				else if (this.IsHeaderHilite && !this.isDraggingMove && !this.isDraggingWidth && !this.isDraggingAttach)
				{
					this.DrawRoundButton(graphics, this.PositionCloseButton, AbstractObject.buttonRadius, GlyphShape.Close, false, false);
				}
			}

			//	Dessine le bouton pour modifier la largeur.
			if (this.hilitedElement == ActiveElement.InfoWidth)
			{
				this.DrawRoundButton(graphics, this.PositionWidthButton, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, true, false);
			}
			else if (this.IsHeaderHilite && !this.isDraggingMove && !this.isDraggingWidth && !this.isDraggingAttach)
			{
				this.DrawRoundButton(graphics, this.PositionWidthButton, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, false, false);
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
			//	Indique si la souris est dans l'en-t�te.
			get
			{
				return (this.hilitedElement == ActiveElement.InfoEdit ||
						this.hilitedElement == ActiveElement.InfoMove ||
						this.hilitedElement == ActiveElement.InfoClose);
			}
		}


		protected Path GetFramePath()
		{
			//	Retourne le chemin du cadre du commentaire.
			AttachMode mode = this.GetAttachMode();
			Point himself = this.GetAttachHimself(mode);
			Point other = this.GetAttachOther(mode);

			double d = Point.Distance(himself, other) - ObjectNode.roundFrameRadius - ObjectInfo.roundFrameRadius;

			himself = Point.Move(himself, other, ObjectInfo.roundFrameRadius);
			other = Point.Move(other, himself, ObjectNode.roundFrameRadius);

			Rectangle bounds = this.bounds;
			bounds.Inflate(0.5);
			Path path = this.PathRoundRectangle(bounds, ObjectInfo.roundFrameRadius);

			if (himself.X == other.X)
			{
				himself.X = System.Math.Floor(himself.X)+0.5;
				other.X = System.Math.Floor(other.X)+0.5;
			}

			if (himself.Y == other.Y)
			{
				himself.Y = System.Math.Floor(himself.Y)+0.5;
				other.Y = System.Math.Floor(other.Y)+0.5;
			}

			if (mode != AttachMode.None && !himself.IsZero && !other.IsZero && d > 0)
			{
				path.MoveTo(himself);
				path.LineTo(other);
			}

			return path;
		}

		protected Point GetAttachHimself(AttachMode mode)
		{
			//	Retourne le point d'attache sur les informations.
			Point pos = Point.Zero;

			if (mode != AttachMode.None)
			{
				Rectangle bounds = this.bounds;
				bounds.Inflate(0.5);
				bounds.Deflate(ObjectInfo.roundFrameRadius);

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

				if (pos.IsZero && this.attachObject is ObjectNode)
				{
					ObjectNode box = this.attachObject as ObjectNode;
					Rectangle boxBounds = box.Bounds;
					boxBounds.Deflate(ObjectNode.roundFrameRadius);

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

		protected Point GetAttachOther(AttachMode mode)
		{
			//	Retourne le point d'attache sur l'objet li� (bo�te).
			Point pos = Point.Zero;

			if (mode != AttachMode.None)
			{
				Rectangle bounds = this.bounds;
				bounds.Inflate(0.5);
				bounds.Deflate(ObjectInfo.roundFrameRadius);

				if (this.attachObject is ObjectNode)
				{
					ObjectNode box = this.attachObject as ObjectNode;
					Rectangle boxBounds = box.Bounds;
					boxBounds.Deflate(ObjectNode.roundFrameRadius);

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

		protected AttachMode GetAttachMode()
		{
			//	Cherche d'o� doit partir la queue du commentaire (de quel c�t�).
			if (this.attachObject is ObjectNode)
			{
				Rectangle bounds = this.bounds;
				bounds.Inflate(0.5);
				bounds.Deflate(ObjectInfo.roundFrameRadius);

				ObjectNode box = this.attachObject as ObjectNode;
				Rectangle boxBounds = box.Bounds;
				boxBounds.Deflate(ObjectNode.roundFrameRadius);

				if (!bounds.IntersectsWith(boxBounds))
				{
					if (bounds.Bottom >= boxBounds.Top && bounds.Right <= boxBounds.Left)
					{
						return AttachMode.BottomRight;
					}
					
					if (bounds.Top <= boxBounds.Bottom && bounds.Right <= boxBounds.Left)
					{
						return AttachMode.TopRight;
					}
					
					if (bounds.Bottom >= boxBounds.Top && bounds.Left >= boxBounds.Right)
					{
						return AttachMode.BottomLeft;
					}
					
					if (bounds.Top <= boxBounds.Bottom && bounds.Left >= boxBounds.Right)
					{
						return AttachMode.TopLeft;
					}
					
					if (bounds.Bottom >= boxBounds.Top)  // commentaire en dessus ?
					{
						return AttachMode.Bottom;
					}
					
					if (bounds.Top <= boxBounds.Bottom)  // commentaire en dessous ?
					{
						return AttachMode.Top;
					}

					if (bounds.Left >= boxBounds.Right)  // commentaire � droite ?
					{
						return AttachMode.Left;
					}

					if (bounds.Right <= boxBounds.Left)  // commentaire � gauche ?
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
				rect.Height = ObjectInfo.infoHeaderHeight;
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

		protected Point PositionAttachToConnexionButton
		{
			//	Retourne la position du bouton pour modifier l'attache � la connexion.
			get
			{
				if (this.attachObject != null && this.attachObject is ObjectEdge)
				{
					ObjectEdge connexion = this.attachObject as ObjectEdge;
					return connexion.PositionEdgeComment;
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


		protected void UpdateFieldColor()
		{
			//	Met � jour l'information de couleur dans le champ associ�.
			if (this.attachObject is ObjectEdge)
			{
				ObjectEdge connexion = this.attachObject as ObjectEdge;
				connexion.Edge.CommentMainColor = this.BackgroundMainColor;
			}
		}


		private WorkflowNodeEntity Entity
		{
			get
			{
				return this.entity as WorkflowNodeEntity;
			}
		}


		#region Serialization
		public void WriteXml(XmlWriter writer)
		{
#if false
			//	S�rialise toutes les informations du commentaire.
			//	Utilis� seulement pour les commentaires associ�s � des bo�tes.
			//	Les commentaires associ�s � des connexions sont s�rialis�s par Field.
			writer.WriteStartElement(Xml.Info);
			
			writer.WriteElementString(Xml.Bounds, this.bounds.ToString());
			writer.WriteElementString(Xml.Text, this.textLayoutInfo.Text);
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
						this.textLayoutInfo.Text = element;
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
					System.Diagnostics.Debug.Assert(reader.Name == Xml.Info);
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


		protected static readonly double infoHeaderHeight = 24;
		protected static readonly double textMargin = 5;
		protected static readonly double queueThickness = 5;
		protected static readonly double roundFrameRadius = 12;
		protected static readonly double shadowOffset = 8;

		protected Rectangle bounds;
		protected AbstractObject attachObject;
		protected bool isVisible;
		protected TextLayout textLayoutTitle;
		protected TextLayout textLayoutInfo;

		protected bool isDraggingMove;
		protected bool isDraggingWidth;
		protected bool isDraggingAttach;
		protected Point draggingPos;
	}
}
