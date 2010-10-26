//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

namespace Epsitec.Cresus.WorkflowDesigner.Objects
{
	public class ObjectInfo : BalloonObject
	{
		public ObjectInfo(Editor editor, AbstractEntity entity)
			: base (editor, entity)
		{
			this.colorFactory.ColorItem = ColorItem.Blue;

			this.textLayoutTitle.Text = "Informations";

			this.textLayouts = new List<TextLayout> ();
		}


		public void UpdateAfterAttachChanged()
		{
			this.UpdateTextLayouts ();
			this.UpdateHeight ();
		}


		protected override string GetToolTipText(ActiveElement element)
		{
			//	Retourne le texte pour le tooltip.
			if (this.isDraggingMove || this.isDraggingWidth || this.isDraggingLine)
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
			//	Met en �vidence la bo�te selon la position de la souris.
			//	Si la souris est dans cette bo�te, retourne true.
			base.MouseMove (message, pos);

			if (this.isMouseDownForDrag && !this.isDraggingMove && this.HilitedElement == ActiveElement.InfoMove)
			{
				this.isDraggingMove = true;
				this.UpdateButtonsState ();
				this.draggingPos = this.initialPos;
			}

			if (this.isDraggingMove)
			{
				Rectangle bounds = this.bounds;

				bounds.Offset(pos-this.draggingPos);
				this.draggingPos = pos;

				this.Bounds = bounds;
				this.editor.Invalidate();
				return true;
			}
			else if (this.isDraggingWidth)
			{
				Rectangle bounds = this.bounds;

				bounds.Right = pos.X;
				bounds.Width = System.Math.Max(bounds.Width, AbstractObject.commentMinWidth);

				this.Bounds = bounds;
				this.UpdateHeight();
				this.editor.Invalidate();
				return true;
			}
			else if (this.isDraggingLine)
			{
				int lineCount = this.LineCount;
				int sel = -1;
				for (int i = 0; i <= lineCount; i++)
				{
					if (this.RectangleLineSeparator (i).Contains (pos))
					{
						sel = i;
						break;
					}
				}
				if (this.draggingLineCurrentRank != sel)
				{
					this.draggingLineCurrentRank = sel;
					this.editor.Invalidate ();
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		public override void MouseDown(Message message, Point pos)
		{
			//	Le bouton de la souris est press�.
			base.MouseDown (message, pos);

			if (this.HilitedElement == ActiveElement.InfoWidth)
			{
				this.isDraggingWidth = true;
				this.UpdateButtonsState ();
			}

			if (this.HilitedElement >= ActiveElement.InfoLine1 &&
				this.HilitedElement <= ActiveElement.InfoLine1+ObjectInfo.maxLines)
			{
				this.isDraggingLine = true;
				this.draggingLineInitialRank = this.HilitedElement - ActiveElement.InfoLine1;
				this.draggingLineCurrentRank = -1;
				this.editor.Invalidate ();
			}
		}

		public override void MouseUp(Message message, Point pos)
		{
			//	Le bouton de la souris est rel�ch�.
			base.MouseUp (message, pos);

			if (this.isDraggingMove)
			{
				this.isDraggingMove = false;
				this.UpdateButtonsState ();
				this.editor.UpdateAfterCommentChanged();
				this.editor.SetLocalDirty ();
			}
			else if (this.isDraggingWidth)
			{
				this.isDraggingWidth = false;
				this.UpdateButtonsState ();
				this.editor.UpdateAfterCommentChanged();
				this.editor.SetLocalDirty ();
			}
			else if (this.isDraggingLine)
			{
				if (this.draggingLineCurrentRank != -1 && this.draggingLineInitialRank != -1)
				{
					if (this.draggingLineCurrentRank <= this.draggingLineInitialRank-1)
					{
						var t = this.Node.Entity.Edges[this.draggingLineInitialRank];
						this.Node.Entity.Edges.RemoveAt (this.draggingLineInitialRank);
						this.Node.Entity.Edges.Insert (this.draggingLineCurrentRank, t);
					}
					else if (this.draggingLineCurrentRank >= this.draggingLineInitialRank+2)
					{
						var t = this.Node.Entity.Edges[this.draggingLineInitialRank];
						this.Node.Entity.Edges.RemoveAt (this.draggingLineInitialRank);
						this.Node.Entity.Edges.Insert (this.draggingLineCurrentRank-1, t);
					}
				}

				this.isDraggingLine = false;
				this.UpdateButtonsState ();
				this.editor.UpdateAfterCommentChanged ();
				this.editor.SetLocalDirty ();
			}
			else
			{
				if (this.HilitedElement == ActiveElement.InfoClose)
				{
					if (this.Node != null)
					{
						this.Node.Info = null;
					}

					this.editor.RemoveBalloon (this);
					this.editor.UpdateAfterCommentChanged ();
				}
			}
		}

		public override ActiveElement MouseDetectBackground(Point pos)
		{
			//	D�tecte l'�l�ment actif vis� par la souris.
			if (pos.IsZero || this.editor.CurrentModifyMode == Editor.ModifyMode.Locked)
			{
				return ActiveElement.None;
			}

			//	Souris dans l'en-t�te ?
			if (this.HeaderRectangle.Contains (pos))
			{
				return ActiveElement.InfoMove;
			}

			//	Souris dans la bo�te ?
			if (this.bounds.Contains(pos))
			{
				int  lineCount = this.LineCount;
				for (int i = 0; i < lineCount; i++)
				{
					if (this.RectangleLine (i).Contains (pos))
					{
						return ActiveElement.InfoLine1+i;
					}
				}

				return ActiveElement.InfoInside;
			}

			return ActiveElement.None;
		}

		public override ActiveElement MouseDetectForeground(Point pos)
		{
			//	D�tecte l'�l�ment actif vis� par la souris.
			if (pos.IsZero || this.editor.CurrentModifyMode == Editor.ModifyMode.Locked)
			{
				return ActiveElement.None;
			}

			if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked)
			{
				return this.DetectButtons (pos);
			}

			return ActiveElement.None;
		}


		public override MouseCursorType MouseCursor
		{
			get
			{
				if (this.HilitedElement == ActiveElement.InfoMove)
				{
					return MouseCursorType.Move;
				}

				if (this.HilitedElement >= ActiveElement.InfoLine1 &&
					this.HilitedElement <= ActiveElement.InfoLine1+ObjectInfo.maxLines)
				{
					return MouseCursorType.VerticalMove;
				}

				return MouseCursorType.Finger;
			}
		}


		public void UpdateHeight()
		{
			//	Adapte la hauteur de l'information en fonction de sa largeur et du contenu.
			Rectangle rect = this.bounds;

			double h = System.Math.Max (this.LineCount, 1) * ObjectInfo.lineHeight;

			var a = this.GetAttachMode();
			if (a == AttachMode.Bottom || a == AttachMode.None)
			{
				this.bounds.Height = h;
			}
			else
			{
				this.bounds.Bottom = this.bounds.Top-h;
			}

			this.UpdateGeometry ();
		}


		public override void DrawBackground(Graphics graphics)
		{
			//	Dessine le fond de l'objet.
			this.colorFactory.ColorItem = this.ParentColorFartory.ColorItem;

			base.DrawBackground (graphics);

			Rectangle rect;

			bool dragging = (this.hilitedElement == ActiveElement.InfoInside);

			//	Dessine les lignes.
			if (this.Node != null)
			{
				int sel = -1;

				if (this.hilitedElement >= ActiveElement.InfoLine1 &&
					this.hilitedElement <= ActiveElement.InfoLine1+ObjectInfo.maxLines)
				{
					sel = this.hilitedElement - ActiveElement.InfoLine1;
					rect = this.RectangleLine (sel);

					graphics.AddFilledRectangle (rect);
					graphics.RenderSolid (this.colorFactory.GetColorMain (this.isDraggingLine ? 0.3 : 1.0));
				}

				int lineCount = this.LineCount;
				for (int i = 0; i < lineCount; i++)
				{
					var colorText = (i == sel) ? this.colorFactory.GetColor (1) : this.colorFactory.GetColor (0);
					rect = this.RectangleLine (i);

					if (i < lineCount-1)
					{
						this.DrawGradientLine (graphics, new Point (rect.Left+1, rect.Bottom-0.5), new Point (rect.Right-1, rect.Bottom-0.5));
					}

					rect.Deflate (10, 0);
					this.textLayouts[i].LayoutSize = rect.Size;
					this.textLayouts[i].Paint (rect.BottomLeft, graphics, Rectangle.MaxValue, colorText, GlyphPaintStyle.Normal);
				}
			}

			//	Dessine la fl�che pendant un d�placement de ligne.
			if (this.isDraggingLine && this.draggingLineCurrentRank != -1)
			{
				if (this.draggingLineCurrentRank <= this.draggingLineInitialRank-1 ||
					this.draggingLineCurrentRank >= this.draggingLineInitialRank+2)
				{
					rect = this.RectangleLineSeparator (this.draggingLineCurrentRank);
					rect = new Rectangle (rect.Left, rect.Center.Y-2, rect.Width, 4);
					graphics.AddFilledRectangle (rect);
					graphics.RenderSolid (this.colorFactory.GetColorMain ());

					Point p1 = this.RectangleLine (this.draggingLineInitialRank).Center;
					Point p2 = this.RectangleLineSeparator (this.draggingLineCurrentRank).Center;
					this.DrawMovingArrow (graphics, p1, p2);
				}
			}
		}

		public override void DrawForeground(Graphics graphics)
		{
			//	Dessine le dessus de l'objet.
			base.DrawForeground (graphics);
		}

		private void DrawGradientLine(Graphics graphics, Point p1, Point p2)
		{
			Color c1 = this.colorFactory.GetColor (0, 0.0);  // noir transparent
			Color c2 = this.colorFactory.GetColor (0, 0.5);  // noir semi-opaque

			Rectangle rect;

			//	Dessine la moiti� gauche.
			rect = new Rectangle (p1.X, p1.Y-0.5, (p2.X-p1.X)/2, 1);
			graphics.AddFilledRectangle (rect);
			this.RenderHorizontalGradient (graphics, this.bounds, c1, c2);

			//	Dessine la moiti� droite.
			rect.Offset (rect.Width, 0);
			graphics.AddFilledRectangle (rect);
			this.RenderHorizontalGradient (graphics, this.bounds, c2, c1);
		}

		private void DrawMovingArrow(Graphics graphics, Point p1, Point p2)
		{
			//	Dessine une fl�che pendant le d�placement d'un champ.
			p2 = Point.Move (p2, p1, 1);
			double d = (p1.Y > p2.Y) ? -6 : 6;
			double sx = 3;

			Path path = new Path ();
			path.MoveTo (p2);
			path.LineTo (p2.X-d*3/sx, p2.Y-d*2);
			path.LineTo (p2.X-d/sx, p2.Y-d*2);
			path.LineTo (p1.X-d/sx, p1.Y);
			path.LineTo (p1.X+d/sx, p1.Y);
			path.LineTo (p2.X+d/sx, p2.Y-d*2);
			path.LineTo (p2.X+d*3/sx, p2.Y-d*2);
			path.Close ();

			graphics.Rasterizer.AddSurface (path);
			graphics.RenderSolid (this.colorFactory.GetColorMain ());
		}

		private bool IsHeaderHilite
		{
			//	Indique si la souris est dans l'en-t�te.
			get
			{
				return (this.hilitedElement == ActiveElement.InfoInside ||
						this.hilitedElement == ActiveElement.InfoMove ||
						this.hilitedElement == ActiveElement.InfoClose ||
						this.hilitedElement == ActiveElement.InfoWidth ||
						(this.hilitedElement >= ActiveElement.InfoLine1 &&
						 this.hilitedElement <= ActiveElement.InfoLine1+ObjectInfo.maxLines));
			}
		}


		private void UpdateTextLayouts()
		{
			if (this.Node == null)
			{
				return;
			}

			int lineCount = this.LineCount;

			//	Supprime les TextLayout en exc�s.
			while (this.textLayouts.Count > lineCount)
			{
				this.textLayouts.RemoveAt (0);
			}

			//	Cr�e les TextLayout manquants.
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
				this.textLayouts[i].Text = this.Node.Entity.Edges[i].Name.ToString ();
			}
		}


		private ColorFactory ParentColorFartory
		{
			get
			{
				if (this.Node == null)
				{
					return this.colorFactory;
				}
				else
				{
					return this.Node.ColorFartory;
				}
			}
		}


		private Rectangle RectangleLine(int rank)
		{
			return new Rectangle (this.bounds.Left, this.bounds.Top-ObjectInfo.lineHeight*(rank+1), this.bounds.Width, ObjectInfo.lineHeight);
		}

		private Rectangle RectangleLineSeparator(int rank)
		{
			return new Rectangle (this.bounds.Left, this.bounds.Top+ObjectInfo.lineHeight/2-ObjectInfo.lineHeight*(rank+1), this.bounds.Width, ObjectInfo.lineHeight);
		}

		private int LineCount
		{
			get
			{
				if (this.Node == null)
				{
					return 0;
				}
				else
				{
					return this.Node.Entity.Edges.Count;
				}
			}
		}

		private ObjectNode Node
		{
			get
			{
				return this.attachObject as ObjectNode;
			}
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


		#region Serialize
		public override void Serialize(XElement xml)
		{
			base.Serialize (xml);
		}

		public override void Deserialize(XElement xml)
		{
			base.Deserialize (xml);
		}
		#endregion


		private static readonly double			lineHeight = 20;
		public static readonly int				maxLines = 20;

		private List<TextLayout>				textLayouts;

		private bool							isDraggingLine;
		private int								draggingLineInitialRank;
		private int								draggingLineCurrentRank;
	}
}
