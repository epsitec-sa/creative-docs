//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	using ContentAlignment = Drawing.ContentAlignment;
	using BundleAttribute  = Support.BundleAttribute;
	
	
	public delegate bool WalkWidgetCallback(Widget widget);
	public delegate void PaintBoundsCallback(Widget widget, ref Drawing.Rectangle bounds);
	
	#region WidgetState enum
	[System.Flags] public enum WidgetState : uint
	{
		ActiveNo			= 0,
		ActiveYes			= 1,
		ActiveMaybe			= 2,
		ActiveMask			= ActiveNo | ActiveYes | ActiveMaybe,
		
		
		None				= 0x00000000,		//	=> neutre
		Enabled				= 0x00010000,		//	=> pas grisé
		Focused				= 0x00020000,		//	=> reçoit les événements clavier
		Entered				= 0x00040000,		//	=> contient la souris
		Selected			= 0x00080000,		//	=> sélectionné
		Engaged				= 0x00100000,		//	=> pression en cours
		Error				= 0x00200000,		//	=> signale une erreur
	}
	#endregion
	
	#region InternalState enum
	[System.Flags] public enum InternalState : uint
	{
		None				= 0,
		
		ChildrenChanged		= 0x00000001,		//	certains enfants ont changé
		LayoutChanged		= 0x00000002,		//	le layout a changé
		ChildrenDocked		= 0x00000004,		//	certains enfants spécifient un DockStyle
		
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
		AutoDoubleClick		= 0x00400000,
		AutoResolveResRef	= 0x00800000,		//	une référence à une ressource [res:...] est remplacée automatiquement
		
		PossibleContainer	= 0x01000000,		//	widget peut être la cible d'un drag & drop en mode édition
		EditionEnabled		= 0x02000000,		//	widget peut être édité
		
		InheritFocus		= 0x10000000,
		SyncPaint			= 0x20000000,		//	peinture synchrone
		
		DebugActive			= 0x80000000		//	widget marqué pour le debug
	}
	#endregion
	
	#region AnchorStyles enum
	[System.Flags] public enum AnchorStyles : byte
	{
		None				= 0x00,
		Top					= 0x10,
		Bottom				= 0x20,
		Left				= 0x40,
		Right				= 0x80,
		
		TopLeft				= Top | Left,
		BottomLeft			= Bottom | Left,
		TopRight			= Top | Right,
		BottomRight			= Bottom | Right,
		LeftAndRight		= Left | Right,
		TopAndBottom		= Top | Bottom,
		All					= TopAndBottom | LeftAndRight
	}
	#endregion
	
	#region DockStyle enum
	public enum DockStyle : byte
	{
		None				= 0,
		
		Top					= 1,				//	colle en haut
		Bottom				= 2,				//	colle en bas
		Left				= 3,				//	colle à gauche
		Right				= 4,				//	colle à droite
		Fill				= 5,				//	remplit tout
	}
	#endregion
	
	#region ContainerLayoutMode enum
	public enum ContainerLayoutMode : byte
	{
		None				= 0,				//	pas de préférence					
		HorizontalFlow		= 1,				//	remplit horizontalement
		VerticalFlow		= 2					//	remplit verticalement
	}
	#endregion
	
	#region LayoutStyles enum
	[System.Flags] public enum LayoutStyles : byte
	{
		Manual			= 0,
			
		DockLeft		= DockStyle.Left,
		DockRight		= DockStyle.Right,
		DockTop			= DockStyle.Top,
		DockBottom		= DockStyle.Bottom,
		DockFill		= DockStyle.Fill,
			
		AnchorLeft		= AnchorStyles.Left,
		AnchorRight		= AnchorStyles.Right,
		AnchorTop		= AnchorStyles.Top,
		AnchorBottom	= AnchorStyles.Bottom,
			
		MaskDock		= 0x0F,
		MaskAnchor		= 0xF0,
	}
	#endregion
	
	
	/// <summary>
	/// La classe Widget implémente la classe de base dont dérivent tous les
	/// widgets de l'interface graphique ("controls" dans l'appellation Windows).
	/// </summary>
	public class Widget : System.IDisposable, Support.IBundleSupport, Support.ICommandDispatcherHost, Support.Data.IPropertyProvider
	{
		public Widget()
		{
			if (Support.ObjectBundler.IsBooting)
			{
				//	N'initialise rien, car cela prend passablement de temps... et de toute
				//	manière, on n'a pas besoin de toutes ces informations pour pouvoir
				//	utiliser IBundleSupport.
				
				return;
			}
			
			this.widget_id = Widget.next_widget_id++;
			
			if (Widget.DebugDispose)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("{1}+ Created {0}", this.GetType ().Name, this.widget_id));
			}
			
			this.internal_state |= InternalState.Visible;
			this.internal_state |= InternalState.AutoCapture;
			this.internal_state |= InternalState.AutoMnemonic;
			this.internal_state |= InternalState.AutoResolveResRef;
			
			this.widget_state   |= WidgetState.Enabled;
			
			this.default_font_height = System.Math.Floor(this.DefaultFont.LineHeight*this.DefaultFontSize);
			this.alignment           = this.DefaultAlignment;
			this.layout              = LayoutStyles.Manual;
			this.back_color          = Drawing.Color.Empty;
			
			this.Size = new Drawing.Size (this.DefaultWidth, this.DefaultHeight);
			
			this.min_size = this.DefaultMinSize;
			this.max_size = this.DefaultMaxSize;
			
			lock (Widget.alive_widgets)
			{
				Widget.alive_widgets.Add (new System.WeakReference (this));
			}
		}
		
		public Widget(Widget embedder) : this()
		{
			this.SetEmbedder (embedder);
		}
		
		
		~Widget()
		{
			this.Dispose (false);
		}
		
		static Widget()
		{
			Drawing.Font.Initialise ();
			
			Res.Initialise (typeof (Widget), "Common.Widgets");
			
			Support.ImageProvider.Initialise ();
			Widgets.CommandState.Initialise ();
			
			Support.ObjectBundler.Initialise ();
			Support.ImageProvider.Initialise ();
			
			System.Threading.Thread          thread  = System.Threading.Thread.CurrentThread;
			System.Globalization.CultureInfo culture = thread.CurrentCulture;
			
			thread.CurrentUICulture = culture;
		}
		
		
		public static void Initialise()
		{
			//	En appelant cette méthode statique, on peut garantir que le constructeur
			//	statique de Widget a bien été exécuté.
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
		
		public virtual string				BundleName
		{
			get
			{
				string name = this.Name;
				
				if (name == "")
				{
					return null;
				}
				
				return name;
			}
		}
		
		void Support.IBundleSupport.AttachResourceManager(Support.ResourceManager resource_manager)
		{
			System.Diagnostics.Debug.Assert (this.resource_manager == null);
			System.Diagnostics.Debug.Assert (resource_manager != null);
			
			this.ResourceManager = resource_manager;
		}
		
		public virtual void RestoreFromBundle(Support.ObjectBundler bundler, Support.ResourceBundle bundle)
		{
//			this.SuspendLayout ();
			
			System.Diagnostics.Debug.Assert (this.resource_manager != null);
			System.Diagnostics.Debug.Assert (this.resource_manager == bundler.ResourceManager);
			
			
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
			
			Support.ResourceBundle.FieldList validator_list = bundle["validators"].AsList;
			
			if (validator_list != null)
			{
				foreach (Support.ResourceBundle.Field field in validator_list)
				{
					Support.ResourceBundle validator_bundle = field.AsBundle;
					Validators.AbstractValidator validator = bundler.CreateFromBundle (validator_bundle) as Validators.AbstractValidator;
					
					validator.InternalAttach (this);
				}
			}
			
//			this.ResumeLayout ();
		}
		
		public virtual void SerializeToBundle(Support.ObjectBundler bundler, Support.ResourceBundle bundle)
		{
			if (this.HasChildren)
			{
				System.Collections.ArrayList list    = new System.Collections.ArrayList ();
				Widget[]                     widgets = this.Children.Widgets;
				
				for (int i = 0; i < widgets.Length; i++)
				{
					if (! widgets[i].IsEmbedded)
					{
						Support.ResourceBundle child_bundle = bundler.CreateEmptyBundle (widgets[i].BundleName);
						
						bundler.FillBundleFromObject (child_bundle, widgets[i]);
						
						list.Add (child_bundle);
					}
				}
				
				if (list.Count > 0)
				{
					Support.ResourceBundle.Field field = bundle.CreateField (Support.ResourceFieldType.List, list);
					field.SetName ("widgets");
					bundle.Add (field);
				}
			}
			
			if (this.validator != null)
			{
				System.Collections.ArrayList list       = new System.Collections.ArrayList ();
				Support.IValidator[]         validators = Support.MulticastValidator.ToArray (this.validator);
				
				for (int i = 0; i < validators.Length; i++)
				{
					if (this.ShouldSerializeValidator (validators[i]) == false)
					{
						//	La classe ne veut pas que ce validateur soit sérialisé. On le saute donc tout
						//	simplement.
					}
					else if (validators[i] is Support.IBundleSupport)
					{
						Support.ResourceBundle validator_bundle = bundler.CreateEmptyBundle (string.Format (System.Globalization.CultureInfo.InvariantCulture, "v{0}", i));
						
						bundler.FillBundleFromObject (validator_bundle, validators[i]);
						
						list.Add (validator_bundle);
					}
					else
					{
						System.Diagnostics.Debug.WriteLine (string.Format ("Validator {0} cannot be serialized.", validators[i].GetType ().Name));
					}
				}
				
				if (list.Count > 0)
				{
					Support.ResourceBundle.Field field = bundle.CreateField (Support.ResourceFieldType.List, list);
					field.SetName ("validators");
					bundle.Add (field);
				}
			}
		}
		#endregion
		
		#region IPropertyProvider Members
		public string[] GetPropertyNames()
		{
			if (this.property_hash == null)
			{
				return new string[0];
			}
			
			string[] names = new string[this.property_hash.Count];
			this.property_hash.Keys.CopyTo (names, 0);
			System.Array.Sort (names);
			
			return names;
		}
		
		public void SetProperty(string key, object value)
		{
			if (this.property_hash == null)
			{
				this.property_hash = new System.Collections.Hashtable ();
			}
			
			this.property_hash[key] = value;
		}
		
		public object GetProperty(string key)
		{
			if (this.property_hash != null)
			{
				return this.property_hash[key];
			}
			
			return null;
		}
		
		public bool IsPropertyDefined(string key)
		{
			if (this.property_hash != null)
			{
				return this.property_hash.Contains (key);
			}
			
			return false;
		}
		
		public void ClearProperty(string key)
		{
			if (this.property_hash != null)
			{
				this.property_hash.Remove (key);
			}
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (Widget.DebugDispose)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("{2}- {0} widget {1}", (disposing ? "Disposing" : "Collecting"), this.ToString (), this.widget_id.ToString ()));
			}
			
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
				
				if (this.Disposed != null)
				{
					this.Disposed (this);
					this.Disposed = null;
				}
			}
		}
		
		
		#region Debugging Support
		public bool							DebugActive
		{
			get
			{
				return (this.internal_state & InternalState.DebugActive) != 0;
			}
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
		
		public static bool					DebugDispose
		{
			get
			{
				return Widget.debug_dispose;
			}
			set
			{
				Widget.debug_dispose = value;
			}
		}
		
		public static int					DebugAliveWidgetsCount
		{
			get
			{
				System.Collections.ArrayList alive = new System.Collections.ArrayList ();
				
				lock (Widget.alive_widgets)
				{
					//	Passe en revue tous les widgets connus (même les décédés) et reconstruit
					//	une liste ne contenant que les widgets vivants :
					
					foreach (System.WeakReference weak_ref in Widget.alive_widgets)
					{
						if (weak_ref.IsAlive)
						{
							alive.Add (weak_ref);
						}
					}
					
					//	Remplace la liste des widgets connus par la liste à jour qui vient d'être
					//	construite :
					
					Widget.alive_widgets = alive;
				}
				
				return alive.Count;
			}
		}
		
		public static Widget[]				DebugAliveWidgets
		{
			get
			{
				System.Collections.ArrayList alive = new System.Collections.ArrayList ();
				
				lock (Widget.alive_widgets)
				{
					foreach (System.WeakReference weak_ref in Widget.alive_widgets)
					{
						if (weak_ref.IsAlive)
						{
							alive.Add (weak_ref.Target);
						}
					}
				}
				
				Widget[] widgets = new Widget[alive.Count];
				alive.CopyTo (widgets);
				
				return widgets;
			}
		}
		#endregion
		
		public virtual LayoutStyles					Layout
		{
			get
			{
				return this.layout;
			}
			set
			{
				if (this.layout != value)
				{
					Drawing.Size size = this.Size;
					
					this.Invalidate ();
					
					LayoutStyles dock_old = this.layout & LayoutStyles.MaskDock;
					LayoutStyles dock_new = value & LayoutStyles.MaskDock;
					
					if (((dock_old == LayoutStyles.DockLeft) && (dock_new == LayoutStyles.DockRight)) ||
						((dock_old == LayoutStyles.DockRight) && (dock_new == LayoutStyles.DockLeft)) ||
						((dock_old == LayoutStyles.DockTop) && (dock_new == LayoutStyles.DockBottom)) ||
						((dock_old == LayoutStyles.DockBottom) && (dock_new == LayoutStyles.DockTop)) ||
						(dock_old == LayoutStyles.Manual))
					{
						//	Ne change pas la géométrie, puisque le docking reste défini selon les
						//	mêmes grandeurs (gauche <-> droite ou haut <-> bas), ou qu'il n'y avait
						//	pas de docking actif avant.
					}
					else
					{
						//	Le style de docking change; il faut donc mieux remettre les dimensions
						//	par défaut
						
						this.x2 = this.x1 + this.DefaultWidth;
						this.y2 = this.y1 + this.DefaultHeight;
					}
					
					this.layout = value;
					
					if ((this.parent != null) &&
						(this.IsLayoutSuspended == false))
					{
						//	Si le widget a un parent, il faut donner l'occasion au parent de
						//	repositionner tous ses enfants (donc nous aussi) pour tenir compte
						//	de notre nouveau mode de docking.
						
						this.parent.UpdateChildrenLayout ();
						
						if (this.Size != size)
						{
							this.UpdateClientGeometry ();
							this.OnSizeChanged ();
						}
						this.Invalidate ();
					}
				}
			}
		}
		
		
		[Bundle]			public AnchorStyles		Anchor
		{
			get
			{
				return (AnchorStyles) (this.layout & LayoutStyles.MaskAnchor);
			}
			set
			{
				this.Layout = (LayoutStyles) value;
			}
		}
		
		[Bundle]			public Drawing.Margins	AnchorMargins
		{
			get
			{
				return this.anchor_margins;
			}
			set
			{
				if (this.anchor_margins != value)
				{
					this.anchor_margins = value;
					
					if ((this.parent != null) &&
						(this.IsLayoutSuspended == false))
					{
						//	Si le widget a un parent, il faut donner l'occasion au parent de
						//	repositionner tous ses enfants (donc nous aussi) pour tenir compte
						//	de nos nouvelles marges d'ancrage.
						
						this.parent.UpdateChildrenLayout ();
					}
				}
			}
		}
		
		[Bundle]			public DockStyle		Dock
		{
			get
			{
				return (DockStyle) (this.layout & LayoutStyles.MaskDock);
			}
			set
			{
				this.Layout = (LayoutStyles) value;
			}
		}
		
		[Bundle]			public Drawing.Margins	DockPadding
		{
			get
			{
				return this.dock_padding;
			}
			set
			{
				if (this.dock_padding != value)
				{
					this.dock_padding = value;
					this.UpdateChildrenLayout ();
				}
			}
		}
		
		[Bundle]			public Drawing.Margins	DockMargins
		{
			get
			{
				return this.dock_margins;
			}
			set
			{
				if (this.dock_margins != value)
				{
					this.dock_margins = value;
					
					if ((this.parent != null) &&
						(this.IsLayoutSuspended == false))
					{
						//	Si le widget a un parent, il faut donner l'occasion au parent de
						//	repositionner tous ses enfants (donc nous aussi) pour tenir compte
						//	de nos nouvelles marges de docking.
						
						this.parent.UpdateChildrenLayout ();
					}
				}
			}
		}
		
		[Bundle] public ContainerLayoutMode			ContainerLayoutMode
		{
			get
			{
				if ((this.internal_state & InternalState.PreferXLayout) != 0)
				{
					return ContainerLayoutMode.HorizontalFlow;
				}
				else
				{
					return ContainerLayoutMode.VerticalFlow;
				}
			}
			set
			{
				if (this.ContainerLayoutMode != value)
				{
					switch (value)
					{
						case ContainerLayoutMode.HorizontalFlow:
							this.internal_state |= InternalState.PreferXLayout;
							break;
						case ContainerLayoutMode.VerticalFlow:
							this.internal_state &= ~ InternalState.PreferXLayout;
							break;
					}
					
					this.UpdateChildrenLayout ();
				}
			}
		}
		
		public Layouts.LayoutInfo					LayoutInfo
		{
			get { return this.layout_info; }
		}
		
		
		public MouseCursor							MouseCursor
		{
			get
			{
				return this.mouse_cursor == null ? MouseCursor.Default : this.mouse_cursor;
			}
			set
			{
				if (this.mouse_cursor != value)
				{
					this.mouse_cursor = value;
					
					if (this.IsEntered)
					{
						this.Window.MouseCursor = this.MouseCursor;
					}
				}
			}
		}
		
		public virtual Drawing.Color				BackColor
		{
			get
			{
				return this.back_color;
			}
			set
			{
				if (this.back_color != value)
				{
					this.back_color = value;
					this.Invalidate (InvalidateReason.ColorChanged);
				}
			}
		}
		
		
		
		public double								Top
		{
			get { return this.y2; }
			set { this.Bounds = new Drawing.Rectangle (this.x1, this.y1, this.x2 - this.x1, value - this.y1); }
		}
		
		public double								Left
		{
			get { return this.x1; }
			set { this.Bounds = new Drawing.Rectangle (value, this.y1, this.x2 - value, this.y2 - this.y1); }
		}
		
		public double								Bottom
		{
			get { return this.y1; }
			set { this.Bounds = new Drawing.Rectangle (this.x1, value, this.x2 - this.x1, this.y2 - value); }
		}
		
		public double								Right
		{
			get { return this.x2; }
			set { this.Bounds = new Drawing.Rectangle (this.x1, this.y1, value - this.x1, this.y2 - this.y1); }
		}
		
		public Drawing.Rectangle					Bounds
		{
			get { return new Drawing.Rectangle (this.x1, this.y1, this.x2 - this.x1, this.y2 - this.y1); }
			set
			{
				if (this.Bounds != value)
				{
					if (this.parent != null)
					{
						Drawing.Rectangle bounds    = value;
						Drawing.Rectangle container = this.parent.Client.Bounds;
						Drawing.Margins   margins   = new Drawing.Margins (bounds.Left - container.Left, container.Right - bounds.Right,
							/**/										   container.Top - bounds.Top, bounds.Bottom - container.Bottom);
						
						this.anchor_margins = margins;
					}
					
					this.SetBounds (value.X, value.Y, value.X + value.Width, value.Y + value.Height);
				}
			}
		}
		
		
		[Bundle]			public Drawing.Point	Location
		{
			get { return new Drawing.Point (this.x1, this.y1); }
			set
			{
				if ((this.Location != value) &&
					(this.Dock == DockStyle.None))
				{
					if (this.parent != null)
					{
						Drawing.Rectangle bounds    = new Drawing.Rectangle (value, this.Size);
						Drawing.Rectangle container = this.parent.Client.Bounds;
						Drawing.Margins   margins   = new Drawing.Margins (bounds.Left - container.Left, container.Right - bounds.Right,
							/**/										   container.Top - bounds.Top, bounds.Bottom - container.Bottom);
						
						this.anchor_margins = margins;
					}
					
					this.SetBounds (value.X, value.Y, value.X + this.x2 - this.x1, value.Y + this.y2 - this.y1);
					
					if (this.Layout != LayoutStyles.Manual)
					{
						this.ForceLayout ();
					}
				}
			}
		}
		
		[Bundle]			public Drawing.Size		Size
		{
			get { return new Drawing.Size (this.x2 - this.x1, this.y2 - this.y1); }
			set
			{
				if (this.Size != value)
				{
					if (this.parent != null)
					{
						Drawing.Rectangle bounds    = new Drawing.Rectangle (this.Location, value);
						Drawing.Rectangle container = this.parent.Client.Bounds;
						Drawing.Margins   margins   = new Drawing.Margins (bounds.Left - container.Left, container.Right - bounds.Right,
							/**/										   container.Top - bounds.Top, bounds.Bottom - container.Bottom);
						
						this.anchor_margins = margins;
					}
					
					this.SetBounds (this.x1, this.y1, this.x1 + value.Width, this.y1 + value.Height);
					
					if (this.Layout != LayoutStyles.Manual)
					{
						this.ForceLayout ();
					}
				}
			}
		}
		
		
		public double								Width
		{
			get { return this.x2 - this.x1; }
			set
			{
				if (this.Width != value)
				{
					this.SetBounds (this.x1, this.y1, this.x1 + value, this.y2);
				}
			}
		}
		
		public double								Height
		{
			get { return this.y2 - this.y1; }
			set
			{
				if (this.Height != value)
				{
					this.SetBounds (this.x1, this.y1, this.x2, this.y1 + value);
				}
			}
		}
		
		public ContentAlignment						Alignment
		{
			get
			{
				return this.alignment;
			}
			set
			{
				if (this.alignment != value)
				{
					this.alignment = value;
					this.UpdateTextLayout ();
					this.Invalidate ();
				}
			}
		}
		
		public ClientInfo							Client
		{
			get { return this.client_info; }
		}
		
		public virtual Drawing.Rectangle			InnerBounds
		{
			get { return this.Client.Bounds; }
		}
		
		[Bundle] public virtual Drawing.Size		MinSize
		{
			get
			{
				return this.min_size;
			}
			set
			{
				if (this.min_size != value)
				{
					this.min_size = value;
					this.OnMinSizeChanged ();
				}
			}
		}
		
		[Bundle] public virtual Drawing.Size		MaxSize
		{
			get
			{
				return this.max_size;
			}
			set
			{
				if (this.max_size != value)
				{
					this.max_size = value;
					this.OnMaxSizeChanged ();
				}
			}
		}
		
		public virtual Drawing.Size					AutoMinSize
		{
			get
			{
				return this.auto_min_size;
			}
			set
			{
				if (this.auto_min_size != value)
				{
					this.auto_min_size = value;
					this.OnMinSizeChanged ();
				}
			}
		}
		
		public virtual Drawing.Size					AutoMaxSize
		{
			get
			{
				return this.auto_max_size;
			}
			set
			{
				if (this.auto_max_size != value)
				{
					this.auto_max_size = value;
					this.OnMaxSizeChanged ();
				}
			}
		}
		
		public Drawing.Size							RealMinSize
		{
			get
			{
				double width  = System.Math.Max (this.min_size.Width, this.auto_min_size.Width);
				double height = System.Math.Max (this.min_size.Height, this.auto_min_size.Height);
				
				return new Drawing.Size (width, height);
			}
		}
		
		public Drawing.Size							RealMaxSize
		{
			get
			{
				double width  = System.Math.Min (this.max_size.Width, this.auto_max_size.Width);
				double height = System.Math.Min (this.max_size.Height, this.auto_max_size.Height);
				
				return new Drawing.Size (width, height);
			}
		}
		
		
		public virtual ContentAlignment				DefaultAlignment
		{
			get { return ContentAlignment.MiddleLeft; }
		}

		public virtual Drawing.Font					DefaultFont
		{
			get { return Drawing.Font.DefaultFont; }
		}
		
		public virtual double						DefaultFontSize
		{
			get { return Drawing.Font.DefaultFontSize; }
		}
		
		public virtual double						DefaultWidth
		{
			get { return 80; }
		}
		public virtual double						DefaultHeight
		{
			get { return 20; }
		}
		public virtual double						DefaultFontHeight
		{
			get { return this.default_font_height; }
		}
		
		public virtual Drawing.Size					DefaultMinSize
		{
			get { return new Drawing.Size (4, 4); }
		}
		
		public virtual Drawing.Size					DefaultMaxSize
		{
			get { return Drawing.Size.Infinite; }
		}
		
		public virtual Drawing.Size					PreferredSize
		{
			get { return new Drawing.Size (this.DefaultWidth, this.DefaultHeight); }
		}
		
		public virtual Drawing.Point				BaseLine
		{
			get
			{
				return Drawing.Point.Empty;
			}
		}
		
		
		public bool									IsCommand
		{
			get { return (this.command != null); }
		}
		
		public virtual bool							IsEnabled
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
		
		public virtual bool							IsFrozen
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
		
		public virtual bool							IsLayoutSuspended
		{
			get
			{
				if (this.suspend_counter > 0)
				{
					return true;
				}
//				if (this.parent != null)
//				{
//					return this.parent.IsLayoutSuspended;
//				}
				
				return false;
			}
		}
		
		public virtual bool							IsVisible
		{
			get
			{
				if ((this.IsVisibleFlagSet == false) ||
					(this.parent == null))
				{
					return false;
				}
				
				return this.parent.IsVisible;
			}
		}
		
		public virtual bool							IsVisibleFlagSet
		{
			get
			{
				return (this.internal_state & InternalState.Visible) != 0;
			}
		}

		public bool									IsActive
		{
			get
			{
				return this.ActiveState == WidgetState.ActiveYes;
			}
		}
		
		public bool									IsFocused
		{
			get
			{
				if (this.IsFocusedFlagSet)
				{
					Window window = this.Window;
				
					if (window == null)
					{
						return false;
					}
				
					return window.IsFocused;
				}
				
				if ((this.InheritFocus) &&
					(this.parent != null))
				{
					return this.parent.IsFocused;
				}
				
				return false;
			}
		}
		
		public bool									IsFocusedFlagSet
		{
			get
			{
				return (this.widget_state & WidgetState.Focused) != 0;
			}
		}
		
		public bool									IsEntered
		{
			get { return (this.widget_state & WidgetState.Entered) != 0; }
		}
		
		public bool									IsSelected
		{
			get { return (this.widget_state & WidgetState.Selected) != 0; }
		}
		
		public bool									IsEngaged
		{
			get { return (this.widget_state & WidgetState.Engaged) != 0; }
		}
		
		public bool									IsError
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
		
		public bool									IsEmbedded
		{
			get { return (this.internal_state & InternalState.Embedded) != 0; }
		}
		
		public bool									IsEditionEnabled
		{
			get
			{
				if ((this.internal_state & InternalState.EditionEnabled) != 0)
				{
					return true;
				}
				
				if (this.parent != null)
				{
					return this.parent.IsEditionEnabled;
				}
				
				return false;
			}
			set
			{
				bool enabled = (this.internal_state & InternalState.EditionEnabled) != 0;
				
				if (enabled != value)
				{
					if (value)
					{
						this.internal_state |= InternalState.EditionEnabled;
					}
					else
					{
						this.internal_state &= ~InternalState.EditionEnabled;
					}
				}
			}
		}
		
		public bool									IsValid
		{
			get
			{
				if (this.Validator != null)
				{
					return this.Validator.IsValid;
				}
				
				//	Un widget qui n'a pas de validateur est considéré comme étant en tout
				//	temps valide.
				
				return true;
			}
		}
		
		
		public bool									AutoCapture
		{
			get
			{
				return (this.internal_state & InternalState.AutoCapture) != 0;
			}
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
		
		public bool									AutoMinMax
		{
			get
			{
				return (this.internal_state & InternalState.AutoMinMax) != 0;
			}
			set
			{
				if (this.AutoMinMax != value)
				{
					if (value)
					{
						this.internal_state |= InternalState.AutoMinMax;
					}
					else
					{
						this.internal_state &= ~ InternalState.AutoMinMax;
					}
					
					this.UpdateChildrenLayout ();
				}
			}
		}
		
		public bool									AutoFocus
		{
			get
			{
				return (this.internal_state & InternalState.AutoFocus) != 0;
			}
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
		
		public bool									AutoEngage
		{
			get
			{
				return (this.internal_state & InternalState.AutoEngage) != 0;
			}
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
		
		public bool									AutoRepeatEngaged
		{
			get
			{
				return (this.internal_state & InternalState.AutoRepeatEngaged) != 0;
			}
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
		
		public bool									AutoToggle
		{
			get
			{
				return (this.internal_state & InternalState.AutoToggle) != 0;
			}
			set
			{
				if (value)
				{
					this.internal_state |= InternalState.AutoToggle;
				}
				else
				{
					this.internal_state &= ~InternalState.AutoToggle;
				}
			}
		}
		
		public bool									AutoMnemonic
		{
			get
			{
				return (this.internal_state & InternalState.AutoMnemonic) != 0;
			}
			set
			{
				if (this.AutoMnemonic != value)
				{
					if (value)
					{
						this.internal_state |= InternalState.AutoMnemonic;
					}
					else
					{
						this.internal_state &= ~InternalState.AutoMnemonic;
					}
					
					this.ResetMnemonicShortcut ();
				}
			}
		}
		
		public bool									AutoResolveResRef
		{
			get
			{
				return (this.resource_manager != null) && ((this.internal_state & InternalState.AutoResolveResRef) != 0);
			}
			set
			{
				if (this.AutoResolveResRef != value)
				{
					if (value)
					{
						this.internal_state |= InternalState.AutoResolveResRef;
					}
					else
					{
						this.internal_state &= ~InternalState.AutoResolveResRef;
					}
				}
			}
		}
		
		
		public bool									InheritFocus
		{
			get
			{
				return (this.internal_state & InternalState.InheritFocus) != 0;
			}
			set
			{
				if (value)
				{
					this.internal_state |= InternalState.InheritFocus;
				}
				else
				{
					this.internal_state &= ~InternalState.InheritFocus;
				}
			}
		}
		
		
		protected InternalState						InternalState
		{
			get { return this.internal_state; }
			set { this.internal_state = value; }
		}
		
		
		public WidgetState							State
		{
			get
			{
				WidgetState state = this.widget_state;
				
				if ((this.InheritFocus) &&
					(this.parent != null))
				{
					if (this.parent.IsFocused)
					{
						state |= WidgetState.Focused;
					}
				}
				
				return state;
			}
		}
		
		public WidgetState							ActiveState
		{
			get
			{
				return this.widget_state & WidgetState.ActiveMask;
			}
			set
			{
				WidgetState active = this.widget_state & WidgetState.ActiveMask;
				
				System.Diagnostics.Debug.Assert ((value & WidgetState.ActiveMask) == value);
				
				if (active != value)
				{
					this.widget_state &= ~WidgetState.ActiveMask;
					this.widget_state |= value & WidgetState.ActiveMask;
					this.OnActiveStateChanged ();
					this.Invalidate (InvalidateReason.ActiveStateChanged);
				}
			}
		}
		
		public WidgetState							PaintState
		{
			get
			{
				WidgetState mask  = WidgetState.ActiveMask |
					/**/			WidgetState.Entered |
					/**/			WidgetState.Engaged |
					/**/			WidgetState.Selected |
					/**/			WidgetState.Error;
				
				if (this.InheritFocus)
				{
					mask |= WidgetState.Focused;
				}
				
				WidgetState state = this.State & mask;
				
				if (this.IsEnabled)
				{
					state |= WidgetState.Enabled;
				}
				
				if (((state & WidgetState.Focused) == 0) &&
					(this.IsFocused))
				{
					state |= WidgetState.Focused;
				}
				
				return state;
			}
		}
		
		public int									ZOrder
		{
			get
			{
				//	Retourne la position dans la "pile" des widgets, en ne considérant que les
				//	frères et soeurs. 0 => widget sur le sommet de la pile des widgets.
				
				if (this.parent == null)
				{
					return -1;
				}
				
				Widget[] siblings = this.parent.children.Widgets;
				int      bottom_z = siblings.Length;
				
				for (int i = 0; i < bottom_z; i++)
				{
					if (siblings[i] == this)
					{
						return bottom_z - i - 1;
					}
				}
				
				return -1;
			}
			set
			{
				if (this.parent == null)
				{
					throw new System.InvalidOperationException ("Cannot change Z-order of an orphan.");
				}
				
				if ((value < 0) ||
					(value >= this.parent.children.Count))
				{
					throw new System.ArgumentOutOfRangeException ("value", value, "Invalid Z-order specified.");
				}
				
				this.parent.children.ChangeZOrder (this, value);
				
				System.Diagnostics.Debug.Assert (this.ZOrder == value);
			}
		}
		
		public bool									ContainsFocus
		{
			get
			{
				if (this.IsFocusedFlagSet)
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
		
		public virtual bool							CanFocus
		{
			get { return ((this.internal_state & InternalState.Focusable) != 0) && !this.IsFrozen; }
		}
		
		public bool									CanSelect
		{
			get { return ((this.internal_state & InternalState.Selectable) != 0) && !this.IsFrozen; }
		}
		
		public bool									CanEngage
		{
			get { return ((this.internal_state & InternalState.Engageable) != 0) && this.IsEnabled && !this.IsFrozen; }
		}
		
		public bool									AcceptThreeState
		{
			get
			{
				return (this.internal_state & InternalState.AcceptThreeState) != 0;
			}
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
		
		public bool									AcceptDefocus
		{
			get
			{
				return this.InternalAboutToLoseFocus (Widget.TabNavigationDir.None, Widget.TabNavigationMode.Passive);
			}
		}
		public bool									PossibleContainer
		{
			get { return ((this.internal_state & InternalState.PossibleContainer) != 0) && !this.IsFrozen; }
		}
		
		
		public virtual ChildrenCollection			Children
		{
			get
			{
				if (this.children == null)
				{
					lock (this)
					{
						if (this.children == null)
						{
							this.CreateChildrenCollection ();
						}
					}
				}
				
				return this.children;
			}
		}
		
		public Widget								Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				if (value != this.parent)
				{
					Widget parent;
					
					if (value == null)
					{
						parent = this.parent;
						this.parent.Children.Remove (this);
					}
					else
					{
						parent = value;
						parent.Children.Add (this);
					}
					
					System.Diagnostics.Debug.Assert (this.parent == value);
					
					if (this.Dock != DockStyle.None)
					{
						parent.UpdateChildrenLayout ();
					}
				}
			}
		}
		
		public virtual Window						Window
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
		
		
		#region ICommandDispatcherHost Members
		public virtual Support.CommandDispatcher	CommandDispatcher
		{
			get
			{
				if (this.dispatcher != null)
				{
					return this.dispatcher;
				}
				
				if (this.parent != null)
				{
					return this.parent.CommandDispatcher;
				}
				
				return null;
			}
			set
			{
				if (this.dispatcher != value)
				{
					this.SetCommandDispatcher (value);
				}
			}
		}
		#endregion
		
		public virtual Support.OpletQueue			OpletQueue
		{
			get
			{
				Support.CommandDispatcher dispatcher = this.CommandDispatcher;
				
				return dispatcher == null ? null : dispatcher.OpletQueue;
			}
			set
			{
				throw new System.InvalidOperationException ("Cannot set OpletQueue on Widget.");
			}
		}
		
		public virtual Support.ResourceManager		ResourceManager
		{
			get
			{
				if (this.resource_manager == null)
				{
					if (this.parent != null)
					{
						return this.parent.ResourceManager;
					}
					
//-					System.Diagnostics.Debug.WriteLine ("Falling back to default resource manager: " + this.ToString ());
					return Support.Resources.DefaultManager;
				}
				
				return this.resource_manager;
			}
			set
			{
				if (this.resource_manager != value)
				{
					this.resource_manager = value;
					
					if (this.text_layout != null)
					{
						this.text_layout.ResourceManager = value;
					}
					
					this.OnResourceManagerChanged ();
				}
			}
		}
		
		
		public Widget								RootParent
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
		
		public int									RootAngle
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
		
		public double								RootZoom
		{
			get
			{
				Widget widget = this;
				double zoom   = 1.0;
				
				while (widget != null)
				{
					zoom  *= widget.Client.Zoom;
					widget = widget.parent;
				}
				
				return zoom;
			}
		}
		
		public Direction							RootDirection
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
		
		
		public bool									IsEmpty
		{
			get { return (this.children == null) || (this.children.Count == 0); }
		}
		
		public bool									HasChildren
		{
			get { return (this.children != null) && (this.children.Count > 0); }
		}
		
		public bool									HasParent
		{
			get { return this.parent != null; }
		}
		
		public bool									HasSiblings
		{
			get
			{
				if ((this.parent != null) &&
					(this.parent.children.Count > 1))
				{
					return true;
				}
				
				return false;
			}
		}
		
		public bool									HasDockedChildren
		{
			get { return (this.internal_state & InternalState.ChildrenDocked) != 0; }
		}
		
		
		[Bundle] public int							Index
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
		
		
		[Bundle]			public string			Name
		{
			get
			{
				if (this.name == null)
				{
					return "";
				}
				
				return this.name;
			}
			
			set
			{
				if ((value == null) || (value.Length == 0))
				{
					if (this.name != null)
					{
						this.name = null;
						this.OnNameChanged ();
					}
				}
				else if (this.name != value)
				{
					this.name = value;
					this.OnNameChanged ();
				}
			}
		}

		[Bundle]			public string			Command
		{
			get
			{
				if (this.command == null)
				{
					return "";
				}
				
				return this.command;
			}
			
			set
			{
				if ((value == null) || (value.Length == 0))
				{
					this.command = null;
				}
				else if (this.command != value)
				{
					this.command = value;
					
					Support.CommandDispatcher dispatcher = this.CommandDispatcher;
					
					if (dispatcher != null)
					{
						dispatcher.SyncCommandStates (this.command);
					}
				}
			}
		}

		public string								CommandName
		{
			get
			{
				if (this.command == null)
				{
					return "";
				}

				return Support.CommandDispatcher.ExtractCommandName (this.command);
			}
		}
		
		public CommandState							CommandState
		{
			get
			{
				return this.CreateCommandState ();
			}
		}

		public string								FullPathName
		{
			get
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				
				this.BuildFullPathName (buffer);
				
				return buffer.ToString ();
			}
		}
		
		[Bundle]			public string			Text
		{
			get
			{
				if (this.text_layout == null)
				{
					return "";
				}
				
				string text = this.text;
				
				if ((this.AutoResolveResRef == false) ||
					(Support.Resources.IsTextRef (text)))
				{
					text = this.text_layout.Text;
				}
				
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
					this.text = null;
					
					if (this.text_layout != null)
					{
						this.DisposeTextLayout ();
						this.OnTextDefined ();
						this.OnTextChanged ();
						this.Invalidate ();
					}
				}
				else
				{
					if (this.text_layout == null)
					{
						this.CreateTextLayout ();
					}
					
					this.ModifyText (value);
					
					string text = this.AutoResolveResRef ? this.resource_manager.ResolveTextRef (value) : value;
					
					if (this.text_layout.Text != text)
					{
						this.ModifyTextLayout (text);
						this.OnTextDefined ();
						this.OnTextChanged ();
						this.Invalidate ();
					}
				}
				
				if (this.AutoMnemonic)
				{
					this.ResetMnemonicShortcut ();
				}
			}
		}
		
		public virtual TextLayout					TextLayout
		{
			get { return this.text_layout; }
		}
		
		public Drawing.TextBreakMode				TextBreakMode
		{
			get
			{
				if (this.text_layout != null)
				{
					return this.text_layout.BreakMode;
				}
				
				return Drawing.TextBreakMode.None;
			}
			set
			{
				if (this.text_layout == null)
				{
					this.CreateTextLayout ();
				}
				
				if (this.text_layout.BreakMode != value)
				{
					this.text_layout.BreakMode = value;
					this.Invalidate ();
				}
			}
		}
		
		
		public char									Mnemonic
		{
			get
			{
				if (this.AutoMnemonic)
				{
					//	Le code mnémonique est encapsulé par des tags <m>..</m>.
					
					if (this.AutoResolveResRef)
					{
						string text = this.resource_manager.ResolveTextRef (this.Text);
						return TextLayout.ExtractMnemonic (text);
					}
					else
					{
						return TextLayout.ExtractMnemonic (this.Text);
					}
				}
				
				return (char) 0;
			}
		}
		
		[Bundle]			public int				TabIndex
		{
			get { return this.tab_index; }
			set
			{
				if (this.tab_index != value)
				{
					this.tab_index = value;
					
					if ((this.tab_navigation_mode == TabNavigationMode.Passive) &&
						(this.tab_index > 0))
					{
						this.tab_navigation_mode = TabNavigationMode.ActivateOnTab;
					}
					else if ((this.tab_navigation_mode == TabNavigationMode.ActivateOnTab) &&
						/**/ (this.tab_index <= 0))
					{
						this.tab_navigation_mode = TabNavigationMode.Passive;
					}
				}
			}
		}
		
		public TabNavigationMode					TabNavigation
		{
			get { return this.tab_navigation_mode; }
			set { this.tab_navigation_mode = value; }
		}
		
		public Shortcut								Shortcut
		{
			get
			{
				if (this.shortcut == null)
				{
					this.shortcut = new Shortcut ();
				}
				
				return this.shortcut;
			}
			
			set
			{
				if (this.AutoMnemonic)
				{
					//	Supprime le flag 'auto mnemonic' sans altérer le raccourci,
					//	ce qui évite de générer un événement ShortcutChanged avant
					//	l'heure :
					
					this.internal_state &= ~InternalState.AutoMnemonic;
				}
				
				if (this.shortcut != value)
				{
					this.shortcut = value;
					this.OnShortcutChanged ();
				}
			}
		}
		
		public string								Hypertext
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
		
		public Support.IValidator					Validator
		{
			get
			{
				return this.validator;
			}
		}
		
		[Bundle]			public string			BindingInfo
		{
			get
			{
				if (this.IsPropertyDefined (Widget.prop_binding))
				{
					return this.GetProperty (Widget.prop_binding) as string;
				}
				
				return null;
			}
			set
			{
				if (this.BindingInfo != value)
				{
					if (value == null)
					{
						this.ClearProperty (Widget.prop_binding);
					}
					else
					{
						this.SetProperty (Widget.prop_binding, value);
					}
					
					this.OnBindingInfoChanged ();
				}
			}
		}
		
		
		#region Serialization support
		protected virtual bool ShouldSerializeLocation()
		{
			return (this.Dock == DockStyle.None) && (this.Anchor == AnchorStyles.None);
		}
		
		protected virtual bool ShouldSerializeSize()
		{
			return (this.Dock != DockStyle.Fill);
		}
		#endregion
		
		public void SuspendLayout()
		{
			lock (this)
			{
				this.suspend_counter++;
			}
		}
		
		public void ResumeLayout()
		{
			this.ResumeLayout (true);
		}
		
		public void ResumeLayout(bool update)
		{
			lock (this)
			{
				if (this.suspend_counter > 0)
				{
					this.suspend_counter--;
				}
				
				if (this.suspend_counter == 0)
				{
					if ((this.internal_state & InternalState.LayoutChanged) != 0)
					{
						if (update)
						{
							this.UpdateChildrenLayout ();
							System.Diagnostics.Debug.Assert (this.layout_info == null);
						}
					}
					if ((this.internal_state & InternalState.ChildrenChanged) != 0)
					{
						this.internal_state &= ~ InternalState.ChildrenChanged;
						this.HandleChildrenChanged ();
					}
				}
			}
		}
		
		public void ForceLayout()
		{
			if (this.parent != null)
			{
				this.parent.UpdateChildrenLayout ();
			}
		}
		
		public void ForceState(WidgetState state)
		{
			this.widget_state = state;
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
		
		
		public virtual Drawing.Size GetBestFitSize()
		{
			return new Drawing.Size (this.DefaultWidth, this.DefaultHeight);
		}
		
		
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
		
		
		public virtual void Validate()
		{
			if (this.Validator != null)
			{
				if (this.Validator.State == Support.ValidationState.Dirty)
				{
					this.Validator.Validate ();
				}
			}
		}
		
		
		internal void AddValidator(Support.IValidator value)
		{
			this.validator = Support.MulticastValidator.Combine (this.validator, value);
			this.OnValidatorChanged ();
		}
		
		internal void RemoveValidator(Support.IValidator value)
		{
			if (this.validator != null)
			{
				Support.MulticastValidator mv = this.validator as Support.MulticastValidator;
				
				if (mv != null)
				{
					mv.Remove (value);
					this.validator = Support.MulticastValidator.Simplify (mv);
				}
				else if (this.validator == value)
				{
					this.validator = null;
				}
				
				this.OnValidatorChanged ();
			}
		}
		
		protected virtual bool ShouldSerializeValidator(Support.IValidator validator)
		{
			return true;
		}
		
		
		public virtual void SetVisible(bool visible)
		{
			if ((this.internal_state & InternalState.Visible) == 0)
			{
				if (visible)
				{
					this.internal_state |= InternalState.Visible;
					this.Invalidate ();
					
					if (this.Dock != DockStyle.None)
					{
						this.parent.UpdateChildrenLayout ();
					}
					
					this.NotifyChangedToVisible ();
				}
			}
			else
			{
				if (!visible)
				{
					//	Rend invisible un widget qui était visible avant. Il faut
					//	aussi s'assurer que le widget n'est plus "Entered".
					
					this.SetEntered (false);
					this.Invalidate ();
					this.internal_state &= ~ InternalState.Visible;
					
					if (this.Dock != DockStyle.None)
					{
						this.parent.UpdateChildrenLayout ();
					}
					
					this.NotifyChangedToHidden ();
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
					this.Invalidate (InvalidateReason.EnabledChanged);
				}
			}
			else
			{
				if (!enabled)
				{
					this.widget_state &= ~ WidgetState.Enabled;
					this.Invalidate (InvalidateReason.EnabledChanged);
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
					this.Invalidate (InvalidateReason.FrozenChanged);
				}
			}
			else
			{
				if (!frozen)
				{
					this.internal_state &= ~ InternalState.Frozen;
					this.Invalidate (InvalidateReason.FrozenChanged);
				}
			}
		}
		
		public void Focus()
		{
			Window window = this.Window;
			
			if (window != null)
			{
				window.FocusWidget (this);
			}
		}
		
		internal void SetFocused(bool focused)
		{
			Window window = this.Window;
			
			if (! this.IsFocusedFlagSet)
			{
				if (focused)
				{
					if (window != null)
					{
						this.widget_state |= WidgetState.Focused;
						window.FocusedWidget = this;
					}
					
					if (this.IsFocused)
					{
						this.OnFocused ();
						this.Invalidate (InvalidateReason.FocusedChanged);
					}
				}
			}
			else
			{
				if (!focused)
				{
					this.widget_state &= ~ WidgetState.Focused;
					
					if (window != null)
					{
						window.FocusedWidget = null;
					}
					
					this.OnDefocused ();
					this.Invalidate (InvalidateReason.FocusedChanged);
				}
			}
		}
		
		public void SetSelected(bool selected)
		{
			if ((this.widget_state & WidgetState.Selected) == 0)
			{
				if (selected)
				{
					this.widget_state |= WidgetState.Selected;
					this.OnSelected ();
					this.Invalidate (InvalidateReason.SelectedChanged);
				}
			}
			else
			{
				if (!selected)
				{
					this.widget_state &= ~WidgetState.Selected;
					this.OnDeselected ();
					this.Invalidate (InvalidateReason.SelectedChanged);
				}
			}
		}
		
		public void SetEngaged(bool engaged)
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
					this.OnEngaged ();
					this.Invalidate (InvalidateReason.EngagedChanged);
				}
			}
			else
			{
				if (!engaged)
				{
					this.widget_state &= ~ WidgetState.Engaged;
					window.EngagedWidget = null;
					this.OnDisengaged ();
					this.Invalidate (InvalidateReason.EngagedChanged);
				}
			}
		}
		
		public void SetEventPropagation(Propagate events, bool on, Setting setting)
		{
			if (on)
			{
				this.propagate |= events;
			}
			else
			{
				this.propagate &= ~events;
			}
			
			if ((setting == Setting.IncludeChildren) &&
				(this.HasChildren))
			{
				for (int i = 0; i < this.children.Count; i++)
				{
					this.children[i].SetEventPropagation (events, on, setting);
				}
			}
		}
		
		public void SetCommandDispatcher(Support.CommandDispatcher dispatcher)
		{
			this.dispatcher = dispatcher;
		}
		
		public void SetSyncPaint(bool enabled)
		{
			if (enabled)
			{
				this.internal_state |= InternalState.SyncPaint;
			}
			else
			{
				this.internal_state &= ~InternalState.SyncPaint;
			}
		}
		
		
		public CommandState CreateCommandState()
		{
			if (this.command == null)
			{
				return null;
			}
			
			return CommandState.Find (this.CommandName, this.CommandDispatcher);
		}
		
		
		internal void FireStillEngaged()
		{
			if (this.IsEngaged)
			{
				this.OnStillEngaged ();
			}
		}
		
		internal void SimulatePressed()
		{
			if (this.IsEnabled)
			{
				this.OnPressed (null);
			}
		}
		
		internal void SimulateReleased()
		{
			if (this.IsEnabled)
			{
				this.OnReleased (null);
			}
		}
		
		internal void SimulateClicked()
		{
			if (this.IsEnabled)
			{
				this.OnClicked (null);
			}
		}
		
		internal void SimulateFocused()
		{
			if (this.IsEnabled)
			{
				this.OnFocused ();
				this.Invalidate (InvalidateReason.FocusedChanged);
			}
		}
		
		internal void SimulateDefocused()
		{
			if (this.IsEnabled)
			{
				this.OnDefocused ();
				this.Invalidate (InvalidateReason.FocusedChanged);
			}
		}
		
		
		public void SetEmbedder(Widget embedder)
		{
			this.Parent = embedder;
			this.internal_state |= InternalState.Embedded;
		}
		
		
		public override string ToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append (this.widget_id.ToString ());
			buffer.Append (":");
			buffer.Append (this.GetType ().Name);
			this.BuildFullPathName (buffer);
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
					
					if ((this.parent != null) &&
						(this.parent.IsEntered == false) &&
						(!(this.parent is WindowRoot)))
					{
						this.parent.SetEntered (true);
					}
					
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
			}
		}
		
		protected static void ExitWidgetsNotParentOf(Widget widget)
		{
			int i = 0;
			
			while (i < Widget.entered_widgets.Count)
			{
				Widget candidate = Widget.entered_widgets[i] as Widget;
				
				if (widget.IsAncestorWidget (candidate) == false)
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
			if (this.IsVisible)
			{
				this.Invalidate (this.GetPaintBounds ());
			}
		}
		
		public virtual void Invalidate(Drawing.Rectangle rect)
		{
			if (this.IsVisible)
			{
				if (this.parent != null)
				{
					if ((this.InternalState & InternalState.SyncPaint) != 0)
					{
						Window window = this.Window;
						
						if (window != null)
						{
							if (window.IsSyncPaintDisabled)
							{
								this.parent.Invalidate (this.MapClientToParent (rect));
							}
							else
							{
								window.SynchronousRepaint ();
								this.parent.Invalidate (this.MapClientToParent (rect));
								window.PaintFilter = new WidgetPaintFilter (this);
								window.SynchronousRepaint ();
								window.PaintFilter = null;
							}
						}
					}
					else
					{
						this.parent.Invalidate (this.MapClientToParent (rect));
					}
				}
			}
		}
		
		protected virtual void Invalidate(InvalidateReason reason)
		{
			this.Invalidate ();
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
			bool flip_x = rect.Width < 0;
			bool flip_y = rect.Height < 0;
			
			Drawing.Point p1 = this.MapRootToClient (new Drawing.Point (rect.Left, rect.Bottom));
			Drawing.Point p2 = this.MapRootToClient (new Drawing.Point (rect.Right, rect.Top));
			
			rect.X = System.Math.Min (p1.X, p2.X);
			rect.Y = System.Math.Min (p1.Y, p2.Y);
			
			rect.Width  = System.Math.Abs (p1.X - p2.X);
			rect.Height = System.Math.Abs (p1.Y - p2.Y);
			
			int angle = this.RootAngle;
			
			switch (angle)
			{
				case 0:
				case 180:
					if (flip_x) rect.FlipX ();
					if (flip_y) rect.FlipY ();
					break;
				
				case 90:
				case 270:
					if (flip_y) rect.FlipX ();
					if (flip_x) rect.FlipY ();
					break;
				
				default:
					throw new System.ArgumentOutOfRangeException ("Invalid angle");
			}
			
			return rect;
		}
		
		public virtual Drawing.Rectangle MapClientToRoot(Drawing.Rectangle rect)
		{
			bool flip_x = rect.Width < 0;
			bool flip_y = rect.Height < 0;
			
			Drawing.Point p1 = this.MapClientToRoot (new Drawing.Point (rect.Left, rect.Bottom));
			Drawing.Point p2 = this.MapClientToRoot (new Drawing.Point (rect.Right, rect.Top));
			
			rect.X = System.Math.Min (p1.X, p2.X);
			rect.Y = System.Math.Min (p1.Y, p2.Y);
			
			rect.Width  = System.Math.Abs (p1.X - p2.X);
			rect.Height = System.Math.Abs (p1.Y - p2.Y);
			
			int angle = this.RootAngle;
			
			switch (angle)
			{
				case 0:
				case 180:
					if (flip_x) rect.FlipX ();
					if (flip_y) rect.FlipY ();
					break;
				
				case 90:
				case 270:
					if (flip_y) rect.FlipX ();
					if (flip_x) rect.FlipY ();
					break;
				
				default:
					throw new System.ArgumentOutOfRangeException ("Invalid angle");
			}
			
			return rect;
		}
		
		
		public virtual Drawing.Rectangle MapParentToClient(Drawing.Rectangle rect)
		{
			bool flip_x = rect.Width < 0;
			bool flip_y = rect.Height < 0;
			
			Drawing.Point p1 = this.MapParentToClient (new Drawing.Point (rect.Left, rect.Bottom));
			Drawing.Point p2 = this.MapParentToClient (new Drawing.Point (rect.Right, rect.Top));
			
			rect.X = System.Math.Min (p1.X, p2.X);
			rect.Y = System.Math.Min (p1.Y, p2.Y);
			
			rect.Width  = System.Math.Abs (p1.X - p2.X);
			rect.Height = System.Math.Abs (p1.Y - p2.Y);
			
			int angle = this.client_info.angle;
			
			switch (angle)
			{
				case 0:
				case 180:
					if (flip_x) rect.FlipX ();
					if (flip_y) rect.FlipY ();
					break;
				
				case 90:
				case 270:
					if (flip_y) rect.FlipX ();
					if (flip_x) rect.FlipY ();
					break;
				
				default:
					throw new System.ArgumentOutOfRangeException ("Invalid angle");
			}
			
			return rect;
		}
		
		public virtual Drawing.Rectangle MapClientToParent(Drawing.Rectangle rect)
		{
			bool flip_x = rect.Width < 0;
			bool flip_y = rect.Height < 0;
			
			Drawing.Point p1 = this.MapClientToParent (new Drawing.Point (rect.Left, rect.Bottom));
			Drawing.Point p2 = this.MapClientToParent (new Drawing.Point (rect.Right, rect.Top));
			
			rect.X = System.Math.Min (p1.X, p2.X);
			rect.Y = System.Math.Min (p1.Y, p2.Y);
			
			rect.Width  = System.Math.Abs (p1.X - p2.X);
			rect.Height = System.Math.Abs (p1.Y - p2.Y);
			
			int angle = this.client_info.angle;
			
			switch (angle)
			{
				case 0:
				case 180:
					if (flip_x) rect.FlipX ();
					if (flip_y) rect.FlipY ();
					break;
				
				case 90:
				case 270:
					if (flip_y) rect.FlipX ();
					if (flip_x) rect.FlipY ();
					break;
				
				default:
					throw new System.ArgumentOutOfRangeException ("Invalid angle");
			}
			
			return rect;
		}

		public virtual Drawing.Rectangle MapScreenToClient(Drawing.Rectangle rect)
		{
			bool flip_x = rect.Width < 0;
			bool flip_y = rect.Height < 0;
			
			Drawing.Point p1 = this.MapScreenToClient (new Drawing.Point (rect.Left, rect.Bottom));
			Drawing.Point p2 = this.MapScreenToClient (new Drawing.Point (rect.Right, rect.Top));
			
			rect.X = System.Math.Min (p1.X, p2.X);
			rect.Y = System.Math.Min (p1.Y, p2.Y);
			
			rect.Width  = System.Math.Abs (p1.X - p2.X);
			rect.Height = System.Math.Abs (p1.Y - p2.Y);
			
			int angle = this.client_info.angle;
			
			switch (angle)
			{
				case 0:
				case 180:
					if (flip_x) rect.FlipX ();
					if (flip_y) rect.FlipY ();
					break;
				
				case 90:
				case 270:
					if (flip_y) rect.FlipX ();
					if (flip_x) rect.FlipY ();
					break;
				
				default:
					throw new System.ArgumentOutOfRangeException ("Invalid angle");
			}
			
			return rect;
		}
		
		public virtual Drawing.Rectangle MapClientToScreen(Drawing.Rectangle rect)
		{
			bool flip_x = rect.Width < 0;
			bool flip_y = rect.Height < 0;
			
			Drawing.Point p1 = this.MapClientToScreen (new Drawing.Point (rect.Left, rect.Bottom));
			Drawing.Point p2 = this.MapClientToScreen (new Drawing.Point (rect.Right, rect.Top));
			
			rect.X = System.Math.Min (p1.X, p2.X);
			rect.Y = System.Math.Min (p1.Y, p2.Y);
			
			rect.Width  = System.Math.Abs (p1.X - p2.X);
			rect.Height = System.Math.Abs (p1.Y - p2.Y);
			
			int angle = this.client_info.angle;
			
			switch (angle)
			{
				case 0:
				case 180:
					if (flip_x) rect.FlipX ();
					if (flip_y) rect.FlipY ();
					break;
				
				case 90:
				case 270:
					if (flip_y) rect.FlipX ();
					if (flip_x) rect.FlipY ();
					break;
				
				default:
					throw new System.ArgumentOutOfRangeException ("Invalid angle");
			}
			
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
			t.RotateDeg (-this.client_info.angle);
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
			t.RotateDeg (this.client_info.angle);
			t.Translate (ox, oy);
			t.Round ();
			
			return t;
		}
		
		
		public bool IsAncestorWidget(Widget widget)
		{
			if (this.parent == widget)
			{
				return true;
			}
			if (this.parent == null)
			{
				return false;
			}
			
			return this.parent.IsAncestorWidget (widget);
		}
		
		public bool IsDescendantWidget(Widget widget)
		{
			if (widget == null)
			{
				return false;
			}
			
			return widget.IsAncestorWidget (this);
		}
		
		
		public static void BaseLineAlign(Widget model, Widget widget)
		{
			if ((model == null) ||
				(widget == null))
			{
				return;
			}
			
			double model_offset  = model.BaseLine.Y;
			double widget_offset = widget.BaseLine.Y;
			
			double y_bottom = model.Bottom + model_offset - widget_offset;
			
			widget.Bounds = new Drawing.Rectangle (widget.Left, y_bottom, widget.Width, widget.Height);
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
				
				if ((mode & ChildFindMode.SkipMask) != ChildFindMode.All)
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
						if (widget.IsVisibleFlagSet == false)
						{
							continue;
						}
					}
					if ((mode & ChildFindMode.SkipNonContainer) != 0)
					{
						if (widget.PossibleContainer == false)
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
					
					if ((mode & ChildFindMode.Deep) != 0)
					{
						//	Si on fait une recherche en profondeur, on regarde si le point correspond à
						//	un descendant du widget trouvé...
						
						Widget deep = widget.FindChild (widget.MapParentToClient (point), mode);
						
						//	Si oui, pas de test supplémentaire: on s'arrête et on retourne le widget
						//	terminal trouvé lors de la descente récursive :
						
						if (deep != null)
						{
							return deep;
						}
					}
					
					if ((mode & ChildFindMode.SkipEmbedded) != 0)
					{
						//	Si l'appelant a demandé de sauter les widgets spéciaux, marqués comme étant
						//	"embedded" dans un parent, on vérifie que l'on ne retourne pas un tel widget.
						//	Ce test doit se faire en dernier, parce qu'une descente récursive dans un
						//	widget "embedded" peut éventuellement donner des résultats positifs :
						
						if (widget.IsEmbedded)
						{
							continue;
						}
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
				
				if ((mode & ChildFindMode.SkipMask) != ChildFindMode.All)
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
						if (widget.IsVisibleFlagSet == false)
						{
							continue;
						}
					}
					else if ((mode & ChildFindMode.SkipNonContainer) != 0)
					{
						if (widget.PossibleContainer == false)
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
		
		public virtual Widget	FindChildByPath(string[] names, int offset)
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
					Widget child = widget.FindChildByPath (names, offset);
					
					if (child != null)
					{
						return child;
					}
				}
				else if (widget.Name == names[offset])
				{
					return widget.FindChildByPath (names, offset+1);
				}
			}
			
			return null;
		}
		
		public Widget			FindChildByPath(string path)
		{
			string[] names = path.Split ('.');
			
			if (this.Name == "")
			{
				return this.FindChildByPath (names, 0);
			}
			if (this.Name == names[0])
			{
				return this.FindChildByPath (names, 1);
			}
			
			return null;
		}
		
		public Widget[]	        FindCommandWidgets()
		{
			//	Passe en revue tous les widgets de la descendance et accumule
			//	ceux qui sont des widgets de commande.
			
			CommandWidgetFinder finder = new CommandWidgetFinder ();
			
			this.WalkChildren (new WalkWidgetCallback (finder.Analyse));
			
			return finder.Widgets;
		}
		
		public Widget[]	        FindCommandWidgets(string command)
		{
			//	Passe en revue tous les widgets de la descendance et accumule
			//	ceux qui sont des widgets de commande qui correspondent au critère
			//	de recherche.
			
			CommandWidgetFinder finder = new CommandWidgetFinder (command);
			
			this.WalkChildren (new WalkWidgetCallback (finder.Analyse));
			
			return finder.Widgets;
		}
		
		public Widget[]	        FindCommandWidgets(System.Text.RegularExpressions.Regex regex)
		{
			//	Passe en revue tous les widgets de la descendance et accumule
			//	ceux qui sont des widgets de commande qui correspondent au critère
			//	de recherche.
			
			CommandWidgetFinder finder = new CommandWidgetFinder (regex);
			
			this.WalkChildren (new WalkWidgetCallback (finder.Analyse));
			
			return finder.Widgets;
		}
		
		public Widget	        FindCommandWidget(string command)
		{
			//	Passe en revue tous les widgets de la descendance et retourne le
			//	premier qui correspond parfaitement.
			
			CommandWidgetFinder finder = new CommandWidgetFinder (command);
			
			this.WalkChildren (new WalkWidgetCallback (finder.Analyse));
			
			if (finder.Widgets.Length > 0)
			{
				return finder.Widgets[0];
			}
			
			return null;
		}
		
		public Widget[]         FindAllChildren()
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			this.FindAllChildren (list);
			
			Widget[] widgets = new Widget[list.Count];
			list.CopyTo (widgets);
			
			return widgets;
		}
		
		public Widget			FindFocusedChild()
		{
			Window window = this.Window;
			
			if (window != null)
			{
				if (window.FocusedWidget != null)
				{
					//	Il y a un widget avec le focus. Ca peut être nous, un de nos descendants
					//	ou un autre widget sans aucun lien.
					
					if (this.IsFocusedFlagSet)
					{
						return this;
					}
					
					if (this.IsDescendantWidget (window.FocusedWidget))
					{
						return window.FocusedWidget;
					}
				}
			}
			
			return null;
		}
		
		
		protected virtual void FindAllChildren(System.Collections.ArrayList list)
		{
			foreach (Widget child in this.Children)
			{
				list.Add (child);
				child.FindAllChildren (list);
			}
		}
		
		
		public static Widget[]	FindAllCommandWidgets(string command)
		{
			return Widget.FindAllCommandWidgets (command, null);
		}
		
		public static Widget[]	FindAllCommandWidgets(System.Text.RegularExpressions.Regex regex)
		{
			return Widget.FindAllCommandWidgets (regex, null);
		}
		
		public static Widget[]	FindAllCommandWidgets(string command, Support.CommandDispatcher dispatcher)
		{
			//	Passe en revue absolument tous les widgets qui existent et cherche ceux qui ont
			//	une commande qui correspond au critère spécifié.
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			System.Collections.ArrayList dead = new System.Collections.ArrayList ();
			
			lock (Widget.alive_widgets)
			{
				foreach (System.WeakReference weak_ref in Widget.alive_widgets)
				{
					//	On utilise la liste des widgets connus qui permet d'avoir accès immédiatement
					//	à tous les widgets sans nécessiter de descente récursive :
					
					if (weak_ref.IsAlive)
					{
						//	Le widget trouvé existe (encore) :
						
						Widget widget = weak_ref.Target as Widget;
						
						if ((widget != null) &&
							(widget.IsCommand))
						{
							if ((dispatcher == null) ||
								(widget.CommandDispatcher == dispatcher))
							{
								if (widget.CommandName == command)
								{
									list.Add (widget);
								}
							}
						}
					}
					else
					{
						dead.Add (weak_ref);
					}
				}
				
				//	Profite de l'occasion, puisqu'on vient de passer en revue tous les widgets,
				//	de supprimer ceux qui sont morts entre temps :
				
				foreach (System.WeakReference weak_ref in dead)
				{
					Widget.alive_widgets.Remove (weak_ref);
				}
			}
			
			Widget[] widgets = new Widget[list.Count];
			list.CopyTo (widgets);
			
			return widgets;
		}
		
		public static Widget[]	FindAllCommandWidgets(System.Text.RegularExpressions.Regex regex, Support.CommandDispatcher dispatcher)
		{
			//	Passe en revue absolument tous les widgets qui existent et cherche ceux qui ont
			//	une commande qui correspond au critère spécifié.
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			System.Collections.ArrayList dead = new System.Collections.ArrayList ();
			
			lock (Widget.alive_widgets)
			{
				foreach (System.WeakReference weak_ref in Widget.alive_widgets)
				{
					//	On utilise la liste des widgets connus qui permet d'avoir accès immédiatement
					//	à tous les widgets sans nécessiter de descente récursive :
					
					if (weak_ref.IsAlive)
					{
						//	Le widget trouvé existe (encore) :
						
						Widget widget = weak_ref.Target as Widget;
						
						if ((widget != null) &&
							(widget.IsCommand))
						{
							if ((dispatcher == null) ||
								(widget.CommandDispatcher == dispatcher))
							{
								if (regex.IsMatch (widget.CommandName))
								{
									list.Add (widget);
								}
							}
						}
					}
					else
					{
						dead.Add (weak_ref);
					}
				}
				
				//	Profite de l'occasion, puisqu'on vient de passer en revue tous les widgets,
				//	de supprimer ceux qui sont morts entre temps :
				
				foreach (System.WeakReference weak_ref in dead)
				{
					Widget.alive_widgets.Remove (weak_ref);
				}
			}
			
			Widget[] widgets = new Widget[list.Count];
			list.CopyTo (widgets);
			
			return widgets;
		}
		
		public static Widget[]	FindAllFullPathWidgets(System.Text.RegularExpressions.Regex regex)
		{
			//	Passe en revue absolument tous les widgets qui existent et cherche ceux qui ont
			//	une commande qui correspond au critère spécifié.
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			System.Collections.ArrayList dead = new System.Collections.ArrayList ();
			
			lock (Widget.alive_widgets)
			{
				foreach (System.WeakReference weak_ref in Widget.alive_widgets)
				{
					//	On utilise la liste des widgets connus qui permet d'avoir accès immédiatement
					//	à tous les widgets sans nécessiter de descente récursive :
					
					if (weak_ref.IsAlive)
					{
						//	Le widget trouvé existe (encore) :
						
						Widget widget = weak_ref.Target as Widget;
						
						if ((widget != null) &&
							(widget.Name != ""))
						{
							if (regex.IsMatch (widget.FullPathName))
							{
								list.Add (widget);
							}
						}
					}
					else
					{
						dead.Add (weak_ref);
					}
				}
				
				//	Profite de l'occasion, puisqu'on vient de passer en revue tous les widgets,
				//	de supprimer ceux qui sont morts entre temps :
				
				foreach (System.WeakReference weak_ref in dead)
				{
					Widget.alive_widgets.Remove (weak_ref);
				}
			}
			
			Widget[] widgets = new Widget[list.Count];
			list.CopyTo (widgets);
			
			return widgets;
		}
		
		
		#region CommandWidgetFinder class
		protected class CommandWidgetFinder
		{
			public CommandWidgetFinder()
			{
			}
			
			public CommandWidgetFinder(string filter)
			{
				this.filter = filter;
			}
			
			public CommandWidgetFinder(System.Text.RegularExpressions.Regex regex)
			{
				this.regex = regex;
			}
			
			
			public bool Analyse(Widget widget)
			{
				if (widget.IsCommand)
				{
					if (this.regex == null)
					{
						if (this.filter == null)
						{
							this.list.Add (widget);
						}
						else if (this.filter == widget.CommandName)
						{
							this.list.Add (widget);
						}
					}
					else
					{
						//	Une expression régulière a été définie pour filtrer les widgets en
						//	fonction de leur nom. On applique cette expression pour voir si le
						//	nom de la commande est conforme...
						
						System.Text.RegularExpressions.Match match = this.regex.Match (widget.CommandName);
						
						//	...en cas de succès, on prend note du widget, sinon on passe simplement
						//	au suivant.
						
						if (match.Success)
						{
							this.list.Add (widget);
						}
					}
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
			
			
			System.Collections.ArrayList			list   = new System.Collections.ArrayList ();
			System.Text.RegularExpressions.Regex	regex  = null;
			string									filter = null;
		}
		#endregion
		
		public void SetFocusOnTabWidget()
		{
			Widget widget = this.FindTabWidget (TabNavigationDir.Forwards, TabNavigationMode.ActivateOnTab, false, true);
			
			if (widget == null)
			{
				widget = this;
			}
			
			widget.SetFocused (true);
		}
		
		public Widget FindTabWidget(TabNavigationDir dir, TabNavigationMode mode)
		{
			return this.FindTabWidget (dir, mode, false, false);
		}
		
		
		protected virtual Widget FindTabWidget(TabNavigationDir dir, TabNavigationMode mode, bool disable_first_enter, bool accept_focus)
		{
			if (this.ProcessTab (dir, mode))
			{
				return this;
			}
			
			Widget find = null;
			
			if ((!disable_first_enter) &&
				((this.tab_navigation_mode & TabNavigationMode.ForwardToChildren) != 0) &&
				(this.HasChildren))
			{
				//	Ce widget permet aux enfants d'entrer dans la liste accessible par la
				//	touche TAB.
				
				Widget[] candidates = this.Children[0].FindTabWidgets (mode);
				
				if (candidates.Length > 0)
				{
					if (dir == TabNavigationDir.Forwards)
					{
						find = candidates[0].FindTabWidget (dir, mode, false, true);
					}
					else if (accept_focus)
					{
						int count = candidates.Length;
						find = candidates[count-1].FindTabWidget (dir, mode, false, true);
					}
					
					if (find != null)
					{
						return find;
					}
				}
			}
			
			if (accept_focus)
			{
				if ((this.tab_navigation_mode & mode) != 0)
				{
					return this;
				}
			}
			
			//	Cherche parmi les frères...
			
			Widget[] siblings = this.FindTabWidgets (mode);
			bool     search_z = true;
			
			for (int i = 0; i < siblings.Length; i++)
			{
				if (siblings[i] == this)
				{
					//	On vient de trouver notre position dans la liste des widgets activables
					//	par la touche TAB.
					
					search_z = false;
					
					switch (dir)
					{
						case TabNavigationDir.Backwards:
							
							find = this.GetTabFromSiblings (i, dir, siblings);
							
							if (find != null)
							{
								if (((find.tab_navigation_mode & TabNavigationMode.ForwardToChildren) != 0) &&
									(find.HasChildren))
								{
									//	Entre en marche arrière dans le widget...
									
									Widget[] candidates = find.Children[0].FindTabWidgets (mode);
									
									if (candidates.Length > 0)
									{
										int    count = candidates.Length;
										Widget enter = candidates[count-1].FindTabWidget (dir, mode, false, true);
										
										if (enter != null)
										{
											find = enter;
										}
									}
								}
							}
							break;
						
						case TabNavigationDir.Forwards:
							
							find = this.GetTabFromSiblings (i, dir, siblings);
							
							if (find != null)
							{
								if (((find.tab_navigation_mode & TabNavigationMode.ForwardToChildren) != 0) &&
									((find.tab_navigation_mode & TabNavigationMode.ForwardOnly) != 0) &&
									(find.HasChildren))
								{
									//	Entre en marche avant dans le widget...
									
									Widget[] candidates = find.Children[0].FindTabWidgets (mode);
									
									if (candidates.Length > 0)
									{
										Widget enter = candidates[0].FindTabWidget (dir, mode, false, true);
										
										if (enter != null)
										{
											find = enter;
										}
									}
								}
							}
							break;
					}
					
					break;
				}
			}
			
			if (search_z)
			{
				find = this;
				
				while (true)
				{
					if (dir == TabNavigationDir.Forwards)
					{
						find = this.Children.FindNext (find);
					}
					else if (dir == TabNavigationDir.Backwards)
					{
						find = this.Children.FindPrevious (find);
					}
					if ((find == null) ||
						((find.tab_navigation_mode & mode) != 0))
					{
						break;
					}
				}
			}
			
			if (find == null)
			{
				//	Toujours rien trouvé. On a demandé aux enfants et aux frères. Il ne nous
				//	reste plus qu'à transmettre au père.
				
				if (this.parent != null)
				{
					if (this.parent.ProcessTabChildrenExit (dir, mode, out find))
					{
						return find;
					}
					
					find = null;
					
					if ((this.parent.tab_navigation_mode & TabNavigationMode.ForwardToChildren) != 0)
					{
						bool accept;
						
						switch (dir)
						{
							case TabNavigationDir.Backwards:
								accept = (this.parent.tab_navigation_mode & TabNavigationMode.ForwardOnly) == 0;
								find   = this.parent.FindTabWidget (dir, mode, true, accept);
								break;
							
							case TabNavigationDir.Forwards:
								accept = false;
								find = this.parent.FindTabWidget (dir, mode, true, accept);
								break;
						}
					}
				}
				else if (this.HasChildren)
				{
					//	Il n'y a plus de parents au-dessus. C'est donc vraisemblablement WindowRoot et
					//	dans ce cas, il ne sert à rien de boucler. On va simplement tenter d'activer le
					//	premier descendant trouvé :
					
					Widget[] candidates = this.Children[0].FindTabWidgets (mode);
					
					if (candidates.Length > 0)
					{
						if (dir == TabNavigationDir.Forwards)
						{
							find = candidates[0].FindTabWidget (dir, mode, true, true);
						}
						else if (accept_focus)
						{
							int count = candidates.Length;
							find = candidates[count-1].FindTabWidget (dir, mode, true, true);
						}
						
						if (find != null)
						{
							return find;
						}
					}
				}
			}
			
			if (find == null)
			{
				//	On ne peut plus avancer, donc on tente de boucler.
				
				if (siblings.Length > 1)
				{
					switch (dir)
					{
						case TabNavigationDir.Backwards:
							find = siblings[siblings.Length-1].FindTabWidget (dir, mode, false, true);
							break;
							
						case TabNavigationDir.Forwards:
							find = siblings[0].FindTabWidget (dir, mode, false, true);
							break;
					}
				}
			}
			
			return find;
		}
		
		protected Widget[] FindTabWidgets(TabNavigationMode mode)
		{
			System.Collections.ArrayList list = this.FindTabWidgetList (mode);
			
			Widget[] widgets = new Widget[list.Count];
			list.CopyTo (widgets);
			
			return widgets;
		}
		
		protected virtual System.Collections.ArrayList FindTabWidgetList(TabNavigationMode mode)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			Widget parent = this.parent;
			
			if (parent != null)
			{
				Widget[] siblings = parent.Children.Widgets;
				
				for (int i = 0; i < siblings.Length; i++)
				{
					Widget sibling = siblings[i];
					
					if (((sibling.TabNavigation & mode) != 0) &&
						(sibling.IsEnabled) &&
						(sibling.IsVisibleFlagSet))
					{
						if (((sibling.TabNavigation & TabNavigationMode.SkipIfReadOnly) != 0) &&
							(sibling is Types.IReadOnly))
						{
							//	Saute aussi les widgets qui déclarent être en lecture seule. Ils ne
							//	sont pas intéressants pour une navigation clavier :
							
							Types.IReadOnly read_only = sibling as Types.IReadOnly;
							
							if (read_only.IsReadOnly)
							{
								continue;
							}
						}
						
						list.Add (sibling);
					}
				}
			}
			
			list.Sort (new TabIndexComparer ());
			
			return list;
		}
		
		protected virtual Widget GetTabFromSiblings(int index, TabNavigationDir dir, Widget[] siblings)
		{
			switch (dir)
			{
				case TabNavigationDir.Backwards:
					if (index > 0)
					{
						return siblings[index-1];
					}
					break;
				
				case TabNavigationDir.Forwards:
					if (index < siblings.Length-1)
					{
						return siblings[index+1];
					}
					break;
			}
			
			return null;
		}
		
		
		#region TabIndexComparer class
		protected class TabIndexComparer : System.Collections.IComparer
		{
			public int Compare(object x, object y)
			{
				Widget wx = x as Widget;
				Widget wy = y as Widget;
				if (wx == wy) return 0;
				if (wx == null) return -1;
				if (wy == null) return 1;
				return (wx.TabIndex == wy.TabIndex) ? wx.Index - wy.Index : wx.TabIndex - wy.TabIndex;
			}
		}
		#endregion
		
		protected virtual bool ProcessTab(TabNavigationDir dir, TabNavigationMode mode)
		{
			//	Une classe qui désire gérer l'événement de déplacement de manière interne,
			//	par exemple dans le cas d'un widget gérant lui-même plusieurs zones sensibles,
			//	a la possibilité de le faire ici; si l'événement a été consommé de manière
			//	interne, il faut retourner 'true'.
			
			return false;
		}
		
		protected virtual bool ProcessTabChildrenExit(TabNavigationDir dir, TabNavigationMode mode, out Widget focus)
		{
			//	Une classe qui désire gérer l'événement de déplacement lorsque le focus quitte
			//	le dernier (ou le premier) des enfants peut le faire ici. Si cette méthode
			//	retourne 'true', c'est le widget retourné par 'focus' qui sera activé; dans
			//	le cas contraire, un algorithme de navigation par défaut sera utilisé.
			
			focus = null;
			
			return false;
		}
		
		
		internal bool InternalAboutToGetFocus(TabNavigationDir dir, TabNavigationMode mode, out Widget focus)
		{
			return this.AboutToGetFocus (dir, mode, out focus);
		}
		
		internal bool InternalAboutToLoseFocus(TabNavigationDir dir, TabNavigationMode mode)
		{
			return this.AboutToLoseFocus (dir, mode);
		}
		
		protected virtual bool AboutToGetFocus(TabNavigationDir dir, TabNavigationMode mode, out Widget focus)
		{
			focus = this;
			return true;
		}
		
		protected virtual bool AboutToLoseFocus(TabNavigationDir dir, TabNavigationMode mode)
		{
			return true;
		}
		
		
		protected virtual void AboutToBecomeOrphan()
		{
			this.SetFocused (false);
			this.SetEngaged (false);
			this.SetEntered (false);
			
			if (this.HasChildren)
			{
				Widget[] children = this.Children.Widgets;
				int  children_num = children.Length;
				
				for (int i = 0; i < children_num; i++)
				{
					children[i].AboutToBecomeOrphan ();
				}
			}
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
			if ((this.IsCommand) &&
				(this.IsEnabled))
			{
				Window window = this.Window;
				
				if (window != null)
				{
					window.QueueCommand (this);
				}
			}
		}
		
		public virtual void ExecuteCommand(string command)
		{
			Window window = this.Window;
			
			if (window != null)
			{
				window.QueueCommand (this, command, this.CommandDispatcher);
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
			
			bool loc_changed  = (this.x1 != x1) || (this.y1 != y1);
			bool size_changed = (this.x2-this.x1 != x2-x1) || (this.y2-this.y1 != y2-y1);
			
			this.x1 = x1;
			this.y1 = y1;
			this.x2 = x2;
			this.y2 = y2;
			
			this.UpdateClientGeometry ();
			
			if (loc_changed)
			{
				this.OnLocationChanged ();
			}
			
			if (size_changed)
			{
				this.OnSizeChanged ();
			}
			
			this.Invalidate ();
		}
		
		protected virtual void UpdateClientGeometry()
		{
			if (this.layout_info == null)
			{
				this.layout_info = new Layouts.LayoutInfo (this.client_info.width, this.client_info.height);
			}
			
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
			
			if (this.IsLayoutSuspended == false)
			{
				this.UpdateChildrenLayout ();
				this.OnClientGeometryUpdated ();
				System.Diagnostics.Debug.Assert (this.layout_info == null);
			}
		}
		
		protected virtual void UpdateTextLayout()
		{
			if (this.text_layout != null)
			{
				this.text_layout.Alignment  = this.Alignment;
				this.text_layout.LayoutSize = this.Client.Size;
			}
		}
		
		
		protected void UpdateChildrenLayout()
		{
			if (this.IsLayoutSuspended)
			{
				//	L'utilisateur a suspendu toute opération de layout, donc on ne va rien faire maintenant
				//	mais laisser le soin au ResumeLayout final de nous appeler à nouveau.
				
				this.internal_state |= InternalState.LayoutChanged;
				
				return;
			}
			
			Widget[] children = this.Children.Widgets;
			
			if (this.TextLayout != null)
			{
				this.UpdateTextLayout ();
			}
			
			//	La méthode UpdateMinMaxBasedOnDockedChildren peut être utilisée (par Panel, par ex.) pour
			//	déterminer une nouvelle taille pour le widget. Si on ne désactive pas temporairement le
			//	layout ici, on bouclerait (peut-être sans fin) :
			
			this.suspend_counter++;
			
			try
			{
				this.UpdateHasDockedChildren (children);
				this.UpdateMinMaxBasedOnDockedChildren (children);
			}
			finally
			{
				this.suspend_counter--;
			}
			
			//	Ce n'est que maintenant que l'on va s'occuper de placer les enfants correctement, en
			//	tenant compte de leur docking et ancrage :
			
			this.UpdateDockedChildrenLayout (children);
			
			System.Diagnostics.Debug.Assert (this.client_info != null);
			
			try
			{
				bool update = false;
				
				if (this.HasChildren)
				{
					Layouts.UpdateEventArgs e = new Layouts.UpdateEventArgs (this, children, this.layout_info);
					this.OnLayoutUpdate (e);
					update = ! e.Cancel;
				}
				
				if (update)
				{
					for (int i = 0; i < children.Length; i++)
					{
						Widget child = children[i];
						
						if ((child.Dock != DockStyle.None) ||
							(child.Anchor == AnchorStyles.None))
						{
							//	Saute les widgets qui sont "docked" dans le parent, car ils ont déjà été
							//	positionnés par la méthode UpdateDockedChildrenLayout. Ceux qui ne sont
							//	pas ancrés ne bougent pas non plus.
							
							continue;
						}
						
						AnchorStyles anchor_x = child.Anchor & AnchorStyles.LeftAndRight;
						AnchorStyles anchor_y = child.Anchor & AnchorStyles.TopAndBottom;
						
						double x1 = child.x1;
						double x2 = child.x2;
						double y1 = child.y1;
						double y2 = child.y2;
						
						switch (anchor_x)
						{
							case AnchorStyles.Left:							//	[x1] fixe à gauche
								x1 = child.anchor_margins.Left;
								x2 = x1 + child.Width;
								break;
							case AnchorStyles.Right:						//	[x2] fixe à droite
								x2 = this.client_info.width - child.anchor_margins.Right;
								x1 = x2 - child.Width;
								break;
							case AnchorStyles.None:							//	ne touche à rien...
								break;
							case AnchorStyles.LeftAndRight:					//	[x1] fixe à gauche, [x2] fixe à droite
								x1 = child.anchor_margins.Left;
								x2 = this.client_info.width - child.anchor_margins.Right;
								break;
						}
						
						switch (anchor_y)
						{
							case AnchorStyles.Bottom:						//	[y1] fixe en bas
								y1 = child.anchor_margins.Bottom;
								y2 = y1 + child.Height;
								break;
							case AnchorStyles.Top:							//	[y2] fixe en haut
								y2 = this.client_info.height - child.anchor_margins.Top;
								y1 = y2 - child.Height;
								break;
							case AnchorStyles.None:							//	ne touche à rien...
								break;
							case AnchorStyles.TopAndBottom:					//	[y1] fixe en bas, [y2] fixe en haut
								y1 = child.anchor_margins.Bottom;
								y2 = this.client_info.height - child.anchor_margins.Top;
								break;
						}
						
						child.SetBounds (x1, y1, x2, y2);
					}
				}
				
				this.OnLayoutChanged ();
			}
			finally
			{
				this.layout_info = null;
			}
		}
		
		protected void UpdateHasDockedChildren(Widget[] children)
		{
			//	Met à jour le flag interne qui indique s'il y a des widgets dans l'état
			//	docked, ou non.
			
			lock (this)
			{
				this.internal_state &= ~InternalState.ChildrenDocked;
				
				for (int i = 0; i < children.Length; i++)
				{
					Widget child = children[i];
					
					if (child.Dock != DockStyle.None)
					{
						this.internal_state |= InternalState.ChildrenDocked;
						break;
					}
				}
			}
		}
		
		protected virtual void UpdateMinMaxBasedOnDockedChildren(Widget[] children)
		{
			//	Recalcule les tailles minimales et maximales en se basant sur les enfants
			//	contenus dans le widget.
			
			if (this.HasDockedChildren == false)
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
			
			switch (this.ContainerLayoutMode)
			{
				case ContainerLayoutMode.HorizontalFlow:
					fill_max_dy = max_dy;
					break;
				case ContainerLayoutMode.VerticalFlow:
					fill_max_dx = max_dx;
					break;
			}
			
			for (int i = 0; i < children.Length; i++)
			{
				Widget child = children[i];
				
				if (child.Dock == DockStyle.None)
				{
					//	Saute les widgets qui ne sont pas "docked", car leur taille n'est pas prise
					//	en compte dans le calcul des minima/maxima.
					
					continue;
				}
				
				if (child.IsVisibleFlagSet == false)
				{
					continue;
				}
				
				Drawing.Size margins = child.DockMargins.Size;
				Drawing.Size min = child.RealMinSize + margins;
				Drawing.Size max = child.RealMaxSize + margins;
				
				switch (child.Dock)
				{
					case DockStyle.Top:
						min_dx  = System.Math.Max (min_dx, min.Width    + min_ox);
						min_dy  = System.Math.Max (min_dy, child.Height + min_oy);
						min_oy += child.Height + margins.Height;
						max_dx  = System.Math.Min (max_dx, max.Width    + max_ox);
//						max_dy  = System.Math.Min (max_dy, child.Height + max_oy);
						max_oy += child.Height + margins.Height;
						break;
					
					case DockStyle.Bottom:
						min_dx  = System.Math.Max (min_dx, min.Width    + min_ox);
						min_dy  = System.Math.Max (min_dy, child.Height + min_oy);
						min_oy += child.Height + margins.Height;
						max_dx  = System.Math.Min (max_dx, max.Width    + max_ox);
//						max_dy  = System.Math.Min (max_dy, child.Height + max_oy);
						max_oy += child.Height + margins.Height;
						break;
						
					case DockStyle.Left:
						min_dx  = System.Math.Max (min_dx, child.Width  + min_ox);
						min_dy  = System.Math.Max (min_dy, min.Height   + min_oy);
						min_ox += child.Width + margins.Width;
//						max_dx  = System.Math.Min (max_dx, child.Width  + max_ox);
						max_dy  = System.Math.Min (max_dy, max.Height   + max_oy);
						max_ox += child.Width + margins.Width;
						break;
					
					case DockStyle.Right:
						min_dx  = System.Math.Max (min_dx, child.Width  + min_ox);
						min_dy  = System.Math.Max (min_dy, min.Height   + min_oy);
						min_ox += child.Width + margins.Width;
//						max_dx  = System.Math.Min (max_dx, child.Width  + max_ox);
						max_dy  = System.Math.Min (max_dy, max.Height   + max_oy);
						max_ox += child.Width + margins.Width;
						break;
					
					case DockStyle.Fill:
						switch (this.ContainerLayoutMode)
						{
							case ContainerLayoutMode.HorizontalFlow:
								fill_min_dx += min.Width;
								fill_min_dy  = System.Math.Max (fill_min_dy, min.Height);
								fill_max_dx += max.Width;
								fill_max_dy  = System.Math.Min (fill_max_dy, max.Height);
								break;
							case ContainerLayoutMode.VerticalFlow:
								fill_min_dx  = System.Math.Max (fill_min_dx, min.Width);
								fill_min_dy += min.Height;
								fill_max_dx  = System.Math.Min (fill_max_dx, max.Width);
								fill_max_dy += max.Height;
								break;
						}
						break;
				}
			}
			
			if (fill_max_dx == 0)
			{
				fill_max_dx = 1000000;
			}
			
			if (fill_max_dy == 0)
			{
				fill_max_dy = 1000000;
			}
			
			double pad_width  = this.DockPadding.Width  + this.Client.Width  - this.InnerBounds.Width;
			double pad_height = this.DockPadding.Height + this.Client.Height - this.InnerBounds.Height;
			
			double min_width  = System.Math.Max (min_dx, fill_min_dx + min_ox) + pad_width;
			double min_height = System.Math.Max (min_dy, fill_min_dy + min_oy) + pad_height;
			double max_width  = System.Math.Min (max_dx, fill_max_dx + max_ox) + pad_width;
			double max_height = System.Math.Min (max_dy, fill_max_dy + max_oy) + pad_height;
			
			//	Tous les calculs ont été faits en coordonnées client, il faut donc encore transformer
			//	ces dimensions en coordonnées parents.
			
			this.AutoMinSize = this.MapClientToParent (new Drawing.Size (min_width, min_height));
			this.AutoMaxSize = this.MapClientToParent (new Drawing.Size (max_width, max_height));
		}
		
		protected void UpdateDockedChildrenLayout(Widget[] children)
		{
			if (this.HasDockedChildren == false)
			{
				return;
			}
			
			System.Diagnostics.Debug.Assert (this.client_info != null);
			System.Diagnostics.Debug.Assert (this.HasChildren);
			
			System.Collections.Queue fill_queue = null;
			Drawing.Rectangle client_rect = this.InnerBounds;
			Drawing.Rectangle bounds;
			
			client_rect.Deflate (this.DockPadding);
			
			for (int i = 0; i < children.Length; i++)
			{
				Widget child = children[i];
				
				if (child.Dock == DockStyle.None)
				{
					//	Saute les widgets qui ne sont pas "docked", car ils doivent être
					//	positionnés par d'autres moyens.
					
					continue;
				}
				
				if (child.IsVisibleFlagSet == false)
				{
					continue;
				}
				
				bounds = child.Bounds;
				bounds.Inflate (child.DockMargins);
				
				double dx = bounds.Width;
				double dy = bounds.Height;
				
				switch (child.Dock)
				{
					case DockStyle.Top:
						bounds = new Drawing.Rectangle (client_rect.Left, client_rect.Top - dy, client_rect.Width, dy);
						bounds.Deflate (child.DockMargins);
						child.SetBounds (bounds.Left, bounds.Bottom, bounds.Right, bounds.Top);
						client_rect.Top -= dy;
						break;
						
					case DockStyle.Bottom:
						bounds = new Drawing.Rectangle (client_rect.Left, client_rect.Bottom, client_rect.Width, dy);
						bounds.Deflate (child.DockMargins);
						child.SetBounds (bounds.Left, bounds.Bottom, bounds.Right, bounds.Top);
						client_rect.Bottom += dy;
						break;
					
					case DockStyle.Left:
						bounds = new Drawing.Rectangle (client_rect.Left, client_rect.Bottom, dx, client_rect.Height);
						bounds.Deflate (child.DockMargins);
						child.SetBounds (bounds.Left, bounds.Bottom, bounds.Right, bounds.Top);
						client_rect.Left += dx;
						break;
					
					case DockStyle.Right:
						bounds = new Drawing.Rectangle (client_rect.Right - dx, client_rect.Bottom, dx, client_rect.Height);
						bounds.Deflate (child.DockMargins);
						child.SetBounds (bounds.Left, bounds.Bottom, bounds.Right, bounds.Top);
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
				int n = fill_queue.Count;
				
				double fill_dx = client_rect.Width;
				double fill_dy = client_rect.Height;
				
				switch (this.ContainerLayoutMode)
				{
					case ContainerLayoutMode.HorizontalFlow:
						foreach (Widget child in fill_queue)
						{
							bounds = new Drawing.Rectangle (client_rect.Left, client_rect.Bottom, fill_dx / n, client_rect.Height);
							bounds.Deflate (child.DockMargins);
						
							child.SetBounds (bounds.Left, bounds.Bottom, bounds.Right, bounds.Top);
							client_rect.Left += fill_dx / n;
						}
						break;
					
					case ContainerLayoutMode.VerticalFlow:
						foreach (Widget child in fill_queue)
						{
							bounds = new Drawing.Rectangle (client_rect.Left, client_rect.Top - fill_dy / n, client_rect.Width, fill_dy / n);
							bounds.Deflate (child.DockMargins);
						
							child.SetBounds (bounds.Left, bounds.Bottom, bounds.Right, bounds.Top);
							client_rect.Top -= fill_dy / n;
						}
						break;
				}
			}
		}
		
		
		public virtual void PaintHandler(Drawing.Graphics graphics, Drawing.Rectangle repaint, IPaintFilter paint_filter)
		{
			if ((paint_filter != null) &&
				(paint_filter.IsWidgetFullyDiscarded (this)))
			{
				return;
			}
			
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
				Drawing.Transform original_transform = graphics.Transform;
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
					if (this.hypertext_list != null)
					{
						this.hypertext_list.Clear ();
					}
					
					PaintEventArgs local_paint_args = new PaintEventArgs (graphics, repaint);
					
					//	Peint l'arrière-plan du widget. En principe, tout va dans l'arrière plan, sauf
					//	si l'on désire réaliser des effets de transparence par dessus le dessin des
					//	widgets enfants.
					
					if ((paint_filter == null) ||
						(paint_filter.IsWidgetPaintDiscarded (this) == false))
					{
						graphics.ResetLineStyle ();
						this.OnPaintBackground (local_paint_args);
						
						if (paint_filter != null)
						{
							paint_filter.EnableChildren ();
						}
					}
					
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
						
							if (widget.IsVisibleFlagSet)
							{
								widget.PaintHandler (graphics, repaint, paint_filter);
							}
						}
					}
				
					//	Peint l'avant-plan du widget, à n'utiliser que pour faire un "effet" spécial
					//	après coup.
					
					if ((paint_filter == null) ||
						(paint_filter.IsWidgetPaintDiscarded (this) == false))
					{
						graphics.ResetLineStyle ();
						this.OnPaintForeground (local_paint_args);
						
						if (paint_filter != null)
						{
							paint_filter.DisableChildren ();
						}
					}
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
		
		
		public void DispatchDummyMouseMoveEvent()
		{
			Window window = this.Window;
			
			if (window != null)
			{
				window.DispatchMessage (Message.CreateDummyMouseMoveEvent ());
			}
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
			
			if (! this.PreProcessMessage (message, client_pos))
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
					Widget widget         = children[children_num-1 - i];
					bool   contains_focus = widget.ContainsFocus;
					
					if ((widget.IsFrozen == false) &&
						((widget.IsVisibleFlagSet) || (contains_focus && message.IsKeyType)) &&
						((message.FilterOnlyFocused == false) || (contains_focus)) &&
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
			if (this.IsVisibleFlagSet || message.IsKeyType)
			{
				bool is_entered = this.IsEntered;
				
				switch (message.Type)
				{
					case MessageType.MouseUp:
						
						if (Message.State.IsSameWindowAsButtonDown == false)
						{
							return;
						}
						
						if ((this.internal_state & InternalState.AutoDoubleClick) == 0)
						{
							Message.ResetButtonDownCounter ();
						}
						
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
				
				if (this.hypertext_list != null)
				{
					foreach (HypertextInfo info in this.hypertext_list)
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
					(widget.IsVisibleFlagSet))
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
		
		protected virtual void ResetMnemonicShortcut()
		{
			if (this.AutoMnemonic)
			{
				if (this.Shortcut.Mnemonic != this.Mnemonic)
				{
					this.Shortcut.Mnemonic = this.Mnemonic;
					this.OnShortcutChanged ();
				}
			}
		}
		
		
		protected virtual void BuildFullPathName(System.Text.StringBuilder buffer)
		{
			if (this.parent != null)
			{
				this.parent.BuildFullPathName (buffer);
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
		
		
		protected virtual void CreateChildrenCollection()
		{
			this.children = new ChildrenCollection (this);
		}
		
		protected virtual void CreateTextLayout()
		{
			if (this.text_layout == null)
			{
				this.text_layout = new TextLayout ();
				
				this.text_layout.DefaultFont     = this.DefaultFont;
				this.text_layout.DefaultFontSize = this.DefaultFontSize;
				this.text_layout.Anchor         += new AnchorEventHandler (this.HandleTextLayoutAnchor);
				this.text_layout.ResourceManager = this.ResourceManager;
				
				this.UpdateTextLayout ();
			}
		}
		
		protected virtual void ModifyText(string text)
		{
			this.text = text;
		}
		
		protected virtual void ModifyTextLayout(string text)
		{
			if (this.text_layout != null)
			{
				this.text_layout.Text = text;
			}
		}
		
		protected virtual void DisposeTextLayout()
		{
			if (this.text_layout != null)
			{
				this.text_layout.Anchor -= new AnchorEventHandler (this.HandleTextLayoutAnchor);
				this.text_layout = null;
			}
		}
		
		
		protected void HandleParentChanged()
		{
			//	Cette méthode est appelée chaque fois qu'un widget change de parent.
			
			if ((this.propagate & Propagate.ParentChanged) != 0)
			{
				if (this.HasChildren)
				{
					for (int i = 0; i < this.children.Count; i++)
					{
						this.children[i].OnParentChanged ();
					}
				}
			}
			
			this.OnParentChanged ();
			
			if (this.parent == null)
			{
				if (this.IsVisibleFlagSet)
				{
					this.NotifyChangedToHidden ();
				}
			}
			else
			{
				if ((this.IsVisibleFlagSet) &&
					(this.IsVisible))
				{
					this.NotifyChangedToVisible ();
				}
			}
		}
		
		protected void HandleChildrenChanged()
		{
			//	Cette méthode est appelée chaque fois qu'un widget fils a été ajouté ou supprimé
			//	de ce widget.
			
			System.Diagnostics.Debug.Assert (this.suspend_counter >= 0);
			
			if (this.IsLayoutSuspended)
			{
				//	L'utilisateur pour suspendre le traitement des événements de layout (ceci comprend
				//	aussi les événements liés aux changement de widgets fils), ce qui permet d'accélérer
				//	les modifications massives de l'interface graphique :
				
				this.internal_state |= InternalState.ChildrenChanged;
				
				return;
			}
			
			//	On veut éviter que le parent génère un nouveau layout de notre instance, car on va le
			//	forcer nous-même en fin de méthode, alors on suspend temporairement le layout.
			
			try
			{
				this.SuspendLayout ();
				
				if ((this.propagate & Propagate.ChildrenChanged) != 0)
				{
					if (this.parent != null)
					{
						this.parent.HandleChildrenChanged ();
					}
				}
				
				this.OnChildrenChanged ();
			}
			finally
			{
				this.ResumeLayout (false);
			}
			
			this.UpdateChildrenLayout ();
			this.Invalidate ();
		}
		
		protected void HandleAdornerChanged()
		{
			foreach (Widget child in this.Children)
			{
				child.HandleAdornerChanged ();
			}
			
			this.OnAdornerChanged ();
		}
		
		protected void HandleCultureChanged()
		{
			if (Support.Resources.IsTextRef (this.text))
			{
				this.Text = this.text;
			}
			
			foreach (Widget child in this.Children)
			{
				child.HandleCultureChanged ();
			}
			
			this.OnCultureChanged ();
		}
		
		protected void HandleTextLayoutAnchor(object sender, AnchorEventArgs e)
		{
			System.Diagnostics.Debug.Assert (sender == this.text_layout);
			
			HypertextInfo info = new HypertextInfo (this.text_layout, e.Bounds, e.Index);
			
			if (this.hypertext_list == null)
			{
				this.hypertext_list = new System.Collections.ArrayList ();
			}
			
			this.hypertext_list.Add (info);
		}
		
		
		protected virtual void OnClientGeometryUpdated()
		{
			if (this.ClientGeometryUpdated != null)
			{
				this.ClientGeometryUpdated (this);
			}
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
			if (this.ChildrenChanged != null)
			{
				this.ChildrenChanged (this);
			}
		}
		
		protected virtual void OnParentChanged()
		{
			if (this.ParentChanged != null)
			{
				this.ParentChanged (this);
			}
		}
		
		protected virtual void OnAdornerChanged()
		{
			if (this.AdornerChanged != null)
			{
				this.AdornerChanged (this);
			}
		}
		
		protected virtual void OnCultureChanged()
		{
			if (this.CultureChanged != null)
			{
				this.CultureChanged (this);
			}
		}
		
		protected virtual void OnLayoutChanged()
		{
			if (this.LayoutChanged != null)
			{
				this.LayoutChanged (this);
			}
		}
		
		protected virtual void OnLayoutUpdate(Layouts.UpdateEventArgs e)
		{
			if (this.LayoutUpdate != null)
			{
				this.LayoutUpdate (this, e);
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
				if (e != null)
				{
					this.OnHypertextClicked (e);
					
					if (e.Message.Consumer != null)
					{
						return;
					}
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
			Window window = this.Window;
				
			if (window != null)
			{
				window.MouseCursor = this.MouseCursor;
				
				if (window.CapturingWidget == this)
				{
					e.Message.FilterNoChildren = true;
					e.Message.Captured         = true;
				}
			}
			
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
					if (window.CapturingWidget == null)
					{
						window.MouseCursor = this.parent.MouseCursor;
					}
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
		
		protected virtual void OnShortcutChanged()
		{
			if (this.ShortcutChanged != null)
			{
				this.ShortcutChanged (this);
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
		
		protected virtual void OnNameChanged()
		{
			if (this.NameChanged != null)
			{
				this.NameChanged (this);
			}
		}
		
		protected virtual void OnTextDefined()
		{
			if (this.TextDefined != null)
			{
				this.TextDefined (this);
			}
		}
		
		protected virtual void OnTextChanged()
		{
			if (this.TextChanged != null)
			{
				this.TextChanged (this);
			}
			if (this.validator != null)
			{
				this.validator.MakeDirty (true);
			}
		}
		
		protected virtual void OnSizeChanged()
		{
			if (this.SizeChanged != null)
			{
				this.SizeChanged (this);
			}
		}
		
		protected virtual void OnLocationChanged()
		{
			if (this.LocationChanged != null)
			{
				this.LocationChanged (this);
			}
		}
		
		
		private void NotifyChangedToHidden()
		{
			this.OnVisibleChanged ();
			
			Widget[] children = this.Children.Widgets;
			
			foreach (Widget child in children)
			{
				if (child.IsVisibleFlagSet)
				{
					child.NotifyChangedToHidden ();
				}
			}
		}
		
		private void NotifyChangedToVisible()
		{
			this.OnVisibleChanged ();
			
			Widget[] children = this.Children.Widgets;
			
			foreach (Widget child in children)
			{
				if (child.IsVisibleFlagSet)
				{
					child.NotifyChangedToVisible ();
				}
			}
		}
		
		
		protected virtual void OnVisibleChanged()
		{
			if (this.VisibleChanged != null)
			{
				this.VisibleChanged (this);
			}
		}
		
		protected virtual void OnFocusChanged()
		{
			if (this.FocusChanged != null)
			{
				this.FocusChanged (this);
			}
		}
		
		protected virtual void OnFocused()
		{
			this.OnFocusChanged ();
			
			if (this.Focused != null)
			{
				this.Focused (this);
			}
		}
		
		protected virtual void OnDefocused()
		{
			this.OnFocusChanged ();
			
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
		
		protected virtual void OnValidatorChanged()
		{
			if (this.ValidatorChanged != null)
			{
				this.ValidatorChanged (this);
			}
		}
		
		protected virtual void OnBindingInfoChanged()
		{
			if (this.BindingInfoChanged != null)
			{
				this.BindingInfoChanged (this);
			}
		}
		
		protected virtual void OnResourceManagerChanged()
		{
			if (this.ResourceManagerChanged != null)
			{
				this.ResourceManagerChanged (this);
			}
		}
		
		
		#region Events
		public event Support.EventHandler			ClientGeometryUpdated;
		public event Support.EventHandler			PreparePaint;
		public event PaintEventHandler				PaintBackground;
		public event PaintEventHandler				PaintForeground;
		public event Support.EventHandler			ChildrenChanged;
		public event Support.EventHandler			ParentChanged;
		public event Support.EventHandler			AdornerChanged;
		public event Support.EventHandler			CultureChanged;
		public event Support.EventHandler			LayoutChanged;
		
		public event Layouts.UpdateEventHandler		LayoutUpdate;
		
		public event MessageEventHandler			Pressed;
		public event MessageEventHandler			Released;
		public event MessageEventHandler			Clicked;
		public event MessageEventHandler			DoubleClicked;
		public event MessageEventHandler			Entered;
		public event MessageEventHandler			Exited;
		public event Support.EventHandler			ShortcutPressed;
		public event Support.EventHandler			ShortcutChanged;
		public event Support.EventHandler			HypertextHot;
		public event MessageEventHandler			HypertextClicked;
		public event Support.EventHandler			ValidatorChanged;
		public event Support.EventHandler			BindingInfoChanged;
		public event Support.EventHandler			ResourceManagerChanged;
		
		public event MessageEventHandler			PreProcessing;
		public event MessageEventHandler			PostProcessing;
		
		public event Support.EventHandler			FocusChanged;
		public event Support.EventHandler			Focused;
		public event Support.EventHandler			Defocused;
		public event Support.EventHandler			Selected;
		public event Support.EventHandler			Deselected;
		public event Support.EventHandler			Engaged;
		public event Support.EventHandler			StillEngaged;
		public event Support.EventHandler			Disengaged;
		public event Support.EventHandler			ActiveStateChanged;
		public event Support.EventHandler			MinSizeChanged;
		public event Support.EventHandler			MaxSizeChanged;
		public event Support.EventHandler			Disposed;
		public event Support.EventHandler			TextDefined;
		public event Support.EventHandler			TextChanged;
		public event Support.EventHandler			NameChanged;
		public event Support.EventHandler			SizeChanged;
		public event Support.EventHandler			LocationChanged;
		public event Support.EventHandler			VisibleChanged;
		
		public event PaintBoundsCallback			PaintBoundsCallback;
		#endregion
		
		#region Various enums
		public enum Setting : byte
		{
			None				= 0,
			IncludeChildren		= 1
		}
		
		[System.Flags] public enum Propagate : uint
		{
			None				= 0,
			
			ChildrenChanged		= 0x00000001,		//	propage au parent: ChildrenChanged
			
			ParentChanged		= 0x00010000,		//	propage aux enfants: ParentChanged
		}
		
		[System.Flags] public enum TabNavigationMode
		{
			Passive				= 0,
			
			ActivateOnTab		= 0x00000001,
			ActivateOnCursorX	= 0x00000002,
			ActivateOnCursorY	= 0x00000004,
			ActivateOnCursor	= ActivateOnCursorX + ActivateOnCursorY,
			ActivateOnPage		= 0x00000008,
			
			ForwardToChildren	= 0x00010000,		//	transmet aux widgets enfants
			ForwardOnly			= 0x00020000,		//	utilisé avec ForwardToChilden: ne prend pas le focus soi-même
			SkipIfReadOnly		= 0x00040000,		//	saute si 'read-only'
			
			ForwardTabActive	= ActivateOnTab | ForwardToChildren,
			ForwardTabPassive	= ActivateOnTab | ForwardToChildren | ForwardOnly,
		}
		
		public enum TabNavigationDir
		{
			None				=  0,
			Forwards			=  1,
			Backwards			= -1
		}
		
		[System.Flags] public enum ChildFindMode
		{
			All					= 0,
			SkipHidden			= 0x00000001,
			SkipDisabled		= 0x00000002,
			SkipTransparent		= 0x00000004,
			SkipEmbedded		= 0x00000008,
			SkipNonContainer	= 0x00000010,
			SkipMask			= 0x000000ff,
			
			Deep				= 0x00010000
		}
		#endregion
		
		#region ClientInfo class
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
		#endregion
		
		#region ChildrenCollection class
		public class ChildrenCollection : System.Collections.IList
		{
			public ChildrenCollection(Widget widget)
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
			
			
			public Widget FindNext(Widget widget)
			{
				Widget[] widgets = this.Widgets;
				
				for (int i = 0; i < widgets.Length; i++)
				{
					if (widgets[i] == widget)
					{
						return (++i < widgets.Length) ? widgets[i] : null;
					}
				}
				
				return null;
			}
			
			public Widget FindPrevious(Widget widget)
			{
				Widget[] widgets = this.Widgets;
				
				for (int i = 0; i < widgets.Length; i++)
				{
					if (widgets[i] == widget)
					{
						return (--i >= 0) ? widgets[i] : null;
					}
				}
				
				return null;
			}
			
			
			internal void ChangeZOrder(Widget widget, int z)
			{
				System.Diagnostics.Debug.Assert (widget != null);
				
				int z_old = widget.ZOrder;
				int z_new = z;
				
				System.Diagnostics.Debug.Assert (z_old >= 0);
				System.Diagnostics.Debug.Assert (z_new >= 0);
				
				if (z_old == z_new)
				{
					return;
				}
				
				int n = this.list.Count;
				
				System.Diagnostics.Debug.Assert (z_old < n);
				System.Diagnostics.Debug.Assert (z_new < n);
				
				z_old = n - z_old - 1;
				z_new = n - z_new - 1;
				
				System.Diagnostics.Debug.Assert (this.list[z_old] == widget);
				
				this.list.RemoveAt (z_old);
				this.list.Insert (z_new, widget);
				
				this.array = null;
				this.NotifyChanged ();
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
				widget.HandleParentChanged ();
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
				widget.AboutToBecomeOrphan ();
				widget.parent = null;
				widget.HandleParentChanged ();
			}
			
			private void NotifyChanged()
			{
				this.widget.HandleChildrenChanged ();
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
			
			public void AddRange(System.Collections.IList list)
			{
				for (int i = 0; i < list.Count; i++)
				{
					this.Add (list[i]);
				}
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
		#endregion
		
		#region HypertextInfo class
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
		#endregion
		
		
		private LayoutStyles					layout;
		private Layouts.LayoutInfo				layout_info;
		private Drawing.Margins					anchor_margins;
		private Drawing.Margins					dock_padding;
		private Drawing.Margins					dock_margins;
		
		private ClientInfo						client_info = new ClientInfo ();
		
		private InternalState					internal_state;
		private WidgetState						widget_state;
		
		private Propagate						propagate;
		
		private Drawing.Color					back_color;
		private double							x1, y1, x2, y2;
		private Drawing.Size					min_size;
		private Drawing.Size					max_size;
		private Drawing.Size					auto_min_size	= new Drawing.Size (0, 0);
		private Drawing.Size					auto_max_size	= new Drawing.Size (1000000, 1000000);
		private System.Collections.ArrayList	hypertext_list;
		private HypertextInfo					hypertext;
		
		private ChildrenCollection				children;
		private Widget							parent;
		private string							name;
		private string							command;
		private int								index = -1;
		private string							text;
		private TextLayout						text_layout;
		private ContentAlignment				alignment;
		private int								suspend_counter;
		private int								tab_index = 0;
		private TabNavigationMode				tab_navigation_mode;
		private Shortcut						shortcut;
		private double							default_font_height;
		private MouseCursor						mouse_cursor;
		private System.Collections.Hashtable	property_hash;
		private Support.ResourceManager			resource_manager;
		private Support.CommandDispatcher		dispatcher;
		private Support.IValidator				validator;
		private int								widget_id;
		
		static System.Collections.ArrayList		entered_widgets = new System.Collections.ArrayList ();
		static System.Collections.ArrayList		alive_widgets   = new System.Collections.ArrayList ();
		static int								next_widget_id  = 0;
		static bool								debug_dispose	= false;
		
		private const string					prop_binding	= "$widget$binding$";
	}
}
