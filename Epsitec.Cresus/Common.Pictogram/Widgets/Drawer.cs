using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe Drawer permet de repr�senter vectoriellement des ic�nes.
	/// </summary>
	
	[SuppressBundleSupport]
	
	public class Drawer : Epsitec.Common.Widgets.Widget
	{
		public Drawer()
		{
			this.InternalState |= InternalState.AutoFocus;
			this.InternalState |= InternalState.Focusable;
			this.InternalState |= InternalState.AutoDoubleClick;

			this.BackColor = Drawing.Color.FromBrightness(1);  // fond blanc

			this.iconObjects = new IconObjects();
			this.iconObjects.Drawer = this;
			this.iconObjects.Clear();

			this.iconContext = new IconContext();
			this.objectMemory = new ObjectMemory();

			this.iconPrinter = new IconPrinter();
			this.iconPrinter.IconObjects = this.iconObjects;

			this.textRuler = new TextRuler(this);
			this.textRuler.Changed += new EventHandler(this.HandleRulerChanged);

			this.Exited += new MessageEventHandler(this.HandleMouseExited);
		}
		
		public Drawer(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public override double DefaultWidth
		{
			//	Retourne la largeur standard d'une ic�ne.
			get
			{
				return 22;
			}
		}

		public override double DefaultHeight
		{
			//	Retourne la hauteur standard d'une ic�ne.
			get
			{
				return 22;
			}
		}


		public bool IsIconActive
		{
			//	Indique si l'ic�ne est active.
			get { return this.isActive; }
			set { this.isActive = value; }
		}

		public bool IsIconEditable
		{
			//	Indique si l'ic�ne est �ditable.
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

		public double Zoom
		{
			//	Facteur de zoom de la loupe.
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
					this.OnScrollerChanged();
					this.OnInfoZoomChanged();
					this.OnCommandChanged();
					this.InvalidateDraw();
				}
			}
		}

		public double OriginX
		{
			//	Origine X selon l'ascenseur horizontal.
			get
			{
				return this.iconContext.OriginX;
			}

			set
			{
				if ( this.iconContext.OriginX != value )
				{
					this.iconContext.OriginX = value;
					this.OnScrollerChanged();
					this.OnCommandChanged();
					this.InvalidateDraw();
				}
			}
		}

		public double OriginY
		{
			//	Origine Y selon l'ascenseur vertical.
			get
			{
				return this.iconContext.OriginY;
			}

			set
			{
				if ( this.iconContext.OriginY != value )
				{
					this.iconContext.OriginY = value;
					this.OnScrollerChanged();
					this.OnCommandChanged();
					this.InvalidateDraw();
				}
			}
		}

		public IconObjects IconObjects
		{
			//	Objet global.
			get { return this.iconObjects; }
			set { this.iconObjects = value; }
		}

		public GlobalModifier GlobalModifier
		{
			//	Modificateur global.
			get { return this.globalModifier; }
		}

		public UndoList Objects
		{
			//	Liste des objets.
			get { return this.iconObjects.Objects; }
			set { this.iconObjects.Objects = value; }
		}

		public AbstractObject EditObject
		{
			//	Objet en cours d'�dition.
			get { return this.editObject; }
		}

		public void SwapEditObject(ref AbstractObject obj)
		{
			//	Permute l'objet en cours d'�dition (pour undo).
			AbstractObject temp = this.editObject;
			this.editObject = obj;
			obj = temp;
		}

		public void AddClone(Widget widget)
		{
			//	Ajoute un widget SampleButton qui repr�sente la m�me icone.
			this.clones.Add(widget);
		}

		public string TextInfoObject
		{
			//	Texte pour les informations.
			get
			{
				string text;
				text = string.Format("S�lection: {0}/{1}", this.iconObjects.TotalSelected(), this.iconObjects.Count);
				return text;
			}
		}

		public string TextInfoMouse
		{
			//	Texte pour les informations.
			get
			{
				string text;
				text = string.Format("(x:{0} y:{1})", mousePos.X.ToString("F2"), mousePos.Y.ToString("F2"));
				return text;
			}
		}

		public string TextInfoZoom
		{
			//	Texte pour les informations.
			get
			{
				string text;
				text = string.Format("Zoom {0}%", (this.iconContext.Zoom*100).ToString("F0"));
				return text;
			}
		}

		public AbstractObject CreatingObject
		{
			//	Indique quel est l'objet en cours de cr�ation.
			get
			{
				if ( this.createRank == -1 )  return null;
				return this.iconObjects[this.createRank];
			}
		}

		public void PageOrLayerChanged()
		{
			//	Appel� lorsqu'on change de page ou de calque.
			this.rankLastCreated = -1;
			this.hiliteHandleObject = null;
			this.editObject = null;
			this.iconObjects.DeselectAll();
			this.iconObjects.UpdateEditProperties(this.objectMemory);
			this.globalModifier.Visible = false;
		}

		public string SelectedTool
		{
			//	Nom de l'outil s�lectionn�.
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
							this.UndoBeginning("Select");
							this.UndoSelectionWillBeChanged();
							int nb = this.Select(this.iconObjects[this.rankLastCreated], false, false);
							this.UndoValidate(nb > 0);

							this.iconObjects.UpdateEditProperties(this.objectMemory);
							this.rankLastCreated = -1;
							this.InvalidateAll();
							this.OnInfoObjectChanged();
						}

						this.MouseTool();

						if ( this.IsTool() && this.selectedTool != "Edit" )
						{
							if ( this.editObject != null )
							{
								this.editObject.Select(true);
								this.editObject = null;
								this.OnPanelChanged();
								this.OnInfoObjectChanged();
								this.InvalidateAll();
							}
						}

						if ( this.selectedTool == "Edit" )
						{
							int total = this.iconObjects.TotalSelected();
							if ( total == 1 )
							{
								AbstractObject sel = this.iconObjects.RetFirstSelected();
								if ( sel != null )
								{
									if ( sel.IsEditable() )
									{
										sel.Select(true, true);
										this.editObject = sel;
									}
									else
									{
										this.DeselectAll();
										this.editObject = null;
									}
									this.OnPanelChanged();
									this.OnInfoObjectChanged();
									this.InvalidateAll();
								}
							}
							else if ( total > 1 )
							{
								this.DeselectAll();
								this.editObject = null;
								this.OnPanelChanged();
								this.OnInfoObjectChanged();
								this.InvalidateAll();
							}
						}
					}
					else
					{
						this.UndoBeginning("Select");
						this.UndoSelectionWillBeChanged();
						int nb = this.Select(null, false, false);  // d�s�lectionne tout
						this.UndoValidate(nb > 0);

						this.editObject = null;
						this.InvalidateAll();
						this.OnInfoObjectChanged();
						this.MouseCursor = MouseCursor.AsCross;
					}
				}
			}
		}

		public void SwapTool(ref string tool)
		{
			//	Permute l'outil (pour le undo).
			if ( tool == this.selectedTool )  return;

			string temp = this.selectedTool;
			this.selectedTool = tool;
			tool = temp;

			this.MouseTool();
		}

		protected void MouseTool()
		{
			//	Utilise le bon sprite de la souris, selon l'outil s�lectionn�.
			if ( this.selectedTool == "Select" )
			{
				this.MouseCursor = MouseCursor.AsArrow;
			}
			if ( this.selectedTool == "Edit" )
			{
				this.MouseCursor = MouseCursor.AsIBeam;
			}
			if ( this.selectedTool == "Zoom" )
			{
				this.MouseCursorImage(ref this.mouseCursorZoom, @"file:images/zoom.icon");
			}
			if ( this.selectedTool == "Hand" )
			{
				this.MouseCursorImage(ref this.mouseCursorHand, @"file:images/hand.icon");
			}
			if ( this.selectedTool == "Picker" )
			{
				this.MouseCursorImage(ref this.mouseCursorPicker, @"file:images/picker.icon");
			}
		}

		protected void MouseCursorImage(ref Drawing.Image image, string name)
		{
			//	Choix du sprite de la souris.
			if ( image == null )
			{
				image = Support.Resources.DefaultManager.GetImage(name);
			}
			
			this.MouseCursor = MouseCursor.FromImage(image);
		}


		private void HandleMouseExited(object sender, MessageEventArgs e)
		{
			//	Appel� lorsque la souris est sortie du widget.
			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			this.iconObjects.Hilite(null, ref bbox);
			this.HiliteHandle(null, -1, ref bbox);
			this.InvalidateAll(bbox);
		}

		private void HandleRulerChanged(object sender)
		{
			//	Appel� lorsque la r�gle est chang�e.
			if ( this.editObject == null )  return;

			Drawing.Rectangle bbox = this.editObject.BoundingBox;
			this.InvalidateAll(bbox);
		}

		protected bool IsTool()
		{
			//	Indique si l'outil s�lectionn� n'est pas un objet.
			if ( this.selectedTool == "Select" )  return true;
			if ( this.selectedTool == "Edit"   )  return true;
			if ( this.selectedTool == "Zoom"   )  return true;
			if ( this.selectedTool == "Hand"   )  return true;
			if ( this.selectedTool == "Picker" )  return true;
			return false;
		}


		public System.Collections.ArrayList PropertiesList()
		{
			//	Retourne la liste des propri�t�s en fonction du ou des objets s�lectionn�s.
			//	Un type de propri�t� donn� n'est qu'une fois dans la liste.
			System.Collections.ArrayList list = new System.Collections.ArrayList();

			if ( this.IsTool() )
			{
				this.iconObjects.PropertiesList(list);
			}
			else
			{
				this.newObject = this.CreateObject();
				this.newObject.CloneProperties(this.objectMemory);
				this.newObject.PropertiesList(list, true);
			}

			return list;
		}

		public void SetPropertyExtended(PropertyType type, bool extended)
		{
			//	M�morise l'�tat �tendu d'une propri�t�.
			this.objectMemory.SetPropertyExtended(type, extended);
			this.iconObjects.UpdateEditProperties(this.objectMemory);
			this.InvalidateAll();
		}

		public bool GetPropertyExtended(PropertyType type)
		{
			//	Retourne l'�tat �tendu d'une propri�t�.
			return this.objectMemory.GetPropertyExtended(type);
		}

		public void SetProperty(AbstractProperty property, bool changeStylesCollection)
		{
			//	Modifie une propri�t�.
			if ( this.IsTool() || property.StyleID != 0 )
			{
				if ( changeStylesCollection )
				{
					AbstractObject obj = this.iconObjects.RetFirstSelected();
					int nbSel = this.iconObjects.TotalSelected();
					this.UndoBeginning("PropertyChanged", obj, nbSel, property.Type, property.StyleID);
					this.UndoSelectionWillBeChanged();
					this.UndoValidate();
				}

				Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
				this.iconObjects.SetProperty(property, ref bbox, changeStylesCollection);
				this.iconObjects.UpdateEditProperties(this.objectMemory);
				this.InvalidateAll(bbox);
			}

			if ( this.newObject != null )
			{
				this.newObject.SetProperty(property);
			}

			this.objectMemory.SetProperty(property);  // m�morise les changements
		}

		public AbstractProperty GetProperty(PropertyType type)
		{
			//	Retourne une propri�t�.
			if ( this.IsTool() )
			{
				return this.iconObjects.GetProperty(type, this.objectMemory);
			}
			else
			{
				return this.newObject.GetProperty(type);
			}
		}

		public AbstractProperty NewProperty(PropertyType type)
		{
			//	Retourne une nouvelle propri�t�.
			return this.objectMemory.GetProperty(type);
		}

		public int StyleMake(PropertyType type)
		{
			//	Fabrique un nouveau style d'un type donn�.
			AbstractProperty property = this.NewProperty(type);
			if ( property == null )  return -1;
			int sel = this.iconObjects.StylesCollection.CreateProperty(property);
			if ( sel == -1 )  return -1;
			this.StyleUse(sel);
			return sel;
		}

		public void StyleFree(PropertyType type)
		{
			//	Lib�re les objets s�lectionn�s du style.
			if ( this.IsTool() )
			{
				this.iconObjects.StyleFree(type);
			}
			else
			{
				AbstractProperty property = this.newObject.GetProperty(type);
				property.StyleID = 0;
				property.StyleName = "";
				this.newObject.SetProperty(property);
				this.objectMemory.SetProperty(property);
			}
		}

		public void StyleFreeAll(AbstractProperty property)
		{
			//	Lib�re tous les objets du style.
			this.iconObjects.StyleFreeAll(property);

			if ( this.newObject != null &&
				this.newObject.IsLinkProperty(property) )
			{
				AbstractProperty objProp = this.newObject.GetProperty(property.Type);
				objProp.StyleID = 0;
				objProp.StyleName = "";
				this.newObject.SetProperty(objProp);
				this.objectMemory.SetProperty(objProp);
			}
		}

		public void StyleFreeAll()
		{
			//	Lib�re tous les styles.
			int total = this.objectMemory.TotalProperty;
			for ( int i=0 ; i<total ; i++ )
			{
				AbstractProperty property = this.objectMemory.Property(i);
				if ( property == null )  break;
				property.StyleID = 0;
				property.StyleName = "";
			}
			this.newObject = null;
		}

		public void StyleUse(int rank)
		{
			//	Utilise un style donn�.
			AbstractProperty property = this.iconObjects.StylesCollection.GetProperty(rank);

			if ( this.IsTool() )
			{
				this.iconObjects.StyleUse(property);
			}
			else
			{
				this.newObject.SetProperty(property);
			}

			this.objectMemory.SetProperty(property);  // m�morise les changements
		}

		public void AdaptDeletePattern(int rank)
		{
			//	Adapte tout apr�s la destruction d'un pattern.
			if ( this.newObject != null )
			{
				this.newObject.DeletePattern(rank);
			}
			this.objectMemory.DeletePattern(rank);
			this.iconObjects.UpdateDeletePattern(rank);
			this.iconObjects.StylesDeletePattern(rank);
		}


		public void InitCommands(CommandDispatcher commandDispatcher)
		{
			//	Initialise toutes les commandes.
			this.commandDispatcher = commandDispatcher;

			this.saveState = new CommandState("Save", this.commandDispatcher);
			this.deleteState = new CommandState("Delete", this.commandDispatcher);
			this.duplicateState = new CommandState("Duplicate", this.commandDispatcher);
			this.cutState = new CommandState("Cut", this.commandDispatcher);
			this.copyState = new CommandState("Copy", this.commandDispatcher);
			this.pasteState = new CommandState("Paste", this.commandDispatcher);
			this.orderUpState = new CommandState("OrderUp", this.commandDispatcher);
			this.orderDownState = new CommandState("OrderDown", this.commandDispatcher);
			this.mergeState = new CommandState("Merge", this.commandDispatcher);
			this.groupState = new CommandState("Group", this.commandDispatcher);
			this.ungroupState = new CommandState("Ungroup", this.commandDispatcher);
			this.insideState = new CommandState("Inside", this.commandDispatcher);
			this.outsideState = new CommandState("Outside", this.commandDispatcher);
			this.undoState = new CommandState("Undo", this.commandDispatcher);
			this.redoState = new CommandState("Redo", this.commandDispatcher);
			this.deselectState = new CommandState("Deselect", this.commandDispatcher);
			this.selectAllState = new CommandState("SelectAll", this.commandDispatcher);
			this.selectInvertState = new CommandState("SelectInvert", this.commandDispatcher);
			this.selectGlobalState = new CommandState("SelectGlobal", this.commandDispatcher);
			this.selectModeState = new CommandState("SelectMode", this.commandDispatcher);
			this.hideHalfState = new CommandState("HideHalf", this.commandDispatcher);
			this.hideSelState = new CommandState("HideSel", this.commandDispatcher);
			this.hideRestState = new CommandState("HideRest", this.commandDispatcher);
			this.hideCancelState = new CommandState("HideCancel", this.commandDispatcher);
			this.zoomMinState = new CommandState("ZoomMin", this.commandDispatcher);
			this.zoomDefaultState = new CommandState("ZoomDefault", this.commandDispatcher);
			this.zoomSelState = new CommandState("ZoomSel", this.commandDispatcher);
			this.zoomPrevState = new CommandState("ZoomPrev", this.commandDispatcher);
			this.zoomSubState = new CommandState("ZoomSub", this.commandDispatcher);
			this.zoomAddState = new CommandState("ZoomAdd", this.commandDispatcher);
			this.previewState = new CommandState("Preview", this.commandDispatcher);
			this.gridState = new CommandState("Grid", this.commandDispatcher);
			this.modeState = new CommandState("Mode", this.commandDispatcher);

			this.arrayOutlineFrameState = new CommandState("ArrayOutlineFrame", this.commandDispatcher);
			this.arrayOutlineHorizState = new CommandState("ArrayOutlineHoriz", this.commandDispatcher);
			this.arrayOutlineVertiState = new CommandState("ArrayOutlineVerti", this.commandDispatcher);
			this.arrayAddColumnLeftState = new CommandState("ArrayAddColumnLeft", this.commandDispatcher);
			this.arrayAddColumnRightState = new CommandState("ArrayAddColumnRight", this.commandDispatcher);
			this.arrayAddRowTopState = new CommandState("ArrayAddRowTop", this.commandDispatcher);
			this.arrayAddRowBottomState = new CommandState("ArrayAddRowBottom", this.commandDispatcher);
			this.arrayDelColumnState = new CommandState("ArrayDelColumn", this.commandDispatcher);
			this.arrayDelRowState = new CommandState("ArrayDelRow", this.commandDispatcher);
			this.arrayAlignColumnState = new CommandState("ArrayAlignColumn", this.commandDispatcher);
			this.arrayAlignRowState = new CommandState("ArrayAlignRow", this.commandDispatcher);
			this.arraySwapColumnState = new CommandState("ArraySwapColumn", this.commandDispatcher);
			this.arraySwapRowState = new CommandState("ArraySwapRow", this.commandDispatcher);
			this.arrayLookState = new CommandState("ArrayLook", this.commandDispatcher);

			this.debugBboxThinState = new CommandState("DebugBboxThin", this.commandDispatcher);
			this.debugBboxGeomState = new CommandState("DebugBboxGeom", this.commandDispatcher);
			this.debugBboxFullState = new CommandState("DebugBboxFull", this.commandDispatcher);
		}

		public void UpdateCommands()
		{
			//	Met � jour toutes les commandes (dans les menus, les barres, etc.).
			Widget[] toolWidgets = Widget.FindAllCommandWidgets("SelectTool", this.commandDispatcher);
			foreach ( Widget widget in toolWidgets )
			{
				widget.ActiveState = ( widget.Name == this.selectedTool ) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			}

			Widget[] lookWidgets = Widget.FindAllCommandWidgets("SelectLook", this.commandDispatcher);
			foreach ( Widget widget in lookWidgets )
			{
				widget.ActiveState = ( widget.Name == Epsitec.Common.Widgets.Adorner.Factory.ActiveName ) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			}

			int totalSelected = this.iconObjects.TotalSelected();
			int totalHide = this.iconObjects.RetTotalHide();
			int count = this.iconObjects.Count;
			AbstractObject first = this.iconObjects.RetFirstSelected();

			this.saveState.Enabled = ( this.iconObjects.TotalCount() > 2 && this.createRank == -1 );
			this.deleteState.Enabled = ( totalSelected > 0 );
			this.duplicateState.Enabled = ( totalSelected > 0 && this.createRank == -1 );
			this.cutState.Enabled = ( totalSelected > 0 && this.createRank == -1 );
			this.copyState.Enabled = ( totalSelected > 0 && this.createRank == -1 );
			this.pasteState.Enabled = ( !this.iconObjects.IsEmptyClipboard() && this.createRank == -1 );
			this.orderUpState.Enabled = ( count > 1 && totalSelected > 0 && this.createRank == -1 );
			this.orderDownState.Enabled = ( count > 1 && totalSelected > 0 && this.createRank == -1 );
			this.mergeState.Enabled = ( totalSelected > 1 && this.createRank == -1 );
			this.groupState.Enabled = ( totalSelected > 1 && this.createRank == -1 );
			this.ungroupState.Enabled = ( totalSelected == 1 && first is ObjectGroup && this.createRank == -1 );
			this.insideState.Enabled = ( totalSelected == 1 && first is ObjectGroup && this.createRank == -1 );
			this.outsideState.Enabled = ( !this.iconObjects.IsInitialGroup() && this.createRank == -1 );
			this.undoState.Enabled = ( IconEditor.OpletQueue.CanUndo && this.createRank == -1 );
			this.redoState.Enabled = ( IconEditor.OpletQueue.CanRedo && this.createRank == -1 );
			this.deselectState.Enabled = ( totalSelected > 0 && this.createRank == -1 );
			this.selectAllState.Enabled = ( totalSelected < count && this.createRank == -1 );
			this.selectInvertState.Enabled = ( count > 0 && this.createRank == -1 );
			this.selectGlobalState.Enabled = ( totalSelected > 0 && this.createRank == -1 );
			this.selectModeState.ActiveState = this.selectModePartial ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.selectModeState.Enabled = ( this.createRank == -1 );
			this.hideHalfState.ActiveState = this.iconContext.HideHalfActive ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.hideSelState.Enabled = ( totalSelected > 0 && this.createRank == -1 );
			this.hideRestState.Enabled = ( totalSelected > 0 && count-totalSelected-totalHide > 0 && this.createRank == -1 );
			this.hideCancelState.Enabled = ( totalHide > 0 && this.createRank == -1 );
			this.zoomMinState.Enabled = ( this.Zoom > this.ZoomMin );
			this.zoomDefaultState.Enabled = ( this.Zoom != 1 || this.OriginX != 0 || this.OriginY != 0 );
			this.zoomSelState.Enabled = ( totalSelected > 0 );
			this.zoomPrevState.Enabled = ( this.zoomHistory.Count > 0 );
			this.zoomSubState.Enabled = ( this.Zoom > this.ZoomMin );
			this.zoomAddState.Enabled = ( this.Zoom < this.ZoomMax );
			this.previewState.ActiveState = this.iconContext.PreviewActive ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.gridState.ActiveState = this.iconContext.GridActive ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.modeState.ActiveState = !this.isActive ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.modeState.Enabled = ( this.createRank == -1 );

			ObjectArray array = first as ObjectArray;
			bool enabled = ( totalSelected == 1 ) && ( array != null );
			this.arrayOutlineFrameState.Enabled = ( enabled );
			this.arrayOutlineHorizState.Enabled = ( enabled );
			this.arrayOutlineVertiState.Enabled = ( enabled );
			this.arrayOutlineFrameState.ActiveState = (enabled && array.OutlineFrame) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.arrayOutlineHorizState.ActiveState = (enabled && array.OutlineHoriz) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.arrayOutlineVertiState.ActiveState = (enabled && array.OutlineVerti) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.arrayAddColumnLeftState.Enabled = ( enabled && array.EnabledAddColumnLeft );
			this.arrayAddColumnRightState.Enabled = ( enabled && array.EnabledAddColumnRight );
			this.arrayAddRowTopState.Enabled = ( enabled && array.EnabledAddRowTop );
			this.arrayAddRowBottomState.Enabled = ( enabled && array.EnabledAddRowBottom );
			this.arrayDelColumnState.Enabled = ( enabled && array.EnabledDelColumn );
			this.arrayDelRowState.Enabled = ( enabled && array.EnabledDelRow );
			this.arrayAlignColumnState.Enabled = ( enabled && array.EnabledAlignColumn );
			this.arrayAlignRowState.Enabled = ( enabled && array.EnabledAlignRow );
			this.arraySwapColumnState.Enabled = ( enabled && array.EnabledSwapColumn );
			this.arraySwapRowState.Enabled = ( enabled && array.EnabledSwapRow );
			this.arrayLookState.Enabled = ( enabled && array.EnabledLook );

			this.debugBboxThinState.ActiveState = this.iconContext.IsDrawBoxThin ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.debugBboxGeomState.ActiveState = this.iconContext.IsDrawBoxGeom ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.debugBboxFullState.ActiveState = this.iconContext.IsDrawBoxFull ? WidgetState.ActiveYes : WidgetState.ActiveNo;
		}

		public void CommandNew()
		{
			this.UndoFlush();
			this.iconObjects.Clear();
			this.objectMemory.DeletePattern();
			this.StyleFreeAll();
			this.globalModifier.Visible = false;
			this.rankLastCreated = -1;
			this.createRank = -1;
			this.hiliteHandleObject = null;
			this.editObject = null;
			this.Zoom = 1;
			this.OriginX = 0;
			this.OriginY = 0;
			this.OnPanelChanged();
			this.OnInfoObjectChanged();
			this.OnCommandChanged();
			this.InvalidateAll();
		}

		public void CommandOpen(string filename)
		{
			if ( filename == "" )  return;
			this.UndoFlush();
			this.iconObjects.Clear();
			this.objectMemory.DeletePattern();
			this.StyleFreeAll();
			this.globalModifier.Visible = false;
			this.rankLastCreated = -1;
			this.createRank = -1;
			this.hiliteHandleObject = null;
			this.editObject = null;
			this.iconObjects.Read(filename);
			this.UpdateSizeSamples();
			this.Zoom = 1;
			this.OriginX = 0;
			this.OriginY = 0;
			this.OnPanelChanged();
			this.OnInfoObjectChanged();
			this.OnCommandChanged();
			this.InvalidateAll();
		}

		public void CommandSave(string filename)
		{
			if ( filename == "" )  return;
			if ( this.iconObjects.TotalCount() <= 2 )  return;
			this.iconObjects.Write(filename);
		}

		[Command ("Delete")]
		void CommandDelete()
		{
			if ( this.createRank == -1 )
			{
				this.UndoBeginning("Delete");
				this.iconObjects.DeleteSelection();
				this.UndoValidate();
			}
			else	// d�truit l'objet en cours de cr�ation ?
			{
				this.UndoCancel();
				this.iconObjects.DeleteSelection();
			}
			this.globalModifier.Visible = false;
			this.createRank = -1;
			this.editObject = null;
			this.OnPanelChanged();
			this.OnCommandChanged();
			this.OnInfoObjectChanged();
			this.InvalidateAll();
		}

		[Command ("Duplicate")]
		void CommandDuplicate(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.UndoBeginning("Duplicate");
			this.UndoSelectionWillBeChanged();
			Drawing.Point move = new Drawing.Point(1, 1);
			this.iconObjects.DuplicateSelection(move);
			this.globalModifier.MoveAll(move);
			this.UndoValidate();

			this.iconObjects.UpdateEditProperties(this.objectMemory);
			this.OnPanelChanged();
			this.OnCommandChanged();
			this.OnInfoObjectChanged();
			this.InvalidateAll();
		}

		[Command ("Cut")]
		void CommandCut(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.UndoBeginning("Cut");
			this.iconObjects.CopySelection();
			this.iconObjects.DeleteSelection();
			this.globalModifier.Visible = false;
			this.UndoValidate();

			this.OnPanelChanged();
			this.OnCommandChanged();
			this.OnInfoObjectChanged();
			this.InvalidateAll();
		}

		[Command ("Copy")]
		void CommandCopy(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.iconObjects.CopySelection();
			this.OnCommandChanged();
		}

		[Command ("Paste")]
		void CommandPaste(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.UndoBeginning("Paste");
			this.iconObjects.DeselectAll();
			this.iconObjects.PasteSelection();
			this.UndoValidate();

			this.globalModifier.Visible = true;
			this.iconObjects.GlobalSelect(this.globalModifier.Visible);
			if ( this.globalModifier.Visible )
			{
				this.globalModifier.Initialize(this.iconObjects.RetSelectedBbox());
			}
			
			this.iconObjects.UpdateEditProperties(this.objectMemory);
			this.OnCommandChanged();
			this.OnInfoObjectChanged();
			this.InvalidateAll();
		}

		[Command ("Undo")]
		void CommandUndo(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.UndoRestore() )  return;
			this.InvalidateAll();
			this.OnCommandChanged();
		}

		[Command ("Redo")]
		void CommandRedo(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.RedoRestore() )  return;
			this.InvalidateAll();
			this.OnCommandChanged();
		}

		[Command ("OrderUp")]
		void CommandOrderUp(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.UndoBeginning("OrderUp");
			this.iconObjects.OrderSelection(1);
			this.UndoValidate();

			this.OnCommandChanged();
			this.InvalidateAll();
		}

		[Command ("OrderDown")]
		void CommandOrderDown(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.UndoBeginning("OrderDown");
			this.iconObjects.OrderSelection(-1);
			this.UndoValidate();

			this.OnCommandChanged();
			this.InvalidateAll();
		}

		[Command ("Merge")]
		void CommandMerge(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.UndoBeginning("Merge");
			this.UndoSelectionWillBeChanged();
			string name = this.iconObjects.GroupName();
			this.iconObjects.UngroupSelection();
			this.iconObjects.GroupSelection();
			this.iconObjects.GroupName(name);
			this.globalModifier.Visible = false;
			this.UndoValidate();

			this.OnPanelChanged();
			this.OnCommandChanged();
			this.OnInfoObjectChanged();
			this.InvalidateAll();
		}

		[Command ("Group")]
		void CommandGroup(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.UndoBeginning("Group");
			this.UndoSelectionWillBeChanged();
			this.iconObjects.GroupSelection();
			this.globalModifier.Visible = false;
			this.UndoValidate();

			this.OnPanelChanged();
			this.OnCommandChanged();
			this.OnInfoObjectChanged();
			this.InvalidateAll();
		}

		[Command ("Ungroup")]
		void CommandUngroup(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.UndoBeginning("UnGroup");
			this.UndoSelectionWillBeChanged();
			this.iconObjects.UngroupSelection();
			this.UndoValidate();

			this.OnPanelChanged();
			this.OnCommandChanged();
			this.OnInfoObjectChanged();
			this.InvalidateAll();
		}

		[Command ("Inside")]
		void CommandInside(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.UndoBeginning("UnGroup");
			this.UndoSelectionWillBeChanged();
			this.InsideSelection();
			this.globalModifier.Visible = false;
			this.UndoValidate();

			this.OnPanelChanged();
			this.OnCommandChanged();
			this.OnInfoObjectChanged();
			this.InvalidateAll();
		}

		[Command ("Outside")]
		void CommandOutside(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.UndoBeginning("UnGroup");
			this.UndoSelectionWillBeChanged();
			this.OutsideSelection();
			this.globalModifier.Visible = false;
			this.UndoValidate();

			this.OnPanelChanged();
			this.OnCommandChanged();
			this.OnInfoObjectChanged();
			this.InvalidateAll();
		}

		[Command ("Grid")]
		void CommandGrid(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.iconContext.GridActive = !this.iconContext.GridActive;
			this.InvalidateDraw();
			this.OnCommandChanged();
		}

		[Command ("Preview")]
		void CommandPreview(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.iconContext.PreviewActive = !this.iconContext.PreviewActive;
			this.InvalidateDraw();
			this.OnCommandChanged();
		}

		[Command ("Deselect")]
		void CommandDeselect(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.rankLastCreated = -1;
			this.SelectedTool = "Select";
			this.ChangeSelection("Deselect");
			this.OnPanelChanged();
			this.iconObjects.UpdateEditProperties(this.objectMemory);
			this.OnCommandChanged();
			this.OnInfoObjectChanged();
			this.InvalidateAll();
		}

		[Command ("SelectAll")]
		void CommandSelectAll(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.rankLastCreated = -1;
			this.SelectedTool = "Select";
			this.ChangeSelection("SelectAll");
			this.OnPanelChanged();
			this.iconObjects.UpdateEditProperties(this.objectMemory);
			this.OnCommandChanged();
			this.OnInfoObjectChanged();
			this.InvalidateAll();
			this.OnUsePropertiesPanel();
		}

		[Command ("SelectInvert")]
		void CommandSelectInvert(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.rankLastCreated = -1;
			this.SelectedTool = "Select";
			this.ChangeSelection("SelectInvert");
			this.OnPanelChanged();
			this.iconObjects.UpdateEditProperties(this.objectMemory);
			this.OnCommandChanged();
			this.OnInfoObjectChanged();
			this.InvalidateAll();
			this.OnUsePropertiesPanel();
		}

		[Command ("SelectGlobal")]
		void CommandSelectGlobal(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.UndoBeginning("SelectGlobal");
			this.UndoSelectionWillBeChanged();
			this.UndoValidate();

			this.SelectedTool = "Select";
			this.globalModifier.Visible = !this.globalModifier.Visible;
			this.iconObjects.GlobalSelect(this.globalModifier.Visible);
			if ( this.globalModifier.Visible )
			{
				this.globalModifier.Initialize(this.iconObjects.RetSelectedBbox());
			}
			this.OnPanelChanged();
			this.iconObjects.UpdateEditProperties(this.objectMemory);
			this.OnCommandChanged();
			this.OnInfoObjectChanged();
			this.InvalidateAll();
		}

		[Command ("SelectMode")]
		void CommandSelectMode(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.SelectedTool = "Select";
			this.selectModePartial = !this.selectModePartial;
			this.OnPanelChanged();
			this.OnCommandChanged();
			this.InvalidateAll();
		}

		[Command ("HideHalf")]
		void CommandHideHalf(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.SelectedTool = "Select";
			this.iconContext.HideHalfActive = !this.iconContext.HideHalfActive;
			this.OnCommandChanged();
			this.InvalidateAll();
		}

		[Command ("HideSel")]
		void CommandHideSel(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.SelectedTool = "Select";
			this.UndoBeginning("HideSel");
			this.iconObjects.HideSelection();
			this.globalModifier.Visible = false;
			this.UndoValidate();

			this.OnPanelChanged();
			this.iconObjects.UpdateEditProperties(this.objectMemory);
			this.OnCommandChanged();
			this.OnInfoObjectChanged();
			this.InvalidateAll();
		}

		[Command ("HideRest")]
		void CommandHideRest(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.SelectedTool = "Select";
			this.UndoBeginning("HideRest");
			this.UndoSelectionWillBeChanged();
			this.iconObjects.HideRest();
			this.UndoValidate();

			this.OnPanelChanged();
			this.iconObjects.UpdateEditProperties(this.objectMemory);
			this.OnCommandChanged();
			this.OnInfoObjectChanged();
			this.InvalidateAll();
		}

		[Command ("HideCancel")]
		void CommandHideCancel(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.SelectedTool = "Select";
			this.UndoBeginning("HideCancel");
			this.iconObjects.HideCancel();
			this.globalModifier.Visible = true;
			this.iconObjects.GlobalSelect(this.globalModifier.Visible);
			this.globalModifier.Initialize(this.iconObjects.RetSelectedBbox());
			this.UndoValidate();

			this.OnPanelChanged();
			this.iconObjects.UpdateEditProperties(this.objectMemory);
			this.OnCommandChanged();
			this.OnInfoObjectChanged();
			this.InvalidateAll();
		}

		[Command ("ZoomMin")]
		void CommandZoomMin(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ZoomMemorize();
			this.Zoom = this.ZoomMin;
			this.OriginX = 0;
			this.OriginY = 0;
		}

		[Command ("ZoomDefault")]
		void CommandZoomDefault(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ZoomMemorize();
			this.Zoom = 1;
			this.OriginX = 0;
			this.OriginY = 0;
		}

		[Command ("ZoomSel")]
		void CommandZoomSel(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ZoomSel();
		}

		[Command ("ZoomPrev")]
		void CommandZoomPrev(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ZoomPrev();
		}

		[Command ("ZoomSub")]
		void CommandZoomSub(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ZoomChange(0.5);
		}

		[Command ("ZoomAdd")]
		void CommandZoomAdd(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ZoomChange(2.0);
		}

		[Command ("ArrayOutlineFrame")]
		[Command ("ArrayOutlineHoriz")]
		[Command ("ArrayOutlineVerti")]
		[Command ("ArrayAddColumnLeft")]
		[Command ("ArrayAddColumnRight")]
		[Command ("ArrayAddRowTop")]
		[Command ("ArrayAddRowBottom")]
		[Command ("ArrayDelColumn")]
		[Command ("ArrayDelRow")]
		[Command ("ArrayAlignColumn")]
		[Command ("ArrayAlignRow")]
		[Command ("ArraySwapColumn")]
		[Command ("ArraySwapRow")]
		[Command ("ArrayLook")]
		void CommandArray(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			ObjectArray array = this.iconObjects.RetFirstSelected() as ObjectArray;
			if ( array == null )  return;

			this.UndoBeginning("ArrayOperation");
			this.UndoSelectionWillBeChanged();
			array.ExecuteCommand(e.CommandName, (e.CommandArgs.Length == 0) ? "" : e.CommandArgs[0]);
			this.UndoValidate();

			this.OnPanelChanged();
			this.OnInfoObjectChanged();
			this.OnCommandChanged();
			this.InvalidateAll();
		}

		[Command ("DebugBboxThin")]
		void CommandDebugBboxThin(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.iconContext.IsDrawBoxThin = !this.iconContext.IsDrawBoxThin;
			this.OnCommandChanged();
			this.InvalidateAll();
		}

		[Command ("DebugBboxGeom")]
		void CommandDebugBboxGeom(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.iconContext.IsDrawBoxGeom = !this.iconContext.IsDrawBoxGeom;
			this.OnCommandChanged();
			this.InvalidateAll();
		}

		[Command ("DebugBboxFull")]
		void CommandDebugBboxFull(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.iconContext.IsDrawBoxFull = !this.iconContext.IsDrawBoxFull;
			this.OnCommandChanged();
			this.InvalidateAll();
		}


		protected AbstractObject CreateObject()
		{
			//	Cr�e un nouvel objet selon l'outil s�lectionn�.
			AbstractObject obj = null;
			switch ( this.selectedTool )
			{
				case "ObjectLine":       obj = new ObjectLine();       break;
				case "ObjectRectangle":  obj = new ObjectRectangle();  break;
				case "ObjectCircle":     obj = new ObjectCircle();     break;
				case "ObjectEllipse":    obj = new ObjectEllipse();    break;
				case "ObjectRegular":    obj = new ObjectRegular();    break;
				case "ObjectPoly":       obj = new ObjectPoly();       break;
				case "ObjectBezier":     obj = new ObjectBezier();     break;
				case "ObjectTextLine":   obj = new ObjectTextLine();   break;
				case "ObjectTextBox":    obj = new ObjectTextBox();    break;
				case "ObjectArray":      obj = new ObjectArray();      break;
				case "ObjectImage":      obj = new ObjectImage();      break;
			}
			if ( obj == null )  return null;
			return obj;
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

		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			//	Gestion d'un �v�nement.
			if ( !this.isEditable )  return;

			pos = this.ScreenToIcon(pos);

			if ( this.selectedTool == "Edit" && this.editObject != null )
			{
				if ( message.Type == MessageType.MouseUp )
				{
					this.UndoValidate();
					this.mouseDown = false;
				}

				if ( this.EditProcessMessage(message, pos) )
				{
					message.Consumer = this;
					return;
				}
			}

			switch ( message.Type )
			{
				case MessageType.MouseDown:
					this.OnUsePropertiesPanel();
					if ( message.IsLeftButton )
					{
						if ( this.selectedTool == "Select" )
						{
							this.SelectMouseDown(pos, message.IsShiftPressed, message.IsCtrlPressed, message.ButtonDownCount);
						}
						else if ( this.selectedTool == "Edit" )
						{
							if ( this.EditMouseDown(pos, message.IsShiftPressed, message.IsCtrlPressed, message.ButtonDownCount) )
							{
								this.EditMouseDownMessage(pos);
							}
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
						this.SelectMouseDown(pos, message.IsShiftPressed, message.IsCtrlPressed, message.ButtonDownCount);
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
					else if ( this.selectedTool == "Edit" )
					{
						if ( this.EditMouseMove(pos, message.IsShiftPressed, message.IsCtrlPressed) )
						{
							this.EditMouseDownMessage(pos);
						}
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
						else if ( this.selectedTool == "Edit" )
						{
							this.EditMouseUp(pos, message.IsShiftPressed, message.IsCtrlPressed, message.IsRightButton);
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
					if ( message.IsAltPressed )
					{
						//	Il ne faut jamais manger les pressions de touches avec ALT, car elles sont
						//	utilis�es par les raccourcis clavier globaux.
						return;
					}
					if ( message.KeyCode == KeyCode.ControlKey )
					{
						this.iconContext.IsCtrl = true;
						this.InvalidateDraw();
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
						this.CommandDelete();
					}
					break;

				case MessageType.KeyUp:
					if ( message.KeyCode == KeyCode.ControlKey )
					{
						this.iconContext.IsCtrl = false;
						this.InvalidateDraw();
					}
					break;
			}
			
			message.Consumer = this;
		}


		protected void ContextMenu(Drawing.Point mouse, bool globalMenu)
		{
			//	Construit le menu contextuel.
			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			this.iconObjects.Hilite(null, ref bbox);
			this.InvalidateAll(bbox);

			this.UpdateCommands();  // utile si l'objet vient d'�tre s�lectionn� !

			int nbSel = this.iconObjects.TotalSelected();
			System.Collections.ArrayList list = new System.Collections.ArrayList();
			if ( globalMenu || nbSel == 0 )
			{
				this.MenuAddItem(list, "Deselect",     "file:images/deselect.icon",     "D�s�lectionner tout");
				this.MenuAddItem(list, "SelectAll",    "file:images/selectall.icon",    "Tout s�lectionner");
				this.MenuAddItem(list, "SelectInvert", "file:images/selectinvert.icon", "Inverser la s�lection");
				this.MenuAddItem(list, "SelectGlobal", "file:images/selectglobal.icon", "Changer le mode de s�lection");
				this.MenuAddSep(list);
				this.MenuAddItem(list, "HideSel",      "file:images/hidesel.icon",      "Cacher la s�lection");
				this.MenuAddItem(list, "HideRest",     "file:images/hiderest.icon",     "Cacher le reste");
				this.MenuAddItem(list, "HideCancel",   "file:images/hidecancel.icon",   "Montrer tout");
				this.MenuAddSep(list);
				this.MenuAddItem(list, "ZoomMin",      "file:images/zoommin.icon",      "Zoom minimal");
				this.MenuAddItem(list, "ZoomDefault",  "file:images/zoomdefault.icon",  "Zoom 100%");
				this.MenuAddItem(list, "ZoomSel",      "file:images/zoomsel.icon",      "Zoom s�lection");
				this.MenuAddItem(list, "ZoomPrev",     "file:images/zoomprev.icon",     "Zoom pr�c�dent");
				this.MenuAddItem(list, "ZoomSub",      "file:images/zoomsub.icon",      "R�duction");
				this.MenuAddItem(list, "ZoomAdd",      "file:images/zoomadd.icon",      "Agrandissement");
				this.MenuAddSep(list);
				this.MenuAddItem(list, "Outside",      "file:images/outside.icon",      "Sortir du groupe");
				this.MenuAddItem(list, "SelectMode",   "file:images/selectmode.icon",   "S�lection partielle");
				this.MenuAddItem(list, "Grid",         "file:images/grid.icon",         "Grille magn�tique");
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

				this.MenuAddItem(list, "Delete",       "file:images/delete.icon",       "Supprimer");
				this.MenuAddItem(list, "Duplicate",    "file:images/duplicate.icon",    "Dupliquer");
				this.MenuAddItem(list, "OrderUp",      "file:images/orderup.icon",      "Dessus");
				this.MenuAddItem(list, "OrderDown",    "file:images/orderdown.icon",    "Dessous");
				this.MenuAddItem(list, "Merge",        "file:images/merge.icon",        "Fusionner");
				this.MenuAddItem(list, "Group",        "file:images/group.icon",        "Associer");
				this.MenuAddItem(list, "Ungroup",      "file:images/ungroup.icon",      "Dissocier");
				this.MenuAddItem(list, "Inside",       "file:images/inside.icon",       "Entrer dans groupe");
				this.MenuAddItem(list, "Outside",      "file:images/outside.icon",      "Sortir du groupe");
				this.MenuAddSep(list);
				this.MenuAddItem(list, "SelectGlobal", "file:images/selectglobal.icon", "Changer le mode de s�lection");
				this.MenuAddItem(list, "ZoomSel",      "file:images/zoomsel.icon",      "Zoom s�lection");
				this.MenuAddSep(list);
				this.MenuAddItem(list, "HideSel",      "file:images/hidesel.icon",      "Cacher la s�lection");
				this.MenuAddItem(list, "HideRest",     "file:images/hiderest.icon",     "Cacher le reste");
				this.MenuAddItem(list, "HideCancel",   "file:images/hidecancel.icon",   "Montrer tout");

				if ( nbSel == 1 && this.contextMenuObject != null )
				{
					this.contextMenuObject.ContextMenu(list, mouse, this.contextMenuRank);
				}
			}

			this.contextMenu = new VMenu();
			this.contextMenu.Host = this;
			
			foreach ( ContextMenuItem cmi in list )
			{
				if ( cmi.Name == "" )
				{
					this.contextMenu.Items.Add(new MenuSeparator());
				}
				else
				{
					MenuItem mi = new MenuItem(cmi.Command, cmi.Icon, cmi.Text, "", cmi.Name);
					mi.IconNameActiveNo = cmi.IconActiveNo;
					mi.IconNameActiveYes = cmi.IconActiveYes;
					mi.ActiveState = cmi.Active ? WidgetState.ActiveYes : WidgetState.ActiveNo;
					this.contextMenu.Items.Add(mi);
				}
			}
			this.contextMenu.AdjustSize();
			mouse = this.IconToScreen(mouse);
			mouse = this.MapClientToScreen(mouse);
			this.commandDispatcher.SyncCommandStates();
			this.contextMenu.ShowAsContextMenu(this.Window, mouse);
		}

		protected void MenuAddItem(System.Collections.ArrayList list, string cmd, string icon, string text)
		{
			//	Ajoute une case dans le menu.
			CommandDispatcher.CommandState state = this.commandDispatcher[cmd];
			if ( state != null )
			{
				if ( !state.Enabled )  return;
			}

			ContextMenuItem item = new ContextMenuItem();
			item.Command = cmd;
			item.Name = cmd;
			item.Icon = @icon;
			item.Text = text;
			list.Add(item);
		}

		protected void MenuAddSep(System.Collections.ArrayList list)
		{
			//	Ajoute un s�parateur dans le menu.
			ContextMenuItem item = new ContextMenuItem();
			list.Add(item);  // s�parateur
		}

		[Command ("Object")]
		void CommandObject(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			//	Ex�cute une commande locale � un objet.
			Widget widget = e.Source as Widget;
			if ( widget.Name == "CreateEnding" )
			{
				this.CreateEnding();
				return;
			}
			if ( widget.Name == "CreateAndSelect" )
			{
				this.CreateEnding();
				this.SelectedTool = "Select";
				this.UpdateCommands();
				return;
			}
			this.UndoBeginning("Object");
			this.UndoSelectionWillBeChanged();
			this.UndoValidate();

			this.contextMenuObject.ContextCommand(widget.Name, this.contextMenuPos, this.contextMenuRank);
			this.InvalidateAll();
		}


		protected void SelectMouseDown(Drawing.Point mouse, bool isShift, bool isCtrl, int downCount)
		{
			this.UndoBeginning("Select");
			this.UndoSelectionWillBeChanged();

			this.moveStart = mouse;
			this.moveAccept = false;
			this.iconContext.IsCtrl = isCtrl;
			this.iconContext.ConstrainFixStarting(mouse);
			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			this.iconObjects.Hilite(null, ref bbox);
			this.globalModifier.HiliteHandle(-1, this.iconContext, ref bbox);
			this.HiliteHandle(null, -1, ref bbox);
			this.moveGlobal = -1;
			this.moveObject = null;
			this.cellObject = null;
			this.editObject = null;
			this.selectRect = false;

			AbstractObject obj;
			int rank;
			if ( this.globalModifier.Detect(mouse, out rank) )
			{
				this.moveGlobal = rank;
				this.globalModifier.MoveStarting(this.moveGlobal, mouse, this.iconContext);
				this.globalModifier.HiliteHandle(this.moveGlobal, this.iconContext, ref bbox);
			}
			else
			{
				if ( this.globalModifier.Visible )
				{
					this.globalModifier.Visible = false;
					this.Select(null, false, false);
				}

				if ( this.iconObjects.DetectHandle(mouse, out obj, out rank) )
				{
					this.moveObject = obj;
					this.moveHandle = rank;
					this.moveOffset = mouse-obj.Handle(rank).Position;
					this.moveObject.MoveHandleStarting(this.moveHandle, mouse, this.iconContext);
					this.HiliteHandle(this.moveObject, this.moveHandle, ref bbox);
					this.iconContext.ConstrainFixStarting(obj.Handle(rank).Position);
				}
				else if ( this.iconObjects.DetectCell(mouse, out obj, out rank) )
				{
					this.cellObject = obj;
					this.cellRank   = rank;
					this.cellObject.MoveCellStarting(this.cellRank, mouse, isShift, isCtrl, downCount, this.iconContext);
					this.iconContext.ConstrainDelStarting();
				}
				else
				{
					obj = this.iconObjects.Detect(mouse);
					if ( obj == null )
					{
						this.selectRect = true;
						this.selectRectP1 = mouse;
						this.selectRectP2 = mouse;
						this.globalModifier.Visible = false;
					}
					else
					{
						if ( !obj.IsSelected() )
						{
							this.Select(obj, false, isCtrl);
							this.OnInfoObjectChanged();
						}
						else
						{
							if ( isCtrl )
							{
								obj.Deselect();
								this.iconObjects.UpdateEditProperties(this.objectMemory);
								this.OnPanelChanged();
								this.OnInfoObjectChanged();
							}
						}
						this.moveObject = obj;
						this.moveHandle = -1;  // d�place tout l'objet
						this.iconContext.SnapGrid(ref mouse);
						this.moveOffset = mouse;
					}
				}
			}

			this.InvalidateAll();
		}

		protected void SelectMouseMove(Drawing.Point mouse, bool isShift, bool isCtrl)
		{
			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			this.iconContext.IsCtrl = isCtrl;
			this.HiliteHandle(null, -1, ref bbox);
			this.globalModifier.HiliteHandle(-1, this.iconContext, ref bbox);

			if ( this.mouseDown )  // bouton souris press� ?
			{
				if ( this.selectRect )
				{
					Drawing.Rectangle rSelect = new Drawing.Rectangle(this.selectRectP1, this.selectRectP2);
					bbox.MergeWith(rSelect);

					this.selectRectP2 = mouse;

					rSelect = new Drawing.Rectangle(this.selectRectP1, this.selectRectP2);
					bbox.MergeWith(rSelect);
				}
				else if ( this.moveGlobal != -1 )  // d�place le modificateur global ?
				{
					GlobalModifierData initial = this.globalModifier.CloneData();
					bbox.MergeWith(this.globalModifier.BoundingBox());
					this.globalModifier.MoveProcess(this.moveGlobal, mouse, this.iconContext);
					this.globalModifier.HiliteHandle(this.moveGlobal, this.iconContext, ref bbox);
					bbox.MergeWith(this.globalModifier.BoundingBox());
					this.iconObjects.MoveSelection(initial, this.globalModifier.Data, ref bbox);
				}
				else if ( this.moveObject != null )
				{
					if ( this.moveHandle != -1 )  // d�place une poign�e ?
					{
						mouse -= this.moveOffset;
						this.iconObjects.MoveHandleProcess(this.moveObject, this.moveHandle, mouse, this.iconContext, ref bbox);
						this.HiliteHandle(this.moveObject, this.moveHandle, ref bbox);
					}
					else	// d�place tout l'objet ?
					{
						this.iconContext.ConstrainSnapPos(ref mouse);

						if ( !this.moveAccept )
						{
							double len = Drawing.Point.Distance(mouse, this.moveStart);
							if ( len <= this.iconContext.MinimalSize )
							{
								mouse = this.moveStart;
							}
							else
							{
								this.moveAccept = true;
							}
						}
						this.iconContext.SnapGrid(ref mouse);
						this.iconObjects.MoveSelection(mouse-this.moveOffset, ref bbox);
						this.moveOffset = mouse;
					}
				}
				else if ( this.cellObject != null )
				{
					this.cellRank = this.cellObject.DetectCell(mouse);
					this.cellObject.MoveCellProcess(this.cellRank, mouse, isShift, isCtrl, this.iconContext);
					bbox.MergeWith(this.cellObject.BoundingBox);
				}
			}
			else	// bouton souris rel�ch� ?
			{
				AbstractObject obj;
				int rank;
				if ( this.globalModifier.Detect(mouse, out rank) )
				{
					this.globalModifier.HiliteHandle(rank, this.iconContext, ref bbox);
					this.MouseCursor = MouseCursor.AsHand;
				}
				else if ( this.iconObjects.DetectHandle(mouse, out obj, out rank) )
				{
					this.iconObjects.Hilite(null, ref bbox);
					this.HiliteHandle(obj, rank, ref bbox);
					this.MouseCursor = MouseCursor.AsHand;
				}
				else
				{
					obj = this.iconObjects.Detect(mouse);
					this.iconObjects.Hilite(obj, ref bbox);
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

			this.InvalidateAll(bbox);
		}

		protected void SelectMouseUp(Drawing.Point mouse, bool isShift, bool isCtrl, bool isRight)
		{
			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			this.iconContext.IsCtrl = isCtrl;
			bool globalMenu = false;
			this.globalModifier.HiliteHandle(-1, this.iconContext, ref bbox);
			this.HiliteHandle(null, -1, ref bbox);

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
					this.Select(rSelect, isCtrl);  // s�lectionne les objets dans le rectangle
					if ( this.iconObjects.TotalSelected() > 0 )
					{
						this.iconObjects.GlobalSelect(true);
						this.globalModifier.Initialize(rSelect);
						this.globalModifier.Visible = true;
					}
					this.OnInfoObjectChanged();
				}
				bbox = Drawing.Rectangle.Infinite;
			}
			else if ( this.moveGlobal != -1 )  // d�place le modificateur global ?
			{
				this.iconObjects.GroupUpdate(ref bbox);
				this.iconObjects.GroupUpdateParents(ref bbox);
			}
			else if ( this.moveObject != null )
			{
				this.iconObjects.GroupUpdate(ref bbox);
				this.iconObjects.GroupUpdateParents(ref bbox);

				if ( this.moveHandle != -1 )  // d�place une poign�e ?
				{
					if ( this.moveObject.IsMoveHandlePropertyChanged(this.moveHandle) ||
						this.moveObject.Handle(this.moveHandle).Type == HandleType.Property )
					{
						this.OnPanelChanged();
					}
				}

				this.moveObject = null;
				this.moveHandle = -1;
			}
			else if ( this.cellObject != null )
			{
				this.cellObject = null;
				this.cellRank   = -1;
				this.OnPanelChanged();
			}

			if ( this.iconContext.ConstrainDelStarting() )
			{
				bbox = Drawing.Rectangle.Infinite;
			}

			if ( bbox == Drawing.Rectangle.Infinite )
			{
				this.InvalidateAll();
			}
			else
			{
				this.InvalidateAll(bbox);
			}

			if ( isRight )  // avec le bouton de droite de la souris ?
			{
				this.ContextMenu(mouse, globalMenu);
			}

			this.UndoValidate();
		}


		protected bool EditProcessMessage(Message message, Drawing.Point pos)
		{
			if ( this.editObject.EditProcessMessage(message, pos) )
			{
				this.InvalidateAll(this.editObject.BoundingBox);
				return true;
			}
			else
			{
				return false;
			}
		}

		protected void EditMouseDownMessage(Drawing.Point pos)
		{
			this.editObject.EditMouseDownMessage(pos);
			this.InvalidateAll(this.editObject.BoundingBox);
		}

		protected bool EditMouseDown(Drawing.Point mouse, bool isShift, bool isCtrl, int downCount)
		{
			this.UndoBeginning("Edit");
			this.UndoSelectionWillBeChanged();

			bool newEdit = false;
			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;

			AbstractObject initialObject = this.editObject;
			this.editObjectSel = this.iconObjects.DetectEdit(mouse);
			if ( this.editObjectSel != null )
			{
				this.editObject = this.editObjectSel;
				this.Select(this.editObject, true, false);
				this.OnInfoObjectChanged();
				bbox.MergeWith(this.editObject.BoundingBox);
				newEdit = true;
			}
			if ( initialObject != null && initialObject != this.editObjectSel )
			{
				bbox.MergeWith(initialObject.BoundingBox);
			}

			this.InvalidateAll(bbox);
			return newEdit;
		}

		protected bool EditMouseMove(Drawing.Point mouse, bool isShift, bool isCtrl)
		{
			bool newEdit = false;
			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;

			if ( this.mouseDown )  // bouton souris press� ?
			{
				if ( this.editObjectSel == null )
				{
					this.editObjectSel = this.iconObjects.DetectEdit(mouse);
					if ( this.editObjectSel != null )
					{
						this.editObject = this.editObjectSel;
						this.Select(this.editObject, true, false);
						this.OnInfoObjectChanged();
						bbox.MergeWith(this.editObject.BoundingBox);
						newEdit = true;
					}
				}
			}
			else	// bouton souris rel�ch� ?
			{
				AbstractObject obj = this.iconObjects.DetectEdit(mouse);
				this.iconObjects.Hilite(obj, ref bbox);
				if ( obj != null )
				{
					bbox.MergeWith(obj.BoundingBox);
				}

				this.MouseCursor = MouseCursor.AsIBeam;
				this.Window.MouseCursor = this.MouseCursor;
			}

			this.InvalidateAll(bbox);
			return newEdit;
		}

		protected void EditMouseUp(Drawing.Point mouse, bool isShift, bool isCtrl, bool isRight)
		{
			this.UndoValidate();
			this.OnPanelChanged();
			this.InvalidateAll();
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
				this.InvalidateDraw();
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

		protected void ZoomSel()
		{
			//	Zoom sur les objets s�lectionn�s.
			Drawing.Rectangle bbox = this.iconObjects.RetSelectedBbox();
			if ( bbox.IsEmpty )  return;
			bbox.Inflate(2);
			this.ZoomChange(bbox.BottomLeft, bbox.TopRight);
		}

		protected void ZoomChange(Drawing.Point p1, Drawing.Point p2)
		{
			//	Change le zoom d'un certain facteur pour agrandir une zone rectangulaire.
			if ( p1.X == p2.X || p1.Y == p2.Y )  return;
			double fx = this.Width/(System.Math.Abs(p1.X-p2.X)*this.iconContext.ScaleX);
			double fy = this.Height/(System.Math.Abs(p1.Y-p2.Y)*this.iconContext.ScaleY);
			double factor = System.Math.Min(fx, fy);
			this.ZoomChange(factor, (p1+p2)/2);
		}

		protected void ZoomChange(double factor)
		{
			//	Change le zoom d'un certain facteur, avec centrage au centre du dessin.
			Drawing.Point center = new Drawing.Point();
			center.X = -this.OriginX+(this.IconObjects.Size.Width/this.Zoom)/2;
			center.Y = -this.OriginY+(this.IconObjects.Size.Height/this.Zoom)/2;
			this.ZoomChange(factor, center);
		}

		protected void ZoomChange(double factor, Drawing.Point center)
		{
			//	Change le zoom d'un certain facteur, avec centrage quelconque.
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

		protected void ZoomMemorize()
		{
			//	M�morise le zoom actuel.
			ZoomHistory.ZoomElement item = new ZoomHistory.ZoomElement();
			item.zoom = this.Zoom;
			item.ox   = this.OriginX;
			item.oy   = this.OriginY;
			this.zoomHistory.Add(item);
		}

		protected void ZoomPrev()
		{
			//	Revient au niveau de zoom pr�c�dent.
			ZoomHistory.ZoomElement item = this.zoomHistory.Remove();
			if ( item == null )  return;
			this.Zoom    = item.zoom;
			this.OriginX = item.ox;
			this.OriginY = item.oy;
		}

		protected double ZoomMin
		{
			//	Retourne le zoom minimal.
			get
			{
				double x = this.IconObjects.SizeArea.Width/this.IconObjects.Size.Width;
				double y = this.IconObjects.SizeArea.Height/this.IconObjects.Size.Height;
				return 1.0/System.Math.Min(x,y);
			}
		}

		protected double ZoomMax
		{
			//	Retourne le zoom maximal.
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
			this.UndoBeginning("Picker");
			this.UndoSelectionWillBeChanged();
			this.UndoValidate();

			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			AbstractObject obj = this.iconObjects.DeepDetect(mouse);
			this.PickerProperties(mouse, obj, ref bbox);
			this.InvalidateAll(bbox);
		}

		protected void PickerMouseMove(Drawing.Point mouse, bool isShift, bool isCtrl)
		{
			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			AbstractObject obj = this.iconObjects.DeepDetect(mouse);
			this.iconObjects.DeepHilite(obj, ref bbox);

			if ( this.mouseDown )
			{
				this.PickerProperties(mouse, obj, ref bbox);
			}

			this.InvalidateAll(bbox);
		}

		protected void PickerMouseUp(Drawing.Point mouse, bool isShift, bool isCtrl)
		{
			this.iconObjects.UpdateEditProperties(this.objectMemory);
		}

		protected void PickerProperties(Drawing.Point mouse, AbstractObject seed, ref Drawing.Rectangle bbox)
		{
			//	Modifie toutes les propri�t�s des objets s�lectionn�s en fonction
			//	d'un objet quelconque (seed).
			if ( seed == null )  return;

			int total = seed.TotalProperty;
			if ( this.iconObjects.TotalSelected() == 0 )
			{
				for ( int i=0 ; i<total ; i++ )
				{
					AbstractProperty property = seed.Property(i);
					if ( property == null )  break;
					this.newObject.SetProperty(property);
					this.objectMemory.SetProperty(property);
				}
			}
			else
			{
				for ( int i=0 ; i<total ; i++ )
				{
					AbstractProperty property = seed.Property(i);
					if ( property == null )  break;
					this.iconObjects.SetPropertyPicker(property, ref bbox);
					this.objectMemory.SetProperty(property);
				}
				this.OnPanelChanged();
			}
		}


		protected void HiliteHandle(AbstractObject obj, int rank, ref Drawing.Rectangle bbox)
		{
			//	Survolle une poign�e.
			if ( hiliteHandleObject != null &&
				 hiliteHandleObject.Handle(hiliteHandleRank).IsHilited == true )
			{
				hiliteHandleObject.Handle(hiliteHandleRank).IsHilited = false;
				hiliteHandleObject.Handle(hiliteHandleRank).BoundingBox(this.iconContext, ref bbox);
			}

			hiliteHandleObject = obj;
			hiliteHandleRank = rank;

			if ( hiliteHandleObject != null &&
				 hiliteHandleObject.Handle(hiliteHandleRank).IsHilited == false )
			{
				hiliteHandleObject.Handle(hiliteHandleRank).IsHilited = true;
				hiliteHandleObject.Handle(hiliteHandleRank).BoundingBox(this.iconContext, ref bbox);
			}
		}


		protected int Select(AbstractObject obj, bool edit, bool add)
		{
			//	S�lectionne un objet et d�s�lectionne tous les autres.
			int nb = this.iconObjects.Select(obj, edit, add);
			this.iconObjects.UpdateEditProperties(this.objectMemory);
			this.globalModifier.Visible = false;
			this.OnPanelChanged();
			return nb;
		}

		protected int Select(Drawing.Rectangle rect, bool add)
		{
			//	S�lectionne tous les objets dans le rectangle.
			int nb = this.iconObjects.Select(rect, add, !this.selectModePartial);
			this.iconObjects.UpdateEditProperties(this.objectMemory);
			this.globalModifier.Visible = false;
			this.OnPanelChanged();
			return nb;
		}


		protected void InsideSelection()
		{
			//	Entre dans le groupe s�lectionn�.
			AbstractObject obj = this.iconObjects.RetFirstSelected();
			this.iconObjects.InsideGroup(obj);
		}

		protected void OutsideSelection()
		{
			//	Sort du groupe en cours.
			this.iconObjects.DeselectAll();
			this.iconObjects.UpdateEditProperties(this.objectMemory);
			this.rankLastCreated = -1;
			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			this.iconObjects.GroupUpdateParents(ref bbox);
			this.iconObjects.OutsideGroup();
			this.iconObjects.UpdateEditProperties(this.objectMemory);
		}

		protected void DeselectAll()
		{
			//	D�s�lectionne tous les objets.
			this.UndoBeginning("DeselectAll");
			this.UndoSelectionWillBeChanged();
			int nb = this.iconObjects.DeselectAll();
			this.UndoValidate(nb > 0);
		}

		protected void ChangeSelection(string cmd)
		{
			//	Modifie la s�lection courante.
			this.UndoBeginning("ChangeSelection");

			bool validate = false;
			int nbsel = 0;
			UndoList cg = this.iconObjects.CurrentGroup;
			for ( int index=0 ; index<this.iconObjects.Count ; index++ )
			{
				AbstractObject obj = this.iconObjects[index];

				if ( cmd == "Deselect" )
				{
					if ( obj.IsSelected() )
					{
						cg.WillBeChanged(index);
						obj.Deselect();
						validate = true;
					}
				}

				if ( cmd == "SelectAll" )
				{
					if ( obj.IsHide )  continue;
					if ( !obj.IsSelected() )
					{
						cg.WillBeChanged(index);
						obj.Select();
						validate = true;
					}
					nbsel ++;
				}

				if ( cmd == "SelectInvert" )
				{
					if ( obj.IsHide )  continue;
					cg.WillBeChanged(index);
					obj.Select(!obj.IsSelected());
					if ( obj.IsSelected() )  nbsel ++;
					validate = true;
				}
			}

			this.UndoValidate(validate);

			if ( nbsel <= 1 )
			{
				this.globalModifier.Visible = false;
			}
			else
			{
				this.iconObjects.GlobalSelect(true);
				this.globalModifier.Initialize(this.iconObjects.RetSelectedBbox());
				this.globalModifier.Visible = true;
			}
		}


		protected void CreateMouseDown(Drawing.Point mouse, bool isShift, bool isCtrl)
		{
			this.iconContext.IsCtrl = isCtrl;
			this.iconContext.SnapGrid(ref mouse);

			if ( this.createRank == -1 )
			{
				this.UndoBeginning("Create");
				AbstractObject obj = this.CreateObject();
				if ( obj == null )  return;
				obj.CloneProperties(this.newObject);
				obj.PasteAdaptProperties(this.iconObjects.CurrentPattern == 0);
				obj.Select();
				obj.EditProperties = false;
				this.createRank = this.iconObjects.Add(obj, true);  // ajoute � la fin de la liste
			}

			this.iconObjects[this.createRank].CreateMouseDown(mouse, this.iconContext);
			this.InvalidateAll();
			this.OnInfoObjectChanged();
		}

		protected void CreateMouseMove(Drawing.Point mouse, bool isShift, bool isCtrl)
		{
			this.iconContext.IsCtrl = isCtrl;
			//?this.iconContext.SnapGrid(ref mouse);

			if ( this.createRank == -1 )  return;
			Drawing.Rectangle bbox = this.iconObjects[this.createRank].BoundingBox;
			this.iconObjects[this.createRank].CreateMouseMove(mouse, this.iconContext);
			bbox.MergeWith(this.iconObjects[this.createRank].BoundingBox);
			this.InvalidateAll(bbox);
		}

		protected void CreateMouseUp(Drawing.Point mouse, bool isShift, bool isCtrl)
		{
			this.iconContext.IsCtrl = isCtrl;
			//?this.iconContext.SnapGrid(ref mouse);

			if ( this.createRank == -1 )  return;
			this.iconObjects[this.createRank].CreateMouseUp(mouse, this.iconContext);

			bool selectAfterCreation = false;
			bool editAfterCreation = false;
			if ( this.iconObjects[this.createRank].CreateIsEnding(this.iconContext) )
			{
				if ( this.iconObjects[this.createRank].CreateIsExist(this.iconContext) )
				{
					this.iconObjects[this.createRank].Deselect();
					this.rankLastCreated = this.createRank;
					selectAfterCreation = this.iconObjects[this.createRank].SelectAfterCreation();
					editAfterCreation = this.iconObjects[this.createRank].EditAfterCreation();
					this.UndoValidate();
				}
				else
				{
					this.iconObjects.RemoveAt(this.createRank);
					this.UndoCancel();
				}
				this.createRank = -1;
			}

			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			this.iconObjects.GroupUpdateParents(ref bbox);

			if ( selectAfterCreation )
			{
				this.SelectedTool = "Select";
			}
			if ( editAfterCreation )
			{
				this.SelectedTool = "Edit";
			}

			this.InvalidateAll();
			this.OnPanelChanged();
			this.OnInfoObjectChanged();
			this.OnCommandChanged();
		}

		public void CreateEnding()
		{
			if ( this.createRank == -1 )  return;
			if ( this.iconObjects[this.createRank].CreateEnding(this.iconContext) )
			{
				this.iconObjects[this.createRank].Deselect();
				this.rankLastCreated = this.createRank;
				this.UndoValidate();
			}
			else
			{
				this.iconObjects.RemoveAt(this.createRank);
				this.UndoCancel();
			}
			this.createRank = -1;
			this.InvalidateAll();
			this.OnPanelChanged();
			this.OnInfoObjectChanged();
		}


		protected void UndoFlush()
		{
			//	Vide la liste des annulations.
			if ( IconEditor.OpletQueue != null )
			{
				IconEditor.OpletQueue.PurgeUndo();
				IconEditor.OpletQueue.PurgeRedo();
			}
			this.zoomHistory.Clear();
		}

		public void UndoBeginning(string operation)
		{
			//	Indique que l'on va modifier l'ic�ne.
			this.UndoBeginning(operation, null, 0, PropertyType.None, 0);
		}

		public void UndoBeginning(string operation,
								  AbstractObject obj, int nbSel,
								  PropertyType propertyType, int styleID)
		{
			System.Diagnostics.Debug.Assert(!UndoList.Beginning);
			UndoList.Beginning = true;

			System.Diagnostics.Debug.WriteLine(string.Format("UndoBeginning: {0}", operation));
			if ( IconEditor.OpletQueue == null )  return;

			if ( propertyType != PropertyType.None )
			{
				if ( this.undoOperation      == operation    &&
					 this.undoObjectSelected == obj          &&
					 this.undoTotalSelected  == nbSel        &&
					 this.undoPropertyType   == propertyType )
				{
					return;  // situation initiale d�j� m�moris�e
				}
			}

			this.undoInProgress     = true;
			this.undoOperation      = operation;
			this.undoObjectSelected = obj;
			this.undoTotalSelected  = nbSel;
			this.undoPropertyType   = propertyType;

			UndoList.Operations = new System.Collections.ArrayList();  // d�but m�morisation

			Data.OpletBeginning ob = new Data.OpletBeginning(this, styleID, operation);
			UndoList.Operations.Add(ob);

			Data.OpletMisc om = new Data.OpletMisc(this);
			UndoList.Operations.Add(om);
		}

		public void UndoSelectionWillBeChanged()
		{
			//	M�morise tous les objets s�lectionn�s et leurs fils.
			System.Diagnostics.Debug.Assert(UndoList.Beginning);
			System.Diagnostics.Debug.WriteLine("UndoSelectionWillBeChanged");
			if ( IconEditor.OpletQueue == null )  return;
			if ( !this.undoInProgress )  return;

			this.iconObjects.SelectionWillBeChanged();
		}

		public void UndoValidate(bool validate)
		{
			//	Valide ou annule le dernier UndoBeginning.
			if ( validate )  this.UndoValidate();
			else             this.UndoCancel();
		}

		public void UndoValidate()
		{
			//	Valide le dernier UndoBeginning.
			System.Diagnostics.Debug.Assert(UndoList.Beginning);
			UndoList.Beginning = false;

			System.Diagnostics.Debug.WriteLine("UndoValidate");
			if ( IconEditor.OpletQueue == null )  return;
			if ( !this.undoInProgress )  return;

			if ( UndoList.Operations.Count > 2 )  // plus que OpletBeginning et OpletMisc ?
			{
				OpletBeginning beginning = UndoList.Operations[0] as OpletBeginning;
				Data.OpletEnding oe = new Data.OpletEnding(this, beginning);
				UndoList.Operations.Add(oe);

				using ( IconEditor.OpletQueue.BeginAction() )
				{
					foreach ( Support.AbstractOplet oplet in UndoList.Operations )
					{
						IconEditor.OpletQueue.Insert(oplet);
					}
					IconEditor.OpletQueue.ValidateAction();
				}
			}

			UndoList.Operations.Clear();
			UndoList.Operations = null;  // fin m�morisation
			this.undoInProgress = false;
			this.OnCommandChanged();
		}

		public void UndoCancel()
		{
			//	Annule le dernier UndoBeginning.
			System.Diagnostics.Debug.Assert(UndoList.Beginning);
			UndoList.Beginning = false;

			System.Diagnostics.Debug.WriteLine("UndoCancel");
			if ( IconEditor.OpletQueue == null )  return;
			if ( !this.undoInProgress )  return;

			UndoList.Operations.Clear();
			UndoList.Operations = null;  // fin m�morisation
			this.undoInProgress = false;
		}

		protected bool UndoRestore()
		{
			//	Remet le dessin dans son �tat pr�c�dent.
			System.Diagnostics.Debug.Assert(!UndoList.Beginning);
			if ( IconEditor.OpletQueue == null )  return false;
			if ( !IconEditor.OpletQueue.CanUndo )  return false;
			IconEditor.OpletQueue.UndoAction();
			return true;
		}

		protected bool RedoRestore()
		{
			//	Annule la derni�re annulation.
			System.Diagnostics.Debug.Assert(!UndoList.Beginning);
			if ( IconEditor.OpletQueue == null )  return false;
			if ( !IconEditor.OpletQueue.CanRedo )  return false;
			IconEditor.OpletQueue.RedoAction();
			return true;
		}

		public void UndoInitialWork()
		{
			//	Effectue quelques petites op�rations, toujours au d�but d'un
			//	undo ou d'un redo.
			UndoList.SelectAfterCreate = false;
		}

		public void UndoFinalWork(int styleID)
		{
			//	Effectue quelques petites op�rations, toujours � la fin d'un
			//	undo ou d'un redo.
			this.iconObjects.DeselectNoUndoStamp();

			if ( styleID != 0 )
			{
				AbstractProperty property = this.iconObjects.StylesCollection.GetProperty(styleID-1) as AbstractProperty;
				this.iconObjects.StyleSpread(property);
			}

			if ( UndoList.SelectAfterCreate )
			{
				this.SelectedTool = "Select";
			}

			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			this.iconObjects.GroupUpdateParents(ref bbox);

			this.globalModifier.UpdateHandle();
			this.OnAllChanged();
			this.iconObjects.StylesCollection.CollectionChanged();
			this.OnInfoObjectChanged();
			this.OnPageLayerChanged();
			this.rankLastCreated = -1;
		}


		protected void UpdateSizeSamples()
		{
			//	Met � jour les tailles des SampleButton.
			foreach ( Widget widget in this.clones )
			{
				SampleButton sample = widget as SampleButton;
				if ( sample != null )
				{
					sample.IconObjects.Size   = this.iconObjects.Size;
					sample.IconObjects.Origin = this.iconObjects.Origin;
				}
			}
		}

		public void InvalidateAll(Drawing.Rectangle bbox)
		{
			//	Invalide la zone ainsi que celles des clones.
			if ( bbox.IsEmpty )  return;

			this.UpdateRulerGeometry();

			bbox.Inflate(this.iconContext.SelectMarginSize);
			bbox.BottomLeft = this.IconToScreen(bbox.BottomLeft);
			bbox.TopRight   = this.IconToScreen(bbox.TopRight);

			this.SetSyncPaint(true);
			this.Invalidate(bbox);
			this.SetSyncPaint(false);

			foreach ( Widget widget in this.clones )
			{
				widget.SetSyncPaint(true);
				widget.Invalidate();
				widget.SetSyncPaint(false);
			}
		}

		public void InvalidateAll()
		{
			//	Invalide la zone ainsi que celles des clones.
			this.UpdateRulerGeometry();
			this.Invalidate();

			foreach ( Widget widget in this.clones )
			{
				widget.Invalidate();
			}
		}

		public void InvalidateDraw()
		{
			//	Invalide la zone de dessin. Il faut appeler cette m�thode � la
			//	place de this.Invalidate() pour positionner correctement la r�gle.
			this.UpdateRulerGeometry();
			this.Invalidate();
		}


		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
			this.UpdateRulerGeometry();
		}

		protected void UpdateRulerGeometry()
		{
			//	Positionne la r�gle en fonction de l'�ventuel objet en cours d'�dition.
			if ( this.textRuler == null )  return;

			if ( this.editObject == null )
			{
				this.textRuler.SetVisible(false);
				this.textRuler.DetachFromText();
			}
			else
			{
				this.iconContext.ScaleX = this.iconContext.Zoom*this.Client.Width/this.iconObjects.Size.Width;
				this.iconContext.ScaleY = this.iconContext.Zoom*this.Client.Height/this.iconObjects.Size.Height;

				if ( this.editObject.EditRulerLink(this.textRuler, this.iconContext) )
				{
					Drawing.Rectangle editRect = this.editObject.BoundingBoxThin;
					Drawing.Rectangle rulerRect = new Drawing.Rectangle();
					Drawing.Point p1 = this.IconToScreen(editRect.TopLeft);
					Drawing.Point p2 = this.IconToScreen(editRect.TopRight);
					rulerRect.BottomLeft = p1;
					rulerRect.Width = System.Math.Max(p2.X-p1.X, this.textRuler.MinimalWidth);
					rulerRect.Height = this.textRuler.DefaultHeight;
					rulerRect.RoundFloor();
					this.textRuler.Bounds = rulerRect;

					if ( p2.X-p1.X < this.textRuler.MinimalWidth )
					{
						this.textRuler.RightMargin += this.textRuler.MinimalWidth-(p2.X-p1.X);
					}

					this.textRuler.Scale = this.iconContext.ScaleX;
					this.textRuler.SetVisible(true);
				}
				else
				{
					this.textRuler.SetVisible(false);
					this.textRuler.DetachFromText();
				}
			}
		}


		protected void DrawGridBackground(Drawing.Graphics graphics)
		{
			//	Dessine la grille magn�tique dessous.
			double initialWidth = graphics.LineWidth;
			graphics.LineWidth = 1.0/this.iconContext.ScaleX;

			double ix = 0.5/this.iconContext.ScaleX;
			double iy = 0.5/this.iconContext.ScaleY;

			if ( this.iconContext.GridActive )
			{
				double step = this.iconContext.GridStep.X;
				for ( double pos=this.iconObjects.OriginArea.X ; pos<=this.iconObjects.SizeArea.Width ; pos+=step )
				{
					double x = pos;
					double y = this.iconObjects.OriginArea.Y;
					graphics.Align(ref x, ref y);
					x += ix;
					y += iy;
					graphics.AddLine(x, y, x, this.iconObjects.SizeArea.Height);
				}
				step = this.iconContext.GridStep.Y;
				for ( double pos=this.iconObjects.OriginArea.Y ; pos<=this.iconObjects.SizeArea.Height ; pos+=step )
				{
					double x = this.iconObjects.OriginArea.X;
					double y = pos;
					graphics.Align(ref x, ref y);
					x += ix;
					y += iy;
					graphics.AddLine(x, y, this.iconObjects.SizeArea.Width, y);
				}
				graphics.RenderSolid(Drawing.Color.FromARGB(0.3, 0.6,0.6,0.6));
			}

			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.iconObjects.Size.Width, this.iconObjects.Size.Height);
			graphics.Align(ref rect);
			rect.Offset(ix, iy);
			graphics.AddRectangle(rect);

			if ( this.iconObjects.CurrentPattern == 0 )
			{
				rect.Offset(-ix, -iy);
				rect.Deflate(2);
				graphics.Align(ref rect);
				rect.Offset(ix, iy);
				graphics.AddRectangle(rect);
			}

			double cx = this.iconObjects.Size.Width/2;
			double cy = this.iconObjects.Size.Height/2;
			graphics.Align(ref cx, ref cy);
			cx += ix;
			cy += iy;
			graphics.AddLine(cx, 0, cx, this.iconObjects.Size.Height);
			graphics.AddLine(0, cy, this.iconObjects.Size.Width, cy);
			graphics.RenderSolid(Drawing.Color.FromARGB(0.4, 0.5,0.5,0.5));

			graphics.LineWidth = initialWidth;
		}

		protected void DrawGridForeground(Drawing.Graphics graphics)
		{
			//	Dessine la grille magn�tique dessus.
			if ( !this.iconContext.GridActive )  return;

			double ix = 0.5/this.iconContext.ScaleX;
			double iy = 0.5/this.iconContext.ScaleY;

			double sx = this.iconContext.GridStep.X;
			double sy = this.iconContext.GridStep.Y;
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

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			//	Dessine l'ic�ne.
			//?Drawing.Rectangle clip = graphics.SaveClippingRectangle();
			//?graphics.ResetClippingRectangle();
			//?graphics.SetClippingRectangle(this.InnerBounds);

			if ( this.iconObjects.CurrentPattern != 0 )
			{
				graphics.AddFilledRectangle(clipRect);
				graphics.RenderSolid(Drawing.Color.FromRGB(0.75, 1.0, 1.0));  // cyan pastel
			}
			else if ( !this.BackColor.IsTransparent && this.iconContext.PreviewActive )
			{
				graphics.AddFilledRectangle(clipRect);
				graphics.RenderSolid(this.BackColor);
			}

			IAdorner adorner = Epsitec.Common.Widgets.Adorner.Factory.Active;
			this.iconContext.UniqueColor = Drawing.Color.Empty;

			double initialWidth = graphics.LineWidth;
			Drawing.Transform save = graphics.Transform;
			this.iconContext.ScaleX = this.iconContext.Zoom*this.Client.Width/this.iconObjects.Size.Width;
			this.iconContext.ScaleY = this.iconContext.Zoom*this.Client.Height/this.iconObjects.Size.Height;
			graphics.ScaleTransform(this.iconContext.ScaleX, this.iconContext.ScaleY, 0, 0);
			graphics.TranslateTransform(this.iconContext.OriginX, this.iconContext.OriginY);

			clipRect.BottomLeft = this.ScreenToIcon(clipRect.BottomLeft);
			clipRect.TopRight   = this.ScreenToIcon(clipRect.TopRight);

			//	Dessine la grille magn�tique.
			if ( this.isEditable )
			{
				//?this.DrawGridBackground(graphics);
			}

			//	Dessine les g�om�tries.
#if true
			this.iconContext.IsFocused = this.IsFocused;
			this.iconObjects.DrawGeometry(graphics, this.iconContext, this.iconObjects, adorner, clipRect, false);
#else
			Drawer.totalObjectDraw = 0;
			long cycles = Drawing.Agg.Library.Cycles;
			this.iconObjects.DrawGeometry(graphics, this.iconContext, this.iconObjects, adorner, clipRect, false);
			cycles = Drawing.Agg.Library.Cycles - cycles;
			System.Diagnostics.Debug.WriteLine(string.Format("DrawGeometry: {0} us/object @ 1.7GHz", cycles/1700/Drawer.totalObjectDraw));
#endif

			//	Dessine la grille magn�tique.
			if ( this.isEditable && !this.iconContext.PreviewActive )
			{
				//?this.DrawGridForeground(graphics);
				this.DrawGridBackground(graphics);
			}

			//	Dessine les poign�es.
			if ( this.isEditable )
			{
				this.iconObjects.DrawHandle(graphics, this.iconContext);
			}

			//	Dessine le rectangle de s�lection.
			if ( this.isEditable && this.selectRect )
			{
				Drawing.Rectangle rSelect = new Drawing.Rectangle(this.selectRectP1, this.selectRectP2);
				graphics.LineWidth = 1.0/this.iconContext.ScaleX;
				rSelect.Deflate(0.5/this.iconContext.ScaleX, 0.5/this.iconContext.ScaleY);
				graphics.AddRectangle(rSelect);
				graphics.RenderSolid(this.iconContext.HiliteOutlineColor);
			}

			//	Dessine le rectangle de modification.
			this.globalModifier.Draw(graphics, iconContext);

			//	Dessine les contraintes.
			if ( this.isEditable )
			{
				this.iconContext.DrawConstrain(graphics, this.iconObjects.SizeArea);
			}

			graphics.Transform = save;
			graphics.LineWidth = initialWidth;
			//?graphics.RestoreClippingRectangle(clip);

			//	Dessine le cadre.
			if ( this.isEditable )
			{
				Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
				rect.Deflate(0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(adorner.ColorBorder);
			}
		}

		static public int totalObjectDraw = 0;


		//	Imprime l'ic�ne.
		public void Print(Dialogs.Print dp)
		{
			this.iconPrinter.Print(dp, this.iconContext);
		}


		protected virtual void OnPanelChanged()
		{
			//	G�n�re un �v�nement pour dire qu'il faut changer les panneaux.
			if ( this.PanelChanged != null )  // qq'un �coute ?
			{
				this.PanelChanged(this);
			}
		}

		public event EventHandler PanelChanged;


		//	G�n�re un �v�nement pour dire qu'il faut changer l'�tat d'une commande.
		protected virtual void OnCommandChanged()
		{
			if ( this.CommandChanged != null )  // qq'un �coute ?
			{
				this.CommandChanged(this);
			}
		}

		public event EventHandler CommandChanged;

		//	G�n�re un �v�nement pour dire qu'il faut changer les commandes et les panneaux.
		protected virtual void OnAllChanged()
		{
			if ( this.AllChanged != null )  // qq'un �coute ?
			{
				this.AllChanged(this);
			}
		}

		public event EventHandler AllChanged;

		//	G�n�re un �v�nement pour dire qu'il faut changer les ascenseurs.
		protected virtual void OnScrollerChanged()
		{
			if ( this.ScrollerChanged != null )  // qq'un �coute ?
			{
				this.ScrollerChanged(this);
			}
		}

		public event EventHandler ScrollerChanged;

		//	G�n�re un �v�nement pour dire qu'il faut changer les informations.
		public virtual void OnInfoDocumentChanged()
		{
			if ( this.InfoDocumentChanged != null )  // qq'un �coute ?
			{
				this.InfoDocumentChanged(this);
			}
		}

		public event EventHandler InfoDocumentChanged;

		//	G�n�re un �v�nement pour dire qu'il faut changer les informations.
		protected virtual void OnInfoObjectChanged()
		{
			if ( this.InfoObjectChanged != null )  // qq'un �coute ?
			{
				this.InfoObjectChanged(this);
			}
		}

		public event EventHandler InfoObjectChanged;

		//	G�n�re un �v�nement pour dire qu'il faut changer les informations.
		protected virtual void OnInfoMouseChanged()
		{
			if ( this.InfoMouseChanged != null )  // qq'un �coute ?
			{
				this.InfoMouseChanged(this);
			}
		}

		public event EventHandler InfoMouseChanged;

		//	G�n�re un �v�nement pour dire qu'il faut changer les informations.
		protected virtual void OnInfoZoomChanged()
		{
			if ( this.InfoZoomChanged != null )  // qq'un �coute ?
			{
				this.InfoZoomChanged(this);
			}
		}

		public event EventHandler InfoZoomChanged;


		//	G�n�re un �v�nement pour dire qu'il faut utiliser le panneau des propri�t�s.
		protected virtual void OnUsePropertiesPanel()
		{
			if ( this.UsePropertiesPanel != null )  // qq'un �coute ?
			{
				this.UsePropertiesPanel(this);
			}
		}

		public event EventHandler UsePropertiesPanel;


		//	G�n�re un �v�nement pour dire que la page ou le calque a chang�.
		protected virtual void OnPageLayerChanged()
		{
			if ( this.PageLayerChanged != null )  // qq'un �coute ?
			{
				this.PageLayerChanged(this);
			}
		}

		public event EventHandler PageLayerChanged;


		protected bool					isActive = true;
		protected bool					isEditable = false;
		protected string				selectedTool;
		protected Drawing.Point			mousePos;
		protected bool					mouseDown = false;
		protected int					createRank = -1;
		protected AbstractObject		moveObject;
		protected int					moveGlobal;
		protected int					moveHandle;
		protected Drawing.Point			moveStart;
		protected bool					moveAccept;
		protected Drawing.Point			moveOffset;
		protected AbstractObject		cellObject;
		protected int					cellRank;
		protected bool					selectRect;
		protected Drawing.Point			selectRectP1;
		protected Drawing.Point			selectRectP2;
		protected int					rankLastCreated = -1;
		protected bool					selectModePartial = false;
		protected GlobalModifier		globalModifier = new GlobalModifier();
		protected AbstractObject		hiliteHandleObject = null;
		protected int					hiliteHandleRank = -1;
		protected AbstractObject		editObject;
		protected AbstractObject		editObjectSel;
		protected TextRuler				textRuler;

		protected VMenu					contextMenu;
		protected AbstractObject		contextMenuObject;
		protected Drawing.Point			contextMenuPos;
		protected int					contextMenuRank;

		protected IconObjects			iconObjects;
		protected IconContext			iconContext;
		protected IconPrinter			iconPrinter;
		protected AbstractObject		objectMemory;
		protected AbstractObject		newObject;
		protected Drawer				link = null;
		protected System.Collections.ArrayList	clones = new System.Collections.ArrayList();

		protected bool					undoInProgress;
		protected string				undoOperation;
		protected AbstractObject		undoObjectSelected;
		protected int					undoTotalSelected;
		protected PropertyType			undoPropertyType;
		protected OpletQueue			undoQueue = new OpletQueue();
		protected ZoomHistory			zoomHistory = new ZoomHistory();

		protected CommandDispatcher		commandDispatcher;
		protected CommandState			saveState;
		protected CommandState			deleteState;
		protected CommandState			duplicateState;
		protected CommandState			cutState;
		protected CommandState			copyState;
		protected CommandState			pasteState;
		protected CommandState			orderUpState;
		protected CommandState			orderDownState;
		protected CommandState			mergeState;
		protected CommandState			groupState;
		protected CommandState			ungroupState;
		protected CommandState			insideState;
		protected CommandState			outsideState;
		protected CommandState			undoState;
		protected CommandState			redoState;
		protected CommandState			deselectState;
		protected CommandState			selectAllState;
		protected CommandState			selectInvertState;
		protected CommandState			selectGlobalState;
		protected CommandState			selectModeState;
		protected CommandState			hideHalfState;
		protected CommandState			hideSelState;
		protected CommandState			hideRestState;
		protected CommandState			hideCancelState;
		protected CommandState			zoomMinState;
		protected CommandState			zoomDefaultState;
		protected CommandState			zoomSelState;
		protected CommandState			zoomPrevState;
		protected CommandState			zoomSubState;
		protected CommandState			zoomAddState;
		protected CommandState			previewState;
		protected CommandState			gridState;
		protected CommandState			modeState;
		protected CommandState			arrayOutlineFrameState;
		protected CommandState			arrayOutlineHorizState;
		protected CommandState			arrayOutlineVertiState;
		protected CommandState			arrayAddColumnLeftState;
		protected CommandState			arrayAddColumnRightState;
		protected CommandState			arrayAddRowTopState;
		protected CommandState			arrayAddRowBottomState;
		protected CommandState			arrayDelColumnState;
		protected CommandState			arrayDelRowState;
		protected CommandState			arrayAlignColumnState;
		protected CommandState			arrayAlignRowState;
		protected CommandState			arraySwapColumnState;
		protected CommandState			arraySwapRowState;
		protected CommandState			arrayLookState;
		protected CommandState			debugBboxThinState;
		protected CommandState			debugBboxGeomState;
		protected CommandState			debugBboxFullState;

		protected Drawing.Image			mouseCursorZoom = null;
		protected Drawing.Image			mouseCursorHand = null;
		protected Drawing.Image			mouseCursorPicker = null;
	}
}
