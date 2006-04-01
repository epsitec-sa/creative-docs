//	Copyright � 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets.Helpers;
using System.Collections.Generic;

namespace Epsitec.Common.Widgets
{
	using PropertyChangedEventHandler = Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs>;
	
	/// <summary>
	/// Visual.
	/// </summary>
	public class Visual : Types.DependencyObject, ICommandDispatcherHost
	{
		public Visual()
		{
			//	Since IsFocused would be automatically inherited, we have to define
			//	it locally. Setting InheritParentFocus will clear the definition.
			
			this.SetValueBase (Visual.IsFocusedProperty, false);
		}
		
		
		internal short							VisualSerialId
		{
			get
			{
				return this.visual_serial_id;
			}
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
				return this.parent;
			}
			set
			{
				if (this.parent != value)
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
		
		public CommandDispatcher[]				CommandDispatchers
		{
			get
			{
				return (CommandDispatcher[]) this.GetValue (Visual.CommandDispatchersProperty);
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
				return new Drawing.Margins (this.left, this.right, this.top, this.bottom);
			}
			set
			{
				Drawing.Margins oldValue = this.AnchorMargins;
				Drawing.Margins newValue = value;

				if (oldValue != newValue)
				{
					this.left   = value.Left;
					this.right  = value.Right;
					this.bottom = value.Bottom;
					this.top    = value.Top;
					
					this.InvalidateProperty (Visual.AnchorMarginsProperty, oldValue, newValue);
				}
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
				return new Drawing.Size (this.width, this.height);
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
				return new Drawing.Point (this.left, this.bottom);
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
				Drawing.Size  size     = this.Size;
				Drawing.Point location = this.Location;
				
				return new Drawing.Rectangle (location, size);
			}
			set
			{
				double left   = value.Left;
				double bottom = value.Bottom;
				double width  = value.Width;
				double height = value.Height;
				
				if (this.PreferredSize != value.Size)
				{
					this.PreferredSize = value.Size;
				}
				
				if ((this.left != left) ||
					(this.bottom != bottom) ||
					(this.width != width) ||
					(this.height != height))
				{
					if (this.parent == null)
					{
						this.SuspendLayout ();
						this.SetBounds (left, bottom, width, height);
						this.ResumeLayout ();
					}
					else
					{
						Drawing.Size host = parent.Client.Size;

						double right = host.Width - left - width;
						double top = host.Height - bottom - height;
						
						this.AnchorMargins = new Drawing.Margins (left, right, top, bottom);

						if ((this.Anchor == AnchorStyles.None) &&
							(this.Dock == DockStyle.None))
						{
							this.SuspendLayout ();
							this.SetBounds (left, bottom, width, height);
							this.NotifyGeometryChanged ();
							this.ResumeLayout ();
						}
						else
						{
							this.parent.SuspendLayout ();
							this.NotifyGeometryChanged ();
							this.parent.ResumeLayout ();
						}
					}
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
		
		
		public bool								KeyboardFocus
		{
			get
			{
				return (bool) this.GetValueBase (Visual.KeyboardFocusProperty);
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
		
		
		public virtual bool						Visibility
		{
			get
			{
				return (bool) this.GetValue (Visual.VisibilityProperty);
			}
			set
			{
				if (this.Visibility != value)
				{
					this.SetValueBase (Visual.VisibilityProperty, value);

					if (value)
					{
						this.ClearValueBase (Visual.IsVisibleProperty);
					}
					else
					{
						this.SetValueBase (Visual.IsVisibleProperty, false);
					}
				}
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
				if (this.Enable != value)
				{
					this.SetValueBase (Visual.EnableProperty, value);

					if (value)
					{
						this.ClearValueBase (Visual.IsEnabledProperty);
					}
					else
					{
						this.SetValueBase (Visual.IsEnabledProperty, false);
					}
				}
			}
		}
		
		
		public bool								IsVisible
		{
			get
			{
				return (bool) this.GetValue (Visual.IsVisibleProperty);
			}
		}
		
		public bool								IsEnabled
		{
			get
			{
				return (bool) this.GetValue (Visual.IsEnabledProperty);
			}
		}
		
		public bool								IsFocused
		{
			get
			{
				return (bool) this.GetValue (Visual.IsFocusedProperty);
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
		
		public bool								HasChildren
		{
			get
			{
				return (this.children != null) && (this.children.Count > 0);
			}
		}


		public Collections.FlatChildrenCollection	Children
		{
			get
			{
				if (this.children == null)
				{
					this.children = new Collections.FlatChildrenCollection (this);
				}
				
				return this.children;
			}
		}
		
		
		
		internal int GetCommandCacheId()
		{
			return this.command_cache_id;
		}
		
		internal void SetCommandCacheId(int value)
		{
			this.command_cache_id = value;
		}



		private void SetBounds(double left, double bottom, double width, double height)
		{
			this.SetBounds (new Drawing.Rectangle (left, bottom, width, height));
		}
		
		internal virtual void SetBounds(Drawing.Rectangle value)
		{
			Drawing.Rectangle old_value = this.Bounds;

			this.left   = value.Left;
			this.bottom = value.Bottom;
			this.width  = value.Width;
			this.height = value.Height;
			
			Drawing.Rectangle new_value = value;
			
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

		internal void NotifyGeometryChanged()
		{
			this.NotifyLayoutChanged ();
			this.NotifyParentLayoutChanged ();
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
			
			CommandDispatcher[] dispatchers = this.CommandDispatchers;
			
			if ((dispatchers == null) ||
				(dispatchers.Length == 0))
			{
				this.SetValueBase (Visual.CommandDispatchersProperty, new CommandDispatcher[] { value });
			}
			else if ((dispatchers.Length == 1) &&
				/**/ (dispatchers[0] == value))
			{
				throw new System.InvalidOperationException ("Cannot attach same CommandDispatcher twice");
			}
			else
			{
				//	TODO: terminer AttachCommandDispatcher
				
				throw new System.NotImplementedException ("AttachCommandDispatcher not fully implemented");
			}
		}
		
		public void DetachCommandDispatcher(CommandDispatcher value)
		{
			System.Diagnostics.Debug.Assert (value != null);
			
			CommandDispatcher[] dispatchers = this.CommandDispatchers;
			
			if ((dispatchers == null) ||
				(dispatchers.Length == 0))
			{
				throw new System.InvalidOperationException ("Cannot detach unknown CommandDispatcher");
			}
			else if ((dispatchers.Length == 1) &&
				/**/ (dispatchers[0] == value))
			{
				this.SetValueBase (Visual.CommandDispatchersProperty, null);
			}
			else
			{
				//	TODO: terminer DetachCommandDispatcher
				
				throw new System.NotImplementedException ("DetachCommandDispatcher not fully implemented");
			}
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


		private static object GetParentValue(DependencyObject o)
		{
			Visual that = o as Visual;
			return that.Parent;
		}
		
		private static object GetChildrenValue(DependencyObject o)
		{
			Visual that = o as Visual;
			return that.Children;
		}

		private static object GetHasChildrenValue(DependencyObject o)
		{
			Visual that = o as Visual;
			return that.HasChildren;
		}

		private static object GetBoundsValue(DependencyObject o)
		{
			Visual that = o as Visual;
			return that.Bounds;
		}
		
		private static object GetSizeValue(DependencyObject o)
		{
			Visual that = o as Visual;
			return that.Size;
		}
		
		private static void SetSizeValue(DependencyObject o, object value)
		{
			Visual that = o as Visual;
			that.Size = (Drawing.Size) value;
		}
		
		private static object GetKeyboardFocusValue(DependencyObject o)
		{
			Visual that = o as Visual;
			return that.KeyboardFocus;
		}

		private static void SetKeyboardFocusValue(DependencyObject o, object value)
		{
			Visual that = o as Visual;
			bool focus = (bool) value;
			
			if (that.KeyboardFocus != focus)
			{
				that.SetValueBase (Visual.KeyboardFocusProperty, value);

				if (focus)
				{
					that.SetValueBase (Visual.IsFocusedProperty, true);
				}
				else
				{
					if (that.InheritParentFocus)
					{
						that.ClearValueBase (Visual.IsFocusedProperty);
					}
					else
					{
						that.SetValueBase (Visual.IsFocusedProperty, false);
					}
				}
			}
		}

		private static void SetInheritParentFocus(DependencyObject o, object value)
		{
			Visual that = o as Visual;
			bool enable = (bool) value;

			if (that.InheritParentFocus != enable)
			{
				that.SetValueBase (Visual.InheritParentFocusProperty, value);

				if (enable)
				{
					if (that.KeyboardFocus == false)
					{
						that.ClearValueBase (Visual.IsFocusedProperty);
					}
				}
				else
				{
					if (that.KeyboardFocus == false)
					{
						that.SetValueBase (Visual.IsFocusedProperty, false);
					}
				}
			}
		}
		
		private static object GetContainsKeyboardFocusValue(DependencyObject o)
		{
			Visual that = o as Visual;
			return that.ContainsKeyboardFocus;
		}
		
		private static object GetCommandDispatchersValue(DependencyObject o)
		{
			Visual that = o as Visual;
			CommandDispatcher[] value = (CommandDispatcher[]) that.GetValueBase (Visual.CommandDispatchersProperty);
			return value == null ? new CommandDispatcher[0] : value.Clone ();
		}
		
		
		private static void SetEnableValue(DependencyObject o, object value)
		{
			Visual that = o as Visual;
			that.Enable = (bool) value;
		}
		
		private static void SetVisibilityValue(DependencyObject o, object value)
		{
			Visual that = o as Visual;
			that.Visibility = (bool) value;
		}
		
		private static void NotifySizeChanged(DependencyObject o, object old_value, object new_value)
		{
			Visual that = o as Visual;
			that.OnSizeChanged (new DependencyPropertyChangedEventArgs (Visual.SizeProperty, old_value, new_value));
		}
		
		private static void NotifyMinSizeChanged(DependencyObject o, object old_value, object new_value)
		{
			Visual that = o as Visual;
			that.OnMinSizeChanged (new DependencyPropertyChangedEventArgs (Visual.MinSizeProperty, old_value, new_value));
		}
		
		private static void NotifyMaxSizeChanged(DependencyObject o, object old_value, object new_value)
		{
			Visual that = o as Visual;
			that.OnMaxSizeChanged (new DependencyPropertyChangedEventArgs (Visual.MaxSizeProperty, old_value, new_value));
		}
		
		private static void NotifyParentChanged(DependencyObject o, object old_value, object new_value)
		{
			Visual that = o as Visual;
			that.OnParentChanged (new DependencyPropertyChangedEventArgs (Visual.ParentProperty, old_value, new_value));
		}
		
		private static void NotifyKeyboardFocusChanged(DependencyObject o, object old_value, object new_value)
		{
			Visual that = o as Visual;
			that.OnKeyboardFocusChanged (new DependencyPropertyChangedEventArgs (Visual.KeyboardFocusProperty, old_value, new_value));
		}
		
		private static void NotifyCommandChanged(DependencyObject o, object old_value, object new_value)
		{
			Visual that = o as Visual;
			that.OnCommandChanged (new DependencyPropertyChangedEventArgs (Visual.CommandProperty, old_value, new_value));
		}
		
		private static void NotifyCommandDispatchersChanged(DependencyObject o, object old_value, object new_value)
		{
			Visual that = o as Visual;
			that.OnCommandDispatchersChanged (new DependencyPropertyChangedEventArgs (Visual.CommandDispatchersProperty, old_value, new_value));
		}
		
		
		protected virtual void OnSizeChanged(Types.DependencyPropertyChangedEventArgs e)
		{
		}
		
		protected virtual void OnMinSizeChanged(Types.DependencyPropertyChangedEventArgs e)
		{
		}
		
		protected virtual void OnMaxSizeChanged(Types.DependencyPropertyChangedEventArgs e)
		{
		}
		
		protected virtual void OnParentChanged(Types.DependencyPropertyChangedEventArgs e)
		{
			Helpers.VisualTree.InvalidateCommandDispatcher (this);
		}
		
		protected virtual void OnKeyboardFocusChanged(Types.DependencyPropertyChangedEventArgs e)
		{
		}
		
		protected virtual void OnCommandChanged(Types.DependencyPropertyChangedEventArgs e)
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
		
		protected virtual void OnCommandDispatchersChanged(Types.DependencyPropertyChangedEventArgs e)
		{
			Helpers.VisualTree.InvalidateCommandDispatcher (this);
		}
		
		
		protected virtual void UpdateClientGeometry()
		{
		}
		
		
		public event PropertyChangedEventHandler	MaxSizeChanged
		{
			add
			{
				this.AddEventHandler (Visual.MaxSizeProperty, value);
			}
			remove
			{
				this.RemoveEventHandler (Visual.MaxSizeProperty, value);
			}
		}
		
		public event PropertyChangedEventHandler	MinSizeChanged
		{
			add
			{
				this.AddEventHandler (Visual.MinSizeProperty, value);
			}
			remove
			{
				this.RemoveEventHandler (Visual.MinSizeProperty, value);
			}
		}
		
		public event PropertyChangedEventHandler	SizeChanged
		{
			add
			{
				this.AddEventHandler (Visual.SizeProperty, value);
			}
			remove
			{
				this.RemoveEventHandler (Visual.SizeProperty, value);
			}
		}

		public event PropertyChangedEventHandler	ParentChanged
		{
			add
			{
				this.AddEventHandler (Visual.ParentProperty, value);
			}
			remove
			{
				this.RemoveEventHandler (Visual.ParentProperty, value);
			}
		}

		public event PropertyChangedEventHandler	ChildrenChanged
		{
			add
			{
				this.AddEventHandler (Visual.ChildrenProperty, value);
			}
			remove
			{
				this.RemoveEventHandler (Visual.ChildrenProperty, value);
			}
		}

		public event PropertyChangedEventHandler	IsVisibleChanged
		{
			add
			{
				this.AddEventHandler (Visual.IsVisibleProperty, value);
			}
			remove
			{
				this.RemoveEventHandler (Visual.IsVisibleProperty, value);
			}
		}
		
		public event PropertyChangedEventHandler	IsEnabledChanged
		{
			add
			{
				this.AddEventHandler (Visual.IsEnabledProperty, value);
			}
			remove
			{
				this.RemoveEventHandler (Visual.IsEnabledProperty, value);
			}
		}
		
		public event PropertyChangedEventHandler	IsFocusedChanged
		{
			add
			{
				this.AddEventHandler (Visual.IsFocusedProperty, value);
			}
			remove
			{
				this.RemoveEventHandler (Visual.IsFocusedProperty, value);
			}
		}
		
		public event PropertyChangedEventHandler	KeyboardFocusChanged
		{
			add
			{
				this.AddEventHandler (Visual.KeyboardFocusProperty, value);
			}
			remove
			{
				this.RemoveEventHandler (Visual.KeyboardFocusProperty, value);
			}
		}
		
		public event PropertyChangedEventHandler	ContainsKeyboardFocusChanged
		{
			add
			{
				this.AddEventHandler (Visual.ContainsKeyboardFocusProperty, value);
			}
			remove
			{
				this.RemoveEventHandler (Visual.ContainsKeyboardFocusProperty, value);
			}
		}
		
		
		public static readonly DependencyProperty IndexProperty					= DependencyProperty.Register ("Index", typeof (int), typeof (Visual), new DependencyPropertyMetadata (-1));
		public static readonly DependencyProperty GroupProperty					= DependencyProperty.Register ("Group", typeof (string), typeof (Visual));
		public static readonly DependencyProperty NameProperty					= DependencyObjectTree.NameProperty.AddOwner (typeof (Visual));
		public static readonly DependencyProperty ParentProperty				= DependencyObjectTree.ParentProperty.AddOwner (typeof (Visual), new DependencyPropertyMetadata (new GetValueOverrideCallback (Visual.GetParentValue), new PropertyInvalidatedCallback (Visual.NotifyParentChanged)));
		public static readonly DependencyProperty ChildrenProperty				= DependencyObjectTree.ChildrenProperty.AddOwner (typeof (Visual), new DependencyPropertyMetadata (new GetValueOverrideCallback (Visual.GetChildrenValue)));
		public static readonly DependencyProperty HasChildrenProperty			= DependencyObjectTree.HasChildrenProperty.AddOwner (typeof (Visual), new DependencyPropertyMetadata (new GetValueOverrideCallback (Visual.GetHasChildrenValue)));
		public static readonly DependencyProperty WindowProperty				= DependencyProperty.RegisterReadOnly ("Window", typeof (Window), typeof (Visual), new VisualPropertyMetadata (null, VisualPropertyMetadataOptions.InheritsValue | VisualPropertyMetadataOptions.ChangesSilently));
		
		public static readonly DependencyProperty AnchorProperty				= DependencyProperty.Register ("Anchor", typeof (AnchorStyles), typeof (Visual), new VisualPropertyMetadata (AnchorStyles.None, VisualPropertyMetadataOptions.AffectsArrange));
		public static readonly DependencyProperty AnchorMarginsProperty			= DependencyProperty.Register ("AnchorMargins", typeof (Drawing.Margins), typeof (Visual), new VisualPropertyMetadata (Drawing.Margins.Zero, VisualPropertyMetadataOptions.AffectsArrange));
		public static readonly DependencyProperty DockProperty					= DependencyProperty.Register ("Dock", typeof (DockStyle), typeof (Visual), new VisualPropertyMetadata (DockStyle.None, VisualPropertyMetadataOptions.AffectsArrange));
		public static readonly DependencyProperty DockPaddingProperty			= DependencyProperty.Register ("DockPadding", typeof (Drawing.Margins), typeof (Visual), new VisualPropertyMetadata (Drawing.Margins.Zero, VisualPropertyMetadataOptions.AffectsArrange));
		public static readonly DependencyProperty DockMarginsProperty			= DependencyProperty.Register ("DockMargins", typeof (Drawing.Margins), typeof (Visual), new VisualPropertyMetadata (Drawing.Margins.Zero, VisualPropertyMetadataOptions.AffectsArrange));
		public static readonly DependencyProperty ContainerLayoutModeProperty	= DependencyProperty.Register ("ContainerLayoutMode", typeof (ContainerLayoutMode), typeof (Visual), new VisualPropertyMetadata (ContainerLayoutMode.VerticalFlow, VisualPropertyMetadataOptions.AffectsChildrenLayout));
		
		public static readonly DependencyProperty BoundsProperty				= DependencyProperty.RegisterReadOnly ("Bounds", typeof (Drawing.Rectangle), typeof (Visual), new DependencyPropertyMetadata (Drawing.Rectangle.Empty, new GetValueOverrideCallback (Visual.GetBoundsValue)));
		public static readonly DependencyProperty SizeProperty					= DependencyProperty.Register ("Size", typeof (Drawing.Size), typeof (Visual), new DependencyPropertyMetadata (new GetValueOverrideCallback (Visual.GetSizeValue), new SetValueOverrideCallback (Visual.SetSizeValue), new PropertyInvalidatedCallback (Visual.NotifySizeChanged)));
		public static readonly DependencyProperty PreferredSizeProperty			= DependencyProperty.Register ("PreferredSize", typeof (Drawing.Size), typeof (Visual), new VisualPropertyMetadata (Drawing.Size.Empty, VisualPropertyMetadataOptions.AffectsArrange));
		public static readonly DependencyProperty MinSizeProperty				= DependencyProperty.Register ("MinSize", typeof (Drawing.Size), typeof (Visual), new VisualPropertyMetadata (Drawing.Size.Empty, new PropertyInvalidatedCallback (Visual.NotifyMinSizeChanged), VisualPropertyMetadataOptions.AffectsArrange));
		public static readonly DependencyProperty MaxSizeProperty				= DependencyProperty.Register ("MaxSize", typeof (Drawing.Size), typeof (Visual), new VisualPropertyMetadata (Drawing.Size.Infinite, new PropertyInvalidatedCallback (Visual.NotifyMaxSizeChanged), VisualPropertyMetadataOptions.AffectsArrange));
		
		public static readonly DependencyProperty VisibilityProperty			= DependencyProperty.Register ("Visibility", typeof (bool), typeof (Visual), new VisualPropertyMetadata (true, new SetValueOverrideCallback (Visual.SetVisibilityValue), VisualPropertyMetadataOptions.None));
		public static readonly DependencyProperty EnableProperty				= DependencyProperty.Register ("Enable", typeof (bool), typeof (Visual), new VisualPropertyMetadata (true, new SetValueOverrideCallback (Visual.SetEnableValue), VisualPropertyMetadataOptions.None));

		public static readonly DependencyProperty InheritParentFocusProperty	= DependencyProperty.Register ("InheritParentFocus", typeof (bool), typeof (Visual), new VisualPropertyMetadata (false, Visual.SetInheritParentFocus, VisualPropertyMetadataOptions.None));

		public static readonly DependencyProperty IsVisibleProperty				= DependencyProperty.RegisterReadOnly ("IsVisible", typeof (bool), typeof (Visual), new VisualPropertyMetadata (false, VisualPropertyMetadataOptions.InheritsValue | VisualPropertyMetadataOptions.AffectsArrange | VisualPropertyMetadataOptions.AffectsDisplay));
		public static readonly DependencyProperty IsEnabledProperty				= DependencyProperty.RegisterReadOnly ("IsEnabled", typeof (bool), typeof (Visual), new VisualPropertyMetadata (true, VisualPropertyMetadataOptions.InheritsValue | VisualPropertyMetadataOptions.AffectsDisplay));
		public static readonly DependencyProperty IsFocusedProperty				= DependencyProperty.RegisterReadOnly ("IsFocused", typeof (bool), typeof (Visual), new VisualPropertyMetadata (false, VisualPropertyMetadataOptions.InheritsValue | VisualPropertyMetadataOptions.AffectsDisplay));
		
		public static readonly DependencyProperty KeyboardFocusProperty			= DependencyProperty.RegisterReadOnly ("KeyboardFocus", typeof (bool), typeof (Visual), new VisualPropertyMetadata (false, new GetValueOverrideCallback (Visual.GetKeyboardFocusValue), new SetValueOverrideCallback (Visual.SetKeyboardFocusValue), VisualPropertyMetadataOptions.AffectsDisplay));
		public static readonly DependencyProperty ContainsKeyboardFocusProperty	= DependencyProperty.RegisterReadOnly ("ContainsKeyboardFocus", typeof (bool), typeof (Visual), new VisualPropertyMetadata (false, new GetValueOverrideCallback (Visual.GetContainsKeyboardFocusValue), VisualPropertyMetadataOptions.None));
		
		public static readonly DependencyProperty AutoCaptureProperty			= DependencyProperty.Register ("AutoCapture", typeof (bool), typeof (Visual), new DependencyPropertyMetadata (true));
		public static readonly DependencyProperty AutoFocusProperty				= DependencyProperty.Register ("AutoFocus", typeof (bool), typeof (Visual), new DependencyPropertyMetadata (false));
		public static readonly DependencyProperty AutoEngageProperty			= DependencyProperty.Register ("AutoEngage", typeof (bool), typeof (Visual), new DependencyPropertyMetadata (false));
		public static readonly DependencyProperty AutoRepeatProperty			= DependencyProperty.Register ("AutoRepeat", typeof (bool), typeof (Visual), new DependencyPropertyMetadata (false));
		public static readonly DependencyProperty AutoToggleProperty			= DependencyProperty.Register ("AutoToggle", typeof (bool), typeof (Visual), new DependencyPropertyMetadata (false));
		public static readonly DependencyProperty AutoRadioProperty				= DependencyProperty.Register ("AutoRadio", typeof (bool), typeof (Visual), new DependencyPropertyMetadata (false));
		public static readonly DependencyProperty AutoDoubleClickProperty		= DependencyProperty.Register ("AutoDoubleClick", typeof (bool), typeof (Visual), new DependencyPropertyMetadata (false));
		
		public static readonly DependencyProperty AcceptThreeStatePropery		= DependencyProperty.Register ("AcceptThreeState", typeof (bool), typeof (Visual), new DependencyPropertyMetadata (false));
		
		public static readonly DependencyProperty BackColorProperty				= DependencyProperty.Register ("BackColor", typeof (Drawing.Color), typeof (Visual), new VisualPropertyMetadata (Drawing.Color.Empty, VisualPropertyMetadataOptions.AffectsDisplay));
		
		public static readonly DependencyProperty CommandDispatchersProperty	= DependencyProperty.RegisterReadOnly ("CommandDispatchers", typeof (CommandDispatcher[]), typeof (Visual), new DependencyPropertyMetadata (null, new GetValueOverrideCallback (Visual.GetCommandDispatchersValue), new PropertyInvalidatedCallback (Visual.NotifyCommandDispatchersChanged)));
		public static readonly DependencyProperty CommandProperty				= DependencyProperty.Register ("Command", typeof (string), typeof (Visual), new DependencyPropertyMetadata (null, new PropertyInvalidatedCallback (Visual.NotifyCommandChanged)));
		
		private int								command_cache_id = -1;
		private short							visual_serial_id = Visual.next_serial_id++;
		private short							suspend_layout_counter;
		protected bool							has_layout_changed;
		protected bool							have_children_changed;
		protected byte							currently_updating_layout;


		private double							left, right, top, bottom;
		private double							width, height;
		
		private static short					next_serial_id;
		Collections.FlatChildrenCollection		children;
		private Visual							parent;

		internal void SetParentVisual(Visual visual)
		{
			this.parent = visual;
		}

		internal void NotifyChildrenChanged()
		{
			//	TODO: ...
			this.NotifyLayoutChanged ();
		}
	}
}
