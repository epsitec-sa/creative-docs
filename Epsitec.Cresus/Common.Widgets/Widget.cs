//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	using ContentAlignment = Drawing.ContentAlignment;
	using BundleAttribute  = Support.BundleAttribute;
	
	
	public delegate bool WalkWidgetCallback(Widget widget);
	
	[System.Flags] public enum ActiveState
	{
		No			= 0,
		Yes			= (int) WidgetState.ActiveYes,
		Maybe		= (int) WidgetState.ActiveMaybe,
	}
	
	
	#region WidgetState enum
	[System.Flags] public enum WidgetState : uint
	{
		None			= 0x00000000,				//	=> neutre
		
		ActiveYes		= 0x00000001,				//	=> mode ActiveState.Yes
		ActiveMaybe		= 0x00000002,				//	=> mode ActiveState.Maybe
		ActiveMask		= ActiveYes | ActiveMaybe,
		
		Enabled			= 0x00010000,				//	=> reçoit des événements
		Focused			= 0x00020000,				//	=> reçoit les événements clavier
		Entered			= 0x00040000,				//	=> contient la souris
		Selected		= 0x00080000,				//	=> sélectionné
		Engaged			= 0x00100000,				//	=> pression en cours
		Error			= 0x00200000,				//	=> signale une erreur
		ThreeState		= 0x00400000,				//	=> accepte 3 états
	}
	#endregion
	
	#region InternalState enum
	[System.Flags] public enum InternalState : uint
	{
		None				= 0,
		
		ChildrenDocked		= 0x00000004,		//	certains enfants spécifient un DockStyle
		
		Embedded			= 0x00000008,		//	=> widget appartient au parent (widgets composés)
		
		Focusable			= 0x00000010,
		Selectable			= 0x00000020,
		Engageable			= 0x00000040,		//	=> peut être enfoncé par une pression
		Frozen				= 0x00000080,		//	=> n'accepte aucun événement
		
		AutoMnemonic		= 0x00100000,
		AutoResolveResRef	= 0x00800000,		//	une référence à une ressource [res:...] est remplacée automatiquement
		
		PossibleContainer	= 0x01000000,		//	widget peut être la cible d'un drag & drop en mode édition
		EditionEnabled		= 0x02000000,		//	widget peut être édité
		
		SyncPaint			= 0x20000000,		//	peinture synchrone
		
		DebugActive			= 0x80000000		//	widget marqué pour le debug
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
	
	
	/// <summary>
	/// La classe Widget implémente la classe de base dont dérivent tous les
	/// widgets de l'interface graphique ("controls" dans l'appellation Windows).
	/// </summary>
	public class Widget : Visual, Support.IBundleSupport, Support.Data.IPropertyProvider, Collections.IShortcutCollectionHost
	{
		public Widget()
		{
//-			this.AddEventHandler (Types.DependencyObjectTree.ChildrenProperty, this.HandleWidgetChildrenChanged);
			
			if (Support.ObjectBundler.IsBooting)
			{
				//	N'initialise rien, car cela prend passablement de temps... et de toute
				//	manière, on n'a pas besoin de toutes ces informations pour pouvoir
				//	utiliser IBundleSupport.
				
				return;
			}
			
			if (Widget.DebugDispose)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("{1}+ Created {0}", this.GetType ().Name, this.VisualSerialId));
			}
			
			this.InternalState |= InternalState.AutoMnemonic;
			this.InternalState |= InternalState.AutoResolveResRef;
			
			this.default_font_height = System.Math.Floor(this.DefaultFont.LineHeight*this.DefaultFontSize);
			this.alignment           = this.DefaultAlignment;
			
			this.Size = new Drawing.Size (this.DefaultWidth, this.DefaultHeight);
			
			lock (Widget.alive_widgets)
			{
				Widget.alive_widgets.Add (new System.WeakReference (this));
			}
		}

//-		void HandleWidgetChildrenChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
//-		{
//-			System.Diagnostics.Debug.WriteLine (string.Format ("{0} has {1} children", this.ToString (), this.HasChildren ? this.Children.Count.ToString () : "no"));
//-		}
		
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
			System.Diagnostics.Debug.WriteLine ("Initializing Widget infrastructure.");
			
			Helpers.FontPreviewer.Initialize ();
			
			Res.Initialise (typeof (Widget), "Common.Widgets");
			
			Support.ImageProvider.Initialise ();
			Support.ObjectBundler.Initialise ();
			
			System.Threading.Thread          thread  = System.Threading.Thread.CurrentThread;
			System.Globalization.CultureInfo culture = thread.CurrentCulture;
			
			thread.CurrentUICulture = culture;
		}
		
		
		public static void Initialise()
		{
			//	En appelant cette méthode statique, on peut garantir que le constructeur
			//	statique de Widget a bien été exécuté.
		}
		
		
		#region IShortcutCollectionHost Members
		public void NotifyShortcutsChanged(Epsitec.Common.Widgets.Collections.ShortcutCollection collection)
		{
			if (collection.Count == 0)
			{
				this.shortcuts = null;
			}
			else
			{
				this.shortcuts = collection;
			}
			
			if (this.AutoMnemonic)
			{
				//	Supprime le flag 'auto mnemonic' sans altérer le raccourci,
				//	ce qui évite de générer un événement ShortcutChanged avant
				//	l'heure :
				
				this.internal_state &= ~InternalState.AutoMnemonic;
			}
			
			this.OnShortcutChanged ();
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
				IValidator[]                 validators = MulticastValidator.ToArray (this.validator);
				
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
		
		protected override void Dispose(bool disposing)
		{
			if (Widget.DebugDispose)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("{2}- {0} widget {1}", (disposing ? "Disposing" : "Collecting"), this.ToString (), this.VisualSerialId));
			}
			
			if (disposing)
			{
				if (this.HasChildren)
				{
					Widget[] widgets = this.Children.Widgets;
					
					for (int i = 0; i < widgets.Length; i++)
					{
						widgets[i].Dispose ();
						System.Diagnostics.Debug.Assert (widgets[i].Parent == null);
					}
					
					System.Diagnostics.Debug.Assert (this.Children.Count == 0);
				}
				
				this.SetParent (null);
				
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
					object old_value = this.MinSize;
					this.auto_min_size = value;
					object new_value = this.MinSize;
					
					this.InvalidateProperty (Visual.MinSizeProperty, old_value, new_value);
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
					object old_value = this.MaxSize;
					this.auto_max_size = value;
					object new_value = this.MaxSize;
					
					this.InvalidateProperty (Visual.MaxSizeProperty, old_value, new_value);
				}
			}
		}
		
		public Drawing.Size							RealMinSize
		{
			get
			{
				double width  = System.Math.Max (this.MinWidth, this.auto_min_size.Width);
				double height = System.Math.Max (this.MinHeight, this.auto_min_size.Height);
				
				return new Drawing.Size (width, height);
			}
		}
		
		public Drawing.Size							RealMaxSize
		{
			get
			{
				double width  = System.Math.Min (this.MaxWidth, this.auto_max_size.Width);
				double height = System.Math.Min (this.MaxHeight, this.auto_max_size.Height);
				
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
			get { return Drawing.Size.MaxValue; }
		}
		
		
		public bool									IsCommand
		{
			get
			{
				string command = this.Command;
				
				return (command != null) && (command.Length > 0);
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
				if (this.Parent != null)
				{
					return this.Parent.IsFrozen;
				}
				
				return false;
			}
		}
		
		
		public bool									IsActive
		{
			get
			{
				return this.ActiveState == ActiveState.Yes;
			}
		}
		
		public bool									IsEntered
		{
			get
			{
				return (this.widget_state & WidgetState.Entered) != 0;
			}
		}
		
		public bool									IsSelected
		{
			get
			{
				return (this.widget_state & WidgetState.Selected) != 0;
			}
		}
		
		public bool									IsEngaged
		{
			get
			{
				return (this.widget_state & WidgetState.Engaged) != 0;
			}
		}
		
		public bool									IsError
		{
			get
			{
				return (this.widget_state & WidgetState.Error) != 0;
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
				
				if (this.Parent != null)
				{
					return this.Parent.IsEditionEnabled;
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
		
		private bool								AutoResolveResRef
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
		
		
		protected InternalState						InternalState
		{
			get
			{
				return this.internal_state;
			}
			set
			{
				this.internal_state = value;
				
//				this.AutoCapture = (this.internal_state & InternalState.AutoCapture) != 0;
//				this.AutoEngage  = (this.internal_state & InternalState.AutoEngage) != 0;
//				this.AutoFocus   = (this.internal_state & InternalState.AutoFocus) != 0;
//				this.AutoRepeat  = (this.internal_state & InternalState.AutoRepeat) != 0;
//				this.AutoToggle  = (this.internal_state & InternalState.AutoToggle) != 0;
			}
		}
		
		
		public ActiveState							ActiveState
		{
			get
			{
				return (ActiveState) (this.widget_state & WidgetState.ActiveMask);
			}
			set
			{
				if (this.ActiveState != value)
				{
					this.widget_state &= ~WidgetState.ActiveMask;
					this.widget_state |= (WidgetState) value;
					
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
				
				WidgetState state = this.widget_state & mask;
				
				if (this.IsEnabled)
				{
					state |= WidgetState.Enabled;
				}
				if (this.IsFocused)
				{
					state |= WidgetState.Focused;
				}
				if (this.AcceptThreeState)
				{
					state |= WidgetState.ThreeState;
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
				
				if (this.Parent == null)
				{
					return -1;
				}
				
				Widget[] siblings = this.Parent.Children.Widgets;
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
//			set
//			{
//				if (this.Parent == null)
//				{
//					throw new System.InvalidOperationException ("Cannot change Z-order of an orphan.");
//				}
//				
//				if ((value < 0) ||
//					(value >= this.Parent.Children.Count))
//				{
//					throw new System.ArgumentOutOfRangeException ("value", value, "Invalid Z-order specified.");
//				}
//				
//				this.Parent.Children.ChangeZOrder (this, value);
//				
//				System.Diagnostics.Debug.Assert (this.ZOrder == value);
//			}
		}
		
		
		public virtual bool							CanFocus
		{
			get
			{
				return ((this.internal_state & InternalState.Focusable) != 0)
					&& (!this.IsFrozen);
			}
		}
		
		public bool									CanSelect
		{
			get { return ((this.internal_state & InternalState.Selectable) != 0) && !this.IsFrozen; }
		}
		
		public bool									CanEngage
		{
			get { return ((this.internal_state & InternalState.Engageable) != 0) && this.IsEnabled && !this.IsFrozen; }
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
		
		
		public new Widget							Parent
		{
			get
			{
				return base.Parent as Widget;
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
		
		
		public virtual Support.OpletQueue			OpletQueue
		{
			get
			{
				return Helpers.VisualTree.GetOpletQueue (this);
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
					if (this.Parent != null)
					{
						return this.Parent.ResourceManager;
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
				
				while (widget.Parent != null)
				{
					widget = widget.Parent;
				}
				
				return widget;
			}
		}
		
		public bool									IsEmpty
		{
			get { return this.HasChildren == false; }
		}
		
		public bool									HasParent
		{
			get { return this.Parent != null; }
		}
		
		public bool									HasSiblings
		{
			get
			{
				if ((this.Parent != null) &&
					(this.Parent.Children.Count > 1))
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
		

		public CommandState							CommandState
		{
			get
			{
				return this.GetCommandState ();
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
		
		public virtual TabNavigationMode			TabNavigation
		{
			get
			{
				return this.tab_navigation_mode;
			}
			set
			{
				this.tab_navigation_mode = value;
			}
		}
		
		public Collections.ShortcutCollection		Shortcuts
		{
			get
			{
				if (this.shortcuts == null)
				{
					return new Collections.HostedShortcutCollection (this);
				}
				else
				{
					return this.shortcuts;
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
		
		public IValidator							Validator
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
		
		public virtual Drawing.Size GetBestFitSize()
		{
			return new Drawing.Size (this.DefaultWidth, this.DefaultHeight);
		}
		
		public virtual Drawing.Point GetBaseLine()
		{
			return Drawing.Point.Zero;
		}
		
		
		public virtual void Hide()
		{
			this.Visibility = false;
		}
		
		public virtual void Show()
		{
			this.Visibility = true;
		}
		
		public virtual void Toggle()
		{
			if ((this.AutoRadio) &&
				(this.ActiveState == ActiveState.Yes))
			{
				return;
			}
			
			if (this.AcceptThreeState)
			{
				switch (this.ActiveState)
				{
					case ActiveState.Yes:
						this.ActiveState = ActiveState.Maybe;
						break;
					case ActiveState.Maybe:
						this.ActiveState = ActiveState.No;
						break;
					case ActiveState.No:
						this.ActiveState = ActiveState.Yes;
						break;
				}
			}
			else
			{
				switch (this.ActiveState)
				{
					case ActiveState.Yes:
					case ActiveState.Maybe:
						this.ActiveState = ActiveState.No;
						break;
					case ActiveState.No:
						this.ActiveState = ActiveState.Yes;
						break;
				}
			}
		}
		
		
		public virtual void Validate()
		{
			if (this.Validator != null)
			{
				if (this.Validator.State == ValidationState.Dirty)
				{
					this.Validator.Validate ();
				}
			}
		}
		
		
		internal void AddValidator(IValidator value)
		{
			this.validator = MulticastValidator.Combine (this.validator, value);
			this.OnValidatorChanged ();
		}
		
		internal void RemoveValidator(IValidator value)
		{
			if (this.validator != null)
			{
				MulticastValidator mv = this.validator as MulticastValidator;
				
				if (mv != null)
				{
					mv.Remove (value);
					this.validator = MulticastValidator.Simplify (mv);
				}
				else if (this.validator == value)
				{
					this.validator = null;
				}
				
				this.OnValidatorChanged ();
			}
		}
		
		protected virtual bool ShouldSerializeValidator(IValidator validator)
		{
			return true;
		}
		
		
		public void SetParent(Widget widget)
		{
			Widget oldParent = this.Parent;
			Widget newParent = widget;
			
			if (newParent != oldParent)
			{
				if (newParent == null)
				{
					oldParent.Children.Remove (this);
				}
				else
				{
					newParent.Children.Add (this);
				}
				
				System.Diagnostics.Debug.Assert (this.Parent == newParent);
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
		
		public virtual void SetError(bool value)
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
			//	Utiliser Focus() en lieu et place de SetFocused(true), pour
			//	avoir une gestion complète des conditions de focus.
			
			bool old_focus = this.KeyboardFocus;
			bool new_focus = focused;
			
			if (old_focus == new_focus)
			{
				return;
			}
			
			Window window = this.Window;

			if (new_focus)
			{
				if (window != null)
				{
					this.SetValue (Visual.KeyboardFocusProperty, true);
					window.FocusedWidget = this;
				}
				
				this.Invalidate (InvalidateReason.FocusedChanged);
			}
			else
			{
				this.SetValue (Visual.KeyboardFocusProperty, false);
				this.ClearValueBase (Visual.KeyboardFocusProperty);
				
				if (window != null)
				{
					window.FocusedWidget = null;
				}
				
				this.Invalidate (InvalidateReason.FocusedChanged);
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
			
			if ((this.internal_state & InternalState.Frozen) != 0)
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
		
		
		public CommandState GetCommandState()
		{
			if (this.IsCommand)
			{
				return CommandCache.Default.GetCommandState (this);
			}
			else
			{
				return null;
			}
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
		
		
		public void SetEmbedder(Widget embedder)
		{
			this.SetParent (embedder);
			this.internal_state |= InternalState.Embedded;
		}
		
		
		public override string ToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append (this.VisualSerialId.ToString ());
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
					
					if ((this.Parent != null) &&
						(this.Parent.IsEntered == false) &&
						(!(this.Parent is WindowRoot)))
					{
						this.Parent.SetEntered (true);
					}
					
					message = Message.FromMouseEvent (MessageType.MouseEnter, null, null);
					
					this.OnEntered (new MessageEventArgs (message, Message.CurrentState.LastPosition));
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
					
					this.OnExited (new MessageEventArgs (message, Message.CurrentState.LastPosition));
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
				
				if (Helpers.VisualTree.IsAncestor (widget, candidate) == false)
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
				(point_in_widget.X >= widget.Client.Size.Width) ||
				(point_in_widget.Y >= widget.Client.Size.Height) ||
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
			return this.Bounds.Contains (point);
		}
		
		
		public virtual Drawing.Rectangle GetPaintBounds()
		{
			return this.GetShapeBounds ();
		}
		
		
		public virtual Drawing.Rectangle GetShapeBounds()
		{
			return this.Client.Bounds;
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
			
			if (this.Parent != null)
			{
				clip = Drawing.Rectangle.Intersection (clip, this.MapParentToClient (this.Parent.GetClipStackBounds ()));
			}
			
			return clip;
		}
		
		public override void Invalidate()
		{
			bool invalidate = false;
			
			if (this.IsVisible)
			{
				invalidate = true;
			}
			else
			{
				Window window = this.Window;
				
				if ((window == null) ||
					(window.IsVisible == false))
				{
					invalidate = true;
				}
			}
			
			if (invalidate)
			{
				this.Invalidate (this.GetPaintBounds ());
			}
		}
		
		public override void Invalidate(Drawing.Rectangle rect)
		{
			bool invalidate = false;
			
			if (this.IsVisible)
			{
				invalidate = true;
			}
			else
			{
				Window window = this.Window;
				
				if ((window == null) ||
					(window.IsVisible == false))
				{
					invalidate = true;
				}
			}
			
			if (invalidate)
			{
				if (this.Parent != null)
				{
					if ((this.InternalState & InternalState.SyncPaint) != 0)
					{
						Window window = this.Window;
						
						if (window != null)
						{
							if (window.IsSyncPaintDisabled)
							{
								this.Parent.Invalidate (this.MapClientToParent (rect));
							}
							else
							{
								window.SynchronousRepaint ();
								this.Parent.Invalidate (this.MapClientToParent (rect));
								window.PaintFilter = new WidgetPaintFilter (this);
								window.SynchronousRepaint ();
								window.PaintFilter = null;
							}
						}
					}
					else
					{
						this.Parent.Invalidate (this.MapClientToParent (rect));
					}
				}
			}
		}
		
		public virtual void Invalidate(InvalidateReason reason)
		{
			this.Invalidate ();
		}
		
		
		public virtual Drawing.Point MapParentToClient(Drawing.Point point)
		{
			Drawing.Point result = new Drawing.Point ();
			
			result.X = point.X - this.Left;
			result.Y = point.Y - this.Bottom;
			
			return result;
		}
		
		public virtual Drawing.Point MapClientToParent(Drawing.Point point)
		{
			Drawing.Point result = new Drawing.Point ();
			
			result.X = point.X + this.Left;
			result.Y = point.Y + this.Bottom;
			
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
			
			if (flip_x) rect.FlipX ();
			if (flip_y) rect.FlipY ();
			
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
			
			if (flip_x) rect.FlipX ();
			if (flip_y) rect.FlipY ();
			
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
			
			if (flip_x) rect.FlipX ();
			if (flip_y) rect.FlipY ();
			
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
			
			if (flip_x) rect.FlipX ();
			if (flip_y) rect.FlipY ();
			
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
			
			if (flip_x) rect.FlipX ();
			if (flip_y) rect.FlipY ();
			
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
			
			if (flip_x) rect.FlipX ();
			if (flip_y) rect.FlipY ();
			
			return rect;
		}

		
		public virtual Drawing.Size MapParentToClient(Drawing.Size size)
		{
			Drawing.Size result = new Drawing.Size ();
			
			result.Width  = size.Width;
			result.Height = size.Height;
			
			return result;
		}
		
		public virtual Drawing.Size MapClientToParent(Drawing.Size size)
		{
			Drawing.Size result = new Drawing.Size ();
			
			result.Width  = size.Width;
			result.Height = size.Height;
			
			return result;
		}
		
		
		
		
		public static void BaseLineAlign(Widget model, Widget widget)
		{
			if ((model == null) ||
				(widget == null))
			{
				return;
			}
			
			double model_offset  = model.GetBaseLine ().Y;
			double widget_offset = widget.GetBaseLine ().Y;
			
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
						if (widget.Visibility == false)
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
						if (widget.Visibility == false)
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
				
				if ((widget.Name == null) ||
					(widget.Name.Length == 0))
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
				
				if ((widget.Name == null) ||
					(widget.Name.Length == 0))
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
			
			if ((this.Name == null) ||
				(this.Name.Length == 0))
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
					
					if (this.KeyboardFocus)
					{
						return this;
					}
					
					if (Helpers.VisualTree.IsDescendant (this, window.FocusedWidget))
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
		
#if false //#fix
		public static Widget[]	FindAllCommandWidgets(string command)
		{
			return Widget.FindAllCommandWidgets (command, null);
		}
		
		public static Widget[]	FindAllCommandWidgets(System.Text.RegularExpressions.Regex regex)
		{
			return Widget.FindAllCommandWidgets (regex, null);
		}
		
		public static Widget[]	FindAllCommandWidgets(string command, CommandDispatcher dispatcher)
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
		
		public static Widget[]	FindAllCommandWidgets(System.Text.RegularExpressions.Regex regex, CommandDispatcher dispatcher)
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
#endif
		
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
			
			if (this.AutoRadio)
			{
				if (mode == TabNavigationMode.ActivateOnCursorX)
				{
					Helpers.GroupController controller = Helpers.GroupController.GetGroupController (this);
					
					find = controller.FindXWidget (this, dir == TabNavigationDir.Backwards ? -1 : 1);
					
					if ((find == null) &&
						(controller.FindXWidget (this, dir == TabNavigationDir.Backwards ? 1 : -1) == null))
					{
						//	L'utilisateur demande un déplacement horizontal bien que la disposition
						//	soit purement verticale. On corrige pour lui :
						
						find = controller.FindYWidget (this, dir == TabNavigationDir.Backwards ? -1 : 1);
					}
					
					if (find != null)
					{
						find.ActiveState = ActiveState.Yes;
						return find;
					}
				}
				
				if (mode == TabNavigationMode.ActivateOnCursorY)
				{
					Helpers.GroupController controller = Helpers.GroupController.GetGroupController (this);
					
					find = controller.FindYWidget (this, dir == TabNavigationDir.Backwards ? -1 : 1);
					
					if ((find == null) &&
						(controller.FindYWidget (this, dir == TabNavigationDir.Backwards ? 1 : -1) == null))
					{
						//	L'utilisateur demande un déplacement vertical bien que la disposition
						//	soit purement horizontale. On corrige pour lui :
						
						find = controller.FindXWidget (this, dir == TabNavigationDir.Backwards ? -1 : 1);
					}
					
					if (find != null)
					{
						find.ActiveState = ActiveState.Yes;
						return find;
					}
				}
			}
			
			if ((!disable_first_enter) &&
				((this.tab_navigation_mode & TabNavigationMode.ForwardToChildren) != 0) &&
				(this.HasChildren))
			{
				//	Ce widget permet aux enfants d'entrer dans la liste accessible par la
				//	touche TAB.
				
				Widget[] candidates = this.Children.Widgets[0].FindTabWidgets (mode);
				
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
									
									Widget[] candidates = find.Children.Widgets[0].FindTabWidgets (mode);
									
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
									
									Widget[] candidates = find.Children.Widgets[0].FindTabWidgets (mode);
									
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
						find = this.Children.FindNext (find) as Widget;
					}
					else if (dir == TabNavigationDir.Backwards)
					{
						find = this.Children.FindPrevious (find) as Widget;
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
				
				if (this.Parent != null)
				{
					if (this.Parent.ProcessTabChildrenExit (dir, mode, out find))
					{
						return find;
					}
					
					find = null;
					
					if ((this.Parent.tab_navigation_mode & TabNavigationMode.ForwardToChildren) != 0)
					{
						bool accept;
						
						switch (dir)
						{
							case TabNavigationDir.Backwards:
								accept = (this.Parent.tab_navigation_mode & TabNavigationMode.ForwardOnly) == 0;
								find   = this.Parent.FindTabWidget (dir, mode, true, accept);
								break;
							
							case TabNavigationDir.Forwards:
								accept = false;
								find = this.Parent.FindTabWidget (dir, mode, true, accept);
								break;
						}
					}
				}
				else if (this.HasChildren)
				{
					//	Il n'y a plus de parents au-dessus. C'est donc vraisemblablement WindowRoot et
					//	dans ce cas, il ne sert à rien de boucler. On va simplement tenter d'activer le
					//	premier descendant trouvé :
					
					Widget[] candidates = this.Children.Widgets[0].FindTabWidgets (mode);
					
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
			
			Widget parent = this.Parent;
			
			if (parent != null)
			{
				Widget[] siblings = parent.Children.Widgets;
				
				for (int i = 0; i < siblings.Length; i++)
				{
					Widget sibling = siblings[i];
					
					if (((sibling.TabNavigation & mode) != 0) &&
						(sibling.IsEnabled) &&
						(sibling.Visibility))
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
			
			if ((mode == TabNavigationMode.ActivateOnTab) &&
				(this.AutoRadio))
			{
				//	On recherche les frères de ce widget, pour déterminer lequel devra être activé par la
				//	pression de la touche TAB. Pour bien faire, il faut supprimer les autres boutons radio
				//	qui appartiennent à notre groupe :
			
				System.Collections.ArrayList copy = new System.Collections.ArrayList ();
				
				string group = this.Group;
				
				foreach (Widget widget in list)
				{
					if ((widget != this) &&
						(widget.Group == group))
					{
						//	Saute les boutons du même groupe. Ils ne sont pas accessibles par la
						//	touche TAB.
					}
					else
					{
						copy.Add (widget);
					}
				}
				
				return copy;
			}
			else
			{
				return list;
			}
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
			if ((this.AutoRadio) &&
				(this.ActiveState != ActiveState.Yes) &&
				(mode == TabNavigationMode.ActivateOnTab))
			{
				//	Ce n'est pas ce bouton radio qui est allumé. TAB voudrait nous
				//	donner le focus, mais ce n'est pas adéquat; mieux vaut mettre
				//	le focus sur le frère qui est actuellement activé :
				
				Helpers.GroupController controller    = Helpers.GroupController.GetGroupController (this);
				Widget                  active_widget = controller.FindActiveWidget ();
				
				if (active_widget != null)
				{
					return active_widget.AboutToGetFocus (dir, mode, out focus);
				}
			}
			
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
		
		internal void ExecuteCommand(string command)
		{
			Window window = this.Window;
			
			if (window != null)
			{
				window.QueueCommand (this, command, this);
			}
		}
		
		
		internal void InternalUpdateGeometry()
		{
			this.UpdateClientGeometry ();
		}

		protected override void SetBoundsOverride(Drawing.Rectangle oldRect, Drawing.Rectangle newRect)
		{
			base.SetBoundsOverride(oldRect, newRect);
			
			if (this.TextLayout != null)
			{
				this.UpdateTextLayout ();
			}
		}
		
		
		protected override void UpdateClientGeometry()
		{
		}
		
		protected virtual void UpdateTextLayout()
		{
			if (this.text_layout != null)
			{
				this.text_layout.Alignment  = this.Alignment;
				this.text_layout.LayoutSize = this.Client.Size;
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
				
				if (child.Visibility == false)
				{
					continue;
				}

				Drawing.Size margins = child.Margins.Size;
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
			
			double pad_width  = this.Padding.Width  + this.InternalPadding.Width;
			double pad_height = this.Padding.Height + this.InternalPadding.Height;
			
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
			
//			System.Diagnostics.Debug.Assert (this.client_info != null);
			System.Diagnostics.Debug.Assert (this.HasChildren);
			
			System.Collections.Queue fill_queue = null;
			Drawing.Rectangle client_rect = this.Client.Bounds;
			Drawing.Rectangle bounds;
			
			client_rect.Deflate (this.Padding);
			client_rect.Deflate (this.InternalPadding);
			
			double push_dx = 0;
			double push_dy = 0;
			
			for (int i = 0; i < children.Length; i++)
			{
				Widget child = children[i];
				
				if (child.Dock == DockStyle.None)
				{
					//	Saute les widgets qui ne sont pas "docked", car ils doivent être
					//	positionnés par d'autres moyens.
					
					continue;
				}
				
				if (child.Visibility == false)
				{
					continue;
				}
				
				bounds = child.Bounds;
				bounds.Inflate (child.Margins);

				double dx = child.PreferredWidth;
				double dy = child.PreferredHeight;

				if (double.IsNaN (dx))
				{
					dx = child.Width;		//	TODO: améliorer
				}
				if (double.IsNaN (dy))
				{
					dy = child.Height;		//	TODO: améliorer
				}

				dx += child.Margins.Width;
				dy += child.Margins.Height;
				
				switch (child.Dock)
				{
					case DockStyle.Top:
						bounds = new Drawing.Rectangle (client_rect.Left, client_rect.Top - dy, client_rect.Width, dy);
						bounds.Deflate (child.Margins);
						child.SetBounds (bounds);
						client_rect.Top -= dy;
						break;
						
					case DockStyle.Bottom:
						bounds = new Drawing.Rectangle (client_rect.Left, client_rect.Bottom, client_rect.Width, dy);
						bounds.Deflate (child.Margins);
						child.SetBounds (bounds);
						client_rect.Bottom += dy;
						break;
					
					case DockStyle.Left:
						bounds = new Drawing.Rectangle (client_rect.Left, client_rect.Bottom, dx, client_rect.Height);
						bounds.Deflate (child.Margins);
						child.SetBounds (bounds);
						client_rect.Left += dx;
						break;
					
					case DockStyle.Right:
						bounds = new Drawing.Rectangle (client_rect.Right - dx, client_rect.Bottom, dx, client_rect.Height);
						bounds.Deflate (child.Margins);
						child.SetBounds (bounds);
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
							double min_dx = child.MinWidth;
							double new_dx = fill_dx / n;
							
							if (new_dx < min_dx)
							{
								push_dx += min_dx - new_dx;
								new_dx   = min_dx;
							}
							
							bounds = new Drawing.Rectangle (client_rect.Left, client_rect.Bottom, new_dx, client_rect.Height);
							bounds.Deflate (child.Margins);
						
							child.SetBounds (bounds);
							client_rect.Left += new_dx;
						}
						break;
					
					case ContainerLayoutMode.VerticalFlow:
						foreach (Widget child in fill_queue)
						{
							double min_dy = child.MinHeight;
							double new_dy = fill_dy / n;
							
							if (new_dy < min_dy)
							{
								push_dy += min_dy - new_dy;
								new_dy   = min_dy;
							}
							
							bounds = new Drawing.Rectangle (client_rect.Left, client_rect.Top - new_dy, client_rect.Width, new_dy);
							bounds.Deflate (child.Margins);
							
							child.SetBounds (bounds);
							client_rect.Top -= new_dy;
						}
						break;
				}
			}
			
			if (push_dy > 0)
			{
				for (int i = 0; i < children.Length; i++)
				{
					Widget child = children[i];
					
					if ((child.Dock != DockStyle.Bottom) ||
						(child.Visibility == false))
					{
						continue;
					}
					
					bounds = child.Bounds;
					bounds.Offset (0, - push_dy);
					child.SetBounds (bounds);
				}
			}
			
			if (push_dx > 0)
			{
				for (int i = 0; i < children.Length; i++)
				{
					Widget child = children[i];
					
					if ((child.Dock != DockStyle.Right) ||
						(child.Visibility == false))
					{
						continue;
					}
					
					bounds = child.Bounds;
					bounds.Offset (push_dx, 0);
					child.SetBounds (bounds);
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
				Drawing.Transform graphics_transform = Drawing.Transform.FromTranslation (this.Location);
				
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
						
							if (widget.Visibility)
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
		
		public void DispatchDummyMouseUpEvent(MouseButtons button, Drawing.Point pos)
		{
			Window window = this.Window;
			
			if (window != null)
			{
				window.DispatchMessage (Message.CreateDummyMouseUpEvent (button, pos));
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

				WindowRoot root = message.WindowRoot ?? Helpers.VisualTree.GetWindowRoot (this);
				
				for (int i = 0; i < children_num; i++)
				{
					Widget widget         = children[children_num-1 - i];
					bool   contains_focus = (root == null) ? false : root.DoesVisualContainKeyboardFocus (widget);
					
					if ((widget.IsFrozen == false) &&
						((widget.Visibility) || (contains_focus && message.IsKeyType)) &&
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
			if (this.Visibility || message.IsKeyType)
			{
				bool is_entered = this.IsEntered;
				
				switch (message.Type)
				{
					case MessageType.MouseUp:
						
						if (Message.CurrentState.IsSameWindowAsButtonDown == false)
						{
							return;
						}
						
						if (! this.AutoDoubleClick)
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
				
					if (widget.ContainsKeyboardFocus)
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
					(widget.Visibility))
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
			if ((this.shortcuts != null) &&
				(this.shortcuts.Match (shortcut)))
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
				this.Shortcuts.Define (this.GetMnemonicShortcuts ());
			}
		}
		
		protected virtual Shortcut[] GetMnemonicShortcuts()
		{
			if (this.Mnemonic == '\0')
			{
				return new Shortcut[0];
			}
			else
			{
				Shortcut[] shortcuts = new Shortcut[2];
				
				shortcuts[0] = new Shortcut (this.Mnemonic, ModifierKeys.None);
				shortcuts[1] = new Shortcut (this.Mnemonic, ModifierKeys.Alt);
				
				return shortcuts;
			}
		}
		
		protected virtual void BuildFullPathName(System.Text.StringBuilder buffer)
		{
			if (this.Parent != null)
			{
				this.Parent.BuildFullPathName (buffer);
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
		

		protected override void OnParentChanged(Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			base.OnParentChanged (e);
			
			Widget old_parent = e.OldValue as Widget;
			Widget new_parent = e.NewValue as Widget;
			
			if (new_parent == null)
			{
				this.AboutToBecomeOrphan ();
			}
			
			Window old_window = old_parent == null ? null : Helpers.VisualTree.GetWindow (old_parent);
			Window new_window = new_parent == null ? null : Helpers.VisualTree.GetWindow (new_parent);
			
			if (old_window != new_window)
			{
				this.NotifyWindowChanged (old_window, new_window);
			}
		}
		
		protected virtual void NotifyWindowChanged(Window old_window, Window new_window)
		{
			foreach (Widget widget in this.Children.Widgets)
			{
				widget.NotifyWindowChanged (old_window, new_window);
			}
		}
		


#if false //#fix
		protected void HandleParentChanged()
		{
			//	Cette méthode est appelée chaque fois qu'un widget change de parent.
			
			this.OnParentChanged ();
			
			if (this.Parent == null)
			{
				if (this.Visibility)
				{
					this.NotifyChangedToHidden ();
				}
			}
			else
			{
				if ((this.Visibility) &&
					(this.IsVisible))
				{
					this.NotifyChangedToVisible ();
				}
			}
		}
#endif
		
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
			PaintEventHandler handler = this.GetUserEventHandler ("PaintBackground") as PaintEventHandler;
			
			if (handler != null)
			{
				e.Suppress = false;
				
				handler (this, e);
				
				if (e.Suppress)
				{
					return;
				}
			}
			
			this.PaintBackgroundImplementation (e.Graphics, e.ClipRectangle);
		}
		
		protected virtual void OnPaintForeground(PaintEventArgs e)
		{
			PaintEventHandler handler = this.GetUserEventHandler ("PaintForeground") as PaintEventHandler;
			
			if (handler != null)
			{
				e.Suppress = false;
				
				handler (this, e);
				
				if (e.Suppress)
				{
					return;
				}
			}
			
			this.PaintForegroundImplementation (e.Graphics, e.ClipRectangle);
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
		
		protected virtual void OnPressed(MessageEventArgs e)
		{
	//		protected virtual void OnLayoutUpdate(Layouts.UpdateEventArgs e)
	//		{
	//			if (this.LayoutUpdate != null)
	//			{
	//				this.LayoutUpdate (this, e);
	//			}
	//		}
			
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
//-					e.Message.Consumer = this;
				}
				
				this.Entered (this, e);
			}
		}
		
		protected virtual void OnExited(MessageEventArgs e)
		{
			if (this.Parent != null)
			{
				Window window = this.Window;
				
				if (window != null)
				{
					if (window.CapturingWidget == null)
					{
						window.MouseCursor = this.Parent.MouseCursor;
					}
				}
			}
			
			if (this.Exited != null)
			{
				if (e != null)
				{
//-					e.Message.Consumer = this;
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
		
		protected override void OnSizeChanged(Types.DependencyPropertyChangedEventArgs e)
		{
		}
		
		protected virtual void OnLocationChanged()
		{
			if (this.LocationChanged != null)
			{
				this.LocationChanged (this);
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
			if ((this.ActiveState == ActiveState.Yes) &&
				(this.Group != null) &&
				(this.Group.Length > 0))
			{
				//	Eteint les autres boutons du groupe (s'il y en a) :
				
				Helpers.GroupController controller = Helpers.GroupController.GetGroupController (this);
				
				controller.TurnOffAllButOne (this);
				controller.SetActiveIndex (this.Index);
			}
			
			if (this.ActiveStateChanged != null)
			{
				this.ActiveStateChanged (this);
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
		
		public event PaintEventHandler				PaintBackground
		{
			add
			{
				this.AddUserEventHandler ("PaintBackground", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("PaintBackground", value);
			}
		}
		
		public event PaintEventHandler				PaintForeground
		{
			add
			{
				this.AddUserEventHandler ("PaintForeground", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("PaintForeground", value);
			}
		}
		
		#region Events
		public event Support.EventHandler			ClientGeometryUpdated;
		public event Support.EventHandler			PreparePaint;
		public event Support.EventHandler			AdornerChanged;
		public event Support.EventHandler			CultureChanged;
		public event Support.EventHandler			LayoutChanged;
		
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
		
		public event Support.EventHandler			Selected;
		public event Support.EventHandler			Deselected;
		public event Support.EventHandler			Engaged;
		public event Support.EventHandler			StillEngaged;
		public event Support.EventHandler			Disengaged;
		public event Support.EventHandler			ActiveStateChanged;
		public event Support.EventHandler			Disposed;
		public event Support.EventHandler			TextDefined;
		public event Support.EventHandler			TextChanged;
		public event Support.EventHandler			NameChanged;
		public event Support.EventHandler			LocationChanged;
		#endregion
		
		#region Various enums
		public enum Setting : byte
		{
			None				= 0,
			IncludeChildren		= 1
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
		
		
		private InternalState					internal_state;
		private WidgetState						widget_state;
		
		private Drawing.Size					auto_min_size	= new Drawing.Size (0, 0);
		private Drawing.Size					auto_max_size	= new Drawing.Size (1000000, 1000000);
		private System.Collections.ArrayList	hypertext_list;
		private HypertextInfo					hypertext;
		
		private string							text;
		private TextLayout						text_layout;
		private ContentAlignment				alignment;
		private int								tab_index = 0;
		private TabNavigationMode				tab_navigation_mode;
		private Collections.ShortcutCollection	shortcuts;
		private double							default_font_height;
		private MouseCursor						mouse_cursor;
		private System.Collections.Hashtable	property_hash;
		private Support.ResourceManager			resource_manager;
		private IValidator						validator;
		
		static System.Collections.ArrayList		entered_widgets = new System.Collections.ArrayList ();
		static System.Collections.ArrayList		alive_widgets   = new System.Collections.ArrayList ();
		static bool								debug_dispose	= false;
		
		private const string					prop_binding	= "$widget$binding$";
	}
}
