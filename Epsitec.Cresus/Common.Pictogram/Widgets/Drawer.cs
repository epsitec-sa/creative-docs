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

			this.objects = new IconObjects();
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


		public override void SetEnabled(bool enabled)
		{
			base.SetEnabled(enabled);
			this.iconContext.IsEnable = this.IsEnabled;
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

		// Liste des objets.
		public IconObjects IconObjects
		{
			get
			{
				return this.objects;
			}

			set
			{
				this.objects = value;
			}
		}

		// Liste des objets.
		public System.Collections.ArrayList Objects
		{
			get
			{
				return this.objects.Objects;
			}

			set
			{
				this.objects.Objects = value;
			}
		}

		// Ajoute un widget SampleButton qui représente la même icone.
		public void AddClone(Widget widget)
		{
			this.clones.Add(widget);
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
					this.selectedTool = value;
					this.Select(null, false);  // désélectionne tout
					this.InvalidateAll();

					if ( this.selectedTool == "select" )
					{
						if ( this.rankLastCreated != -1 )
						{
							this.objects[this.rankLastCreated].SelectObject();
							this.rankLastCreated = -1;
						}

						this.MouseCursor = MouseCursor.AsArrow;
					}
					else
					{
						this.MouseCursor = MouseCursor.AsCross;
					}
				}
			}
		}


		// Cherche une propriété d'un type donné dans une liste.
		protected AbstractProperty PropertySearch(System.Collections.ArrayList list, PropertyType type)
		{
			foreach ( AbstractProperty property in list )
			{
				if ( property.Type == type )  return property;
			}
			return null;
		}

		// Retourne la liste des propriétés en fonction de l'objet sélectionné.
		// Un type de propriété donné n'est qu'une fois dans la liste.
		public System.Collections.ArrayList PropertiesList()
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList();

			if ( this.selectedTool == "select" )
			{
				for ( int index=0 ; index<this.objects.Count ; index++ )
				{
					AbstractObject obj = this.objects[index];
					if ( !obj.IsSelected() )  continue;

					obj.CloneInfoProperties(this.objectMemory);

					int total = obj.TotalProperty;
					for ( int i=0 ; i<total ; i++ )
					{
						AbstractProperty property = obj.Property(i);
						AbstractProperty existing = this.PropertySearch(list, property.Type);
						if ( existing == null )
						{
							property.Multi = false;
							list.Add(property);
						}
						else
						{
							if ( !property.Compare(existing) )
							{
								existing.Multi = true;
							}
						}
					}
				}
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
			if ( this.selectedTool == "select" )
			{
				for ( int index=0 ; index<this.objects.Count ; index++ )
				{
					AbstractObject obj = this.objects[index];
					if ( !obj.IsSelected() )  continue;
					obj.SetPropertyExtended(property);
				}
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
			if ( this.selectedTool == "select" )
			{
				bool first = true;
				for ( int index=0 ; index<this.objects.Count ; index++ )
				{
					AbstractObject obj = this.objects[index];
					if ( !obj.IsSelected() )  continue;
					if ( first )
					{
						this.UndoMemorize("property", obj, property.Type);
						first = false;
					}
					obj.SetProperty(property);
				}
				this.InvalidateAll();
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
			if ( this.selectedTool == "select" )
			{
				for ( int index=0 ; index<this.objects.Count ; index++ )
				{
					AbstractObject obj = this.objects[index];
					if ( !obj.IsSelected() )  continue;
					AbstractProperty property = obj.GetProperty(type);
					if ( property != null )
					{
						this.objectMemory.SetProperty(property);  // mémorise l'état
						return property;
					}
				}
			}
			else
			{
				return this.newObject.GetProperty(type);
			}
			return null;
		}


		// Indique si une commande est enable.
		public bool IsCommandEnable(string cmd)
		{
			bool enable = true;

			switch ( cmd )
			{
				case "save":
					enable = ( this.objects.Count > 0 );
					break;

				case "delete":
				case "duplicate":
				case "orderup":
				case "orderdown":
					enable = ( this.TotalSelected() > 0 );
					break;

				case "undo":
					enable = ( this.undoIndex > 0 );
					break;

				case "redo":
					enable = ( this.undoIndex < this.undoList.Count-1 );
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
				case "grid":
					active = this.gridShow;
					break;

				case "mode":
					active = !this.isActive;
					break;

				default:
					active = ( cmd == this.selectedTool );
					break;
			}

			return active;
		}

		// Ouvre une nouvelle icône.
		public void ActionOpen(string filename)
		{
			if ( filename == "" )  return;
			this.UndoFlush();
			this.objects.Read(filename);
			this.InvalidateAll();
			this.OnToolChanged();
		}

		// Sauve l'icône.
		public void ActionSave(string filename)
		{
			if ( filename == "" )  return;
			if ( this.objects.Count == 0 )  return;
			this.objects.Write(filename);
		}

		// Détruit tous les objets.
		public void ActionNew()
		{
			this.UndoFlush();
			this.objects.Clear();
			this.rankLastCreated = -1;
			this.OnPanelChanged();
			this.InvalidateAll();
			this.OnToolChanged();
		}

		// Détruit les objets sélectionnés.
		public void ActionDelete()
		{
			this.UndoMemorize("delete");
			this.DeleteSelection();
			this.OnPanelChanged();
			this.OnToolChanged();
		}

		// Duplique les objets sélectionnés.
		public void ActionDuplicate()
		{
			this.UndoMemorize("duplicate");
			this.DuplicateSelection(new Drawing.Point(1, 1));
			this.OnToolChanged();
		}

		// Annule la dernière action.
		public void ActionUndo()
		{
			if ( !this.UndoRestore() )  return;
			this.InvalidateAll();
			this.OnToolChanged();
		}

		// Refait la dernière action annulée.
		public void ActionRedo()
		{
			if ( !this.RedoRestore() )  return;
			this.InvalidateAll();
			this.OnToolChanged();
		}

		// Modifie l'ordre des objets sélectionnés.
		public void ActionOrder(int dir)
		{
			this.UndoMemorize("order");
			this.OrderSelection(dir);
			this.OnToolChanged();
		}

		// Active ou désactive la grille.
		public void ActionGrid()
		{
			this.gridShow = !this.gridShow;
			this.Invalidate();
			this.OnToolChanged();
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


		// Gestion d'un événement.
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			if ( !this.isEditable )  return;

			pos.X = pos.X/this.iconContext.ScaleX;
			pos.Y = pos.Y/this.iconContext.ScaleY;

			switch ( message.Type )
			{
				case MessageType.MouseDown:
					if ( this.selectedTool == "select" )
					{
						this.SelectMouseDown(pos, message.IsShiftPressed, message.IsCtrlPressed);
					}
					else
					{
						if ( message.IsLeftButton )
						{
							this.CreateMouseDown(pos, message.IsShiftPressed, message.IsCtrlPressed);
						}
						if ( message.IsRightButton )
						{
							this.rankLastCreated = -1;
							this.SelectedTool = "select";
							this.OnAllChanged();
							this.SelectMouseDown(pos, message.IsShiftPressed, message.IsCtrlPressed);
						}
					}
					this.mouseDown = true;
					break;
				
				case MessageType.MouseMove:
					if ( this.selectedTool == "select" )
					{
						this.SelectMouseMove(pos, message.IsShiftPressed, message.IsCtrlPressed);
					}
					else
					{
						if ( !message.IsRightButton )
						{
							this.CreateMouseMove(pos, message.IsShiftPressed, message.IsCtrlPressed);
						}
					}
					break;

				case MessageType.MouseUp:
					if ( this.mouseDown )
					{
						if ( this.selectedTool == "select" )
						{
							this.SelectMouseUp(pos, message.IsShiftPressed, message.IsCtrlPressed, message.IsRightButton);
						}
						else
						{
							if ( message.IsLeftButton )
							{
								this.CreateMouseUp(pos, message.IsShiftPressed, message.IsCtrlPressed);
							}
						}
						this.mouseDown = false;
					}
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
						this.DeleteSelection();
						this.OnPanelChanged();
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
		protected void ContextMenu(Drawing.Point mouse)
		{
			int nbSel = this.TotalSelected();
			if ( nbSel == 0 )  return;

			System.Collections.ArrayList list = new System.Collections.ArrayList();

			this.contextMenuPos = mouse;
			this.DetectHandle(mouse, out this.contextMenuObject, out this.contextMenuRank);
			if ( nbSel == 1 && this.contextMenuObject == null )
			{
				this.contextMenuObject = this.RetFirstSelected();
				this.contextMenuRank = -1;
			}

			ContextMenuItem item;

			item = new ContextMenuItem();
			item.Name = "delete";
			item.Icon = @"file:images/delete1.icon";
			item.Text = "Supprimer";
			list.Add(item);

			item = new ContextMenuItem();
			item.Name = "duplicate";
			item.Icon = @"file:images/duplicate1.icon";
			item.Text = "Dupliquer";
			list.Add(item);

			item = new ContextMenuItem();
			item.Name = "orderup";
			item.Icon = @"file:images/orderup1.icon";
			item.Text = "Devant";
			list.Add(item);

			item = new ContextMenuItem();
			item.Name = "orderdown";
			item.Icon = @"file:images/orderdown1.icon";
			item.Text = "Derriere";
			list.Add(item);

			if ( nbSel == 1 && this.contextMenuObject != null )
			{
				this.contextMenuObject.ContextMenu(list, mouse, this.contextMenuRank);
			}

			this.contextMenu = new VMenu();
			foreach ( ContextMenuItem cmi in list )
			{
				if ( cmi.Name == "" )
				{
					this.contextMenu.Items.Add(new MenuSeparator());
				}
				else
				{
					MenuItem mi = new MenuItem(cmi.Name, cmi.Icon, cmi.Text, "");
					mi.Pressed += new MessageEventHandler(this.MenuPressed);
					this.contextMenu.Items.Add(mi);
				}
			}
			this.contextMenu.AdjustSize();
			mouse.X *= this.iconContext.ScaleX;
			mouse.Y *= this.iconContext.ScaleY;
			mouse = this.MapClientToScreen(mouse);
			this.contextMenu.ShowContextMenu(mouse);
		}

		private void MenuPressed(object sender, MessageEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			this.ContextCommand(item.Name);
		}

		protected void ContextCommand(string cmd)
		{
			if ( cmd == "delete" )
			{
				this.DeleteSelection();
				this.OnPanelChanged();
			}
			else if ( cmd == "duplicate" )
			{
				this.DuplicateSelection(new Drawing.Point(10, 10));
			}
			else if ( cmd == "orderup" )
			{
				this.OrderSelection(1);
			}
			else if ( cmd == "orderdown" )
			{
				this.OrderSelection(-1);
			}
			else if ( this.contextMenuObject != null )
			{
				this.UndoMemorize("object");
				this.contextMenuObject.ContextCommand(cmd, this.contextMenuPos, this.contextMenuRank);
				this.Invalidate();
			}
		}


		protected void SelectMouseDown(Drawing.Point mouse, bool isShift, bool isCtrl)
		{
			this.UndoMemorize("select");
			this.moveStart = mouse;
			this.iconContext.IsCtrl = isCtrl;
			this.iconContext.ConstrainFixStarting(mouse);
			this.Hilite(null);
			this.moveObject = null;
			this.selectRect = false;

			AbstractObject obj;
			int rank;
			if ( this.DetectHandle(mouse, out obj, out rank) )
			{
				this.moveObject = obj;
				this.moveHandle = rank;
				this.moveOffset = mouse-obj.Handle(rank).Position;
			}
			else
			{
				obj = this.Detect(mouse);
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
					}
					else
					{
						if ( isCtrl )
						{
							obj.DeselectObject();
							this.OnPanelChanged();
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
			this.iconContext.IsCtrl = isCtrl;
			if ( this.mouseDown )
			{
				this.iconContext.ConstrainSnapPos(ref mouse);

				if ( this.selectRect )
				{
					this.selectRectP2 = mouse;
				}
				else if ( this.moveObject != null )
				{
					if ( this.moveHandle == -1 )  // déplace tout l'objet ?
					{
						double len = Drawing.Point.Distance(mouse, this.moveStart);
						if ( len <= this.iconContext.MinimalSize )
						{
							mouse = this.moveStart;
						}
						this.SnapGrid(ref mouse);
						this.MoveSelection(mouse-this.moveOffset);
						this.moveOffset = mouse;
					}
					else	// déplace une poignée ?
					{
						mouse -= this.moveOffset;
						this.SnapGrid(ref mouse);
						this.moveObject.MoveHandle(this.moveHandle, mouse);
					}
				}
			}
			else
			{
				AbstractObject obj;
				int rank;
				if ( this.DetectHandle(mouse, out obj, out rank) )
				{
					this.Hilite(null);
					this.MouseCursor = MouseCursor.AsCross;
				}
				else
				{
					obj = this.Detect(mouse);
					this.Hilite(obj);
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
				this.Window.MouseCursor = this.MouseCursor;
			}

			this.InvalidateAll();
		}

		protected void SelectMouseUp(Drawing.Point mouse, bool isShift, bool isCtrl, bool isRight)
		{
			this.iconContext.IsCtrl = isCtrl;
			if ( this.selectRect )
			{
				Drawing.Rectangle rSelect = new Drawing.Rectangle();
				rSelect.Left   = this.selectRectP1.X;
				rSelect.Right  = this.selectRectP2.X;
				rSelect.Bottom = this.selectRectP1.Y;
				rSelect.Top    = this.selectRectP2.Y;
				rSelect.Normalise();
				this.Select(rSelect, isCtrl);  // sélectionne les objets dans le rectangle
				this.selectRect = false;
				this.UndoMemorizeRemove();
			}

			if ( this.moveObject != null )
			{
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
				double len = Drawing.Point.Distance(mouse, this.moveStart);
				if ( len <= this.iconContext.MinimalSize )
				{
					this.ContextMenu(mouse);
				}
			}

			this.OnToolChanged();
		}

		// Détecte l'objet pointé par la souris.
		protected AbstractObject Detect(Drawing.Point mouse)
		{
			int total = this.objects.Count;
			for ( int i=total-1 ; i>=0 ; i-- )
			{
				AbstractObject obj = this.objects[i];
				if ( obj.Detect(mouse) )  return obj;
			}
			return null;
		}

		// Détecte la poignée pointée par la souris.
		protected bool DetectHandle(Drawing.Point mouse, out AbstractObject obj, out int rank)
		{
			int total = this.objects.Count;
			for ( int i=total-1 ; i>=0 ; i-- )
			{
				obj = this.objects[i];
				if ( !obj.IsSelected() )  continue;

				rank = obj.DetectHandle(mouse);
				if ( rank != -1 )  return true;
			}

			obj = null;
			rank = -1;
			return false;
		}

		// Hilite un objet.
		protected void Hilite(AbstractObject obj)
		{
			for ( int index=0 ; index<this.objects.Count ; index++ )
			{
				AbstractObject ob = this.objects[index];
				ob.IsHilite = (ob == obj);
			}
		}

		// Retourne le premier objet sélectionné.
		protected AbstractObject RetFirstSelected()
		{
			for ( int index=0 ; index<this.objects.Count ; index++ )
			{
				AbstractObject obj = this.objects[index];
				if ( obj.IsSelected() )  return obj;
			}
			return null;
		}

		// Retourne le nombre d'objets sélectionnés.
		protected int TotalSelected()
		{
			int total = 0;
			for ( int index=0 ; index<this.objects.Count ; index++ )
			{
				AbstractObject obj = this.objects[index];
				if ( obj.IsSelected() )  total ++;
			}
			return total;
		}

		// Sélectionne un objet et désélectionne tous les autres.
		protected void Select(AbstractObject obj, bool add)
		{
			for ( int index=0 ; index<this.objects.Count ; index++ )
			{
				AbstractObject ob = this.objects[index];
				if ( ob == obj )
				{
					ob.SelectObject();
				}
				else
				{
					if ( !add )  ob.DeselectObject();
				}
			}
			this.OnPanelChanged();
		}

		// Sélectionne tous les objets dans le rectangle.
		protected void Select(Drawing.Rectangle rect, bool add)
		{
			for ( int index=0 ; index<this.objects.Count ; index++ )
			{
				AbstractObject obj = this.objects[index];
				if ( obj.Detect(rect) )
				{
					obj.SelectObject();
				}
				else
				{
					if ( !add )  obj.DeselectObject();
				}
			}
			this.OnPanelChanged();
		}

		// Détruit tous les objets sélectionnés.
		protected void DeleteSelection()
		{
			bool bDo = false;
			do
			{
				bDo = false;
				int total = this.objects.Count;
				for ( int i=0 ; i<total ; i++ )
				{
					AbstractObject obj = this.objects[i];
					if ( obj.IsSelected() )
					{
						this.objects.RemoveAt(i);
						bDo = true;
						break;
					}
				}
			}
			while ( bDo );

			this.InvalidateAll();
		}

		// Duplique tous les objets sélectionnés.
		protected void DuplicateSelection(Drawing.Point move)
		{
			int total = this.objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = this.objects[index];
				if ( obj.IsSelected() )
				{
					AbstractObject newObject = null;
					if ( !obj.DuplicateObject(ref newObject) )  continue;
					newObject.MoveAll(move);
					this.objects.Add(newObject);

					obj.DeselectObject();
				}
			}
			this.InvalidateAll();
		}

		// Change l'ordre de tous les objets sélectionnés.
		protected void OrderSelection(int dir)
		{
			System.Collections.ArrayList extract = new System.Collections.ArrayList();

			// Extrait tous les objets sélectionnés dans la liste extract.
			for ( int index=0 ; index<this.objects.Count ; index++ )
			{
				AbstractObject obj = this.objects[index];
				if ( obj.IsSelected() )
				{
					extract.Add(obj);
				}
			}

			// Supprime les objets sélectionnés de la liste principale.
			this.DeleteSelection();

			// Remet les objets extraits au début ou à la fin de la liste principale.
			int i = 0;
			if ( dir > 0 )  i = this.objects.Count;
			foreach ( AbstractObject obj in extract )
			{
				this.objects.Insert(i++, obj);
			}

			this.InvalidateAll();
		}

		// Déplace tous les objets sélectionnés.
		protected void MoveSelection(Drawing.Point move)
		{
			for ( int index=0 ; index<this.objects.Count ; index++ )
			{
				AbstractObject obj = this.objects[index];
				if ( obj.IsSelected() )
				{
					obj.MoveAll(move);
				}
			}
		}


		protected void CreateMouseDown(Drawing.Point mouse, bool isShift, bool isCtrl)
		{
			this.iconContext.IsCtrl = isCtrl;
			this.SnapGrid(ref mouse);

			if ( this.createRank == -1 )
			{
				this.UndoMemorize("create");
				AbstractObject obj = this.CreateObject();
				if ( obj == null )  return;
				obj.CloneProperties(this.newObject);
				this.objects.Add(obj);
				this.createRank = this.objects.Count-1;
			}

			this.objects[this.createRank].CreateMouseDown(mouse, this.iconContext);
			this.InvalidateAll();
		}

		protected void CreateMouseMove(Drawing.Point mouse, bool isShift, bool isCtrl)
		{
			this.iconContext.IsCtrl = isCtrl;
			this.SnapGrid(ref mouse);

			if ( this.createRank == -1 )  return;
			this.objects[this.createRank].CreateMouseMove(mouse, this.iconContext);
			this.InvalidateAll();
		}

		protected void CreateMouseUp(Drawing.Point mouse, bool isShift, bool isCtrl)
		{
			this.iconContext.IsCtrl = isCtrl;
			this.SnapGrid(ref mouse);

			if ( this.createRank == -1 )  return;
			this.objects[this.createRank].CreateMouseUp(mouse, this.iconContext);

			if ( this.objects[this.createRank].CreateIsEnding(this.iconContext) )
			{
				if ( this.objects[this.createRank].CreateIsExist(this.iconContext) )
				{
					this.rankLastCreated = this.createRank;
				}
				else
				{
					this.objects.RemoveAt(this.createRank);
					this.UndoMemorizeRemove();
				}
				this.createRank = -1;
			}
			this.InvalidateAll();
			this.OnToolChanged();
		}

		protected void CreateEnding()
		{
			if ( this.createRank == -1 )  return;
			if ( this.objects[this.createRank].CreateEnding(this.iconContext) )
			{
				this.rankLastCreated = this.createRank;
			}
			else
			{
				this.objects.RemoveAt(this.createRank);
				this.UndoMemorizeRemove();
			}
			this.createRank = -1;
			this.InvalidateAll();
		}


		// Vide la liste des annulations.
		protected void UndoFlush()
		{
			this.undoList.Clear();
			this.undoIndex = 0;
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
			this.objects.CopyTo(io);
			situation.Objects = io;

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
				this.UndoMemorize("undo");
				this.undoIndex --;
			}

			UndoSituation last = this.undoList[--this.undoIndex] as UndoSituation;
			last.Objects.CopyTo(this.objects);
			this.selectedTool = last.SelectedTool;
			this.OnAllChanged();
			this.rankLastCreated = -1;
			return true;
		}

		// Annule la dernière annulation.
		protected bool RedoRestore()
		{
			if ( this.undoIndex >= this.undoList.Count-1 )  return false;

			UndoSituation last = this.undoList[++this.undoIndex] as UndoSituation;
			last.Objects.CopyTo(this.objects);
			this.selectedTool = last.SelectedTool;
			this.OnAllChanged();
			return true;
		}


		// Invalide la zone ainsi que celles des clones.
		protected void InvalidateAll()
		{
			this.Invalidate();

			foreach ( Widget widget in this.clones )
			{
				SampleButton sample = widget as SampleButton;
				if ( sample != null )
				{
					sample.IconObjects.Size = this.objects.Size;
					sample.IconObjects.Origin = this.objects.Origin;
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

		// Dessine la grille magnétique.
		protected void DrawGrid(Drawing.Graphics graphics)
		{
			double initialWidth = graphics.LineWidth;
			graphics.LineWidth = 1.0/this.iconContext.ScaleX;

			double ix = 0.5/this.iconContext.ScaleX;
			double iy = 0.5/this.iconContext.ScaleY;

			if ( this.gridShow )
			{
				double step = this.gridStep.X;
				for ( double pos=step ; pos<this.objects.Size.Width ; pos+=step )
				{
					double x = pos;
					double y = 0;
					graphics.Align(ref x, ref y);
					x += ix;
					y += iy;
					graphics.AddLine(x, 0, x, this.objects.Size.Height);
				}
				step = this.gridStep.Y;
				for ( double pos=step ; pos<this.objects.Size.Height ; pos+=step )
				{
					double x = 0;
					double y = pos;
					graphics.Align(ref x, ref y);
					x += ix;
					y += iy;
					graphics.AddLine(0, y, this.objects.Size.Width, y);
				}
				graphics.RenderSolid(Drawing.Color.FromBrightness(0.9));
			}

			Drawing.Rectangle rect = new Drawing.Rectangle(2, 2, this.objects.Size.Width-4, this.objects.Size.Height-4);
			graphics.Align(ref rect);
			rect.Offset(ix, iy);
			graphics.AddRectangle(rect);

			double cx = this.objects.Size.Width/2;
			double cy = this.objects.Size.Height/2;
			graphics.Align(ref cx, ref cy);
			cx += ix;
			cy += iy;
			graphics.AddLine(cx, 0, cx, this.objects.Size.Height);
			graphics.AddLine(0, cy, this.objects.Size.Width, cy);
			graphics.RenderSolid(Drawing.Color.FromBrightness(1.0));

			graphics.LineWidth = initialWidth;
		}

		// Dessine les contraintes.
		protected void DrawConstrain(Drawing.Graphics graphics, Drawing.Point pos, ConstrainType type)
		{
			graphics.LineWidth = 1.0/this.iconContext.ScaleX;

			if ( type == ConstrainType.Normal || type == ConstrainType.Line )
			{
				graphics.AddLine(pos.X, 0, pos.X, this.objects.Size.Height);
				graphics.AddLine(0, pos.Y, this.objects.Size.Width, pos.Y);
				graphics.RenderSolid(Drawing.Color.FromARGB(0.5, 1,0,0));
			}

			if ( type == ConstrainType.Normal || type == ConstrainType.Square )
			{
				Drawing.Point p1 = Drawing.Transform.RotatePoint(pos, System.Math.PI*0.25, pos+new Drawing.Point(this.objects.Size.Width,0));
				Drawing.Point p2 = Drawing.Transform.RotatePoint(pos, System.Math.PI*1.25, pos+new Drawing.Point(this.objects.Size.Width,0));
				graphics.AddLine(p1, p2);

				p1 = Drawing.Transform.RotatePoint(pos, System.Math.PI*0.75, pos+new Drawing.Point(this.objects.Size.Width,0));
				p2 = Drawing.Transform.RotatePoint(pos, System.Math.PI*1.75, pos+new Drawing.Point(this.objects.Size.Width,0));
				graphics.AddLine(p1, p2);

				graphics.RenderSolid(Drawing.Color.FromARGB(0.5, 1,0,0));
			}
		}

		// Dessine l'icône.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			double initialWidth = graphics.LineWidth;
			Drawing.Transform save = graphics.SaveTransform();
			this.iconContext.ScaleX = this.Client.Width/this.objects.Size.Width;
			this.iconContext.ScaleY = this.Client.Height/this.objects.Size.Height;
			graphics.ScaleTransform(this.iconContext.ScaleX, this.iconContext.ScaleY, 0, 0);

			// Dessine la grille magnétique.
			if ( this.isEditable )
			{
				this.DrawGrid(graphics);
			}

			// Dessine les géométries.
			this.objects.DrawGeometry(graphics, this.iconContext);

			// Dessine les poignées.
			if ( this.isEditable )
			{
				this.objects.DrawHandle(graphics, this.iconContext);
			}

			// Dessine le rectangle de sélection.
			if ( this.isEditable && this.selectRect )
			{
				Drawing.Rectangle rSelect = new Drawing.Rectangle();
				rSelect.Left   = this.selectRectP1.X;
				rSelect.Right  = this.selectRectP2.X;
				rSelect.Bottom = this.selectRectP1.Y;
				rSelect.Top    = this.selectRectP2.Y;
				rSelect.Normalise();

				graphics.LineWidth = 1.0/this.iconContext.ScaleX;
				rSelect.Inflate(-0.5/this.iconContext.ScaleX, -0.5/this.iconContext.ScaleY);
				graphics.AddRectangle(rSelect);
				graphics.RenderSolid(Drawing.Color.FromBrightness(0.6));
			}

			// Dessine les contraintes.
			Drawing.Point pos;
			ConstrainType type;
			if ( this.isEditable && this.iconContext.ConstrainGetStarting(out pos, out type) )
			{
				this.DrawConstrain(graphics, pos, type);
			}

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


		protected bool				isActive = true;
		protected bool				isEditable = false;
		protected string			selectedTool;
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

		protected IconObjects		objects;
		protected IconContext		iconContext;
		protected AbstractObject	objectMemory;
		protected AbstractObject	newObject;
		protected Drawer			link = null;
		protected System.Collections.ArrayList	clones = new System.Collections.ArrayList();

		protected int				undoIndex = 0;
		protected System.Collections.ArrayList	undoList = new System.Collections.ArrayList();
	}
}
