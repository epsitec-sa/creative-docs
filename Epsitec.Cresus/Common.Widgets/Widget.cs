//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 09/10/2003

namespace Epsitec.Common.Widgets
{
	using ContentAlignment = Drawing.ContentAlignment;
	using BundleAttribute  = Support.BundleAttribute;
	
	
	public delegate bool WalkWidgetCallback(Widget widget);
	public delegate void PaintBoundsCallback(Widget widget, ref Drawing.Rectangle bounds);
	
	[System.Flags] public enum AnchorStyles : byte
	{
		None			= 0,
		Top				= 1,
		Bottom			= 2,
		Left			= 4,
		Right			= 8,
			
		LeftAndRight	= Left + Right,
		TopAndBottom	= Top + Bottom,
	}
	
	[System.Flags] public enum WidgetState : uint
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
	
	[System.Flags] public enum InternalState : uint
	{
		None				= 0,
		ChildrenChanged		= 0x00000001,
		ChildrenDocked		= 0x00000002,		//	=> il y a des enfants avec Dock != None
		
		Embedded			= 0x00000008,		//	=> widget appartient au parent (widgets composés)
		
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
		
		PossibleContainer	= 0x01000000,		//	widget peut être la cible d'un drag & drop en mode édition
		EditionDisabled		= 0x02000000,		//	widget ne peut en aucun cas être édité
		
		Command				= 0x40000000,		//	widget génère des commandes
		DebugActive			= 0x80000000		//	widget marqué pour le debug
	}
	
	public enum DockStyle : byte
	{
		None			= 0,
		
		Top				= 1,				//	colle en haut
		Bottom			= 2,				//	colle en bas
		Left			= 3,				//	colle à gauche
		Right			= 4,				//	colle à droite
		Fill			= 5,				//	remplit tout
		
		Layout			= 6,				//	utilise un Layout Manager externe
	}
	
	public enum LayoutFlags : byte
	{
		None			= 0,
		
		StartNewLine	= 0x40,				//	force layout sur une nouvelle ligne
		IncludeChildren	= 0x80				//	inclut les enfants
	}
	
	
	/// <summary>
	/// La classe Widget implémente la classe de base dont dérivent tous les
	/// widgets de l'interface graphique ("controls" dans l'appellation Windows).
	/// </summary>
	public class Widget : System.IDisposable, Support.IBundleSupport
	{
		public Widget()
		{
			this.internal_state |= InternalState.Visible;
			this.internal_state |= InternalState.AutoCapture;
			this.internal_state |= InternalState.AutoMnemonic;
			
			this.widget_state |= WidgetState.Enabled;
			
			this.defaultFontHeight = System.Math.Floor(this.DefaultFont.LineHeight*this.DefaultFontSize);
			this.alignment = this.DefaultAlignment;
			this.anchor    = this.DefaultAnchor;
			this.Size      = new Drawing.Size (this.DefaultWidth, this.DefaultHeight);
			
			this.backColor = Drawing.Color.FromName ("Control");
			this.foreColor = Drawing.Color.FromName ("ControlText");
			
			this.minSize = this.DefaultMinSize;
			this.maxSize = this.DefaultMaxSize;
			
			Widget.alive_widgets.Add (new System.WeakReference (this));
		}
		
		public Widget(Widget embedder) : this()
		{
			this.SetEmbedder (embedder);
		}
		
		static Widget()
		{
			Drawing.Font.Initialise ();
			Support.ObjectBundler.RegisterAssembly (typeof (Widget).Assembly);
		}
		
		public static void Initialise()
		{
			System.Threading.Thread.CurrentThread.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentCulture;
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
		
		public virtual void RestoreFromBundle(Support.ObjectBundler bundler, Support.ResourceBundle bundle)
		{
//			this.SuspendLayout ();
			
			//	L'ObjectBundler sait initialiser la plupart des propriétés simples (celles
			//	qui sont marquées par l'attribut [Bundle]), mais il ne sait pas comment
			//	restitue les enfants du widget :
			
			Support.ResourceBundle.FieldList widget_list = bundle["widgets"].AsList;
			
			if (widget_list != null)
			{
				//	Notre bundle contient une liste de sous-bundles contenant les descriptions des
				//	widgets enfants. On les restitue nous-même et on les ajoute dans la liste des
				//	enfants.
				
				foreach (Support.ResourceBundle.Field field in widget_list)
				{
					Support.ResourceBundle widget_bundle = field.AsBundle;
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
				
				if (this.Disposing != null)
				{
					this.Disposing (this);
					this.Disposing = null;
				}
				
				this.Parent = null;
			}
		}
		
		
		public static int					DebugAliveWidgetsCount
		{
			get
			{
				System.Collections.ArrayList alive = new System.Collections.ArrayList ();
				
				foreach (System.WeakReference weak_ref in Widget.alive_widgets)
				{
					if (weak_ref.IsAlive)
					{
						alive.Add (weak_ref);
					}
				}
				
				Widget.alive_widgets = alive;
				return alive.Count;
			}
		}
		
		public static Widget[]				DebugAliveWidgets
		{
			get
			{
				Widget[] widgets = new Widget[Widget.DebugAliveWidgetsCount];
				
				int i = 0;
				
				foreach (System.WeakReference weak_ref in Widget.alive_widgets)
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
			get { return this.dock_margins; }
			set
			{
				if (this.dock_margins != value)
				{
					this.dock_margins = value;
					if (this.parent != null)
					{
						this.parent.UpdateChildrenLayout ();
					}
				}
			}
		}
		
		[Bundle ("lay_f")]	public LayoutFlags		LayoutFlags
		{
			get { return this.layout_flags; }
			set
			{
				if (this.layout_flags != value)
				{
					this.layout_flags = value;
					
					if (this.parent != null)
					{
						this.parent.HandleChildrenChanged ();
						this.parent.UpdateChildrenLayout ();
					}
				}
			}
		}
		
		[Bundle ("lay_m")]	public Drawing.Margins	LayoutMargins
		{
			get { return this.layout_margins; }
			set
			{
				if (this.layout_margins != value)
				{
					this.layout_margins = value;
					if (this.parent != null)
					{
						this.parent.UpdateChildrenLayout ();
					}
				}
			}
		}
		
		[Bundle ("dock_h")]	public bool				PreferHorizontalDockLayout
		{
			get { return (this.internal_state & InternalState.PreferXLayout) != 0; }
			set
			{
				if (value != this.PreferHorizontalDockLayout)
				{
					if (value)
					{
						this.internal_state |= InternalState.PreferXLayout;
					}
					else
					{
						this.internal_state &= ~ InternalState.PreferXLayout;
					}
					
					this.UpdateDockedChildrenLayout ();
				}
			}
		}
		
		
		public int							LayoutArg1
		{
			get { return this.layout_arg1; }
		}
		
		public int							LayoutArg2
		{
			get { return this.layout_arg2; }
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
			get { return this.client_info; }
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
					if (this.layout_info != null)
					{
						this.UpdateChildrenLayout ();
						this.OnLayoutChanged ();
						this.layout_info = null;
					}
					if ((this.internal_state & InternalState.ChildrenChanged) != 0)
					{
						this.internal_state -= InternalState.ChildrenChanged;
						this.HandleChildrenChanged ();
					}
				}
			}
		}
		
		
		protected virtual void HandleChildrenChanged()
		{
			this.internal_state &= ~InternalState.ChildrenDocked;
			
			foreach (Widget child in this.Children)
			{
				if ((child.Dock != DockStyle.None) &&
					(child.Dock != DockStyle.Layout))
				{
					this.internal_state |= InternalState.ChildrenDocked;
					break;
				}
			}
			
			if ((this.internal_state & InternalState.ChildrenDocked) != 0)
			{
				this.UpdateDockedChildrenLayout ();
			}
			
			this.Invalidate ();
			this.OnChildrenChanged ();
		}
		
		protected virtual void HandleAdornerChanged()
		{
			foreach (Widget child in this.Children)
			{
				child.HandleAdornerChanged ();
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
		
		public void SetClientOffset(double ox, double oy)
		{
			this.client_info.SetOffset (ox, oy);
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
			get { return (this.internal_state & InternalState.DebugActive) != 0; }
			set
			{
				if (value)
				{
					this.internal_state |= InternalState.DebugActive;
				}
				else
				{
					this.internal_state &= ~ InternalState.DebugActive;
				}
			}
		}
		
		
		[Bundle ("cmd")] public bool		IsCommand
		{
			get { return (this.internal_state & InternalState.Command) != 0; }
			set
			{
				if (value)
				{
					this.internal_state |= InternalState.Command;
				}
				else
				{
					this.internal_state &= ~ InternalState.Command;
				}
			}
		}
		
		
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
		
		public bool							IsEmbedded
		{
			get { return (this.internal_state & InternalState.Embedded) != 0; }
		}
		
		public bool							IsEditionDisabled
		{
			get { return (this.internal_state & InternalState.EditionDisabled) != 0; }
			set
			{
				if (this.IsEditionDisabled != value)
				{
					if (value)
					{
						this.internal_state |= InternalState.EditionDisabled;
					}
					else
					{
						this.internal_state &= ~InternalState.EditionDisabled;
					}
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
		
		
		protected InternalState				InternalState
		{
			get { return this.internal_state; }
			set { this.internal_state = value; }
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
		
		public virtual bool					PossibleContainer
		{
			get { return ((this.internal_state & InternalState.PossibleContainer) != 0) && !this.IsFrozen; }
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
		
		public virtual Window				Window
		{
			get
			{
				Widget root = this.RootParent;
				
				if ((root == null) ||
					(root == this))
				{
					return null;
				}
				
				return root.Window;
			}
		}
		
		public Support.CommandDispatcher	CommandDispatcher
		{
			get
			{
				Window window = this.Window;
				return (window == null) ? null : window.CommandDispatcher;
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
		
		public int							Index
		{
			get
			{
				return this.index;
			}
			set
			{
				this.index = value;
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
		
		public string						Hypertext
		{
			get
			{
				if (this.hypertext == null)
				{
					return null;
				}
				
				return this.hypertext.Anchor;
			}
		}
		
		public Drawing.TextBreakMode		TextBreakMode
		{
			get
			{
				if (this.textLayout != null)
				{
					return this.textLayout.BreakMode;
				}
				
				return Drawing.TextBreakMode.None;
			}
			set
			{
				if (this.textLayout == null)
				{
					this.CreateTextLayout ();
				}
				
				this.textLayout.BreakMode = value;
			}
		}
		
		
		public event EventHandler			PreparePaint;
		public event PaintEventHandler		PaintBackground;
		public event PaintEventHandler		PaintForeground;
		public event EventHandler			ChildrenChanged;
		public event EventHandler			ParentChanged;
		public event EventHandler			LayoutChanged;
		
		public event MessageEventHandler	Pressed;
		public event MessageEventHandler	Released;
		public event MessageEventHandler	Clicked;
		public event MessageEventHandler	DoubleClicked;
		public event MessageEventHandler	Entered;
		public event MessageEventHandler	Exited;
		public event EventHandler			ShortcutPressed;
		public event EventHandler			HypertextHot;
		public event MessageEventHandler	HypertextClicked;
		
		public event MessageEventHandler	PreProcessing;
		public event MessageEventHandler	PostProcessing;
		
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
		public event EventHandler			Disposing;
		
		public event PaintBoundsCallback	PaintBoundsCallback;
		
		
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
					//	Rend invisible un widget qui était visible avant. Il faut
					//	aussi s'assurer que le widget n'est plus "Entered".
					
					this.SetEntered (false);
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
			Window window = this.Window;
			
			if (window == null)
			{
				return;
			}
			
			if ((this.widget_state & WidgetState.Focused) == 0)
			{
				if (focused)
				{
					this.widget_state |= WidgetState.Focused;
					window.FocusedWidget = this;
					this.OnFocused ();
					this.Invalidate ();
				}
			}
			else
			{
				if (!focused)
				{
					this.widget_state &= ~ WidgetState.Focused;
					window.FocusedWidget = null;
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
			Window window = this.Window;
			
			if (window == null)
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
					window.EngagedWidget = this;
					this.Invalidate ();
					this.OnEngaged ();
				}
			}
			else
			{
				if (!engaged)
				{
					this.widget_state &= ~ WidgetState.Engaged;
					window.EngagedWidget = null;
					this.Invalidate ();
					this.OnDisengaged ();
				}
			}
		}
		
		public virtual void SetAutoMinMax(bool automatic)
		{
			if (automatic)
			{
				this.internal_state |= InternalState.AutoMinMax;
				this.UpdateMinMaxBasedOnDockedChildren ();
			}
			else
			{
				this.internal_state &= ~ InternalState.AutoMinMax;
			}
		}
		
		public virtual void SetLayoutArgs(int arg1, int arg2)
		{
			this.layout_arg1 = (byte) arg1;
			this.layout_arg2 = (byte) arg2;
		}
		
		public virtual void GetLayoutArgs(out int arg1, out int arg2)
		{
			arg1 = this.layout_arg1;
			arg2 = this.layout_arg2;
		}
		
		public virtual void SetPropagationModes(PropagationModes modes, bool on, PropagationSetting propagation)
		{
			if (on)
			{
				this.propagation |= modes;
			}
			else
			{
				this.propagation &= ~modes;
			}
			
			if ((propagation == PropagationSetting.IncludeChildren) &&
				(this.HasChildren))
			{
				for (int i = 0; i < this.children.Count; i++)
				{
					this.children[i].SetPropagationModes (modes, on, propagation);
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
		
		
		public override string ToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append (this.GetType ().Name);
			this.BuildCommandName (buffer);
			buffer.Append (@".""");
			buffer.Append (this.Text);
			buffer.Append (@"""");
			
			return buffer.ToString ();
		}
		
		
		protected void SetEntered(bool entered)
		{
			if (this.IsEntered != entered)
			{
				Window window = this.Window;
				Message message = null;
				
				if (entered)
				{
					Widget.ExitWidgetsNotParentOf (this);
					Widget.entered_widgets.Add (this);
					this.widget_state |= WidgetState.Entered;
					
					System.Diagnostics.Debug.Assert ((this.parent == null) || (this.parent.IsEntered) || (this.parent == this.RootParent));
					
					message = Message.FromMouseEvent (MessageType.MouseEnter, null, null);
					
					this.OnEntered (new MessageEventArgs (message, Message.State.LastPosition));
				}
				else
				{
					Widget.entered_widgets.Remove (this);
					this.widget_state &= ~ WidgetState.Entered;
					
					//	Il faut aussi supprimer les éventuels enfants encore marqués comme 'entered'.
					//	Pour ce faire, on passe en revue tous les widgets à la recherche d'enfants
					//	directs.
					
					int i = 0;
					
					while (i < Widget.entered_widgets.Count)
					{
						Widget candidate = Widget.entered_widgets[i] as Widget;
						
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
				
				if (window != null)
				{
					window.PostProcessMessage (message);
				}
				
				this.Invalidate ();
			}
		}
		
		protected static void ExitWidgetsNotParentOf(Widget widget)
		{
			int i = 0;
			
			while (i < Widget.entered_widgets.Count)
			{
				Widget candidate = Widget.entered_widgets[i] as Widget;
				
				if (widget.IsAncestor (candidate) == false)
				{
					//	Ce candidat n'est pas un ancêtre (parent direct ou indirect) du widget
					//	considéré; il faut donc changer son état Entered pour refléter le fait
					//	que le candidat n'a plus la souris :
					
					candidate.SetEntered (false);
					
					//	Note: le fait de changer l'état du candidat va modifier la liste des
					//	widgets sur laquelle on est en train d'itérer. On reprend donc, par
					//	précaution, l'itération au début...
					
					i = 0;
				}
				else
				{
					i++;
				}
			}
		}
		
		public void SetEmbedder(Widget embedder)
		{
			this.Parent = embedder;
			this.internal_state |= InternalState.Embedded;
		}
		
		
		public static void UpdateEntered(Window window, Message message)
		{
			int index = Widget.entered_widgets.Count;
			
			while (index > 0)
			{
				index--;
				
				if (index < Widget.entered_widgets.Count)
				{
					Widget widget = Widget.entered_widgets[index] as Widget;
					Widget.UpdateEntered (window, widget, message);
				}
			}
		}
		
		public static void UpdateEntered(Window window, Widget widget, Message message)
		{
			Drawing.Point point_in_widget = widget.MapRootToClient (message.Cursor);
			
			if ((widget.Window != window) ||
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
		
		
		public Drawing.Rectangle GetPaintBounds()
		{
			Drawing.Rectangle bounds = this.GetShapeBounds ();
			
			if (this.PaintBoundsCallback != null)
			{
				this.PaintBoundsCallback (this, ref bounds);
			}
			
			return bounds;
		}
		
		
		public virtual Drawing.Rectangle GetShapeBounds()
		{
			return new Drawing.Rectangle (-this.client_info.ox, -this.client_info.oy, this.client_info.width, this.client_info.height);
		}
		
		public virtual Drawing.Rectangle GetClipBounds()
		{
			return this.GetShapeBounds ();
		}
		
		public virtual Drawing.Rectangle GetClipStackBounds()
		{
			//	Calcule le rectangle de clipping (relatif au widget) en tenant compte des
			//	rectangles de clipping de tous les parents.
			
			Drawing.Rectangle clip = this.GetClipBounds ();
			
			if (this.parent != null)
			{
				clip = Drawing.Rectangle.Intersection (clip, this.MapParentToClient (this.parent.GetClipStackBounds ()));
			}
			
			return clip;
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
			
			double z = this.client_info.zoom;
			
			switch (this.client_info.angle)
			{
				case 0:
					result.X = (point.X - this.x1) / z - this.client_info.ox;
					result.Y = (point.Y - this.y1) / z - this.client_info.oy;
					break;
				
				case 90:
					result.X = (point.Y - this.y1) / z - this.client_info.ox;
					result.Y = (this.x2 - point.X) / z - this.client_info.oy;
					break;
				
				case 180:
					result.X = (this.x2 - point.X) / z - this.client_info.ox;
					result.Y = (this.y2 - point.Y) / z - this.client_info.oy;
					break;
				
				case 270:
					result.X = (this.y2 - point.Y) / z - this.client_info.ox;
					result.Y = (point.X - this.x1) / z - this.client_info.oy;
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
			
			point.X += this.client_info.ox;
			point.Y += this.client_info.oy;
			
			
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
			point = this.Window.MapScreenToWindow (point);
			point = this.MapRootToClient (point);
			return point;
		}
		
		public virtual Drawing.Point MapScreenToParent(Drawing.Point point)
		{
			point = this.Window.MapScreenToWindow (point);
			point = this.MapRootToClient (point);
			point = this.MapClientToParent (point);
			return point;
		}
		
		public virtual Drawing.Point MapClientToScreen(Drawing.Point point)
		{
			point = this.MapClientToRoot (point);
			Drawing.Point point_wdo = point;
			point = this.Window.MapWindowToScreen (point);
			System.Diagnostics.Debug.WriteLine (string.Format ("{0} -> {1}", point_wdo, point));
			return point;
		}
		
		public virtual Drawing.Point MapParentToScreen(Drawing.Point point)
		{
			point = this.MapParentToClient (point);
			point = this.MapClientToRoot (point);
			point = this.Window.MapWindowToScreen (point);
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
			
			double z = this.client_info.zoom;
			
			switch (this.client_info.angle)
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
			
			double z = this.client_info.zoom;
			
			switch (this.client_info.angle)
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
		
		
		public virtual Drawing.Transform GetRootToClientTransform()
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
		
		public virtual Drawing.Transform GetClientToRootTransform()
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
		
		
		public virtual Drawing.Transform GetTransformToClient()
		{
			Drawing.Transform t = new Drawing.Transform ();
			
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
			t.Translate (-this.client_info.ox, -this.client_info.oy);
			t.Round ();
			
			return t;
		}
		
		public virtual Drawing.Transform GetTransformToParent()
		{
			Drawing.Transform t = new Drawing.Transform ();
			
			double ox, oy;
			
			switch (this.client_info.angle)
			{
				case 0:		ox = this.x1; oy = this.y1; break;
				case 90:	ox = this.x2; oy = this.y1; break;
				case 180:	ox = this.x2; oy = this.y2; break;
				case 270:	ox = this.x1; oy = this.y2; break;
				default:	throw new System.ArgumentOutOfRangeException ("Invalid angle");
			}
			
			t.Translate (this.client_info.ox, this.client_info.oy);
			t.Scale (this.client_info.zoom);
			t.Rotate (this.client_info.angle);
			t.Translate (ox, oy);
			t.Round ();
			
			return t;
		}
		
		
		public bool IsAncestor(Widget widget)
		{
			if (this.parent == widget)
			{
				return true;
			}
			if (this.parent == null)
			{
				return false;
			}
			
			return this.parent.IsAncestor (widget);
		}
		
		public bool IsDescendant(Widget widget)
		{
			if (widget == null)
			{
				return false;
			}
			
			return widget.IsAncestor (this);
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
					if ((mode & ChildFindMode.SkipHidden) != 0)
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
		
		
		public virtual bool WalkChildren(WalkWidgetCallback callback)
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
		
		public virtual void ExecuteCommand()
		{
			if (this.IsCommand)
			{
				Window window = this.Window;
				
				if (window != null)
				{
					window.QueueCommand (this);
				}
			}
		}
		
		
		internal void InternalUpdateGeometry()
		{
			this.UpdateClientGeometry ();
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
			if (this.textLayout != null)
			{
				this.textLayout.Alignment  = this.Alignment;
				this.textLayout.LayoutSize = this.Client.Size;
			}
		}
		
		protected virtual void UpdateClientGeometry()
		{
			if (this.layout_info == null)
			{
				this.layout_info = new LayoutInfo (this.client_info.width, this.client_info.height);
			}
			
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
					this.layout_info = null;
				}
			}
		}
		
		protected virtual void UpdateMinMaxBasedOnDockedChildren()
		{
			if ((this.internal_state & InternalState.ChildrenDocked) == 0)
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
				if ((child.Dock == DockStyle.None) ||
					(child.Dock == DockStyle.Layout))
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
			if ((this.internal_state & InternalState.AutoMinMax) != 0)
			{
				this.UpdateMinMaxBasedOnDockedChildren ();
			}
			
			if ((this.internal_state & InternalState.ChildrenDocked) == 0)
			{
				return;
			}
			
			System.Diagnostics.Debug.Assert (this.client_info != null);
			System.Diagnostics.Debug.Assert (this.HasChildren);
			
			System.Collections.Queue fill_queue = null;
			Drawing.Rectangle client_rect = this.client_info.Bounds;
			
			this.AdjustDockBounds (ref client_rect);
			
			foreach (Widget child in this.Children)
			{
				if ((child.Dock == DockStyle.None) ||
					(child.Dock == DockStyle.Layout))
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
			
			if (this.layout_info == null)
			{
				return;
			}
			
			System.Diagnostics.Debug.Assert (this.client_info != null);
			System.Diagnostics.Debug.Assert (this.layout_info != null);
			
			if (this.HasChildren)
			{
				double width_diff  = this.client_info.width  - this.layout_info.OriginalWidth;
				double height_diff = this.client_info.height - this.layout_info.OriginalHeight;
				
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
//			System.Diagnostics.Debug.WriteLine ("Paint: " + this.ToString () + " " + this.Bounds.ToString () + " oy=" + this.client_info.oy);
			
			this.OnPreparePaint ();
			
			long cycles = Drawing.Agg.Library.Cycles;
			
			if (this.DebugActive)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("{0}: clip {1}, widget {2}", this.ToString (), graphics.SaveClippingRectangle ().ToString (), this.MapClientToRoot (this.Client.Bounds).ToString ()));
			}
			
			if (this.PaintCheckClipping (repaint))
			{
				Drawing.Rectangle bounds = this.GetClipBounds ();
				
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
					if (this.hypertextList != null)
					{
						this.hypertextList.Clear ();
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
#if false
			else
			{
				Drawing.Rectangle bounds = this.GetPaintBounds ();
				System.Diagnostics.Debug.WriteLine ("Clipped : repaint="+repaint+", bounds="+bounds+", parent="+this.MapClientToParent (bounds));
				System.Diagnostics.Debug.WriteLine ("          widget ="+this.Bounds);
			}
#endif
			
			if (this.DebugActive)
			{
				cycles = Drawing.Agg.Library.Cycles - cycles;
				System.Diagnostics.Debug.WriteLine (string.Format ("{0}: {1} us @ 1.7GHz", this.ToString (), cycles/1700));
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
			
			if (this.PreProcessMessage (message, client_pos) == false)
			{
				return;
			}
			
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
			if (this.IsVisible)
			{
				bool is_entered = this.IsEntered;
				
				switch (message.Type)
				{
					case MessageType.MouseUp:
						//	Le bouton a été relâché. Ceci génère l'événement 'Released' pour signaler
						//	ce relâchement, mais aussi un événement 'Clicked' ou 'DoubleClicked' en
						//	fonction du nombre de clics.
						
						this.OnReleased (new MessageEventArgs (message, pos));
						
						if (is_entered)
						{
							switch (message.ButtonDownCount)
							{
								case 1:	this.OnClicked (new MessageEventArgs (message, pos));		break;
								case 2:	this.OnDoubleClicked (new MessageEventArgs (message, pos));	break;
							}
						}
						break;
					
					case MessageType.MouseDown:
						this.OnPressed (new MessageEventArgs (message, pos));
						break;
				}
				
				
				this.ProcessMessage (message, pos);
			}
			else
			{
				System.Diagnostics.Debug.WriteLine ("Dispatching to invisible widget: "  + this.ToString ());
			}
		}
		
		protected virtual bool PreProcessMessage(Message message, Drawing.Point pos)
		{
			//	...appelé avant que l'événement ne soit traité...
			
			if (this.PreProcessing != null)
			{
				MessageEventArgs e = new MessageEventArgs (message, pos);
				this.PreProcessing (this, e);
				
				if (e.Suppress)
				{
					return false;
				}
			}
			
			if (message.IsMouseType)
			{
				bool reset = true;
				
				if (this.hypertextList != null)
				{
					foreach (HypertextInfo info in this.hypertextList)
					{
						if (info.Bounds.Contains (pos))
						{
							this.SetHypertext (info);
							reset = false;
							break;
						}
					}
				}
				
				if (reset)
				{
					this.SetHypertext (null);
				}
			}
			
			return true;
		}
		
		protected virtual void SetHypertext(HypertextInfo info)
		{
			if (this.hypertext == null)
			{
				if (info == null)
				{
					return;
				}
			}
			else if (this.hypertext.Equals (info))
			{
				return;
			}
			
			this.hypertext = info;
			this.OnHypertextHot ();
		}
		
		protected virtual void ProcessMessage(Message message, Drawing.Point pos)
		{
			//	...appelé pour traiter l'événement...
		}
		
		protected virtual bool PostProcessMessage(Message message, Drawing.Point pos)
		{
			//	...appelé après que l'événement ait été traité...
			
			if (this.PostProcessing != null)
			{
				MessageEventArgs e = new MessageEventArgs (message, pos);
				this.PostProcessing (this, e);
				
				if (e.Suppress)
				{
					return false;
				}
			}
			
			return true;
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
			
			string name = this.Name;
			
			if (name != "")
			{
				//	Ne tient pas compte du nom si celui-ci est absent, sinon le chemin de la
				//	commande risque de contenir des suites de ".." inutiles.
				
				int length = buffer.Length;
				
				if ((length > 0) &&
					(buffer[length-1] != '.'))
				{
					buffer.Append (".");
				}
				
				buffer.Append (name);
			}
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
			
			HypertextInfo info = new HypertextInfo (this.textLayout, e.Bounds, e.Index);
			
			if (this.hypertextList == null)
			{
				this.hypertextList = new System.Collections.ArrayList ();
			}
			
			this.hypertextList.Add (info);
		}
		
		protected virtual void OnPreparePaint()
		{
			if (this.PreparePaint != null)
			{
				this.PreparePaint (this);
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
			if ((this.propagation & PropagationModes.UpChildrenChanged) != 0)
			{
				if (this.parent != null)
				{
					this.parent.OnChildrenChanged ();
				}
			}
			
			if (this.ChildrenChanged != null)
			{
				this.ChildrenChanged (this);
			}
		}
		
		protected virtual void OnParentChanged()
		{
			if ((this.propagation & PropagationModes.DownParentChanged) != 0)
			{
				if (this.HasChildren)
				{
					for (int i = 0; i < this.children.Count; i++)
					{
						this.children[i].OnParentChanged ();
					}
				}
			}
			
			if (this.ParentChanged != null)
			{
				this.ParentChanged (this);
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
			if (this.hypertext != null)
			{
				this.OnHypertextClicked (e);
				
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
			
			this.ExecuteCommand ();
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
			this.Window.MouseCursor = this.MouseCursor;
			
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
				Window window = this.Window;
				
				if (window != null)
				{
					window.MouseCursor = this.parent.MouseCursor;
				}
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
		
		protected virtual void OnHypertextHot()
		{
			Window window = this.Window;
			if (window != null)
			{
				if (this.hypertext == null)
				{
				
					window.MouseCursor = this.MouseCursor;
				}
				else
				{
					window.MouseCursor = MouseCursor.AsHand;
				}
			}
			
			if (this.HypertextHot != null)
			{
				this.HypertextHot (this);
			}
		}
		
		protected virtual void OnHypertextClicked(MessageEventArgs e)
		{
			if (this.HypertextClicked != null)
			{
				e.Message.Consumer = this;
				this.HypertextClicked (this, e);
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
		
		
		public enum PropagationSetting : byte
		{
			None			= 0,
			IncludeChildren	= 1
		}
		
		[System.Flags] public enum PropagationModes : uint
		{
			None				= 0,
			
			UpChildrenChanged	= 0x00000001,		//	propage au parent: ChildrenChanged
			
			DownParentChanged	= 0x00010000,		//	propage aux enfants: ParentChanged
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
			
			internal void SetOffset(double ox, double oy)
			{
				this.ox = ox;
				this.oy = oy;
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
			
			public Drawing.Point			Offset
			{
				get { return new Drawing.Point (this.ox, this.oy); }
			}
			
			
			internal double					width	= 0.0;
			internal double					height	= 0.0;
			internal short					angle	= 0;
			internal double					zoom	= 1.0;
			internal double					ox		= 0.0;
			internal double					oy		= 0.0;
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
			
			public Widget					this[int index]
			{
				get { return this.list[index] as Widget; }
			}
			
			
			internal void Replace(Widget old_widget, Widget new_widget)
			{
				if (old_widget == new_widget)
				{
					return;
				}
				
				int pos1 = this.list.IndexOf (old_widget);
				int pos2 = this.list.IndexOf (new_widget);
				
				if (pos1 < 0)
				{
					throw new System.ArgumentException ("Widget not in collection.", "old_widget");
				}
				if (pos2 < 0)
				{
					this.PreRemove (old_widget);
					this.PreInsert (new_widget);
					
					this.list[pos1] = new_widget;
					
					if (this.array != null)
					{
						this.array[pos1] = new_widget;
					}
					
					this.NotifyChanged ();
				}
				else
				{
					this.list[pos1] = new_widget;
					this.list[pos2] = old_widget;
					
					if (this.array != null)
					{
						this.array[pos1] = new_widget;
						this.array[pos2] = old_widget;
					}
					
					this.NotifyChanged ();
				}
			}
			
			internal void InsertAt(int index, Widget widget)
			{
				if (widget == null)
				{
					throw new System.ArgumentException ("Widget is not valid.", "widget");
				}
				
				this.PreInsert (widget);
				this.array = null;
				this.list.Insert (index, widget);
				this.NotifyChanged ();
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
				widget.OnParentChanged ();
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
				widget.OnParentChanged ();
			}
			
			private void NotifyChanged()
			{
				if (this.widget.suspendCounter == 0)
				{
					this.widget.HandleChildrenChanged ();
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
			
			object							System.Collections.IList.this[int index]
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
		
		protected sealed class HypertextInfo : System.ICloneable, System.IComparable
		{
			internal HypertextInfo(TextLayout layout, Drawing.Rectangle bounds, int index)
			{
				this.layout = layout;
				this.bounds = bounds;
				this.index  = index;
			}
			
			
			#region ICloneable Members
			public object Clone()
			{
				return new HypertextInfo (this.layout, this.bounds, this.index);
			}
			#endregion

			#region IComparable Members
			public int CompareTo(object obj)
			{
				if (obj == null)
				{
					return 1;
				}
				
				HypertextInfo that = obj as HypertextInfo;
				
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
		
		
		private AnchorStyles					anchor;
		
		private DockStyle						dock;
		private Drawing.Margins					dock_margins;
		
		private ClientInfo						client_info = new ClientInfo ();
		
		private InternalState					internal_state;
		private WidgetState						widget_state;
		
		private LayoutInfo						layout_info;
		private LayoutFlags						layout_flags;
		private byte							layout_arg1;
		private byte							layout_arg2;
		private Drawing.Margins					layout_margins;
		
		private PropagationModes				propagation;
		
		protected Drawing.Color					backColor;
		protected Drawing.Color					foreColor;
		protected double						x1, y1, x2, y2;
		protected Drawing.Size					minSize;
		protected Drawing.Size					maxSize;
		protected System.Collections.ArrayList	hypertextList;
		protected HypertextInfo					hypertext;
		
		protected WidgetCollection				children;
		protected Widget						parent;
		protected string						name;
		protected int							index;
		protected TextLayout					textLayout;
		protected ContentAlignment				alignment;
		protected int							suspendCounter;
		protected int							tabIndex;
		protected TabNavigationMode				tabNavigationMode;
		protected Shortcut						shortcut;
		protected double						defaultFontHeight;
		protected MouseCursor					mouseCursor;
		
		static System.Collections.ArrayList		entered_widgets = new System.Collections.ArrayList ();
		static System.Collections.ArrayList		alive_widgets   = new System.Collections.ArrayList ();
	}
}
