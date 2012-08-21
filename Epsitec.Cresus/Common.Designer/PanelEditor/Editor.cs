using System.Collections.Generic;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Layouts;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.PanelEditor
{
	/// <summary>
	/// Widget venant par-dessus le conteneur UI.Panel pour éditer ce dernier.
	/// </summary>
	public class Editor : AbstractGroup, IPaintFilter
	{
		protected enum Attachment
		{
			None,
			Left,
			Right,
			Bottom,
			Top,
		}

		protected enum MouseCursorType
		{
			Unknown,
			Arrow,
			ArrowPlus,
			Global,
			Grid,
			GridPlus,
			Hand,
			Edit,
			Pen,
			Zoom,
			Finger,
		}


		public Editor() : base()
		{
			this.AutoFocus = true;
			this.InternalState |= WidgetInternalState.Focusable;
		}

		public Editor(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		static Editor()
		{
			Widgets.Helpers.VisualPropertyMetadata metadata = new Widgets.Helpers.VisualPropertyMetadata(ContentAlignment.TopLeft, Widgets.Helpers.VisualPropertyMetadataOptions.AffectsTextLayout);
			Widgets.Visual.ContentAlignmentProperty.OverrideMetadata(typeof(Editor), metadata);
		}


		public void Initialize(Viewers.Panels viewersPanels, Module module, PanelsContext context, UI.Panel panel)
		{
			this.viewersPanels = viewersPanels;
			this.module = module;
			this.context = context;
			this.panel = panel;
			this.sizeMark = this.panel.PreferredSize;

			this.objectModifier = new ObjectModifier(this);
			this.constrainsList = new ConstrainsList(this);
			this.handlesList = new HandlesList(this);
			this.dimensionsList = new DimensionsList(this);
		}

		public Viewers.Panels ViewersPanels
		{
			get
			{
				return this.viewersPanels;
			}
		}

		public Module Module
		{
			//	Module associé.
			get
			{
				return this.module;
			}
		}

		public PanelsContext Context
		{
			//	Contexte asocié.
			get
			{
				return this.context;
			}
		}

		public UI.Panel Panel
		{
			//	Panneau associé qui est le conteneur de tous les widgets.
			//	Editor est frère de Panel et vient par-dessus.
			get
			{
				return this.panel;
			}
			set
			{
				this.selectedObjects.Clear();
				this.GridClearSelection();
				this.UpdateAfterChanging(Viewers.Changing.Show);
				this.lastCreatedObject = null;
				
				this.panel = value;
				this.sizeMark = this.panel.PreferredSize;
				this.Invalidate();
			}
		}

		public Druid Druid
		{
			//	Druid du conteneur des widgets.
			get
			{
				return this.druid;
			}
			set
			{
				this.druid = value;
			}
		}

		public ObjectModifier ObjectModifier
		{
			//	Retourne le modificateur d'objets.
			get
			{
				return this.objectModifier;
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

		public List<Widget> SelectedObjects
		{
			//	Retourne la liste des objets sélectionnés.
			get
			{
				return this.selectedObjects;
			}
		}

		public bool IsEditEnabled
		{
			//	Est-ce que l'édition est possible ? Pour cela, il faut avoir sélectionné un bundle
			//	dans la liste de gauche.
			get
			{
				return this.isEditEnabled;
			}
			set
			{
				this.isEditEnabled = value;
			}
		}

		public void UpdateGeometry()
		{
			//	Mise à jour après avoir changé la géométrie d'un ou plusieurs objets.
			this.handlesList.UpdateGeometry();
		}


		public void DoCommand(string name)
		{
			//	Exécute une commande.
			switch (name)
			{
				case "PanelDelete":
					this.viewersPanels.UndoMemorize(Res.Strings.Action.DeleteObject, false);
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

				case "PanelSelectRoot":
					this.SelectRoot();
					break;

				case "PanelSelectParent":
					this.SelectParent();
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
					this.viewersPanels.UndoMemorize(Res.Strings.Action.AlignLeft, false);
					this.SelectAlign(-1, false);
					break;

				case "AlignCenterX":
					this.viewersPanels.UndoMemorize(Res.Strings.Action.AlignCenterX, false);
					this.SelectAlign(0, false);
					break;

				case "AlignRight":
					this.viewersPanels.UndoMemorize(Res.Strings.Action.AlignRight, false);
					this.SelectAlign(1, false);
					break;

				case "AlignTop":
					this.viewersPanels.UndoMemorize(Res.Strings.Action.AlignTop, false);
					this.SelectAlign(1, true);
					break;

				case "AlignCenterY":
					this.viewersPanels.UndoMemorize(Res.Strings.Action.AlignCenterY, false);
					this.SelectAlign(0, true);
					break;

				case "AlignBottom":
					this.viewersPanels.UndoMemorize(Res.Strings.Action.AlignBottom, false);
					this.SelectAlign(-1, true);
					break;

				case "AlignBaseLine":
					this.viewersPanels.UndoMemorize(Res.Strings.Action.AlignBaseLine, false);
					this.SelectAlignBaseLine();
					break;

				case "AdjustWidth":
					this.viewersPanels.UndoMemorize(Res.Strings.Action.AdjustWidth, false);
					this.SelectAdjust(false);
					break;

				case "AdjustHeight":
					this.viewersPanels.UndoMemorize(Res.Strings.Action.AdjustHeight, false);
					this.SelectAdjust(true);
					break;

				case "AlignGrid":
					this.viewersPanels.UndoMemorize(Res.Strings.Action.AlignGrid, false);
					this.SelectAlignGrid();
					break;

				case "PanelOrderUpAll":
					this.viewersPanels.UndoMemorize(Res.Strings.Action.PanelOrderUpAll, false);
					this.SelectOrder(-10000);
					break;

				case "PanelOrderDownAll":
					this.viewersPanels.UndoMemorize(Res.Strings.Action.PanelOrderDownAll, false);
					this.SelectOrder(10000);
					break;

				case "PanelOrderUpOne":
					this.viewersPanels.UndoMemorize(Res.Strings.Action.PanelOrderUpOne, false);
					this.SelectOrder(-1);
					break;

				case "PanelOrderDownOne":
					this.viewersPanels.UndoMemorize(Res.Strings.Action.PanelOrderDownOne, false);
					this.SelectOrder(1);
					break;

				case "TabIndexClear":
					this.viewersPanels.UndoMemorize(Res.Strings.Action.TabIndexClear, false);
					this.SelectTabIndex(0);
					break;

				case "TabIndexRenum":
					this.viewersPanels.UndoMemorize(Res.Strings.Action.TabIndexRenum, false);
					this.SelectTabIndexRenum();
					break;

				case "TabIndexFirst":
					this.viewersPanels.UndoMemorize(Res.Strings.Action.TabIndexFirst, false);
					this.SelectTabIndex(-10000);
					break;

				case "TabIndexPrev":
					this.viewersPanels.UndoMemorize(Res.Strings.Action.TabIndexPrev, false);
					this.SelectTabIndex(-1);
					break;

				case "TabIndexNext":
					this.viewersPanels.UndoMemorize(Res.Strings.Action.TabIndexNext, false);
					this.SelectTabIndex(1);
					break;

				case "TabIndexLast":
					this.viewersPanels.UndoMemorize(Res.Strings.Action.TabIndexLast, false);
					this.SelectTabIndex(10000);
					break;

			}
		}

		public void GetSelectionInfo(out int selected, out int count, out bool isRoot)
		{
			//	Donne des informations sur la sélection en cours.
			selected = this.selectedObjects.Count;
			count = this.panel.Children.Count;
			isRoot = false;

			if (selected == 1)
			{
				ObjectModifier.ObjectType type = ObjectModifier.GetObjectType(this.selectedObjects[0]);
				isRoot = (type == ObjectModifier.ObjectType.MainPanel);
			}
		}

		public string SelectionInfo
		{
			//	Donne le texte pour les statuts.
			get
			{
				string sel = "-";
				Rectangle rect = Rectangle.Empty;

				if (this.creatingObject != null)
				{
					rect = this.isInside ? this.creatingRectangle : Rectangle.Empty;
				}
				else if (this.isDragging)
				{
					rect = this.isInside ? this.draggingRectangle : Rectangle.Empty;
				}
				else if (this.isHandling)
				{
					rect = this.handlingRectangle;
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
				bool isRoot;
				this.GetSelectionInfo(out objSelected, out objCount, out isRoot);
				string text = string.Format(Res.Strings.Viewers.Panels.Info, objSelected.ToString(), objCount.ToString(), sel);

				return text;
			}
		}


		protected override void ProcessMessage(Message message, Point pos)
		{
			if (!this.isEditEnabled)
			{
				return;
			}

			pos = this.ConvEditorToPanel(pos);

			//	Informe l'ensemble des cotes de l'état de la touche Shift.
			this.dimensionsList.IsShiftPressed = message.IsShiftPressed;

			switch (message.MessageType)
			{
				case MessageType.MouseDown:
					this.SetDirty();
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

				case MessageType.MouseWheel:
					if (this.dimensionsList.Wheel(message.Wheel))
					{
						this.OnChildrenGeometryChanged();  // met à jour les proxies
						this.module.DesignerApplication.UpdateInfoViewer();
					}
					message.Consumer = this;
					break;

				case MessageType.MouseLeave:
					this.SetEnteredObjects(null);
					this.SetHilitedObject(null, null);
					this.SetHilitedParent(null, GridSelection.Invalid, GridSelection.Invalid, 0, 0);
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

			if (this.context.Tool == "ToolGrid")
			{
				this.GridDown(pos, isRightButton, isControlPressed, isShiftPressed);
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

			if (this.context.Tool == "ToolGrid")
			{
				this.GridMove(pos, isRightButton, isControlPressed, isShiftPressed);
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

			if (this.context.Tool == "ToolGrid")
			{
				this.GridUp(pos, isRightButton, isControlPressed, isShiftPressed);
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

			if (this.context.Tool == "ToolGrid")
			{
				this.GridKeyChanged(isControlPressed, isShiftPressed);
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
			Widget obj;

			if (this.selectedObjects.Count != 0 && !this.isDragging && !this.handlesList.IsDragging)
			{
				this.isDraggingDimension = this.dimensionsList.DraggingStart(pos, isControlPressed, isShiftPressed);
				if (this.isDraggingDimension)
				{
					return;
				}
			}

#if false
			if (this.HandlingStart(pos))
			{
				return;
			}
#endif

#if false
			if (this.SizeMarkDraggingStart(pos))
			{
				return;
			}
#endif

			Attachment attachment;
			if (this.AttachmentDetect(pos, out obj, out attachment))
			{
				this.ChangeObjectAttachment(obj, attachment);  // modifie les attachements
				return;
			}

			obj = this.Detect(pos, isShiftPressed, false);  // objet visé par la souris

			if (!isShiftPressed)  // touche Shift relâchée ?
			{
				if (obj != null && this.selectedObjects.Contains(obj) && obj != this.panel)
				{
					this.DraggingStart(pos);
					return;
				}
				this.selectedObjects.Clear();
				this.GridClearSelection();
				this.UpdateAfterChanging(Viewers.Changing.Selection);
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
				this.GridClearSelection();
				this.UpdateAfterChanging(Viewers.Changing.Selection);

				if (obj != this.panel)
				{
					this.DraggingStart(pos);
				}
			}

			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection ponctuelle, souris déplacée.
			if (this.handlesList.IsFinger || this.isSizeMarkHorizontal || this.isSizeMarkVertical || this.isHilitedDimension || this.isDraggingDimension)
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
			else if (this.isDraggingDimension)
			{
				this.dimensionsList.DraggingMove(pos, isControlPressed, isShiftPressed);
			}
			else
			{
				//	Met en évidence la cote survolée par la souris.
				if (this.selectedObjects.Count != 0 && !this.isDragging && !this.handlesList.IsDragging)
				{
					this.isHilitedDimension = this.dimensionsList.Hilite(pos);
					if (this.isHilitedDimension)
					{
						this.SetHilitedObject(null, null);
						this.SetHilitedAttachmentRectangle(Rectangle.Empty);
						return;
					}
				}

				Widget obj = this.Detect(pos, isShiftPressed, false);
				this.SetHilitedObject(obj, null);  // met en évidence l'objet survolé par la souris

				Rectangle rect = Rectangle.Empty;
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

			if (this.isDraggingDimension)
			{
				this.dimensionsList.DraggingEnd(pos);
				this.isDraggingDimension = false;
				this.OnChildrenGeometryChanged();  // met à jour les proxies
				this.module.DesignerApplication.UpdateInfoViewer();
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

			Widget obj = this.Detect(pos, isShiftPressed, false);  // objet visé par la souris

			if (!isShiftPressed)  // touche Shift relâchée ?
			{
				if (obj != null && this.selectedObjects.Contains(obj) && obj != this.panel)
				{
					this.DraggingStart(pos);
					return;
				}
				this.selectedObjects.Clear();
				this.GridClearSelection();
				this.UpdateAfterChanging(Viewers.Changing.Selection);
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

		#region ProcessMouse grid
		protected void GridDown(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection de tableaux, souris pressée.
			this.lastCreatedObject = null;

			this.startingPos = pos;
			this.isGridding = false;

			if (this.HandlingStart(pos))
			{
				return;
			}

			Widget obj = this.Detect(pos, false, true);  // objet tableau visé par la souris
			int column, row;
			this.GridDetect(pos, obj, out column, out row);
			if (column == GridSelection.Invalid && row == GridSelection.Invalid)
			{
				this.selectedObjects.Clear();
				this.GridClearSelection();
				this.UpdateAfterChanging(Viewers.Changing.Selection);
			}
			else
			{
				this.selectedObjects.Clear();
				this.selectedObjects.Add(obj);
				this.UpdateAfterChanging(Viewers.Changing.Selection);
				this.SetHilitedObject(null, null);

				this.griddingInitial = null;
				if (isShiftPressed)
				{
					GridSelection gs = GridSelection.Get(obj);
					if (gs != null)
					{
						this.griddingInitial = new GridSelection(obj);
						gs.CopyTo(this.griddingInitial);
					}
				}
				else
				{
					this.GridClearSelection(obj);
				}

				this.isGridding = true;
				this.isGriddingColumn = false;
				this.isGriddingRow = false;
				this.griddingColumn = column;
				this.griddingRow = row;
				this.GridAdaptSelection(obj, column, row, isShiftPressed);
			}

			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void GridMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection de tableaux, souris déplacée.
			if (this.handlesList.IsFinger)
			{
				this.ChangeMouseCursor(MouseCursorType.Finger);
			}
			else if (isShiftPressed)
			{
				this.ChangeMouseCursor(MouseCursorType.GridPlus);
			}
			else
			{
				this.ChangeMouseCursor(MouseCursorType.Grid);
			}

			Widget obj = this.Detect(pos, false, true);  // objet tableau visé par la souris
			int column, row;
			this.GridDetect(pos, obj, out column, out row);

			if (this.isGridding)
			{
				this.GridAdaptSelection(obj, column, row, isShiftPressed);
			}
			else if (this.handlesList.IsDragging)
			{
				this.HandlingMove(pos);
			}
			else
			{
				if (obj == null)
				{
					this.SetHilitedObject(null, null);
				}
				else
				{
					GridSelection hgs = new GridSelection(obj);

					if (isShiftPressed)
					{
						GridSelection ags = GridSelection.Get(obj);

						if (column != GridSelection.Invalid && (ags == null || !ags.AreOnlyRows))
						{
							hgs.Add(GridSelection.Unit.Column, column);
						}

						if (row != GridSelection.Invalid && (ags == null || !ags.AreOnlyColumns))
						{
							hgs.Add(GridSelection.Unit.Row, row);
						}

						if (ags != null && ags.AreMix)
						{
							hgs.SelectColumnsAndRows(this.griddingColumn, column, this.griddingRow, row);
						}
					}
					else
					{
						if (column != GridSelection.Invalid)
						{
							hgs.Add(GridSelection.Unit.Column, column);
						}

						if (row != GridSelection.Invalid)
						{
							hgs.Add(GridSelection.Unit.Row, row);
						}
					}

					this.SetHilitedObject(obj, hgs);
				}
			}
		}

		protected void GridUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection de tableaux, souris relâchée.
			if (this.isGridding)
			{
				this.OnChildrenSelected();
				this.isGridding = false;
				this.griddingInitial = null;
			}

			if (this.handlesList.IsDragging)
			{
				this.HandlingEnd(pos);
			}
		}

		protected void GridAdaptSelection(Widget obj, int column, int row, bool isShiftPressed)
		{
			//	Adapte la sélection selon le mouvement de la souris.
			if (obj == null)
			{
				return;
			}

			GridSelection gs;

			if (isShiftPressed)
			{
				gs = GridSelection.Get(obj);
				if (gs != null)
				{
					if (gs.AreOnlyColumns && column != GridSelection.Invalid)
					{
						gs.ChangeColumnSelection(column, this.griddingInitial);
						this.isGriddingColumn = true;
						this.isGriddingRow = false;

						this.Invalidate();
						return;
					}

					if (gs.AreOnlyRows && row != GridSelection.Invalid)
					{
						gs.ChangeRowSelection(row, this.griddingInitial);
						this.isGriddingColumn = false;
						this.isGriddingRow = true;

						this.Invalidate();
						return;
					}
				}

				if (this.isGriddingColumn || this.isGriddingRow)
				{
					return;
				}

				gs = new GridSelection(obj);
				gs.SelectColumnsAndRows(this.griddingColumn, column, this.griddingRow, row);

				if (!GridSelection.EqualValues(gs, GridSelection.Get(obj)))
				{
					GridSelection.Attach(obj, gs);
					this.Invalidate();
				}
				return;
			}

			gs = new GridSelection(obj);

			if (column == this.griddingColumn && row == this.griddingRow)
			{
				if (column != GridSelection.Invalid)
				{
					gs.Add(GridSelection.Unit.Column, column);
				}

				if (row != GridSelection.Invalid)
				{
					gs.Add(GridSelection.Unit.Row, row);
				}

				this.isGriddingColumn = (row == GridSelection.Invalid);
				this.isGriddingRow = (column == GridSelection.Invalid);
			}
			else if (column == this.griddingColumn && column != GridSelection.Invalid)
			{
				gs.Add(GridSelection.Unit.Column, column);

				this.isGriddingColumn = true;
				this.isGriddingRow = false;
			}
			else if (row == this.griddingRow && row != GridSelection.Invalid)
			{
				gs.Add(GridSelection.Unit.Row, row);

				this.isGriddingColumn = false;
				this.isGriddingRow = true;
			}
			else if (this.isGriddingColumn && column != GridSelection.Invalid)
			{
				if (column >= this.griddingColumn)
				{
					for (int i=column; i>=this.griddingColumn; i--)
					{
						gs.Add(GridSelection.Unit.Column, i);
					}
				}
				else
				{
					for (int i=column; i<=this.griddingColumn; i++)
					{
						gs.Add(GridSelection.Unit.Column, i);
					}
				}
			}
			else if (this.isGriddingRow && row != GridSelection.Invalid)
			{
				if (row >= this.griddingRow)
				{
					for (int i=row; i>=this.griddingRow; i--)
					{
						gs.Add(GridSelection.Unit.Row, i);
					}
				}
				else
				{
					for (int i=row; i<=this.griddingRow; i++)
					{
						gs.Add(GridSelection.Unit.Row, i);
					}
				}
			}

			if (!GridSelection.EqualValues(gs, GridSelection.Get(obj)))
			{
				GridSelection.Attach(obj, gs);
				this.Invalidate();
			}
		}

		protected void GridKeyChanged(bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection de tableaux, touche pressée ou relâchée.
			if (isShiftPressed)
			{
				this.ChangeMouseCursor(MouseCursorType.GridPlus);
			}
			else
			{
				this.ChangeMouseCursor(MouseCursorType.Grid);
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

			this.SetHilitedObject(this.Detect(pos, false, false), null);  // met en évidence l'objet survolé par la souris
		}

		protected void EditUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Edition, souris relâchée.
			Widget obj = this.Detect(pos, false, false);
			if (obj != null)
			{
				this.ChangeTextResource(obj);
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

			Point initialPos = pos;
			this.isInside = true;
			Widget parent = this.DetectGroup(pos);

			this.creatingObject = this.CreateObjectItem();
			this.CreateObjectAdjust(ref pos, parent, out this.creatingRectangle);

			this.creatingOrigin = this.MapClientToScreen(this.ConvPanelToEditor(Point.Zero));
			this.creatingWindow = new DragWindow();
			this.creatingWindow.DefineWidget(this.creatingObject, this.creatingObject.PreferredSize, Margins.Zero);
			this.creatingWindow.WindowLocation = this.creatingOrigin + pos;
			this.creatingWindow.Owner = this.Window;
			this.creatingWindow.FocusWidget (this.creatingObject);
			this.creatingWindow.Show();

			Separator sep = new Separator();
			sep.Anchor = AnchorStyles.All;
			sep.Color = PanelsContext.ColorOutsideForeground;
			sep.Alpha = 0;
			sep.SetParent(this.creatingWindow.Root);  // parent en dernier pour éviter les flashs !

			this.constrainsList.Starting(Rectangle.Empty, false);

			if (this.objectModifier.AreChildrenAnchored(parent))
			{
				this.constrainsList.Activate(this.creatingRectangle, this.GetObjectBaseLine(this.creatingObject), null);
			}

			int column, row;
			this.GridDetect(initialPos, parent, out column, out row);
			this.SetHilitedParent(parent, column, row, 1, 1);  // met en évidence le futur parent survolé par la souris

			this.module.DesignerApplication.UpdateInfoViewer();
		}

		protected void CreateObjectMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, souris déplacée.
			this.ChangeMouseCursor(MouseCursorType.Pen);

			if (this.creatingObject != null)
			{
				Point initialPos = pos;
				this.isInside = this.IsInside(pos);
				Widget parent = this.DetectGroup(pos);
				int column = GridSelection.Invalid;
				int row = GridSelection.Invalid;

				this.CreateObjectAdjust(ref pos, parent, out this.creatingRectangle);

				if (this.objectModifier.AreChildrenAnchored(parent))
				{
					Rectangle rect = this.isInside ? this.creatingRectangle : Rectangle.Empty;
					this.constrainsList.Activate(rect, this.GetObjectBaseLine(this.creatingObject), null);

					this.SetHilitedZOrderRectangle(Rectangle.Empty);
				}
				else if (this.objectModifier.AreChildrenStacked(parent))
				{
					this.constrainsList.Activate(Rectangle.Empty, 0, null);

					Widget group;
					int order;
					ObjectModifier.StackedHorizontalAttachment ha;
					ObjectModifier.StackedVerticalAttachment va;
					Rectangle hilite;
					this.ZOrderDetect(initialPos, parent, out group, out order, out ha, out va, out hilite);
					this.SetHilitedZOrderRectangle(hilite);
				}
				else if (this.objectModifier.AreChildrenGrid(parent))
				{
					this.GridDetect(initialPos, parent, out column, out row);
					if (!this.objectModifier.IsGridCellEmpty(parent, null, column, row, 1, 1))
					{
						this.isInside = false;
					}

					this.constrainsList.Activate(Rectangle.Empty, 0, null);
					this.SetHilitedZOrderRectangle(Rectangle.Empty);
				}
				else
				{
					this.constrainsList.Activate(Rectangle.Empty, 0, null);
					this.SetHilitedZOrderRectangle(Rectangle.Empty);
				}

				this.creatingWindow.WindowLocation = this.creatingOrigin + pos;
				this.creatingWindow.SuperLight = !this.isInside;
				this.ChangeSeparatorAlpha(this.creatingWindow);

				if (this.isInside)
				{
					this.SetHilitedParent(parent, column, row, 1, 1);  // met en évidence le futur parent survolé par la souris
				}
				else
				{
					this.SetHilitedParent(null, GridSelection.Invalid, GridSelection.Invalid, 0, 0);
				}

				this.module.DesignerApplication.UpdateInfoViewer();
			}
		}

		protected void CreateObjectUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, souris relâchée.
			if (this.creatingObject != null)
			{
				this.isInside = this.IsInside(pos);
				Widget parent = this.DetectGroup(pos);
				Undo.Shapshot action = null;

				if (this.isInside)
				{
					action = this.viewersPanels.UndoCurrentSnapshot(Res.Strings.Action.PanelCreateObject);

					Point initialPos = pos;
					this.CreateObjectAdjust(ref pos, parent, out this.creatingRectangle);

					if (this.objectModifier.AreChildrenAnchored(parent))
					{
						this.creatingObject = this.CreateObjectItem();
						this.creatingObject.SetParent(parent);
						this.creatingObject.TabNavigationMode = TabNavigationMode.None;
						this.objectModifier.AdaptFromParent(this.creatingObject, ObjectModifier.StackedHorizontalAttachment.None, ObjectModifier.StackedVerticalAttachment.None);
						this.SetObjectPosition(this.creatingObject, pos);
					}

					if (this.objectModifier.AreChildrenStacked(parent))
					{
						Widget group;
						int order;
						ObjectModifier.StackedHorizontalAttachment ha;
						ObjectModifier.StackedVerticalAttachment va;
						Rectangle hilite;
						this.ZOrderDetect(initialPos, parent, out group, out order, out ha, out va, out hilite);

						this.creatingObject = this.CreateObjectItem();
						this.creatingObject.SetParent(group);
						this.creatingObject.ZOrder = order;
						this.creatingObject.TabNavigationMode = TabNavigationMode.None;
						this.objectModifier.AdaptFromParent(this.creatingObject, ha, va);
					}

					if (this.objectModifier.AreChildrenGrid(parent))
					{
						int column, row;
						this.GridDetect(initialPos, parent, out column, out row);

						if (this.objectModifier.IsGridCellEmpty(parent, null, column, row, 1, 1))
						{
							this.creatingObject = this.CreateObjectItem();
							this.objectModifier.SetGridParentColumnRow(this.creatingObject, parent, column, row);
						}
						else
						{
							this.isInside = false;
						}
					}
				}

				if (this.isInside)
				{
					this.creatingWindow.Hide();
					this.creatingWindow.Dispose();
					this.creatingWindow = null;
				}
				else  // relâché hors de la fenêtre ?
				{
					this.creatingWindow.DissolveAndDisposeWindow();
					this.creatingWindow = null;

					this.creatingObject = null;
				}

				this.SetHilitedZOrderRectangle(Rectangle.Empty);
				this.constrainsList.Ending();
				this.SetHilitedParent(null, GridSelection.Invalid, GridSelection.Invalid, 0, 0);

				this.lastCreatedObject = this.creatingObject;
				this.creatingObject = null;

				if (this.ChangeTextResource(this.lastCreatedObject))  // choix d'un Druid...
				{
					this.viewersPanels.UndoMemorize(action);
				}
				else
				{
					this.lastCreatedObject.Parent.Children.Remove(this.lastCreatedObject);
					this.lastCreatedObject.Dispose();
					this.lastCreatedObject = null;

					this.Invalidate();
				}

				this.module.DesignerApplication.UpdateInfoViewer();
				this.UpdateAfterChanging(Viewers.Changing.Create);
				this.OnUpdateCommands();
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

			if (this.context.Tool == "ObjectSquareButton")
			{
				MetaButton button = new MetaButton();
				button.Text = Misc.Italic("Button");
				this.objectModifier.SetButtonClass(button, ButtonClass.RichDialogButton);
				button.MinWidth = button.PreferredHeight;  // largeur minimale pour former un carré
				button.MinHeight = button.PreferredHeight;
				button.PreferredWidth = button.PreferredHeight;

				item = button;
			}

			if (this.context.Tool == "ObjectRectButton")
			{
				MetaButton button = new MetaButton();
				button.Text = Misc.Italic("Button");
				this.objectModifier.SetButtonClass(button, ButtonClass.FlatButton);
				button.MinWidth = button.PreferredHeight;  // largeur minimale pour former un carré
				button.MinHeight = button.PreferredHeight;

				item = button;
			}

			if (this.context.Tool == "ObjectText")
			{
				item = new UI.Placeholder();
				item.Text = Misc.Italic("TextField");
				item.DrawDesignerFrame = true;  // nécessaire pour voir le cadre pendant la création
			}

			if (this.context.Tool == "ObjectTable")
			{
				item = new UI.TablePlaceholder();
				item.Text = Misc.Italic("Table");
				item.DrawDesignerFrame = true;  // nécessaire pour voir le cadre pendant la création
			}

			if (this.context.Tool == "ObjectGroup")
			{
				FrameBox group = new FrameBox();
				group.ChildrenLayoutMode = LayoutMode.Stacked;
				group.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;
				group.PreferredSize = new Size(200, 100);
				group.Padding = new Margins(10, 10, 10, 10);
				group.DrawFullFrame = false;
				group.DrawDesignerFrame = true;  // nécessaire pour voir le cadre pendant la création

				item = group;
			}

			if (this.context.Tool == "ObjectGroupFrame")
			{
				FrameBox group = new FrameBox();
				group.ChildrenLayoutMode = LayoutMode.Stacked;
				group.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;
				group.PreferredSize = new Size(200, 100);
				group.Padding = new Margins(10, 10, 10, 10);
				group.DrawFullFrame = true;

				item = group;
			}

			if (this.context.Tool == "ObjectGroupBox")
			{
				GroupBox group = new GroupBox();
				group.ChildrenLayoutMode = LayoutMode.Stacked;
				group.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;
				group.Text = Misc.Italic("GroupBox");
				group.PreferredSize = new Size(200, 100);

				item = group;
			}

			if (this.context.Tool == "ObjectPanel")
			{
				UI.PanelPlaceholder panel = new UI.PanelPlaceholder();
				panel.ChildrenLayoutMode = LayoutMode.Stacked;
				panel.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;
				panel.Text = Misc.Italic("Panel");
				panel.PreferredSize = new Size(200, 100);
				panel.DrawDesignerFrame = true;  // nécessaire pour voir le cadre pendant la création
				panel.ResourceManager = this.module.ResourceManager;

				item = panel;
			}

			return item;
		}

		protected void CreateObjectAdjust(ref Point pos, Widget parent, out Rectangle bounds)
		{
			//	Ajuste la position de l'objet à créer selon les contraintes.
			pos.X -= System.Math.Floor(this.creatingObject.PreferredWidth/2);
			pos.Y -= System.Math.Floor(this.creatingObject.PreferredHeight/2);

			bounds = new Rectangle(pos, this.creatingObject.PreferredSize);

			if (this.objectModifier.AreChildrenAnchored(parent) && this.isInside)
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


		protected bool ChangeTextResource(Widget obj)
		{
			//	Choix de la ressource (un Druid) pour l'objet.
			//	Retourne false s'il fallait choisir un Druid et que l'utilisateur ne l'a pas fait.
			if (ObjectModifier.HasDruid(obj))
			{
				ResourceAccess.Type type;
				List<Druid> exclude = null;

				ObjectModifier.ObjectType oType = ObjectModifier.GetObjectType(obj);
				if (oType == ObjectModifier.ObjectType.Button)
				{
					type = ResourceAccess.Type.Commands;
				}
				else if (oType == ObjectModifier.ObjectType.SubPanel)
				{
					type = ResourceAccess.Type.Panels;

					exclude = new List<Druid>();
					exclude.Add(this.druid);
				}
				else
				{
					type = ResourceAccess.Type.Captions;
				}

				StructuredTypeClass typeClass = StructuredTypeClass.None;
				Druid druid = Druid.Parse(ObjectModifier.GetDruid(obj));
				bool isNullable = false;
				var result = this.module.DesignerApplication.DlgResourceSelector(Dialogs.ResourceSelectorDialog.Operation.Selection, this.module, type, ref typeClass, ref druid, ref isNullable, exclude, Druid.Empty);
				ObjectModifier.SetDruid(obj, druid.ToString());

				if (result != Common.Dialogs.DialogResult.Yes)  // annuler ?
				{
					return false;
				}

				if (type == ResourceAccess.Type.Panels)
				{
					UI.PanelPlaceholder panel = obj as UI.PanelPlaceholder;
					Size size = panel.PanelPreferredSize;
					Point center = this.objectModifier.GetBounds(obj).Center;

					if (this.objectModifier.HasBounds(obj))
					{
						Rectangle bounds = new Rectangle(center-size/2, size);
						this.objectModifier.SetBounds(obj, bounds);
					}
				}
			}

			if (ObjectModifier.HasBinding(obj))
			{
				//	Choix de la rubrique.
				StructuredType type = ObjectModifier.GetStructuredType(this.panel);
				System.Diagnostics.Debug.Assert(type != null);

				Binding binding = ObjectModifier.GetBinding(obj);
				ObjectModifier.ObjectType oType = ObjectModifier.GetObjectType(obj);
				if (!this.module.DesignerApplication.DlgBindingSelector(this.module, type, oType, ref binding))
				{
					return false;
				}

				ObjectModifier.SetBinding(obj, binding, type);

				if (oType == ObjectModifier.ObjectType.Table)
				{
					ObjectModifier.TableDefineAllColumns(obj);
				}
			}

			return true;
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
				Widget obj = this.selectedObjects[0];

				bool isHorizontalSymetry = false;
				bool isVerticalSymetry = false;

				ObjectModifier.ChildrenPlacement cp = this.objectModifier.GetChildrenPlacement(obj.Parent);

				if (cp == ObjectModifier.ChildrenPlacement.VerticalStacked)
				{
					if (this.objectModifier.GetStackedVerticalAttachment(obj) == ObjectModifier.StackedVerticalAttachment.Fill)
					{
						isVerticalSymetry = true;
					}

					if (this.objectModifier.GetStackedHorizontalAlignment(obj) == ObjectModifier.StackedHorizontalAlignment.Center)
					{
						isHorizontalSymetry = true;
					}
				}

				if (cp == ObjectModifier.ChildrenPlacement.HorizontalStacked)
				{
					if (this.objectModifier.GetStackedHorizontalAttachment(obj) == ObjectModifier.StackedHorizontalAttachment.Fill)
					{
						isHorizontalSymetry = true;
					}

					if (this.objectModifier.GetStackedVerticalAlignment(obj) == ObjectModifier.StackedVerticalAlignment.Center)
					{
						isVerticalSymetry = true;
					}
				}

				if (cp == ObjectModifier.ChildrenPlacement.Grid)
				{
					ObjectModifier.StackedHorizontalAlignment ha = this.objectModifier.GetStackedHorizontalAlignment(obj);
					if (ha == ObjectModifier.StackedHorizontalAlignment.Center || ha == ObjectModifier.StackedHorizontalAlignment.Stretch)
					{
						isHorizontalSymetry = true;
					}

					ObjectModifier.StackedVerticalAlignment va = this.objectModifier.GetStackedVerticalAlignment(obj);
					if (va == ObjectModifier.StackedVerticalAlignment.Center || va == ObjectModifier.StackedVerticalAlignment.Stretch)
					{
						isVerticalSymetry = true;
					}
				}

				this.isHandling = true;
				this.handlingRectangle = this.SelectBounds;
				this.handlesList.DraggingStart(pos, this.handlingType, isHorizontalSymetry, isVerticalSymetry);

				CloneView clone = new CloneView();
				clone.Model = this.selectedObjects[0];

				this.handlingWindow = new DragWindow();
				this.handlingWindow.DefineWidget(clone, this.handlingRectangle.Size, Margins.Zero);
				this.handlingWindow.WindowBounds = this.MapClientToScreen(this.ConvPanelToEditor(this.handlingRectangle));
				this.handlingWindow.Owner = this.Window;
				this.handlingWindow.FocusWidget (clone);
				this.handlingWindow.Show();

				this.SetEnteredObjects(null);
				this.SetHilitedObject(null, null);
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
				this.handlingRectangle = this.handlesList.DraggingMove(pos);
				this.handlingWindow.WindowBounds = this.MapClientToScreen(this.ConvPanelToEditor(this.handlingRectangle));
				this.module.DesignerApplication.UpdateInfoViewer();
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
				this.OnChildrenGeometryChanged();  // met à jour les proxies
				this.module.DesignerApplication.UpdateInfoViewer();
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
			this.isInside = true;
			Widget parent = this.DetectGroup(pos);

			int column = GridSelection.Invalid;
			int row = GridSelection.Invalid;
			this.draggingSpanColumnOffset = 0;
			this.draggingSpanRowOffset = 0;
			this.draggingSpanColumnCount = 1;
			this.draggingSpanRowCount = 1;

			if (this.objectModifier.AreChildrenAnchored(parent))
			{
				this.constrainsList.Starting(this.draggingRectangle, false);
			}
			else if (this.objectModifier.AreChildrenGrid(parent))
			{
				this.constrainsList.Starting(Rectangle.Empty, false);

				if (this.selectedObjects.Count == 1)
				{
					Widget obj = this.selectedObjects[0];
					this.GridDetect(pos, parent, out column, out row);

					this.draggingSpanColumnOffset = column - this.objectModifier.GetGridColumn(obj);
					this.draggingSpanRowOffset = row - this.objectModifier.GetGridRow(obj);
					this.draggingSpanColumnCount = this.objectModifier.GetGridColumnSpan(obj);
					this.draggingSpanRowCount = this.objectModifier.GetGridRowSpan(obj);

					column -= this.draggingSpanColumnOffset;
					row -= this.draggingSpanRowOffset;
				}
			}
			else
			{
				this.constrainsList.Starting(Rectangle.Empty, false);
			}

			this.SetHilitedParent(parent, column, row, this.draggingSpanColumnCount, this.draggingSpanRowCount);  // met en évidence le futur parent survolé par la souris

			FrameBox container = new FrameBox();
			container.PreferredSize = this.draggingRectangle.Size;

			foreach (Widget obj in this.selectedObjects)
			{
				Point origin = this.objectModifier.GetActualBounds(obj).BottomLeft - this.draggingRectangle.BottomLeft;
				CloneView clone = new CloneView(container);
				clone.PreferredSize = obj.ActualSize;
				clone.Margins = new Margins(origin.X, 0, 0, origin.Y);
				clone.Anchor = AnchorStyles.BottomLeft;
				clone.Model = obj;

				Separator sep = new Separator(container);
				sep.PreferredSize = obj.ActualSize;
				sep.Margins = new Margins(origin.X, 0, 0, origin.Y);
				sep.Anchor = AnchorStyles.BottomLeft;
				sep.Color = PanelsContext.ColorOutsideForeground;
				sep.Alpha = 0;
			}

			this.draggingOrigin = this.MapClientToScreen(this.ConvPanelToEditor(this.draggingOffset));
			//?this.draggingOrigin.Y -= 1;  // TODO: cette correction devrait être inutile !
			this.draggingWindow = new DragWindow();
			this.draggingWindow.DefineWidget(container, container.PreferredSize, Margins.Zero);
			this.draggingWindow.WindowLocation = this.draggingOrigin + pos;
			this.draggingWindow.Owner = this.Window;
			this.draggingWindow.FocusWidget (container);
			this.draggingWindow.Show();

			this.SetEnteredObjects(null);
			this.SetHilitedObject(null, null);
			this.SetHilitedAttachmentRectangle(Rectangle.Empty);
			this.isDragging = true;
			this.Invalidate();
		}

		protected void DraggingMove(Point pos)
		{
			//	Mouvement du drag pour déplacer les objets sélectionnés.
			this.isInside = this.IsInside(pos);
			Widget parent = this.DetectGroup(pos);
			int column = GridSelection.Invalid;
			int row = GridSelection.Invalid;
			Point adjust = Point.Zero;

			if (this.objectModifier.AreChildrenAnchored(parent))
			{
				this.draggingRectangle.Offset((this.draggingOffset+pos)-this.draggingRectangle.BottomLeft);
				Rectangle rect = this.isInside ? this.draggingRectangle : Rectangle.Empty;
				this.constrainsList.Activate(rect, this.draggingBaseLine, this.draggingArraySelected);
				this.Invalidate();

				adjust = this.draggingRectangle.BottomLeft;
				if (this.isInside)
				{
					this.draggingRectangle = this.constrainsList.Snap(this.draggingRectangle, this.draggingBaseLine);
				}
				adjust = this.draggingRectangle.BottomLeft - adjust;

				this.SetHilitedZOrderRectangle(Rectangle.Empty);
			}
			else if (this.objectModifier.AreChildrenStacked(parent))
			{
				this.constrainsList.Activate(Rectangle.Empty, 0, null);

				Widget group;
				int order;
				ObjectModifier.StackedHorizontalAttachment ha;
				ObjectModifier.StackedVerticalAttachment va;
				Rectangle hilite;
				this.ZOrderDetect(pos, parent, out group, out order, out ha, out va, out hilite);
				this.SetHilitedZOrderRectangle(hilite);
			}
			else if (this.objectModifier.AreChildrenGrid(parent))
			{
				this.GridDetect(pos, parent, out column, out row);
				if (!this.IsDraggingGridPossible(parent, ref column, ref row))
				{
					this.isInside = false;
				}
			}
			else
			{
				this.constrainsList.Activate(Rectangle.Empty, 0, null);
				this.SetHilitedZOrderRectangle(Rectangle.Empty);
			}

			this.draggingWindow.WindowLocation = this.draggingOrigin + pos + adjust;
			this.draggingWindow.SuperLight = !this.isInside;

			this.ChangeSeparatorAlpha(this.draggingWindow);

			this.SetHilitedParent(parent, column, row, this.draggingSpanColumnCount, this.draggingSpanRowCount);  // met en évidence le futur parent survolé par la souris
			this.module.DesignerApplication.UpdateInfoViewer();
		}

		protected void DraggingEnd(Point pos)
		{
			//	Fin du drag pour déplacer les objets sélectionnés.
			this.isInside = this.IsInside(pos);
			Widget parent = this.DetectGroup(pos);
			Undo.Shapshot action = this.viewersPanels.UndoCurrentSnapshot(null);

			if (this.isInside)
			{
				Widget initialParent = this.draggingArraySelected[0].Parent;

				if (this.objectModifier.AreChildrenAnchored(parent))
				{
					Rectangle initial = this.SelectBounds;
					if (this.MoveSelection(this.draggingRectangle.BottomLeft - initial.BottomLeft, parent))
					{
						this.viewersPanels.UndoMemorize(action, Res.Strings.Action.PanelMove);
						this.OnChildrenGeometryChanged();  // met à jour les proxies
					}
				}

				if (this.objectModifier.AreChildrenStacked(parent))
				{
					Widget group;
					int order;
					ObjectModifier.StackedHorizontalAttachment ha;
					ObjectModifier.StackedVerticalAttachment va;
					Rectangle hilite;
					this.ZOrderDetect(pos, parent, out group, out order, out ha, out va, out hilite);
					if (this.ZOrderChangeSelection(group, order, ha, va))
					{
						this.viewersPanels.UndoMemorize(action, Res.Strings.Action.PanelMove);
						this.OnChildrenGeometryChanged();  // met à jour les proxies
					}
				}

				if (this.objectModifier.AreChildrenGrid(parent))
				{
					int column, row;
					this.GridDetect(pos, parent, out column, out row);

					if (this.IsDraggingGridPossible(parent, ref column, ref row))
					{
						Widget select = this.selectedObjects[0];
						Widget actual = this.objectModifier.GetGridCellWidget(parent, null, column, row);
						bool isChanging = false;

						if (select == actual)
						{
							isChanging |= this.objectModifier.SetGridParentColumnRow(select, parent, column, row);
						}
						else
						{
							Widget ip = select.Parent;
							int ic = this.objectModifier.GetGridColumn(select);
							int ir = this.objectModifier.GetGridRow(select);

							isChanging |= this.objectModifier.SetGridParentColumnRow(select, parent, column, row);
							this.objectModifier.AdaptFromParent(select, ObjectModifier.StackedHorizontalAttachment.None, ObjectModifier.StackedVerticalAttachment.None);

							if (actual != null)
							{
								isChanging |= this.objectModifier.SetGridParentColumnRow(actual, ip, ic, ir);
							}
						}

						if (isChanging)
						{
							this.viewersPanels.UndoMemorize(action, Res.Strings.Action.PanelMove);
							this.OnChildrenGeometryChanged();  // met à jour les proxies
						}
					}
					else
					{
						//	Si on relâche l'objet dans une cellule occupée, on ne fait rien.
						//	L'objet reste ainsi à sa position initiale.
					}
				}

				if (initialParent != this.selectedObjects[0].Parent)
				{
					this.UpdateAfterChanging(Viewers.Changing.Move);
				}
			}

			if (this.isInside)
			{
				this.draggingWindow.Hide();
				this.draggingWindow.Dispose();
				this.draggingWindow = null;
			}
			else  // relâché hors de la fenêtre ?
			{
				this.draggingWindow.DissolveAndDisposeWindow();
				this.draggingWindow = null;
				this.DeleteSelection();

				this.viewersPanels.UndoMemorize(action, Res.Strings.Action.DeleteObject);
			}

			this.SetHilitedParent(null, GridSelection.Invalid, GridSelection.Invalid, 0, 0);
			this.SetHilitedZOrderRectangle(Rectangle.Empty);
			this.isDragging = false;
			this.draggingArraySelected = null;
			this.constrainsList.Ending();
			this.handlesList.UpdateGeometry();
			this.Invalidate();
		}

		protected bool IsDraggingGridPossible(Widget parent, ref int column, ref int row)
		{
			//	Vérifie si la sélection peut être déplacée dans le tableau de destination
			//	donné (parent, column, row). Si la cellule destination est occupée, mais que
			//	l'objet déplacé provient du même tableau, l'opération est autorisée. Les deux
			//	widgets seront alors permutés.
			if (this.selectedObjects.Count != 1)
			{
				return false;
			}

			if (column == GridSelection.Invalid || row == GridSelection.Invalid)
			{
				return false;
			}

			Widget obj = this.selectedObjects[0];
			column -= this.draggingSpanColumnOffset;
			row -= this.draggingSpanRowOffset;

			if (!this.objectModifier.IsGridCellEmpty(parent, obj, column, row, this.draggingSpanColumnCount, this.draggingSpanRowCount))
			{
				Widget sourceParent = obj.Parent;
				if (sourceParent != parent)
				{
					return false;
				}

				Widget actual = this.objectModifier.GetGridCellWidget(obj.Parent, obj, column, row);
				if (actual == null)
				{
					return false;
				}

				if (this.objectModifier.GetGridColumnSpan(obj) != this.objectModifier.GetGridColumnSpan(actual)||
					this.objectModifier.GetGridRowSpan(obj) != this.objectModifier.GetGridRowSpan(actual)||
					column != this.objectModifier.GetGridColumn(actual)||
					row != this.objectModifier.GetGridRow(actual))
				{
					return false;
				}
			}

			return true;
		}
		#endregion


		#region Selection
		public List<int> SelectionToList()
		{
			//	Retourne une liste représentant tous les objets sélectionnés.
			List<int> list = new List<int>();
			int rank = 0;

			if (this.selectedObjects.Contains(this.panel))
			{
				list.Add(rank);
			}
			rank++;

			this.AddSelectionToList(this.panel, list, ref rank);

			return list;
		}

		protected void AddSelectionToList(Widget root, List<int> list, ref int rank)
		{
			foreach (Widget widget in root.Children)
			{
				if (this.selectedObjects.Contains(widget))
				{
					list.Add(rank);
				}

				rank++;

				if (widget.Children.Count > 0)
				{
					this.AddSelectionToList(widget, list, ref rank);
				}
			}
		}

		public void SelectFromList(List<int> list)
		{
			//	Sélectionne les objets d'après une liste.
			this.selectedObjects.Clear();
			int rank = 0;

			if (list.Contains(rank))
			{
				this.selectedObjects.Add(this.panel);
			}
			rank++;

			this.SelectFromList(this.panel, list, ref rank);

			this.GridClearSelection();
			this.UpdateAfterChanging(Viewers.Changing.Selection);
			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectFromList(Widget root, List<int> list, ref int rank)
		{
			foreach (Widget widget in root.Children)
			{
				if (list.Contains(rank))
				{
					this.selectedObjects.Add(widget);
				}

				rank++;

				if (widget.Children.Count > 0)
				{
					this.SelectFromList(widget, list, ref rank);
				}
			}
		}

		protected Widget Detect(Point pos, bool sameParent, bool onlyGrid)
		{
			//	Détecte l'objet visé par la souris, avec priorité au dernier objet
			//	dessiné (donc placé dessus).
			//	Si sameParent = true, l'objet détecté doit avoir le même parent que
			//	l'objet éventuellement déjà sélectionné.
			Widget detected = this.Detect(pos, sameParent, onlyGrid, this.panel);
			if (detected == null && (!sameParent || this.selectedObjects.Count == 0) && (!onlyGrid || this.objectModifier.AreChildrenGrid(this.panel)))
			{
				Rectangle rect = this.panel.Client.Bounds;
				if (rect.Contains(pos))
				{
					rect.Deflate(this.GetDetectPadding(this.panel, onlyGrid));
					if (!rect.Contains(pos))
					{
						detected = this.panel;
					}
				}
			}
			return detected;
		}

		protected Widget Detect(Point pos, bool sameParent, bool onlyGrid, Widget parent)
		{
			Widget[] children = parent.Children.Widgets;
			for (int i=children.Length-1; i>=0; i--)
			{
				Widget widget = children[i];

				if (!widget.Visibility || widget.IsEmbedded)
				{
					continue;
				}

				Rectangle rect = widget.ActualBounds;
				if (rect.Contains(pos))
				{
					Widget deep = this.Detect(widget.MapParentToClient(pos), sameParent, onlyGrid, widget);
					if (deep != null)
					{
						return deep;
					}

					if (ObjectModifier.IsAbstractGroup(widget))
					{
						rect.Deflate(this.GetDetectPadding(widget, onlyGrid));
						if (rect.Contains(pos))
						{
							continue;
						}
					}

					if (sameParent && this.selectedObjects.Count > 0)
					{
						if (widget.Parent != this.selectedObjects[0].Parent)
						{
							continue;
						}
					}

					if (onlyGrid && !this.objectModifier.AreChildrenGrid(widget))
					{
						continue;
					}

					return widget;
				}
			}

			return null;
		}

		protected Margins GetDetectPadding(Widget obj, bool insideGrid)
		{
			//	Retourne les marges intérieures pour la détection du padding.
			if (insideGrid && this.objectModifier.AreChildrenGrid(obj))
			{
				//	On rend des marges maximales pour accepter la détection dans toute
				//	la surface de l'objet.
				return new Margins(double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue);
			}

			Margins padding = obj.Padding;
			padding += obj.GetInternalPadding();

			padding.Left   = System.Math.Max(padding.Left,   this.context.GroupOutline);
			padding.Right  = System.Math.Max(padding.Right,  this.context.GroupOutline);
			padding.Bottom = System.Math.Max(padding.Bottom, this.context.GroupOutline);
			padding.Top    = System.Math.Max(padding.Top,    this.context.GroupOutline);

			return padding;
		}

		protected Widget DetectGroup(Point pos)
		{
			//	Détecte le groupe visé par la souris.
			if (this.IsInside(pos))
			{
				Widget container = this.panel.FindChild(pos, this.selectedObjects, WidgetChildFindMode.Deep | WidgetChildFindMode.SkipHidden | WidgetChildFindMode.SkipNonContainer | WidgetChildFindMode.SkipEmbedded);
				return container ?? this.panel;
			}
			else
			{
				return null;
			}
		}

		public void PrepareForDelete()
		{
			//	Préparation en vue de la suppression de l'interface.
			this.creatingObject = null;
			this.lastCreatedObject = null;
			this.hilitedObject = null;
			this.hilitedParent = null;
			this.DeselectAll();
		}

		public void DeselectAll()
		{
			//	Désélectionne tous les objets.
			if (this.selectedObjects.Count > 0)
			{
				this.selectedObjects.Clear();
				this.GridClearSelection();
				this.UpdateAfterChanging(Viewers.Changing.Selection);
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

			this.GridClearSelection();
			this.UpdateAfterChanging(Viewers.Changing.Selection);
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

			this.GridClearSelection();
			this.UpdateAfterChanging(Viewers.Changing.Selection);
			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectRoot()
		{
			//	Sélectionne le panneau de base.
			this.selectedObjects.Clear();
			this.selectedObjects.Add(this.panel);

			this.GridClearSelection();
			this.UpdateAfterChanging(Viewers.Changing.Selection);
			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectParent()
		{
			//	Sélectionne l'objet parent de l'actuelle sélection.
			System.Diagnostics.Debug.Assert(this.selectedObjects.Count != 0);
			Widget parent = this.selectedObjects[0].Parent;
			this.selectedObjects.Clear();
			this.selectedObjects.Add(parent);

			this.GridClearSelection();
			this.UpdateAfterChanging(Viewers.Changing.Selection);
			this.OnChildrenSelected();
			this.Invalidate();
		}

		public void SelectOneObject(Widget obj)
		{
			//	Sélectionne un objet.
			this.selectedObjects.Clear();
			this.selectedObjects.Add(obj);
			this.GridClearSelection();
			this.UpdateAfterChanging(Viewers.Changing.Selection);
			this.OnChildrenSelected();
			this.Invalidate();
		}

		public void SelectListObject(List<Widget> list)
		{
			//	Sélectionne une liste d'objets.
			this.selectedObjects = list;
			this.GridClearSelection();
			this.UpdateAfterChanging(Viewers.Changing.Selection);
			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectObjectsInRectangle(Rectangle sel)
		{
			//	Sélectionne tous les objets entièrement inclus dans un rectangle.
			//	Tous les objets sélectionnés doivent avoir le même parent.
			this.SelectObjectsInRectangle(sel, this.panel);
			this.GridClearSelection();
			this.UpdateAfterChanging(Viewers.Changing.Selection);
			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectObjectsInRectangle(Rectangle sel, Widget parent)
		{
			if (parent.HasChildren)
			{
				foreach (Widget obj in parent.Children)
				{
					if (sel.Contains(this.objectModifier.GetActualBounds(obj)))
					{
						if (this.selectedObjects.Count > 0)
						{
							if (obj.Parent != this.selectedObjects[0].Parent)
							{
								continue;
							}
						}

						this.selectedObjects.Add(obj);
					}
					else
					{
						if (ObjectModifier.IsAbstractGroup(obj))
						{
							this.SelectObjectsInRectangle(sel, obj);
						}
					}
				}
			}
		}

		public void RegenerateDimensions()
		{
			//	Régénère les cotes s'il y a eu un changement.
			this.dimensionsList.UpdateSelection();
		}

		public void UpdateAfterSelectionGridChanged()
		{
			//	Mise à jour après un changement de sélection dans un tableau.
			this.dimensionsList.UpdateSelection();
			this.OnChildrenSelected();  // met à jour les panneaux des proxies à droite
		}

		protected void UpdateAfterChanging(Viewers.Changing oper)
		{
			//	Mise à jour après un changement de sélection, ou après un changement dans
			//	l'arbre des objets (création, changement de parenté, etc.).
			this.module.DesignerApplication.UpdateViewer(oper);
			this.handlesList.UpdateSelection();
			this.dimensionsList.UpdateSelection();
			GeometryCache.Clear(this.panel);
		}

		public void SetEnteredObjects(List<Widget> list)
		{
			//	Détermine l'objet survolé depuis la barre de statut.
			this.enteredObjects = list;
			this.Invalidate();
		}

		protected void SetHilitedObject(Widget obj, GridSelection grid)
		{
			//	Détermine l'objet à mettre en évidence lors d'un survol.
			if (this.hilitedObject != obj || !GridSelection.EqualValues(this.hilitedGrid, grid))
			{
				this.hilitedObject = obj;
				this.hilitedGrid = grid;
				this.Invalidate();
			}
		}

		protected void SetHilitedParent(Widget obj, int column, int row, int columnCount, int rowCount)
		{
			//	Détermine l'objet parent à mettre en évidence lors d'un survol.
			if (this.hilitedParent != obj || this.hilitedParentColumn != column || this.hilitedParentRow != row || this.hilitedParentColumnCount != columnCount || this.hilitedParentRowCount != rowCount)
			{
				this.hilitedParent = obj;
				this.hilitedParentColumn = column;
				this.hilitedParentRow = row;
				this.hilitedParentColumnCount = columnCount;
				this.hilitedParentRowCount = rowCount;
				this.Invalidate();
			}
		}

		protected void SetSelectRectangle(Rectangle rect)
		{
			//	Détermine la zone du rectangle de sélection.
			if (this.selectedRectangle != rect)
			{
				this.Invalidate(this.ConvPanelToEditor(this.selectedRectangle));  // invalide l'ancienne zone
				this.selectedRectangle = rect;
				this.Invalidate(this.ConvPanelToEditor(this.selectedRectangle));  // invalide la nouvelle zone
			}
		}

		protected void SetHilitedAttachmentRectangle(Rectangle rect)
		{
			//	Détermine la zone du rectangle d'attachement.
			if (this.hilitedAttachmentRectangle != rect)
			{
				this.Invalidate(this.ConvPanelToEditor(this.hilitedAttachmentRectangle));  // invalide l'ancienne zone
				this.hilitedAttachmentRectangle = rect;
				this.Invalidate(this.ConvPanelToEditor(this.hilitedAttachmentRectangle));  // invalide la nouvelle zone
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
			this.GridClearSelection();
			this.UpdateAfterChanging(Viewers.Changing.Delete);
			this.OnChildrenSelected();
			this.Invalidate();
			this.SetDirty();
		}

		protected void DuplicateSelection()
		{
			//	Duplique tous les objets sélectionnés.
			//	TODO:
		}

		protected void MoveRibbonSelection(Point direction)
		{
			//	Déplace tous les objets sélectionnés selon le ruban 'Move'.
			this.viewersPanels.UndoMemorize(Res.Strings.Action.PanelMove, false);

			direction.X *= this.module.DesignerApplication.MoveHorizontal;
			direction.Y *= this.module.DesignerApplication.MoveVertical;
			this.MoveSelection(direction, null);
			this.handlesList.UpdateGeometry();
			this.SetDirty();
		}

		protected bool MoveSelection(Point move, Widget parent)
		{
			//	Déplace et change de parent pour tous les objets sélectionnés.
			bool isMoving = false;

			foreach (Widget obj in this.selectedObjects)
			{
				Rectangle bounds = this.objectModifier.GetBounds(obj);
				if (!move.IsZero)
				{
					bounds.Offset(move);
				}

				if (parent != null)
				{
					if (obj.Parent != parent)
					{
						obj.Parent.Children.Remove(obj);
						parent.Children.Add(obj);
						isMoving = true;
					}

					this.objectModifier.AdaptFromParent(obj, ObjectModifier.StackedHorizontalAttachment.None, ObjectModifier.StackedVerticalAttachment.None);
				}

				Rectangle current = this.GetObjectPreferredBounds(obj);
				bounds.Size = current.Size;

				if (bounds != current)
				{
					this.SetObjectPreferredBounds(obj, bounds);
					isMoving = true;
				}
			}

			this.Invalidate();
			return isMoving;
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
						this.SetObjectPositionY(obj, bounds.Top-this.GetObjectPreferredBounds(obj).Height);
					}
				}
				else  // centré verticalement ?
				{
					foreach (Widget obj in this.selectedObjects)
					{
						this.SetObjectPositionY(obj, System.Math.Floor(bounds.Center.Y-this.GetObjectPreferredBounds(obj).Height/2));
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
						this.SetObjectPositionX(obj, bounds.Right-this.GetObjectPreferredBounds(obj).Width);
					}
				}
				else  // centré horizontalement ?
				{
					foreach (Widget obj in this.selectedObjects)
					{
						this.SetObjectPositionX(obj, System.Math.Floor(bounds.Center.X-this.GetObjectPreferredBounds(obj).Width/2));
					}
				}
			}

			this.Invalidate();
			this.SetDirty();
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
			this.SetDirty();
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
					Rectangle rect = this.GetObjectPreferredBounds(obj);
					rect.Bottom = bounds.Bottom;
					rect.Height = bounds.Height;
					this.SetObjectPreferredBounds(obj, rect);
				}
			}
			else
			{
				foreach (Widget obj in this.selectedObjects)
				{
					Rectangle rect = this.GetObjectPreferredBounds(obj);
					rect.Left  = bounds.Left;
					rect.Width = bounds.Width;
					this.SetObjectPreferredBounds(obj, rect);
				}
			}

			this.Invalidate();
			this.SetDirty();
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
			this.UpdateAfterChanging(Viewers.Changing.Move);
			this.OnUpdateCommands();
			this.SetDirty();
		}

		protected void SelectTabIndexRenum()
		{
			//	Renumérote toutes les touches Tab.
			this.SelectTabIndexRenum(this.panel);

			this.Invalidate();
			this.context.ShowTabIndex = true;
			this.OnUpdateCommands();
			this.SetDirty();
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
				obj.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				if (ObjectModifier.IsAbstractGroup(obj))
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
					obj.ClearValue (Widget.TabNavigationModeProperty);
					obj.ClearValue (Widget.TabIndexProperty);
				}
				else
				{
					int oldIndex = obj.TabIndex;
					int index = obj.TabIndex + direction;
					index = System.Math.Max(index, 0);
					index = System.Math.Min(index, obj.Parent.Children.Count-1);
					this.SelectTabIndex(obj.Parent, index, oldIndex);
					obj.TabIndex = index;
					obj.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				}
			}

			this.Invalidate();
			this.context.ShowTabIndex = true;
			this.OnUpdateCommands();
			this.SetDirty();
		}

		protected void SelectTabIndex(Widget parent, int oldIndex, int newIndex)
		{
			foreach (Widget obj in parent.Children)
			{
				if (Editor.IsObjectTabActive(obj) && obj.TabIndex == oldIndex)
				{
					obj.TabIndex = newIndex;
					obj.TabNavigationMode = TabNavigationMode.ActivateOnTab;
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
					bounds = Rectangle.Union(bounds, this.objectModifier.GetActualBounds(obj));
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


		protected void ChangeObjectAttachment(Widget obj, Attachment attachment)
		{
			//	Modifie le système d'attachement d'un objet.
			if (this.objectModifier.AreChildrenAnchored(obj.Parent))
			{
				Rectangle bounds = this.objectModifier.GetBounds(obj);

				ObjectModifier.AnchoredHorizontalAttachment ha = this.objectModifier.GetAnchoredHorizontalAttachment(obj);
				ObjectModifier.AnchoredVerticalAttachment   va = this.objectModifier.GetAnchoredVerticalAttachment(obj);

				if (attachment == Attachment.Left)
				{
					if (ha == ObjectModifier.AnchoredHorizontalAttachment.Right)
					{
						ha = ObjectModifier.AnchoredHorizontalAttachment.Fill;
					}
					else
					{
						ha = ObjectModifier.AnchoredHorizontalAttachment.Right;
					}
				}

				if (attachment == Attachment.Right)
				{
					if (ha == ObjectModifier.AnchoredHorizontalAttachment.Left)
					{
						ha = ObjectModifier.AnchoredHorizontalAttachment.Fill;
					}
					else
					{
						ha = ObjectModifier.AnchoredHorizontalAttachment.Left;
					}
				}

				if (attachment == Attachment.Bottom)
				{
					if (va == ObjectModifier.AnchoredVerticalAttachment.Top)
					{
						va = ObjectModifier.AnchoredVerticalAttachment.Fill;
					}
					else
					{
						va = ObjectModifier.AnchoredVerticalAttachment.Top;
					}
				}

				if (attachment == Attachment.Top)
				{
					if (va == ObjectModifier.AnchoredVerticalAttachment.Bottom)
					{
						va = ObjectModifier.AnchoredVerticalAttachment.Fill;
					}
					else
					{
						va = ObjectModifier.AnchoredVerticalAttachment.Bottom;
					}
				}

				this.objectModifier.SetAnchoredHorizontalAttachment(obj, ha);
				this.objectModifier.SetAnchoredVerticalAttachment(obj, va);
				this.objectModifier.SetBounds(obj, bounds);

				this.handlesList.UpdateGeometry();
				this.OnChildrenGeometryChanged();  // met à jour les proxies
			}
		}

		public void SetObjectPositionX(Widget obj, double x)
		{
			if (this.objectModifier.AreChildrenAnchored(obj.Parent))
			{
				Rectangle bounds = this.objectModifier.GetBounds(obj);
				bounds.Offset(x-bounds.Left, 0);
				this.objectModifier.SetBounds(obj, bounds);
			}
		}

		public void SetObjectPositionY(Widget obj, double y)
		{
			if (this.objectModifier.AreChildrenAnchored(obj.Parent))
			{
				Rectangle bounds = this.objectModifier.GetBounds(obj);
				bounds.Offset(0, y-bounds.Bottom);
				this.objectModifier.SetBounds(obj, bounds);
			}
		}

		public void SetObjectPosition(Widget obj, Point pos)
		{
			if (this.objectModifier.AreChildrenAnchored(obj.Parent))
			{
				Rectangle bounds = this.objectModifier.GetBounds(obj);
				bounds.Offset(pos-bounds.BottomLeft);
				this.objectModifier.SetBounds(obj, bounds);
			}
		}

		public Rectangle GetObjectPreferredBounds(Widget obj)
		{
			return this.objectModifier.GetBounds(obj);
		}

		public void SetObjectPreferredBounds(Widget obj, Rectangle bounds)
		{
			if (this.objectModifier.AreChildrenAnchored(obj.Parent))
			{
				this.objectModifier.SetBounds(obj, bounds);
			}

			if (this.objectModifier.AreChildrenStacked(obj.Parent))
			{
				if (this.objectModifier.HasWidth(obj))
				{
					this.objectModifier.SetWidth(obj, bounds.Width);
				}

				if (this.objectModifier.HasHeight(obj))
				{
					this.objectModifier.SetHeight(obj, bounds.Height);
				}
			}

			if (this.objectModifier.AreChildrenGrid(obj.Parent))
			{
				this.objectModifier.SetBounds(obj, bounds);
			}
		}

		public bool IsObjectWidthChanging(Widget obj)
		{
			//	Indique si la largeur d'un objet peut changer.
			//	Utilisé par HandlesList pour déterminer quelles poignées sont visibles.
			return this.objectModifier.HasBounds(obj) || this.objectModifier.HasWidth(obj);
		}

		public bool IsObjectHeightChanging(Widget obj)
		{
			//	Indique si la hauteur d'un objet peut changer.
			//	Utilisé par HandlesList pour déterminer quelles poignées sont visibles.
			return this.objectModifier.HasBounds(obj) || this.objectModifier.HasHeight(obj);
		}

		public double GetObjectBaseLine(Widget obj)
		{
			//	Retourne la position relative de la ligne de base depuis le bas de l'objet.
			return System.Math.Floor(obj.GetBaseLine().Y);
		}

		protected static bool IsObjectTabActive(Widget obj)
		{
			//	Indique si l'objet à un ordre pour la touche Tab.
			return (obj.TabNavigationMode & TabNavigationMode.ActivateOnTab) != 0;
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
				if (Editor.IsObjectTabActive(obj))
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
				if (Editor.IsObjectTabActive(obj))
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
			if (!this.context.ShowAttachment || this.selectedObjects.Count != 1)
			{
				obj = null;
				attachment = Attachment.None;
				return false;
			}

			Attachment[] attachments = { Attachment.Left, Attachment.Right, Attachment.Bottom, Attachment.Top };

			foreach (Widget o in this.selectedObjects)
			{
				if (this.objectModifier.AreChildrenAnchored(o.Parent))
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
			}

			obj = null;
			attachment = Attachment.None;
			return false;
		}

		protected Rectangle GetAttachmentBounds(Widget obj, Attachment style)
		{
			//	Retourne le rectangle englobant un attachement.
			Rectangle bounds = this.objectModifier.GetActualBounds(obj.Parent);
			Rectangle rect = this.objectModifier.GetActualBounds(obj);
			Point p1, p2, p1a, p2a;

			if (style == Attachment.Left)
			{
				p1 = new Point(bounds.Left, rect.Center.Y);
				p2 = new Point(rect.Left, rect.Center.Y);
				p1a = Point.Scale(p1, p2, Editor.attachmentScale);
				p2a = Point.Scale(p2, p1, Editor.attachmentScale);
				p1a.Y -= Editor.attachmentThickness;
				p2a.Y += Editor.attachmentThickness;
				return new Rectangle(p1a, p2a);
			}

			if (style == Attachment.Right)
			{
				p1 = new Point(bounds.Right, rect.Center.Y);
				p2 = new Point(rect.Right, rect.Center.Y);
				p1a = Point.Scale(p1, p2, Editor.attachmentScale);
				p2a = Point.Scale(p2, p1, Editor.attachmentScale);
				p1a.Y -= Editor.attachmentThickness;
				p2a.Y += Editor.attachmentThickness;
				return new Rectangle(p1a, p2a);
			}

			if (style == Attachment.Bottom)
			{
				p1 = new Point(rect.Center.X, bounds.Bottom);
				p2 = new Point(rect.Center.X, rect.Bottom);
				p1a = Point.Scale(p1, p2, Editor.attachmentScale);
				p2a = Point.Scale(p2, p1, Editor.attachmentScale);
				p1a.X -= Editor.attachmentThickness;
				p2a.X += Editor.attachmentThickness;
				return new Rectangle(p1a, p2a);
			}

			if (style == Attachment.Top)
			{
				p1 = new Point(rect.Center.X, bounds.Top);
				p2 = new Point(rect.Center.X, rect.Top);
				p1a = Point.Scale(p1, p2, Editor.attachmentScale);
				p2a = Point.Scale(p2, p1, Editor.attachmentScale);
				p1a.X -= Editor.attachmentThickness;
				p2a.X += Editor.attachmentThickness;
				return new Rectangle(p1a, p2a);
			}

			return Rectangle.Empty;
		}
		#endregion


		#region ZOrder
		protected void ZOrderDetect(Point mouse, Widget parent, out Widget group, out int order, out ObjectModifier.StackedHorizontalAttachment ha, out ObjectModifier.StackedVerticalAttachment va, out Rectangle hilite)
		{
			//	Détecte le ZOrder à utiliser pour une position donnée, ainsi que le rectangle
			//	à utiliser pour la mise ne évidence.
			if (parent == null)
			{
				group = null;
				order = -1;
				ha = ObjectModifier.StackedHorizontalAttachment.None;
				va = ObjectModifier.StackedVerticalAttachment.None;
				hilite = Rectangle.Empty;
				return;
			}

			group = parent;
			order = 0;
			ha = ObjectModifier.StackedHorizontalAttachment.None;
			va = ObjectModifier.StackedVerticalAttachment.None;
			hilite = Rectangle.Empty;

			double t = this.context.ZOrderThickness;

			Widget obj = this.ZOrderDetectNearest(mouse, parent);  // objet le plus proche
			if (obj == null)
			{
				Rectangle bounds = this.objectModifier.GetActualBounds(parent);

				if (this.objectModifier.AreChildrenHorizontal(parent))
				{
					if (mouse.X < bounds.Center.X)
					{
						hilite = new Rectangle(bounds.Left, bounds.Bottom, t*2, bounds.Height);
						ha = ObjectModifier.StackedHorizontalAttachment.Left;
					}
					else
					{
						hilite = new Rectangle(bounds.Right-t*2, bounds.Bottom, t*2, bounds.Height);
						ha = ObjectModifier.StackedHorizontalAttachment.Right;
					}
				}
				else
				{
					if (mouse.Y < bounds.Center.Y)
					{
						hilite = new Rectangle(bounds.Left, bounds.Bottom, bounds.Width, t*2);
						va = ObjectModifier.StackedVerticalAttachment.Bottom;
					}
					else
					{
						hilite = new Rectangle(bounds.Left, bounds.Top-t*2, bounds.Width, t*2);
						va = ObjectModifier.StackedVerticalAttachment.Top;
					}
				}

				order = parent.Children.Count;
			}
			else
			{
				group = obj.Parent;
				order = obj.ZOrder;

				Rectangle bounds = this.objectModifier.GetActualBounds(obj);
				bounds.Inflate(this.objectModifier.GetMargins(obj));

				if (this.objectModifier.AreChildrenHorizontal(parent))
				{
					if (this.objectModifier.HasAttachmentLeft(obj))
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

					ha = this.objectModifier.GetStackedHorizontalAttachment(obj);
				}
				else
				{
					if (this.objectModifier.HasAttachmentBottom(obj))
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

					va = this.objectModifier.GetStackedVerticalAttachment(obj);
				}

				hilite.Inflate(t);
			}

			if (this.selectedObjects.Contains(obj))
			{
				//	Un objet sélectionné n'est jamais pris en compte.
				group = null;
				order = -1;  // aucun changement
				ha = ObjectModifier.StackedHorizontalAttachment.None;
				va = ObjectModifier.StackedVerticalAttachment.None;
				hilite = Rectangle.Empty;
				return;
			}

			if (this.selectedObjects.Count != 0)
			{
				//	Un ZOrder équivalent au ZOrder de l'objet actuellement sélectionné n'est
				//	pas retourné, pour éviter à l'utilisateur de croire qu'il va changer
				//	quelque chose.
				foreach (Widget selectedObj in this.selectedObjects)
				{
					if (selectedObj.Parent == parent)
					{
						if (selectedObj.ZOrder == order || selectedObj.ZOrder == order-1)
						{
							if (this.objectModifier.AreChildrenHorizontal(parent))
							{
								if (this.objectModifier.GetStackedHorizontalAttachment(selectedObj) != ha)
								{
									continue;
								}
							}
							else
							{
								if (this.objectModifier.GetStackedVerticalAttachment(selectedObj) != va)
								{
									continue;
								}
							}

							group = null;
							order = -1;  // aucun changement
							ha = ObjectModifier.StackedHorizontalAttachment.None;
							va = ObjectModifier.StackedVerticalAttachment.None;
							hilite = Rectangle.Empty;
							return;
						}
					}
				}
			}
		}

		protected Widget ZOrderDetectNearest(Point pos, Widget parent)
		{
			//	Détecte l'objet le plus proche de la souris.
			bool horizontal = (this.objectModifier.GetChildrenPlacement(parent) == ObjectModifier.ChildrenPlacement.HorizontalStacked);
			Widget minWidget = null;
			Widget maxWidget = null;

			//	Cherche les objets aux 'extrémités'. Par exemple, en mode VerticalDocked, minWidget
			//	sera l'objet docké en bas le plus haut, et maxWidget l'objet docké en haut le plus bas.
			for (int i=parent.Children.Count-1; i>=0; i--)
			{
				Widget obj = parent.Children[i] as Widget;

				if (horizontal)
				{
					if (minWidget == null && this.objectModifier.HasAttachmentLeft(obj))
					{
						minWidget = obj;
					}

					if (maxWidget == null && this.objectModifier.HasAttachmentRight(obj))
					{
						maxWidget = obj;
					}
				}
				else
				{
					if (minWidget == null && this.objectModifier.HasAttachmentBottom(obj))
					{
						minWidget = obj;
					}

					if (maxWidget == null && this.objectModifier.HasAttachmentTop(obj))
					{
						maxWidget = obj;
					}
				}
			}

			//	S'il n'existe aucun objet à une 'extrémité', détecte si la position est
			//	dans cette zone pour retourner null. Cela permettra à ZOrderDetect d'y
			//	placer un objet.
			if (minWidget != null && maxWidget == null)
			{
				Rectangle box = this.objectModifier.GetActualBounds(parent);

				if (horizontal)
				{
					if (pos.X >= box.Right-parent.Padding.Right)
					{
						return null;
					}
				}
				else
				{
					if (pos.Y >= box.Top-parent.Padding.Top)
					{
						return null;
					}
				}
			}

			if (minWidget == null && maxWidget != null)
			{
				Rectangle box = this.objectModifier.GetActualBounds(parent);

				if (horizontal)
				{
					if (pos.X <= box.Left+parent.Padding.Left)
					{
						return null;
					}
				}
				else
				{
					if (pos.Y <= box.Bottom+parent.Padding.Bottom)
					{
						return null;
					}
				}
			}

			//	Détecte l'objet le plus proche, qu'il soit sélectionné ou non.
			Widget best = null;
			double min = 1000000;

			for (int i=parent.Children.Count-1; i>=0; i--)
			{
				Widget obj = parent.Children[i] as Widget;

				Rectangle bounds = this.objectModifier.GetActualBounds(obj);
				double d;

				if (horizontal)
				{
					d = System.Math.Abs(bounds.Center.X-pos.X);
				}
				else
				{
					d = System.Math.Abs(bounds.Center.Y-pos.Y);
				}

				if (min > d)
				{
					min = d;
					best = obj;
				}
			}

			return best;
		}

		protected void SetHilitedZOrderRectangle(Rectangle rect)
		{
			//	Détermine la zone du rectangle d'insertion ZOrder.
			if (this.hilitedZOrderRectangle != rect)
			{
				this.Invalidate(this.ConvPanelToEditor(this.hilitedZOrderRectangle));  // invalide l'ancienne zone
				this.hilitedZOrderRectangle = rect;
				this.Invalidate(this.ConvPanelToEditor(this.hilitedZOrderRectangle));  // invalide la nouvelle zone
			}
		}

		protected bool ZOrderChangeSelection(Widget parent, int order, ObjectModifier.StackedHorizontalAttachment ha, ObjectModifier.StackedVerticalAttachment va)
		{
			//	Change le ZOrder de tous les objets sélectionnés.
			bool isChanging = false;

			if (order == -1 || this.selectedObjects.Contains(parent))
			{
				return isChanging;
			}

			foreach (Widget obj in this.selectedObjects)
			{
				int newOrder = order;

				if (newOrder > obj.ZOrder && obj.Parent == parent)
				{
					newOrder--;
				}

				bool stacked = this.objectModifier.AreChildrenStacked(obj.Parent);

				if (obj.Parent != parent || obj.ZOrder != newOrder)
				{
					obj.SetParent(parent);
					obj.ZOrder = newOrder;
					isChanging = true;
				}

				if (!stacked)
				{
					this.objectModifier.AdaptFromParent(obj, ha, va);
				}
			}

			return isChanging;
		}
		#endregion


		#region Grid
		public void GridClearSelection()
		{
			//	Supprime toutes les sélections de cellules/colonnes/lignes dans les tableaux.
			this.GridClearSelection(null);
		}

		protected void GridClearSelection(Widget exclude)
		{
			//	Supprime toutes les sélections de cellules/colonnes/lignes dans les tableaux, sauf une.
			if (this.panel != exclude && GridSelection.Get(this.panel) != null)
			{
				GridSelection.Detach(this.panel);
				this.panel.Invalidate();
				this.Invalidate();
			}

			this.GridClearSelection(this.panel, exclude);
		}

		protected void GridClearSelection(Widget parent, Widget exclude)
		{
			if (parent.HasChildren)
			{
				foreach (Widget obj in parent.Children)
				{
					if (ObjectModifier.IsAbstractGroup(obj))
					{
						if (obj != exclude && GridSelection.Get(obj) != null)
						{
							GridSelection.Detach(obj);
							obj.Invalidate();
							this.Invalidate();
						}

						this.GridClearSelection(obj, exclude);
					}
				}
			}
		}

		protected void GridDetect(Point mouse, Widget parent, out int column, out int row)
		{
			//	Détecte la colonne et la ligne visée dans un tableau.
			column = GridSelection.Invalid;
			row = GridSelection.Invalid;

			if (!this.objectModifier.AreChildrenGrid(parent))  return;

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(parent) as GridLayoutEngine;
			if (engine == null)  return;

			Rectangle rect = this.objectModifier.GetFinalPadding(parent);

			int columns = engine.ColumnDefinitions.Count;
			double x = rect.Left;
			for (int c=0; c<columns; c++)
			{
				ColumnDefinition def = engine.ColumnDefinitions[c];
				double w = def.LeftBorder + def.ActualWidth + def.RightBorder;
				if (mouse.X >= x && mouse.X < x+w)
				{
					column = c;
				}
				x += w;
			}

			int rows = engine.RowDefinitions.Count;
			double y = rect.Top;
			for (int r=0; r<rows; r++)
			{
				RowDefinition def = engine.RowDefinitions[r];
				double h = def.TopBorder + def.ActualHeight + def.BottomBorder;
				if (mouse.Y < y && mouse.Y >= y-h)
				{
					row = r;
				}
				y -= h;
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
				this.sizeMarkOffset.Y = pos.Y-this.SizeMarkHorizontalRect.Top;
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
				//	TODO: mettre à jour les proxies...
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
			if (!this.isEditEnabled)
			{
				return;
			}

			IAdorner adorner = Widgets.Adorners.Factory.Active;

			//	Dessine les surfaces inutilisées.
			Rectangle box = this.Client.Bounds;
			Rectangle bounds = this.ConvPanelToEditor(this.RealBounds);

			if (bounds.Top < box.Top)  // bande supérieure ?
			{
				Rectangle part = new Rectangle(box.Left, bounds.Top, box.Width, box.Top-bounds.Top);
				graphics.AddFilledRectangle(part);
				graphics.RenderSolid(PanelsContext.ColorOutsurface);
			}

			if (box.Bottom < bounds.Bottom)  // bande inférieure ?
			{
				Rectangle part = new Rectangle(box.Left, box.Bottom, box.Width, bounds.Bottom-box.Bottom);
				graphics.AddFilledRectangle(part);
				graphics.RenderSolid(PanelsContext.ColorOutsurface);
			}

			if (bounds.Right < box.Right)  // bande droite ?
			{
				Rectangle part = new Rectangle(bounds.Right, bounds.Bottom, box.Right-bounds.Right, bounds.Height);
				graphics.AddFilledRectangle(part);
				graphics.RenderSolid(PanelsContext.ColorOutsurface);
			}

			if (box.Left < bounds.Left)  // bande gauche ?
			{
				Rectangle part = new Rectangle(box.Left, bounds.Bottom, bounds.Left-box.Left, bounds.Height);
				graphics.AddFilledRectangle(part);
				graphics.RenderSolid(PanelsContext.ColorOutsurface);
			}

			Transform it = graphics.Transform;
			graphics.TranslateTransform(Dimension.margin, Dimension.margin);

			bounds = this.RealBounds;

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
				//?this.DrawSizeMark(graphics);
			}

			//	Dessine les objets sélectionnés.
			if (this.selectedObjects.Count > 0 && !this.isDragging && !this.handlesList.IsDragging)
			{
				this.DrawSelectedObjects(graphics);
			}

			//	Dessine les attachements des objets sélectionnés.
			if (this.context.ShowAttachment && this.selectedObjects.Count == 1 && !this.isDragging && !this.handlesList.IsDragging)
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
				this.DrawHilitedObject(graphics, this.hilitedObject, this.hilitedGrid);
			}

			//	Dessine l'objet parent survolé.
			if (this.hilitedParent != null)
			{
				this.DrawHilitedParent(graphics, this.hilitedParent, this.hilitedParentColumn, this.hilitedParentRow, this.hilitedParentColumnCount, this.hilitedParentRowCount);
			}

			//	Dessine l'objet survolé depuis la barre de statut.
			if (this.enteredObjects != null)
			{
				this.DrawEnteredObjects(graphics, this.enteredObjects);
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

			//	Dessine les cotes de l'objet sélectionné.
			if (this.selectedObjects.Count != 0 && !this.isDragging && !this.handlesList.IsDragging)
			{
				this.dimensionsList.Draw(graphics);
			}

			//	Dessine les poignées.
			if (this.selectedObjects.Count == 1 && !this.isDragging)
			{
				//?this.handlesList.Draw(graphics);
			}

			graphics.Transform = it;
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

		protected void DrawSelectedObjects(Graphics graphics)
		{
			foreach (Widget obj in this.selectedObjects)
			{
				this.DrawSelectedObject(graphics, obj);
				this.DrawPadding(graphics, obj, 1.0);
			}

			if (this.selectedObjects.Count > 0)
			{
				Widget obj = this.selectedObjects[0];
				if (obj != this.panel)
				{
					this.DrawPadding(graphics, obj.Parent, 0.4);

					if (this.objectModifier.AreChildrenGrid(obj.Parent))
					{
						this.DrawGrid(graphics, obj.Parent, PanelsContext.ColorHiliteOutline, 0.4);
					}
				}
			}
		}

		protected void DrawSelectedObject(Graphics graphics, Widget obj)
		{
			Rectangle bounds = this.objectModifier.GetActualBounds(obj);
			bounds.Deflate(0.5);

#if false
			if (this.objectModifier.HasBounds(obj))
			{
				Rectangle pref = this.objectModifier.GetBounds(obj);
				pref.Deflate(0.5);

				if (pref != bounds)
				{
					Path path = new Path();
					path.AppendRectangle(pref);
					Misc.DrawPathDash(graphics, path, 1, 8, 3, PanelsContext.ColorHiliteOutline);
				}
			}
#endif

			graphics.LineWidth = 3;
			graphics.AddRectangle(bounds);
			graphics.RenderSolid(PanelsContext.ColorHiliteOutline);
			graphics.LineWidth = 1;

			if (this.objectModifier.HasMargins(obj))
			{
				Rectangle ext = bounds;
				ext.Inflate(this.objectModifier.GetMargins(obj));
				ext.Deflate(0.5);

				Path path = new Path();
				path.AppendRectangle(ext);
				Misc.DrawPathDash(graphics, path, 2, 0, 4, false, PanelsContext.ColorHiliteOutline);
			}

			if (this.objectModifier.AreChildrenGrid(obj))
			{
				this.DrawGrid(graphics, obj, PanelsContext.ColorHiliteOutline, 1.0);
			}

			GridSelection gs = GridSelection.Get(obj);
			if (gs != null)
			{
				this.DrawGridSelected(graphics, obj, gs, PanelsContext.ColorGridCellOutline, true);
			}
		}

		protected void DrawPadding(Graphics graphics, Widget obj, double factor)
		{
			//	Dessine les marges de padding d'un objet, sous forme de hachures.
			if (ObjectModifier.IsAbstractGroup(obj))
			{
				Rectangle bounds = graphics.Align(this.objectModifier.GetActualBounds(obj));
				bounds.Deflate(0.5);

				Rectangle inside = graphics.Align(this.objectModifier.GetFinalPadding(obj));
				inside.Deflate(0.5);

				Rectangle left   = new Rectangle(bounds.Left, bounds.Bottom, inside.Left-bounds.Left, bounds.Height);
				Rectangle right  = new Rectangle(inside.Right, bounds.Bottom, bounds.Right-inside.Right, bounds.Height);
				Rectangle bottom = new Rectangle(inside.Left, bounds.Bottom, inside.Width, inside.Bottom-bounds.Bottom);
				Rectangle top    = new Rectangle(inside.Left, inside.Top, inside.Width, bounds.Top-inside.Top);

				graphics.Rasterizer.AddOutline(Misc.GetHatchPath(left,   6, bounds.BottomLeft));
				graphics.Rasterizer.AddOutline(Misc.GetHatchPath(right,  6, bounds.BottomLeft));
				graphics.Rasterizer.AddOutline(Misc.GetHatchPath(bottom, 6, bounds.BottomLeft));
				graphics.Rasterizer.AddOutline(Misc.GetHatchPath(top,    6, bounds.BottomLeft));

				graphics.AddRectangle(bounds);
				graphics.AddRectangle(inside);

				graphics.RenderSolid(Misc.FactorColor(PanelsContext.ColorHiliteOutline, factor));
			}
		}

		protected void DrawEnteredObjects(Graphics graphics, List<Widget> list)
		{
			//	Dessine l'objet survolé depuis la barre de statut.
			Color red = Color.FromAlphaRgb(0.7, 255.0/255.0, 80.0/255.0, 60.0/255.0);

			foreach (Widget obj in list)
			{
				Rectangle rect = this.objectModifier.GetActualBounds(obj);
				rect.Deflate(1.5);

				graphics.LineWidth = 3;
				graphics.AddRectangle(rect);
				graphics.RenderSolid(red);
				graphics.LineWidth = 1;
			}
		}

		protected void DrawHilitedObject(Graphics graphics, Widget obj, GridSelection gs)
		{
			//	Met en évidence l'objet survolé par la souris.
			if (this.context.ShowAttachment)
			{
				this.DrawAttachment(graphics, obj, PanelsContext.ColorHiliteOutline);
			}

			Color color = PanelsContext.ColorHiliteSurface;

			if (ObjectModifier.IsAbstractGroup(obj))
			{
				this.DrawPadding(graphics, obj, 1.0);

				Rectangle outline = this.objectModifier.GetActualBounds(obj);
				outline.Deflate(this.context.GroupOutline/2);
				graphics.LineWidth = this.context.GroupOutline;
				graphics.AddRectangle(outline);
				graphics.RenderSolid(PanelsContext.ColorHiliteOutline);
				graphics.LineWidth = 1;

				color = Misc.FactorColor(color, 0.25);
			}

			//	Si le rectangle est trop petit (par exemple objet Separator), il est engraissé.
			Rectangle rect = this.objectModifier.GetActualBounds(obj);

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
			graphics.RenderSolid(color);

			if (this.objectModifier.AreChildrenGrid(obj))
			{
				this.DrawGrid(graphics, obj, PanelsContext.ColorHiliteOutline, 1.0);
			}

			if (gs != null)
			{
				this.DrawGridSelected(graphics, obj, gs, PanelsContext.ColorGridCellOutline, false);
			}
		}

		protected void DrawHilitedParent(Graphics graphics, Widget obj, int column, int row, int columnCount, int rowCount)
		{
			//	Met en évidence l'objet parent survolé par la souris.
			if (this.context.ShowAttachment && obj != this.panel)
			{
				this.DrawAttachment(graphics, obj, PanelsContext.ColorHiliteParent);
			}

			double thickness = 2.0;

			Rectangle rect = this.objectModifier.GetActualBounds(obj);
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(PanelsContext.ColorHiliteParent);
			rect.Deflate(thickness/2+0.5);

			Path path = new Path();
			path.AppendRectangle(rect);
			Misc.DrawPathDash(graphics, path, thickness, thickness*2-1, thickness*2+1, false, PanelsContext.ColorHiliteParent);

			rect.Deflate(thickness/2+0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(PanelsContext.ColorHiliteParent);

			if (this.objectModifier.AreChildrenGrid(obj))
			{
				this.DrawGrid(graphics, obj, PanelsContext.ColorHiliteOutline, 1.0);
			}

			if (column != GridSelection.Invalid && row != GridSelection.Invalid)
			{
				Rectangle area = this.objectModifier.GetGridCellArea(obj, column, row, columnCount, rowCount);
				this.DrawGridHilited(graphics, area, PanelsContext.ColorGridCellOutline);
			}
		}

		protected void DrawGrid(Graphics graphics, Widget obj, Color color, double factor)
		{
			if (!this.objectModifier.AreChildrenGrid(obj))  return;

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			if (engine == null)  return;

			Rectangle rect = this.objectModifier.GetFinalPadding(obj);
			rect = graphics.Align (rect);

			int columns = engine.ColumnDefinitions.Count;
			double x = rect.Left;
			for (int c=0; c<columns; c++)
			{
				ColumnDefinition def = engine.ColumnDefinitions[c];
				x += def.LeftBorder + def.ActualWidth + def.RightBorder;
				Point p1 = new Point(x, rect.Bottom+1);
				Point p2 = new Point(x, rect.Top-1);
				Misc.AlignForLine(graphics, ref p1);
				Misc.AlignForLine(graphics, ref p2);
				graphics.AddLine(p1, p2);
			}

			int rows = engine.RowDefinitions.Count;
			double y = rect.Top;
			for (int r=0; r<rows; r++)
			{
				RowDefinition def = engine.RowDefinitions[r];
				y -= def.TopBorder + def.ActualHeight + def.BottomBorder;
				Point p1 = new Point(rect.Left+1, y);
				Point p2 = new Point(rect.Right-1, y);
				Misc.AlignForLine(graphics, ref p1);
				Misc.AlignForLine(graphics, ref p2);
				graphics.AddLine(p1, p2);
			}

			if (x < rect.Right)  // zone inutilisée à droite ?
			{
				Rectangle part = new Rectangle(x, rect.Bottom, rect.Right-x, rect.Height);
				graphics.Rasterizer.AddOutline(Misc.GetHatchPath(part, 6, rect.BottomLeft));
			}

			if (y > rect.Bottom)  // zone inutilisée en bas ?
			{
				Rectangle part = new Rectangle(rect.Left, rect.Bottom, x-rect.Left, y-rect.Bottom);
				graphics.Rasterizer.AddOutline(Misc.GetHatchPath(part, 6, rect.BottomLeft));
			}

			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(Misc.FactorColor(color, factor));
		}

		protected void DrawGridSelected(Graphics graphics, Widget obj, GridSelection gs, Color color, bool selection)
		{
			//	Dessine une ou plusieurs cellules sélectionnées. Le dessin est optimisé
			//	visuellement lorsqu'une ligne sélectionnée suit une colonne sélectionnée
			//	pour former une croix, ou lorsque plusieurs lignes/colonnes sont sélectionnées
			//	pour former un seul 'bloc'.
			int i=0;
			while (i < gs.Count)
			{
				GridSelection.OneItem item1 = gs[i];

				if (i+1 < gs.Count)
				{
					GridSelection.OneItem item2 = gs[i+1];

					//	Colonne suivie d'une ligne formant une croix ?
					if (item1.Unit == GridSelection.Unit.Column && item2.Unit == GridSelection.Unit.Row)
					{
						Rectangle area1 = this.objectModifier.GetGridItemArea(obj, item1);
						Rectangle area2 = this.objectModifier.GetGridItemArea(obj, item2);
						this.DrawGridSelected(graphics, area1, area2, color, selection);

						i += 2;
						continue;
					}

					//	Plusieurs colonnes ou lignes formant un seul 'bloc' ?
					if (item1.Unit == item2.Unit && item1.Index+1 == item2.Index)
					{
						int ii = i;
						i += 2;
						while (i < gs.Count && gs[i].Unit == item1.Unit && gs[i-1].Index+1 == gs[i].Index)
						{
							i++;
						}

						for (int j=ii; j<i; j++)
						{
							Rectangle area1 = this.objectModifier.GetGridItemArea(obj, gs[j]);

							if (item1.Unit == GridSelection.Unit.Column)
							{
								if (j > ii)
								{
									area1.Left -= 1;
								}
								if (j < i-1)
								{
									area1.Right += 2;
								}
							}
							else
							{
								if (j > ii)
								{
									area1.Top += 2;
								}
								if (j < i-1)
								{
									area1.Bottom -= 1;
								}
							}

							this.DrawGridSelected(graphics, area1, color, selection);
						}

						for (int j=ii; j<i; j++)
						{
							Rectangle area1 = this.objectModifier.GetGridItemArea(obj, gs[j]);

							if (item1.Unit == GridSelection.Unit.Column)
							{
								if (j > ii)
								{
									graphics.AddLine(area1.Left+0.5, area1.Bottom+1.5, area1.Left+0.5, area1.Top-1.5);
								}
							}
							else
							{
								if (j > ii)
								{
									graphics.AddLine(area1.Left+1.5, area1.Top+0.5, area1.Right-1.5, area1.Top+0.5);
								}
							}

							graphics.RenderSolid(Color.FromBrightness(1));
						}

						continue;
					}
				}

				Rectangle area = this.objectModifier.GetGridItemArea(obj, item1);
				this.DrawGridSelected(graphics, area, color, selection);

				i += 1;
			}

		}

		protected void DrawGridSelected(Graphics graphics, Rectangle area1, Rectangle area2, Color color, bool selection)
		{
			//	Dessine une ligne et une colonne sélectionnées.
			area1.Deflate(0.5);
			area2.Deflate(0.5);
			Path path;

			if (selection)  // sélection ?
			{
				graphics.AddFilledRectangle(area1);
				graphics.AddFilledRectangle(area2);
				graphics.RenderSolid(PanelsContext.ColorGridCellSurface);

				path = Misc.GetCrossPath(area1, area2);
				graphics.Rasterizer.AddOutline(path);

				area1.Deflate(2.0);
				area2.Deflate(2.0);
				path = Misc.GetCrossPath(area1, area2);
				graphics.Rasterizer.AddOutline(path);

				graphics.RenderSolid(color);
			}
			else  // hilite ?
			{
				path = Misc.GetCrossPath(area1, area2);
				Misc.DrawPathDash(graphics, path, 1, 8, 3, false, color);
			}
		}

		protected void DrawGridSelected(Graphics graphics, Rectangle area, Color color, bool selection)
		{
			//	Dessine une ligne ou une colonne sélectionnée.
			area.Deflate(0.5);

			if (selection)  // sélection ?
			{
				graphics.AddFilledRectangle(area);
				graphics.RenderSolid(PanelsContext.ColorGridCellSurface);

				graphics.AddRectangle(area);
				area.Deflate(2.0);
				graphics.AddRectangle(area);
				graphics.RenderSolid(color);
			}
			else  // hilite ?
			{
				Path path = new Path();
				path.AppendRectangle(area);
				Misc.DrawPathDash(graphics, path, 1, 8, 3, false, color);
			}
		}

		protected void DrawGridHilited(Graphics graphics, Rectangle area, Color color)
		{
			//	Dessine une cellule survolée.
			graphics.Rasterizer.AddSurface(Misc.GetCornerPath(area));
			graphics.RenderSolid(color);
		}

		protected void DrawAttachment(Graphics graphics, Widget obj, Color color)
		{
			//	Dessine tous les attachements d'un objet.
			if (this.objectModifier.AreChildrenAnchored(obj.Parent))
			{
				Rectangle bounds = this.objectModifier.GetActualBounds(obj.Parent);
				Rectangle rect = this.objectModifier.GetActualBounds(obj);
				Point p1, p2;

				p1 = new Point(bounds.Left, rect.Center.Y);
				p2 = new Point(rect.Left, rect.Center.Y);
				this.DrawSpring(graphics, p1, p2, this.objectModifier.HasAttachmentLeft(obj), color);

				p1 = new Point(rect.Right, rect.Center.Y);
				p2 = new Point(bounds.Right, rect.Center.Y);
				this.DrawSpring(graphics, p1, p2, this.objectModifier.HasAttachmentRight(obj), color);

				p1 = new Point(rect.Center.X, bounds.Bottom);
				p2 = new Point(rect.Center.X, rect.Bottom);
				this.DrawSpring(graphics, p1, p2, this.objectModifier.HasAttachmentBottom(obj), color);

				p1 = new Point(rect.Center.X, rect.Top);
				p2 = new Point(rect.Center.X, bounds.Top);
				this.DrawSpring(graphics, p1, p2, this.objectModifier.HasAttachmentTop(obj), color);
			}

			if (this.objectModifier.AreChildrenStacked(obj.Parent))
			{
				color = PanelsContext.ColorHiliteOutline;
				Rectangle rect = this.objectModifier.GetActualBounds(obj);
				Point p;

				if (this.objectModifier.HasAttachmentLeft(obj))
				{
					p = new Point(rect.Left, rect.Center.Y);
					this.DrawTriangleLeft(graphics, p, color);
				}

				if (this.objectModifier.HasAttachmentRight(obj))
				{
					p = new Point(rect.Right, rect.Center.Y);
					this.DrawTriangleRight(graphics, p, color);
				}

				if (this.objectModifier.HasAttachmentBottom(obj))
				{
					p = new Point(rect.Center.X, rect.Bottom);
					this.DrawTriangleBottom(graphics, p, color);
				}

				if (this.objectModifier.HasAttachmentTop(obj))
				{
					p = new Point(rect.Center.X, rect.Top);
					this.DrawTriangleTop(graphics, p, color);
				}
			}
		}

		protected void DrawSpring(Graphics graphics, Point p1, Point p2, bool rigid, Color color)
		{
			//	Dessine un ressort horizontal ou vertical d'un objet.
			Point p1a = Point.Scale(p1, p2, Editor.attachmentScale);
			Point p2a = Point.Scale(p2, p1, Editor.attachmentScale);

			Misc.AlignForLine(graphics, ref p1);
			Misc.AlignForLine(graphics, ref p2);
			Misc.AlignForLine(graphics, ref p1a);
			Misc.AlignForLine(graphics, ref p2a);

			if (rigid)  // rigide ?
			{
				double dim = Editor.attachmentThickness;
				Rectangle box = this.GetDrawBox(graphics, p1a, p2a, dim);

				graphics.AddFilledRectangle(box);
				graphics.RenderSolid(Color.FromBrightness(1));

				Point delta = (p1.Y == p2.Y) ? new Point(0, 1) : new Point(1, 0);
				graphics.AddLine(p1+delta, p1a+delta);
				graphics.AddLine(p2+delta, p2a+delta);
				graphics.AddLine(p1-delta, p1a-delta);
				graphics.AddLine(p2-delta, p2a-delta);

				graphics.AddRectangle(box);
			}
			else  // élastique (ressort) ?
			{
				graphics.AddLine(p1, p1a);
				graphics.AddLine(p2, p2a);

				double dim = Editor.attachmentThickness;
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

		protected Rectangle GetDrawBox(Graphics graphics, Point p1, Point p2, double thickness)
		{
			//	Donne le rectangle d'une boîte horizontale ou verticale.
			if (p1.Y == p2.Y)  // boîte horizontale ?
			{
				p1.Y -= thickness+1;
				p2.Y += thickness-1;
				p2.X -= 1;
				Misc.AlignForLine(graphics, ref p1);
				Misc.AlignForLine(graphics, ref p2);
				return new Rectangle(p1, p2);
			}
			else if (p1.X == p2.X)  // boîte verticale ?
			{
				p1.X -= thickness+1;
				p2.X += thickness-1;
				p2.Y -= 1;
				Misc.AlignForLine(graphics, ref p1);
				Misc.AlignForLine(graphics, ref p2);
				return new Rectangle(p1, p2);
			}
			else
			{
				throw new System.Exception("This geometry is not implemented.");
			}
		}

		protected void DrawTriangleLeft(Graphics graphics, Point p, Color color)
		{
			Path path = new Path();
			path.MoveTo(p.X-this.context.DockedTriangleLength, p.Y);
			path.LineTo(p.X, p.Y-this.context.DockedTriangleThickness);
			path.LineTo(p.X, p.Y+this.context.DockedTriangleThickness);
			path.Close();

			graphics.PaintSurface(path);
			graphics.RenderSolid(color);
		}

		protected void DrawTriangleRight(Graphics graphics, Point p, Color color)
		{
			Path path = new Path();
			path.MoveTo(p.X+this.context.DockedTriangleLength, p.Y);
			path.LineTo(p.X, p.Y-this.context.DockedTriangleThickness);
			path.LineTo(p.X, p.Y+this.context.DockedTriangleThickness);
			path.Close();

			graphics.PaintSurface(path);
			graphics.RenderSolid(color);
		}

		protected void DrawTriangleBottom(Graphics graphics, Point p, Color color)
		{
			Path path = new Path();
			path.MoveTo(p.X, p.Y-this.context.DockedTriangleLength);
			path.LineTo(p.X-this.context.DockedTriangleThickness, p.Y);
			path.LineTo(p.X+this.context.DockedTriangleThickness, p.Y);
			path.Close();

			graphics.PaintSurface(path);
			graphics.RenderSolid(color);
		}

		protected void DrawTriangleTop(Graphics graphics, Point p, Color color)
		{
			Path path = new Path();
			path.MoveTo(p.X, p.Y+this.context.DockedTriangleLength);
			path.LineTo(p.X-this.context.DockedTriangleThickness, p.Y);
			path.LineTo(p.X+this.context.DockedTriangleThickness, p.Y);
			path.Close();

			graphics.PaintSurface(path);
			graphics.RenderSolid(color);
		}

		protected void DrawZOrder(Graphics graphics, Widget parent)
		{
			//	Dessine les numéros d'ordre d'un groupe.
			if (parent.HasChildren)
			{
				foreach (Widget obj in parent.Children)
				{
					Rectangle rect = this.objectModifier.GetActualBounds(obj);
					Rectangle box = new Rectangle(rect.BottomLeft+new Point(1, 1), new Size(20, 10));

					graphics.AddFilledRectangle(box);
					graphics.RenderSolid(Color.FromBrightness(1));

					string text = this.GetObjectZOrder(obj);
					graphics.AddText(box.Left, box.Bottom, box.Width, box.Height, text, Font.DefaultFont, 9.0, ContentAlignment.MiddleCenter);
					graphics.RenderSolid(PanelsContext.ColorZOrder);

					if (ObjectModifier.IsAbstractGroup(obj))
					{
						this.DrawZOrder(graphics, obj);
					}
				}
			}
		}

		protected void DrawTabIndex(Graphics graphics, Widget parent)
		{
			//	Dessine les numéros pour la touche Tab d'un groupe.
			if (parent.HasChildren)
			{
				foreach (Widget obj in parent.Children)
				{
					Rectangle rect = this.objectModifier.GetActualBounds(obj);
					Rectangle box = new Rectangle(rect.BottomRight+new Point(-20-1, 1), new Size(20, 10));

					graphics.AddFilledRectangle(box);
					graphics.RenderSolid(Color.FromBrightness(1));

					string text = this.GetObjectTabIndex(obj);
					graphics.AddText(box.Left, box.Bottom, box.Width, box.Height, text, Font.DefaultFont, 9.0, ContentAlignment.MiddleCenter);
					graphics.RenderSolid(PanelsContext.ColorTabIndex);

					if (ObjectModifier.IsAbstractGroup(obj))
					{
						this.DrawTabIndex(graphics, obj);
					}
				}
			}
		}
		#endregion


		#region Misc
		public void AdaptAfterToolChanged()
		{
			//	Adaptation après un changement d'outil ou d'objet.
			if (this.context.Tool == "ToolSelect" || this.context.Tool == "ToolGlobal")
			{

				if (this.lastCreatedObject != null)
				{
					this.GridClearSelection();
					this.SelectOneObject(this.lastCreatedObject);
					this.lastCreatedObject = null;
				}
				else if (this.selectedObjects.Count == 1)
				{
					Widget obj = this.selectedObjects[0];
					GridSelection gs = GridSelection.Get(obj);
					if (gs != null)
					{
						List<Widget> list = new List<Widget>();
						foreach (Widget children in obj.Children)
						{
							int column = this.objectModifier.GetGridColumn(children);
							if (gs.Search(GridSelection.Unit.Column, column) != -1)
							{
								list.Add(children);
								continue;
							}

							int row = this.objectModifier.GetGridRow(children);
							if (gs.Search(GridSelection.Unit.Row, row) != -1)
							{
								list.Add(children);
								continue;
							}
						}
						if (list.Count > 0)
						{
							this.selectedObjects = list;
							this.GridClearSelection();
							this.UpdateAfterChanging(Viewers.Changing.Selection);
							this.OnChildrenSelected();
							this.Invalidate();
						}
					}
				}
				else
				{
					this.GridClearSelection();
				}
			}

			if (this.context.Tool == "ToolGrid")
			{
				if (this.selectedObjects.Count != 0)
				{
					Widget parent = this.selectedObjects[0].Parent;
					if (this.objectModifier.AreChildrenGrid(parent))
					{
						int c = GridSelection.Invalid;
						int r = GridSelection.Invalid;
						bool mc = false;
						bool mr = false;
						foreach (Widget children in this.selectedObjects)
						{
							int column = this.objectModifier.GetGridColumn(children);
							if (c == GridSelection.Invalid)
							{
								c = column;
							}
							else
							{
								if (c != column)
								{
									mc = true;
								}
							}

							int row = this.objectModifier.GetGridRow(children);
							if (r == GridSelection.Invalid)
							{
								r = row;
							}
							else
							{
								if (r != row)
								{
									mr = true;
								}
							}
						}

						GridSelection gs = new GridSelection(parent);
						foreach (Widget children in this.selectedObjects)
						{
							if (mr || (!mc && !mr))
							{
								int column = this.objectModifier.GetGridColumn(children);
								if (gs.Search(GridSelection.Unit.Column, column) == -1)
								{
									gs.Add(GridSelection.Unit.Column, column);
								}
							}
							else if (mc)
							{
								int row = this.objectModifier.GetGridRow(children);
								if (gs.Search(GridSelection.Unit.Row, row) == -1)
								{
									gs.Add(GridSelection.Unit.Row, row);
								}
							}
						}
						this.SelectOneObject(parent);
						GridSelection.Attach(parent, gs);
					}
					else
					{
						this.DeselectAll();
					}
				}
			}

			if (this.context.Tool.StartsWith("Object"))
			{
				this.DeselectAll();
			}

			this.SizeMarkDeselect();
			this.Invalidate();
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

		protected void ChangeSeparatorAlpha(DragWindow window)
		{
			//	Modifie la transparence des tous les Separators d'une fenêtre.
			double alpha = this.isInside ? 0 : PanelsContext.ColorOutsideForeground.A;
			this.ChangeSeparatorAlpha(window.Root, alpha);
		}

		protected void ChangeSeparatorAlpha(Widget parent, double alpha)
		{
			if (parent.HasChildren)
			{
				foreach (Widget obj in parent.Children)
				{
					ObjectModifier.ObjectType type = ObjectModifier.GetObjectType(obj);
					if (type == ObjectModifier.ObjectType.HSeparator ||
						type == ObjectModifier.ObjectType.VSeparator)
					{
						Separator sep = obj as Separator;
						sep.Alpha = alpha;
					}

					if (obj.HasChildren)
					{
						this.ChangeSeparatorAlpha(obj, alpha);
					}
				}
			}
		}

		protected void SetDirty()
		{
			this.module.AccessPanels.SetLocalDirty();
		}

		protected Point ConvPanelToEditor(Point pos)
		{
			pos.X += Dimension.margin;
			pos.Y += Dimension.margin;
			return pos;
		}

		protected Point ConvEditorToPanel(Point pos)
		{
			pos.X -= Dimension.margin;
			pos.Y -= Dimension.margin;
			return pos;
		}

		protected Rectangle ConvPanelToEditor(Rectangle rect)
		{
			rect.Offset(Dimension.margin, Dimension.margin);
			return rect;
		}

		protected Rectangle ConvEditorToPanel(Rectangle rect)
		{
			rect.Offset(-Dimension.margin, -Dimension.margin);
			return rect;
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

		void IPaintFilter.NotifyAboutToProcessChildren(Widget sender, PaintEventArgs e)
		{
		}

		void IPaintFilter.NotifyChildrenProcessed(Widget sender, PaintEventArgs e)
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

				case MouseCursorType.Grid:
					this.SetMouseCursorImage(ref this.mouseCursorGrid, Misc.Icon("CursorGrid"));
					break;

				case MouseCursorType.GridPlus:
					this.SetMouseCursorImage(ref this.mouseCursorGridPlus, Misc.Icon("CursorGridPlus"));
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
				image = Support.ImageProvider.Instance.GetImage(name, Support.Resources.DefaultManager);
			}

			this.MouseCursor = MouseCursor.FromImage(image);
		}
		#endregion


		#region Events
		protected virtual void OnChildrenAdded()
		{
			var handler = this.GetUserEventHandler("ChildrenAdded");
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
			var handler = this.GetUserEventHandler("ChildrenSelected");
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

		protected virtual void OnChildrenGeometryChanged()
		{
			var handler = this.GetUserEventHandler("ChildrenGeometryChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event EventHandler ChildrenGeometryChanged
		{
			add
			{
				this.AddUserEventHandler("ChildrenGeometryChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("ChildrenGeometryChanged", value);
			}
		}

		protected virtual void OnUpdateCommands()
		{
			var handler = this.GetUserEventHandler("UpdateCommands");
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

		protected Viewers.Panels			viewersPanels;
		protected Module					module;
		protected UI.Panel					panel;
		protected Druid						druid;
		protected PanelsContext				context;
		protected ObjectModifier			objectModifier;
		protected ConstrainsList			constrainsList;
		protected HandlesList				handlesList;
		protected DimensionsList			dimensionsList;
		protected bool						isEditEnabled = false;

		protected DragWindow				creatingWindow;
		protected Point						creatingOrigin;
		protected Rectangle					creatingRectangle;
		protected Widget					creatingObject;
		protected Widget					lastCreatedObject;
		protected List<Widget>				selectedObjects = new List<Widget>();
		protected Rectangle					selectedRectangle = Rectangle.Empty;
		protected Rectangle					hilitedAttachmentRectangle = Rectangle.Empty;
		protected Rectangle					hilitedZOrderRectangle = Rectangle.Empty;
		protected Widget					hilitedObject;
		protected GridSelection				hilitedGrid;
		protected Widget					hilitedParent;
		protected int						hilitedParentColumn = GridSelection.Invalid;
		protected int						hilitedParentRow = GridSelection.Invalid;
		protected int						hilitedParentColumnCount = 0;
		protected int						hilitedParentRowCount = 0;
		protected bool						isHilitedDimension;
		protected bool						isRectangling;  // j'invente des mots si je veux !
		protected bool						isDragging;
		protected DragWindow				draggingWindow;
		protected Point						draggingOffset;
		protected Point						draggingOrigin;
		protected Rectangle					draggingRectangle;
		protected double					draggingBaseLine;
		protected Widget[]					draggingArraySelected;
		protected int						draggingSpanColumnOffset;
		protected int						draggingSpanRowOffset;
		protected int						draggingSpanColumnCount;
		protected int						draggingSpanRowCount;
		protected bool						isDraggingDimension;
		protected bool						isHandling;
		protected Handle.Type				handlingType;
		protected DragWindow				handlingWindow;
		protected Rectangle					handlingRectangle;
		protected bool						isGridding;
		protected bool						isGriddingColumn;
		protected bool						isGriddingRow;
		protected GridSelection				griddingInitial;
		protected int						griddingColumn;
		protected int						griddingRow;
		protected Point						startingPos;
		protected MouseCursorType			lastCursor = MouseCursorType.Unknown;
		protected Size						sizeMark;
		protected bool						isSizeMarkDragging;
		protected bool						isSizeMarkHorizontal;
		protected bool						isSizeMarkVertical;
		protected Point						sizeMarkOffset;
		protected bool						isInside;
		protected List<Widget>				enteredObjects;

		protected Image						mouseCursorArrow = null;
		protected Image						mouseCursorArrowPlus = null;
		protected Image						mouseCursorGlobal = null;
		protected Image						mouseCursorGrid = null;
		protected Image						mouseCursorGridPlus = null;
		protected Image						mouseCursorEdit = null;
		protected Image						mouseCursorPen = null;
		protected Image						mouseCursorZoom = null;
		protected Image						mouseCursorHand = null;
		protected Image						mouseCursorFinger = null;
	}
}
