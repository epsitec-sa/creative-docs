using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget de type 'groupe' avec un cadre permettant d'�diter les widgets enfants.
	/// </summary>
	public class PanelFrame : AbstractGroup
	{
		protected enum MouseCursorType
		{
			Arrow,
			ArrowPlus,
			Global,
			Hand,
			Edit,
			Pen,
			Zoom,
		}


		public PanelFrame() : base()
		{
		}

		public PanelFrame(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		static PanelFrame()
		{
			Widgets.Helpers.VisualPropertyMetadata metadata = new Widgets.Helpers.VisualPropertyMetadata(ContentAlignment.TopLeft, Widgets.Helpers.VisualPropertyMetadataOptions.AffectsTextLayout);
			Widgets.Visual.ContentAlignmentProperty.OverrideMetadata(typeof(PanelFrame), metadata);
		}


		public UI.Panel					Panel
		{
			get
			{
				return this.panel;
			}

			set
			{
				this.panel = value;
			}
		}

		public string					Tool
		{
			get
			{
				return this.tool;
			}

			set
			{
				this.tool = value;
			}
		}

		public void DoCommand(string name)
		{
			switch (name)
			{
				case "PanelDelete":
					this.DeleteSelection();
					break;

				case "PanelDuplicate":
					this.DuplicateSelection();
					break;

				case "PanelDeselectAll":
					this.DeselectAll();
					break;

				case "PanelSelectAll":
					this.SelectAll();
					break;

				case "PanelSelectInvert":
					this.SelectInvert();
					break;
			}
		}


		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch (message.Type)
			{
				case MessageType.MouseDown:
					this.ProcessMouseDown(pos, message.IsRightButton, message.IsControlPressed, message.IsShiftPressed);
					message.Captured = true;
					message.Consumer = this;
					break;

				case MessageType.MouseMove:
					this.ProcessMouseMove(pos, message.IsRightButton, message.IsControlPressed, message.IsShiftPressed);
					message.Captured = true;
					message.Consumer = this;
					break;

				case MessageType.MouseUp:
					this.ProcessMouseUp(pos, message.IsRightButton, message.IsControlPressed, message.IsShiftPressed);
					message.Captured = true;
					message.Consumer = this;
					break;
			}
		}

		#region ProcessMouse
		void ProcessMouseDown(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	La souris a �t� press�e.
			if (this.tool == "ToolSelect")
			{
				this.SelectDown(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.tool == "ToolGlobal")
			{
				this.GlobalDown(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.tool == "ToolEdit")
			{
				this.EditDown(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.tool == "ToolZoom")
			{
				this.ZoomDown(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.tool == "ToolHand")
			{
				this.HandDown(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.tool.StartsWith("Object"))
			{
				this.ObjectDown(pos, isRightButton, isControlPressed, isShiftPressed);
			}
		}

		void ProcessMouseMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	La souris a �t� d�plac�e.
			if (this.tool == "ToolSelect")
			{
				this.SelectMove(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.tool == "ToolGlobal")
			{
				this.GlobalMove(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.tool == "ToolEdit")
			{
				this.EditMove(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.tool == "ToolZoom")
			{
				this.ZoomMove(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.tool == "ToolHand")
			{
				this.HandMove(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.tool.StartsWith("Object"))
			{
				this.ObjectMove(pos, isRightButton, isControlPressed, isShiftPressed);
			}
		}

		void ProcessMouseUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	La souris a �t� rel�ch�e.
			if (this.tool == "ToolSelect")
			{
				this.SelectUp(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.tool == "ToolGlobal")
			{
				this.GlobalUp(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.tool == "ToolEdit")
			{
				this.EditUp(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.tool == "ToolZoom")
			{
				this.ZoomUp(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.tool == "ToolHand")
			{
				this.HandUp(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.tool.StartsWith("Object"))
			{
				this.ObjectUp(pos, isRightButton, isControlPressed, isShiftPressed);
			}
		}
		#endregion

		#region Drawing select
		protected void SelectDown(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	S�lection ponctuelle, souris press�e.
			Widget obj = this.Detect(pos);  // objet vis� par la souris

			this.startingPos = pos;
			this.dragging = false;
			if (!isShiftPressed)
			{
				if (obj != null && this.selectedObjects.Contains(obj))
				{
					this.dragging = true;
					return;
				}
				this.selectedObjects.Clear();
			}

			if (obj != null)
			{
				if (this.selectedObjects.Contains(obj))
				{
					this.selectedObjects.Remove(obj);
				}
				else
				{
					this.selectedObjects.Add(obj);
				}
			}

			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	S�lection ponctuelle, souris d�plac�e.
			this.ChangeMouseCursor(MouseCursorType.Arrow);

			if (this.dragging)
			{
				Point move = pos-this.startingPos;
				this.startingPos = pos;
				this.MoveSelection(move);
				this.HiliteRectangle(Rectangle.Empty);
			}
			else
			{
				Widget obj = this.Detect(pos);
				Rectangle rect = Rectangle.Empty;
				if (obj != null)
				{
					rect = obj.ActualBounds;
				}
				this.HiliteRectangle(rect);
			}
		}

		protected void SelectUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	S�lection ponctuelle, souris rel�ch�e.
			this.dragging = false;
		}

		protected Widget Detect(Point pos)
		{
			//	D�tecte l'objet vis� par la souris.
			for (int i=this.panel.Children.Count-1; i>=0; i--)
			{
				Widget widget = this.panel.Children[i] as Widget;
				if (widget.ActualBounds.Contains(pos))
				{
					return widget;
				}
			}
			return null;
		}

		protected void Select(Widget obj)
		{
			//	S�lectionne un objet.
			this.selectedObjects.Clear();
			this.selectedObjects.Add(obj);
			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void DeselectAll()
		{
			//	D�s�lectionne tous les objets.
			if (this.selectedObjects.Count > 0)
			{
				this.selectedObjects.Clear();
				this.OnChildrenSelected();
				this.Invalidate();
			}
		}

		protected void SelectAll()
		{
			//	S�lectionne tous les objets.
			this.selectedObjects.Clear();

			foreach (Widget obj in this.panel.Children)
			{
				this.selectedObjects.Add(obj);
			}

			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectInvert()
		{
			//	Inverse la s�lection.
			List<Widget> list = new List<Widget>();

			foreach (Widget obj in this.panel.Children)
			{
				if (!this.selectedObjects.Contains(obj))
				{
					list.Add(obj);
				}
			}

			this.selectedObjects = list;

			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void DeleteSelection()
		{
			//	Supprime tous les objets s�lectionn�s.
			foreach (Widget obj in this.selectedObjects)
			{
				this.panel.Children.Remove(obj);
				obj.Dispose();
			}

			this.selectedObjects.Clear();

			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void DuplicateSelection()
		{
			//	Duplique tous les objets s�lectionn�s.
			//	TODO:
		}

		protected void MoveSelection(Point move)
		{
			//	D�place tous les objets s�lectionn�s.
			Rectangle toRepaint = Rectangle.Empty;

			foreach (Widget obj in this.selectedObjects)
			{
				toRepaint = Rectangle.Union(toRepaint, obj.ActualBounds);

				Margins margin = obj.Margins;
				margin.Left += move.X;
				margin.Bottom += move.Y;
				obj.Margins = margin;

				toRepaint = Rectangle.Union(toRepaint, obj.ActualBounds);
			}

			toRepaint.Inflate(1);
			this.Invalidate(toRepaint);
			this.Invalidate();  // TODO: faire mieux !
		}

		protected void HiliteRectangle(Rectangle rect)
		{
			//	D�termine la zone � mettre en �vidence.
			if (this.hilitedRectangle != rect)
			{
				this.Invalidate(this.hilitedRectangle);  // invalide l'ancienne zone
				this.hilitedRectangle = rect;
				this.Invalidate(this.hilitedRectangle);  // invalide la nouvelle zone
			}
		}
		#endregion

		#region Drawing global
		protected void GlobalDown(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	S�lection rectangulaire, souris press�e.
		}

		protected void GlobalMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	S�lection rectangulaire, souris d�plac�e.
			this.ChangeMouseCursor(MouseCursorType.Global);
		}

		protected void GlobalUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	S�lection rectangulaire, souris rel�ch�e.
		}
		#endregion

		#region Drawing edit
		protected void EditDown(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Edition, souris press�e.
		}

		protected void EditMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Edition, souris d�plac�e.
			this.ChangeMouseCursor(MouseCursorType.Edit);
		}

		protected void EditUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Edition, souris rel�ch�e.
		}
		#endregion

		#region Drawing zoom
		protected void ZoomDown(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Loupe, souris press�e.
		}

		protected void ZoomMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Loupe, souris d�plac�e.
			this.ChangeMouseCursor(MouseCursorType.Zoom);
		}

		protected void ZoomUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Loupe, souris rel�ch�e.
		}
		#endregion

		#region Drawing hand
		protected void HandDown(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Main, souris press�e.
		}

		protected void HandMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Main, souris d�plac�e.
			this.ChangeMouseCursor(MouseCursorType.Hand);
		}

		protected void HandUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Main, souris rel�ch�e.
		}
		#endregion

		#region Drawing object
		protected void ObjectDown(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, souris press�e.
			if (this.tool == "ObjectLine")
			{
				this.creatingObject = new Separator(this.panel);
				this.creatingObject.PreferredHeight = 1;
			}

			if (this.tool == "ObjectButton")
			{
				this.creatingObject = new Button(this.panel);
				this.creatingObject.Text = "Button";
			}

			if (this.tool == "ObjectText")
			{
				this.creatingObject = new TextField(this.panel);
				this.creatingObject.Text = "TextField";
			}

			this.creatingObject.Anchor = AnchorStyles.BottomLeft;
			this.creatingObject.Margins = new Margins(pos.X, 0, 0, pos.Y);

			this.OnChildrenAdded();
		}

		protected void ObjectMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, souris d�plac�e.
			this.ChangeMouseCursor(MouseCursorType.Pen);

			if (this.creatingObject != null)
			{
				this.creatingObject.Margins = new Margins(pos.X, 0, 0, pos.Y);
				this.Invalidate();  // TODO: faire mieux !
			}
		}

		protected void ObjectUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, souris rel�ch�e.
			this.Select(this.creatingObject);
			this.creatingObject = null;
		}
		#endregion


		#region Paint
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine le panneau.
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			//	Dessine les surfaces inutilis�es.
			Rectangle bounds = this.RealBounds;
			Rectangle box = this.Client.Bounds;

			if (bounds.Top < box.Top)
			{
				Rectangle part = new Rectangle(box.Left, bounds.Top, box.Width, box.Top-bounds.Top);
				graphics.AddFilledRectangle(part);
				graphics.RenderSolid(Color.FromAlphaRgb(0.2, 0.5, 0.5, 0.5));
			}

			if (bounds.Right < box.Right)
			{
				Rectangle part = new Rectangle(bounds.Right, box.Bottom, box.Right-bounds.Right, bounds.Height);
				graphics.AddFilledRectangle(part);
				graphics.RenderSolid(Color.FromAlphaRgb(0.2, 0.5, 0.5, 0.5));
			}

			//	Dessine les objets s�lectionn�s.
			if (this.selectedObjects.Count > 0)
			{
				foreach ( Widget obj in this.selectedObjects)
				{
					Rectangle rect = obj.ActualBounds;
					rect.Deflate(0.5);

					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.HiliteSurfaceColor);

					graphics.LineWidth = 3;
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.HiliteOutlineColor);
					graphics.LineWidth = 1;
				}
			}

			//	Dessine l'objet survol�.
			if (!this.hilitedRectangle.IsEmpty)
			{
				graphics.AddFilledRectangle(this.hilitedRectangle);
				graphics.RenderSolid(this.HiliteSurfaceColor);
			}
		}

		protected Rectangle RealBounds
		{
			get
			{
				Rectangle bounds = Rectangle.Empty;

				foreach (Widget obj in this.panel.Children)
				{
					bounds = Rectangle.Union(bounds, obj.ActualBounds);
				}

				bounds.Left = 0;
				bounds.Bottom = 0;

				return bounds;
			}
		}

		protected Color HiliteOutlineColor
		{
			//	Couleur lorsqu'un objet est survol� par la souris.
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
				return Color.FromColor(adorner.ColorCaption, 0.8);
			}
		}

		protected Color HiliteSurfaceColor
		{
			//	Couleur lorsqu'un objet est survol� par la souris.
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
				return Color.FromColor(adorner.ColorCaption, 0.4);
			}
		}
		#endregion


		#region MouseCursor
		protected void ChangeMouseCursor(MouseCursorType cursor)
		{
			//	Change le sprite de la souris.
			switch (cursor)
			{
				case MouseCursorType.Arrow:
					this.MouseCursorImage(ref this.mouseCursorArrow, Misc.Icon("CursorArrow"));
					break;

				case MouseCursorType.ArrowPlus:
					this.MouseCursorImage(ref this.mouseCursorArrowPlus, Misc.Icon("CursorArrowPlus"));
					break;

				case MouseCursorType.Global:
					this.MouseCursorImage(ref this.mouseCursorGlobal, Misc.Icon("CursorGlobal"));
					break;

				case MouseCursorType.Edit:
					this.MouseCursorImage(ref this.mouseCursorEdit, Misc.Icon("CursorEdit"));
					break;

				case MouseCursorType.Hand:
					this.MouseCursorImage(ref this.mouseCursorHand, Misc.Icon("CursorHand"));
					break;

				case MouseCursorType.Pen:
					this.MouseCursorImage(ref this.mouseCursorPen, Misc.Icon("CursorPen"));
					break;

				case MouseCursorType.Zoom:
					this.MouseCursorImage(ref this.mouseCursorZoom, Misc.Icon("CursorZoom"));
					break;

				default:
					this.MouseCursor = MouseCursor.AsArrow;
					break;
			}

			this.Window.MouseCursor = this.MouseCursor;
		}

		protected void MouseCursorImage(ref Image image, string name)
		{
			//	Choix du sprite de la souris.
			if (image == null)
			{
				image = Support.Resources.DefaultManager.GetImage(name);
			}

			this.MouseCursor = MouseCursor.FromImage(image);
		}
		#endregion

		#region Events
		protected virtual void OnChildrenAdded()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler("ChildrenAdded");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event EventHandler ChildrenAdded
		{
			add
			{
				this.AddUserEventHandler("ChildrenAdded", value);
			}
			remove
			{
				this.RemoveUserEventHandler("ChildrenAdded", value);
			}
		}

		protected virtual void OnChildrenSelected()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler("ChildrenSelected");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event EventHandler ChildrenSelected
		{
			add
			{
				this.AddUserEventHandler("ChildrenSelected", value);
			}
			remove
			{
				this.RemoveUserEventHandler("ChildrenSelected", value);
			}
		}
		#endregion


		protected UI.Panel					panel;
		protected string					tool = "ToolSelect";
		protected Widget					creatingObject;
		protected List<Widget>				selectedObjects = new List<Widget>();
		protected Rectangle					hilitedRectangle = Rectangle.Empty;
		protected bool						dragging;
		protected Point						startingPos;

		protected Image						mouseCursorArrow = null;
		protected Image						mouseCursorArrowPlus = null;
		protected Image						mouseCursorGlobal = null;
		protected Image						mouseCursorEdit = null;
		protected Image						mouseCursorPen = null;
		protected Image						mouseCursorZoom = null;
		protected Image						mouseCursorHand = null;
	}
}
