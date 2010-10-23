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
			if (this.isDraggingMove || this.isDraggingWidth || this.isDraggingAttach)
			{
				return null;  // pas de tooltip
			}

			switch (element)
			{
				case ActiveElement.CommentAttachTo:
					return "D�place l'attache du commentaire";

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
			//	Met en �vidence la bo�te selon la position de la souris.
			//	Si la souris est dans cette bo�te, retourne true.
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
					this.Bounds = bounds;  // d�place le commentaire

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
			base.MouseDown (message, pos);

			if (this.hilitedElement == ActiveElement.CommentMove)
			{
				this.isDraggingMove = true;
				this.UpdateButtonsState ();
				this.draggingPos = pos;
				this.editor.LockObject(this);
			}

			if (this.hilitedElement == ActiveElement.CommentWidth)
			{
				this.isDraggingWidth = true;
				this.UpdateButtonsState ();
				this.editor.LockObject (this);
			}

			if (this.hilitedElement == ActiveElement.CommentAttachTo)
			{
				this.isDraggingAttach = true;
				this.UpdateButtonsState ();
				this.editor.LockObject (this);
			}
		}

		public override void MouseUp(Message message, Point pos)
		{
			//	Le bouton de la souris est rel�ch�.
			base.MouseUp (message, pos);

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
			else if (this.isDraggingAttach)
			{
				this.isDraggingAttach = false;
				this.UpdateButtonsState ();
				this.editor.LockObject (null);
				this.editor.UpdateAfterCommentChanged();
				this.editor.SetLocalDirty ();
			}
			else
			{
				if (this.hilitedElement == ActiveElement.CommentClose)
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

					this.editor.RemoveBalloon (this);
				}

				if (this.hilitedElement == ActiveElement.CommentColor1)
				{
					this.BackgroundColorItem = ColorItem.Yellow;
				}

				if (this.hilitedElement == ActiveElement.CommentColor2)
				{
					this.BackgroundColorItem = ColorItem.Orange;
				}

				if (this.hilitedElement == ActiveElement.CommentColor3)
				{
					this.BackgroundColorItem = ColorItem.Red;
				}

				if (this.hilitedElement == ActiveElement.CommentColor4)
				{
					this.BackgroundColorItem = ColorItem.Lilac;
				}

				if (this.hilitedElement == ActiveElement.CommentColor5)
				{
					this.BackgroundColorItem = ColorItem.Purple;
				}

				if (this.hilitedElement == ActiveElement.CommentColor6)
				{
					this.BackgroundColorItem = ColorItem.Blue;
				}

				if (this.hilitedElement == ActiveElement.CommentColor7)
				{
					this.BackgroundColorItem = ColorItem.Green;
				}

				if (this.hilitedElement == ActiveElement.CommentColor8)
				{
					this.BackgroundColorItem = ColorItem.DarkGrey;
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
				return ActiveElement.CommentMove;
			}

			//	Souris dans la bo�te ?
			if (this.bounds.Contains (pos))
			{
				return ActiveElement.CommentEdit;
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

			this.UpdateButtonsGeometry ();
		}


		public override void DrawBackground(Graphics graphics)
		{
			//	Dessine le fond de l'objet.
			base.DrawBackground (graphics);

			//	Dessine le texte.
			var rect = this.bounds;
			rect.Deflate (ObjectComment.textMargin);
			this.textLayoutComment.LayoutSize = rect.Size;
			this.textLayoutComment.Paint (rect.BottomLeft, graphics, Rectangle.MaxValue, this.colorFactory.GetColor (this.colorFactory.IsDarkColorMain ? 1:0), GlyphPaintStyle.Normal);
		}

		public override void DrawForeground(Graphics graphics)
		{
			//	Dessine le dessus de l'objet.
			this.DrawButtons (graphics);
		}

		private bool IsHeaderHilite
		{
			//	Indique si la souris est dans l'en-t�te.
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

		private bool IsDragging
		{
			get
			{
				return this.isDraggingMove || this.isDraggingWidth || this.isDraggingAttach || this.editor.IsEditing;
			}
		}


		#region Serialize
		public override XElement Serialize(string xmlNodeName)
		{
			return null;
		}

		public override void Deserialize(XElement xml)
		{
		}
		#endregion


		private TextLayout						textLayoutComment;
		private AbstractTextField				editingTextField;
	}
}
