namespace Epsitec.Common.Widgets
{
	using ContentAlignment = Drawing.ContentAlignment;
	
	[System.Flags] public enum AnchorStyles
	{
		None			= 0,
		Top				= 1,
		Bottom			= 2,
		Left			= 4,
		Right			= 8,
			
		LeftAndRight	= Left + Right,
		TopAndBottom	= Top + Bottom,
	}
	
	[System.Flags] public enum WidgetState
	{
		ActiveNo		= 0,
		ActiveYes		= 1,
		ActiveMaybe		= 2,
		ActiveMask		= ActiveNo | ActiveYes | ActiveMaybe,
		
		
		None			= 0x00000000,		//	=> neutre
		Enabled			= 0x00010000,		//	=> pas grisé
		Focused			= 0x00020000,		//	=> reçoit les événements clavier
		Entered			= 0x00040000,		//	=> contient la souris
		Selected		= 0x00080000,		//	=> sélectionné
		Engaged			= 0x00100000,		//	=> pression en cours
		Error			= 0x00200000,		//	=> signale une erreur
	}
	
	
	/// <summary>
	/// 
	/// </summary>
	public class Widget : System.IDisposable
	{
		public Widget()
		{
			this.internal_state |= InternalState.Visible;
			this.internal_state |= InternalState.AutoCapture;
			this.internal_state |= InternalState.AutoMnemonic;
			
			this.widget_state |= WidgetState.Enabled;
			
			this.alignment = this.DefaultAlignment;
			this.Width     = this.DefaultWidth;
			this.Height    = this.DefaultHeight;
			this.anchor    = this.DefaultAnchor;
			
			this.back_color = Drawing.Color.FromName ("Control");
			this.fore_color = Drawing.Color.FromName ("ControlText");
		}
		
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.SetEntered (false);
			}
		}

		
		public AnchorStyles					Anchor
		{
			get { return this.anchor; }
			set { this.anchor = value; }
		}
		
		public Drawing.Color				BackColor
		{
			get { return this.back_color; }
			set
			{
				if (this.back_color != value)
				{
					this.back_color = value;
					this.Invalidate ();
				}
			}
		}
		
		public Drawing.Color				ForeColor
		{
			get { return this.fore_color; }
			set
			{
				if (this.fore_color != value)
				{
					this.fore_color = value;
					this.Invalidate ();
				}
			}
		}
		
		
		public double						Top
		{
			get { return this.y2; }
			set { this.SetBounds (this.x1, this.y1, this.x2, value); }
		}
		
		public double						Left
		{
			get { return this.x1; }
			set { this.SetBounds (value, this.y1, this.x2, this.y2); }
		}
		
		public double						Bottom
		{
			get { return this.y1; }
			set { this.SetBounds (this.x1, value, this.x2, this.y2); }
		}
		
		public double						Right
		{
			get { return this.x2; }
			set { this.SetBounds (this.x1, this.y1, value, this.y2); }
		}
		
		public Drawing.Rectangle			Bounds
		{
			get { return new Drawing.Rectangle (this.x1, this.y1, this.x2 - this.x1, this.y2 - this.y1); }
			set { this.SetBounds (value.X, value.Y, value.X + value.Width, value.Y + value.Height); }
		}
		
		public Drawing.Point				Location
		{
			get { return new Drawing.Point (this.x1, this.y1); }
			set { this.SetBounds (value.X, value.Y, value.X + this.x2 - this.x1, value.Y + this.y2 - this.y1); }
		}
		
		public Drawing.Size					Size
		{
			get { return new Drawing.Size (this.x2 - this.x1, this.y2 - this.y1); }
			set { this.SetBounds (this.x1, this.y1, this.x1 + value.Width, this.y1 + value.Height); }
		}
		
		public double						Width
		{
			get { return this.x2 - this.x1; }
			set { this.SetBounds (this.x1, this.y1, this.x1 + value, this.y2); }
		}
		
		public double						Height
		{
			get { return this.y2 - this.y1; }
			set { this.SetBounds (this.x1, this.y1, this.x2, this.y1 + value); }
		}
		
		public ContentAlignment				Alignment
		{
			get { return this.alignment; }
			set
			{
				if (this.alignment != value)
				{
					this.alignment = value;
					this.UpdateLayoutSize ();
					this.Invalidate ();
				}
			}
		}
		
		public ClientInfo					Client
		{
			get { return this.client_info; }
		}
		
		
		public void SuspendLayout()
		{
			lock (this)
			{
				this.suspend_counter++;
			}
		}
		
		public void ResumeLayout()
		{
			lock (this)
			{
				if (this.suspend_counter > 0)
				{
					this.suspend_counter--;
				}
				
				if (this.suspend_counter == 0)
				{
					if ((this.internal_state & InternalState.ChildrenChanged) != 0)
					{
						this.internal_state -= InternalState.ChildrenChanged;
						this.OnChildrenChanged ();
					}
				}
			}
		}
		
		public void SetClientAngle(int angle)
		{
			this.client_info.SetAngle (angle);
			this.UpdateClientGeometry ();
		}
		
		public void SetClientZoom(double zoom)
		{
			this.client_info.SetZoom (zoom);
			this.UpdateClientGeometry ();
		}
		
		
		public virtual ContentAlignment		DefaultAlignment
		{
			get { return ContentAlignment.MiddleLeft; }
		}

		public virtual AnchorStyles			DefaultAnchor
		{
			get { return AnchorStyles.Left | AnchorStyles.Top; }
		}
		
		public virtual Drawing.Font			DefaultFont
		{
			get { return Drawing.Font.DefaultFont; }
		}
		
		public virtual double				DefaultFontSize
		{
			get { return Drawing.Font.DefaultFontSize; }
		}
		
		public virtual double				DefaultWidth
		{
			get { return 80; }
		}
		public virtual double				DefaultHeight
		{
			get { return 20; }
		}
#if false
		public bool							CausesValidation
		{
			get;
			set;
		}
#endif
		public virtual bool					IsEnabled
		{
			get
			{
				if ((this.widget_state & WidgetState.Enabled) == 0)
				{
					return false;
				}
				if (this.parent != null)
				{
					return this.parent.IsEnabled;
				}
				
				return true;
			}
		}
		
		public virtual bool					IsFrozen
		{
			get
			{
				if ((this.internal_state & InternalState.Frozen) != 0)
				{
					return true;
				}
				if (this.parent != null)
				{
					return this.parent.IsFrozen;
				}
				
				return false;
			}
		}
		
		public virtual bool					IsVisible
		{
			get
			{
				if (((this.internal_state & InternalState.Visible) == 0) ||
					(this.parent == null))
				{
					return false;
				}
				
				return this.parent.IsVisible;
			}
		}

		public bool							IsFocused
		{
			get { return (this.widget_state & WidgetState.Focused) != 0; }
		}
		
		public bool							IsEntered
		{
			get { return (this.widget_state & WidgetState.Entered) != 0; }
		}
		
		public bool							IsSelected
		{
			get { return (this.widget_state & WidgetState.Selected) != 0; }
		}
		
		public bool							IsEngaged
		{
			get { return (this.widget_state & WidgetState.Engaged) != 0; }
		}
		
		public bool							IsError
		{
			get { return (this.widget_state & WidgetState.Error) != 0; }
			set
			{
				if (this.IsError != value)
				{
					if (value)
					{
						this.widget_state |= WidgetState.Error;
					}
					else
					{
						this.widget_state &= ~WidgetState.Error;
					}
					
					this.Invalidate ();
				}
			}
		}
		
		public bool							AutoCapture
		{
			get { return (this.internal_state & InternalState.AutoCapture) != 0; }
			set
			{
				if (value)
				{
					this.internal_state |= InternalState.AutoCapture;
				}
				else
				{
					this.internal_state &= ~InternalState.AutoCapture;
				}
			}
		}
		
		public bool							AutoFocus
		{
			get { return (this.internal_state & InternalState.AutoFocus) != 0; }
			set
			{
				if (value)
				{
					this.internal_state |= InternalState.AutoFocus;
				}
				else
				{
					this.internal_state &= ~InternalState.AutoFocus;
				}
			}
		}
		
		public bool							AutoEngage
		{
			get { return (this.internal_state & InternalState.AutoEngage) != 0; }
			set
			{
				if (value)
				{
					this.internal_state |= InternalState.AutoEngage;
				}
				else
				{
					this.internal_state &= ~InternalState.AutoEngage;
				}
			}
		}
		
		public bool							AutoRepeatEngaged
		{
			get { return (this.internal_state & InternalState.AutoRepeatEngaged) != 0; }
			set
			{
				if (value)
				{
					this.internal_state |= InternalState.AutoRepeatEngaged;
				}
				else
				{
					this.internal_state &= ~InternalState.AutoRepeatEngaged;
				}
			}
		}
		
		public bool							AutoToggle
		{
			get { return (this.internal_state & InternalState.AutoToggle) != 0; }
			set
			{
				if (value)
				{
					this.internal_state |= InternalState.AutoToggle;
				}
				else
				{
					this.internal_state &= InternalState.AutoToggle;
				}
			}
		}
		
		public bool							AutoMnemonic
		{
			get { return (this.internal_state & InternalState.AutoMnemonic) != 0; }
		}
		
		
		public virtual WidgetState			State
		{
			get { return this.widget_state; }
		}
		
		public virtual WidgetState			ActiveState
		{
			get { return this.widget_state & WidgetState.ActiveMask; }
			set
			{
				WidgetState active = this.widget_state & WidgetState.ActiveMask;
				
				System.Diagnostics.Debug.Assert ((value & WidgetState.ActiveMask) == value);
				
				if (active != value)
				{
					this.widget_state &= ~WidgetState.ActiveMask;
					this.widget_state |= value & WidgetState.ActiveMask;
					this.OnActiveStateChanged ();
					this.Invalidate ();
				}
			}
		}
		
		public virtual WidgetState			PaintState
		{
			get
			{
				WidgetState mask  = WidgetState.ActiveMask |
									WidgetState.Focused |
									WidgetState.Entered |
									WidgetState.Engaged |
									WidgetState.Selected |
									WidgetState.Error;
				
				WidgetState state = this.widget_state & mask;
				
				if (this.IsEnabled)
				{
					state |= WidgetState.Enabled;
				}
				
				return state;
			}
		}
		
		
		public bool							ContainsFocus
		{
			get
			{
				if (this.IsFocused)
				{
					return true;
				}
				
				if (this.Children.Count > 0)
				{
					Widget[] children = this.Children.Widgets;
					int  children_num = children.Length;
					
					for (int i = 0; i < children_num; i++)
					{
						if (children[i].ContainsFocus)
						{
							return true;
						}
					}
				}
				
				return false;
			}
		}
		
		public bool							CanFocus
		{
			get { return ((this.internal_state & InternalState.Focusable) != 0) && !this.IsFrozen; }
		}
		
		public bool							CanSelect
		{
			get { return ((this.internal_state & InternalState.Selectable) != 0) && !this.IsFrozen; }
		}
		
		public bool							CanEngage
		{
			get { return ((this.internal_state & InternalState.Engageable) != 0) && this.IsEnabled && !this.IsFrozen; }
		}
		
		public bool							AcceptThreeState
		{
			get { return (this.internal_state & InternalState.AcceptThreeState) != 0; }
			set
			{
				if (value)
				{
					this.internal_state |= InternalState.AcceptThreeState;
				}
				else
				{
					this.internal_state &= ~InternalState.AcceptThreeState;
				}
			}
		}
		
		
		public WidgetCollection				Children
		{
			get
			{
				if (this.children == null)
				{
					lock (this)
					{
						if (this.children == null)
						{
							this.CreateWidgetCollection ();
						}
					}
				}
				
				return this.children;
			}
		}
		
		public Widget						Parent
		{
			get { return this.parent; }
			
			set
			{
				if (value != this.parent)
				{
					if (value == null)
					{
						this.parent.Children.Remove (this);
					}
					else
					{
						value.Children.Add (this);
					}
				}
			}
		}
		
		public virtual WindowFrame			WindowFrame
		{
			get
			{
				Widget root = this.RootParent;
				
				if ((root == null) ||
					(root == this))
				{
					return null;
				}
				
				return root.WindowFrame;
			}
		}
		
		public virtual Widget				RootParent
		{
			get
			{
				Widget widget = this;
				
				while (widget.parent != null)
				{
					widget = widget.parent;
				}
				
				return widget;
			}
		}
		
		public int							RootAngle
		{
			get
			{
				Widget widget = this;
				int    angle  = 0;
				
				while (widget != null)
				{
					angle += widget.Client.Angle;
					widget = widget.parent;
				}
				
				return angle % 360;
			}
		}
		
		public double						RootZoom
		{
			get
			{
				Widget widget = this;
				double zoom   = 0;
				
				while (widget != null)
				{
					zoom  *= widget.Client.Zoom;
					widget = widget.parent;
				}
				
				return zoom;
			}
		}
		
		public Direction					RootDirection
		{
			get
			{
				switch (this.RootAngle)
				{
					case 0:		return Direction.Up;
					case 90:	return Direction.Left;
					case 180:	return Direction.Down;
					case 270:	return Direction.Right;
				}
				
				return Direction.None;
			}
		}
		
		
		public bool							HasChildren
		{
			get { return (this.children != null) && (this.children.Count > 0); }
		}
		
		public bool							HasParent
		{
			get { return this.parent != null; }
		}
		
		public string						Name
		{
			get
			{
				if ((this.name == null) || (this.name.Length == 0))
				{
					return "";
				}
				
				return this.name;
			}
			
			set
			{
				if ((value == null) || (value.Length == 0))
				{
					this.name = null;
				}
				else
				{
					this.name = value;
				}
			}
		}

		public virtual string				Text
		{
			get
			{
				if (this.text_layout == null)
				{
					return "";
				}
				
				string text = this.text_layout.Text;
				
				if (text == null)
				{
					return "";
				}
				
				return text;
			}
			
			set
			{
				if ((value == null) || (value.Length == 0))
				{
					this.DisposeTextLayout ();
					this.Shortcut.Mnemonic = (char) 0;
				}
				else
				{
					this.CreateTextLayout ();
					this.text_layout.Text = value;
					this.Shortcut.Mnemonic = this.Mnemonic;
				}
			}
		}
		
		public char							Mnemonic
		{
			get
			{
				if (this.AutoMnemonic)
				{
					//	Le code mnémonique est encapsulé par des tags <m>..</m>.
					
					return TextLayout.ExtractMnemonic (this.Text);
				}
				
				return (char) 0;
			}
		}
		
		public int							TabIndex
		{
			get { return this.tab_index; }
			set { this.tab_index = value; }
		}
		
		public TabNavigationMode			TabNavigation
		{
			get { return this.tab_navigation_mode; }
			set { this.tab_navigation_mode = value; }
		}
		
		public Shortcut						Shortcut
		{
			get
			{
				if (this.shortcut == null)
				{
					this.shortcut = new Shortcut ();
				}
				
				return this.shortcut;
			}
		}
		
		
		public event PaintEventHandler		PaintBackground;
		public event PaintEventHandler		PaintForeground;
		public event EventHandler			ChildrenChanged;
		public event EventHandler			LayoutChanged;
		
		public event MessageEventHandler	Pressed;
		public event MessageEventHandler	Released;
		public event MessageEventHandler	Clicked;
		public event MessageEventHandler	DoubleClicked;
		public event EventHandler			ShortcutPressed;
		
		public event EventHandler			Focused;
		public event EventHandler			Defocused;
		public event EventHandler			Selected;
		public event EventHandler			Deselected;
		public event EventHandler			Engaged;
		public event EventHandler			StillEngaged;
		public event EventHandler			Disengaged;
		public event EventHandler			ActiveStateChanged;
		
		
		//	Cursor
		//	FindNextWidget/FindPrevWidget
		
		public virtual void Hide()
		{
			this.SetVisible (false);
		}
		
		public virtual void Show()
		{
			this.SetVisible (true);
		}
		
		public virtual void Toggle()
		{
			if (this.AcceptThreeState)
			{
				switch (this.ActiveState)
				{
					case WidgetState.ActiveYes:
						this.ActiveState = WidgetState.ActiveMaybe;
						break;
					case WidgetState.ActiveMaybe:
						this.ActiveState = WidgetState.ActiveNo;
						break;
					case WidgetState.ActiveNo:
						this.ActiveState = WidgetState.ActiveYes;
						break;
				}
			}
			else
			{
				switch (this.ActiveState)
				{
					case WidgetState.ActiveYes:
					case WidgetState.ActiveMaybe:
						this.ActiveState = WidgetState.ActiveNo;
						break;
					case WidgetState.ActiveNo:
						this.ActiveState = WidgetState.ActiveYes;
						break;
				}
			}
		}
		
		
		public virtual void SetVisible(bool visible)
		{
			if ((this.internal_state & InternalState.Visible) == 0)
			{
				if (visible)
				{
					this.internal_state |= InternalState.Visible;
					this.Invalidate ();
				}
			}
			else
			{
				if (!visible)
				{
					this.internal_state &= ~ InternalState.Visible;
					this.Invalidate ();
				}
			}
		}
		
		public virtual void SetEnabled(bool enabled)
		{
			if ((this.widget_state & WidgetState.Enabled) == 0)
			{
				if (enabled)
				{
					this.widget_state |= WidgetState.Enabled;
					this.Invalidate ();
				}
			}
			else
			{
				if (!enabled)
				{
					this.widget_state &= ~ WidgetState.Enabled;
					this.Invalidate ();
				}
			}
		}
		
		public virtual void SetFrozen(bool frozen)
		{
			if ((this.internal_state & InternalState.Frozen) == 0)
			{
				if (frozen)
				{
					this.internal_state |= InternalState.Frozen;
					this.Invalidate ();
				}
			}
			else
			{
				if (!frozen)
				{
					this.internal_state &= ~ InternalState.Frozen;
					this.Invalidate ();
				}
			}
		}
		
		public virtual void SetFocused(bool focused)
		{
			WindowFrame frame = this.WindowFrame;
			
			if (frame == null)
			{
				return;
			}
			
			if ((this.widget_state & WidgetState.Focused) == 0)
			{
				if (focused)
				{
					this.widget_state |= WidgetState.Focused;
					frame.FocusedWidget = this;
					this.OnFocused ();
					this.Invalidate ();
				}
			}
			else
			{
				if (!focused)
				{
					this.widget_state &= ~ WidgetState.Focused;
					frame.FocusedWidget = null;
					this.OnDefocused ();
					this.Invalidate ();
				}
			}
		}
		
		public virtual void SetSelected(bool selected)
		{
			if ((this.widget_state & WidgetState.Selected) == 0)
			{
				if (selected)
				{
					this.widget_state |= WidgetState.Selected;
					this.Invalidate ();
					this.OnSelected ();
				}
			}
			else
			{
				if (!selected)
				{
					this.widget_state &= ~WidgetState.Selected;
					this.Invalidate ();
					this.OnDeselected ();
				}
			}
		}
		
		public virtual void SetEngaged(bool engaged)
		{
			WindowFrame frame = this.WindowFrame;
			
			if (frame == null)
			{
				return;
			}
			
			if ((this.internal_state & InternalState.Engageable) == 0)
			{
				return;
			}
			
			if ((this.widget_state & WidgetState.Engaged) == 0)
			{
				if (engaged)
				{
					this.widget_state |= WidgetState.Engaged;
					frame.EngagedWidget = this;
					this.Invalidate ();
					this.OnEngaged ();
				}
			}
			else
			{
				if (!engaged)
				{
					this.widget_state &= ~ WidgetState.Engaged;
					frame.EngagedWidget = null;
					this.Invalidate ();
					this.OnDisengaged ();
				}
			}
		}
		
		internal virtual void FireStillEngaged()
		{
			if (this.IsEngaged)
			{
				this.OnStillEngaged ();
			}
		}
		
		internal virtual void SimulatePressed()
		{
			this.OnPressed (null);
		}
		
		internal virtual void SimulateReleased()
		{
			this.OnReleased (null);
		}
		
		internal virtual void SimulateClicked()
		{
			this.OnClicked (null);
		}
		
		protected void SetEntered(bool entered)
		{
			if (this.IsEntered != entered)
			{
				WindowFrame frame = this.WindowFrame;
				Message message = null;
				
				if (entered)
				{
					Widget.entered_widgets.Add (this);
					this.widget_state |= WidgetState.Entered;
					
					message = Message.FromMouseEvent (MessageType.MouseEnter, null, null);
				}
				else
				{
					Widget.entered_widgets.Remove (this);
					this.widget_state &= ~ WidgetState.Entered;
					
					message = Message.FromMouseEvent (MessageType.MouseLeave, null, null);
				}
				
				this.MessageHandler (message);
				frame.PostProcessMessage (message);
				this.Invalidate ();
			}
		}
		
		public static void UpdateEntered(Message message)
		{
			int index = Widget.entered_widgets.Count;
			
			while (index > 0)
			{
				index--;
				
				if (index < Widget.entered_widgets.Count)
				{
					Widget widget = Widget.entered_widgets[index] as Widget;
					
					Drawing.Point point_in_widget = widget.MapRootToClient (message.Cursor);
					
					if ((point_in_widget.X < 0) ||
						(point_in_widget.Y < 0) ||
						(point_in_widget.X >= widget.Client.Width) ||
						(point_in_widget.Y >= widget.Client.Height))
					{
						widget.SetEntered (false);
					}
				}
			}
		}
		
		
		public virtual bool HitTest(Drawing.Point point)
		{
			if ((point.X >= this.x1) &&
				(point.X <  this.x2) &&
				(point.Y >= this.y1) &&
				(point.Y <  this.y2))
			{
				return true;
			}
			
			return false;
		}
		
		
		public virtual Drawing.Rectangle GetPaintBounds()
		{
			return new Drawing.Rectangle (0, 0, this.client_info.width, this.client_info.height);
		}
		
		public virtual void Invalidate()
		{
			this.Invalidate (this.GetPaintBounds ());
		}
		
		public virtual void Invalidate(Drawing.Rectangle rect)
		{
			rect = this.MapClientToParent (rect);
			
			if (this.parent != null)
			{
				this.parent.Invalidate (rect);
			}
		}
		
		
		public virtual Drawing.Point MapParentToClient(Drawing.Point point)
		{
			Drawing.Point result = new Drawing.Point ();
			
			double z = this.client_info.zoom;
			
			switch (this.client_info.angle)
			{
				case 0:
					result.X = (point.X - this.x1) / z;
					result.Y = (point.Y - this.y1) / z;
					break;
				
				case 90:
					result.X = (point.Y - this.y1) / z;
					result.Y = (this.x2 - point.X) / z;
					break;
				
				case 180:
					result.X = (this.x2 - point.X) / z;
					result.Y = (this.y2 - point.Y) / z;
					break;
				
				case 270:
					result.X = (this.y2 - point.Y) / z;
					result.Y = (point.X - this.x1) / z;
					break;
				
				default:
					throw new System.ArgumentOutOfRangeException ("Invalid angle");
			}
			
			return result;
		}
		
		public virtual Drawing.Point MapClientToParent(Drawing.Point point)
		{
			Drawing.Point result = new Drawing.Point ();
			
			double z = this.client_info.zoom;
			
			switch (this.client_info.angle)
			{
				case 0:
					result.X = point.X * z + this.x1;
					result.Y = point.Y * z + this.y1;
					break;
				
				case 90:
					result.X = this.x2 - z * point.Y;
					result.Y = this.y1 + z * point.X;
					break;
				
				case 180:
					result.X = this.x2 - z * point.X;
					result.Y = this.y2 - z * point.Y;
					break;
				
				case 270:
					result.X = this.x1 + z * point.Y;
					result.Y = this.y2 - z * point.X;
					break;
				
				default:
					throw new System.ArgumentOutOfRangeException ("Invalid angle");
			}
			
			return result;
		}
		
		public virtual Drawing.Point MapRootToClient(Drawing.Point point)
		{
			Widget parent = this.Parent;
			
			//	Le plus simple est d'utiliser la récursion, afin de commencer la conversion depuis la
			//	racine, puis d'enfant en enfant jusqu'au widget final.
			
			if (parent != null)
			{
				point = parent.MapRootToClient (point);
			}
			
			return this.MapParentToClient (point);
		}
		
		public virtual Drawing.Rectangle MapRootToClient(Drawing.Rectangle rect)
		{
			Drawing.Point p1 = this.MapRootToClient (new Drawing.Point (rect.Left, rect.Bottom));
			Drawing.Point p2 = this.MapRootToClient (new Drawing.Point (rect.Right, rect.Top));
			
			rect.X = System.Math.Min (p1.X, p2.X);
			rect.Y = System.Math.Min (p1.Y, p2.Y);
			
			rect.Width  = System.Math.Abs (p1.X - p2.X);
			rect.Height = System.Math.Abs (p1.Y - p2.Y);
			
			return rect;
		}
		
		public virtual Drawing.Point MapClientToRoot(Drawing.Point point)
		{
			Widget iter = this;
			
			//	On a le choix entre une solution récursive et une solution itérative. La version
			//	itérative devrait être un petit peu plus rapide ici.
			
			while (iter != null)
			{
				point = iter.MapClientToParent (point);
				iter = iter.Parent;
			}
			
			return point;
		}
		
		public virtual Drawing.Rectangle MapClientToRoot(Drawing.Rectangle rect)
		{
			Drawing.Point p1 = this.MapClientToRoot (new Drawing.Point (rect.Left, rect.Bottom));
			Drawing.Point p2 = this.MapClientToRoot (new Drawing.Point (rect.Right, rect.Top));
			
			rect.X = System.Math.Min (p1.X, p2.X);
			rect.Y = System.Math.Min (p1.Y, p2.Y);
			
			rect.Width  = System.Math.Abs (p1.X - p2.X);
			rect.Height = System.Math.Abs (p1.Y - p2.Y);
			
			return rect;
		}
		
		
		public virtual Drawing.Rectangle MapParentToClient(Drawing.Rectangle rect)
		{
			Drawing.Point p1 = this.MapParentToClient (new Drawing.Point (rect.Left, rect.Bottom));
			Drawing.Point p2 = this.MapParentToClient (new Drawing.Point (rect.Right, rect.Top));
			
			rect.X = System.Math.Min (p1.X, p2.X);
			rect.Y = System.Math.Min (p1.Y, p2.Y);
			
			rect.Width  = System.Math.Abs (p1.X - p2.X);
			rect.Height = System.Math.Abs (p1.Y - p2.Y);
			
			return rect;
		}
		
		public virtual Drawing.Rectangle MapClientToParent(Drawing.Rectangle rect)
		{
			Drawing.Point p1 = this.MapClientToParent (new Drawing.Point (rect.Left, rect.Bottom));
			Drawing.Point p2 = this.MapClientToParent (new Drawing.Point (rect.Right, rect.Top));
			
			rect.X = System.Math.Min (p1.X, p2.X);
			rect.Y = System.Math.Min (p1.Y, p2.Y);
			
			rect.Width  = System.Math.Abs (p1.X - p2.X);
			rect.Height = System.Math.Abs (p1.Y - p2.Y);
			
			return rect;
		}

		
		public virtual Epsitec.Common.Drawing.Transform GetRootToClientTransform()
		{
			Widget iter = this;
			
			Drawing.Transform full_transform = new Drawing.Transform ();
			
			while (iter != null)
			{
				Drawing.Transform local_transform = iter.GetTransformToClient ();
				
				//	Les transformations de la racine au client doivent s'appliquer en commençant par
				//	la racine. Comme nous remontons la hiérarchie des widgets en sens inverse, il nous
				//	suffit d'utiliser la multiplication post-fixe pour arriver au même résultat :
				//
				//	 T = Tn * ... * T2 * T1 * T0, P' = T * P
				//
				//	avec Ti la transformation pour le widget 'i', où i=0 correspond à la racine,
				//	P le point en coordonnées racine et P' le point en coordonnées client.
				
				full_transform.MultiplyByPostfix (local_transform);
				iter = iter.Parent;
			}
			
			return full_transform;
		}
		
		public virtual Epsitec.Common.Drawing.Transform GetClientToRootTransform()
		{
			Widget iter = this;
			
			Drawing.Transform full_transform = new Drawing.Transform ();
			
			while (iter != null)
			{
				Drawing.Transform local_transform = iter.GetTransformToParent ();
				
				//	Les transformations du client à la racine doivent s'appliquer en commençant par
				//	le client. Comme nous remontons la hiérarchie des widgets dans ce sens là, il nous
				//	suffit d'utiliser la multiplication normale pour arriver à ce résultat :
				//
				//	 T = T0 * T1 * T2 * ... * Tn, P' = T * P
				//
				//	avec Ti la transformation pour le widget 'i', où i=0 correspond à la racine.
				//	P le point en coordonnées client et P' le point en coordonnées racine.
				
				full_transform.MultiplyBy (local_transform);
				iter = iter.Parent;
			}
			
			return full_transform;
		}
		
		
		public virtual Epsitec.Common.Drawing.Transform GetTransformToClient()
		{
			Epsitec.Common.Drawing.Transform t = new Epsitec.Common.Drawing.Transform ();
			
			double ox, oy;
			
			switch (this.client_info.angle)
			{
				case 0:		ox = this.x1; oy = this.y1; break;
				case 90:	ox = this.x2; oy = this.y1; break;
				case 180:	ox = this.x2; oy = this.y2; break;
				case 270:	ox = this.x1; oy = this.y2; break;
				default:	throw new System.ArgumentOutOfRangeException ("Invalid angle");
			}
			
			t.Translate (-ox, -oy);
			t.Rotate (-this.client_info.angle);
			t.Scale (1 / this.client_info.zoom);
			t.Round ();
			
			return t;
		}
		
		public virtual Epsitec.Common.Drawing.Transform GetTransformToParent()
		{
			Epsitec.Common.Drawing.Transform t = new Epsitec.Common.Drawing.Transform ();
			
			double ox, oy;
			
			switch (this.client_info.angle)
			{
				case 0:		ox = this.x1; oy = this.y1; break;
				case 90:	ox = this.x2; oy = this.y1; break;
				case 180:	ox = this.x2; oy = this.y2; break;
				case 270:	ox = this.x1; oy = this.y2; break;
				default:	throw new System.ArgumentOutOfRangeException ("Invalid angle");
			}
			
			t.Scale (this.client_info.zoom);
			t.Rotate (this.client_info.angle);
			t.Translate (ox, oy);
			t.Round ();
			
			return t;
		}
		
		
		public Widget FindChild(Drawing.Point point)
		{
			return this.FindChild (point, ChildFindMode.SkipHidden);
		}
		
		public virtual Widget FindChild(Drawing.Point point, ChildFindMode mode)
		{
			if (this.Children.Count == 0)
			{
				return null;
			}
			
			Widget[] children = this.Children.Widgets;
			int  children_num = children.Length;
			
			for (int i = 0; i < children_num; i++)
			{
				Widget widget = children[children_num-1 - i];
				
				System.Diagnostics.Debug.Assert (widget != null);
				
				if (mode != ChildFindMode.All)
				{
					if ((mode & ChildFindMode.SkipDisabled) != 0)
					{
						if (widget.IsEnabled == false)
						{
							continue;
						}
					}
					else if ((mode & ChildFindMode.SkipHidden) != 0)
					{
						if (widget.IsVisible == false)
						{
							continue;
						}
					}
				}
				
				if (widget.HitTest (point))
				{
					if ((mode & ChildFindMode.SkipTransparent) != 0)
					{
						//	TODO: vérifier que le point en question n'est pas transparent
					}
					
					return widget;
				}
			}
			
			return null;
		}
		
		
		protected virtual void SetBounds(double x1, double y1, double x2, double y2)
		{
			if ((x1 == this.x1) && (y1 == this.y1) && (x2 == this.x2) && (y2 == this.y2))
			{
				return;
			}
			
			this.Invalidate ();
			
			this.x1 = x1;
			this.y1 = y1;
			this.x2 = x2;
			this.y2 = y2;
			
			this.Invalidate ();
			this.UpdateClientGeometry ();
			this.UpdateLayoutSize ();
		}
		
		
		protected virtual void UpdateLayoutSize()
		{
			if (this.text_layout != null)
			{
				this.text_layout.Alignment  = this.Alignment;
				this.text_layout.LayoutSize = this.Client.Size;
			}
		}
		
		
		
		protected virtual void UpdateClientGeometry()
		{
			System.Diagnostics.Debug.Assert (this.layout_info == null);
			
			this.layout_info = new LayoutInfo (this.client_info.width, this.client_info.height);
			
			try
			{
				double zoom = this.client_info.zoom;
				
				double dx = (this.x2 - this.x1) / zoom;
				double dy = (this.y2 - this.y1) / zoom;
				
				switch (this.client_info.angle)
				{
					case 0:
					case 180:
						this.client_info.SetSize (dx, dy);
						break;
					
					case 90:
					case 270:
						this.client_info.SetSize (dy, dx);
						break;
					
					default:
						double angle = this.client_info.angle * System.Math.PI / 180.0;
						double cos = System.Math.Cos (angle);
						double sin = System.Math.Sin (angle);
						this.client_info.SetSize (cos*cos*dx + sin*sin*dy, sin*sin*dx + cos*cos*dy);
						break;
				}
				
				this.UpdateChildrenLayout ();
				this.OnLayoutChanged ();
			}
			finally
			{
				this.layout_info = null;
			}
		}
		
		protected virtual void UpdateChildrenLayout()
		{
			System.Diagnostics.Debug.Assert (this.client_info != null);
			System.Diagnostics.Debug.Assert (this.layout_info != null);
			
			if (this.HasChildren)
			{
				double width_diff  = this.client_info.width  - this.layout_info.OriginalWidth;
				double height_diff = this.client_info.height - this.layout_info.OriginalHeight;
				
				foreach (Widget child in this.children)
				{
					AnchorStyles anchor_x = (AnchorStyles) child.Anchor & AnchorStyles.LeftAndRight;
					AnchorStyles anchor_y = (AnchorStyles) child.Anchor & AnchorStyles.TopAndBottom;
					
					double x1 = child.x1;
					double x2 = child.x2;
					double y1 = child.y1;
					double y2 = child.y2;
					
					switch (anchor_x)
					{
						case AnchorStyles.Left:							//	[x1] fixe à gauche
							break;
						case AnchorStyles.Right:						//	[x2] fixe à droite
							x1 += width_diff;
							x2 += width_diff;
							break;
						case AnchorStyles.None:							//	[x1] et [x2] mobiles (centré)
							x1 += width_diff / 2.0f;
							x2 += width_diff / 2.0f;
							break;
						case AnchorStyles.LeftAndRight:					//	[x1] fixe à gauche, [x2] fixe à droite
							x2 += width_diff;
							break;
					}
					
					switch (anchor_y)
					{
						case AnchorStyles.Bottom:						//	[y1] fixe en bas
							break;
						case AnchorStyles.Top:							//	[y2] fixe en haut
							y1 += height_diff;
							y2 += height_diff;
							break;
						case AnchorStyles.None:							//	[y1] et [y2] mobiles (centré)
							y1 += height_diff / 2.0f;
							y2 += height_diff / 2.0f;
							break;
						case AnchorStyles.TopAndBottom:					//	[y1] fixe en bas, [y2] fixe en haut
							y2 += height_diff;
							break;
					}
					
					child.SetBounds (x1, y1, x2, y2);
				}
			}
		}
		
		
		public virtual void PaintHandler(Drawing.Graphics graphics, Drawing.Rectangle repaint)
		{
			if (this.PaintCheckClipping (repaint))
			{
				Drawing.Rectangle bounds = this.GetPaintBounds ();
				
				bounds  = this.MapClientToRoot (bounds);
				repaint = this.MapParentToClient (repaint);
				
				Drawing.Rectangle original_clipping  = graphics.SaveClippingRectangle ();
				Drawing.Transform original_transform = graphics.SaveTransform ();
				Drawing.Transform graphics_transform = this.GetTransformToParent ();
				
				graphics.SetClippingRectangle (bounds);
				graphics_transform.MultiplyBy (original_transform);
				
				graphics.Transform = graphics_transform;
			
				try
				{
					PaintEventArgs local_paint_args = new PaintEventArgs (graphics, repaint);
					
					//	Peint l'arrière-plan du widget. En principe, tout va dans l'arrière plan, sauf
					//	si l'on désire réaliser des effets de transparence par dessus le dessin des
					//	widgets enfants.
					
					this.OnPaintBackground (local_paint_args);
					
					//	Peint tous les widgets enfants, en commençant par le numéro 0, lequel se trouve
					//	derrière tous les autres, etc. On saute les widgets qui ne sont pas visibles.
					
					if (this.Children.Count > 0)
					{
						Widget[] children = this.Children.Widgets;
						int  children_num = children.Length;
						
						for (int i = 0; i < children_num; i++)
						{
							Widget widget = children[i];
						
							System.Diagnostics.Debug.Assert (widget != null);
						
							if (widget.IsVisible)
							{
								widget.PaintHandler (graphics, repaint);
							}
						}
					}
				
					//	Peint l'avant-plan du widget, à n'utiliser que pour faire un "effet" spécial
					//	après coup.
					
					this.OnPaintForeground (local_paint_args);
				}
				finally
				{
					graphics.Transform = original_transform;
					graphics.RestoreClippingRectangle (original_clipping);
				}
			}
		}
		
		protected virtual void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clip_rect)
		{
			//	Implémenter le dessin du fond dans cette méthode.
		}
		
		protected virtual void PaintForegroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clip_rect)
		{
			//	Implémenter le dessin des enjoliveurs additionnels dans cette méthode.
		}
		
		protected virtual bool PaintCheckClipping(Drawing.Rectangle repaint)
		{
			Drawing.Rectangle bounds = this.GetPaintBounds ();
			bounds = this.MapClientToParent (bounds);
			return repaint.IntersectsWithAligned (bounds);
		}
		
		
		public void MessageHandler(Message message)
		{
			Drawing.Point point = message.Cursor;
			
			point = this.MapRootToClient (point);
			point = this.MapClientToParent (point);
			
			this.MessageHandler (message, point);
		}
		
		public virtual void MessageHandler(Message message, Drawing.Point pos)
		{
			Drawing.Point client_pos = this.MapParentToClient (pos);
			
			this.PreProcessMessage (message, client_pos);
			
			//	En premier lieu, si le message peut être transmis aux descendants de ce widget, passe
			//	en revue ceux-ci dans l'ordre inverse de leur affichage (commence par le widget qui est
			//	visuellement au sommet).
			
			if ((message.FilterNoChildren == false) &&
				(message.Handled == false) &&
				(this.Children.Count > 0))
			{
				Widget[] children = this.Children.Widgets;
				int  children_num = children.Length;
				
				for (int i = 0; i < children_num; i++)
				{
					Widget widget = children[children_num-1 - i];
					
					if ((widget.IsVisible) &&
						(widget.IsFrozen == false) &&
						((message.FilterOnlyFocused == false) || (widget.ContainsFocus)) &&
						((message.FilterOnlyOnHit == false) || (widget.HitTest (client_pos))))
					{
						if (widget.IsEnabled)
						{
							if (message.IsMouseType)
							{
								//	C'est un message souris. Vérifions d'abord si le widget contenait déjà
								//	la souris auparavant.
								
								if (widget.IsEntered == false)
								{
									widget.SetEntered (true);
								}
							}
							
							widget.MessageHandler (message, client_pos);
							
							if (message.Handled)
							{
								break;
							}
						}
						else
						{
							break;
						}
					}
				}
			}
			else if ((message.Handled == false) &&
				/**/ (message.Captured) &&
				/**/ (message.IsMouseType))
			{
				if ((this.IsEntered == false) &&
					(message.InWidget == this))
				{
					this.SetEntered (true);
				}
			}
			
			if (message.Handled == false)
			{
				this.DispatchMessage (message, client_pos);
			}
			
			this.PostProcessMessage (message, client_pos);
		}
		
		protected virtual void DispatchMessage(Message message, Drawing.Point pos)
		{
			switch (message.Type)
			{
				case MessageType.MouseUp:
					//	Le bouton a été relâché. Ceci génère l'événement 'Released' pour signaler
					//	ce relâchement, mais aussi un événement 'Clicked' ou 'DoubleClicked' en
					//	fonction du nombre de clics.
					
					this.OnReleased (new MessageEventArgs (message, pos));
					
					switch (message.ButtonDownCount)
					{
						case 1:	this.OnClicked (new MessageEventArgs (message, pos));		break;
						case 2:	this.OnDoubleClicked (new MessageEventArgs (message, pos));	break;
					}
					break;
				
				case MessageType.MouseDown:
					this.OnPressed (new MessageEventArgs (message, pos));
					break;
			}
			
			
			this.ProcessMessage (message, pos);
		}
		
		protected virtual void PreProcessMessage(Message message, Drawing.Point pos)
		{
			//	...appelé avant que l'événement ne soit traité...
		}
		
		protected virtual void ProcessMessage(Message message, Drawing.Point pos)
		{
			//	...appelé pour traiter l'événement...
		}
		
		protected virtual void PostProcessMessage(Message message, Drawing.Point pos)
		{
			//	...appelé après que l'événement ait été traité...
		}
		
		
		public virtual bool ShortcutHandler(Shortcut shortcut)
		{
			return this.ShortcutHandler (shortcut, true);
		}
		
		protected virtual bool ShortcutHandler(Shortcut shortcut, bool execute_focused)
		{
			Widget[] children = this.Children.Widgets;
			int  children_num = children.Length;
			
			if (execute_focused)
			{
				for (int i = 0; i < children_num; i++)
				{
					Widget widget = children[children_num-1 - i];
				
					if (widget.ContainsFocus)
					{
						if (widget.ShortcutHandler (shortcut))
						{
							return true;
						}
					}
				}
			}
			
			if (this.ProcessShortcut (shortcut))
			{
				return true;
			}
			
			for (int i = 0; i < children_num; i++)
			{
				Widget widget = children[children_num-1 - i];
				
				if ((widget.IsEnabled) &&
					(widget.IsFrozen == false) &&
					(widget.IsVisible))
				{
					if (widget.ShortcutHandler (shortcut, false))
					{
						return true;
					}
				}
			}
			
			return false;
		}
		
		protected virtual bool ProcessShortcut(Shortcut shortcut)
		{
			if ((this.shortcut != null) &&
				(this.shortcut.Match (shortcut)))
			{
				this.OnShortcutPressed ();
				return true;
			}
			
			return false;
		}
		
		
		protected virtual void CreateWidgetCollection()
		{
			this.children = new WidgetCollection (this);
		}
		
		protected virtual void CreateTextLayout()
		{
			if (this.text_layout == null)
			{
				this.text_layout = new TextLayout ();
				
				this.text_layout.Font     = this.DefaultFont;
				this.text_layout.FontSize = this.DefaultFontSize;
				
				this.UpdateLayoutSize ();
			}
		}
		
		protected virtual void DisposeTextLayout()
		{
			if (this.text_layout != null)
			{
				this.text_layout = null;
			}
		}
		
		
		protected virtual void OnPaintBackground(PaintEventArgs e)
		{
			if (this.PaintBackground != null)
			{
				e.Suppress = false;
				
				this.PaintBackground (this, e);
				
				if (e.Suppress)
				{
					return;
				}
			}
			
			this.PaintBackgroundImplementation (e.Graphics, e.ClipRectangle);
		}
		
		protected virtual void OnPaintForeground(PaintEventArgs e)
		{
			if (this.PaintForeground != null)
			{
				e.Suppress = false;
				
				this.PaintForeground (this, e);
				
				if (e.Suppress)
				{
					return;
				}
			}
			
			this.PaintForegroundImplementation (e.Graphics, e.ClipRectangle);
		}
		
		protected virtual void OnChildrenChanged()
		{
			this.Invalidate ();
			
			if (this.ChildrenChanged != null)
			{
				this.ChildrenChanged (this);
			}
		}
		
		protected virtual void OnLayoutChanged()
		{
			if (this.LayoutChanged != null)
			{
				this.LayoutChanged (this);
			}
		}
		
		
		protected virtual void OnPressed(MessageEventArgs e)
		{
			if (this.Pressed != null)
			{
				if (e != null)
				{
					e.Message.Consumer = this;
				}
				
				this.Pressed (this, e);
			}
		}
		
		protected virtual void OnReleased(MessageEventArgs e)
		{
			if (this.Released != null)
			{
				if (e != null)
				{
					e.Message.Consumer = this;
				}
				
				this.Released (this, e);
			}
		}
		
		protected virtual void OnClicked(MessageEventArgs e)
		{
			if (this.Clicked != null)
			{
				if (e != null)
				{
					e.Message.Consumer = this;
				}
				
				this.Clicked (this, e);
			}
		}
		
		protected virtual void OnDoubleClicked(MessageEventArgs e)
		{
			if (this.DoubleClicked != null)
			{
				e.Message.Consumer = this;
				this.DoubleClicked (this, e);
			}
		}
		
		protected virtual void OnShortcutPressed()
		{
			if (this.ShortcutPressed != null)
			{
				this.ShortcutPressed (this);
			}
		}
		
		
		protected virtual void OnFocused()
		{
			if (this.Focused != null)
			{
				this.Focused (this);
			}
		}
		
		protected virtual void OnDefocused()
		{
			if (this.Defocused != null)
			{
				this.Defocused (this);
			}
		}
		
		protected virtual void OnSelected()
		{
			if (this.Selected != null)
			{
				this.Selected (this);
			}
		}
		
		protected virtual void OnDeselected()
		{
			if (this.Deselected != null)
			{
				this.Deselected (this);
			}
		}
		
		protected virtual void OnEngaged()
		{
			if (this.Engaged != null)
			{
				this.Engaged (this);
			}
		}
		
		protected virtual void OnDisengaged()
		{
			if (this.Disengaged != null)
			{
				this.Disengaged (this);
			}
		}
		
		protected virtual void OnStillEngaged()
		{
			if (this.StillEngaged != null)
			{
				this.StillEngaged (this);
			}
		}
		
		protected virtual void OnActiveStateChanged()
		{
			if (this.ActiveStateChanged != null)
			{
				this.ActiveStateChanged (this);
			}
		}
		
		
		[System.Flags] protected enum InternalState
		{
			None				= 0,
			ChildrenChanged		= 0x00000001,
			
			Focusable			= 0x00000010,
			Selectable			= 0x00000020,
			Engageable			= 0x00000040,		//	=> peut être enfoncé par une pression
			Frozen				= 0x00000080,		//	=> n'accepte aucun événement
			Visible				= 0x00000100,
			AcceptThreeState	= 0x00000200,
			
			AutoCapture			= 0x00010000,
			AutoFocus			= 0x00020000,
			AutoEngage			= 0x00040000,
			AutoToggle			= 0x00080000,
			AutoMnemonic		= 0x00100000,
			AutoRepeatEngaged	= 0x00200000,
		}
		
		[System.Flags] public enum TabNavigationMode
		{
			Passive				= 0,
			
			ActivateOnTab		= 0x00000001,
			ActivateOnCursorX	= 0x00000002,
			ActivateOnCursorY	= 0x00000004,
			ActivateOnCursor	= ActivateOnCursorX + ActivateOnCursorY,
			ActivateOnPage		= 0x00000008,
		}
		
		[System.Flags] public enum ChildFindMode
		{
			All				= 0,
			SkipHidden		= 1,
			SkipDisabled	= 2,
			SkipTransparent	= 4
		}
		
		
		public class ClientInfo
		{
			internal ClientInfo()
			{
			}
			
			internal void SetSize(double width, double height)
			{
				this.width  = width;
				this.height = height;
			}
			
			internal void SetAngle(int angle)
			{
				angle = angle % 360;
				this.angle = (angle < 0) ? (short) (angle + 360) : (short) (angle);
			}
			
			internal void SetZoom(double zoom)
			{
				System.Diagnostics.Debug.Assert (zoom > 0.0f);
				this.zoom = zoom;
			}
			
			public double					Width
			{
				get { return this.width; }
			}
			
			public double					Height
			{
				get { return this.height; }
			}
			
			public Drawing.Size				Size
			{
				get { return new Drawing.Size (this.width, this.height); }
			}
			
			public Drawing.Rectangle		Bounds
			{
				get { return new Drawing.Rectangle (0, 0, this.width, this.height); }
			}
			
			public int						Angle
			{
				get { return this.angle; }
			}
			
			public double					Zoom
			{
				get { return this.zoom; }
			}
			
			internal double					width	= 0.0;
			internal double					height	= 0.0;
			internal short					angle	= 0;
			internal double					zoom	= 1.0;
		}
		
		public class WidgetCollection : System.Collections.IList
		{
			public WidgetCollection(Widget widget)
			{
				this.list   = new System.Collections.ArrayList ();
				this.widget = widget;
			}
			
			
			public Widget[]					Widgets
			{
				get
				{
					if (this.array == null)
					{
						this.array = new Widget[this.list.Count];
						this.list.CopyTo (this.array);
					}
					
					return this.array;
				}
			}
			
			private void PreInsert(object widget)
			{
				if (widget is Widget)
				{
					this.PreInsert (widget as Widget);
				}
				else
				{
					throw new System.ArgumentException ("Widget");
				}
			}
			
			private void PreInsert(Widget widget)
			{
				if (widget.parent != null)
				{
					Widget parent = widget.parent;
					parent.Children.Remove (widget);
					System.Diagnostics.Debug.Assert (widget.parent == null);
				}
				widget.parent = this.widget;
			}
			
			private void PostInsert(object widget)
			{
				if (this.widget.suspend_counter == 0)
				{
					this.widget.OnChildrenChanged ();
				}
				else
				{
					this.widget.internal_state |= InternalState.ChildrenChanged;
				}
			}
			
			private void PreRemove(object widget)
			{
				if (widget is Widget)
				{
					this.PreRemove (widget as Widget);
				}
				else
				{
					throw new System.ArgumentException ("Widget");
				}
			}
			
			private void PreRemove(Widget widget)
			{
				System.Diagnostics.Debug.Assert (widget.parent == this.widget);
				widget.SetFocused (false);
				widget.SetEngaged (false);
				widget.SetEntered (false);
				widget.parent = null;
			}
			
			private void NotifyChanged()
			{
				if (this.widget.suspend_counter == 0)
				{
					this.widget.OnChildrenChanged ();
				}
				else
				{
					this.widget.internal_state |= InternalState.ChildrenChanged;
				}
			}
			
			
			
			#region IList Members
			
			public bool IsReadOnly
			{
				get	{ return false; }
			}
			
			public object this[int index]
			{
				get	{ return this.list[index]; }
				set	{ throw new System.NotSupportedException ("Widget"); }
			}
			
			public void RemoveAt(int index)
			{
				System.Diagnostics.Debug.Assert (this.list[index] != null);
				this.PreRemove (this.list[index]);
				this.list.RemoveAt (index);
				this.array = null;
				this.NotifyChanged ();
			}
			
			public void Insert(int index, object value)
			{
				throw new System.NotSupportedException ("Widget");
			}
			
			public void Remove(object value)
			{
				this.PreRemove (value);
				this.list.Remove (value);
				this.array = null;
				this.NotifyChanged ();
			}
			
			public bool Contains(object value)
			{
				return this.list.Contains (value);
			}
			
			public void Clear()
			{
				while (this.Count > 0)
				{
					this.RemoveAt (this.Count - 1);
				}
			}
			
			public int IndexOf(object value)
			{
				return this.list.IndexOf (value);
			}
			
			public int Add(object value)
			{
				this.PreInsert (value);
				int result = this.list.Add (value);
				this.array = null;
				this.NotifyChanged ();
				return result;
			}
			
			public bool IsFixedSize
			{
				get	{ return false; }
			}

			#endregion

			#region ICollection Members

			public bool IsSynchronized
			{
				get { return false; }
			}
			
			public int Count
			{
				get	{ return this.list.Count; }
			}

			public void CopyTo(System.Array array, int index)
			{
				this.list.CopyTo (array, index);
			}
			
			public object SyncRoot
			{
				get { return this.list.SyncRoot; }
			}

			#endregion

			#region IEnumerable Members

			public System.Collections.IEnumerator GetEnumerator()
			{
				return this.list.GetEnumerator ();
			}
			
			#endregion
			
			System.Collections.ArrayList	list;
			Widget[]						array;
			Widget							widget;
		}
		
		
		protected class LayoutInfo
		{
			internal LayoutInfo(double width, double height)
			{
				this.width  = width;
				this.height = height;
			}
			
			public double					OriginalWidth
			{
				get { return this.width; }
			}
			
			public double					OriginalHeight
			{
				get { return this.height; }
			}
			
			private double					width, height;
		}
		
		
		
		protected AnchorStyles				anchor;
		protected Drawing.Color				back_color;
		protected Drawing.Color				fore_color;
		protected double					x1, y1, x2, y2;
		protected ClientInfo				client_info = new ClientInfo ();
		
		protected WidgetCollection			children;
		protected Widget					parent;
		protected string					name;
		protected TextLayout				text_layout;
		protected ContentAlignment			alignment;
		protected LayoutInfo				layout_info;
		protected InternalState				internal_state;
		protected WidgetState				widget_state;
		protected int						suspend_counter;
		protected int						tab_index;
		protected TabNavigationMode			tab_navigation_mode;
		protected Shortcut					shortcut;
		
		static System.Collections.ArrayList	entered_widgets = new System.Collections.ArrayList ();
	}
}
