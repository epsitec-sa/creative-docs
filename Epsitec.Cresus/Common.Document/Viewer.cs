using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// Summary description for Viewer.
	/// </summary>
	[SuppressBundleSupport]
	public class Viewer : Widgets.Widget
	{
		protected enum MouseCursorType
		{
			Unknow,
			Arrow,
			ArrowPlus,
			ArrowDup,
			Hand,
			IBeam,
			Cross,
			Finger,
			FingerDup,
			Pen,
			Zoom,
			Picker,
			PickerEmpty,
		}

		public Viewer(Document document)
		{
			this.InternalState |= InternalState.AutoFocus;
			this.InternalState |= InternalState.Focusable;
			this.InternalState |= InternalState.AutoDoubleClick;

			this.BackColor = Color.FromBrightness(1);  // fond blanc

			this.document = document;
			this.drawingContext = new DrawingContext(this.document, this);
			this.selector = new Selector(this.document);
			this.zoomer = new Selector(this.document);
			this.mousePos = new Point(0,0);
			this.mousePosValid = false;
			this.mouseDown = false;
			this.RedrawAreaFlush();

			this.textRuler = new TextRuler(this);
			this.textRuler.SetVisible(false);
			if ( this.document.Type == DocumentType.Pictogram )
			{
				this.textRuler.AllFonts = false;
			}
			this.textRuler.Changed += new EventHandler(this.HandleRulerChanged);

			this.autoScrollTimer = new Timer();
			this.autoScrollTimer.AutoRepeat = 0.1;
			this.autoScrollTimer.TimeElapsed += new EventHandler(this.HandleTimeElapsed);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.textRuler.Changed -= new EventHandler(this.HandleRulerChanged);
				this.autoScrollTimer.TimeElapsed -= new EventHandler(this.HandleTimeElapsed);
				this.autoScrollTimer.Dispose();
				this.autoScrollTimer = null;
			}
			
			base.Dispose(disposing);
		}


		public Document Document
		{
			get { return this.document; }
		}

		public DrawingContext DrawingContext
		{
			get { return this.drawingContext; }
		}

		public Selector Selector
		{
			get { return this.selector; }
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


		// Retourne le rectangle correspondant à la zone visible dans le Viewer.
		public Rectangle RectangleDisplayed
		{
			get
			{
				Rectangle rect = this.Client.Bounds;
				return ScreenToInternal(rect);
			}
		}

		
		#region AutoScroll
		// Démarre le timer pour l'auto-scroll.
		protected void AutoScrollTimerStart(Message message)
		{
			if ( this.document.Modifier.Tool == "Hand" )  return;
			this.autoScrollTimer.Start();
		}

		// Stoppe le timer pour l'auto-scroll.
		protected void AutoScrollTimerStop()
		{
			this.autoScrollTimer.Suspend();
		}

		// Appelé lorsque le timer arrive à échéance.
		// Effectue éventuellement un scroll si la souris est proche des bords.
		protected void HandleTimeElapsed(object sender)
		{
			Point mouse = this.mousePosWidget;

			Rectangle view = this.Client.Bounds;
			if ( this.textRuler != null && this.textRuler.IsVisible )
			{
				if ( this.textRuler.Top == view.Top )  // règle tout en haut ?
				{
					view.Top -= this.textRuler.Height;  // sous la règle
				}
			}
			//?view.Deflate(10);
			if ( view.Contains(mouse) )  return;

			Point move = new Point(0,0);

			if ( mouse.X > view.Right )
			{
				move.X = mouse.X-view.Right;
			}
			if ( mouse.X < view.Left )
			{
				move.X = mouse.X-view.Left;
			}
			if ( mouse.Y > view.Top )
			{
				move.Y = mouse.Y-view.Top;
			}
			if ( mouse.Y < view.Bottom )
			{
				move.Y = mouse.Y-view.Bottom;
			}

			move.X /= this.drawingContext.ScaleX;
			move.Y /= this.drawingContext.ScaleY;
			this.AutoScroll(move);

			mouse = this.ScreenToInternal(mouse);  // position en coordonnées internes
			this.DispatchDummyMouseMoveEvent();
		}

		// Retourne le rectangle correspondant à la zone visible dans le Viewer
		// dans laquelle il faut scroller.
		public Rectangle ScrollRectangle
		{
			get
			{
				Rectangle rect = this.Client.Bounds;
				if ( this.textRuler != null && this.textRuler.IsVisible )
				{
					if ( this.textRuler.Top == rect.Top )  // règle tout en haut ?
					{
						rect.Top -= this.textRuler.Height;  // sous la règle
					}
				}
				rect.Deflate(5);  // ch'tite marge
				return ScreenToInternal(rect);
			}
		}

		// Effectue un scroll automatique.
		public void AutoScroll(Point move)
		{
			if ( move.X == 0.0 && move.Y == 0.0 )  return;

			Point origin = new Point(this.drawingContext.OriginX, this.drawingContext.OriginY);
			origin -= move;
			origin.X = -System.Math.Max(-origin.X, this.drawingContext.MinOriginX);
			origin.X = -System.Math.Min(-origin.X, this.drawingContext.MaxOriginX);
			origin.Y = -System.Math.Max(-origin.Y, this.drawingContext.MinOriginY);
			origin.Y = -System.Math.Min(-origin.Y, this.drawingContext.MaxOriginY);
			this.drawingContext.Origin(origin);
		}
		#endregion


		// Gestion d'un événement.
		protected override void ProcessMessage(Message message, Point pos)
		{
			//System.Diagnostics.Debug.WriteLine(string.Format("Message: {0}", message.Type));
			if ( !this.IsActiveViewer )  return;

			Modifier modifier = this.document.Modifier;
			if ( modifier == null )  return;

			this.mousePosWidget = pos;
			pos = this.ScreenToInternal(pos);  // position en coordonnées internes

			if ( pos.X != this.mousePos.X || pos.Y != this.mousePos.Y )
			{
				this.mousePos = pos;
				this.mousePosValid = true;
				this.document.Notifier.NotifyMouseChanged();
			}

			this.drawingContext.IsShift = message.IsShiftPressed;
			this.drawingContext.IsCtrl  = message.IsCtrlPressed;
			this.drawingContext.IsAlt   = message.IsAltPressed;

			switch ( message.Type )
			{
				case MessageType.MouseDown:
					this.document.IsDirtySerialize = true;
					this.AutoScrollTimerStart(message);
					this.ProcessMouseDown(message, pos);
					this.mouseDown = true;
					break;
				
				case MessageType.MouseMove:
					this.ProcessMouseMove(message, pos);
					break;

				case MessageType.MouseUp:
					if ( this.mouseDown )
					{
						this.AutoScrollTimerStop();
						this.ProcessMouseUp(message, pos);
						this.mouseDown = false;
					}
					break;

				case MessageType.MouseLeave:
					this.mousePosValid = false;
					this.Hilite(null);
					break;

				case MessageType.MouseWheel:
					this.ProcessMouseWheel(message.Wheel, pos);
					break;

				case MessageType.KeyDown:
					if ( message.KeyCode == KeyCode.ShiftKey   ||
						 message.KeyCode == KeyCode.ControlKey )
					{
						this.UpdateMouseCursor(message);
					}

					if ( this.document.Modifier.Tool == "Select" )
					{
						this.MoveSelection(message);
					}

					this.EditProcessMessage(message, pos);

					if ( message.IsAltPressed || message.IsCtrlPressed )
					{
						// Il ne faut jamais manger les pressions de touches avec
						// ALT ou Ctrl, car elles sont utilisées par les raccourcis
						// clavier globaux.
						return;
					}
					if ( this.createRank != -1 )
					{
						if ( message.KeyCode == KeyCode.Escape )
						{
							this.CreateEnding(false);
						}
					}

					if ( modifier.RetEditObject() == null )
					{
						if ( message.KeyCode == KeyCode.Back   ||
							 message.KeyCode == KeyCode.Delete )
						{
							modifier.DeleteSelection();
						}
					}

					if ( message.KeyCode == KeyCode.FuncF12 )
					{
						this.DirtyAllViews();
					}
					break;

				case MessageType.KeyUp:
					if ( message.KeyCode == KeyCode.ShiftKey   ||
						 message.KeyCode == KeyCode.ControlKey )
					{
						this.UpdateMouseCursor(message);
					}

					this.EditProcessMessage(message, pos);
					break;

				default:
					if ( message.Type == MessageType.KeyPress )
					{
						if ( message.IsAltPressed || message.IsCtrlPressed )
						{
							// Il ne faut jamais manger les pressions de touches avec
							// ALT ou Ctrl, car elles sont utilisées par les raccourcis
							// clavier globaux.
							return;
						}
					}
					this.EditProcessMessage(message, pos);
					break;
			}
			
			this.document.Notifier.GenerateEvents();
			message.Consumer = this;
		}

		// Gestion d'un bouton de la souris pressé.
		protected void ProcessMouseDown(Message message, Point pos)
		{
			Modifier modifier = this.document.Modifier;

			if ( message.IsLeftButton )
			{
				if ( modifier.Tool == "Select" )
				{
					this.SelectMouseDown(pos, message.ButtonDownCount, false);
				}
				else if ( modifier.Tool == "Global" )
				{
					this.SelectMouseDown(pos, message.ButtonDownCount, true);
				}
				else if ( modifier.Tool == "Edit" )
				{
					this.EditMouseDown(message, pos, message.ButtonDownCount);
				}
				else if ( modifier.Tool == "Zoom" )
				{
					this.ZoomMouseDown(pos);
				}
				else if ( modifier.Tool == "Hand" )
				{
					this.HandMouseDown(pos);
				}
				else if ( modifier.Tool == "Picker" )
				{
					this.PickerMouseDown(pos);
				}
				else if ( modifier.Tool == "HotSpot" )
				{
					this.HotSpotMouseDown(pos);
				}
				else
				{
					this.CreateMouseDown(pos);
				}
			}

			if ( message.IsRightButton )
			{
				modifier.Tool = "Select";
				this.SelectMouseDown(pos, message.ButtonDownCount, false);
			}
		}

		// Gestion d'un déplacement de la souris pressé.
		protected void ProcessMouseMove(Message message, Point pos)
		{
			Modifier modifier = this.document.Modifier;

			if ( this.guideInteractive != -1 )
			{
				this.GuideInteractiveMove(pos);
				return;
			}

			if ( modifier.Tool == "Select" )
			{
				this.SelectMouseMove(pos, false);
			}
			else if ( modifier.Tool == "Global" )
			{
				this.SelectMouseMove(pos, true);
			}
			else if ( modifier.Tool == "Edit" )
			{
				this.EditMouseMove(message, pos);
			}
			else if ( modifier.Tool == "Zoom" )
			{
				this.ZoomMouseMove(pos);
			}
			else if ( modifier.Tool == "Hand" )
			{
				this.HandMouseMove(pos);
			}
			else if ( modifier.Tool == "Picker" )
			{
				this.PickerMouseMove(pos);
			}
			else if ( modifier.Tool == "HotSpot" )
			{
				this.HotSpotMouseMove(pos);
			}
			else
			{
				this.CreateMouseMove(pos);
			}

			this.UpdateMouseCursor(message);
		}

		// Gestion d'un bouton de la souris relâché.
		protected void ProcessMouseUp(Message message, Point pos)
		{
			Modifier modifier = this.document.Modifier;

			if ( this.guideInteractive != -1 )
			{
				this.GuideInteractiveEnd();
				return;
			}

			if ( modifier.Tool == "Select" )
			{
				this.SelectMouseUp(pos, message.IsRightButton, false);
			}
			else if ( modifier.Tool == "Global" )
			{
				this.SelectMouseUp(pos, message.IsRightButton, true);
			}
			else if ( modifier.Tool == "Edit" )
			{
				this.EditMouseUp(message, pos, message.IsRightButton);
			}
			else if ( modifier.Tool == "Zoom" )
			{
				this.ZoomMouseUp(pos);
			}
			else if ( modifier.Tool == "Hand" )
			{
				this.HandMouseUp(pos);
			}
			else if ( modifier.Tool == "Picker" )
			{
				this.PickerMouseUp(pos);
			}
			else if ( modifier.Tool == "HotSpot" )
			{
				this.HotSpotMouseUp(pos);
			}
			else
			{
				this.CreateMouseUp(pos);
			}
		}

		// Action lorsque la molette est actionnée.
		protected void ProcessMouseWheel(int wheel, Point pos)
		{
			if ( this.document.GlobalSettings.MouseWheelAction == Settings.MouseWheelAction.Zoom )
			{
				double zoom = this.document.GlobalSettings.DefaultZoom;
				double factor = (wheel > 0) ? zoom : 1.0/zoom;
				this.document.Modifier.ZoomChange(factor, pos);
			}

			if ( this.document.GlobalSettings.MouseWheelAction == Settings.MouseWheelAction.VScroll )
			{
				double move = 30.0/this.drawingContext.ScaleX;
				if ( wheel > 0 )  move = -move;

				Point origin = new Point();
				origin.X = this.drawingContext.OriginX;
				origin.Y = this.drawingContext.OriginY + move;

				origin.X = -System.Math.Max(-origin.X, this.drawingContext.MinOriginX);
				origin.X = -System.Math.Min(-origin.X, this.drawingContext.MaxOriginX);

				origin.Y = -System.Math.Max(-origin.Y, this.drawingContext.MinOriginY);
				origin.Y = -System.Math.Min(-origin.Y, this.drawingContext.MaxOriginY);
				
				this.drawingContext.Origin(origin);
			}
		}

		// Déplace la sélection en fonction des touches flèches, Shirt et Ctrl.
		protected void MoveSelection(Message message)
		{
			Point move = new Point(0,0);

			if ( message.KeyCode == KeyCode.ArrowRight )
			{
				move.X = this.document.Modifier.ArrowMove.X;
			}
			if ( message.KeyCode == KeyCode.ArrowLeft )
			{
				move.X = -this.document.Modifier.ArrowMove.X;
			}
			if ( message.KeyCode == KeyCode.ArrowUp )
			{
				move.Y = this.document.Modifier.ArrowMove.Y;
			}
			if ( message.KeyCode == KeyCode.ArrowDown )
			{
				move.Y = -this.document.Modifier.ArrowMove.Y;
			}

			if ( message.IsShiftPressed )
			{
				move *= this.document.Modifier.ArrowMoveMul;
			}
			if ( message.IsCtrlPressed )
			{
				move /= this.document.Modifier.ArrowMoveDiv;
			}

			if ( move.X != 0.0 || move.Y != 0.0 )
			{
				this.document.Modifier.MoveSelection(move);
			}
		}


		#region CreateMouse
		protected void CreateMouseDown(Point mouse)
		{
			this.drawingContext.SnapGrid(ref mouse);

			if ( this.createRank == -1 )
			{
				if ( this.document.Modifier.TotalSelected > 0 )
				{
					this.document.Modifier.DeselectAll();
				}

				this.document.Modifier.OpletQueueBeginAction("Create");
				Objects.Abstract obj = Objects.Abstract.CreateObject(this.document, this.document.Modifier.Tool, this.document.Modifier.ObjectMemoryTool);

				this.document.Modifier.OpletQueueEnable = false;
				Objects.Abstract layer = this.drawingContext.RootObject();
				this.createRank = layer.Objects.Add(obj);  // ajoute à la fin de la liste
			}

			if ( this.createRank != -1 )
			{
				Objects.Abstract layer = this.drawingContext.RootObject();
				Objects.Abstract obj = layer.Objects[this.createRank] as Objects.Abstract;

				obj.CreateMouseDown(mouse, this.drawingContext);
			}
		}

		protected void CreateMouseMove(Point mouse)
		{
			this.ChangeMouseCursor(MouseCursorType.Pen);
			//this.drawingContext.SnapGrid(ref mouse);

			if ( this.createRank == -1 )  return;

			Objects.Abstract layer = this.drawingContext.RootObject();
			Objects.Abstract obj = layer.Objects[this.createRank] as Objects.Abstract;

			obj.CreateMouseMove(mouse, this.drawingContext);
		}

		protected void CreateMouseUp(Point mouse)
		{
			//this.drawingContext.SnapGrid(ref mouse);

			if ( this.createRank == -1 )  return;

			Objects.Abstract layer = this.drawingContext.RootObject();
			Objects.Abstract obj = layer.Objects[this.createRank] as Objects.Abstract;

			obj.CreateMouseUp(mouse, this.drawingContext);

			bool selectAfterCreation = false;
			bool editAfterCreation = false;
			if ( obj.CreateIsEnding(this.drawingContext) )
			{
				if ( obj.CreateIsExist(this.drawingContext) )
				{
					layer.Objects.RemoveAt(this.createRank);

					this.document.Modifier.OpletQueueEnable = true;
					this.createRank = layer.Objects.Add(obj);  // ajoute à la fin de la liste
					this.document.Modifier.GroupUpdateParents();
					this.document.Modifier.OpletQueueValidateAction();

					selectAfterCreation = obj.SelectAfterCreation();
					editAfterCreation = obj.EditAfterCreation();
				}
				else
				{
					this.document.Notifier.NotifyArea(obj.BoundingBox);
					layer.Objects.RemoveAt(this.createRank);

					this.document.Modifier.OpletQueueEnable = true;
					obj.Dispose();
					this.document.Modifier.OpletQueueCancelAction();  // annule les propriétés
				}
				this.createRank = -1;
			}

			this.document.Notifier.NotifyCreateChanged();

			if ( selectAfterCreation )
			{
				this.document.Modifier.Tool = "Select";
			}
			if ( editAfterCreation )
			{
				this.document.Modifier.Tool = "Edit";
			}

			this.document.Notifier.NotifySelectionChanged();
		}

		// Si nécessaire, termine la création en cours.
		public void CreateEnding(bool delete)
		{
			if ( this.createRank == -1 )  return;

			Objects.Abstract layer = this.drawingContext.RootObject();
			Objects.Abstract obj = layer.Objects[this.createRank] as Objects.Abstract;

			this.document.Notifier.NotifyArea(obj.BoundingBox);
			if ( obj.CreateEnding(this.drawingContext) && !delete )
			{
				layer.Objects.RemoveAt(this.createRank);

				this.document.Modifier.OpletQueueEnable = true;
				this.createRank = layer.Objects.Add(obj);  // ajoute à la fin de la liste
				this.document.Modifier.GroupUpdateParents();
				this.document.Modifier.OpletQueueValidateAction();
			}
			else
			{
				layer.Objects.RemoveAt(this.createRank);

				this.document.Modifier.OpletQueueEnable = true;
				obj.Dispose();
				this.document.Modifier.OpletQueueCancelAction();  // annule les propriétés
			}
			this.createRank = -1;
			this.document.Notifier.NotifyCreateChanged();
			this.document.Notifier.NotifySelectionChanged();
		}

		// Indique s'il existe un objet en cours de création.
		public bool IsCreating
		{
			get
			{
				return ( this.createRank != -1 );
			}
		}

		// Retourne le rang de l'objet en cours de création.
		public int CreateRank()
		{
			return this.createRank;
		}
		#endregion


		#region SelectMouse
		protected void SelectMouseDown(Point mouse, int downCount, bool global)
		{
			this.document.Modifier.OpletQueueBeginAction();
			this.moveStart = mouse;
			this.moveAccept = false;
			this.drawingContext.ConstrainFixStarting(mouse);
			this.drawingContext.ConstrainFixType(ConstrainType.None);
			this.Hilite(null);
			this.selector.HiliteHandle(-1);
			this.HiliteHandle(null, -1);
			this.moveGlobal = -1;
			this.moveObject = null;
			this.cellObject = null;
			this.guideInteractive = -1;
			this.guideCreate = false;
			this.ctrlDuplicate = false;

			Objects.Abstract obj;
			int rank;
			if ( this.selector.Detect(mouse, !this.drawingContext.IsShift, out rank) )
			{
				if ( this.drawingContext.IsCtrl && rank == 0 )
				{
					this.document.Modifier.DuplicateSelection(new Point(0,0));
					this.document.Modifier.ActiveViewer.UpdateSelector();
					this.ctrlDuplicate = true;
				}
				this.moveGlobal = rank;
				this.selector.MoveStarting(this.moveGlobal, mouse, this.drawingContext);
				this.selector.HiliteHandle(this.moveGlobal);
				this.MoveGlobalStarting();
			}
			else
			{
				if ( this.selector.Visible && !this.drawingContext.IsShift )
				{
					this.selector.Visible = false;
					this.Select(null, false, false);
				}

				if ( this.DetectHandle(mouse, out obj, out rank) )
				{
					this.moveObject = obj;
					this.moveHandle = rank;
					this.moveOffset = mouse-obj.GetHandlePosition(rank);
					this.moveObject.MoveHandleStarting(this.moveHandle, mouse, this.drawingContext);
					this.HiliteHandle(this.moveObject, this.moveHandle);
					this.drawingContext.ConstrainFixStarting(obj.GetHandlePosition(rank));
				}
				else if ( this.GuideDetect(mouse, out rank) )
				{
					this.guideInteractive = rank;
					this.document.Dialogs.SelectGuide(this.guideInteractive);
				}
				else
				{
					obj = this.Detect(mouse, true);
					if ( obj == null )
					{
						this.selector.FixStarting(mouse);
					}
					else
					{
						if ( !obj.IsSelected )
						{
							if ( global )
							{
								this.selector.FixStarting(mouse);
							}
							else
							{
								this.Select(obj, false, this.drawingContext.IsShift);

								if ( this.selector.Visible && this.drawingContext.IsShift )
								{
									obj.GlobalSelect(true);
									this.SelectorInitialize(this.document.Modifier.SelectedBbox);
								}
							}
						}
						else
						{
							if ( this.drawingContext.IsShift )
							{
								obj.Deselect();
								this.document.Modifier.TotalSelected --;
							}
						}

						if ( this.drawingContext.IsCtrl && obj.IsSelected )
						{
							this.document.Modifier.DuplicateSelection(new Point(0,0));
							this.document.Modifier.ActiveViewer.UpdateSelector();
							this.ctrlDuplicate = true;
						}

						this.moveObject = obj;
						this.moveHandle = -1;  // déplace tout l'objet
						this.drawingContext.SnapGrid(ref mouse);
						this.moveOffset = mouse;
						this.MoveAllStarting();
					}
				}
			}
		}

		protected void SelectMouseMove(Point mouse, bool global)
		{
			this.HiliteHandle(null, -1);
			this.selector.HiliteHandle(-1);

			if ( this.mouseDown )  // bouton souris pressé ?
			{
				if ( this.selector.Visible && !this.selector.Handles )
				{
					this.selector.FixEnding(mouse);
				}
				else if ( this.moveGlobal != -1 )  // déplace le modificateur global ?
				{
					this.selector.MoveProcess(this.moveGlobal, mouse, this.drawingContext);
					this.selector.HiliteHandle(this.moveGlobal);
					this.MoveGlobalProcess(this.selector);
				}
				else if ( this.moveObject != null )
				{
					this.drawingContext.ConstrainFixType(ConstrainType.Normal);

					if ( this.moveHandle != -1 )  // déplace une poignée ?
					{
						mouse -= this.moveOffset;
						this.MoveHandleProcess(this.moveObject, this.moveHandle, mouse);
						this.HiliteHandle(this.moveObject, this.moveHandle);
					}
					else	// déplace tout l'objet ?
					{
						this.drawingContext.ConstrainSnapPos(ref mouse);

						if ( !this.moveAccept )
						{
							double len = Point.Distance(mouse, this.moveStart);
							if ( len <= this.drawingContext.MinimalSize )
							{
								mouse = this.moveStart;
							}
							else
							{
								this.moveAccept = true;
							}
						}
						this.drawingContext.SnapGrid(ref mouse);
						this.MoveAllProcess(mouse-this.moveOffset);
						this.moveOffset = mouse;
					}
				}
				else if ( this.cellObject != null )
				{
					this.cellRank = this.cellObject.DetectCell(mouse);
					this.cellObject.MoveCellProcess(this.cellRank, mouse, this.drawingContext);
				}
			}
			else	// bouton souris relâché ?
			{
				Objects.Abstract hiliteObj = this.Detect(mouse, true);
				this.document.Modifier.ContainerHilite(hiliteObj);

				Objects.Abstract obj;
				int rank;
				int guideRank = -1;

				if ( this.selector.Detect(mouse, !this.drawingContext.IsShift, out rank) )
				{
					this.Hilite(null);
					this.selector.HiliteHandle(rank);
					this.ChangeMouseCursor(MouseCursorType.Finger);
				}
				else if ( this.DetectHandle(mouse, out obj, out rank) )
				{
					this.Hilite(null);
					this.HiliteHandle(obj, rank);
					this.ChangeMouseCursor(MouseCursorType.Finger);
				}
				else if ( !global && this.GuideDetect(mouse, out guideRank) )
				{
					this.ChangeMouseCursor(MouseCursorType.Finger);
				}
				else
				{
					obj = hiliteObj;
					if ( !global )
					{
						this.Hilite(obj);
					}
					if ( obj == null )
					{
						this.ChangeMouseCursor(MouseCursorType.Arrow);
					}
					else
					{
						if ( obj.IsSelected )
						{
							this.ChangeMouseCursor(MouseCursorType.Finger);
						}
						else
						{
							this.ChangeMouseCursor(MouseCursorType.Arrow);
						}
					}
				}

				this.GuideHilite(guideRank);
			}
		}

		protected void SelectMouseUp(Point mouse, bool isRight, bool global)
		{
			bool globalMenu = false;
			this.selector.HiliteHandle(-1);  // supprime tous les hilites
			this.HiliteHandle(null, -1);

			if ( this.selector.Visible && !this.selector.Handles )
			{
				double len = Point.Distance(mouse, this.moveStart);
				if ( isRight && len <= this.drawingContext.MinimalSize )
				{
					this.selector.Visible = false;
					globalMenu = true;
				}
				else
				{
					Rectangle rSelect = this.selector.Rectangle;
					this.Select(rSelect, this.drawingContext.IsShift, this.partialSelect);
				}
			}
			else if ( this.moveGlobal != -1 )  // déplace le modificateur global ?
			{
				this.document.Modifier.GroupUpdateChildrens();
				this.document.Modifier.GroupUpdateParents();
				this.MoveGlobalEnding();
			}
			else if ( this.moveObject != null )
			{
				this.document.Modifier.GroupUpdateChildrens();
				this.document.Modifier.GroupUpdateParents();

				this.moveObject = null;
				this.moveHandle = -1;
				this.UpdateSelector();
			}
			else if ( this.cellObject != null )
			{
				this.cellObject = null;
				this.cellRank   = -1;
			}

			this.drawingContext.ConstrainDelStarting();
			this.document.Notifier.GenerateEvents();
			this.document.Modifier.OpletQueueValidateAction();

			if ( this.ctrlDuplicate )
			{
				double len = Point.Distance(mouse, this.moveStart);
				if ( len <= this.drawingContext.MinimalSize )
				{
					this.document.Modifier.Undo();
					return;
				}
			}

			if ( isRight )  // avec le bouton de droite de la souris ?
			{
				this.ContextMenu(mouse, globalMenu);
			}
		}
		#endregion


		#region EditMouse
		protected void EditMouseDown(Message message, Point mouse, int downCount)
		{
			Objects.Abstract obj = this.DetectEdit(mouse);

			if ( obj != this.document.Modifier.RetEditObject() )
			{
				this.document.Modifier.OpletQueueBeginAction();
				this.Select(obj, true, false);
				this.document.Modifier.OpletQueueValidateAction();
			}

			this.EditProcessMessage(message, mouse);
		}

		protected void EditMouseMove(Message message, Point mouse)
		{
			this.ChangeMouseCursor(MouseCursorType.IBeam);

			if ( this.mouseDown )  // bouton souris pressé ?
			{
				this.EditProcessMessage(message, mouse);
			}
			else	// bouton souris relâché ?
			{
				Objects.Abstract obj = this.DetectEdit(mouse);
				this.Hilite(obj);

				this.ChangeMouseCursor(MouseCursorType.IBeam);
			}
		}

		protected void EditMouseUp(Message message, Point mouse, bool isRight)
		{
			this.EditProcessMessage(message, mouse);
		}

		protected void EditProcessMessage(Message message, Point pos)
		{
			Objects.Abstract editObject = this.document.Modifier.RetEditObject();
			if ( editObject == null )  return;

			if ( editObject.EditProcessMessage(message, pos) )
			{
				this.document.Notifier.NotifyArea(editObject.BoundingBox);
			}
		}
		#endregion


		#region ZoomMouse
		protected void ZoomMouseDown(Point mouse)
		{
			this.document.Modifier.OpletQueueEnable = false;
			this.moveStart = mouse;
			this.zoomer.FixStarting(mouse);
		}
		
		protected void ZoomMouseMove(Point mouse)
		{
			this.ChangeMouseCursor(MouseCursorType.Zoom);

			if ( this.mouseDown )
			{
				this.zoomer.FixEnding(mouse);
			}
		}
		
		protected void ZoomMouseUp(Point mouse)
		{
			this.zoomer.Visible = false;
			Rectangle rect = this.zoomer.Rectangle;

			if ( this.drawingContext.IsShift || this.drawingContext.IsCtrl )
			{
				this.document.Modifier.ZoomChange(0.5, rect.Center);
			}
			else
			{
				double len = Point.Distance(mouse, this.moveStart);
				if ( len <= this.drawingContext.MinimalSize )
				{
					this.document.Modifier.ZoomChange(2.0, rect.Center);
				}
				else
				{
					this.document.Modifier.ZoomChange(rect.BottomLeft, rect.TopRight);
				}
			}

			this.document.Modifier.OpletQueueEnable = true;
		}
		#endregion


		#region HandMouse
		protected void HandMouseDown(Point mouse)
		{
			mouse.X += this.drawingContext.OriginX;
			mouse.Y += this.drawingContext.OriginY;
			this.handMouseStart = mouse;
		}

		protected void HandMouseMove(Point mouse)
		{
			this.ChangeMouseCursor(MouseCursorType.Hand);

			if ( this.mouseDown )
			{
				mouse.X += this.drawingContext.OriginX;
				mouse.Y += this.drawingContext.OriginY;
				Point move = mouse-this.handMouseStart;
				this.handMouseStart = mouse;

				Point origin = new Point();
				origin.X = this.drawingContext.OriginX + move.X;
				origin.Y = this.drawingContext.OriginY + move.Y;

				origin.X = -System.Math.Max(-origin.X, this.drawingContext.MinOriginX);
				origin.X = -System.Math.Min(-origin.X, this.drawingContext.MaxOriginX);

				origin.Y = -System.Math.Max(-origin.Y, this.drawingContext.MinOriginY);
				origin.Y = -System.Math.Min(-origin.Y, this.drawingContext.MaxOriginY);
				
				this.drawingContext.Origin(origin);
			}
		}

		protected void HandMouseUp(Point mouse)
		{
		}
		#endregion


		#region PickerMouse
		protected void PickerMouseDown(Point mouse)
		{
			Objects.Abstract model = this.Detect(mouse, false);
			if ( model != null )
			{
				if ( this.document.Modifier.ActiveContainer is Containers.Styles )
				{
					this.document.Modifier.PickerProperty(model);
				}
				else if ( this.document.Modifier.TotalSelected == 0 )
				{
					this.document.Modifier.ObjectMemory.PickerProperties(model);
					this.document.Modifier.ObjectMemoryText.PickerProperties(model);
				}
				else
				{
					using ( this.document.Modifier.OpletQueueBeginAction() )
					{
						Objects.Abstract layer = this.drawingContext.RootObject();
						foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
						{
							if ( obj == model )  continue;
							this.document.Notifier.NotifyArea(obj.BoundingBox);
							obj.PickerProperties(model);
							this.document.Notifier.NotifyArea(obj.BoundingBox);
						}
						this.document.Notifier.NotifySelectionChanged();
						this.document.Modifier.OpletQueueValidateAction();
					}
				}
			}
		}

		protected void PickerMouseMove(Point mouse)
		{
			Objects.Abstract model = this.Detect(mouse, false);
			if ( model == null )
			{
				this.ChangeMouseCursor(MouseCursorType.PickerEmpty);
			}
			else
			{
				this.ChangeMouseCursor(MouseCursorType.Picker);
			}
		}

		protected void PickerMouseUp(Point mouse)
		{
		}
		#endregion

		
		#region HotSpotMouse
		protected void HotSpotMouseDown(Point mouse)
		{
			this.drawingContext.SnapGrid(ref mouse);
			this.document.HotSpot = mouse;
			this.document.Notifier.NotifyArea(this);
		}

		protected void HotSpotMouseMove(Point mouse)
		{
			this.ChangeMouseCursor(MouseCursorType.Finger);

			if ( this.mouseDown )
			{
				this.drawingContext.SnapGrid(ref mouse);
				this.document.HotSpot = mouse;
				this.document.Notifier.NotifyArea(this);
			}
		}

		protected void HotSpotMouseUp(Point mouse)
		{
		}
		#endregion

		
		// Détecte l'objet pointé par la souris.
		protected Objects.Abstract Detect(Point mouse, bool selectFirst)
		{
			Objects.Abstract layer = this.drawingContext.RootObject();

			if ( selectFirst )
			{
				foreach ( Objects.Abstract obj in this.document.FlatReverse(layer) )
				{
					if ( !obj.IsSelected )  continue;
					if ( obj.Detect(mouse) )  return obj;
				}
			}

			foreach ( Objects.Abstract obj in this.document.FlatReverse(layer) )
			{
				if ( obj.Detect(mouse) )  return obj;
			}
			return null;
		}

		// Détecte l'objet éditable pointé par la souris.
		protected Objects.Abstract DetectEdit(Point mouse)
		{
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.FlatReverse(layer) )
			{
				if ( obj.DetectEdit(mouse) )  return obj;
			}
			return null;
		}

		// Détecte la poignée pointée par la souris.
		protected bool DetectHandle(Point mouse, out Objects.Abstract detect, out int rank)
		{
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.FlatReverse(layer, true) )
			{
				rank = obj.DetectHandle(mouse);
				if ( rank != -1 )
				{
					detect = obj;
					return true;
				}
			}

			detect = null;
			rank = -1;
			return false;
		}

		// Hilite un objet.
		protected void Hilite(Objects.Abstract item)
		{
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer) )
			{
				if ( obj.IsHilite != (obj == item) )
				{
					obj.IsHilite = (obj == item);
				}
			}
		}

		// Survolle une poignée.
		protected void HiliteHandle(Objects.Abstract obj, int rank)
		{
			if ( this.hiliteHandleObject != null )
			{
				this.hiliteHandleObject.HandleHilite(this.hiliteHandleRank, false);
			}

			this.hiliteHandleObject = obj;
			this.hiliteHandleRank = rank;

			if ( this.hiliteHandleObject != null )
			{
				this.hiliteHandleObject.HandleHilite(this.hiliteHandleRank, true);
			}
		}

		// Déplace une poignée d'un objet.
		protected void MoveHandleProcess(Objects.Abstract obj, int rank, Point pos)
		{
			obj.MoveHandleProcess(rank, pos, this.drawingContext);
		}

		// Mode de sélection.
		public SelectorType SelectorType
		{
			get
			{
				return this.selector.TypeChoice;
			}

			set
			{
				using ( this.document.Modifier.OpletQueueBeginAction() )
				{
					this.selector.TypeChoice = value;
					this.UpdateSelector();
					this.document.Notifier.NotifySelectionChanged();
					this.document.Modifier.OpletQueueValidateAction();
				}
			}
		}

		// Mode de sélection partiel (objets élastiques).
		public bool PartialSelect
		{
			get
			{
				return this.partialSelect;
			}

			set
			{
				if ( this.partialSelect != value )
				{
					this.partialSelect = value;
					this.document.Notifier.NotifySelectionChanged();
				}
			}
		}

		// Met à jour le selector en fonction des objets sélectionnés.
		public void UpdateSelector()
		{
			this.UpdateSelector(this.document.Modifier.SelectedBbox);
		}

		public void UpdateSelector(Rectangle rect)
		{
			if ( this.document.Modifier.TotalSelected == 0 )
			{
				this.selector.Visible = false;
				this.selector.Handles = false;
			}
			else
			{
				SelectorType st = this.selector.TypeChoice;
				if ( st == SelectorType.Auto )
				{
					if ( this.document.Modifier.TotalSelected == 1 )
					{
						st = SelectorType.Individual;
					}
					else
					{
						st = SelectorType.Zoomer;
					}
				}

				if ( st == SelectorType.Individual )
				{
					this.selector.Visible = false;
					this.selector.Handles = false;
					this.GlobalSelectUpdate(false);
				}
				else
				{
					this.selector.TypeInUse = st;
					this.SelectorInitialize(rect);
					this.GlobalSelectUpdate(true);
				}
			}
		}

		// Initialise le rectangle du selector.
		protected void SelectorInitialize(Drawing.Rectangle rect)
		{
			rect.Inflate(10.0/this.drawingContext.ScaleX);
			this.selector.Initialize(rect);
		}

		// Indique si tous les objets sélectionnés le sont globalement.
		protected void GlobalSelectUpdate(bool global)
		{
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
			{
				obj.GlobalSelect(global);
			}
		}

		// Sélectionne un objet et désélectionne tous les autres.
		public void Select(Objects.Abstract item, bool edit, bool add)
		{
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer) )
			{
				if ( obj == item )
				{
					if ( !obj.IsSelected || obj.IsEdited != edit )
					{
						obj.Select(true, edit);
						this.document.Modifier.TotalSelected ++;
					}
				}
				else
				{
					if ( !add )
					{
						if ( obj.IsSelected )
						{
							obj.Deselect();
							this.document.Modifier.TotalSelected --;
						}
					}
				}
			}
		}

		// Sélectionne tous les objets dans le rectangle.
		// partial = false -> toutes les poignées doivent être dans le rectangle
		// partial = true  -> une seule poignée doit être dans le rectangle
		protected void Select(Rectangle rect, bool add, bool partial)
		{
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer) )
			{
				if ( obj.Detect(rect, partial) )
				{
					if ( !obj.IsSelected )
					{
						this.document.Modifier.TotalSelected ++;
					}
					obj.Select(rect);
				}
				else
				{
					if ( !add )
					{
						if ( obj.IsSelected )
						{
							obj.Deselect();
							this.document.Modifier.TotalSelected --;
						}
					}
				}
			}

			if ( this.document.Modifier.TotalSelected == 0 )
			{
				this.selector.Visible = false;
				this.selector.Handles = false;
			}
			else
			{
				Rectangle bbox = this.document.Modifier.SelectedBbox;
				if ( !rect.Contains(bbox) && !partial )
				{
					rect.MergeWith(bbox);
				}
				this.UpdateSelector(rect);
			}
		}

		// Début du déplacement de tous les objets sélectionnés.
		protected void MoveAllStarting()
		{
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Deep(layer, true) )
			{
				obj.MoveAllStarting();
			}
		}

		// Effectue le déplacement de tous les objets sélectionnés.
		protected void MoveAllProcess(Point move)
		{
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Deep(layer, true) )
			{
				obj.MoveAllProcess(move);
			}
		}

		// Début du déplacement global de tous les objets sélectionnés.
		public void MoveGlobalStarting()
		{
			this.selector.FinalToInitialData();

			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Deep(layer, true) )
			{
				obj.MoveGlobalStarting();
			}
		}

		// Effectue le déplacement global de tous les objets sélectionnés.
		public void MoveGlobalProcess(Selector selector)
		{
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Deep(layer, true) )
			{
				obj.MoveGlobalProcess(selector);
			}
		}

		// Termine le déplacement global de tous les objets sélectionnés.
		public void MoveGlobalEnding()
		{
			if ( this.selector.TypeInUse != SelectorType.Stretcher )  return;

			this.document.Notifier.NotifyArea(this.selector.Rectangle);
			this.SelectorInitialize(this.document.Modifier.SelectedBbox);
			this.document.Notifier.NotifyArea(this.selector.Rectangle);
		}

		// Dessine les poignées de tous les objets.
		public void DrawHandle(Graphics graphics)
		{
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer) )
			{
				obj.DrawHandle(graphics, this.drawingContext);
			}
		}


		// Retourne la position de la souris.
		public bool MousePos(out Point pos)
		{
			pos = this.mousePos;
			return this.mousePosValid;
		}

		// Indique si on est dans le viewer actif.
		protected bool IsActiveViewer
		{
			get
			{
				return ( this == this.document.Modifier.ActiveViewer );
			}
		}


		#region ContextMenu
		// Construit le menu contextuel.
		protected void ContextMenu(Point mouse, bool globalMenu)
		{
			this.Hilite(null);

			int nbSel = this.document.Modifier.TotalSelected;
			System.Collections.ArrayList list = new System.Collections.ArrayList();
			if ( globalMenu || nbSel == 0 )
			{
				this.MenuAddItem(list, "Deselect",     "manifest:Epsitec.App.DocumentEditor.Images.Deselect.icon",     "Désélectionner tout");
				this.MenuAddItem(list, "SelectAll",    "manifest:Epsitec.App.DocumentEditor.Images.SelectAll.icon",    "Tout sélectionner");
				this.MenuAddItem(list, "SelectInvert", "manifest:Epsitec.App.DocumentEditor.Images.SelectInvert.icon", "Inverser la sélection");
				this.MenuAddSep(list);
				this.MenuAddItem(list, "HideSel",      "manifest:Epsitec.App.DocumentEditor.Images.HideSel.icon",      "Cacher la sélection");
				this.MenuAddItem(list, "HideRest",     "manifest:Epsitec.App.DocumentEditor.Images.HideRest.icon",     "Cacher le reste");
				this.MenuAddItem(list, "HideCancel",   "manifest:Epsitec.App.DocumentEditor.Images.HideCancel.icon",   "Montrer tout");
				this.MenuAddSep(list);
				this.MenuAddItem(list, "ZoomMin",      "manifest:Epsitec.App.DocumentEditor.Images.ZoomMin.icon",      "Zoom minimal");
				this.MenuAddItem(list, "ZoomDefault",  "manifest:Epsitec.App.DocumentEditor.Images.ZoomDefault.icon",  "Zoom 100%");
				this.MenuAddItem(list, "ZoomSel",      "manifest:Epsitec.App.DocumentEditor.Images.ZoomSel.icon",      "Zoom sélection");
				this.MenuAddItem(list, "ZoomPrev",     "manifest:Epsitec.App.DocumentEditor.Images.ZoomPrev.icon",     "Zoom précédent");
				this.MenuAddItem(list, "ZoomSub",      "manifest:Epsitec.App.DocumentEditor.Images.ZoomSub.icon",      "Réduction");
				this.MenuAddItem(list, "ZoomAdd",      "manifest:Epsitec.App.DocumentEditor.Images.ZoomAdd.icon",      "Agrandissement");
				this.MenuAddSep(list);
				this.MenuAddItem(list, "Outside",      "manifest:Epsitec.App.DocumentEditor.Images.Outside.icon",      "Sortir du groupe");
				this.MenuAddItem(list, "SelectMode",   "manifest:Epsitec.App.DocumentEditor.Images.SelectMode.icon",   "Sélection partielle");
				this.MenuAddItem(list, "Grid",         "manifest:Epsitec.App.DocumentEditor.Images.Grid.icon",         "Grille magnétique");
			}
			else
			{
				this.contextMenuPos = mouse;
				this.DetectHandle(mouse, out this.contextMenuObject, out this.contextMenuRank);
				if ( nbSel == 1 && this.contextMenuObject == null )
				{
					this.contextMenuObject = this.document.Modifier.RetOnlySelectedObject();
					this.contextMenuRank = -1;
				}

				this.MenuAddItem(list, "Delete",    "manifest:Epsitec.App.DocumentEditor.Images.Delete.icon",    "Supprimer");
				this.MenuAddItem(list, "Duplicate", "manifest:Epsitec.App.DocumentEditor.Images.Duplicate.icon", "Dupliquer");
				this.MenuAddItem(list, "OrderUp",   "manifest:Epsitec.App.DocumentEditor.Images.OrderUp.icon",   "Dessus");
				this.MenuAddItem(list, "OrderDown", "manifest:Epsitec.App.DocumentEditor.Images.OrderDown.icon", "Dessous");
				this.MenuAddItem(list, "Merge",     "manifest:Epsitec.App.DocumentEditor.Images.Merge.icon",     "Fusionner");
				this.MenuAddItem(list, "Group",     "manifest:Epsitec.App.DocumentEditor.Images.Group.icon",     "Associer");
				this.MenuAddItem(list, "Ungroup",   "manifest:Epsitec.App.DocumentEditor.Images.Ungroup.icon",   "Dissocier");
				this.MenuAddItem(list, "Inside",    "manifest:Epsitec.App.DocumentEditor.Images.Inside.icon",    "Entrer dans groupe");
				this.MenuAddItem(list, "Outside",   "manifest:Epsitec.App.DocumentEditor.Images.Outside.icon",   "Sortir du groupe");
				this.MenuAddSep(list);
				this.MenuAddItem(list, "ZoomSel",   "manifest:Epsitec.App.DocumentEditor.Images.ZoomSel.icon",   "Zoom sélection");
				if ( this.document.Type != DocumentType.Pictogram )
				{
					this.MenuAddItem(list, "ZoomSelWidth", "manifest:Epsitec.App.DocumentEditor.Images.ZoomSelWidth.icon", "Zoom largeur sélection");
				}
				this.MenuAddSep(list);
				this.MenuAddItem(list, "HideSel",    "manifest:Epsitec.App.DocumentEditor.Images.HideSel.icon",    "Cacher la sélection");
				this.MenuAddItem(list, "HideRest",   "manifest:Epsitec.App.DocumentEditor.Images.HideRest.icon",   "Cacher le reste");
				this.MenuAddItem(list, "HideCancel", "manifest:Epsitec.App.DocumentEditor.Images.HideCancel.icon", "Montrer tout");
				this.MenuAddSep(list);
				this.MenuAddItem(list, "Combine",   "manifest:Epsitec.App.DocumentEditor.Images.Combine.icon",   "Combiner");
				this.MenuAddItem(list, "Uncombine", "manifest:Epsitec.App.DocumentEditor.Images.Uncombine.icon", "Scinder");
				this.MenuAddItem(list, "ToBezier",  "manifest:Epsitec.App.DocumentEditor.Images.ToBezier.icon",  "Convertir en courbes");
				this.MenuAddItem(list, "ToPoly",    "manifest:Epsitec.App.DocumentEditor.Images.ToPoly.icon",    "Convertir en droites");
				this.MenuAddItem(list, "Fragment",  "manifest:Epsitec.App.DocumentEditor.Images.Fragment.icon",  "Fragmenter");

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
			mouse = this.InternalToScreen(mouse);
			mouse = this.MapClientToScreen(mouse);
			this.CommandDispatcher.SyncCommandStates();

			ScreenInfo si = ScreenInfo.Find(mouse);
			Drawing.Rectangle wa = si.WorkingArea;
			if ( mouse.Y-this.contextMenu.Height < wa.Bottom )
			{
				mouse.Y = wa.Bottom+this.contextMenu.Height;
			}
			
			this.contextMenu.ShowAsContextMenu(this.Window, mouse);
		}

		// Ajoute une case dans le menu.
		protected void MenuAddItem(System.Collections.ArrayList list, string cmd, string icon, string text)
		{
			CommandDispatcher.CommandState state = this.CommandDispatcher[cmd];
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

		// Ajoute un séparateur dans le menu.
		protected void MenuAddSep(System.Collections.ArrayList list)
		{
			ContextMenuItem item = new ContextMenuItem();
			list.Add(item);  // séparateur
		}

		// Exécute une commande locale à un objet.
		public void CommandObject(string cmd)
		{
			if ( cmd == "CreateEnding" )
			{
				this.CreateEnding(false);
				return;
			}

			if ( cmd == "CreateAndSelect" )
			{
				this.CreateEnding(false);
				this.document.Modifier.Tool = "Select";
				return;
			}

			this.document.Notifier.NotifyArea(this.contextMenuObject.BoundingBox);
			this.contextMenuObject.ContextCommand(cmd, this.contextMenuPos, this.contextMenuRank);
			this.document.Notifier.NotifyArea(this.contextMenuObject.BoundingBox);
		}
		#endregion

		
		protected override void UpdateClientGeometry()
		{
			if ( this.drawingContext == null )  return;
			Point center = this.drawingContext.Center;
			base.UpdateClientGeometry();
			this.UpdateRulerGeometry();
			this.drawingContext.ZoomAndCenter(this.drawingContext.Zoom, center);
		}


		#region TextRuler
		// Appelé lorsque la règle est changée.
		private void HandleRulerChanged(object sender)
		{
			Objects.Abstract editObject = this.document.Modifier.RetEditObject();
			if ( editObject == null )  return;
			this.document.Notifier.NotifyArea(editObject.BoundingBox);
		}

		// Positionne la règle en fonction de l'éventuel objet en cours d'édition.
		public void UpdateRulerGeometry()
		{
			if ( this.textRuler == null )  return;

			Objects.Abstract editObject = this.document.Modifier.RetEditObject();
			if ( editObject == null )
			{
				this.textRuler.SetVisible(false);
				this.textRuler.DetachFromText();
			}
			else
			{
				if ( editObject.EditRulerLink(this.textRuler, this.drawingContext) )
				{
					Rectangle editRect = editObject.BoundingBoxThin;
					Rectangle rulerRect = new Rectangle();
					Point p1 = this.InternalToScreen(editRect.TopLeft);
					Point p2 = this.InternalToScreen(editRect.TopRight);
					rulerRect.BottomLeft = p1;
					rulerRect.Width = System.Math.Max(p2.X-p1.X, this.textRuler.MinimalWidth);
					rulerRect.Height = this.textRuler.DefaultHeight;
					rulerRect.RoundFloor();
					if ( rulerRect.Top > this.Height )
					{
						rulerRect.Offset(0, this.Height-rulerRect.Top);
					}
					this.textRuler.Bounds = rulerRect;

					if ( p2.X-p1.X < this.textRuler.MinimalWidth )
					{
						this.textRuler.RightMargin += this.textRuler.MinimalWidth-(p2.X-p1.X);
					}

					this.textRuler.PPM = this.document.Modifier.RealScale;
					this.textRuler.Scale = this.drawingContext.ScaleX;
					this.textRuler.SetVisible(true);
				}
				else
				{
					this.textRuler.SetVisible(false);
					this.textRuler.DetachFromText();
				}
			}
		}
		#endregion


		#region MouseCursor
		// Adapte le sprite de la souris en fonction des touches Shift et Ctrl.
		protected void UpdateMouseCursor(Message message)
		{
			if ( this.lastMouseCursorType == MouseCursorType.Arrow     ||
				 this.lastMouseCursorType == MouseCursorType.ArrowPlus )
			{
				if ( message.IsShiftPressed )
				{
					this.ChangeMouseCursor(MouseCursorType.ArrowPlus);
				}
				else
				{
					this.ChangeMouseCursor(MouseCursorType.Arrow);
				}
			}

			if ( this.lastMouseCursorType == MouseCursorType.Finger    ||
				 this.lastMouseCursorType == MouseCursorType.FingerDup )
			{
				if ( message.IsCtrlPressed )
				{
					this.ChangeMouseCursor(MouseCursorType.FingerDup);
				}
				else
				{
					this.ChangeMouseCursor(MouseCursorType.Finger);
				}
			}
		}

		// Change le sprite de la souris.
		protected void ChangeMouseCursor(MouseCursorType cursor)
		{
			if ( this.lastMouseCursorType == cursor )  return;

			switch ( cursor )
			{
				case MouseCursorType.Arrow:
					this.MouseCursorImage(ref this.mouseCursorArrow, "manifest:Epsitec.App.DocumentEditor.Images.Arrow.icon");
					break;

				case MouseCursorType.ArrowPlus:
					this.MouseCursorImage(ref this.mouseCursorArrowPlus, "manifest:Epsitec.App.DocumentEditor.Images.ArrowPlus.icon");
					break;

				case MouseCursorType.ArrowDup:
					this.MouseCursorImage(ref this.mouseCursorArrowDup, "manifest:Epsitec.App.DocumentEditor.Images.ArrowDup.icon");
					break;

				case MouseCursorType.Finger:
					this.MouseCursorImage(ref this.mouseCursorFinger, "manifest:Epsitec.App.DocumentEditor.Images.Finger.icon");
					break;

				case MouseCursorType.FingerDup:
					this.MouseCursorImage(ref this.mouseCursorFingerDup, "manifest:Epsitec.App.DocumentEditor.Images.FingerDup.icon");
					break;

				case MouseCursorType.Cross:
					this.MouseCursor = MouseCursor.AsCross;
					break;

				case MouseCursorType.IBeam:
					this.MouseCursor = MouseCursor.AsIBeam;
					break;

				case MouseCursorType.Hand:
					this.MouseCursorImage(ref this.mouseCursorHand, "manifest:Epsitec.App.DocumentEditor.Images.Hand.icon");
					break;

				case MouseCursorType.Pen:
					this.MouseCursorImage(ref this.mouseCursorPen, "manifest:Epsitec.App.DocumentEditor.Images.Pen.icon");
					break;

				case MouseCursorType.Zoom:
					this.MouseCursorImage(ref this.mouseCursorZoom, "manifest:Epsitec.App.DocumentEditor.Images.Zoom.icon");
					break;

				case MouseCursorType.Picker:
					this.MouseCursorImage(ref this.mouseCursorPicker, "manifest:Epsitec.App.DocumentEditor.Images.Picker.icon");
					break;

				case MouseCursorType.PickerEmpty:
					this.MouseCursorImage(ref this.mouseCursorPickerEmpty, "manifest:Epsitec.App.DocumentEditor.Images.PickerEmpty.icon");
					break;

				default:
					this.MouseCursor = MouseCursor.AsArrow;
					break;
			}

			this.Window.MouseCursor = this.MouseCursor;
			this.lastMouseCursorType = cursor;
		}

		// Choix du sprite de la souris.
		protected void MouseCursorImage(ref Image image, string name)
		{
			if ( image == null )
			{
				image = Support.Resources.DefaultManager.GetImage(name);
			}
			
			this.MouseCursor = MouseCursor.FromImage(image);
		}
		#endregion


		#region GuideInteractive
		// Détecte le guide pointé par la souris.
		protected bool GuideDetect(Point pos, out int rank)
		{
			if ( !this.drawingContext.GuidesMouse ||
				 !this.drawingContext.GuidesShow  )
			{
				rank = -1;
				return false;
			}

			double margin = this.drawingContext.GuideMargin/2;
			int total = this.document.Settings.GuidesCount;
			for ( int i=total-1 ; i>=0 ; i-- )
			{
				Settings.Guide guide = this.document.Settings.GuidesGet(i);
				double gpos = guide.AbsolutePosition;

				if ( guide.IsHorizontal )  // repère horizontal ?
				{
					if ( pos.Y >= gpos-margin && pos.Y <= gpos+margin )
					{
						rank = i;
						return true;
					}
				}
				else	// repère vertical ?
				{
					if ( pos.X >= gpos-margin && pos.X <= gpos+margin )
					{
						rank = i;
						return true;
					}
				}
			}
			rank = -1;
			return false;
		}

		// Met en évidence le guide survolé par la souris.
		protected void GuideHilite(int rank)
		{
			if ( !this.drawingContext.GuidesMouse ||
				 !this.drawingContext.GuidesShow  )  return;

			bool changed = false;
			int total = this.document.Settings.GuidesCount;
			for ( int i=0 ; i<total ; i++ )
			{
				Settings.Guide guide = this.document.Settings.GuidesGet(i);

				if ( guide.Hilite != (i == rank) )
				{
					guide.Hilite = (i == rank);
					changed = true;
				}
			}

			if ( changed )
			{
				this.document.Notifier.NotifyArea(this);
			}
		}

		// Début de l'insertion interactive d'un guide (drag depuis une règle).
		public void GuideInteractiveStart(bool horizontal)
		{
			this.document.Modifier.OpletQueueBeginAction();
			this.drawingContext.GuidesShow = true;
			this.drawingContext.GuidesMouse = true;

			Settings.Guide guide = new Settings.Guide(this.document);
			guide.Type = horizontal ? Settings.GuideType.HorizontalBottom : Settings.GuideType.VerticalLeft;
			guide.Position = Settings.Guide.Undefined;
			this.guideInteractive = this.document.Settings.GuidesAdd(guide);
			this.document.Dialogs.SelectGuide(this.guideInteractive);
			this.guideCreate = true;
			this.mouseDown = true;
		}

		// Positionne un guide interactif.
		public void GuideInteractiveMove(Point pos)
		{
			if ( this.guideInteractive == -1 )  return;

			// Ne pas utiliser SnapGrid pour ignorer les repères !
			if ( this.drawingContext.GridActive )
			{
				this.drawingContext.SnapGridForce(ref pos);
			}

			Settings.Guide guide = this.document.Settings.GuidesGet(this.guideInteractive);
			if ( guide.IsHorizontal )
			{
				guide.AbsolutePosition = pos.Y;
			}
			else
			{
				guide.AbsolutePosition = pos.X;
			}

			this.document.Notifier.NotifyArea(this);
		}

		// Termine le déplacement d'un guide interactif.
		public void GuideInteractiveEnd()
		{
			if ( this.guideInteractive == -1 )  return;

			Size size = this.document.Size;
			Settings.Guide guide = this.document.Settings.GuidesGet(this.guideInteractive);
			if ( guide.Position == Settings.Guide.Undefined )
			{
				this.document.Settings.GuidesRemoveAt(this.guideInteractive);
			}
			else
			{
				if ( this.guideCreate )
				{
					if ( guide.IsHorizontal )
					{
						if ( guide.AbsolutePosition > size.Height/2 )
						{
							guide.Type = Settings.GuideType.HorizontalTop;
							guide.Position = size.Height-guide.Position;
						}
					}
					else
					{
						if ( guide.AbsolutePosition > size.Width/2 )
						{
							guide.Type = Settings.GuideType.VerticalRight;
							guide.Position = size.Width-guide.Position;
						}
					}
				}
			}

			this.document.Modifier.OpletQueueValidateAction();
			this.document.Notifier.NotifyGuidesChanged();
			this.guideInteractive = -1;
		}
		#endregion

		
		#region DialogError
		// Affiche le dialogue pour signaler une erreur.
		public void DialogError(string error)
		{
			if ( error == "" )  return;

			string title = "Crésus";
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";
			string message = error;

			Common.Dialogs.IDialog dialog = Common.Dialogs.Message.CreateOk(title, icon, message, "", this.CommandDispatcher);
			dialog.Owner = this.Window;
			dialog.OpenDialog();
		}
		#endregion


		#region Drawing
		// Dessine la grille magnétique dessous.
		protected void DrawGridBackground(Graphics graphics, Rectangle clipRect)
		{
			double initialWidth = graphics.LineWidth;
			graphics.LineWidth = 1.0/this.drawingContext.ScaleX;

			double ix = 0.5/this.drawingContext.ScaleX;
			double iy = 0.5/this.drawingContext.ScaleY;

			if ( this.IsActiveViewer )
			{
				if ( this.document.Type == DocumentType.Graphic )
				{
					// Dessine la "page".
					Rectangle rect = new Rectangle(0, 0, this.document.Size.Width, this.document.Size.Height);
					graphics.Align(ref rect);
					rect.Offset(ix, iy);

					if ( !this.BackColor.IsTransparent )
					{
						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.BackColor);
					}
					
					graphics.AddRectangle(rect);
					graphics.RenderSolid(Color.FromARGB(0.4, 0.5,0.5,0.5));
				}

				Rectangle area = this.document.Modifier.RectangleArea;
				graphics.Align(ref area);
				area.Offset(ix, iy);
				graphics.AddRectangle(area);
				graphics.RenderSolid(Color.FromARGB(0.4, 0.5,0.5,0.5));
			}

			graphics.LineWidth = initialWidth;
		}

		// Dessine la grille magnétique dessus.
		protected void DrawGridForeground(Graphics graphics, Rectangle clipRect)
		{
			if ( this.drawingContext.PreviewActive )  return;

			double initialWidth = graphics.LineWidth;
			graphics.LineWidth = 1.0/this.drawingContext.ScaleX;

			double ix = 0.5/this.drawingContext.ScaleX;
			double iy = 0.5/this.drawingContext.ScaleY;

			clipRect = ScreenToInternal(clipRect);
			clipRect = Rectangle.Intersection(clipRect, this.document.Modifier.RectangleArea);

			// Dessine la grille.
			if ( this.drawingContext.GridShow )
			{
				double s = System.Math.Min(this.drawingContext.GridStep.X*this.drawingContext.ScaleX,
										   this.drawingContext.GridStep.Y*this.drawingContext.ScaleY);
				int mul = (int) System.Math.Max(10.0/s, 1.0);

				Point origin = this.document.Modifier.OriginArea;
				origin = Point.GridAlign(origin, -this.drawingContext.GridOffset, this.drawingContext.GridStep);

				// Dessine les traits verticaux.
				double step = this.drawingContext.GridStep.X*mul;
				int subdiv = (int) this.drawingContext.GridSubdiv.X;
				int rank = subdiv-(int)(-this.document.Modifier.OriginArea.X/step);
				for ( double pos=origin.X ; pos<=this.document.Modifier.SizeArea.Width ; pos+=step )
				{
					if ( pos >= clipRect.Left && pos <= clipRect.Right )
					{
						double x = pos;
						double y = clipRect.Bottom;
						graphics.Align(ref x, ref y);
						x += ix;
						y += iy;
						graphics.AddLine(x, y, x, clipRect.Top);
						if ( rank%subdiv == 0 )
						{
							graphics.RenderSolid(Color.FromARGB(0.3, 0.6,0.6,0.6));  // gris
						}
						else
						{
							graphics.RenderSolid(Color.FromARGB(0.1, 0.6,0.6,0.6));  // gris
						}
					}
					rank ++;
				}

				// Dessine les traits horizontaux.
				step = this.drawingContext.GridStep.Y*mul;
				subdiv = (int) this.drawingContext.GridSubdiv.Y;
				rank = subdiv-(int)(-this.document.Modifier.OriginArea.Y/step);
				for ( double pos=origin.Y ; pos<=this.document.Modifier.SizeArea.Height ; pos+=step )
				{
					if ( pos >= clipRect.Bottom && pos <= clipRect.Top )
					{
						double x = clipRect.Left;
						double y = pos;
						graphics.Align(ref x, ref y);
						x += ix;
						y += iy;
						graphics.AddLine(x, y, clipRect.Right, y);
						if ( rank%subdiv == 0 )
						{
							graphics.RenderSolid(Color.FromARGB(0.3, 0.6,0.6,0.6));  // gris
						}
						else
						{
							graphics.RenderSolid(Color.FromARGB(0.1, 0.6,0.6,0.6));  // gris
						}
					}
					rank ++;
				}
			}

			// Dessine les repères.
			if ( this.drawingContext.GuidesShow )
			{
				int total = this.document.Settings.GuidesCount;
				for ( int i=0 ; i<total ; i++ )
				{
					Settings.Guide guide = this.document.Settings.GuidesGet(i);

					if ( guide.IsHorizontal )  // repère horizontal ?
					{
						double x = clipRect.Left;
						double y = guide.AbsolutePosition;
						graphics.Align(ref x, ref y);
						x += ix;
						y += iy;
						graphics.AddLine(x, y, clipRect.Right, y);
					}
					else	// repère vertical ?
					{
						double x = guide.AbsolutePosition;
						double y = clipRect.Bottom;
						graphics.Align(ref x, ref y);
						x += ix;
						y += iy;
						graphics.AddLine(x, y, x, clipRect.Top);
					}

					if ( guide.Hilite )
					{
						graphics.RenderSolid(Color.FromARGB(0.5, 0.8,0.0,0.0));  // rouge
					}
					else
					{
						graphics.RenderSolid(Color.FromARGB(0.5, 0.0,0.0,0.8));  // bleuté
					}
				}
			}

			// Dessine la cible.
			if ( this.IsActiveViewer )
			{
				if ( this.document.Type == DocumentType.Pictogram )
				{
					Rectangle rect = new Rectangle(0, 0, this.document.Size.Width, this.document.Size.Height);
					graphics.Align(ref rect);
					rect.Offset(ix, iy);
					graphics.AddRectangle(rect);

					rect.Offset(-ix, -iy);
					rect.Deflate(2);
					graphics.Align(ref rect);
					rect.Offset(ix, iy);
					graphics.AddRectangle(rect);

					double cx = this.document.Size.Width/2;
					double cy = this.document.Size.Height/2;
					graphics.Align(ref cx, ref cy);
					cx += ix;
					cy += iy;
					graphics.AddLine(cx, 0, cx, this.document.Size.Height);
					graphics.AddLine(0, cy, this.document.Size.Width, cy);
					graphics.RenderSolid(Color.FromARGB(0.4, 0.5,0.5,0.5));
				}
			}

			graphics.LineWidth = initialWidth;
		}

		// Dessine le hotspot.
		public void DrawHotSpot(Graphics graphics)
		{
			double initialWidth = graphics.LineWidth;
			graphics.LineWidth = 1.0/this.drawingContext.ScaleX;

			double ix = 0.5/this.drawingContext.ScaleX;
			double iy = 0.5/this.drawingContext.ScaleY;

			double x = this.document.HotSpot.X;
			double y = this.document.Modifier.OriginArea.Y;
			graphics.Align(ref x, ref y);
			x += ix;
			y += iy;
			graphics.AddLine(x, y, x, this.document.Modifier.SizeArea.Height);

			x = this.document.Modifier.OriginArea.X;
			y = this.document.HotSpot.Y;
			graphics.Align(ref x, ref y);
			x += ix;
			y += iy;
			graphics.AddLine(x, y, this.document.Modifier.SizeArea.Width, y);

			graphics.RenderSolid(Color.FromRGB(1.0, 0.0, 0.0));  // rouge

			Objects.Handle handle = new Objects.Handle(this.document);
			handle.Type = Objects.HandleType.Center;
			handle.Position = this.document.HotSpot;
			handle.IsVisible = true;
			handle.Draw(graphics, this.drawingContext);

			graphics.LineWidth = initialWidth;
		}

		// Dessine le document.
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Epsitec.Common.Widgets.Adorner.Factory.Active;

			if ( this.document.Type == DocumentType.Pictogram )
			{
				if ( !this.BackColor.IsTransparent && this.drawingContext.PreviewActive )
				{
					graphics.AddFilledRectangle(clipRect);
					graphics.RenderSolid(this.BackColor);
				}
				else
				{
					graphics.AddFilledRectangle(clipRect);
					graphics.RenderSolid(Color.FromBrightness(0.95));
				}
			}
			else
			{
				graphics.AddFilledRectangle(clipRect);
				graphics.RenderSolid(Color.FromBrightness(0.95));
			}

			double initialWidth = graphics.LineWidth;
			Transform save = graphics.Transform;
			Point scale = this.drawingContext.Scale;
			graphics.ScaleTransform(scale.X, scale.Y, 0, 0);
			graphics.TranslateTransform(this.drawingContext.OriginX, this.drawingContext.OriginY);

			// Dessine la grille magnétique dessous.
			this.DrawGridBackground(graphics, clipRect);

			// Dessine les géométries.
			this.document.Paint(graphics, this.drawingContext, clipRect);

			// Dessine la grille magnétique dessus.
			this.DrawGridForeground(graphics, clipRect);

			// Dessine les poignées.
			if ( this.IsActiveViewer )
			{
				this.DrawHandle(graphics);
			}

			// Dessine le hotspot.
			if ( this.IsActiveViewer && this.document.Modifier.Tool == "HotSpot" )
			{
				this.DrawHotSpot(graphics);
			}

			// Dessine le rectangle de modification.
			this.selector.Draw(graphics, this.drawingContext);
			this.zoomer.Draw(graphics, this.drawingContext);

			// Dessine les contraintes.
			this.drawingContext.DrawConstrain(graphics, this.document.Modifier.SizeArea);

			graphics.Transform = save;
			graphics.LineWidth = initialWidth;

			// Dessine le cadre.
			Rectangle rect = new Rectangle(0, 0, this.Client.Width, this.Client.Height);
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(adorner.ColorBorder);

			if ( this.debugDirty )
			{
				graphics.AddFilledRectangle(clipRect);
				graphics.RenderSolid(Color.FromARGB(0.5, 0.2, 0.2, 0.0));  // beurk !
				this.debugDirty = false;
			}
		}

		// Gère l'invalidation en fonction de la raison.
		protected override void Invalidate(InvalidateReason reason)
		{
			if ( reason != InvalidateReason.FocusedChanged )
			{
				base.Invalidate(reason);
			}
		}

		// Appelé lorsque la vue prend le focus.
		protected override void OnFocused()
		{
			Objects.Abstract edit = this.document.Modifier.RetEditObject();
			if ( edit != null )
			{
				this.document.Notifier.NotifyArea(this, edit.EditSelectBox);
			}
		}

		// Appelé lorsque la vue perd le focus.
		protected override void OnDefocused()
		{
			Objects.Abstract edit = this.document.Modifier.RetEditObject();
			if ( edit != null )
			{
				this.document.Notifier.NotifyArea(this, edit.EditSelectBox);
			}
		}

		// Salit tous les visualisateurs.
		public void DirtyAllViews()
		{
			foreach ( Viewer viewer in this.document.Modifier.AttachViewers )
			{
				viewer.debugDirty = true;
				this.document.Notifier.NotifyArea();
			}
		}
		#endregion


		#region Convert
		// Conversion d'un rectangle écran -> rectangle interne.
		public Rectangle ScreenToInternal(Rectangle rect)
		{
			if ( !rect.IsInfinite )
			{
				rect.BottomLeft = this.ScreenToInternal(rect.BottomLeft);
				rect.TopRight   = this.ScreenToInternal(rect.TopRight);
			}
			return rect;
		}

		// Conversion d'une coordonnée écran -> coordonnée interne.
		public Point ScreenToInternal(Point pos)
		{
			pos.X = pos.X/this.drawingContext.ScaleX - this.drawingContext.OriginX;
			pos.Y = pos.Y/this.drawingContext.ScaleY - this.drawingContext.OriginY;
			return pos;
		}

		// Conversion d'un rectangle interne -> rectangle écran.
		public Rectangle InternalToScreen(Rectangle rect)
		{
			if ( !rect.IsInfinite )
			{
				rect.BottomLeft = this.InternalToScreen(rect.BottomLeft);
				rect.TopRight   = this.InternalToScreen(rect.TopRight);
			}
			return rect;
		}

		// Conversion d'une coordonnée interne -> coordonnée écran.
		public Point InternalToScreen(Point pos)
		{
			pos.X = (pos.X+this.drawingContext.OriginX)*this.drawingContext.ScaleX;
			pos.Y = (pos.Y+this.drawingContext.OriginY)*this.drawingContext.ScaleY;
			return pos;
		}
		#endregion


		#region RedrawArea
		// Vide la zone de redessin.
		public void RedrawAreaFlush()
		{
			this.redrawArea = Rectangle.Empty;
		}

		// Agrandit la zone de redessin.
		public void RedrawAreaMerge(Rectangle rect)
		{
			if ( rect.IsEmpty )  return;
			this.redrawArea.MergeWith(rect);
		}

		// Retourne la zone de redessin.
		public Rectangle RedrawArea
		{
			get { return this.redrawArea; }
		}
		#endregion


		protected Document						document;
		protected DrawingContext				drawingContext;
		protected Selector						selector;
		protected Selector						zoomer;
		protected bool							partialSelect;
		protected Rectangle						redrawArea;
		protected Point							mousePosWidget;
		protected Point							mousePos;
		protected bool							mousePosValid = false;
		protected bool							mouseDown;
		protected Point							handMouseStart;
		protected Point							moveStart;
		protected Point							moveOffset;
		protected bool							moveAccept;
		protected int							moveHandle = -1;
		protected int							moveGlobal = -1;
		protected Objects.Abstract				moveObject;
		protected Objects.Abstract				cellObject;
		protected Objects.Abstract				hiliteHandleObject;
		protected int							hiliteHandleRank = -1;
		protected int							cellRank = -1;
		protected int							createRank = -1;
		protected bool							debugDirty;
		protected TextRuler						textRuler;
		protected Timer							autoScrollTimer;
		protected int							guideInteractive = -1;
		protected bool							guideCreate = false;
		protected bool							ctrlDuplicate = false;

		protected VMenu							contextMenu;
		protected Objects.Abstract				contextMenuObject;
		protected Point							contextMenuPos;
		protected int							contextMenuRank;

		protected MouseCursorType				lastMouseCursorType = MouseCursorType.Unknow;
		protected Image							mouseCursorArrow = null;
		protected Image							mouseCursorArrowPlus = null;
		protected Image							mouseCursorArrowDup = null;
		protected Image							mouseCursorFinger = null;
		protected Image							mouseCursorFingerDup = null;
		protected Image							mouseCursorPen = null;
		protected Image							mouseCursorZoom = null;
		protected Image							mouseCursorHand = null;
		protected Image							mouseCursorPicker = null;
		protected Image							mouseCursorPickerEmpty = null;
	}
}
