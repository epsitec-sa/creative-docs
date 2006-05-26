using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget venant par-dessus UI.Panel pour éditer ce dernier.
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

				case MessageType.KeyPress:
					if (message.KeyCode == KeyCode.Space)
					{
						this.ConstrainLock();
					}
					break;
			}
		}

		#region ProcessMouse
		void ProcessMouseDown(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	La souris a été pressée.
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
			//	La souris a été déplacée.
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
			//	La souris a été relâchée.
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
			//	La souris a été relâchée.
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
			//	Sélection ponctuelle, souris pressée.
			this.lastCreatedObject = null;

			Widget obj = this.Detect(pos);  // objet visé par la souris

			this.startingPos = pos;
			this.isDragging = false;
			this.isRectangling = false;

			if (!isShiftPressed)  // touche Shift relâchée ?
			{
				if (obj != null && this.selectedObjects.Contains(obj))
				{
					this.DraggingStart(pos);
					return;
				}
				this.selectedObjects.Clear();
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
				this.DraggingStart(pos);
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

			if (this.isDragging)
			{
				this.DraggingMove(pos);
			}
			else if (this.isRectangling)
			{
				this.SetSelectRectangle(new Rectangle(this.startingPos, pos));
			}
			else
			{
				Widget obj = this.Detect(pos);
				Rectangle rect = Rectangle.Empty;
				if (obj != null)
				{
					rect = this.GetObjectBounds(obj);
				}
				this.SetHiliteRectangle(rect);  // met en évidence l'objet survolé par la souris
			}
		}

		protected void SelectUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection ponctuelle, souris relâchée.
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
			this.lastCreatedObject = null;

			Widget obj = this.Detect(pos);  // objet visé par la souris

			this.startingPos = pos;
			this.isDragging = false;
			this.isRectangling = false;

			if (!isShiftPressed)  // touche Shift relâchée ?
			{
				if (obj != null && this.selectedObjects.Contains(obj))
				{
					this.DraggingStart(pos);
					return;
				}
				this.selectedObjects.Clear();
			}

			this.isRectangling = true;

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

			if (this.isDragging)
			{
				this.DraggingMove(pos);
			}
			else if (this.isRectangling)
			{
				this.SetSelectRectangle(new Rectangle(this.startingPos, pos));
			}
		}

		protected void GlobalUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection rectangulaire, souris relâchée.
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

		#region ProcessMouse create object
		protected void CreateObjectDown(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, souris pressée.
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

			this.ConstrainStart(Rectangle.Empty, 0);
			this.ConstrainActivate(bounds, this.GetObjectBaseLine(this.creatingObject), null);
		}

		protected void CreateObjectMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, souris déplacée.
			this.ChangeMouseCursor(MouseCursorType.Pen);

			if (this.creatingObject != null)
			{
				Rectangle bounds;
				this.CreateObjectAdjust(ref pos, out bounds);
				this.ConstrainActivate(bounds, this.GetObjectBaseLine(this.creatingObject), null);
				this.creatingWindow.WindowLocation = this.creatingOrigin + pos;
			}
		}

		protected void CreateObjectUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, souris relâchée.
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

				this.ConstrainEnd();

				this.lastCreatedObject = this.creatingObject;
				this.creatingObject = null;
			}
		}

		protected Widget CreateObjectItem()
		{
			//	Crée un objet selon la palette d'outils.
			Widget item = null;

			if (this.context.Tool == "ObjectLine")
			{
				item = new Separator();
				item.PreferredHeight = 1;
			}

			if (this.context.Tool == "ObjectStatic")
			{
				item = new StaticText();
				item.Text = "StaticText";
			}

			if (this.context.Tool == "ObjectButton")
			{
				item = new Button();
				item.Text = "Button";
			}

			if (this.context.Tool == "ObjectText")
			{
				item = new TextField();
				item.Text = "TextField";
			}

			if (this.context.Tool == "ObjectGroup")
			{
				item = new GroupBox();
				item.Text = "Group";
				item.PreferredSize = new Size(200, 100);
			}

			return item;
		}

		protected void CreateObjectAdjust(ref Point pos, out Rectangle bounds)
		{
			//	Ajuste la position de l'objet à créer selon les contraintes.
			pos.X -= this.creatingObject.PreferredWidth/2;
			pos.Y -= this.creatingObject.PreferredHeight/2;
			bounds = new Rectangle(pos, this.creatingObject.PreferredSize);
			Rectangle adjust = this.ConstrainSnap(bounds, this.GetObjectBaseLine(this.creatingObject));
			Point corr = adjust.BottomLeft - bounds.BottomLeft;
			pos += corr;
			bounds.Offset(corr);
		}

		protected void CreateObjectKeyChanged(bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, touche pressée ou relâchée.
		}
		#endregion


		#region Dragging
		protected void DraggingStart(Point pos)
		{
			//	Début du drag pour déplacer les objets sélectionnés.
			this.draggingArraySelected = this.selectedObjects.ToArray();

			this.draggingRectangle = this.SelectBounds;
			this.draggingBaseLine = this.SelectBaseLine;
			this.draggingOffset = this.draggingRectangle.BottomLeft - pos;
			this.ConstrainStart(this.draggingRectangle, 0);

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
			//	Mouvement du drag pour déplacer les objets sélectionnés.
			this.draggingRectangle.Offset((this.draggingOffset+pos)-this.draggingRectangle.BottomLeft);
			this.ConstrainActivate(this.draggingRectangle, this.draggingBaseLine, this.draggingArraySelected);
			this.Invalidate();

			Point adjust = this.draggingRectangle.BottomLeft;
			this.draggingRectangle = this.ConstrainSnap(this.draggingRectangle, this.draggingBaseLine);
			adjust = this.draggingRectangle.BottomLeft - adjust;

			this.draggingWindow.WindowLocation = this.draggingOrigin + pos + adjust;
		}

		protected void DraggingEnd()
		{
			//	Fin du drag pour déplacer les objets sélectionnés.
			this.draggingWindow.Hide();
			this.draggingWindow.Dispose();
			this.draggingWindow = null;

			Rectangle initial = this.SelectBounds;
			this.MoveSelection(this.draggingRectangle.BottomLeft - initial.BottomLeft);
			this.isDragging = false;
			this.draggingArraySelected = null;
			this.ConstrainEnd();
			this.Invalidate();
		}
		#endregion


		#region Selection
		protected Widget Detect(Point pos)
		{
			//	Détecte l'objet visé par la souris, avec priorité au dernier objet
			//	dessiné (donc placé dessus).
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
			//	Détecte dans quel groupe est entièrement inclu un rectangle donné.
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

		public void SelectLastCreatedObject()
		{
			//	Sélectionne l'objet qui vient d'être créé.
			if (this.lastCreatedObject != null)
			{
				this.SelectOneObject(this.lastCreatedObject);
				this.lastCreatedObject = null;
			}
		}

		protected void SelectOneObject(Widget obj)
		{
			//	Sélectionne un objet.
			this.selectedObjects.Clear();
			this.selectedObjects.Add(obj);
			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectObjectsInRectangle(Rectangle sel)
		{
			//	Sélectionne tous les objets entièrement inclus dans un rectangle.
			foreach (Widget obj in this.panel.Children)
			{
				if (sel.Contains(this.GetObjectBounds(obj)))
				{
					this.selectedObjects.Add(obj);
				}
			}

			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SetHiliteRectangle(Rectangle rect)
		{
			//	Détermine la zone à mettre en évidence lors d'un survol.
			if (this.hilitedRectangle != rect)
			{
				this.Invalidate(this.hilitedRectangle);  // invalide l'ancienne zone
				this.hilitedRectangle = rect;
				this.Invalidate(this.hilitedRectangle);  // invalide la nouvelle zone
			}
		}

		protected void SetSelectRectangle(Rectangle rect)
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
			//	Aligne tous les objets sélectionnés.
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
				else  // centré verticalement ?
				{
					foreach (Widget obj in this.selectedObjects)
					{
						this.SetObjectPositionY(obj, bounds.Center.Y-this.GetObjectSize(obj).Height/2);
					}
				}
			}
			else
			{
				if (direction < 0)  // à gauche ?
				{
					foreach (Widget obj in this.selectedObjects)
					{
						this.SetObjectPositionX(obj, bounds.Left);
					}
				}
				else if (direction > 0)  // à droite ?
				{
					foreach (Widget obj in this.selectedObjects)
					{
						this.SetObjectPositionX(obj, bounds.Right-this.GetObjectSize(obj).Width);
					}
				}
				else  // centré horizontalement ?
				{
					foreach (Widget obj in this.selectedObjects)
					{
						this.SetObjectPositionX(obj, bounds.Center.X-this.GetObjectSize(obj).Width/2);
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
			//	Aligne sur la grille tous les objets sélectionnés.
			//	TODO:
		}

		protected void SelectOrder(int direction)
		{
			//	Modifie l'ordre de tous les objets sélectionnés.
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
			//	Renumérote toutes les touches Tab.
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
			//	Modifie l'ordre pour la touche Tab de tous les objets sélectionnés.
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
			//	Retourne le rectangle englobant tous les objets sélectionnés.
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
			//	Retourne la position de la ligne de base des objets sélectionnés.
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


		protected Rectangle GetObjectBounds(Widget obj)
		{
			//	Retourne la boîte d'un objet.
			Point pos = new Point(obj.Margins.Left, obj.Margins.Bottom);
			//?this.Window.ForceLayout();
			//?pos = this.panel.MapClientToParent(pos);
			pos.X += this.panel.Padding.Left;
			pos.Y += this.panel.Padding.Bottom;
			return new Rectangle(pos, obj.PreferredSize);
		}

		protected void SetObjectBounds(Widget obj, Rectangle rect)
		{
			//	Modifie la boîte d'un objet.
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
			//	Déplace l'origine gauche d'un objet.
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
			//	Déplace l'origine inférieure d'un objet.
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
			//	Déplace l'origine d'un objet.
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

		protected double GetObjectBaseLine(Widget obj)
		{
			//	Retourne la position relative de la ligne de base depuis le bas de l'objet.
			if (obj is AbstractGroup || obj is StaticText)
			{
				return 0;
			}

			return 6;  // TODO: faire mieux !!!
		}

		protected bool IsObjectAnchorLeft(Widget obj)
		{
			//	Indique si l'objet est ancré à gauche.
			return (obj.Anchor & AnchorStyles.Left) != 0;
		}

		protected bool IsObjectAnchorRight(Widget obj)
		{
			//	Indique si l'objet est ancré à gauche.
			return (obj.Anchor & AnchorStyles.Right) != 0;
		}

		protected bool IsObjectAnchorBottom(Widget obj)
		{
			//	Indique si l'objet est ancré à gauche.
			return (obj.Anchor & AnchorStyles.Bottom) != 0;
		}

		protected bool IsObjectAnchorTop(Widget obj)
		{
			//	Indique si l'objet est ancré à gauche.
			return (obj.Anchor & AnchorStyles.Top) != 0;
		}

		protected bool ObjectTabActive(Widget obj)
		{
			//	Indique si l'objet est ancré à gauche.
			return (obj.TabNavigation & TabNavigationMode.ActivateOnTab) != 0;
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
				graphics.RenderSolid(this.colorOutsurface);
			}

			if (bounds.Right < box.Right)
			{
				Rectangle part = new Rectangle(bounds.Right, box.Bottom, box.Right-bounds.Right, bounds.Height);
				graphics.AddFilledRectangle(part);
				graphics.RenderSolid(this.colorOutsurface);
			}

			//	Dessine la grille magnétique
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

			//	Dessine les objets sélectionnés.
			if (this.selectedObjects.Count > 0 && !this.isDragging)
			{
				foreach (Widget obj in this.selectedObjects)
				{
					Rectangle rect = this.GetObjectBounds(obj);
					rect.Deflate(0.5);

					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(PanelEditor.HiliteSurfaceColor);

					graphics.LineWidth = 3;
					graphics.AddRectangle(rect);
					graphics.RenderSolid(PanelEditor.HiliteOutlineColor);
					graphics.LineWidth = 1;
				}
			}

			//	Dessine les ancrages.
			if (this.context.ShowAnchor && this.selectedObjects.Count > 0 && !this.isDragging)
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
			this.ConstrainDraw(graphics, bounds);

			//	Dessine les numéros d'ordre.
			if (this.context.ShowZOrder && !this.isDragging)
			{
				foreach (Widget obj in this.panel.Children)
				{
					Rectangle rect = this.GetObjectBounds(obj);
					box = new Rectangle(rect.BottomLeft+new Point(1, 1), new Size(12, 10));

					graphics.AddFilledRectangle(box);
					graphics.RenderSolid(Color.FromBrightness(1));

					string text = (obj.ZOrder+1).ToString();
					graphics.AddText(box.Left, box.Bottom, box.Width, box.Height, text, Font.DefaultFont, 9.0, ContentAlignment.MiddleCenter);
					graphics.RenderSolid(this.colorZOrder);
				}
			}

			//	Dessine les numéros d'index pour la touche Tab.
			if (this.context.ShowTabIndex && !this.isDragging)
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
						graphics.RenderSolid(this.colorTabIndex);
					}
				}
			}

			//	Dessine l'objet survolé.
			if (!this.hilitedRectangle.IsEmpty)
			{
				graphics.AddFilledRectangle(this.hilitedRectangle);
				graphics.RenderSolid(PanelEditor.HiliteSurfaceColor);
			}

			//	Dessine le rectangle de sélection.
			if (!this.selectedRectangle.IsEmpty)
			{
				Rectangle sel = this.selectedRectangle;
				sel.Deflate(0.5);
				graphics.AddRectangle(sel);
				graphics.RenderSolid(PanelEditor.HiliteOutlineColor);
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
			//	Couleur lorsqu'un objet est survolé par la souris.
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
				return Color.FromColor(adorner.ColorCaption, 0.8);
			}
		}

		protected static Color HiliteSurfaceColor
		{
			//	Couleur lorsqu'un objet est survolé par la souris.
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
				return Color.FromColor(adorner.ColorCaption, 0.4);
			}
		}
		#endregion


		#region IPaintFilter Members
		bool IPaintFilter.IsWidgetFullyDiscarded(Widget widget)
		{
			//	Retourne true pour indiquer que le widget en question ne doit
			//	pas être peint, ni ses enfants d'ailleurs. Ceci évite que les
			//	widgets sélectionnés ne soient peints.
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


		#region Constrain
		protected void ConstrainStart(Rectangle initialRectangle, double baseLine)
		{
			//	Début des contraintes.
			if (!this.context.ShowConstrain)
			{
				return;
			}

			this.constrainInitialRectangle = initialRectangle;
			this.constrainInitialBaseLine = baseLine;
			this.isConstrainObjectLock = false;
			this.constrainList.Clear();
			this.isConstrainStarted = true;
		}

		protected void ConstrainEnd()
		{
			//	Fin des contraintes.
			if (this.constrainList.Count != 0)
			{
				this.constrainList.Clear();
				this.Invalidate();
			}

			this.isConstrainStarted = false;
		}

		protected void ConstrainLock()
		{
			//	Vérouille ou dévérouille l'objet le plus proche.
			if (!this.isConstrainStarted)
			{
				return;
			}

			this.isConstrainObjectLock = !this.isConstrainObjectLock;

			if (!this.isConstrainObjectLock)
			{
				this.constrainList.Clear();
				this.Invalidate();
			}
		}

		protected void ConstrainActivate(Rectangle rect, double baseLine, params Widget[] excludes)
		{
			//	Active les contraintes pour un rectangle donné.
			if (!this.isConstrainStarted)
			{
				return;
			}

			if (!this.isConstrainObjectLock)
			{
				this.constrainList.Clear();
				double minX, minY;
				this.ConstrainNearestDistance(rect, out minX, out minY, excludes);
				this.ConstrainNearestObjects(rect, minX, minY, excludes);
			}

			foreach (Constrain constrain in this.constrainList)
			{
				constrain.IsActivate = false;
			}

			List<Constrain> bestX, bestY;
			this.ConstrainBest(rect, baseLine, out bestX, out bestY);

			foreach (Constrain best in bestX)
			{
				best.IsActivate = true;
			}

			foreach (Constrain best in bestY)
			{
				best.IsActivate = true;
			}
		}

		protected void ConstrainNearestDistance(Rectangle rect, out double minX, out double minY, params Widget[] excludes)
		{
			//	Cherche la distance à l'objet le plus proche d'une position donnée.
			Point center = rect.Center;
			minX = 1000000;
			minY = 1000000;
			double distance;

			foreach (Widget obj in this.panel.Children)
			{
				if (!this.ConstrainContain(excludes, obj))
				{
					Rectangle bounds = this.GetObjectBounds(obj);

					distance = System.Math.Abs(bounds.Center.X-center.X);
					if (minX > distance)
					{
						minX = distance;
					}

					distance = System.Math.Abs(bounds.Center.Y-center.Y);
					if (minY > distance)
					{
						minY = distance;
					}
				}
			}
		}

		protected void ConstrainNearestObjects(Rectangle rect, double distanceX, double distanceY, params Widget[] excludes)
		{
			//	Initialise les contraintes pour tous les objets dont la distance est
			//	inférieure ou égale à une distance donnée.
			Point center = rect.Center;
			double distance;
			Constrain constrain;

			//	Ajoute les contraintes du conteneur parent.
			Rectangle box = this.RealBounds;
			if (!box.IsEmpty)
			{
				box.Deflate(this.panel.Padding);

				constrain = new Constrain(box.BottomLeft, Constrain.Type.Left, this.context.ConstrainMargin);
				constrain.IsLimit = true;
				this.ConstrainAdd(constrain);

				constrain = new Constrain(box.BottomRight, Constrain.Type.Right, this.context.ConstrainMargin);
				constrain.IsLimit = true;
				this.ConstrainAdd(constrain);

				constrain = new Constrain(box.BottomLeft, Constrain.Type.Bottom, this.context.ConstrainMargin);
				constrain.IsLimit = true;
				this.ConstrainAdd(constrain);

				constrain = new Constrain(box.TopLeft, Constrain.Type.Top, this.context.ConstrainMargin);
				constrain.IsLimit = true;
				this.ConstrainAdd(constrain);
			}

			if (!this.constrainInitialRectangle.IsEmpty)
			{
				//	Ajoute les contraintes correspondant au rectangle initial.
				if (System.Math.Abs(rect.Left-this.constrainInitialRectangle.Left) <= this.context.ConstrainMargin)
				{
					constrain = new Constrain(this.constrainInitialRectangle.BottomLeft, Constrain.Type.Left, this.context.ConstrainMargin);
					this.ConstrainAdd(constrain);

					constrain = new Constrain(this.constrainInitialRectangle.BottomRight, Constrain.Type.Right, this.context.ConstrainMargin);
					this.ConstrainAdd(constrain);
				}

				if (System.Math.Abs(rect.Bottom-this.constrainInitialRectangle.Bottom) <= this.context.ConstrainMargin)
				{
					constrain = new Constrain(this.constrainInitialRectangle.BottomLeft, Constrain.Type.Bottom, this.context.ConstrainMargin);
					this.ConstrainAdd(constrain);

					constrain = new Constrain(this.constrainInitialRectangle.TopLeft, Constrain.Type.Top, this.context.ConstrainMargin);
					this.ConstrainAdd(constrain);
				}
			}

			foreach (Widget obj in this.panel.Children)
			{
				if (!this.ConstrainContain(excludes, obj))
				{
					Rectangle bounds = this.GetObjectBounds(obj);

					distance = System.Math.Abs(bounds.Center.X-center.X);
					if (distance <= distanceX)
					{
						this.ConstrainInitialise(obj);
					}

					distance = System.Math.Abs(bounds.Center.Y-center.Y);
					if (distance <= distanceY)
					{
						this.ConstrainInitialise(obj);
					}
				}
			}
		}

		protected bool ConstrainContain(Widget[] list, Widget searched)
		{
			if (list != null)
			{
				foreach (Widget obj in list)
				{
					if (obj == searched)
					{
						return true;
					}
				}
			}
			return false;
		}

		protected void ConstrainInitialise(Widget obj)
		{
			//	Initialise les contraintes pour un objet.
			Rectangle bounds = this.GetObjectBounds(obj);
			Constrain constrain;

			constrain = new Constrain(bounds.BottomLeft, Constrain.Type.Left, this.context.ConstrainMargin);
			this.ConstrainAdd(constrain);

			constrain = new Constrain(bounds.BottomLeft-this.context.ConstrainSpacing, Constrain.Type.Right, this.context.ConstrainMargin);
			this.ConstrainAdd(constrain);

			constrain = new Constrain(bounds.BottomRight, Constrain.Type.Right, this.context.ConstrainMargin);
			this.ConstrainAdd(constrain);

			constrain = new Constrain(bounds.BottomRight+this.context.ConstrainSpacing, Constrain.Type.Left, this.context.ConstrainMargin);
			this.ConstrainAdd(constrain);

			constrain = new Constrain(bounds.BottomLeft, Constrain.Type.Bottom, this.context.ConstrainMargin);
			this.ConstrainAdd(constrain);

			constrain = new Constrain(bounds.TopLeft, Constrain.Type.Top, this.context.ConstrainMargin);
			this.ConstrainAdd(constrain);

			if (obj is AbstractGroup)
			{
				constrain = new Constrain(bounds.BottomLeft-this.context.ConstrainSpacing, Constrain.Type.Top, this.context.ConstrainMargin);
				this.ConstrainAdd(constrain);

				constrain = new Constrain(bounds.TopLeft+this.context.ConstrainSpacing, Constrain.Type.Bottom, this.context.ConstrainMargin);
				this.ConstrainAdd(constrain);

				bounds.Deflate(this.context.ConstrainGroupMargins);

				constrain = new Constrain(bounds.BottomLeft, Constrain.Type.Left, this.context.ConstrainMargin);
				this.ConstrainAdd(constrain);

				constrain = new Constrain(bounds.BottomRight, Constrain.Type.Right, this.context.ConstrainMargin);
				this.ConstrainAdd(constrain);

				constrain = new Constrain(bounds.BottomLeft, Constrain.Type.Bottom, this.context.ConstrainMargin);
				this.ConstrainAdd(constrain);

				constrain = new Constrain(bounds.TopLeft, Constrain.Type.Top, this.context.ConstrainMargin);
				this.ConstrainAdd(constrain);
			}
			else
			{
				Point baseLine = bounds.BottomLeft;
				baseLine.Y += this.GetObjectBaseLine(obj);
				constrain = new Constrain(baseLine, Constrain.Type.BaseLine, this.context.ConstrainMargin);
				this.ConstrainAdd(constrain);

				baseLine.Y += this.context.Leading;
				constrain = new Constrain(baseLine, Constrain.Type.BaseLine, this.context.ConstrainMargin);
				this.ConstrainAdd(constrain);

				baseLine.Y -= this.context.Leading*2;
				constrain = new Constrain(baseLine, Constrain.Type.BaseLine, this.context.ConstrainMargin);
				this.ConstrainAdd(constrain);
			}

			this.Invalidate();
		}

		protected void ConstrainAdd(Constrain toAdd)
		{
			//	Ajoute une contrainte dans une liste, si elle n'y est pas déjà.
			foreach (Constrain constrain in this.constrainList)
			{
				if (constrain.IsEqualTo(toAdd))
				{
					return;
				}
			}

			this.constrainList.Add(toAdd);
		}

		protected Rectangle ConstrainSnap(Rectangle rect, double baseLine)
		{
			//	Adapte un rectangle en fonction de l'ensemble des contraintes.
			if (this.isConstrainStarted)
			{
				List<Constrain> bestX, bestY;
				this.ConstrainBest(rect, baseLine, out bestX, out bestY);

				if (bestX.Count > 0)
				{
					rect = bestX[0].Snap(rect, baseLine);
				}

				if (bestY.Count > 0)
				{
					rect = bestY[0].Snap(rect, baseLine);
				}
			}

			return rect;
		}

		protected void ConstrainBest(Rectangle rect, double baseLine, out List<Constrain> bestListX, out List<Constrain> bestListY)
		{
			//	Cherches les contraintes les plus pertinentes parmi l'ensemble des contraintes.
			double adjust;
			double minX = 1000000;
			double minY = 1000000;

			foreach (Constrain constrain in this.constrainList)
			{
				if (constrain.AdjustX(rect, out adjust))
				{
					adjust = System.Math.Abs(adjust);
					if (minX > adjust)
					{
						minX = adjust;
					}
				}

				if (constrain.AdjustY(rect, baseLine, out adjust))
				{
					adjust = System.Math.Abs(adjust);
					if (minY > adjust)
					{
						minY = adjust;
					}
				}
			}

			bestListX = new List<Constrain>();
			bestListY = new List<Constrain>();

			foreach (Constrain constrain in this.constrainList)
			{
				if (constrain.AdjustX(rect, out adjust))
				{
					if (System.Math.Abs(adjust) <= minX)
					{
						bestListX.Add(constrain);
					}
				}

				if (constrain.AdjustY(rect, baseLine, out adjust))
				{
					if (System.Math.Abs(adjust) <= minY)
					{
						bestListY.Add(constrain);
					}
				}
			}
		}

		protected void ConstrainDraw(Graphics graphics, Rectangle box)
		{
			//	Dessine toutes les contraintes.
			if (this.isConstrainStarted)
			{
				foreach (Constrain constrain in this.constrainList)
				{
					constrain.Draw(graphics, box);
				}
			}
		}

		protected class Constrain
		{
			public enum Type
			{
				Left,		// contrainte verticale à gauche
				Right,		// contrainte verticale à droite
				Bottom,		// contrainte horizontale en bas
				Top,		// contrainte horizontale en haut
				BaseLine,	// contrainte horizontale sur la ligne de base
			}

			public Constrain(Point position, Type type, double margin)
			{
				this.position   = position;
				this.type       = type;
				this.margin     = margin;
				this.isLimit    = false;
				this.isActivate = false;
			}

			public bool IsLimit
			{
				get
				{
					return this.isLimit;
				}
				set
				{
					this.isLimit = value;
				}
			}

			public bool IsVertical
			{
				get
				{
					return (this.type == Type.Left || this.type == Type.Right);
				}
			}

			public bool IsLeft
			{
				get
				{
					return (this.type == Type.Left);
				}
			}

			public bool IsRight
			{
				get
				{
					return (this.type == Type.Right);
				}
			}

			public bool IsBottom
			{
				get
				{
					return (this.type == Type.Bottom);
				}
			}

			public bool IsTop
			{
				get
				{
					return (this.type == Type.Top);
				}
			}

			public bool IsBaseLine
			{
				get
				{
					return (this.type == Type.BaseLine);
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

				Color color = PanelEditor.HiliteOutlineColor;
				if (this.isLimit)
				{
					color = Color.FromAlphaRgb(0.7, 1,0,0);  // rouge transparent
				}
				if (!this.isActivate)
				{
					color.A *= 0.2;  // plus transparent s'il s'agit d'une contrainte inactive
				}
				graphics.RenderSolid(color);
			}

			public bool Detect(Point position)
			{
				//	Détecte si une position est proche d'une contrainte.
				if (this.IsVertical)
				{
					if (System.Math.Abs(position.X-this.position.X) <= this.margin)
					{
						return true;
					}
				}
				else
				{
					if (System.Math.Abs(position.Y-this.position.Y) <= this.margin)
					{
						return true;
					}
				}

				return false;
			}

			public Rectangle Snap(Rectangle rect, double baseLine)
			{
				//	Adapte un rectangle à une contrainte.
				if (this.IsVertical)
				{
					double adjust;
					this.AdjustX(rect, out adjust);
					rect.Offset(adjust, 0);
				}
				else
				{
					double adjust;
					this.AdjustY(rect, baseLine, out adjust);
					rect.Offset(0, adjust);
				}

				return rect;
			}

			public bool AdjustX(Rectangle rect, out double adjust)
			{
				//	Calcule l'ajustement horizontal nécessaire pour s'adapter à une contrainte.
				if (this.IsLeft && this.Detect(rect.BottomLeft))
				{
					adjust = this.position.X-rect.Left;
					return true;
				}

				if (this.IsRight && this.Detect(rect.BottomRight))
				{
					adjust = this.position.X-rect.Right;
					return true;
				}

				adjust = 0;
				return false;
			}

			public bool AdjustY(Rectangle rect, double baseLine, out double adjust)
			{
				//	Calcule l'ajustement vertical nécessaire pour s'adapter à une contrainte.
				if (this.IsBottom && this.Detect(rect.BottomLeft))
				{
					adjust = this.position.Y-rect.Bottom;
					return true;
				}

				if (this.IsTop && this.Detect(rect.TopLeft))
				{
					adjust = this.position.Y-rect.Top;
					return true;
				}

				if (this.IsBaseLine && this.Detect(new Point(0, rect.Bottom+baseLine)))
				{
					adjust = this.position.Y-(rect.Bottom+baseLine);
					return true;
				}

				adjust = 0;
				return false;
			}

			protected Point					position;
			protected Type					type;
			protected double				margin;
			protected bool					isLimit;
			protected bool					isActivate;
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
		protected Color						colorOutsurface = Color.FromAlphaRgb(0.2, 0.5, 0.5, 0.5);
		protected Color						colorZOrder = Color.FromRgb(1,0,0);
		protected Color						colorTabIndex = Color.FromRgb(0,0,1);
		protected Color						colorAnchor = Color.FromRgb(1,0,0);
		protected Color						colorGrid1 = Color.FromAlphaRgb(0.2, 0.4, 0.4, 0.4);
		protected Color						colorGrid2 = Color.FromAlphaRgb(0.2, 0.7, 0.7, 0.7);
		protected bool						isConstrainStarted;
		protected bool						isConstrainObjectLock;
		protected Rectangle					constrainInitialRectangle;
		protected double					constrainInitialBaseLine;
		protected List<Constrain>			constrainList = new List<Constrain>();
		protected MouseCursorType			lastCursor = MouseCursorType.Unknow;

		protected Image						mouseCursorArrow = null;
		protected Image						mouseCursorArrowPlus = null;
		protected Image						mouseCursorGlobal = null;
		protected Image						mouseCursorEdit = null;
		protected Image						mouseCursorPen = null;
		protected Image						mouseCursorZoom = null;
		protected Image						mouseCursorHand = null;
	}
}
