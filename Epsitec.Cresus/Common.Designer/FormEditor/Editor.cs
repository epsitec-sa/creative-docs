using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Layouts;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.FormEditor
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
			this.InternalState |= InternalState.Focusable;
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


		public void Initialize(Module module, PanelsContext context, UI.Panel panel)
		{
			this.module = module;
			this.context = context;
			this.panel = panel;
			this.sizeMark = this.panel.PreferredSize;
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
				this.UpdateAfterChanging(PanelEditor.Editor.Changing.Show);
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

				case "PanelOrderUpAll":
					this.SelectOrder(-10000);
					break;

				case "PanelOrderDownAll":
					this.SelectOrder(10000);
					break;

				case "PanelOrderUpOne":
					this.SelectOrder(-1);
					break;

				case "PanelOrderDownOne":
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

		public void GetSelectionInfo(out int selected, out int count, out bool isRoot)
		{
			//	Donne des informations sur la sélection en cours.
			selected = this.selectedObjects.Count;
			count = this.panel.Children.Count;
			isRoot = false;
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
					message.Consumer = this;
					break;

				case MessageType.MouseLeave:
					this.SetEnteredObjects(null);
					this.SetHilitedObject(null, null);
					break;

				case MessageType.KeyDown:
					this.ProcessKeyChanged(message.IsControlPressed, message.IsShiftPressed);
					break;

				case MessageType.KeyUp:
					this.ProcessKeyChanged(message.IsControlPressed, message.IsShiftPressed);
					break;

				case MessageType.KeyPress:
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

			obj = this.Detect(pos, isShiftPressed, false);  // objet visé par la souris

			if (!isShiftPressed)  // touche Shift relâchée ?
			{
				if (obj != null && this.selectedObjects.Contains(obj) && obj != this.panel)
				{
					this.DraggingStart(pos);
					return;
				}
				this.selectedObjects.Clear();
				this.UpdateAfterChanging(PanelEditor.Editor.Changing.Selection);
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
				this.UpdateAfterChanging(PanelEditor.Editor.Changing.Selection);

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
			if (this.isSizeMarkHorizontal || this.isSizeMarkVertical || this.isHilitedDimension || this.isDraggingDimension)
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
			else if (this.SizeMarkDraggingMove(pos))
			{
			}
			else
			{
				//	Met en évidence la cote survolée par la souris.
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

			if (this.isDraggingDimension)
			{
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
				this.UpdateAfterChanging(PanelEditor.Editor.Changing.Selection);
			}

			this.isRectangling = true;
			this.SetHilitedAttachmentRectangle(Rectangle.Empty);

			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void GlobalMove(Point pos, bool isRightButton, bool isControlPressed, bool isShiftPressed)
		{
			//	Sélection rectangulaire, souris déplacée.
			if (this.isSizeMarkHorizontal || this.isSizeMarkVertical)
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
			this.creatingWindow.FocusedWidget = this.creatingObject;
			this.creatingWindow.Show();

			Separator sep = new Separator();
			sep.Anchor = AnchorStyles.All;
			sep.Color = PanelsContext.ColorOutsideForeground;
			sep.Alpha = 0;
			sep.SetParent(this.creatingWindow.Root);  // parent en dernier pour éviter les flashs !

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
				int column = PanelEditor.GridSelection.Invalid;
				int row = PanelEditor.GridSelection.Invalid;

				this.CreateObjectAdjust(ref pos, parent, out this.creatingRectangle);

				this.creatingWindow.WindowLocation = this.creatingOrigin + pos;
				this.creatingWindow.SuperLight = !this.isInside;
				this.ChangeSeparatorAlpha(this.creatingWindow);

				if (this.isInside)
				{
					this.SetHilitedParent(parent, column, row, 1, 1);  // met en évidence le futur parent survolé par la souris
				}
				else
				{
					this.SetHilitedParent(null, PanelEditor.GridSelection.Invalid, PanelEditor.GridSelection.Invalid, 0, 0);
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

				if (this.isInside)
				{
					Point initialPos = pos;
					this.CreateObjectAdjust(ref pos, parent, out this.creatingRectangle);
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

				this.SetHilitedParent(null, PanelEditor.GridSelection.Invalid, PanelEditor.GridSelection.Invalid, 0, 0);

				this.lastCreatedObject = this.creatingObject;
				this.creatingObject = null;

				if (!this.ChangeTextResource(this.lastCreatedObject))  // choix d'un Druid...
				{
					this.lastCreatedObject.Parent.Children.Remove(this.lastCreatedObject);
					this.lastCreatedObject.Dispose();
					this.lastCreatedObject = null;

					this.Invalidate();
				}

				this.module.DesignerApplication.UpdateInfoViewer();
				this.UpdateAfterChanging(PanelEditor.Editor.Changing.Create);
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
				button.MinWidth = button.PreferredHeight;  // largeur minimale pour former un carré
				button.MinHeight = button.PreferredHeight;
				button.PreferredWidth = button.PreferredHeight;

				item = button;
			}

			if (this.context.Tool == "ObjectRectButton")
			{
				MetaButton button = new MetaButton();
				button.Text = Misc.Italic("Button");
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
			bounds = Rectangle.Empty;
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
			return true;
		}


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

			int column = PanelEditor.GridSelection.Invalid;
			int row = PanelEditor.GridSelection.Invalid;
			this.draggingSpanColumnOffset = 0;
			this.draggingSpanRowOffset = 0;
			this.draggingSpanColumnCount = 1;
			this.draggingSpanRowCount = 1;

			this.SetHilitedParent(parent, column, row, this.draggingSpanColumnCount, this.draggingSpanRowCount);  // met en évidence le futur parent survolé par la souris

			FrameBox container = new FrameBox();
			container.PreferredSize = this.draggingRectangle.Size;

			foreach (Widget obj in this.selectedObjects)
			{
			}

			this.draggingOrigin = this.MapClientToScreen(this.ConvPanelToEditor(this.draggingOffset));
			//?this.draggingOrigin.Y -= 1;  // TODO: cette correction devrait être inutile !
			this.draggingWindow = new DragWindow();
			this.draggingWindow.DefineWidget(container, container.PreferredSize, Margins.Zero);
			this.draggingWindow.WindowLocation = this.draggingOrigin + pos;
			this.draggingWindow.Owner = this.Window;
			this.draggingWindow.FocusedWidget = container;
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
			int column = PanelEditor.GridSelection.Invalid;
			int row = PanelEditor.GridSelection.Invalid;
			Point adjust = Point.Zero;

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

			if (this.isInside)
			{
				Widget initialParent = this.draggingArraySelected[0].Parent;

				if (initialParent != this.selectedObjects[0].Parent)
				{
					this.UpdateAfterChanging(PanelEditor.Editor.Changing.Move);
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
			}

			this.SetHilitedParent(null, PanelEditor.GridSelection.Invalid, PanelEditor.GridSelection.Invalid, 0, 0);
			this.isDragging = false;
			this.draggingArraySelected = null;
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

			if (column == PanelEditor.GridSelection.Invalid || row == PanelEditor.GridSelection.Invalid)
			{
				return false;
			}

			Widget obj = this.selectedObjects[0];
			column -= this.draggingSpanColumnOffset;
			row -= this.draggingSpanRowOffset;

			return true;
		}
		#endregion


		#region Selection
		protected Widget Detect(Point pos, bool sameParent, bool onlyGrid)
		{
			//	Détecte l'objet visé par la souris, avec priorité au dernier objet
			//	dessiné (donc placé dessus).
			//	Si sameParent = true, l'objet détecté doit avoir le même parent que
			//	l'objet éventuellement déjà sélectionné.
			Widget detected = this.Detect(pos, sameParent, onlyGrid, this.panel);
			if (detected == null && (!sameParent || this.selectedObjects.Count == 0) && !onlyGrid)
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

					if (sameParent && this.selectedObjects.Count > 0)
					{
						if (widget.Parent != this.selectedObjects[0].Parent)
						{
							continue;
						}
					}

					return widget;
				}
			}

			return null;
		}

		protected Margins GetDetectPadding(Widget obj, bool insideGrid)
		{
			//	Retourne les marges intérieures pour la détection du padding.
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
				Widget container = this.panel.FindChild(pos, this.selectedObjects, ChildFindMode.Deep | ChildFindMode.SkipHidden | ChildFindMode.SkipNonContainer | ChildFindMode.SkipEmbedded);
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
				this.UpdateAfterChanging(PanelEditor.Editor.Changing.Selection);
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

			this.UpdateAfterChanging(PanelEditor.Editor.Changing.Selection);
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

			this.UpdateAfterChanging(PanelEditor.Editor.Changing.Selection);
			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectRoot()
		{
			//	Sélectionne le panneau de base.
			this.selectedObjects.Clear();
			this.selectedObjects.Add(this.panel);

			this.UpdateAfterChanging(PanelEditor.Editor.Changing.Selection);
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

			this.UpdateAfterChanging(PanelEditor.Editor.Changing.Selection);
			this.OnChildrenSelected();
			this.Invalidate();
		}

		public void SelectOneObject(Widget obj)
		{
			//	Sélectionne un objet.
			this.selectedObjects.Clear();
			this.selectedObjects.Add(obj);
			this.UpdateAfterChanging(PanelEditor.Editor.Changing.Selection);
			this.OnChildrenSelected();
			this.Invalidate();
		}

		public void SelectListObject(List<Widget> list)
		{
			//	Sélectionne une liste d'objets.
			this.selectedObjects = list;
			this.UpdateAfterChanging(PanelEditor.Editor.Changing.Selection);
			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectObjectsInRectangle(Rectangle sel)
		{
			//	Sélectionne tous les objets entièrement inclus dans un rectangle.
			//	Tous les objets sélectionnés doivent avoir le même parent.
			this.SelectObjectsInRectangle(sel, this.panel);
			this.UpdateAfterChanging(PanelEditor.Editor.Changing.Selection);
			this.OnChildrenSelected();
			this.Invalidate();
		}

		protected void SelectObjectsInRectangle(Rectangle sel, Widget parent)
		{
		}

		public void RegenerateDimensions()
		{
			//	Régénère les cotes s'il y a eu un changement.
		}

		public void UpdateAfterSelectionGridChanged()
		{
			//	Mise à jour après un changement de sélection dans un tableau.
			this.OnChildrenSelected();  // met à jour les panneaux des proxies à droite
		}

		protected void UpdateAfterChanging(PanelEditor.Editor.Changing oper)
		{
			//	Mise à jour après un changement de sélection, ou après un changement dans
			//	l'arbre des objets (création, changement de parenté, etc.).
			this.module.DesignerApplication.UpdateViewer(oper);
			PanelEditor.GeometryCache.Clear(this.panel);
		}

		public void SetEnteredObjects(List<Widget> list)
		{
			//	Détermine l'objet survolé depuis la barre de statut.
			this.enteredObjects = list;
			this.Invalidate();
		}

		protected void SetHilitedObject(Widget obj, PanelEditor.GridSelection grid)
		{
			//	Détermine l'objet à mettre en évidence lors d'un survol.
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
			this.UpdateAfterChanging(PanelEditor.Editor.Changing.Delete);
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
			direction.X *= this.module.DesignerApplication.MoveHorizontal;
			direction.Y *= this.module.DesignerApplication.MoveVertical;
			this.MoveSelection(direction, null);
			this.SetDirty();
		}

		protected void MoveSelection(Point move, Widget parent)
		{
			//	Déplace et change de parent pour tous les objets sélectionnés.
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
			this.UpdateAfterChanging(PanelEditor.Editor.Changing.Move);
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
		}

		public void SetObjectPositionX(Widget obj, double x)
		{
		}

		public void SetObjectPositionY(Widget obj, double y)
		{
		}

		public void SetObjectPosition(Widget obj, Point pos)
		{
		}

		public Rectangle GetObjectPreferredBounds(Widget obj)
		{
			return Rectangle.Empty;
		}

		public void SetObjectPreferredBounds(Widget obj, Rectangle bounds)
		{
		}

		public bool IsObjectWidthChanging(Widget obj)
		{
			//	Indique si la largeur d'un objet peut changer.
			//	Utilisé par HandlesList pour déterminer quelles poignées sont visibles.
			return false;
		}

		public bool IsObjectHeightChanging(Widget obj)
		{
			//	Indique si la hauteur d'un objet peut changer.
			//	Utilisé par HandlesList pour déterminer quelles poignées sont visibles.
			return false;
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
			graphics.TranslateTransform(Editor.margin, Editor.margin);

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
			if (this.selectedObjects.Count > 0 && !this.isDragging)
			{
				this.DrawSelectedObjects(graphics);
			}

			//	Dessine les attachements des objets sélectionnés.
			if (this.context.ShowAttachment && this.selectedObjects.Count == 1 && !this.isDragging)
			{
				foreach (Widget obj in this.selectedObjects)
				{
					this.DrawAttachment(graphics, obj, PanelsContext.ColorAttachment);
				}
			}

			//	Dessine les numéros d'index pour la touche Tab.
			if (this.context.ShowTabIndex && !this.isDragging)
			{
				this.DrawTabIndex(graphics, this.panel);
			}

			//	Dessine l'objet survolé.
			if (this.hilitedObject != null)
			{
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
				}
			}
		}

		protected void DrawSelectedObject(Graphics graphics, Widget obj)
		{
		}

		protected void DrawPadding(Graphics graphics, Widget obj, double factor)
		{
			//	Dessine les marges de padding d'un objet, sous forme de hachures.
		}

		protected void DrawEnteredObjects(Graphics graphics, List<Widget> list)
		{
			//	Dessine l'objet survolé depuis la barre de statut.
		}

		protected void DrawHilitedObject(Graphics graphics, Widget obj, PanelEditor.GridSelection gs)
		{
			//	Met en évidence l'objet survolé par la souris.
		}

		protected void DrawHilitedParent(Graphics graphics, Widget obj, int column, int row, int columnCount, int rowCount)
		{
			//	Met en évidence l'objet parent survolé par la souris.
		}

		protected void DrawGrid(Graphics graphics, Widget obj, Color color, double factor)
		{
		}

		protected void DrawGridSelected(Graphics graphics, Widget obj, PanelEditor.GridSelection gs, Color color, bool selection)
		{
			//	Dessine une ou plusieurs cellules sélectionnées. Le dessin est optimisé
			//	visuellement lorsqu'une ligne sélectionnée suit une colonne sélectionnée
			//	pour former une croix, ou lorsque plusieurs lignes/colonnes sont sélectionnées
			//	pour former un seul 'bloc'.
		}

		protected void DrawGridSelected(Graphics graphics, Rectangle area1, Rectangle area2, Color color, bool selection)
		{
			//	Dessine une ligne et une colonne sélectionnées.
		}

		protected void DrawGridSelected(Graphics graphics, Rectangle area, Color color, bool selection)
		{
			//	Dessine une ligne ou une colonne sélectionnée.
		}

		protected void DrawGridHilited(Graphics graphics, Rectangle area, Color color)
		{
			//	Dessine une cellule survolée.
		}

		protected void DrawAttachment(Graphics graphics, Widget obj, Color color)
		{
			//	Dessine tous les attachements d'un objet.
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

		protected void DrawTabIndex(Graphics graphics, Widget parent)
		{
			//	Dessine les numéros pour la touche Tab d'un groupe.
		}
		#endregion


		#region Misc
		public void AdaptAfterToolChanged()
		{
			//	Adaptation après un changement d'outil ou d'objet.
		}

		public Rectangle RealBounds
		{
			//	Retourne le rectangle englobant de tous les objets contenus dans le panneau.
			get
			{
				return new Rectangle(Point.Zero, this.panel.ActualSize);
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
		}

		protected void SetDirty()
		{
			this.module.AccessPanels.SetLocalDirty();
		}

		protected Point ConvPanelToEditor(Point pos)
		{
			pos.X += Editor.margin;
			pos.Y += Editor.margin;
			return pos;
		}

		protected Point ConvEditorToPanel(Point pos)
		{
			pos.X -= Editor.margin;
			pos.Y -= Editor.margin;
			return pos;
		}

		protected Rectangle ConvPanelToEditor(Rectangle rect)
		{
			rect.Offset(Editor.margin, Editor.margin);
			return rect;
		}

		protected Rectangle ConvEditorToPanel(Rectangle rect)
		{
			rect.Offset(-Editor.margin, -Editor.margin);
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
				image = Support.ImageProvider.Default.GetImage(name, Support.Resources.DefaultManager);
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


		public static readonly double		margin = 10;

		protected static readonly double	attachmentThickness = 3.0;
		protected static readonly double	attachmentScale = 0.4;

		protected Module					module;
		protected UI.Panel					panel;
		protected Druid						druid;
		protected PanelsContext				context;
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
		protected int						hilitedParentColumn = PanelEditor.GridSelection.Invalid;
		protected int						hilitedParentRow = PanelEditor.GridSelection.Invalid;
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
		protected DragWindow				handlingWindow;
		protected Rectangle					handlingRectangle;
		protected bool						isGridding;
		protected bool						isGriddingColumn;
		protected bool						isGriddingRow;
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
