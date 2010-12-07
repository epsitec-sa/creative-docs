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
	public class ObjectComment : BalloonObject
	{
		public ObjectComment(Editor editor, AbstractEntity entity)
			: base (editor, entity)
		{
			this.colorFactory.ColorItem = ColorItem.Yellow;

			this.textLayoutTitle.Text = "Commentaires";

			this.textLayoutComment = new TextLayout();
			this.textLayoutComment.DefaultFontSize = 10;
			this.textLayoutComment.BreakMode = TextBreakMode.Hyphenate | TextBreakMode.Split;
			this.textLayoutComment.Text = "Cliquez ici pour entrer un commentaire...";
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
				this.AttachedNodeDescription = value;
			}
		}

		public string AttachedNodeDescription
		{
			//	Texte de la description du noeud auquel est rattaché le commentaire.
			get
			{
				if (this.attachObject is ObjectNode)
				{
					var node = this.attachObject as ObjectNode;
					return node.Entity.Description.ToString ();
				}

				return null;
			}
			set
			{
				if (this.attachObject is ObjectNode)
				{
					var node = this.attachObject as ObjectNode;
					node.Entity.Description = value;
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


		protected override string GetToolTipText(ActiveElement element)
		{
			//	Retourne le texte pour le tooltip.
			switch (element)
			{
				case ActiveElement.CommentAttachTo:
					return "Déplace l'attache du commentaire";

				case ActiveElement.CommentClose:
					return "Ferme le commentaire";

				case ActiveElement.CommentWidth:
					return "Change la largeur du commentaire";

				case ActiveElement.CommentColor1:
					return "Jaune";

				case ActiveElement.CommentColor2:
					return "Orange";

				case ActiveElement.CommentColor3:
					return "Rouge";

				case ActiveElement.CommentColor4:
					return "Lilas";

				case ActiveElement.CommentColor5:
					return "Violet";

				case ActiveElement.CommentColor6:
					return "Bleu";

				case ActiveElement.CommentColor7:
					return "Vert";

				case ActiveElement.CommentColor8:
					return "Gris";
			}

			return base.GetToolTipText (element);
		}


		public override bool MouseMove(Message message, Point pos)
		{
			//	Met en évidence la boîte selon la position de la souris.
			//	Si la souris est dans cette boîte, retourne true.
			base.MouseMove (message, pos);

			if (this.isMouseDownForDrag && this.draggingMode == DraggingMode.None && this.hilitedElement == ActiveElement.CommentMove)
			{
				this.draggingMode = DraggingMode.MoveObject;
				this.UpdateButtonsState ();
				this.draggingPos = this.initialPos;
			}

			if (this.draggingMode == DraggingMode.MoveObject)
			{
				Rectangle bounds = this.bounds;

				bounds.Offset(pos-this.draggingPos);
				this.draggingPos = pos;

				this.Bounds = bounds;
				this.editor.Invalidate();
				return true;
			}
			else if (this.draggingMode == DraggingMode.ChangeWidth)
			{
				this.ChangeBoundsWidth (pos.X, AbstractObject.commentMinWidth);
				this.UpdateHeight();
				this.editor.Invalidate();
				return true;
			}
			else if (this.draggingMode == DraggingMode.MoveCommentAttach)
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
					this.Bounds = bounds;  // déplace le commentaire

					this.editor.Invalidate();
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
			//	Le bouton de la souris est pressé.
			base.MouseDown (message, pos);

			if (this.HilitedElement == ActiveElement.CommentWidth)
			{
				this.draggingMode = DraggingMode.ChangeWidth;
				this.UpdateButtonsState ();
			}

			if (this.HilitedElement == ActiveElement.CommentAttachTo)
			{
				this.draggingMode = DraggingMode.MoveCommentAttach;
				this.UpdateButtonsState ();
			}
		}

		public override void MouseUp(Message message, Point pos)
		{
			//	Le bouton de la souris est relâché.
			base.MouseUp (message, pos);

			if (this.HilitedElement == ActiveElement.CommentEdit && this.draggingMode == DraggingMode.None)
			{
				this.StartEdition (this.HilitedElement);
				return;
			}

			if (this.draggingMode == DraggingMode.MoveObject)
			{
				this.draggingMode = DraggingMode.None;
				this.UpdateButtonsState ();
				this.editor.UpdateAfterCommentChanged();
				this.editor.SetLocalDirty ();
			}
			else if (this.draggingMode == DraggingMode.ChangeWidth)
			{
				this.draggingMode = DraggingMode.None;
				this.UpdateButtonsState ();
				this.editor.UpdateAfterCommentChanged();
				this.editor.SetLocalDirty ();
			}
			else if (this.draggingMode == DraggingMode.MoveCommentAttach)
			{
				this.draggingMode = DraggingMode.None;
				this.UpdateButtonsState ();
				this.editor.UpdateAfterCommentChanged();
				this.editor.SetLocalDirty ();
			}
			else
			{
				if (this.HilitedElement == ActiveElement.CommentClose)
				{
					ObjectLink link = this.attachObject as ObjectLink;
					if (link != null)
					{
						link.Comment = null;
					}

					LinkableObject obj = this.attachObject as LinkableObject;
					if (obj != null)
					{
						obj.Comment = null;
					}

					this.AttachedNodeDescription = null;
					this.editor.RemoveBalloon (this);
					this.editor.UpdateAfterCommentChanged ();
				}

				if (this.HilitedElement == ActiveElement.CommentColor1)
				{
					this.BackgroundColorItem = ColorItem.Yellow;
				}

				if (this.HilitedElement == ActiveElement.CommentColor2)
				{
					this.BackgroundColorItem = ColorItem.Orange;
				}

				if (this.HilitedElement == ActiveElement.CommentColor3)
				{
					this.BackgroundColorItem = ColorItem.Red;
				}

				if (this.HilitedElement == ActiveElement.CommentColor4)
				{
					this.BackgroundColorItem = ColorItem.Lilac;
				}

				if (this.HilitedElement == ActiveElement.CommentColor5)
				{
					this.BackgroundColorItem = ColorItem.Purple;
				}

				if (this.HilitedElement == ActiveElement.CommentColor6)
				{
					this.BackgroundColorItem = ColorItem.Blue;
				}

				if (this.HilitedElement == ActiveElement.CommentColor7)
				{
					this.BackgroundColorItem = ColorItem.Green;
				}

				if (this.HilitedElement == ActiveElement.CommentColor8)
				{
					this.BackgroundColorItem = ColorItem.Grey;
				}
			}
		}

		public override ActiveElement MouseDetectBackground(Point pos)
		{
			//	Détecte l'élément actif visé par la souris.
			if (pos.IsZero || this.editor.CurrentModifyMode == Editor.ModifyMode.Locked)
			{
				return ActiveElement.None;
			}

			//	Souris dans l'en-tête ?
			if (this.HeaderRectangle.Contains (pos))
			{
				return ActiveElement.CommentMove;
			}

			//	Souris dans la boîte ?
			if (this.bounds.Contains (pos))
			{
				return ActiveElement.CommentEdit;
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
				if (this.HilitedElement == ActiveElement.CommentEdit)
				{
					return MouseCursorType.IBeam;
				}

				if (this.HilitedElement == ActiveElement.CommentMove)
				{
					return MouseCursorType.Move;
				}

				return MouseCursorType.Finger;
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

			this.UpdateGeometry ();
		}


		public override void DrawBackground(Graphics graphics)
		{
			//	Dessine le fond de l'objet.
			base.DrawBackground (graphics);

			//	Dessine le texte.
			var rect = this.bounds;
			rect.Deflate (ObjectComment.textMargin);
			this.textLayoutComment.LayoutSize = rect.Size;
			this.textLayoutComment.Paint (rect.BottomLeft, graphics, Rectangle.MaxValue, this.colorFactory.GetColor (0), GlyphPaintStyle.Normal);
		}

		public override void DrawForeground(Graphics graphics)
		{
			//	Dessine le dessus de l'objet.
			base.DrawForeground (graphics);
		}

		private bool IsHeaderHilite
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
						this.hilitedElement == ActiveElement.CommentWidth ||
						this.hilitedElement == ActiveElement.CommentAttachTo);
			}
		}


		protected override void CreateButtons()
		{
			this.buttons.Add (new ActiveButton (ActiveElement.CommentClose,    this.colorFactory, GlyphShape.Close,          this.UpdateButtonGeometryClose,    this.UpdateButtonStateClose));
			this.buttons.Add (new ActiveButton (ActiveElement.CommentWidth,    this.colorFactory, GlyphShape.HorizontalMove, this.UpdateButtonGeometryWidth,    this.UpdateButtonStateWidth));
			this.buttons.Add (new ActiveButton (ActiveElement.CommentAttachTo, this.colorFactory, GlyphShape.HorizontalMove, this.UpdateButtonGeometryAttachTo, this.UpdateButtonStateAttachTo));

			this.buttons.Add (new ActiveButton (ActiveElement.CommentColor1, this.colorFactory, ColorItem.Yellow, this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.CommentColor2, this.colorFactory, ColorItem.Orange, this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.CommentColor3, this.colorFactory, ColorItem.Red,    this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.CommentColor4, this.colorFactory, ColorItem.Lilac,  this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.CommentColor5, this.colorFactory, ColorItem.Purple, this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.CommentColor6, this.colorFactory, ColorItem.Blue,   this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.CommentColor7, this.colorFactory, ColorItem.Green,  this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.CommentColor8, this.colorFactory, ColorItem.Grey,   this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
		}

		private void UpdateButtonGeometryClose(ActiveButton button)
		{
			button.Center = this.PositionCloseButton;
		}

		private void UpdateButtonGeometryWidth(ActiveButton button)
		{
			button.Center = this.PositionWidthButton;
		}

		private void UpdateButtonGeometryAttachTo(ActiveButton button)
		{
			button.Center = this.PositionAttachToLinkButton;
		}

		private void UpdateButtonGeometryColor(ActiveButton button)
		{
			int rank = button.Element - ActiveElement.CommentColor1;

			button.Center = this.PositionColorButton (rank);
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

		private void UpdateButtonStateAttachTo(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Visible = button.State.Hilited || (this.IsHeaderHilite && !this.IsDragging);
		}

		private void UpdateButtonStateColor(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Selected = this.colorFactory.ColorItem == button.ColorItem;
			button.State.Visible = this.IsHeaderHilite && !this.IsDragging;
		}


		#region Serialize
		public override void Serialize(XElement xml)
		{
			base.Serialize (xml);

			xml.Add (new XAttribute ("Text", this.textLayoutComment.Text));
		}

		public override void Deserialize(XElement xml)
		{
			base.Deserialize (xml);

			this.textLayoutComment.Text = (string) xml.Attribute ("Text");
		}
		#endregion


		private TextLayout						textLayoutComment;
		private AbstractTextField				editingTextField;
	}
}
