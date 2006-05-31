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


		public Module					Module
		{
			get
			{
				return this.module;
			}
			set
			{
				this.module = value;
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

		public ConstrainsList			ConstrainsList
		{
			get
			{
				return this.constrainsList;
			}
		}

		public List<Widget>				SelectedObjects
		{
			get
			{
				return this.selectedObjects;
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
			//	Donne des informations sur la sélection en cours.
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

			this.startingPos = pos;
			this.isDragging = false;
			this.isRectangling = false;

			if (this.HandlingStart(pos))
			{
				return;
			}

			Widget obj;
			AnchorStyles style;
			if (this.AnchorDetect(pos, out obj, out style))
			{
				this.SetObjectAnchor(obj, style);  // modifie les ressorts
				return;
			}

			obj = this.Detect(pos);  // objet visé par la souris

			if (!isShiftPressed)  // touche Shift relâchée ?
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
				this.SetAnchorRectangle(Rectangle.Empty);
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
			//	Sélection ponctuelle, souris déplacée.
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
				this.HandlingMove(pos);
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

				Rectangle anchor = Rectangle.Empty;
				AnchorStyles style;
				if (this.AnchorDetect(pos, out obj, out style))
				{
					anchor = this.GetAnchorBounds(obj, style);
					anchor.Offset(0.5, 0.5);
					anchor.Inflate(3);
				}
				this.SetAnchorRectangle(anchor);  // met en évidence le ressort survolé par la souris
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

			if (this.handlesList.IsDragging)
			{
				this.HandlingEnd();
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

			this.startingPos = pos;
			this.isDragging = false;
			this.isRectangling = false;

			if (this.HandlingStart(pos))
			{
				return;
			}

			Widget obj = this.Detect(pos);  // objet visé par la souris

			if (!isShiftPressed)  // touche Shift relâchée ?
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
			this.SetAnchorRectangle(Rectangle.Empty);

			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void GlobalMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection rectangulaire, souris déplacée.
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
				this.HandlingMove(pos);
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

			if (this.handlesList.IsDragging)
			{
				this.HandlingEnd();
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

			Widget obj = this.Detect(pos);
			Rectangle rect = Rectangle.Empty;
			if (obj != null)
			{
				rect = this.GetObjectBounds(obj);
			}
			this.SetHiliteRectangle(rect);  // met en évidence l'objet survolé par la souris
		}

		protected void EditUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Edition, souris relâchée.
			Widget obj = this.Detect(pos);
			if (obj != null)
			{
				this.ChangeTextRessource(obj);
			}
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

			this.constrainsList.Starting(Rectangle.Empty, false);
			this.constrainsList.Activate(bounds, this.GetObjectBaseLine(this.creatingObject), null);
		}

		protected void CreateObjectMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, souris déplacée.
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
			//	Dessin d'un objet, souris relâchée.
			if (this.creatingObject != null)
			{
				this.creatingWindow.Hide();
				this.creatingWindow.Dispose();
				this.creatingWindow = null;

				Rectangle bounds;
				this.CreateObjectAdjust(ref pos, out bounds);

				Widget parent = this.DetectGroup(bounds);

				this.creatingObject = this.CreateObjectItem();
				this.creatingObject.SetParent(parent);
				this.creatingObject.Anchor = AnchorStyles.BottomLeft;
				this.creatingObject.TabNavigation = TabNavigationMode.Passive;

				this.SetObjectPosition(this.creatingObject, pos);

				this.constrainsList.Ending();

				this.lastCreatedObject = this.creatingObject;
				this.creatingObject = null;
				this.OnUpdateCommands();

				this.ChangeTextRessource(this.lastCreatedObject);
			}
		}

		protected Widget CreateObjectItem()
		{
			//	Crée un objet selon la palette d'outils.
			Widget item = null;

			if (this.context.Tool == "ObjectHLine")
			{
				item = new Separator();
				item.PreferredWidth = 100;
				item.PreferredHeight = 1;
				item.MinWidth = 10;
				item.MinHeight = 1;
			}

			if (this.context.Tool == "ObjectVLine")
			{
				item = new Separator();
				item.PreferredWidth = 1;
				item.PreferredHeight = 100;
				item.MinWidth = 1;
				item.MinHeight = 10;
			}

			if (this.context.Tool == "ObjectStatic")
			{
				item = new StaticText();
				item.Text = Misc.Italic("StaticText");
				item.MinWidth = 10;
				item.MinHeight = 10;
			}

			if (this.context.Tool == "ObjectButton")
			{
				item = new Button();
				item.Text = Misc.Italic("Button");
				item.MinWidth = 20;
				item.MinHeight = 10;
			}

			if (this.context.Tool == "ObjectText")
			{
				item = new TextField();
				item.Text = Misc.Italic("TextField");
				item.MinWidth = 20;
				item.MinHeight = 10;
			}

			if (this.context.Tool == "ObjectGroup")
			{
				item = new GroupBox();
				item.Text = Misc.Italic("GroupBox");
				item.PreferredSize = new Size(200, 100);
				item.MinWidth = 50;
				item.MinHeight = 50;
			}

			return item;
		}

		protected void CreateObjectAdjust(ref Point pos, out Rectangle bounds)
		{
			//	Ajuste la position de l'objet à créer selon les contraintes.
			pos.X -= System.Math.Floor(this.creatingObject.PreferredWidth/2);
			pos.Y -= System.Math.Floor(this.creatingObject.PreferredHeight/2);
			bounds = new Rectangle(pos, this.creatingObject.PreferredSize);
			Rectangle adjust = this.constrainsList.Snap(bounds, this.GetObjectBaseLine(this.creatingObject));
			Point corr = adjust.BottomLeft - bounds.BottomLeft;
			pos += corr;
			bounds.Offset(corr);
		}

		protected void CreateObjectKeyChanged(bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, touche pressée ou relâchée.
		}
		#endregion


		protected void ChangeTextRessource(Widget obj)
		{
			//	Choix de la ressource de type texte pour l'objet.
			//	TODO: tout ceci est provisoire !!!
			if (obj is Button || obj is StaticText || obj is GroupBox)
			{
				obj.Name = this.module.MainWindow.DlgTextSelector(obj.Name);

				if (!string.IsNullOrEmpty(obj.Name))
				{
					ResourceBundleCollection bundles = this.module.Bundles;
					ResourceBundle bundle = bundles[ResourceLevel.Default];
					ResourceBundle.Field field = bundle[obj.Name];
					if (field != null)
					{
						obj.Text = field.AsString;
					}
				}
			}
		}


		#region Handling
		protected bool HandlingStart(Point pos)
		{
			//	Début du drag pour déplacer une poignée.
			this.handlesList.DraggingStart(pos);
			if (this.handlesList.IsDragging)
			{
				this.handlesList.DraggingMove(pos);
				this.SetHiliteRectangle(Rectangle.Empty);
				this.SetAnchorRectangle(Rectangle.Empty);
				return true;
			}

			return false;
		}

		protected void HandlingMove(Point pos)
		{
			//	Mouvement du drag pour déplacer une poignée.
			this.handlesList.DraggingMove(pos);
		}

		protected void HandlingEnd()
		{
			//	Fin du drag pour déplacer une poignée.
			this.handlesList.DraggingStop();
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
			this.constrainsList.Starting(this.draggingRectangle, false);

			Widget container = new Widget();
			container.PreferredSize = this.draggingRectangle.Size;

			foreach (Widget obj in this.selectedObjects)
			{
				Point origin = this.GetObjectPosition(obj)-this.draggingRectangle.BottomLeft;
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
			this.SetAnchorRectangle(Rectangle.Empty);
			this.isDragging = true;
			this.Invalidate();
		}

		protected void DraggingMove(Point pos)
		{
			//	Mouvement du drag pour déplacer les objets sélectionnés.
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
			//	Fin du drag pour déplacer les objets sélectionnés.
			this.draggingWindow.Hide();
			this.draggingWindow.Dispose();
			this.draggingWindow = null;

			Rectangle initial = this.SelectBounds;
			this.MoveSelection(this.draggingRectangle.BottomLeft - initial.BottomLeft);
			this.isDragging = false;
			this.draggingArraySelected = null;
			this.constrainsList.Ending();
			this.handlesList.UpdateGeometry();
			this.Invalidate();
		}
		#endregion


		#region Selection
		protected Widget Detect(Point pos)
		{
			//	Détecte l'objet visé par la souris, avec priorité au dernier objet
			//	dessiné (donc placé dessus).
			return this.panel.FindChild(pos, ChildFindMode.Deep | ChildFindMode.SkipHidden | ChildFindMode.SkipEmbedded);
		}

		protected Widget DetectGroup(Rectangle rect)
		{
			//	Détecte dans quel groupe est entièrement inclu un rectangle donné.
			Widget container = this.panel.FindChild(rect, ChildFindMode.Deep | ChildFindMode.SkipHidden | ChildFindMode.SkipNonContainer | ChildFindMode.SkipEmbedded);
			return container ?? this.panel;
		}

		public void DeselectAll()
		{
			//	Désélectionne tous les objets.
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
			//	Sélectionne tous les objets.
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

			this.UpdateAfterSelectionChanged();
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
			this.UpdateAfterSelectionChanged();
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

			this.UpdateAfterSelectionChanged();
			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void UpdateAfterSelectionChanged()
		{
			//	Mise à jour après un changement de sélection.
			this.handlesList.UpdateSelection();
		}

		protected void SetHiliteRectangle(Rectangle rect)
		{
			//	Détermine la zone à mettre en évidence lors d'un survol.
			if (!rect.IsEmpty)
			{
				double ix = 0;
				if (rect.Width < this.context.MinimalSize)
				{
					ix = this.context.MinimalSize;
				}

				double iy = 0;
				if (rect.Height < this.context.MinimalSize)
				{
					iy = this.context.MinimalSize;
				}

				rect.Inflate(ix, iy);
			}

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

		protected void SetAnchorRectangle(Rectangle rect)
		{
			//	Détermine la zone du rectangle de ressort d'ancrage.
			if (this.anchorRectangle != rect)
			{
				this.Invalidate(this.anchorRectangle);  // invalide l'ancienne zone
				this.anchorRectangle = rect;
				this.Invalidate(this.anchorRectangle);  // invalide la nouvelle zone
			}
		}
		#endregion


		#region Operations
		protected void DeleteSelection()
		{
			//	Supprime tous les objets sélectionnés.
			foreach (Widget obj in this.selectedObjects)
			{
				//?this.panel.Children.Remove(obj);
				obj.Parent.Children.Remove(obj);
				obj.Dispose();
			}

			this.selectedObjects.Clear();
			this.UpdateAfterSelectionChanged();
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
						this.SetObjectPositionY(obj, System.Math.Floor(bounds.Center.Y-this.GetObjectSize(obj).Height/2));
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
						this.SetObjectPositionX(obj, System.Math.Floor(bounds.Center.X-this.GetObjectSize(obj).Width/2));
					}
				}
			}

			this.Invalidate();
		}

		protected void SelectAlignBaseLine()
		{
			//	Aligne sur la ligne de base tous les objets sélectionnés.
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
			this.SelectTabIndexRenum(this.panel);

			this.Invalidate();
			this.context.ShowTabIndex = true;
			this.OnUpdateCommands();
		}

		protected void SelectTabIndexRenum(Widget parent)
		{
			List<Widget> list = new List<Widget>();
			foreach (Widget obj in parent.Children)
			{
				list.Add(obj);
			}
			list.Sort(new Comparers.WidgetDisposition());

			int index = 0;
			foreach (Widget obj in list)
			{
				obj.TabIndex = index++;
				obj.TabNavigation = TabNavigationMode.ActivateOnTab;

				if (obj is AbstractGroup)
				{
					this.SelectTabIndexRenum(obj);
				}
			}
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
					index = System.Math.Min(index, obj.Parent.Children.Count-1);
					this.SelectTabIndex(obj.Parent, index, oldIndex);
					obj.TabIndex = index;
					obj.TabNavigation = TabNavigationMode.ActivateOnTab;
				}
			}

			this.Invalidate();
			this.context.ShowTabIndex = true;
			this.OnUpdateCommands();
		}

		protected void SelectTabIndex(Widget parent, int oldIndex, int newIndex)
		{
			foreach (Widget obj in parent.Children)
			{
				if (this.IsObjectTabActive(obj) && obj.TabIndex == oldIndex)
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


		protected Point GetObjectPosition(Widget obj)
		{
			//	Retourne l'origine d'un objet.
			return this.GetObjectBounds(obj).BottomLeft;
		}

		protected Size GetObjectSize(Widget obj)
		{
			//	Retourne les dimensions d'un objet.
			return this.GetObjectBounds(obj).Size;
		}

		protected void SetObjectPosition(Widget obj, Point pos)
		{
			//	Déplace l'origine d'un objet.
			Rectangle bounds = new Rectangle(pos, this.GetObjectSize(obj));
			this.SetObjectBounds(obj, bounds);
		}

		protected void SetObjectPositionX(Widget obj, double x)
		{
			//	Déplace l'origine gauche d'un objet.
			Rectangle bounds = this.GetObjectBounds(obj);
			double w = bounds.Width;
			bounds.Left = x;
			bounds.Width = w;
			this.SetObjectBounds(obj, bounds);
		}

		protected void SetObjectPositionY(Widget obj, double y)
		{
			//	Déplace l'origine inférieure d'un objet.
			Rectangle bounds = this.GetObjectBounds(obj);
			double h = bounds.Height;
			bounds.Bottom = y;
			bounds.Height = h;
			this.SetObjectBounds(obj, bounds);
		}

		protected void SetObjectWidth(Widget obj, double dx)
		{
			//	Modifie la largeur d'un objet.
			Rectangle bounds = this.GetObjectBounds(obj);
			bounds.Width = dx;
			this.SetObjectBounds(obj, bounds);
		}

		protected void SetObjectHeight(Widget obj, double dy)
		{
			//	Modifie la hauteur d'un objet.
			Rectangle bounds = this.GetObjectBounds(obj);
			bounds.Height = dy;
			this.SetObjectBounds(obj, bounds);
		}

		protected void SetObjectSize(Widget obj, Size size)
		{
			//	Modifie les dimensions d'un objet.
			Rectangle bounds = this.GetObjectBounds(obj);
			bounds.Size = size;
			this.SetObjectBounds(obj, bounds);
		}

		public Rectangle GetObjectBounds(Widget obj)
		{
			//	Retourne la boîte d'un objet.
			//	Les coordonnées sont toujours relative au panneau (this.panel) propriétaire.
			this.Window.ForceLayout();
			Rectangle bounds = obj.ActualBounds;

			Widget parent = obj.Parent;
			while (parent != this.panel)
			{
				bounds = parent.MapClientToParent(bounds);
				parent = parent.Parent;
			}

			return bounds;
		}

		public void SetObjectBounds(Widget obj, Rectangle bounds)
		{
			//	Modifie la boîte d'un objet.
			//	Les coordonnées sont toujours relative au panneau (this.panel) propriétaire.
			bounds.Normalise();

			if (bounds.Width < obj.MinWidth)
			{
				bounds.Width = obj.MinWidth;
			}

			if (bounds.Height < obj.MinHeight)
			{
				bounds.Height = obj.MinHeight;
			}

			Widget parent = obj.Parent;
			while (parent != this.panel)
			{
				bounds = parent.MapParentToClient(bounds);
				parent = parent.Parent;
			}

			Rectangle box = this.RealBounds;
			Margins margins = obj.Margins;

			if (this.IsObjectAnchorLeft(obj))
			{
				double px = bounds.Left;
				px -= this.panel.Padding.Left;
				px = System.Math.Max(px, 0);
				margins.Left = px;
			}

			if (this.IsObjectAnchorRight(obj))
			{
				double px = box.Right - bounds.Right;
				px -= this.panel.Padding.Right;
				px = System.Math.Max(px, 0);
				margins.Right = px;
			}

			if (this.IsObjectAnchorBottom(obj))
			{
				double py = bounds.Bottom;
				py -= this.panel.Padding.Bottom;
				py = System.Math.Max(py, 0);
				margins.Bottom = py;
			}

			if (this.IsObjectAnchorTop(obj))
			{
				double py = box.Top - bounds.Top;
				py -= this.panel.Padding.Top;
				py = System.Math.Max(py, 0);
				margins.Top = py;
			}

			obj.Margins = margins;
			obj.PreferredSize = bounds.Size;

			this.Invalidate();
		}

		public double GetObjectBaseLine(Widget obj)
		{
			//	Retourne la position relative de la ligne de base depuis le bas de l'objet.
			return System.Math.Floor(obj.GetBaseLine().Y);
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

		protected void SetObjectAnchor(Widget obj, AnchorStyles style)
		{
			//	Modifie le système d'ancrage d'un objet.
			Rectangle bounds = this.GetObjectBounds(obj);

			if ((obj.Anchor & style) == 0)
			{
				obj.Anchor |= style;
			}
			else
			{
				obj.Anchor &= ~style;

				if ((obj.Anchor & Misc.OppositeAnchor(style)) == 0)
				{
					obj.Anchor |= Misc.OppositeAnchor(style);
				}
			}

			this.SetObjectBounds(obj, bounds);
			this.handlesList.UpdateGeometry();
			this.Invalidate();
		}

		protected bool IsObjectTabActive(Widget obj)
		{
			//	Indique si l'objet est ancré à gauche.
			return (obj.TabNavigation & TabNavigationMode.ActivateOnTab) != 0;
		}

		protected string GetObjectZOrder(Widget obj)
		{
			//	Retourne la chaîne indiquant l'ordre Z, y compris des parents, sous la forme "n.n.n".
			if (obj.Parent == this.panel)
			{
				return (obj.ZOrder+1).ToString();
			}

			List<int> list = new List<int>();
			while (obj != this.panel)
			{
				list.Add(obj.ZOrder);
				obj = obj.Parent;
			}

			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			for (int i=list.Count-1; i>=0; i--)
			{
				if (builder.Length > 0)
				{
					builder.Append(".");
				}

				builder.Append((list[i]+1).ToString());
			}
			return builder.ToString();
		}

		protected string GetObjectTabIndex(Widget obj)
		{
			//	Retourne la chaîne indiquant l'ordre Z, y compris des parents, sous la forme "n.n.n".
			if (obj.Parent == this.panel)
			{
				if (this.IsObjectTabActive(obj))
				{
					return (obj.TabIndex+1).ToString();
				}
				else
				{
					return "x";
				}
			}

			List<string> list = new List<string>();
			while (obj != this.panel)
			{
				if (this.IsObjectTabActive(obj))
				{
					list.Add((obj.TabIndex+1).ToString());
				}
				else
				{
					list.Add("x");
				}

				obj = obj.Parent;
			}

			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			for (int i=list.Count-1; i>=0; i--)
			{
				if (builder.Length > 0)
				{
					builder.Append(".");
				}

				builder.Append(list[i]);
			}
			return builder.ToString();
		}
		#endregion


		#region Anchor
		protected bool AnchorDetect(Point mouse, out Widget obj, out AnchorStyles style)
		{
			//	Détecte dans quel ressort d'un objet est la souris.
			if (!this.context.ShowAnchor)
			{
				obj = null;
				style = AnchorStyles.None;
				return false;
			}

			AnchorStyles[] styles = {AnchorStyles.Left, AnchorStyles.Right, AnchorStyles.Bottom, AnchorStyles.Top};

			foreach (Widget o in this.selectedObjects)
			{
				foreach (AnchorStyles s in styles)
				{
					Rectangle bounds = this.GetAnchorBounds(o, s);
					if (bounds.Contains(mouse))
					{
						obj = o;
						style = s;
						return true;
					}
				}
			}

			obj = null;
			style = AnchorStyles.None;
			return false;
		}

		protected Rectangle GetAnchorBounds(Widget obj, AnchorStyles style)
		{
			//	Retourne le rectangle englobant un ressort d'ancrage.
			Rectangle bounds = this.RealBounds;
			Rectangle rect = this.GetObjectBounds(obj);
			Point p1, p2, p1a, p2a;
			double thickness = PanelEditor.anchorThickness;

			if (style == AnchorStyles.Left)
			{
				p1 = new Point(bounds.Left, rect.Center.Y);
				p2 = new Point(rect.Left, rect.Center.Y);
				p1a = Point.Scale(p1, p2, PanelEditor.anchorScale);
				p2a = Point.Scale(p2, p1, PanelEditor.anchorScale);
				p1a.Y -= thickness;
				p2a.Y += thickness;
				return new Rectangle(p1a, p2a);
			}

			if (style == AnchorStyles.Right)
			{
				p1 = new Point(bounds.Right, rect.Center.Y);
				p2 = new Point(rect.Right, rect.Center.Y);
				p1a = Point.Scale(p1, p2, PanelEditor.anchorScale);
				p2a = Point.Scale(p2, p1, PanelEditor.anchorScale);
				p1a.Y -= thickness;
				p2a.Y += thickness;
				return new Rectangle(p1a, p2a);
			}

			if (style == AnchorStyles.Bottom)
			{
				p1 = new Point(rect.Center.X, bounds.Bottom);
				p2 = new Point(rect.Center.X, rect.Bottom);
				p1a = Point.Scale(p1, p2, PanelEditor.anchorScale);
				p2a = Point.Scale(p2, p1, PanelEditor.anchorScale);
				p1a.X -= thickness;
				p2a.X += thickness;
				return new Rectangle(p1a, p2a);
			}

			if (style == AnchorStyles.Top)
			{
				p1 = new Point(rect.Center.X, bounds.Top);
				p2 = new Point(rect.Center.X, rect.Top);
				p1a = Point.Scale(p1, p2, PanelEditor.anchorScale);
				p2a = Point.Scale(p2, p1, PanelEditor.anchorScale);
				p1a.X -= thickness;
				p2a.X += thickness;
				return new Rectangle(p1a, p2a);
			}

			return Rectangle.Empty;
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
				graphics.RenderSolid(PanelsContext.ColorOutsurface);
			}

			if (bounds.Right < box.Right)
			{
				Rectangle part = new Rectangle(bounds.Right, box.Bottom, box.Right-bounds.Right, bounds.Height);
				graphics.AddFilledRectangle(part);
				graphics.RenderSolid(PanelsContext.ColorOutsurface);
			}

			//	Dessine la grille magnétique
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

			//	Dessine les objets sélectionnés.
			if (this.selectedObjects.Count > 0 && !this.isDragging && !this.handlesList.IsDragging)
			{
				foreach (Widget obj in this.selectedObjects)
				{
					Rectangle rect = this.GetObjectBounds(obj);
					rect.Deflate(0.5);

					//?graphics.AddFilledRectangle(rect);
					//?graphics.RenderSolid(PanelsContext.ColorHiliteSurface);

					graphics.LineWidth = 3;
					graphics.AddRectangle(rect);
					graphics.RenderSolid(PanelsContext.ColorHiliteOutline);
					graphics.LineWidth = 1;
				}
			}

			//	Dessine les ancrages des objets sélectionnés.
			if (this.context.ShowAnchor && this.selectedObjects.Count == 1 && !this.isDragging && !this.handlesList.IsDragging)
			{
				foreach (Widget obj in this.selectedObjects)
				{
					Rectangle rect = this.GetObjectBounds(obj);
					Point p1, p2;

					p1 = new Point(bounds.Left, rect.Center.Y);
					p2 = new Point(rect.Left, rect.Center.Y);
					this.DrawAnchor(graphics, p1, p2, this.IsObjectAnchorLeft(obj));

					p1 = new Point(bounds.Right, rect.Center.Y);
					p2 = new Point(rect.Right, rect.Center.Y);
					this.DrawAnchor(graphics, p1, p2, this.IsObjectAnchorRight(obj));

					p1 = new Point(rect.Center.X, bounds.Bottom);
					p2 = new Point(rect.Center.X, rect.Bottom);
					this.DrawAnchor(graphics, p1, p2, this.IsObjectAnchorBottom(obj));

					p1 = new Point(rect.Center.X, bounds.Top);
					p2 = new Point(rect.Center.X, rect.Top);
					this.DrawAnchor(graphics, p1, p2, this.IsObjectAnchorTop(obj));
				}
			}

			//	Dessine les contraintes.
			this.constrainsList.Draw(graphics, bounds);

			//	Dessine les numéros d'ordre.
			if (this.context.ShowZOrder && !this.isDragging && !this.handlesList.IsDragging)
			{
				this.DrawZOrder(graphics, this.panel);
			}

			//	Dessine les numéros d'index pour la touche Tab.
			if (this.context.ShowTabIndex && !this.isDragging && !this.handlesList.IsDragging)
			{
				this.DrawTabIndex(graphics, this.panel);
			}

			//	Dessine l'objet survolé.
			if (!this.hilitedRectangle.IsEmpty)
			{
				graphics.AddFilledRectangle(this.hilitedRectangle);
				graphics.RenderSolid(PanelsContext.ColorHiliteSurface);
			}

			//	Dessine le rectangle de sélection.
			if (!this.selectedRectangle.IsEmpty)
			{
				Rectangle sel = this.selectedRectangle;
				sel.Deflate(0.5);
				graphics.AddRectangle(sel);
				graphics.RenderSolid(PanelsContext.ColorHiliteOutline);
			}

			//	Dessine le rectangle de ressort d'ancrage.
			if (!this.anchorRectangle.IsEmpty)
			{
				Rectangle anchor = this.anchorRectangle;
				graphics.AddFilledRectangle(anchor);
				graphics.RenderSolid(PanelsContext.ColorHiliteSurface);
			}

			//	Dessine les poignées.
			if (this.selectedObjects.Count == 1 && !this.isDragging)
			{
				this.handlesList.Draw(graphics);
			}
		}

		protected void DrawAnchor(Graphics graphics, Point p1, Point p2, bool rigid)
		{
			//	Dessine un ancrage horizontal ou vertical d'un objet.
			Point p1a = Point.Scale(p1, p2, PanelEditor.anchorScale);
			Point p2a = Point.Scale(p2, p1, PanelEditor.anchorScale);

			Misc.AlignForLine(graphics, ref p1);
			Misc.AlignForLine(graphics, ref p2);
			Misc.AlignForLine(graphics, ref p1a);
			Misc.AlignForLine(graphics, ref p2a);

			if (rigid)  // rigide ?
			{
				Point delta = (p1.Y == p2.Y) ? new Point(0, 1) : new Point(1, 0);
				graphics.AddLine(p1+delta, p1a+delta);
				graphics.AddLine(p2+delta, p2a+delta);
				graphics.AddLine(p1-delta, p1a-delta);
				graphics.AddLine(p2-delta, p2a-delta);

				double dim = PanelEditor.anchorThickness;
				Misc.AddBox(graphics, p1a, p2a, dim);
			}
			else  // élastique ?
			{
				graphics.AddLine(p1, p1a);
				graphics.AddLine(p2, p2a);

				double dim = PanelEditor.anchorThickness;
				double length = Point.Distance(p1a, p2a);
				int loops = (int) (length/(dim*2));
				loops = System.Math.Max(loops, 1);
				Misc.AddSpring(graphics, p1a, p2a, dim, loops);
			}

			graphics.RenderSolid(PanelsContext.ColorAnchor);

			//	Dessine les extrémités.
			graphics.AddFilledCircle(p1, 3.0);
			graphics.AddFilledCircle(p2, 3.0);
			graphics.RenderSolid(PanelsContext.ColorAnchor);
		}

		protected void DrawZOrder(Graphics graphics, Widget parent)
		{
			//	Dessine les numéros d'ordre d'un groupe.
			foreach (Widget obj in parent.Children)
			{
				Rectangle rect = this.GetObjectBounds(obj);
				Rectangle box = new Rectangle(rect.BottomLeft+new Point(1, 1), new Size(20, 10));

				graphics.AddFilledRectangle(box);
				graphics.RenderSolid(Color.FromBrightness(1));

				string text = this.GetObjectZOrder(obj);
				graphics.AddText(box.Left, box.Bottom, box.Width, box.Height, text, Font.DefaultFont, 9.0, ContentAlignment.MiddleCenter);
				graphics.RenderSolid(PanelsContext.ColorZOrder);

				if (obj is AbstractGroup)
				{
					this.DrawZOrder(graphics, obj);
				}
			}
		}

		protected void DrawTabIndex(Graphics graphics, Widget parent)
		{
			//	Dessine les numéros pour la touche Tab d'un groupe.
			foreach (Widget obj in parent.Children)
			{
				Rectangle rect = this.GetObjectBounds(obj);
				Rectangle box = new Rectangle(rect.BottomRight+new Point(-20-1, 1), new Size(20, 10));

				graphics.AddFilledRectangle(box);
				graphics.RenderSolid(Color.FromBrightness(1));

				string text = this.GetObjectTabIndex(obj);
				graphics.AddText(box.Left, box.Bottom, box.Width, box.Height, text, Font.DefaultFont, 9.0, ContentAlignment.MiddleCenter);
				graphics.RenderSolid(PanelsContext.ColorTabIndex);

				if (obj is AbstractGroup)
				{
					this.DrawTabIndex(graphics, obj);
				}
			}
		}

		public Rectangle RealBounds
		{
			//	Retourne le rectangle englobant de tous les objets contenus dans le panneau.
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


		protected static readonly double	anchorThickness = 3.0;
		protected static readonly double	anchorScale = 0.4;

		protected Module					module;
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
		protected Rectangle					anchorRectangle = Rectangle.Empty;
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
