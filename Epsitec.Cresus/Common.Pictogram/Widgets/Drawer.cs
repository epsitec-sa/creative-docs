using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe Drawer permet de représenter vectoriellement des icônes.
	/// </summary>
	public class Drawer : Epsitec.Common.Widgets.Button
	{
		public Drawer()
		{
			this.ButtonStyle = ButtonStyle.ToolItem;

			this.colorBlack   = Drawing.Color.FromName("WindowFrame");
			this.colorWindow  = Drawing.Color.FromName("Control");
			this.colorControl = Drawing.Color.FromName("Control");

			this.iconObjects = new IconObjects();
			this.iconContext = new IconContext();
			this.objectMemory = new ObjectMemory();
			this.objectMemory.CreateProperties();
		}
		
		public Drawer(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		// Retourne la largeur standard d'une icône.
		public override double DefaultWidth
		{
			get
			{
				return 22;
			}
		}

		// Retourne la hauteur standard d'une icône.
		public override double DefaultHeight
		{
			get
			{
				return 22;
			}
		}


		// Indique si l'icône est active.
		public bool IsActive
		{
			get
			{
				return this.isActive;
			}

			set
			{
				this.isActive = value;
			}
		}

		// Indique si l'icône est éditable.
		public bool IsEditable
		{
			get
			{
				return this.isEditable;
			}

			set
			{
				this.isEditable = value;
				this.iconContext.IsEditable = value;
			}
		}

		// Facteur de zoom de la loupe.
		public double Zoom
		{
			get
			{
				return this.iconContext.Zoom;
			}

			set
			{
				value = System.Math.Max(value, this.ZoomMin);
				value = System.Math.Min(value, this.ZoomMax);

				if ( this.iconContext.Zoom != value )
				{
					this.iconContext.Zoom = value;
					this.Invalidate();
					this.OnScrollerChanged();
					this.OnInfoZoomChanged();
					this.OnToolChanged();
				}
			}
		}

		// Origine X selon l'ascenseur horizontal.
		public double OriginX
		{
			get
			{
				return this.iconContext.OriginX;
			}

			set
			{
				if ( this.iconContext.OriginX != value )
				{
					this.iconContext.OriginX = value;
					this.Invalidate();
					this.OnScrollerChanged();
					this.OnToolChanged();
				}
			}
		}

		// Origine Y selon l'ascenseur vertical.
		public double OriginY
		{
			get
			{
				return this.iconContext.OriginY;
			}

			set
			{
				if ( this.iconContext.OriginY != value )
				{
					this.iconContext.OriginY = value;
					this.Invalidate();
					this.OnScrollerChanged();
					this.OnToolChanged();
				}
			}
		}

		// Objet global.
		public IconObjects IconObjects
		{
			get
			{
				return this.iconObjects;
			}

			set
			{
				this.iconObjects = value;
			}
		}

		// Liste des objets.
		public System.Collections.ArrayList Objects
		{
			get
			{
				return this.iconObjects.Objects;
			}

			set
			{
				this.iconObjects.Objects = value;
			}
		}

		// Ajoute un widget SampleButton qui représente la même icone.
		public void AddClone(Widget widget)
		{
			this.clones.Add(widget);
		}

		// Texte pour les informations.
		public string TextInfoObject
		{
			get
			{
				string text;
				text = string.Format("Selection: {0}/{1}", this.iconObjects.TotalSelected(), this.iconObjects.Count);
				return text;
			}
		}

		// Texte pour les informations.
		public string TextInfoMouse
		{
			get
			{
				string text;
				text = string.Format("(x:{0} y:{1})", mousePos.X.ToString("F2"), mousePos.Y.ToString("F2"));
				return text;
			}
		}

		// Texte pour les informations.
		public string TextInfoZoom
		{
			get
			{
				string text;
				text = string.Format("Zoom {0}%", (this.iconContext.Zoom*100).ToString("F0"));
				return text;
			}
		}

		// Nom de l'outil sélectionné.
		public string SelectedTool
		{
			get
			{
				return this.selectedTool;
			}

			set
			{
				this.CreateEnding();

				if ( this.selectedTool != value )
				{
					bool lastTool = this.IsTool();
					this.selectedTool = value;

					if ( this.IsTool() )
					{
						if ( lastTool == false && this.rankLastCreated != -1 )
						{
							this.Select(this.iconObjects[this.rankLastCreated], false);
							this.iconObjects.UpdateEditProperties();
							this.rankLastCreated = -1;
							this.InvalidateAll();
							this.OnInfoObjectChanged();
						}

						if ( this.selectedTool == "Select" )
						{
							this.MouseCursor = MouseCursor.AsArrow;
						}
						if ( this.selectedTool == "Zoom" )
						{
							this.MouseCursor = MouseCursor.AsArrow;  // TODO: loupe ?
						}
						if ( this.selectedTool == "Hand" )
						{
							this.MouseCursor = MouseCursor.AsHand;
						}
						if ( this.selectedTool == "Picker" )
						{
							this.MouseCursor = MouseCursor.AsArrow;
						}
					}
					else
					{
						this.Select(null, false);  // désélectionne tout
						this.InvalidateAll();
						this.OnInfoObjectChanged();
						this.MouseCursor = MouseCursor.AsCross;
					}
				}
			}
		}


		// Indique si l'outil sélectionné n'est pas un objet.
		protected bool IsTool()
		{
			if ( this.selectedTool == "Select" )  return true;
			if ( this.selectedTool == "Zoom"   )  return true;
			if ( this.selectedTool == "Hand"   )  return true;
			if ( this.selectedTool == "Picker" )  return true;
			return false;
		}

		// Retourne la liste des propriétés en fonction de l'objet sélectionné.
		// Un type de propriété donné n'est qu'une fois dans la liste.
		public System.Collections.ArrayList PropertiesList()
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList();

			if ( this.IsTool() )
			{
				this.iconObjects.PropertiesList(list, this.objectMemory);
			}
			else
			{
				this.newObject = this.CreateObject();
				this.newObject.CloneProperties(this.objectMemory);

				int total = this.newObject.TotalProperty;
				for ( int i=0 ; i<total ; i++ )
				{
					AbstractProperty property = this.newObject.Property(i);
					property.Multi = false;
					list.Add(property);
				}
			}

			return list;
		}

		// Modifie juste l'état "étendu" d'une propriété.
		public void SetPropertyExtended(AbstractProperty property)
		{
			if ( this.IsTool() )
			{
				this.iconObjects.SetPropertyExtended(property);
				this.InvalidateAll();
			}
			else
			{
				this.newObject.SetPropertyExtended(property);
			}

			this.objectMemory.SetPropertyExtended(property);
		}

		// Modifie une propriété.
		public void SetProperty(AbstractProperty property)
		{
			if ( this.IsTool() )
			{
				AbstractObject obj = this.iconObjects.RetFirstSelected();
				this.UndoMemorize("Property", obj, property.Type);

				Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
				this.iconObjects.SetProperty(property, ref bbox);
				this.iconObjects.UpdateEditProperties();
				this.InvalidateAll(bbox);
			}
			else
			{
				this.newObject.SetProperty(property);
			}

			this.objectMemory.SetProperty(property);  // mémorise les changements
		}

		// Retourne une propriété.
		public AbstractProperty GetProperty(PropertyType type)
		{
			if ( this.IsTool() )
			{
				return this.iconObjects.GetProperty(type, this.objectMemory);
			}
			else
			{
				return this.newObject.GetProperty(type);
			}
		}


		// Exécute une commande.
		public void ExecuteCommand(string cmd)
		{
			this.ExecuteCommand(cmd, "");
		}

		// Exécute une commande.
		public void ExecuteCommand(string cmd, string filename)
		{
			switch ( cmd )
			{
				case "New":
					this.UndoFlush();
					this.iconObjects.Clear();
					this.rankLastCreated = -1;
					this.Zoom = 1;
					this.OriginX = 0;
					this.OriginY = 0;
					this.OnPanelChanged();
					this.OnInfoObjectChanged();
					this.OnToolChanged();
					this.InvalidateAll();
					break;

				case "Open":
					if ( filename == "" )  break;
					this.UndoFlush();
					this.iconObjects.Clear();
					this.rankLastCreated = -1;
					this.iconObjects.Read(filename);
					this.Zoom = 1;
					this.OriginX = 0;
					this.OriginY = 0;
					this.OnInfoObjectChanged();
					this.OnToolChanged();
					this.InvalidateAll();
					break;

				case "Save":
					if ( filename == "" )  break;
					if ( this.iconObjects.InitialCount == 0 )  break;
					this.iconObjects.Write(filename);
					break;

				case "Delete":
					this.UndoMemorize(cmd);
					this.iconObjects.DeleteSelection();
					this.OnPanelChanged();
					this.OnToolChanged();
					this.OnInfoObjectChanged();
					this.InvalidateAll();
					break;

				case "Duplicate":
					this.UndoMemorize(cmd);
					this.iconObjects.DuplicateSelection(new Drawing.Point(1, 1));
					this.iconObjects.UpdateEditProperties();
					this.OnToolChanged();
					this.OnInfoObjectChanged();
					this.InvalidateAll();
					break;

				case "Undo":
					if ( !this.UndoRestore() )  break;
					this.InvalidateAll();
					this.OnToolChanged();
					break;

				case "Redo":
					if ( !this.RedoRestore() )  break;
					this.InvalidateAll();
					this.OnToolChanged();
					break;

				case "OrderUp":
					this.UndoMemorize(cmd);
					this.iconObjects.OrderSelection(1);
					this.OnToolChanged();
					this.InvalidateAll();
					break;

				case "OrderDown":
					this.UndoMemorize(cmd);
					this.iconObjects.OrderSelection(-1);
					this.OnToolChanged();
					this.InvalidateAll();
					break;

				case "Merge":
					this.UndoMemorize(cmd);
					this.iconObjects.UngroupSelection();
					this.iconObjects.GroupSelection();
					this.OnToolChanged();
					this.OnInfoObjectChanged();
					this.InvalidateAll();
					break;

				case "Group":
					this.UndoMemorize(cmd);
					this.iconObjects.GroupSelection();
					this.OnToolChanged();
					this.OnInfoObjectChanged();
					this.InvalidateAll();
					break;

				case "Ungroup":
					this.UndoMemorize(cmd);
					this.iconObjects.UngroupSelection();
					this.OnToolChanged();
					this.OnInfoObjectChanged();
					this.InvalidateAll();
					break;

				case "Inside":
					this.InsideSelection();
					this.OnToolChanged();
					this.OnInfoObjectChanged();
					this.InvalidateAll();
					break;

				case "Outside":
					this.OutsideSelection();
					this.OnPanelChanged();
					this.OnToolChanged();
					this.OnInfoObjectChanged();
					this.InvalidateAll();
					break;

				case "Grid":
					this.gridShow = !this.gridShow;
					this.Invalidate();
					this.OnToolChanged();
					break;

				case "Deselect":
				case "SelectAll":
				case "SelectInvert":
					this.SelectedTool = "Select";
					this.ChangeSelection(cmd);
					this.OnPanelChanged();
					this.iconObjects.UpdateEditProperties();
					this.OnToolChanged();
					this.OnInfoObjectChanged();
					this.InvalidateAll();
					break;

				case "ZoomMin":
					this.ZoomMemorize();
					this.Zoom = this.ZoomMin;
					this.OriginX = 0;
					this.OriginY = 0;
					break;

				case "ZoomDefault":
					this.ZoomMemorize();
					this.Zoom = 1;
					this.OriginX = 0;
					this.OriginY = 0;
					break;

				case "ZoomSel":
					this.ZoomSel();
					break;

				case "ZoomPrev":
					this.ZoomPrev();
					break;

				case "ZoomSub":
					this.ZoomChange(0.5);
					break;

				case "ZoomAdd":
					this.ZoomChange(2);
					break;
			}
		}

		// Indique si une commande est enable.
		public bool IsCommandEnable(string cmd)
		{
			bool enable = true;

			switch ( cmd )
			{
				case "Save":
					enable = ( this.iconObjects.InitialCount > 0 );
					break;

				case "Delete":
				case "Duplicate":
					enable = ( this.iconObjects.TotalSelected() > 0 );
					break;

				case "OrderUp":
				case "OrderDown":
					enable = ( this.iconObjects.Count > 1 && this.iconObjects.TotalSelected() > 0 );
					break;

				case "Merge":
				case "Group":
					enable = ( this.iconObjects.TotalSelected() > 1 );
					break;

				case "Ungroup":
					enable = ( this.iconObjects.TotalSelected() == 1 && this.iconObjects.RetFirstSelected() is ObjectGroup );
					break;

				case "Inside":
					enable = ( this.iconObjects.TotalSelected() == 1 && this.iconObjects.RetFirstSelected() is ObjectGroup );
					break;

				case "Outside":
					enable = ( !this.iconObjects.IsInitialGroup() );
					break;

				case "Undo":
					enable = ( this.undoIndex > 0 );
					break;

				case "Redo":
					enable = ( this.undoIndex < this.undoList.Count-1 );
					break;

				case "Deselect":
					enable = ( this.iconObjects.TotalSelected() > 0 );
					break;

				case "SelectAll":
					enable = ( this.iconObjects.TotalSelected() < this.iconObjects.Count );
					break;

				case "SelectInvert":
					enable = ( this.iconObjects.Count > 0 );
					break;

				case "ZoomMin":
					enable = ( this.Zoom > this.ZoomMin );
					break;

				case "ZoomDefault":
					enable = ( this.Zoom != 1 || this.OriginX != 0 || this.OriginY != 0 );
					break;

				case "ZoomSel":
					enable = ( this.iconObjects.TotalSelected() > 0 );
					break;

				case "ZoomPrev":
					enable = ( this.zoomHistory.Count > 0 );
					break;

				case "ZoomSub":
					enable = ( this.Zoom > this.ZoomMin );
					break;

				case "ZoomAdd":
					enable = ( this.Zoom < this.ZoomMax );
					break;
			}

			return enable;
		}

		// Indique si une commande est active.
		public bool IsCommandActive(string cmd)
		{
			bool active = true;

			switch ( cmd )
			{
				case "Grid":
					active = this.gridShow;
					break;

				case "Mode":
					active = !this.isActive;
					break;

				default:
					active = ( cmd == this.selectedTool );
					break;
			}

			if ( cmd.StartsWith("Look.") )
			{
				active = ( cmd.Substring(5) == Epsitec.Common.Widgets.Adorner.Factory.ActiveName );
			}

			return active;
		}


		// Crée un nouvel objet selon l'outil sélectionné.
		protected AbstractObject CreateObject()
		{
#if true
			AbstractObject obj = null;
			switch ( this.selectedTool )
			{
				case "ObjectLine":       obj = new ObjectLine();       break;
				case "ObjectArrow":      obj = new ObjectArrow();      break;
				case "ObjectRectangle":  obj = new ObjectRectangle();  break;
				case "ObjectCircle":     obj = new ObjectCircle();     break;
				case "ObjectEllipse":    obj = new ObjectEllipse();    break;
				case "ObjectRegular":    obj = new ObjectRegular();    break;
				case "ObjectPoly":       obj = new ObjectPoly();       break;
				case "ObjectBezier":     obj = new ObjectBezier();     break;
				case "ObjectText":       obj = new ObjectText();       break;
			}
			if ( obj == null )  return null;
			obj.CreateProperties();
			return obj;
#else
			return AbstractObject obj = System.Activator.CreateInstance(null, this.selectedTool);
#endif
		}


		protected Drawing.Point ScreenToIcon(Drawing.Point pos)
		{
			pos.X = pos.X/this.iconContext.ScaleX - this.iconContext.OriginX;
			pos.Y = pos.Y/this.iconContext.ScaleY - this.iconContext.OriginY;
			return pos;
		}

		protected Drawing.Point IconToScreen(Drawing.Point pos)
		{
			pos.X = (pos.X+this.iconContext.OriginX)*this.iconContext.ScaleX;
			pos.Y = (pos.Y+this.iconContext.OriginY)*this.iconContext.ScaleY;
			return pos;
		}

		// Gestion d'un événement.
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			if ( !this.isEditable )  return;

			pos = this.ScreenToIcon(pos);

			switch ( message.Type )
			{
				case MessageType.MouseDown:
					if ( message.IsLeftButton )
					{
						if ( this.selectedTool == "Select" )
						{
							this.SelectMouseDown(pos, message.IsShiftPressed, message.IsCtrlPressed);
						}
						else if ( this.selectedTool == "Zoom" )
						{
							this.ZoomMouseDown(pos, message.IsShiftPressed, message.IsCtrlPressed);
						}
						else if ( this.selectedTool == "Hand" )
						{
							this.HandMouseDown(pos, message.IsShiftPressed, message.IsCtrlPressed);
						}
						else if ( this.selectedTool == "Picker" )
						{
							this.PickerMouseDown(pos, message.IsShiftPressed, message.IsCtrlPressed);
						}
						else
						{
							this.CreateMouseDown(pos, message.IsShiftPressed, message.IsCtrlPressed);
						}
					}
					if ( message.IsRightButton )
					{
						this.rankLastCreated = -1;
						this.SelectedTool = "Select";
						this.OnAllChanged();
						this.SelectMouseDown(pos, message.IsShiftPressed, message.IsCtrlPressed);
					}
					this.mouseDown = true;
					break;
				
				case MessageType.MouseMove:
					this.mousePos = pos;
					this.OnInfoMouseChanged();

					if ( this.selectedTool == "Select" )
					{
						this.SelectMouseMove(pos, message.IsShiftPressed, message.IsCtrlPressed);
					}
					else if ( this.selectedTool == "Zoom" )
					{
						this.ZoomMouseMove(pos, message.IsShiftPressed, message.IsCtrlPressed);
					}
					else if ( this.selectedTool == "Hand" )
					{
						this.HandMouseMove(pos, message.IsShiftPressed, message.IsCtrlPressed);
					}
					else if ( this.selectedTool == "Picker" )
					{
						this.PickerMouseMove(pos, message.IsShiftPressed, message.IsCtrlPressed);
					}
					else
					{
						this.CreateMouseMove(pos, message.IsShiftPressed, message.IsCtrlPressed);
					}
					break;

				case MessageType.MouseUp:
					if ( this.mouseDown )
					{
						if ( this.selectedTool == "Select" )
						{
							this.SelectMouseUp(pos, message.IsShiftPressed, message.IsCtrlPressed, message.IsRightButton);
						}
						else if ( this.selectedTool == "Zoom" )
						{
							this.ZoomMouseUp(pos, message.IsShiftPressed, message.IsCtrlPressed);
						}
						else if ( this.selectedTool == "Hand" )
						{
							this.HandMouseUp(pos, message.IsShiftPressed, message.IsCtrlPressed);
						}
						else if ( this.selectedTool == "Picker" )
						{
							this.PickerMouseUp(pos, message.IsShiftPressed, message.IsCtrlPressed);
						}
						else
						{
							this.CreateMouseUp(pos, message.IsShiftPressed, message.IsCtrlPressed);
						}
						this.mouseDown = false;
					}
					break;

				case MessageType.MouseWheel:
					double factor = (message.Wheel > 0) ? 1.5 : 1.0/1.5;
					this.ZoomChange(factor, pos);
					break;

				case MessageType.KeyDown:
					if ( message.KeyCode == KeyCode.ControlKey )
					{
						this.iconContext.IsCtrl = true;
						this.Invalidate();
					}
					if ( this.createRank != -1 )
					{
						if ( message.KeyCode == KeyCode.Escape )
						{
							this.CreateEnding();
						}
					}

					if ( message.KeyCode == KeyCode.Back   ||
						 message.KeyCode == KeyCode.Delete )
					{
						this.ExecuteCommand("Delete");
					}
					break;

				case MessageType.KeyUp:
					if ( message.KeyCode == KeyCode.ControlKey )
					{
						this.iconContext.IsCtrl = false;
						this.Invalidate();
					}
					break;
			}
			
			message.Consumer = this;
		}


		// Construit le menu contextuel.
		protected void ContextMenu(Drawing.Point mouse, bool globalMenu)
		{
			this.iconObjects.Hilite(null);

			int nbSel = this.iconObjects.TotalSelected();
			System.Collections.ArrayList list = new System.Collections.ArrayList();
			if ( globalMenu || nbSel == 0 )
			{
				this.MenuAddItem(list, "Deselect",     "file:images/deselect1.icon",     "Deselectionner tout");
				this.MenuAddItem(list, "SelectAll",    "file:images/selectall1.icon",    "Tout selectionner");
				this.MenuAddItem(list, "SelectInvert", "file:images/selectinvert1.icon", "Inverser la selection");
				this.MenuAddSep(list);
				this.MenuAddItem(list, "ZoomMin",      "file:images/zoommin1.icon",      "Zoom minimal");
				this.MenuAddItem(list, "ZoomDefault",  "file:images/zoomdefault1.icon",  "Zoom 100%");
				this.MenuAddItem(list, "ZoomSel",      "file:images/zoomsel1.icon",      "Zoom selection");
				this.MenuAddItem(list, "ZoomPrev",     "file:images/zoomprev1.icon",     "Zoom precedent");
				this.MenuAddItem(list, "ZoomSub",      "file:images/zoomsub1.icon",      "Reduction");
				this.MenuAddItem(list, "ZoomAdd",      "file:images/zoomadd1.icon",      "Agrandissement");
				this.MenuAddSep(list);
				this.MenuAddItem(list, "Outside",      "file:images/outside1.icon",      "Sortir du groupe");
				this.MenuAddItem(list, "Grid",         "file:images/grid1.icon",         "Grille");
			}
			else
			{
				this.contextMenuPos = mouse;
				this.iconObjects.DetectHandle(mouse, out this.contextMenuObject, out this.contextMenuRank);
				if ( nbSel == 1 && this.contextMenuObject == null )
				{
					this.contextMenuObject = this.iconObjects.RetFirstSelected();
					this.contextMenuRank = -1;
				}

				this.MenuAddItem(list, "Delete",    "file:images/delete1.icon",    "Supprimer");
				this.MenuAddItem(list, "Duplicate", "file:images/duplicate1.icon", "Dupliquer");
				this.MenuAddItem(list, "OrderUp",   "file:images/orderup1.icon",   "Devant");
				this.MenuAddItem(list, "OrderDown", "file:images/orderdown1.icon", "Derriere");
				this.MenuAddItem(list, "Merge",     "file:images/merge1.icon",     "Fusionner");
				this.MenuAddItem(list, "Group",     "file:images/group1.icon",     "Associer");
				this.MenuAddItem(list, "Ungroup",   "file:images/ungroup1.icon",   "Dissocier");
				this.MenuAddItem(list, "Inside",    "file:images/inside1.icon",    "Entrer dans groupe");
				this.MenuAddItem(list, "Outside",   "file:images/outside1.icon",   "Sortir du groupe");
				this.MenuAddSep(list);
				this.MenuAddItem(list, "ZoomSel",   "file:images/zoomsel1.icon",   "Zoom selection");

				if ( nbSel == 1 && this.contextMenuObject != null )
				{
					this.contextMenuObject.ContextMenu(list, mouse, this.contextMenuRank);
				}
			}

			this.contextMenu = new VMenu();
			this.contextMenu.AppWindow = this.Window;
			
			foreach ( ContextMenuItem cmi in list )
			{
				if ( cmi.Name == "" )
				{
					this.contextMenu.Items.Add(new MenuSeparator());
				}
				else
				{
					MenuItem mi = new MenuItem(cmi.Name, cmi.Icon, cmi.Text, "");
					mi.IconNameActiveNo = cmi.IconActiveNo;
					mi.IconNameActiveYes = cmi.IconActiveYes;
					mi.ActiveState = cmi.Active ? WidgetState.ActiveYes : WidgetState.ActiveNo;
					mi.Pressed += new MessageEventHandler(this.MenuPressed);
					this.contextMenu.Items.Add(mi);
				}
			}
			this.contextMenu.AdjustSize();
			mouse = this.IconToScreen(mouse);
			mouse = this.MapClientToScreen(mouse);
			this.contextMenu.ShowContextMenu(mouse);
		}

		// Ajoute une case dans le menu.
		protected void MenuAddItem(System.Collections.ArrayList list, string cmd, string icon, string text)
		{
			if ( !this.IsCommandEnable(cmd) )  return;

			ContextMenuItem item = new ContextMenuItem();
			item.Name = cmd;
			item.Icon = @icon;
			item.Text = text;
			list.Add(item);
		}

		// Ajoute un séparateur dans le menu.
		protected void MenuAddSep(System.Collections.ArrayList list)
		{
			ContextMenuItem item = new ContextMenuItem();
			list.Add(item);  // séparateur
		}

		// Indique si le menu est visible.
		protected bool IsMenu()
		{
			if ( this.contextMenu == null )  return false;
			return this.contextMenu.IsVisible;
		}

		// Appelé lorsqu'une case du menu est actionnée.
		private void MenuPressed(object sender, MessageEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			this.ContextCommand(item.Name);
			this.contextMenu = null;
		}

		// Effectue une commande du menu.
		protected void ContextCommand(string cmd)
		{
			if ( this.contextMenuObject != null && cmd.StartsWith("Object.") )
			{
				this.UndoMemorize("Object");
				this.contextMenuObject.ContextCommand(cmd, this.contextMenuPos, this.contextMenuRank);
				this.InvalidateAll();
			}
			else
			{
				this.ExecuteCommand(cmd);
			}
		}


		protected void SelectMouseDown(Drawing.Point mouse, bool isShift, bool isCtrl)
		{
			this.UndoMemorize("Select");
			this.moveStart = mouse;
			this.iconContext.IsCtrl = isCtrl;
			this.iconContext.ConstrainFixStarting(mouse);
			this.iconObjects.Hilite(null);
			this.moveObject = null;
			this.selectRect = false;

			AbstractObject obj;
			int rank;
			if ( this.iconObjects.DetectHandle(mouse, out obj, out rank) )
			{
				this.moveObject = obj;
				this.moveHandle = rank;
				this.moveOffset = mouse-obj.Handle(rank).Position;
				this.moveObject.MoveHandleStarting(this.moveHandle, mouse, this.iconContext);
			}
			else
			{
				obj = this.iconObjects.Detect(mouse);
				if ( obj == null )
				{
					this.selectRect = true;
					this.selectRectP1 = mouse;
					this.selectRectP2 = mouse;
				}
				else
				{
					if ( !obj.IsSelected() )
					{
						this.Select(obj, isCtrl);
						this.OnInfoObjectChanged();
					}
					else
					{
						if ( isCtrl )
						{
							obj.Deselect();
							this.iconObjects.UpdateEditProperties();
							this.OnPanelChanged();
							this.OnInfoObjectChanged();
						}
					}
					this.moveObject = obj;
					this.moveHandle = -1;  // déplace tout l'objet
					this.SnapGrid(ref mouse);
					this.moveOffset = mouse;
				}
			}

			this.InvalidateAll();
		}

		protected void SelectMouseMove(Drawing.Point mouse, bool isShift, bool isCtrl)
		{
			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			this.iconContext.IsCtrl = isCtrl;
			if ( this.mouseDown )
			{
				if ( this.selectRect )
				{
					Drawing.Rectangle rSelect = new Drawing.Rectangle(this.selectRectP1, this.selectRectP2);
					bbox.MergeWith(rSelect);

					this.selectRectP2 = mouse;

					rSelect = new Drawing.Rectangle(this.selectRectP1, this.selectRectP2);
					bbox.MergeWith(rSelect);
				}
				else if ( this.moveObject != null )
				{
					if ( this.moveHandle == -1 )  // déplace tout l'objet ?
					{
						this.iconContext.ConstrainSnapPos(ref mouse);

						double len = Drawing.Point.Distance(mouse, this.moveStart);
						if ( len <= this.iconContext.MinimalSize )
						{
							mouse = this.moveStart;
						}
						this.SnapGrid(ref mouse);
						this.iconObjects.MoveSelection(mouse-this.moveOffset, ref bbox);
						this.moveOffset = mouse;
					}
					else	// déplace une poignée ?
					{
						bbox.MergeWith(this.moveObject.BoundingBox);
						mouse -= this.moveOffset;
						this.SnapGrid(ref mouse);
						this.moveObject.MoveHandleProcess(this.moveHandle, mouse, this.iconContext);
						bbox.MergeWith(this.moveObject.BoundingBox);
					}
				}
			}
			else
			{
				if ( this.IsMenu() )
				{
					this.MouseCursor = MouseCursor.AsArrow;
				}
				else
				{
					AbstractObject obj;
					int rank;
					if ( this.iconObjects.DetectHandle(mouse, out obj, out rank) )
					{
						this.iconObjects.Hilite(null);
						this.MouseCursor = MouseCursor.AsCross;
					}
					else
					{
						obj = this.iconObjects.Detect(mouse);
						this.iconObjects.Hilite(obj);
						if ( obj == null )
						{
							this.MouseCursor = MouseCursor.AsArrow;
						}
						else
						{
							if ( obj.IsSelected() )
							{
								this.MouseCursor = MouseCursor.AsHand;
							}
							else
							{
								this.MouseCursor = MouseCursor.AsArrow;
							}
						}
					}
				}
				this.Window.MouseCursor = this.MouseCursor;
			}

			this.InvalidateAll(bbox);
		}

		protected void SelectMouseUp(Drawing.Point mouse, bool isShift, bool isCtrl, bool isRight)
		{
			this.iconContext.IsCtrl = isCtrl;
			bool globalMenu = false;

			if ( this.selectRect )
			{
				this.selectRect = false;
				double len = Drawing.Point.Distance(mouse, this.moveStart);
				if ( isRight && len <= this.iconContext.MinimalSize )
				{
					globalMenu = true;
				}
				else
				{
					Drawing.Rectangle rSelect = new Drawing.Rectangle(this.selectRectP1, this.selectRectP2);
					this.Select(rSelect, isCtrl);  // sélectionne les objets dans le rectangle
					this.OnInfoObjectChanged();
					this.UndoMemorizeRemove();
				}
			}

			if ( this.moveObject != null )
			{
				Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
				this.iconObjects.GroupUpdate(ref bbox);
				if ( !bbox.IsEmpty )
				{
					this.InvalidateAll(bbox);
				}

				if ( this.moveHandle != -1 )  // déplace une poignée ?
				{
					if ( this.moveObject.Handle(this.moveHandle).Type == HandleType.Property )
					{
						this.OnPanelChanged();
					}
				}

				this.moveObject = null;
				this.moveHandle = -1;

				double len = Drawing.Point.Distance(mouse, this.moveStart);
				if ( len <= this.iconContext.MinimalSize )
				{
					this.UndoMemorizeRemove();
				}
			}

			this.iconContext.ConstrainDelStarting();

			if ( isRight )  // avec le bouton de droite de la souris ?
			{
				this.InvalidateAll();
				this.ContextMenu(mouse, globalMenu);
			}

			this.OnToolChanged();
		}


		protected void ZoomMouseDown(Drawing.Point mouse, bool isShift, bool isCtrl)
		{
			this.selectRect = true;
			this.selectRectP1 = mouse;
			this.selectRectP2 = mouse;
		}
		
		protected void ZoomMouseMove(Drawing.Point mouse, bool isShift, bool isCtrl)
		{
			if ( this.mouseDown )
			{
				this.selectRectP2 = mouse;
				this.Invalidate();
			}
		}
		
		protected void ZoomMouseUp(Drawing.Point mouse, bool isShift, bool isCtrl)
		{
			this.selectRect = false;

			if ( isCtrl )
			{
				this.ZoomChange(1.0/2.0, (this.selectRectP1+this.selectRectP2)/2);
			}
			else
			{
				this.ZoomChange(this.selectRectP1, this.selectRectP2);
			}
		}

		// Zoom sur les objets sélectionnés.
		protected void ZoomSel()
		{
			Drawing.Rectangle bbox = this.iconObjects.RetSelectedBbox();
			if ( bbox.IsEmpty )  return;

			bbox.Inflate(2, 2);
			Drawing.Point p1 = new Drawing.Point(bbox.Left, bbox.Bottom);
			Drawing.Point p2 = new Drawing.Point(bbox.Right, bbox.Top);
			this.ZoomChange(p1, p2);
		}

		// Change le zoom d'un certain facteur pour agrandir un rectangle.
		protected void ZoomChange(Drawing.Point p1, Drawing.Point p2)
		{
			if ( p1.X == p2.X || p1.Y == p2.Y )  return;
			double fx = this.Width/(System.Math.Abs(p1.X-p2.X)*this.iconContext.ScaleX);
			double fy = this.Height/(System.Math.Abs(p1.Y-p2.Y)*this.iconContext.ScaleY);
			double factor = System.Math.Min(fx, fy);
			this.ZoomChange(factor, (p1+p2)/2);
		}

		// Change le zoom d'un certain facteur, avec centrage au centre du dessin.
		protected void ZoomChange(double factor)
		{
			Drawing.Point center = new Drawing.Point();
			center.X = -this.OriginX+(this.IconObjects.Size.Width/this.Zoom)/2;
			center.Y = -this.OriginY+(this.IconObjects.Size.Height/this.Zoom)/2;
			this.ZoomChange(factor, center);
		}

		// Change le zoom d'un certain facteur, avec centrage quelconque.
		protected void ZoomChange(double factor, Drawing.Point center)
		{
			double newZoom = this.Zoom*factor;
			newZoom = System.Math.Max(newZoom, this.ZoomMin);
			newZoom = System.Math.Min(newZoom, this.ZoomMax);
			if ( newZoom == this.Zoom )  return;

			this.ZoomMemorize();
			Drawing.Point origin = this.IconToScreen(center);
			this.Zoom = newZoom;

			origin = this.ScreenToIcon(origin);
			origin.X -= this.iconObjects.Size.Width/this.Zoom/2;
			origin.Y -= this.iconObjects.Size.Height/this.Zoom/2;
			this.OriginX = -origin.X;
			this.OriginY = -origin.Y;
		}

		// Mémorise le zoom actuel.
		protected void ZoomMemorize()
		{
			ZoomHistory.ZoomElement item = new ZoomHistory.ZoomElement();
			item.zoom = this.Zoom;
			item.ox   = this.OriginX;
			item.oy   = this.OriginY;
			this.zoomHistory.Add(item);
		}

		// Revient au niveau de zoom précédent.
		protected void ZoomPrev()
		{
			ZoomHistory.ZoomElement item = this.zoomHistory.Remove();
			if ( item == null )  return;
			this.Zoom    = item.zoom;
			this.OriginX = item.ox;
			this.OriginY = item.oy;
		}

		// Retourne le zoom minimal.
		protected double ZoomMin
		{
			get
			{
				double x = this.IconObjects.SizeArea.Width/this.IconObjects.Size.Width;
				double y = this.IconObjects.SizeArea.Height/this.IconObjects.Size.Height;
				return 1.0/System.Math.Min(x,y);
			}
		}

		// Retourne le zoom maximal.
		protected double ZoomMax
		{
			get
			{
				return 8.0;
			}
		}


		protected void HandMouseDown(Drawing.Point mouse, bool isShift, bool isCtrl)
		{
			mouse.X += this.iconContext.OriginX;
			mouse.Y += this.iconContext.OriginY;
			this.moveStart = mouse;
		}

		protected void HandMouseMove(Drawing.Point mouse, bool isShift, bool isCtrl)
		{
			if ( this.mouseDown )
			{
				mouse.X += this.iconContext.OriginX;
				mouse.Y += this.iconContext.OriginY;
				Drawing.Point move = mouse-this.moveStart;
				this.moveStart = mouse;
				this.OriginX += move.X;
				this.OriginY += move.Y;
			}
		}

		protected void HandMouseUp(Drawing.Point mouse, bool isShift, bool isCtrl)
		{
		}


		protected void PickerMouseDown(Drawing.Point mouse, bool isShift, bool isCtrl)
		{
			this.UndoMemorize("Picker");
			AbstractObject obj = this.iconObjects.DeepDetect(mouse);
			this.PickerProperties(mouse, obj);
		}

		protected void PickerMouseMove(Drawing.Point mouse, bool isShift, bool isCtrl)
		{
			AbstractObject obj = this.iconObjects.DeepDetect(mouse);
			this.iconObjects.DeepHilite(obj);

			if ( this.mouseDown )
			{
				this.PickerProperties(mouse, obj);
			}
			else
			{
				this.InvalidateAll();
			}
		}

		protected void PickerMouseUp(Drawing.Point mouse, bool isShift, bool isCtrl)
		{
			this.iconObjects.UpdateEditProperties();
		}

		// Modifie toutes les propriétés des objets sélectionnés en fonction
		// d'un objet quelconque (seed).
		protected void PickerProperties(Drawing.Point mouse, AbstractObject seed)
		{
			if ( seed == null )  return;

			int total = seed.TotalProperty;
			if ( this.iconObjects.TotalSelected() == 0 )
			{
				for ( int i=0 ; i<total ; i++ )
				{
					seed.CloneInfoProperties(this.objectMemory);
					AbstractProperty property = seed.Property(i);
					this.newObject.SetProperty(property);
					this.objectMemory.SetProperty(property);
				}
			}
			else
			{
				Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
				for ( int i=0 ; i<total ; i++ )
				{
					seed.CloneInfoProperties(this.objectMemory);
					AbstractProperty property = seed.Property(i);
					this.iconObjects.SetProperty(property, ref bbox);
					this.objectMemory.SetProperty(property);
				}
				this.InvalidateAll(bbox);
				this.OnPanelChanged();
			}
		}


		// Sélectionne un objet et désélectionne tous les autres.
		protected void Select(AbstractObject obj, bool add)
		{
			this.iconObjects.Select(obj, add);
			this.iconObjects.UpdateEditProperties();
			this.OnPanelChanged();
		}

		// Sélectionne tous les objets dans le rectangle.
		protected void Select(Drawing.Rectangle rect, bool add)
		{
			this.iconObjects.Select(rect, add);
			this.iconObjects.UpdateEditProperties();
			this.OnPanelChanged();
		}


		// Entre dans le groupe sélectionné.
		protected void InsideSelection()
		{
			AbstractObject obj = this.iconObjects.RetFirstSelected();
			this.iconObjects.InsideGroup(obj);
		}

		// Sort du groupe en cours.
		protected void OutsideSelection()
		{
			this.rankLastCreated = -1;
			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			this.iconObjects.GroupUpdate(ref bbox);
			this.iconObjects.OutsideGroup();
		}

		// Modifie la sélection courante.
		protected void ChangeSelection(string cmd)
		{
			for ( int index=0 ; index<this.iconObjects.Count ; index++ )
			{
				AbstractObject obj = this.iconObjects[index];

				if ( cmd == "Deselect" )
				{
					if ( obj.IsSelected() )
					{
						obj.Deselect();
					}
				}

				if ( cmd == "SelectAll" )
				{
					if ( !obj.IsSelected() )
					{
						obj.Select();
					}
				}

				if ( cmd == "SelectInvert" )
				{
					obj.Select(!obj.IsSelected());
				}
			}
		}


		protected void CreateMouseDown(Drawing.Point mouse, bool isShift, bool isCtrl)
		{
			this.iconContext.IsCtrl = isCtrl;
			this.SnapGrid(ref mouse);

			if ( this.createRank == -1 )
			{
				this.UndoMemorize("Create");
				AbstractObject obj = this.CreateObject();
				if ( obj == null )  return;
				obj.CloneProperties(this.newObject);
				obj.Select();
				obj.EditProperties = false;
				this.createRank = this.iconObjects.Add(obj);
			}

			this.iconObjects[this.createRank].CreateMouseDown(mouse, this.iconContext);
			this.InvalidateAll();
			this.OnInfoObjectChanged();
		}

		protected void CreateMouseMove(Drawing.Point mouse, bool isShift, bool isCtrl)
		{
			this.iconContext.IsCtrl = isCtrl;
			this.SnapGrid(ref mouse);

			if ( this.createRank == -1 )  return;
			Drawing.Rectangle bbox = this.iconObjects[this.createRank].BoundingBox;
			this.iconObjects[this.createRank].CreateMouseMove(mouse, this.iconContext);
			bbox.MergeWith(this.iconObjects[this.createRank].BoundingBox);
			this.InvalidateAll(bbox);
		}

		protected void CreateMouseUp(Drawing.Point mouse, bool isShift, bool isCtrl)
		{
			this.iconContext.IsCtrl = isCtrl;
			this.SnapGrid(ref mouse);

			if ( this.createRank == -1 )  return;
			this.iconObjects[this.createRank].CreateMouseUp(mouse, this.iconContext);

			if ( this.iconObjects[this.createRank].CreateIsEnding(this.iconContext) )
			{
				if ( this.iconObjects[this.createRank].CreateIsExist(this.iconContext) )
				{
					this.iconObjects[this.createRank].Deselect();
					this.rankLastCreated = this.createRank;
				}
				else
				{
					this.iconObjects.RemoveAt(this.createRank);
					this.UndoMemorizeRemove();
				}
				this.createRank = -1;
			}

			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			this.iconObjects.GroupUpdate(ref bbox);

			this.InvalidateAll();
			this.OnInfoObjectChanged();
			this.OnToolChanged();
		}

		protected void CreateEnding()
		{
			if ( this.createRank == -1 )  return;
			if ( this.iconObjects[this.createRank].CreateEnding(this.iconContext) )
			{
				this.iconObjects[this.createRank].Deselect();
				this.rankLastCreated = this.createRank;
			}
			else
			{
				this.iconObjects.RemoveAt(this.createRank);
				this.UndoMemorizeRemove();
			}
			this.createRank = -1;
			this.InvalidateAll();
			this.OnInfoObjectChanged();
		}


		// Vide la liste des annulations.
		protected void UndoFlush()
		{
			this.undoList.Clear();
			this.undoIndex = 0;
			this.zoomHistory.Clear();
		}

		// Mémorise l'icône dans son état actuel.
		protected void UndoMemorize(string operation)
		{
			this.UndoMemorize(operation, null, PropertyType.None);
		}

		// Mémorise l'icône dans son état actuel.
		protected void UndoMemorize(string operation, AbstractObject obj, PropertyType propertyType)
		{
			bool toolChanged = false;

			int total = this.undoList.Count;
			if ( total > 0 && propertyType != PropertyType.None )
			{
				UndoSituation last = this.undoList[total-1] as UndoSituation;
				if ( operation    == last.Operation    &&
					 obj          == last.Object       &&
					 propertyType == last.PropertyType )
				{
					return;
				}
				toolChanged = true;
			}

			while ( this.undoIndex < this.undoList.Count )
			{
				this.undoList.RemoveAt(this.undoList.Count-1);  // efface le redo
			}

			UndoSituation situation = new UndoSituation();
			situation.Operation    = operation;
			situation.Object       = obj;
			situation.PropertyType = propertyType;
			situation.SelectedTool = this.selectedTool;

			IconObjects io = new IconObjects();
			this.iconObjects.CopyTo(io.Objects);
			situation.IconObjects = io;

			this.undoList.Add(situation);
			this.undoIndex = this.undoList.Count;

			if ( toolChanged )  this.OnToolChanged();
		}

		// Supprime le dernier UndoMemorize inutile, par exemple parce que l'objet
		// créé a finalement été détruit car il était trop petit.
		protected void UndoMemorizeRemove()
		{
			int total = this.undoList.Count;
			if ( total == 0 )  return;
			this.undoList.RemoveAt(total-1);
			this.undoIndex = this.undoList.Count;
		}

		// Remet le dessin dans son état précédent.
		protected bool UndoRestore()
		{
			if ( this.undoIndex == 0 )  return false;

			if ( this.undoIndex == this.undoList.Count )
			{
				this.UndoMemorize("Undo");
				this.undoIndex --;
			}

			UndoSituation last = this.undoList[--this.undoIndex] as UndoSituation;
			last.IconObjects.CopyTo(this.iconObjects.Objects);
			this.selectedTool = last.SelectedTool;
			this.OnAllChanged();
			this.OnInfoObjectChanged();
			this.rankLastCreated = -1;
			return true;
		}

		// Annule la dernière annulation.
		protected bool RedoRestore()
		{
			if ( this.undoIndex >= this.undoList.Count-1 )  return false;

			UndoSituation last = this.undoList[++this.undoIndex] as UndoSituation;
			last.IconObjects.CopyTo(this.iconObjects.Objects);
			this.selectedTool = last.SelectedTool;
			this.OnAllChanged();
			this.OnInfoObjectChanged();
			return true;
		}


		// Invalide la zone ainsi que celles des clones.
		protected void InvalidateAll(Drawing.Rectangle bbox)
		{
			//?System.Diagnostics.Debug.WriteLine("InvalidateAll bbox");
#if false
			if ( bbox.IsEmpty )  return;

			bbox.BottomLeft = this.IconToScreen(bbox.BottomLeft);
			bbox.TopRight   = this.IconToScreen(bbox.TopRight);
			this.Invalidate(bbox);
#else
			this.Invalidate();
#endif

			foreach ( Widget widget in this.clones )
			{
				SampleButton sample = widget as SampleButton;
				if ( sample != null )
				{
					sample.IconObjects.Size = this.iconObjects.Size;
					sample.IconObjects.Origin = this.iconObjects.Origin;
				}
				
				widget.Invalidate();
			}
		}

		// Invalide la zone ainsi que celles des clones.
		protected void InvalidateAll()
		{
			//?System.Diagnostics.Debug.WriteLine("InvalidateAll");
			this.Invalidate();

			foreach ( Widget widget in this.clones )
			{
				SampleButton sample = widget as SampleButton;
				if ( sample != null )
				{
					sample.IconObjects.Size = this.iconObjects.Size;
					sample.IconObjects.Origin = this.iconObjects.Origin;
				}
				
				widget.Invalidate();
			}
		}


		// Force un point sur la grille magnétique.
		protected void SnapGrid(ref Drawing.Point pos)
		{
			if ( !this.gridShow )  return;
			pos = pos.GridAlign(new Drawing.Point(this.gridStep.X/2, this.gridStep.Y/2), this.gridStep);
		}

		// Dessine la grille magnétique dessous.
		protected void DrawGridBackground(Drawing.Graphics graphics)
		{
			double initialWidth = graphics.LineWidth;
			graphics.LineWidth = 1.0/this.iconContext.ScaleX;

			double ix = 0.5/this.iconContext.ScaleX;
			double iy = 0.5/this.iconContext.ScaleY;

			if ( this.gridShow )
			{
				double step = this.gridStep.X;
				for ( double pos=this.iconObjects.OriginArea.X ; pos<=this.iconObjects.SizeArea.Width ; pos+=step )
				{
					double x = pos;
					double y = this.iconObjects.OriginArea.Y;
					graphics.Align(ref x, ref y);
					x += ix;
					y += iy;
					graphics.AddLine(x, y, x, this.iconObjects.SizeArea.Height);
				}
				step = this.gridStep.Y;
				for ( double pos=this.iconObjects.OriginArea.Y ; pos<=this.iconObjects.SizeArea.Height ; pos+=step )
				{
					double x = this.iconObjects.OriginArea.X;
					double y = pos;
					graphics.Align(ref x, ref y);
					x += ix;
					y += iy;
					graphics.AddLine(x, y, this.iconObjects.SizeArea.Width, y);
				}
				//?graphics.RenderSolid(Drawing.Color.FromBrightness(0.9));
				graphics.RenderSolid(Drawing.Color.FromARGB(0.3, 0.6,0.6,0.6));
			}

			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.iconObjects.Size.Width, this.iconObjects.Size.Height);
			graphics.Align(ref rect);
			rect.Offset(ix, iy);
			graphics.AddRectangle(rect);

			rect.Offset(-ix, -iy);
			rect.Inflate(-2, -2);
			graphics.Align(ref rect);
			rect.Offset(ix, iy);
			graphics.AddRectangle(rect);

			double cx = this.iconObjects.Size.Width/2;
			double cy = this.iconObjects.Size.Height/2;
			graphics.Align(ref cx, ref cy);
			cx += ix;
			cy += iy;
			graphics.AddLine(cx, 0, cx, this.iconObjects.Size.Height);
			graphics.AddLine(0, cy, this.iconObjects.Size.Width, cy);
			//?graphics.RenderSolid(Drawing.Color.FromBrightness(1.0));
			graphics.RenderSolid(Drawing.Color.FromARGB(0.4, 0.5,0.5,0.5));

			graphics.LineWidth = initialWidth;
		}

		// Dessine la grille magnétique dessus.
		protected void DrawGridForeground(Drawing.Graphics graphics)
		{
			if ( !this.gridShow )  return;

			double ix = 0.5/this.iconContext.ScaleX;
			double iy = 0.5/this.iconContext.ScaleY;

			double sx = this.gridStep.X;
			double sy = this.gridStep.Y;
			for ( double py=this.iconObjects.OriginArea.Y ; py<=this.iconObjects.SizeArea.Height ; py+=sy )
			{
				for ( double px=this.iconObjects.OriginArea.X ; px<=this.iconObjects.SizeArea.Width ; px+=sx )
				{
					double x = px;
					double y = py;
					graphics.Align(ref x, ref y);
					graphics.AddFilledRectangle(x, y, 1.0/this.iconContext.ScaleX, 1.0/this.iconContext.ScaleY);
				}
			}
			graphics.RenderSolid(Drawing.Color.FromBrightness(0.9));
		}

		// Dessine les contraintes.
		protected void DrawConstrain(Drawing.Graphics graphics, Drawing.Point pos, ConstrainType type)
		{
			graphics.LineWidth = 1.0/this.iconContext.ScaleX;

			if ( type == ConstrainType.Normal || type == ConstrainType.Line )
			{
				graphics.AddLine(pos.X, 0, pos.X, this.iconObjects.Size.Height);
				graphics.AddLine(0, pos.Y, this.iconObjects.Size.Width, pos.Y);
				graphics.RenderSolid(Drawing.Color.FromARGB(0.5, 1,0,0));
			}

			if ( type == ConstrainType.Normal || type == ConstrainType.Square )
			{
				Drawing.Point p1 = Drawing.Transform.RotatePoint(pos, System.Math.PI*0.25, pos+new Drawing.Point(this.iconObjects.Size.Width,0));
				Drawing.Point p2 = Drawing.Transform.RotatePoint(pos, System.Math.PI*1.25, pos+new Drawing.Point(this.iconObjects.Size.Width,0));
				graphics.AddLine(p1, p2);

				p1 = Drawing.Transform.RotatePoint(pos, System.Math.PI*0.75, pos+new Drawing.Point(this.iconObjects.Size.Width,0));
				p2 = Drawing.Transform.RotatePoint(pos, System.Math.PI*1.75, pos+new Drawing.Point(this.iconObjects.Size.Width,0));
				graphics.AddLine(p1, p2);

				graphics.RenderSolid(Drawing.Color.FromARGB(0.5, 1,0,0));
			}
		}

		// Dessine l'icône.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Epsitec.Common.Widgets.Adorner.Factory.Active;
			this.iconContext.Adorner = adorner;
			this.iconContext.UniqueColor = Drawing.Color.Empty;

			double initialWidth = graphics.LineWidth;
			Drawing.Transform save = graphics.SaveTransform();
			this.iconContext.ScaleX = this.iconContext.Zoom*this.Client.Width/this.iconObjects.Size.Width;
			this.iconContext.ScaleY = this.iconContext.Zoom*this.Client.Height/this.iconObjects.Size.Height;
			graphics.ScaleTransform(this.iconContext.ScaleX, this.iconContext.ScaleY, 0, 0);
			graphics.TranslateTransform(this.iconContext.OriginX, this.iconContext.OriginY);

			//TODO: Drawing.Rectangle clip = graphics.SaveClippingRectangle();
			//TODO: graphics.ResetClippingRectangle();
			//TODO: graphics.SetClippingRectangle(clipRect);

			// Dessine la grille magnétique.
			if ( this.isEditable )
			{
				//?this.DrawGridBackground(graphics);
			}

			// Dessine les géométries.
			this.iconObjects.DrawGeometry(graphics, this.iconContext, adorner);

			// Dessine la grille magnétique.
			if ( this.isEditable )
			{
				//?this.DrawGridForeground(graphics);
				this.DrawGridBackground(graphics);
			}

			// Dessine les poignées.
			if ( this.isEditable )
			{
				this.iconObjects.DrawHandle(graphics, this.iconContext);
			}

			// Dessine le rectangle de sélection.
			if ( this.isEditable && this.selectRect )
			{
				Drawing.Rectangle rSelect = new Drawing.Rectangle(this.selectRectP1, this.selectRectP2);
				graphics.LineWidth = 1.0/this.iconContext.ScaleX;
				rSelect.Inflate(-0.5/this.iconContext.ScaleX, -0.5/this.iconContext.ScaleY);
				graphics.AddRectangle(rSelect);
				graphics.RenderSolid(this.iconContext.HiliteColor);
			}

			// Dessine les contraintes.
			Drawing.Point pos;
			ConstrainType type;
			if ( this.isEditable && this.iconContext.ConstrainGetStarting(out pos, out type) )
			{
				this.DrawConstrain(graphics, pos, type);
			}

			//TODO: graphics.RestoreClippingRectangle(clip);
			graphics.Transform = save;
			graphics.LineWidth = initialWidth;

			// Dessine le cadre.
			if ( this.isEditable )
			{
				Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
				rect.Inflate(-0.5, -0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(this.colorBlack);
			}
		}


		// Génère un événement pour dire qu'il faut changer les panneaux.
		protected virtual void OnPanelChanged()
		{
			if ( this.PanelChanged != null )  // qq'un écoute ?
			{
				this.PanelChanged(this);
			}
		}

		public event EventHandler PanelChanged;


		// Génère un événement pour dire qu'il faut changer la barre d'outil.
		protected virtual void OnToolChanged()
		{
			if ( this.ToolChanged != null )  // qq'un écoute ?
			{
				this.ToolChanged(this);
			}
		}

		public event EventHandler ToolChanged;

		// Génère un événement pour dire qu'il faut changer l'outil et les panneaux.
		protected virtual void OnAllChanged()
		{
			if ( this.AllChanged != null )  // qq'un écoute ?
			{
				this.AllChanged(this);
			}
		}

		public event EventHandler AllChanged;

		// Génère un événement pour dire qu'il faut changer les ascenseurs.
		protected virtual void OnScrollerChanged()
		{
			if ( this.ScrollerChanged != null )  // qq'un écoute ?
			{
				this.ScrollerChanged(this);
			}
		}

		public event EventHandler ScrollerChanged;

		// Génère un événement pour dire qu'il faut changer les informations.
		protected virtual void OnInfoObjectChanged()
		{
			if ( this.InfoObjectChanged != null )  // qq'un écoute ?
			{
				this.InfoObjectChanged(this);
			}
		}

		public event EventHandler InfoObjectChanged;

		// Génère un événement pour dire qu'il faut changer les informations.
		protected virtual void OnInfoMouseChanged()
		{
			if ( this.InfoMouseChanged != null )  // qq'un écoute ?
			{
				this.InfoMouseChanged(this);
			}
		}

		public event EventHandler InfoMouseChanged;

		// Génère un événement pour dire qu'il faut changer les informations.
		protected virtual void OnInfoZoomChanged()
		{
			if ( this.InfoZoomChanged != null )  // qq'un écoute ?
			{
				this.InfoZoomChanged(this);
			}
		}

		public event EventHandler InfoZoomChanged;


		protected bool				isActive = true;
		protected bool				isEditable = false;
		protected string			selectedTool;
		protected Drawing.Point		mousePos;
		protected bool				mouseDown = false;
		protected int				createRank = -1;
		protected AbstractObject	moveObject;
		protected int				moveHandle;
		protected Drawing.Point		moveStart;
		protected Drawing.Point		moveOffset;
		protected bool				selectRect;
		protected Drawing.Point		selectRectP1;
		protected Drawing.Point		selectRectP2;
		protected bool				gridShow = false;
		protected Drawing.Point		gridStep = new Drawing.Point(1, 1);
		protected int				rankLastCreated = -1;

		protected Drawing.Color		colorBlack;
		protected Drawing.Color		colorWindow;
		protected Drawing.Color		colorControl;

		protected Window			contextMenuWindow;
		protected VMenu				contextMenu;
		protected AbstractObject	contextMenuObject;
		protected Drawing.Point		contextMenuPos;
		protected int				contextMenuRank;

		protected IconObjects		iconObjects;
		protected IconContext		iconContext;
		protected AbstractObject	objectMemory;
		protected AbstractObject	newObject;
		protected Drawer			link = null;
		protected System.Collections.ArrayList	clones = new System.Collections.ArrayList();

		protected int				undoIndex = 0;
		protected System.Collections.ArrayList	undoList = new System.Collections.ArrayList();
		protected ZoomHistory		zoomHistory = new ZoomHistory();
	}
}
