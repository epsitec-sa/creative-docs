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

		public PanelsContext			Context
		{
			get
			{
				return this.context;
			}

			set
			{
				this.context = value;
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

				case "PanelShowGrid":
					this.context.ShowGrid = !this.context.ShowGrid;
					this.Invalidate();
					this.OnUpdateCommands();
					break;

				case "PanelShowZOrder":
					this.context.ShowZOrder = !this.context.ShowZOrder;
					this.Invalidate();
					this.OnUpdateCommands();
					break;

				case "PanelShowTabIndex":
					this.context.ShowTabIndex = !this.context.ShowTabIndex;
					this.Invalidate();
					this.OnUpdateCommands();
					break;

				case "PanelShowExpand":
					this.context.ShowExpand = !this.context.ShowExpand;
					this.Invalidate();
					this.OnUpdateCommands();
					break;

				case "PanelShowAnchor":
					this.context.ShowAnchor = !this.context.ShowAnchor;
					this.Invalidate();
					this.OnUpdateCommands();
					break;

				case "PanelShowConstrain":
					this.context.ShowConstrain = !this.context.ShowConstrain;
					this.Invalidate();
					this.OnUpdateCommands();
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
					this.SelectOrder(-10000);
					break;

				case "OrderDownAll":
					this.SelectOrder(10000);
					break;

				case "OrderUpOne":
					this.SelectOrder(-1);
					break;

				case "OrderDownOne":
					this.SelectOrder(1);
					break;

				case "TabIndexClear":
					this.SelectTabIndex(0);
					break;

				case "TabIndexRenum":
					this.SelectTabIndexRenum();
					break;

				case "TabIndexFirst":
					this.SelectTabIndex(-10000);
					break;

				case "TabIndexPrev":
					this.SelectTabIndex(-1);
					break;

				case "TabIndexNext":
					this.SelectTabIndex(1);
					break;

				case "TabIndexLast":
					this.SelectTabIndex(10000);
					break;

			}
		}

		public void GetSelectionInfo(out int selected, out int count)
		{
			//	Donne des informations sur la s�lection en cours.
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
			//	La souris a �t� press�e.
			if (this.context.Tool == "ToolSelect")
			{
				this.SelectDown(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.context.Tool == "ToolGlobal")
			{
				this.GlobalDown(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.context.Tool == "ToolEdit")
			{
				this.EditDown(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.context.Tool == "ToolZoom")
			{
				this.ZoomDown(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.context.Tool == "ToolHand")
			{
				this.HandDown(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.context.Tool.StartsWith("Object"))
			{
				this.ObjectDown(pos, isRightButton, isControlPressed, isShiftPressed);
			}
		}

		void ProcessMouseMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	La souris a �t� d�plac�e.
			if (this.context.Tool == "ToolSelect")
			{
				this.SelectMove(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.context.Tool == "ToolGlobal")
			{
				this.GlobalMove(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.context.Tool == "ToolEdit")
			{
				this.EditMove(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.context.Tool == "ToolZoom")
			{
				this.ZoomMove(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.context.Tool == "ToolHand")
			{
				this.HandMove(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.context.Tool.StartsWith("Object"))
			{
				this.ObjectMove(pos, isRightButton, isControlPressed, isShiftPressed);
			}
		}

		void ProcessMouseUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	La souris a �t� rel�ch�e.
			if (this.context.Tool == "ToolSelect")
			{
				this.SelectUp(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.context.Tool == "ToolGlobal")
			{
				this.GlobalUp(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.context.Tool == "ToolEdit")
			{
				this.EditUp(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.context.Tool == "ToolZoom")
			{
				this.ZoomUp(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.context.Tool == "ToolHand")
			{
				this.HandUp(pos, isRightButton, isControlPressed, isShiftPressed);
			}

			if (this.context.Tool.StartsWith("Object"))
			{
				this.ObjectUp(pos, isRightButton, isControlPressed, isShiftPressed);
			}
		}

		void ProcessKeyChanged(bool isControlPressed, bool isShiftPressed)
		{
			//	La souris a �t� rel�ch�e.
			if (this.context.Tool == "ToolSelect")
			{
				this.SelectKeyChanged(isControlPressed, isShiftPressed);
			}

			if (this.context.Tool == "ToolGlobal")
			{
				this.GlobalKeyChanged(isControlPressed, isShiftPressed);
			}

			if (this.context.Tool == "ToolEdit")
			{
				this.EditKeyChanged(isControlPressed, isShiftPressed);
			}

			if (this.context.Tool == "ToolZoom")
			{
				this.ZoomKeyChanged(isControlPressed, isShiftPressed);
			}

			if (this.context.Tool == "ToolHand")
			{
				this.HandKeyChanged(isControlPressed, isShiftPressed);
			}

			if (this.context.Tool.StartsWith("Object"))
			{
				this.ObjectKeyChanged(isControlPressed, isShiftPressed);
			}
		}
		#endregion

		#region ProcessMouse select
		protected void SelectDown(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	S�lection ponctuelle, souris press�e.

			Widget obj = this.Detect(pos);  // objet vis� par la souris

			this.startingPos = pos;
			this.dragging = false;
			this.rectangling = false;

			if (!isShiftPressed)  // touche Shift rel�ch�e ?
			{
				if (obj != null && this.selectedObjects.Contains(obj))
				{
					this.dragging = true;
					this.ConstrainStart();
					this.ConstrainActivate(this.SelectBounds);
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
				this.ConstrainStart();
				this.ConstrainActivate(this.SelectBounds);
			}

			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	S�lection ponctuelle, souris d�plac�e.
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
				Rectangle bounds = this.SelectBounds;
				Point move = pos-this.startingPos;
				Point corr = this.CorrectionSelection(bounds, move);
				this.startingPos = pos+corr;
				this.MoveSelection(move+corr);
				
				this.HiliteRectangle(Rectangle.Empty);

				bounds.Offset(move+corr);
				this.ConstrainActivate(bounds);
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
				this.HiliteRectangle(rect);  // met en �vidence l'objet survol� par la souris
			}
		}

		protected void SelectUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	S�lection ponctuelle, souris rel�ch�e.
			this.dragging = false;
			this.ConstrainEnd();

			if (this.rectangling)
			{
				this.SelectObjectsInRectangle(this.selectedRectangle);
				this.SelectRectangle(Rectangle.Empty);
				this.rectangling = false;
			}
		}

		protected void SelectKeyChanged(bool isControlPressed, bool isShiftPressed)
		{
			//	S�lection ponctuelle, touche press�e ou rel�ch�e.
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
			//	S�lection rectangulaire, souris press�e.
			Widget obj = this.Detect(pos);  // objet vis� par la souris

			this.startingPos = pos;
			this.dragging = false;
			this.rectangling = false;

			if (!isShiftPressed)  // touche Shift rel�ch�e ?
			{
				if (obj != null && this.selectedObjects.Contains(obj))
				{
					this.dragging = true;
					this.ConstrainStart();
					this.ConstrainActivate(this.SelectBounds);
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
			//	S�lection rectangulaire, souris d�plac�e.
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
				Rectangle bounds = this.SelectBounds;
				Point move = pos-this.startingPos;
				Point corr = this.CorrectionSelection(bounds, move);
				this.startingPos = pos+corr;
				this.MoveSelection(move+corr);

				this.HiliteRectangle(Rectangle.Empty);

				bounds.Offset(move+corr);
				this.ConstrainActivate(bounds);
			}
			else if (this.rectangling)
			{
				this.SelectRectangle(new Rectangle(this.startingPos, pos));
			}
		}

		protected void GlobalUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	S�lection rectangulaire, souris rel�ch�e.
			this.dragging = false;
			this.ConstrainEnd();

			if (this.rectangling)
			{
				this.SelectObjectsInRectangle(this.selectedRectangle);
				this.SelectRectangle(Rectangle.Empty);
				this.rectangling = false;
			}
		}

		protected void GlobalKeyChanged(bool isControlPressed, bool isShiftPressed)
		{
			//	S�lection rectangulaire, touche press�e ou rel�ch�e.
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

		protected void EditKeyChanged(bool isControlPressed, bool isShiftPressed)
		{
			//	Edition, touche press�e ou rel�ch�e.
		}
		#endregion

		#region ProcessMouse zoom
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

		protected void ZoomKeyChanged(bool isControlPressed, bool isShiftPressed)
		{
			//	Loupe, touche press�e ou rel�ch�e.
		}
		#endregion

		#region ProcessMouse hand
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

		protected void HandKeyChanged(bool isControlPressed, bool isShiftPressed)
		{
			//	Main, touche press�e ou rel�ch�e.
		}
		#endregion

		#region ProcessMouse object
		protected void ObjectDown(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, souris press�e.
			this.ConstrainStart();

			if (this.context.Tool == "ObjectLine")
			{
				this.creatingObject = new Separator(this.panel);
				this.creatingObject.PreferredHeight = 1;
			}

			if (this.context.Tool == "ObjectButton")
			{
				this.creatingObject = new Button(this.panel);
				this.creatingObject.Text = "Button";
			}

			if (this.context.Tool == "ObjectText")
			{
				this.creatingObject = new TextField(this.panel);
				this.creatingObject.Text = "TextField";
			}

			if (this.context.Tool == "ObjectGroup")
			{
				this.creatingObject = new GroupBox(this.panel);
				this.creatingObject.Text = "Group";
				this.creatingObject.PreferredSize = new Size(200, 100);
			}

			this.creatingObject.Anchor = AnchorStyles.BottomLeft;
			this.ObjectPosition(this.creatingObject, pos);
			this.creatingObject.TabNavigation = TabNavigationMode.Passive;

			Rectangle bounds = new Rectangle(pos, this.creatingObject.PreferredSize);
			this.ConstrainActivate(bounds);

			this.OnChildrenAdded();
		}

		protected void ObjectMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, souris d�plac�e.
			this.ChangeMouseCursor(MouseCursorType.Pen);

			if (this.creatingObject != null)
			{
				Rectangle bounds = new Rectangle(pos, this.creatingObject.PreferredSize);
				Rectangle adjust = this.ConstrainSnap(bounds);
				Point corr = adjust.BottomLeft - bounds.BottomLeft;

				this.ObjectPosition(this.creatingObject, pos+corr);
				this.Invalidate();

				bounds.Offset(corr);
				this.ConstrainActivate(bounds);
			}
		}

		protected void ObjectUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, souris rel�ch�e.
			this.ConstrainEnd();

			this.SelectOneObject(this.creatingObject);
			this.creatingObject = null;
		}

		protected void ObjectKeyChanged(bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, touche press�e ou rel�ch�e.
		}
		#endregion


		#region Selection
		protected Widget Detect(Point pos)
		{
			//	D�tecte l'objet vis� par la souris, avec priorit� au dernier objet
			//	dessin� (donc plac� dessus).
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

		protected void SelectOneObject(Widget obj)
		{
			//	S�lectionne un objet.
			this.selectedObjects.Clear();
			this.selectedObjects.Add(obj);
			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectObjectsInRectangle(Rectangle sel)
		{
			//	S�lectionne tous les objets enti�rement inclus dans un rectangle.
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
			//	D�termine la zone � mettre en �vidence lors d'un survol.
			if (this.hilitedRectangle != rect)
			{
				this.Invalidate(this.hilitedRectangle);  // invalide l'ancienne zone
				this.hilitedRectangle = rect;
				this.Invalidate(this.hilitedRectangle);  // invalide la nouvelle zone
			}
		}

		protected void SelectRectangle(Rectangle rect)
		{
			//	D�termine la zone du rectangle de s�lection.
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

		protected Point CorrectionSelection(Rectangle bounds, Point move)
		{
			//	Calcule la correction a apporter au d�placement pour tenir compte des contraintes.
			if (this.constrainList.Count == 0)
			{
				return Point.Zero;
			}

			Rectangle moved = bounds;
			moved.Offset(move);

			Rectangle snaped = this.ConstrainSnap(moved);

			return snaped.BottomLeft - moved.BottomLeft;
		}

		protected void MoveSelection(Point move)
		{
			//	D�place tous les objets s�lectionn�s.
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
			//	Aligne tous les objets s�lectionn�s.
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
				else  // centr� verticalement ?
				{
					foreach (Widget obj in this.selectedObjects)
					{
						this.ObjectPositionY(obj, bounds.Center.Y-this.ObjectSizeY(obj)/2);
					}
				}
			}
			else
			{
				if (direction < 0)  // � gauche ?
				{
					foreach (Widget obj in this.selectedObjects)
					{
						this.ObjectPositionX(obj, bounds.Left);
					}
				}
				else if (direction > 0)  // � droite ?
				{
					foreach (Widget obj in this.selectedObjects)
					{
						this.ObjectPositionX(obj, bounds.Right-this.ObjectSizeX(obj));
					}
				}
				else  // centr� horizontalement ?
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
			//	Ajuste les dimensions de tous les objets s�lectionn�s.
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
			//	Aligne sur la grille tous les objets s�lectionn�s.
			//	TODO:
		}

		protected void SelectOrder(int direction)
		{
			//	Modifie l'ordre de tous les objets s�lectionn�s.
			foreach (Widget obj in this.selectedObjects)
			{
				obj.ZOrder += direction;
			}

			this.Invalidate();
			this.context.ShowZOrder = true;
			this.OnUpdateCommands();
		}

		protected void SelectTabIndexRenum()
		{
			//	Renum�rote toutes les touches Tab.
			//	TODO: tenir compte de la position des objets.
			if (this.selectedObjects.Count == 0)
			{
				int index = 0;
				foreach (Widget obj in this.panel.Children)
				{
					obj.TabIndex = index++;
					obj.TabNavigation = TabNavigationMode.ActivateOnTab;
				}
			}
			else
			{
				int index = 0;
				foreach (Widget obj in this.selectedObjects)
				{
					obj.TabIndex = index++;
					obj.TabNavigation = TabNavigationMode.ActivateOnTab;
				}
			}

			this.Invalidate();
			this.context.ShowTabIndex = true;
			this.OnUpdateCommands();
		}

		protected void SelectTabIndex(int direction)
		{
			//	Modifie l'ordre pour la touche Tab de tous les objets s�lectionn�s.
			foreach (Widget obj in this.selectedObjects)
			{
				if (direction == 0)
				{
					obj.TabNavigation = TabNavigationMode.Passive;
				}
				else
				{
					int oldIndex = obj.TabIndex;
					int index = obj.TabIndex + direction;
					index = System.Math.Max(index, 0);
					index = System.Math.Min(index, this.panel.Children.Count-1);
					this.SelectTabIndex(index, oldIndex);
					obj.TabIndex = index;
					obj.TabNavigation = TabNavigationMode.ActivateOnTab;
				}
			}

			this.Invalidate();
			this.context.ShowTabIndex = true;
			this.OnUpdateCommands();
		}

		protected void SelectTabIndex(int oldIndex, int newIndex)
		{
			foreach (Widget obj in this.panel.Children)
			{
				if (this.ObjectTabActive(obj) && obj.TabIndex == oldIndex)
				{
					obj.TabIndex = newIndex;
					obj.TabNavigation = TabNavigationMode.ActivateOnTab;
				}
			}
		}

		protected Rectangle SelectBounds
		{
			//	Retourne le rectangle englobant tous les objets s�lectionn�s.
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
			//	Retourne l'origine inf�rieure d'un objet.
			return obj.Margins.Bottom;
		}

		protected Point ObjectPosition(Widget obj)
		{
			//	Retourne l'origine d'un objet.
			return new Point(obj.Margins.Left, obj.Margins.Bottom);
		}

		protected void ObjectPositionX(Widget obj, double x)
		{
			//	D�place l'origine gauche d'un objet.
			x = System.Math.Max(x, 0);

			Margins margins = obj.Margins;
			margins.Left = x;
			obj.Margins = margins;
		}

		protected void ObjectPositionY(Widget obj, double y)
		{
			//	D�place l'origine inf�rieure d'un objet.
			y = System.Math.Max(y, 0);

			Margins margins = obj.Margins;
			margins.Bottom = y;
			obj.Margins = margins;
		}

		protected void ObjectPosition(Widget obj, Point pos)
		{
			//	D�place l'origine d'un objet.
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

		protected bool ObjectAnchorLeft(Widget obj)
		{
			//	Indique si l'objet est ancr� � gauche.
			return (obj.Anchor & AnchorStyles.Left) != 0;
		}

		protected bool ObjectAnchorRight(Widget obj)
		{
			//	Indique si l'objet est ancr� � gauche.
			return (obj.Anchor & AnchorStyles.Right) != 0;
		}

		protected bool ObjectAnchorBottom(Widget obj)
		{
			//	Indique si l'objet est ancr� � gauche.
			return (obj.Anchor & AnchorStyles.Bottom) != 0;
		}

		protected bool ObjectAnchorTop(Widget obj)
		{
			//	Indique si l'objet est ancr� � gauche.
			return (obj.Anchor & AnchorStyles.Top) != 0;
		}

		protected bool ObjectTabActive(Widget obj)
		{
			//	Indique si l'objet est ancr� � gauche.
			return (obj.TabNavigation & TabNavigationMode.ActivateOnTab) != 0;
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
				graphics.RenderSolid(this.colorOutsurface);
			}

			if (bounds.Right < box.Right)
			{
				Rectangle part = new Rectangle(bounds.Right, box.Bottom, box.Right-bounds.Right, bounds.Height);
				graphics.AddFilledRectangle(part);
				graphics.RenderSolid(this.colorOutsurface);
			}

			//	Dessine la grille magn�tique
			if (this.context.ShowGrid)
			{
				double step = this.context.GridStep;
				int hilite = 0;
				for (double x=step+0.5; x<bounds.Width; x+=step)
				{
					graphics.AddLine(x, bounds.Bottom, x, bounds.Top);
					graphics.RenderSolid(((++hilite)%10 == 0) ? this.colorGrid1 : this.colorGrid2);
				}
				hilite = 0;
				for (double y=step+0.5; y<bounds.Height; y+=step)
				{
					graphics.AddLine(bounds.Left, y, bounds.Right, y);
					graphics.RenderSolid(((++hilite)%10 == 0) ? this.colorGrid1 : this.colorGrid2);
				}
			}

			//	Dessine les objets s�lectionn�s.
			if (this.selectedObjects.Count > 0)
			{
				foreach (Widget obj in this.selectedObjects)
				{
					Rectangle rect = obj.ActualBounds;
					rect.Deflate(0.5);

					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(PanelFrame.HiliteSurfaceColor);

					graphics.LineWidth = 3;
					graphics.AddRectangle(rect);
					graphics.RenderSolid(PanelFrame.HiliteOutlineColor);
					graphics.LineWidth = 1;
				}
			}

			//	Dessine les ancrages.
			if (this.context.ShowAnchor && this.selectedObjects.Count > 0)
			{
				foreach (Widget obj in this.selectedObjects)
				{
					Rectangle rect = obj.ActualBounds;
					Point p1, p2;

					if (this.ObjectAnchorLeft(obj))
					{
						p1 = new Point(bounds.Left, rect.Center.Y);
						p2 = new Point(rect.Left, rect.Center.Y);
						this.DrawAnchor(graphics, p1, p2);
					}

					if (this.ObjectAnchorRight(obj))
					{
						p1 = new Point(bounds.Right, rect.Center.Y);
						p2 = new Point(rect.Right, rect.Center.Y);
						this.DrawAnchor(graphics, p1, p2);
					}

					if (this.ObjectAnchorBottom(obj))
					{
						p1 = new Point(rect.Center.X, bounds.Bottom);
						p2 = new Point(rect.Center.X, rect.Bottom);
						this.DrawAnchor(graphics, p1, p2);
					}

					if (this.ObjectAnchorTop(obj))
					{
						p1 = new Point(rect.Center.X, bounds.Top);
						p2 = new Point(rect.Center.X, rect.Top);
						this.DrawAnchor(graphics, p1, p2);
					}
				}
			}

			//	Dessine les contraintes.
			this.ConstrainDraw(graphics, bounds);

			//	Dessine les num�ros d'ordre.
			if (this.context.ShowZOrder)
			{
				foreach (Widget obj in this.panel.Children)
				{
					box = new Rectangle(obj.ActualBounds.BottomLeft+new Point(1, 1), new Size(12, 10));

					graphics.AddFilledRectangle(box);
					graphics.RenderSolid(Color.FromBrightness(1));

					string text = (obj.ZOrder+1).ToString();
					graphics.AddText(box.Left, box.Bottom, box.Width, box.Height, text, Font.DefaultFont, 9.0, ContentAlignment.MiddleCenter);
					graphics.RenderSolid(this.colorZOrder);
				}
			}

			//	Dessine les num�ros d'index pour la touche Tab.
			if (this.context.ShowTabIndex)
			{
				foreach (Widget obj in this.panel.Children)
				{
					if (this.ObjectTabActive(obj))
					{
						box = new Rectangle(obj.ActualBounds.BottomRight+new Point(-12-1, 1), new Size(12, 10));

						graphics.AddFilledRectangle(box);
						graphics.RenderSolid(Color.FromBrightness(1));

						string text = (obj.TabIndex+1).ToString();
						graphics.AddText(box.Left, box.Bottom, box.Width, box.Height, text, Font.DefaultFont, 9.0, ContentAlignment.MiddleCenter);
						graphics.RenderSolid(this.colorTabIndex);
					}
				}
			}

			//	Dessine l'objet survol�.
			if (!this.hilitedRectangle.IsEmpty)
			{
				graphics.AddFilledRectangle(this.hilitedRectangle);
				graphics.RenderSolid(PanelFrame.HiliteSurfaceColor);
			}

			//	Dessine le rectangle de s�lection.
			if (!this.selectedRectangle.IsEmpty)
			{
				Rectangle sel = this.selectedRectangle;
				sel.Deflate(0.5);
				graphics.AddRectangle(sel);
				graphics.RenderSolid(PanelFrame.HiliteOutlineColor);
			}
		}

		protected void DrawAnchor(Graphics graphics, Point p1, Point p2)
		{
			graphics.Align(ref p1);
			graphics.Align(ref p2);
			p1.X += 0.5;
			p1.Y += 0.5;
			p2.X += 0.5;
			p2.Y += 0.5;

			graphics.AddLine(p1, p2);
			graphics.RenderSolid(this.colorAnchor);

			graphics.AddFilledCircle(p1, 4.0);
			graphics.AddFilledCircle(p2, 4.0);
			graphics.RenderSolid(this.colorAnchor);
		}

		protected Rectangle RealBounds
		{
			get
			{
				return new Rectangle(Point.Zero, this.panel.RealMinSize);
			}
		}

		protected static Color HiliteOutlineColor
		{
			//	Couleur lorsqu'un objet est survol� par la souris.
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
				return Color.FromColor(adorner.ColorCaption, 0.8);
			}
		}

		protected static Color HiliteSurfaceColor
		{
			//	Couleur lorsqu'un objet est survol� par la souris.
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
				return Color.FromColor(adorner.ColorCaption, 0.4);
			}
		}
		#endregion


		#region Constrain
		protected void ConstrainStart()
		{
			if (this.context.ShowConstrain)
			{
				this.ConstrainInitialise(this.panel.Children, this.context.ConstrainMargin);
			}
			else
			{
				this.constrainList.Clear();
			}
		}

		protected void ConstrainEnd()
		{
			if (this.constrainList.Count != 0)
			{
				this.constrainList.Clear();
				this.Invalidate();
			}
		}

		public void ConstrainInitialise(Widgets.Collections.FlatChildrenCollection widgets, double margin)
		{
			//	Initialise les contraintes en fonction de l'ensemble des objets.
			Constrain constrain;
			this.constrainList.Clear();

			foreach (Widget obj in widgets)
			{
				constrain = new Constrain(obj.ActualBounds.BottomLeft, Constrain.Type.Left, margin);
				this.ConstrainAdd(constrain);

				constrain = new Constrain(obj.ActualBounds.BottomRight, Constrain.Type.Right, margin);
				this.ConstrainAdd(constrain);

				constrain = new Constrain(obj.ActualBounds.BottomLeft, Constrain.Type.Bottom, margin);
				this.ConstrainAdd(constrain);

				constrain = new Constrain(obj.ActualBounds.TopLeft, Constrain.Type.Top, margin);
				this.ConstrainAdd(constrain);
			}
		}

		protected void ConstrainAdd(Constrain toAdd)
		{
			//	Ajoute une contrainte dans une liste, si elle n'y est pas d�j�.
			foreach (Constrain constrain in this.constrainList)
			{
				if (constrain.IsEqualTo(toAdd))
				{
					return;
				}
			}

			this.constrainList.Add(toAdd);
		}

		public void ConstrainActivate(Rectangle rect)
		{
			//	Active les contraintes pour un rectangle donn�.
			foreach (Constrain constrain in this.constrainList)
			{
				constrain.IsActivate = false;

				if (constrain.Detect(rect.BottomLeft))
				{
					constrain.IsActivate = true;
					continue;
				}

				if (constrain.Detect(rect.BottomRight))
				{
					constrain.IsActivate = true;
					continue;
				}

				if (constrain.Detect(rect.TopLeft))
				{
					constrain.IsActivate = true;
					continue;
				}

				if (constrain.Detect(rect.TopRight))
				{
					constrain.IsActivate = true;
					continue;
				}
			}
		}

		public Rectangle ConstrainSnap(Rectangle rect)
		{
			foreach (Constrain constrain in this.constrainList)
			{
				rect = constrain.Snap(rect);
			}

			return rect;
		}

		public void ConstrainDraw(Graphics graphics, Rectangle box)
		{
			foreach (Constrain constrain in this.constrainList)
			{
				constrain.Draw(graphics, box);
			}
		}

		public class Constrain
		{
			public enum Type
			{
				Left,
				Right,
				Bottom,
				Top,
			}

			public Constrain(Point position, Type type, double margin)
			{
				this.position = position;
				this.type = type;
				this.margin = margin;
				this.isActivate = false;
			}

			public bool IsVertical
			{
				get
				{
					return (this.type == Type.Left || this.type == Type.Right);
				}
			}

			public bool IsActivate
			{
				get
				{
					return this.isActivate;
				}
				set
				{
					this.isActivate = value;
				}
			}

			public bool IsEqualTo(Constrain constrain)
			{
				//	Teste si deux contraintes sont identiques (sans tenir compte de l'activation).
				if (this.type != constrain.type)
				{
					return false;
				}

				if (this.IsVertical)
				{
					return (this.position.X == constrain.position.X);
				}
				else
				{
					return (this.position.Y == constrain.position.Y);
				}
			}

			public void Draw(Graphics graphics, Rectangle box)
			{
				//	Dessine une contrainte.
				if (this.IsVertical)
				{
					graphics.AddLine(this.position.X+0.5, box.Bottom, this.position.X+0.5, box.Top);
				}
				else
				{
					graphics.AddLine(box.Left, this.position.Y+0.5, box.Right, this.position.Y+0.5);
				}

				Color color = PanelFrame.HiliteOutlineColor;
				if (!this.isActivate)
				{
					color.A *= 0.2;  // plus transparent s'il s'agit d'une contrainte inactive
				}
				graphics.RenderSolid(color);
			}

			public bool Detect(Point position)
			{
				//	D�tecte si une position est proche d'une contrainte.
				if (this.IsVertical)
				{
					if (position.X >= this.position.X-this.margin && position.X <= this.position.X+this.margin)
					{
						return true;
					}
				}
				else
				{
					if (position.Y >= this.position.Y-this.margin && position.Y <= this.position.Y+this.margin)
					{
						return true;
					}
				}

				return false;
			}

			public Rectangle Snap(Rectangle rect)
			{
				//	Adapte un rectangle � une contrainte.
				if (this.type == Type.Left && this.Detect(rect.BottomLeft))
				{
					rect.Offset(this.position.X-rect.Left, 0);
				}

				if (this.type == Type.Right && this.Detect(rect.BottomRight))
				{
					rect.Offset(this.position.X-rect.Right, 0);
				}

				if (this.type == Type.Bottom && this.Detect(rect.BottomLeft))
				{
					rect.Offset(0, this.position.Y-rect.Bottom);
				}

				if (this.type == Type.Top && this.Detect(rect.TopLeft))
				{
					rect.Offset(0, this.position.Y-rect.Top);
				}

				return rect;
			}

			protected Point					position;
			protected Type					type;
			protected double				margin;
			protected bool					isActivate;
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

		protected virtual void OnUpdateCommands()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler("UpdateCommands");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event EventHandler UpdateCommands
		{
			add
			{
				this.AddUserEventHandler("UpdateCommands", value);
			}
			remove
			{
				this.RemoveUserEventHandler("UpdateCommands", value);
			}
		}
		#endregion


		protected UI.Panel					panel;
		protected PanelsContext				context;

		protected Widget					creatingObject;
		protected List<Widget>				selectedObjects = new List<Widget>();
		protected Rectangle					selectedRectangle = Rectangle.Empty;
		protected Rectangle					hilitedRectangle = Rectangle.Empty;
		protected bool						rectangling;  // j'invente des mots si je veux !
		protected bool						dragging;
		protected Point						startingPos;
		protected Color						colorOutsurface = Color.FromAlphaRgb(0.2, 0.5, 0.5, 0.5);
		protected Color						colorZOrder = Color.FromRgb(1,0,0);
		protected Color						colorTabIndex = Color.FromRgb(0,0,1);
		protected Color						colorAnchor = Color.FromRgb(1,0,0);
		protected Color						colorGrid1 = Color.FromAlphaRgb(0.2, 0.4, 0.4, 0.4);
		protected Color						colorGrid2 = Color.FromAlphaRgb(0.2, 0.7, 0.7, 0.7);
		protected List<Constrain>			constrainList = new List<Constrain>();

		protected Image						mouseCursorArrow = null;
		protected Image						mouseCursorArrowPlus = null;
		protected Image						mouseCursorGlobal = null;
		protected Image						mouseCursorEdit = null;
		protected Image						mouseCursorPen = null;
		protected Image						mouseCursorZoom = null;
		protected Image						mouseCursorHand = null;
	}
}
