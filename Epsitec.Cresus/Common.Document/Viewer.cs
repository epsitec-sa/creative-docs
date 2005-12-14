using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// Summary description for Viewer.
	/// </summary>
	[SuppressBundleSupport]
	public class Viewer : Common.Widgets.Widget
	{
		protected enum MouseCursorType
		{
			Unknow,
			Arrow,
			ArrowPlus,
			ArrowDup,
			ArrowGlobal,
			ShaperNorm,
			ShaperPlus,
			ShaperMove,
			ShaperMulti,
			Hand,
			IBeam,
			HSplit,
			VSplit,
			Cross,
			Finger,
			FingerPlus,
			FingerDup,
			Pen,
			Zoom,
			ZoomMinus,
			ZoomShift,
			ZoomShiftCtrl,
			Picker,
			PickerEmpty,
			TextFlow,
			TextFlowCreate,
			TextFlowAdd,
			TextFlowRemove,
			Fine,
		}

		protected enum ZoomType
		{
			None,			// zoom quelconque
			Page,			// zoom sur toute la page
			PageWidth,		// zoom sur la largeur de la page
		}


		public Viewer(Document document)
		{
			this.AutoFocus       = true;
			this.AutoDoubleClick = true;
			
			this.InternalState |= InternalState.Focusable;
			
			this.BackColor = Color.FromBrightness(1);  // fond blanc

			this.document = document;
			this.drawingContext = new DrawingContext(this.document, this);
			this.selector = new Selector(this.document);
			this.zoomer = new Selector(this.document);
			this.mousePos = new Point(0,0);
			this.mousePosValid = false;
			this.mouseDragging = false;
			this.RedrawAreaFlush();

			this.textRuler = new TextRuler(this);
			this.textRuler.Visibility = false;
			if ( this.document.Type == DocumentType.Pictogram )
			{
				this.textRuler.AllFonts = false;
			}
			this.textRuler.Changed += new EventHandler(this.HandleRulerChanged);
			this.textRuler.ColorClicked += new EventHandler(this.HandleRulerColorClicked);
			this.textRuler.ColorNavigatorChanged += new EventHandler(this.HandleRulerColorNavigatorChanged);

			this.hotSpotHandle = new Objects.Handle(this.document);
			this.hotSpotHandle.Type = Objects.HandleType.Center;

			this.autoScrollTimer = new Timer();
			this.autoScrollTimer.AutoRepeat = 0.1;
			this.autoScrollTimer.TimeElapsed += new EventHandler(this.HandleAutoScrollTimeElapsed);

			this.miniBarTimer = new Timer();
			this.miniBarTimer.TimeElapsed += new Support.EventHandler (this.HandleMiniBarTimeElapsed);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.textRuler.Changed -= new EventHandler(this.HandleRulerChanged);
				this.textRuler.ColorClicked -= new EventHandler(this.HandleRulerColorClicked);
				this.textRuler.ColorNavigatorChanged -= new EventHandler(this.HandleRulerColorNavigatorChanged);

				this.autoScrollTimer.TimeElapsed -= new EventHandler(this.HandleAutoScrollTimeElapsed);
				this.autoScrollTimer.Dispose();
				this.autoScrollTimer = null;

				this.miniBarTimer.TimeElapsed -= new EventHandler(this.HandleMiniBarTimeElapsed);
				this.miniBarTimer.Dispose();
				this.miniBarTimer = null;
			}
			
			base.Dispose(disposing);
		}


		protected override void OnIsVisibleChanged(Types.PropertyChangedEventArgs e)
		{
			this.CloseMiniBar(false);  // ferme la mini-palette si le viewer devient invisible
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


		// Position horizontale du marqueur vertical.
		public double MarkerVertical
		{
			get
			{
				return this.markerVertical;
			}

			set
			{
				if ( this.markerVertical != value )
				{
					this.markerVertical = value;
					this.document.Notifier.NotifyArea();
				}
			}
		}

		// Position verticale du marqueur horizontal.
		public double MarkerHorizontal
		{
			get
			{
				return this.markerHorizontal;
			}

			set
			{
				if ( this.markerHorizontal != value )
				{
					this.markerHorizontal = value;
					this.document.Notifier.NotifyArea();
				}
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
			if ( this.document.Modifier.Tool == "ToolHand" )  return;
			this.autoScrollTimer.Start();
		}

		// Stoppe le timer pour l'auto-scroll.
		protected void AutoScrollTimerStop()
		{
			this.autoScrollTimer.Suspend();
		}

		// Appelé lorsque le timer arrive à échéance.
		// Effectue éventuellement un scroll si la souris est proche des bords.
		protected void HandleAutoScrollTimeElapsed(object sender)
		{
			if ( this.mouseDragging && this.zoomShift )  return;
			if ( this.mouseDragging && this.guideInteractive != -1 )  return;

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
			//?System.Diagnostics.Debug.WriteLine(string.Format("Message: {0}", message.Type));
			if ( !this.IsActiveViewer )  return;

			Modifier modifier = this.document.Modifier;
			if ( modifier == null )  return;

			// Après un MouseUp, on reçoit toujours un MouseMove inutile,
			// qui est filtré ici !!!
			if ( message.Type == MessageType.MouseMove &&
				 this.lastMessageType == MessageType.MouseUp &&
				 pos == this.mousePosWidget )
			{
				//?System.Diagnostics.Debug.WriteLine("ProcessMessage: MouseMove après MouseUp poubellisé !");
				return;
			}
			this.lastMessageType = message.Type;

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
					this.mouseDragging = true;
					break;
				
				case MessageType.MouseMove:
					this.ProcessMouseMove(message, pos);
					break;

				case MessageType.MouseUp:
					if ( this.mouseDragging )
					{
						this.AutoScrollTimerStop();
						this.ProcessMouseUp(message, pos);
						this.mouseDragging = false;
						this.ProcessMouseMove(message, pos);
					}
					break;

				case MessageType.MouseLeave:
					this.mousePosValid = false;
					if ( !this.IsCreating )
					{
						this.ClearHilite();
					}
					break;

				case MessageType.MouseWheel:
					this.ProcessMouseWheel(message.Wheel, pos);
					break;

				case MessageType.KeyDown:
					if ( message.KeyCode == KeyCode.ShiftKey   ||
						 message.KeyCode == KeyCode.ControlKey )
					{
						this.UpdateMouseCursor(message);
						this.UpdateHotSpot();
						this.DispatchDummyMouseMoveEvent();
					}

					if ( this.EditProcessMessage(message, pos) )
					{
						break;
					}

					if ( this.createRank != -1 )
					{
						if ( message.KeyCode == KeyCode.Escape )
						{
							this.CreateEnding(false, false);
							break;
						}
					}

					if ( modifier.RetEditObject() == null )
					{
						if ( message.KeyCode == KeyCode.Back   ||
							 message.KeyCode == KeyCode.Delete )
						{
							modifier.DeleteSelection();
							break;
						}
					}

					if ( message.KeyCode == KeyCode.Space )
					{
						if ( !this.NextHotSpot() )
						{
							if ( this.miniBar == null )
							{
								this.OpenMiniBar(pos, false, true);
							}
							else
							{
								this.CloseMiniBar(false);
							}
						}
						break;
					}

					this.UseMouseCursor();
					return;

				case MessageType.KeyUp:
					if ( message.KeyCode == KeyCode.ShiftKey   ||
						 message.KeyCode == KeyCode.ControlKey )
					{
						this.UpdateMouseCursor(message);
						this.UpdateHotSpot();
						this.DispatchDummyMouseMoveEvent();
					}

					if ( this.EditProcessMessage(message, pos) )
					{
						break;
					}

					this.UseMouseCursor();
					return;

				default:
					if ( this.EditProcessMessage(message, pos) )
					{
						break;
					}

					this.UseMouseCursor();
					return;
			}
			
			this.document.Notifier.GenerateEvents();
			message.Consumer = this;
			this.UseMouseCursor();
		}

		// Gestion d'un bouton de la souris pressé.
		protected void ProcessMouseDown(Message message, Point pos)
		{
			Modifier modifier = this.document.Modifier;

			if ( modifier.Tool == "ToolSelect" )
			{
				this.SelectMouseDown(pos, message.ButtonDownCount, message.IsRightButton, false);
			}
			else if ( modifier.Tool == "ToolGlobal" )
			{
				this.SelectMouseDown(pos, message.ButtonDownCount, message.IsRightButton, true);
			}
			else if ( modifier.Tool == "ToolShaper" )
			{
				this.ShaperMouseDown(pos, message.ButtonDownCount, message.IsRightButton);
			}
			else if ( modifier.Tool == "ToolEdit" )
			{
				this.EditMouseDown(message, pos, message.ButtonDownCount);
			}
			else if ( modifier.Tool == "ToolZoom" )
			{
				this.ZoomMouseDown(pos, message.IsRightButton);
			}
			else if ( modifier.Tool == "ToolHand" )
			{
				this.HandMouseDown(pos);
			}
			else if ( modifier.Tool == "ToolPicker" )
			{
				this.PickerMouseDown(pos);
			}
			else if ( modifier.Tool == "ToolHotSpot" )
			{
				this.HotSpotMouseDown(pos);
			}
			else
			{
				this.CreateMouseDown(pos);
			}
		}

		// Gestion d'un déplacement de la souris pressé.
		protected void ProcessMouseMove(Message message, Point pos)
		{
			Modifier modifier = this.document.Modifier;

			if ( this.guideInteractive != -1 )
			{
				this.GuideInteractiveMove(pos, message.IsAltPressed);
				return;
			}

			if ( modifier.Tool == "ToolSelect" )
			{
				this.SelectMouseMove(pos, message.IsRightButton, false);
			}
			else if ( modifier.Tool == "ToolGlobal" )
			{
				this.SelectMouseMove(pos, message.IsRightButton, true);
			}
			else if ( modifier.Tool == "ToolShaper" )
			{
				this.ShaperMouseMove(pos, message.IsRightButton);
			}
			else if ( modifier.Tool == "ToolEdit" )
			{
				this.EditMouseMove(message, pos);
			}
			else if ( modifier.Tool == "ToolZoom" )
			{
				this.ZoomMouseMove(pos, message.IsRightButton);
			}
			else if ( modifier.Tool == "ToolHand" )
			{
				this.HandMouseMove(pos);
			}
			else if ( modifier.Tool == "ToolPicker" )
			{
				this.PickerMouseMove(pos);
			}
			else if ( modifier.Tool == "ToolHotSpot" )
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

			if ( modifier.Tool == "ToolSelect" )
			{
				this.SelectMouseUp(pos, message.IsRightButton, false);
			}
			else if ( modifier.Tool == "ToolGlobal" )
			{
				this.SelectMouseUp(pos, message.IsRightButton, true);
			}
			else if ( modifier.Tool == "ToolShaper" )
			{
				this.ShaperMouseUp(pos, message.IsRightButton);
			}
			else if ( modifier.Tool == "ToolEdit" )
			{
				this.EditMouseUp(message, pos, message.IsRightButton);
			}
			else if ( modifier.Tool == "ToolZoom" )
			{
				this.ZoomMouseUp(pos, message.IsRightButton);
			}
			else if ( modifier.Tool == "ToolHand" )
			{
				this.HandMouseUp(pos);
			}
			else if ( modifier.Tool == "ToolPicker" )
			{
				this.PickerMouseUp(pos);
			}
			else if ( modifier.Tool == "ToolHotSpot" )
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


		#region CreateMouse
		protected void CreateMouseDown(Point mouse)
		{
			this.drawingContext.MagnetClearStarting();
			this.drawingContext.MagnetSnapPos(ref mouse);
			this.drawingContext.SnapGrid(ref mouse);

			if ( this.createRank == -1 )
			{
				if ( this.document.Modifier.TotalSelected > 0 )
				{
					this.document.Modifier.DeselectAll();
				}

				string name = string.Format(Res.Strings.Action.ObjectCreate, this.document.Modifier.ToolName(this.document.Modifier.Tool));
				this.document.Modifier.OpletQueueBeginAction(name, "Create");
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
			this.drawingContext.MagnetSnapPos(ref mouse);
			//this.drawingContext.SnapGrid(ref mouse);

			if ( this.createRank == -1 )  return;

			Objects.Abstract layer = this.drawingContext.RootObject();
			Objects.Abstract obj = layer.Objects[this.createRank] as Objects.Abstract;

			obj.CreateMouseMove(mouse, this.drawingContext);
		}

		protected void CreateMouseUp(Point mouse)
		{
			this.drawingContext.MagnetSnapPos(ref mouse);
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

					if ( this.editFlowAfterCreate != null && obj is Objects.TextBox2 )
					{
						this.document.Modifier.TextFlowChange(obj as Objects.TextBox2, this.editFlowAfterCreate, true);
						this.editFlowAfterCreate = null;
					}
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

			if ( selectAfterCreation )
			{
				this.document.Modifier.Tool = "ToolSelect";
			}
			if ( editAfterCreation )
			{
				this.document.Modifier.Tool = "ToolEdit";
			}

			this.document.Notifier.NotifyCreateChanged();
			this.document.Notifier.NotifySelectionChanged();
		}

		// Si nécessaire, termine la création en cours.
		public void CreateEnding(bool delete, bool close)
		{
			if ( this.createRank == -1 )  return;

			Objects.Abstract layer = this.drawingContext.RootObject();
			Objects.Abstract obj = layer.Objects[this.createRank] as Objects.Abstract;

			if ( close )
			{
				obj.ChangePropertyPolyClose(close);
			}

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
		protected void SelectMouseDown(Point mouse, int downCount, bool isRight, bool global)
		{
			this.CloseMiniBar(false);
			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.Select);
			this.moveStart = mouse;
			this.moveAccept = false;
			this.moveInitialSel = false;
			this.drawingContext.ConstrainFlush();
			this.drawingContext.ConstrainAddHV(mouse);
			this.ClearHilite();
			this.selector.HiliteHandle(-1);
			this.HiliteHandle(null, -1);
			this.moveGlobal = -1;
			this.moveObject = null;
			this.guideInteractive = -1;
			this.guideCreate = false;
			this.ctrlDown = this.drawingContext.IsCtrl;
			this.ctrlDuplicate = false;
			this.moveReclick = false;

			Objects.Abstract obj;
			int rank;
			if ( this.selector.Detect(mouse, !this.drawingContext.IsShift, out rank) )
			{
				this.moveGlobal = rank;
				this.selector.MoveNameAction(rank);
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
					this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.HandleMove);
					this.moveObject = obj;
					this.moveHandle = rank;
					this.moveOffset = mouse-obj.GetHandlePosition(rank);
					this.moveObject.MoveHandleStarting(this.moveHandle, mouse, this.drawingContext);
					this.HiliteHandle(this.moveObject, this.moveHandle);
					this.document.Modifier.FlushMoveAfterDuplicate();
				}
				else
				{
					obj = this.Detect(mouse, !this.drawingContext.IsShift);
					if ( obj == null )
					{
						if ( this.GuideDetect(mouse, out rank) )
						{
							this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.GuideMove);
							this.guideInteractive = rank;
							this.document.Dialogs.SelectGuide(this.guideInteractive);
						}
						else
						{
							this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.SelectGlobal);
							this.selector.FixStarting(mouse);
							this.document.Modifier.FlushMoveAfterDuplicate();
						}
					}
					else
					{
						if ( !obj.IsSelected )
						{
							this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.SelectObject);
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
									this.SelectorInitialize(this.document.Modifier.SelectedBbox, 0.0);
								}
							}
							this.document.Modifier.FlushMoveAfterDuplicate();
						}
						else
						{
							this.moveInitialSel = true;

							if ( this.drawingContext.IsShift )
							{
								this.document.Modifier.UpdateCounters();
								obj.Deselect();
								this.document.Modifier.TotalSelected --;
								this.document.Modifier.FlushMoveAfterDuplicate();
							}
							else
							{
								this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.MoveObject);
								this.moveReclick = true;
							}
						}

						this.moveObject = obj;
						this.moveHandle = -1;  // déplace tout l'objet
						this.moveCenter = this.moveObject.HotSpotPosition;
						this.moveLast = this.moveCenter;
						this.moveOffset = mouse-this.moveLast;
						this.drawingContext.ConstrainFlush();
						this.drawingContext.ConstrainAddHV(this.moveLast);
						this.MoveAllStarting();
						this.hotSpotHandle.Position = this.moveCenter;
						this.hotSpotHandle.IsVisible = this.drawingContext.MagnetActiveAndExist;
						this.hotSpotHandle.IsHilited = true;
					}
				}
			}
		}

		protected void SelectMouseMove(Point mouse, bool isRight, bool global)
		{
			this.HiliteHandle(null, -1);
			this.selector.HiliteHandle(-1);

			if ( this.mouseDragging )  // bouton souris pressé ?
			{
				// Duplique le ou les objets sélectionnés ?
				if ( this.ctrlDown && !this.ctrlDuplicate &&
					 (this.moveGlobal != -1 || (this.moveObject != null && this.moveHandle == -1)) )
				{
					double len = Point.Distance(mouse, this.moveStart);
					if ( len > this.drawingContext.MinimalSize )
					{
						this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.DuplicateAndMove);

						// Remet la sélection à la position de départ:
						if ( this.moveGlobal != -1 )  // déplace le modificateur global ?
						{
							this.selector.MoveProcess(this.moveGlobal, this.moveStart, this.drawingContext);
							this.MoveGlobalProcess(this.selector);
							this.document.Modifier.GroupUpdateChildrens();
							this.document.Modifier.GroupUpdateParents();
						}

						this.document.Modifier.DuplicateSelection(new Point(0,0));
						this.document.Modifier.ActiveViewer.UpdateSelector();
						this.ctrlDuplicate = true;

						if ( this.moveGlobal != -1 )  // déplace le modificateur global ?
						{
							this.selector.MoveStarting(this.moveGlobal, this.moveStart, this.drawingContext);
							this.MoveGlobalStarting();
						}

						if ( this.moveObject != null )
						{
							this.moveObject = this.document.Modifier.RetOnlySelectedObject();
						}
					}
				}

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
				else if ( this.moveObject != null && !this.drawingContext.IsShift )
				{
					if ( this.moveHandle != -1 )  // déplace une poignée ?
					{
						mouse -= this.moveOffset;
						this.moveObject.MoveHandleProcess(this.moveHandle, mouse, this.drawingContext);
						this.HiliteHandle(this.moveObject, this.moveHandle);
						this.document.Modifier.FlushMoveAfterDuplicate();
					}
					else	// déplace tout l'objet ?
					{
						if ( !this.moveInitialSel && this.moveAccept )
						{
							this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.SelectAndMove);
						}

						if ( !this.moveAccept )
						{
							double len = Point.Distance(mouse, this.moveStart);
							if ( len > this.drawingContext.MinimalSize )
							{
								this.moveAccept = true;
							}
						}

						if ( this.moveAccept )
						{
							mouse -= this.moveOffset;
							if ( !this.drawingContext.ConstrainSnapPos(ref mouse) )
							{
								Rectangle box = this.moveObject.BoundingBoxThin;
								box.Offset(mouse-this.moveLast);
								this.drawingContext.SnapGrid(ref mouse, -this.moveCenter, box);
							}
							this.MoveAllProcess(mouse-this.moveLast);
							this.moveLast = mouse;

							this.hotSpotHandle.IsVisible = (this.drawingContext.IsCtrl || this.drawingContext.MagnetActiveAndExist);
							this.hotSpotHandle.Position = this.moveObject.HotSpotPosition;
						}
					}
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
				else if ( !global && hiliteObj == null && this.GuideDetect(mouse, out guideRank) )
				{
					this.ChangeMouseCursor(this.GuideIsHorizontal(guideRank) ? MouseCursorType.HSplit : MouseCursorType.VSplit);
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
						this.ChangeMouseCursor(global ? MouseCursorType.ArrowGlobal : MouseCursorType.Arrow);
					}
					else
					{
						if ( obj.IsSelected )
						{
							this.ChangeMouseCursor(MouseCursorType.Finger);
						}
						else
						{
							this.ChangeMouseCursor(global ? MouseCursorType.ArrowGlobal : MouseCursorType.Arrow);
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

			bool mb = true;
			if ( this.moveAccept )  mb = false;
			if ( this.moveHandle != -1 && !Point.Equals(mouse, this.moveStart) )  mb = false;
			if ( this.moveGlobal != -1 )  mb = false;

			if ( this.selector.Visible && !this.selector.Handles )
			{
				double len = Point.Distance(mouse, this.moveStart);
				if ( isRight && len <= this.drawingContext.MinimalSize )
				{
					globalMenu = true;
				}
				else
				{
					Rectangle rSelect = this.selector.Rectangle;
					this.Select(rSelect, this.drawingContext.IsShift, this.partialSelect);
					this.UpdateSelector();
				}
				if ( this.document.Modifier.TotalSelected == 0 )
				{
					this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.DeselectAll);
				}
				else if ( this.drawingContext.IsShift )
				{
					this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.SelectGlobalAdd);
				}
			}
			else if ( this.moveGlobal != -1 )  // déplace le modificateur global ?
			{
				this.selector.MoveEnding(this.moveGlobal, mouse, this.drawingContext);
				this.document.Modifier.GroupUpdateChildrens();
				this.document.Modifier.GroupUpdateParents();
			}
			else if ( this.moveObject != null )
			{
				if ( this.moveHandle != -1 )  // déplace une poignée ?
				{
					this.moveObject.MoveHandleEnding(this.moveHandle, mouse, this.drawingContext);
				}

				if ( this.drawingContext.IsShift )
				{
					if ( this.moveObject.IsSelected )
					{
						this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.SelectObjectAdd);
					}
					else
					{
						this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.SelectObjectSub);
					}
				}

				if ( this.moveReclick && !this.moveAccept && !isRight && !this.drawingContext.IsShift )
				{
					this.SelectOther(mouse, this.moveObject);
				}
				else
				{
					this.document.Modifier.GroupUpdateChildrens();
					this.document.Modifier.GroupUpdateParents();

					this.moveHandle = -1;
					this.UpdateSelector();
				}

				this.hotSpotHandle.IsVisible = false;
			}

			this.moveObject = null;
			this.drawingContext.ConstrainDelStarting();
			this.drawingContext.MagnetDelStarting();
			this.document.Modifier.OpletQueueValidateAction();

			if ( isRight )  // avec le bouton de droite de la souris ?
			{
				this.document.Notifier.GenerateEvents();
				this.ContextMenu(mouse, globalMenu);
			}
			else
			{
				if ( mb )
				{
					this.OpenMiniBar(mouse, true, false);
				}
			}
		}

		// Change le point chaud.
		protected bool NextHotSpot()
		{
			if ( this.moveObject == null )  return false;

			if ( this.drawingContext.IsCtrl )
			{
				this.drawingContext.ConstrainSpacePressed();
			}
			else if ( !this.document.Modifier.IsToolShaper )
			{
				Point move = this.moveObject.HotSpotPosition;
				this.moveObject.ChangeHotSpot(1);
				move -= this.moveObject.HotSpotPosition;
				this.moveOffset += move;
				this.moveCenter += move;
				this.moveLast   -= move;

				this.hotSpotHandle.Position = this.moveObject.HotSpotPosition;

				this.drawingContext.MagnetDelStarting();
				this.drawingContext.ConstrainDelStarting();
				this.drawingContext.ConstrainFlush();
				this.drawingContext.ConstrainAddHV(this.moveLast);
			}
			return true;
		}

		// Met à jour la poignée spéciale "hot spot".
		protected void UpdateHotSpot()
		{
			if ( this.moveObject == null || this.moveHandle != -1 )  return;
			if ( this.document.Modifier.IsToolShaper )  return;
			this.hotSpotHandle.IsVisible = (this.drawingContext.IsCtrl || this.drawingContext.MagnetActiveAndExist);
		}
		#endregion


		#region ShaperMouse
		protected void ShaperMouseDown(Point mouse, int downCount, bool isRight)
		{
			this.CloseMiniBar(false);
			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.Shaper);
			this.moveStart = mouse;
			this.moveAccept = false;
			this.moveInitialSel = false;
			this.drawingContext.ConstrainFlush();
			this.drawingContext.ConstrainAddHV(mouse);
			this.ShaperHilite(null, Point.Empty);
			this.selector.HiliteHandle(-1);
			this.HiliteHandle(null, -1);
			this.moveGlobal = -1;
			this.moveObject = null;
			this.moveHandle = -1;
			this.moveSelectedSegment = -1;
			this.guideInteractive = -1;
			this.guideCreate = false;
			this.ctrlDown = this.drawingContext.IsCtrl;
			this.ctrlDuplicate = false;
			this.moveReclick = false;
			this.moveSelectedHandle = false;
			this.hotSpotHandle.IsVisible = false;

			Objects.Abstract obj;
			int rank;
			if ( this.DetectHandle(mouse, out obj, out rank) )
			{
				if ( this.drawingContext.IsShift )
				{
					if ( obj.IsShaperHandleSelected(rank) )
					{
						this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.SelectHandleSub);
					}
					else
					{
						this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.SelectHandleAdd);
					}
				}
				else
				{
					this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.SelectHandle);
				}
				obj.SelectHandle(rank, this.drawingContext.IsShift);
				this.moveObject = obj;
				this.moveHandle = rank;
				this.moveOffset = mouse-obj.GetHandlePosition(rank);
				this.moveObject.MoveHandleStarting(this.moveHandle, mouse, this.drawingContext);
				this.HiliteHandle(this.moveObject, this.moveHandle);
			}
			else if ( this.DetectSelectedSegmentHandle(mouse, out obj, out rank) )
			{
				this.moveObject = obj;
				this.moveSelectedSegment = rank;
				this.moveOffset = mouse-this.moveObject.GetSelectedSegmentPosition(rank);
				mouse -= this.moveOffset;
				this.moveObject.MoveSelectedSegmentStarting(this.moveSelectedSegment, mouse, this.drawingContext);
			}
			else
			{
				obj = this.Detect(mouse, !this.drawingContext.IsShift);
				if ( obj == null )
				{
					this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.SelectGlobal);
					this.selector.FixStarting(mouse);
				}
				else
				{
					if ( !obj.IsSelected )
					{
						this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.SelectObject);
						this.Select(obj, false, this.drawingContext.IsShift);
					}
					this.moveObject = obj;
					this.moveHandle = -1;
					this.moveSelectedSegment = -1;

					int rankSegment = this.moveObject.ShaperDetectSegment(mouse);

					if ( this.DetectHandle(mouse, out obj, out rank) )
					{
						if ( this.drawingContext.IsShift )
						{
							if ( obj.IsShaperHandleSelected(rank) )
							{
								this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.SelectHandleSub);
							}
							else
							{
								this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.SelectHandleAdd);
							}
						}
						else
						{
							this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.SelectHandle);
						}
						obj.SelectHandle(rank, this.drawingContext.IsShift);
						this.moveHandle = rank;
						this.moveOffset = mouse-obj.GetHandlePosition(rank);
						this.moveObject.MoveHandleStarting(this.moveHandle, mouse, this.drawingContext);
						this.HiliteHandle(this.moveObject, this.moveHandle);
					}
					else if ( rankSegment != -1 )
					{
						Point pos = mouse;
						rank = this.moveObject.SelectedSegmentAdd(rankSegment, ref pos, this.drawingContext.IsShift);

						this.moveSelectedSegment = rank;
						this.moveOffset = mouse-this.moveObject.GetSelectedSegmentPosition(rank);
						mouse -= this.moveOffset;
						this.moveObject.MoveSelectedSegmentStarting(this.moveSelectedSegment, mouse, this.drawingContext);
					}
					else
					{
						this.moveObject.MoveSelectedHandlesStarting(mouse, this.drawingContext);
						this.moveSelectedHandle = true;
					}
				}
			}
		}

		protected void ShaperMouseMove(Point mouse, bool isRight)
		{
			this.HiliteHandle(null, -1);
			this.selector.HiliteHandle(-1);

			if ( this.mouseDragging )  // bouton souris pressé ?
			{
				if ( this.selector.Visible && !this.selector.Handles )
				{
					this.selector.FixEnding(mouse);
				}
				else if ( this.moveObject != null && !this.drawingContext.IsShift )
				{
					if ( this.moveHandle != -1 )  // déplace une poignée ?
					{
						mouse -= this.moveOffset;
						this.moveObject.MoveHandleProcess(this.moveHandle, mouse, this.drawingContext);
						this.HiliteHandle(this.moveObject, this.moveHandle);
						this.document.Modifier.FlushMoveAfterDuplicate();
					}

					if ( this.moveSelectedSegment != -1 )  // déplace un segment sélectionné ?
					{
						mouse -= this.moveOffset;
						this.moveObject.MoveSelectedSegmentProcess(this.moveSelectedSegment, mouse, this.drawingContext);
					}

					if ( this.moveSelectedHandle )  // déplace plusieurs poignées ?
					{
						this.moveObject.MoveSelectedHandlesProcess(mouse, this.drawingContext);
					}
				}
			}
			else	// bouton souris relâché ?
			{
				this.ChangeMouseCursor(MouseCursorType.ShaperNorm);

				Objects.Abstract hiliteObj = this.Detect(mouse, true);
				this.document.Modifier.ContainerHilite(hiliteObj);

				Objects.Abstract obj;
				int rank;

				if ( this.DetectHandle(mouse, out obj, out rank) )
				{
					this.ShaperHilite(null, Point.Empty);
					this.HiliteHandle(obj, rank);
					this.ChangeMouseCursor(MouseCursorType.ShaperMove);
				}
				else
				{
					obj = hiliteObj;
					if ( obj == null )
					{
						this.ShaperHilite(null, Point.Empty);
					}
					else
					{
						this.ShaperHilite(obj, mouse);

						rank = obj.ShaperDetectSegment(mouse);
						if ( rank == -1 )
						{
							if ( obj.IsSelected && (obj.IsShaperHandleSelected() || obj.IsSelectedSegments()) )
							{
								this.ChangeMouseCursor(MouseCursorType.ShaperMulti);
							}
						}
						else
						{
							this.ChangeMouseCursor(MouseCursorType.ShaperMove);
						}
					}
				}
			}
		}

		protected void ShaperMouseUp(Point mouse, bool isRight)
		{
			bool globalMenu = false;
			this.HiliteHandle(null, -1);

			if ( this.moveSelectedHandle )
			{
				this.moveObject.MoveSelectedHandlesEnding(mouse, this.drawingContext);
				this.moveSelectedHandle = false;
			}

			bool mb = true;
			if ( !Point.Equals(mouse, this.moveStart) )  mb = false;

			if ( this.selector.Visible && !this.selector.Handles )
			{
				double len = Point.Distance(mouse, this.moveStart);
				if ( isRight && len <= this.drawingContext.MinimalSize )
				{
					globalMenu = true;
				}
				else
				{
					Rectangle rSelect = this.selector.Rectangle;
					this.Select(rSelect, this.drawingContext.IsShift, true);
				}
				if ( this.document.Modifier.TotalSelected == 0 )
				{
					this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.DeselectAll);
				}
			}
			else if ( this.moveObject != null )
			{
				if ( this.moveHandle != -1 )  // déplace une poignée ?
				{
					if ( mouse.X != this.moveStart.X ||
						 mouse.Y != this.moveStart.Y )
					{
						this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.HandleMove);
					}

					mouse -= this.moveOffset;
					this.moveObject.MoveHandleEnding(this.moveHandle, mouse, this.drawingContext);
				}
				else if ( this.moveSelectedSegment != -1 )  // déplace un segment sélectionné ?
				{
					mouse -= this.moveOffset;
					this.moveObject.MoveSelectedSegmentEnding(this.moveSelectedSegment, mouse, this.drawingContext);
				}

				this.document.Modifier.GroupUpdateChildrens();
				this.document.Modifier.GroupUpdateParents();

				this.moveHandle = -1;
				this.moveSelectedSegment = -1;
				this.UpdateSelector();
			}

			this.moveObject = null;
			this.drawingContext.ConstrainDelStarting();
			this.drawingContext.MagnetDelStarting();
			this.document.Modifier.OpletQueueValidateAction();

			if ( isRight )  // avec le bouton de droite de la souris ?
			{
				this.document.Notifier.GenerateEvents();
				this.ContextMenu(mouse, globalMenu);
			}
			else
			{
				if ( mb )
				{
					this.document.Notifier.NotifyShaperChanged();
					this.OpenMiniBar(mouse, true, false);
				}
			}
		}
		#endregion

		
		#region EditMouse
		public bool EditFlowTerminate()
		{
			if ( this.editFlowPress  == Objects.DetectEditType.Out &&
				 this.editFlowSelect == Objects.DetectEditType.Out )
			{
				return false;
			}

			this.EditFlowReset();
			this.ChangeMouseCursor(MouseCursorType.IBeam);
			this.UseMouseCursor();
			return true;
		}

		public void EditFlowReset()
		{
			this.editFlowPress  = Objects.DetectEditType.Out;
			this.editFlowSelect = Objects.DetectEditType.Out;
			this.editFlowAfterCreate = null;
		}

		protected void EditMouseDown(Message message, Point mouse, int downCount)
		{
			Objects.DetectEditType handle;

			if ( this.editFlowSelect != Objects.DetectEditType.Out )
			{
				Objects.TextBox2 edit = this.DetectEdit(mouse, true, out handle) as Objects.TextBox2;
				if ( edit == null )
				{
					edit = this.editFlowSrc as Objects.TextBox2;
					this.document.Modifier.Tool = "ObjectTextBox2";
					this.editFlowAfterCreate = edit;
					this.CreateMouseDown(mouse);
				}
				return;
			}

			this.EditFlowReset();
			Objects.Abstract obj = this.DetectEdit(mouse, false, out handle);

			if ( obj != this.document.Modifier.RetEditObject() )
			{
				this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.Edit);
				this.Select(obj, true, false);
				this.document.Modifier.OpletQueueValidateAction();
			}
			else
			{
				if ( handle == Objects.DetectEditType.HandleFlowPrev ||
					 handle == Objects.DetectEditType.HandleFlowNext )
				{
					this.editFlowPress = handle;
					this.editFlowSrc = obj;
					return;
				}
			}

			this.EditProcessMessage(message, mouse);
		}

		protected void EditMouseMove(Message message, Point mouse)
		{
			this.ChangeMouseCursor(MouseCursorType.IBeam);

			if ( this.editFlowPress != Objects.DetectEditType.Out )
			{
				this.ChangeMouseCursor(MouseCursorType.Arrow);
				return;
			}

			if ( this.editFlowSelect != Objects.DetectEditType.Out )
			{
				Objects.DetectEditType handle;
				Objects.Abstract obj = this.DetectEdit(mouse, true, out handle);
				this.Hilite(obj);

				if ( obj == null )
				{
					this.ChangeMouseCursor(MouseCursorType.TextFlowCreate);
				}
				else
				{
					Objects.Abstract edit = this.document.Modifier.RetEditObject();
					if ( obj == edit )
					{
						this.ChangeMouseCursor(MouseCursorType.TextFlowRemove);
					}
					else
					{
						this.ChangeMouseCursor(MouseCursorType.TextFlowAdd);
					}
				}
				return;
			}

			if ( this.mouseDragging )  // bouton souris pressé ?
			{
				this.EditProcessMessage(message, mouse);
			}
			else	// bouton souris relâché ?
			{
				Objects.DetectEditType handle;
				Objects.Abstract obj = this.DetectEdit(mouse, false, out handle);
				this.Hilite(obj);

				if ( handle == Objects.DetectEditType.HandleFlowPrev ||
					 handle == Objects.DetectEditType.HandleFlowNext )
				{
					this.ChangeMouseCursor(MouseCursorType.TextFlow);
				}
				else
				{
					this.ChangeMouseCursor(MouseCursorType.IBeam);
				}
			}
		}

		protected void EditMouseUp(Message message, Point mouse, bool isRight)
		{
			if ( this.editFlowPress != Objects.DetectEditType.Out )
			{
				this.editFlowSelect = this.editFlowPress;
				this.editFlowPress = Objects.DetectEditType.Out;
				return;
			}

			if ( this.editFlowSelect != Objects.DetectEditType.Out )
			{
				this.Hilite(null);

				Objects.DetectEditType handle;
				Objects.TextBox2 obj = this.DetectEdit(mouse, true, out handle) as Objects.TextBox2;
				if ( obj != null )
				{
					bool after = (this.editFlowSelect == Objects.DetectEditType.HandleFlowNext);
					Objects.TextBox2 edit = this.editFlowSrc as Objects.TextBox2;
					System.Diagnostics.Debug.Assert(edit != null);

					if ( obj == edit )
					{
						this.document.Modifier.TextFlowChange(obj, null, after);
					}
					else
					{
						this.document.Modifier.TextFlowChange(obj, edit, after);
					}
				}
				this.editFlowSelect = Objects.DetectEditType.Out;
				return;
			}

			this.EditProcessMessage(message, mouse);
		}

		protected bool EditProcessMessage(Message message, Point pos)
		{
			Objects.Abstract editObject = this.document.Modifier.RetEditObject();
			if ( editObject == null )  return false;

#if false
			Rectangle ibbox = editObject.BoundingBox;
			if ( editObject.EditProcessMessage(message, pos) )
			{
				this.document.Notifier.NotifyArea(ibbox);
				this.document.Notifier.NotifyArea(editObject.BoundingBox);
				return true;
			}
			return false;
#else
			return editObject.EditProcessMessage(message, pos);
#endif
		}
		#endregion


		#region ZoomMouse
		protected void ZoomMouseDown(Point mouse, bool isRight)
		{
			this.document.Modifier.OpletQueueEnable = false;
			this.moveStart = mouse;

			if ( this.drawingContext.IsShift )
			{
				this.zoomShift = true;
				this.zoomCtrl = this.drawingContext.IsCtrl;
				this.zoomOrigin = this.mousePosWidget;
				this.zoomStart = this.drawingContext.Zoom;
				this.zoomOffset = mouse-this.RectangleDisplayed.Center;
				this.document.Modifier.ZoomMemorize();
			}
			else
			{
				this.zoomShift = false;
				this.zoomer.FixStarting(mouse);
			}
		}
		
		protected void ZoomMouseMove(Point mouse, bool isRight)
		{
			this.ChangeMouseCursor(MouseCursorType.Zoom);

			if ( this.mouseDragging )
			{
				if ( this.zoomShift )
				{
					double dist = this.mousePosWidget.Y-this.zoomOrigin.Y;
					double zoom = this.zoomStart;
					if ( dist > 0.0 )
					{
						zoom *= 1.0+dist/50.0;
					}
					if ( dist < 0.0 )
					{
						zoom /= 1.0-dist/50.0;
					}
					zoom = System.Math.Max(zoom, this.document.Modifier.ZoomMin);
					zoom = System.Math.Min(zoom, this.document.Modifier.ZoomMax);

					if ( this.zoomCtrl )
					{
						this.drawingContext.ZoomAndCenter(zoom, this.moveStart);
					}
					else
					{
						this.drawingContext.ZoomAndCenter(zoom, this.moveStart-this.zoomOffset);
					}
				}
				else
				{
					this.zoomer.FixEnding(mouse);
				}
			}
		}
		
		protected void ZoomMouseUp(Point mouse, bool isRight)
		{
			this.zoomer.Visible = false;
			Rectangle rect = this.zoomer.Rectangle;

			if ( this.zoomShift )
			{
				this.zoomShift = false;
			}
			else
			{
				if ( this.drawingContext.IsCtrl || isRight )
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

			if ( this.mouseDragging )
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
				this.document.Modifier.AggregatePicker(model);
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

			if ( this.mouseDragging )
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


		// Trouve le CommandDispatcher associé au document
		protected CommandDispatcher GetCommandDispatcher()
		{
			return this.document.CommandDispatcher;
		}
		
		// Détecte l'objet pointé par la souris.
		protected Objects.Abstract Detect(Point mouse, bool selectFirst)
		{
			System.Collections.ArrayList list = this.Detects(mouse, selectFirst);
			if ( list.Count == 0 )  return null;
			return list[0] as Objects.Abstract;
		}

		// Détecte les objets pointés par la souris.
		protected System.Collections.ArrayList Detects(Point mouse, bool selectFirst)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList();
			Objects.Abstract layer = this.drawingContext.RootObject();

			if ( selectFirst )
			{
				foreach ( Objects.Abstract obj in this.document.FlatReverse(layer) )
				{
					if ( !obj.IsSelected )  continue;
					if ( obj.Detect(mouse) )  list.Add(obj);
				}

				foreach ( Objects.Abstract obj in this.document.FlatReverse(layer) )
				{
					if ( obj.IsSelected )  continue;
					if ( obj.Detect(mouse) )  list.Add(obj);
				}
			}
			else
			{
				foreach ( Objects.Abstract obj in this.document.FlatReverse(layer) )
				{
					if ( obj.Detect(mouse) )  list.Add(obj);
				}
			}

			return list;
		}

		// Détecte l'objet éditable pointé par la souris.
		protected Objects.Abstract DetectEdit(Point mouse, bool onlyTextBox2, out Objects.DetectEditType handle)
		{
			Objects.Abstract layer = this.drawingContext.RootObject();

			foreach ( Objects.Abstract obj in this.document.FlatReverse(layer) )
			{
				if ( onlyTextBox2 && !(obj is Objects.TextBox2) )  continue;
				if ( !obj.IsSelected )  continue;

				handle = obj.DetectEdit(mouse);
				if ( handle != Objects.DetectEditType.Out )  return obj;
			}

			foreach ( Objects.Abstract obj in this.document.FlatReverse(layer) )
			{
				if ( onlyTextBox2 && !(obj is Objects.TextBox2) )  continue;
				if ( obj.IsSelected )  continue;

				handle = obj.DetectEdit(mouse);
				if ( handle != Objects.DetectEditType.Out )  return obj;
			}

			handle = Objects.DetectEditType.Out;
			return null;
		}

		// Détecte la poignée pointée par la souris.
		protected bool DetectHandle(Point mouse, out Objects.Abstract detect, out int rank)
		{
			Objects.Abstract layer = this.drawingContext.RootObject();
			double min = 1000000.0;
			Objects.Abstract best = null;
			int found = -1;
			foreach ( Objects.Abstract obj in this.document.FlatReverse(layer, true) )
			{
				rank = obj.DetectHandle(mouse);
				if ( rank != -1 )
				{
					double distance = Point.Distance(obj.Handle(rank).Position, mouse);
					if ( distance < min )
					{
						min = distance;
						best = obj;
						found = rank;
					}
				}
			}

			detect = best;
			rank = found;
			return ( detect != null );
		}

		// Détecte la poignée d'un segment sélectionné pointée par la souris.
		protected bool DetectSelectedSegmentHandle(Point mouse, out Objects.Abstract detect, out int rank)
		{
			Objects.Abstract layer = this.drawingContext.RootObject();
			double min = 1000000.0;
			Objects.Abstract best = null;
			int found = -1;
			foreach ( Objects.Abstract obj in this.document.FlatReverse(layer, true) )
			{
				rank = obj.DetectSelectedSegmentHandle(mouse);
				if ( rank != -1 )
				{
					double distance = Point.Distance(obj.Handle(rank).Position, mouse);
					if ( distance < min )
					{
						min = distance;
						best = obj;
						found = rank;
					}
				}
			}

			detect = best;
			rank = found;
			return ( detect != null );
		}

		// Annule le hilite des objets.
		public void ClearHilite()
		{
			this.Hilite(null);
			this.ShaperHilite(null, Point.Empty);
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

		// Hilite un objet pour le modeleur.
		protected void ShaperHilite(Objects.Abstract item, Point mouse)
		{
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer) )
			{
				if ( obj.IsSelected )
				{
					obj.ShaperHiliteSegment(obj == item, mouse);
				}
				else
				{
					obj.ShaperHiliteHandles(obj == item);
				}
			}
		}

		// Adapte les objets sélectionnés à l'outil modeleur.
		public void SelectToShaper()
		{
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
			{
				obj.ShaperHiliteHandles(true);
				obj.SelectHandle(-1, false);
			}

			this.selector.Visible = false;
			this.selector.Handles = false;
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

		// Mode de sélection.
		public SelectorType SelectorType
		{
			get
			{
				return this.selector.TypeChoice;
			}

			set
			{
				using ( this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.SelectMode) )
				{
					this.selector.TypeChoice = value;
					this.UpdateSelector();
					this.document.Notifier.NotifySelectionChanged();
					this.document.Modifier.OpletQueueValidateAction();
				}
			}
		}

		// Mode de sélection.
		public SelectorTypeStretch SelectorTypeStretch
		{
			get
			{
				return this.selector.TypeStretch;
			}

			set
			{
				using ( this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.StretchType) )
				{
					this.selector.TypeChoice = SelectorType.Stretcher;
					this.selector.TypeStretch = value;
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

		// Adapte les traits lors d'un zoom ou d'une rotation.
		public bool SelectorAdaptLine
		{
			get
			{
				return this.selectorAdaptLine;
			}

			set
			{
				if ( this.selectorAdaptLine != value )
				{
					this.selectorAdaptLine = value;
					this.document.Notifier.NotifySelectionChanged();
				}
			}
		}

		// Adapte les textes (Font et Justif) lors d'un zoom ou d'une rotation.
		public bool SelectorAdaptText
		{
			get
			{
				return this.selectorAdaptText;
			}
			
			set
			{
				if ( this.selectorAdaptText != value )
				{
					this.selectorAdaptText = value;
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
			if ( this.document.Modifier.IsToolShaper )  return;

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
						st = SelectorType.Scaler;
					}
				}

				if ( st == SelectorType.Individual )
				{
					this.selector.Visible = false;
					this.selector.Handles = false;
					this.GlobalSelectedUpdate(false);
				}
				else
				{
					this.selector.TypeInUse = st;
					double angle = this.GetSelectedAngle();
					this.SelectorInitialize(rect, angle);
					this.GlobalSelectedUpdate(true);
				}
			}
		}

		// Initialise le rectangle du selector.
		protected void SelectorInitialize(Drawing.Rectangle rect, double angle)
		{
			rect.Inflate(5.0/this.drawingContext.ScaleX);
			this.selector.Initialize(rect, angle);
		}

		// Donne l'angle de ou des objets sélectionnés.
		protected double GetSelectedAngle()
		{
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
			{
				return obj.Direction;
			}
			return 0.0;
		}

		// Indique si tous les objets sélectionnés le sont globalement.
		protected void GlobalSelectedUpdate(bool global)
		{
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
			{
				obj.GlobalSelect(global);
			}
		}

		// Sélectionne l'objet directement dessous l'objet déjà sélectionné.
		protected void SelectOther(Point mouse, Objects.Abstract actual)
		{
			System.Collections.ArrayList list = this.Detects(mouse, false);
			if ( list.Count == 0 )  return;

			int i = list.IndexOf(actual);
			if ( i == -1 )  return;

			i ++;  // l'objet directement dessous
			if ( i >= list.Count )  i = 0;
			Objects.Abstract obj = list[i] as Objects.Abstract;

			this.Select(obj, false, false);
		}

		// Sélectionne un objet et désélectionne tous les autres.
		public void Select(Objects.Abstract item, bool edit, bool add)
		{
			this.document.Modifier.UpdateCounters();
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
			this.document.Modifier.UpdateCounters();
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

			if ( this.document.Modifier.TotalSelected == 0 || this.document.Modifier.IsToolShaper )
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

			this.document.Modifier.AddMoveAfterDuplicate(move);
		}

		// Début du déplacement global de tous les objets sélectionnés.
		public void MoveGlobalStarting()
		{
			this.selector.InitialBBoxThin = this.document.Modifier.SelectedBboxThin;
			this.selector.FinalToInitialData();

			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Deep(layer, true) )
			{
				obj.MoveGlobalStartingProperties();
				obj.MoveGlobalStarting();
			}
		}

		// Effectue le déplacement global de tous les objets sélectionnés.
		public void MoveGlobalProcess(Selector selector)
		{
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Deep(layer, true) )
			{
				obj.MoveGlobalProcessProperties(selector);
				obj.MoveGlobalProcess(selector);
			}
		}

		// Dessine les noms de tous les objets.
		public void DrawLabels(Graphics graphics)
		{
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer) )
			{
				obj.DrawLabel(graphics, this.drawingContext);
			}
		}

		// Dessine les noms de tous les styles.
		public void DrawAggregates(Graphics graphics)
		{
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer) )
			{
				obj.DrawAggregate(graphics, this.drawingContext);
			}
		}

		// Dessine les poignées de tous les objets.
		public void DrawHandles(Graphics graphics)
		{
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer) )
			{
				obj.DrawHandle(graphics, this.drawingContext);
			}

			this.hotSpotHandle.Draw(graphics, this.drawingContext);
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


		#region MiniBar
		// Ouvre la mini-palette.
		public void OpenMiniBar(Point mouse, bool delayed, bool noSelected)
		{
			this.CloseMiniBar(false);

			System.Collections.ArrayList cmds = this.MiniBarCommands(noSelected);
			if ( cmds == null || cmds.Count == 0 )  return;

			Widgets.Balloon frame = new Widgets.Balloon();
			IconButton button = new IconButton();
			IconSeparator sep = new IconSeparator();

			mouse = this.InternalToScreen(mouse);
			mouse.Y ++;  // pour ne pas être sur le pixel visé par la souris

			ScreenInfo si = ScreenInfo.Find(this.MapClientToScreen(mouse));
			Drawing.Rectangle wa = si.WorkingArea;

			this.miniBarLines = 1;
			double maxWidth = 0;
			double width = 0;
			foreach ( string cmd in cmds )
			{
				if ( cmd == "" )  // séparateur ?
				{
					width += sep.DefaultWidth;
				}
				else if ( cmd == "#" )  // fin de ligne ?
				{
					maxWidth = System.Math.Max(maxWidth, width);
					width = 0;
					this.miniBarLines ++;
				}
				else
				{
					width += button.DefaultWidth;
				}
			}
			maxWidth = System.Math.Max(maxWidth, width);

			double mx = maxWidth + frame.Margin*2 + 1;
			double my = button.DefaultHeight*this.miniBarLines + frame.Margin*(this.miniBarLines-1) + frame.Margin*2 + frame.Distance;
			Size size = new Size(mx, my);
			mouse.X -= size.Width/2;
			this.miniBarRect = new Drawing.Rectangle(mouse, size);
			this.miniBarHot = size.Width/2;

			Drawing.Rectangle rect = this.MapClientToScreen(this.miniBarRect);
			if ( rect.Left < wa.Left )  // dépasse à gauche ?
			{
				double dx = wa.Left-rect.Left;
				this.miniBarRect.Offset(dx, 0);
				this.miniBarHot -= dx;
			}

			if ( rect.Right > wa.Right )  // dépasse à droite ?
			{
				double dx = rect.Right-wa.Right;
				this.miniBarRect.Offset(-dx, 0);
				this.miniBarHot += dx;
			}

			this.miniBarCmds = cmds;

			this.miniBarTimer.Delay = delayed ? 0.2 : 0.01;
			this.miniBarTimer.Start();
		}

		// Appelé lorsque le timer arrive à échéance.
		protected void HandleMiniBarTimeElapsed(object sender)
		{
			this.miniBarTimer.Suspend();
			this.CreateMiniBar();
		}

		// Crée la mini-palette.
		protected void CreateMiniBar()
		{
			Point mouse;
			if ( !this.MousePos(out mouse) )
			{
				this.miniBarCmds = null;
				return;
			}

			this.miniBar = new Window();
			this.miniBar.MakeFramelessWindow();
			this.miniBar.MakeFloatingWindow();
			this.miniBar.DisableMouseActivation();
			this.miniBar.MakeLayeredWindow(true);
			this.miniBar.Root.SetSyncPaint(true);
			this.miniBar.WindowSize = this.miniBarRect.Size;
			this.miniBar.WindowLocation = this.MapClientToScreen(this.miniBarRect.BottomLeft);
			this.miniBar.Root.BackColor = Color.FromARGB(0, 1,1,1);
			this.miniBar.Owner = this.Window;
			this.miniBar.AttachCommandDispatcher(this.GetCommandDispatcher());

			this.miniBarBalloon = new Widgets.Balloon();
			this.miniBarBalloon.Hot = this.miniBarHot;
			this.miniBarBalloon.SetParent(this.miniBar.Root);
			this.miniBarBalloon.Anchor = AnchorStyles.All;
			this.miniBarBalloon.CloseNeeded += new EventHandler(this.HandleMiniBarCloseNeeded);
			this.miniBarBalloon.Attach();

			mouse = this.InternalToScreen(mouse);
			mouse = this.MapClientToScreen(mouse);
			if ( this.miniBarBalloon.IsAway(mouse) )  // souris déjà trop loin ?
			{
				this.CloseMiniBar(false);
				this.miniBarCmds = null;
				return;
			}

			CommandDispatcher cd = this.GetCommandDispatcher();

			bool beginOfLine = true;
			Widget line = null;
			foreach ( string cmd in this.miniBarCmds )
			{
				if ( beginOfLine )
				{
					double m = (line == null) ? 0 : this.miniBarBalloon.Margin;
					IconButton button = new IconButton();
					line = new Widget(this.miniBarBalloon);
					line.Height = button.DefaultHeight;
					line.Dock = DockStyle.Top;
					line.DockMargins = new Margins(0, 0, m, 0);
					beginOfLine = false;
				}

				if ( cmd == "" )  // séparateur ?
				{
					IconSeparator sep = new IconSeparator();
					sep.Dock = DockStyle.Left;
					sep.SetParent(line);
				}
				else if ( cmd == "#" )  // fin de ligne ?
				{
					beginOfLine = true;
				}
				else
				{
					IconButton button = new IconButton(cmd, Misc.Icon(cmd), cmd);
					
					CommandState state = cd[cmd];
					if ( state.Statefull )
					{
						button.ButtonStyle = ButtonStyle.ActivableIcon;
					}

					button.Dock = DockStyle.Left;
					button.SetParent(line);
					button.Clicked += new MessageEventHandler(this.HandleMiniBarButtonClicked);

					string s = Res.Strings.GetString("Action."+cmd);
					if ( s != null )
					{
						ToolTip.Default.SetToolTip(button, s);
					}
				}
			}
			this.miniBarCmds = null;

			this.miniBar.Show();
		}

		// Appelé lorsque la souris s'est éloignée est que la fermeture est nécessaire.
		private void HandleMiniBarCloseNeeded(object sender)
		{
			this.CloseMiniBar(true);
		}

		private void HandleMiniBarButtonClicked(object sender, MessageEventArgs e)
		{
			IconButton button = sender as IconButton;
			if ( button != null )
			{
				if ( button.Name == "OrderUpAll"            ||
					 button.Name == "OrderUpOne"            ||
					 button.Name == "OrderDownOne"          ||
					 button.Name == "OrderDownAll"          ||
					 button.Name == "FontSizeMinus"         ||
					 button.Name == "FontSizePlus"          ||
					 button.Name == "ParagraphIndentMinus"  ||
					 button.Name == "ParagraphIndentPlus"   ||
					 button.Name == "ParagraphLeadingMinus" ||
					 button.Name == "ParagraphLeadingPlus"  )  return;
			}

			this.CloseMiniBar(false);
		}

		// Ferme la mini-palette.
		public void CloseMiniBar(bool fadeout)
		{
			this.miniBarTimer.Suspend();

			if ( this.miniBar != null )
			{
				if ( fadeout )
				{
					this.miniBar.WindowAnimationEnded += new EventHandler(this.HandleMiniBarWindowAnimationEnded);
					this.miniBar.AnimateShow(Animation.FadeOut);
				}
				else
				{
					this.miniBar.Close();
					this.miniBar.AsyncDispose();
				}

				this.miniBarBalloon.CloseNeeded -= new EventHandler(this.HandleMiniBarCloseNeeded);
				this.miniBarBalloon.Detach();

				this.miniBarBalloon = null;
				this.miniBar = null;
			}
		}
		
		// Quand l'animation de fermeture de la mini-palette est terminée, il faut
		// encore supprimer la fenêtre, pour éviter qu'elle ne traîne ad eternum.
		private void HandleMiniBarWindowAnimationEnded(object sender)
		{
			Window miniBar = sender as Window;
			miniBar.AsyncDispose();
		}

		// Retourne la liste des commandes pour la mini-palette.
		protected System.Collections.ArrayList MiniBarCommands(bool noSelected)
		{
			this.document.Notifier.GenerateEvents();
			System.Collections.ArrayList list = new System.Collections.ArrayList();

			int nbSel = this.document.Modifier.TotalSelected;
			if ( nbSel == 0 )
			{
				if ( noSelected )
				{
					this.MiniBarAdd(list, "DeselectAll");
					this.MiniBarAdd(list, "SelectAll");
					this.MiniBarAdd(list, "SelectInvert");
					this.MiniBarAdd(list, "");
					this.MiniBarAdd(list, "HideSel");
					this.MiniBarAdd(list, "HideRest");
					this.MiniBarAdd(list, "HideCancel");
					this.MiniBarAdd(list, "");
					this.MiniBarAdd(list, "ZoomPage");
					this.MiniBarAdd(list, "ZoomDefault");
					this.MiniBarAdd(list, "");
					this.MiniBarAdd(list, "Outside");
					this.MiniBarAdd(list, "Grid");
					this.MiniBarAdd(list, "Magnet");
					this.MiniBarAdd(list, "");
				}
			}
			else
			{
				if ( this.document.Modifier.Tool == "ToolSelect" )
				{
					this.MiniBarAdd(list, "Delete");
					this.MiniBarAdd(list, "Duplicate");
					this.MiniBarAdd(list, "");
					this.MiniBarAdd(list, "Cut");
					this.MiniBarAdd(list, "Copy");
					this.MiniBarAdd(list, "");
					this.MiniBarAdd(list, "HideSel");
					this.MiniBarAdd(list, "HideRest");
					this.MiniBarAdd(list, "HideCancel");
					this.MiniBarAdd(list, "");
					this.MiniBarAdd(list, "ZoomSel");
					this.MiniBarAdd(list, "ZoomSelWidth");
					this.MiniBarAdd(list, "");
					this.MiniBarAdd(list, "OrderDownAll");
					this.MiniBarAdd(list, "OrderDownOne");
					this.MiniBarAdd(list, "OrderUpOne");
					this.MiniBarAdd(list, "OrderUpAll");
					this.MiniBarAdd(list, "");
					this.MiniBarAdd(list, "Group");
					this.MiniBarAdd(list, "Merge");
					this.MiniBarAdd(list, "Extract");
					this.MiniBarAdd(list, "Ungroup");
					this.MiniBarAdd(list, "Inside");
					this.MiniBarAdd(list, "Outside");
					this.MiniBarAdd(list, "");
#if false
					this.MiniBarAdd(list, "Rotate90");
					this.MiniBarAdd(list, "Rotate180");
					this.MiniBarAdd(list, "Rotate270");
					this.MiniBarAdd(list, "");
					this.MiniBarAdd(list, "MirrorH");
					this.MiniBarAdd(list, "MirrorV");
					this.MiniBarAdd(list, "");
					this.MiniBarAdd(list, "ScaleDiv2");
					this.MiniBarAdd(list, "ScaleMul2");
					this.MiniBarAdd(list, "");
					this.MiniBarAdd(list, "Combine");
					this.MiniBarAdd(list, "Uncombine");
					this.MiniBarAdd(list, "ToBezier");
					this.MiniBarAdd(list, "ToPoly");
					this.MiniBarAdd(list, "Fragment");
					this.MiniBarAdd(list, "");
					this.MiniBarAdd(list, "BooleanOr");
					this.MiniBarAdd(list, "BooleanAnd");
					this.MiniBarAdd(list, "BooleanXor");
					this.MiniBarAdd(list, "BooleanFrontMinus");
					this.MiniBarAdd(list, "BooleanBackMinus");
					this.MiniBarAdd(list, "");
#endif
				}

				Objects.Abstract layer = this.drawingContext.RootObject();
				foreach ( Objects.Abstract obj in this.document.Deep(layer, true) )
				{
					obj.PutCommands(list);
				}
			}

			if ( list.Count != 0 )
			{
				string last = list[list.Count-1] as string;
				if ( last == "" )  // terminé par un séparateur ?
				{
					list.RemoveAt(list.Count-1);  // supprime le séparateur final
				}
			}

			// Essaie différentes largeurs de justifications, pour retenir la meilleure,
			// c'est-à-dire celle qui a le moins de déchets (place perdue sur la dernière ligne).
			int bestScraps = 1000;
			int bestHope = 8;
			int linesRequired = this.MiniBarCount(list)/8 + 1;
			for ( int hope=2 ; hope<=16 ; hope++ )
			{
				if ( this.MiniBarJustifDo(list, hope) == linesRequired )
				{
					int scraps = this.MiniBarJustifScraps(list);
					if ( bestScraps > scraps )
					{
						bestScraps = scraps;
						bestHope = hope;
					}
				}

				this.MiniBarJustifClear(list);
			}
			this.MiniBarJustifDo(list, bestHope);

			return list;
		}

		// Compte le nombre de commandes dans une liste.
		protected int MiniBarCount(System.Collections.ArrayList list)
		{
			int count = 0;
			foreach ( string cmd in list )
			{
				if ( cmd != "" )  count ++;
			}
			return count;
		}

		// Justifie la mini-palette, en remplaçant certains séparateurs ("") par une marque
		// de fin de ligne ("#").
		// Retourne le nombre de lignes nécessaires.
		protected int MiniBarJustifDo(System.Collections.ArrayList list, int hope)
		{
			int count = this.MiniBarCount(list);
			if ( count < hope )  return 1;

			int inlineCount = 0;
			int lineCount = 1;
			for ( int i=0 ; i<list.Count ; i++ )
			{
				string cmd = list[i] as string;

				if ( cmd == "" )  // séparateur ?
				{
					if ( inlineCount >= hope )
					{
						list.RemoveAt(i);     // supprime le séparateur...
						list.Insert(i, "#");  // ...et remplace-le par une marque de fin de ligne
						inlineCount = 0;
						lineCount ++;
					}
				}
				else	// commande ?
				{
					inlineCount ++;
				}
			}
			return lineCount;
		}

		// Supprime la justification de la mini-palette.
		protected void MiniBarJustifClear(System.Collections.ArrayList list)
		{
			for ( int i=0 ; i<list.Count ; i++ )
			{
				string cmd = list[i] as string;

				if ( cmd == "#" )  // fin de ligne d'un essai précédent ?
				{
					list.RemoveAt(i);    // supprime la marque de fin de ligne...
					list.Insert(i, "");  // ...et remplace-la par un séparateur
				}
			}
		}

		// Retourne la longueur inutilisée la plus grande. Il s'agit généralement de la place
		// perdue à la fin de la dernière ligne.
		protected int MiniBarJustifScraps(System.Collections.ArrayList list)
		{
			int shortestLine = 1000;
			int longestLine = 0;
			int index = 0;
			foreach ( string cmd in list )
			{
				if ( cmd == "" )  // séparateur ?
				{
				}
				else if ( cmd == "#" )  // fin de ligne ?
				{
					shortestLine = System.Math.Min(shortestLine, index);
					longestLine  = System.Math.Max(longestLine,  index);
					index = 0;
				}
				else	// commande ?
				{
					index ++;
				}
			}
			shortestLine = System.Math.Min(shortestLine, index);
			longestLine  = System.Math.Max(longestLine,  index);

			return longestLine-shortestLine;
		}

		// Ajoute une commande dans la liste pour la mini-palette.
		// Une commande n'est qu'une seule fois dans la liste.
		// Si la commande est disable (selon le CommandDispatcher), elle n'est pas ajoutée.
		public void MiniBarAdd(System.Collections.ArrayList list, string cmd)
		{
			if ( cmd == "" )  // séparateur ?
			{
				if ( list.Count == 0 )  return;

				string last = list[list.Count-1] as string;
				if ( last == "" )  return;  // déjà un séparateur ?
			}
			else	// commande ?
			{
				if ( list.Contains(cmd) )  return;  // déjà dans la liste ?

				CommandDispatcher cd = this.GetCommandDispatcher();
				CommandState state = cd[cmd];
				if ( state != null )
				{
					if ( !state.Enable )  return;
				}
			}

			list.Add(cmd);
		}
		#endregion


		#region ContextMenu
		// Construit le menu contextuel.
		protected void ContextMenu(Point mouse, bool globalMenu)
		{
			this.ClearHilite();

			int nbSel = this.document.Modifier.TotalSelected;
			bool exist;

			// Construit le sous-menu "ordre".
			if ( globalMenu || nbSel == 0 )
			{
				this.contextMenuOrder = null;
			}
			else
			{
				System.Collections.ArrayList listOrder = new System.Collections.ArrayList();

				exist = false;
				exist |= ContextMenuItem.MenuAddItem(listOrder, this.GetCommandDispatcher(), "OrderUpAll",   Misc.Icon("OrderUpAll"),   Res.Strings.Action.OrderUpAll);
				exist |= ContextMenuItem.MenuAddItem(listOrder, this.GetCommandDispatcher(), "OrderUpOne",   Misc.Icon("OrderUpOne"),   Res.Strings.Action.OrderUpOne);
				exist |= ContextMenuItem.MenuAddItem(listOrder, this.GetCommandDispatcher(), "OrderDownOne", Misc.Icon("OrderDownOne"), Res.Strings.Action.OrderDownOne);
				exist |= ContextMenuItem.MenuAddItem(listOrder, this.GetCommandDispatcher(), "OrderDownAll", Misc.Icon("OrderDownAll"), Res.Strings.Action.OrderDownAll);

				if ( ContextMenuItem.IsMenuActive(listOrder) )
				{
					this.contextMenuOrder = new VMenu();
					this.contextMenuOrder.Host = this;
					ContextMenuItem.MenuCreate(this.contextMenuOrder, listOrder);
					this.contextMenuOrder.AdjustSize();
				}
				else
				{
					this.contextMenuOrder = null;
				}
			}

			// Construit le sous-menu "opérations".
			if ( globalMenu || nbSel == 0 )
			{
				this.contextMenuOper = null;
			}
			else
			{
				System.Collections.ArrayList listOper = new System.Collections.ArrayList();

				exist = false;
				exist |= ContextMenuItem.MenuAddItem(listOper, this.GetCommandDispatcher(), "Rotate90",  Misc.Icon("Rotate90"),  Res.Strings.Action.Rotate90);
				exist |= ContextMenuItem.MenuAddItem(listOper, this.GetCommandDispatcher(), "Rotate180", Misc.Icon("Rotate180"), Res.Strings.Action.Rotate180);
				exist |= ContextMenuItem.MenuAddItem(listOper, this.GetCommandDispatcher(), "Rotate270", Misc.Icon("Rotate270"), Res.Strings.Action.Rotate270);
				if ( exist )  ContextMenuItem.MenuAddSep(listOper);

				exist = false;
				exist |= ContextMenuItem.MenuAddItem(listOper, this.GetCommandDispatcher(), "MirrorH",   Misc.Icon("MirrorH"),   Res.Strings.Action.MirrorH);
				exist |= ContextMenuItem.MenuAddItem(listOper, this.GetCommandDispatcher(), "MirrorV",   Misc.Icon("MirrorV"),   Res.Strings.Action.MirrorV);
				if ( exist )  ContextMenuItem.MenuAddSep(listOper);

				exist = false;
				exist |= ContextMenuItem.MenuAddItem(listOper, this.GetCommandDispatcher(), "ScaleDiv2", Misc.Icon("ScaleDiv2"), Res.Strings.Action.ScaleDiv2);
				exist |= ContextMenuItem.MenuAddItem(listOper, this.GetCommandDispatcher(), "ScaleMul2", Misc.Icon("ScaleMul2"), Res.Strings.Action.ScaleMul2);

				if ( ContextMenuItem.IsMenuActive(listOper) )
				{
					this.contextMenuOper = new VMenu();
					this.contextMenuOper.Host = this;
					ContextMenuItem.MenuCreate(this.contextMenuOper, listOper);
					this.contextMenuOper.AdjustSize();
				}
				else
				{
					this.contextMenuOper = null;
				}
			}

			// Construit le sous-menu "géométrie".
			if ( globalMenu || nbSel == 0 )
			{
				this.contextMenuGeom = null;
			}
			else
			{
				System.Collections.ArrayList listGeom = new System.Collections.ArrayList();

				exist = false;
				exist |= ContextMenuItem.MenuAddItem(listGeom, this.GetCommandDispatcher(), "Combine",   Misc.Icon("Combine"),   Res.Strings.Action.Combine);
				exist |= ContextMenuItem.MenuAddItem(listGeom, this.GetCommandDispatcher(), "Uncombine", Misc.Icon("Uncombine"), Res.Strings.Action.Uncombine);
				exist |= ContextMenuItem.MenuAddItem(listGeom, this.GetCommandDispatcher(), "ToBezier",  Misc.Icon("ToBezier"),  Res.Strings.Action.ToBezier);
				exist |= ContextMenuItem.MenuAddItem(listGeom, this.GetCommandDispatcher(), "ToPoly",    Misc.Icon("ToPoly"),    Res.Strings.Action.ToPoly);
				exist |= ContextMenuItem.MenuAddItem(listGeom, this.GetCommandDispatcher(), "Fragment",  Misc.Icon("Fragment"),  Res.Strings.Action.Fragment);

				if ( ContextMenuItem.IsMenuActive(listGeom) )
				{
					this.contextMenuGeom = new VMenu();
					this.contextMenuGeom.Host = this;
					ContextMenuItem.MenuCreate(this.contextMenuGeom, listGeom);
					this.contextMenuGeom.AdjustSize();
				}
				else
				{
					this.contextMenuGeom = null;
				}
			}

			// Construit le sous-menu "booléen".
			if ( globalMenu || nbSel == 0 )
			{
				this.contextMenuBool = null;
			}
			else
			{
				System.Collections.ArrayList listBool = new System.Collections.ArrayList();

				exist = false;
				exist |= ContextMenuItem.MenuAddItem(listBool, this.GetCommandDispatcher(), "BooleanOr",         Misc.Icon("BooleanOr"),         Res.Strings.Action.BooleanOr);
				exist |= ContextMenuItem.MenuAddItem(listBool, this.GetCommandDispatcher(), "BooleanAnd",        Misc.Icon("BooleanAnd"),        Res.Strings.Action.BooleanAnd);
				exist |= ContextMenuItem.MenuAddItem(listBool, this.GetCommandDispatcher(), "BooleanXor",        Misc.Icon("BooleanXor"),        Res.Strings.Action.BooleanXor);
				exist |= ContextMenuItem.MenuAddItem(listBool, this.GetCommandDispatcher(), "BooleanFrontMinus", Misc.Icon("BooleanFrontMinus"), Res.Strings.Action.BooleanFrontMinus);
				exist |= ContextMenuItem.MenuAddItem(listBool, this.GetCommandDispatcher(), "BooleanBackMinus",  Misc.Icon("BooleanBackMinus"),  Res.Strings.Action.BooleanBackMinus);

				if ( ContextMenuItem.IsMenuActive(listBool) )
				{
					this.contextMenuBool = new VMenu();
					this.contextMenuBool.Host = this;
					ContextMenuItem.MenuCreate(this.contextMenuBool, listBool);
					this.contextMenuBool.AdjustSize();
				}
				else
				{
					this.contextMenuBool = null;
				}
			}

			// Construit le menu principal.
			System.Collections.ArrayList list = new System.Collections.ArrayList();
			if ( globalMenu || nbSel == 0 )
			{
				exist = false;
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "DeselectAll",  Misc.Icon("DeselectAll"),  Res.Strings.Action.DeselectAll);
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "SelectAll",    Misc.Icon("SelectAll"),    Res.Strings.Action.SelectAll);
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "SelectInvert", Misc.Icon("SelectInvert"), Res.Strings.Action.SelectInvert);
				if ( exist )  ContextMenuItem.MenuAddSep(list);

				exist = false;
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "HideSel",      Misc.Icon("HideSel"),      Res.Strings.Action.HideSel);
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "HideRest",     Misc.Icon("HideRest"),     Res.Strings.Action.HideRest);
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "HideCancel",   Misc.Icon("HideCancel"),   Res.Strings.Action.HideCancel);
				if ( exist )  ContextMenuItem.MenuAddSep(list);

				exist = false;
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "ZoomMin",      Misc.Icon("ZoomMin"),      Res.Strings.Action.ZoomMin);
				if ( this.document.Type != DocumentType.Pictogram )
				{
					exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "ZoomPage",      Misc.Icon("ZoomPage"),      Res.Strings.Action.ZoomPage);
					exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "ZoomPageWidth", Misc.Icon("ZoomPageWidth"), Res.Strings.Action.ZoomPageWidth);
				}
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "ZoomDefault",  Misc.Icon("ZoomDefault"),  Res.Strings.Action.ZoomDefault);
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "ZoomSel",      Misc.Icon("ZoomSel"),      Res.Strings.Action.ZoomSel);
				if ( this.document.Type != DocumentType.Pictogram )
				{
					exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "ZoomSelWidth",  Misc.Icon("ZoomSelWidth"),  Res.Strings.Action.ZoomSelWidth);
				}
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "ZoomPrev",     Misc.Icon("ZoomPrev"),     Res.Strings.Action.ZoomPrev);
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "ZoomSub",      Misc.Icon("ZoomSub"),      Res.Strings.Action.ZoomSub);
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "ZoomAdd",      Misc.Icon("ZoomAdd"),      Res.Strings.Action.ZoomAdd);
				if ( exist )  ContextMenuItem.MenuAddSep(list);

				exist = false;
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "Outside",      Misc.Icon("Outside"),      Res.Strings.Action.Outside);
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "Grid",         Misc.Icon("Grid"),         Res.Strings.Action.Grid);
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "Magnet",       Misc.Icon("Magnet"),       Res.Strings.Action.Magnet);
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

				exist = false;
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "Delete",    Misc.Icon("Delete"),    Res.Strings.Action.Delete);
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "Duplicate", Misc.Icon("Duplicate"), Res.Strings.Action.Duplicate);
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "Group",     Misc.Icon("Group"),     Res.Strings.Action.Group);
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "Merge",     Misc.Icon("Merge"),     Res.Strings.Action.Merge);
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "Extract",   Misc.Icon("Extract"),   Res.Strings.Action.Extract);
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "Ungroup",   Misc.Icon("Ungroup"),   Res.Strings.Action.Ungroup);
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "Inside",    Misc.Icon("Inside"),    Res.Strings.Action.Inside);
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "Outside",   Misc.Icon("Outside"),   Res.Strings.Action.Outside);
				if ( exist )  ContextMenuItem.MenuAddSep(list);

				exist = false;
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "ZoomSel",   Misc.Icon("ZoomSel"),   Res.Strings.Action.ZoomSel);
				if ( this.document.Type != DocumentType.Pictogram )
				{
					exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "ZoomSelWidth", Misc.Icon("ZoomSelWidth"), Res.Strings.Action.ZoomSelWidth);
				}
				if ( exist )  ContextMenuItem.MenuAddSep(list);

				exist = false;
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "HideSel",    Misc.Icon("HideSel"),    Res.Strings.Action.HideSel);
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "HideRest",   Misc.Icon("HideRest"),   Res.Strings.Action.HideRest);
				exist |= ContextMenuItem.MenuAddItem(list, this.GetCommandDispatcher(), "HideCancel", Misc.Icon("HideCancel"), Res.Strings.Action.HideCancel);
				if ( exist )  ContextMenuItem.MenuAddSep(list);

				exist = false;
				exist |= ContextMenuItem.MenuAddSubmenu(list, this.contextMenuOrder, Misc.Icon("OrderUpAll"), Res.Strings.Action.OrderMain);
				exist |= ContextMenuItem.MenuAddSubmenu(list, this.contextMenuOper,  Misc.Icon("MoveH"),      Res.Strings.Action.OperationMain);
				exist |= ContextMenuItem.MenuAddSubmenu(list, this.contextMenuGeom,  Misc.Icon("Combine"),    Res.Strings.Action.GeometryMain);
				exist |= ContextMenuItem.MenuAddSubmenu(list, this.contextMenuBool,  Misc.Icon("BooleanOr"),  Res.Strings.Action.BooleanMain);
			}

			this.contextMenu = new VMenu();
			this.contextMenu.Host = this;
			ContextMenuItem.MenuCreate(this.contextMenu, list);
			this.contextMenu.AdjustSize();
			mouse = this.InternalToScreen(mouse);
			mouse = this.MapClientToScreen(mouse);

			ScreenInfo si = ScreenInfo.Find(mouse);
			Drawing.Rectangle wa = si.WorkingArea;
			if ( mouse.Y-this.contextMenu.Height < wa.Bottom )
			{
				mouse.Y = wa.Bottom+this.contextMenu.Height;
			}
			
			this.contextMenu.ShowAsContextMenu(this.Window, mouse);
		}

		// Exécute une commande locale à un objet.
		public void CommandObject(string cmd)
		{
			if ( cmd == "CreateEnding" )
			{
				this.CreateEnding(false, false);
				return;
			}

			if ( cmd == "CreateCloseEnding" )
			{
				this.CreateEnding(false, true);
				return;
			}

			
			if ( cmd == "CreateAndSelect" )
			{
				this.CreateEnding(false, false);
				this.document.Modifier.Tool = "ToolSelect";
				return;
			}

			if ( cmd == "CreateCloseAndSelect" )
			{
				this.CreateEnding(false, true);
				this.document.Modifier.Tool = "ToolSelect";
				return;
			}

			if ( cmd == "CreateAndShaper" )
			{
				this.CreateEnding(false, false);
				this.document.Modifier.Tool = "ToolShaper";
				return;
			}

			if ( cmd == "CreateCloseAndShaper" )
			{
				this.CreateEnding(false, true);
				this.document.Modifier.Tool = "ToolShaper";
				return;
			}
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

		// Appelé lorsque la couleur dans la règle est cliquée.
		private void HandleRulerColorClicked(object sender)
		{
			Common.Widgets.TextRuler ruler = sender as Common.Widgets.TextRuler;
			this.document.Notifier.NotifyTextRulerColorClicked(ruler);
		}

		// Appelé lorsque la couleur dans la règle a changé suite à une navigation.
		private void HandleRulerColorNavigatorChanged(object sender)
		{
			Common.Widgets.TextRuler ruler = sender as Common.Widgets.TextRuler;
			this.document.Notifier.NotifyTextRulerColorChanged(ruler);
		}

		// Positionne la règle en fonction de l'éventuel objet en cours d'édition.
		public void UpdateRulerGeometry()
		{
			if ( this.textRuler == null )  return;

			Objects.Abstract editObject = this.document.Modifier.RetEditObject();
			if ( editObject == null )
			{
				this.HideRuler();
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
					this.textRuler.Visibility = true;
				}
				else
				{
					this.HideRuler();
				}
			}
		}

		// Cache la règle.
		protected void HideRuler()
		{
			if ( this.textRuler.IsVisible )
			{
				Rectangle rect = this.ScreenToInternal(this.textRuler.Bounds);
				this.document.Notifier.NotifyArea(this, rect);
			}

			this.textRuler.Visibility = false;
			this.textRuler.DetachFromText();
		}
		#endregion


		#region MouseCursor
		// Adapte le sprite de la souris en fonction des touches Shift et Ctrl.
		protected void UpdateMouseCursor(Message message)
		{
			if ( this.mouseCursorType == MouseCursorType.Arrow       ||
				 this.mouseCursorType == MouseCursorType.ArrowDup    ||
				 this.mouseCursorType == MouseCursorType.ArrowPlus   ||
				 this.mouseCursorType == MouseCursorType.ArrowGlobal )
			{
				if ( message.IsCtrlPressed && !this.mouseDragging )
				{
					this.ChangeMouseCursor(MouseCursorType.ArrowDup);
				}
				else if ( message.IsShiftPressed && !this.mouseDragging )
				{
					this.ChangeMouseCursor(MouseCursorType.ArrowPlus);
				}
				else
				{
					if ( this.document.Modifier.Tool == "ToolGlobal" )
					{
						this.ChangeMouseCursor(MouseCursorType.ArrowGlobal);
					}
					else
					{
						this.ChangeMouseCursor(MouseCursorType.Arrow);
					}
				}
			}

			if ( this.mouseCursorType == MouseCursorType.ShaperNorm  ||
				 this.mouseCursorType == MouseCursorType.ShaperPlus  ||
				 this.mouseCursorType == MouseCursorType.ShaperMove  ||
				 this.mouseCursorType == MouseCursorType.ShaperMulti )
			{
				if ( message.IsShiftPressed && !this.mouseDragging )
				{
					this.ChangeMouseCursor(MouseCursorType.ShaperPlus);
				}
				else
				{
					this.ChangeMouseCursor(this.mouseCursorType);
				}
			}

			if ( this.mouseCursorType == MouseCursorType.Finger     ||
				 this.mouseCursorType == MouseCursorType.FingerDup  ||
				 this.mouseCursorType == MouseCursorType.FingerPlus )
			{
				if ( message.IsCtrlPressed && !this.mouseDragging )
				{
					this.ChangeMouseCursor(MouseCursorType.FingerDup);
				}
				else if ( message.IsShiftPressed && !this.mouseDragging )
				{
					this.ChangeMouseCursor(MouseCursorType.FingerPlus);
				}
				else
				{
					this.ChangeMouseCursor(MouseCursorType.Finger);
				}
			}

			if ( this.mouseCursorType == MouseCursorType.Zoom          ||
				 this.mouseCursorType == MouseCursorType.ZoomMinus     ||
				 this.mouseCursorType == MouseCursorType.ZoomShift     ||
				 this.mouseCursorType == MouseCursorType.ZoomShiftCtrl )
			{
				if ( message.IsShiftPressed || (this.mouseDragging && this.zoomShift) )
				{
					if ( message.IsCtrlPressed || (this.mouseDragging && this.zoomCtrl) )
					{
						this.ChangeMouseCursor(MouseCursorType.ZoomShiftCtrl);
					}
					else
					{
						this.ChangeMouseCursor(MouseCursorType.ZoomShift);
					}
				}
				else if ( message.IsCtrlPressed )
				{
					this.ChangeMouseCursor(MouseCursorType.ZoomMinus);
				}
				else
				{
					this.ChangeMouseCursor(MouseCursorType.Zoom);
				}
			}
		}

		// Change le sprite de la souris.
		protected void ChangeMouseCursor(MouseCursorType cursor)
		{
			this.mouseCursorType = cursor;
		}

		// Utilise le bon sprite pour la souris.
		protected void UseMouseCursor()
		{
			MouseCursorType cursor = this.mouseCursorType;
			if ( this.document.GlobalSettings.FineCursor )
			{
				cursor = MouseCursorType.Fine;
			}

			if ( this.mouseCursorTypeUse == cursor )  return;

			switch ( cursor )
			{
				case MouseCursorType.Arrow:
					this.MouseCursorImage(ref this.mouseCursorArrow, Misc.Icon("Arrow"));
					break;

				case MouseCursorType.ArrowPlus:
					this.MouseCursorImage(ref this.mouseCursorArrowPlus, Misc.Icon("ArrowPlus"));
					break;

				case MouseCursorType.ArrowDup:
					this.MouseCursorImage(ref this.mouseCursorArrowDup, Misc.Icon("ArrowDup"));
					break;

				case MouseCursorType.ArrowGlobal:
					this.MouseCursorImage(ref this.mouseCursorArrowGlobal, Misc.Icon("ArrowGlobal"));
					break;

				case MouseCursorType.ShaperNorm:
					this.MouseCursorImage(ref this.mouseCursorShaperNorm, Misc.Icon("ShaperNorm"));
					break;

				case MouseCursorType.ShaperPlus:
					this.MouseCursorImage(ref this.mouseCursorShaperPlus, Misc.Icon("ShaperPlus"));
					break;

				case MouseCursorType.ShaperMove:
					this.MouseCursorImage(ref this.mouseCursorShaperMove, Misc.Icon("ShaperMove"));
					break;

				case MouseCursorType.ShaperMulti:
					this.MouseCursorImage(ref this.mouseCursorShaperMulti, Misc.Icon("ShaperMulti"));
					break;

				case MouseCursorType.Finger:
					this.MouseCursorImage(ref this.mouseCursorFinger, Misc.Icon("Finger"));
					break;

				case MouseCursorType.FingerPlus:
					this.MouseCursorImage(ref this.mouseCursorFingerPlus, Misc.Icon("FingerPlus"));
					break;

				case MouseCursorType.FingerDup:
					this.MouseCursorImage(ref this.mouseCursorFingerDup, Misc.Icon("FingerDup"));
					break;

				case MouseCursorType.Cross:
					this.MouseCursor = MouseCursor.AsCross;
					break;

				case MouseCursorType.IBeam:
					this.MouseCursor = MouseCursor.AsIBeam;
					break;

				case MouseCursorType.HSplit:
					this.MouseCursor = MouseCursor.AsHSplit;
					break;

				case MouseCursorType.VSplit:
					this.MouseCursor = MouseCursor.AsVSplit;
					break;

				case MouseCursorType.Hand:
					this.MouseCursorImage(ref this.mouseCursorHand, Misc.Icon("Hand"));
					break;

				case MouseCursorType.Pen:
					this.MouseCursorImage(ref this.mouseCursorPen, Misc.Icon("Pen"));
					break;

				case MouseCursorType.Zoom:
					this.MouseCursorImage(ref this.mouseCursorZoom, Misc.Icon("Zoom"));
					break;

				case MouseCursorType.ZoomMinus:
					this.MouseCursorImage(ref this.mouseCursorZoomMinus, Misc.Icon("ZoomMinus"));
					break;

				case MouseCursorType.ZoomShift:
					this.MouseCursorImage(ref this.mouseCursorZoomShift, Misc.Icon("ZoomShift"));
					break;

				case MouseCursorType.ZoomShiftCtrl:
					this.MouseCursorImage(ref this.mouseCursorZoomShiftCtrl, Misc.Icon("ZoomShiftCtrl"));
					break;

				case MouseCursorType.Picker:
					this.MouseCursorImage(ref this.mouseCursorPicker, Misc.Icon("Picker"));
					break;

				case MouseCursorType.PickerEmpty:
					this.MouseCursorImage(ref this.mouseCursorPickerEmpty, Misc.Icon("PickerEmpty"));
					break;

				case MouseCursorType.TextFlow:
					this.MouseCursorImage(ref this.mouseCursorTextFlow, Misc.Icon("TextFlow"));
					break;

				case MouseCursorType.TextFlowCreate:
					this.MouseCursorImage(ref this.mouseCursorTextFlowCreate, Misc.Icon("TextFlowCreate"));
					break;

				case MouseCursorType.TextFlowAdd:
					this.MouseCursorImage(ref this.mouseCursorTextFlowAdd, Misc.Icon("TextFlowAdd"));
					break;

				case MouseCursorType.TextFlowRemove:
					this.MouseCursorImage(ref this.mouseCursorTextFlowRemove, Misc.Icon("TextFlowRemove"));
					break;

				case MouseCursorType.Fine:
					this.MouseCursorImage(ref this.mouseCursorFine, Misc.Icon("FineCursor"));
					break;

				default:
					this.MouseCursor = MouseCursor.AsArrow;
					break;
			}

			this.Window.MouseCursor = this.MouseCursor;
			this.mouseCursorTypeUse = cursor;
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

		// Indique si un guide est horizontal.
		protected bool GuideIsHorizontal(int rank)
		{
			Settings.Guide guide = this.document.Settings.GuidesGet(rank);
			return guide.IsHorizontal;
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
			if ( this.document.Modifier.RetEditObject() != null )  return;

			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.GuideCreateAndMove);
			this.drawingContext.GuidesShow = true;
			this.drawingContext.GuidesMouse = true;

			Settings.Guide guide = new Settings.Guide(this.document);
			guide.Type = horizontal ? Settings.GuideType.HorizontalBottom : Settings.GuideType.VerticalLeft;
			guide.Position = Settings.Guide.Undefined;
			this.guideInteractive = this.document.Settings.GuidesAdd(guide);
			this.document.Dialogs.SelectGuide(this.guideInteractive);
			this.guideCreate = true;
			this.mouseDragging = true;
			this.ChangeMouseCursor(horizontal ? MouseCursorType.HSplit : MouseCursorType.VSplit);
		}

		// Positionne un guide interactif.
		public void GuideInteractiveMove(Point pos, bool isAlt)
		{
			if ( this.guideInteractive == -1 )  return;

			// Ne pas utiliser SnapGrid pour ignorer les repères !
			if ( this.drawingContext.GridActive ^ isAlt )
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

			// Supprime le repère s'il est tiré hors de la vue.
			Rectangle rd = this.RectangleDisplayed;
			double pos = guide.AbsolutePosition;
			if ( guide.IsHorizontal )
			{
				if ( pos < rd.Bottom || pos > rd.Top )
				{
					guide.Position = Settings.Guide.Undefined;
				}
			}
			else
			{
				if ( pos < rd.Left || pos > rd.Right )
				{
					guide.Position = Settings.Guide.Undefined;
				}
			}

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

			string title = Res.Strings.Dialog.Title;
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";
			string message = error;

			Common.Dialogs.IDialog dialog = Common.Dialogs.Message.CreateOk(title, icon, message, "", this.GetCommandDispatcher());
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
					Rectangle rect = this.document.Modifier.PageArea;
					graphics.Align(ref rect);
					rect.Offset(ix, iy);

					if ( !this.BackColor.IsTransparent )
					{
						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.BackColor);
					}
				}
			}

			graphics.LineWidth = initialWidth;
		}

		// Dessine la grille magnétique dessus.
		protected void DrawGridForeground(Graphics graphics, Rectangle clipRect)
		{
			double initialWidth = graphics.LineWidth;
			graphics.LineWidth = 1.0/this.drawingContext.ScaleX;

			double ix = 0.5/this.drawingContext.ScaleX;
			double iy = 0.5/this.drawingContext.ScaleY;

			clipRect.Inflate(1);
			clipRect = ScreenToInternal(clipRect);
			clipRect = Rectangle.Intersection(clipRect, this.document.Modifier.RectangleArea);

			if ( this.IsActiveViewer )
			{
				if ( this.document.Type == DocumentType.Graphic )
				{
					// Dessine la "page".
					Rectangle rect = this.document.Modifier.PageArea;
					graphics.Align(ref rect);
					rect.Offset(ix, iy);

					graphics.AddRectangle(rect);
					graphics.RenderSolid(Color.FromARGB(0.4, 0.5,0.5,0.5));
				}

				Rectangle area = this.document.Modifier.RectangleArea;
				graphics.Align(ref area);
				area.Offset(ix, iy);
				graphics.AddRectangle(area);
				graphics.RenderSolid(Color.FromARGB(0.4, 0.5,0.5,0.5));
			}

			if ( this.drawingContext.PreviewActive )
			{
				if ( this.document.Type == DocumentType.Graphic )
				{
					// Dessine la "page".
					Rectangle rect = this.document.Modifier.PageArea;
					graphics.Align(ref rect);
					rect.Offset(ix, iy);

					graphics.AddRectangle(rect);
					graphics.RenderSolid(Color.FromBrightness(0));

					if ( this.document.Settings.PrintInfo.Target   &&
						!this.document.Settings.PrintInfo.AutoZoom )
					{
						this.document.Printer.PaintTarget(graphics, this.drawingContext);
					}
				}
			}
			else
			{
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

				// Dessine la grille pour le texte.
				if ( this.drawingContext.TextGridShow )
				{
					double s = this.drawingContext.TextGridStep*this.drawingContext.ScaleY;
					int mul = (int) System.Math.Max(10.0/s, 1.0);

					Point origin = this.document.Modifier.OriginArea;
					origin = Point.GridAlign(origin, new Point(0, -this.drawingContext.TextGridOffset), new Point(0, this.drawingContext.TextGridStep));

					// Dessine les traits horizontaux.
					double step = this.drawingContext.TextGridStep*mul;
					int subdiv = (int) this.drawingContext.TextGridSubdiv;
					int rank = subdiv-(int)(-this.document.Modifier.OriginArea.Y/step);
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
								graphics.RenderSolid(Color.FromARGB(0.3, 0.6,0.8,0.9));  // gris-bleu
							}
							else
							{
								graphics.RenderSolid(Color.FromARGB(0.1, 0.6,0.8,0.9));  // gris-bleu
							}
						}
						rank ++;
					}
				}

				// Dessine les repères.
				if ( this.drawingContext.GuidesShow )
				{
					Objects.Page page = this.document.GetObjects[this.drawingContext.CurrentPage] as Objects.Page;

					if ( page.MasterGuides && this.drawingContext.MasterPageList.Count > 0 )
					{
						foreach ( Objects.Page masterPage in this.drawingContext.MasterPageList )
						{
							this.DrawGuides(graphics, masterPage.Guides, false);
						}
					}

					this.DrawGuides(graphics, page.Guides, !this.document.Settings.GlobalGuides);
					this.DrawGuides(graphics, this.document.Settings.GuidesListGlobal, this.document.Settings.GlobalGuides);
				}

				// Dessine les marqueurs
				this.DrawMarker(graphics);

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
			}

			graphics.LineWidth = initialWidth;
		}

		// Dessine tous les repères d'une liste.
		protected void DrawGuides(Graphics graphics, UndoableList guides, bool editable)
		{
			double ix = 0.5/this.drawingContext.ScaleX;
			double iy = 0.5/this.drawingContext.ScaleY;
			Rectangle rd = this.RectangleDisplayed;

			int total = guides.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				Settings.Guide guide = guides[i] as Settings.Guide;

				if ( guide.IsHorizontal )  // repère horizontal ?
				{
					double x = rd.Left;
					double y = guide.AbsolutePosition;
					graphics.Align(ref x, ref y);
					x += ix;
					y += iy;
					graphics.AddLine(x, y, rd.Right, y);
				}
				else	// repère vertical ?
				{
					double x = guide.AbsolutePosition;
					double y = rd.Bottom;
					graphics.Align(ref x, ref y);
					x += ix;
					y += iy;
					graphics.AddLine(x, y, x, rd.Top);
				}

				if ( editable )
				{
					if ( guide.Hilite )
					{
						graphics.RenderSolid(Color.FromARGB(0.5, 0.0,0.8,0.0));  // vert
					}
					else
					{
						graphics.RenderSolid(Color.FromARGB(0.5, 0.0,0.0,0.8));  // bleuté
					}
				}
				else
				{
					graphics.RenderSolid(Color.FromARGB(0.5, 0.8,0.0,0.0));  // rouge
				}
			}
		}

		// Dessine toutes les marques.
		protected void DrawMarker(Graphics graphics)
		{
			double ix = 0.5/this.drawingContext.ScaleX;
			double iy = 0.5/this.drawingContext.ScaleY;
			Rectangle rd = this.RectangleDisplayed;

			if ( !double.IsNaN(this.markerVertical) )
			{
				double x = this.markerVertical;
				double y = rd.Bottom;
				graphics.Align(ref x, ref y);
				x += ix;
				y += iy;
				graphics.AddLine(x, y, x, rd.Top);
				graphics.RenderSolid(Color.FromARGB(0.5, 0.0,0.0,0.8));  // bleuté
			}

			if ( !double.IsNaN(this.markerHorizontal) )
			{
				double x = rd.Left;
				double y = this.markerHorizontal;
				graphics.Align(ref x, ref y);
				x += ix;
				y += iy;
				graphics.AddLine(x, y, rd.Right, y);
				graphics.RenderSolid(Color.FromARGB(0.5, 0.0,0.0,0.8));  // bleuté
			}
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
			if ( this.Window.IsSizeMoveInProgress )
			{
				return;
			}
			
			// Ignore une zone de repeinture d'un pixel à gauche ou en haut,
			// à cause des règles qui chevauchent Viewer.
			if ( clipRect.Right == 1              ||  // un pixel à gauche ?
				 clipRect.Bottom == this.Height-1 )   // un pixel en haut ?
			{
				return;  // ignore
			}

			if ( this.lastSize != this.Size     &&  // redimensionnement ?
				 this.zoomType != ZoomType.None )   // zoom spécial ?
			{
				if ( this.zoomType == ZoomType.Page )
				{
					this.drawingContext.ZoomPageAndCenter();
				}
				if ( this.zoomType == ZoomType.PageWidth )
				{
					this.drawingContext.ZoomPageWidthAndCenter();
				}
			}
			this.lastSize = this.Size;

			if ( this.drawingContext.IsZoomPage )
			{
				this.zoomType = ZoomType.Page;
			}
			else if ( this.drawingContext.IsZoomPageWidth )
			{
				this.zoomType = ZoomType.PageWidth;
			}
			else
			{
				this.zoomType = ZoomType.None;
			}
			
			//?System.Diagnostics.Debug.WriteLine("PaintBackgroundImplementation "+clipRect.ToString());
			IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;

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

			// Dessine les noms de objets.
			if ( this.IsActiveViewer && this.drawingContext.LabelsShow && !this.drawingContext.PreviewActive )
			{
				this.DrawLabels(graphics);
			}

			// Dessine les noms de styles.
			if ( this.IsActiveViewer && this.drawingContext.AggregatesShow && !this.drawingContext.PreviewActive )
			{
				this.DrawAggregates(graphics);
			}

			// Dessine les poignées.
			if ( this.IsActiveViewer )
			{
				this.DrawHandles(graphics);
			}

			// Dessine le hotspot.
			if ( this.IsActiveViewer && this.document.Modifier.Tool == "ToolHotSpot" )
			{
				this.DrawHotSpot(graphics);
			}

			// Dessine le rectangle de modification.
			this.selector.Draw(graphics, this.drawingContext);
			this.zoomer.Draw(graphics, this.drawingContext);

			// Dessine les contraintes.
			this.drawingContext.DrawConstrain(graphics, this.document.Modifier.SizeArea);
			this.drawingContext.DrawMagnet(graphics, this.document.Modifier.SizeArea);

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
		public override void Invalidate(InvalidateReason reason)
		{
			if ( reason != InvalidateReason.FocusedChanged )
			{
				base.Invalidate(reason);
			}
		}

		protected override void OnIsFocusedChanged(Types.PropertyChangedEventArgs e)
		{
			bool focused = (bool) e.NewValue;
			
			if ( focused )
			{
				this.HandleFocused();
			}
			else
			{
				this.HandleDefocused();
			}
			
			base.OnIsFocusedChanged(e);
		}

		protected override void NotifyWindowChanged(Window oldWindow, Window newWindow)
		{
			base.NotifyWindowChanged(oldWindow, newWindow);
			
			if ( oldWindow != null )
			{
				oldWindow.DetachLogicalFocus(this);
			}
			
			if ( newWindow != null )
			{
				newWindow.AttachLogicalFocus(this);
			}
		}
		
		// Appelé lorsque la vue prend le focus.
		protected void HandleFocused()
		{
			Objects.Abstract edit = this.document.Modifier.RetEditObject();
			if ( edit != null )
			{
				this.document.Notifier.NotifyArea(this, edit.EditSelectBox);
			}
		}

		// Appelé lorsque la vue perd le focus.
		protected void HandleDefocused()
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
		protected bool							partialSelect = false;
		protected bool							selectorAdaptLine = true;
		protected bool							selectorAdaptText = true;
		protected Rectangle						redrawArea;
		protected MessageType					lastMessageType;
		protected Point							mousePosWidget;
		protected Point							mousePos;
		protected bool							mousePosValid = false;
		protected bool							mouseDragging;
		protected Point							handMouseStart;
		protected Point							moveStart;
		protected Point							moveOffset;
		protected Point							moveLast;
		protected Point							moveCenter;
		protected bool							moveAccept;
		protected bool							moveInitialSel;
		protected bool							moveReclick;
		protected int							moveHandle = -1;
		protected int							moveSelectedSegment = -1;
		protected bool							moveSelectedHandle = false;
		protected int							moveGlobal = -1;
		protected Objects.Abstract				moveObject;
		protected Objects.Abstract				hiliteHandleObject;
		protected int							hiliteHandleRank = -1;
		protected int							createRank = -1;
		protected bool							debugDirty;
		protected TextRuler						textRuler;
		protected Timer							autoScrollTimer;
		protected int							guideInteractive = -1;
		protected bool							guideCreate = false;
		protected bool							ctrlDown = false;
		protected bool							ctrlDuplicate = false;
		protected bool							zoomShift = false;
		protected bool							zoomCtrl = false;
		protected Point							zoomOrigin;
		protected Point							zoomOffset;
		protected double						zoomStart;
		protected Size							lastSize = new Size(0, 0);
		protected ZoomType						zoomType = ZoomType.None;
		protected Objects.Handle				hotSpotHandle;
		protected double						markerVertical = double.NaN;
		protected double						markerHorizontal = double.NaN;
		protected Objects.DetectEditType		editFlowPress = Objects.DetectEditType.Out;
		protected Objects.DetectEditType		editFlowSelect = Objects.DetectEditType.Out;
		protected Objects.Abstract				editFlowSrc = null;
		protected Objects.TextBox2				editFlowAfterCreate = null;

		protected Timer							miniBarTimer;
		protected System.Collections.ArrayList	miniBarCmds = null;
		protected int							miniBarLines;
		protected Drawing.Rectangle				miniBarRect;
		protected double						miniBarHot;
		protected Window						miniBar = null;
		protected Widgets.Balloon				miniBarBalloon = null;
		protected VMenu							contextMenu;
		protected VMenu							contextMenuOrder;
		protected VMenu							contextMenuOper;
		protected VMenu							contextMenuGeom;
		protected VMenu							contextMenuBool;
		protected Objects.Abstract				contextMenuObject;
		protected Point							contextMenuPos;
		protected int							contextMenuRank;

		protected MouseCursorType				mouseCursorType = MouseCursorType.Unknow;
		protected MouseCursorType				mouseCursorTypeUse = MouseCursorType.Unknow;
		protected Image							mouseCursorArrow = null;
		protected Image							mouseCursorArrowPlus = null;
		protected Image							mouseCursorArrowDup = null;
		protected Image							mouseCursorArrowGlobal = null;
		protected Image							mouseCursorShaperNorm = null;
		protected Image							mouseCursorShaperPlus = null;
		protected Image							mouseCursorShaperMove = null;
		protected Image							mouseCursorShaperMulti = null;
		protected Image							mouseCursorFinger = null;
		protected Image							mouseCursorFingerPlus = null;
		protected Image							mouseCursorFingerDup = null;
		protected Image							mouseCursorPen = null;
		protected Image							mouseCursorZoom = null;
		protected Image							mouseCursorZoomMinus = null;
		protected Image							mouseCursorZoomShift = null;
		protected Image							mouseCursorZoomShiftCtrl = null;
		protected Image							mouseCursorHand = null;
		protected Image							mouseCursorPicker = null;
		protected Image							mouseCursorPickerEmpty = null;
		protected Image							mouseCursorTextFlow = null;
		protected Image							mouseCursorTextFlowCreate = null;
		protected Image							mouseCursorTextFlowAdd = null;
		protected Image							mouseCursorTextFlowRemove = null;
		protected Image							mouseCursorFine = null;
	}
}
