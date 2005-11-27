//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets.Helpers;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// Visual.
	/// </summary>
	public class Visual : Types.Object, ICommandDispatcherHost
	{
		public Visual()
		{
		}
		
		
		public int								Index
		{
			get
			{
				return (int) this.GetValue (Visual.IndexProperty);
			}
			set
			{
				this.SetValue (Visual.IndexProperty, value);
			}
		}
		
		public string							Group
		{
			get
			{
				return (string) this.GetValue (Visual.GroupProperty);
			}
			set
			{
				this.SetValue (Visual.GroupProperty, value);
			}
		}
		
		public string							Name
		{
			get
			{
				return (string) this.GetValue (Visual.NameProperty);
			}
			set
			{
				this.SetValue (Visual.NameProperty, value);
			}
		}
		
		public Visual							Parent
		{
			get
			{
				if (this.parent_layer == null)
				{
					return null;
				}
				else
				{
					return this.parent_layer.Visual;
				}
			}
		}
		
		public Layouts.Layer					ParentLayer
		{
			get
			{
				return this.parent_layer;
			}
		}
		
		
		public CommandDispatcher				CommandDispatcher
		{
			get
			{
				return (CommandDispatcher) this.GetValue (Visual.CommandDispatcherProperty);
			}
		}
		
		public string							Command
		{
			get
			{
				return (string) this.GetValue (Visual.CommandProperty);
			}
			set
			{
				this.SetValue (Visual.CommandProperty, value);
			}
		}
		
		public string							CommandName
		{
			get
			{
				return CommandDispatcher.ExtractCommandName (this.Command);
			}
		}
		
		
		public AnchorStyles						Anchor
		{
			get
			{
				return (AnchorStyles) this.GetValue (Visual.AnchorProperty);
			}
			set
			{
				this.SetValue (Visual.AnchorProperty, value);
			}
		}
		
		public Drawing.Margins					AnchorMargins
		{
			get
			{
				return (Drawing.Margins) this.GetValue (Visual.AnchorMarginsProperty);
			}
			set
			{
				this.SetValue (Visual.AnchorMarginsProperty, value);
			}
		}
		
		public DockStyle						Dock
		{
			get
			{
				return (DockStyle) this.GetValue (Visual.DockProperty);
			}
			set
			{
				this.SetValue (Visual.DockProperty, value);
			}
		}
		
		public Drawing.Margins					DockPadding
		{
			get
			{
				return (Drawing.Margins) this.GetValue (Visual.DockPaddingProperty);
			}
			set
			{
				this.SetValue (Visual.DockPaddingProperty, value);
			}
		}
		
		public Drawing.Margins					DockMargins
		{
			get
			{
				return (Drawing.Margins) this.GetValue (Visual.DockMarginsProperty);
			}
			set
			{
				this.SetValue (Visual.DockMarginsProperty, value);
			}
		}
		
		public ContainerLayoutMode				ContainerLayoutMode
		{
			get
			{
				return (ContainerLayoutMode) this.GetValue (Visual.ContainerLayoutModeProperty);
			}
			set
			{
				this.SetValue (Visual.ContainerLayoutModeProperty, value);
			}
		}
		
		
		public Drawing.Size						Size
		{
			get
			{
				return this.Bounds.Size;
			}
			set
			{
				Drawing.Rectangle bounds = this.Bounds;
				
				bounds.Size = value;
				
				this.Bounds = bounds;
			}
		}
		
		public Drawing.Point					Location
		{
			get
			{
				return this.Bounds.Location;
			}
			set
			{
				Drawing.Rectangle bounds = this.Bounds;
				
				bounds.Location = value;
				
				this.Bounds = bounds;
			}
		}
		
		public Drawing.Rectangle				Bounds
		{
			get
			{
				return (Drawing.Rectangle) this.GetValueBase (Visual.BoundsProperty);
			}
			set
			{
				if (this.Bounds != value)
				{
					Visual parent = this.Parent;
					
					this.SuspendLayout ();
					
					if (parent == null)
					{
						this.PreferredSize = value.Size;
						
						this.SetBounds (value);
					}
					else
					{
						Drawing.Size host = parent.Client.Size;
						
						this.PreferredSize = value.Size;
						this.AnchorMargins = new Drawing.Margins (value.Left, host.Width - value.Right, host.Height - value.Top, value.Bottom);
						
						if (this.Anchor == AnchorStyles.None)
						{
							this.SetBounds (value);
						}
					}
					
					this.NotifyGeometryChanged ();
					this.ResumeLayout ();
				}
			}
		}
		
		public Drawing.Size						PreferredSize
		{
			get
			{
				return (Drawing.Size) this.GetValue (Visual.PreferredSizeProperty);
			}
			set
			{
				this.SetValue (Visual.PreferredSizeProperty, value);
			}
		}
		
		public Widget.ClientInfo				Client
		{
			get
			{
				return new Widget.ClientInfo (this.Size);
			}
		}
		
		
		public virtual Drawing.Margins			InternalPadding
		{
			get
			{
				return Drawing.Margins.Zero;
			}
		}
		
		
		public virtual bool						IsKeyboardFocused
		{
			get
			{
				return false;
			}
		}
		
		public bool								ContainsKeyboardFocus
		{
			get
			{
				return Helpers.VisualTree.ContainsKeyboardFocus (this);
			}
		}
		
		
		public bool								InheritParentFocus
		{
			get
			{
				return (bool) this.GetValue (Visual.InheritParentFocusProperty);
			}
			set
			{
				this.SetValue (Visual.InheritParentFocusProperty, value);
			}
		}
		
		
		public double							Left
		{
			get
			{
				return this.Bounds.Left;
			}
			set
			{
				Drawing.Rectangle bounds = this.Bounds;
				
				bounds.Left = value;
				
				this.Bounds = bounds;
			}
		}
		
		public double							Right
		{
			get
			{
				return this.Bounds.Right;
			}
			set
			{
				Drawing.Rectangle bounds = this.Bounds;
				
				bounds.Right = value;
				
				this.Bounds = bounds;
			}
		}
		
		public double							Top
		{
			get
			{
				return this.Bounds.Top;
			}
			set
			{
				Drawing.Rectangle bounds = this.Bounds;
				
				bounds.Top = value;
				
				this.Bounds = bounds;
			}
		}
		
		public double							Bottom
		{
			get
			{
				return this.Bounds.Bottom;
			}
			set
			{
				Drawing.Rectangle bounds = this.Bounds;
				
				bounds.Bottom = value;
				
				this.Bounds = bounds;
			}
		}
		
		public double							Width
		{
			get
			{
				return this.Bounds.Width;
			}
			set
			{
				Drawing.Rectangle bounds = this.Bounds;
				
				bounds.Width = value;
				
				this.Bounds = bounds;
			}
		}
		
		public double							Height
		{
			get
			{
				return this.Bounds.Height;
			}
			set
			{
				Drawing.Rectangle bounds = this.Bounds;
				
				bounds.Height = value;
				
				this.Bounds = bounds;
			}
		}
		
		
		public Drawing.Size						MinSize
		{
			get
			{
				return (Drawing.Size) this.GetValue (Visual.MinSizeProperty);
			}
			set
			{
				this.SetValue (Visual.MinSizeProperty, value);
			}
		}
		
		public Drawing.Size						MaxSize
		{
			get
			{
				return (Drawing.Size) this.GetValue (Visual.MaxSizeProperty);
			}
			set
			{
				this.SetValue (Visual.MaxSizeProperty, value);
			}
		}
		
		
		public Drawing.Size						ResultingMinSize
		{
			get
			{
				//	TODO: tenir compte de la taille des enfants
				return this.MinSize;
			}
		}
		
		public Drawing.Size						ResultingMaxSize
		{
			get
			{
				//	TODO: tenir compte de la taille des enfants
				return this.MaxSize;
			}
		}
		
		
		public bool								Visibility
		{
			get
			{
				return (bool) this.GetValue (Visual.VisibilityProperty);
			}
			set
			{
				Helpers.VisualTreeSnapshot snapshot = Helpers.VisualTree.SnapshotProperties (this, Visual.IsVisibleProperty);
				this.SetValueBase (Visual.VisibilityProperty, value);
				snapshot.InvalidateDifferent ();
			}
		}
		
		public bool								Enable
		{
			get
			{
				return (bool) this.GetValue (Visual.EnableProperty);
			}
			set
			{
				Helpers.VisualTreeSnapshot snapshot = Helpers.VisualTree.SnapshotProperties (this, Visual.IsEnabledProperty);
				this.SetValueBase (Visual.EnableProperty, value);
				snapshot.InvalidateDifferent ();
			}
		}
		
		
		public bool								IsVisible
		{
			get
			{
				return VisualTree.IsVisible (this);
			}
		}
		
		public bool								IsEnabled
		{
			get
			{
				return VisualTree.IsEnabled (this);
			}
		}
		
		public bool								IsFocused
		{
			get
			{
				return VisualTree.IsFocused (this);
			}
		}
		
		
		public bool								AutoCapture
		{
			get
			{
				return (bool) this.GetValue (Visual.AutoCaptureProperty);
			}
			set
			{
				this.SetValue (Visual.AutoCaptureProperty, value);
			}
		}
		
		public bool								AutoFocus
		{
			get
			{
				return (bool) this.GetValue (Visual.AutoFocusProperty);
			}
			set
			{
				this.SetValue (Visual.AutoFocusProperty, value);
			}
		}
		
		public bool								AutoEngage
		{
			get
			{
				return (bool) this.GetValue (Visual.AutoEngageProperty);
			}
			set
			{
				this.SetValue (Visual.AutoEngageProperty, value);
			}
		}
		
		public bool								AutoRepeat
		{
			get
			{
				return (bool) this.GetValue (Visual.AutoRepeatProperty);
			}
			set
			{
				this.SetValue (Visual.AutoRepeatProperty, value);
			}
		}
		
		public bool								AutoToggle
		{
			get
			{
				return (bool) this.GetValue (Visual.AutoToggleProperty);
			}
			set
			{
				this.SetValue (Visual.AutoToggleProperty, value);
			}
		}
		
		public bool								AutoRadio
		{
			get
			{
				return (bool) this.GetValue (Visual.AutoRadioProperty);
			}
			set
			{
				this.SetValue (Visual.AutoRadioProperty, value);
			}
		}
		
		public bool								AutoDoubleClick
		{
			get
			{
				return (bool) this.GetValue (Visual.AutoDoubleClickProperty);
			}
			set
			{
				this.SetValue (Visual.AutoDoubleClickProperty, value);
			}
		}
		
		
		public bool								AcceptThreeState
		{
			get
			{
				return (bool) this.GetValue (Visual.AcceptThreeStatePropery);
			}
			set
			{
				this.SetValue (Visual.AcceptThreeStatePropery, value);
			}
		}
		
		
		public Drawing.Color					BackColor
		{
			get
			{
				return (Drawing.Color) this.GetValue (Visual.BackColorProperty);
			}
			set
			{
				this.SetValue (Visual.BackColorProperty, value);
			}
		}
		
		
		internal bool							IsLayoutSuspended
		{
			get
			{
				return this.suspend_layout_counter > 0 ? true : false;
			}
		}
		
		internal bool							HasLayerCollection
		{
			get
			{
				return this.layer_collection == null ? false : true;
			}
		}
		
		public bool								HasChildren
		{
			get
			{
				if (this.HasLayerCollection)
				{
					Collections.ChildrenCollection children = new Collections.ChildrenCollection (this);
					return children.Count == 0 ? false : true;
				}
				
				return false;
			}
		}
		
		
		
		public Collections.ChildrenCollection	Children
		{
			get
			{
				return this.GetChildrenCollection ();
			}
		}
		
		
		internal Collections.ChildrenCollection GetChildrenCollection()
		{
			return new Collections.ChildrenCollection (this);
		}
		
		
		internal int GetCommandCacheId()
		{
			return this.command_cache_id;
		}
		
		internal void SetCommandCacheId(int value)
		{
			this.command_cache_id = value;
		}
		
		
		internal void SetParentLayer(Layouts.Layer parent_layer)
		{
			Helpers.VisualTreeSnapshot snapshot = Helpers.VisualTree.SnapshotProperties (this, Visual.IsVisibleProperty);
			
			Visual old_parent = this.Parent;
			
			if (old_parent != null)
			{
				old_parent.Invalidate (this.Bounds);
			}
			
			this.parent_layer = parent_layer;
			
			Visual new_parent = this.Parent;
			
			if (new_parent != null)
			{
				new_parent.Invalidate (this.Bounds);
			}
			
			if (old_parent != new_parent)
			{
				this.InvalidateProperty (Visual.ParentProperty, old_parent, new_parent);
			}
			
			snapshot.InvalidateDifferent ();
		}
		
		internal virtual void SetBounds(Drawing.Rectangle value)
		{
			Drawing.Rectangle old_value = this.Bounds;
			this.SetValueBase (Visual.BoundsProperty, value);
			Drawing.Rectangle new_value = this.Bounds;
			
			if (old_value != new_value)
			{
				this.UpdateClientGeometry ();
				
				Visual parent = this.Parent;
				
				if (parent != null)
				{
					parent.Invalidate (old_value);
					parent.Invalidate (new_value);
				}
			}
			if (old_value.Size != new_value.Size)
			{
				this.InvalidateProperty (Visual.SizeProperty, old_value.Size, new_value.Size);
			}
		}
		
		internal Collections.LayerCollection GetLayerCollection()
		{
			if (this.layer_collection == null)
			{
				lock (this)
				{
					if (this.layer_collection == null)
					{
						this.layer_collection = new Collections.LayerCollection (this);
					}
				}
			}
			
			return this.layer_collection;
		}
		
		
		internal void NotifyGeometryChanged()
		{
			this.NotifyLayoutChanged ();
			this.NotifyParentLayoutChanged ();
		}
		
		internal void NotifyChildrenChanged(Layouts.Layer layer)
		{
			this.NotifyLayoutChanged ();
		}
		
		internal void NotifyLayoutChanged()
		{
			if (this.currently_updating_layout > 0)
			{
				return;
			}
			
			this.has_layout_changed = true;
			
			if (this.suspend_layout_counter == 0)
			{
				this.ExecutePendingLayoutOperations ();
			}
		}
		
		internal void NotifyParentLayoutChanged()
		{
			Visual parent = this.Parent;
			
			if (parent != null)
			{
				parent.NotifyLayoutChanged ();
			}
			else
			{
				this.NotifyLayoutChanged ();
			}
		}
		
		internal void NotifyDisplayChanged()
		{
			this.Invalidate ();
		}
		
		
		public void AttachCommandDispatcher(CommandDispatcher value)
		{
			System.Diagnostics.Debug.Assert (value != null);
			
			CommandDispatcher dispatcher = this.CommandDispatcher;
			
			if (dispatcher != null)
			{
				if (dispatcher != value)
				{
					//	TODO: gérer les CommandDispatchers chaînés
				}
			}
            
			this.SetValueBase (Visual.CommandDispatcherProperty, value);
		}
		
		public void DetachCommandDispatcher(CommandDispatcher value)
		{
			System.Diagnostics.Debug.Assert (value != null);
			
			CommandDispatcher dispatcher = this.CommandDispatcher;
			
			if (dispatcher != null)
			{
				if (dispatcher == value)
				{
					value = null;
				}
				else
				{
					//	TODO: gérer les CommandDispatchers chaînés
				}
			}
            
			this.SetValueBase (Visual.CommandDispatcherProperty, value);
		}
		
		public virtual void Invalidate()
		{
		}
		
		public virtual void Invalidate(Drawing.Rectangle rect)
		{
		}
		
		public void SuspendLayout()
		{
			this.suspend_layout_counter++;
		}
		
		public void ResumeLayout()
		{
			this.ResumeLayout (true);
		}
		
		public void ResumeLayout(bool update)
		{
			if (this.suspend_layout_counter > 0)
			{
				this.suspend_layout_counter--;
			}
			if (this.suspend_layout_counter == 0)
			{
				if (update)
				{
					this.ExecutePendingLayoutOperations ();
				}
			}
		}
		
		public virtual void ExecutePendingLayoutOperations()
		{
		}
		
		
		static Visual()
		{
		}
		
		
		private static object GetParentValue(Object o)
		{
			Visual that = o as Visual;
			return that.Parent;
		}
		
		private static object GetParentLayerValue(Object o)
		{
			Visual that = o as Visual;
			return that.ParentLayer;
		}
		
		private static object GetBoundsValue(Object o)
		{
			Visual that = o as Visual;
			return that.Bounds;
		}
		
		private static void SetBoundsValue(Object o, object value)
		{
			Visual that = o as Visual;
			that.Bounds = (Drawing.Rectangle) value;
		}
		
		private static object GetSizeValue(Object o)
		{
			Visual that = o as Visual;
			return that.Size;
		}
		
		private static void SetSizeValue(Object o, object value)
		{
			Visual that = o as Visual;
			that.Size = (Drawing.Size) value;
		}
		
		private static object GetIsEnabledValue(Object o)
		{
			Visual that = o as Visual;
			return that.IsEnabled;
		}
		
		private static object GetIsVisibleValue(Object o)
		{
			Visual that = o as Visual;
			return that.IsVisible;
		}
		
		private static object GetIsFocusedValue(Object o)
		{
			Visual that = o as Visual;
			return that.IsFocused;
		}
		
		private static object GetIsKeyboardFocusedValue(Object o)
		{
			Visual that = o as Visual;
			return that.IsKeyboardFocused;
		}
		
		private static object GetContainsKeyboardFocusValue(Object o)
		{
			Visual that = o as Visual;
			return that.ContainsKeyboardFocus;
		}
		
		private static void SetEnableValue(Object o, object value)
		{
			Visual that = o as Visual;
			that.Enable = (bool) value;
		}
		
		private static void SetVisibilityValue(Object o, object value)
		{
			Visual that = o as Visual;
			that.Visibility = (bool) value;
		}
		
		private static void NotifySizeChanged(Object o, object old_value, object new_value)
		{
			Visual that = o as Visual;
			that.OnSizeChanged (new PropertyChangedEventArgs (Visual.SizeProperty, old_value, new_value));
		}
		
		private static void NotifyParentChanged(Object o, object old_value, object new_value)
		{
			Visual that = o as Visual;
			that.OnParentChanged (new PropertyChangedEventArgs (Visual.ParentProperty, old_value, new_value));
		}
		
		private static void NotifyIsVisibleChanged(Object o, object old_value, object new_value)
		{
			Visual that = o as Visual;
			that.OnIsVisibleChanged (new PropertyChangedEventArgs (Visual.IsVisibleProperty, old_value, new_value));
		}
		
		
		private static void NotifyIsFocusedChanged(Object o, object old_value, object new_value)
		{
			Visual that = o as Visual;
			that.OnIsFocusedChanged (new PropertyChangedEventArgs (Visual.IsFocusedProperty, old_value, new_value));
		}
		
		private static void NotifyIsKeyboardFocusedChanged(Object o, object old_value, object new_value)
		{
			Visual that = o as Visual;
			that.OnIsKeyboardFocusedChanged (new PropertyChangedEventArgs (Visual.IsKeyboardFocusedProperty, old_value, new_value));
		}
		
		private static void NotifyCommandChanged(Object o, object old_value, object new_value)
		{
			Visual that = o as Visual;
			that.OnCommandChanged (new PropertyChangedEventArgs (Visual.CommandProperty, old_value, new_value));
		}
		
		private static void NotifyCommandDispatcherChanged(Object o, object old_value, object new_value)
		{
			Visual that = o as Visual;
			that.OnCommandDispatcherChanged (new PropertyChangedEventArgs (Visual.CommandDispatcherProperty, old_value, new_value));
		}
		
		
		protected virtual void OnSizeChanged(Types.PropertyChangedEventArgs e)
		{
		}
		
		protected virtual void OnParentChanged(Types.PropertyChangedEventArgs e)
		{
			Helpers.VisualTree.InvalidateCommandDispatcher (this);
		}
		
		protected virtual void OnIsVisibleChanged(Types.PropertyChangedEventArgs e)
		{
		}
		
		protected virtual void OnIsFocusedChanged(Types.PropertyChangedEventArgs e)
		{
		}
		
		protected virtual void OnIsKeyboardFocusedChanged(Types.PropertyChangedEventArgs e)
		{
		}
		
		protected virtual void OnCommandChanged(Types.PropertyChangedEventArgs e)
		{
			string old_command = e.OldValue as string;
			string new_command = e.NewValue as string;
			string old_command_name = CommandDispatcher.ExtractCommandName (old_command);
			string new_command_name = CommandDispatcher.ExtractCommandName (new_command);
			
			if (old_command_name == new_command_name)
			{
				//	Quelqu'un a fait un changement minime (espace ou argument de
				//	la commande). Ca ne compte pas ici.
			}
			else if (old_command_name == null)
			{
				CommandCache.Default.AttachVisual (this);
			}
			else if (new_command_name == null)
			{
				CommandCache.Default.DetachVisual (this);
			}
			else
			{
				CommandCache.Default.Invalidate (this);
			}
		}
		
		protected virtual void OnCommandDispatcherChanged(Types.PropertyChangedEventArgs e)
		{
			Helpers.VisualTree.InvalidateCommandDispatcher (this);
		}
		
		
		protected virtual void UpdateClientGeometry()
		{
		}
		
		
		public event PropertyChangedEventHandler	SizeChanged
		{
			add
			{
				this.AddEvent (Visual.SizeProperty, value);
			}
			remove
			{
				this.RemoveEvent (Visual.SizeProperty, value);
			}
		}
		
		public event PropertyChangedEventHandler	ParentChanged
		{
			add
			{
				this.AddEvent (Visual.ParentProperty, value);
			}
			remove
			{
				this.RemoveEvent (Visual.ParentProperty, value);
			}
		}
		
		public event PropertyChangedEventHandler	IsVisibleChanged
		{
			add
			{
				this.AddEvent (Visual.IsVisibleProperty, value);
			}
			remove
			{
				this.RemoveEvent (Visual.IsVisibleProperty, value);
			}
		}
		
		public event PropertyChangedEventHandler	IsEnabledChanged
		{
			add
			{
				this.AddEvent (Visual.IsEnabledProperty, value);
			}
			remove
			{
				this.RemoveEvent (Visual.IsEnabledProperty, value);
			}
		}
		
		public event PropertyChangedEventHandler	IsFocusedChanged
		{
			add
			{
				this.AddEvent (Visual.IsFocusedProperty, value);
			}
			remove
			{
				this.RemoveEvent (Visual.IsFocusedProperty, value);
			}
		}
		
		public event PropertyChangedEventHandler	IsKeyboardFocusedChanged
		{
			add
			{
				this.AddEvent (Visual.IsKeyboardFocusedProperty, value);
			}
			remove
			{
				this.RemoveEvent (Visual.IsKeyboardFocusedProperty, value);
			}
		}
		
		public event PropertyChangedEventHandler	ContainsKeyboardFocusChanged
		{
			add
			{
				this.AddEvent (Visual.ContainsKeyboardFocusProperty, value);
			}
			remove
			{
				this.RemoveEvent (Visual.ContainsKeyboardFocusProperty, value);
			}
		}
		
		
		public static readonly Property IndexProperty				= Property.Register ("Index", typeof (int), typeof (Visual), new PropertyMetadata (-1));
		public static readonly Property GroupProperty				= Property.Register ("Group", typeof (string), typeof (Visual));
		public static readonly Property NameProperty				= Property.Register ("Name", typeof (string), typeof (Visual));
		public static readonly Property ParentProperty				= Property.RegisterReadOnly ("Parent", typeof (Visual), typeof (Visual), new PropertyMetadata (new GetValueOverrideCallback (Visual.GetParentValue), new PropertyInvalidatedCallback (Visual.NotifyParentChanged)));
		public static readonly Property ParentLayerProperty			= Property.RegisterReadOnly ("ParentLayer", typeof (Layouts.Layer), typeof (Visual), new PropertyMetadata (new GetValueOverrideCallback (Visual.GetParentLayerValue)));
		
		public static readonly Property AnchorProperty				= Property.Register ("Anchor", typeof (AnchorStyles), typeof (Visual), new VisualPropertyMetadata (AnchorStyles.None, VisualPropertyFlags.AffectsParentLayout));
		public static readonly Property AnchorMarginsProperty		= Property.Register ("AnchorMargins", typeof (Drawing.Margins), typeof (Visual), new VisualPropertyMetadata (Drawing.Margins.Zero, VisualPropertyFlags.AffectsParentLayout));
		public static readonly Property DockProperty				= Property.Register ("Dock", typeof (DockStyle), typeof (Visual), new VisualPropertyMetadata (DockStyle.None, VisualPropertyFlags.AffectsParentLayout));
		public static readonly Property DockPaddingProperty			= Property.Register ("DockPadding", typeof (Drawing.Margins), typeof (Visual), new VisualPropertyMetadata (Drawing.Margins.Zero, VisualPropertyFlags.AffectsParentLayout));
		public static readonly Property DockMarginsProperty			= Property.Register ("DockMargins", typeof (Drawing.Margins), typeof (Visual), new VisualPropertyMetadata (Drawing.Margins.Zero, VisualPropertyFlags.AffectsParentLayout));
		public static readonly Property ContainerLayoutModeProperty	= Property.Register ("ContainerLayoutMode", typeof (ContainerLayoutMode), typeof (Visual), new VisualPropertyMetadata (ContainerLayoutMode.VerticalFlow, VisualPropertyFlags.AffectsLayout));
		
		public static readonly Property BoundsProperty				= Property.Register ("Bounds", typeof (Drawing.Rectangle), typeof (Visual), new PropertyMetadata (Drawing.Rectangle.Empty, new GetValueOverrideCallback (Visual.GetBoundsValue), new SetValueOverrideCallback (Visual.SetBoundsValue)));
		public static readonly Property SizeProperty				= Property.Register ("Size", typeof (Drawing.Size), typeof (Visual), new PropertyMetadata (new GetValueOverrideCallback (Visual.GetSizeValue), new SetValueOverrideCallback (Visual.SetSizeValue), new PropertyInvalidatedCallback (Visual.NotifySizeChanged)));
		public static readonly Property PreferredSizeProperty		= Property.Register ("PreferredSize", typeof (Drawing.Size), typeof (Visual), new VisualPropertyMetadata (Drawing.Size.Empty, VisualPropertyFlags.AffectsParentLayout));
		public static readonly Property MinSizeProperty				= Property.Register ("MinSize", typeof (Drawing.Size), typeof (Visual), new VisualPropertyMetadata (Drawing.Size.Empty, VisualPropertyFlags.AffectsParentLayout));
		public static readonly Property MaxSizeProperty				= Property.Register ("MaxSize", typeof (Drawing.Size), typeof (Visual), new VisualPropertyMetadata (Drawing.Size.Infinite, VisualPropertyFlags.AffectsParentLayout));
		
		public static readonly Property VisibilityProperty			= Property.Register ("Visibility", typeof (bool), typeof (Visual), new VisualPropertyMetadata (true, new SetValueOverrideCallback (Visual.SetVisibilityValue), VisualPropertyFlags.AffectsParentLayout | VisualPropertyFlags.AffectsDisplay));
		public static readonly Property EnableProperty				= Property.Register ("Enable", typeof (bool), typeof (Visual), new VisualPropertyMetadata (true, new SetValueOverrideCallback (Visual.SetEnableValue), VisualPropertyFlags.AffectsDisplay));
		
		public static readonly Property InheritParentFocusProperty	= Property.Register ("InheritParentFocus", typeof (bool), typeof (Visual), new VisualPropertyMetadata (false));
		
		public static readonly Property IsVisibleProperty			= Property.RegisterReadOnly ("IsVisible", typeof (bool), typeof (Visual), new VisualPropertyMetadata (new GetValueOverrideCallback (Visual.GetIsVisibleValue), new PropertyInvalidatedCallback (Visual.NotifyIsVisibleChanged), VisualPropertyFlags.InheritsValue));
		public static readonly Property IsEnabledProperty			= Property.RegisterReadOnly ("IsEnabled", typeof (bool), typeof (Visual), new VisualPropertyMetadata (new GetValueOverrideCallback (Visual.GetIsEnabledValue), VisualPropertyFlags.InheritsValue | VisualPropertyFlags.AffectsDisplay));
		public static readonly Property IsFocusedProperty			= Property.RegisterReadOnly ("IsFocused", typeof (bool), typeof (Visual), new VisualPropertyMetadata (new GetValueOverrideCallback (Visual.GetIsFocusedValue), new PropertyInvalidatedCallback (Visual.NotifyIsFocusedChanged), VisualPropertyFlags.InheritsValue | VisualPropertyFlags.AffectsDisplay));
		
		public static readonly Property IsKeyboardFocusedProperty	= Property.RegisterReadOnly ("IsKeyboardFocused", typeof (bool), typeof (Visual), new VisualPropertyMetadata (false, new GetValueOverrideCallback (Visual.GetIsKeyboardFocusedValue), new PropertyInvalidatedCallback (Visual.NotifyIsKeyboardFocusedChanged), VisualPropertyFlags.AffectsDisplay));
		public static readonly Property ContainsKeyboardFocusProperty = Property.RegisterReadOnly ("ContainsKeyboardFocus", typeof (bool), typeof (Visual), new VisualPropertyMetadata (false, new GetValueOverrideCallback (Visual.GetContainsKeyboardFocusValue), VisualPropertyFlags.None));
		
		public static readonly Property AutoCaptureProperty			= Property.Register ("AutoCapture", typeof (bool), typeof (Visual), new PropertyMetadata (true));
		public static readonly Property AutoFocusProperty			= Property.Register ("AutoFocus", typeof (bool), typeof (Visual), new PropertyMetadata (false));
		public static readonly Property AutoEngageProperty			= Property.Register ("AutoEngage", typeof (bool), typeof (Visual), new PropertyMetadata (false));
		public static readonly Property AutoRepeatProperty			= Property.Register ("AutoRepeat", typeof (bool), typeof (Visual), new PropertyMetadata (false));
		public static readonly Property AutoToggleProperty			= Property.Register ("AutoToggle", typeof (bool), typeof (Visual), new PropertyMetadata (false));
		public static readonly Property AutoRadioProperty			= Property.Register ("AutoRadio", typeof (bool), typeof (Visual), new PropertyMetadata (false));
		public static readonly Property AutoDoubleClickProperty		= Property.Register ("AutoDoubleClick", typeof (bool), typeof (Visual), new PropertyMetadata (false));
		
		public static readonly Property AcceptThreeStatePropery		= Property.Register ("AcceptThreeState", typeof (bool), typeof (Visual), new PropertyMetadata (false));
		
		public static readonly Property BackColorProperty			= Property.Register ("BackColor", typeof (Drawing.Color), typeof (Visual), new VisualPropertyMetadata (Drawing.Color.Empty, VisualPropertyFlags.AffectsDisplay));
		
		public static readonly Property CommandDispatcherProperty	= Property.RegisterReadOnly ("CommandDispatcher", typeof (CommandDispatcher), typeof (Visual), new PropertyMetadata (null, new PropertyInvalidatedCallback (Visual.NotifyCommandDispatcherChanged)));
		public static readonly Property CommandProperty				= Property.Register ("Command", typeof (string), typeof (Visual), new PropertyMetadata (null, new PropertyInvalidatedCallback (Visual.NotifyCommandChanged)));
		
//-		public static readonly Property ChildrenProperty = Property.Register ("Children", typeof (Collections.VisualCollection), typeof (Visual));
		
		
		private int								command_cache_id = -1;
		private short							suspend_layout_counter;
		
		protected bool							has_layout_changed;
		protected bool							have_children_changed;
		protected byte							currently_updating_layout;
		
		private Collections.LayerCollection		layer_collection;
		private Layouts.Layer					parent_layer;
	}
}
