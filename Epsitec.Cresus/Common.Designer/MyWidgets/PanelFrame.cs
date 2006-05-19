using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget de type 'groupe' avec un cadre permettant d'éditer les widgets enfants.
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

				case "AlignLeft":
					this.SelectAlign(-1, false);
					break;

				case "AlignCenterX":
					this.SelectAlign(0, false);
					break;

				case "AlignRight":
					this.SelectAlign(1, false);
					break;

				case "AlignTop":
					this.SelectAlign(1, true);
					break;

				case "AlignCenterY":
					this.SelectAlign(0, true);
					break;

				case "AlignBottom":
					this.SelectAlign(-1, true);
					break;

				case "AdjustWidth":
					this.SelectAdjust(false);
					break;

				case "AdjustHeight":
					this.SelectAdjust(true);
					break;

				case "AlignGrid":
					this.SelectAlignGrid();
					break;

				case "OrderUpAll":
					this.SelectOrder(10000);
					break;

				case "OrderDownAll":
					this.SelectOrder(-10000);
					break;

				case "OrderUpOne":
					this.SelectOrder(1);
					break;

				case "OrderDownOne":
					this.SelectOrder(-1);
					break;

			}
		}

		public void GetSelectionInfo(out int selected, out int count)
		{
			//	Donne des informations sur la sélection en cours.
			selected = this.selectedObjects.Count;
			count = this.panel.Children.Count;
		}


		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			System.Diagnostics.Debug.WriteLine(message.Type.ToString());

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

				case MessageType.KeyDown:
					this.ProcessKeyChanged(message.IsControlPressed, message.IsShiftPressed);
					break;

				case MessageType.KeyUp:
					this.ProcessKeyChanged(message.IsControlPressed, message.IsShiftPressed);
					break;
			}
		}

		#region ProcessMouse
		void ProcessMouseDown(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	La souris a été pressée.
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
			//	La souris a été déplacée.
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
			//	La souris a été relâchée.
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

		void ProcessKeyChanged(bool isControlPressed, bool isShiftPressed)
		{
			//	La souris a été relâchée.
			if (this.tool == "ToolSelect")
			{
				this.SelectKeyChanged(isControlPressed, isShiftPressed);
			}

			if (this.tool == "ToolGlobal")
			{
				this.GlobalKeyChanged(isControlPressed, isShiftPressed);
			}

			if (this.tool == "ToolEdit")
			{
				this.EditKeyChanged(isControlPressed, isShiftPressed);
			}

			if (this.tool == "ToolZoom")
			{
				this.ZoomKeyChanged(isControlPressed, isShiftPressed);
			}

			if (this.tool == "ToolHand")
			{
				this.HandKeyChanged(isControlPressed, isShiftPressed);
			}

			if (this.tool.StartsWith("Object"))
			{
				this.ObjectKeyChanged(isControlPressed, isShiftPressed);
			}
		}
		#endregion

		#region ProcessMouse select
		protected void SelectDown(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection ponctuelle, souris pressée.
			Widget obj = this.Detect(pos);  // objet visé par la souris

			this.startingPos = pos;
			this.dragging = false;
			this.rectangling = false;

			if (!isShiftPressed)  // touche Shift relâchée ?
			{
				if (obj != null && this.selectedObjects.Contains(obj))
				{
					this.dragging = true;
					return;
				}
				this.selectedObjects.Clear();
			}

			if (obj == null)
			{
				this.rectangling = true;
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
				this.dragging = true;
			}

			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection ponctuelle, souris déplacée.
			if (isShiftPressed)
			{
				this.ChangeMouseCursor(MouseCursorType.ArrowPlus);
			}
			else
			{
				this.ChangeMouseCursor(MouseCursorType.Arrow);
			}

			if (this.dragging)
			{
				Point move = pos-this.startingPos;
				this.startingPos = pos;
				this.MoveSelection(move);
				this.HiliteRectangle(Rectangle.Empty);
			}
			else if (this.rectangling)
			{
				this.SelectRectangle(new Rectangle(this.startingPos, pos));
			}
			else
			{
				Widget obj = this.Detect(pos);
				Rectangle rect = Rectangle.Empty;
				if (obj != null)
				{
					rect = obj.ActualBounds;
				}
				this.HiliteRectangle(rect);  // met en évidence l'objet survolé par la souris
			}
		}

		protected void SelectUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection ponctuelle, souris relâchée.
			this.dragging = false;

			if (this.rectangling)
			{
				this.SelectObjectsInRectangle(this.selectedRectangle);
				this.SelectRectangle(Rectangle.Empty);
				this.rectangling = false;
			}
		}

		protected void SelectKeyChanged(bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection ponctuelle, touche pressée ou relâchée.
			if (isShiftPressed)
			{
				this.ChangeMouseCursor(MouseCursorType.ArrowPlus);
			}
			else
			{
				this.ChangeMouseCursor(MouseCursorType.Arrow);
			}
		}
		#endregion

		#region ProcessMouse global
		protected void GlobalDown(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection rectangulaire, souris pressée.
			Widget obj = this.Detect(pos);  // objet visé par la souris

			this.startingPos = pos;
			this.dragging = false;
			this.rectangling = false;

			if (!isShiftPressed)  // touche Shift relâchée ?
			{
				if (obj != null && this.selectedObjects.Contains(obj))
				{
					this.dragging = true;
					return;
				}
				this.selectedObjects.Clear();
			}

			this.rectangling = true;

			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void GlobalMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection rectangulaire, souris déplacée.
			if (isShiftPressed)
			{
				this.ChangeMouseCursor(MouseCursorType.ArrowPlus);
			}
			else
			{
				this.ChangeMouseCursor(MouseCursorType.Global);
			}

			if (this.dragging)
			{
				Point move = pos-this.startingPos;
				this.startingPos = pos;
				this.MoveSelection(move);
				this.HiliteRectangle(Rectangle.Empty);
			}
			else if (this.rectangling)
			{
				this.SelectRectangle(new Rectangle(this.startingPos, pos));
			}
		}

		protected void GlobalUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection rectangulaire, souris relâchée.
			this.dragging = false;

			if (this.rectangling)
			{
				this.SelectObjectsInRectangle(this.selectedRectangle);
				this.SelectRectangle(Rectangle.Empty);
				this.rectangling = false;
			}
		}

		protected void GlobalKeyChanged(bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection rectangulaire, touche pressée ou relâchée.
			if (isShiftPressed)
			{
				this.ChangeMouseCursor(MouseCursorType.ArrowPlus);
			}
			else
			{
				this.ChangeMouseCursor(MouseCursorType.Global);
			}
		}
		#endregion

		#region ProcessMouse edit
		protected void EditDown(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Edition, souris pressée.
		}

		protected void EditMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Edition, souris déplacée.
			this.ChangeMouseCursor(MouseCursorType.Edit);
		}

		protected void EditUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Edition, souris relâchée.
		}

		protected void EditKeyChanged(bool isControlPressed, bool isShiftPressed)
		{
			//	Edition, touche pressée ou relâchée.
		}
		#endregion

		#region ProcessMouse zoom
		protected void ZoomDown(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Loupe, souris pressée.
		}

		protected void ZoomMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Loupe, souris déplacée.
			this.ChangeMouseCursor(MouseCursorType.Zoom);
		}

		protected void ZoomUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Loupe, souris relâchée.
		}

		protected void ZoomKeyChanged(bool isControlPressed, bool isShiftPressed)
		{
			//	Loupe, touche pressée ou relâchée.
		}
		#endregion

		#region ProcessMouse hand
		protected void HandDown(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Main, souris pressée.
		}

		protected void HandMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Main, souris déplacée.
			this.ChangeMouseCursor(MouseCursorType.Hand);
		}

		protected void HandUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Main, souris relâchée.
		}

		protected void HandKeyChanged(bool isControlPressed, bool isShiftPressed)
		{
			//	Main, touche pressée ou relâchée.
		}
		#endregion

		#region ProcessMouse object
		protected void ObjectDown(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, souris pressée.
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
			//	Dessin d'un objet, souris déplacée.
			this.ChangeMouseCursor(MouseCursorType.Pen);

			if (this.creatingObject != null)
			{
				this.creatingObject.Margins = new Margins(pos.X, 0, 0, pos.Y);
				this.Invalidate();  // TODO: faire mieux !
			}
		}

		protected void ObjectUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, souris relâchée.
			this.Select(this.creatingObject);
			this.creatingObject = null;
		}

		protected void ObjectKeyChanged(bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, touche pressée ou relâchée.
		}
		#endregion


		#region Selection
		protected Widget Detect(Point pos)
		{
			//	Détecte l'objet visé par la souris.
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
			//	Sélectionne un objet.
			this.selectedObjects.Clear();
			this.selectedObjects.Add(obj);
			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void DeselectAll()
		{
			//	Désélectionne tous les objets.
			if (this.selectedObjects.Count > 0)
			{
				this.selectedObjects.Clear();
				this.OnChildrenSelected();
				this.Invalidate();
			}
		}

		protected void SelectAll()
		{
			//	Sélectionne tous les objets.
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
			//	Inverse la sélection.
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

		protected void SelectObjectsInRectangle(Rectangle sel)
		{
			//	Sélectionne tous les objets entièrement inclus dans un rectangle.
			foreach (Widget obj in this.panel.Children)
			{
				if (sel.Contains(obj.ActualBounds))
				{
					this.selectedObjects.Add(obj);
				}
			}

			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void HiliteRectangle(Rectangle rect)
		{
			//	Détermine la zone à mettre en évidence.
			if (this.hilitedRectangle != rect)
			{
				this.Invalidate(this.hilitedRectangle);  // invalide l'ancienne zone
				this.hilitedRectangle = rect;
				this.Invalidate(this.hilitedRectangle);  // invalide la nouvelle zone
			}
		}

		protected void SelectRectangle(Rectangle rect)
		{
			//	Détermine la zone du rectangle de sélection.
			if (this.selectedRectangle != rect)
			{
				this.Invalidate(this.selectedRectangle);  // invalide l'ancienne zone
				this.selectedRectangle = rect;
				this.Invalidate(this.selectedRectangle);  // invalide la nouvelle zone
			}
		}
		#endregion

		#region Operations
		protected void DeleteSelection()
		{
			//	Supprime tous les objets sélectionnés.
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
			//	Duplique tous les objets sélectionnés.
			//	TODO:
		}

		protected void MoveSelection(Point move)
		{
			//	Déplace tous les objets sélectionnés.
			Rectangle toRepaint = Rectangle.Empty;

			foreach (Widget obj in this.selectedObjects)
			{
				toRepaint = Rectangle.Union(toRepaint, obj.ActualBounds);
				this.ObjectPosition(obj, this.ObjectPosition(obj)+move);
				toRepaint = Rectangle.Union(toRepaint, obj.ActualBounds);
			}

			toRepaint.Inflate(1);
			this.Invalidate(toRepaint);
			this.Invalidate();  // TODO: faire mieux !
		}

		protected void SelectAlign(int direction, bool isVertical)
		{
			//	Aligne tous les objets sélectionnés.
			Rectangle bounds = this.SelectBounds;

			if (isVertical)
			{
				if (direction < 0)  // en bas ?
				{
					foreach (Widget obj in this.selectedObjects)
					{
						this.ObjectPositionY(obj, bounds.Bottom);
					}
				}
				else if (direction > 0)  // en haut ?
				{
					foreach (Widget obj in this.selectedObjects)
					{
						this.ObjectPositionY(obj, bounds.Top-this.ObjectSizeY(obj));
					}
				}
				else  // centré verticalement ?
				{
					foreach (Widget obj in this.selectedObjects)
					{
						this.ObjectPositionY(obj, bounds.Center.Y-this.ObjectSizeY(obj)/2);
					}
				}
			}
			else
			{
				if (direction < 0)  // à gauche ?
				{
					foreach (Widget obj in this.selectedObjects)
					{
						this.ObjectPositionX(obj, bounds.Left);
					}
				}
				else if (direction > 0)  // à droite ?
				{
					foreach (Widget obj in this.selectedObjects)
					{
						this.ObjectPositionX(obj, bounds.Right-this.ObjectSizeX(obj));
					}
				}
				else  // centré horizontalement ?
				{
					foreach (Widget obj in this.selectedObjects)
					{
						this.ObjectPositionX(obj, bounds.Center.X-this.ObjectSizeX(obj)/2);
					}
				}
			}

			this.Invalidate();
		}

		protected void SelectAdjust(bool isVertical)
		{
			//	Ajuste les dimensions de tous les objets sélectionnés.
			Rectangle bounds = this.SelectBounds;

			if (isVertical)
			{
				foreach (Widget obj in this.selectedObjects)
				{
					this.ObjectPositionY(obj, bounds.Bottom);
					this.ObjectSizeY(obj, bounds.Height);
				}
			}
			else
			{
				foreach (Widget obj in this.selectedObjects)
				{
					this.ObjectPositionX(obj, bounds.Left);
					this.ObjectSizeX(obj, bounds.Width);
				}
			}

			this.Invalidate();
		}

		protected void SelectAlignGrid()
		{
			//	Aligne sur la grille tous les objets sélectionnés.
			//	TODO:
		}

		protected void SelectOrder(int direction)
		{
			//	Modifie l'ordre de tous les objets sélectionnés.
		}

		protected Rectangle SelectBounds
		{
			//	Retourne le rectangle englobant tous les objets sélectionnés.
			get
			{
				Rectangle bounds = Rectangle.Empty;

				foreach (Widget obj in this.selectedObjects)
				{
					bounds = Rectangle.Union(bounds, obj.ActualBounds);
				}

				return bounds;
			}
		}


		protected double ObjectPositionX(Widget obj)
		{
			//	Retourne l'origine gauche d'un objet.
			return obj.Margins.Left;
		}

		protected double ObjectPositionY(Widget obj)
		{
			//	Retourne l'origine inférieure d'un objet.
			return obj.Margins.Bottom;
		}

		protected Point ObjectPosition(Widget obj)
		{
			//	Retourne l'origine d'un objet.
			return new Point(obj.Margins.Left, obj.Margins.Bottom);
		}

		protected void ObjectPositionX(Widget obj, double x)
		{
			//	Déplace l'origine gauche d'un objet.
			x = System.Math.Max(x, 0);

			Margins margins = obj.Margins;
			margins.Left = x;
			obj.Margins = margins;
		}

		protected void ObjectPositionY(Widget obj, double y)
		{
			//	Déplace l'origine inférieure d'un objet.
			y = System.Math.Max(y, 0);

			Margins margins = obj.Margins;
			margins.Bottom = y;
			obj.Margins = margins;
		}

		protected void ObjectPosition(Widget obj, Point pos)
		{
			//	Déplace l'origine d'un objet.
			pos.X = System.Math.Max(pos.X, 0);
			pos.Y = System.Math.Max(pos.Y, 0);

			Margins margins = obj.Margins;
			margins.Left   = pos.X;
			margins.Bottom = pos.Y;
			obj.Margins = margins;
		}

		protected double ObjectSizeX(Widget obj)
		{
			//	Retourne la largeur d'un objet.
			return obj.ActualWidth;
		}

		protected double ObjectSizeY(Widget obj)
		{
			//	Retourne la hauteur d'un objet.
			return obj.ActualHeight;
		}

		protected Size ObjectSize(Widget obj)
		{
			//	Retourne les dimensions d'un objet.
			return obj.ActualSize;
		}

		protected void ObjectSizeX(Widget obj, double dx)
		{
			//	Modifie la largeur d'un objet.
			obj.PreferredWidth = dx;
		}

		protected void ObjectSizeY(Widget obj, double dy)
		{
			//	Modifie la hauteur d'un objet.
			obj.PreferredHeight = dy;
		}

		protected void ObjectSize(Widget obj, Size size)
		{
			//	Modifie les dimensions d'un objet.
			obj.PreferredSize = size;
		}
		#endregion


		#region Paint
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine le panneau.
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			//	Dessine les surfaces inutilisées.
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

			//	Dessine les objets sélectionnés.
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

			//	Dessine l'objet survolé.
			if (!this.hilitedRectangle.IsEmpty)
			{
				graphics.AddFilledRectangle(this.hilitedRectangle);
				graphics.RenderSolid(this.HiliteSurfaceColor);
			}

			//	Dessine le rectangle de sélection.
			if (!this.selectedRectangle.IsEmpty)
			{
				Rectangle sel = this.selectedRectangle;
				sel.Deflate(0.5);
				graphics.AddRectangle(sel);
				graphics.RenderSolid(this.HiliteOutlineColor);
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
			//	Couleur lorsqu'un objet est survolé par la souris.
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
				return Color.FromColor(adorner.ColorCaption, 0.8);
			}
		}

		protected Color HiliteSurfaceColor
		{
			//	Couleur lorsqu'un objet est survolé par la souris.
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
		protected Rectangle					selectedRectangle = Rectangle.Empty;
		protected Rectangle					hilitedRectangle = Rectangle.Empty;
		protected bool						rectangling;  // j'invente des mots si je veux !
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
