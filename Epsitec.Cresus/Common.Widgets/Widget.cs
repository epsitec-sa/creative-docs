//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 09/10/2003

namespace Epsitec.Common.Widgets
{
	using ContentAlignment = Drawing.ContentAlignment;
	using BundleAttribute  = Epsitec.Common.Support.BundleAttribute;
	
	
	public delegate bool WalkWidgetCallback(Widget widget);
	
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
	
	public enum DockStyle
	{
		None			= 0,
		Top				= 1,
		Bottom			= 2,
		Left			= 3,
		Right			= 4,
		Fill			= 5
	}
	
	
	/// <summary>
	/// La classe Widget implémente la classe de base dont dérivent tous les
	/// widgets de l'interface graphique ("controls" dans l'appellation Windows).
	/// </summary>
	public class Widget : System.IDisposable, Epsitec.Common.Support.IBundleSupport
	{
		public Widget()
		{
			this.internalState |= InternalState.Visible;
			this.internalState |= InternalState.AutoCapture;
			this.internalState |= InternalState.AutoMnemonic;
			
			this.widgetState |= WidgetState.Enabled;
			
			this.defaultFontHeight = System.Math.Floor(this.DefaultFont.LineHeight*this.DefaultFontSize);
			this.alignment = this.DefaultAlignment;
			this.anchor    = this.DefaultAnchor;
			this.Size      = new Drawing.Size (this.DefaultWidth, this.DefaultHeight);
			
			this.backColor = Drawing.Color.FromName ("Control");
			this.foreColor = Drawing.Color.FromName ("ControlText");
			
			this.minSize = this.DefaultMinSize;
			this.maxSize = this.DefaultMaxSize;
			
			Widget.aliveWidgets.Add (new System.WeakReference (this));
		}
		
		
		#region Interface IDisposable
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		#region Interface IBundleSupport
		public virtual string				PublicClassName
		{
			get { return this.GetType ().Name; }
		}
		
		public virtual void RestoreFromBundle(Epsitec.Common.Support.ObjectBundler bundler, Epsitec.Common.Support.ResourceBundle bundle)
		{
//			this.SuspendLayout ();
			
			//	L'ObjectBundler sait initialiser la plupart des propriétés simples (celles
			//	qui sont marquées par l'attribut [Bundle]), mais il ne sait pas comment
			//	restitue les enfants du widget :
			
			System.Collections.IList widget_list = bundle.GetFieldBundleList ("widgets");
			
			if (widget_list != null)
			{
				//	Notre bundle contient une liste de sous-bundles contenant les descriptions des
				//	widgets enfants. On les restitue nous-même et on les ajoute dans la liste des
				//	enfants.
				
				foreach (Support.ResourceBundle widget_bundle in widget_list)
				{
					Widget widget = bundler.CreateFromBundle (widget_bundle) as Widget;
					
					this.Children.Add (widget);
				}
			}
			
//			this.ResumeLayout ();
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.HasChildren)
				{
					Widget[] widgets = this.children.Widgets;
					
					for (int i = 0; i < widgets.Length; i++)
					{
						widgets[i].Dispose ();
						System.Diagnostics.Debug.Assert (widgets[i].parent == null);
					}
					
					System.Diagnostics.Debug.Assert (this.children.Count == 0);
				}
				
				this.Parent = null;
			}
		}
		
		
		public static int					DebugAliveWidgetsCount
		{
			get
			{
				System.Collections.ArrayList alive = new System.Collections.ArrayList ();
				
				foreach (System.WeakReference weak_ref in Widget.aliveWidgets)
				{
					if (weak_ref.IsAlive)
					{
						alive.Add (weak_ref);
					}
				}
				
				Widget.aliveWidgets = alive;
				return alive.Count;
			}
		}
		
		public static Widget[]				DebugAliveWidgets
		{
			get
			{
				Widget[] widgets = new Widget[Widget.DebugAliveWidgetsCount];
				
				int i = 0;
				
				foreach (System.WeakReference weak_ref in Widget.aliveWidgets)
				{
					if (weak_ref.IsAlive)
					{
						widgets[i++] = weak_ref.Target as Widget;
					}
				}
				
				return widgets;
			}
		}

		
		[Bundle ("anchor")]	public AnchorStyles		Anchor
		{
			get { return this.anchor; }
			set { this.anchor = value; }
		}
		
		[Bundle ("dock")]	public DockStyle		Dock
		{
			get{ return this.dock; }
			set
			{
				if (this.dock != value)
				{
					this.dock = value;
					
					if (this.parent != null)
					{
						this.parent.HandleChildrenChanged ();
						this.parent.UpdateChildrenLayout ();
					}
				}
			}
		}
		
		[Bundle ("dock_m")] public Drawing.Margins	DockMargins
		{
			get { return this.dockMargins; }
			set
			{
				if (this.dockMargins != value)
				{
					this.dockMargins = value;
					if (this.parent != null)
					{
						this.parent.UpdateChildrenLayout ();
					}
				}
			}
		}
		
		[Bundle ("dock_h")]	public bool				PreferHorizontalDockLayout
		{
			get { return (this.internalState & InternalState.PreferXLayout) != 0; }
			set
			{
				if (value != this.PreferHorizontalDockLayout)
				{
					if (value)
					{
						this.internalState |= InternalState.PreferXLayout;
					}
					else
					{
						this.internalState &= ~ InternalState.PreferXLayout;
					}
					
					this.UpdateDockedChildrenLayout ();
				}
			}
		}
		
		
		public MouseCursor					MouseCursor
		{
			get { return this.mouseCursor == null ? MouseCursor.Default : this.mouseCursor; }
			set { this.mouseCursor = value; }
		}
		
		public Drawing.Color				BackColor
		{
			get { return this.backColor; }
			set
			{
				if (this.backColor != value)
				{
					this.backColor = value;
					this.Invalidate ();
				}
			}
		}
		
		public Drawing.Color				ForeColor
		{
			get { return this.foreColor; }
			set
			{
				if (this.foreColor != value)
				{
					this.foreColor = value;
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
		
		
		[Bundle ("pos")]	public Drawing.Point	Location
		{
			get { return new Drawing.Point (this.x1, this.y1); }
			set { this.SetBounds (value.X, value.Y, value.X + this.x2 - this.x1, value.Y + this.y2 - this.y1); }
		}
		
		[Bundle ("size")]	public Drawing.Size		Size
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
			get { return this.clientInfo; }
		}
		
		public virtual Drawing.Size			MinSize
		{
			get
			{
				return this.minSize;
			}
			set
			{
				if (this.minSize != value)
				{
					this.minSize = value;
					this.OnMinSizeChanged ();
				}
			}
		}
		
		public virtual Drawing.Size			MaxSize
		{
			get
			{
				return this.maxSize;
			}
			set
			{
				if (this.maxSize != value)
				{
					this.maxSize = value;
					this.OnMaxSizeChanged ();
				}
			}
		}
		
		
		public void SuspendLayout()
		{
			lock (this)
			{
				this.suspendCounter++;
			}
		}
		
		public void ResumeLayout()
		{
			lock (this)
			{
				if (this.suspendCounter > 0)
				{
					this.suspendCounter--;
				}
				
				if (this.suspendCounter == 0)
				{
					if (this.layoutInfo != null)
					{
						this.UpdateChildrenLayout ();
						this.OnLayoutChanged ();
						this.layoutInfo = null;
					}
					if ((this.internalState & InternalState.ChildrenChanged) != 0)
					{
						this.internalState -= InternalState.ChildrenChanged;
						this.HandleChildrenChanged ();
					}
				}
			}
		}
		
		protected virtual void HandleChildrenChanged()
		{
			this.internalState &= ~InternalState.ChildrenDocked;
			
			foreach (Widget child in this.Children)
			{
				if (child.Dock != DockStyle.None)
				{
					this.internalState |= InternalState.ChildrenDocked;
					break;
				}
			}
			
			if ((this.internalState & InternalState.ChildrenDocked) != 0)
			{
				this.UpdateDockedChildrenLayout ();
			}
			
			this.Invalidate ();
			this.OnChildrenChanged ();
		}
		
		public void SetClientAngle(int angle)
		{
			this.clientInfo.SetAngle (angle);
			this.UpdateClientGeometry ();
		}
		
		public void SetClientZoom(double zoom)
		{
			this.clientInfo.SetZoom (zoom);
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
		public virtual double				DefaultFontHeight
		{
			get { return this.defaultFontHeight; }
		}
		public virtual Drawing.Size			DefaultMinSize
		{
			get { return new Drawing.Size (4, 4); }
		}
		
		public virtual Drawing.Size			DefaultMaxSize
		{
			get { return new Drawing.Size (1000000, 1000000); }
		}
		
		
#if false
		public bool							CausesValidation
		{
			get;
			set;
		}
#endif
		public bool							DebugActive
		{
			get { return (this.internalState & InternalState.DebugActive) != 0; }
			set
			{
				if (value)
				{
					this.internalState |= InternalState.DebugActive;
				}
				else
				{
					this.internalState &= ~ InternalState.DebugActive;
				}
			}
		}
		
		
		[Bundle ("cmd")] public bool		IsCommand
		{
			get { return (this.internalState & InternalState.Command) != 0; }
			set
			{
				if (value)
				{
					this.internalState |= InternalState.Command;
				}
				else
				{
					this.internalState &= ~ InternalState.Command;
				}
			}
		}
		
		
		public virtual bool					IsEnabled
		{
			get
			{
				if ((this.widgetState & WidgetState.Enabled) == 0)
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
				if ((this.internalState & InternalState.Frozen) != 0)
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
				if (((this.internalState & InternalState.Visible) == 0) ||
					(this.parent == null))
				{
					return false;
				}
				
				return this.parent.IsVisible;
			}
		}

		public bool							IsFocused
		{
			get { return (this.widgetState & WidgetState.Focused) != 0; }
		}
		
		public bool							IsEntered
		{
			get { return (this.widgetState & WidgetState.Entered) != 0; }
		}
		
		public bool							IsSelected
		{
			get { return (this.widgetState & WidgetState.Selected) != 0; }
		}
		
		public bool							IsEngaged
		{
			get { return (this.widgetState & WidgetState.Engaged) != 0; }
		}
		
		public bool							IsError
		{
			get { return (this.widgetState & WidgetState.Error) != 0; }
			set
			{
				if (this.IsError != value)
				{
					if (value)
					{
						this.widgetState |= WidgetState.Error;
					}
					else
					{
						this.widgetState &= ~WidgetState.Error;
					}
					
					this.Invalidate ();
				}
			}
		}
		
		public bool							AutoCapture
		{
			get { return (this.internalState & InternalState.AutoCapture) != 0; }
			set
			{
				if (value)
				{
					this.internalState |= InternalState.AutoCapture;
				}
				else
				{
					this.internalState &= ~InternalState.AutoCapture;
				}
			}
		}
		
		public bool							AutoFocus
		{
			get { return (this.internalState & InternalState.AutoFocus) != 0; }
			set
			{
				if (value)
				{
					this.internalState |= InternalState.AutoFocus;
				}
				else
				{
					this.internalState &= ~InternalState.AutoFocus;
				}
			}
		}
		
		public bool							AutoEngage
		{
			get { return (this.internalState & InternalState.AutoEngage) != 0; }
			set
			{
				if (value)
				{
					this.internalState |= InternalState.AutoEngage;
				}
				else
				{
					this.internalState &= ~InternalState.AutoEngage;
				}
			}
		}
		
		public bool							AutoRepeatEngaged
		{
			get { return (this.internalState & InternalState.AutoRepeatEngaged) != 0; }
			set
			{
				if (value)
				{
					this.internalState |= InternalState.AutoRepeatEngaged;
				}
				else
				{
					this.internalState &= ~InternalState.AutoRepeatEngaged;
				}
			}
		}
		
		public bool							AutoToggle
		{
			get { return (this.internalState & InternalState.AutoToggle) != 0; }
			set
			{
				if (value)
				{
					this.internalState |= InternalState.AutoToggle;
				}
				else
				{
					this.internalState &= InternalState.AutoToggle;
				}
			}
		}
		
		public bool							AutoMnemonic
		{
			get { return (this.internalState & InternalState.AutoMnemonic) != 0; }
		}
		
		
		public virtual WidgetState			State
		{
			get { return this.widgetState; }
		}
		
		public virtual WidgetState			ActiveState
		{
			get { return this.widgetState & WidgetState.ActiveMask; }
			set
			{
				WidgetState active = this.widgetState & WidgetState.ActiveMask;
				
				System.Diagnostics.Debug.Assert ((value & WidgetState.ActiveMask) == value);
				
				if (active != value)
				{
					this.widgetState &= ~WidgetState.ActiveMask;
					this.widgetState |= value & WidgetState.ActiveMask;
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
				
				WidgetState state = this.widgetState & mask;
				
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
				
				if (this.HasChildren)
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
			get { return ((this.internalState & InternalState.Focusable) != 0) && !this.IsFrozen; }
		}
		
		public bool							CanSelect
		{
			get { return ((this.internalState & InternalState.Selectable) != 0) && !this.IsFrozen; }
		}
		
		public bool							CanEngage
		{
			get { return ((this.internalState & InternalState.Engageable) != 0) && this.IsEnabled && !this.IsFrozen; }
		}
		
		public bool							AcceptThreeState
		{
			get { return (this.internalState & InternalState.AcceptThreeState) != 0; }
			set
			{
				if (value)
				{
					this.internalState |= InternalState.AcceptThreeState;
				}
				else
				{
					this.internalState &= ~InternalState.AcceptThreeState;
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
		
		public Support.CommandDispatcher	CommandDispatcher
		{
			get
			{
				WindowFrame frame = this.WindowFrame;
				return (frame == null) ? null : frame.CommandDispatcher;
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
		
		
		public string						CommandName
		{
			get
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				
				this.BuildCommandName (buffer);
				
				return buffer.ToString ();
			}
		}
		
		
		[Bundle ("name")]	public string			Name
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

		[Bundle ("text")]	public virtual string	Text
		{
			get
			{
				if (this.textLayout == null)
				{
					return "";
				}
				
				string text = this.textLayout.Text;
				
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
					this.textLayout.Text = value;
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
			get { return this.tabIndex; }
			set { this.tabIndex = value; }
		}
		
		public TabNavigationMode			TabNavigation
		{
			get { return this.tabNavigationMode; }
			set { this.tabNavigationMode = value; }
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
		
		public string						HyperText
		{
			get
			{
				if (this.hyperText == null)
				{
					return null;
				}
				
				return this.hyperText.Anchor;
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
		public event MessageEventHandler	Entered;
		public event MessageEventHandler	Exited;
		public event EventHandler			ShortcutPressed;
		public event EventHandler			HyperTextHot;
		public event MessageEventHandler	HyperTextClicked;
		
		public event EventHandler			Focused;
		public event EventHandler			Defocused;
		public event EventHandler			Selected;
		public event EventHandler			Deselected;
		public event EventHandler			Engaged;
		public event EventHandler			StillEngaged;
		public event EventHandler			Disengaged;
		public event EventHandler			ActiveStateChanged;
		public event EventHandler			MinSizeChanged;
		public event EventHandler			MaxSizeChanged;
		
		
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
			if ((this.internalState & InternalState.Visible) == 0)
			{
				if (visible)
				{
					this.internalState |= InternalState.Visible;
					this.Invalidate ();
				}
			}
			else
			{
				if (!visible)
				{
					this.internalState &= ~ InternalState.Visible;
					this.Invalidate ();
				}
			}
		}
		
		public virtual void SetEnabled(bool enabled)
		{
			if ((this.widgetState & WidgetState.Enabled) == 0)
			{
				if (enabled)
				{
					this.widgetState |= WidgetState.Enabled;
					this.Invalidate ();
				}
			}
			else
			{
				if (!enabled)
				{
					this.widgetState &= ~ WidgetState.Enabled;
					this.Invalidate ();
				}
			}
		}
		
		public virtual void SetFrozen(bool frozen)
		{
			if ((this.internalState & InternalState.Frozen) == 0)
			{
				if (frozen)
				{
					this.internalState |= InternalState.Frozen;
					this.Invalidate ();
				}
			}
			else
			{
				if (!frozen)
				{
					this.internalState &= ~ InternalState.Frozen;
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
			
			if ((this.widgetState & WidgetState.Focused) == 0)
			{
				if (focused)
				{
					this.widgetState |= WidgetState.Focused;
					frame.FocusedWidget = this;
					this.OnFocused ();
					this.Invalidate ();
				}
			}
			else
			{
				if (!focused)
				{
					this.widgetState &= ~ WidgetState.Focused;
					frame.FocusedWidget = null;
					this.OnDefocused ();
					this.Invalidate ();
				}
			}
		}
		
		public virtual void SetSelected(bool selected)
		{
			if ((this.widgetState & WidgetState.Selected) == 0)
			{
				if (selected)
				{
					this.widgetState |= WidgetState.Selected;
					this.Invalidate ();
					this.OnSelected ();
				}
			}
			else
			{
				if (!selected)
				{
					this.widgetState &= ~WidgetState.Selected;
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
			
			if ((this.internalState & InternalState.Engageable) == 0)
			{
				return;
			}
			
			if ((this.widgetState & WidgetState.Engaged) == 0)
			{
				if (engaged)
				{
					this.widgetState |= WidgetState.Engaged;
					frame.EngagedWidget = this;
					this.Invalidate ();
					this.OnEngaged ();
				}
			}
			else
			{
				if (!engaged)
				{
					this.widgetState &= ~ WidgetState.Engaged;
					frame.EngagedWidget = null;
					this.Invalidate ();
					this.OnDisengaged ();
				}
			}
		}
		
		public virtual void SetAutoMinMax(bool automatic)
		{
			if (automatic)
			{
				this.internalState |= InternalState.AutoMinMax;
				this.UpdateMinMaxBasedOnDockedChildren ();
			}
			else
			{
				this.internalState &= ~ InternalState.AutoMinMax;
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
					Widget.enteredWidgets.Add (this);
					this.widgetState |= WidgetState.Entered;
					
					System.Diagnostics.Debug.Assert ((this.parent == null) || (this.parent.IsEntered) || (this.parent == this.RootParent));
					
					message = Message.FromMouseEvent (MessageType.MouseEnter, null, null);
					
					this.OnEntered (new MessageEventArgs (message, Message.State.LastPosition));
				}
				else
				{
					Widget.enteredWidgets.Remove (this);
					this.widgetState &= ~ WidgetState.Entered;
					
					//	Il faut aussi supprimer les éventuels enfants encore marqués comme 'entered'.
					//	Pour ce faire, on passe en revue tous les widgets à la recherche d'enfants
					//	directs.
					
					int i = 0;
					
					while (i < Widget.enteredWidgets.Count)
					{
						Widget candidate = Widget.enteredWidgets[i] as Widget;
						
						if (candidate.Parent == this)
						{
							candidate.SetEntered (false);
							
							//	Note: le fait de changer l'état de l'enfant va modifier la liste des
							//	widgets sur laquelle on est en train d'itérer. On reprend donc, par
							//	précaution, l'itération au début...
							
							i = 0;
						}
						else
						{
							i++;
						}
					}
					
					message = Message.FromMouseEvent (MessageType.MouseLeave, null, null);
					
					this.OnExited (new MessageEventArgs (message, Message.State.LastPosition));
				}
				
				this.MessageHandler (message);
				frame.PostProcessMessage (message);
				this.Invalidate ();
			}
		}
		
		
		public static void UpdateEntered(WindowFrame window, Message message)
		{
			int index = Widget.enteredWidgets.Count;
			
			while (index > 0)
			{
				index--;
				
				if (index < Widget.enteredWidgets.Count)
				{
					Widget widget = Widget.enteredWidgets[index] as Widget;
					Widget.UpdateEntered (window, widget, message);
				}
			}
		}
		
		public static void UpdateEntered(WindowFrame window, Widget widget, Message message)
		{
			Drawing.Point point_in_widget = widget.MapRootToClient (message.Cursor);
			
			if ((widget.WindowFrame != window) ||
				(point_in_widget.X < 0) ||
				(point_in_widget.Y < 0) ||
				(point_in_widget.X >= widget.Client.Width) ||
				(point_in_widget.Y >= widget.Client.Height) ||
				(message.Type == MessageType.MouseLeave))
			{
				widget.SetEntered (false);
			}
			else
			{
				widget.SetEntered (true);
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
			return new Drawing.Rectangle (0, 0, this.clientInfo.width, this.clientInfo.height);
		}
		
		public virtual void Invalidate()
		{
			this.Invalidate (this.GetPaintBounds ());
		}
		
		public virtual void Invalidate(Drawing.Rectangle rect)
		{
			if (this.parent != null)
			{
				this.parent.Invalidate (this.MapClientToParent (rect));
			}
		}
		
		
		public virtual Drawing.Point MapParentToClient(Drawing.Point point)
		{
			Drawing.Point result = new Drawing.Point ();
			
			double z = this.clientInfo.zoom;
			
			switch (this.clientInfo.angle)
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
			
			double z = this.clientInfo.zoom;
			
			switch (this.clientInfo.angle)
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
		
		
		public virtual Drawing.Point MapScreenToClient(Drawing.Point point)
		{
			point = this.WindowFrame.MapScreenToWindow (point);
			point = this.MapRootToClient (point);
			return point;
		}
		
		public virtual Drawing.Point MapScreenToParent(Drawing.Point point)
		{
			point = this.WindowFrame.MapScreenToWindow (point);
			point = this.MapRootToClient (point);
			point = this.MapClientToParent (point);
			return point;
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

		
		public virtual Drawing.Size MapParentToClient(Drawing.Size size)
		{
			Drawing.Size result = new Drawing.Size ();
			
			double z = this.clientInfo.zoom;
			
			switch (this.clientInfo.angle)
			{
				case 0:
				case 180:
					result.Width  = size.Width / z;
					result.Height = size.Height / z;
					break;
				
				case 90:
				case 270:
					result.Width  = size.Height / z;
					result.Height = size.Width / z;
					break;
				
				default:
					throw new System.ArgumentOutOfRangeException ("Invalid angle");
			}
			
			return result;
		}
		
		public virtual Drawing.Size MapClientToParent(Drawing.Size size)
		{
			Drawing.Size result = new Drawing.Size ();
			
			double z = this.clientInfo.zoom;
			
			switch (this.clientInfo.angle)
			{
				case 0:
				case 180:
					result.Width  = size.Width * z;
					result.Height = size.Height * z;
					break;
				
				case 90:
				case 270:
					result.Width  = size.Height * z;
					result.Height = size.Width * z;
					break;
				
				default:
					throw new System.ArgumentOutOfRangeException ("Invalid angle");
			}
			
			return result;
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
			
			switch (this.clientInfo.angle)
			{
				case 0:		ox = this.x1; oy = this.y1; break;
				case 90:	ox = this.x2; oy = this.y1; break;
				case 180:	ox = this.x2; oy = this.y2; break;
				case 270:	ox = this.x1; oy = this.y2; break;
				default:	throw new System.ArgumentOutOfRangeException ("Invalid angle");
			}
			
			t.Translate (-ox, -oy);
			t.Rotate (-this.clientInfo.angle);
			t.Scale (1 / this.clientInfo.zoom);
			t.Round ();
			
			return t;
		}
		
		public virtual Epsitec.Common.Drawing.Transform GetTransformToParent()
		{
			Epsitec.Common.Drawing.Transform t = new Epsitec.Common.Drawing.Transform ();
			
			double ox, oy;
			
			switch (this.clientInfo.angle)
			{
				case 0:		ox = this.x1; oy = this.y1; break;
				case 90:	ox = this.x2; oy = this.y1; break;
				case 180:	ox = this.x2; oy = this.y2; break;
				case 270:	ox = this.x1; oy = this.y2; break;
				default:	throw new System.ArgumentOutOfRangeException ("Invalid angle");
			}
			
			t.Scale (this.clientInfo.zoom);
			t.Rotate (this.clientInfo.angle);
			t.Translate (ox, oy);
			t.Round ();
			
			return t;
		}
		
		
		public Widget			FindChild(Drawing.Point point)
		{
			return this.FindChild (point, ChildFindMode.SkipHidden);
		}
		
		public virtual Widget	FindChild(Drawing.Point point, ChildFindMode mode)
		{
			if (this.HasChildren == false)
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
		
		public Widget			FindChild(string name)
		{
			return this.FindChild (name, ChildFindMode.All);
		}
		
		public virtual Widget	FindChild(string name, ChildFindMode mode)
		{
			if (this.HasChildren == false)
			{
				return null;
			}
			
			Widget[] children = this.Children.Widgets;
			int  children_num = children.Length;
			
			for (int i = 0; i < children_num; i++)
			{
				Widget widget = children[i];
				
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
				
				if (widget.Name == "")
				{
					Widget child = widget.FindChild (name, mode);
					
					if (child != null)
					{
						return child;
					}
				}
				else if (widget.Name == name)
				{
					return widget;
				}
			}
			
			return null;
		}
		
		public virtual Widget	FindChild(string[] names, int offset)
		{
			if (offset >= names.Length)
			{
				return this;
			}
			
			if (this.HasChildren == false)
			{
				return null;
			}
			
			Widget[] children = this.Children.Widgets;
			int  children_num = children.Length;
			
			for (int i = 0; i < children_num; i++)
			{
				Widget widget = children[i];
				
				System.Diagnostics.Debug.Assert (widget != null);
				
				if (widget.Name == "")
				{
					Widget child = widget.FindChild (names, offset);
					
					if (child != null)
					{
						return child;
					}
				}
				else if (widget.Name == names[offset])
				{
					return widget.FindChild (names, offset+1);
				}
			}
			
			return null;
		}
		
		public Widget			FindCommandWidget(string command_name)
		{
			string[] names = command_name.Split (new char[] { '.' });
			
			if (this.Name == "")
			{
				return this.FindChild (names, 0);
			}
			if (this.Name == names[0])
			{
				return this.FindChild (names, 1);
			}
			
			return null;
		}
		
		
		public Widget[] FindCommandWidgets()
		{
			//	Passe en revue tous les widgets de la descendance et accumule
			//	ceux qui sont des widgets de commande.
			
			CommandWidgetFinder finder = new CommandWidgetFinder ();
			
			this.WalkChildren (new WalkWidgetCallback (finder.Analyse));
			
			return finder.Widgets;
		}
		
		
		protected class CommandWidgetFinder
		{
			public CommandWidgetFinder()
			{
			}
			
			public bool Analyse(Widget widget)
			{
				if (widget.IsCommand)
				{
					this.list.Add (widget);
				}
				
				return true;
			}
			
			public Widget[]					Widgets
			{
				get
				{
					Widget[] widgets = new Widget[this.list.Count];
					this.list.CopyTo (widgets);
					return widgets;
				}
			}
			
			System.Collections.ArrayList	list = new System.Collections.ArrayList ();
		}
		
		
		public bool WalkChildren(WalkWidgetCallback callback)
		{
			if (this.HasChildren == false)
			{
				return true;
			}
			
			Widget[] children = this.Children.Widgets;
			int  children_num = children.Length;
			
			for (int i = 0; i < children_num; i++)
			{
				Widget widget = children[i];
				
				if (!callback (widget))
				{
					return false;
				}
				
				widget.WalkChildren (callback);
			}
			
			return true;
		}
		
		
		protected virtual void SetBounds(double x1, double y1, double x2, double y2)
		{
			if ((x1 == this.x1) && (y1 == this.y1) && (x2 == this.x2) && (y2 == this.y2))
			{
				return;
			}
			
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
			if (this.textLayout != null)
			{
				this.textLayout.Alignment  = this.Alignment;
				this.textLayout.LayoutSize = this.Client.Size;
			}
		}
		
		
		
		protected virtual void UpdateClientGeometry()
		{
			if (this.layoutInfo == null)
			{
				this.layoutInfo = new LayoutInfo (this.clientInfo.width, this.clientInfo.height);
			}
			
			try
			{
				double zoom = this.clientInfo.zoom;
				
				double dx = (this.x2 - this.x1) / zoom;
				double dy = (this.y2 - this.y1) / zoom;
				
				switch (this.clientInfo.angle)
				{
					case 0:
					case 180:
						this.clientInfo.SetSize (dx, dy);
						break;
					
					case 90:
					case 270:
						this.clientInfo.SetSize (dy, dx);
						break;
					
					default:
						double angle = this.clientInfo.angle * System.Math.PI / 180.0;
						double cos = System.Math.Cos (angle);
						double sin = System.Math.Sin (angle);
						this.clientInfo.SetSize (cos*cos*dx + sin*sin*dy, sin*sin*dx + cos*cos*dy);
						break;
				}
				
				if (this.suspendCounter == 0)
				{
					this.UpdateChildrenLayout ();
					this.OnLayoutChanged ();
				}
			}
			finally
			{
				if (this.suspendCounter == 0)
				{
					this.layoutInfo = null;
				}
			}
		}
		
		protected virtual void UpdateMinMaxBasedOnDockedChildren()
		{
			if ((this.internalState & InternalState.ChildrenDocked) == 0)
			{
				return;
			}
			
			System.Diagnostics.Debug.Assert (this.HasChildren);
			
			//	Décompose les dimensions comme suit :
			//
			//	|											  |
			//	|<---min_ox1--->| zone de travail |<-min_ox2->|
			//	|											  |
			//	|<-------------------min_dx------------------>|
			//
			//	min_ox = min_ox1 + min_ox2
			//	min_dx = minimum courant
			//
			//	La partie centrale (DockStyle.Fill) va s'additionner au reste de manière
			//	indépendante au moyen du fill_min_dx.
			//
			//	Idem par analogie pour dy et max.
			
			double min_ox = 0;
			double min_oy = 0;
			double max_ox = 0;
			double max_oy = 0;
			
			double min_dx = 0;
			double min_dy = 0;
			double max_dx = 1000000;
			double max_dy = 1000000;
			
			double fill_min_dx = 0;
			double fill_min_dy = 0;
			double fill_max_dx = 0;
			double fill_max_dy = 0;
			
			if (this.PreferHorizontalDockLayout)
			{
				fill_max_dy = max_dy;
			}
			else
			{
				fill_max_dx = max_dx;
			}
			
			foreach (Widget child in this.Children)
			{
				if (child.Dock == DockStyle.None)
				{
					//	Saute les widgets qui ne sont pas "docked", car leur taille n'est pas prise
					//	en compte dans le calcul des minima/maxima.
					
					continue;
				}
				
				Drawing.Size min = child.MinSize;
				Drawing.Size max = child.MaxSize;
				
				switch (child.Dock)
				{
					case DockStyle.Top:
						min_dx  = System.Math.Max (min_dx, min.Width    + min_ox);
						min_dy  = System.Math.Max (min_dy, child.Height + min_oy);
						min_oy += child.Height;
						max_dx  = System.Math.Min (max_dx, max.Width    + max_ox);
						max_dy  = System.Math.Min (max_dy, child.Height + max_oy);
						max_oy += child.Height;
						break;
					
					case DockStyle.Bottom:
						min_dx  = System.Math.Max (min_dx, min.Width    + min_ox);
						min_dy  = System.Math.Max (min_dy, child.Height + min_oy);
						min_oy += child.Height;
						max_dx  = System.Math.Min (max_dx, max.Width    + max_ox);
						max_dy  = System.Math.Min (max_dy, child.Height + max_oy);
						max_oy += child.Height;
						break;
						
					case DockStyle.Left:
						min_dx  = System.Math.Max (min_dx, child.Width  + min_ox);
						min_dy  = System.Math.Max (min_dy, min.Height   + min_oy);
						min_ox += child.Width;
						max_dx  = System.Math.Min (max_dx, child.Width  + max_ox);
						max_dy  = System.Math.Min (max_dy, max.Height   + max_oy);
						max_ox += child.Width;
						break;
					
					case DockStyle.Right:
						min_dx  = System.Math.Max (min_dx, child.Width  + min_ox);
						min_dy  = System.Math.Max (min_dy, min.Height   + min_oy);
						min_ox += child.Width;
						max_dx  = System.Math.Min (max_dx, child.Width  + max_ox);
						max_dy  = System.Math.Min (max_dy, max.Height   + max_oy);
						max_ox += child.Width;
						break;
					
					case DockStyle.Fill:
						if (this.PreferHorizontalDockLayout)
						{
							fill_min_dx += min.Width;
							fill_min_dy  = System.Math.Max (fill_min_dy, min.Height);
							fill_max_dx += max.Width;
							fill_max_dy  = System.Math.Min (fill_max_dy, max.Height);
						}
						else
						{
							fill_min_dx  = System.Math.Max (fill_min_dx, min.Width);
							fill_min_dy += min.Height;
							fill_max_dx  = System.Math.Min (fill_max_dx, max.Width);
							fill_max_dy += max.Height;
						}
						break;
				}
			}
			
			double min_width  = System.Math.Max (min_dx, fill_min_dx + min_ox);
			double min_height = System.Math.Max (min_dy, fill_min_dy + min_oy);
			double max_width  = System.Math.Min (max_dx, fill_max_dx + max_ox);
			double max_height = System.Math.Min (max_dy, fill_max_dy + max_oy);
			
			//	Tous les calculs ont été faits en coordonnées client, il faut donc encore transformer
			//	ces dimensions en coordonnées parents.
			
			this.MinSize = this.MapClientToParent (new Drawing.Size (min_width, min_height));
			this.MaxSize = this.MapClientToParent (new Drawing.Size (max_width, max_height));
		}
		
		protected virtual void AdjustDockBounds(ref Drawing.Rectangle bounds)
		{
			bounds.Left   += this.DockMargins.Left;
			bounds.Right  -= this.DockMargins.Right;
			bounds.Top    -= this.DockMargins.Top;
			bounds.Bottom += this.DockMargins.Bottom;
		}
		
		protected virtual void UpdateDockedChildrenLayout()
		{
			if ((this.internalState & InternalState.AutoMinMax) != 0)
			{
				this.UpdateMinMaxBasedOnDockedChildren ();
			}
			
			if ((this.internalState & InternalState.ChildrenDocked) == 0)
			{
				return;
			}
			
			System.Diagnostics.Debug.Assert (this.clientInfo != null);
			System.Diagnostics.Debug.Assert (this.HasChildren);
			
			System.Collections.Queue fill_queue = null;
			Drawing.Rectangle client_rect = this.clientInfo.Bounds;
			
			this.AdjustDockBounds (ref client_rect);
			
			foreach (Widget child in this.Children)
			{
				if (child.Dock == DockStyle.None)
				{
					//	Saute les widgets qui ne sont pas "docked", car ils doivent être
					//	positionnés par d'autres moyens.
					
					continue;
				}
				
				double dx = child.Width;
				double dy = child.Height;
				
				switch (child.Dock)
				{
					case DockStyle.Top:
						child.SetBounds (client_rect.Left, client_rect.Top - dy, client_rect.Right, client_rect.Top);
						client_rect.Top -= dy;
						break;
						
					case DockStyle.Bottom:
						child.SetBounds (client_rect.Left, client_rect.Bottom, client_rect.Right, client_rect.Bottom + dy);
						client_rect.Bottom += dy;
						break;
					
					case DockStyle.Left:
						child.SetBounds (client_rect.Left, client_rect.Bottom, client_rect.Left + dx, client_rect.Top);
						client_rect.Left += dx;
						break;
					
					case DockStyle.Right:
						child.SetBounds (client_rect.Right - dx, client_rect.Bottom, client_rect.Right, client_rect.Top);
						client_rect.Right -= dx;
						break;
					
					case DockStyle.Fill:
						if (fill_queue == null)
						{
							fill_queue = new System.Collections.Queue ();
						}
						fill_queue.Enqueue (child);
						break;
				}
			}
			
			if (fill_queue != null)
			{
				if (this.PreferHorizontalDockLayout)
				{
					int n = fill_queue.Count;
					double fill_dx = client_rect.Width;
					
					foreach (Widget child in fill_queue)
					{
						child.SetBounds (client_rect.Left, client_rect.Bottom, client_rect.Left + fill_dx / n, client_rect.Top);
						client_rect.Left += fill_dx / n;
					}
				}
				else
				{
					int n = fill_queue.Count;
					double fill_dy = client_rect.Height;
					
					foreach (Widget child in fill_queue)
					{
						child.SetBounds (client_rect.Left, client_rect.Top - fill_dy / n, client_rect.Right, client_rect.Top);
						client_rect.Top -= fill_dy / n;
					}
				}
			}
		}
		
		protected virtual void UpdateChildrenLayout()
		{
			this.UpdateDockedChildrenLayout ();
			
			if (this.layoutInfo == null)
			{
				return;
			}
			
			System.Diagnostics.Debug.Assert (this.clientInfo != null);
			System.Diagnostics.Debug.Assert (this.layoutInfo != null);
			
			if (this.HasChildren)
			{
				double width_diff  = this.clientInfo.width  - this.layoutInfo.OriginalWidth;
				double height_diff = this.clientInfo.height - this.layoutInfo.OriginalHeight;
				
				foreach (Widget child in this.Children)
				{
					if (child.Dock != DockStyle.None)
					{
						//	Saute les widgets qui sont "docked" dans le parent, car ils ont déjà été
						//	positionnés par la méthode UpdateDockedChildrenLayout.
						
						continue;
					}
					
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
			long cycles = Drawing.Agg.Library.Cycles;
			
			if (this.DebugActive)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("{0}: clip {1}, widget {2}", this.Name, graphics.SaveClippingRectangle ().ToString (), this.MapClientToRoot (this.Client.Bounds).ToString ()));
			}
			
			if (this.PaintCheckClipping (repaint))
			{
				Drawing.Rectangle bounds = this.GetPaintBounds ();
				
				bounds  = this.MapClientToRoot (bounds);
				repaint = this.MapParentToClient (repaint);
				
				Drawing.Rectangle original_clipping  = graphics.SaveClippingRectangle ();
				Drawing.Transform original_transform = graphics.SaveTransform ();
				Drawing.Transform graphics_transform = this.GetTransformToParent ();
				
				graphics.SetClippingRectangle (bounds);
				
				if (graphics.TestForEmptyClippingRectangle ())
				{
					//	Optimisation du cas où la région de clipping devient vide: on restaure
					//	la région précédente et on ne fait rien de plus.
					
					graphics.RestoreClippingRectangle (original_clipping);
					return;
				}
				
				graphics_transform.MultiplyBy (original_transform);
				
				graphics.Transform = graphics_transform;
			
				try
				{
					if (this.hyperTextList != null)
					{
						this.hyperTextList.Clear ();
					}
					
					PaintEventArgs local_paint_args = new PaintEventArgs (graphics, repaint);
					
					//	Peint l'arrière-plan du widget. En principe, tout va dans l'arrière plan, sauf
					//	si l'on désire réaliser des effets de transparence par dessus le dessin des
					//	widgets enfants.
					
					this.OnPaintBackground (local_paint_args);
					
					//	Peint tous les widgets enfants, en commençant par le numéro 0, lequel se trouve
					//	derrière tous les autres, etc. On saute les widgets qui ne sont pas visibles.
					
					if (this.HasChildren)
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
			if (this.DebugActive)
			{
				cycles = Drawing.Agg.Library.Cycles - cycles;
				System.Diagnostics.Debug.WriteLine (string.Format ("{0}: {1} us @ 1.7GHz", this.Name, cycles/1700));
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
				(this.HasChildren))
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
								
								if ((widget.IsEntered == false) &&
									(message.Type != MessageType.MouseLeave))
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
			
			if (message.IsMouseType)
			{
				bool reset = true;
				
				if (this.hyperTextList != null)
				{
					foreach (HyperTextInfo info in this.hyperTextList)
					{
						if (info.Bounds.Contains (pos))
						{
							this.SetHyperText (info);
							reset = false;
							break;
						}
					}
				}
				
				if (reset)
				{
					this.SetHyperText (null);
				}
			}
		}
		
		protected virtual void SetHyperText(HyperTextInfo info)
		{
			if (this.hyperText == null)
			{
				if (info == null)
				{
					return;
				}
			}
			else if (this.hyperText.Equals (info))
			{
				return;
			}
			
			this.hyperText = info;
			this.OnHyperTextHot ();
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
		
		
		protected virtual void BuildCommandName(System.Text.StringBuilder buffer)
		{
			if (this.parent != null)
			{
				this.parent.BuildCommandName (buffer);
			}
			int length = buffer.Length;
			
			if ((length > 0) &&
				(buffer[length-1] != '.'))
			{
				buffer.Append (".");
			}
			
			buffer.Append (this.Name);
		}
		
		
		protected virtual void CreateWidgetCollection()
		{
			this.children = new WidgetCollection (this);
		}
		
		protected virtual void CreateTextLayout()
		{
			if (this.textLayout == null)
			{
				this.textLayout = new TextLayout ();
				
				this.textLayout.Font     = this.DefaultFont;
				this.textLayout.FontSize = this.DefaultFontSize;
				this.textLayout.Anchor  += new AnchorEventHandler (this.HandleTextLayoutAnchor);
				
				this.UpdateLayoutSize ();
			}
		}
		
		protected virtual void DisposeTextLayout()
		{
			if (this.textLayout != null)
			{
				this.textLayout.Anchor -= new AnchorEventHandler (this.HandleTextLayoutAnchor);
				this.textLayout = null;
			}
		}
		
		protected virtual void HandleTextLayoutAnchor(object sender, AnchorEventArgs e)
		{
			System.Diagnostics.Debug.Assert (sender == this.textLayout);
			
			HyperTextInfo info = new HyperTextInfo (this.textLayout, e.Bounds, e.Index);
			
			if (this.hyperTextList == null)
			{
				this.hyperTextList = new System.Collections.ArrayList ();
			}
			
			this.hyperTextList.Add (info);
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
			if (this.hyperText != null)
			{
				this.OnHyperTextClicked (e);
				
				if (e.Message.Consumer != null)
				{
					return;
				}
			}
			
			if (this.Clicked != null)
			{
				if (e != null)
				{
					e.Message.Consumer = this;
				}
				
				this.Clicked (this, e);
			}
			
			if (this.IsCommand)
			{
				WindowFrame window = this.WindowFrame;
				
				if (window != null)
				{
					window.QueueCommand (this);
				}
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
		
		protected virtual void OnEntered(MessageEventArgs e)
		{
			this.MouseCursor.SetWindowCursor (this.WindowFrame);
			
			if (this.Entered != null)
			{
				if (e != null)
				{
					e.Message.Consumer = this;
				}
				
				this.Entered (this, e);
			}
		}
		
		protected virtual void OnExited(MessageEventArgs e)
		{
			if (this.parent != null)
			{
				this.parent.MouseCursor.SetWindowCursor (this.WindowFrame);
			}
			
			if (this.Exited != null)
			{
				if (e != null)
				{
					e.Message.Consumer = this;
				}
				
				this.Exited (this, e);
			}
		}
		
		protected virtual void OnShortcutPressed()
		{
			if (this.ShortcutPressed != null)
			{
				this.ShortcutPressed (this);
			}
		}
		
		protected virtual void OnHyperTextHot()
		{
			if (this.hyperText == null)
			{
				this.MouseCursor.SetWindowCursor (this.WindowFrame);
			}
			else
			{
				MouseCursor.AsHand.SetWindowCursor (this.WindowFrame);
			}
			
			if (this.HyperTextHot != null)
			{
				this.HyperTextHot (this);
			}
		}
		
		protected virtual void OnHyperTextClicked(MessageEventArgs e)
		{
			if (this.HyperTextClicked != null)
			{
				e.Message.Consumer = this;
				this.HyperTextClicked (this, e);
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
		
		protected virtual void OnMinSizeChanged()
		{
			if (this.MinSizeChanged != null)
			{
				this.MinSizeChanged (this);
			}
		}
		
		protected virtual void OnMaxSizeChanged()
		{
			if (this.MaxSizeChanged != null)
			{
				this.MaxSizeChanged (this);
			}
		}
		
		
		[System.Flags] protected enum InternalState
		{
			None				= 0,
			ChildrenChanged		= 0x00000001,
			ChildrenDocked		= 0x00000002,		//	=> il y a des enfants avec Dock != None
			
			Focusable			= 0x00000010,
			Selectable			= 0x00000020,
			Engageable			= 0x00000040,		//	=> peut être enfoncé par une pression
			Frozen				= 0x00000080,		//	=> n'accepte aucun événement
			Visible				= 0x00000100,
			AcceptThreeState	= 0x00000200,
			
			PreferXLayout		= 0x00000400,		//	=> en cas de DockStyle.Fill multiple, place le contenu horizontalement
			
			AutoMinMax			= 0x00008000,		//	=> calcule automatiquement les tailles min et max
			AutoCapture			= 0x00010000,
			AutoFocus			= 0x00020000,
			AutoEngage			= 0x00040000,
			AutoToggle			= 0x00080000,
			AutoMnemonic		= 0x00100000,
			AutoRepeatEngaged	= 0x00200000,
			
			Command				= 0x20000000,		//	widget génère des commandes
			DebugActive			= 0x40000000		//	widget marqué pour le debug
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
				if (this.widget.suspendCounter == 0)
				{
					this.widget.HandleChildrenChanged ();
				}
				else
				{
					this.widget.internalState |= InternalState.ChildrenChanged;
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
				if (this.widget.suspendCounter == 0)
				{
					this.widget.HandleChildrenChanged ();
				}
				else
				{
					this.widget.internalState |= InternalState.ChildrenChanged;
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
		
		protected class HyperTextInfo : System.ICloneable, System.IComparable
		{
			internal HyperTextInfo(TextLayout layout, Drawing.Rectangle bounds, int index)
			{
				this.layout = layout;
				this.bounds = bounds;
				this.index  = index;
			}
			
			
			#region ICloneable Members
			public object Clone()
			{
				return new HyperTextInfo (this.layout, this.bounds, this.index);
			}
			#endregion

			#region IComparable Members
			public int CompareTo(object obj)
			{
				if (obj == null)
				{
					return 1;
				}
				
				HyperTextInfo that = obj as HyperTextInfo;
				
				if ((that == null) || (that.layout != this.layout))
				{
					throw new System.ArgumentException ("Invalid argument");
				}
				
				return this.index.CompareTo (that.index);
			}
			#endregion
			
			public override bool Equals(object obj)
			{
				return this.CompareTo (obj) == 0;
			}
		
			public override int GetHashCode()
			{
				return this.index;
			}
			
			
			public Drawing.Rectangle		Bounds
			{
				get { return this.bounds; }
			}
			
			public string					Anchor
			{
				get { return this.layout.FindAnchor (this.index); }
			}
			
			
			private TextLayout				layout;
			private Drawing.Rectangle		bounds;
			private int						index;
		}
		
		
		protected AnchorStyles					anchor;
		protected DockStyle						dock;
		protected Drawing.Margins				dockMargins;
		protected Drawing.Color					backColor;
		protected Drawing.Color					foreColor;
		protected double						x1, y1, x2, y2;
		protected Drawing.Size					minSize;
		protected Drawing.Size					maxSize;
		protected ClientInfo					clientInfo = new ClientInfo ();
		protected System.Collections.ArrayList	hyperTextList;
		protected HyperTextInfo					hyperText;
		
		protected WidgetCollection				children;
		protected Widget						parent;
		protected string						name;
		protected TextLayout					textLayout;
		protected ContentAlignment				alignment;
		protected LayoutInfo					layoutInfo;
		protected InternalState					internalState;
		protected WidgetState					widgetState;
		protected int							suspendCounter;
		protected int							tabIndex;
		protected TabNavigationMode				tabNavigationMode;
		protected Shortcut						shortcut;
		protected double						defaultFontHeight;
		protected MouseCursor					mouseCursor;
		
		static System.Collections.ArrayList		enteredWidgets = new System.Collections.ArrayList ();
		static System.Collections.ArrayList		aliveWidgets = new System.Collections.ArrayList ();
	}
}
