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
			FingerPlus,
			FingerDup,
			Pen,
			Zoom,
			ZoomMinus,
			ZoomShift,
			ZoomShiftCtrl,
			Picker,
			PickerEmpty,
			Fine,
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


		// Retourne la largeur standard d'une ic�ne.
		public override double DefaultWidth
		{
			get
			{
				return 22;
			}
		}

		// Retourne la hauteur standard d'une ic�ne.
		public override double DefaultHeight
		{
			get
			{
				return 22;
			}
		}


		// Retourne le rectangle correspondant � la zone visible dans le Viewer.
		public Rectangle RectangleDisplayed
		{
			get
			{
				Rectangle rect = this.Client.Bounds;
				return ScreenToInternal(rect);
			}
		}

		
		#region AutoScroll
		// D�marre le timer pour l'auto-scroll.
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

		// Appel� lorsque le timer arrive � �ch�ance.
		// Effectue �ventuellement un scroll si la souris est proche des bords.
		protected void HandleTimeElapsed(object sender)
		{
			if ( this.mouseDown && this.zoomShift )  return;

			Point mouse = this.mousePosWidget;

			Rectangle view = this.Client.Bounds;
			if ( this.textRuler != null && this.textRuler.IsVisible )
			{
				if ( this.textRuler.Top == view.Top )  // r�gle tout en haut ?
				{
					view.Top -= this.textRuler.Height;  // sous la r�gle
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

			mouse = this.ScreenToInternal(mouse);  // position en coordonn�es internes
			this.DispatchDummyMouseMoveEvent();
		}

		// Retourne le rectangle correspondant � la zone visible dans le Viewer
		// dans laquelle il faut scroller.
		public Rectangle ScrollRectangle
		{
			get
			{
				Rectangle rect = this.Client.Bounds;
				if ( this.textRuler != null && this.textRuler.IsVisible )
				{
					if ( this.textRuler.Top == rect.Top )  // r�gle tout en haut ?
					{
						rect.Top -= this.textRuler.Height;  // sous la r�gle
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


		// Gestion d'un �v�nement.
		protected override void ProcessMessage(Message message, Point pos)
		{
			//?System.Diagnostics.Debug.WriteLine(string.Format("Message: {0}", message.Type));
			if ( !this.IsActiveViewer )  return;

			Modifier modifier = this.document.Modifier;
			if ( modifier == null )  return;

			// Apr�s un MouseUp, on re�oit toujours un MouseMove inutile,
			// qui est filtr� ici !!!
			if ( message.Type == MessageType.MouseMove &&
				 this.lastMessageType == MessageType.MouseUp &&
				 pos == this.mousePosWidget )
			{
				//?System.Diagnostics.Debug.WriteLine("ProcessMessage: MouseMove apr�s MouseUp poubellis� !");
				return;
			}
			this.lastMessageType = message.Type;

			this.mousePosWidget = pos;
			pos = this.ScreenToInternal(pos);  // position en coordonn�es internes

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
						this.ProcessMouseMove(message, pos);
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

					if ( this.EditProcessMessage(message, pos) )
					{
						break;
					}

					if ( this.createRank != -1 )
					{
						if ( message.KeyCode == KeyCode.Escape )
						{
							this.CreateEnding(false);
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

					if ( message.KeyCode == KeyCode.FuncF12 )
					{
						this.DirtyAllViews();
						break;
					}

					this.UseMouseCursor();
					return;

				case MessageType.KeyUp:
					if ( message.KeyCode == KeyCode.ShiftKey   ||
						 message.KeyCode == KeyCode.ControlKey )
					{
						this.UpdateMouseCursor(message);
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

		// Gestion d'un bouton de la souris press�.
		protected void ProcessMouseDown(Message message, Point pos)
		{
			Modifier modifier = this.document.Modifier;

			if ( modifier.Tool == "Select" )
			{
				this.SelectMouseDown(pos, message.ButtonDownCount, message.IsRightButton, false);
			}
			else if ( modifier.Tool == "Global" )
			{
				this.SelectMouseDown(pos, message.ButtonDownCount, message.IsRightButton, true);
			}
			else if ( modifier.Tool == "Edit" )
			{
				this.EditMouseDown(message, pos, message.ButtonDownCount);
			}
			else if ( modifier.Tool == "Zoom" )
			{
				this.ZoomMouseDown(pos, message.IsRightButton);
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

		// Gestion d'un d�placement de la souris press�.
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
				this.SelectMouseMove(pos, message.IsRightButton, false);
			}
			else if ( modifier.Tool == "Global" )
			{
				this.SelectMouseMove(pos, message.IsRightButton, true);
			}
			else if ( modifier.Tool == "Edit" )
			{
				this.EditMouseMove(message, pos);
			}
			else if ( modifier.Tool == "Zoom" )
			{
				this.ZoomMouseMove(pos, message.IsRightButton);
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

		// Gestion d'un bouton de la souris rel�ch�.
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
				this.ZoomMouseUp(pos, message.IsRightButton);
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

		// Action lorsque la molette est actionn�e.
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
				this.createRank = layer.Objects.Add(obj);  // ajoute � la fin de la liste
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
					this.createRank = layer.Objects.Add(obj);  // ajoute � la fin de la liste
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
					this.document.Modifier.OpletQueueCancelAction();  // annule les propri�t�s
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

		// Si n�cessaire, termine la cr�ation en cours.
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
				this.createRank = layer.Objects.Add(obj);  // ajoute � la fin de la liste
				this.document.Modifier.GroupUpdateParents();
				this.document.Modifier.OpletQueueValidateAction();
			}
			else
			{
				layer.Objects.RemoveAt(this.createRank);

				this.document.Modifier.OpletQueueEnable = true;
				obj.Dispose();
				this.document.Modifier.OpletQueueCancelAction();  // annule les propri�t�s
			}
			this.createRank = -1;
			this.document.Notifier.NotifyCreateChanged();
			this.document.Notifier.NotifySelectionChanged();
		}

		// Indique s'il existe un objet en cours de cr�ation.
		public bool IsCreating
		{
			get
			{
				return ( this.createRank != -1 );
			}
		}

		// Retourne le rang de l'objet en cours de cr�ation.
		public int CreateRank()
		{
			return this.createRank;
		}
		#endregion


		#region SelectMouse
		protected void SelectMouseDown(Point mouse, int downCount, bool isRight, bool global)
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
					this.document.Modifier.FlushMoveAfterDuplicate();
				}
				else if ( this.GuideDetect(mouse, out rank) )
				{
					this.guideInteractive = rank;
					this.document.Dialogs.SelectGuide(this.guideInteractive);
				}
				else
				{
					obj = this.Detect(mouse, !this.drawingContext.IsShift);
					if ( obj == null )
					{
						this.selector.FixStarting(mouse);
						this.document.Modifier.FlushMoveAfterDuplicate();
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
							this.document.Modifier.FlushMoveAfterDuplicate();
						}
						else
						{
							if ( this.drawingContext.IsShift )
							{
								obj.Deselect();
								this.document.Modifier.TotalSelected --;
								this.document.Modifier.FlushMoveAfterDuplicate();
							}
							else
							{
								this.moveReclick = true;
							}
						}

						this.moveObject = obj;
						this.moveHandle = -1;  // d�place tout l'objet
						this.drawingContext.SnapGrid(ref mouse);
						this.moveOffset = mouse;
						this.MoveAllStarting();
					}
				}
			}
		}

		protected void SelectMouseMove(Point mouse, bool isRight, bool global)
		{
			this.HiliteHandle(null, -1);
			this.selector.HiliteHandle(-1);

			if ( this.mouseDown )  // bouton souris press� ?
			{
				// Duplique le ou les objets s�lectionn�s ?
				if ( this.ctrlDown && !this.ctrlDuplicate &&
					 (this.moveGlobal != -1 || (this.moveObject != null && this.moveHandle == -1)) )
				{
					double len = Point.Distance(mouse, this.moveStart);
					if ( len > this.drawingContext.MinimalSize )
					{
						// Remet la s�lection � la position de d�part:
						if ( this.moveGlobal != -1 )  // d�place le modificateur global ?
						{
							this.selector.MoveProcess(this.moveGlobal, this.moveStart, this.drawingContext);
							this.MoveGlobalProcess(this.selector);
							this.document.Modifier.GroupUpdateChildrens();
							this.document.Modifier.GroupUpdateParents();
							this.MoveGlobalEnding();
						}

						this.document.Modifier.DuplicateSelection(new Point(0,0));
						this.document.Modifier.ActiveViewer.UpdateSelector();
						this.ctrlDuplicate = true;

						if ( this.moveGlobal != -1 )  // d�place le modificateur global ?
						{
							this.selector.MoveStarting(this.moveGlobal, this.moveStart, this.drawingContext);
							this.MoveGlobalStarting();
						}
					}
				}

				if ( this.selector.Visible && !this.selector.Handles )
				{
					this.selector.FixEnding(mouse);
				}
				else if ( this.moveGlobal != -1 )  // d�place le modificateur global ?
				{
					this.selector.MoveProcess(this.moveGlobal, mouse, this.drawingContext);
					this.selector.HiliteHandle(this.moveGlobal);
					this.MoveGlobalProcess(this.selector);
				}
				else if ( this.moveObject != null )
				{
					this.drawingContext.ConstrainFixType(ConstrainType.Normal);

					if ( this.moveHandle != -1 )  // d�place une poign�e ?
					{
						mouse -= this.moveOffset;
						this.MoveHandleProcess(this.moveObject, this.moveHandle, mouse);
						this.HiliteHandle(this.moveObject, this.moveHandle);
						this.document.Modifier.FlushMoveAfterDuplicate();
					}
					else	// d�place tout l'objet ?
					{
						this.drawingContext.ConstrainSnapPos(ref mouse);

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
							this.drawingContext.SnapGrid(ref mouse);
							this.MoveAllProcess(mouse-this.moveOffset);
							this.moveOffset = mouse;
						}
					}
				}
			}
			else	// bouton souris rel�ch� ?
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
					globalMenu = true;
				}
				else
				{
					Rectangle rSelect = this.selector.Rectangle;
					this.Select(rSelect, this.drawingContext.IsShift, this.partialSelect);
					this.UpdateSelector();
				}
			}
			else if ( this.moveGlobal != -1 )  // d�place le modificateur global ?
			{
				this.document.Modifier.GroupUpdateChildrens();
				this.document.Modifier.GroupUpdateParents();
				this.MoveGlobalEnding();
			}
			else if ( this.moveObject != null )
			{
				if ( this.moveReclick && !this.moveAccept && !isRight && !this.drawingContext.IsShift )
				{
					this.SelectOther(mouse, this.moveObject);
				}
				else
				{
					this.document.Modifier.GroupUpdateChildrens();
					this.document.Modifier.GroupUpdateParents();

					this.moveObject = null;
					this.moveHandle = -1;
					this.UpdateSelector();
				}
			}

			this.drawingContext.ConstrainDelStarting();
			this.document.Modifier.OpletQueueValidateAction();

			if ( isRight )  // avec le bouton de droite de la souris ?
			{
				this.document.Notifier.GenerateEvents();
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

			if ( this.mouseDown )  // bouton souris press� ?
			{
				this.EditProcessMessage(message, mouse);
			}
			else	// bouton souris rel�ch� ?
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

		protected bool EditProcessMessage(Message message, Point pos)
		{
			Objects.Abstract editObject = this.document.Modifier.RetEditObject();
			if ( editObject == null )  return false;

			Rectangle ibbox = editObject.BoundingBox;
			if ( editObject.EditProcessMessage(message, pos) )
			{
				this.document.Notifier.NotifyArea(ibbox);
				this.document.Notifier.NotifyArea(editObject.BoundingBox);
				return true;
			}
			return false;
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

			if ( this.mouseDown )
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
					this.document.Notifier.NotifySelectionChanged();
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

		
		// D�tecte l'objet point� par la souris.
		protected Objects.Abstract Detect(Point mouse, bool selectFirst)
		{
			System.Collections.ArrayList list = this.Detects(mouse, selectFirst);
			if ( list.Count == 0 )  return null;
			return list[0] as Objects.Abstract;
		}

		// D�tecte les objets point�s par la souris.
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

		// D�tecte l'objet �ditable point� par la souris.
		protected Objects.Abstract DetectEdit(Point mouse)
		{
			Objects.Abstract layer = this.drawingContext.RootObject();

			foreach ( Objects.Abstract obj in this.document.FlatReverse(layer) )
			{
				if ( !obj.IsSelected )  continue;
				if ( obj.DetectEdit(mouse) )  return obj;
			}

			foreach ( Objects.Abstract obj in this.document.FlatReverse(layer) )
			{
				if ( obj.IsSelected )  continue;
				if ( obj.DetectEdit(mouse) )  return obj;
			}

			return null;
		}

		// D�tecte la poign�e point�e par la souris.
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

		// Survolle une poign�e.
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

		// D�place une poign�e d'un objet.
		protected void MoveHandleProcess(Objects.Abstract obj, int rank, Point pos)
		{
			obj.MoveHandleProcess(rank, pos, this.drawingContext);
		}

		// Mode de s�lection.
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

		// Mode de s�lection partiel (objets �lastiques).
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

		// Met � jour le selector en fonction des objets s�lectionn�s.
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
			rect.Inflate(5.0/this.drawingContext.ScaleX);
			this.selector.Initialize(rect);
		}

		// Indique si tous les objets s�lectionn�s le sont globalement.
		protected void GlobalSelectUpdate(bool global)
		{
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
			{
				obj.GlobalSelect(global);
			}
		}

		// S�lectionne l'objet directement dessous l'objet d�j� s�lectionn�.
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

		// S�lectionne un objet et d�s�lectionne tous les autres.
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

		// S�lectionne tous les objets dans le rectangle.
		// partial = false -> toutes les poign�es doivent �tre dans le rectangle
		// partial = true  -> une seule poign�e doit �tre dans le rectangle
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

		// D�but du d�placement de tous les objets s�lectionn�s.
		protected void MoveAllStarting()
		{
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Deep(layer, true) )
			{
				obj.MoveAllStarting();
			}
		}

		// Effectue le d�placement de tous les objets s�lectionn�s.
		protected void MoveAllProcess(Point move)
		{
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Deep(layer, true) )
			{
				obj.MoveAllProcess(move);
			}

			this.document.Modifier.AddMoveAfterDuplicate(move);
		}

		// D�but du d�placement global de tous les objets s�lectionn�s.
		public void MoveGlobalStarting()
		{
			this.selector.FinalToInitialData();

			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Deep(layer, true) )
			{
				obj.MoveGlobalStarting();
			}
		}

		// Effectue le d�placement global de tous les objets s�lectionn�s.
		public void MoveGlobalProcess(Selector selector)
		{
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Deep(layer, true) )
			{
				obj.MoveGlobalProcess(selector);
			}
		}

		// Termine le d�placement global de tous les objets s�lectionn�s.
		public void MoveGlobalEnding()
		{
			if ( this.selector.TypeInUse != SelectorType.Stretcher )  return;

			this.document.Notifier.NotifyArea(this.selector.Rectangle);
			this.SelectorInitialize(this.document.Modifier.SelectedBbox);
			this.document.Notifier.NotifyArea(this.selector.Rectangle);
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

		// Dessine les poign�es de tous les objets.
		public void DrawHandles(Graphics graphics)
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
			bool exist;

			// Construit le sous-menu "op�rations".
			if ( globalMenu || nbSel == 0 )
			{
				this.contextMenuOper = null;
			}
			else
			{
				System.Collections.ArrayList listOper = new System.Collections.ArrayList();

				exist = false;
				exist |= ContextMenuItem.MenuAddItem(listOper, this.CommandDispatcher, "Rotate90",  "manifest:Epsitec.App.DocumentEditor.Images.OperRot90.icon",    "Quart de tour � gauche");
				exist |= ContextMenuItem.MenuAddItem(listOper, this.CommandDispatcher, "Rotate180", "manifest:Epsitec.App.DocumentEditor.Images.OperRot180.icon",   "Demi-tour");
				exist |= ContextMenuItem.MenuAddItem(listOper, this.CommandDispatcher, "Rotate270", "manifest:Epsitec.App.DocumentEditor.Images.OperRot270.icon",   "Quart de tour � droite");
				if ( exist )  ContextMenuItem.MenuAddSep(listOper);

				exist = false;
				exist |= ContextMenuItem.MenuAddItem(listOper, this.CommandDispatcher, "MirrorH",   "manifest:Epsitec.App.DocumentEditor.Images.OperMirrorH.icon",  "Miroir horizontal");
				exist |= ContextMenuItem.MenuAddItem(listOper, this.CommandDispatcher, "MirrorV",   "manifest:Epsitec.App.DocumentEditor.Images.OperMirrorV.icon",  "Miroir vertical");
				if ( exist )  ContextMenuItem.MenuAddSep(listOper);

				exist = false;
				exist |= ContextMenuItem.MenuAddItem(listOper, this.CommandDispatcher, "ZoomDiv2",  "manifest:Epsitec.App.DocumentEditor.Images.OperZoomDiv2.icon", "R�duction /2");
				exist |= ContextMenuItem.MenuAddItem(listOper, this.CommandDispatcher, "ZoomMul2",  "manifest:Epsitec.App.DocumentEditor.Images.OperZoomMul2.icon", "Agrandissement x2");

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

			// Construit le sous-menu "g�om�trie".
			if ( globalMenu || nbSel == 0 )
			{
				this.contextMenuGeom = null;
			}
			else
			{
				System.Collections.ArrayList listGeom = new System.Collections.ArrayList();

				exist = false;
				exist |= ContextMenuItem.MenuAddItem(listGeom, this.CommandDispatcher, "Combine",   "manifest:Epsitec.App.DocumentEditor.Images.Combine.icon",   "Combiner");
				exist |= ContextMenuItem.MenuAddItem(listGeom, this.CommandDispatcher, "Uncombine", "manifest:Epsitec.App.DocumentEditor.Images.Uncombine.icon", "Scinder");
				exist |= ContextMenuItem.MenuAddItem(listGeom, this.CommandDispatcher, "ToBezier",  "manifest:Epsitec.App.DocumentEditor.Images.ToBezier.icon",  "Convertir en courbes");
				exist |= ContextMenuItem.MenuAddItem(listGeom, this.CommandDispatcher, "ToPoly",    "manifest:Epsitec.App.DocumentEditor.Images.ToPoly.icon",    "Convertir en droites");
				exist |= ContextMenuItem.MenuAddItem(listGeom, this.CommandDispatcher, "Fragment",  "manifest:Epsitec.App.DocumentEditor.Images.Fragment.icon",  "Fragmenter");

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

			// Construit le sous-menu "bool�en".
			if ( globalMenu || nbSel == 0 )
			{
				this.contextMenuBool = null;
			}
			else
			{
				System.Collections.ArrayList listBool = new System.Collections.ArrayList();

				exist = false;
				exist |= ContextMenuItem.MenuAddItem(listBool, this.CommandDispatcher, "BooleanOr",         "manifest:Epsitec.App.DocumentEditor.Images.BooleanOr.icon",         "Union");
				exist |= ContextMenuItem.MenuAddItem(listBool, this.CommandDispatcher, "BooleanAnd",        "manifest:Epsitec.App.DocumentEditor.Images.BooleanAnd.icon",        "Intersection");
				exist |= ContextMenuItem.MenuAddItem(listBool, this.CommandDispatcher, "BooleanXor",        "manifest:Epsitec.App.DocumentEditor.Images.BooleanXor.icon",        "Exclusion");
				exist |= ContextMenuItem.MenuAddItem(listBool, this.CommandDispatcher, "BooleanFrontMinus", "manifest:Epsitec.App.DocumentEditor.Images.BooleanFrontMinus.icon", "Avant moins arri�res");
				exist |= ContextMenuItem.MenuAddItem(listBool, this.CommandDispatcher, "BooleanBackMinus",  "manifest:Epsitec.App.DocumentEditor.Images.BooleanBackMinus.icon",  "Arri�re moins avants");

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
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "Deselect",     "manifest:Epsitec.App.DocumentEditor.Images.Deselect.icon",     "D�s�lectionner tout");
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "SelectAll",    "manifest:Epsitec.App.DocumentEditor.Images.SelectAll.icon",    "Tout s�lectionner");
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "SelectInvert", "manifest:Epsitec.App.DocumentEditor.Images.SelectInvert.icon", "Inverser la s�lection");
				if ( exist )  ContextMenuItem.MenuAddSep(list);

				exist = false;
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "HideSel",      "manifest:Epsitec.App.DocumentEditor.Images.HideSel.icon",      "Cacher la s�lection");
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "HideRest",     "manifest:Epsitec.App.DocumentEditor.Images.HideRest.icon",     "Cacher le reste");
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "HideCancel",   "manifest:Epsitec.App.DocumentEditor.Images.HideCancel.icon",   "Montrer tout");
				if ( exist )  ContextMenuItem.MenuAddSep(list);

				exist = false;
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "ZoomMin",      "manifest:Epsitec.App.DocumentEditor.Images.ZoomMin.icon",      "Zoom minimal");
				if ( this.document.Type != DocumentType.Pictogram )
				{
					exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "ZoomPage",      "manifest:Epsitec.App.DocumentEditor.Images.ZoomPage.icon",      "Zoom pleine page");
					exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "ZoomPageWidth", "manifest:Epsitec.App.DocumentEditor.Images.ZoomPageWidth.icon", "Zoom largeur page");
				}
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "ZoomDefault",  "manifest:Epsitec.App.DocumentEditor.Images.ZoomDefault.icon",  "Zoom 100%");
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "ZoomSel",      "manifest:Epsitec.App.DocumentEditor.Images.ZoomSel.icon",      "Zoom s�lection");
				if ( this.document.Type != DocumentType.Pictogram )
				{
					exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "ZoomSelWidth",  "manifest:Epsitec.App.DocumentEditor.Images.ZoomSelWidth.icon",  "Zoom s�lection");
				}
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "ZoomPrev",     "manifest:Epsitec.App.DocumentEditor.Images.ZoomPrev.icon",     "Zoom pr�c�dent");
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "ZoomSub",      "manifest:Epsitec.App.DocumentEditor.Images.ZoomSub.icon",      "R�duction");
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "ZoomAdd",      "manifest:Epsitec.App.DocumentEditor.Images.ZoomAdd.icon",      "Agrandissement");
				if ( exist )  ContextMenuItem.MenuAddSep(list);

				exist = false;
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "Outside",      "manifest:Epsitec.App.DocumentEditor.Images.Outside.icon",      "Sortir du groupe");
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "Grid",         "manifest:Epsitec.App.DocumentEditor.Images.Grid.icon",         "Grille magn�tique");
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
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "Delete",    "manifest:Epsitec.App.DocumentEditor.Images.Delete.icon",    "Supprimer");
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "Duplicate", "manifest:Epsitec.App.DocumentEditor.Images.Duplicate.icon", "Dupliquer");
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "OrderUp",   "manifest:Epsitec.App.DocumentEditor.Images.OrderUp.icon",   "Dessus");
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "OrderDown", "manifest:Epsitec.App.DocumentEditor.Images.OrderDown.icon", "Dessous");
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "Merge",     "manifest:Epsitec.App.DocumentEditor.Images.Merge.icon",     "Fusionner");
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "Group",     "manifest:Epsitec.App.DocumentEditor.Images.Group.icon",     "Associer");
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "Ungroup",   "manifest:Epsitec.App.DocumentEditor.Images.Ungroup.icon",   "Dissocier");
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "Inside",    "manifest:Epsitec.App.DocumentEditor.Images.Inside.icon",    "Entrer dans groupe");
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "Outside",   "manifest:Epsitec.App.DocumentEditor.Images.Outside.icon",   "Sortir du groupe");
				if ( exist )  ContextMenuItem.MenuAddSep(list);

				exist = false;
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "ZoomSel",   "manifest:Epsitec.App.DocumentEditor.Images.ZoomSel.icon",   "Zoom s�lection");
				if ( this.document.Type != DocumentType.Pictogram )
				{
					exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "ZoomSelWidth", "manifest:Epsitec.App.DocumentEditor.Images.ZoomSelWidth.icon", "Zoom largeur s�lection");
				}
				if ( exist )  ContextMenuItem.MenuAddSep(list);

				exist = false;
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "HideSel",    "manifest:Epsitec.App.DocumentEditor.Images.HideSel.icon",    "Cacher la s�lection");
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "HideRest",   "manifest:Epsitec.App.DocumentEditor.Images.HideRest.icon",   "Cacher le reste");
				exist |= ContextMenuItem.MenuAddItem(list, this.CommandDispatcher, "HideCancel", "manifest:Epsitec.App.DocumentEditor.Images.HideCancel.icon", "Montrer tout");
				if ( exist )  ContextMenuItem.MenuAddSep(list);

				exist = false;
				exist |= ContextMenuItem.MenuAddSubmenu(list, this.contextMenuOper, "manifest:Epsitec.App.DocumentEditor.Images.OperMoveH.icon", "Op�rations");
				exist |= ContextMenuItem.MenuAddSubmenu(list, this.contextMenuGeom, "manifest:Epsitec.App.DocumentEditor.Images.Combine.icon",   "G�om�trie");
				exist |= ContextMenuItem.MenuAddSubmenu(list, this.contextMenuBool, "manifest:Epsitec.App.DocumentEditor.Images.BooleanOr.icon", "Bool�en");

				if ( nbSel == 1 && this.contextMenuObject != null )
				{
					this.contextMenuObject.ContextMenu(list, mouse, this.contextMenuRank);
				}
			}

			this.contextMenu = new VMenu();
			this.contextMenu.Host = this;
			ContextMenuItem.MenuCreate(this.contextMenu, list);
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

		// Ex�cute une commande locale � un objet.
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
		// Appel� lorsque la r�gle est chang�e.
		private void HandleRulerChanged(object sender)
		{
			Objects.Abstract editObject = this.document.Modifier.RetEditObject();
			if ( editObject == null )  return;
			this.document.Notifier.NotifyArea(editObject.BoundingBox);
		}

		// Positionne la r�gle en fonction de l'�ventuel objet en cours d'�dition.
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
					this.textRuler.SetVisible(true);
				}
				else
				{
					this.HideRuler();
				}
			}
		}

		// Cache la r�gle.
		protected void HideRuler()
		{
			if ( this.textRuler.IsVisible )
			{
				Rectangle rect = this.ScreenToInternal(this.textRuler.Bounds);
				this.document.Notifier.NotifyArea(this, rect);
			}

			this.textRuler.SetVisible(false);
			this.textRuler.DetachFromText();
		}
		#endregion


		#region MouseCursor
		// Adapte le sprite de la souris en fonction des touches Shift et Ctrl.
		protected void UpdateMouseCursor(Message message)
		{
			if ( this.mouseCursorType == MouseCursorType.Arrow     ||
				 this.mouseCursorType == MouseCursorType.ArrowDup  ||
				 this.mouseCursorType == MouseCursorType.ArrowPlus )
			{
				if ( message.IsCtrlPressed && !this.mouseDown )
				{
					this.ChangeMouseCursor(MouseCursorType.ArrowDup);
				}
				else if ( message.IsShiftPressed && !this.mouseDown )
				{
					this.ChangeMouseCursor(MouseCursorType.ArrowPlus);
				}
				else
				{
					this.ChangeMouseCursor(MouseCursorType.Arrow);
				}
			}

			if ( this.mouseCursorType == MouseCursorType.Finger     ||
				 this.mouseCursorType == MouseCursorType.FingerDup  ||
				 this.mouseCursorType == MouseCursorType.FingerPlus )
			{
				if ( message.IsCtrlPressed && !this.mouseDown )
				{
					this.ChangeMouseCursor(MouseCursorType.FingerDup);
				}
				else if ( message.IsShiftPressed && !this.mouseDown )
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
				if ( message.IsShiftPressed || (this.mouseDown && this.zoomShift) )
				{
					if ( message.IsCtrlPressed || (this.mouseDown && this.zoomCtrl) )
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

				case MouseCursorType.FingerPlus:
					this.MouseCursorImage(ref this.mouseCursorFingerPlus, "manifest:Epsitec.App.DocumentEditor.Images.FingerPlus.icon");
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

				case MouseCursorType.ZoomMinus:
					this.MouseCursorImage(ref this.mouseCursorZoomMinus, "manifest:Epsitec.App.DocumentEditor.Images.ZoomMinus.icon");
					break;

				case MouseCursorType.ZoomShift:
					this.MouseCursorImage(ref this.mouseCursorZoomShift, "manifest:Epsitec.App.DocumentEditor.Images.ZoomShift.icon");
					break;

				case MouseCursorType.ZoomShiftCtrl:
					this.MouseCursorImage(ref this.mouseCursorZoomShiftCtrl, "manifest:Epsitec.App.DocumentEditor.Images.ZoomShiftCtrl.icon");
					break;

				case MouseCursorType.Picker:
					this.MouseCursorImage(ref this.mouseCursorPicker, "manifest:Epsitec.App.DocumentEditor.Images.Picker.icon");
					break;

				case MouseCursorType.PickerEmpty:
					this.MouseCursorImage(ref this.mouseCursorPickerEmpty, "manifest:Epsitec.App.DocumentEditor.Images.PickerEmpty.icon");
					break;

				case MouseCursorType.Fine:
					this.MouseCursorImage(ref this.mouseCursorFine, "manifest:Epsitec.App.DocumentEditor.Images.FineCursor.icon");
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
		// D�tecte le guide point� par la souris.
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

				if ( guide.IsHorizontal )  // rep�re horizontal ?
				{
					if ( pos.Y >= gpos-margin && pos.Y <= gpos+margin )
					{
						rank = i;
						return true;
					}
				}
				else	// rep�re vertical ?
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

		// Met en �vidence le guide survol� par la souris.
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

		// D�but de l'insertion interactive d'un guide (drag depuis une r�gle).
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

			// Ne pas utiliser SnapGrid pour ignorer les rep�res !
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

		// Termine le d�placement d'un guide interactif.
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

			string title = "Cr�sus";
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";
			string message = error;

			Common.Dialogs.IDialog dialog = Common.Dialogs.Message.CreateOk(title, icon, message, "", this.CommandDispatcher);
			dialog.Owner = this.Window;
			dialog.OpenDialog();
		}
		#endregion


		#region Drawing
		// Dessine la grille magn�tique dessous.
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

		// Dessine la grille magn�tique dessus.
		protected void DrawGridForeground(Graphics graphics, Rectangle clipRect)
		{
			double initialWidth = graphics.LineWidth;
			graphics.LineWidth = 1.0/this.drawingContext.ScaleX;

			double ix = 0.5/this.drawingContext.ScaleX;
			double iy = 0.5/this.drawingContext.ScaleY;

			clipRect = ScreenToInternal(clipRect);
			clipRect = Rectangle.Intersection(clipRect, this.document.Modifier.RectangleArea);

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

				// Dessine les rep�res.
				if ( this.drawingContext.GuidesShow )
				{
					int total = this.document.Settings.GuidesCount;
					for ( int i=0 ; i<total ; i++ )
					{
						Settings.Guide guide = this.document.Settings.GuidesGet(i);

						if ( guide.IsHorizontal )  // rep�re horizontal ?
						{
							double x = clipRect.Left;
							double y = guide.AbsolutePosition;
							graphics.Align(ref x, ref y);
							x += ix;
							y += iy;
							graphics.AddLine(x, y, clipRect.Right, y);
						}
						else	// rep�re vertical ?
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
							graphics.RenderSolid(Color.FromARGB(0.5, 0.0,0.0,0.8));  // bleut�
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
			//?System.Diagnostics.Debug.WriteLine("PaintBackgroundImplementation "+clipRect.ToString());
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

			// Dessine la grille magn�tique dessous.
			this.DrawGridBackground(graphics, clipRect);

			// Dessine les g�om�tries.
			this.document.Paint(graphics, this.drawingContext, clipRect);

			// Dessine la grille magn�tique dessus.
			this.DrawGridForeground(graphics, clipRect);

			// Dessine les noms de objets.
			if ( this.IsActiveViewer && this.drawingContext.LabelsShow && !this.drawingContext.PreviewActive )
			{
				this.DrawLabels(graphics);
			}

			// Dessine les poign�es.
			if ( this.IsActiveViewer )
			{
				this.DrawHandles(graphics);
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

		// G�re l'invalidation en fonction de la raison.
		protected override void Invalidate(InvalidateReason reason)
		{
			if ( reason != InvalidateReason.FocusedChanged )
			{
				base.Invalidate(reason);
			}
		}

		// Appel� lorsque la vue prend le focus.
		protected override void OnFocused()
		{
			Objects.Abstract edit = this.document.Modifier.RetEditObject();
			if ( edit != null )
			{
				this.document.Notifier.NotifyArea(this, edit.EditSelectBox);
			}
		}

		// Appel� lorsque la vue perd le focus.
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
		// Conversion d'un rectangle �cran -> rectangle interne.
		public Rectangle ScreenToInternal(Rectangle rect)
		{
			if ( !rect.IsInfinite )
			{
				rect.BottomLeft = this.ScreenToInternal(rect.BottomLeft);
				rect.TopRight   = this.ScreenToInternal(rect.TopRight);
			}
			return rect;
		}

		// Conversion d'une coordonn�e �cran -> coordonn�e interne.
		public Point ScreenToInternal(Point pos)
		{
			pos.X = pos.X/this.drawingContext.ScaleX - this.drawingContext.OriginX;
			pos.Y = pos.Y/this.drawingContext.ScaleY - this.drawingContext.OriginY;
			return pos;
		}

		// Conversion d'un rectangle interne -> rectangle �cran.
		public Rectangle InternalToScreen(Rectangle rect)
		{
			if ( !rect.IsInfinite )
			{
				rect.BottomLeft = this.InternalToScreen(rect.BottomLeft);
				rect.TopRight   = this.InternalToScreen(rect.TopRight);
			}
			return rect;
		}

		// Conversion d'une coordonn�e interne -> coordonn�e �cran.
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
		protected MessageType					lastMessageType;
		protected Point							mousePosWidget;
		protected Point							mousePos;
		protected bool							mousePosValid = false;
		protected bool							mouseDown;
		protected Point							handMouseStart;
		protected Point							moveStart;
		protected Point							moveOffset;
		protected bool							moveAccept;
		protected bool							moveReclick;
		protected int							moveHandle = -1;
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

		protected VMenu							contextMenu;
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
		protected Image							mouseCursorFine = null;
	}
}
