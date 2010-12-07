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
	public class ObjectCartridge : AbstractObject
	{
		public ObjectCartridge(Editor editor, AbstractEntity entity)
			: base (editor, entity)
		{
			System.Diagnostics.Debug.Assert (this.entity != null);

			this.textLayoutHeader = new TextLayout ();
			this.textLayoutHeader.DefaultFontSize = 14;
			this.textLayoutHeader.BreakMode = TextBreakMode.SingleLine | TextBreakMode.Ellipsis;
			this.textLayoutHeader.Alignment = ContentAlignment.MiddleCenter;
			this.textLayoutHeader.Text = "Cartouche";

			this.textLayoutTitle = new TextLayout ();
			this.textLayoutTitle.DefaultFontSize = 12;
			this.textLayoutTitle.Alignment = ContentAlignment.MiddleCenter;
			this.textLayoutTitle.BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;

			this.textLayoutSubtitle = new TextLayout ();
			this.textLayoutSubtitle.DefaultFontSize = 9;
			this.textLayoutSubtitle.Alignment = ContentAlignment.MiddleLeft;
			this.textLayoutSubtitle.BreakMode = TextBreakMode.Hyphenate;

			if (this.Entity.WorkflowName.IsNullOrWhiteSpace)
			{
				this.Entity.WorkflowName = "Nouveau workflow";
			}

			if (this.Entity.WorkflowDescription.IsNullOrWhiteSpace)
			{
				this.Entity.WorkflowDescription = "Cliquez ici pour entrer une description...";
			}

			this.UpdateTitle ();
			this.UpdateSubtitle ();

			this.Bounds = new Rectangle (Point.Zero, ObjectCartridge.frameSize);
		}


		public override Margins RedimMargin
		{
			get
			{
				return new Margins (0);
			}
		}

		public override void Move(double dx, double dy)
		{
			//	Déplace l'objet.
			this.bounds.Offset (dx, dy);
			this.UpdateGeometry ();
		}

	
		public override void AcceptEdition()
		{
			if (this.editingElement == ActiveElement.CartridgeEditName)
			{
				this.Entity.WorkflowName = this.editingTextField.Text;
				this.UpdateTitle ();
			}

			if (this.editingElement == ActiveElement.CartridgeEditDescription)
			{
				this.Entity.WorkflowDescription = this.editingTextField.Text;
				this.UpdateSubtitle ();
			}

			this.StopEdition ();
		}

		public override void CancelEdition()
		{
			this.StopEdition ();
		}

		public override void StartEdition()
		{
			this.StartEdition (ActiveElement.NodeHeader);
		}

		private void StartEdition(ActiveElement element)
		{
			Rectangle rect = Rectangle.Empty;
			string text = null;

			if (element == ActiveElement.CartridgeEditName)
			{
				rect = this.RectangleTitle;
				text = this.Entity.WorkflowName.ToString ();

				this.editingTextField  = new TextField ();
			}

			if (element == ActiveElement.CartridgeEditDescription)
			{
				rect = this.RectangleSubtitle;
				text = this.Entity.WorkflowDescription.ToString ();

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
			double width  = System.Math.Max (p2.X-p1.X, 30);
			double height = System.Math.Max (p1.Y-p2.Y, 20);

			rect = new Rectangle (new Point (p1.X, p1.Y-height), new Size (width, height));

			this.editingTextField.Parent = this.editor;
			this.editingTextField.SetManualBounds (rect);
			this.editingTextField.Text = text;
			this.editingTextField.TabIndex = 1;
			this.editingTextField.SelectAll ();
			this.editingTextField.Focus ();

			this.editor.EditingObject = this;
			this.editor.ClearHilited ();
			this.UpdateButtonsState ();
		}

		private void StopEdition()
		{
			this.editor.Children.Remove (this.editingTextField);
			this.editingTextField = null;

			this.editor.EditingObject = null;
			this.UpdateButtonsState ();
		}


		private void UpdateTitle()
		{
			string text = this.Entity.WorkflowName.ToString ();

			this.textLayoutTitle.Text = Misc.Bold (text);
		}

		private void UpdateSubtitle()
		{
			string text = this.Entity.WorkflowDescription.ToString ();

			this.textLayoutSubtitle.Text = text;
		}


		protected override string GetToolTipText(ActiveElement element)
		{
			//	Retourne le texte pour le tooltip.
			switch (element)
			{
				case ActiveElement.CartridgeWidth:
					return "Change la largeur du cartouche";
			}

			return base.GetToolTipText (element);
		}


		public override bool MouseMove(Message message, Point pos)
		{
			//	Met en évidence la boîte selon la position de la souris.
			//	Si la souris est dans cette boîte, retourne true.
			base.MouseMove (message, pos);

			if (this.isMouseDownForDrag &&
				this.draggingMode == DraggingMode.None &&
				(this.HilitedElement == ActiveElement.CartridgeMove ||
				 this.HilitedElement == ActiveElement.CartridgeEditName ||
				 this.HilitedElement == ActiveElement.CartridgeEditDescription))
			{
				this.draggingMode = DraggingMode.MoveObject;
				this.UpdateButtonsState ();
				this.draggingPos = this.initialPos;
			}

			if (this.draggingMode == DraggingMode.MoveObject)
			{
				Rectangle bounds = this.bounds;

				bounds.Offset (pos-this.draggingPos);
				this.draggingPos = pos;

				this.Bounds = bounds;
				this.editor.Invalidate ();
				return true;
			}

			if (this.draggingMode == DraggingMode.ChangeWidth)
			{
				this.ChangeBoundsWidth (pos.X, AbstractObject.commentMinWidth);
				this.editor.Invalidate ();
				return true;
			}

			return false;
		}

		public override void MouseDown(Message message, Point pos)
		{
			//	Le bouton de la souris est pressé.
			base.MouseDown (message, pos);

			if (this.HilitedElement == ActiveElement.CartridgeWidth)
			{
				this.draggingMode = DraggingMode.ChangeWidth;
				this.UpdateButtonsState ();
			}
		}

		public override void MouseUp(Message message, Point pos)
		{
			//	Le bouton de la souris est relâché.
			base.MouseUp (message, pos);

			if ((this.HilitedElement == ActiveElement.CartridgeEditName ||
				 this.HilitedElement == ActiveElement.CartridgeEditDescription) &&
				this.draggingMode == DraggingMode.None)
			{
				this.StartEdition (this.HilitedElement);
				return;
			}

			if (this.draggingMode == DraggingMode.MoveObject)
			{
				this.draggingMode = DraggingMode.None;
				this.UpdateButtonsState ();
				this.editor.UpdateAfterGeometryChanged (this);
				this.editor.SetLocalDirty ();
			}
			else if (this.draggingMode == DraggingMode.ChangeWidth)
			{
				this.draggingMode = DraggingMode.None;
				this.UpdateButtonsState ();
				this.editor.UpdateAfterGeometryChanged (this);
				this.editor.SetLocalDirty ();
			}
		}

		public override ActiveElement MouseDetectBackground(Point pos)
		{
			//	Détecte l'élément actif visé par la souris.
			if (this.RectangleHeader.Contains (pos))
			{
				return ActiveElement.CartridgeMove;
			}

			if (this.RectangleTitle.Contains (pos))
			{
				return ActiveElement.CartridgeEditName;
			}

			if (this.RectangleSubtitle.Contains (pos))
			{
				return ActiveElement.CartridgeEditDescription;
			}

			return ActiveElement.None;
		}

		public override ActiveElement MouseDetectForeground(Point pos)
		{
			//	Détecte l'élément actif visé par la souris.
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
				if (this.HilitedElement == ActiveElement.CartridgeEditName ||
					this.HilitedElement == ActiveElement.CartridgeEditDescription)
				{
					if (this.draggingMode == DraggingMode.MoveObject)
					{
						return MouseCursorType.Move;
					}
					else
					{
						return MouseCursorType.MoveOrEdit;
					}
				}

				if (this.HilitedElement == ActiveElement.CartridgeMove)
				{
					return MouseCursorType.Move;
				}

				return MouseCursorType.Finger;
			}
		}


		public override void DrawBackground(Graphics graphics)
		{
			//	Dessine le fond de l'objet.
			base.DrawBackground (graphics);

			Rectangle rect;
			Color textColor = Color.FromBrightness (0);
			Color frameColor = (this.hilitedElement == ActiveElement.None) ? Color.FromBrightness (1) : Color.FromBrightness (0.9);

			//	Dessine l'en-tête.
			if (this.hilitedElement != ActiveElement.None)
			{
				rect = this.RectangleHeader;
				rect.Inflate (-0.5, -0.5, 0.5, 0.5);
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (this.colorFactory.GetColor (0.5, (this.draggingMode == DraggingMode.None) ? 1 : 0.1));
				graphics.AddRectangle (rect);
				graphics.RenderSolid (this.colorFactory.GetColor (0, (this.draggingMode == DraggingMode.None) ? 1 : 0.2));

				rect.Offset (0, 1);
				this.textLayoutHeader.LayoutSize = rect.Size;
				this.textLayoutHeader.Paint (rect.BottomLeft, graphics, Rectangle.MaxValue, this.colorFactory.GetColor (1), GlyphPaintStyle.Normal);
			}

			//	Dessine le cartouche.
			Rectangle titleRect    = this.RectangleTitle;
			Rectangle subtitleRect = this.RectangleSubtitle;

			titleRect   .Inflate (-0.5, -0.5, -0.5, -0.5);
			subtitleRect.Inflate (-0.5, -0.5,  0.5, -0.5);

			graphics.AddFilledRectangle (titleRect);
			graphics.AddFilledRectangle (subtitleRect);
			graphics.RenderSolid (frameColor);

			rect = titleRect;
			rect.Inflate (-4, -4, -2, -4);
			this.textLayoutTitle.LayoutSize = rect.Size;
			this.textLayoutTitle.Paint (rect.BottomLeft, graphics, Rectangle.MaxValue, textColor, GlyphPaintStyle.Normal);

			rect = subtitleRect;
			rect.Inflate (-4, -4, -2, -4);
			this.textLayoutSubtitle.LayoutSize = rect.Size;
			this.textLayoutSubtitle.Paint (rect.BottomLeft, graphics, Rectangle.MaxValue, textColor, GlyphPaintStyle.Normal);

			graphics.AddRectangle (titleRect);
			graphics.AddRectangle (subtitleRect);
			
			graphics.RenderSolid (textColor);
		}

		public override void DrawForeground(Graphics graphics)
		{
			//	Dessine le dessus de l'objet.
			base.DrawForeground (graphics);
		}


		protected Rectangle RectangleHeader
		{
			get
			{
				Rectangle rect = this.bounds;
				rect.Bottom = rect.Top;
				rect.Height = ObjectCartridge.headerHeight;
				return rect;
			}
		}

		private Rectangle RectangleTitle
		{
			get
			{
				return new Rectangle (this.bounds.Left, this.bounds.Top-ObjectCartridge.titleHeight, this.bounds.Width, ObjectCartridge.titleHeight);
			}
		}

		private Rectangle RectangleSubtitle
		{
			get
			{
				return new Rectangle (this.bounds.Left, this.bounds.Bottom, this.bounds.Width, ObjectCartridge.frameSize.Height-ObjectCartridge.titleHeight);
			}
		}


		public WorkflowDefinitionEntity Entity
		{
			get
			{
				return this.entity as WorkflowDefinitionEntity;
			}
		}


		private bool IsHeaderHilite
		{
			//	Indique si la souris est dans l'en-tête.
			get
			{
				return (this.hilitedElement == ActiveElement.CartridgeMove ||
						this.hilitedElement == ActiveElement.CartridgeWidth ||
						this.hilitedElement == ActiveElement.CartridgeEditName ||
						this.hilitedElement == ActiveElement.CartridgeEditDescription);
			}
		}


		protected Point PositionWidthButton
		{
			//	Retourne la position du bouton pour modifier la largeur.
			get
			{
				return new Point (this.bounds.Right, this.bounds.Center.Y);
			}
		}

	
		protected override void CreateButtons()
		{
			this.buttons.Add (new ActiveButton (ActiveElement.CartridgeWidth, this.colorFactory, GlyphShape.HorizontalMove, this.UpdateButtonGeometryWidth, this.UpdateButtonStateWidth));
		}

		private void UpdateButtonGeometryWidth(ActiveButton button)
		{
			button.Center = this.PositionWidthButton;
		}

		private void UpdateButtonStateWidth(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Visible = button.State.Hilited || (this.IsHeaderHilite && !this.IsDragging);
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


		private static readonly Size			frameSize = new Size (250, 70);
		private static readonly double			titleHeight = 20;
		private static readonly double			headerHeight = 24;

		private TextLayout						textLayoutHeader;
		private TextLayout						textLayoutTitle;
		private TextLayout						textLayoutSubtitle;

		private ActiveElement					editingElement;
		private AbstractTextField				editingTextField;

		private Point							draggingPos;
	}
}
