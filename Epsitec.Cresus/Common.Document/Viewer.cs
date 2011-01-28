using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

using System.Collections.Generic;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// Summary description for Viewer.
	/// </summary>
	public class Viewer : Common.Widgets.Widget
	{
		protected enum MouseCursorType
		{
			Unknown,
			Arrow,
			ArrowPlus,
			ArrowMinus,
			ArrowDup,
			ArrowGlobal,
			ShaperNorm,
			ShaperPlus,
			ShaperMove,
			ShaperMulti,
			Hand,
			IBeam,
			IBeamCreate,
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
			TextFlowCreateBox,
			TextFlowCreateLine,
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

		public enum MiniBarDelayed
		{
			Immediately,
			Delayed,
			DoubleClick,
		}


		public Viewer(Document document)
		{
			this.AutoFocus       = true;
			this.AutoDoubleClick = true;
			
			this.InternalState |= WidgetInternalState.Focusable;
			
			this.BackColor = Color.FromBrightness(1);  // fond blanc

			this.document = document;
			this.drawingContext = new DrawingContext(this.document, this);
			this.drawingContext.SetImageNameFilter(0, "Bilinear");
			this.drawingContext.SetImageNameFilter(1, "Bilinear");
			this.selector = new Selector(this.document);
			this.zoomer = new Selector(this.document);
			this.RedrawAreaFlush();

			this.hotSpotHandle = new Objects.Handle(this.document);
			this.hotSpotHandle.Type = Objects.HandleType.Center;

			this.autoScrollTimer = new Timer();
			this.autoScrollTimer.AutoRepeat = 0.1;
			this.autoScrollTimer.TimeElapsed += this.HandleAutoScrollTimeElapsed;

			this.miniBarTimer = new Timer();
			this.miniBarTimer.TimeElapsed += this.HandleMiniBarTimeElapsed;
			
			this.IsVisibleChanged += this.HandleIsVisibleChanged;
			this.IsFocusedChanged += this.HandleIsFocusedChanged;
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.autoScrollTimer.TimeElapsed -= this.HandleAutoScrollTimeElapsed;
				this.autoScrollTimer.Dispose();
				this.autoScrollTimer = null;

				this.miniBarTimer.TimeElapsed -= this.HandleMiniBarTimeElapsed;
				this.miniBarTimer.Dispose();
				this.miniBarTimer = null;

				this.IsFocusedChanged -= this.HandleIsFocusedChanged;
				this.IsVisibleChanged -= this.HandleIsVisibleChanged;
			}
			
			base.Dispose(disposing);
		}


		private void HandleIsVisibleChanged(object sender, Types.DependencyPropertyChangedEventArgs e)
		{
			this.ClosePopupInterface();
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

		public bool IsMiniature
		{
			//	Miniature d'une page ou d'un calque.
			get
			{
				return this.isMiniature;
			}
			set
			{
				this.isMiniature = value;
			}
		}

		public bool IsLayerMiniature
		{
			//	Miniature d'un calque (si false, c'est une miniature d'une page).
			get
			{
				return this.isLayerMiniature;
			}
			set
			{
				this.isLayerMiniature = value;
			}
		}

		public double PictogramMiniatureZoom
		{
			//	Zoom spécial pour CrPicto permettant de voir les pixels lorsque le zoom est plus grand que 1.
			get
			{
				return this.pictogramMiniatureZoom;
			}
			set
			{
				this.pictogramMiniatureZoom = value;
			}
		}


		public double MarkerVertical
		{
			//	Position horizontale du marqueur vertical.
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

		public double MarkerHorizontal
		{
			//	Position verticale du marqueur horizontal.
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


		public Rectangle RectangleDisplayed
		{
			//	Retourne le rectangle correspondant à la zone visible dans le Viewer.
			get
			{
				Rectangle rect = this.Client.Bounds;
				return this.ScreenToInternal(rect);
			}
		}

		
		#region AutoScroll
		protected void AutoScrollTimerStart(Message message)
		{
			//	Démarre le timer pour l'auto-scroll.
			if ( this.document.Modifier.Tool == "ToolHand" )  return;
			this.autoScrollTimer.Start();
		}

		protected void AutoScrollTimerStop()
		{
			//	Stoppe le timer pour l'auto-scroll.
			this.autoScrollTimer.Suspend();
		}

		protected void HandleAutoScrollTimeElapsed(object sender)
		{
			//	Appelé lorsque le timer arrive à échéance.
			//	Effectue éventuellement un scroll si la souris est proche des bords.
			if ( this.mouseDragging && this.zoomShift )  return;
			if ( this.mouseDragging && this.guideInteractive != -1 )  return;

			Point mouse = this.mousePosWidget;

			Rectangle view = this.Client.Bounds;
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

		public Rectangle ScrollRectangle
		{
			//	Retourne le rectangle correspondant à la zone visible dans le Viewer
			//	dans laquelle il faut scroller.
			get
			{
				Rectangle rect = this.Client.Bounds;
				rect.Deflate(5);  // ch'tite marge
				return this.ScreenToInternal(rect);
			}
		}

		public void AutoScroll(Point move)
		{
			//	Effectue un scroll automatique.
			if ( move.X == 0.0 && move.Y == 0.0 )  return;

			Point origin = new Point(this.drawingContext.OriginX, this.drawingContext.OriginY);
			origin -= move;
			origin.X = -System.Math.Max(-origin.X, this.drawingContext.MinOriginX);
			origin.X = -System.Math.Min(-origin.X, this.drawingContext.MaxOriginX);
			origin.Y = -System.Math.Max(-origin.Y, this.drawingContext.MinOriginY);
			origin.Y = -System.Math.Min(-origin.Y, this.drawingContext.MaxOriginY);
			this.drawingContext.SetOrigin(origin);
		}
		#endregion


		protected override void ProcessMessage(Message message, Point pos)
		{
			//	Gestion d'un événement.
			//?System.Diagnostics.Debug.WriteLine(string.Format("Message: {0}", message.Type));
			if ( !this.IsActiveViewer )  return;

			Modifier modifier = this.document.Modifier;
			if ( modifier == null )  return;

			//	Après un MouseUp, on reçoit toujours un MouseMove inutile,
			//	qui est filtré ici !!!
			if ( message.MessageType == MessageType.MouseMove &&
				 this.lastMessageType == MessageType.MouseUp &&
				 pos == this.mousePosWidget )
			{
				//?System.Diagnostics.Debug.WriteLine("ProcessMessage: MouseMove après MouseUp poubellisé !");
				return;
			}
			this.lastMessageType = message.MessageType;

			this.mousePosWidget = pos;
			pos = this.ScreenToInternal(pos);  // position en coordonnées internes

			if ( pos.X != this.mousePos.X || pos.Y != this.mousePos.Y )
			{
				this.mousePos = pos;
				this.mousePosValid = true;
				this.document.Notifier.NotifyMouseChanged();
			}

			this.drawingContext.IsShift = message.IsShiftPressed;
			this.drawingContext.IsCtrl  = message.IsControlPressed;
			this.drawingContext.IsAlt   = message.IsAltPressed;

			switch ( message.MessageType )
			{
				case MessageType.MouseDown:
					if ( !this.mouseDragging )
					{
						//?this.document.SetDirtySerialize(DirtyMode.Local);
						this.AutoScrollTimerStart(message);
						this.RestartMiniBar();
						this.ProcessMouseDown(message, pos);
						this.mouseDragging = true;
					}
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

						if (!message.IsMiddleButton)
						{
							this.ProcessMouseMove(message, pos);
							this.document.SetDirtySerialize(CacheBitmapChanging.Local);
						}
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
						if (modifier.Tool != "ToolEdit")
						{
							if (this.moveObject != null || this.moveGlobal != -1 || this.createRank != -1)
							{
								Objects.Abstract obj = null;
								int rank = -1;

								if (this.moveObject != null)
								{
									obj = this.moveObject;
									rank = this.moveHandle;
								}
								else if (this.createRank != -1)
								{
									Objects.Abstract layer = this.drawingContext.RootObject();
									obj = layer.Objects[this.createRank] as Objects.Abstract;
									rank = obj.CreationLastHandle;
								}

								if (this.drawingContext.ConstrainSpacePressed(pos, obj, rank))
								{
									this.drawingContext.MagnetDelStarting();
								}
							}
							else
							{
								if (this.miniBar == null)
								{
									this.OpenMiniBar(pos, MiniBarDelayed.Immediately, true, false, 0);
								}
								else
								{
									this.CloseMiniBar(false);
								}
							}
						}
						break;
					}

					if ( message.KeyCode == KeyCode.Tab )
					{
						this.NextHotSpot();
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

		protected void ProcessMouseDown(Message message, Point pos)
		{
			//	Gestion d'un bouton de la souris pressé.
			Modifier modifier = this.document.Modifier;

			if (message.IsMiddleButton)
			{
				this.HandMouseDown(pos);
			}
			else
			{
				if (modifier.Tool == "ToolSelect")
				{
					this.SelectMouseDown(pos, message.ButtonDownCount, message.IsRightButton, false);
				}
				else if (modifier.Tool == "ToolGlobal")
				{
					this.SelectMouseDown(pos, message.ButtonDownCount, message.IsRightButton, true);
				}
				else if (modifier.Tool == "ToolShaper")
				{
					this.ShaperMouseDown(pos, message.ButtonDownCount, message.IsRightButton);
				}
				else if (modifier.Tool == "ToolEdit")
				{
					this.EditMouseDown(message, pos, message.ButtonDownCount);
				}
				else if (modifier.Tool == "ToolZoom")
				{
					this.ZoomMouseDown(pos, message.IsRightButton);
				}
				else if (modifier.Tool == "ToolHand")
				{
					this.HandMouseDown(pos);
				}
				else if (modifier.Tool == "ToolPicker")
				{
					this.PickerMouseDown(pos);
				}
				else if (modifier.Tool == "ToolHotSpot")
				{
					this.HotSpotMouseDown(pos);
				}
				else
				{
					this.CreateMouseDown(pos);
				}
			}
		}

		protected void ProcessMouseMove(Message message, Point pos)
		{
			//	Gestion d'un déplacement de la souris pressé.
			Modifier modifier = this.document.Modifier;

			if ( this.guideInteractive != -1 )
			{
				this.GuideInteractiveMove(pos, message.IsAltPressed);
				return;
			}

			if (message.IsMiddleButton)
			{
				this.HandMouseMove(pos);
			}
			else
			{
				if (modifier.Tool == "ToolSelect")
				{
					this.SelectMouseMove(pos, message.IsRightButton, false);
				}
				else if (modifier.Tool == "ToolGlobal")
				{
					this.SelectMouseMove(pos, message.IsRightButton, true);
				}
				else if (modifier.Tool == "ToolShaper")
				{
					this.ShaperMouseMove(pos, message.IsRightButton);
				}
				else if (modifier.Tool == "ToolEdit")
				{
					this.EditMouseMove(message, pos);
				}
				else if (modifier.Tool == "ToolZoom")
				{
					this.ZoomMouseMove(pos, message.IsRightButton);
				}
				else if (modifier.Tool == "ToolHand")
				{
					this.HandMouseMove(pos);
				}
				else if (modifier.Tool == "ToolPicker")
				{
					this.PickerMouseMove(pos);
				}
				else if (modifier.Tool == "ToolHotSpot")
				{
					this.HotSpotMouseMove(pos);
				}
				else
				{
					this.CreateMouseMove(pos);
				}
			}

			this.UpdateMouseCursor(message);
		}

		protected void ProcessMouseUp(Message message, Point pos)
		{
			//	Gestion d'un bouton de la souris relâché.
			Modifier modifier = this.document.Modifier;

			if ( this.guideInteractive != -1 )
			{
				this.GuideInteractiveEnd();
				return;
			}

			if (message.IsMiddleButton)
			{
				this.HandMouseUp(pos);

				if (modifier.Tool == "ToolSelect")
				{
					this.ChangeMouseCursor(MouseCursorType.Arrow);
				}
				else if (modifier.Tool == "ToolGlobal")
				{
					this.ChangeMouseCursor(MouseCursorType.ArrowGlobal);
				}
				else if (modifier.Tool == "ToolShaper")
				{
					this.ChangeMouseCursor(MouseCursorType.ShaperNorm);
				}
				else if (modifier.Tool == "ToolEdit")
				{
					this.ChangeMouseCursor(MouseCursorType.IBeam);
				}
				else if (modifier.Tool == "ToolZoom")
				{
					this.ChangeMouseCursor(MouseCursorType.Zoom);
				}
				else if (modifier.Tool == "ToolHand")
				{
					this.ChangeMouseCursor(MouseCursorType.Hand);
				}
				else if (modifier.Tool == "ToolPicker")
				{
					this.ChangeMouseCursor(MouseCursorType.PickerEmpty);
				}
				else if (modifier.Tool == "ToolHotSpot")
				{
					this.ChangeMouseCursor(MouseCursorType.Arrow);
				}
				else
				{
					this.ChangeMouseCursor(MouseCursorType.Pen);
				}
			}
			else
			{
				if (modifier.Tool == "ToolSelect")
				{
					this.SelectMouseUp(pos, message.IsRightButton, false);
				}
				else if (modifier.Tool == "ToolGlobal")
				{
					this.SelectMouseUp(pos, message.IsRightButton, true);
				}
				else if (modifier.Tool == "ToolShaper")
				{
					this.ShaperMouseUp(pos, message.IsRightButton);
				}
				else if (modifier.Tool == "ToolEdit")
				{
					this.EditMouseUp(message, pos, message.IsRightButton);
				}
				else if (modifier.Tool == "ToolZoom")
				{
					this.ZoomMouseUp(pos, message.IsRightButton);
				}
				else if (modifier.Tool == "ToolHand")
				{
					this.HandMouseUp(pos);
				}
				else if (modifier.Tool == "ToolPicker")
				{
					this.PickerMouseUp(pos);
				}
				else if (modifier.Tool == "ToolHotSpot")
				{
					this.HotSpotMouseUp(pos);
				}
				else
				{
					this.CreateMouseUp(pos);
				}
			}
		}

		protected void ProcessMouseWheel(int wheel, Point pos)
		{
			//	Action lorsque la molette est actionnée.
			if ( this.document.GlobalSettings.MouseWheelAction == Settings.MouseWheelAction.Zoom )
			{
				double zoom = this.document.GlobalSettings.DefaultZoom;
				double factor = (wheel > 0) ? zoom : 1.0/zoom;
				this.document.Modifier.ZoomChange(factor, pos, true);
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
				
				this.drawingContext.SetOrigin(origin);
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
				this.CreateRank = layer.Objects.Add(obj);  // ajoute à la fin de la liste
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

			if ( this.createRank == -1 )
			{
				this.EditCreateRect(mouse);
			}
			else
			{
				this.ClearEditCreateRect();

				Objects.Abstract layer = this.drawingContext.RootObject();
				Objects.Abstract obj = layer.Objects[this.createRank] as Objects.Abstract;

				obj.CreateMouseMove(mouse, this.drawingContext);
			}
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
					this.CreateRank = layer.Objects.Add(obj);  // ajoute à la fin de la liste
					this.document.Modifier.GroupUpdateParents();
					this.document.Modifier.OpletQueueValidateAction();

					selectAfterCreation = obj.SelectAfterCreation();
					editAfterCreation = obj.EditAfterCreation();

					if ( this.editFlowAfterCreate != null && obj is Objects.AbstractText )
					{
						this.document.Modifier.TextFlowChange(obj as Objects.AbstractText, this.editFlowAfterCreate, true);
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

					this.document.Modifier.IsObjectJustCreated = false;
				}

				this.CreateRank = -1;
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

		public Objects.Abstract CreateEnding(bool delete, bool close)
		{
			//	Si nécessaire, termine la création en cours.
			if (this.createRank == -1)
			{
				return null;
			}

			Objects.Abstract layer = this.drawingContext.RootObject();
			Objects.Abstract obj = layer.Objects[this.createRank] as Objects.Abstract;

			if (close)
			{
				obj.ChangePropertyPolyClose(close);
			}

			this.document.Notifier.NotifyArea(obj.BoundingBox);
			if (obj.CreateEnding(this.drawingContext) && !delete)  // créeation ok ?
			{
				layer.Objects.RemoveAt(this.createRank);

				this.document.Modifier.OpletQueueEnable = true;
				this.CreateRank = layer.Objects.Add(obj);  // ajoute à la fin de la liste
				this.document.Modifier.GroupUpdateParents();
				this.document.Modifier.OpletQueueValidateAction();
			}
			else  // création impossible ?
			{
				layer.Objects.RemoveAt(this.createRank);

				this.document.Modifier.OpletQueueEnable = true;
				obj.Dispose();
				this.document.Modifier.OpletQueueCancelAction();  // annule les propriétés

				this.document.Modifier.IsObjectJustCreated = false;
				obj = null;
			}

			this.CreateRank = -1;
			this.document.Notifier.NotifyCreateChanged();
			this.document.Notifier.NotifySelectionChanged();
			
			return obj;
		}

		public bool IsCreating
		{
			//	Indique s'il existe un objet en cours de création.
			get
			{
				return ( this.createRank != -1 );
			}
		}

		public int CreateRank
		{
			//	Rang de l'objet en cours de création.
			get
			{
				return this.createRank;
			}

			set
			{
				if ( this.createRank != value )
				{
					this.createRank = value;
					this.document.Notifier.NotifySaveChanged();
				}
			}
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
			this.drawingContext.ConstrainClear();
			this.drawingContext.ConstrainAddHV(mouse, false, -1);
			this.ClearHilite();
			this.selector.HiliteHandle(-1);
			this.HiliteHandle(null, -1);
			this.moveObject = null;
			this.moveGlobal = -1;
			this.guideInteractive = -1;
			this.guideCreate = false;
			this.ctrlDown = this.drawingContext.IsCtrl;
			this.ctrlDuplicate = false;
			this.moveReclick = false;
			this.moveAutoSel = false;

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

				if (!this.drawingContext.IsShift && !this.drawingContext.IsCtrl && this.DetectHandle(mouse, out obj, out rank))
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
								this.moveAutoSel = true;

								if (!this.drawingContext.IsShift)
								{
									this.ChangeMouseCursor(MouseCursorType.Finger);
								}

								if ( this.selector.Visible && this.drawingContext.IsShift )
								{
									obj.GlobalSelect(true, false);
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
						this.drawingContext.ConstrainClear();
						this.drawingContext.ConstrainAddHV(this.moveLast, false, -1);
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
				//	Duplique le ou les objets sélectionnés ?
				if ( this.ctrlDown && !this.ctrlDuplicate &&
					 (this.moveGlobal != -1 || (this.moveObject != null && this.moveHandle == -1)) )
				{
					double len = Point.Distance(mouse, this.moveStart);
					if ( len > this.drawingContext.MinimalSize )
					{
						this.document.Modifier.OpletQueueNameAction(Res.Strings.Action.DuplicateAndMove);

						//	Remet la sélection à la position de départ:
						if ( this.moveGlobal != -1 )  // déplace le modificateur global ?
						{
							this.selector.MoveProcess(this.moveGlobal, this.moveStart, this.drawingContext);
							this.MoveGlobalProcess(this.selector);
							this.document.Modifier.GroupUpdateChildren();
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
								if (!this.moveAutoSel || this.IsFullyDisplayed(this.moveObject))
								{
									this.moveAccept = true;
								}
								else
								{
									this.document.Modifier.UpdateCounters();
									this.moveObject.Deselect();
									this.document.Modifier.TotalSelected--;

									this.selector.FixStarting(this.moveStart);
								}
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
				else if (!this.drawingContext.IsShift && !this.drawingContext.IsCtrl && this.DetectHandle(mouse, out obj, out rank))
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
					if (this.drawingContext.IsShift)
					{
						obj = this.Detect(mouse, false);
					}

					if (!global)
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
			bool hot = true;
			if ( this.moveAccept )  mb = false;
			if ( this.moveHandle != -1 && !Point.Equals(mouse, this.moveStart) )  mb = false;
			if ( this.moveGlobal != -1 )  mb = false;

			if ( this.selector.Visible && !this.selector.Handles )
			{
				hot = false;
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
				this.document.Modifier.GroupUpdateChildren();
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
					this.document.Modifier.GroupUpdateChildren();
					this.document.Modifier.GroupUpdateParents();

					this.moveHandle = -1;
					this.UpdateSelector();
				}

				this.hotSpotHandle.IsVisible = false;
			}

			this.moveObject = null;
			this.moveGlobal = -1;
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
					this.OpenMiniBar(mouse, MiniBarDelayed.Delayed, false, hot, 0);
				}
			}
		}

		protected bool NextHotSpot()
		{
			//	Change le point chaud.
			if (this.moveObject == null)
			{
				return false;
			}

			if (this.moveHandle == -1 && !this.document.Modifier.IsToolShaper)
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
				this.drawingContext.ConstrainClear();
				this.drawingContext.ConstrainAddHV(this.moveLast, false, -1);
			}
			return true;
		}

		protected void UpdateHotSpot()
		{
			//	Met à jour la poignée spéciale "hot spot".
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
			this.drawingContext.ConstrainClear();
			this.drawingContext.ConstrainAddHV(mouse, false, -1);
			this.ShaperHilite(null, Point.Zero);
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
			this.moveAutoSel = false;
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
				double len = Point.Distance(mouse, this.moveStart);
				if (len > this.drawingContext.MinimalSize)
				{
					if (this.selector.Visible && !this.selector.Handles)
					{
						this.selector.FixEnding(mouse);
					}
					else if (this.moveObject != null && !this.drawingContext.IsShift)
					{
						if (this.moveHandle != -1)  // déplace une poignée ?
						{
							mouse -= this.moveOffset;
							this.moveObject.MoveHandleProcess(this.moveHandle, mouse, this.drawingContext);
							this.HiliteHandle(this.moveObject, this.moveHandle);
							this.document.Modifier.FlushMoveAfterDuplicate();
						}

						if (this.moveSelectedSegment != -1)  // déplace un segment sélectionné ?
						{
							mouse -= this.moveOffset;
							this.moveObject.MoveSelectedSegmentProcess(this.moveSelectedSegment, mouse, this.drawingContext);
						}

						if (this.moveSelectedHandle)  // déplace plusieurs poignées ?
						{
							this.moveObject.MoveSelectedHandlesProcess(mouse, this.drawingContext);
						}
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
					this.ShaperHilite(null, Point.Zero);
					this.HiliteHandle(obj, rank);
					this.ChangeMouseCursor(MouseCursorType.ShaperMove);
				}
				else
				{
					obj = hiliteObj;
					if ( obj == null )
					{
						this.ShaperHilite(null, Point.Zero);
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
			bool hot = true;
			if ( !Point.Equals(mouse, this.moveStart) )  mb = false;

			if ( this.selector.Visible && !this.selector.Handles )
			{
				mb = true;
				hot = false;
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

				this.document.Modifier.GroupUpdateChildren();
				this.document.Modifier.GroupUpdateParents();

				this.moveHandle = -1;
				this.moveSelectedSegment = -1;
				this.UpdateSelector();
			}

			this.moveObject = null;
			this.moveHandle = -1;
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
					this.OpenMiniBar(mouse, MiniBarDelayed.Delayed, false, hot, 0);
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
			this.editPosPress = Point.Zero;
			Objects.DetectEditType handle;

			if ( this.editFlowSelect != Objects.DetectEditType.Out )
			{
				Objects.AbstractText edit = this.DetectEdit(mouse, out handle) as Objects.AbstractText;
				if ( edit == null )
				{
					edit = this.editFlowSrc as Objects.AbstractText;

					if ( edit is Objects.TextBox2  )  this.document.Modifier.Tool = "ObjectTextBox2";
					if ( edit is Objects.TextLine2 )  this.document.Modifier.Tool = "ObjectTextLine2";

					this.editFlowAfterCreate = edit;
					this.CreateMouseDown(mouse);
				}
				return;
			}

			this.EditFlowReset();
			Objects.Abstract obj = this.DetectEdit(mouse, out handle);

			if ( obj != this.document.Modifier.RetEditObject() )
			{
				this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.Edit);
				this.Select(obj, true, false);
				this.EditProcessMessage(message, mouse);
				this.document.Modifier.OpletQueueValidateAction();
			}
			else
			{
				if ( obj == null )
				{
					this.editPosPress = mouse;
				}

				if ( handle == Objects.DetectEditType.HandleFlowPrev ||
					 handle == Objects.DetectEditType.HandleFlowNext )
				{
					this.editFlowPress = handle;
					this.editFlowSrc = obj;
					return;
				}
				
				this.EditProcessMessage(message, mouse);
			}
		}

		protected void EditMouseMove(Message message, Point mouse)
		{
			this.ChangeMouseCursor(MouseCursorType.IBeam);

			if ( this.editFlowPress != Objects.DetectEditType.Out )
			{
				this.ChangeMouseCursor(MouseCursorType.Arrow);
				return;
			}

			Objects.Abstract editObj = null;
			Objects.DetectEditType editHandle;

			if ( this.editFlowSelect != Objects.DetectEditType.Out )
			{
				Objects.DetectEditType handle;
				Objects.Abstract obj = this.DetectEdit(mouse, out handle);
				this.Hilite(obj);

				if ( obj == null )
				{
					if ( this.editFlowSrc is Objects.TextLine2 )
					{
						this.ChangeMouseCursor(MouseCursorType.TextFlowCreateLine);
					}
					else
					{
						this.ChangeMouseCursor(MouseCursorType.TextFlowCreateBox);
					}
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
			}
			else
			{
				if ( this.mouseDragging )  // bouton souris pressé ?
				{
					this.EditProcessMessage(message, mouse);
				}
				else	// bouton souris relâché ?
				{
					editObj = this.DetectEdit(mouse, out editHandle);
					this.Hilite(editObj);

					if ( editHandle == Objects.DetectEditType.HandleFlowPrev ||
						editHandle == Objects.DetectEditType.HandleFlowNext )
					{
						this.ChangeMouseCursor(MouseCursorType.TextFlow);
					}
					else
					{
						this.ChangeMouseCursor(MouseCursorType.IBeam);
					}
				}
			}

			// Cherche le rectangle créable pour l'édition.
			if ( !this.mouseDragging && editObj == null && !this.document.Wrappers.IsWrappersAttached )
			{
				this.EditCreateRect(mouse);
			}
			else
			{
				this.ClearEditCreateRect();
			}

			if ( !this.editCreateRect.IsEmpty )
			{
				this.ChangeMouseCursor(MouseCursorType.IBeamCreate);
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
				Objects.AbstractText obj = this.DetectEdit(mouse, out handle) as Objects.AbstractText;
				if ( obj != null )
				{
					bool after = (this.editFlowSelect == Objects.DetectEditType.HandleFlowNext);
					Objects.AbstractText edit = this.editFlowSrc as Objects.AbstractText;
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

			//	S'il n'y avait initialement aucun objet en édition et qu'on a cliqué sans bouger,
			//	essayer de créer et d'éditer un nouveau TextBox2 ici.
			if ( !this.editPosPress.IsZero && Point.Distance(mouse, this.editPosPress) <= this.drawingContext.MinimalSize )
			{
				Drawing.Rectangle box = this.GuidesSearchBox(this.editPosPress);
				if ( box.IsEmpty )  return;
				if ( !this.IsFreeForNewTextBox2(box, null) )  return;

				string name = string.Format(Res.Strings.Action.ObjectCreate, this.document.Modifier.ToolName("ObjectTextBox2"));
				this.document.Modifier.OpletQueueBeginAction(name, "CreateAuto");

				Objects.TextBox2 t2 = Objects.Abstract.CreateObject(this.document, "ObjectTextBox2", this.document.Modifier.ObjectMemoryText) as Objects.TextBox2;
				Objects.Abstract layer = this.drawingContext.RootObject();
				layer.Objects.Add(t2);
				t2.CreateMouseDown(this.editPosPress, this.drawingContext);
				t2.CreateMouseUp  (this.editPosPress, this.drawingContext);
				if ( t2.CreateIsExist(this.drawingContext) )
				{
					this.Select(t2, true, false);
					this.document.Modifier.GroupUpdateParents();
				}
				else
				{
					t2.Dispose();
					layer.Objects.Remove(t2);
				}

				this.document.Modifier.OpletQueueValidateAction();
			}

			this.EditProcessMessage(message, mouse);
		}

		protected bool EditProcessMessage(Message message, Point pos)
		{
			if (this.document.Modifier.IsToolObject)
			{
				if ( this.createRank != -1 )
				{
					Objects.Abstract layer = this.drawingContext.RootObject();
					Objects.Abstract obj = layer.Objects[this.createRank] as Objects.Abstract;

					obj.CreateProcessMessage(message, pos);
				}
			}

			Objects.AbstractText editObject = this.document.Modifier.RetEditObject();
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
						this.drawingContext.ZoomAndCenter(zoom, this.moveStart, true);
					}
					else
					{
						this.drawingContext.ZoomAndCenter(zoom, this.moveStart-this.zoomOffset, true);
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
					this.document.Modifier.ZoomChange(0.5, rect.Center, true);
				}
				else
				{
					double len = Point.Distance(mouse, this.moveStart);
					if ( len <= this.drawingContext.MinimalSize )
					{
						this.document.Modifier.ZoomChange(2.0, rect.Center, true);
					}
					else
					{
						this.document.Modifier.ZoomChange(rect.BottomLeft, rect.TopRight, false);
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
				
				this.drawingContext.SetOrigin(origin);
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


		protected CommandDispatcher GetCommandDispatcher()
		{
			//	Trouve le CommandDispatcher associé au document
			return this.document.CommandDispatcher;
		}

		protected bool IsFullyDisplayed(Objects.Abstract obj)
		{
			//	Indique si l'objet est entièrement visible.
			Rectangle displayed = this.RectangleDisplayed;
			return displayed.Contains(obj.BoundingBox);
		}
		
		protected Objects.Abstract Detect(Point mouse, bool selectFirst)
		{
			//	Détecte l'objet pointé par la souris.
			IList<Objects.Abstract> list = this.DetectMultiple(mouse, selectFirst);
			if ( list.Count == 0 )  return null;
			return list[0];
		}

		protected IList<Objects.Abstract> DetectMultiple(Point mouse, bool selectFirst)
		{
			//	Détecte les objets pointés par la souris.
			//	Avec le mode selectFirst, on détecte en priorité les objets sélectionnés.
			List<Objects.Abstract> list = new List<Objects.Abstract> ();
			Objects.Abstract layer = this.drawingContext.RootObject();


			if (selectFirst)
			{
				foreach (Objects.Abstract obj in this.document.FlatReverse(layer))
				{
					if (!obj.IsSelected)
					{
						continue;
					}

					if (obj.Detect(mouse))
					{
						list.Add(obj);
					}
				}

				foreach (Objects.Abstract obj in this.document.FlatReverse(layer))
				{
					if (obj.IsSelected)
					{
						continue;
					}

					if (obj.Detect(mouse))
					{
						list.Add(obj);
					}
				}
			}
			else
			{
				foreach (Objects.Abstract obj in this.document.FlatReverse(layer))
				{
					if (obj.Detect(mouse))
					{
						list.Add(obj);
					}
				}
			}

			return list;
		}

		protected Objects.Abstract DetectEdit(Point mouse, out Objects.DetectEditType handle)
		{
			//	Détecte l'objet éditable pointé par la souris.
			Objects.Abstract layer = this.drawingContext.RootObject();

			foreach ( Objects.Abstract obj in this.document.FlatReverse(layer) )
			{
				if ( !(obj is Objects.AbstractText) )  continue;
				if ( !obj.IsSelected )  continue;

				handle = obj.DetectEdit(mouse);
				if ( handle != Objects.DetectEditType.Out )  return obj;
			}

			foreach ( Objects.Abstract obj in this.document.FlatReverse(layer) )
			{
				if ( !(obj is Objects.AbstractText) )  continue;
				if ( obj.IsSelected )  continue;

				handle = obj.DetectEdit(mouse);
				if ( handle != Objects.DetectEditType.Out )  return obj;
			}

			handle = Objects.DetectEditType.Out;
			return null;
		}

		protected bool DetectHandle(Point mouse, out Objects.Abstract detect, out int rank)
		{
			//	Détecte la poignée pointée par la souris.
			Objects.Abstract layer = this.drawingContext.RootObject();
			double min = 1000000.0;
			Objects.Abstract best = null;
			int found = -1;
			foreach (Objects.Abstract obj in this.document.FlatReverse(layer, true))
			{
				rank = obj.DetectHandle(mouse);
				if (rank != -1)
				{
					double distance = Point.Distance(obj.Handle(rank).Position, mouse);
					if (distance < min)
					{
						min = distance;
						best = obj;
						found = rank;
					}
				}
			}

			detect = best;
			rank = found;
			return (detect != null);
		}

		protected bool DetectSelectedSegmentHandle(Point mouse, out Objects.Abstract detect, out int rank)
		{
			//	Détecte la poignée d'un segment sélectionné pointée par la souris.
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

		public void ClearHilite()
		{
			//	Annule le hilite des objets.
			this.Hilite(null);
			this.ShaperHilite(null, Point.Zero);
		}

		protected void Hilite(Objects.Abstract item)
		{
			//	Hilite un objet.
			this.hiliteObject = item;
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer) )
			{
				if ( obj.IsHilite != (obj == item) )
				{
					obj.IsHilite = (obj == item);
				}
			}
		}

		protected void ShaperHilite(Objects.Abstract item, Point mouse)
		{
			//	Hilite un objet pour le modeleur.
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

		public void SelectToShaper()
		{
			//	Adapte les objets sélectionnés à l'outil modeleur.
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
			{
				obj.ShaperHiliteHandles(true);
				obj.SelectHandle(-1, false);
			}

			this.selector.Visible = false;
			this.selector.Handles = false;
		}

		protected void HiliteHandle(Objects.Abstract obj, int rank)
		{
			//	Survole une poignée.
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

		public SelectorType SelectorType
		{
			//	Mode de sélection.
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

		public SelectorTypeStretch SelectorTypeStretch
		{
			//	Mode de sélection.
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

		public bool PartialSelect
		{
			//	Mode de sélection partiel (objets élastiques).
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

		public bool SelectorAdaptLine
		{
			//	Adapte les traits lors d'un zoom ou d'une rotation.
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

		public bool SelectorAdaptText
		{
			//	Adapte les textes (Font et Justif) lors d'un zoom ou d'une rotation.
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

		public void UpdateSelector()
		{
			//	Met à jour le selector en fonction des objets sélectionnés.
			this.UpdateSelector(this.document.Modifier.SelectedBbox);
		}

		public void UpdateSelector(Rectangle rect)
		{
			if ( this.document.Modifier.IsToolShaper )  return;

			if ( this.document.Modifier.TotalSelected == 0 ||
				 this.document.Modifier.IsToolEdit )
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

		protected void SelectorInitialize(Drawing.Rectangle rect, double angle)
		{
			//	Initialise le rectangle du selector.
			rect.Inflate(5.0/this.drawingContext.ScaleX);
			this.selector.Initialize(rect, angle);
		}

		protected double GetSelectedAngle()
		{
			//	Donne l'angle de ou des objets sélectionnés.
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
			{
				return obj.Direction;
			}
			return 0.0;
		}

		protected void GlobalSelectedUpdate(bool global)
		{
			//	Indique si tous les objets sélectionnés le sont globalement.
			Objects.Abstract layer = this.drawingContext.RootObject();

			bool many = false;
			int totalSelected = this.document.Modifier.TotalSelected;
			if ( global && totalSelected > 20 )  // plus de 20 objets sélectionnés globalement ?
			{
				int totalHandle = 0;
				foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
				{
					totalHandle += obj.TotalMainHandle;
				}
				if ( totalHandle > totalSelected*5 || totalHandle > 200 )
				{
					many = true;
				}
			}

			foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
			{
				obj.GlobalSelect(global, many);
			}
		}

		protected void SelectOther(Point mouse, Objects.Abstract actual)
		{
			//	Sélectionne l'objet directement dessous l'objet déjà sélectionné.
			IList<Objects.Abstract> list = this.DetectMultiple(mouse, false);
			if ( list.Count == 0 )  return;

			int i = list.IndexOf(actual);
			if ( i == -1 )  return;

			i ++;  // l'objet directement dessous
			if ( i >= list.Count )  i = 0;
			Objects.Abstract obj = list[i] as Objects.Abstract;

			this.Select(obj, false, false);
		}

		public void Select(Objects.Abstract item, bool edit, bool add)
		{
			//	Sélectionne un objet et désélectionne tous les autres.
			this.document.Modifier.UpdateCounters();
			Objects.Abstract layer = this.drawingContext.RootObject();

			if ( edit )
			{
				if ( item != null && !item.IsEditable )  return;

				// Désélectionne tous les objets pour éventuellement masquer les règles
				// de l'objet en édition.
				foreach ( Objects.Abstract obj in this.document.Flat(layer) )
				{
					if ( obj != item )
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

				// Sélectionne et édite l'objet demandé et montre sa zone dans les règles.
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
				}
			}
			else
			{
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
		}

		protected void Select(Rectangle rect, bool add, bool partial)
		{
			//	Sélectionne tous les objets dans le rectangle.
			//	partial = false -> toutes les poignées doivent être dans le rectangle
			//	partial = true  -> une seule poignée doit être dans le rectangle
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

		protected void MoveAllStarting()
		{
			//	Début du déplacement de tous les objets sélectionnés.
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Deep(layer, true) )
			{
				obj.MoveAllStarting();
			}
		}

		protected void MoveAllProcess(Point move)
		{
			//	Effectue le déplacement de tous les objets sélectionnés.
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Deep(layer, true) )
			{
				obj.MoveAllProcess(move);
			}

			this.document.Modifier.AddMoveAfterDuplicate(move);
		}

		public void MoveGlobalStarting()
		{
			//	Début du déplacement global de tous les objets sélectionnés.
			this.selector.InitialBBoxThin = this.document.Modifier.SelectedBboxThin;
			this.selector.FinalToInitialData();

			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Deep(layer, true) )
			{
				obj.MoveGlobalStartingProperties();
				obj.MoveGlobalStarting();
			}
		}

		public void MoveGlobalProcess(Selector selector)
		{
			//	Effectue le déplacement global de tous les objets sélectionnés.
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Deep(layer, true) )
			{
				obj.MoveGlobalProcessProperties(selector);
				obj.MoveGlobalProcess(selector);
			}
		}

		public void DrawLabels(Graphics graphics)
		{
			//	Dessine les noms de tous les objets.
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer) )
			{
				obj.DrawLabel(graphics, this.drawingContext);
			}
		}

		public void DrawAggregates(Graphics graphics)
		{
			//	Dessine les noms de tous les styles.
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer) )
			{
				obj.DrawAggregate(graphics, this.drawingContext);
			}
		}

		public void DrawHandles(Graphics graphics)
		{
			//	Dessine les poignées de tous les objets.
			Objects.Abstract layer = this.drawingContext.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer) )
			{
				obj.DrawHandle(graphics, this.drawingContext);
			}

			this.hotSpotHandle.Draw(graphics, this.drawingContext);
		}

		public void DrawEditCreateRect(Graphics graphics)
		{
			//	Dessine le rectangle créable pour l'édition.
			if ( this.editCreateRect.IsEmpty )  return;

			double initialWidth = graphics.LineWidth;
			graphics.LineWidth = 1.0/this.drawingContext.ScaleX;

			Drawing.Rectangle rect = this.editCreateRect;
			graphics.Align(ref rect);
			rect.Offset(0.5/this.drawingContext.ScaleX, 0.5/this.drawingContext.ScaleX);

			Path path = null;

			if ( rect.Height == 0 )
			{
				path = Path.FromLine(rect.BottomLeft, rect.BottomRight);
			}
			else
			{
				rect.Deflate(1.0/this.drawingContext.ScaleX);
				path = Path.FromRectangle(rect);
			}

			Drawer.DrawPathDash(graphics, this.drawingContext, path, 1.0, 4.0, 5.0, Color.FromBrightness(0.6));
			graphics.LineWidth = initialWidth;
		}


		public bool MousePos(out Point pos)
		{
			//	Retourne la position de la souris.
			pos = this.mousePos;
			return this.mousePosValid;
		}

		protected bool IsActiveViewer
		{
			//	Indique si on est dans le viewer actif.
			get
			{
				if ((this.document != null) &&
					(this.document.Modifier != null))
				{
					return (this == this.document.Modifier.ActiveViewer);
				}
				else
				{
					return false;
				}
			}
		}


		#region Popup interface
		public void OpenPopupInterface(Widget frame, Objects.Abstract obj, System.Func<Viewer, Objects.Abstract, Point> getPosition)
		{
			this.ClosePopupInterface ();

			this.popupInterfaceWindow = new Window ();
			this.popupInterfaceWindow.MakeFramelessWindow ();
			this.popupInterfaceWindow.MakeFloatingWindow ();
			this.popupInterfaceWindow.DisableMouseActivation ();
			this.popupInterfaceWindow.MakeLayeredWindow (true);
			this.popupInterfaceWindow.Root.SyncPaint = true;
			this.popupInterfaceWindow.WindowSize = frame.PreferredSize;
			this.popupInterfaceWindow.Root.BackColor = Color.FromAlphaRgb (0, 1, 1, 1);
			this.popupInterfaceWindow.Owner = this.Window;

			frame.Parent = this.popupInterfaceWindow.Root;
			frame.Dock = DockStyle.Fill;

			this.popupInterfaceFrame = frame;
			this.popupInterfaceObject = obj;
			this.popupInterfaceGetPosition = getPosition;

			this.popupInterfaceWindow.Show ();
		}

		public void MovePopupInterface()
		{
			if (this.popupInterfaceWindow != null)
			{
				var pos = this.popupInterfaceGetPosition (this, this.popupInterfaceObject);
				pos = new Point (pos.X, pos.Y-this.popupInterfaceFrame.PreferredHeight);

				this.popupInterfaceWindow.WindowLocation = this.MapClientToScreen (pos);
			}
		}

		public void ClosePopupInterface()
		{
			if (this.popupInterfaceWindow == null)
			{
				return;
			}

			this.popupInterfaceFrame.Dispose ();
			this.popupInterfaceFrame = null;
			this.popupInterfaceObject = null;

			this.popupInterfaceWindow.Close ();
			this.popupInterfaceWindow.AsyncDispose ();
			this.popupInterfaceWindow = null;
		}

		public Objects.Abstract PopupInterfaceObject
		{
			get
			{
				return this.popupInterfaceObject;
			}
		}

		public Widget PopupInterfaceFrame
		{
			get
			{
				return this.popupInterfaceFrame;
			}
		}
		#endregion


		#region MiniBar
		public void OpenMiniBar(Point mouse, MiniBarDelayed delayed, bool noSelected, bool hot, double distance)
		{
			//	Ouvre la mini-palette.
			this.CloseMiniBar(false);

			List<string> cmds = this.MiniBarCommands(noSelected);
			if (cmds == null || cmds.Count == 0)
			{
				return;
			}

			Balloon frame = new Balloon();
			this.miniBarDistance = (distance == 0) ? frame.Distance : distance;
			if (!hot && distance == 0)
			{
				this.miniBarDistance = 0;
			}

			this.miniBarClickPos = mouse;
			mouse = this.InternalToScreen(mouse);
			mouse.Y ++;  // pour ne pas être sur le pixel visé par la souris

			ScreenInfo si = ScreenInfo.Find(this.MapClientToScreen(mouse));
			Drawing.Rectangle wa = si.WorkingArea;

			this.miniBarLines = 1;
			double maxWidth = 0;
			double width = 0;
			foreach ( string cmd in cmds )
			{
				if ( cmd == "#" )  // fin de ligne ?
				{
					maxWidth = System.Math.Max(maxWidth, width);
					width = 0;
					this.miniBarLines ++;
				}
				else	// commande ou séparateur ?
				{
					width += this.MiniBarCommandWidth(cmd);
				}
			}
			maxWidth = System.Math.Max(maxWidth, width);

			double mx = maxWidth + frame.Margin*2 + 1;
			double my = 22*this.miniBarLines + frame.Margin*(this.miniBarLines-1) + frame.Margin*2 + this.miniBarDistance;
			Size size = new Size(mx, my);
			mouse.X -= size.Width/2;
			this.miniBarRect = new Drawing.Rectangle(mouse, size);
			this.miniBarHot = hot ? size.Width/2 : double.NaN;

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

			switch ( delayed )
			{
				case MiniBarDelayed.Delayed:      this.miniBarTimer.Delay = 0.2;  break;
				case MiniBarDelayed.DoubleClick:  this.miniBarTimer.Delay = SystemInformation.DoubleClickDelay;  break;
				default:                          this.miniBarTimer.Delay = 0.01;  break;
			}
			this.miniBarTimer.Start();
		}

		protected void RestartMiniBar()
		{
			if ( this.miniBarCmds != null )
			{
				this.miniBarTimer.Stop();
				this.miniBarTimer.Delay = SystemInformation.DoubleClickDelay;
				this.miniBarTimer.Start();
			}
		}

		protected void HandleMiniBarTimeElapsed(object sender)
		{
			//	Appelé lorsque le timer arrive à échéance.
			this.miniBarTimer.Suspend();
			this.CreateMiniBar();
		}

		protected void CreateMiniBar()
		{
			//	Crée la mini-palette.
			Point mouse;
			if ( !this.MousePos(out mouse) )
			{
				this.miniBarCmds = null;
				return;
			}

			double d = Point.Distance(mouse, this.miniBarClickPos)*this.drawingContext.ScaleX;
			if ( d > 4 )
			{
				this.miniBarCmds = null;
				return;
			}

			this.miniBar = new Window();
			this.miniBar.MakeFramelessWindow();
			this.miniBar.MakeFloatingWindow();
			this.miniBar.DisableMouseActivation();
			this.miniBar.MakeLayeredWindow(true);
			this.miniBar.Root.SyncPaint = true;
			this.miniBar.WindowSize = this.miniBarRect.Size;
			this.miniBar.WindowLocation = this.MapClientToScreen(this.miniBarRect.BottomLeft);
			this.miniBar.Root.BackColor = Color.FromAlphaRgb(0, 1,1,1);
			this.miniBar.Owner = this.Window;

			this.miniBarBalloon = new Balloon();
			this.miniBarBalloon.Hot = this.miniBarHot;
			this.miniBarBalloon.Distance = this.miniBarDistance;
			this.miniBarBalloon.SetParent(this.miniBar.Root);
			this.miniBarBalloon.Anchor = AnchorStyles.All;
			this.miniBarBalloon.CloseNeeded += this.HandleMiniBarCloseNeeded;
			this.miniBarBalloon.Attach();

			this.miniBar.Show();  // nécessaire pour que IsAway fonctionne !

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
					line.PreferredHeight = button.PreferredHeight;
					line.Dock = DockStyle.Top;
					line.Margins = new Margins(0, 0, m, 0);
					beginOfLine = false;
				}

				if ( cmd == "" )  // séparateur ?
				{
					IconSeparator sep = new IconSeparator();
					sep.PreferredWidth = this.MiniBarCommandWidth(cmd);
					sep.Dock = DockStyle.Left;
					sep.SetParent(line);
				}
				else if ( cmd == "#" )  // fin de ligne ?
				{
					beginOfLine = true;
				}
				else
				{
					Command c = Common.Widgets.Command.Find(cmd);

					IconButton button = new IconButton(c.CommandId, c.Icon, c.CommandId);
				
					if ( c.Statefull )
					{
						button.ButtonStyle = ButtonStyle.ActivableIcon;
					}

					button.PreferredWidth = this.MiniBarCommandWidth(cmd);
					button.Dock = DockStyle.Left;
					button.SetParent(line);
					button.Clicked += this.HandleMiniBarButtonClicked;

					ToolTip.Default.SetToolTip(button, c.GetDescriptionWithShortcut());
				}
			}
			this.miniBarCmds = null;

			this.miniBar.Show();
		}

		private void HandleMiniBarCloseNeeded(object sender)
		{
			//	Appelé lorsque la souris s'est éloignée est que la fermeture est nécessaire.
			this.CloseMiniBar(true);
		}

		private void HandleMiniBarButtonClicked(object sender, MessageEventArgs e)
		{
			Widget button = sender as Widget;
			if ( button != null )
			{
				if (
					button.Name == "OrderUpAll" ||
					button.Name == "OrderUpOne" ||
					button.Name == "OrderDownOne" ||
					button.Name == "OrderDownAll" ||

					button.Name == Res.Commands.FontBold.CommandId ||
					button.Name == Res.Commands.FontItalic.CommandId ||
					button.Name == Res.Commands.FontUnderline.CommandId ||
					button.Name == Commands.FontOverline ||
					button.Name == Commands.FontStrikeout ||
					button.Name == Commands.FontSubscript ||
					button.Name == Commands.FontSuperscript ||
					button.Name == Commands.FontSizePlus ||
					button.Name == Commands.FontSizeMinus ||
					button.Name == Commands.FontClear ||
					button.Name == "ParagraphLeading08" ||
					button.Name == "ParagraphLeading10" ||
					button.Name == "ParagraphLeading15" ||
					button.Name == "ParagraphLeading20" ||
					button.Name == "ParagraphLeading30" ||
					button.Name == "ParagraphLeadingPlus" ||
					button.Name == "ParagraphLeadingMinus" ||
					button.Name == "ParagraphIndentPlus" ||
					button.Name == "ParagraphIndentMinus" ||
					button.Name == "ParagraphClear" ||
					button.Name == "JustifHLeft" ||
					button.Name == "JustifHCenter" ||
					button.Name == "JustifHRight" ||
					button.Name == "JustifHJustif" ||
					button.Name == "JustifHAll"
					)  return;
			}

			this.CloseMiniBar(false);
		}

		public bool CloseMiniBar()
		{
			//	Ferme la mini-palette si nécessaire. Retourne true si elle a été fermée.
			if ( this.miniBar == null )  return false;
			this.CloseMiniBar(false);
			return true;
		}

		public void CloseMiniBar(bool fadeout)
		{
			//	Ferme la mini-palette.
			this.miniBarTimer.Suspend();

			if ( this.miniBar != null )
			{
				if ( fadeout )
				{
					this.miniBar.WindowAnimationEnded += this.HandleMiniBarWindowAnimationEnded;
					this.miniBar.AnimateShow(Animation.FadeOut);
				}
				else
				{
					this.miniBar.Close();
					this.miniBar.AsyncDispose();
				}

				this.miniBarBalloon.CloseNeeded -= this.HandleMiniBarCloseNeeded;
				this.miniBarBalloon.Detach();

				this.miniBarBalloon = null;
				this.miniBar = null;
			}
		}
		
		private void HandleMiniBarWindowAnimationEnded(object sender)
		{
			//	Quand l'animation de fermeture de la mini-palette est terminée, il faut
			//	encore supprimer la fenêtre, pour éviter qu'elle ne traîne ad eternum.
			Window miniBar = sender as Window;
			miniBar.AsyncDispose();
		}

		protected virtual List<string> MiniBarCommands(bool noSelected)
		{
			//	Retourne la liste des commandes pour la mini-palette.
			this.document.Notifier.GenerateEvents();
			List<string> list = new List<string> ();

			int nbSel = this.document.Modifier.TotalSelected;
			if ( nbSel == 0 )
			{
				if ( noSelected )
				{
					this.MiniBarAdd(list, Common.Widgets.Res.Commands.SelectAll.CommandId);
					this.MiniBarAdd(list, "");
					this.MiniBarAdd(list, "Paste");
					this.MiniBarAdd(list, "");
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
					this.MiniBarAdd(list, "ToTextBox2");
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

			//	Essaie différentes largeurs de justifications, pour retenir la meilleure,
			//	c'est-à-dire celle qui a le moins de déchets (place perdue sur la dernière ligne).
			double bestScraps = 10000;
			double bestHope = 8*22;
			int linesRequired = this.MiniBarCount(list)/8 + 1;
			for ( double hope=2*22 ; hope<=16*22 ; hope+=22 )
			{
				if ( this.MiniBarJustifDo(list, hope) == linesRequired )
				{
					double scraps = this.MiniBarJustifScraps(list);
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

		protected int MiniBarCount(IEnumerable<string> list)
		{
			//	Compte le nombre de commandes dans une liste.
			int count = 0;
			foreach ( string cmd in list )
			{
				if ( cmd != "" )  count ++;
			}
			return count;
		}

		protected int MiniBarJustifDo(List<string> list, double widthHope)
		{
			//	Justifie la mini-palette, en remplaçant certains séparateurs ("") par une marque
			//	de fin de ligne ("#").
			//	Retourne le nombre de lignes nécessaires.
			double width = 0;
			int lines = 1;
			for ( int i=0 ; i<list.Count ; i++ )
			{
				string cmd = list[i] as string;

				if ( cmd == "" )  // séparateur ?
				{
					if ( width >= widthHope )
					{
						list.RemoveAt(i);     // supprime le séparateur...
						list.Insert(i, "#");  // ...et remplace-le par une marque de fin de ligne
						width = 0;
						lines ++;
					}
					else
					{
						width += this.MiniBarCommandWidth(cmd);
					}
				}
				else	// commande ?
				{
					width += this.MiniBarCommandWidth(cmd);
				}
			}
			return lines;
		}

		protected void MiniBarJustifClear(List<string> list)
		{
			//	Supprime la justification de la mini-palette.
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

		protected double MiniBarJustifScraps(List<string> list)
		{
			//	Retourne la longueur inutilisée la plus grande. Il s'agit généralement de la place
			//	perdue à la fin de la dernière ligne.
			double shortestLine = 10000;
			double longestLine = 0;
			double width = 0;
			foreach ( string cmd in list )
			{
				if ( cmd == "#" )  // fin de ligne ?
				{
					shortestLine = System.Math.Min(shortestLine, width);
					longestLine  = System.Math.Max(longestLine,  width);
					width = 0;
				}
				else	// commande ou séparateur ?
				{
					width += this.MiniBarCommandWidth(cmd);
				}
			}
			shortestLine = System.Math.Min(shortestLine, width);
			longestLine  = System.Math.Max(longestLine,  width);

			return longestLine-shortestLine;
		}

		protected double MiniBarCommandWidth(string cmd)
		{
			//	Retourne la largeur du widget d'une commande.
			if ( cmd == "" )  // séparateur ?
			{
				return 12;
			}
			else if ( cmd == "#" )  // fin de ligne ?
			{
				return 0;
			}
			else	// commande ?
			{
				return 22;
			}
		}

		public void MiniBarAdd(List<string> list, string cmd)
		{
			//	Ajoute une commande dans la liste pour la mini-palette.
			//	Une commande n'est qu'une seule fois dans la liste.
			//	Si la commande est disable (selon le CommandDispatcher), elle n'est pas ajoutée.
			if ( cmd == "" )  // séparateur ?
			{
				if ( list.Count == 0 )  return;

				string last = list[list.Count-1] as string;
				if ( last == "" )  return;  // déjà un séparateur ?
			}
			else	// commande ?
			{
				if ( list.Contains(cmd) )  return;  // déjà dans la liste ?

				CommandState cs = this.document.GetCommandState(cmd);
				if ( cs != null )
				{
					if ( !cs.Enable )  return;
				}
			}

			list.Add(cmd);
		}
		#endregion


		#region ContextMenu
		protected void ContextMenu(Point mouse, bool globalMenu)
		{
			//	Construit le menu contextuel.
			this.ClearHilite();

			int nbSel = this.document.Modifier.TotalSelected;
			bool exist;

			//	Construit le sous-menu "ordre".
			if ( globalMenu || nbSel == 0 )
			{
				this.contextMenuOrder = null;
			}
			else
			{
				List<Menus.ContextMenuItem> listOrder = new List<Menus.ContextMenuItem> ();

				exist = false;
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, listOrder, "OrderUpAll");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, listOrder, "OrderUpOne");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, listOrder, "OrderDownOne");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, listOrder, "OrderDownAll");

				if ( Menus.ContextMenuItem.IsMenuActive(listOrder) )
				{
					this.contextMenuOrder = new VMenu();
					this.contextMenuOrder.Host = this;
					Menus.ContextMenuItem.MenuCreate(this.contextMenuOrder, listOrder);
					this.contextMenuOrder.AdjustSize();
				}
				else
				{
					this.contextMenuOrder = null;
				}
			}

			//	Construit le sous-menu "opérations".
			if ( globalMenu || nbSel == 0 )
			{
				this.contextMenuOper = null;
			}
			else
			{
				List<Menus.ContextMenuItem> listOper = new List<Menus.ContextMenuItem> ();

				exist = false;
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, listOper, "Rotate90");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, listOper, "Rotate180");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, listOper, "Rotate270");
				if ( exist )  Menus.ContextMenuItem.MenuAddSep(listOper);

				exist = false;
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, listOper, "MirrorH");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, listOper, "MirrorV");
				if ( exist )  Menus.ContextMenuItem.MenuAddSep(listOper);

				exist = false;
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, listOper, "ScaleDiv2");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, listOper, "ScaleMul2");

				if ( Menus.ContextMenuItem.IsMenuActive(listOper) )
				{
					this.contextMenuOper = new VMenu();
					this.contextMenuOper.Host = this;
					Menus.ContextMenuItem.MenuCreate(this.contextMenuOper, listOper);
					this.contextMenuOper.AdjustSize();
				}
				else
				{
					this.contextMenuOper = null;
				}
			}

			//	Construit le sous-menu "géométrie".
			if ( globalMenu || nbSel == 0 )
			{
				this.contextMenuGeom = null;
			}
			else
			{
				List<Menus.ContextMenuItem> listGeom = new List<Menus.ContextMenuItem> ();

				exist = false;
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, listGeom, "Combine");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, listGeom, "Uncombine");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, listGeom, "ToBezier");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, listGeom, "ToPoly");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, listGeom, "ToTextBox2");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, listGeom, "Fragment");

				if ( Menus.ContextMenuItem.IsMenuActive(listGeom) )
				{
					this.contextMenuGeom = new VMenu();
					this.contextMenuGeom.Host = this;
					Menus.ContextMenuItem.MenuCreate(this.contextMenuGeom, listGeom);
					this.contextMenuGeom.AdjustSize();
				}
				else
				{
					this.contextMenuGeom = null;
				}
			}

			//	Construit le sous-menu "booléen".
			if ( globalMenu || nbSel == 0 )
			{
				this.contextMenuBool = null;
			}
			else
			{
				List<Menus.ContextMenuItem> listBool = new List<Menus.ContextMenuItem> ();

				exist = false;
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, listBool, "BooleanOr");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, listBool, "BooleanAnd");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, listBool, "BooleanXor");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, listBool, "BooleanFrontMinus");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, listBool, "BooleanBackMinus");

				if ( Menus.ContextMenuItem.IsMenuActive(listBool) )
				{
					this.contextMenuBool = new VMenu();
					this.contextMenuBool.Host = this;
					Menus.ContextMenuItem.MenuCreate(this.contextMenuBool, listBool);
					this.contextMenuBool.AdjustSize();
				}
				else
				{
					this.contextMenuBool = null;
				}
			}

			//	Construit le menu principal.
			List<Menus.ContextMenuItem> list = new List<Menus.ContextMenuItem> ();
			if ( globalMenu || nbSel == 0 )
			{
				exist = false;
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, Common.Widgets.Res.Commands.DeselectAll.CommandId);
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, Common.Widgets.Res.Commands.SelectAll.CommandId);
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "SelectInvert");
				if ( exist )  Menus.ContextMenuItem.MenuAddSep(list);

				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "Cut");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "Copy");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "Paste");
				if ( exist )  Menus.ContextMenuItem.MenuAddSep(list);

				exist = false;
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "HideSel");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "HideRest");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "HideCancel");
				if ( exist )  Menus.ContextMenuItem.MenuAddSep(list);

				exist = false;
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "ZoomMin");
				if ( this.document.Type != DocumentType.Pictogram )
				{
					exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "ZoomPage");
					exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "ZoomPageWidth");
				}
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "ZoomDefault");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "ZoomSel");
				if ( this.document.Type != DocumentType.Pictogram )
				{
					exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "ZoomSelWidth");
				}
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "ZoomPrev");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "ZoomSub");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "ZoomAdd");
				if ( exist )  Menus.ContextMenuItem.MenuAddSep(list);

				exist = false;
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "Outside");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "Grid");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "Magnet");
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
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "Delete");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "Duplicate");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "Cut");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "Copy");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "Group");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "Merge");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "Extract");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "Ungroup");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "Inside");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "Outside");
				if ( exist )  Menus.ContextMenuItem.MenuAddSep(list);

				exist = false;
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "ZoomSel");
				if ( this.document.Type != DocumentType.Pictogram )
				{
					exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "ZoomSelWidth");
				}
				if ( exist )  Menus.ContextMenuItem.MenuAddSep(list);

				exist = false;
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "HideSel");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "HideRest");
				exist |= Menus.ContextMenuItem.MenuAddItem(this.document.CommandContext, list, "HideCancel");
				if ( exist )  Menus.ContextMenuItem.MenuAddSep(list);

				exist = false;
				exist |= Menus.ContextMenuItem.MenuAddSubmenu(list, this.contextMenuOrder, Misc.Icon("OrderUpAll"), Res.Strings.Action.OrderMain);
				exist |= Menus.ContextMenuItem.MenuAddSubmenu(list, this.contextMenuOper, Misc.Icon("MoveH"), Res.Strings.Action.OperationMain);
				exist |= Menus.ContextMenuItem.MenuAddSubmenu(list, this.contextMenuGeom, Misc.Icon("Combine"), Res.Strings.Action.GeometryMain);
				exist |= Menus.ContextMenuItem.MenuAddSubmenu(list, this.contextMenuBool, Misc.Icon("BooleanOr"), Res.Strings.Action.BooleanMain);
			}

			this.contextMenu = new VMenu();
			this.contextMenu.Host = this;
			Menus.ContextMenuItem.MenuCreate(this.contextMenu, list);
			this.contextMenu.AdjustSize();
			mouse = this.InternalToScreen(mouse);
			mouse = this.MapClientToScreen(mouse);

			ScreenInfo si = ScreenInfo.Find(mouse);
			Drawing.Rectangle wa = si.WorkingArea;
			if ( mouse.Y-this.contextMenu.ActualHeight < wa.Bottom )
			{
				mouse.Y = wa.Bottom+this.contextMenu.ActualHeight;
			}
			
			this.contextMenu.ShowAsContextMenu(this.Window, mouse);
		}

		public void ExecuteObjectCommand(string cmd)
		{
			//	Exécute une commande locale à un objet.
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
			if (this.drawingContext == null)
			{
				return;
			}
			if (this.drawingContext.RootStackIsEmpty)
			{
				base.UpdateClientGeometry();
			}
			else
			{
				Point center = this.drawingContext.Center;
				base.UpdateClientGeometry();
				this.drawingContext.ZoomAndCenter(this.drawingContext.Zoom, center, false);
			}
		}


		#region MouseCursor
		protected void UpdateMouseCursor(Message message)
		{
			//	Adapte le sprite de la souris en fonction des touches Shift et Ctrl.
			if ( this.mouseCursorType == MouseCursorType.Arrow       ||
				 this.mouseCursorType == MouseCursorType.ArrowDup    ||
				 this.mouseCursorType == MouseCursorType.ArrowPlus   ||
				 this.mouseCursorType == MouseCursorType.ArrowGlobal )
			{
				if ( message.IsControlPressed && !this.mouseDragging && this.hiliteObject != null )
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
				 this.mouseCursorType == MouseCursorType.ArrowMinus )
			{
				if ( message.IsControlPressed && !this.mouseDragging )
				{
					this.ChangeMouseCursor(MouseCursorType.FingerDup);
				}
				else if ( message.IsShiftPressed && !this.mouseDragging )
				{
					this.ChangeMouseCursor(MouseCursorType.ArrowMinus);
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
					if ( message.IsControlPressed || (this.mouseDragging && this.zoomCtrl) )
					{
						this.ChangeMouseCursor(MouseCursorType.ZoomShiftCtrl);
					}
					else
					{
						this.ChangeMouseCursor(MouseCursorType.ZoomShift);
					}
				}
				else if ( message.IsControlPressed )
				{
					this.ChangeMouseCursor(MouseCursorType.ZoomMinus);
				}
				else
				{
					this.ChangeMouseCursor(MouseCursorType.Zoom);
				}
			}
		}

		protected void ChangeMouseCursor(MouseCursorType cursor)
		{
			//	Change le sprite de la souris.
			this.mouseCursorType = cursor;
		}

		protected void UseMouseCursor()
		{
			//	Utilise le bon sprite pour la souris.
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

				case MouseCursorType.ArrowMinus:
					this.MouseCursorImage(ref this.mouseCursorArrowMinus, Misc.Icon("ArrowMinus"));
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
					this.MouseCursorImage(ref this.mouseCursorIBeam, Misc.Icon("IBeam"));
					break;

				case MouseCursorType.IBeamCreate:
					this.MouseCursorImage(ref this.mouseCursorIBeamCreate, Misc.Icon("IBeamCreate"));
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

				case MouseCursorType.TextFlowCreateBox:
					this.MouseCursorImage(ref this.mouseCursorTextFlowCreateBox, Misc.Icon("TextFlowCreateBox"));
					break;

				case MouseCursorType.TextFlowCreateLine:
					this.MouseCursorImage(ref this.mouseCursorTextFlowCreateLine, Misc.Icon("TextFlowCreateLine"));
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

		protected void MouseCursorImage(ref Image image, string name)
		{
			//	Choix du sprite de la souris.
			if ( image == null )
			{
				image = Support.ImageProvider.Default.GetImage(name, Support.Resources.DefaultManager);
			}
			
			this.MouseCursor = MouseCursor.FromImage(image);
		}
		#endregion


		#region GuideInteractive
		protected bool GuideDetect(Point pos, out int rank)
		{
			//	Détecte le guide pointé par la souris.
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

		protected bool GuideIsHorizontal(int rank)
		{
			//	Indique si un guide est horizontal.
			Settings.Guide guide = this.document.Settings.GuidesGet(rank);
			return guide.IsHorizontal;
		}

		protected void GuideHilite(int rank)
		{
			//	Met en évidence le guide survolé par la souris.
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

		public void GuideInteractiveStart(bool horizontal)
		{
			//	Début de l'insertion interactive d'un guide (drag depuis une règle).
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

		public void GuideInteractiveMove(Point pos, bool isAlt)
		{
			//	Positionne un guide interactif.
			if ( this.guideInteractive == -1 )  return;

			//	Ne pas utiliser SnapGrid pour ignorer les repères !
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

		public void GuideInteractiveEnd()
		{
			//	Termine le déplacement d'un guide interactif.
			if ( this.guideInteractive == -1 )  return;

			Size size = this.document.PageSize;
			Settings.Guide guide = this.document.Settings.GuidesGet(this.guideInteractive);

			//	Supprime le repère s'il est tiré hors de la vue.
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

			bool nothing = false;
			if ( guide.Position == Settings.Guide.Undefined )
			{
				this.document.Settings.GuidesRemoveAt(this.guideInteractive);
				
				if (this.guideCreate)
				{
					nothing = true;
				}
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

			if (nothing)
			{
				this.document.Modifier.OpletQueueCancelAction();
			}
			else
			{
				this.document.Modifier.OpletQueueValidateAction();
			}
			this.document.Notifier.NotifyGuidesChanged();
			this.guideInteractive = -1;
			this.mouseDragging = false;
		}
		#endregion

		
		#region DialogError
		public void DialogError(string error)
		{
			//	Affiche le dialogue pour signaler une erreur.
			if ( error == "" )  return;

			string title = Res.Strings.Dialog.Title;
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";
			string message = error;

			Common.Dialogs.IDialog dialog = Common.Dialogs.MessageDialog.CreateOk(title, icon, message, "", this.GetCommandDispatcher());
			dialog.OwnerWindow = this.Window;
			dialog.OpenDialog();
		}

		public bool DialogYesNo(string question)
		{
			//	Affiche un dialogue pour poser une question oui/non.
			string title = Res.Strings.Dialog.Title;
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Question.icon";
			string message = question;

			Common.Dialogs.IDialog dialog = Common.Dialogs.MessageDialog.CreateYesNo(title, icon, message, null, null, this.GetCommandDispatcher());
			dialog.OwnerWindow = this.Window;
			dialog.OpenDialog();
			return (dialog.Result == Common.Dialogs.DialogResult.Yes);
		}
		#endregion


		#region Drawing
		protected void DrawGridBackground(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine la grille magnétique dessous.
			double initialWidth = graphics.LineWidth;
			graphics.LineWidth = 1.0/this.drawingContext.ScaleX;

			double ix = 0.5/this.drawingContext.ScaleX;
			double iy = 0.5/this.drawingContext.ScaleY;

			if ( this.IsActiveViewer )
			{
				if ( this.document.Type == DocumentType.Graphic )
				{
					//	Dessine la "page".
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

		protected void DrawGridForeground(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine la grille magnétique dessus.
			double initialWidth = graphics.LineWidth;
			graphics.LineWidth = 1.0/this.drawingContext.ScaleX;

			double ix = 0.5/this.drawingContext.ScaleX;
			double iy = 0.5/this.drawingContext.ScaleY;

			clipRect.Inflate(1);
			clipRect = this.ScreenToInternal(clipRect);
			clipRect = Rectangle.Intersection(clipRect, this.document.Modifier.RectangleArea);

			if ( this.IsActiveViewer )
			{
				if (this.paintPageFrame)
				{
					if (this.document.Type == DocumentType.Graphic)
					{
						//	Dessine la "page".
						Rectangle rect = this.document.Modifier.PageArea;
						graphics.Align(ref rect);
						rect.Offset(ix, iy);

						graphics.AddRectangle(rect);
						graphics.RenderSolid(Color.FromAlphaRgb(0.4, 0.5, 0.5, 0.5));
					}

					Rectangle area = this.document.Modifier.RectangleArea;
					graphics.Align(ref area);
					area.Offset(ix, iy);
					graphics.AddRectangle(area);
					graphics.RenderSolid(Color.FromAlphaRgb(0.4, 0.5, 0.5, 0.5));
				}
			}

			if ( this.drawingContext.PreviewActive )
			{
				if ( this.document.Type == DocumentType.Graphic )
				{
					if (this.paintPageFrame)
					{

						//	Dessine la "page".
						Rectangle rect = this.document.Modifier.PageArea;
						graphics.Align(ref rect);
						rect.Offset(ix, iy);

						graphics.AddRectangle(rect);
						graphics.RenderSolid(Color.FromBrightness(0));
					}

					if ( this.document.Settings.PrintInfo.Target   &&
						!this.document.Settings.PrintInfo.AutoZoom )
					{
						this.document.Printer.PaintTarget(graphics, this.drawingContext, this.drawingContext.CurrentPage);
					}
				}
			}
			else
			{
				//	Dessine la grille.
				if ( this.drawingContext.GridShow && this.drawingContext.GridStep.X != 0 && this.drawingContext.GridStep.Y != 0 )
				{
					double s = System.Math.Min(this.drawingContext.GridStep.X*this.drawingContext.ScaleX,
											   this.drawingContext.GridStep.Y*this.drawingContext.ScaleY);
					int mul = (int) System.Math.Max(10.0/s, 1.0);

					Point origin = this.document.Modifier.OriginArea;
					origin = Point.GridAlign(origin, -this.drawingContext.GridOffset, this.drawingContext.GridStep);

					//	Dessine les traits verticaux.
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
								graphics.RenderSolid(Color.FromAlphaRgb(0.3, 0.6,0.6,0.6));  // gris
							}
							else
							{
								graphics.RenderSolid(Color.FromAlphaRgb(0.1, 0.6,0.6,0.6));  // gris
							}
						}
						rank ++;
					}

					//	Dessine les traits horizontaux.
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
								graphics.RenderSolid(Color.FromAlphaRgb(0.3, 0.6,0.6,0.6));  // gris
							}
							else
							{
								graphics.RenderSolid(Color.FromAlphaRgb(0.1, 0.6,0.6,0.6));  // gris
							}
						}
						rank ++;
					}
				}

				//	Dessine la grille pour le texte.
				if ( this.drawingContext.TextGridShow )
				{
					double s = this.drawingContext.TextGridStep*this.drawingContext.ScaleY;
					int mul = (int) System.Math.Max(10.0/s, 1.0);

					Point origin = this.document.Modifier.OriginArea;
					origin = Point.GridAlign(origin, new Point(0, -this.drawingContext.TextGridOffset), new Point(0, this.drawingContext.TextGridStep));

					//	Dessine les traits horizontaux.
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
								graphics.RenderSolid(Color.FromAlphaRgb(0.3, 0.6,0.8,0.9));  // gris-bleu
							}
							else
							{
								graphics.RenderSolid(Color.FromAlphaRgb(0.1, 0.6,0.8,0.9));  // gris-bleu
							}
						}
						rank ++;
					}
				}

				//	Dessine les repères.
				this.DrawGuides(graphics);

				//	Dessine les marqueurs
				this.DrawMarker(graphics);

				//	Dessine la cible.
				if ( this.IsActiveViewer )
				{
					if ( this.document.Type == DocumentType.Pictogram )
					{
						Rectangle rect = new Rectangle(0, 0, this.document.PageSize.Width, this.document.PageSize.Height);
						graphics.Align(ref rect);
						rect.Offset(ix, iy);
						graphics.AddRectangle(rect);

						rect.Offset(-ix, -iy);
						rect.Deflate(2);
						graphics.Align(ref rect);
						rect.Offset(ix, iy);
						graphics.AddRectangle(rect);

						double cx = this.document.PageSize.Width/2;
						double cy = this.document.PageSize.Height/2;
						graphics.Align(ref cx, ref cy);
						cx += ix;
						cy += iy;
						graphics.AddLine(cx, 0, cx, this.document.PageSize.Height);
						graphics.AddLine(0, cy, this.document.PageSize.Width, cy);
						graphics.RenderSolid(Color.FromAlphaRgb(0.4, 0.5,0.5,0.5));
					}
				}
			}

			graphics.LineWidth = initialWidth;
		}

		public void DrawGuides(Graphics graphics)
		{
			//	Dessine tous les repères.
			if (this.drawingContext.GuidesShow)
			{
				Objects.Page page = this.document.DocumentObjects[this.drawingContext.CurrentPage] as Objects.Page;

				if (page.MasterGuides && this.drawingContext.MasterPageList.Count > 0)
				{
					foreach (Objects.Page masterPage in this.drawingContext.MasterPageList)
					{
						this.DrawGuides(graphics, masterPage.Guides, false);
					}
				}

				this.DrawGuides(graphics, page.Guides, !this.document.Settings.GlobalGuides);
				this.DrawGuides(graphics, this.document.Settings.GuidesListGlobal, this.document.Settings.GlobalGuides);
			}
		}

		protected void DrawGuides(Graphics graphics, UndoableList guides, bool editable)
		{
			//	Dessine tous les repères d'une liste.
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
						graphics.RenderSolid(Color.FromAlphaRgb(0.5, 0.0,0.8,0.0));  // vert
					}
					else
					{
						graphics.RenderSolid(Color.FromAlphaRgb(0.5, 0.0,0.0,0.8));  // bleuté
					}
				}
				else
				{
					graphics.RenderSolid(Color.FromAlphaRgb(0.5, 0.8,0.0,0.0));  // rouge
				}
			}
		}

		protected void DrawMarker(Graphics graphics)
		{
			//	Dessine toutes les marques.
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
				graphics.RenderSolid(Color.FromAlphaRgb(0.5, 0.0,0.0,0.8));  // bleuté
			}

			if ( !double.IsNaN(this.markerHorizontal) )
			{
				double x = rd.Left;
				double y = this.markerHorizontal;
				graphics.Align(ref x, ref y);
				x += ix;
				y += iy;
				graphics.AddLine(x, y, rd.Right, y);
				graphics.RenderSolid(Color.FromAlphaRgb(0.5, 0.0,0.0,0.8));  // bleuté
			}
		}

		public void DrawHotSpot(Graphics graphics)
		{
			//	Dessine le hotspot.
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

			graphics.RenderSolid(Color.FromRgb(1.0, 0.0, 0.0));  // rouge

			Objects.Handle handle = new Objects.Handle(this.document);
			handle.Type = Objects.HandleType.Center;
			handle.Position = this.document.HotSpot;
			handle.IsVisible = true;
			handle.Draw(graphics, this.drawingContext);

			graphics.LineWidth = initialWidth;
		}

		

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine le document.
			if (!this.redrawWhenResizing)
			{
				if (this.Window.IsSizeMoveInProgress)
				{
					//	N'affiche pas le document pendant le redimensionnement de la fenêtre.
					return;
				}
			}

			if (this.document.InitializationInProgress)
			{
				//	N'affiche pas le document pendant la phase d'initialisation.
				return;
			}

			if (this.drawingContext.RootStackIsEmpty)
			{
				return;
			}

			IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;

			//	Ignore une zone de repeinture d'un pixel à gauche ou en haut,
			//	à cause des règles qui chevauchent Viewer.
			if ( clipRect.Right == 1                    ||  // un pixel à gauche ?
				 clipRect.Bottom == this.ActualHeight-1 )   // un pixel en haut ?
			{
				return;  // ignore
			}

			if ( this.lastSize != this.ActualSize &&  // redimensionnement ?
				 this.zoomType != ZoomType.None   )   // zoom spécial ?
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
			this.lastSize = this.ActualSize;

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

			if (this.paintWorkSurface)
			{
				//	Peint le fond de la surface de travail.
				if (this.document.Type == DocumentType.Pictogram)
				{
					if (this.isMiniature)
					{
						graphics.AddFilledRectangle(clipRect);
						graphics.RenderSolid(Color.FromBrightness(1));  // fond blanc
					}
					else if (!this.BackColor.IsTransparent && this.drawingContext.PreviewActive)
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
					if (this.isMiniature)
					{
						graphics.AddFilledRectangle(clipRect);
						graphics.RenderSolid(Color.FromBrightness(1));  // fond blanc
					}
					else
					{
						graphics.AddFilledRectangle(clipRect);
						graphics.RenderSolid(Color.FromBrightness(0.95));
					}
				}
			}

			if (this.drawingContext.Viewer != null && this.isMiniature)  // dessin d'une miniature ?
			{
				Rectangle rect = this.drawingContext.Viewer.Client.Bounds;
				rect.Deflate(1);  // laisse un cadre d'un pixel
				Transform it = Transform.Identity;

				if (this.pictogramMiniatureZoom != 1)  // gros pixels ?
				{
					it = graphics.Transform;
					graphics.ScaleTransform(this.pictogramMiniatureZoom, this.pictogramMiniatureZoom, rect.Left, rect.Bottom);
					graphics.ImageFilter = new ImageFilter(ImageFilteringMode.None);  // ne surtout pas lisser
					rect.Width  /= this.pictogramMiniatureZoom;
					rect.Height /= this.pictogramMiniatureZoom;
				}

				if (this.isLayerMiniature)
				{
					Objects.Abstract layer = this.drawingContext.RootObject(2);
					if (layer.CacheBitmap == null)
					{
						graphics.AddLine(rect.BottomLeft, rect.TopRight);
						graphics.AddLine(rect.BottomRight, rect.TopLeft);
						graphics.RenderSolid(Color.FromBrightness(0));  // desssine un 'X' s'il n'y a pas de bitmap caché
					}
					else
					{
						graphics.PaintImage(layer.CacheBitmap, rect);  // dessine le bitmap caché
					}
				}
				else
				{
					Objects.Abstract page = this.drawingContext.RootObject(1);
					if (page.CacheBitmap == null)
					{
						graphics.AddLine(rect.BottomLeft, rect.TopRight);
						graphics.AddLine(rect.BottomRight, rect.TopLeft);
						graphics.RenderSolid(Color.FromBrightness(0));  // desssine un 'X' s'il n'y a pas de bitmap caché
					}
					else
					{
						graphics.PaintImage(page.CacheBitmap, rect);  // dessine le bitmap caché
					}
				}

				if (this.pictogramMiniatureZoom != 1)  // gros pixels ?
				{
					graphics.Transform = it;
					rect.Width  *= this.pictogramMiniatureZoom;
					rect.Height *= this.pictogramMiniatureZoom;
				}

				rect.Inflate(0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(Color.FromBrightness(0.3));  // dessine le cadre de la page
				return;
			}

			double initialWidth = graphics.LineWidth;
			Transform save = graphics.Transform;
			Point scale = this.drawingContext.Scale;
			graphics.ScaleTransform(scale.X, scale.Y, 0, 0);
			graphics.TranslateTransform(this.drawingContext.OriginX, this.drawingContext.OriginY);
			
			//	Dessine la grille magnétique dessous.
			this.DrawGridBackground(graphics, clipRect);

			//	Dessine les géométries.
			this.document.Paint(graphics, this.drawingContext, clipRect);

			//	Dessine la grille magnétique dessus.
			this.DrawGridForeground(graphics, clipRect);

			//	Dessine les noms de objets.
			if ( this.IsActiveViewer && this.drawingContext.LabelsShow && !this.drawingContext.PreviewActive )
			{
				this.DrawLabels(graphics);
			}

			//	Dessine les noms de styles.
			if ( this.IsActiveViewer && this.drawingContext.AggregatesShow && !this.drawingContext.PreviewActive )
			{
				this.DrawAggregates(graphics);
			}

			//	Dessine le rectangle créable pour l'édition.
			if ( this.IsActiveViewer )
			{
				this.DrawEditCreateRect(graphics);
			}

			//	Dessine les poignées.
			if ( this.IsActiveViewer )
			{
				this.DrawHandles(graphics);
			}

			//	Dessine le hotspot.
			if ( this.IsActiveViewer && this.document.Modifier.Tool == "ToolHotSpot" )
			{
				this.DrawHotSpot(graphics);
			}

			//	Dessine le rectangle de modification.
			this.selector.Draw(graphics, this.drawingContext);
			this.zoomer.Draw(graphics, this.drawingContext);

			//	Dessine les contraintes.
			this.drawingContext.DrawConstrain(graphics, this.document.Modifier.SizeArea);
			this.drawingContext.DrawMagnet(graphics, this.document.Modifier.SizeArea);

			graphics.Transform = save;
			graphics.LineWidth = initialWidth;

			if (this.paintWorkSurface)
			{
				//	Dessine le cadre.
				Rectangle rect = new Rectangle(0, 0, this.Client.Size.Width, this.Client.Size.Height);
				rect.Deflate(0.5);
				graphics.AddRectangle (rect);
				graphics.RenderSolid (adorner.ColorBorder);
			}

			if ( this.debugDirty )
			{
				graphics.AddFilledRectangle(clipRect);
				graphics.RenderSolid(Color.FromAlphaRgb(0.5, 0.2, 0.2, 0.0));  // beurk !
				this.debugDirty = false;
			}

			this.MovePopupInterface ();
		}

		public override void InvalidateRectangle(Rectangle rect, bool sync)
		{
			base.InvalidateRectangle(rect, sync);
		}

		public override void Invalidate(InvalidateReason reason)
		{
			//	Gère l'invalidation en fonction de la raison.
			if ( reason != InvalidateReason.FocusedChanged )
			{
				base.Invalidate(reason);
			}
		}

		private void HandleIsFocusedChanged(object sender, Types.DependencyPropertyChangedEventArgs e)
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
		
		protected void HandleFocused()
		{
			//	Appelé lorsque la vue prend le focus.
			Objects.AbstractText edit = this.document.Modifier.RetEditObject();
			if ( edit != null )
			{
				this.document.Notifier.NotifyArea(this, edit.EditSelectBox);
			}
		}

		protected void HandleDefocused()
		{
			//	Appelé lorsque la vue perd le focus.
			if (this.document.Modifier == null)
			{
				return;
			}

			Objects.AbstractText edit = this.document.Modifier.RetEditObject();
			if ( edit != null )
			{
				this.document.Notifier.NotifyArea(this, edit.EditSelectBox);
			}
		}

		public void DirtyAllViews()
		{
			//	Salit tous les visualisateurs.
			foreach ( Viewer viewer in this.document.Modifier.Viewers )
			{
				viewer.debugDirty = true;
				this.document.Notifier.NotifyArea();
			}
		}
		#endregion


		#region GuidesSearchBox
		public void ClearEditCreateRect()
		{
			//	Supprime le rectangle créable pour l'édition.
			if ( this.editCreateRect.IsEmpty )  return;
			this.editCreateRect = Drawing.Rectangle.Empty;
			this.document.Notifier.NotifyArea(this);
		}

		protected void EditCreateRect(Point pos)
		{
			//	Met en évidence le rectangle créable pour l'édition.
			Drawing.Rectangle rect = Drawing.Rectangle.Empty;

			if ( this.document.Modifier.IsToolEdit || this.document.Modifier.IsToolText )
			{
				Drawing.Rectangle box = this.GuidesSearchBox(pos);
				if ( !box.IsEmpty )
				{
					if ( this.document.Modifier.Tool == "ObjectTextLine2" ||
						 this.IsFreeForNewTextBox2(box, null) )
					{
						rect = box;

						if ( this.document.Modifier.Tool == "ObjectTextLine2" )
						{
							rect.Bottom = pos.Y;
							rect.Top    = pos.Y;  // juste une ligne horizontale

							Point p1 = rect.BottomLeft;
							Point p2 = rect.TopRight;
							this.drawingContext.SnapGrid(ref p1);
							this.drawingContext.SnapGrid(ref p2);
							rect.BottomLeft = p1;
							rect.TopRight = p2;
						}
					}
				}
			}

			if ( this.editCreateRect != rect )
			{
				this.editCreateRect = rect;
				this.document.Notifier.NotifyArea(this);
			}
		}

		public bool IsFreeForNewTextBox2(Drawing.Rectangle box, Objects.Abstract exclude)
		{
			//	Vérifie qu'une zone rectangulaire n'empiète sur aucun TextBox2 existant.
			return this.SearchIntersectedTextBox2(box, exclude) == null;
		}

		public Objects.TextBox2 SearchIntersectedTextBox2(Drawing.Rectangle box, Objects.Abstract exclude)
		{
			//	Retourne l'éventuel objet TextBox2 qui intersecte une zone rectangulaire.
			Objects.Abstract page = this.drawingContext.RootObject(1);
			foreach (Objects.Abstract obj in this.document.Deep(page))
			{
				if (obj is Objects.TextBox2 && obj != exclude)
				{
					if (obj.BoundingBoxThin.IntersectsWith(box))
					{
						return obj as Objects.TextBox2;
					}
				}
			}
			return null;
		}

		public Drawing.Rectangle GuidesSearchBox(Point pos)
		{
			//	Cherche la boîte délimitée par des repères, autour d'une position.
			System.Collections.ArrayList list = new System.Collections.ArrayList();

			Objects.Page page = this.document.DocumentObjects[this.drawingContext.CurrentPage] as Objects.Page;
			if ( page.MasterGuides && this.drawingContext.MasterPageList.Count > 0 )
			{
				foreach ( Objects.Page masterPage in this.drawingContext.MasterPageList )
				{
					this.GuidesSearchAdd(list, masterPage.Guides);
				}
			}

			this.GuidesSearchAdd(list, page.Guides);
			this.GuidesSearchAdd(list, this.document.Settings.GuidesListGlobal);

			double minX = this.GuidesSearchBest(list, pos.X, true,  false);
			double maxX = this.GuidesSearchBest(list, pos.X, false, false);
			double minY = this.GuidesSearchBest(list, pos.Y, true,  true );
			double maxY = this.GuidesSearchBest(list, pos.Y, false, true );

			list.Clear();
			list = null;

			Size size = this.document.PageSize;
			double mx = 0;
			double my = 0;
			double min = 0;
			if ( this.document.Type == DocumentType.Pictogram )
			{
				mx = 1.0;
				my = 1.0;
				min = 1.0;  // largeur/hauteur minimale
			}
			else
			{
				if ( System.Globalization.RegionInfo.CurrentRegion.IsMetric )
				{
					mx = 100.0;  // 10mm
					my = 100.0;
					min = 100.0;  // largeur/hauteur minimale
				}
				else
				{
					mx = 127.0;  // 0.5in
					my = 127.0;
					min = 127.0;  // largeur/hauteur minimale
				}
			}
			mx = System.Math.Min(mx, size.Width*0.25);
			my = System.Math.Min(my, size.Height*0.25);

			if ( double.IsNaN(minX) )  minX = mx;
			if ( double.IsNaN(maxX) )  maxX = size.Width-mx;
			if ( double.IsNaN(minY) )  minY = my;
			if ( double.IsNaN(maxY) )  maxY = size.Height-my;

			if ( maxX-minX >= min && maxY-minY >= min )
			{
				Drawing.Rectangle box = new Drawing.Rectangle(minX, minY, maxX-minX, maxY-minY);
				if ( box.Contains(pos) )
				{
					return box;
				}
			}

			return Drawing.Rectangle.Empty;
		}

		protected void GuidesSearchAdd(System.Collections.ArrayList list, UndoableList guides)
		{
			//	Ajoute tous les guides dans une liste unique.
			int total = guides.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				Settings.Guide guide = guides[i] as Settings.Guide;
				list.Add(guide);
			}
		}

		protected double GuidesSearchBest(System.Collections.ArrayList list, double pos, bool isMin, bool isHorizontal)
		{
			//	Cherche le guide le plus proche.
			double best = double.NaN;

			foreach ( Settings.Guide guide in list )
			{
				if ( isHorizontal != guide.IsHorizontal )  continue;
				double gp = guide.AbsolutePosition;

				if ( isMin )  // à gauche/en bas ?
				{
					if ( pos < gp )  continue;

					if ( double.IsNaN(best) )
					{
						best = gp;
					}
					else
					{
						if ( best < gp )
						{
							best = gp;
						}
					}
				}
				else	// à droite/en haut ?
				{
					if ( pos > gp )  continue;

					if ( double.IsNaN(best) )
					{
						best = gp;
					}
					else
					{
						if ( best > gp )
						{
							best = gp;
						}
					}
				}
			}

			return best;
		}
		#endregion


		#region Convert
		public Rectangle ScreenToInternal(Rectangle rect)
		{
			//	Conversion d'un rectangle écran -> rectangle interne.
			if ( !rect.IsInfinite )
			{
				rect.BottomLeft = this.ScreenToInternal(rect.BottomLeft);
				rect.TopRight   = this.ScreenToInternal(rect.TopRight);
			}
			return rect;
		}

		public Point ScreenToInternal(Point pos)
		{
			//	Conversion d'une coordonnée écran -> coordonnée interne.
			pos.X = pos.X/this.drawingContext.ScaleX - this.drawingContext.OriginX;
			pos.Y = pos.Y/this.drawingContext.ScaleY - this.drawingContext.OriginY;
			return pos;
		}

		public Rectangle InternalToScreen(Rectangle rect)
		{
			//	Conversion d'un rectangle interne -> rectangle écran.
			if ( !rect.IsInfinite )
			{
				rect.BottomLeft = this.InternalToScreen(rect.BottomLeft);
				rect.TopRight   = this.InternalToScreen(rect.TopRight);
			}
			return rect;
		}

		public Point InternalToScreen(Point pos)
		{
			//	Conversion d'une coordonnée interne -> coordonnée écran.
			pos.X = (pos.X+this.drawingContext.OriginX)*this.drawingContext.ScaleX;
			pos.Y = (pos.Y+this.drawingContext.OriginY)*this.drawingContext.ScaleY;
			return pos;
		}
		#endregion


		#region RedrawArea
		public void RedrawAreaFlush()
		{
			//	Vide la zone de redessin.
			this.redrawArea = Rectangle.Empty;
		}

		public void RedrawAreaMerge(Rectangle rect)
		{
			//	Agrandit la zone de redessin.
			if ( rect.IsEmpty )  return;
			this.redrawArea.MergeWith(rect);
		}

		public Rectangle RedrawArea
		{
			//	Retourne la zone de redessin.
			get { return this.redrawArea; }
		}

		public bool RedrawWhenResizing
		{
			//	Peint le contenu de la page même pendant un redimensionnement du widget.
			get
			{
				return this.redrawWhenResizing;
			}
			set
			{
				this.redrawWhenResizing = value;
			}
		}
		#endregion


		#region Paint Settings
		public bool PaintPageFrame
		{
			//	Peint le cadre de la page.
			get
			{
				return this.paintPageFrame;
			}
			set
			{
				this.paintPageFrame = value;
			}
		}

		public bool PaintWorkSurface
		{
			//	Peint le cadre de la page et la surface de travail.
			get
			{
				return this.paintWorkSurface;
			}
			set
			{
				this.paintWorkSurface = value;
			}
		}
		#endregion


		protected Document						document;
		protected DrawingContext				drawingContext;
		protected Selector						selector;
		protected Selector						zoomer;
		protected bool							isMiniature;
		protected bool							isLayerMiniature;
		protected double						pictogramMiniatureZoom = 1;
		protected bool							partialSelect;
		protected bool							selectorAdaptLine = true;
		protected bool							selectorAdaptText = true;
		protected bool							redrawWhenResizing;
		protected bool							paintPageFrame = true;
		protected bool							paintWorkSurface = true;
		protected Rectangle						redrawArea;
		protected MessageType					lastMessageType;
		protected Point							mousePosWidget;
		protected Point							mousePos;
		protected bool							mousePosValid;
		protected bool							mouseDragging;
		protected Point							handMouseStart;
		protected Point							moveStart;
		protected Point							moveOffset;
		protected Point							moveLast;
		protected Point							moveCenter;
		protected bool							moveAccept;
		protected bool							moveInitialSel;
		protected bool							moveReclick;
		protected bool							moveAutoSel;
		protected int							moveHandle = -1;
		protected int							moveSelectedSegment = -1;
		protected bool							moveSelectedHandle;
		protected int							moveGlobal = -1;
		protected Objects.Abstract				moveObject;
		protected Objects.Abstract				hiliteObject;
		protected Objects.Abstract				hiliteHandleObject;
		protected int							hiliteHandleRank = -1;
		protected int							createRank = -1;
		protected bool							debugDirty;
		protected Timer							autoScrollTimer;
		protected int							guideInteractive = -1;
		protected bool							guideCreate;
		protected bool							ctrlDown;
		protected bool							ctrlDuplicate;
		protected bool							zoomShift;
		protected bool							zoomCtrl;
		protected Point							zoomOrigin;
		protected Point							zoomOffset;
		protected double						zoomStart;
		protected Size							lastSize;
		protected ZoomType						zoomType = ZoomType.None;
		protected Objects.Handle				hotSpotHandle;
		protected double						markerVertical = double.NaN;
		protected double						markerHorizontal = double.NaN;
		protected Objects.DetectEditType		editFlowPress = Objects.DetectEditType.Out;
		protected Objects.DetectEditType		editFlowSelect = Objects.DetectEditType.Out;
		protected Objects.Abstract				editFlowSrc;
		protected Objects.AbstractText			editFlowAfterCreate;
		protected Point							editPosPress;
		protected Drawing.Rectangle				editCreateRect = Drawing.Rectangle.Empty;

		protected Point							miniBarClickPos;
		protected Timer							miniBarTimer;
		protected List<string>					miniBarCmds;
		protected int							miniBarLines;
		protected Drawing.Rectangle				miniBarRect;
		protected double						miniBarHot;
		protected double						miniBarDistance;
		protected Window						miniBar;
		protected Balloon						miniBarBalloon;
		protected VMenu							contextMenu;
		protected VMenu							contextMenuOrder;
		protected VMenu							contextMenuOper;
		protected VMenu							contextMenuGeom;
		protected VMenu							contextMenuBool;
		protected Objects.Abstract				contextMenuObject;
		protected Point							contextMenuPos;
		protected int							contextMenuRank;
		protected Window						popupInterfaceWindow;
		protected System.Func<Viewer, Objects.Abstract, Point> popupInterfaceGetPosition;
		protected Objects.Abstract				popupInterfaceObject;
		protected Widget						popupInterfaceFrame;

		protected MouseCursorType				mouseCursorType = MouseCursorType.Unknown;
		protected MouseCursorType				mouseCursorTypeUse = MouseCursorType.Unknown;
		protected Image							mouseCursorArrow;
		protected Image							mouseCursorArrowPlus;
		protected Image							mouseCursorArrowMinus;
		protected Image							mouseCursorArrowDup;
		protected Image							mouseCursorArrowGlobal;
		protected Image							mouseCursorShaperNorm;
		protected Image							mouseCursorShaperPlus;
		protected Image							mouseCursorShaperMove;
		protected Image							mouseCursorShaperMulti;
		protected Image							mouseCursorFinger;
		protected Image							mouseCursorFingerPlus;
		protected Image							mouseCursorFingerDup;
		protected Image							mouseCursorPen;
		protected Image							mouseCursorZoom;
		protected Image							mouseCursorZoomMinus;
		protected Image							mouseCursorZoomShift;
		protected Image							mouseCursorZoomShiftCtrl;
		protected Image							mouseCursorHand;
		protected Image							mouseCursorPicker;
		protected Image							mouseCursorPickerEmpty;
		protected Image							mouseCursorIBeam;
		protected Image							mouseCursorIBeamCreate;
		protected Image							mouseCursorTextFlow;
		protected Image							mouseCursorTextFlowCreateBox;
		protected Image							mouseCursorTextFlowCreateLine;
		protected Image							mouseCursorTextFlowAdd;
		protected Image							mouseCursorTextFlowRemove;
		protected Image							mouseCursorFine;
	}
}
