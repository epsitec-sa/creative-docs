//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using System.Collections.Generic;

namespace Epsitec.Common.Widgets
{
	using PropertyChangedEventHandler=Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs>;
	using FlatChildrenCollection=Collections.FlatChildrenCollection;
	using VisualPropertyMetadata=Helpers.VisualPropertyMetadata;
	using VisualPropertyMetadataOptions=Helpers.VisualPropertyMetadataOptions;
	using ContentAlignment=Drawing.ContentAlignment;
	
	/// <summary>
	/// Visual.
	/// </summary>
	public class Visual : Types.DependencyObject, Helpers.IClientInfo, System.IEquatable<Visual>
	{
		public Visual()
		{
			this.visualSerialId = System.Threading.Interlocked.Increment (ref Visual.nextSerialId);
			
			//	Since IsFocused would be automatically inherited, we have to define
			//	it locally. Setting InheritsParentFocus will clear the definition.
			
			this.SetValueBase (Visual.IsFocusedProperty, false);

			this.width = this.PreferredWidth;
			this.height = this.PreferredHeight;
		}
		
		
		internal long							VisualSerialId
		{
			get
			{
				return this.visualSerialId;
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

		public virtual Window					Window
		{
			get
			{
				return Helpers.VisualTree.GetWindow (this);
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
		
		public Drawing.Margins					Margins
		{
			get
			{
				return (Drawing.Margins) this.GetValue (Visual.MarginsProperty);
			}
			set
			{
				this.SetValue (Visual.MarginsProperty, value);
			}
		}
		
		public Drawing.Margins					Padding
		{
			get
			{
				return (Drawing.Margins) this.GetValue (Visual.PaddingProperty);
			}
			set
			{
				this.SetValue (Visual.PaddingProperty, value);
			}
		}
		
		public HorizontalAlignment				HorizontalAlignment
		{
			get
			{
				return (HorizontalAlignment) this.GetValue (Visual.HorizontalAlignmentProperty);
			}
			set
			{
				this.SetValue (Visual.HorizontalAlignmentProperty, value);
			}
		}
		
		public VerticalAlignment				VerticalAlignment
		{
			get
			{
				return (VerticalAlignment) this.GetValue (Visual.VerticalAlignmentProperty);
			}
			set
			{
				this.SetValue (Visual.VerticalAlignmentProperty, value);
			}
		}

		public ContentAlignment					ContentAlignment
		{
			get
			{
				return (ContentAlignment) this.GetValue (Visual.ContentAlignmentProperty);
			}
			set
			{
				this.SetValue (Visual.ContentAlignmentProperty, value);
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


		public Drawing.Size						PreferredSize
		{
			get
			{
				return new Drawing.Size (this.PreferredWidth, this.PreferredHeight);
			}
			set
			{
				double width = value.Width;
				double height = value.Height;

				this.SetValue (Visual.PreferredWidthProperty, width);
				this.SetValue (Visual.PreferredHeightProperty, height);
			}
		}
		
		public double							PreferredWidth
		{
			get
			{
				return (double) this.GetValue (Visual.PreferredWidthProperty);
			}
			set
			{
				this.SetValue (Visual.PreferredWidthProperty, value);
			}
		}
		
		public double							PreferredHeight
		{
			get
			{
				return (double) this.GetValue (Visual.PreferredHeightProperty);
			}
			set
			{
				this.SetValue (Visual.PreferredHeightProperty, value);
			}
		}

		public bool								IsActualGeometryValid
		{
			get
			{
				return this.dirtyLayout == false;
			}
		}

		public Drawing.Rectangle				ActualBounds
		{
			get
			{
				if (!this.IsActualGeometryValid)
				{
//					System.Diagnostics.Debug.Assert (this.IsActualGeometryValid, "Layout dirty when calling ActualBounds");
				}
				return new Drawing.Rectangle (this.ActualLocation, this.ActualSize);
			}
		}

		public Drawing.Point					ActualLocation
		{
			get
			{
				if (!this.IsActualGeometryValid)
				{
//					System.Diagnostics.Debug.Assert (this.IsActualGeometryValid, "Layout dirty when calling ActualLocation");
				}
				return new Drawing.Point (this.x, this.y);
			}
		}

		public Drawing.Size						ActualSize
		{
			get
			{
				if (!this.IsActualGeometryValid)
				{
//					System.Diagnostics.Debug.Assert (this.IsActualGeometryValid, "Layout dirty when calling ActualSize");
				}
				return new Drawing.Size (this.width, this.height);
			}
		}

		public double							ActualWidth
		{
			get
			{
				if (!this.IsActualGeometryValid)
				{
//					System.Diagnostics.Debug.Assert (this.IsActualGeometryValid, "Layout dirty when calling ActualWidth");
				}
				return this.width;
			}
		}
		
		public double							ActualHeight
		{
			get
			{
				if (!this.IsActualGeometryValid)
				{
//					System.Diagnostics.Debug.Assert (this.IsActualGeometryValid, "Layout dirty when calling ActualHeight");
				}
				return this.height;
			}
		}
		
		public Helpers.IClientInfo				Client
		{
			get
			{
				return this;
			}
		}
		
		
		public bool								KeyboardFocus
		{
			get
			{
				//	Return true when the visual has the keyboard focus.
				
				return (bool) this.GetValue (Visual.KeyboardFocusProperty);
			}
		}
		
		public bool								ContainsKeyboardFocus
		{
			get
			{
				//	Return true when the visual, or one of its children,
				//	has the keyboard focus.
				
				return Helpers.VisualTree.ContainsKeyboardFocus (this);
			}
		}
		
		public bool								InheritsParentFocus
		{
			get
			{
				//	When true, IsFocus is inherited from the parent.

				return (bool) this.GetValue (Visual.InheritsParentFocusProperty);
			}
			set
			{
				this.SetValue (Visual.InheritsParentFocusProperty, value);
			}
		}
		
		
		public Drawing.Size						MinSize
		{
			get
			{
				return new Drawing.Size (this.MinWidth, this.MinHeight);
			}
			set
			{
				this.MinWidth  = value.Width;
				this.MinHeight = value.Height;
			}
		}

		public Drawing.Size						MaxSize
		{
			get
			{
				return new Drawing.Size (this.MaxWidth, this.MaxHeight);
			}
			set
			{
				this.MaxWidth  = value.Width;
				this.MaxHeight = value.Height;
			}
		}

		public double							MinWidth
		{
			get
			{
				return (double) this.GetValue (Visual.MinWidthProperty);
			}
			set
			{
				this.SetValue (Visual.MinWidthProperty, value);
			}
		}
		
		public double							MaxWidth
		{
			get
			{
				return (double) this.GetValue (Visual.MaxWidthProperty);
			}
			set
			{
				this.SetValue (Visual.MaxWidthProperty, value);
			}
		}
		
		public double							MinHeight
		{
			get
			{
				return (double) this.GetValue (Visual.MinHeightProperty);
			}
			set
			{
				this.SetValue (Visual.MinHeightProperty, value);
			}
		}
		
		public double							MaxHeight
		{
			get
			{
				return (double) this.GetValue (Visual.MaxHeightProperty);
			}
			set
			{
				this.SetValue (Visual.MaxHeightProperty, value);
			}
		}


		public ActiveState						ActiveState
		{
			get
			{
				return (ActiveState) this.GetValue (Visual.ActiveStateProperty);
			}
			set
			{
				this.SetValue (Visual.ActiveStateProperty, value);
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
				if (this.Visibility != value)
				{
					this.SetValueBase (Visual.VisibilityProperty, value);

					if (value)
					{
						this.ClearValueBase (Visual.IsVisibleProperty);
					}
					else
					{
						if (this.IsVisible)
						{
							this.Invalidate ();
						}
						
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
				//	Return true when KeyboardFocus is true or when the parent
				//	IsFocus is true and focus inheritance has been enabled
				//	through InheritsParentFocus.
				
				return (bool) this.GetValue (Visual.IsFocusedProperty);
			}
		}

		public bool								IsActive
		{
			get
			{
				return this.ActiveState == ActiveState.Yes;
			}
		}
		
		public bool								IsEntered
		{
			get
			{
				return (bool) this.GetValue (Visual.EnteredProperty);
			}
		}

		public bool								IsSelected
		{
			get
			{
				return (bool) this.GetValue (Visual.SelectedProperty);
			}
		}

		public bool								IsEngaged
		{
			get
			{
				return (bool) this.GetValue (Visual.EngagedProperty);
			}
		}

		public bool								IsValid
		{
			get
			{
				IValidator validator = this.Validator;
				
				if (validator != null)
				{
					return validator.IsValid;
				}
				else
				{
					return true;
				}
			}
		}

		public bool								InError
		{
			get
			{
				return (bool) this.GetValue (Visual.InErrorProperty);
			}
		}


		public IValidator						Validator
		{
			get
			{
				return (IValidator) this.GetValue (Visual.ValidatorProperty);
			}
		}

		public bool								HasValidator
		{
			get
			{
				return this.ContainsLocalValue (Visual.ValidatorProperty);
			}
		}

		public string							ValidationGroups
		{
			get
			{
				return (string) this.GetValue (Visual.ValidationGroupsProperty);
			}
			set
			{
				this.SetValue (Visual.ValidationGroupsProperty, value);
			}
		}

		public bool								HasValidationGroups
		{
			get
			{
				return this.ContainsLocalValue (Visual.ValidationGroupsProperty);
			}
		}

		public bool								SyncPaint
		{
			get
			{
				return (bool) this.GetValue (Visual.SyncPaintProperty);
			}
			set
			{
				this.SetValue (Visual.SyncPaintProperty, value);
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
		
		
		public bool								HasChildren
		{
			get
			{
				return (this.children != null) && (this.children.Count > 0);
			}
		}


		public FlatChildrenCollection			Children
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
		
		
		public IEnumerable<Visual> GetAllChildren()
		{
			if (this.HasChildren)
			{
				foreach (Visual child in this.children)
				{
					if (child.HasChildren)
					{
						foreach (Visual subChild in child.GetAllChildren ())
						{
							yield return subChild;
						}
					}
					yield return child;
				}
			}
		}

		public virtual Drawing.Margins GetInternalPadding()
		{
			return Drawing.Margins.Zero;
		}

		#region CommandCache Support Methods

		internal int GetCommandCacheId()
		{
			return this.commandCacheId;
		}
		
		internal void SetCommandCacheId(int value)
		{
			this.commandCacheId = value;
		}

		#endregion

		public virtual void SetManualBounds(Drawing.Rectangle value)
		{
			System.Diagnostics.Debug.Assert (this.Anchor == AnchorStyles.None);
			System.Diagnostics.Debug.Assert (this.Dock == DockStyle.None);
			
			this.SetBounds (value);
			this.Arrange (null);
		}
		
		internal virtual void SetBounds(Drawing.Rectangle value)
		{
			Drawing.Rectangle oldValue = this.GetCurrentBounds ();
			Drawing.Rectangle newValue = value;
			
			this.x      = value.Left;
			this.y      = value.Bottom;
			this.width  = value.Width;
			this.height = value.Height;

			this.ClearDirtyLayoutFlag ();

			if (oldValue != newValue)
			{
				this.SetBoundsOverride (oldValue, newValue);

				Visual parent = this.Parent;

				if (parent != null)
				{
					parent.Invalidate (Drawing.Rectangle.Inflate (oldValue, this.GetPaintMargins ()));
					parent.Invalidate (Drawing.Rectangle.Inflate (newValue, this.GetPaintMargins ()));

					this.dirtyDisplay = false;
				}

				if (oldValue.Size != newValue.Size)
				{
					//	TODO: on pourrait générer un événement ici
				}
			}
			
			if (this.dirtyDisplay)
			{
				this.Invalidate ();
			}
		}

		internal Drawing.Rectangle GetCurrentBounds()
		{
			return new Drawing.Rectangle (this.x, this.y, this.width, this.height);
		}

		protected virtual void SetBoundsOverride(Drawing.Rectangle oldRect, Drawing.Rectangle newRect)
		{
			//	Override if you want to do something when the bounds of the
			//	widget change.
		}



		public void Measure(Layouts.LayoutContext context)
		{
			this.MeasureOverride (context);
			
			Drawing.Size min = Drawing.Size.Zero;
			Drawing.Size max = Drawing.Size.PositiveInfinity;

			this.MeasureMinMax (ref min, ref max);

			Drawing.Size desired = this.GetDesiredSize ();
			
			context.DefineMinWidth (this, min.Width);
			context.DefineMinHeight (this, min.Height);
			context.DefineMaxWidth (this, max.Width);
			context.DefineMaxHeight (this, max.Height);
			
			context.DefineDesiredWidth (this, desired.Width);
			context.DefineDesiredHeight (this, desired.Height);
		}
		
		public void Arrange(Layouts.LayoutContext context)
		{
			this.ClearDirtyLayoutFlag ();

			this.ArrangeOverride (context);

			if (this.HasChildren)
			{
				IEnumerable<Visual> children = this.Children;

				Drawing.Rectangle rect = this.Client.Bounds;
				
				rect.Deflate (this.Padding);
				rect.Deflate (this.GetInternalPadding ());

				if (this.children.AnchorLayoutCount > 0)
				{
					Layouts.LayoutEngine.AnchorEngine.UpdateLayout (this, rect, children);
				}
				
				if (this.children.DockLayoutCount > 0)
				{
					Layouts.LayoutEngine.DockEngine.UpdateLayout (this, rect, children);
				}

				this.ManualArrange ();
			}

			if (this.dirtyDisplay)
			{
				this.Invalidate ();
			}

			if (context != null)
			{
				context.RemoveVisualFromArrangeQueue (this);
			}
		}

		protected virtual void ManualArrange()
		{
			//	Override if you need to layout children manually during the arrange
			//	phase.
		}

		protected virtual void MeasureMinMax(ref Drawing.Size min, ref Drawing.Size max)
		{
			min.Width = System.Math.Max (min.Width, this.MinWidth);
			min.Height = System.Math.Max (min.Height, this.MinHeight);
			max.Width = System.Math.Min (max.Width, this.MaxWidth);
			max.Height = System.Math.Min (max.Height, this.MaxHeight);

			if (this.HasChildren)
			{
				IEnumerable<Visual> children = this.children;

				if (this.children.DockLayoutCount > 0)
				{
					Layouts.LayoutEngine.DockEngine.UpdateMinMax (this, children, ref min, ref max);
				}
				if (this.children.AnchorLayoutCount > 0)
				{
					Layouts.LayoutEngine.AnchorEngine.UpdateMinMax (this, children, ref min, ref max);
				}
			}
		}

		protected virtual void MeasureOverride(Layouts.LayoutContext context)
		{
		}

		protected virtual void ArrangeOverride(Layouts.LayoutContext context)
		{
		}

		protected virtual Drawing.Size GetDesiredSize()
		{
			return this.PreferredSize;
		}
		
		
		public virtual Drawing.Margins GetShapeMargins()
		{
			return Drawing.Margins.Zero;
		}

		public virtual Drawing.Margins GetPaintMargins()
		{
			return this.GetShapeMargins ();
		}
		
		public Drawing.Rectangle GetShapeBounds()
		{
			return Drawing.Rectangle.Inflate (this.Client.Bounds, this.GetShapeMargins ());
		}

		public Drawing.Rectangle GetPaintBounds()
		{
			return Drawing.Rectangle.Inflate (this.Client.Bounds, this.GetPaintMargins ());
		}

		public void Invalidate()
		{
			if (this.IsVisible)
			{
				if (this.dirtyLayout)
				{
					this.dirtyDisplay = true;
				}
				else
				{
					this.InvalidateRectangle (this.GetPaintBounds (), this.SyncPaint);
					this.dirtyDisplay = false;
				}
			}
			else
			{
				Window window = this.Window;
				
				if ((window != null) &&
					(window.IsVisible == false))
				{
					if (this.dirtyLayout)
					{
						this.dirtyDisplay = true;
					}
					else
					{
						this.InvalidateRectangle (this.GetPaintBounds (), this.SyncPaint);
						this.dirtyDisplay = false;
					}
				}
			}
		}

		public void Invalidate(Drawing.Rectangle rect)
		{
			if (this.IsVisible)
			{
				if (this.dirtyLayout)
				{
					this.dirtyDisplay = true;
				}
				else if (this.dirtyDisplay == false)
				{
					this.InvalidateRectangle (rect, this.SyncPaint);
				}
			}
			else
			{
				Window window = this.Window;

				if ((window != null) &&
					(window.IsVisible == false))
				{
					if (this.dirtyLayout)
					{
						this.dirtyDisplay = true;
					}
					else if (this.dirtyDisplay == false)
					{
						this.InvalidateRectangle (rect, this.SyncPaint);
					}
				}
			}
		}

		public virtual void InvalidateRectangle(Drawing.Rectangle rect, bool sync)
		{
		}

		internal virtual void InvalidateTextLayout()
		{
		}

		internal virtual void SetDirtyLayoutFlag()
		{
			if (this.dirtyLayout == false)
			{
				this.dirtyLayout = true;
				
				Window window = this.Window;
				
				if (window != null)
				{
					window.AsyncLayout ();
				}
			}
		}
		
		internal virtual void ClearDirtyLayoutFlag()
		{
			this.dirtyLayout = false;
		}

		internal void SetParentVisual(Visual visual)
		{
			if (visual == null)
			{
				Layouts.LayoutContext.RemoveFromQueues (this);
				
				this.Invalidate ();
				this.parent = visual;
			}
			else
			{
				Layouts.LayoutContext context = null;

				if (this.parent == null)
				{
					context = Layouts.LayoutContext.GetLayoutContext (this);
				}

				this.parent = visual;

				if (context == null)
				{
					Layouts.LayoutContext.AddToMeasureQueue (this);
				}
				else
				{
					Layouts.LayoutContext.ClearLayoutContext (this);
					Layouts.LayoutContext.AddToMeasureQueue (this, context);
				}

			}
		}

		#region Internal Methods for FlatChildrenCollection

		internal void NotifyChildrenChanged()
		{
			Layouts.LayoutContext.AddToMeasureQueue (this);

			this.Invalidate ();
			this.OnChildrenChanged ();
		}

		#endregion

		protected virtual void OnChildrenChanged()
		{
			//	TODO: si besoin, rajouter un jour un événement ChildrenChanged
		}

		#region Helpers.IClientInfo Members

		Drawing.Rectangle Helpers.IClientInfo.Bounds
		{
			get
			{
				if (this.IsActualGeometryValid == false)
				{
//					System.Diagnostics.Debug.WriteLine ("Layout dirty when calling Client.Bounds");
				}
				
				return new Drawing.Rectangle (0, 0, this.width, this.height);
			}
		}

		Drawing.Size Helpers.IClientInfo.Size
		{
			get
			{
				if (this.IsActualGeometryValid == false)
				{
//					System.Diagnostics.Debug.WriteLine ("Layout dirty when calling Client.Size");
				}

				return new Drawing.Size (this.width, this.height);
			}
		}

		#endregion

		#region IEquatable<Visual> Members

		public bool Equals(Visual other)
		{
			return object.ReferenceEquals (this, other);
		}

		#endregion

		public override bool Equals(object obj)
		{
			return this.Equals (obj as Visual);
		}

		public override int GetHashCode()
		{
			return (int) this.visualSerialId;
		}
		
		
		static Visual()
		{
		}

		private static object GetIsValidValue(DependencyObject o)
		{
			Visual that = o as Visual;
			return that.IsValid;
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

		private static object GetActualWidthValue(DependencyObject o)
		{
			Visual that = o as Visual;
			return that.ActualWidth;
		}
		
		private static object GetActualHeightValue(DependencyObject o)
		{
			Visual that = o as Visual;
			return that.ActualHeight;
		}
		
		private static void SetKeyboardFocusValue(DependencyObject o, object value)
		{
			Visual that = o as Visual;
			bool focus = (bool) value;
			
			if (that.KeyboardFocus != focus)
			{
				//	When setting KeyboardFocus, we have to update IsFocused according
				//	to how the focus inheritance has been defined :
				
				that.SetValueBase (Visual.KeyboardFocusProperty, value);

				if (focus)
				{
					//	When we have the focus, IsFocused will be true.
					
					that.SetValueBase (Visual.IsFocusedProperty, true);
				}
				else
				{
					//	When we don't have the focus, we will either clear the IsFocused
					//	property (and thus inherit its value from the parent) or set it
					//	to false (no inheritance).
					
					if (that.InheritsParentFocus)
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

		private static void SetInheritsParentFocus(DependencyObject o, object value)
		{
			Visual that = o as Visual;
			bool enable = (bool) value;

			//	See SetKeyboardFocusValue for details about how InheritsParentFocus
			//	and IsFocus interact.
			
			if (that.InheritsParentFocus != enable)
			{
				that.SetValueBase (Visual.InheritsParentFocusProperty, value);

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

		private static void NotifyParentChanged(DependencyObject o, object oldValue, object newValue)
		{
			Visual that = o as Visual;
			that.OnParentChanged (new DependencyPropertyChangedEventArgs (Visual.ParentProperty, oldValue, newValue));
		}

		private static void NotifyKeyboardFocusChanged(DependencyObject o, object oldValue, object newValue)
		{
			Visual that = o as Visual;
			that.OnKeyboardFocusChanged (new DependencyPropertyChangedEventArgs (Visual.KeyboardFocusProperty, oldValue, newValue));
		}

		private static void NotifyCommandChanged(DependencyObject o, object oldValue, object newValue)
		{
			Visual that = o as Visual;
			that.OnCommandChanged (new DependencyPropertyChangedEventArgs (Visual.CommandProperty, oldValue, newValue));
		}

		private static void NotifyAnchorChanged(DependencyObject o, object oldValue, object newValue)
		{
			Visual that = o as Visual;
			
			if (that.parent != null)
			{
				DockStyle dock = that.Dock;
				AnchorStyles anchorOld = (AnchorStyles) oldValue;
				AnchorStyles anchorNew = (AnchorStyles) newValue;
				
				that.parent.children.UpdateLayoutStatistics (dock, dock, anchorOld, anchorNew);
			}
		}

		private static void NotifyDockChanged(DependencyObject o, object oldValue, object newValue)
		{
			Visual that = o as Visual;

			if (that.parent != null)
			{
				AnchorStyles anchor = that.Anchor;
				DockStyle dockOld = (DockStyle) oldValue;
				DockStyle dockNew = (DockStyle) newValue;

				that.parent.children.UpdateLayoutStatistics (dockOld, dockNew, anchor, anchor);
			}
		}

		private static void NotifyActiveStateChanged(DependencyObject o, object oldValue, object newValue)
		{
			Visual that = o as Visual;
			that.OnActiveStateChanged ();
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
				CommandCache.Default.InvalidateVisual (this);
			}
		}
		
		protected virtual void OnActiveStateChanged()
		{
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
		
		public static readonly DependencyProperty IndexProperty					= DependencyProperty.Register ("Index", typeof (int), typeof (Visual), new DependencyPropertyMetadata (-1));
		public static readonly DependencyProperty GroupProperty					= DependencyProperty.Register ("Group", typeof (string), typeof (Visual));
		public static readonly DependencyProperty NameProperty					= DependencyObjectTree.NameProperty.AddOwner (typeof (Visual));
		public static readonly DependencyProperty ParentProperty				= DependencyObjectTree.ParentProperty.AddOwner (typeof (Visual), new DependencyPropertyMetadata (new GetValueOverrideCallback (Visual.GetParentValue), new PropertyInvalidatedCallback (Visual.NotifyParentChanged)));
		public static readonly DependencyProperty ChildrenProperty				= DependencyObjectTree.ChildrenProperty.AddOwner (typeof (Visual), new DependencyPropertyMetadata (new GetValueOverrideCallback (Visual.GetChildrenValue)));
		public static readonly DependencyProperty HasChildrenProperty			= DependencyObjectTree.HasChildrenProperty.AddOwner (typeof (Visual), new DependencyPropertyMetadata (new GetValueOverrideCallback (Visual.GetHasChildrenValue)));

		public static readonly DependencyProperty AnchorProperty				= DependencyProperty.Register ("Anchor", typeof (AnchorStyles), typeof (Visual), new VisualPropertyMetadata (AnchorStyles.None, Visual.NotifyAnchorChanged, VisualPropertyMetadataOptions.AffectsArrange));
		public static readonly DependencyProperty DockProperty					= DependencyProperty.Register ("Dock", typeof (DockStyle), typeof (Visual), new VisualPropertyMetadata (DockStyle.None, Visual.NotifyDockChanged, VisualPropertyMetadataOptions.AffectsArrange));
		public static readonly DependencyProperty MarginsProperty				= DependencyProperty.Register ("Margins", typeof (Drawing.Margins), typeof (Visual), new VisualPropertyMetadata (Drawing.Margins.Zero, VisualPropertyMetadataOptions.AffectsArrange));
		public static readonly DependencyProperty PaddingProperty				= DependencyProperty.Register ("Padding", typeof (Drawing.Margins), typeof (Visual), new VisualPropertyMetadata (Drawing.Margins.Zero, VisualPropertyMetadataOptions.AffectsChildrenLayout));
		public static readonly DependencyProperty HorizontalAlignmentProperty	= DependencyProperty.Register ("HorizontalAlignment", typeof (HorizontalAlignment), typeof (Visual), new VisualPropertyMetadata (HorizontalAlignment.Stretch, VisualPropertyMetadataOptions.AffectsArrange));
		public static readonly DependencyProperty VerticalAlignmentProperty		= DependencyProperty.Register ("VerticalAlignment", typeof (VerticalAlignment), typeof (Visual), new VisualPropertyMetadata (VerticalAlignment.Stretch, VisualPropertyMetadataOptions.AffectsArrange));
		public static readonly DependencyProperty ContentAlignmentProperty		= DependencyProperty.Register ("ContentAlignment", typeof (ContentAlignment), typeof (Visual), new VisualPropertyMetadata (ContentAlignment.MiddleLeft, VisualPropertyMetadataOptions.AffectsTextLayout));
		public static readonly DependencyProperty ContainerLayoutModeProperty	= DependencyProperty.Register ("ContainerLayoutMode", typeof (ContainerLayoutMode), typeof (Visual), new VisualPropertyMetadata (ContainerLayoutMode.VerticalFlow, VisualPropertyMetadataOptions.AffectsChildrenLayout));

		public static readonly DependencyProperty PreferredWidthProperty		= DependencyProperty.Register ("PreferredWidth", typeof (double), typeof (Visual), new VisualPropertyMetadata (80.0, VisualPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty PreferredHeightProperty		= DependencyProperty.Register ("PreferredHeight", typeof (double), typeof (Visual), new VisualPropertyMetadata (20.0, VisualPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty ActualWidthProperty			= DependencyProperty.RegisterReadOnly ("ActualWidth", typeof (double), typeof (Visual), new VisualPropertyMetadata (Visual.GetActualWidthValue, VisualPropertyMetadataOptions.None));
		public static readonly DependencyProperty ActualHeightProperty			= DependencyProperty.RegisterReadOnly ("ActualHeight", typeof (double), typeof (Visual), new VisualPropertyMetadata (Visual.GetActualHeightValue, VisualPropertyMetadataOptions.None));
		public static readonly DependencyProperty MinWidthProperty				= DependencyProperty.Register ("MinWidth", typeof (double), typeof (Visual), new VisualPropertyMetadata (0.0, VisualPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty MinHeightProperty				= DependencyProperty.Register ("MinHeight", typeof (double), typeof (Visual), new VisualPropertyMetadata (0.0, VisualPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty MaxWidthProperty				= DependencyProperty.Register ("MaxWidth", typeof (double), typeof (Visual), new VisualPropertyMetadata (double.PositiveInfinity, VisualPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty MaxHeightProperty				= DependencyProperty.Register ("MaxHeight", typeof (double), typeof (Visual), new VisualPropertyMetadata (double.PositiveInfinity, VisualPropertyMetadataOptions.AffectsMeasure));

		public static readonly DependencyProperty ActiveStateProperty			= DependencyProperty.Register ("ActiveState", typeof (ActiveState), typeof (Visual), new VisualPropertyMetadata (ActiveState.No, Visual.NotifyActiveStateChanged, VisualPropertyMetadataOptions.AffectsDisplay));
		
		public static readonly DependencyProperty VisibilityProperty			= DependencyProperty.Register ("Visibility", typeof (bool), typeof (Visual), new VisualPropertyMetadata (true, new SetValueOverrideCallback (Visual.SetVisibilityValue), VisualPropertyMetadataOptions.None));
		public static readonly DependencyProperty EnableProperty				= DependencyProperty.Register ("Enable", typeof (bool), typeof (Visual), new VisualPropertyMetadata (true, new SetValueOverrideCallback (Visual.SetEnableValue), VisualPropertyMetadataOptions.None).MakeNotSerializable ());
		public static readonly DependencyProperty EnteredProperty				= DependencyProperty.Register ("Entered", typeof (bool), typeof (Visual), new VisualPropertyMetadata (false, VisualPropertyMetadataOptions.AffectsDisplay).MakeNotSerializable ());
		public static readonly DependencyProperty SelectedProperty				= DependencyProperty.Register ("Selected", typeof (bool), typeof (Visual), new VisualPropertyMetadata (false, VisualPropertyMetadataOptions.AffectsDisplay).MakeNotSerializable ());
		public static readonly DependencyProperty EngagedProperty				= DependencyProperty.Register ("Engaged", typeof (bool), typeof (Visual), new VisualPropertyMetadata (false, VisualPropertyMetadataOptions.AffectsDisplay).MakeNotSerializable ());
		public static readonly DependencyProperty InErrorProperty				= DependencyProperty.Register ("InError", typeof (bool), typeof (Visual), new VisualPropertyMetadata (false, VisualPropertyMetadataOptions.AffectsDisplay).MakeNotSerializable ());

		public static readonly DependencyProperty InheritsParentFocusProperty	= DependencyProperty.Register ("InheritsParentFocus", typeof (bool), typeof (Visual), new VisualPropertyMetadata (false, Visual.SetInheritsParentFocus, VisualPropertyMetadataOptions.None));

		public static readonly DependencyProperty IsVisibleProperty				= DependencyProperty.RegisterReadOnly ("IsVisible", typeof (bool), typeof (Visual), new VisualPropertyMetadata (false, VisualPropertyMetadataOptions.InheritsValue | VisualPropertyMetadataOptions.AffectsArrange | VisualPropertyMetadataOptions.AffectsDisplay));
		public static readonly DependencyProperty IsEnabledProperty				= DependencyProperty.RegisterReadOnly ("IsEnabled", typeof (bool), typeof (Visual), new VisualPropertyMetadata (true, VisualPropertyMetadataOptions.InheritsValue | VisualPropertyMetadataOptions.AffectsDisplay));
		public static readonly DependencyProperty IsFocusedProperty				= DependencyProperty.RegisterReadOnly ("IsFocused", typeof (bool), typeof (Visual), new VisualPropertyMetadata (false, VisualPropertyMetadataOptions.InheritsValue | VisualPropertyMetadataOptions.AffectsDisplay));
		
		public static readonly DependencyProperty KeyboardFocusProperty			= DependencyProperty.RegisterReadOnly ("KeyboardFocus", typeof (bool), typeof (Visual), new VisualPropertyMetadata (false, new SetValueOverrideCallback (Visual.SetKeyboardFocusValue), VisualPropertyMetadataOptions.AffectsDisplay));
		public static readonly DependencyProperty ContainsKeyboardFocusProperty	= DependencyProperty.RegisterReadOnly ("ContainsKeyboardFocus", typeof (bool), typeof (Visual), new VisualPropertyMetadata (false, new GetValueOverrideCallback (Visual.GetContainsKeyboardFocusValue), VisualPropertyMetadataOptions.ChangesSilently));

		public static readonly DependencyProperty IsValidProperty				= DependencyProperty.RegisterReadOnly ("IsValid", typeof (bool), typeof (Visual), new DependencyPropertyMetadata (Visual.GetIsValidValue));
		public static readonly DependencyProperty ValidatorProperty				= DependencyProperty.RegisterReadOnly ("Validator", typeof (IValidator), typeof (Visual), new DependencyPropertyMetadata (null));
		public static readonly DependencyProperty ValidationGroupsProperty		= DependencyProperty.Register ("ValidationGroups", typeof (string), typeof (Visual), new DependencyPropertyMetadata (null));

		public static readonly DependencyProperty SyncPaintProperty				= DependencyProperty.Register ("SyncPaint", typeof (bool), typeof (Visual), new DependencyPropertyMetadata (false));
		public static readonly DependencyProperty AutoCaptureProperty			= DependencyProperty.Register ("AutoCapture", typeof (bool), typeof (Visual), new DependencyPropertyMetadata (true));
		public static readonly DependencyProperty AutoFocusProperty				= DependencyProperty.Register ("AutoFocus", typeof (bool), typeof (Visual), new DependencyPropertyMetadata (false));
		public static readonly DependencyProperty AutoEngageProperty			= DependencyProperty.Register ("AutoEngage", typeof (bool), typeof (Visual), new DependencyPropertyMetadata (false));
		public static readonly DependencyProperty AutoRepeatProperty			= DependencyProperty.Register ("AutoRepeat", typeof (bool), typeof (Visual), new DependencyPropertyMetadata (false));
		public static readonly DependencyProperty AutoToggleProperty			= DependencyProperty.Register ("AutoToggle", typeof (bool), typeof (Visual), new DependencyPropertyMetadata (false));
		public static readonly DependencyProperty AutoRadioProperty				= DependencyProperty.Register ("AutoRadio", typeof (bool), typeof (Visual), new DependencyPropertyMetadata (false));
		public static readonly DependencyProperty AutoDoubleClickProperty		= DependencyProperty.Register ("AutoDoubleClick", typeof (bool), typeof (Visual), new DependencyPropertyMetadata (false));
		
		public static readonly DependencyProperty AcceptThreeStatePropery		= DependencyProperty.Register ("AcceptThreeState", typeof (bool), typeof (Visual), new DependencyPropertyMetadata (false));
		
		public static readonly DependencyProperty BackColorProperty				= DependencyProperty.Register ("BackColor", typeof (Drawing.Color), typeof (Visual), new VisualPropertyMetadata (Drawing.Color.Empty, VisualPropertyMetadataOptions.AffectsDisplay));
		
		public static readonly DependencyProperty CommandProperty				= DependencyProperty.Register ("Command", typeof (string), typeof (Visual), new DependencyPropertyMetadata (null, new PropertyInvalidatedCallback (Visual.NotifyCommandChanged)));

		private static long						nextSerialId = 1;
		
		private int								commandCacheId = -1;
		private long							visualSerialId;
		private bool							dirtyLayout;
		private bool							dirtyDisplay;

		private double							x, y;
		private double							width, height;
		
		private FlatChildrenCollection			children;
		private Visual							parent;
	}
}
