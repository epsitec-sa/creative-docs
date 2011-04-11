//	Copyright © 2005-2009, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Helpers;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (Visual))]

namespace Epsitec.Common.Widgets
{
	using PropertyChangedEventHandler=EventHandler<DependencyPropertyChangedEventArgs>;
	using FlatChildrenCollection=Collections.FlatChildrenCollection;
	using ContentAlignment=Drawing.ContentAlignment;
	
	[System.Flags]
	[DesignerVisible]
	public enum FrameState : uint
	{
		[Hidden]	None	= 0,
					Left	= 0x00000001,
					Right	= 0x00000002,
					Top		= 0x00000004,
					Bottom	= 0x00000008,
		[Hidden]	All		= 0x0000000F,
	}

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
				Types.Serialization.BlackList.Clear (this, Visual.NameProperty);
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
		
		public Command							CommandObject
		{
			get
			{
				return (Command) this.GetValue (Visual.CommandObjectProperty);
			}
			set
			{
				this.SetValue (Visual.CommandObjectProperty, value);
			}
		}

		public Druid							CommandId
		{
			get
			{
				Command command = this.CommandObject;
				return command == null ? Druid.Empty : command.Caption.Id;
			}
			set
			{
				if (value.IsEmpty)
				{
					this.CommandObject = null;
				}
				else
				{
					ICaptionResolver manager = Visual.cachedCaptionResolver ?? Helpers.VisualTree.GetCaptionResolver (this);

					this.CommandObject = Command.Get (value, manager);
				}
			}
		}
		
		public string							CommandName
		{
			get
			{
				Command commandObject = this.CommandObject;

				if (commandObject == null)
				{
					return null;
				}
				else
				{
					return commandObject.Name;
				}
			}
		}

		public bool								HasCommand
		{
			get
			{
				return this.CommandObject != null;
			}
		}

		public CommandState						CommandState
		{
			get
			{
				if (this.HasCommand)
				{
					return CommandCache.Instance.GetCommandState (this);
				}
				else
				{
					return null;
				}
			}
		}

		public ICaptionResolver					CaptionResolver
		{
			get
			{
				return (ICaptionResolver) this.GetValue (Visual.CaptionResolverProperty);
			}
			set
			{
				if (value == null)
				{
					this.ClearValue (Visual.CaptionResolverProperty);
				}
				else
				{
					this.SetValue (Visual.CaptionResolverProperty, value);
				}
			}
		}

		public Caption							Caption
		{
			get
			{
				if (this.caption == null)
				{
					if (this.AttachCaption (this.CaptionId))
					{
						this.InvalidateDisplayCaption ();
					}
				}
				
				return this.caption;
			}
		}

		public Druid							CaptionId
		{
			get
			{
				return (Druid) this.GetValue (Visual.CaptionIdProperty);
			}
			set
			{
				this.SetValue (Visual.CaptionIdProperty, value);
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

		public virtual ContentAlignment			ContentAlignment
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

		public bool								IsActualGeometryDirty
		{
			get
			{
				return this.dirtyLayout;
			}
		}

		public Drawing.Rectangle				ActualBounds
		{
			//	Coordonnées du widget par rapport à son parent.
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
			//	Origine du widget par rapport à son parent.
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
		
		public virtual bool						ContainsKeyboardFocus
		{
			get
			{
				//	Return true when the visual, or one of its children,
				//	has the keyboard focus.

				if (this.KeyboardFocus)
				{
					return true;
				}

				if (!this.containsFocusIsValid)
				{
					this.containsFocus = Helpers.VisualTree.ContainsKeyboardFocus (this);
					this.containsFocusIsValid = true;
				}
				
				return this.containsFocus;
			}
		}
		
		public bool								InheritsParentFocus
		{
			get
			{
				//	When true, IsFocused is inherited from the parent.

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
						this.ClearValue (Visual.IsVisibleProperty);
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
						this.ClearValue (Visual.IsEnabledProperty);
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
		
		public virtual bool						IsFocused
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
				
				if ((validator != null) &&
					(validator.State != ValidationState.Unknown))
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

		public bool								UndefinedLanguage
		{
			get
			{
				return (bool) this.GetValue (Visual.UndefinedLanguageProperty);
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

		/// <summary>
		/// Gets or sets the validation groups associated with this visual.
		/// See <see cref="T:Command.JoinGroupNames"/>. If more than one group
		/// must be used, the group names must be joined using <c>"|"</c>.
		/// </summary>
		/// <value>The validation groups.</value>
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

		/// <summary>
		/// Gets a value indicating whether this visual has validation groups.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this visual has validation groups; otherwise, <c>false</c>.
		/// </value>
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


		public bool								DrawDesignerFrame
		{
			//	Détermine s'il faut dessiner un cadre traitillé (utilisé par Designer).
			get
			{
				return (bool) this.GetValue (Visual.DrawDesignerFrameProperty);
			}
			set
			{
				this.SetValue (Visual.DrawDesignerFrameProperty, value);
			}
		}

		public FrameState						DrawFrameState
		{
			//	Détermine s'il faut dessiner un cadre dans les quatre bords du widget.
			get
			{
				return (FrameState) this.GetValue (Visual.DrawFrameStateProperty);
			}
			set
			{
				this.SetValue (Visual.DrawFrameStateProperty, value);
			}
		}

		public double							DrawFrameWidth
		{
			//	Epaisseur du cadre dans les quatre bords du widget.
			get
			{
				return (double) this.GetValue (Visual.DrawFrameWidthProperty);
			}
			set
			{
				this.SetValue (Visual.DrawFrameWidthProperty, value);
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

		public IEnumerable<Visual>				Parents
		{
			get
			{
				Visual parent = this.parent;

				while (parent != null)
				{
					yield return parent;
					parent = parent.parent;
				}
			}
		}


		public long GetVisualSerialId()
		{
			return this.visualSerialId;
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

		public Drawing.Point GetBaseLine()
		{
			double ascender;
			double descender;
			
			return this.GetBaseLine (this.width, this.height, out ascender, out descender);
		}

		public virtual Drawing.Point GetBaseLine(double width, double height, out double ascender, out double descender)
		{
			ascender = height;
			descender = 0;

			return Drawing.Point.Zero;
		}

		#region CommandCache Support Methods

		internal int GetCommandCacheId()
		{
			return (int) this.GetValue (Visual.CommandCacheIdProperty);
		}
		
		internal void SetCommandCacheId(int value)
		{
			if (value == -1)
			{
				this.ClearValue (Visual.CommandCacheIdProperty);
			}
			else
			{
				this.SetValue (Visual.CommandCacheIdProperty, value);
			}
		}

		#endregion

		public virtual void SetManualBounds(Drawing.Rectangle value)
		{
			System.Diagnostics.Debug.Assert (this.Anchor == AnchorStyles.None);
			System.Diagnostics.Debug.Assert (this.Dock == DockStyle.None);
			System.Diagnostics.Debug.Assert (Layouts.LayoutEngine.GetLayoutMode (this) == Layouts.LayoutMode.None);

			Layouts.LayoutContext.SyncMeasure (this);
			
			this.SetBounds (value);
			this.Arrange (null);
		}

		public void GetMeasures(out Layouts.LayoutMeasure width, out Layouts.LayoutMeasure height)
		{
			Layouts.LayoutContext.SyncMeasure (this);
			
			width  = Layouts.LayoutMeasure.GetWidth (this);
			height = Layouts.LayoutMeasure.GetHeight (this);
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
					this.OnSizeChanged (oldValue.Size, newValue.Size);
				}

				this.OnBoundsChanged (oldValue, newValue);
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
			this.OnMeasuring ();
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

			Layouts.ILayoutEngine engine = Layouts.LayoutEngine.GetLayoutEngine (this);
			
			if ((this.HasChildren) ||
				(engine != null))
			{
				this.LayoutArrange (engine);
				this.ManualArrange ();
			}

			if (this.IsActualGeometryValid)
			{
				if (this.dirtyDisplay)
				{
					this.Invalidate ();
				}

				if (context != null)
				{
					context.RemoveVisualFromArrangeQueue (this);
				}
				
				System.Diagnostics.Debug.Assert (this.IsActualGeometryValid);
			}
		}

		protected virtual void LayoutArrange(Layouts.ILayoutEngine engine)
		{
			Drawing.Rectangle rect = this.Client.Bounds;

			rect.Deflate (this.Padding);
			rect.Deflate (this.GetInternalPadding ());

			if (this.HasChildren)
			{
				IEnumerable<Visual> children = this.Children;

				if (this.children.AnchorLayoutCount > 0)
				{
					Layouts.LayoutEngine.AnchorEngine.UpdateLayout (this, rect, children);
				}
				if (this.children.DockLayoutCount > 0)
				{
					Layouts.LayoutEngine.DockEngine.UpdateLayout (this, rect, children);
				}
				if (this.children.StackLayoutCount > 0)
				{
					Layouts.LayoutEngine.StackEngine.UpdateLayout (this, rect, children);
				}

				if (engine != null)
				{
					engine.UpdateLayout (this, rect, children);
				}
			}
			else if (engine != null)
			{
				engine.UpdateLayout (this, rect, new Visual[0]);
			}
		}

		protected virtual void ManualArrange()
		{
			//	Override if you need to layout children manually during the arrange
			//	phase.
		}

		protected virtual void MeasureMinMax(ref Drawing.Size min, ref Drawing.Size max)
		{
			Layouts.LayoutContext context = Helpers.VisualTree.GetLayoutContext (this);
			
			min.Width = System.Math.Max (min.Width, this.MinWidth);
			min.Height = System.Math.Max (min.Height, this.MinHeight);
			max.Width = System.Math.Min (max.Width, this.MaxWidth);
			max.Height = System.Math.Min (max.Height, this.MaxHeight);

			Layouts.ILayoutEngine engine = Layouts.LayoutEngine.GetLayoutEngine (this);
			
			if (this.HasChildren)
			{
				Drawing.Margins padding = this.Padding + this.GetInternalPadding ();
				IEnumerable<Visual> children = this.children;

				double paddingWidth  = padding.Width;
				double paddingHeight = padding.Height;

				min.Width  = System.Math.Max (0, min.Width  - paddingWidth);
				min.Height = System.Math.Max (0, min.Height - paddingHeight);
				max.Width  = System.Math.Max (0, max.Width  - paddingWidth);
				max.Height = System.Math.Max (0, max.Height - paddingHeight);
				
				if (this.children.DockLayoutCount > 0)
				{
					Layouts.LayoutEngine.DockEngine.UpdateMinMax (this, context, children, ref min, ref max);
				}
				if (this.children.AnchorLayoutCount > 0)
				{
					Layouts.LayoutEngine.AnchorEngine.UpdateMinMax (this, context, children, ref min, ref max);
				}
				if (this.children.StackLayoutCount > 0)
				{
					Layouts.LayoutEngine.StackEngine.UpdateMinMax (this, context, children, ref min, ref max);
				}

				if (engine != null)
				{
					engine.UpdateMinMax (this, context, children, ref min, ref max);
				}

				min.Width  += paddingWidth;
				min.Height += paddingHeight;
				max.Width  += paddingWidth;
				max.Height += paddingHeight;
			}
			else if (engine != null)
			{
				Drawing.Margins padding = this.Padding + this.GetInternalPadding ();
				IEnumerable<Visual> children = new Visual[0];

				double paddingWidth  = padding.Width;
				double paddingHeight = padding.Height;

				min.Width  = System.Math.Max (0, min.Width  - paddingWidth);
				min.Height = System.Math.Max (0, min.Height - paddingHeight);
				max.Width  = System.Math.Max (0, max.Width  - paddingWidth);
				max.Height = System.Math.Max (0, max.Height - paddingHeight);
				
				engine.UpdateMinMax (this, context, children, ref min, ref max);

				min.Width  += paddingWidth;
				min.Height += paddingHeight;
				max.Width  += paddingWidth;
				max.Height += paddingHeight;
			}

			if (context != null)
			{
				context.DefineMinWidth (this, min.Width);
				context.DefineMinHeight (this, min.Height);
				context.DefineMaxWidth (this, max.Width);
				context.DefineMaxHeight (this, max.Height);
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


		public virtual Caption GetDisplayCaption()
		{
			Command commandObject  = this.CommandObject;
			Caption commandCaption = commandObject == null ? null : commandObject.Caption;

			if ((commandCaption != null) &&
				(commandCaption.Id.IsValid))
			{
				ICaptionResolver manager = Visual.cachedCaptionResolver ?? Helpers.VisualTree.GetCaptionResolver (this);
				commandCaption = manager.GetCaption (commandCaption.Id);
			}

			return Caption.Merge (commandCaption, this.Caption);
		}

		public void UpdateDisplayCaptions()
		{
			ICaptionResolver oldManager = Visual.cachedCaptionResolver;
			ICaptionResolver newManager = Helpers.VisualTree.GetCaptionResolver (this);

			Visual.cachedCaptionResolver = newManager;

			this.RecursiveUpdateDisplayCaptions ();

			Visual.cachedCaptionResolver = oldManager;
		}

		private void RecursiveUpdateDisplayCaptions()
		{
			this.OnDisplayCaptionChanged ();

			if (this.HasChildren)
			{
				foreach (Visual child in this.Children)
				{
					child.RecursiveUpdateDisplayCaptions ();
				}
			}
		}

		[System.ThreadStatic]
		private static ICaptionResolver cachedCaptionResolver;
		
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
				//	Detaching from the parent visual means that this visual and all its
				//	children no longer have to be recorded in the measure/arrange queues.
				
				Layouts.LayoutContext.RemoveFromQueues (this);

				this.UpdateVisualTreeChangeCounter ();
				this.ClearCachedFocus ();
				this.Invalidate ();
				this.parent = null;

				//	Make sure that we will measure the children if some of them have
				//	never been measured (they lack the Layouts.LayoutMeasure records).

				System.Diagnostics.Debug.Assert (Helpers.VisualTree.FindLayoutContext (this) == null);
				
				Layouts.LayoutContext.AddUnmeasuredChildrenToMeasureQueue (this);
			}
			else
			{
				Layouts.LayoutContext context = null;

				if (this.parent == null)
				{
					context = Layouts.LayoutContext.GetLayoutContext (this);
				}
				else
				{
					this.ClearCachedFocus ();
					this.UpdateVisualTreeChangeCounter ();
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

				this.ClearCachedFocus ();
				this.UpdateVisualTreeChangeCounter ();
			}
		}

		private void UpdateVisualTreeChangeCounter()
		{
			WindowRoot root = Helpers.VisualTree.GetRoot (this) as WindowRoot;

			if (root != null)
			{
				root.IncrementTreeChangeCounter ();
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

		#region Internal Methods for Command

		internal void NotifyCommandCaptionChanged()
		{
			this.InvalidateDisplayCaption ();
		}

		#endregion

		#region Private Methods for Caption management

		private void InvalidateDisplayCaption()
		{
			this.OnDisplayCaptionChanged ();
		}

		private void DetachCaption()
		{
			if (this.caption != null)
			{
				this.caption.Changed -= this.HandleCaptionChanged;
				this.caption = null;
			}
		}

		private bool AttachCaption(Druid caption)
		{
			if (caption.IsValid)
			{
				ICaptionResolver manager = Helpers.VisualTree.GetCaptionResolver (this);
				return this.AttachCaption (manager.GetCaption (caption));
			}
			else
			{
				return false;
			}
		}

		private bool AttachCaption(Caption caption)
		{
			System.Diagnostics.Debug.Assert (this.caption == null);

			if (caption != null)
			{
				this.caption = caption;
				this.caption.Changed += this.HandleCaptionChanged;

				return true;
			}
			else
			{
				return false;
			}
		}

		private void HandleCaptionChanged(object sender)
		{
			this.InvalidateDisplayCaption ();
		}

		#endregion

		protected virtual void OnDisplayCaptionChanged()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler ("DisplayCaptionChanged");

			if (handler != null)
			{
				handler (this);
			}
		}

		protected virtual void OnMeasuring()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler ("Measuring");

			if (handler != null)
			{
				handler (this);
			}
		}

		protected virtual void OnSizeChanged(Drawing.Size oldValue, Drawing.Size newValue)
		{
			PropertyChangedEventHandler handler = this.GetUserEventHandler<DependencyPropertyChangedEventArgs> ("SizeChanged");

			if (handler != null)
			{
				handler (this, new DependencyPropertyChangedEventArgs ("Size", oldValue, newValue));
			}
		}

		protected virtual void OnBoundsChanged(Drawing.Rectangle oldValue, Drawing.Rectangle newValue)
		{
			PropertyChangedEventHandler handler = this.GetUserEventHandler<DependencyPropertyChangedEventArgs> ("BoundsChanged");

			if (handler != null)
			{
				handler (this, new DependencyPropertyChangedEventArgs ("Bounds", oldValue, newValue));
			}
		}

		protected virtual void OnChildrenChanged()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler ("ChildrenChanged");

			if (handler != null)
			{
				handler (this);
			}
		}

		protected virtual void OnContainsFocusChanged()
		{
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

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.DetachCaption ();
			}
			
			base.Dispose (disposing);
		}

		#region Object Overrides

		public override bool Equals(object obj)
		{
			return this.Equals (obj as Visual);
		}

		public override int GetHashCode()
		{
			return (int) this.visualSerialId;
		}

		#endregion

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
				that.RefreshFocus (focus);
			}
		}

		protected void RefreshFocus(bool focus)
		{
			//	Update IsFocused according to how the focus inheritance has been
			//	defined :

			if (focus)
			{
				//	When we have the focus, IsFocused will be true.

				this.SetValueBase (Visual.IsFocusedProperty, true);

				foreach (Visual parent in this.Parents)
				{
					if (parent.containsFocus)
					{
						break;
					}

					parent.containsFocus = true;
					parent.containsFocusIsValid = true;
					parent.OnContainsFocusChanged ();
				}
			}
			else
			{
				//	When we don't have the focus, we will either clear the IsFocused
				//	property (and thus inherit its value from the parent) or set it
				//	to false (no inheritance).

				if (this.InheritsParentFocus)
				{
					this.ClearValue (Visual.IsFocusedProperty);
				}
				else
				{
					this.SetValueBase (Visual.IsFocusedProperty, false);
				}

				this.ClearCachedFocus ();
			}
		}

		private void ClearCachedFocus()
		{
			List<Visual> notify = new List<Visual> ();

			foreach (Visual parent in this.Parents)
			{
				if (parent.containsFocusIsValid == false)
				{
					break;
				}

				parent.containsFocusIsValid = false;
				parent.containsFocus = false;
				
				notify.Add (parent);
			}

			foreach (Visual parent in notify)
			{
				parent.OnContainsFocusChanged ();
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
						that.ClearValue (Visual.IsFocusedProperty);
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

		private static void NotifyValidationGroupsChanged(DependencyObject o, object oldValue, object newValue)
		{
			Visual that = o as Visual;
			that.OnValidationGroupsChanged (new DependencyPropertyChangedEventArgs (Visual.ValidationGroupsProperty, oldValue, newValue));
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

		private static void NotifyCommandObjectChanged(DependencyObject o, object oldValue, object newValue)
		{
			Visual that = o as Visual;
			that.OnCommandObjectChanged (new DependencyPropertyChangedEventArgs (Visual.CommandObjectProperty, oldValue, newValue));
		}

		private static void NotifyCaptionIdChanged(DependencyObject o, object oldValue, object newValue)
		{
			Visual that = o as Visual;
			
			Druid oldId = (Druid) oldValue;
			Druid newId = (Druid) newValue;

			that.DetachCaption ();
			that.AttachCaption (newId);
			that.InvalidateDisplayCaption ();
		}

		private static void NotifyAnchorChanged(DependencyObject o, object oldValue, object newValue)
		{
			Visual that = o as Visual;
			
			if (that.parent != null)
			{
				DockStyle dock = that.Dock;
				AnchorStyles anchorOld = (AnchorStyles) oldValue;
				AnchorStyles anchorNew = (AnchorStyles) newValue;
				
				that.parent.children.UpdateLayoutStatistics (that, dock, dock, anchorOld, anchorNew);
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

				that.parent.children.UpdateLayoutStatistics (that, dockOld, dockNew, anchor, anchor);
			}
		}

		private static void NotifyActiveStateChanged(DependencyObject o, object oldValue, object newValue)
		{
			Visual that = o as Visual;
			that.OnActiveStateChanged ();
		}

		protected virtual void OnValidationGroupsChanged(Types.DependencyPropertyChangedEventArgs e)
		{
			//	TODO: should invalidate the validation contexts
			//	TODO: should also listen to the IsEnabledChanged and update validation context
		}
		
		protected virtual void OnParentChanged(Types.DependencyPropertyChangedEventArgs e)
		{
			Helpers.VisualTree.InvalidateCommandDispatcher (this);
			//	TODO: should invalidate the validation contexts
		}
		
		protected virtual void OnKeyboardFocusChanged(Types.DependencyPropertyChangedEventArgs e)
		{
		}
		
		protected virtual void OnCommandObjectChanged(Types.DependencyPropertyChangedEventArgs e)
		{
			Command oldCommand = e.OldValue as Command;
			Command newCommand = e.NewValue as Command;

			if (oldCommand == null)
			{
				CommandCache.Instance.AttachVisual (this);
			}
			else if (newCommand == null)
			{
				CommandCache.Instance.DetachVisual (this);
			}
			else
			{
				CommandCache.Instance.InvalidateVisual (this);
			}

			//	Changing the command might/will also change the caption. Pretend
			//	that the caption just changed :

			this.InvalidateDisplayCaption ();
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

		public event EventHandler					ChildrenChanged
		{
			add
			{
				this.AddUserEventHandler ("ChildrenChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("ChildrenChanged", value);
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

        public event PropertyChangedEventHandler    KeyboardFocusChanged
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

        public event PropertyChangedEventHandler    PaddingChanged
        {
            add
            {
                this.AddEventHandler (Visual.PaddingProperty, value);
            }
            remove
            {
                this.RemoveEventHandler (Visual.PaddingProperty, value);
            }
        }

        public event PropertyChangedEventHandler    MarginsChanged
        {
            add
            {
                this.AddEventHandler (Visual.MarginsProperty, value);
            }
            remove
            {
                this.RemoveEventHandler (Visual.MarginsProperty, value);
            }
        }

		public event EventHandler					DisplayCaptionChanged
		{
			add
			{
				this.AddUserEventHandler ("DisplayCaptionChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("DisplayCaptionChanged", value);
			}
		}

		public event PropertyChangedEventHandler	BoundsChanged
		{
			add
			{
				this.AddUserEventHandler ("BoundsChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("BoundsChanged", value);
			}
		}

		public event PropertyChangedEventHandler	SizeChanged
		{
			add
			{
				this.AddUserEventHandler ("SizeChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("SizeChanged", value);
			}
		}

		public event EventHandler					Measuring
		{
			add
			{
				this.AddUserEventHandler ("Measuring", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("Measuring", value);
			}
		}

		public static readonly DependencyProperty IndexProperty					= DependencyProperty.Register ("Index", typeof (int), typeof (Visual), new DependencyPropertyMetadata (-1));
		public static readonly DependencyProperty GroupProperty					= DependencyProperty.Register ("Group", typeof (string), typeof (Visual));
		public static readonly DependencyProperty NameProperty					= DependencyObjectTree.NameProperty.AddOwner (typeof (Visual));
		public static readonly DependencyProperty ParentProperty				= DependencyObjectTree.ParentProperty.AddOwner (typeof (Visual), new VisualPropertyMetadata (new GetValueOverrideCallback (Visual.GetParentValue), new PropertyInvalidatedCallback (Visual.NotifyParentChanged), VisualPropertyMetadataOptions.None));
		public static readonly DependencyProperty ChildrenProperty				= DependencyObjectTree.ChildrenProperty.AddOwner (typeof (Visual), new VisualPropertyMetadata (new GetValueOverrideCallback (Visual.GetChildrenValue)).MakeReadOnlySerializable ());
		public static readonly DependencyProperty HasChildrenProperty			= DependencyObjectTree.HasChildrenProperty.AddOwner (typeof (Visual), new VisualPropertyMetadata (new GetValueOverrideCallback (Visual.GetHasChildrenValue)));

		public static readonly DependencyProperty AnchorProperty				= DependencyProperty.Register ("Anchor", typeof (AnchorStyles), typeof (Visual), new VisualPropertyMetadata (AnchorStyles.None, Visual.NotifyAnchorChanged, VisualPropertyMetadataOptions.AffectsArrange));
		public static readonly DependencyProperty DockProperty					= DependencyProperty.Register ("Dock", typeof (DockStyle), typeof (Visual), new VisualPropertyMetadata (DockStyle.None, Visual.NotifyDockChanged, VisualPropertyMetadataOptions.AffectsArrange));
		public static readonly DependencyProperty MarginsProperty				= DependencyProperty.Register ("Margins", typeof (Drawing.Margins), typeof (Visual), new VisualPropertyMetadata (Drawing.Margins.Zero, VisualPropertyMetadataOptions.AffectsArrange));
		public static readonly DependencyProperty PaddingProperty				= DependencyProperty.Register ("Padding", typeof (Drawing.Margins), typeof (Visual), new VisualPropertyMetadata (Drawing.Margins.Zero, VisualPropertyMetadataOptions.AffectsChildrenLayout | VisualPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty HorizontalAlignmentProperty	= DependencyProperty.Register ("HorizontalAlignment", typeof (HorizontalAlignment), typeof (Visual), new VisualPropertyMetadata (HorizontalAlignment.Stretch, VisualPropertyMetadataOptions.AffectsArrange));
		public static readonly DependencyProperty VerticalAlignmentProperty		= DependencyProperty.Register ("VerticalAlignment", typeof (VerticalAlignment), typeof (Visual), new VisualPropertyMetadata (VerticalAlignment.Stretch, VisualPropertyMetadataOptions.AffectsArrange | VisualPropertyMetadataOptions.AffectsMeasure));
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
		public static readonly DependencyProperty UndefinedLanguageProperty		= DependencyProperty.Register ("UndefinedLanguage", typeof (bool), typeof (Visual), new VisualPropertyMetadata (false, VisualPropertyMetadataOptions.AffectsDisplay).MakeNotSerializable ());

		public static readonly DependencyProperty InheritsParentFocusProperty	= DependencyProperty.Register ("InheritsParentFocus", typeof (bool), typeof (Visual), new VisualPropertyMetadata (false, Visual.SetInheritsParentFocus, VisualPropertyMetadataOptions.None));

		public static readonly DependencyProperty DrawDesignerFrameProperty		= DependencyProperty.Register ("DrawDesignerFrame", typeof(bool), typeof(Visual), new VisualPropertyMetadata(false, VisualPropertyMetadataOptions.InheritsValue | VisualPropertyMetadataOptions.AffectsDisplay).MakeNotSerializable());
		public static readonly DependencyProperty DrawFrameStateProperty		= DependencyProperty.Register ("DrawFrameState", typeof (FrameState), typeof (Visual), new VisualPropertyMetadata (FrameState.None, VisualPropertyMetadataOptions.AffectsDisplay));
		public static readonly DependencyProperty DrawFrameWidthProperty		= DependencyProperty.Register ("DrawFrameWidth", typeof (double), typeof (Visual), new VisualPropertyMetadata (1.0, VisualPropertyMetadataOptions.AffectsDisplay));
		
		public static readonly DependencyProperty IsVisibleProperty				= DependencyProperty.RegisterReadOnly ("IsVisible", typeof (bool), typeof (Visual), new VisualPropertyMetadata (false, VisualPropertyMetadataOptions.InheritsValue | VisualPropertyMetadataOptions.AffectsArrange | VisualPropertyMetadataOptions.AffectsDisplay));
		public static readonly DependencyProperty IsEnabledProperty				= DependencyProperty.RegisterReadOnly ("IsEnabled", typeof (bool), typeof (Visual), new VisualPropertyMetadata (true, VisualPropertyMetadataOptions.InheritsValue | VisualPropertyMetadataOptions.AffectsDisplay));
		public static readonly DependencyProperty IsFocusedProperty				= DependencyProperty.RegisterReadOnly ("IsFocused", typeof (bool), typeof (Visual), new VisualPropertyMetadata (false, VisualPropertyMetadataOptions.InheritsValue | VisualPropertyMetadataOptions.AffectsDisplay));
		
		public static readonly DependencyProperty KeyboardFocusProperty			= DependencyProperty.RegisterReadOnly ("KeyboardFocus", typeof (bool), typeof (Visual), new VisualPropertyMetadata (false, new SetValueOverrideCallback (Visual.SetKeyboardFocusValue), VisualPropertyMetadataOptions.AffectsDisplay));
		public static readonly DependencyProperty ContainsKeyboardFocusProperty	= DependencyProperty.RegisterReadOnly ("ContainsKeyboardFocus", typeof (bool), typeof (Visual), new VisualPropertyMetadata (false, new GetValueOverrideCallback (Visual.GetContainsKeyboardFocusValue), VisualPropertyMetadataOptions.ChangesSilently));

		public static readonly DependencyProperty IsValidProperty				= DependencyProperty.RegisterReadOnly ("IsValid", typeof (bool), typeof (Visual), new DependencyPropertyMetadata (Visual.GetIsValidValue));
		public static readonly DependencyProperty ValidatorProperty				= DependencyProperty.RegisterReadOnly ("Validator", typeof (IValidator), typeof (Visual), new DependencyPropertyMetadata ());
		public static readonly DependencyProperty ValidationGroupsProperty		= DependencyProperty.Register ("ValidationGroups", typeof (string), typeof (Visual), new DependencyPropertyMetadata (Visual.NotifyValidationGroupsChanged));
		public static readonly DependencyProperty CaptionResolverProperty		= DependencyProperty.Register ("CaptionResolver", typeof (ICaptionResolver), typeof (Visual), new DependencyPropertyMetadata ());

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

		public static readonly DependencyProperty CommandObjectProperty			= DependencyProperty.Register ("CommandObject", typeof (Command), typeof (Visual), new VisualPropertyMetadata (null, new PropertyInvalidatedCallback (Visual.NotifyCommandObjectChanged), VisualPropertyMetadataOptions.AffectsDisplay));
		public static readonly DependencyProperty CaptionIdProperty				= DependencyProperty.Register ("CaptionId", typeof (Druid), typeof (Visual), new VisualPropertyMetadata (Druid.Empty, new PropertyInvalidatedCallback (Visual.NotifyCaptionIdChanged), VisualPropertyMetadataOptions.AffectsDisplay));
		private static readonly DependencyProperty CommandCacheIdProperty		= DependencyProperty.Register ("CommandCacheId", typeof (int), typeof (Visual), new VisualPropertyMetadata (-1));

		private static long						nextSerialId = 1;
		
		private readonly long					visualSerialId;
		private bool							dirtyLayout;
		private bool							dirtyDisplay;
		private bool							containsFocus;
		private bool							containsFocusIsValid;

		private double							x, y;
		private double							width, height;
		
		private FlatChildrenCollection			children;
		private Visual							parent;
		private Caption							caption;
	}
}
