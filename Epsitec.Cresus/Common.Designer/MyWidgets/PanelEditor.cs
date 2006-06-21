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


		public void Initialise(Module module, PanelsContext context, UI.Panel panel)
		{
			this.module = module;
			this.context = context;
			this.panel = panel;
			this.sizeMark = this.panel.PreferredSize;

			this.objectModifier = new ObjectModifier(this);
			this.constrainsList = new ConstrainsList(this);
			this.handlesList = new HandlesList(this);
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
			//	PanelEditor est frère de Panel et vient par-dessus.
			get
			{
				return this.panel;
			}
			set
			{
				if (this.panel != value)
				{
					this.selectedObjects.Clear ();
					this.UpdateAfterSelectionChanged ();
					
					this.panel = value;
					this.sizeMark = this.panel.PreferredSize;
					
					//	TODO: @PA invalider ce qui doit l'être... vider les listes des contraintes, poignées, etc.
				}
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
			//	Est-ce que l'édition est possible ? Pour cela, il faut avoir sélectionner un bundle
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
			//	Donne le texte pour les status.
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
				this.GetSelectionInfo(out objSelected, out objCount);
				string text = string.Format(Res.Strings.Viewers.Panels.Info, objSelected.ToString(), objCount.ToString(), sel);

				return text;
			}
		}


		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			if (!this.isEditEnabled)  return;

			switch (message.Type)
			{
				case MessageType.MouseDown:
					this.module.Modifier.IsDirty = true;
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
				if (obj != null && this.selectedObjects.Contains(obj) && obj != this.panel)
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
				if (obj != null && this.selectedObjects.Contains(obj) && obj != this.panel)
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

			this.isInside = true;
			Widget parent = this.DetectGroup(pos);

			this.creatingObject = this.CreateObjectItem();
			this.CreateObjectAdjust(ref pos, parent, out this.creatingRectangle);

			this.creatingOrigin = this.MapClientToScreen(Point.Zero);
			this.creatingWindow = new DragWindow();
			this.creatingWindow.DefineWidget(this.creatingObject, this.creatingObject.PreferredSize, Drawing.Margins.Zero);
			this.creatingWindow.WindowLocation = this.creatingOrigin + pos;
			this.creatingWindow.Owner = this.Window;
			this.creatingWindow.FocusedWidget = this.creatingObject;
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

			this.SetHilitedParent(parent);  // met en évidence le futur parent survolé par la souris

			this.module.MainWindow.UpdateInfoViewer();
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

				this.CreateObjectAdjust(ref pos, parent, out this.creatingRectangle);

				if (this.objectModifier.AreChildrenAnchored(parent))
				{
					Rectangle rect = this.isInside ? this.creatingRectangle : Rectangle.Empty;
					this.constrainsList.Activate(rect, this.GetObjectBaseLine(this.creatingObject), null);

					this.SetHilitedZOrderRectangle(Rectangle.Empty);
				}
				else if (this.objectModifier.AreChildrenDocked(parent))
				{
					this.constrainsList.Activate(Rectangle.Empty, 0, null);

					Widget group;
					int order;
					ObjectModifier.DockedHorizontalAttachment ha;
					ObjectModifier.DockedVerticalAttachment va;
					Rectangle hilite;
					this.ZOrderDetect(initialPos, parent, out group, out order, out ha, out va, out hilite);
					this.SetHilitedZOrderRectangle(hilite);
				}
				else
				{
					this.constrainsList.Activate(Rectangle.Empty, 0, null);
					this.SetHilitedZOrderRectangle(Rectangle.Empty);
				}

				this.creatingWindow.WindowLocation = this.creatingOrigin + pos;
				this.creatingWindow.SuperLight = !this.isInside;
				this.ChangeSeparatorAlpha(this.creatingWindow);

				this.SetHilitedParent(parent);  // met en évidence le futur parent survolé par la souris

				this.module.MainWindow.UpdateInfoViewer();
			}
		}

		protected void CreateObjectUp(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Dessin d'un objet, souris relâchée.
			if (this.creatingObject != null)
			{
				this.isInside = this.IsInside(pos);
				Widget parent = this.DetectGroup(pos);

				if (this.isInside)
				{
					this.creatingWindow.Hide();
					this.creatingWindow.Dispose();
					this.creatingWindow = null;

					Point initialPos = pos;
					this.CreateObjectAdjust(ref pos, parent, out this.creatingRectangle);

					if (this.objectModifier.AreChildrenAnchored(parent))
					{
						this.creatingObject = this.CreateObjectItem();
						this.creatingObject.SetParent(parent);
						this.creatingObject.TabNavigation = TabNavigationMode.Passive;
						this.ObjectAdaptFromParent(this.creatingObject, parent, ObjectModifier.DockedHorizontalAttachment.None, ObjectModifier.DockedVerticalAttachment.None);
						this.SetObjectPosition(this.creatingObject, pos);
					}

					if (this.objectModifier.AreChildrenDocked(parent))
					{
						Widget group;
						int order;
						ObjectModifier.DockedHorizontalAttachment ha;
						ObjectModifier.DockedVerticalAttachment va;
						Rectangle hilite;
						this.ZOrderDetect(initialPos, parent, out group, out order, out ha, out va, out hilite);

						this.creatingObject = this.CreateObjectItem();
						this.creatingObject.SetParent(group);
						this.creatingObject.ZOrder = order;
						this.creatingObject.TabNavigation = TabNavigationMode.Passive;
						this.ObjectAdaptFromParent(this.creatingObject, parent, ha, va);
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
				this.module.MainWindow.UpdateInfoViewer();
				this.OnUpdateCommands();

				this.ChangeTextResource(this.lastCreatedObject);
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
				GroupBox group = new GroupBox();
				group.ChildrenLayoutMode = Widgets.Layouts.LayoutMode.Anchored;
				//?group.ChildrenLayoutMode = Widgets.Layouts.LayoutMode.Docked;
				group.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
				//?group.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;
				group.Text = Misc.Italic("GroupBox");
				group.PreferredSize = new Size(200, 100);
				group.MinWidth = 50;
				group.MinHeight = 50;

				item = group;
			}

			return item;
		}

		protected void ObjectAdaptFromParent(Widget obj, Widget parent, ObjectModifier.DockedHorizontalAttachment ha, ObjectModifier.DockedVerticalAttachment va)
		{
			//	Adapte un objet d'après son parent.
			if (this.objectModifier.AreChildrenAnchored(parent))
			{
				if (this.objectModifier.GetAnchoredVerticalAttachment(obj) == ObjectModifier.AnchoredVerticalAttachment.None)
				{
					this.objectModifier.SetAnchoredHorizontalAttachment(obj, ObjectModifier.AnchoredHorizontalAttachment.Left);
					this.objectModifier.SetAnchoredVerticalAttachment(obj, ObjectModifier.AnchoredVerticalAttachment.Bottom);
				}
			}

			if (this.objectModifier.AreChildrenDocked(parent))
			{
				if (this.objectModifier.GetDockedHorizontalAttachment(obj) == ObjectModifier.DockedHorizontalAttachment.None||
					this.objectModifier.GetDockedVerticalAttachment(obj) == ObjectModifier.DockedVerticalAttachment.None)
				{
					this.objectModifier.SetMargins(obj, new Margins(5, 5, 5, 5));

					if (this.objectModifier.AreChildrenHorizontal(parent))
					{
						this.objectModifier.SetDockedHorizontalAttachment(obj, ha);
					}
					else
					{
						this.objectModifier.SetDockedVerticalAttachment(obj, va);
					}
				}
			}

			if (obj is StaticText)
			{
				obj.PreferredHeight = obj.MinHeight;
			}

			if (obj is Button)
			{
				obj.PreferredHeight = obj.MinHeight;
			}

			if (obj is TextField)
			{
				obj.PreferredHeight = obj.MinHeight;
			}
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


		protected void ChangeTextResource(Widget obj)
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
				this.handlesList.DraggingStart(pos, this.handlingRectangle, this.selectedObjects[0].RealMinSize, this.handlingType);

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
				this.handlingRectangle = this.handlesList.DraggingMove(pos);
				this.handlingWindow.WindowBounds = this.MapClientToScreen(this.handlingRectangle);
				this.module.MainWindow.UpdateInfoViewer();
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
				this.OnChildrenGeometryChanged();
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
			this.isInside = true;
			Widget parent = this.DetectGroup(pos);

			if (this.objectModifier.AreChildrenAnchored(parent))
			{
				this.constrainsList.Starting(this.draggingRectangle, false);
			}

			if (this.objectModifier.AreChildrenDocked(parent))
			{
				this.constrainsList.Starting(Rectangle.Empty, false);
			}

			this.SetHilitedParent(parent);  // met en évidence le futur parent survolé par la souris

			Widget container = new Widget();
			container.PreferredSize = this.draggingRectangle.Size;

			foreach (Widget obj in this.selectedObjects)
			{
				Point origin = this.GetObjectBounds(obj).BottomLeft - this.draggingRectangle.BottomLeft;
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
			this.isInside = this.IsInside(pos);
			Widget parent = this.DetectGroup(pos);
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
			else if (this.objectModifier.AreChildrenDocked(parent))
			{
				this.constrainsList.Activate(Rectangle.Empty, 0, null);

				Widget group;
				int order;
				ObjectModifier.DockedHorizontalAttachment ha;
				ObjectModifier.DockedVerticalAttachment va;
				Rectangle hilite;
				this.ZOrderDetect(pos, parent, out group, out order, out ha, out va, out hilite);
				this.SetHilitedZOrderRectangle(hilite);
			}
			else
			{
				this.constrainsList.Activate(Rectangle.Empty, 0, null);
				this.SetHilitedZOrderRectangle(Rectangle.Empty);
			}

			this.draggingWindow.WindowLocation = this.draggingOrigin + pos + adjust;
			this.draggingWindow.SuperLight = !this.isInside;

			this.ChangeSeparatorAlpha(this.draggingWindow);

			this.SetHilitedParent(parent);  // met en évidence le futur parent survolé par la souris
			this.module.MainWindow.UpdateInfoViewer();
		}

		protected void DraggingEnd(Point pos)
		{
			//	Fin du drag pour déplacer les objets sélectionnés.
			this.isInside = this.IsInside(pos);
			Widget parent = this.DetectGroup(pos);

			if (this.isInside)
			{
				this.draggingWindow.Hide();
				this.draggingWindow.Dispose();
				this.draggingWindow = null;

				if (this.objectModifier.AreChildrenAnchored(parent))
				{
					Rectangle initial = this.SelectBounds;
					this.MoveSelection(this.draggingRectangle.BottomLeft - initial.BottomLeft, parent);
					this.OnChildrenGeometryChanged();
				}

				if (this.objectModifier.AreChildrenDocked(parent))
				{
					Widget group;
					int order;
					ObjectModifier.DockedHorizontalAttachment ha;
					ObjectModifier.DockedVerticalAttachment va;
					Rectangle hilite;
					this.ZOrderDetect(pos, parent, out group, out order, out ha, out va, out hilite);
					this.ZOrderChangeSelection(group, order, ha, va);
					this.OnChildrenGeometryChanged();
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
			Widget detected = this.Detect(pos, this.panel);
			if (detected == null)
			{
				Rectangle rect = this.panel.Client.Bounds;
				if (rect.Contains(pos))
				{
					rect.Deflate(this.context.GroupOutline);
					if (!rect.Contains(pos))
					{
						detected = this.panel;
					}
				}
			}
			return detected;
		}

		protected Widget Detect(Point pos, Widget parent)
		{
			Widget[] children = parent.Children.Widgets;
			for (int i=children.Length-1; i>=0; i--)
			{
				Widget widget = children[i];

				if (widget.Visibility == false)
				{
					continue;
				}

				Rectangle rect = widget.ActualBounds;
				if (rect.Contains(pos))
				{
					Widget deep = this.Detect(widget.MapParentToClient(pos), widget);
					if (deep != null)
					{
						return deep;
					}

					if (widget is AbstractGroup)
					{
						rect.Deflate(this.context.GroupOutline);
						if (rect.Contains(pos))
						{
							continue;
						}
					}

					if (widget.IsEmbedded)
					{
						continue;
					}

					return widget;
				}
			}

			return null;
		}

		protected Widget DetectGroup(Point pos)
		{
			//	Détecte le groupe visé par la souris.
			if (this.IsInside(pos))
			{
				Widget container = this.panel.FindChild(pos, this.selectedObjects, ChildFindMode.Deep | ChildFindMode.SkipHidden | ChildFindMode.SkipNonContainer | ChildFindMode.SkipEmbedded);
				return container ?? this.panel;
			}
			else
			{
				return null;
			}
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
				if (sel.Contains(this.objectModifier.GetBounds(obj)))
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
			this.module.Modifier.IsDirty = true;
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
			this.module.Modifier.IsDirty = true;
		}

		protected void MoveSelection(Point move, Widget parent)
		{
			//	Déplace et change de parent pour tous les objets sélectionnés.
			foreach (Widget obj in this.selectedObjects)
			{
				Rectangle bounds = this.objectModifier.GetBounds(obj);
				bounds.Offset(move);

				if (parent != null)
				{
					if (obj.Parent != parent)
					{
						obj.Parent.Children.Remove(obj);
						parent.Children.Add(obj);
					}

					this.ObjectAdaptFromParent(obj, parent, ObjectModifier.DockedHorizontalAttachment.None, ObjectModifier.DockedVerticalAttachment.None);
				}

				bounds.Size = this.GetObjectBounds(obj).Size;
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
						this.SetObjectPositionY(obj, bounds.Top-this.GetObjectBounds(obj).Height);
					}
				}
				else  // centré verticalement ?
				{
					foreach (Widget obj in this.selectedObjects)
					{
						this.SetObjectPositionY(obj, System.Math.Floor(bounds.Center.Y-this.GetObjectBounds(obj).Height/2));
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
						this.SetObjectPositionX(obj, bounds.Right-this.GetObjectBounds(obj).Width);
					}
				}
				else  // centré horizontalement ?
				{
					foreach (Widget obj in this.selectedObjects)
					{
						this.SetObjectPositionX(obj, System.Math.Floor(bounds.Center.X-this.GetObjectBounds(obj).Width/2));
					}
				}
			}

			this.Invalidate();
			this.module.Modifier.IsDirty = true;
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
			this.module.Modifier.IsDirty = true;
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
					Rectangle rect = this.GetObjectBounds(obj);
					rect.Bottom = bounds.Bottom;
					rect.Height = bounds.Height;
					this.SetObjectBounds(obj, rect);
				}
			}
			else
			{
				foreach (Widget obj in this.selectedObjects)
				{
					Rectangle rect = this.GetObjectBounds(obj);
					rect.Left  = bounds.Left;
					rect.Width = bounds.Width;
					this.SetObjectBounds(obj, rect);
				}
			}

			this.Invalidate();
			this.module.Modifier.IsDirty = true;
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
			this.module.Modifier.IsDirty = true;
		}

		protected void SelectTabIndexRenum()
		{
			//	Renumérote toutes les touches Tab.
			this.SelectTabIndexRenum(this.panel);

			this.Invalidate();
			this.context.ShowTabIndex = true;
			this.OnUpdateCommands();
			this.module.Modifier.IsDirty = true;
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
			this.module.Modifier.IsDirty = true;
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
					bounds = Rectangle.Union(bounds, this.objectModifier.GetBounds(obj));
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
				this.OnChildrenGeometryChanged();
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

		public Rectangle GetObjectBounds(Widget obj)
		{
			return this.objectModifier.GetBounds(obj);
		}

		public void SetObjectBounds(Widget obj, Rectangle bounds)
		{
			if (this.objectModifier.AreChildrenAnchored(obj.Parent))
			{
				this.objectModifier.SetBounds(obj, bounds);
			}

			if (this.objectModifier.AreChildrenDocked(obj.Parent))
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
		}

		public bool IsObjectWidthChanging(Widget obj)
		{
			//	Indique si la largeur d'un objet peut changer.
			if (this.objectModifier.AreChildrenDocked(obj.Parent))
			{
				if (this.objectModifier.HasAttachmentBottom(obj) || this.objectModifier.HasAttachmentTop(obj))
				{
					return (this.objectModifier.GetDockedHorizontalAlignment(obj) != ObjectModifier.DockedHorizontalAlignment.Stretch);
				}
			}
			return true;
		}

		public bool IsObjectHeightChanging(Widget obj)
		{
			//	Indique si la hauteur d'un objet peut changer.
			if (this.objectModifier.AreChildrenDocked(obj.Parent))
			{
				if (this.objectModifier.HasAttachmentLeft(obj) || this.objectModifier.HasAttachmentRight(obj))
				{
					return (this.objectModifier.GetDockedVerticalAlignment(obj) != ObjectModifier.DockedVerticalAlignment.Stretch);
				}
			}
			return true;
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
			Rectangle bounds = this.objectModifier.GetBounds(obj.Parent);
			Rectangle rect = this.objectModifier.GetBounds(obj);
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
		protected void ZOrderDetect(Point mouse, Widget parent, out Widget group, out int order, out ObjectModifier.DockedHorizontalAttachment ha, out ObjectModifier.DockedVerticalAttachment va, out Rectangle hilite)
		{
			//	Détecte le ZOrder à utiliser pour une position donnée, ainsi que le rectangle
			//	à utiliser pour la mise ne évidence.
			if (parent == null)
			{
				group = null;
				order = -1;
				ha = ObjectModifier.DockedHorizontalAttachment.None;
				va = ObjectModifier.DockedVerticalAttachment.None;
				hilite = Rectangle.Empty;
				return;
			}

			group = parent;
			order = 0;
			ha = ObjectModifier.DockedHorizontalAttachment.None;
			va = ObjectModifier.DockedVerticalAttachment.None;
			hilite = Rectangle.Empty;

			double t = this.context.ZOrderThickness;

			Widget obj = this.ZOrderDetectNearest(mouse, parent);  // objet le plus proche
			if (obj == null)
			{
				Rectangle bounds = this.objectModifier.GetBounds(parent);

				if (this.objectModifier.AreChildrenHorizontal(parent))
				{
					if (mouse.X < bounds.Center.X)
					{
						hilite = new Rectangle(bounds.Left, bounds.Bottom, t*2, bounds.Height);
						ha = ObjectModifier.DockedHorizontalAttachment.Left;
					}
					else
					{
						hilite = new Rectangle(bounds.Right-t*2, bounds.Bottom, t*2, bounds.Height);
						ha = ObjectModifier.DockedHorizontalAttachment.Right;
					}
				}
				else
				{
					if (mouse.Y < bounds.Center.Y)
					{
						hilite = new Rectangle(bounds.Left, bounds.Bottom, bounds.Width, t*2);
						va = ObjectModifier.DockedVerticalAttachment.Bottom;
					}
					else
					{
						hilite = new Rectangle(bounds.Left, bounds.Top-t*2, bounds.Width, t*2);
						va = ObjectModifier.DockedVerticalAttachment.Top;
					}
				}

				order = parent.Children.Count;
			}
			else
			{
				group = obj.Parent;
				order = obj.ZOrder;

				Rectangle bounds = this.objectModifier.GetBounds(obj);
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

					ha = this.objectModifier.GetDockedHorizontalAttachment(obj);
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

					va = this.objectModifier.GetDockedVerticalAttachment(obj);
				}

				hilite.Inflate(t);
			}

			if (this.selectedObjects.Contains(obj))
			{
				//	Un objet sélectionné n'est jamais pris en compte.
				group = null;
				order = -1;  // aucun changement
				ha = ObjectModifier.DockedHorizontalAttachment.None;
				va = ObjectModifier.DockedVerticalAttachment.None;
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
								if (this.objectModifier.GetDockedHorizontalAttachment(selectedObj) != ha)
								{
									continue;
								}
							}
							else
							{
								if (this.objectModifier.GetDockedVerticalAttachment(selectedObj) != va)
								{
									continue;
								}
							}

							group = null;
							order = -1;  // aucun changement
							ha = ObjectModifier.DockedHorizontalAttachment.None;
							va = ObjectModifier.DockedVerticalAttachment.None;
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
			bool horizontal = (this.objectModifier.GetChildrenPlacement(parent) == ObjectModifier.ChildrenPlacement.HorizontalDocked);
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
				Rectangle box = parent.Client.Bounds;

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
				Rectangle box = parent.Client.Bounds;

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

				Rectangle bounds = this.objectModifier.GetBounds(obj);
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
				this.Invalidate(this.hilitedZOrderRectangle);  // invalide l'ancienne zone
				this.hilitedZOrderRectangle = rect;
				this.Invalidate(this.hilitedZOrderRectangle);  // invalide la nouvelle zone
			}
		}

		protected void ZOrderChangeSelection(Widget parent, int order, ObjectModifier.DockedHorizontalAttachment ha, ObjectModifier.DockedVerticalAttachment va)
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
				this.ObjectAdaptFromParent(obj, parent, ha, va);
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
			if (!this.isEditEnabled)  return;

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
			Rectangle bounds = this.objectModifier.GetBounds(obj);
			bounds.Deflate(0.5);

			//?graphics.AddFilledRectangle(bounds);
			//?graphics.RenderSolid(PanelsContext.ColorHiliteSurface);

			graphics.LineWidth = 3;
			graphics.AddRectangle(bounds);
			graphics.RenderSolid(PanelsContext.ColorHiliteOutline);
			graphics.LineWidth = 1;

			if (this.objectModifier.AreChildrenDocked(obj.Parent))
			{
				Rectangle ext = bounds;
				ext.Inflate(this.objectModifier.GetMargins(obj));
				ext.Deflate(0.5);

				Path path = new Path();
				path.AppendRectangle(ext);
				Misc.DrawPathDash(graphics, path, 2, 0, 4, PanelsContext.ColorHiliteOutline);
			}

			if (obj is AbstractGroup)
			{
				Margins padding = this.objectModifier.GetPadding(obj);
				if (padding != Margins.Zero)
				{
					Path path = new Path();
					path.AppendRectangle(bounds);
					path = Path.Combine(path, Misc.GetHatchPath(bounds, 6, 1), PathOperation.And);

					bounds.Deflate(padding);
					graphics.Align(ref bounds);
					bounds.Inflate(0.5);

					Path inside = new Path();
					inside.AppendRectangle(bounds);
					path = Path.Combine(path, inside, PathOperation.AMinusB);

					graphics.PaintSurface(path);
					graphics.AddRectangle(bounds);
					graphics.RenderSolid(PanelsContext.ColorHiliteOutline);
				}
			}
		}

		protected void DrawHilitedObject(Graphics graphics, Widget obj)
		{
			//	Met en évidence l'objet survolé par la souris.
			if (this.context.ShowAttachment)
			{
				this.DrawAttachment(graphics, obj, PanelsContext.ColorHiliteOutline);
			}

			Color color = PanelsContext.ColorHiliteSurface;

			if (obj is AbstractGroup)
			{
				Rectangle outline = this.objectModifier.GetBounds(obj);
				outline.Deflate(this.context.GroupOutline/2);
				graphics.LineWidth = this.context.GroupOutline;
				graphics.AddRectangle(outline);
				graphics.RenderSolid(PanelsContext.ColorHiliteOutline);
				graphics.LineWidth = 1;

				color.A *= 0.25;
			}

			//	Si le rectangle est trop petit (par exemple objet Separator), il est engraissé.
			Rectangle rect = this.objectModifier.GetBounds(obj);

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
		}

		protected void DrawHilitedParent(Graphics graphics, Widget obj)
		{
			//	Met en évidence l'objet parent survolé par la souris.
			if (this.context.ShowAttachment && obj != this.panel)
			{
				this.DrawAttachment(graphics, obj, PanelsContext.ColorHiliteParent);
			}

			double thickness = 2.0;

			Rectangle rect = this.objectModifier.GetBounds(obj);
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
			if (this.objectModifier.AreChildrenAnchored(obj.Parent))
			{
				Rectangle bounds = this.objectModifier.GetBounds(obj.Parent);
				Rectangle rect = this.objectModifier.GetBounds(obj);
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

			if (this.objectModifier.AreChildrenDocked(obj.Parent))
			{
				Rectangle rect = this.objectModifier.GetBounds(obj);
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
			foreach (Widget obj in parent.Children)
			{
				Rectangle rect = this.objectModifier.GetBounds(obj);
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
				Rectangle rect = this.objectModifier.GetBounds(obj);
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
		#endregion


		#region Misc
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
			foreach (Widget obj in parent.Children)
			{
				if (obj is Separator)
				{
					Separator sep = obj as Separator;
					sep.Alpha = alpha;
				}

				if (obj.Children.Count != 0)
				{
					this.ChangeSeparatorAlpha(obj, alpha);
				}
			}
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

		protected virtual void OnChildrenGeometryChanged()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler("ChildrenGeometryChanged");
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
		protected ObjectModifier			objectModifier;
		protected ConstrainsList			constrainsList;
		protected HandlesList				handlesList;
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
		protected bool						isInside;

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
