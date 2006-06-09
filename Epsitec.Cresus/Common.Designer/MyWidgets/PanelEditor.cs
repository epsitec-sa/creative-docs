using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget venant par-dessus le conteneur UI.Panel pour éditer ce dernier.
	/// </summary>
	public class PanelEditor : AbstractGroup, IPaintFilter
	{
		[System.Flags]
		protected enum Attachment
		{
			None	= 0x00000000,	// objet libre
			Left	= 0x00000001,	// objet attaché à gauche
			Right	= 0x00000002,	// objet attaché à droite
			Bottom	= 0x00000004,	// objet attaché en bas
			Top		= 0x00000008,	// objet attaché en haut
		}

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
			//	Module associé.
			set
			{
				this.module = value;
			}

			get
			{
				return this.module;
			}
		}

		public PanelsContext			Context
		{
			//	Contexte asocié.
			//	Le set sert d'initialisation interne (bof).
			set
			{
				this.context = value;
				this.constrainsList = new ConstrainsList(this);
				this.handlesList = new HandlesList(this);
			}

			get
			{
				return this.context;
			}
		}

		public UI.Panel					Panel
		{
			//	Panneau associé qui est le conteneur de tous les widgets.
			//	PanelEditor est frère de Panel et vient par-dessus.
			set
			{
				this.panel = value;
				this.sizeMark = this.panel.PreferredSize;
			}

			get
			{
				return this.panel;
			}
		}

		public bool						IsLayoutAnchored
		{
			//	Indique si le panneau associé est en mode LayoutMode.Anchored.
			get
			{
				return (this.panel.ChildrenLayoutMode == Widgets.Layouts.LayoutMode.Anchored);
			}
		}

		public bool						IsLayoutDocking
		{
			//	Indique si le panneau associé est en mode LayoutMode.Docked.
			get
			{
				return (this.panel.ChildrenLayoutMode == Widgets.Layouts.LayoutMode.Docked);
			}
		}

		public bool						IsHorizontalDocking
		{
			//	Indique si le panneau associé est en mode de docking horizontal.
			get
			{
				return (this.panel.ContainerLayoutMode == ContainerLayoutMode.HorizontalFlow);
			}
		}

		public ConstrainsList ConstrainsList
		{
			//	Retourne la liste des contraintes.
			get
			{
				return this.constrainsList;
			}
		}

		public List<Widget>				SelectedObjects
		{
			//	Retourne la liste des objets sélectionnés.
			get
			{
				return this.selectedObjects;
			}
		}


		public void DoCommand(string name)
		{
			//	Exécute une commande.
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

				case "PanelShowAttachment":
					this.context.ShowAttachment = !this.context.ShowAttachment;
					this.Invalidate();
					this.OnUpdateCommands();
					break;

				case "PanelShowConstrain":
					this.context.ShowConstrain = !this.context.ShowConstrain;
					this.Invalidate();
					this.OnUpdateCommands();
					break;

				case "MoveLeft":
					this.MoveRibbonSelection(new Point(-1, 0));
					break;

				case "MoveRight":
					this.MoveRibbonSelection(new Point(1, 0));
					break;

				case "MoveDown":
					this.MoveRibbonSelection(new Point(0, -1));
					break;

				case "MoveUp":
					this.MoveRibbonSelection(new Point(0, 1));
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

		public string SelectionInfo
		{
			//	Donne le texte pour les statuts.
			get
			{
				string sel = "-";
				Rectangle rect = Rectangle.Empty;
				
				if (this.isDragging)
				{
					rect = this.draggingRectangle;
				}
				else
				{
					rect = this.SelectBounds;
				}

				if (!rect.IsEmpty)
				{
					sel = string.Format(Res.Strings.Viewers.Panels.Rectangle, rect.Left, rect.Bottom, rect.Width, rect.Height);
				}

				int objSelected, objCount;
				this.GetSelectionInfo(out objSelected, out objCount);
				string text = string.Format(Res.Strings.Viewers.Panels.Info, objSelected.ToString(), objCount.ToString(), sel);

				return text;
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

			if (this.SizeMarkDraggingStart(pos))
			{
				return;
			}

			Widget obj;
			Attachment attachment;
			if (this.AttachmentDetect(pos, out obj, out attachment))
			{
				this.ChangeObjectAttachment(obj, attachment);  // modifie les attachements
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
				this.SetHilitedAttachmentRectangle(Rectangle.Empty);
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
			if (this.handlesList.IsFinger || this.isSizeMarkHorizontal || this.isSizeMarkVertical)
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
			else if (this.SizeMarkDraggingMove(pos))
			{
			}
			else
			{
				this.SetHilitedObject(this.Detect(pos));  // met en évidence l'objet survolé par la souris

				Rectangle rect = Rectangle.Empty;
				Widget obj;
				Attachment attachment;
				if (this.AttachmentDetect(pos, out obj, out attachment))
				{
					rect = this.GetAttachmentBounds(obj, attachment);
					rect.Offset(0.5, 0.5);
					rect.Inflate(3);
				}
				this.SetHilitedAttachmentRectangle(rect);  // met en évidence l'attachement survolé par la souris
			}
		}

		protected void SelectUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection ponctuelle, souris relâchée.
			if (this.isDragging)
			{
				this.DraggingEnd(pos);
			}

			if (this.isRectangling)
			{
				this.SelectObjectsInRectangle(this.selectedRectangle);
				this.SetSelectRectangle(Rectangle.Empty);
				this.isRectangling = false;
			}

			if (this.handlesList.IsDragging)
			{
				this.HandlingEnd(pos);
			}

			this.SizeMarkDraggingStop(pos);
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

			if (this.SizeMarkDraggingStart(pos))
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
			this.SetHilitedAttachmentRectangle(Rectangle.Empty);

			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void GlobalMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection rectangulaire, souris déplacée.
			if (this.handlesList.IsFinger || this.isSizeMarkHorizontal || this.isSizeMarkVertical)
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
			else if (this.SizeMarkDraggingMove(pos))
			{
			}
		}

		protected void GlobalUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection rectangulaire, souris relâchée.
			if (this.isDragging)
			{
				this.DraggingEnd(pos);
			}

			if (this.isRectangling)
			{
				this.SelectObjectsInRectangle(this.selectedRectangle);
				this.SetSelectRectangle(Rectangle.Empty);
				this.isRectangling = false;
			}

			if (this.handlesList.IsDragging)
			{
				this.HandlingEnd(pos);
			}

			this.SizeMarkDraggingStop(pos);
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

			this.SetHilitedObject(this.Detect(pos));  // met en évidence l'objet survolé par la souris
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

			if (this.IsLayoutAnchored)
			{
				this.constrainsList.Starting(Rectangle.Empty, false);
				this.constrainsList.Activate(bounds, this.GetObjectBaseLine(this.creatingObject), null);

				this.SetHilitedParent(this.DetectGroup(bounds));  // met en évidence le futur parent survolé par la souris
			}
		}

		protected void CreateObjectMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, souris déplacée.
			this.ChangeMouseCursor(MouseCursorType.Pen);

			if (this.creatingObject != null)
			{
				Point initialPos = pos;
				Rectangle bounds;
				this.CreateObjectAdjust(ref pos, out bounds);

				if (this.IsLayoutAnchored)
				{
					Rectangle rect = this.IsInside(initialPos) ? bounds : Rectangle.Empty;
					this.constrainsList.Activate(rect, this.GetObjectBaseLine(this.creatingObject), null);
					this.creatingWindow.WindowLocation = this.creatingOrigin + pos;

					Widget parent = this.IsInside(initialPos) ? this.DetectGroup(bounds) : null;
					this.SetHilitedParent(parent);  // met en évidence le futur parent survolé par la souris
				}

				if (this.IsLayoutDocking)
				{
					this.creatingWindow.WindowLocation = this.creatingOrigin + pos;

					Widget parent;
					int order;
					Rectangle hilite;
					this.ZOrderDetect(initialPos, out parent, out order, out hilite);
					this.SetHilitedZOrderRectangle(hilite);
					this.SetHilitedParent(parent);  // met en évidence le futur parent survolé par la souris
				}

				this.creatingWindow.SuperLight = !this.IsInside(initialPos);
			}
		}

		protected void CreateObjectUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, souris relâchée.
			if (this.creatingObject != null)
			{
				if (this.IsInside(pos))
				{
					this.creatingWindow.Hide();
					this.creatingWindow.Dispose();
					this.creatingWindow = null;

					Point initialPos = pos;
					Rectangle bounds;
					this.CreateObjectAdjust(ref pos, out bounds);

					if (this.IsLayoutAnchored)
					{
						Widget parent = this.DetectGroup(bounds);

						this.creatingObject = this.CreateObjectItem();
						this.creatingObject.SetParent(parent);
						this.creatingObject.TabNavigation = TabNavigationMode.Passive;

						this.creatingObject.Anchor = AnchorStyles.BottomLeft;

						this.SetObjectPosition(this.creatingObject, pos);
					}

					if (this.IsLayoutDocking)
					{
						Widget parent;
						int order;
						Rectangle hilite;
						this.ZOrderDetect(initialPos, out parent, out order, out hilite);

						this.creatingObject = this.CreateObjectItem();
						this.creatingObject.SetParent(parent);
						this.creatingObject.ZOrder = order;
						this.creatingObject.TabNavigation = TabNavigationMode.Passive;

						this.creatingObject.Margins = new Margins(5, 5, 5, 5);

						if (this.IsHorizontalDocking)
						{
							this.creatingObject.Dock = DockStyle.Left;
						}
						else
						{
							this.creatingObject.Dock = DockStyle.Bottom;
						}

					}
				}
				else  // relâché hors de la fenêtre ?
				{
					this.creatingWindow.DissolveAndDisposeWindow();
					this.creatingWindow = null;

					this.creatingObject = null;
				}

				this.SetHilitedZOrderRectangle(Rectangle.Empty);
				this.constrainsList.Ending();
				this.SetHilitedParent(null);

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
				item.MinHeight = item.PreferredHeight;
			}

			if (this.context.Tool == "ObjectVLine")
			{
				item = new Separator();
				item.PreferredWidth = 1;
				item.PreferredHeight = 100;
				item.MinWidth = item.PreferredWidth;
				item.MinHeight = 10;
			}

			if (this.context.Tool == "ObjectStatic")
			{
				item = new StaticText();
				item.Text = Misc.Italic("StaticText");
				item.MinWidth = 10;
				item.MinHeight = item.PreferredHeight;
			}

			if (this.context.Tool == "ObjectButton")
			{
				item = new Button();
				item.Text = Misc.Italic("Button");
				item.MinWidth = 20;
				item.MinHeight = item.PreferredHeight;
			}

			if (this.context.Tool == "ObjectText")
			{
				item = new TextField();
				item.Text = Misc.Italic("TextField");
				item.MinWidth = 20;
				item.MinHeight = item.PreferredHeight;
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
			Point initialPos = pos;

			pos.X -= System.Math.Floor(this.creatingObject.PreferredWidth/2);
			pos.Y -= System.Math.Floor(this.creatingObject.PreferredHeight/2);

			bounds = new Rectangle(pos, this.creatingObject.PreferredSize);

			if (this.IsLayoutAnchored && this.IsInside(initialPos))
			{
				Rectangle adjust = this.constrainsList.Snap(bounds, this.GetObjectBaseLine(this.creatingObject));
				Point corr = adjust.BottomLeft - bounds.BottomLeft;
				pos += corr;
				bounds.Offset(corr);
			}
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
				Types.ResourceBinding binding = obj.GetBinding(Widget.TextProperty) as Types.ResourceBinding;

				Druid druid = Druid.Empty;
				if (binding != null)
				{
					druid = Druid.Parse(binding.ResourceId);
				}

				druid = this.module.MainWindow.DlgTextSelector(druid);
				
				if (druid.IsValid)
				{
					this.module.ResourceManager.Bind(obj, Widget.TextProperty, druid);
				}
			}
		}


		#region Handling
		protected bool HandlingStart(Point pos)
		{
			//	Début du drag pour déplacer une poignée.
			this.handlingType = this.handlesList.HandleDetect(pos);

			if (this.handlingType == Handle.Type.None)
			{
				this.isHandling = false;
				return false;
			}
			else
			{
				System.Diagnostics.Debug.Assert(this.selectedObjects.Count == 1);

				this.isHandling = true;
				this.handlingRectangle = this.SelectBounds;
				this.handlesList.DraggingStart(pos, this.handlingRectangle, this.selectedObjects[0].MinSize, this.handlingType);

				CloneView clone = new CloneView();
				clone.Model = this.selectedObjects[0];

				this.handlingWindow = new DragWindow();
				this.handlingWindow.DefineWidget(clone, this.handlingRectangle.Size, Drawing.Margins.Zero);
				this.handlingWindow.WindowBounds = this.MapClientToScreen(this.handlingRectangle);
				this.handlingWindow.Owner = this.Window;
				this.handlingWindow.FocusedWidget = clone;
				this.handlingWindow.Show();

				this.SetHilitedObject(null);
				this.SetHilitedAttachmentRectangle(Rectangle.Empty);
				this.Invalidate();
				return true;
			}
		}

		protected void HandlingMove(Point pos)
		{
			//	Mouvement du drag pour déplacer une poignée.
			if (this.isHandling)
			{
				Rectangle rect = this.handlesList.DraggingMove(pos);
				this.handlingWindow.WindowBounds = this.MapClientToScreen(rect);
			}
		}

		protected void HandlingEnd(Point pos)
		{
			//	Fin du drag pour déplacer une poignée.
			if (this.isHandling)
			{
				this.handlingWindow.Hide();
				this.handlingWindow.Dispose();
				this.handlingWindow = null;

				this.handlesList.DraggingStop(pos);
				this.isHandling = false;
				this.module.MainWindow.UpdateInfoViewer();
			}
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
			
			if (this.IsLayoutAnchored)
			{
				this.constrainsList.Starting(this.draggingRectangle, false);
				this.SetHilitedParent(this.DetectGroup(this.draggingRectangle));  // met en évidence le futur parent survolé par la souris
			}

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

			this.SetHilitedObject(null);
			this.SetHilitedAttachmentRectangle(Rectangle.Empty);
			this.isDragging = true;
			this.Invalidate();
		}

		protected void DraggingMove(Point pos)
		{
			//	Mouvement du drag pour déplacer les objets sélectionnés.
			if (this.IsLayoutAnchored)
			{
				this.draggingRectangle.Offset((this.draggingOffset+pos)-this.draggingRectangle.BottomLeft);
				Rectangle rect = this.IsInside(pos) ? this.draggingRectangle : Rectangle.Empty;
				this.constrainsList.Activate(rect, this.draggingBaseLine, this.draggingArraySelected);
				this.Invalidate();

				Point adjust = this.draggingRectangle.BottomLeft;
				if (this.IsInside(pos))
				{
					this.draggingRectangle = this.constrainsList.Snap(this.draggingRectangle, this.draggingBaseLine);
				}
				adjust = this.draggingRectangle.BottomLeft - adjust;

				this.draggingWindow.WindowLocation = this.draggingOrigin + pos + adjust;

				Widget parent = this.IsInside(pos) ? this.DetectGroup(this.draggingRectangle) : null;
				this.SetHilitedParent(parent);  // met en évidence le futur parent survolé par la souris
			}

			if (this.IsLayoutDocking)
			{
				this.draggingWindow.WindowLocation = this.draggingOrigin + pos;

				Widget parent;
				int order;
				Rectangle hilite;
				this.ZOrderDetect(pos, out parent, out order, out hilite);
				this.SetHilitedZOrderRectangle(hilite);
				this.SetHilitedParent(parent);  // met en évidence le futur parent survolé par la souris
			}

			this.draggingWindow.SuperLight = !this.IsInside(pos);
			this.module.MainWindow.UpdateInfoViewer();
		}

		protected void DraggingEnd(Point pos)
		{
			//	Fin du drag pour déplacer les objets sélectionnés.
			if (this.IsInside(pos))
			{
				this.draggingWindow.Hide();
				this.draggingWindow.Dispose();
				this.draggingWindow = null;

				if (this.IsLayoutAnchored)
				{
					Rectangle initial = this.SelectBounds;
					Widget parent = this.DetectGroup(this.draggingRectangle);
					this.MoveSelection(this.draggingRectangle.BottomLeft - initial.BottomLeft, parent);
				}

				if (this.IsLayoutDocking)
				{
					Widget parent;
					int order;
					Rectangle hilite;
					this.ZOrderDetect(pos, out parent, out order, out hilite);
					this.ZOrderChangeSelection(parent, order);
				}
			}
			else  // relâché hors de la fenêtre ?
			{
				this.draggingWindow.DissolveAndDisposeWindow();
				this.draggingWindow = null;
				this.DeleteSelection();
			}

			this.SetHilitedParent(null);
			this.SetHilitedZOrderRectangle(Rectangle.Empty);
			this.SetHilitedParent(null);
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
			this.SelectObjectsInRectangle(sel, this.panel);
			this.UpdateAfterSelectionChanged();
			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectObjectsInRectangle(Rectangle sel, Widget parent)
		{
			foreach (Widget obj in parent.Children)
			{
				if (sel.Contains(this.GetObjectBounds(obj)))
				{
					this.selectedObjects.Add(obj);
				}
				else
				{
					if (obj is AbstractGroup)
					{
						this.SelectObjectsInRectangle(sel, obj);
					}
				}
			}
		}

		protected void UpdateAfterSelectionChanged()
		{
			//	Mise à jour après un changement de sélection.
			this.handlesList.UpdateSelection();
		}

		protected void SetHilitedObject(Widget obj)
		{
			//	Détermine l'objet à mettre en évidence lors d'un survol.
			if (this.hilitedObject != obj)
			{
				this.hilitedObject = obj;
				this.Invalidate();
			}
		}

		protected void SetHilitedParent(Widget obj)
		{
			//	Détermine l'objet parent à mettre en évidence lors d'un survol.
			if (obj == this.panel)
			{
				//?obj = null;  // pas utile de mettre en évidence le conteneur principal !
			}

			if (this.hilitedParent != obj)
			{
				this.hilitedParent = obj;
				this.Invalidate();
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

		protected void SetHilitedAttachmentRectangle(Rectangle rect)
		{
			//	Détermine la zone du rectangle d'attachement.
			if (this.hilitedAttachmentRectangle != rect)
			{
				this.Invalidate(this.hilitedAttachmentRectangle);  // invalide l'ancienne zone
				this.hilitedAttachmentRectangle = rect;
				this.Invalidate(this.hilitedAttachmentRectangle);  // invalide la nouvelle zone
			}
		}
		#endregion


		#region Operations
		protected void DeleteSelection()
		{
			//	Supprime tous les objets sélectionnés.
			foreach (Widget obj in this.selectedObjects)
			{
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

		protected void MoveRibbonSelection(Point direction)
		{
			//	Déplace tous les objets sélectionnés selon le ruban 'Move'.
			direction.X *= this.module.MainWindow.MoveHorizontal;
			direction.Y *= this.module.MainWindow.MoveVertical;
			this.MoveSelection(direction, null);
			this.handlesList.UpdateGeometry();
		}

		protected void MoveSelection(Point move, Widget parent)
		{
			//	Déplace et change de parent pour tous les objets sélectionnés.
			foreach (Widget obj in this.selectedObjects)
			{
				Rectangle bounds = this.GetObjectBounds(obj);
				bounds.Offset(move);

				if (parent != null)
				{
					if (obj.Parent != parent)
					{
						obj.Parent.Children.Remove(obj);
						parent.Children.Add(obj);
					}
				}

				this.SetObjectBounds(obj, bounds);
			}

			this.Invalidate();
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
				if (PanelEditor.IsObjectTabActive(obj) && obj.TabIndex == oldIndex)
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

		protected void ChangeObjectAttachment(Widget obj, Attachment attachmentFlag)
		{
			//	Modifie le système d'attachement d'un objet.
			Rectangle bounds = this.GetObjectBounds(obj);
			Attachment attachment = this.GetObjectAttachment(obj);

			if (this.IsLayoutAnchored)
			{
				if ((attachment & attachmentFlag) == 0)
				{
					attachment |= attachmentFlag;
				}
				else
				{
					attachment &= ~attachmentFlag;

					if ((attachment & PanelEditor.OppositeAttachment(attachmentFlag)) == 0)
					{
						attachment |= PanelEditor.OppositeAttachment(attachmentFlag);
					}
				}
			}

			if (this.IsLayoutDocking)
			{
				attachment = attachmentFlag;
			}

			this.SetObjectBounds(obj, bounds, attachment);
			this.handlesList.UpdateGeometry();
			this.Invalidate();
		}

		static protected Attachment OppositeAttachment(Attachment style)
		{
			//	Retourne le style d'attachement opposé.
			switch (style)
			{
				case Attachment.Left:    return Attachment.Right;
				case Attachment.Right:   return Attachment.Left;
				case Attachment.Bottom:  return Attachment.Top;
				case Attachment.Top:     return Attachment.Bottom;
			}
			return style;
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

		public void SetObjectMargins(Widget obj, Margins margins)
		{
			//	Modifie les marges de l'objet, en mode LayoutMode.Docked.
			if (this.IsLayoutDocking)
			{
				obj.Margins = margins;
			}
		}

		public Margins GetObjectMargins(Widget obj)
		{
			//	Donne les marges de l'objet, utile en mode LayoutMode.Docked.
			return obj.Margins;
		}

		public Rectangle GetObjectBounds(Widget obj)
		{
			//	Retourne la boîte d'un objet.
			//	Les coordonnées sont toujours relative au panneau (this.panel) propriétaire.
			this.Window.ForceLayout();
			Rectangle bounds = obj.Client.Bounds;

			while (obj != this.panel)
			{
				bounds = obj.MapClientToParent(bounds);
				obj = obj.Parent;
			}

			return bounds;
		}

		public void SetObjectBounds(Widget obj, Rectangle bounds)
		{
			//	Modifie la boîte d'un objet.
			Attachment attachment = this.GetObjectAttachment(obj);
			this.SetObjectBounds(obj, bounds, attachment);
		}

		protected void SetObjectBounds(Widget obj, Rectangle bounds, Attachment attachment)
		{
			//	Modifie la boîte et le système d'attachement d'un objet.
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

			this.Window.ForceLayout();
			Widget parent = obj.Parent;
			while (parent != this.panel)
			{
				bounds = parent.MapParentToClient(bounds);
				parent = parent.Parent;
			}

			parent = obj.Parent;
			Rectangle box = parent.ActualBounds;
			Margins margins = obj.Margins;
			Margins padding = parent.Padding + parent.GetInternalPadding();

			if ((attachment & Attachment.Left) != 0)
			{
				double px = bounds.Left;
				px -= padding.Left;
				px = System.Math.Max(px, 0);
				margins.Left = px;
			}

			if ((attachment & Attachment.Right) != 0)
			{
				double px = box.Width - bounds.Right;
				px -= padding.Right;
				px = System.Math.Max(px, 0);
				margins.Right = px;
			}

			if ((attachment & Attachment.Bottom) != 0)
			{
				double py = bounds.Bottom;
				py -= padding.Bottom;
				py = System.Math.Max(py, 0);
				margins.Bottom = py;
			}

			if ((attachment & Attachment.Top) != 0)
			{
				double py = box.Height - bounds.Top;
				py -= padding.Top;
				py = System.Math.Max(py, 0);
				margins.Top = py;
			}

			if (this.IsLayoutAnchored)
			{
				obj.Margins = margins;
			}
			obj.PreferredSize = bounds.Size;
			this.SetObjectAttachment(obj, attachment);

			this.Invalidate();
		}

		public bool IsObjectWidthChanging(Widget obj)
		{
			//	Indique si la largeur d'un objet peut changer.
			if (this.IsLayoutDocking)
			{
				if (this.IsObjectAttachmentBottom(obj) || this.IsObjectAttachmentTop(obj))
				{
					return obj.HorizontalAlignment != HorizontalAlignment.Stretch;
				}
			}
			return true;
		}

		public bool IsObjectHeightChanging(Widget obj)
		{
			//	Indique si la hauteur d'un objet peut changer.
			if (this.IsLayoutDocking)
			{
				if (this.IsObjectAttachmentLeft(obj) || this.IsObjectAttachmentRight(obj))
				{
					return obj.VerticalAlignment != VerticalAlignment.Stretch;
				}
			}
			return true;
		}

		public bool IsObjectAttachmentLeft(Widget obj)
		{
			//	Indique si l'objet est ancré à gauche.
			return (this.GetObjectAttachment(obj) & Attachment.Left) != 0;
		}

		public bool IsObjectAttachmentRight(Widget obj)
		{
			//	Indique si l'objet est ancré à gauche.
			return (this.GetObjectAttachment(obj) & Attachment.Right) != 0;
		}

		public bool IsObjectAttachmentBottom(Widget obj)
		{
			//	Indique si l'objet est ancré à gauche.
			return (this.GetObjectAttachment(obj) & Attachment.Bottom) != 0;
		}

		public bool IsObjectAttachmentTop(Widget obj)
		{
			//	Indique si l'objet est ancré à gauche.
			return (this.GetObjectAttachment(obj) & Attachment.Top) != 0;
		}

		protected Attachment GetObjectAttachment(Widget obj)
		{
			//	Retourne le mode d'attachement d'un objet.
			Attachment attachment = Attachment.None;

			if (this.IsLayoutAnchored)
			{
				if ((obj.Anchor & AnchorStyles.Left  ) != 0)  attachment |= Attachment.Left;
				if ((obj.Anchor & AnchorStyles.Right ) != 0)  attachment |= Attachment.Right;
				if ((obj.Anchor & AnchorStyles.Bottom) != 0)  attachment |= Attachment.Bottom;
				if ((obj.Anchor & AnchorStyles.Top   ) != 0)  attachment |= Attachment.Top;
			}

			if (this.IsLayoutDocking)
			{
				if (obj.Dock == DockStyle.Left  )  attachment |= Attachment.Left;
				if (obj.Dock == DockStyle.Right )  attachment |= Attachment.Right;
				if (obj.Dock == DockStyle.Bottom)  attachment |= Attachment.Bottom;
				if (obj.Dock == DockStyle.Top   )  attachment |= Attachment.Top;
			}

			return attachment;
		}

		protected void SetObjectAttachment(Widget obj, Attachment attachment)
		{
			//	Modifie le mode d'attachement d'un objet.
			if (this.IsLayoutAnchored)
			{
				AnchorStyles style = AnchorStyles.None;

				if ((attachment & Attachment.Left  ) != 0)  style |= AnchorStyles.Left;
				if ((attachment & Attachment.Right ) != 0)  style |= AnchorStyles.Right;
				if ((attachment & Attachment.Bottom) != 0)  style |= AnchorStyles.Bottom;
				if ((attachment & Attachment.Top   ) != 0)  style |= AnchorStyles.Top;

				obj.Anchor = style;
			}

			if (this.IsLayoutDocking)
			{
				DockStyle style = DockStyle.None;

				if (attachment == Attachment.Left  )  style = DockStyle.Left;
				if (attachment == Attachment.Right )  style = DockStyle.Right;
				if (attachment == Attachment.Bottom)  style = DockStyle.Bottom;
				if (attachment == Attachment.Top   )  style = DockStyle.Top;

				obj.Dock = style;
			}
		}

		public double GetObjectBaseLine(Widget obj)
		{
			//	Retourne la position relative de la ligne de base depuis le bas de l'objet.
			return System.Math.Floor(obj.GetBaseLine().Y);
		}

		protected static bool IsObjectTabActive(Widget obj)
		{
			//	Indique si l'objet à un ordre pour la touche Tab.
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
				if (PanelEditor.IsObjectTabActive(obj))
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
				if (PanelEditor.IsObjectTabActive(obj))
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


		#region Attachment
		protected bool AttachmentDetect(Point mouse, out Widget obj, out Attachment attachment)
		{
			//	Détecte dans quel attachement d'un objet est la souris.
			if (!this.IsLayoutAnchored || !this.context.ShowAttachment || this.selectedObjects.Count != 1)
			{
				obj = null;
				attachment = Attachment.None;
				return false;
			}

			Attachment[] attachments = { Attachment.Left, Attachment.Right, Attachment.Bottom, Attachment.Top };

			foreach (Widget o in this.selectedObjects)
			{
				foreach (Attachment s in attachments)
				{
					Rectangle bounds = this.GetAttachmentBounds(o, s);
					if (bounds.Contains(mouse))
					{
						obj = o;
						attachment = s;
						return true;
					}
				}
			}

			obj = null;
			attachment = Attachment.None;
			return false;
		}

		protected Rectangle GetAttachmentBounds(Widget obj, Attachment style)
		{
			//	Retourne le rectangle englobant un attachement.
			Rectangle bounds = this.GetObjectBounds(obj.Parent);
			Rectangle rect = this.GetObjectBounds(obj);
			Point p1, p2, p1a, p2a;

			if (style == Attachment.Left)
			{
				p1 = new Point(bounds.Left, rect.Center.Y);
				p2 = new Point(rect.Left, rect.Center.Y);
				p1a = Point.Scale(p1, p2, PanelEditor.attachmentScale);
				p2a = Point.Scale(p2, p1, PanelEditor.attachmentScale);
				p1a.Y -= PanelEditor.attachmentThickness;
				p2a.Y += PanelEditor.attachmentThickness;
				return new Rectangle(p1a, p2a);
			}

			if (style == Attachment.Right)
			{
				p1 = new Point(bounds.Right, rect.Center.Y);
				p2 = new Point(rect.Right, rect.Center.Y);
				p1a = Point.Scale(p1, p2, PanelEditor.attachmentScale);
				p2a = Point.Scale(p2, p1, PanelEditor.attachmentScale);
				p1a.Y -= PanelEditor.attachmentThickness;
				p2a.Y += PanelEditor.attachmentThickness;
				return new Rectangle(p1a, p2a);
			}

			if (style == Attachment.Bottom)
			{
				p1 = new Point(rect.Center.X, bounds.Bottom);
				p2 = new Point(rect.Center.X, rect.Bottom);
				p1a = Point.Scale(p1, p2, PanelEditor.attachmentScale);
				p2a = Point.Scale(p2, p1, PanelEditor.attachmentScale);
				p1a.X -= PanelEditor.attachmentThickness;
				p2a.X += PanelEditor.attachmentThickness;
				return new Rectangle(p1a, p2a);
			}

			if (style == Attachment.Top)
			{
				p1 = new Point(rect.Center.X, bounds.Top);
				p2 = new Point(rect.Center.X, rect.Top);
				p1a = Point.Scale(p1, p2, PanelEditor.attachmentScale);
				p2a = Point.Scale(p2, p1, PanelEditor.attachmentScale);
				p1a.X -= PanelEditor.attachmentThickness;
				p2a.X += PanelEditor.attachmentThickness;
				return new Rectangle(p1a, p2a);
			}

			return Rectangle.Empty;
		}
		#endregion


		#region ZOrder
		protected void ZOrderDetect(Point mouse, out Widget parent, out int order, out Rectangle hilite)
		{
			//	Détecte le ZOrder à utiliser pour une position donnée, ainsi que le rectangle
			//	à utiliser pour la mise ne évidence.
			if (!this.IsInside(mouse))
			{
				parent = null;
				order = -1;
				hilite = Rectangle.Empty;
				return;
			}

			parent = this.panel;
			order = 0;
			hilite = Rectangle.Empty;

			Widget obj = this.ZOrderDetectNearest(mouse);  // objet le plus proche
			if (obj == null)
			{
				Rectangle bounds = this.RealBounds;
				
				if (this.IsHorizontalDocking)
				{
					hilite = new Rectangle(bounds.Left, bounds.Bottom, 0, bounds.Height);
				}
				else
				{
					hilite = new Rectangle(bounds.Left, bounds.Bottom, bounds.Width, 0);
				}
			}
			else
			{
				if (obj is AbstractGroup)
				{
					Rectangle inside = this.GetObjectBounds(obj);
					inside.Deflate(obj.Padding);
					if (inside.Contains(mouse))
					{
						parent = obj;
						order = 0;
						hilite = inside;
						return;
					}
				}

				parent = obj.Parent;
				order = obj.ZOrder;

				Rectangle bounds = this.GetObjectBounds(obj);
				bounds.Inflate(this.GetObjectMargins(obj));

				if (this.IsHorizontalDocking)
				{
					if (this.IsObjectAttachmentLeft(obj))
					{
						if (mouse.X > bounds.Center.X)
						{
							hilite = new Rectangle(bounds.Right, bounds.Bottom, 0, bounds.Height);
						}
						else
						{
							order++;
							hilite = new Rectangle(bounds.Left, bounds.Bottom, 0, bounds.Height);
						}
					}
					else
					{
						if (mouse.X < bounds.Center.X)
						{
							hilite = new Rectangle(bounds.Left, bounds.Bottom, 0, bounds.Height);
						}
						else
						{
							order++;
							hilite = new Rectangle(bounds.Right, bounds.Bottom, 0, bounds.Height);
						}
					}
				}
				else
				{
					if (this.IsObjectAttachmentBottom(obj))
					{
						if (mouse.Y > bounds.Center.Y)
						{
							hilite = new Rectangle(bounds.Left, bounds.Top, bounds.Width, 0);
						}
						else
						{
							order++;
							hilite = new Rectangle(bounds.Left, bounds.Bottom, bounds.Width, 0);
						}
					}
					else
					{
						if (mouse.Y < bounds.Center.Y)
						{
							hilite = new Rectangle(bounds.Left, bounds.Bottom, bounds.Width, 0);
						}
						else
						{
							order++;
							hilite = new Rectangle(bounds.Left, bounds.Top, bounds.Width, 0);
						}
					}
				}
			}

			if (!hilite.IsEmpty)
			{
				hilite.Inflate(this.context.ZOrderThickness);
			}

			if (this.selectedObjects.Count != 0)
			{
				//	Un ZOrder équivalent au ZOrder de l'objet actuellement sélectionné n'est
				//	pas retourné, pour éviter à l'utilisateur de croire qu'il va changer
				//	quelque chose.
				foreach (Widget selectedObj in this.selectedObjects)
				{
					if (selectedObj.ZOrder == order || selectedObj.ZOrder == order-1)
					{
						parent = null;
						order = -1;  // aucun changement
						hilite = Rectangle.Empty;
						return;
					}
				}
			}
		}

		protected Widget ZOrderDetectNearest(Point pos)
		{
			//	Détecte l'objet le plus proche de la souris.
			Widget bestObj = null;
			double bestDistance = 1000000;
			this.ZOrderDetectNearest(pos, this.panel, ref bestObj, ref bestDistance);
			return bestObj;
		}

		protected void ZOrderDetectNearest(Point pos, Widget parent, ref Widget bestObj, ref double bestDistance)
		{
			for (int i=parent.Children.Count-1; i>=0; i--)
			{
				Widget obj = parent.Children[i] as Widget;

				if (obj is AbstractGroup && obj.Children.Count != 0)
				{
					this.ZOrderDetectNearest(pos, obj, ref bestObj, ref bestDistance);
				}
				else
				{
					Rectangle bounds = this.GetObjectBounds(obj);
					double distance = Point.Distance(bounds.Center, pos);

					if (bestDistance > distance)
					{
						bestDistance = distance;
						bestObj = obj;
					}
				}
			}
		}

		protected void SetHilitedZOrderRectangle(Rectangle rect)
		{
			//	Détermine la zone du rectangle d'insertion ZOrder.
			if (this.hilitedZOrderRectangle != rect)
			{
				this.Invalidate(this.hilitedZOrderRectangle);  // invalide l'ancienne zone
				this.hilitedZOrderRectangle = rect;
				this.Invalidate(this.hilitedZOrderRectangle);  // invalide la nouvelle zone
			}
		}

		protected void ZOrderChangeSelection(Widget parent, int order)
		{
			//	Change le ZOrder de tous les objets sélectionnés.
			if (order == -1 || this.selectedObjects.Contains(parent))
			{
				return;
			}

			foreach (Widget obj in this.selectedObjects)
			{
				int newOrder = order;

				if (newOrder > obj.ZOrder && obj.Parent == parent)
				{
					newOrder--;
				}

				obj.SetParent(parent);
				obj.ZOrder = newOrder;
			}
		}
		#endregion


		#region SizeMark
		protected bool SizeMarkDraggingStart(Point pos)
		{
			//	Début du déplacement d'un marqueur de taille préférentielle.
			//	Retourne true en cas de début effectif.
			this.isSizeMarkDragging = false;

			if (this.isSizeMarkHorizontal)
			{
				this.isSizeMarkDragging = true;
				this.sizeMarkOffset.Y = pos.Y-this.SizeMarkHorizontalRect.Bottom;
			}

			if (this.isSizeMarkVertical)
			{
				this.isSizeMarkDragging = true;
				this.sizeMarkOffset.X = pos.X-this.SizeMarkVerticalRect.Right;
			}

			return this.isSizeMarkDragging;
		}

		protected bool SizeMarkDraggingMove(Point pos)
		{
			//	Déplacement d'un marqueur de taille préférentielle.
			//	Retourne true en cas de déplacement effectif.
			if (this.isSizeMarkDragging)
			{
				if (this.isSizeMarkHorizontal)
				{
					this.sizeMark.Height = System.Math.Max(pos.Y-this.sizeMarkOffset.Y, this.context.SizeMarkThickness);
				}

				if (this.isSizeMarkVertical)
				{
					this.sizeMark.Width = System.Math.Max(pos.X-this.sizeMarkOffset.X, this.context.SizeMarkThickness);
				}

				this.panel.PreferredSize = this.sizeMark;
				this.handlesList.UpdateGeometry();
				this.Invalidate();
			}
			else
			{
				bool h = this.SizeMarkHorizontalRect.Contains(pos);
				bool v = this.SizeMarkVerticalRect.Contains(pos);

				if (this.isSizeMarkHorizontal != h)
				{
					this.isSizeMarkHorizontal = h;
					this.Invalidate();
				}

				if (this.isSizeMarkVertical != v)
				{
					this.isSizeMarkVertical = v;
					this.Invalidate();
				}

				if (this.isSizeMarkHorizontal || this.isSizeMarkVertical)
				{
					this.ChangeMouseCursor(MouseCursorType.Finger);
				}
			}

			return this.isSizeMarkDragging;
		}

		protected void SizeMarkDraggingStop(Point pos)
		{
			//	Fin du déplacement d'un marqueur de taille préférentielle.
			if (this.isSizeMarkDragging)
			{
				this.isSizeMarkDragging = false;
			}
		}

		public void SizeMarkDeselect()
		{
			//	Désélectionne les marqueurs de taille préférentielle.
			if (this.isSizeMarkHorizontal)
			{
				this.isSizeMarkHorizontal = false;
				this.Invalidate();
			}

			if (this.isSizeMarkVertical)
			{
				this.isSizeMarkVertical = false;
				this.Invalidate();
			}
		}

		protected Rectangle SizeMarkHorizontalRect
		{
			//	Retourne le rectangle du marqueur de taille préférentielle horizontal.
			get
			{
				Rectangle bounds = this.RealBounds;
				Rectangle box = this.Client.Bounds;
				double t = this.context.SizeMarkThickness;
				return new Rectangle(bounds.Right, this.sizeMark.Height-t, box.Right, t);
			}
		}

		protected Rectangle SizeMarkVerticalRect
		{
			//	Retourne le rectangle du marqueur de taille préférentielle vertical.
			get
			{
				Rectangle bounds = this.RealBounds;
				Rectangle box = this.Client.Bounds;
				double t = this.context.SizeMarkThickness;
				return new Rectangle(this.sizeMark.Width-t, bounds.Top, t, box.Top);
			}
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

			//	Dessine les marques pour la taille préférentielle.
			if (this.context.Tool == "ToolSelect" || this.context.Tool == "ToolGlobal")
			{
				this.DrawSizeMark(graphics);
			}

			//	Dessine les objets sélectionnés.
			if (this.selectedObjects.Count > 0 && !this.isDragging && !this.handlesList.IsDragging)
			{
				foreach (Widget obj in this.selectedObjects)
				{
					this.DrawSelectedObject(graphics, obj);
				}
			}

			//	Dessine les attachements des objets sélectionnés.
			if (this.IsLayoutAnchored && this.context.ShowAttachment && this.selectedObjects.Count == 1 && !this.isDragging && !this.handlesList.IsDragging)
			{
				foreach (Widget obj in this.selectedObjects)
				{
					this.DrawAttachment(graphics, obj, PanelsContext.ColorAttachment);
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
			if (this.hilitedObject != null)
			{
				this.DrawHilitedObject(graphics, this.hilitedObject);
			}

			//	Dessine l'objet parentsurvolé.
			if (this.hilitedParent != null)
			{
				this.DrawHilitedParent(graphics, this.hilitedParent);
			}

			//	Dessine le rectangle de sélection.
			if (!this.selectedRectangle.IsEmpty)
			{
				Rectangle sel = this.selectedRectangle;
				sel.Deflate(0.5);
				graphics.AddRectangle(sel);
				graphics.RenderSolid(PanelsContext.ColorHiliteOutline);
			}

			//	Dessine le rectangle d'attachement survolé.
			if (!this.hilitedAttachmentRectangle.IsEmpty)
			{
				Rectangle rect = this.hilitedAttachmentRectangle;
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(PanelsContext.ColorHiliteSurface);
			}

			//	Dessine le rectangle d'insertion ZOrder survolé.
			if (!this.hilitedZOrderRectangle.IsEmpty)
			{
				Rectangle rect = this.hilitedZOrderRectangle;
				if (rect.Width > this.context.ZOrderThickness && rect.Height > this.context.ZOrderThickness)
				{
					rect.Deflate(1.5);
					graphics.LineWidth = 3;
					graphics.AddRectangle(rect);
					graphics.RenderSolid(PanelsContext.ColorZOrder);
					graphics.LineWidth = 1;
				}
				else
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(PanelsContext.ColorZOrder);
				}
			}

			//	Dessine les poignées.
			if (this.selectedObjects.Count == 1 && !this.isDragging)
			{
				this.handlesList.Draw(graphics);
			}
		}

		protected void DrawSizeMark(Graphics graphics)
		{
			//	Dessine les marqueurs pour la taille préférentielle.
			Rectangle rect;
			Point p1, p2;

			rect = this.SizeMarkHorizontalRect;
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.isSizeMarkHorizontal ? PanelsContext.ColorSizeMarkDark : PanelsContext.ColorSizeMarkLight);

			p1 = rect.TopLeft;
			p2 = rect.TopRight;
			Misc.AlignForLine(graphics, ref p1);
			Misc.AlignForLine(graphics, ref p2);
			graphics.AddLine(p1, p2);
			graphics.RenderSolid(PanelsContext.ColorSizeMarkLine);

			rect = this.SizeMarkVerticalRect;
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.isSizeMarkVertical ? PanelsContext.ColorSizeMarkDark : PanelsContext.ColorSizeMarkLight);

			p1 = rect.BottomRight;
			p2 = rect.TopRight;
			Misc.AlignForLine(graphics, ref p1);
			Misc.AlignForLine(graphics, ref p2);
			graphics.AddLine(p1, p2);
			graphics.RenderSolid(PanelsContext.ColorSizeMarkLine);
		}

		protected void DrawSelectedObject(Graphics graphics, Widget obj)
		{
			Rectangle bounds = this.GetObjectBounds(obj);
			bounds.Deflate(0.5);

			//?graphics.AddFilledRectangle(bounds);
			//?graphics.RenderSolid(PanelsContext.ColorHiliteSurface);

			graphics.LineWidth = 3;
			graphics.AddRectangle(bounds);
			graphics.RenderSolid(PanelsContext.ColorHiliteOutline);
			graphics.LineWidth = 1;

			if (this.IsLayoutDocking)
			{
				Rectangle ext = bounds;
				ext.Inflate(this.GetObjectMargins(obj));
				ext.Deflate(0.5);

				Path path = new Path();
				path.AppendRectangle(ext);
				Misc.DrawPathDash(graphics, path, 2, 0, 4, PanelsContext.ColorHiliteOutline);
			}
		}

		protected void DrawHilitedObject(Graphics graphics, Widget obj)
		{
			//	Met en évidence l'objet survolé par la souris.
			if (this.context.ShowAttachment)
			{
				this.DrawAttachment(graphics, obj, PanelsContext.ColorHiliteOutline);
			}

			Rectangle rect = this.GetObjectBounds(obj);

			//	Si le rectangle est trop petit (par exemple objet Separator), il est engraissé.
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

			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(PanelsContext.ColorHiliteSurface);
		}

		protected void DrawHilitedParent(Graphics graphics, Widget obj)
		{
			//	Met en évidence l'objet parent survolé par la souris.
			if (this.context.ShowAttachment && obj != this.panel)
			{
				this.DrawAttachment(graphics, obj, PanelsContext.ColorHiliteParent);
			}

			double thickness = 2.0;

			Rectangle rect = this.GetObjectBounds(obj);
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(PanelsContext.ColorHiliteParent);
			rect.Deflate(thickness/2+0.5);

			Path path = new Path();
			path.AppendRectangle(rect);
			Misc.DrawPathDash(graphics, path, thickness, thickness*2-1, thickness*2+1, PanelsContext.ColorHiliteParent);

			rect.Deflate(thickness/2+0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(PanelsContext.ColorHiliteParent);
		}

		protected void DrawAttachment(Graphics graphics, Widget obj, Color color)
		{
			//	Dessine tous les attachements d'un objet.
			if (this.IsLayoutAnchored)
			{
				Rectangle bounds = this.GetObjectBounds(obj.Parent);
				Rectangle rect = this.GetObjectBounds(obj);
				Point p1, p2;

				p1 = new Point(bounds.Left, rect.Center.Y);
				p2 = new Point(rect.Left, rect.Center.Y);
				this.DrawAttachment(graphics, p1, p2, this.IsObjectAttachmentLeft(obj), color);

				p1 = new Point(rect.Right, rect.Center.Y);
				p2 = new Point(bounds.Right, rect.Center.Y);
				this.DrawAttachment(graphics, p1, p2, this.IsObjectAttachmentRight(obj), color);

				p1 = new Point(rect.Center.X, bounds.Bottom);
				p2 = new Point(rect.Center.X, rect.Bottom);
				this.DrawAttachment(graphics, p1, p2, this.IsObjectAttachmentBottom(obj), color);

				p1 = new Point(rect.Center.X, rect.Top);
				p2 = new Point(rect.Center.X, bounds.Top);
				this.DrawAttachment(graphics, p1, p2, this.IsObjectAttachmentTop(obj), color);
			}
		}

		protected void DrawAttachment(Graphics graphics, Point p1, Point p2, bool rigid, Color color)
		{
			//	Dessine un attachement horizontal ou vertical d'un objet.
			Point p1a = Point.Scale(p1, p2, PanelEditor.attachmentScale);
			Point p2a = Point.Scale(p2, p1, PanelEditor.attachmentScale);

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

				double dim = PanelEditor.attachmentThickness;
				Misc.AddBox(graphics, p1a, p2a, dim);
			}
			else  // élastique (ressort) ?
			{
				graphics.AddLine(p1, p1a);
				graphics.AddLine(p2, p2a);

				double dim = PanelEditor.attachmentThickness;
				double length = Point.Distance(p1a, p2a);
				int loops = (int) (length/(dim*2));
				loops = System.Math.Max(loops, 1);
				Misc.AddSpring(graphics, p1a, p2a, dim, loops);
			}

			graphics.RenderSolid(color);

			//	Dessine les extrémités.
			graphics.AddFilledCircle(p1, 3.0);
			graphics.AddFilledCircle(p2, 3.0);
			graphics.RenderSolid(color);
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
				Size s1 = this.panel.RealMinSize;
				Size s2 = this.panel.PreferredSize;
				double w = System.Math.Max(s1.Width, s2.Width);
				double h = System.Math.Max(s1.Height, s2.Height);
				return new Rectangle(0, 0, w, h);
			}
		}

		protected bool IsInside(Point pos)
		{
			//	Indique si une position est dans la fenêtre.
			return this.Client.Bounds.Contains(pos);
		}
		#endregion


		#region IPaintFilter Members
		bool IPaintFilter.IsWidgetFullyDiscarded(Widget widget)
		{
			//	Retourne true pour indiquer que le widget en question ne doit
			//	pas être peint, ni ses enfants d'ailleurs. Ceci évite que les
			//	widgets sélectionnés ne soient peints.
			return (this.isDragging || this.isHandling) && this.selectedObjects.Contains(widget);
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


		protected static readonly double	attachmentThickness = 3.0;
		protected static readonly double	attachmentScale = 0.4;

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
		protected Rectangle					hilitedAttachmentRectangle = Rectangle.Empty;
		protected Rectangle					hilitedZOrderRectangle = Rectangle.Empty;
		protected Widget					hilitedObject;
		protected Widget					hilitedParent;
		protected bool						isRectangling;  // j'invente des mots si je veux !
		protected bool						isDragging;
		protected DragWindow				draggingWindow;
		protected Point						draggingOffset;
		protected Point						draggingOrigin;
		protected Rectangle					draggingRectangle;
		protected double					draggingBaseLine;
		protected Widget[]					draggingArraySelected;
		protected bool						isHandling;
		protected Handle.Type				handlingType;
		protected DragWindow				handlingWindow;
		protected Rectangle					handlingRectangle;
		protected Point						startingPos;
		protected MouseCursorType			lastCursor = MouseCursorType.Unknow;
		protected Size						sizeMark;
		protected bool						isSizeMarkDragging;
		protected bool						isSizeMarkHorizontal;
		protected bool						isSizeMarkVertical;
		protected Point						sizeMarkOffset;

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
