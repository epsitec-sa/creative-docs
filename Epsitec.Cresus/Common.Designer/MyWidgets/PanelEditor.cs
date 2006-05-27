using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget venant par-dessus UI.Panel pour �diter ce dernier.
	/// </summary>
	public class PanelEditor : AbstractGroup, IPaintFilter
	{
		protected enum MouseCursorType
		{
			Unknow,
			Arrow,
			ArrowPlus,
			Global,
			Hand,
			Edit,
			Pen,
			Zoom,
			Finger,
		}


		public PanelEditor() : base()
		{
		}

		public PanelEditor(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		static PanelEditor()
		{
			Widgets.Helpers.VisualPropertyMetadata metadata = new Widgets.Helpers.VisualPropertyMetadata(ContentAlignment.TopLeft, Widgets.Helpers.VisualPropertyMetadataOptions.AffectsTextLayout);
			Widgets.Visual.ContentAlignmentProperty.OverrideMetadata(typeof(PanelEditor), metadata);
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
				this.constrainsList = new ConstrainsList(this);
				this.handlesList = new HandlesList(this);
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

				case "AlignBaseLine":
					this.SelectAlignBaseLine();
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

				case MessageType.KeyPress:
					if (message.KeyCode == KeyCode.Space)
					{
						this.constrainsList.InvertLock();
					}
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
				this.CreateObjectDown(pos, isRightButton, isControlPressed, isShiftPressed);
			}
		}

		void ProcessMouseMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	La souris a �t� d�plac�e.
			this.handlesList.Hilite(pos);

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
				this.CreateObjectMove(pos, isRightButton, isControlPressed, isShiftPressed);
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
				this.CreateObjectUp(pos, isRightButton, isControlPressed, isShiftPressed);
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
				this.CreateObjectKeyChanged(isControlPressed, isShiftPressed);
			}
		}
		#endregion

		#region ProcessMouse select
		protected void SelectDown(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	S�lection ponctuelle, souris press�e.
			this.lastCreatedObject = null;

			this.startingPos = pos;
			this.isDragging = false;
			this.isRectangling = false;
			this.handlesList.DraggingStop();

			Widget obj = null;

			if (this.selectedObjects.Count == 1)
			{
				this.handlesList.DraggingStart(pos);

				if (this.handlesList.IsDragging)
				{
					this.SetHiliteRectangle(Rectangle.Empty);
					return;
				}
			}

			obj = this.Detect(pos);  // objet vis� par la souris

			if (!isShiftPressed)  // touche Shift rel�ch�e ?
			{
				if (obj != null && this.selectedObjects.Contains(obj))
				{
					this.DraggingStart(pos);
					return;
				}
				this.selectedObjects.Clear();
				this.UpdateAfterSelectionChanged();
			}

			if (obj == null)
			{
				this.isRectangling = true;
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
				this.UpdateAfterSelectionChanged();
				this.DraggingStart(pos);
			}

			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	S�lection ponctuelle, souris d�plac�e.
			if (this.handlesList.IsFinger)
			{
				this.ChangeMouseCursor(MouseCursorType.Finger);
			}
			else if (isShiftPressed)
			{
				this.ChangeMouseCursor(MouseCursorType.ArrowPlus);
			}
			else
			{
				this.ChangeMouseCursor(MouseCursorType.Arrow);
			}

			if (this.isDragging)
			{
				this.DraggingMove(pos);
			}
			else if (this.isRectangling)
			{
				this.SetSelectRectangle(new Rectangle(this.startingPos, pos));
			}
			else if (this.handlesList.IsDragging)
			{
				this.handlesList.DraggingMove(pos);
			}
			else
			{
				Widget obj = this.Detect(pos);
				Rectangle rect = Rectangle.Empty;
				if (obj != null)
				{
					rect = this.GetObjectBounds(obj);
				}
				this.SetHiliteRectangle(rect);  // met en �vidence l'objet survol� par la souris
			}
		}

		protected void SelectUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	S�lection ponctuelle, souris rel�ch�e.
			if (this.isDragging)
			{
				this.DraggingEnd();
			}

			if (this.isRectangling)
			{
				this.SelectObjectsInRectangle(this.selectedRectangle);
				this.SetSelectRectangle(Rectangle.Empty);
				this.isRectangling = false;
			}

			if (this.handlesList.IsDragging)
			{
				this.handlesList.DraggingStop();
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
			this.lastCreatedObject = null;

			Widget obj = null;

			if (this.selectedObjects.Count == 1)
			{
				this.handlesList.DraggingStart(pos);

				if (this.handlesList.IsDragging)
				{
					this.SetHiliteRectangle(Rectangle.Empty);
					return;
				}
			}

			obj = this.Detect(pos);  // objet vis� par la souris

			this.startingPos = pos;
			this.isDragging = false;
			this.isRectangling = false;

			if (!isShiftPressed)  // touche Shift rel�ch�e ?
			{
				if (obj != null && this.selectedObjects.Contains(obj))
				{
					this.DraggingStart(pos);
					return;
				}
				this.selectedObjects.Clear();
				this.UpdateAfterSelectionChanged();
			}

			this.isRectangling = true;

			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void GlobalMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	S�lection rectangulaire, souris d�plac�e.
			if (this.handlesList.IsFinger)
			{
				this.ChangeMouseCursor(MouseCursorType.Finger);
			}
			else if (isShiftPressed)
			{
				this.ChangeMouseCursor(MouseCursorType.ArrowPlus);
			}
			else
			{
				this.ChangeMouseCursor(MouseCursorType.Global);
			}

			if (this.isDragging)
			{
				this.DraggingMove(pos);
			}
			else if (this.isRectangling)
			{
				this.SetSelectRectangle(new Rectangle(this.startingPos, pos));
			}
			else if (this.handlesList.IsDragging)
			{
				this.handlesList.DraggingMove(pos);
			}
		}

		protected void GlobalUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	S�lection rectangulaire, souris rel�ch�e.
			if (this.isDragging)
			{
				this.DraggingEnd();
			}

			if (this.isRectangling)
			{
				this.SelectObjectsInRectangle(this.selectedRectangle);
				this.SetSelectRectangle(Rectangle.Empty);
				this.isRectangling = false;
			}

			if (this.handlesList.IsDragging)
			{
				this.handlesList.DraggingStop();
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

		#region ProcessMouse create object
		protected void CreateObjectDown(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, souris press�e.
			this.DeselectAll();

			this.creatingObject = this.CreateObjectItem();

			Rectangle bounds;
			this.CreateObjectAdjust(ref pos, out bounds);

			this.creatingOrigin = this.MapClientToScreen(Point.Zero);
			this.creatingWindow = new DragWindow();
			this.creatingWindow.DefineWidget(this.creatingObject, this.creatingObject.PreferredSize, Drawing.Margins.Zero);
			this.creatingWindow.WindowLocation = this.creatingOrigin + pos;
			this.creatingWindow.Owner = this.Window;
			this.creatingWindow.FocusedWidget = this.creatingObject;
			this.creatingWindow.Show();

			this.constrainsList.Starting(Rectangle.Empty);
			this.constrainsList.Activate(bounds, this.GetObjectBaseLine(this.creatingObject), null);
		}

		protected void CreateObjectMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, souris d�plac�e.
			this.ChangeMouseCursor(MouseCursorType.Pen);

			if (this.creatingObject != null)
			{
				Rectangle bounds;
				this.CreateObjectAdjust(ref pos, out bounds);
				this.constrainsList.Activate(bounds, this.GetObjectBaseLine(this.creatingObject), null);
				this.creatingWindow.WindowLocation = this.creatingOrigin + pos;
			}
		}

		protected void CreateObjectUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, souris rel�ch�e.
			if (this.creatingObject != null)
			{
				this.creatingWindow.Hide();
				this.creatingWindow.Dispose();
				this.creatingWindow = null;

				Rectangle bounds;
				this.CreateObjectAdjust(ref pos, out bounds);

				Widget parent = this.DetectGroup(bounds);
				Point offset = parent.ActualBounds.BottomLeft;

				this.creatingObject = this.CreateObjectItem();
				this.creatingObject.SetParent(parent);
				this.creatingObject.Anchor = AnchorStyles.BottomLeft;
				this.creatingObject.TabNavigation = TabNavigationMode.Passive;

				if (parent == this.panel)
				{
					pos.X -= this.panel.Padding.Left;
					pos.Y -= this.panel.Padding.Bottom;
				}
				else
				{
					pos -= offset;
				}
				this.SetObjectPosition(this.creatingObject, pos);

				this.constrainsList.Ending();

				this.lastCreatedObject = this.creatingObject;
				this.creatingObject = null;
				this.OnUpdateCommands();
			}
		}

		protected Widget CreateObjectItem()
		{
			//	Cr�e un objet selon la palette d'outils.
			Widget item = null;

			if (this.context.Tool == "ObjectLine")
			{
				item = new Separator();
				item.PreferredHeight = 1;
				//?item.MinWidth = 10;
			}

			if (this.context.Tool == "ObjectStatic")
			{
				item = new StaticText();
				item.Text = "StaticText";
				//?item.MinWidth = 10;
			}

			if (this.context.Tool == "ObjectButton")
			{
				item = new Button();
				item.Text = "Button";
				//?item.MinWidth = 20;
			}

			if (this.context.Tool == "ObjectText")
			{
				item = new TextField();
				item.Text = "TextField";
				//?item.MinWidth = 20;
			}

			if (this.context.Tool == "ObjectGroup")
			{
				item = new GroupBox();
				item.Text = "Group";
				item.PreferredSize = new Size(200, 100);
				//?item.MinWidth = 50;
				//?item.MinHeight = 50;
			}

			return item;
		}

		protected void CreateObjectAdjust(ref Point pos, out Rectangle bounds)
		{
			//	Ajuste la position de l'objet � cr�er selon les contraintes.
			pos.X -= this.creatingObject.PreferredWidth/2;
			pos.Y -= this.creatingObject.PreferredHeight/2;
			bounds = new Rectangle(pos, this.creatingObject.PreferredSize);
			Rectangle adjust = this.constrainsList.Snap(bounds, this.GetObjectBaseLine(this.creatingObject));
			Point corr = adjust.BottomLeft - bounds.BottomLeft;
			pos += corr;
			bounds.Offset(corr);
		}

		protected void CreateObjectKeyChanged(bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, touche press�e ou rel�ch�e.
		}
		#endregion


		#region Dragging
		protected void DraggingStart(Point pos)
		{
			//	D�but du drag pour d�placer les objets s�lectionn�s.
			this.draggingArraySelected = this.selectedObjects.ToArray();

			this.draggingRectangle = this.SelectBounds;
			this.draggingBaseLine = this.SelectBaseLine;
			this.draggingOffset = this.draggingRectangle.BottomLeft - pos;
			this.constrainsList.Starting(this.draggingRectangle);

			Widget container = new Widget();
			container.PreferredSize = this.draggingRectangle.Size;

			foreach (Widget obj in this.selectedObjects)
			{
				Point origin = this.GetObjectPosition(obj)-this.draggingRectangle.BottomLeft;
				origin.X += this.panel.Padding.Left;
				origin.Y += this.panel.Padding.Bottom;
				CloneView clone = new CloneView(container);
				clone.PreferredSize = obj.ActualSize;
				clone.Margins = new Margins(origin.X, 0, 0, origin.Y);
				clone.Anchor = AnchorStyles.BottomLeft;
				clone.Model = obj;
			}

			this.draggingOrigin = this.MapClientToScreen(this.draggingOffset);
			this.draggingWindow = new DragWindow();
			this.draggingWindow.DefineWidget(container, container.PreferredSize, Drawing.Margins.Zero);
			this.draggingWindow.WindowLocation = this.draggingOrigin + pos;
			this.draggingWindow.Owner = this.Window;
			this.draggingWindow.FocusedWidget = container;
			this.draggingWindow.Show();

			this.SetHiliteRectangle(Rectangle.Empty);
			this.isDragging = true;
			this.Invalidate();
		}

		protected void DraggingMove(Point pos)
		{
			//	Mouvement du drag pour d�placer les objets s�lectionn�s.
			this.draggingRectangle.Offset((this.draggingOffset+pos)-this.draggingRectangle.BottomLeft);
			this.constrainsList.Activate(this.draggingRectangle, this.draggingBaseLine, this.draggingArraySelected);
			this.Invalidate();

			Point adjust = this.draggingRectangle.BottomLeft;
			this.draggingRectangle = this.constrainsList.Snap(this.draggingRectangle, this.draggingBaseLine);
			adjust = this.draggingRectangle.BottomLeft - adjust;

			this.draggingWindow.WindowLocation = this.draggingOrigin + pos + adjust;
		}

		protected void DraggingEnd()
		{
			//	Fin du drag pour d�placer les objets s�lectionn�s.
			this.draggingWindow.Hide();
			this.draggingWindow.Dispose();
			this.draggingWindow = null;

			Rectangle initial = this.SelectBounds;
			this.MoveSelection(this.draggingRectangle.BottomLeft - initial.BottomLeft);
			this.isDragging = false;
			this.draggingArraySelected = null;
			this.constrainsList.Ending();
			this.handlesList.Update();
			this.Invalidate();
		}
		#endregion


		#region Selection
		protected Widget Detect(Point pos)
		{
			//	D�tecte l'objet vis� par la souris, avec priorit� au dernier objet
			//	dessin� (donc plac� dessus).
			return this.Detect(pos, this.panel);
		}

		protected Widget Detect(Point pos, Widget parent)
		{
			for (int i=parent.Children.Count-1; i>=0; i--)
			{
				Widget widget = parent.Children[i] as Widget;

				if (widget.ActualBounds.Contains(pos))
				{
					if (widget is AbstractGroup)
					{
						Widget child = this.Detect(widget.MapParentToClient(pos), widget);
						if (child != null)
						{
							return child;
						}
					}

					return widget;
				}
			}
			return null;
		}

		protected Widget DetectGroup(Rectangle rect)
		{
			//	D�tecte dans quel groupe est enti�rement inclu un rectangle donn�.
#if false
			for (int i=this.panel.Children.Count-1; i>=0; i--)
			{
				Widget widget = this.panel.Children[i] as Widget;
				if (widget is AbstractGroup)
				{
					if (widget.ActualBounds.Contains(rect))
					{
						return widget;
					}
				}
			}
#endif
			return this.panel;
		}

		public void DeselectAll()
		{
			//	D�s�lectionne tous les objets.
			if (this.selectedObjects.Count > 0)
			{
				this.selectedObjects.Clear();
				this.UpdateAfterSelectionChanged();
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

			this.UpdateAfterSelectionChanged();
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

			this.UpdateAfterSelectionChanged();
			this.OnChildrenSelected();
			this.Invalidate();
		}

		public void SelectLastCreatedObject()
		{
			//	S�lectionne l'objet qui vient d'�tre cr��.
			if (this.lastCreatedObject != null)
			{
				this.SelectOneObject(this.lastCreatedObject);
				this.lastCreatedObject = null;
			}
		}

		protected void SelectOneObject(Widget obj)
		{
			//	S�lectionne un objet.
			this.selectedObjects.Clear();
			this.selectedObjects.Add(obj);
			this.UpdateAfterSelectionChanged();
			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectObjectsInRectangle(Rectangle sel)
		{
			//	S�lectionne tous les objets enti�rement inclus dans un rectangle.
			foreach (Widget obj in this.panel.Children)
			{
				if (sel.Contains(this.GetObjectBounds(obj)))
				{
					this.selectedObjects.Add(obj);
				}
			}

			this.UpdateAfterSelectionChanged();
			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void UpdateAfterSelectionChanged()
		{
			//	Mise � jour apr�s un changement de s�lection.
			if (this.selectedObjects.Count == 1)
			{
				this.handlesList.Create(this.selectedObjects[0]);
			}
			else
			{
				this.handlesList.Flush();
			}
		}

		protected void SetHiliteRectangle(Rectangle rect)
		{
			//	D�termine la zone � mettre en �vidence lors d'un survol.
			if (this.hilitedRectangle != rect)
			{
				this.Invalidate(this.hilitedRectangle);  // invalide l'ancienne zone
				this.hilitedRectangle = rect;
				this.Invalidate(this.hilitedRectangle);  // invalide la nouvelle zone
			}
		}

		protected void SetSelectRectangle(Rectangle rect)
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
			this.UpdateAfterSelectionChanged();
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
				toRepaint = Rectangle.Union(toRepaint, this.GetObjectBounds(obj));
				this.SetObjectPosition(obj, this.GetObjectPosition(obj)+move);
				toRepaint = Rectangle.Union(toRepaint, this.GetObjectBounds(obj));
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
						this.SetObjectPositionY(obj, bounds.Bottom);
					}
				}
				else if (direction > 0)  // en haut ?
				{
					foreach (Widget obj in this.selectedObjects)
					{
						this.SetObjectPositionY(obj, bounds.Top-this.GetObjectSize(obj).Height);
					}
				}
				else  // centr� verticalement ?
				{
					foreach (Widget obj in this.selectedObjects)
					{
						this.SetObjectPositionY(obj, System.Math.Floor(bounds.Center.Y-this.GetObjectSize(obj).Height/2));
					}
				}
			}
			else
			{
				if (direction < 0)  // � gauche ?
				{
					foreach (Widget obj in this.selectedObjects)
					{
						this.SetObjectPositionX(obj, bounds.Left);
					}
				}
				else if (direction > 0)  // � droite ?
				{
					foreach (Widget obj in this.selectedObjects)
					{
						this.SetObjectPositionX(obj, bounds.Right-this.GetObjectSize(obj).Width);
					}
				}
				else  // centr� horizontalement ?
				{
					foreach (Widget obj in this.selectedObjects)
					{
						this.SetObjectPositionX(obj, System.Math.Floor(bounds.Center.X-this.GetObjectSize(obj).Width/2));
					}
				}
			}

			this.Invalidate();
		}

		protected void SelectAlignBaseLine()
		{
			//	Aligne sur la ligne de base tous les objets s�lectionn�s.
#if false
			Widget model = null;
			foreach (Widget obj in this.selectedObjects)
			{
				double baseLine = this.GetObjectBaseLine(obj);
				if (baseLine != 0)
				{
					model = obj;
					break;
				}
			}

			if (model != null)
			{
				foreach (Widget obj in this.selectedObjects)
				{
					if (obj != model)
					{
						Widget.BaseLineAlign(model, obj);
					}
				}
			}

			this.Invalidate();
#else
			Rectangle bounds = this.SelectBounds;
			double baseLine = bounds.Bottom + 6;  // TODO: faire mieux !!!

			foreach (Widget obj in this.selectedObjects)
			{
				this.SetObjectPositionY(obj, baseLine-this.GetObjectBaseLine(obj));
			}

			this.Invalidate();
#endif
		}

		protected void SelectAdjust(bool isVertical)
		{
			//	Ajuste les dimensions de tous les objets s�lectionn�s.
			Rectangle bounds = this.SelectBounds;

			if (isVertical)
			{
				foreach (Widget obj in this.selectedObjects)
				{
					this.SetObjectPositionY(obj, bounds.Bottom);
					this.SetObjectHeight(obj, bounds.Height);
				}
			}
			else
			{
				foreach (Widget obj in this.selectedObjects)
				{
					this.SetObjectPositionX(obj, bounds.Left);
					this.SetObjectWidth(obj, bounds.Width);
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
			List<Widget> list = new List<Widget>();
			foreach (Widget obj in this.panel.Children)
			{
				list.Add(obj);
			}
			list.Sort(new Comparers.WidgetDisposition());

			int index = 0;
			foreach (Widget obj in list)
			{
				obj.TabIndex = index++;
				obj.TabNavigation = TabNavigationMode.ActivateOnTab;
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
					bounds = Rectangle.Union(bounds, this.GetObjectBounds(obj));
				}

				return bounds;
			}
		}

		protected double SelectBaseLine
		{
			//	Retourne la position de la ligne de base des objets s�lectionn�s.
			get
			{
				if (this.selectedObjects.Count == 1)
				{
					return this.GetObjectBaseLine(this.selectedObjects[0]);
				}
				else
				{
					return 0;
				}
			}
		}


		public Rectangle GetObjectBounds(Widget obj)
		{
			//	Retourne la bo�te d'un objet.
			Point pos = new Point(obj.Margins.Left, obj.Margins.Bottom);
			//?this.Window.ForceLayout();
			//?pos = this.panel.MapClientToParent(pos);
			pos.X += this.panel.Padding.Left;
			pos.Y += this.panel.Padding.Bottom;
			return new Rectangle(pos, obj.PreferredSize);
		}

		public void SetObjectBounds(Widget obj, Rectangle rect)
		{
			//	Modifie la bo�te d'un objet.
			rect.Normalise();

			if (rect.Width < obj.MinWidth)
			{
				rect.Width = obj.MinWidth;
			}

			if (rect.Height < obj.MinHeight)
			{
				rect.Height = obj.MinHeight;
			}

			Point pos = rect.BottomLeft;
			pos.X -= this.panel.Padding.Left;
			pos.Y -= this.panel.Padding.Bottom;
			pos.X = System.Math.Max(pos.X, 0);
			pos.Y = System.Math.Max(pos.Y, 0);
			//?this.Window.ForceLayout();
			//?pos = this.panel.MapParentToClient(pos);

			Margins margins = obj.Margins;
			margins.Left   = pos.X;
			margins.Bottom = pos.Y;
			obj.Margins = margins;
			obj.PreferredSize = rect.Size;

			this.Invalidate();
		}

		protected Point GetObjectPosition(Widget obj)
		{
			//	Retourne l'origine d'un objet.
			Point pos = new Point(obj.Margins.Left, obj.Margins.Bottom);
			//?this.Window.ForceLayout();
			//?return this.panel.MapClientToParent(pos);
			return pos;
		}

		protected void SetObjectPositionX(Widget obj, double x)
		{
			//	D�place l'origine gauche d'un objet.
			Point pos = new Point(x, 0);
			pos.X -= this.panel.Padding.Left;
			pos.Y -= this.panel.Padding.Bottom;
			pos.X = System.Math.Max(pos.X, 0);
			pos.Y = System.Math.Max(pos.Y, 0);
			//?this.Window.ForceLayout();
			//?pos = this.panel.MapParentToClient(pos);

			Margins margins = obj.Margins;
			margins.Left = pos.X;
			obj.Margins = margins;
		}

		protected void SetObjectPositionY(Widget obj, double y)
		{
			//	D�place l'origine inf�rieure d'un objet.
			Point pos = new Point(0, y);
			pos.X -= this.panel.Padding.Left;
			pos.Y -= this.panel.Padding.Bottom;
			pos.X = System.Math.Max(pos.X, 0);
			pos.Y = System.Math.Max(pos.Y, 0);
			//?this.Window.ForceLayout();
			//?pos = this.panel.MapParentToClient(pos);

			Margins margins = obj.Margins;
			margins.Bottom = pos.Y;
			obj.Margins = margins;
		}

		protected void SetObjectPosition(Widget obj, Point pos)
		{
			//	D�place l'origine d'un objet.
			pos.X = System.Math.Max(pos.X, 0);
			pos.Y = System.Math.Max(pos.Y, 0);
			//?this.Window.ForceLayout();
			//?pos = this.panel.MapParentToClient(pos);

			Margins margins = obj.Margins;
			margins.Left   = pos.X;
			margins.Bottom = pos.Y;
			obj.Margins = margins;
		}

		protected Size GetObjectSize(Widget obj)
		{
			//	Retourne les dimensions d'un objet.
			return obj.ActualSize;
		}

		protected void SetObjectWidth(Widget obj, double dx)
		{
			//	Modifie la largeur d'un objet.
			obj.PreferredWidth = dx;
		}

		protected void SetObjectHeight(Widget obj, double dy)
		{
			//	Modifie la hauteur d'un objet.
			obj.PreferredHeight = dy;
		}

		protected void SetObjectSize(Widget obj, Size size)
		{
			//	Modifie les dimensions d'un objet.
			obj.PreferredSize = size;
		}

		public double GetObjectBaseLine(Widget obj)
		{
			//	Retourne la position relative de la ligne de base depuis le bas de l'objet.
			return System.Math.Floor(obj.GetBaseLine().Y);
		}

		protected bool IsObjectAnchorLeft(Widget obj)
		{
			//	Indique si l'objet est ancr� � gauche.
			return (obj.Anchor & AnchorStyles.Left) != 0;
		}

		protected bool IsObjectAnchorRight(Widget obj)
		{
			//	Indique si l'objet est ancr� � gauche.
			return (obj.Anchor & AnchorStyles.Right) != 0;
		}

		protected bool IsObjectAnchorBottom(Widget obj)
		{
			//	Indique si l'objet est ancr� � gauche.
			return (obj.Anchor & AnchorStyles.Bottom) != 0;
		}

		protected bool IsObjectAnchorTop(Widget obj)
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
				graphics.RenderSolid(PanelsContext.ColorOutsurface);
			}

			if (bounds.Right < box.Right)
			{
				Rectangle part = new Rectangle(bounds.Right, box.Bottom, box.Right-bounds.Right, bounds.Height);
				graphics.AddFilledRectangle(part);
				graphics.RenderSolid(PanelsContext.ColorOutsurface);
			}

			//	Dessine la grille magn�tique
			if (this.context.ShowGrid)
			{
				double step = this.context.GridStep;
				int hilite = 0;
				for (double x=step+0.5; x<bounds.Width; x+=step)
				{
					graphics.AddLine(x, bounds.Bottom, x, bounds.Top);
					graphics.RenderSolid(((++hilite)%10 == 0) ? PanelsContext.ColorGrid1 : PanelsContext.ColorGrid2);
				}
				hilite = 0;
				for (double y=step+0.5; y<bounds.Height; y+=step)
				{
					graphics.AddLine(bounds.Left, y, bounds.Right, y);
					graphics.RenderSolid(((++hilite)%10 == 0) ? PanelsContext.ColorGrid1 : PanelsContext.ColorGrid2);
				}
			}

			//	Dessine les objets s�lectionn�s.
			if (this.selectedObjects.Count > 0 && !this.isDragging && !this.handlesList.IsDragging)
			{
				foreach (Widget obj in this.selectedObjects)
				{
					Rectangle rect = this.GetObjectBounds(obj);
					rect.Deflate(0.5);

					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(PanelsContext.ColorHiliteSurface);

					graphics.LineWidth = 3;
					graphics.AddRectangle(rect);
					graphics.RenderSolid(PanelsContext.ColorHiliteOutline);
					graphics.LineWidth = 1;
				}
			}

			//	Dessine les ancrages.
			if (this.context.ShowAnchor && this.selectedObjects.Count > 0 && !this.isDragging && !this.handlesList.IsDragging)
			{
				foreach (Widget obj in this.selectedObjects)
				{
					Rectangle rect = this.GetObjectBounds(obj);
					Point p1, p2;

					if (this.IsObjectAnchorLeft(obj))
					{
						p1 = new Point(bounds.Left, rect.Center.Y);
						p2 = new Point(rect.Left, rect.Center.Y);
						this.DrawAnchor(graphics, p1, p2);
					}

					if (this.IsObjectAnchorRight(obj))
					{
						p1 = new Point(bounds.Right, rect.Center.Y);
						p2 = new Point(rect.Right, rect.Center.Y);
						this.DrawAnchor(graphics, p1, p2);
					}

					if (this.IsObjectAnchorBottom(obj))
					{
						p1 = new Point(rect.Center.X, bounds.Bottom);
						p2 = new Point(rect.Center.X, rect.Bottom);
						this.DrawAnchor(graphics, p1, p2);
					}

					if (this.IsObjectAnchorTop(obj))
					{
						p1 = new Point(rect.Center.X, bounds.Top);
						p2 = new Point(rect.Center.X, rect.Top);
						this.DrawAnchor(graphics, p1, p2);
					}
				}
			}

			//	Dessine les contraintes.
			this.constrainsList.Draw(graphics, bounds);

			//	Dessine les num�ros d'ordre.
			if (this.context.ShowZOrder && !this.isDragging && !this.handlesList.IsDragging)
			{
				foreach (Widget obj in this.panel.Children)
				{
					Rectangle rect = this.GetObjectBounds(obj);
					box = new Rectangle(rect.BottomLeft+new Point(1, 1), new Size(12, 10));

					graphics.AddFilledRectangle(box);
					graphics.RenderSolid(Color.FromBrightness(1));

					string text = (obj.ZOrder+1).ToString();
					graphics.AddText(box.Left, box.Bottom, box.Width, box.Height, text, Font.DefaultFont, 9.0, ContentAlignment.MiddleCenter);
					graphics.RenderSolid(PanelsContext.ColorZOrder);
				}
			}

			//	Dessine les num�ros d'index pour la touche Tab.
			if (this.context.ShowTabIndex && !this.isDragging && !this.handlesList.IsDragging)
			{
				foreach (Widget obj in this.panel.Children)
				{
					if (this.ObjectTabActive(obj))
					{
						Rectangle rect = this.GetObjectBounds(obj);
						box = new Rectangle(rect.BottomRight+new Point(-12-1, 1), new Size(12, 10));

						graphics.AddFilledRectangle(box);
						graphics.RenderSolid(Color.FromBrightness(1));

						string text = (obj.TabIndex+1).ToString();
						graphics.AddText(box.Left, box.Bottom, box.Width, box.Height, text, Font.DefaultFont, 9.0, ContentAlignment.MiddleCenter);
						graphics.RenderSolid(PanelsContext.ColorTabIndex);
					}
				}
			}

			//	Dessine l'objet survol�.
			if (!this.hilitedRectangle.IsEmpty)
			{
				graphics.AddFilledRectangle(this.hilitedRectangle);
				graphics.RenderSolid(PanelsContext.ColorHiliteSurface);
			}

			//	Dessine le rectangle de s�lection.
			if (!this.selectedRectangle.IsEmpty)
			{
				Rectangle sel = this.selectedRectangle;
				sel.Deflate(0.5);
				graphics.AddRectangle(sel);
				graphics.RenderSolid(PanelsContext.ColorHiliteOutline);
			}

			//	Dessine les poign�es.
			if (this.selectedObjects.Count == 1 && !this.isDragging)
			{
				this.handlesList.Draw(graphics);
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
			graphics.RenderSolid(PanelsContext.ColorAnchor);

			graphics.AddFilledCircle(p1, 4.0);
			graphics.AddFilledCircle(p2, 4.0);
			graphics.RenderSolid(PanelsContext.ColorAnchor);
		}

		public Rectangle RealBounds
		{
			get
			{
				return new Rectangle(Point.Zero, this.panel.RealMinSize);
			}
		}
		#endregion


		#region IPaintFilter Members
		bool IPaintFilter.IsWidgetFullyDiscarded(Widget widget)
		{
			//	Retourne true pour indiquer que le widget en question ne doit
			//	pas �tre peint, ni ses enfants d'ailleurs. Ceci �vite que les
			//	widgets s�lectionn�s ne soient peints.
			return this.isDragging && this.selectedObjects.Contains(widget);
		}

		bool IPaintFilter.IsWidgetPaintDiscarded(Widget widget)
		{
			return false;
		}

		void IPaintFilter.NotifyAboutToProcessChildren()
		{
		}

		void IPaintFilter.NotifyChildrenProcessed()
		{
		}
		#endregion


		#region MouseCursor
		protected void ChangeMouseCursor(MouseCursorType cursor)
		{
			//	Change le sprite de la souris.
			if ( cursor == this.lastCursor )  return;
			this.lastCursor = cursor;

			switch (cursor)
			{
				case MouseCursorType.Arrow:
					this.SetMouseCursorImage(ref this.mouseCursorArrow, Misc.Icon("CursorArrow"));
					break;

				case MouseCursorType.ArrowPlus:
					this.SetMouseCursorImage(ref this.mouseCursorArrowPlus, Misc.Icon("CursorArrowPlus"));
					break;

				case MouseCursorType.Global:
					this.SetMouseCursorImage(ref this.mouseCursorGlobal, Misc.Icon("CursorGlobal"));
					break;

				case MouseCursorType.Edit:
					this.SetMouseCursorImage(ref this.mouseCursorEdit, Misc.Icon("CursorEdit"));
					break;

				case MouseCursorType.Hand:
					this.SetMouseCursorImage(ref this.mouseCursorHand, Misc.Icon("CursorHand"));
					break;

				case MouseCursorType.Finger:
					this.SetMouseCursorImage(ref this.mouseCursorFinger, Misc.Icon("CursorFinger"));
					break;

				case MouseCursorType.Pen:
					this.SetMouseCursorImage(ref this.mouseCursorPen, Misc.Icon("CursorPen"));
					break;

				case MouseCursorType.Zoom:
					this.SetMouseCursorImage(ref this.mouseCursorZoom, Misc.Icon("CursorZoom"));
					break;

				default:
					this.MouseCursor = MouseCursor.AsArrow;
					break;
			}

			this.Window.MouseCursor = this.MouseCursor;
		}

		protected void SetMouseCursorImage(ref Image image, string name)
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
		protected ConstrainsList			constrainsList;
		protected HandlesList				handlesList;

		protected DragWindow				creatingWindow;
		protected Point						creatingOrigin;
		protected Widget					creatingObject;
		protected Widget					lastCreatedObject;
		protected List<Widget>				selectedObjects = new List<Widget>();
		protected Rectangle					selectedRectangle = Rectangle.Empty;
		protected Rectangle					hilitedRectangle = Rectangle.Empty;
		protected bool						isRectangling;  // j'invente des mots si je veux !
		protected bool						isDragging;
		protected DragWindow				draggingWindow;
		protected Point						draggingOffset;
		protected Point						draggingOrigin;
		protected Rectangle					draggingRectangle;
		protected double					draggingBaseLine;
		protected Widget[]					draggingArraySelected;
		protected Point						startingPos;
		protected MouseCursorType			lastCursor = MouseCursorType.Unknow;

		protected Image						mouseCursorArrow = null;
		protected Image						mouseCursorArrowPlus = null;
		protected Image						mouseCursorGlobal = null;
		protected Image						mouseCursorEdit = null;
		protected Image						mouseCursorPen = null;
		protected Image						mouseCursorZoom = null;
		protected Image						mouseCursorHand = null;
		protected Image						mouseCursorFinger = null;
	}
}
