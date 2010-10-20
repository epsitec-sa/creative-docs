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
	public class ObjectCartridge : AbstractObject
	{
		public ObjectCartridge(Editor editor, AbstractEntity entity)
			: base (editor, entity)
		{
			System.Diagnostics.Debug.Assert (this.entity != null);

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
			this.title.Text = Misc.Bold (this.Entity.WorkflowName.ToString ());
		}

		private void UpdateSubtitle()
		{
			this.subtitle.Text = this.Entity.WorkflowDescription.ToString ();
		}


		public override void MouseUp(Message message, Point pos)
		{
			//	Le bouton de la souris est relâché.
			base.MouseUp (message, pos);

			if (this.hilitedElement == ActiveElement.CartridgeEditName ||
				this.hilitedElement == ActiveElement.CartridgeEditDescription)
			{
				this.StartEdition (this.hilitedElement);
				return;
			}
		}

		public override ActiveElement MouseDetectBackground(Point pos)
		{
			//	Détecte l'élément actif visé par la souris.
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

		public override void DrawBackground(Graphics graphics)
		{
			//	Dessine le fond de l'objet.
			Rectangle rect;
			Color color = Color.FromBrightness (0);

			rect = this.RectangleTitle;
			rect.Deflate (4, 2);
			this.title.LayoutSize = rect.Size;
			this.title.Paint (rect.BottomLeft, graphics, Rectangle.MaxValue, color, GlyphPaintStyle.Normal);

			rect = this.RectangleSubtitle;
			rect.Deflate (4, 2);
			this.subtitle.LayoutSize = rect.Size;
			this.subtitle.Paint (rect.BottomLeft, graphics, Rectangle.MaxValue, color, GlyphPaintStyle.Normal);

			rect = new Rectangle (Point.Zero, this.editor.AreaSize);
			rect.Deflate (0.5);
			graphics.AddRectangle (rect);

			rect = this.RectangleTitle;
			rect.Inflate (0.5);
			graphics.AddRectangle (rect);

			rect = this.RectangleSubtitle;
			rect.Inflate (0.5);
			graphics.AddRectangle (rect);
			
			graphics.RenderSolid ();
		}

		private Rectangle RectangleTitle
		{
			get
			{
				var bounds = this.CartridgeBounds;
				return new Rectangle (bounds.Left, bounds.Top-ObjectCartridge.titleHeight, bounds.Width, ObjectCartridge.titleHeight);
			}
		}

		private Rectangle RectangleSubtitle
		{
			get
			{
				var bounds = this.CartridgeBounds;
				return new Rectangle (bounds.Left, bounds.Bottom, bounds.Width, ObjectCartridge.frameSize.Height-ObjectCartridge.titleHeight-1);
			}
		}

		private Rectangle CartridgeBounds
		{
			get
			{
				return new Rectangle (this.editor.AreaSize.Width-ObjectCartridge.frameSize.Width-1, 1, ObjectCartridge.frameSize.Width, ObjectCartridge.frameSize.Height);
			}
		}

		public WorkflowDefinitionEntity Entity
		{
			get
			{
				return this.entity as WorkflowDefinitionEntity;
			}
		}


		private static readonly Size			frameSize = new Size (250, 70);
		private static readonly double			titleHeight = 20;

		private TextLayout						title;
		private TextLayout						subtitle;

		private ActiveElement					editingElement;
		private AbstractTextField				editingTextField;
	}
}
